# Session Log — NewPC Project

---

## Session 2026-03-17

### Summary
Created a comprehensive AI model training and LoRA creation guide covering two distinct workflows: FLUX.1 Dev character LoRA training using ai-toolkit, and LLM fine-tuning for a custom knowledge chatbot using Unsloth. Updated CLAUDE.md to reference `Final_Build.md` and `Software_Setup.md` as the authoritative source for system specifications.

### Work Completed
- Created `Model_and_LoRA_Creation.md` — full guide covering both image LoRA and LLM fine-tuning workflows
- Updated `CLAUDE.md` — added System Specifications Reference section pointing to `Final_Build.md` and `Software_Setup.md`
- Used `gemini-it-security-researcher` agent to research and verify all tool recommendations, parameters, and community guidance as of March 2026

### Files Changed
- `it/NewPC/Model_and_LoRA_Creation.md` — **created** — comprehensive training guide (both workflows)
- `it/NewPC/CLAUDE.md` — added system spec reference section with quick-reference hardware/software summary

### Key Decisions
- **ai-toolkit (ostris) selected over kohya_ss and SimpleTuner** for FLUX LoRA training: purpose-built for FLUX, has a verified 24GB config, widest community adoption. SimpleTuner noted as a solid alternative.
- **Unsloth selected over LLaMA-Factory/axolotl** for LLM fine-tuning: easiest install, 70% VRAM reduction, built-in GGUF/Ollama export, NVLink multi-GPU support explicitly documented.
- **Start with Qwen3.5:9B for chatbot fine-tuning**: fits single GPU, 256K context, fast iteration. Upgrade to 27B if quality is insufficient.
- **Training parameters verified from official sources**: ai-toolkit 24GB config (rank 16, lr 1e-4, 2000 steps, bf16, adamw8bit); Unsloth QLoRA config (rank 16, lr 2e-4, 3 epochs, bf16).
- **Civitai blocked in UK** (Online Safety Act) — community resources redirected to r/StableDiffusion and HuggingFace.
- **Photo selection guidance**: 20–30 varied images recommended; multiple angles, lighting, clothing, backgrounds; exclude group photos, occlusion, burst shots, screenshots from video.

### Reference Documents
- `it/NewPC/Model_and_LoRA_Creation.md` — the new guide
- `it/NewPC/Final_Build.md` — hardware specification (now referenced in CLAUDE.md)
- `it/NewPC/Software_Setup.md` — software stack (now referenced in CLAUDE.md)

### Next Actions
- [ ] Curate photo dataset for character LoRA (20–30 images, varied angles/lighting/backgrounds)
- [ ] Install ai-toolkit on the server: `git clone https://github.com/ostris/ai-toolkit.git`
- [ ] Create training dataset (JSONL) for LLM chatbot from source documents
- [ ] Install Unsloth: `pip install "unsloth[colab-new] @ git+https://github.com/unslothai/unsloth.git"`
- [ ] Apply dual `-p` binding to ComfyUI containers (pending from previous session)
- [ ] Set static DHCP reservation on router for `192.168.1.192` if not already done

---

## Session 2026-03-14

### Summary
Created a comprehensive Tailscale networking guide and updated documentation to reflect the dual-binding Docker port strategy (loopback for Tailscale serve, LAN IP for local network access). Worked through real troubleshooting of Tailscale serve configuration, diagnosing and fixing incorrect port mappings. Confirmed and documented final port assignments for all services.

### Work Completed
- Created `Tailscale.md` — full guide covering installation, commands, port forwarding, troubleshooting, and securing services
- Updated `HuggingFace.md` — added instructions for sharing models with Amelia's instance via hard links
- Updated `Software_Setup.md` — dual `-p` bindings on all docker run commands, updated service URLs table, updated access sections
- Worked through live Tailscale serve troubleshooting (wrong port in `off` command, wrong protocol in browser, Docker bypassing Tailscale serve)
- Confirmed final port assignments for all services
- Explained Docker `-p` flag in depth (IP binding, host port vs container port, container port isolation)
- Evaluated and rejected path-based Tailscale routing (ComfyUI has no base URL support)

### Files Changed
- `it/NewPC/Tailscale.md` — **created** — comprehensive Tailscale guide
- `it/NewPC/HuggingFace.md` — added Amelia model sharing section (hard link commands)
- `it/NewPC/Software_Setup.md` — dual `-p` bindings, updated service URLs table and access sections throughout

### Key Decisions
- **Dual Docker binding over iptables blocking**: Use `-p 127.0.0.1:HOST:CONTAINER` for Tailscale serve + `-p 192.168.1.192:PORT:CONTAINER` for LAN access. Cleaner than blocking ports with iptables rules.
- **Port-based Tailscale routing retained**: Path-based routing (`/steve`, `/amelia`) was evaluated but rejected — ComfyUI has no base URL support so the setup would be inconsistent. Port numbers kept for all services.
- **Container port cannot be changed arbitrarily**: The second number in `-p` must match what the application listens on inside the container (e.g. ComfyUI uses 8188, not 80).
- **Static LAN IP required**: Dual binding only reliable with a fixed LAN IP — DHCP reservation on router recommended.

### Confirmed Port Assignments
| Service | Local network | Tailscale |
|---|---|---|
| Open WebUI | `http://192.168.1.192:3000` | `https://amelai.tail926601.ts.net` |
| ComfyUI (Steve) | `http://192.168.1.192:8189` | `https://amelai.tail926601.ts.net:8189` |
| ComfyUI (Amelia) | `http://192.168.1.192:8188` | `https://amelai.tail926601.ts.net:8188` |
| FileBrowser | `http://192.168.1.192:8087` | `https://amelai.tail926601.ts.net:8087` |

### Next Actions
- [ ] Apply dual `-p` binding to ComfyUI and ComfyUI-Amelia docker run commands on the server
- [ ] Verify `sudo ss -tlnup` shows correct bindings for all services after changes
- [ ] Consider setting static DHCP reservation on router for `192.168.1.192` if not already done

---
