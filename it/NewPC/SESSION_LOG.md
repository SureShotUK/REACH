# Session Log — NewPC Project

---

## Session 2026-04-08

### Summary
Recovered from a boot failure caused by `pci=nomsi` being applied to GRUB (following suggestions from an old troubleshooting log). Discovered that the GPU PCIe link showing Gen 1 at idle is expected ASPM power-management behaviour — the GPUs ramp to Gen 4 under load. Issue 5 is confirmed fully resolved. Linux_Troubleshooting.md updated to document the idle/load behaviour and add a clear warning against `pci=nomsi`.

### Work Completed
- Diagnosed boot failure: `pci=nomsi` in `GRUB_CMDLINE_LINUX_DEFAULT` disabled MSI for all PCIe devices including NVMe SSDs; NVMe controllers timed out and dropped to initramfs shell
- Recovered system by editing GRUB at boot menu (held SHIFT, pressed `e`, removed `pci=nomsi` from `linux` line)
- Confirmed GRUB back to `=""` and BIOS ASPM at Auto (both already correct)
- Attempted cold power cycle — GPUs still showing 2.5GT/s at idle
- Identified correct explanation: ASPM power management drops PCIe link to Gen 1 when GPUs are idle; this is normal behaviour, not a fault
- User confirmed Gen 4 (16GT/s) on both cards by checking `lspci` and `nvidia-smi` while a GPU workload was running
- Updated `Linux_Troubleshooting.md`: status line, Performance Impact section rewritten to describe idle/load behaviour, reference table corrected, verification checklist updated, WARNING section added for `pci=nomsi`

### Files Changed
- `it/NewPC/Linux_Troubleshooting.md` — Issue 5 status updated; Performance Impact section rewritten; reference table corrected; verification checklist updated; WARNING section added documenting `pci=nomsi` boot failure and recovery

### Git Commits
- No commits from prior sessions relevant to this session's work — changes committed at end of session

### Key Decisions
- **`pci=nomsi` must never be added** — breaks NVMe boot devices; recovery requires GRUB menu edit
- **Gen 1 at idle is correct** — ASPM idle power management; verify Gen 4 only under active GPU load, not at idle
- **GPU Link Speed Output.txt is obsolete** — was from the pre-fix troubleshooting session; the parameters discussed there are no longer relevant and should not be applied

### Reference Documents
- `it/NewPC/Linux_Troubleshooting.md` — Issue 5 (updated with idle/load behaviour clarification and pci=nomsi warning)
- `it/NewPC/GPU Link Speed Output.txt` — old troubleshooting log (now superseded; caused this session's incident)

### Next Actions
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai (carried over)
- [ ] Verify ComfyUI OOM fix — confirm first generation succeeds without click-OK-retry

---

## Session 2026-04-07

### Summary
Investigated and resolved the dual RTX 3090 PCIe Gen 1 fallback issue on amelai. After exhaustive testing of BIOS settings, BIOS update (2102→2103), GPU reseating, NVLink removal, and single-GPU isolation — the fix turned out to be removing the `pcie_aspm=off` kernel parameter from GRUB and restoring BIOS ASPM to Auto. Both GPUs now running at PCIe Gen 4 (16GT/s). The parameter had been added months earlier to address the Intel igc NIC crashes, but with igc now blacklisted it was safe to remove — and it had been silently blocking PCIe link equalization the entire time.

### Work Completed
- Pulled latest files from GitHub at session start
- Verified all BIOS settings on 2103: CSM, Above 4G Decoding, ReBAR, SR-IOV, ASPM, Bifurcation, PCIe Link Speed all confirmed/configured
- Identified via `sudo lspci -vvv | grep -E "PCI bridge|LnkSta"` that CPU root ports `00:01.1` and `00:01.3` are stuck at 2.5GT/s while `00:01.2` (NVMe) runs at 32GT/s — problem is in the root ports, not the GPU cards
- Confirmed `nvidia-smi --query-gpu=pcie.link.gen.current,...`: Gen 1 current, Gen 4 max on both GPUs
- Updated BIOS from 2102 to 2103 via EZ Flash (required both `.CAP` and `.CFG` files on FAT32 USB) — no change
- Physically reseated both GPUs, removed NVLink bridge — no change
- Tested single GPU only in PCIEX16_1 — still Gen 1; dual-GPU configuration not the cause
- Set CPU PCIE ASPM Mode Control to Disabled in BIOS — no change
- Removed `pcie_aspm=off` from `GRUB_CMDLINE_LINUX_DEFAULT` (safe: igc is blacklisted) and restored BIOS ASPM to Auto — no change to link speed; fixed nvidia-smi width reporting (was showing 1, now correctly shows 8/16)
- Updated `Linux_Troubleshooting.md` — Issue 5 fully rewritten with confirmed diagnosis, complete troubleshooting log table, corrected PCIe speed table, updated BIOS settings table, updated checklist
- Created `ASUS_PCIe_Support_Case.md` — formatted support document with system spec, all diagnostic outputs, chronological step log, and elimination summary table

### Files Changed
- `it/NewPC/Linux_Troubleshooting.md` — Issue 5 comprehensively rewritten (confirmed BIOS/AGESA bug, full troubleshooting log, corrected PCIe speed table)
- `it/NewPC/ASUS_PCIe_Support_Case.md` — new document created for ASUS technical support

### Key Decisions
- **`pcie_aspm=off` removed from GRUB** — no longer needed (igc blacklisted); was interfering with nvidia-smi link width reporting
- **Confirmed BIOS/AGESA bug** — not a physical, BIOS setting, or OS configuration issue; awaiting future ASUS BIOS fix
- **Performance impact is nil** — AI inference is entirely on-GPU; PCIe speed only affects model load time from RAM to VRAM

### Reference Documents
- `it/NewPC/Linux_Troubleshooting.md` — Issue 5 (updated)
- `it/NewPC/ASUS_PCIe_Support_Case.md` — new ASUS support case document

### Next Actions
- [ ] Verify 90-second boot delay (WiFi `wlp11s0`) still resolved — pending from last session
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai

---

## Session 2026-04-06 (2)

### Summary
Diagnosed a second Intel igc NIC PCIe crash on amelai (April 6, ~21:16 BST) — confirmed `pcie_aspm=off` was active and did not prevent recurrence. Permanently fixed by switching the primary network connection to the Aquantia AQC113 10GbE NIC (`ethernet2_5g`, 192.168.1.192) and blacklisting the igc driver. Also fixed system timezone (was UTC, now Europe/London), removed broken WiFi section from netplan, and documented the 90-second boot delay caused by the WiFi adapter with a pending verification on next reboot.

### Work Completed
- Diagnosed igc NIC crash recurrence from `journalctl -b -1 -k` logs — confirmed `pcie_aspm=off` was in `/proc/cmdline` but NIC still dropped
- Identified Aquantia AQC113 10GbE NIC (`ethernet2_5g`) as the permanent alternative — was uncabled and unused
- Updated `/etc/netplan/*.yaml`: swapped IPs and metrics — Aquantia gets 192.168.1.192/metric 100 (primary), Intel igc gets 192.168.1.193/metric 200 (secondary, optional)
- Removed broken WiFi section from netplan (no password set was causing `netplan apply` to error on `netplan-wpa-wlp11s0.service`)
- Moved ethernet cable from Intel port to Aquantia port on rear I/O panel
- Ran `sudo netplan apply` — Aquantia came up at 192.168.1.192, confirmed with `ip addr show ethernet2_5g`
- Blacklisted igc driver: `echo "blacklist igc" | sudo tee /etc/modprobe.d/blacklist-igc.conf && sudo update-initramfs -u`
- Fixed timezone: `sudo timedatectl set-timezone Europe/London`
- Updated `Linux_Troubleshooting.md` — Issue 2 rewritten to document both incidents, failed ASPM fix, and permanent Aquantia switchover; Issue 4 added for WiFi boot delay

### Files Changed
- `it/NewPC/Linux_Troubleshooting.md` — Issue 2 updated (two incidents, permanent fix documented); Issue 4 added (90-second boot delay, WiFi adapter)

### Key Decisions
- **`pcie_aspm=off` is insufficient** — Intel I226-V igc driver has a deeper bug; ASPM disabling only addresses one trigger
- **Aquantia AQC113 as permanent primary** — more reliable driver (`atlantic`), faster (10GbE capable vs 2.5GbE), already on the motherboard
- **igc driver blacklisted** — prevents the faulty NIC from loading and potentially destabilising the system even as a secondary interface
- **WiFi removed from netplan** — was misconfigured (no password), caused `netplan apply` errors, and likely responsible for 90-second boot delay

### Next Actions
- [ ] Verify 90-second boot delay is resolved on next reboot (WiFi removed from netplan)
- [ ] If boot delay persists: `sudo systemctl mask systemd-networkd-wait-online.service`
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai
- [ ] Verify `/docs` NFS mount auto-mounts correctly after reboot

---

## Session 2026-04-06

### Summary
Permanently mounted the Synology DS920+ `MyDocs` NAS share to the Linux server (`amelai`) at `/docs`. Attempted SMB/CIFS first but persistent `STATUS_LOGON_FAILURE` from the credentials file (special characters in password causing auth failure despite correct content) led to switching to NFS, which worked first time. CLAUDE.md updated with a Linux session housekeeping checklist.

### Work Completed
- Installed `nfs-common` and `cifs-utils` (cifs-utils later removed)
- Discovered NFS export path: `/volume2/MyDocs` restricted to `192.168.1.192`
- Added permanent NFS mount to `/etc/fstab`: `192.168.1.216:/volume2/MyDocs /docs nfs defaults,_netdev,nofail 0 0`
- Verified mount works and files are accessible from both Linux and the existing Windows SMB share
- Removed `cifs-utils` and `smbclient` (installed only for troubleshooting)
- Updated `CLAUDE.md` with Linux session housekeeping section (remove temp packages, run updates, clean up credentials/temp files)

### Files Changed
- `it/NewPC/CLAUDE.md` — added "Linux Session Housekeeping" section

### Key Decisions
- Switched from SMB/CIFS to NFS after persistent credentials file auth failures caused by special characters in password
- NFS uses IP-based auth (no credentials file needed) — simpler and more robust for Linux-only mounts
- Files created via NFS are visible on the existing Windows SMB share immediately (same underlying volume)

### Next Actions
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai
- [ ] Verify `/docs` auto-mounts correctly after next reboot

---

## Session 2026-04-05 (2)

### Summary
Diagnosed and resolved two ComfyUI issues: (1) OOM errors on first generation with the Qwen-Rapid-AIO-NSFW-v23 model fixed by adding `--reserve-vram 3` to CLI_ARGS; (2) Tailscale access broken after container rebuild due to wrong loopback port in ComfyUI.md (8189 instead of 18189) — corrected and container rebuilt. Also fixed FileBrowser to expose ComfyUI input/output folders, set up `.gitignore` to exclude build artifacts and junk, and committed all legitimate untracked content to git. Diagnosed that Qwen workflow saves generated images to the input folder (`/opt/comfyui/input/`) as temp files by design.

### Work Completed
- Fixed Qwen OOM error — `--reserve-vram 3` added to `CLI_ARGS` reserves 3GB VRAM headroom preventing fragmentation-induced allocation failures
- Fixed Tailscale access broken by wrong loopback port — `ComfyUI.md` had `127.0.0.1:8189` instead of `127.0.0.1:18189`; container rebuilt with correct binding
- Updated `ComfyUI.md` docker run command: corrected loopback port to `18189` and added `--reserve-vram 3`
- Added `.gitignore` — excludes `.NET bin/obj`, `*.log`, `*.oft`, `*.bin`, `*_files/`, `Backups_*/`, and named temp files
- Committed all legitimate untracked content (139 files): Canada, REACH HVO, IUCLID, hseea subdirs, insurance, ZeroTrust, postgres-security, wsl-postgresql-setup, IT security docs, NewPC workflows/configs, OutlookTemplateCleaner source
- Updated `Docker.md` — FileBrowser now mounts `/opt/comfyui/input/`, `/opt/comfyui/output/`, `/opt/comfyui-amelia/input/`, `/opt/comfyui-amelia/output/`; rebuilt FileBrowser container
- Added "warnings before destructive commands" rule to `CLAUDE.md` (shared) and memory — warnings must appear before commands, not after
- Diagnosed Qwen generated image location: saved to `/opt/comfyui/input/` as `ComfyUI_temp_*` files by design (image-edit workflow outputs feed directly to next input)
- Cross-platform git sync verified end-to-end: Windows pulled successfully after Linux push

### Files Changed
- `it/NewPC/ComfyUI.md` — corrected loopback port `8189`→`18189`; added `--reserve-vram 3` to CLI_ARGS
- `it/NewPC/Docker.md` — FileBrowser command updated with ComfyUI input/output volume mounts
- `CLAUDE.md` (shared) — added "Warnings Before Instructions" principle
- `.gitignore` — created with exclusion rules for build artifacts, logs, binary Outlook files, temp files

### Git Commits
- `50af171` — Add cross-platform git sync infrastructure and update NewPC CLAUDE.md
- `f63318e` — End of session — cross-platform git sync infrastructure added
- `ae427a2` — Normalise line endings and commit local content changes
- `64f3a33` — Add .gitignore and commit all legitimate untracked content

### Key Decisions
- **`--reserve-vram 3`** rather than `--lowvram` — preserves performance while fixing the specific fragmentation issue; `--lowvram` would be slower across the board
- **Loopback port `18189`** — follows the `1XXXX` convention documented in CLAUDE.md; `ComfyUI.md` had a typo that went unnoticed until Tailscale broke
- **Qwen saves to input folder** — this is by design for the image-edit workflow; generated images become available as inputs without downloading. FileBrowser now exposes this folder.
- **`git add -u` not `git add -A`** for line-ending normalisation commit — avoided accidentally committing junk before `.gitignore` was in place

### Reference Documents
- `it/NewPC/ComfyUI.md` — corrected docker run command
- `it/NewPC/Docker.md` — updated FileBrowser command

### Next Actions
- [ ] Verify ComfyUI OOM fix — confirm first generation succeeds without click-OK-retry
- [ ] Verify FileBrowser shows `comfyui-input/` folder with generated images
- [ ] Check if Load Image node in Qwen workflow can browse input folder to select previous generations
- [ ] Confirm Windows git credentials working — run `/sync-files` from Windows Claude Code

---

## Session 2026-04-05

### Summary
Set up cross-platform git synchronisation infrastructure so Claude Code context files (CLAUDE.md, session logs, documentation) can be kept in sync between the Windows 11 PC and the amelai Linux server. Created a `.gitattributes` file to fix line-ending differences between platforms, and a new `/sync-files` slash command that handles commit → pull → push in the correct order for bidirectional sync.

### Work Completed
- Diagnosed that the existing `/sync-session` command only pushes and never pulls — not suitable for cross-platform use
- Created `.gitattributes` — normalises all text files to LF in the git repo; Windows checkouts receive CRLF, Linux receives LF; binary files (PDFs, Office docs, images, model files) exempt
- Created `.claude/commands/sync-files.md` — `/sync-files` command that handles all four sync states: ahead-only, behind-only, diverged, and up-to-date; uses rebase so local changes take priority on conflict
- Staged and pushed session management commands (`session-*.md`) that were untracked
- Updated `CLAUDE.md` with session learnings: ComfyUI model structure (FLUX self-contained), VRAM/Ollama contention table, Docker dual-port binding strategy, bash special characters in passwords, and file delivery pattern via `Temp.txt`

### Files Changed
- `.gitattributes` — created; cross-platform line ending normalisation
- `.claude/commands/sync-files.md` — created; `/sync-files` slash command for bidirectional git sync
- `.claude/commands/session-*.md` — staged (were previously untracked)
- `it/NewPC/CLAUDE.md` — major update with knowledge captured across recent sessions

### Git Commits
- `50af171` — Add cross-platform git sync infrastructure and update NewPC CLAUDE.md

### Key Decisions
- **Rebase strategy for conflict resolution** — `git pull --rebase` is used so local commits are replayed on top of remote changes; in a conflict on the same line, local wins. True "newest file by mtime" is not feasible reliably in git.
- **`.gitattributes` at repo root** — applied once, affects all projects in the monorepo
- **CRLF preserved for `.ps1`, `.bat`, `.cmd`** — PowerShell scripts expect CRLF on Windows; all other files normalised to LF

### Reference Documents
- `.gitattributes` — repo root; line ending configuration
- `.claude/commands/sync-files.md` — `/sync-files` command definition

### Next Actions
- [ ] Pull on Windows 11 PC to get `.gitattributes` and `/sync-files` command
- [ ] Run `/sync-files` from Windows Claude Code to verify end-to-end sync works
- [ ] Verify git credentials configured on Windows (GitHub PAT or Git Credential Manager)

---

## Session 2026-03-24 (2)

### Summary
Investigated symlink behaviour for `.claude/commands/` files on Windows and clarified that they cannot be opened by Windows apps. Diagnosed and resolved an Ollama file-writing hallucination — confirmed `/mnt/uploads` was already mounted in the Open WebUI container (mapped to `/home/steve/rag-output`). Created a Python FileWriter tool for Open WebUI that gives models genuine filesystem write capability, accessible immediately via FileBrowser.

### Work Completed
- Explained that `it/.claude/commands/end-session.md` is a symlink — real file is at `terminai/.claude/commands/end-session.md`
- Diagnosed model hallucination: Ollama/Open WebUI has no built-in filesystem write capability — model was fabricating write confirmations
- Confirmed `/mnt/uploads` already mounted in Open WebUI container via `docker inspect` (maps to `/home/steve/rag-output`)
- Created `FileWriter.py` — Open WebUI Tool function for genuine file writes with path traversal protection
- Confirmed FileBrowser already has access to `rag-output`, so written files are immediately browsable

### Files Changed
- `it/NewPC/FileWriter.py` — created; Open WebUI Tool class for writing files to `/mnt/uploads`

### Key Decisions
- **No container recreation needed** — `/mnt/uploads` bind mount already existed from prior setup
- **`/home/steve/rag-output`** is the host-side path for files written by the model
- **Path traversal protection** via `os.path.basename()` — strips any directory components from the filename parameter

### Next Actions
- [ ] Test FileWriter tool end-to-end — ask model to write a file and verify it appears in FileBrowser

---

## Session 2026-03-24

### Summary
Diagnosed and resolved two reliability issues on amelai: (1) the Intel igc NIC dropping off the PCIe bus due to ASPM, causing SSH/Tailscale unreachability for ~5 hours; (2) Ollama being OOM-killed repeatedly due to ComfyUI holding 28.4GB VRAM overnight, leaving insufficient VRAM for qwen3.5:35b to load. Both issues are now fixed and documented in a new Linux troubleshooting reference guide.

### Work Completed
- Diagnosed NIC failure from journald logs: `igc PCIe link lost, device now detached` at 10:21:38
- Confirmed system was running throughout — the NIC died, not the OS
- Attempted `igc aspm_disable=1` module parameter — confirmed not supported on kernel 6.17
- Applied correct fix: `pcie_aspm=off` kernel boot parameter via GRUB
- Verified fix: `PCIe ASPM is disabled` confirmed in kernel log, NIC stable post-reboot
- Identified Ollama OOM kills from Mar 23 (4 kills in 7 min, ~40GB anon-rss each)
- Root cause: ComfyUI holding 28.4GB VRAM (Qwen-Rapid-AIO-NSFW-v23) left only 19.6GB free — insufficient for qwen3.5:35b (~26-28GB)
- Created browser bookmarklet to call ComfyUI `/free` API endpoint to unload VRAM on demand
- Added cron jobs to restart both ComfyUI containers at 2am nightly as safety net
- Created `Linux_Troubleshooting.md` — comprehensive reference covering log analysis, NIC PCIe ASPM fix, and Ollama/ComfyUI VRAM contention

### Files Changed
- `it/NewPC/Linux_Troubleshooting.md` — created; three issues documented: SSH crash triage guide, igc PCIe ASPM fix, Ollama OOM/ComfyUI VRAM contention fix

### Key Decisions
- **`pcie_aspm=off` system-wide** (not device-specific) — `igc` module has no `aspm_disable` parameter on kernel 6.17; GRUB parameter is the only reliable approach. Power impact negligible on an AI server under load.
- **ComfyUI VRAM budget awareness** — Qwen-Rapid-AIO-NSFW-v23 at 28.4GB effectively blocks all large Ollama models. Must free VRAM between sessions.
- **Bookmarklet approach** for ComfyUI VRAM free — uses relative `/free` URL so works on both ComfyUI instances from the active browser tab. `.json()` must not be used (endpoint returns empty body); use `.ok` status check instead.
- **Cron restart at 2am** as safety net — ensures VRAM is always free by morning even if bookmarklet is forgotten.

### Reference Documents
- `it/NewPC/Linux_Troubleshooting.md` — new troubleshooting reference guide

### Next Actions
- [ ] Monitor NIC stability over coming days — confirm `pcie_aspm=off` holds
- [ ] Address `systemd-networkd-wait-online` timeout warnings (WiFi adapter wlp11s0 — known ASUS X870E Linux issue)
- [ ] Research and confirm current UK pricing for RTX 5070 Ti 16GB (AIB partner selection)
- [ ] Verify Arctic Liquid Freezer III 360 compatibility with Corsair 4000D Airflow case
- [ ] Confirm Ryzen 7 9800X3D UK street price and retailer availability

---

## Session 2026-03-23

### Summary
Diagnosed and resolved SearXNG MCP web search not working in Claude Code on Windows 11. Two root causes found: port 3001 missing from the Tailscale ACL, and a stored Anthropic auth credential overriding the Ollama environment variables. Also investigated `hf-env` auto-activation on SSH login and resolved Open WebUI's Ollama connection error.

### Work Completed
- Identified MCP server (`mcp-searxng.service`) was running correctly on amelai — server-side was healthy throughout
- Added port 3001 to Tailscale ACL at admin.tailscale.com (`src: ["*"]` rule)
- Cleared stored Anthropic auth credential with `claude auth logout` — this was overriding `ANTHROPIC_BASE_URL` and routing requests to the real Anthropic API
- Re-registered MCP as user-scoped (`--scope user`) so it works in all projects, not just NewPC
- Fixed `hf-env` auto-activation by commenting out `conda activate hf-env` in `~/.bashrc` on amelai
- Fixed Open WebUI Ollama connection error — URL changed from `https://` to `http://100.79.83.113:11434` in Admin Panel → Settings → Connections
- Confirmed web search working in both Claude Code and Open WebUI
- Created `SearXNG_Fix.md` — full troubleshooting log and architecture reference

### Files Changed
- `it/NewPC/SearXNG_Fix.md` — created; full troubleshooting log documenting both root causes, misleading `Test-NetConnection` behaviour, and verification steps

### Key Decisions
- **Port 3001 added to `src: ["*"]` ACL rule** (not restricted to 3 IPs) — allows daughter's PC and any future tailnet device to use MCP web search
- **`Test-NetConnection` is unreliable** for Tailscale connectivity testing in this environment — TCP showed as failed on all ports (22, 443, 3001, 11434) even when connections were working. Use SSH or application-level tests instead.

### Reference Documents
- `it/NewPC/SearXNG_Fix.md` — MCP/SearXNG troubleshooting log
- `it/NewPC/LoadClientClaude.md` — Windows client setup guide (referenced throughout)

### Next Actions
- [ ] Research and confirm current UK pricing for RTX 5070 Ti 16GB (AIB partner selection)
- [ ] Verify Arctic Liquid Freezer III 360 compatibility with Corsair 4000D Airflow case
- [ ] Confirm Ryzen 7 9800X3D UK street price and retailer availability

---

## Session 2026-03-22

### Summary
Created `New_PC_Builds.md` — a comprehensive personal Windows 11 PC build guide to replace the user's Windows 10 machine. Used the gemini-researcher agent to research current UK market components, then iteratively refined all component choices through conversation. Final build centres on the Ryzen 7 9800X3D and RTX 5070 Ti for 1440p Minecraft Bedrock RTX, with video editing workloads offloadable to the existing AI PC via Tailscale.

### Work Completed
- Created `New_PC_Builds.md` with full component research, three build options, chosen configuration, and video editing via AI PC section
- Researched and confirmed existing components (MSI MAG X870E Tomahawk WIFI, Samsung 9100 Pro 2TB, Viper Venom DDR5 32GB)
- Confirmed NVIDIA-only requirement for Minecraft Bedrock RTX (DXR hardware implementation)
- Evaluated RTX 5070 Ti vs 5070 — chose 5070 Ti for 256-bit memory bus advantage at 1440p RT
- Evaluated Ryzen 9 9950X3D vs 9800X3D — chose 9800X3D (9950X3D gaming performance equal, extra cores not needed as AI PC handles heavy encoding)
- Added remote video encoding section: DaVinci Resolve render queue + Adobe Media Encoder via Tailscale to dual RTX 3090 server
- Upgraded PSU to be quiet! Power Zone 2 1000W (80+ Platinum, £149.99, verified on be quiet! website)
- Upgraded CPU cooler from Arctic Liquid Freezer III 240 to 360 for quieter sustained operation

### Files Changed
- `it/NewPC/New_PC_Builds.md` — created; full personal Windows 11 PC build guide

### Key Decisions
- **Ryzen 7 9800X3D chosen over 9950X3D**: Gaming performance is equal; 9950X3D's 16 cores benefit video editing but heavy encoding offloaded to AI PC instead — £320 premium not justified
- **RTX 5070 Ti chosen**: 256-bit memory bus (~896 GB/s) vs 5070's 192-bit (~672 GB/s) — tangible difference for 1440p Bedrock RTX path tracing
- **1000W PSU**: ~520W headroom above expected peak load; future-proofs against GPU upgrades without PSU replacement
- **360mm AIO over 240mm**: Runs fans slower for same heat dissipation — quieter sustained operation; consistent with be quiet! PSU noise philosophy
- **Remote encoding strategy**: RTX 5070 Ti NVENC handles most exports locally; dual RTX 3090s on AI PC available for heavy/concurrent jobs via DaVinci Resolve render queue over Tailscale

### Reference Documents
- `it/NewPC/New_PC_Builds.md` — primary output document
- be quiet! Power Zone 2 1000W verified at `https://www.bequiet.com/en/powersupply/5899` (£149.99, March 2026)

### Next Actions
- [ ] Research and confirm current UK pricing for RTX 5070 Ti 16GB (AIB partner selection)
- [ ] Verify Arctic Liquid Freezer III 360 compatibility with Corsair 4000D Airflow case (radiator clearance)
- [ ] Confirm Ryzen 7 9800X3D UK street price and retailer availability
- [ ] Consider monitor research (1440p, high refresh rate to complement the build)

---

## Session 2026-03-19 (Late Evening)

### Summary
Added a dedicated RAM limitation workarounds section to `QwenImageEditTrainingLoRA.md`, making all three required memory fixes visible at the top of the guide before any training steps are attempted. The section clearly states these workarounds are hardware-specific to 64 GB systems and would not be needed with 128 GB or more.

### Work Completed
- Added **RAM Limitation Workarounds** section to `QwenImageEditTrainingLoRA.md` covering all three required changes: 32 GB swap, `pin_memory: false`, and `TORCH_CUDA_ARCH_LIST="8.6"` / `--dataset_num_workers 0`
- Updated training time estimate in Overview to reflect 6–8 hours with workarounds vs 4–5 hours default

### Files Changed
- `it/NewPC/QwenImageEditTrainingLoRA.md` — added RAM Limitation Workarounds section after Overview; updated training time estimate

### Next Actions
- [ ] Confirm full training completes — check `ls ~/DiffSynth-Studio/models/train/my_character_lora/`
- [ ] Restart Docker + Ollama: `docker start comfyui comfyui-amelia && sudo systemctl start ollama`
- [ ] Test each epoch LoRA in ComfyUI — copy to `/mnt/models/comfyui/loras/`
- [ ] Try speed optimisations from `LoRAMemoryFixes.md` once training succeeds
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 — replace obsolete FP8+DDP with ZeRO-3 method

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
