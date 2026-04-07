# Project Status — NewPC AI Server

**Last Updated**: 2026-04-07

---

## Current State

Server (`amelai`) is fully operational. All services running normally. One outstanding hardware-level issue: both RTX 3090 GPUs are stuck at PCIe Gen 1 (2.5GT/s) — confirmed BIOS/AGESA bug on BIOS 2103; all configurable and physical causes eliminated; raised with ASUS. Performance impact on AI workloads is nil. `pcie_aspm=off` kernel parameter removed (was needed for igc NIC, now unnecessary as igc is blacklisted).

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

- Exhaustive PCIe Gen 1 investigation — confirmed BIOS/AGESA bug; `ASUS_PCIe_Support_Case.md` created for ASUS support
- Removed `pcie_aspm=off` kernel parameter — no longer needed (igc blacklisted); fixes nvidia-smi width reporting
- Updated BIOS to 2103 (from 2102) — no change to PCIe Gen issue
- Switched primary NIC to Aquantia AQC113 10GbE — Intel igc I226-V blacklisted after second PCIe crash
- Fixed system timezone to `Europe/London`
- Mounted Synology DS920+ `MyDocs` share permanently at `/docs` via NFS

## Pending / Next Actions

- [ ] **ASUS forum**: Post PCIe Gen 1 issue with diagnostic data from `ASUS_PCIe_Support_Case.md`
- [ ] **ASUS support**: Contact technical team — reference CPU root ports `00:01.1`/`00:01.3` stuck at 2.5GT/s on BIOS 2103
- [ ] **Monitor**: ASUS BIOS 2104+ for PCIe Gen fix in release notes
- [ ] **Verify on next reboot**: 90-second boot delay (WiFi `wlp11s0`) resolved — if not, run `sudo systemctl mask systemd-networkd-wait-online.service`
- [ ] **Verify on next reboot**: `/docs` NFS mount auto-mounts correctly
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai
- [ ] Verify ComfyUI OOM fix — confirm first generation succeeds without click-OK-retry
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
