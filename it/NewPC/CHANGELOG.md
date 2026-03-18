# Changelog — NewPC Project

---

## [Unreleased] - 2026-03-18 (Evening)

### Added
- `QwenImageEditTrainingLoRA.md` — comprehensive standalone guide for Qwen-Image-Edit-2511 character LoRA training:
  - Training image guidelines (count, resolution, angle variety, exclusion rules)
  - metadata.json creation script with correct `{"prompt": ..., "image": ...}` format
  - Accelerate ZeRO-3 config file (`accelerate_zero3_2gpu.yaml`) creation instructions
  - Stage 1 and Stage 2 scripts with all verified parameters
  - Per-run variable table (`MY_LORA_NAME`, `MY_DATASET_PATH`, `MY_TRIGGER_WORD`, `MY_LORA_RANK`)
  - Resource monitoring commands and expected VRAM/RAM usage
  - Output file locations and epoch selection guidance
  - Full troubleshooting section covering all errors encountered during development

### Changed
- `Model_and_LoRA_Creation.md` — `--max_pixels` reduced from 1048576 → 786432 → 524288 → 262144 (final working value); `--lora_rank` parameter table note updated

### Fixed
- ComfyUI process management: corrected from `sudo pkill -f "ComfyUI/main.py"` to `docker stop comfyui comfyui-amelia` — instances ARE Docker containers, not plain Python processes
- Training approach: replaced non-functional FP8+DDP method with DeepSpeed ZeRO-3 CPU offload (`--config_file accelerate_zero3_2gpu.yaml`, `num_processes: 1`)
- `--use_gradient_checkpointing` restored for Stage 2 — required to prevent activation tensors filling VRAM; was incorrectly removed based on outdated incompatibility warning

### Documentation
- Documented that FP8+DDP fundamentally cannot work on 2×24 GB: transformer fills ~23.2 GB per GPU regardless of quantisation, leaving no room for overhead
- Documented ZeRO-3 CPU offload mechanics: 41 GB transformer offloaded to CPU RAM, GPU holds only active layer parameters (~13–15 GB peak VRAM)
- Documented that `num_processes: 1` uses less VRAM than `num_processes: 2` under ZeRO-3 CPU offload (no AllGather communication buffers)
- Documented Stage 1 / Stage 2 `--max_pixels` coupling: cached latent dimensions are fixed at Stage 1 generation time; must delete cache and re-run Stage 1 when changing resolution
- Documented DeepSpeed ZeRO-3 merge date (March 17, 2026) — explains why previous session's approach predated this fix

---

## [Unreleased] - 2026-03-18

### Added
- `MultiFileModels.md` — standalone reference document explaining:
  - HuggingFace diffusers multi-file model format vs single-file safetensors
  - Qwen-Image-Edit-2511 file structure (transformer shards + text encoder + VAE)
  - Three options for using multi-file models in ComfyUI
  - DiffSynth-Studio model loading via `model_id_with_origin_paths`
  - `hf` CLI setup (venv creation, install, activation) from scratch
- `Model_and_LoRA_Creation.md` — **Workflow 3**: Qwen-Image-Edit character LoRA training:
  - Step 1: Dataset preparation with Python metadata.json generation script
  - Step 2: DiffSynth-Studio install + ModelScope model download
  - Step 3: Dependency verification (torchaudio fix, CUDA toolkit)
  - Step 4: Training script with multi-GPU accelerate config
  - Step 5: LoRA installation in ComfyUI

### Changed
- `Model_and_LoRA_Creation.md` Step 4 training command updated:
  - Added `--num_processes 2 --mixed_precision bf16` to `accelerate launch` — required to distribute model across both GPUs
  - Added `export PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True`
  - Added `--initialize_model_on_cpu` — prevents VRAM spike on GPU 0 during model load
  - Reduced `--max_pixels` from 1763584 to 1048576 (1024×1024)
  - Reduced `--lora_rank` from 32 to 16
  - Reduced `--dataset_num_workers` from 8 to 2
  - Added pre-flight GPU check section (stop ComfyUI and Ollama before training)
  - Expanded parameters table with explanations for all new flags
  - Updated download size estimate from ~58GB to ~20GB (4×5GB files via ModelScope)

### Documentation
- Documented why `--num_processes 2` is critical: without it accelerate uses single GPU, causing OOM on the 20B transformer
- Documented `--mixed_precision bf16` effect: causes DiffSynth-Studio to load fp8/quantised variant (~10GB per GPU vs ~25GB)
- Documented ComfyUI process management before training (plain Python processes, not Docker, kill with `sudo kill <PID>`)
- Documented `hf` vs `huggingface-cli` rename in huggingface_hub 1.x
- Documented DiffSynth-Studio metadata.json requirement (not .txt caption files)
- Documented ModelScope cache location (`~/.cache/modelscope/`)

---

## [Unreleased] - 2026-03-17

### Added
- `Model_and_LoRA_Creation.md` — comprehensive training guide covering two full workflows:
  - **Workflow 1**: FLUX.1 Dev character LoRA using ai-toolkit (photo selection criteria, dataset prep, config, training, ComfyUI integration)
  - **Workflow 2**: LLM fine-tuning for a custom knowledge chatbot using Unsloth (JSONL dataset format, training script, GGUF export, Ollama deployment)
  - Common pitfalls section for both workflows
  - Quick reference checklists for both workflows

### Changed
- `CLAUDE.md` — added "System Specifications Reference" section at the top of Project Purpose, directing Claude to read `Final_Build.md` and `Software_Setup.md` for hardware/software specs; includes inline quick-reference summary

### Documentation
- Documented ai-toolkit verified parameters for RTX 3090 24GB: rank 16, lr 1e-4, 2000 steps, bf16, adamw8bit, gradient checkpointing
- Documented Unsloth QLoRA configuration: rank 16, lr 2e-4, 3 epochs, bf16, NVLink multi-GPU support
- Documented photo dataset selection criteria (inclusion/exclusion rules, captioning strategy, trigger word guidance)
- Documented JSONL conversational format for LLM training data with system prompt injection pattern
- Documented GGUF export and Ollama Modelfile creation workflow

---

## [Unreleased] - 2026-03-14

### Added
- `Tailscale.md` — comprehensive guide covering: overview, installation, useful commands (port checking, status, diagnostics), port forwarding via tailscale serve and iptables, UFW alternative, troubleshooting section, securing services with loopback binding, dual binding for LAN + Tailscale access, and how the two access paths work independently

### Changed
- `HuggingFace.md` — added "Sharing Models with Amelia's Instance" section documenting hard link commands for adding/removing models from the restricted instance
- `Software_Setup.md` — updated all docker run commands with dual `-p` bindings (loopback for Tailscale serve, LAN IP for local access); replaced generic service URL table with full local + Tailscale access table; updated Open WebUI and FileBrowser access sections with specific URLs; added `sudo` to Tailscale serve rebuild commands; removed outdated "offset ports" note

### Documentation
- Documented Docker `-p` flag in depth: IP binding to network interfaces, host port vs container port, container isolation meaning ports can repeat across containers
- Documented why path-based Tailscale routing was evaluated and rejected (ComfyUI has no base URL support)
- Documented static DHCP reservation requirement for reliable dual binding

---

## Prior Sessions

Session history prior to 2026-03-14 is tracked in git commit messages. Run `git log --oneline` for a summary.
