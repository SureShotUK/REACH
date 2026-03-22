# Project Status — NewPC AI Server

**Last Updated**: 2026-03-22

---

## Current State

Server (`amelai`) is fully operational. Full 5-epoch Qwen-Image-Edit LoRA training is running in tmux session `lora-training` after diagnosing and fixing persistent OOM failures. Training pipeline confirmed working — `epoch-0.safetensors` produced in test run. Expected to complete overnight (~6–8 hours with current settings).

## Service Status

| Service | Container | Local | Tailscale | Status |
|---|---|---|---|---|
| Ollama | host process | `localhost:11434` | via Open WebUI | Stopped (training in progress) |
| Open WebUI | `open-webui` | `http://192.168.1.192:3000` | `https://amelai.tail926601.ts.net` | Running |
| ComfyUI (Steve) | `comfyui` | `http://192.168.1.192:8189` | `https://amelai.tail926601.ts.net:8189` | Stopped (training in progress) |
| ComfyUI (Amelia) | `comfyui-amelia` | `http://192.168.1.192:8188` | `https://amelai.tail926601.ts.net:8188` | Stopped (training in progress) |
| FileBrowser | `filebrowser` | `http://192.168.1.192:8087` | `https://amelai.tail926601.ts.net:8087` | Running |

## Service Status

| Service | Container | Local | Tailscale | Status |
|---|---|---|---|---|
| Ollama | host process | `localhost:11434` | via Open WebUI | Stopped (training in progress) |
| Open WebUI | `open-webui` | `http://192.168.1.192:3000` | `https://amelai.tail926601.ts.net` | Running |
| ComfyUI (Steve) | `comfyui` | `http://192.168.1.192:8189` | `https://amelai.tail926601.ts.net:8189` | Stopped (training in progress) |
| ComfyUI (Amelia) | `comfyui-amelia` | `http://192.168.1.192:8188` | `https://amelai.tail926601.ts.net:8188` | Stopped (training in progress) |
| FileBrowser | `filebrowser` | `http://192.168.1.192:8087` | `https://amelai.tail926601.ts.net:8087` | Running |

## Active Work Areas

- **Qwen-Image-Edit LoRA Training**: Full 5-epoch run active in tmux session `lora-training`. Output: `~/DiffSynth-Studio/models/train/my_character_lora/epoch-N.safetensors`. ~6–8 hours with current settings (`pin_memory: false`, `--dataset_num_workers 0`).

## Recently Completed

- Created `New_PC_Builds.md` — personal Windows 11 PC build guide (Ryzen 7 9800X3D, RTX 5070 Ti, be quiet! Power Zone 2 1000W, Arctic Liquid Freezer III 360, Corsair 4000D Airflow)
- Diagnosed and fixed persistent LoRA training OOM — root cause was checkpoint save memory spike (41 GB → 87 GB). Fixed with 32 GB swap + `pin_memory: false`
- Confirmed training pipeline working end-to-end (`epoch-0.safetensors` produced in test)
- Created `LoRAMemoryFixes.md` — complete diagnosis, fixes, and speed optimisation guide
- Created `TMUX.md` — tmux reference guide
- Created `Docker.md` — Docker admin guide with all service `docker run` commands

## Pending / Next Actions

- [ ] Confirm full training completes — check `ls ~/DiffSynth-Studio/models/train/my_character_lora/`
- [ ] Restart Docker + Ollama: `docker start comfyui comfyui-amelia && sudo systemctl start ollama`
- [ ] Test each epoch LoRA in ComfyUI — copy to `/mnt/models/comfyui/loras/`
- [ ] Try speed optimisations from `LoRAMemoryFixes.md` — restore `pin_memory: true` then `--dataset_num_workers 2`
- [ ] Update `QwenImageEditTrainingLoRA.md` with memory fix requirements
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 — replace obsolete FP8+DDP with ZeRO-3 method
- [ ] Install ai-toolkit for FLUX LoRA training (Workflow 1)
- [ ] Create JSONL training dataset for LLM knowledge chatbot (Workflow 2)
- [ ] Set static DHCP reservation on router for `192.168.1.192`

## Key Files

| File | Purpose |
|---|---|
| `Final_Build.md` | Complete hardware specification — authoritative system spec reference |
| `Software_Setup.md` | Complete server setup guide — OS through full AI stack |
| `New_PC_Builds.md` | Personal Windows 11 PC build guide — component research and chosen configuration |
| `Model_and_LoRA_Creation.md` | Training guide — FLUX character LoRA, LLM fine-tuning, and Qwen-Image-Edit LoRA |
| `MultiFileModels.md` | HuggingFace diffusers multi-file model format explained |
| `Tailscale.md` | Tailscale commands, port forwarding, troubleshooting, Docker binding strategy |
| `HuggingFace.md` | Model download guide and Amelia instance sharing |
| `ComfyUI.md` | ComfyUI setup, workflows, and model management |
| `CLAUDE.md` | Project-specific guidance for this directory |

## Hardware

- CPU: AMD Ryzen 9 7900X
- GPU: 2× ASUS TUF RTX 3090 24GB (48GB total VRAM)
- RAM: 64GB DDR5
- Storage: 2× Samsung 9100 Pro 2TB NVMe
- OS: Ubuntu 24.04 LTS Server
