# Project Status — NewPC AI Server

**Last Updated**: 2026-06-09

---

## Current State

Server (`amelai`) is fully operational. Both RTX 3090s running at PCIe Gen 4 (16GT/s) under load — ASPM drops link to Gen 1 at idle, which is normal power-management behaviour. `GRUB_CMDLINE_LINUX_DEFAULT` is empty. All services running normally.

## Service Status

| Service | Container | Local | Tailscale | Status |
|---|---|---|---|---|
| Ollama | host process | `http://192.168.1.192:11434` | via Open WebUI | Running |
| Open WebUI | `open-webui` | `http://192.168.1.192:3000` | `https://amelai.tail926601.ts.net` | Running |
| ComfyUI (Steve) | `comfyui` | Tailscale only | `https://amelai.tail926601.ts.net:8189` | **Needs rebuild** — run command updated |
| ComfyUI (Amelia) | `comfyui-amelia` | `http://192.168.1.192:8188` | `https://amelai.tail926601.ts.net:8188` | Running |
| FileBrowser | `filebrowser` | `http://192.168.1.192:8087` | `https://amelai.tail926601.ts.net:8087` | Running |
| MCP Server | systemd service | `http://100.79.83.113:3001` | port 3001 (ACL updated) | Running |
| n8n | `n8n` | `http://192.168.1.192:5678` | `https://amelai.tail926601.ts.net:5678` | Running |
| STT Server | systemd `stt_server` | — | `ws://amelai.tail926601.ts.net:9090` | Running |

## Active Work Areas

- **ComfyUI (Steve) rebuild required** — run command updated with `--reserve-vram 3` and correct workflows volume path; container must be recreated
- **Alexa WOL skill** — submitted for Amazon certification; awaiting approval (3-5 business days)
- **ReActor face swap workflow** — JSON in `Temp.txt`; needs saving to Workflows folder after container rebuild

## Recently Completed

- **AI Voice Android app** — built and sideloaded to S24 Ultra; voice input → Open WebUI → TTS response working over Tailscale; `gradle.properties` created to fix AndroidX build error; web search via SearXNG enabled; Built-in Tools (except web search) disabled to prevent raw tool call XML in responses
- **STT voice input system** — full end-to-end speech-to-text for Claude Code terminal sessions; faster-whisper large-v3 on amelai GPU 1 (port 9090 systemd service); Windows client with pystray tray icon, F9 hotkey, auto-start via Task Scheduler with elevated privileges; setup guides at `STT_Voice_Input.md` and `STT_New_Machine_Guide.md`
- **ComfyUI OOM fix** — diagnosed missing `--reserve-vram 3` from running container via `docker inspect`; corrected in both `ComfyUI.md` and `Docker.md`; CLAUDE.md rule added to keep both files in sync
- **ReActor face swap workflow created** — basic two-image (body + face) → ReActorFaceSwap → SaveImage workflow; widget values corrected for v0.6.2 node order
- **Docker.md overhaul** — `runlike` documented; ComfyUI Steve output moved to NAS (`/docs/Projects/Claude Code Shared/Output`); FileBrowser workflows volume added; loopback port corrected to 18189; LAN binding removed (Tailscale-only); port table fixed
- **FileBrowser delete issue resolved** — root cause was Linux file ownership (ComfyUI writes as root); moving output to NAS with permissive mount options fixes it
- **Alexa Wake-on-LAN infrastructure** — wol-webhook systemd service, Tailscale Funnel (port 8443), AWS Lambda, custom Alexa skill with PIN verification all working; skill submitted for certification
- **Installed n8n** — workflow automation running in Docker; accessible at `https://amelai.tail926601.ts.net:5678`; full setup guide at `N8N_Setup.md`
- **Confirmed PCIe Gen 4 under load** — Gen 1 at idle is normal ASPM idle power management; verified Gen 4 (16GT/s) on both GPUs during active workload
- **Recovered from `pci=nomsi` boot failure** — parameter disables MSI for NVMe controllers, preventing boot; system restored and GRUB left empty
- **Fixed PCIe Gen 1 fallback** — both RTX 3090s now at Gen 4 (16GT/s); root cause was `pcie_aspm=off` blocking PCIe link equalization
- Removed `pcie_aspm=off` kernel parameter — was blocking Gen 4 link training; safe to remove as igc is blacklisted
- Updated BIOS to 2103 (from 2102) — no change to PCIe Gen issue
- Switched primary NIC to Aquantia AQC113 10GbE — Intel igc I226-V blacklisted after second PCIe crash
- Fixed system timezone to `Europe/London`
- Mounted Synology DS920+ `MyDocs` share permanently at `/docs` via NFS

## Pending / Next Actions

- [ ] **Commit `androidApp/gradle.properties`** to git and push
- [ ] Test web search in AI Voice app ("what's in the news today?")
- [ ] Consider `--keep-alive 0` Ollama flag to prevent auto-reload after VRAM pressure incidents
- [ ] **Recreate `comfyui` container** — `docker stop comfyui && docker rm comfyui` then run updated command from `Docker.md` (includes `--reserve-vram 3` and correct workflows path)
- [ ] Verify Tailscale Serve: `sudo tailscale serve status` — confirm `8189 → localhost:18189`; add if missing
- [ ] Test `https://amelai.tail926601.ts.net:8189` after rebuild
- [ ] Save ReActor face swap workflow from `Temp.txt` to `/docs/Projects/Claude Code Shared/Workflows/FaceSwap.json`
- [ ] Await Alexa skill certification approval; test on real Echo device once approved
- [ ] Complete n8n first-login owner account setup
- [ ] Store n8n encryption key in password manager
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
| `N8N_Setup.md` | n8n workflow automation — Docker setup, Tailscale config, update and backup procedures |
| `androidApp/` | AI Voice Android app — native Kotlin voice client for Open WebUI over Tailscale |
| `STT_Voice_Input.md` | Speech-to-text setup guide — server (amelai) and Windows client |
| `STT_New_Machine_Guide.md` | STT client install guide for additional Windows machines on the tailnet |
| `stt/` | STT source files — `stt_server.py`, `stt_client.py`, service unit, requirements |
| `wol/WOL_Setup.md` | Alexa Wake-on-LAN setup — full end-to-end guide including Lambda, Tailscale Funnel, skill config |
| `CLAUDE.md` | Project-specific guidance for this directory |

## Hardware

- CPU: AMD Ryzen 9 7900X
- GPU: 2× ASUS TUF RTX 3090 24GB (48GB total VRAM)
- RAM: 64GB DDR5
- Storage: 2× Samsung 9100 Pro 2TB NVMe
- OS: Ubuntu 24.04 LTS Server
