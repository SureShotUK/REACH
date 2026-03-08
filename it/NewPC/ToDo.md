# NewPC - Outstanding To Do

- [x] Configure Open WebUI system prompt (Phase 3.3: paste workspace system prompt into Admin → Settings → System Prompt)
- [x] Install second RTX 3090 — both GPUs verified in `nvidia-smi`; both running at PCIe x8; ASUS ProArt X870E-CREATOR WIFI configured (BIOS 2102, IOMMU, Above 4G, Resizable BAR, fTPM, AIO pump); M.2 drives moved to M.2_1 and M.2_3 to avoid GPU bandwidth conflict; ASUS_Setup.md created
- [x] Fix ethernet link flapping — restricted advertised modes to 1Gb only (`ethtool advertise 0x020`); made permanent via `fix-ethernet.service` systemd unit
- [x] GPU power limit tuning — both GPUs set to 300W; made permanent via `nvidia-power-limit.service` systemd unit (includes persistence mode)
- [x] Set OLLAMA_MAX_LOADED_MODELS=2 in Ollama systemd override — both GPUs detected, 48GB combined VRAM, 262K context window
- [ ] NVLink bridge (P3669 ordered, arriving Wednesday) — install and verify with `nvidia-smi nvlink --status -i 0`
- [x] Phase 5: Security hardening — Open WebUI sign-up disabled; Tailscale ACL not required (single-user personal tailnet, daughter on separate account, work will use separate org); ai-executor user not viable (Open WebUI requires root in Docker — Docker isolation is the security boundary)
- [x] Test session end workflow — confirmed working; fixed `workspace_commit` bug (branch was hardcoded to `main`, corrected to `master`)
- [x] Install Tailscale on other devices — Windows PC and phone for remote Open WebUI access
- [x] Set up second client machine using `LoadClientClaude.md` — Windows 10 home desktop, Tailscale + Claude Code + MCP registered and verified
