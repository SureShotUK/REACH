# Project Status — NewPC AI Server

**Last Updated**: 2026-03-18 (Evening)

---

## Current State

Server (`amelai`) is fully operational. Qwen-Image-Edit LoRA training is actively running using DeepSpeed ZeRO-3 CPU offload — the only approach that works on 2×24 GB hardware for this 41 GB model.

## Service Status

| Service | Container | Local | Tailscale | Status |
|---|---|---|---|---|
| Ollama | host process | `localhost:11434` | via Open WebUI | Stopped (training in progress) |
| Open WebUI | `open-webui` | `http://192.168.1.192:3000` | `https://amelai.tail926601.ts.net` | Running |
| ComfyUI (Steve) | `comfyui` | `http://192.168.1.192:8189` | `https://amelai.tail926601.ts.net:8189` | Stopped (training in progress) |
| ComfyUI (Amelia) | `comfyui-amelia` | `http://192.168.1.192:8188` | `https://amelai.tail926601.ts.net:8188` | Stopped (training in progress) |
| FileBrowser | `filebrowser` | `http://192.168.1.192:8087` | `https://amelai.tail926601.ts.net:8087` | Running |

## Active Work Areas

- **Qwen-Image-Edit LoRA Training**: Stage 2 actively running (~4–5 hours total). Using DeepSpeed ZeRO-3 CPU offload (`num_processes: 1`), `--max_pixels 262144`, `--use_gradient_checkpointing`. ~13–15 GB VRAM on GPU 0, ~46 GB RAM.

## Recently Completed

- Created `QwenImageEditTrainingLoRA.md` — complete standalone training guide (verified working procedure)
- Resolved all VRAM OOM errors blocking training — migrated from FP8+DDP to ZeRO-3 CPU offload
- Identified ComfyUI instances as Docker containers (corrects previous incorrect documentation)
- Confirmed DeepSpeed ZeRO-3 + `--use_gradient_checkpointing` + `--max_pixels 262144` is the working combination

## Pending / Next Actions

- [ ] Confirm Stage 2 training completes — expected ~5 hours from start, producing `epoch-0.safetensors` through `epoch-4.safetensors`
- [ ] Test each epoch LoRA in ComfyUI — copy from `~/DiffSynth-Studio/models/train/my_character_lora/` to `/mnt/models/comfyui/loras/`
- [ ] Restart Docker containers after training: `docker start comfyui comfyui-amelia`
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 to replace obsolete FP8+DDP approach with ZeRO-3 method
- [ ] Update `CLAUDE.md` ComfyUI process management note — IS Docker, use `docker stop/start`
- [ ] Install ai-toolkit for FLUX LoRA training (Workflow 1)
- [ ] Create JSONL training dataset for LLM knowledge chatbot (Workflow 2)
- [ ] Set static DHCP reservation on router for `192.168.1.192`

## Key Files

| File | Purpose |
|---|---|
| `Final_Build.md` | Complete hardware specification — authoritative system spec reference |
| `Software_Setup.md` | Complete server setup guide — OS through full AI stack |
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
