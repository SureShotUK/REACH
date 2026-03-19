# Session Log — NewPC Project

---

## Session 2026-03-19 (Evening)

### Summary
Diagnosed and resolved persistent SIGKILL failures blocking Qwen-Image-Edit LoRA training. Root cause identified as checkpoint save causing a temporary memory spike from ~46 GB to ~87 GB (ZeRO-3 gathers all 16-bit weights into a second buffer at epoch end), exceeding the 62 GB system RAM. Fixed by increasing swap to 32 GB and disabling pinned memory. Training pipeline confirmed working end-to-end with `epoch-0.safetensors` produced. Full 5-epoch training now running.

### Work Completed
- Identified wrong virtual environment (`hf-env`) as initial cause — must use `diffsynth-env`
- Confirmed Stage 1 cache intact and Stage 2 script correct — not a script issue
- Captured detailed OOM data via `dmesg` — confirmed SIGKILL was kernel OOM killer at ~60 GB RSS
- Diagnosed `stage3_gather_16bit_weights_on_model_save: true` as root cause — temporary memory doubling during epoch-end checkpoint save
- Proved training steps complete successfully (2/2 shown in progress bar) — only save was failing
- Confirmed `stage3_gather_16bit_weights_on_model_save: false` breaks DiffSynth-Studio saving (accelerator.get_state_dict ValueError) — cannot disable
- Increased swap from 8 GB to 32 GB (`/swap.img`) to absorb save spike
- Set `pin_memory: false` in `ds_z3_cpuoffload.json` to allow OS to use swap during save
- Added `export TORCH_CUDA_ARCH_LIST="8.6"` to training scripts
- Set `--dataset_num_workers 0` to reduce RAM pressure
- Confirmed pipeline working: `test_stage2.sh` (1 image, 1 epoch) produced `epoch-0.safetensors`
- Restored full parameters (`--dataset_repeat 50 --num_epochs 5`) and started training in tmux session `lora-training`
- Created `LoRAMemoryFixes.md` — complete diagnosis, all required fixes, and speed optimisation guide
- Created `TMUX.md` and `Docker.md` reference guides earlier in session

### Files Changed
- `it/NewPC/LoRAMemoryFixes.md` — **created** — root cause analysis, all required config changes, confirmed working script, speed optimisations, diagnostics reference
- Server file: `ds_z3_cpuoffload.json` — `pin_memory` changed to `false` for both offload sections
- Server file: `stage2_train.sh` — added `TORCH_CUDA_ARCH_LIST="8.6"`, `--dataset_num_workers 0`
- Server file: `test_stage2.sh` — same changes; swap increased to 32 GB (`/swap.img`)

### Key Decisions
- **Root cause is checkpoint save OOM, not training OOM** — ZeRO-3 `stage3_gather_16bit_weights_on_model_save: true` doubles memory (~41 GB → ~82 GB) at epoch end. Cannot disable — DiffSynth-Studio requires it.
- **Swap increase is the correct fix** — 32 GB swap gives 94 GB virtual memory total, absorbing the ~87 GB peak during save
- **`pin_memory: false` is required alongside swap** — pinned memory cannot be swapped to disk; both changes together are needed
- **Speed optimisations deferred** — `pin_memory: true` and `--dataset_num_workers 2` can be restored once full training succeeds; documented with test procedure in `LoRAMemoryFixes.md`
- **`TORCH_CUDA_ARCH_LIST="8.6"`** — without this, DeepSpeed compiles CUDA extensions for all GPU architectures, adding ~10–14 GB temporary RAM during startup

### Next Actions
- [ ] Confirm full training completes — check `ls ~/DiffSynth-Studio/models/train/my_character_lora/` for `epoch-0.safetensors` through `epoch-4.safetensors`
- [ ] Restart Docker + Ollama after training: `docker start comfyui comfyui-amelia && sudo systemctl start ollama`
- [ ] Test each epoch LoRA in ComfyUI — copy to `/mnt/models/comfyui/loras/`
- [ ] Try speed optimisations from `LoRAMemoryFixes.md` — restore `pin_memory: true` then `--dataset_num_workers 2` incrementally, testing after each change
- [ ] Update `QwenImageEditTrainingLoRA.md` with memory fix requirements
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 to replace obsolete FP8+DDP approach

---

## Session 2026-03-19

### Summary
Confirmed Stage 2 LoRA training had not completed (tmux session lost, output directory absent). Diagnosed the situation, confirmed Stage 1 cache intact, and restarted Stage 2 training. Created two new reference documents: `TMUX.md` (general tmux guide) and `Docker.md` (Docker administration guide including all service `docker run` commands).

### Work Completed
- Diagnosed interrupted Stage 2 training: `my_character_lora/` directory absent, tmux session gone
- Confirmed Stage 1 cache (`my_character_lora_cache/0` and `/1`, 22 `.pth` files each) intact and valid
- Verified `stage2_train.sh` script still present with correct parameters
- Restarted Stage 2 training in a new tmux session (`lora-training`)
- Created `TMUX.md` — tmux reference guide covering sessions, windows, panes, detach/attach, scroll mode
- Created `Docker.md` — Docker administration guide with all service `docker run` commands, common commands, port map, and SSH file access explanation

### Files Changed
- `it/NewPC/TMUX.md` — **created** — tmux guide: concepts, prefix key, sessions, windows, panes, scrolling, practical workflows, quick reference card
- `it/NewPC/Docker.md` — **created** — Docker guide: core concepts, common commands, full `docker run` commands for Open WebUI / ComfyUI (Steve) / ComfyUI (Amelia) / FileBrowser / SearXNG, port map, Tailscale Serve config, SSH file access explanation

### Key Decisions
- Stage 2 script (`stage2_train.sh`) was intact from previous session — no recreation needed; Stage 1 cache was also intact so Stage 1 did not need to be re-run
- Docker.md placed in `it/NewPC/` (not `it/`) because all `docker run` commands are specific to this server's IP addresses, volume paths, and port assignments

### Next Actions
- [ ] Confirm Stage 2 training completes — check `ls ~/DiffSynth-Studio/models/train/my_character_lora/` for `epoch-0.safetensors` through `epoch-4.safetensors`
- [ ] Restart Docker containers after training: `docker start comfyui comfyui-amelia && sudo systemctl start ollama`
- [ ] Test each epoch LoRA in ComfyUI — copy from `~/DiffSynth-Studio/models/train/my_character_lora/` to `/mnt/models/comfyui/loras/`
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 to replace obsolete FP8+DDP approach with ZeRO-3 method

---

## Session 2026-03-18 (Evening)

### Summary
Resolved persistent CUDA OOM errors blocking Qwen-Image-Edit LoRA training, diagnosing the root cause as the FP8+DDP approach being fundamentally incompatible with 2×24 GB hardware. Migrated to DeepSpeed ZeRO-3 CPU offload, which successfully started training at ~13–15 GB VRAM. Created a comprehensive standalone training guide `QwenImageEditTrainingLoRA.md` documenting the verified working procedure.

### Work Completed
- Resolved tmux `duplicate session` error (kill-session before creating new)
- Fixed image permissions error (Stage 1 `PermissionError: [Errno 13]`) with `chmod -R 644`
- Identified ComfyUI instances as Docker containers (not plain Python processes as previously documented) — `docker stop` required, not `pkill`
- Diagnosed FP8+DDP as fundamentally unworkable: transformer fills ~23.2 GB per GPU under DDP regardless of `--fp8_models`, `--lora_rank`, or `--max_pixels` settings
- Researched DiffSynth-Studio GitHub: found DeepSpeed ZeRO-3 support was merged March 17, 2026 (one day before previous session)
- Implemented ZeRO-3 CPU offload with `num_processes: 1` — model offloaded to 64 GB CPU RAM, GPU 0 holds only active layer parameters
- Confirmed training running successfully: `16/2200` steps, ~13–15 GB VRAM, ~46 GB RAM used
- Created `QwenImageEditTrainingLoRA.md` — complete standalone guide covering the full verified procedure

### Files Changed
- `it/NewPC/QwenImageEditTrainingLoRA.md` — **created** — comprehensive LoRA training guide with all verified commands, metadata.json format, config files, monitoring, and troubleshooting
- `it/NewPC/Model_and_LoRA_Creation.md` — updated `--max_pixels` from 1048576 to 786432, then 524288, then 262144; updated `--lora_rank` from 16 to 8; updated `--lora_rank` table note; hardware requirements section notes need updating
- `it/NewPC/Temp.txt` — used throughout session to pass commands safely to server

### Key Decisions
- **ComfyUI IS Docker**: Previous session documented ComfyUI as "plain Python processes" — incorrect. Both instances are Docker containers. Use `docker stop comfyui comfyui-amelia` before training, not `sudo pkill -f "ComfyUI/main.py"`.
- **FP8+DDP is fundamentally broken on 2×24 GB**: `--fp8_models "transformer"` with `--num_processes 2` puts a full ~23.2 GB FP8 model on EACH GPU. No amount of `--lora_rank`, `--max_pixels`, or `PYTORCH_CUDA_ALLOC_CONF` tuning can overcome this. This invalidates the approach documented in the previous session.
- **ZeRO-3 `num_processes: 1` beats `num_processes: 2`**: With CPU offload, a single process uses less VRAM (no inter-GPU AllGather communication buffers). The official DiffSynth-Studio config always used 1 process; the 2-process variant we tried caused higher VRAM usage.
- **`--use_gradient_checkpointing` IS needed with ZeRO-3**: Removing it (based on an incorrect "incompatible" warning from research) caused activation tensors to fill all VRAM. The official low_vram script includes it — it's required.
- **`--max_pixels 262144` required**: At higher resolutions, attention activation tensors (not model parameters) fill VRAM even with ZeRO-3. Stage 1 and Stage 2 must use the same value.
- **Stage 1 must match Stage 2 `--max_pixels`**: Cached latent dimensions are fixed at Stage 1 generation time. Changing `--max_pixels` in Stage 2 alone is insufficient; Stage 1 cache must be deleted and regenerated.

### Next Actions
- [ ] Confirm Stage 2 training completes (currently running, ~4–5 hours at 7.3 s/step)
- [ ] Test each epoch LoRA in ComfyUI — copy from `~/DiffSynth-Studio/models/train/my_character_lora/` to `/mnt/models/comfyui/loras/`
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 section to reflect the ZeRO-3 approach (currently documents the obsolete FP8+DDP method)
- [ ] Update `CLAUDE.md` to correct the ComfyUI process management note (it IS Docker)

---

## Session 2026-03-19

### Summary
Verified Workflow 3 (Qwen-Image-Edit LoRA training) against the actual DiffSynth-Studio source code and official scripts. Found and corrected three significant errors in the guide that would have prevented training from working. All corrections applied to `Model_and_LoRA_Creation.md`.

### Work Completed
- Audited training command against live DiffSynth-Studio source on GitHub
- Corrected Step 4: replaced single-command approach with two-stage split training (`stage1_cache.sh` + `stage2_train.sh`)
- Added `--fp8_models "transformer"` to Stage 2 (essential for fitting on dual 24 GB GPUs)
- Fixed Step 5 output path: was wrong path and wrong filename format
- Fixed VRAM OOM troubleshooting: removed invalid `SPLIT_SCHEME="on"` parameter, replaced with correct `--task` flag guidance
- Updated Hardware requirements section to explain why both GPUs are required
- Updated Qwen-Image-Edit minimum viable checklist

### Files Changed
- `it/NewPC/Model_and_LoRA_Creation.md` — Step 4 rewritten (two-stage split training), Step 5 path corrected, troubleshooting section corrected, hardware requirements updated, checklist updated

### Key Decisions
- **Single-command approach was fundamentally broken**: 57.7 GB BF16 model cannot fit on 2×24 GB under standard DDP regardless of `--mixed_precision bf16` or `--initialize_model_on_cpu`
- **`--mixed_precision bf16` does NOT load fp8 weights**: it only controls compute precision; session notes from previous session were incorrect on this point
- **Two-stage split training is the correct approach**: confirmed from official `Qwen-Image-LoRA.sh` split training script in DiffSynth-Studio
- **`--fp8_models "transformer"`** quantises the transformer from ~41 GB BF16 to ~20 GB FP8 — the only way to fit it per-GPU under DDP in Stage 2
- **Output files are `epoch-N.safetensors`**: flat directory, one file per epoch; NOT `checkpoint-XXXX/pytorch_lora_weights.safetensors` (that is HuggingFace diffusers format, not DiffSynth-Studio)
- **`SPLIT_SCHEME` does not exist**: the old `SPLIT_SCHEME="on"` parameter is from an older project; replaced by `--task "sft:data_process"` / `--task "sft:train"` in current DiffSynth-Studio

### Next Actions
- [ ] Run Stage 1 (`stage1_cache.sh`) — should complete without OOM (~8-9 GB per GPU, text encoder + VAE only)
- [ ] Run Stage 2 (`stage2_train.sh`) — monitor VRAM (~20-23 GB per GPU expected)
- [ ] Monitor training loss; stop early (e.g. epoch 3) if overfitting is suspected
- [ ] Copy best epoch LoRA to `/mnt/models/comfyui/loras/` and test in ComfyUI
- [ ] Report back if Stage 1 or Stage 2 errors — note the exact error message

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
- **ComfyUI processes must be stopped before training**: both instances run as Docker containers; use `docker stop comfyui comfyui-amelia` before training (**correction**: previously documented as plain Python processes — this was wrong, confirmed Docker in session 2026-03-18 evening)
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
