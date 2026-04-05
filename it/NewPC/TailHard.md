# Tailscale ACL Hardening — Restrict ComfyUI (Steve) to Specific Devices

**Status**: PENDING — ACL not yet applied as of 2026-03-17. User rebooted before completing.

---

## Objective

Restrict access to **ComfyUI (Steve)** on Tailscale port **8189** so that only two specific devices can reach it on the tailnet. All other tailnet devices can still reach all other services.

---

## Context: Current Setup

### Server details

| Item | Value |
|---|---|
| Server Tailscale IP | `100.79.83.113` |
| Server LAN IP | `192.168.1.192` |
| Tailscale hostname | `amelai.tail926601.ts.net` |

### Services and ports

| Service | Container port | Loopback binding | LAN binding | Tailscale port | Tailscale URL |
|---|---|---|---|---|---|
| Open WebUI | 8080 | `127.0.0.1:3000` | `192.168.1.192:3000` | 443 | `https://amelai.tail926601.ts.net` |
| ComfyUI (Steve) | 8188 | `127.0.0.1:18189` | `192.168.1.192:8189` | 8189 | `https://amelai.tail926601.ts.net:8189` |
| ComfyUI (Amelia) | 8188 | `127.0.0.1:18188` | `192.168.1.192:8188` | 8188 | `https://amelai.tail926601.ts.net:8188` |
| FileBrowser | 80 | `127.0.0.1:18087` | `192.168.1.192:8087` | 8087 | `https://amelai.tail926601.ts.net:8087` |

### Tailscale serve config (on server)

```bash
sudo tailscale serve --bg --https=443 http://localhost:3000
sudo tailscale serve --bg --https=8087 http://localhost:18087
sudo tailscale serve --bg --https=8188 http://localhost:18188
sudo tailscale serve --bg --https=8189 http://localhost:18189
```

Verify with: `sudo tailscale serve status`

---

## Permitted Devices for Port 8189

| Device | Tailscale IP |
|---|---|
| Permitted device 1 | `100.95.206.56` |
| Permitted device 2 | `100.120.116.23` |

---

## The ACL Policy to Apply

### Key concept — why the catch-all must be removed

Tailscale ACLs are a **pure allowlist** — rules are additive (OR'd together). If a catch-all `"dst": ["*:*"]` rule exists, it matches port 8189 for all devices, making any specific restriction pointless. The catch-all must be replaced with explicit rules that enumerate allowed services but exclude port 8189 from the broad rule.

### ACL JSON — paste this into the admin console

```json
{
  "acls": [
    {
      "action": "accept",
      "src": ["100.95.206.56", "100.120.116.23"],
      "dst": ["100.79.83.113:8189"]
    },
    {
      "action": "accept",
      "src": ["*"],
      "dst": [
        "100.79.83.113:22",
        "100.79.83.113:443",
        "100.79.83.113:8087",
        "100.79.83.113:8088",
        "100.79.83.113:8188",
        "100.79.83.113:11434"
      ]
    }
  ]
}
```

---

## Steps to Apply

1. Go to `https://login.tailscale.com/admin/acls`
2. Replace the existing ACL policy with the JSON above
3. **Before saving — use the Preview button** to verify:
   - A permitted device (`100.95.206.56` or `100.120.116.23`) CAN reach `100.79.83.113:8189`
   - Any other tailnet device CANNOT reach `100.79.83.113:8189`
   - All devices CAN still reach the other services (port 443, 8087, 8088, 8188, 22, 11434)
4. Save — changes propagate to all devices within seconds

### Important check before saving

Confirm your current device is one of the two permitted IPs. If you are on a different device, you will lock yourself out of port 8189 immediately. Run `tailscale ip -4` on your current device to verify.

---

## Scope and Limitations

### What Tailscale ACLs DO control

- Access via Tailscale: `https://amelai.tail926601.ts.net:8189`
- The ACL blocks at the Tailscale network layer — unauthenticated devices cannot reach the port at all

### What Tailscale ACLs do NOT control

- **LAN access**: `http://192.168.1.192:8189` is entirely unaffected — any device on the local network can still reach it directly
- Tailscale ACLs operate on the tailnet only; the LAN path bypasses them entirely

### If you also want to restrict LAN access

Remove the LAN `-p` binding for port 8189 from the Docker run command — change:

```
-p 127.0.0.1:18189:8188 \
-p 192.168.1.192:8189:8188 \
```

to:

```
-p 127.0.0.1:18189:8188 \
```

This means the container is only reachable via Tailscale serve (loopback path). The Tailscale ACL then fully controls who can access it. This requires stopping and recreating the ComfyUI (Steve) container.

---

## Verification After Applying

From a permitted device — should succeed:
```
https://amelai.tail926601.ts.net:8189
```

From any other tailnet device — should get connection refused or timeout:
```
https://amelai.tail926601.ts.net:8189
```

On the server, confirm Tailscale serve is still intact:
```bash
sudo tailscale serve status
sudo tailscale status
```

---

## Related Files

| File | Purpose |
|---|---|
| `Tailscale.md` | Full Tailscale reference — serve, iptables, UFW, dual-binding explanation |
| `Software_Setup.md` | Docker run commands for all containers, service URLs |
| `CLAUDE.md` | Docker port binding strategy and current port assignments |
