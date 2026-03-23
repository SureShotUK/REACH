# Project Status — NewPC AI Server

**Last Updated**: 2026-03-23

---

## Current State

Server (`amelai`) is fully operational. MCP web search is now working from Windows 11 Claude Code and Open WebUI. All services running normally — ComfyUI containers restarted during session (were holding 40GB VRAM idle) and are back up.

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

- No active long-running tasks.

## Recently Completed

- Fixed MCP web search for Windows 11 Claude Code — two root causes: port 3001 missing from Tailscale ACL, and stored Anthropic credential overriding Ollama URL
- Fixed Open WebUI Ollama connection (`https://` → `http://`)
- Fixed `hf-env` auto-activation on SSH login (removed from `~/.bashrc`)
- Re-registered MCP as user-scoped (works in all projects)
- Created `SearXNG_Fix.md` — troubleshooting log and architecture reference
- Created `New_PC_Builds.md` — personal Windows 11 PC build guide (Ryzen 7 9800X3D, RTX 5070 Ti, be quiet! Power Zone 2 1000W, Arctic Liquid Freezer III 360, Corsair 4000D Airflow)

## Pending / Next Actions

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
| `CLAUDE.md` | Project-specific guidance for this directory |

## Hardware

- CPU: AMD Ryzen 9 7900X
- GPU: 2× ASUS TUF RTX 3090 24GB (48GB total VRAM)
- RAM: 64GB DDR5
- Storage: 2× Samsung 9100 Pro 2TB NVMe
- OS: Ubuntu 24.04 LTS Server
