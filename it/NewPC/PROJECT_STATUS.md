# Project Status — NewPC AI Server

**Last Updated**: 2026-03-19

---

## Current State

Server (`amelai`) is fully operational. Qwen-Image-Edit LoRA Stage 2 training restarted after tmux session loss — running in `lora-training` tmux session. Documentation expanded with tmux and Docker reference guides.

## Service Status

| Service | Container | Local | Tailscale | Status |
|---|---|---|---|---|
| Ollama | host process | `localhost:11434` | via Open WebUI | Stopped (training in progress) |
| Open WebUI | `open-webui` | `http://192.168.1.192:3000` | `https://amelai.tail926601.ts.net` | Running |
| ComfyUI (Steve) | `comfyui` | `http://192.168.1.192:8189` | `https://amelai.tail926601.ts.net:8189` | Stopped (training in progress) |
| ComfyUI (Amelia) | `comfyui-amelia` | `http://192.168.1.192:8188` | `https://amelai.tail926601.ts.net:8188` | Stopped (training in progress) |
| FileBrowser | `filebrowser` | `http://192.168.1.192:8087` | `https://amelai.tail926601.ts.net:8087` | Running |

## Active Work Areas

- **Qwen-Image-Edit LoRA Training**: Stage 2 restarted in tmux session `lora-training`. Script: `stage2_train.sh`. Output: `~/DiffSynth-Studio/models/train/my_character_lora/epoch-N.safetensors`. ~4–5 hours runtime.

## Recently Completed

- Created `TMUX.md` — tmux reference guide (sessions, detach/attach, panes, scroll, quick reference)
- Created `Docker.md` — Docker admin guide with all service `docker run` commands, port map, SSH file access
- Diagnosed and restarted interrupted Stage 2 LoRA training (tmux session had been lost)
- Confirmed Stage 1 cache intact — did not need to re-run Stage 1

## Pending / Next Actions

- [ ] Confirm Stage 2 training completes — check `ls ~/DiffSynth-Studio/models/train/my_character_lora/`
- [ ] Restart Docker containers after training: `docker start comfyui comfyui-amelia && sudo systemctl start ollama`
- [ ] Test each epoch LoRA in ComfyUI — copy from `~/DiffSynth-Studio/models/train/my_character_lora/` to `/mnt/models/comfyui/loras/`
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 to replace obsolete FP8+DDP approach with ZeRO-3 method
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
