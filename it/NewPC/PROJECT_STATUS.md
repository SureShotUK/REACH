# Project Status — NewPC AI Server

**Last Updated**: 2026-03-14

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

- **Networking**: Dual binding strategy confirmed and documented — all services accessible on both LAN and Tailscale
- **Documentation**: Tailscale.md created; Software_Setup.md updated to reflect final port assignments and dual binding

## Recently Completed

- Created comprehensive `Tailscale.md` guide
- Implemented dual Docker `-p` binding for FileBrowser and Open WebUI
- Resolved Tailscale serve misconfiguration (FileBrowser was pointing to wrong internal port)
- Confirmed final port assignments for all services
- Updated all docker run commands and service URL tables in `Software_Setup.md`
- Added Amelia model sharing (hard links) to `HuggingFace.md`

## Pending / Next Actions

- [ ] Apply dual `-p` binding to ComfyUI and ComfyUI-Amelia containers on the server
- [ ] Verify all service bindings with `sudo ss -tlnup` after ComfyUI changes
- [ ] Set static DHCP reservation on router for `192.168.1.192` if not already done

## Key Files

| File | Purpose |
|---|---|
| `Software_Setup.md` | Complete server setup guide — OS through full AI stack |
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
