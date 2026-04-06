# Changelog — NewPC Project

---

## [Unreleased] - 2026-04-06 (2)

### Fixed
- Intel igc I226-V NIC recurring PCIe crash — `pcie_aspm=off` confirmed insufficient; permanent fix applied: primary connection moved to Aquantia AQC113 10GbE NIC (`ethernet2_5g`, 192.168.1.192), igc driver blacklisted via `/etc/modprobe.d/blacklist-igc.conf`
- `netplan apply` failing on `netplan-wpa-wlp11s0.service` — removed broken WiFi section (no password configured) from `/etc/netplan/*.yaml`
- System timezone incorrect (was UTC) — set to `Europe/London` via `timedatectl`

### Documentation
- `Linux_Troubleshooting.md` — Issue 2 rewritten: two incidents documented, `pcie_aspm=off` failure noted, permanent Aquantia switchover documented with full netplan config
- `Linux_Troubleshooting.md` — Issue 4 added: 90-second boot delay caused by WiFi adapter `wlp11s0`; fix applied (WiFi removed from netplan); verification pending on next reboot

---

## [Unreleased] - 2026-04-06

### Added
- NFS mount for Synology DS920+ `MyDocs` share at `/docs` — entry added to `/etc/fstab` (`192.168.1.216:/volume2/MyDocs /docs nfs defaults,_netdev,nofail 0 0`)

### Removed
- `cifs-utils` and `smbclient` — installed during SMB troubleshooting, no longer needed

### Documentation
- `CLAUDE.md` — added "Linux Session Housekeeping" section: remove temp packages, run updates, clean up credentials/temp files at end of each Linux session

---

## [Unreleased] - 2026-04-05 (2)

### Fixed
- ComfyUI (Steve) OOM error on first generation with Qwen-Rapid-AIO-NSFW-v23 — added `--reserve-vram 3` to `CLI_ARGS`; reserves 3GB VRAM headroom to prevent fragmentation-induced allocation failures
- ComfyUI (Steve) Tailscale access broken after container rebuild — loopback port was `127.0.0.1:8189` (wrong) corrected to `127.0.0.1:18189` in both `ComfyUI.md` and active container

### Added
- `.gitignore` — excludes `.NET bin/obj`, `*.log`, `*.oft`, `*.bin`, `*_files/`, `it/troubleshooting/Backups_*/`, and named temp files
- FileBrowser now mounts ComfyUI input and output folders: `comfyui-input/`, `comfyui-output/`, `comfyui-amelia-input/`, `comfyui-amelia-output/`
- 139 previously untracked files committed: Canada regulatory docs, REACH HVO, IUCLID, hseea subdirs, insurance, ZeroTrust, postgres-security, wsl-postgresql-setup, IT security docs, NewPC workflows/configs, OutlookTemplateCleaner source

### Documentation
- `ComfyUI.md` — corrected docker run command (loopback port + `--reserve-vram 3`)
- `Docker.md` — FileBrowser command updated with all four ComfyUI volume mounts
- `CLAUDE.md` (shared) — added "Warnings Before Instructions" principle: warnings must appear before commands, not after

---

## [Unreleased] - 2026-04-05

### Added
- `.gitattributes` — repo-root line ending normalisation: LF stored in git, CRLF on Windows checkout, LF on Linux; binary files exempt
- `.claude/commands/sync-files.md` — `/sync-files` slash command for bidirectional cross-platform git sync; handles ahead/behind/diverged cases; uses rebase so local changes win on conflict

### Documentation
- `CLAUDE.md` updated with: FLUX.1 dev fp8 self-contained model structure, VRAM/Ollama contention reference table, Docker dual-port binding strategy (`1XXXX` loopback + LAN IP pattern), bash special character password handling, `Temp.txt` file delivery pattern

---

## [Unreleased] - 2026-03-24 (2)

### Added
- `FileWriter.py` — Open WebUI Tool class; gives models genuine filesystem write capability to `/mnt/uploads` (host: `/home/steve/rag-output`); includes path traversal protection

### Documentation
- Confirmed `/mnt/uploads` bind mount already present in Open WebUI container from prior setup — maps to `/home/steve/rag-output`
- Documented symlink behaviour: `it/.claude/commands/end-session.md` is a symlink; actual file is at `terminai/.claude/commands/end-session.md`

---

## [Unreleased] - 2026-03-24

### Added
- `Linux_Troubleshooting.md` — server reliability reference guide covering:
  - Post-crash log analysis using journalctl (boot-scoped, OOM, kernel errors, NVIDIA XID codes)
  - Issue 2: Intel igc NIC PCIe ASPM fix — `pcie_aspm=off` kernel boot parameter
  - Issue 3: Ollama OOM kills — ComfyUI VRAM contention diagnosis and fix (bookmarklet + cron)
  - VRAM budget reference table for amelai models

### Fixed
- Intel igc NIC (`ethernet10g`) dropping off PCIe bus — `pcie_aspm=off` added to `GRUB_CMDLINE_LINUX_DEFAULT`; PCIe ASPM disabled system-wide on kernel 6.17
- Ollama OOM kill loop — ComfyUI VRAM free bookmarklet created; nightly 2am cron restarts added for both ComfyUI containers

### Documentation
- Documented that `igc` module has no `aspm_disable` parameter on kernel 6.17 — GRUB parameter is the only reliable fix
- Documented ComfyUI `/free` endpoint behaviour (empty response body — do not use `.json()`, use `.ok` status check)

---

## [Unreleased] - 2026-03-23

### Added
- `SearXNG_Fix.md` — MCP web search troubleshooting log covering two root causes (missing Tailscale ACL port, stored auth credential), architecture overview, and quick reference commands

### Fixed
- Tailscale ACL updated to include port 3001 — MCP server now accessible from all tailnet devices
- Stored Anthropic auth credential cleared (`claude auth logout`) — Ollama routing now works correctly
- Open WebUI Ollama URL corrected from `https://` to `http://100.79.83.113:11434`
- `hf-env` conda environment no longer auto-activates on SSH login to amelai
- MCP re-registered as user-scoped — available in all Claude Code projects, not just NewPC

---

## [Unreleased] - 2026-03-22

### Added
- `New_PC_Builds.md` — personal Windows 11 PC build guide covering:
  - Existing components (MSI MAG X870E Tomahawk WIFI, Samsung 9100 Pro 2TB, Viper Venom DDR5 32GB)
  - Chosen build: Ryzen 7 9800X3D + RTX 5070 Ti 16GB + Corsair 4000D Airflow + be quiet! Power Zone 2 1000W + Arctic Liquid Freezer III 360 (~£1,395 total)
  - Full component research with alternatives and rejected options documented
  - Video editing via AI PC remote encoding section (DaVinci Resolve render queue, Adobe Media Encoder, Tailscale)
  - Compatibility summary and pricing notes

---

## [Unreleased] - 2026-03-19 (Late Evening)

### Changed
- `QwenImageEditTrainingLoRA.md` — added **RAM Limitation Workarounds** section after Overview explaining the 64 GB system-specific memory fixes (swap, pin_memory, TORCH_CUDA_ARCH_LIST, dataset_num_workers); updated training time estimate to reflect 6–8 hours with workarounds

---

## [Unreleased] - 2026-03-19 (Evening)

### Added
- `LoRAMemoryFixes.md` — complete guide to the memory issues blocking Qwen-Image-Edit LoRA training:
  - Root cause analysis (ZeRO-3 checkpoint save memory doubling: ~41 GB → ~87 GB)
  - All required fixes: correct venv, 32 GB swap, `pin_memory: false`, `TORCH_CUDA_ARCH_LIST="8.6"`, `--dataset_num_workers 0`
  - Confirmed working Stage 2 script
  - Speed optimisation roadmap (restore `pin_memory: true`, restore `--dataset_num_workers 2`)
  - Diagnostics reference with OOM interpretation table

### Changed
- Server `ds_z3_cpuoffload.json` — `pin_memory` set to `false` for both `offload_optimizer` and `offload_param`
- Server `stage2_train.sh` — added `TORCH_CUDA_ARCH_LIST="8.6"`, changed `--dataset_num_workers 2` to `0`
- Server swap increased from 8 GB to 32 GB (`/swap.img`)

### Fixed
- Qwen-Image-Edit LoRA training OOM on checkpoint save — training now completes and writes `epoch-N.safetensors`

---

## [Unreleased] - 2026-03-19

### Added
- `TMUX.md` — tmux reference guide: what tmux is, sessions/windows/panes concepts, prefix key, all common commands with explanations, practical workflows (long jobs, split monitoring), detach/attach pattern, quick reference card
- `Docker.md` — Docker administration guide: core concepts (image/container/volume/network), all common commands with when-to-use guidance, full `docker run` commands for all five services (Open WebUI, ComfyUI Steve, ComfyUI Amelia, FileBrowser, SearXNG), port map, Tailscale Serve rebuild commands, SSH file access explanation for bind-mounted volumes

### Fixed
- Stage 2 LoRA training restarted after tmux session loss — `stage2_train.sh` intact, Stage 1 cache intact, training resumed in new `lora-training` tmux session

---

## [Unreleased] - 2026-03-18 (Evening)

### Added
- `QwenImageEditTrainingLoRA.md` — comprehensive standalone guide for Qwen-Image-Edit-2511 character LoRA training:
  - Training image guidelines (count, resolution, angle variety, exclusion rules)
  - metadata.json creation script with correct `{"prompt": ..., "image": ...}` format
  - Accelerate ZeRO-3 config file (`accelerate_zero3_2gpu.yaml`) creation instructions
  - Stage 1 and Stage 2 scripts with all verified parameters
  - Per-run variable table (`MY_LORA_NAME`, `MY_DATASET_PATH`, `MY_TRIGGER_WORD`, `MY_LORA_RANK`)
  - Resource monitoring commands and expected VRAM/RAM usage
  - Output file locations and epoch selection guidance
  - Full troubleshooting section covering all errors encountered during development

### Changed
- `Model_and_LoRA_Creation.md` — `--max_pixels` reduced from 1048576 → 786432 → 524288 → 262144 (final working value); `--lora_rank` parameter table note updated

### Fixed
- ComfyUI process management: corrected from `sudo pkill -f "ComfyUI/main.py"` to `docker stop comfyui comfyui-amelia` — instances ARE Docker containers, not plain Python processes
- Training approach: replaced non-functional FP8+DDP method with DeepSpeed ZeRO-3 CPU offload (`--config_file accelerate_zero3_2gpu.yaml`, `num_processes: 1`)
- `--use_gradient_checkpointing` restored for Stage 2 — required to prevent activation tensors filling VRAM; was incorrectly removed based on outdated incompatibility warning

### Documentation
- Documented that FP8+DDP fundamentally cannot work on 2×24 GB: transformer fills ~23.2 GB per GPU regardless of quantisation, leaving no room for overhead
- Documented ZeRO-3 CPU offload mechanics: 41 GB transformer offloaded to CPU RAM, GPU holds only active layer parameters (~13–15 GB peak VRAM)
- Documented that `num_processes: 1` uses less VRAM than `num_processes: 2` under ZeRO-3 CPU offload (no AllGather communication buffers)
- Documented Stage 1 / Stage 2 `--max_pixels` coupling: cached latent dimensions are fixed at Stage 1 generation time; must delete cache and re-run Stage 1 when changing resolution
- Documented DeepSpeed ZeRO-3 merge date (March 17, 2026) — explains why previous session's approach predated this fix

---

## [Unreleased] - 2026-03-18

### Added
- `MultiFileModels.md` — standalone reference document explaining:
  - HuggingFace diffusers multi-file model format vs single-file safetensors
  - Qwen-Image-Edit-2511 file structure (transformer shards + text encoder + VAE)
  - Three options for using multi-file models in ComfyUI
  - DiffSynth-Studio model loading via `model_id_with_origin_paths`
  - `hf` CLI setup (venv creation, install, activation) from scratch
- `Model_and_LoRA_Creation.md` — **Workflow 3**: Qwen-Image-Edit character LoRA training:
  - Step 1: Dataset preparation with Python metadata.json generation script
  - Step 2: DiffSynth-Studio install + ModelScope model download
  - Step 3: Dependency verification (torchaudio fix, CUDA toolkit)
  - Step 4: Training script with multi-GPU accelerate config
  - Step 5: LoRA installation in ComfyUI

### Changed
- `Model_and_LoRA_Creation.md` Step 4 training command updated:
  - Added `--num_processes 2 --mixed_precision bf16` to `accelerate launch` — required to distribute model across both GPUs
  - Added `export PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True`
  - Added `--initialize_model_on_cpu` — prevents VRAM spike on GPU 0 during model load
  - Reduced `--max_pixels` from 1763584 to 1048576 (1024×1024)
  - Reduced `--lora_rank` from 32 to 16
  - Reduced `--dataset_num_workers` from 8 to 2
  - Added pre-flight GPU check section (stop ComfyUI and Ollama before training)
  - Expanded parameters table with explanations for all new flags
  - Updated download size estimate from ~58GB to ~20GB (4×5GB files via ModelScope)

### Documentation
- Documented why `--num_processes 2` is critical: without it accelerate uses single GPU, causing OOM on the 20B transformer
- Documented `--mixed_precision bf16` effect: causes DiffSynth-Studio to load fp8/quantised variant (~10GB per GPU vs ~25GB)
- Documented ComfyUI process management before training (incorrectly documented as plain Python processes — corrected to Docker containers in 2026-03-18 evening session)
- Documented `hf` vs `huggingface-cli` rename in huggingface_hub 1.x
- Documented DiffSynth-Studio metadata.json requirement (not .txt caption files)
- Documented ModelScope cache location (`~/.cache/modelscope/`)

---

## [Unreleased] - 2026-03-17

### Added
- `Model_and_LoRA_Creation.md` — comprehensive training guide covering two full workflows:
  - **Workflow 1**: FLUX.1 Dev character LoRA using ai-toolkit (photo selection criteria, dataset prep, config, training, ComfyUI integration)
  - **Workflow 2**: LLM fine-tuning for a custom knowledge chatbot using Unsloth (JSONL dataset format, training script, GGUF export, Ollama deployment)
  - Common pitfalls section for both workflows
  - Quick reference checklists for both workflows

### Changed
- `CLAUDE.md` — added "System Specifications Reference" section at the top of Project Purpose, directing Claude to read `Final_Build.md` and `Software_Setup.md` for hardware/software specs; includes inline quick-reference summary

### Documentation
- Documented ai-toolkit verified parameters for RTX 3090 24GB: rank 16, lr 1e-4, 2000 steps, bf16, adamw8bit, gradient checkpointing
- Documented Unsloth QLoRA configuration: rank 16, lr 2e-4, 3 epochs, bf16, NVLink multi-GPU support
- Documented photo dataset selection criteria (inclusion/exclusion rules, captioning strategy, trigger word guidance)
- Documented JSONL conversational format for LLM training data with system prompt injection pattern
- Documented GGUF export and Ollama Modelfile creation workflow

---

## [Unreleased] - 2026-03-14

### Added
- `Tailscale.md` — comprehensive guide covering: overview, installation, useful commands (port checking, status, diagnostics), port forwarding via tailscale serve and iptables, UFW alternative, troubleshooting section, securing services with loopback binding, dual binding for LAN + Tailscale access, and how the two access paths work independently

### Changed
- `HuggingFace.md` — added "Sharing Models with Amelia's Instance" section documenting hard link commands for adding/removing models from the restricted instance
- `Software_Setup.md` — updated all docker run commands with dual `-p` bindings (loopback for Tailscale serve, LAN IP for local access); replaced generic service URL table with full local + Tailscale access table; updated Open WebUI and FileBrowser access sections with specific URLs; added `sudo` to Tailscale serve rebuild commands; removed outdated "offset ports" note

### Documentation
- Documented Docker `-p` flag in depth: IP binding to network interfaces, host port vs container port, container isolation meaning ports can repeat across containers
- Documented why path-based Tailscale routing was evaluated and rejected (ComfyUI has no base URL support)
- Documented static DHCP reservation requirement for reliable dual binding

---

## Prior Sessions

Session history prior to 2026-03-14 is tracked in git commit messages. Run `git log --oneline` for a summary.
