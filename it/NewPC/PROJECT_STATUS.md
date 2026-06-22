# Project Status — NewPC AI Server

**Last Updated**: 2026-06-22 (Company Name Lookup workflow)

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
| pdf-to-image | `pdf-to-image` | `http://192.168.1.192:8086` | internal only | Running |

## Active Work Areas

- **n8n Company Name Lookup workflow** — `CompanyLookup_Workflow.json` working for multi-company processing and chat response; CSV attachment fix applied (Outlook node replaced with direct Graph API HTTP Request); needs test to confirm attachment now arrives
- **n8n Customer Profiler workflow** — `CustomerProfilerWorkingEmail.json` working for PDF and iXBRL paths; needs final test confirming net assets extraction from iXBRL
- **n8n Lead Generation workflow** — `LeadGen_Workflow.json` built and ready to import; needs "Companies House API" Basic Auth credential created in n8n before first test run
- **ComfyUI (Steve) rebuild required** — run command updated with `--reserve-vram 3` and correct workflows volume path; container must be recreated
- **Alexa WOL skill** — submitted for Amazon certification; awaiting approval (3-5 business days)
- **ReActor face swap workflow** — JSON in `Temp.txt`; needs saving to Workflows folder after container rebuild

## Recently Completed

- **Company Name Lookup workflow** — 11-node importable workflow (`CompanyLookup_Workflow.json`); accepts names one-per-line or comma-separated; CH search + Amelai confidence scoring + colour-coded email with CSV attachment + markdown chat response; multiple-company bug fixed (Code node batch-mode semantics); CSV attachment fixed (Graph API format)
- **Customer Profiler iXBRL extraction** — dual-path architecture: PDF → pdf-to-image → qwen2.5vl:7b vision; iXBRL → text download → qwen3.5:27b (think:false); format detected from S3 URL before download; dual targeted extracts for P&L + balance sheet sections; `remove` command; profit after tax; parentheses negatives
- **n8n Lead Generation workflow built** — 17-node importable workflow (`it/NewPC/n8n/LeadGen_Workflow.json`); SIC search + single company modes; CH officers + filing + PDF + SearXNG + Ollama qwen3.6:27b + HTML email digest to steve@portland-fuel.co.uk; full usage guide in `LeadGen_Workflow_Design.md`
- **STT client/server fixes** — four client bugs fixed (keyboard hook re-entrancy stealing Ctrl/Esc, spurious toggle, double F9 handler, blocking sleep in async); server now lazy-loads Whisper on first speech and unloads from VRAM after 15 min idle; full documentation at `stt/STT_Documentation.md`
- **AI Voice Android app v3** — dark theme toggle; GPS location injected into system prompt (weather/restaurant/car park queries work); location permission requested on first launch
- **SearXNG Tailscale fix** — container rebounded to `127.0.0.1:18080` + `192.168.1.192:8080`; Tailscale serve `--bg` rule added; ACL rule added for port 8080
- **AI Voice Android app v2** — SearXNG web search added (app queries directly, injects results as context); concise system prompt with no special characters; speech corrections map (Weatherby→Wetherby); settings labels enlarged; SearXNG URL setting added; default model `qwen3.6:27b`; full documentation at `AI_Voice_App.md`
- **AI Voice Android app v1** — built and sideloaded to S24 Ultra; voice input → Open WebUI → TTS response working over Tailscale
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

- [ ] **Company Lookup: verify CSV attachment** — import updated JSON, reselect `MyHotmailEmail` credential in "Send Results" node if needed, test that `company_lookup_results.csv` now arrives
- [ ] **Company Lookup: medium/low confidence** — investigate why Amelai only returns high-confidence results; may need prompt tuning or model change
- [ ] **Customer Profiler: final iXBRL test** — confirm net assets extracted for Centrebus (03872099) after dual-section extraction fix
- [ ] **Customer Profiler: bulk run** — profile a broader set of companies to validate robustness across different iXBRL and PDF account formats
- [ ] **Import and test lead gen workflow** — create "Companies House API" Basic Auth credential in n8n, import `it/NewPC/n8n/LeadGen_Workflow.json`, test with 1–2 known companies
- [ ] **Deploy STT server fix** — `scp stt/stt_server.py steve@amelai.tail926601.ts.net:/opt/stt/stt_server.py` then `sudo systemctl restart stt_server`
- [ ] **Deploy STT client fix** — copy `stt/stt_client.py` to Steve's Windows 11 PC and restart via `restart_stt.bat`
- [ ] Verify STT VRAM freed after 15 min idle: `nvidia-smi --query-gpu=memory.used --format=csv`
- [ ] **Tailscale ACL** — replace hardcoded device IPs with Tailscale tags to prevent stale-IP breakage when devices are re-added
- [ ] **Rebuild and reinstall AI Voice APK** with dark theme + location changes
- [ ] Grant location permission on first launch after reinstall
- [ ] Test location-based queries: "weather here", "nearest Italian restaurant", "nearest car park"
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
| `n8n/N8N_Setup.md` | n8n workflow automation — Docker setup, Tailscale config, update and backup procedures |
| `n8n/CompanyLookup_Workflow.json` | Importable Company Name Lookup workflow — 11 nodes, pfl-company-lookup webhook |
| `n8n/Company_Lookup.md` | User documentation for Company Name Lookup — URL, input formats, output description |
| `n8n/LeadGen_Workflow.json` | Importable lead generation workflow — ready for n8n import |
| `n8n/LeadGen_Workflow_Design.md` | Lead gen design + usage guide — input syntax, examples, scoring, run times |
| `androidApp/` | AI Voice Android app — native Kotlin voice client for Open WebUI over Tailscale |
| `AI_Voice_App.md` | Full documentation — build guide, update workflow, settings, troubleshooting |
| `STT_Voice_Input.md` | Speech-to-text setup guide — server (amelai) and Windows client |
| `STT_New_Machine_Guide.md` | STT client install guide for additional Windows machines on the tailnet |
| `stt/` | STT source files — `stt_server.py`, `stt_client.py`, service unit, requirements |
| `stt/STT_Documentation.md` | Comprehensive STT docs — server setup, client setup, VRAM lifecycle, update procedures, troubleshooting |
| `wol/WOL_Setup.md` | Alexa Wake-on-LAN setup — full end-to-end guide including Lambda, Tailscale Funnel, skill config |
| `CLAUDE.md` | Project-specific guidance for this directory |

## Hardware

- CPU: AMD Ryzen 9 7900X
- GPU: 2× ASUS TUF RTX 3090 24GB (48GB total VRAM)
- RAM: 64GB DDR5
- Storage: 2× Samsung 9100 Pro 2TB NVMe
- OS: Ubuntu 24.04 LTS Server
