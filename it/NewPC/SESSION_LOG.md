# Session Log — NewPC Project

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
