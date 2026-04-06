# Project Status — NewPC AI Server

**Last Updated**: 2026-04-06

---

## Current State

Server (`amelai`) is fully operational. Primary network connection switched to Aquantia AQC113 10GbE NIC (`ethernet2_5g`, 192.168.1.192) after the Intel igc NIC crashed a second time — `pcie_aspm=off` proved insufficient; igc driver now blacklisted. Timezone corrected to Europe/London. NFS mount, ComfyUI, and all services running normally.

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

- Switched primary NIC to Aquantia AQC113 10GbE — Intel igc I226-V blacklisted after second PCIe crash
- Fixed system timezone to `Europe/London` (was UTC, causing log timestamps to show 1hr early)
- Removed broken WiFi section from netplan (no password set was causing `netplan apply` errors)
- Mounted Synology DS920+ `MyDocs` share permanently at `/docs` via NFS
- Fixed Qwen-Rapid-AIO OOM error — `--reserve-vram 3` added to ComfyUI CLI_ARGS
- Fixed ComfyUI Tailscale access — loopback port typo corrected (`8189`→`18189` in docker run)

## Pending / Next Actions

- [ ] **Verify on next reboot**: 90-second boot delay (WiFi `wlp11s0`) resolved — if not, run `sudo systemctl mask systemd-networkd-wait-online.service`
- [ ] **Verify on next reboot**: `/docs` NFS mount auto-mounts correctly
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai
- [ ] Verify ComfyUI OOM fix — confirm first generation succeeds without click-OK-retry
- [ ] Verify FileBrowser shows `comfyui-input/` with Qwen-generated images
- [ ] Set static DHCP reservation on router for `192.168.1.192`
- [ ] Install ai-toolkit for FLUX LoRA training (Workflow 1)
- [ ] Create JSONL training dataset for LLM knowledge chatbot (Workflow 2)
- [ ] Research and confirm current UK pricing for RTX 5070 Ti 16GB (AIB partner selection)
- [ ] Verify Arctic Liquid Freezer III 360 compatibility with Corsair 4000D Airflow case
- [ ] Confirm Ryzen 7 9800X3D UK street price and retailer availability

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
