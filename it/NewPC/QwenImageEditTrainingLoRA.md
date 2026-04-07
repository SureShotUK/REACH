# Qwen-Image-Edit-2511 Character LoRA Training Guide

**Hardware**: Dual RTX 3090 (48GB VRAM total, NVLink) · AMD Ryzen 9 7900X · 64GB DDR5 · Ubuntu 24.04 LTS
**Last Verified**: March 2026 — procedure confirmed working end-to-end

---

## Overview

This guide trains a **character identity LoRA** for the Qwen-Image-Edit-2511 model. The result is a `.safetensors` file that, when loaded alongside the model in ComfyUI, causes Qwen-Image-Edit to consistently generate a specific person's likeness when given an editing instruction.

Unlike FLUX LoRAs (which use ai-toolkit and run on a single GPU), Qwen-Image-Edit requires:
- **DiffSynth-Studio** — the official Alibaba training framework
- **DeepSpeed ZeRO-3 with CPU offload** — the 41 GB BF16 transformer cannot fit in VRAM any other way on 2×24 GB hardware
- **Two-stage split training** — Stage 1 caches text/image embeddings to disk; Stage 2 trains only the transformer

Training time: approximately **4–5 hours** for 5 epochs on 44 images at 512×512 with default settings. With the 64 GB RAM workarounds below, expect **6–8 hours**.

---

## RAM Limitation Workarounds (64 GB System)

> **This section applies only because this server has 64 GB RAM. If the system had 128 GB or more, none of these modifications would be necessary** — the default DiffSynth-Studio configuration would work without changes.

### Why these workarounds are needed

DeepSpeed ZeRO-3 CPU offload stores the full 41 GB BF16 transformer in system RAM during training. At the end of each epoch, the checkpoint save process temporarily creates a second copy of the model weights in RAM (to gather them for writing to disk). This spikes peak RAM usage from ~46 GB to approximately **87 GB** — exceeding the 64 GB physical limit and triggering the Linux OOM killer, which kills the training process with SIGKILL.

The training steps themselves run fine. The failure always happens at the first checkpoint save.

### Required modifications for 64 GB systems

Three changes are needed. They are applied once and persist until the server configuration changes.

#### 1. Increase swap to 32 GB (one-time server change)

The 32 GB swap gives the system 94 GB of total virtual memory, providing enough headroom to absorb the ~87 GB save spike. This is a permanent server configuration change.

```bash
sudo swapoff /swap.img
sudo fallocate -l 32G /swap.img
sudo mkswap /swap.img
sudo swapon /swap.img
free -h   # Verify: Swap line shows ~31G total
```

#### 2. Disable pinned memory in DeepSpeed config

Pinned memory (page-locked RAM) cannot be swapped to disk. With `pin_memory: true`, the OS cannot use the swap space to handle the save spike even if swap is available. Setting it to `false` allows the temporary save buffer to overflow to swap.

File: `examples/qwen_image/model_training/special/low_vram_training/ds_z3_cpuoffload.json`

Change both `pin_memory` values to `false`:

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

> **Note**: `stage3_gather_16bit_weights_on_model_save` must remain `true`. Setting it to `false` breaks DiffSynth-Studio's saving entirely.

#### 3. Add to Stage 2 training scripts

In every Stage 2 script, make two changes:

Add this export alongside the other exports at the top:
```bash
export TORCH_CUDA_ARCH_LIST="8.6"
```

Change `--dataset_num_workers 2` to:
```bash
--dataset_num_workers 0
```

**Why `TORCH_CUDA_ARCH_LIST="8.6"`**: Without this, DeepSpeed compiles CUDA extensions for every GPU architecture it detects, which consumes an additional ~10–14 GB of RAM temporarily on top of the already-loaded 41 GB model. `8.6` is the Ampere architecture of the RTX 3090 — specifying it limits compilation to only what this system needs.

**Why `--dataset_num_workers 0`**: Each worker process holds references into the model memory, increasing the resident set size. Disabling workers reduces peak RAM usage during training steps.

### Impact on training time

These modifications slow training from approximately 4–5 hours to **6–8 hours** per full run. The main cause is `pin_memory: false`, which slows parameter transfers between CPU and GPU. Once training is confirmed working, see `LoRAMemoryFixes.md` for a step-by-step guide to restoring speed.

---

## Variables Unique to Each Training Run

Before starting, decide on these values. Every occurrence of the placeholder names in this guide must be replaced with your chosen values:

| Placeholder | What it is | Example |
|---|---|---|
| `MY_LORA_NAME` | Short name for this LoRA — used in output and cache folder names | `sara_character` |
| `MY_DATASET_PATH` | Full path to the folder containing your training images and metadata.json | `/home/steve/training/sara` |
| `MY_TRIGGER_WORD` | A short unique word used in every caption to activate the LoRA | `saraI` |
| `MY_LORA_RANK` | LoRA capacity — higher = more expressive but slower; 16 is a safe default | `16` |

These four values are the only things you change between training runs. Everything else stays the same.

---

## Prerequisites

These are one-time setup steps. If you have already done them, skip to [Dataset Preparation](#dataset-preparation).

### 1. DiffSynth-Studio and virtual environment

```bash
cd /home/steve
python3 -m venv diffsynth-env
source diffsynth-env/bin/activate

pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu124
git clone https://github.com/modelscope/DiffSynth-Studio.git
cd DiffSynth-Studio
pip install -e .
pip install peft bitsandbytes deepspeed
```

### 2. Accelerate config file (create once, reuse for all runs)

This file tells accelerate to use DeepSpeed ZeRO-3 with a single process and full CPU offload. The `num_processes: 1` is intentional — ZeRO-3 CPU offload on a single GPU uses less VRAM than DDP across two GPUs, because there are no inter-GPU AllGather communication buffers.

```bash
cat > /home/steve/DiffSynth-Studio/examples/qwen_image/model_training/special/low_vram_training/accelerate_zero3_2gpu.yaml << 'EOF'
compute_environment: LOCAL_MACHINE
debug: true
deepspeed_config:
  deepspeed_config_file: examples/qwen_image/model_training/special/low_vram_training/ds_z3_cpuoffload.json
  zero3_init_flag: true
distributed_type: DEEPSPEED
downcast_bf16: 'no'
enable_cpu_affinity: false
machine_rank: 0
main_training_function: main
num_machines: 1
num_processes: 1
rdzv_backend: static
same_network: true
tpu_env: []
tpu_use_cluster: false
tpu_use_sudo: false
use_cpu: false
EOF
```

The `ds_z3_cpuoffload.json` file already exists in DiffSynth-Studio at `examples/qwen_image/model_training/special/low_vram_training/`. Do not modify it.

### 3. Model files

Training requires BF16 weights from two HuggingFace repositories. These are large (~58 GB total) and only need to be downloaded once.

```bash
source /home/steve/hf-env/bin/activate
export HF_HOME=/mnt/models/huggingface

# Transformer — ~41 GB (5 files)
hf download Qwen/Qwen-Image-Edit-2511 \
    --include "transformer/*" \
    --local-dir /mnt/models/huggingface/qwen-image-edit-2511

# Text encoder + VAE — ~17 GB
hf download Qwen/Qwen-Image \
    --include "text_encoder/*" "vae/*" \
    --local-dir /mnt/models/huggingface/qwen-image
```

DiffSynth-Studio downloads these automatically on first run via ModelScope if they are not already present in `~/DiffSynth-Studio/models/`. If you have already run training once and the models downloaded automatically, they are at `~/DiffSynth-Studio/models/Qwen/Qwen-Image-Edit-2511/` and `~/DiffSynth-Studio/models/Qwen/Qwen-Image/`.

---

## Dataset Preparation

### Training image guidelines

The quality and variety of your training images directly determines LoRA quality.

**Ideal images:**

| Requirement | Detail |
|---|---|
| Count | 20–50 images; fewer than 15 risks poor generalisation |
| Resolution | 512×512 minimum; higher is better up to 1328×1328 (the model's native resolution) |
| Angles | Mix of front-facing, three-quarter, and profile shots |
| Lighting | Variety: natural daylight, indoor, overcast, studio — do not use only one lighting type |
| Expression | Mix of neutral, smiling, and other natural expressions |
| Clothing | Varied outfits — the LoRA should learn the face, not a specific outfit |
| Background | Varied — avoids the model associating the trigger word with a specific scene |

**Images to exclude:**

- Group photos (other people in frame)
- Heavy occlusion: sunglasses, hands over face, masks
- Motion blur or out-of-focus shots
- Screenshots from video (low quality, artefact-heavy)
- Near-duplicate shots from a burst sequence — keep only the best one
- Cropped or partial face shots

**Caption strategy:**

Use only your trigger word followed by `person`. Do not use descriptive captions. The model's text encoder (Qwen2.5-VL) is powerful enough that detailed captions cause it to over-fit to specific described attributes rather than learning identity.

```
saraI person
```

Every image gets the same caption — the trigger word plus `person`.

### Folder structure

```
/home/steve/training/MY_DATASET_PATH/
├── photo001.jpg
├── photo002.jpg
├── photo003.png
├── ...
└── metadata.json          ← created by the script below
```

### Create the metadata.json file

Run this from the server. Replace `MY_DATASET_PATH` and `MY_TRIGGER_WORD` with your values:

```bash
python3 - <<'EOF'
import os, json

image_dir = "MY_DATASET_PATH"
trigger_word = "MY_TRIGGER_WORD person"

images = sorted([
    f for f in os.listdir(image_dir)
    if f.lower().endswith(('.jpg', '.jpeg', '.png', '.webp'))
])

metadata = [{"prompt": trigger_word, "image": img} for img in images]

output_path = os.path.join(image_dir, "metadata.json")
with open(output_path, "w") as f:
    json.dump(metadata, f, indent=2)

print(f"Created {output_path} with {len(metadata)} entries")
print("First entry:", metadata[0])
EOF
```

Verify the output before training:

```bash
cat MY_DATASET_PATH/metadata.json | head -20
```

Expected format:
```json
[
  {
    "prompt": "saraI person",
    "image": "photo001.jpg"
  },
  {
    "prompt": "saraI person",
    "image": "photo002.jpg"
  },
  ...
]
```

The `"image"` value is the filename only (not the full path). The dataset base path is passed separately to the training script.

---

## Training

### Before every training run

**Stop Docker containers** (they hold ~256 MiB VRAM each and respawn if killed directly — use docker stop):

```bash
docker stop comfyui comfyui-amelia
```

**Stop Ollama:**

```bash
sudo systemctl stop ollama
```

**Verify both GPUs are free** (should show 0–10 MiB each):

```bash
nvidia-smi
```

If any Python processes appear in the GPU list, kill them:

```bash
sudo kill -9 <PID>
```

### Create your training scripts

Create one pair of scripts per LoRA. All three steps below must be run in the same terminal session in order. Replace `MY_LORA_NAME`, `MY_DATASET_PATH`, and `MY_LORA_RANK` with your values. Copy these commands from a file (not a rendered markdown block) to avoid line-break issues.

**Step 1 — Activate the environment and navigate to DiffSynth-Studio:**

```bash
source /home/steve/diffsynth-env/bin/activate   # skip if already activated in this shell
mkdir -p /home/steve/DiffSynth-Studio/examples/qwen_image/model_training/lora
cd /home/steve/DiffSynth-Studio
```

**Step 2 — Create the Stage 1 script** (run once per dataset — caches embeddings to disk):

```bash
cat > /home/steve/DiffSynth-Studio/examples/qwen_image/model_training/lora/MY_LORA_NAME_stage1.sh << 'EOF'
export HF_HOME=/mnt/models/huggingface
export PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True

accelerate launch --num_processes 2 --mixed_precision bf16 examples/qwen_image/model_training/train.py --dataset_base_path MY_DATASET_PATH --dataset_metadata_path MY_DATASET_PATH/metadata.json --data_file_keys "image" --max_pixels 262144 --dataset_repeat 1 --model_id_with_origin_paths "Qwen/Qwen-Image:text_encoder/model*.safetensors,Qwen/Qwen-Image:vae/diffusion_pytorch_model.safetensors" --output_path "./models/train/MY_LORA_NAME_cache" --dataset_num_workers 2 --find_unused_parameters --zero_cond_t --initialize_model_on_cpu --task "sft:data_process"
EOF
```

**Step 3 — Create the Stage 2 script** (trains the LoRA — run after Stage 1):

```bash
cat > /home/steve/DiffSynth-Studio/examples/qwen_image/model_training/lora/MY_LORA_NAME_stage2.sh << 'EOF'
export HF_HOME=/mnt/models/huggingface
export PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True

accelerate launch --config_file examples/qwen_image/model_training/special/low_vram_training/accelerate_zero3_2gpu.yaml examples/qwen_image/model_training/train.py --dataset_base_path "./models/train/MY_LORA_NAME_cache" --max_pixels 262144 --dataset_repeat 50 --model_id_with_origin_paths "Qwen/Qwen-Image-Edit-2511:transformer/diffusion_pytorch_model*.safetensors" --learning_rate 1e-4 --num_epochs 5 --remove_prefix_in_ckpt "pipe.dit." --output_path "./models/train/MY_LORA_NAME" --lora_base_model "dit" --lora_target_modules "to_q,to_k,to_v,add_q_proj,add_k_proj,add_v_proj,to_out.0,to_add_out,img_mlp.net.2,img_mod.1,txt_mlp.net.2,txt_mod.1" --lora_rank MY_LORA_RANK --use_gradient_checkpointing --dataset_num_workers 2 --find_unused_parameters --zero_cond_t --initialize_model_on_cpu --task "sft:train"
EOF
```

> **Critical**: The `accelerate launch` line must remain on a single line in the script file. Verify with:
> ```bash
> wc -l examples/qwen_image/model_training/lora/MY_LORA_NAME_stage2.sh
> ```
> Should output `4` (two export lines + one blank line + one accelerate line).

### Start a tmux session

Training takes 4–5 hours. Run inside tmux so it survives SSH disconnects:

```bash
tmux new-session -s MY_LORA_NAME-training
source /home/steve/diffsynth-env/bin/activate
cd /home/steve/DiffSynth-Studio
```

### Run Stage 1

```bash
bash examples/qwen_image/model_training/lora/MY_LORA_NAME_stage1.sh
```

**What to expect:**
- First run downloads text encoder + VAE from ModelScope (~17 GB) — may take 30–60 minutes
- GPU VRAM: ~8–9 GB per GPU (text encoder + VAE split across both GPUs)
- Runtime: 5–20 minutes for 20–50 images once models are downloaded
- Output: cached files written to `./models/train/MY_LORA_NAME_cache/`
- No LoRA file is produced — that is Stage 2's output

Stage 1 only needs to run once for a given dataset. If you retrain with the same images, skip Stage 1. If you change your images or change `--max_pixels`, delete the cache and re-run Stage 1:

```bash
rm -rf /home/steve/DiffSynth-Studio/models/train/MY_LORA_NAME_cache
```

### Run Stage 2

```bash
bash examples/qwen_image/model_training/lora/MY_LORA_NAME_stage2.sh
```

**What to expect:**
- First run downloads the transformer from ModelScope (~41 GB) — may take 60–90 minutes
- GPU 0: ~13–15 GB VRAM (active layer parameters + activations)
- GPU 1: ~5 MiB VRAM (idle — ZeRO-3 single-process only uses GPU 0)
- CPU RAM: ~46–50 GB used (41 GB transformer offloaded from VRAM to CPU RAM)
- Runtime: ~4–5 hours for 5 epochs on 44 images at 512×512
- Progress bar shows steps out of total (e.g. `16/2200`)

**Monitor resources in a second tmux pane:**

```bash
watch -n 5 'free -h && echo "---" && nvidia-smi --query-gpu=memory.used --format=csv,noheader'
```

Watch the `available` column in the RAM output. If it drops below ~2 GB the Linux OOM killer may terminate the process. If your RAM usage is consistently near the limit, reduce `--dataset_num_workers` to `0` in the Stage 2 script.

---

## Output Files

LoRA files are saved at the end of each epoch:

```
~/DiffSynth-Studio/models/train/MY_LORA_NAME/
├── epoch-0.safetensors
├── epoch-1.safetensors
├── epoch-2.safetensors
├── epoch-3.safetensors
└── epoch-4.safetensors
```

Each file is a complete LoRA checkpoint. Test them in ComfyUI to find the best epoch — earlier epochs tend to be more flexible (follows prompts well, weaker likeness), later epochs tend to be stronger in likeness but risk overfitting.

---

## Installing and Testing the LoRA

Copy the best epoch to your ComfyUI models directory:

```bash
cp /home/steve/DiffSynth-Studio/models/train/MY_LORA_NAME/epoch-2.safetensors \
   /mnt/models/comfyui/loras/MY_LORA_NAME.safetensors
```

If Amelia's instance should also have access:

```bash
sudo ln /mnt/models/comfyui/loras/MY_LORA_NAME.safetensors \
        /mnt/models/comfyui-amelia/loras/MY_LORA_NAME.safetensors
```

Refresh the ComfyUI browser tab. The LoRA appears in the Load LoRA node dropdown.

**Prompting strategy:**

Always include your trigger word in the editing instruction. Examples:

```
make saraI person wear a red jacket
change the background to show saraI person in a garden
saraI person with different hair colour
```

**Testing all epochs:**

Copy each epoch file with a distinct name and test them back to back in ComfyUI:

```bash
for i in 0 1 2 3 4; do
    cp /home/steve/DiffSynth-Studio/models/train/MY_LORA_NAME/epoch-${i}.safetensors \
       /mnt/models/comfyui/loras/MY_LORA_NAME_epoch${i}.safetensors
done
```

---

## Troubleshooting

### CUDA out of memory during Stage 2 model loading

ZeRO-3 is not working correctly. Check:

1. DeepSpeed is installed: `python3 -c "import deepspeed; print(deepspeed.__version__)"`
2. The `accelerate_zero3_2gpu.yaml` config file exists and has `num_processes: 1`
3. No Docker containers or Ollama are using VRAM (`docker stop comfyui comfyui-amelia && sudo systemctl stop ollama`)
4. No lingering Python processes from a previous failed run (`nvidia-smi`, then `sudo kill -9 <PID>`)

### CUDA out of memory during Stage 2 forward pass

Activation tensors are too large. Reduce `--max_pixels`:

- Current: `262144` (512×512)
- Try: `131072` (362×362)

**Important**: if you reduce `--max_pixels`, you must delete the Stage 1 cache and re-run Stage 1 at the same lower value first:

```bash
rm -rf /home/steve/DiffSynth-Studio/models/train/MY_LORA_NAME_cache
# Update --max_pixels in stage1 script to match, then:
bash examples/qwen_image/model_training/lora/MY_LORA_NAME_stage1.sh
```

### ComfyUI processes won't die

They are managed by Docker. Use `docker stop`, not `kill`:

```bash
docker stop comfyui comfyui-amelia
```

### Training killed by Linux OOM (system RAM exhausted, not CUDA OOM)

The 41 GB transformer in CPU RAM + OS overhead is close to the 64 GB limit. Reduce memory pressure:

1. Close any other large processes
2. Set `--dataset_num_workers 0` in the Stage 2 script (eliminates worker processes)
3. Check swap usage: if swap is nearly full (`free -h`), increase swap space or accept that this machine is at its RAM limit for this model

### `ValueError: optimizer got an empty parameter list`

The `accelerate launch` command wrapped onto multiple lines in the script. Recreate the script using `cat << 'EOF'` and verify with `wc -l` (should be 4 lines).

### Stage 1 or Stage 2 reports `PermissionError: [Errno 13] Permission denied`

Training images are not readable. Fix:

```bash
chmod 755 MY_DATASET_PATH
chmod 644 MY_DATASET_PATH/*.jpg MY_DATASET_PATH/*.png
```

### `duplicate session:` error when creating tmux session

A previous tmux session with that name already exists. Either attach to it or kill it:

```bash
tmux attach -t MY_LORA_NAME-training   # attach and check what's running
# or
tmux kill-session -t MY_LORA_NAME-training   # kill it, then create fresh
```

---

## Key Parameter Reference

| Parameter | Stage 1 | Stage 2 | Notes |
|---|---|---|---|
| `--max_pixels` | `262144` | `262144` | **Must match between Stage 1 and Stage 2.** Controls image resolution in training. |
| `--lora_rank` | — | `16` | Higher = more expressive LoRA, more VRAM. 8 if OOM, 32 for more capacity. |
| `--num_epochs` | — | `5` | Produces one `.safetensors` output file per epoch. |
| `--dataset_repeat` | `1` | `50` | Stage 1 processes each image once. Stage 2 repeats the cached data 50× per epoch for small datasets. |
| `--learning_rate` | — | `1e-4` | Standard starting point. Lower to `5e-5` if later epochs overfit severely. |
| `--dataset_num_workers` | `2` | `2` | Reduce to `0` if RAM is nearly exhausted during Stage 2. |
