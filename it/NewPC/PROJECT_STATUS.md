# Project Status — NewPC AI Server

**Last Updated**: 2026-03-18

---

## Current State

Server (`amelai`) is fully operational with all AI services running and accessible via both local network and Tailscale. Dual Docker port binding strategy implemented for all services.

## Service Status

| Service | Container | Local | Tailscale | Status |
|---|---|---|---|---|
| Ollama | host process | `localhost:11434` | via Open WebUI | Running |
| Open WebUI | `open-webui` | `http://192.168.1.192:3000` | `https://amelai.tail926601.ts.net` | Running |
| ComfyUI (Steve) | `comfyui` | `http://192.168.1.192:8189` | `https://amelai.tail926601.ts.net:8189` | Running |
| ComfyUI (Amelia) | `comfyui-amelia` | `http://192.168.1.192:8188` | `https://amelai.tail926601.ts.net:8188` | Running |
| FileBrowser | `filebrowser` | `http://192.168.1.192:8087` | `https://amelai.tail926601.ts.net:8087` | Running |

## Active Work Areas

- **Qwen-Image-Edit LoRA Training**: First training run in progress — confirmed downloading correctly (~20GB, 4×5GB files via ModelScope). Training command uses `--num_processes 2 --mixed_precision bf16` to distribute model across both GPUs
- **Networking**: Dual binding strategy confirmed and documented — all services accessible on both LAN and Tailscale

## Recently Completed

- Added Workflow 3 to `Model_and_LoRA_Creation.md` — full Qwen-Image-Edit character LoRA training guide via DiffSynth-Studio
- Created `MultiFileModels.md` — standalone reference for HuggingFace diffusers multi-file model format
- Resolved all training setup errors (torchaudio, metadata.json format, `hf` CLI, CUDA OOM from ComfyUI, single-GPU vs multi-GPU accelerate)
- Created `Model_and_LoRA_Creation.md` — comprehensive guide for FLUX character LoRA and LLM chatbot fine-tuning
- Updated `CLAUDE.md` with system spec references and quick-reference hardware/software summary
- Created comprehensive `Tailscale.md` guide
- Implemented dual Docker `-p` binding for all services

## Pending / Next Actions

- [ ] Confirm training completes and LoRA is generated (`./models/train/my_character_lora/`)
- [ ] Test LoRA in ComfyUI — copy to `/mnt/models/comfyui/loras/`
- [ ] Curate photo dataset for character LoRA (20–30 varied images) if not done
- [ ] Install ai-toolkit on server for FLUX LoRA training (Workflow 1)
- [ ] Create JSONL training dataset for LLM knowledge chatbot (Workflow 2)
- [ ] Install Unsloth for LLM fine-tuning (Workflow 2)
- [ ] Apply dual `-p` binding to ComfyUI and ComfyUI-Amelia containers
- [ ] Set static DHCP reservation on router for `192.168.1.192` if not already done

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
