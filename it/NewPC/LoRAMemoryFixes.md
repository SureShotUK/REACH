# Qwen-Image-Edit LoRA Training — Memory Fixes and Speed Tuning

**Hardware**: Dual RTX 3090 (48GB VRAM) · 64GB DDR5 · Ubuntu 24.04 LTS
**Confirmed working**: March 2026

---

## Root Cause Summary

Training consistently failed with SIGKILL (Linux OOM killer) despite 59 GB of free RAM. The cause was the checkpoint save at the end of each epoch:

- DeepSpeed ZeRO-3 stores the full 41 GB BF16 transformer in CPU RAM during training
- At epoch end, `stage3_gather_16bit_weights_on_model_save: true` causes DeepSpeed to copy all 41 GB into a **second buffer** to extract the weights for saving
- Peak RAM during save: ~41 GB (model) + ~41 GB (save buffer) + ~5 GB (overhead) ≈ **87 GB**
- System only has 62 GB RAM → OOM kill

The training steps themselves ran fine. The failure always happened immediately after the final training step, during the checkpoint save.

---

## Required Fixes

All changes below are required for training to complete. Do not revert them.

### 1. Use the correct virtual environment

Training must run in `diffsynth-env`, not `hf-env`. `hf-env` is for downloading models only and does not have DeepSpeed or accelerate installed.

```bash
source /home/steve/diffsynth-env/bin/activate
```

Verify the prompt shows `(diffsynth-env)` before running any training script.

### 2. Increase swap to 32 GB

The swap file must be at least 32 GB to absorb the ~25 GB memory spike during checkpoint save. This is a **one-time server configuration change** — it persists across reboots.

```bash
sudo swapoff /swap.img
sudo fallocate -l 32G /swap.img
sudo mkswap /swap.img
sudo swapon /swap.img
free -h   # Verify: Swap shows ~31G total
```

Verify the fstab entry for `/swap.img` still exists (it should, since we only resized not recreated):

```bash
grep swap /etc/fstab
```

If missing, add it:

```bash
echo '/swap.img none swap sw 0 0' | sudo tee -a /etc/fstab
```

### 3. Disable pinned memory in DeepSpeed config

File: `examples/qwen_image/model_training/special/low_vram_training/ds_z3_cpuoffload.json`

Change both `pin_memory` values from `true` to `false`:

```json
"offload_optimizer": {
    "device": "cpu",
    "pin_memory": false
},
"offload_param": {
    "device": "cpu",
    "pin_memory": false
}
```

**Why**: Pinned memory cannot be swapped to disk. With `pin_memory: true`, the OS cannot use the 32 GB swap to handle the save spike — the memory is locked in RAM. Setting `false` allows the save buffer to overflow to swap temporarily.

**Note**: `stage3_gather_16bit_weights_on_model_save` must remain `true`. Setting it to `false` breaks DiffSynth-Studio's checkpoint saving entirely (`accelerator.get_state_dict()` raises a ValueError).

### 4. Set CUDA architecture in training scripts

Add to the top of `stage2_train.sh` (and any other Stage 2 scripts) alongside the other exports:

```bash
export TORCH_CUDA_ARCH_LIST="8.6"
```

**Why**: Without this, DeepSpeed compiles its CUDA extensions for every GPU architecture it detects. This compilation consumes significant memory on top of the already-loaded 41 GB model, pushing total RAM usage higher. `8.6` is the Ampere architecture of the RTX 3090 — compiling only for this GPU eliminates unnecessary overhead.

### 5. Set dataset workers to 0

In `stage2_train.sh` (and any Stage 2 test scripts):

```bash
--dataset_num_workers 0
```

**Why**: Each worker process holds references into the parent process's memory-mapped model data. With the 41 GB model already in RAM, additional workers push RSS higher. `0` disables parallel workers entirely — data loading becomes sequential.

---

## Confirmed Working Configuration

The following `stage2_train.sh` is the confirmed working configuration:

```bash
export HF_HOME=/mnt/models/huggingface
export PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True
export TORCH_CUDA_ARCH_LIST="8.6"

accelerate launch --config_file examples/qwen_image/model_training/special/low_vram_training/accelerate_zero3_2gpu.yaml examples/qwen_image/model_training/train.py --dataset_base_path "./models/train/my_character_lora_cache" --max_pixels 262144 --dataset_repeat 50 --model_id_with_origin_paths "Qwen/Qwen-Image-Edit-2511:transformer/diffusion_pytorch_model*.safetensors" --learning_rate 1e-4 --num_epochs 5 --remove_prefix_in_ckpt "pipe.dit." --output_path "./models/train/my_character_lora" --lora_base_model "dit" --lora_target_modules "to_q,to_k,to_v,add_q_proj,add_k_proj,add_v_proj,to_out.0,to_add_out,img_mlp.net.2,img_mod.1,txt_mlp.net.2,txt_mod.1" --lora_rank 16 --use_gradient_checkpointing --dataset_num_workers 0 --find_unused_parameters --zero_cond_t --initialize_model_on_cpu --task "sft:train"
```

---

## Speed Optimisations (Once Training Is Confirmed Working)

The fixes above make training slower than necessary. Once you have successfully produced a complete set of LoRA files (`epoch-0.safetensors` through `epoch-4.safetensors`) and confirmed they load in ComfyUI, try these changes incrementally to recover speed.

**Estimated time with current working config**: 6–8 hours (5 epochs, 44 images)
**Estimated time with original config (before fixes)**: 4–5 hours

### Speed test approach

Make one change at a time. Run a quick test with `--dataset_repeat 1 --num_epochs 1` after each change to verify it still completes without OOM. If the test passes, proceed to the next change.

---

### Optimisation 1: Restore pinned memory (saves ~1–2 hours)

**Risk**: Medium — requires the 32 GB swap to remain in place permanently.

In `ds_z3_cpuoffload.json`, set both back to `true`:

```json
"offload_optimizer": {
    "device": "cpu",
    "pin_memory": true
},
"offload_param": {
    "device": "cpu",
    "pin_memory": true
}
```

Pinned memory (page-locked) transfers between CPU and GPU are significantly faster than unpinned transfers. This is the biggest single speed improvement available. It is safe as long as the 32 GB swap is in place to handle the checkpoint save spike.

**Test**: Run `test_stage2.sh` and confirm `epoch-0.safetensors` is produced.

---

### Optimisation 2: Restore dataset workers (saves ~30–60 minutes)

**Risk**: Low — adds ~2–4 GB RAM per worker, well within headroom.

In `stage2_train.sh`, change:

```bash
--dataset_num_workers 0
```
to:
```bash
--dataset_num_workers 2
```

Dataset workers preload the next batch while the GPU is computing the current one, reducing GPU idle time between steps.

**Test**: Run `test_stage2.sh` and confirm `epoch-0.safetensors` is produced.

---

### Optimisation 3: Pre-build DeepSpeed CUDA extensions (saves ~5–10 minutes per run)

**Risk**: None — one-time setup, does not affect training behaviour.

DeepSpeed compiles CUDA extensions on first use each session. Pre-building them once prevents recompilation on every training run:

```bash
source /home/steve/diffsynth-env/bin/activate
TORCH_CUDA_ARCH_LIST="8.6" DS_BUILD_CPU_ADAM=1 pip install deepspeed --no-build-isolation
```

After this, the `TORCH_CUDA_ARCH_LIST is not set` warning disappears and startup is faster.

---

### Optimisation 4: Increase LoRA rank (quality improvement, not speed)

Not a speed improvement — higher rank is slower. However, once the pipeline is stable, consider testing `--lora_rank 32` for better character fidelity at the cost of longer training time (~20–30% slower).

---

## Diagnostics Reference

If training fails again, these commands identify the cause:

```bash
# Check if OOM killer was involved (SIGKILL = -9)
sudo dmesg | grep -i "out of memory\|killed process" | tail -10

# Check actual RAM usage at time of kill (look for anon-rss value)
sudo dmesg | grep "Killed process" | tail -5

# Check swap status
free -h
swapon --show

# Confirm correct environment
which python3   # Should show: /home/steve/diffsynth-env/bin/python3

# Check CUDA architecture setting
echo $TORCH_CUDA_ARCH_LIST   # Should show: 8.6
```

### Interpreting OOM kills

| anon-rss value in dmesg | Likely cause |
|---|---|
| ~60 GB | Save spike — 32 GB swap not in place or `pin_memory: true` without swap |
| ~45 GB | Training OOM — something consuming extra RAM (Docker containers, Ollama running) |
| ~20 GB | Model load OOM — ZeRO-3 config not loading correctly, model loading to VRAM instead |
