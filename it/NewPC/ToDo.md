# NewPC - Outstanding To Do

- [x] Configure Open WebUI system prompt (Phase 3.3: paste workspace system prompt into Admin → Settings → System Prompt)
- [ ] Install second RTX 3090 — verify both GPUs in `nvidia-smi`; set `OLLAMA_MAX_LOADED_MODELS=2` in Ollama systemd override
- [x] Fix ethernet link flapping — restricted advertised modes to 1Gb only (`ethtool advertise 0x020`); made permanent via `fix-ethernet.service` systemd unit
- [ ] GPU power limit tuning — `sudo nvidia-smi -pl 300`; make permanent via systemd service
- [x] Phase 5: Security hardening — Open WebUI sign-up disabled; Tailscale ACL not required (single-user personal tailnet, daughter on separate account, work will use separate org); ai-executor user not viable (Open WebUI requires root in Docker — Docker isolation is the security boundary)
- [x] Test session end workflow — confirmed working; fixed `workspace_commit` bug (branch was hardcoded to `main`, corrected to `master`)
- [x] Install Tailscale on other devices — Windows PC and phone for remote Open WebUI access
- [x] Set up second client machine using `LoadClientClaude.md` — Windows 10 home desktop, Tailscale + Claude Code + MCP registered and verified
