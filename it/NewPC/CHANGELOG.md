# Changelog — NewPC Project

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
