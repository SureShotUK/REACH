# Session Log — NewPC Project

---

## Session 2026-03-18

### Summary
Extended `Model_and_LoRA_Creation.md` with a full Workflow 3 section for Qwen-Image-Edit character LoRA training using DiffSynth-Studio, and created `MultiFileModels.md` explaining the HuggingFace diffusers multi-file model format. The session involved live end-to-end training attempts, diagnosing and resolving multiple errors including missing modules, wrong metadata format, CUDA OOM from ComfyUI processes, and ultimately identifying that `accelerate launch` was not distributing the model across both GPUs. Adding `--num_processes 2 --mixed_precision bf16` resolved the OOM issue by loading a smaller model variant (~20GB across 2 GPUs rather than ~50GB on GPU 0 alone). Training was confirmed downloading correctly by end of session.

### Work Completed
- Added **Workflow 3** to `Model_and_LoRA_Creation.md` — Qwen-Image-Edit character LoRA training via DiffSynth-Studio
- Created `MultiFileModels.md` — standalone document explaining HuggingFace diffusers multi-file model format, Qwen-Image-Edit-2511 structure, three ComfyUI usage options, and DiffSynth-Studio model loading
- Resolved multiple training errors end-to-end (see Key Decisions for full list)
- Updated Step 4 training script with working multi-GPU parameters
- Updated documentation with correct download size (~20GB, 4×5GB files)

### Files Changed
- `it/NewPC/Model_and_LoRA_Creation.md` — added full Workflow 3 (Qwen-Image-Edit LoRA); updated Step 4 training command with `--num_processes 2 --mixed_precision bf16`, `--initialize_model_on_cpu`, `PYTORCH_CUDA_ALLOC_CONF`, reduced `--max_pixels`/`--lora_rank`/`--dataset_num_workers`; added pre-flight GPU check; updated parameters table and download size estimate
- `it/NewPC/MultiFileModels.md` — **created** — standalone guide for HuggingFace diffusers format and Qwen-Image-Edit-2511 multi-file structure

### Key Decisions
- **Qwen-Image-Edit is MMDiT not FLUX**: completely different architecture (no kohya/ai-toolkit); requires DiffSynth-Studio (official Alibaba training framework)
- **DiffSynth-Studio uses ModelScope cache** (`~/.cache/modelscope/`), not HuggingFace cache — models are downloaded automatically on first training run
- **metadata.json not .txt captions**: DiffSynth-Studio requires a JSON metadata file; `.txt` caption files alongside images are not used
- **`hf` not `huggingface-cli`**: huggingface_hub 1.x renamed the CLI to `hf`; requires venv activation; cannot use sudo
- **Two-repo model spec**: transformer from `Qwen/Qwen-Image-Edit-2511`; text encoder + VAE from `Qwen/Qwen-Image` (base repo)
- **`--num_processes 2 --mixed_precision bf16` is essential**: without `--num_processes 2`, accelerate defaults to single-GPU and tries to load the full model onto GPU 0 (OOM). With both flags, DiffSynth-Studio loads a smaller fp8/quantised model variant (~20GB total, ~10GB per GPU)
- **`--initialize_model_on_cpu`**: loads weights to CPU first before distributing to GPUs, preventing a VRAM spike on GPU 0 during model load
- **ComfyUI processes must be stopped before training**: both instances run as plain Python processes (`python3 ./ComfyUI/main.py`), not Docker; kill with `sudo kill <PID>` before training
- **Caption strategy**: use `<name> person` not `<name> woman` as the trigger word format to avoid gender bias in the character representation

### Reference Documents
- `it/NewPC/Model_and_LoRA_Creation.md` — updated training guide (now 3 workflows)
- `it/NewPC/MultiFileModels.md` — new standalone multi-file model reference

### Next Actions
- [ ] Confirm training completes successfully (was downloading correctly at end of session)
- [ ] Monitor training loss values and GPU VRAM usage during run (`nvtop`, `nvidia-smi`)
- [ ] Once LoRA is generated, copy to `/mnt/models/comfyui/loras/` and test in ComfyUI
- [ ] Update `Model_and_LoRA_Creation.md` Step 5 with correct LoRA output path once confirmed
- [ ] Curate photo dataset for character LoRA (20–30 images) if not already done
- [ ] Install ai-toolkit on server for FLUX LoRA training (Workflow 1)

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
