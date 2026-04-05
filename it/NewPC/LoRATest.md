# LoRA Test Run Commands to Run In order

**Create `metadata.json` file**

```
python3 - <<'EOF'
import os, json

image_dir = "/home/steve/training/qwen_test"
trigger_word = "tstng"

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

### Verify the output:

`cat /home/steve/training/qwen_test/metadata.json | head -20`

If out of memory errors persist then close apps using GPU:

```
docker stop comfyui comfyui-amelia
sudo systemctl stop ollama
nvidia-smi
sudo kill -9 <PID>
```

### Create the training scripts

**Stage 1 script** (run once per dataset — caches embeddings to disk):

```
cat > /home/steve/DiffSynth-Studio/examples/qwen_image/model_training/lora/test_stage1.sh << 'EOF'
export HF_HOME=/mnt/models/huggingface
export PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True

accelerate launch --num_processes 2 --mixed_precision bf16 examples/qwen_image/model_training/train.py --dataset_base_path /home/steve/training/qwen_test --dataset_metadata_path /home/steve/training/qwen_test/metadata.json --data_file_keys "image" --max_pixels 262144 --dataset_repeat 1 --model_id_with_origin_paths "Qwen/Qwen-Image:text_encoder/model*.safetensors,Qwen/Qwen-Image:vae/diffusion_pytorch_model.safetensors" --output_path "./models/train/test_cache" --dataset_num_workers 2 --find_unused_parameters --zero_cond_t --initialize_model_on_cpu --task "sft:data_process"
EOF
```

**Stage 2 script** (trains the LoRA — run after Stage 1):

```
cat > /home/steve/DiffSynth-Studio/examples/qwen_image/model_training/lora/test_stage2.sh << 'EOF'
export HF_HOME=/mnt/models/huggingface
export PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True

accelerate launch --config_file examples/qwen_image/model_training/special/low_vram_training/accelerate_zero3_2gpu.yaml examples/qwen_image/model_training/train.py --dataset_base_path "./models/train/test_cache" --max_pixels 262144 --dataset_repeat 1 --model_id_with_origin_paths "Qwen/Qwen-Image-Edit-2511:transformer/diffusion_pytorch_model*.safetensors" --learning_rate 1e-4 --num_epochs 1 --remove_prefix_in_ckpt "pipe.dit." --output_path "./models/train/test" --lora_base_model "dit" --lora_target_modules "to_q,to_k,to_v,add_q_proj,add_k_proj,add_v_proj,to_out.0,to_add_out,img_mlp.net.2,img_mod.1,txt_mlp.net.2,txt_mod.1" --lora_rank 8 --use_gradient_checkpointing --dataset_num_workers 2 --find_unused_parameters --zero_cond_t --initialize_model_on_cpu --task "sft:train"
EOF
```

Verify with 

`wc -l examples/qwen_image/model_training/lora/test_stage2.sh`

### Start a TMUX session

```
tmux new-session -s test-training
source /home/steve/diffsynth-env/bin/activate
cd /home/steve/DiffSynth-Studio
```

### Run Stage 1

`bash examples/qwen_image/model_training/lora/test_stage1.sh`

### Run stage 2

`bash examples/qwen_image/model_training/lora/test_stage2.sh |  tee /home/steve/test_training.log`

## Output Files

**Are found at**

```
~/DiffSynth-Studio/models/train/MY_LORA_NAME/
├── epoch-0.safetensors
├── epoch-1.safetensors
├── epoch-2.safetensors
├── epoch-3.safetensors
└── epoch-4.safetensors
```
