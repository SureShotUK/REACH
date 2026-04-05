# Project Status — NewPC AI Server

**Last Updated**: 2026-04-05

---

## Current State

Server (`amelai`) is fully operational. ComfyUI OOM issue with Qwen-Rapid-AIO-NSFW-v23 resolved with `--reserve-vram 3`. FileBrowser now exposes ComfyUI input and output folders. Git repo fully synced between Windows and Linux with `.gitignore` and `/sync-files` command in place.

## Service Status

| Service | Container | Local | Tailscale | Status |
|---|---|---|---|---|
| Ollama | host process | `http://192.168.1.192:11434` | via Open WebUI | Running |
| Open WebUI | `open-webui` | `http://192.168.1.192:3000` | `https://amelai.tail926601.ts.net` | Running |
| ComfyUI (Steve) | `comfyui` | `http://192.168.1.192:8189` | `https://amelai.tail926601.ts.net:8189` | Running |
| ComfyUI (Amelia) | `comfyui-amelia` | `http://192.168.1.192:8188` | `https://amelai.tail926601.ts.net:8188` | Running |
| FileBrowser | `filebrowser` | `http://192.168.1.192:8087` | `https://amelai.tail926601.ts.net:8087` | Running |
| MCP Server | systemd service | `http://100.79.83.113:3001` | port 3001 (ACL updated) | Running |

## Active Work Areas

- Verify ComfyUI OOM fix works end-to-end on first generation attempt

## Recently Completed

- Fixed Qwen-Rapid-AIO OOM error — `--reserve-vram 3` added to ComfyUI CLI_ARGS
- Fixed ComfyUI Tailscale access — loopback port typo corrected (`8189`→`18189` in docker run)
- FileBrowser updated — now exposes `comfyui-input/`, `comfyui-output/`, `comfyui-amelia-input/`, `comfyui-amelia-output/`
- Created `.gitignore` and committed all 139 previously untracked files
- Cross-platform git sync fully operational — Windows and Linux in sync via GitHub
- Added "warnings before commands" rule to shared CLAUDE.md

## Pending / Next Actions

- [ ] Verify ComfyUI OOM fix — confirm first generation succeeds without click-OK-retry
- [ ] Verify FileBrowser shows `comfyui-input/` with Qwen-generated images
- [ ] Check Load Image node can browse input folder to select previous generations without downloading
- [ ] Monitor NIC stability — confirm `pcie_aspm=off` holds
- [ ] Address `systemd-networkd-wait-online` timeouts (WiFi adapter wlp11s0)
- [ ] Install ai-toolkit for FLUX LoRA training (Workflow 1)
- [ ] Create JSONL training dataset for LLM knowledge chatbot (Workflow 2)
- [ ] Set static DHCP reservation on router for `192.168.1.192`
- [ ] Address `systemd-networkd-wait-online` timeouts (WiFi adapter wlp11s0 — known ASUS X870E Linux issue)
- [ ] Research and confirm current UK pricing for RTX 5070 Ti 16GB (AIB partner selection)
- [ ] Verify Arctic Liquid Freezer III 360 compatibility with Corsair 4000D Airflow case
- [ ] Confirm Ryzen 7 9800X3D UK street price and retailer availability
- [ ] Install ai-toolkit for FLUX LoRA training (Workflow 1)
- [ ] Create JSONL training dataset for LLM knowledge chatbot (Workflow 2)
- [ ] Set static DHCP reservation on router for `192.168.1.192`

## Key Files

| File | Purpose |
|---|---|
| `Final_Build.md` | Complete hardware specification — authoritative system spec reference |
| `Software_Setup.md` | Complete server setup guide — OS through full AI stack |
| `New_PC_Builds.md` | Personal Windows 11 PC build guide — component research and chosen configuration |
| `LoadClientClaude.md` | Windows client setup — Ollama env vars, MCP registration, CLAUDE.md config |
| `SearXNG_Fix.md` | MCP web search troubleshooting log — root causes and fix reference |
| `Model_and_LoRA_Creation.md` | Training guide — FLUX character LoRA, LLM fine-tuning, and Qwen-Image-Edit LoRA |
| `Tailscale.md` | Tailscale commands, port forwarding, troubleshooting, Docker binding strategy |
| `ComfyUI.md` | ComfyUI setup, workflows, and model management |
| `FileWriter.py` | Open WebUI Tool — paste into Workspace → Tools to give models file write capability |
| `CLAUDE.md` | Project-specific guidance for this directory |

## Hardware

- CPU: AMD Ryzen 9 7900X
- GPU: 2× ASUS TUF RTX 3090 24GB (48GB total VRAM)
- RAM: 64GB DDR5
- Storage: 2× Samsung 9100 Pro 2TB NVMe
- OS: Ubuntu 24.04 LTS Server
