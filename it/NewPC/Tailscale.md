# Tailscale

## What is Tailscale?

Tailscale is a zero-configuration VPN built on top of **WireGuard** — a modern, fast, and lightweight VPN protocol. Where traditional VPNs route all traffic through a central server, Tailscale creates a **mesh network** (called a *tailnet*) where every device connects directly to every other device using the shortest path available.

Key concepts:

| Concept | What it means |
|---|---|
| **Tailnet** | Your private network — all your Tailscale-connected devices |
| **MagicDNS** | Each device gets a stable hostname (e.g. `myserver.tail12345.ts.net`) and a stable `100.x.x.x` IP |
| **Coordination server** | Tailscale's server handles key exchange and device authentication — it never sees your traffic |
| **DERP relay** | If direct connection fails (e.g. strict NAT), traffic is relayed through Tailscale's encrypted DERP servers |
| **Exit node** | A device that routes your internet traffic — turns Tailscale into a traditional VPN |
| **Subnet router** | A device that advertises a local subnet (e.g. `192.168.1.0/24`) to the tailnet |

Authentication is handled via OAuth (Google, GitHub, Microsoft) or OIDC. No passwords or certificates to manage.

---

## Installation

```bash
# Install (Ubuntu/Debian)
curl -fsSL https://tailscale.com/install.sh | sudo sh

# Connect and authenticate (opens browser on first run)
sudo tailscale up

# Connect headlessly (server/non-interactive — generates auth URL)
sudo tailscale up --authkey=<key>
```

Auth keys are generated at the Tailscale admin console under **Settings → Keys**.

---

## Useful Commands

### Check which ports are in use

```bash
# Show all listening TCP/UDP ports and the process using each
sudo ss -tlnup

# Filter to a specific port
sudo ss -tlnup | grep :8088
```

| Flag | Meaning |
|---|---|
| `-t` | TCP |
| `-u` | UDP |
| `-l` | Listening only |
| `-n` | Show port numbers (not service names) |
| `-p` | Show process name and PID |

### Status and diagnostics

```bash
# Show connection status and all peers
sudo tailscale status

# Show your Tailscale IP and hostname
sudo tailscale ip
sudo tailscale ip -4      # IPv4 only

# Run connectivity diagnostics (NAT type, latency, relay usage)
sudo tailscale netcheck

# Ping a peer by name or IP (through Tailscale, not ICMP)
sudo tailscale ping myserver
sudo tailscale ping 100.x.x.x
```

### Connecting and disconnecting

```bash
# Bring Tailscale up
sudo tailscale up

# Bring Tailscale down (disconnects from tailnet)
sudo tailscale down

# Log out (removes this device from the tailnet)
sudo tailscale logout
```

### Useful `up` flags

```bash
# Accept routes advertised by subnet routers
sudo tailscale up --accept-routes

# Use a device as an exit node (route internet traffic through it)
sudo tailscale up --exit-node=<tailscale-ip>

# Stop using exit node
sudo tailscale up --exit-node=

# Accept DNS from Tailscale (MagicDNS)
sudo tailscale up --accept-dns

# Advertise this machine as a subnet router
sudo tailscale up --advertise-routes=192.168.1.0/24
```

---

## Port Forwarding: Tailscale Port → Internal Service

This maps an external-facing Tailscale port to an internal service port, so services running on non-standard ports can be reached on cleaner or more memorable ports within your tailnet.

**Example**: ComfyUI runs on `8188`. You want it accessible on port `18188` so multiple instances don't conflict. Traffic arriving on Tailscale interface port `18188` is forwarded to `localhost:8188`.

There are two approaches:

| Approach | Best for |
|---|---|
| `tailscale serve` | HTTP/HTTPS services — Tailscale handles TLS automatically |
| `iptables` NAT | Raw TCP/UDP — any port, any protocol, no TLS overhead |

---

### Method 1: `tailscale serve` (HTTP/HTTPS proxy)

Tailscale terminates TLS and proxies HTTP traffic. The service is only accessible within your tailnet.

> **Important**: The port in `--https=PORT` is the **external Tailscale port** — the port you connect to from another device on your tailnet. The `http://localhost:PORT` at the end is the internal service port on the server. These are independent values. To remove a rule, you must use the same external port it was created with.

```bash
# Forward Tailscale HTTPS port 18088 to local HTTP port 8088
# Connect from tailnet using: https://myserver.tail12345.ts.net:18088
sudo tailscale serve --bg --https=18088 http://localhost:8088

# Forward on HTTP (no TLS — use only if you understand the implications)
sudo tailscale serve --bg --http=18088 http://localhost:8088

# View all active serve configurations
sudo tailscale serve status

# Remove the HTTPS route on port 18088
sudo tailscale serve --https=18088 off

# Remove the HTTP route on port 18088
sudo tailscale serve --http=18088 off

# Remove all serve configurations
sudo tailscale serve reset
```

> **Note**: `tailscale serve` automatically provisions TLS certificates via Let's Encrypt for your tailnet hostname. Services are not exposed to the public internet — only to devices in your tailnet.

---

### Method 2: `iptables` NAT (raw TCP forwarding)

Use this when `tailscale serve` is not appropriate — for non-HTTP protocols, or to avoid TLS overhead on a trusted internal network.

#### Step 1: Enable IP forwarding

```bash
# Enable temporarily (lost on reboot)
echo 1 | sudo tee /proc/sys/net/ipv4/ip_forward

# Enable permanently
echo "net.ipv4.ip_forward = 1" | sudo tee -a /etc/sysctl.conf
sudo sysctl -p
```

#### Step 2: Add the NAT and firewall rules

```bash
# Replace 18088 and 8088 with your external and internal ports

# NAT rule: redirect incoming traffic on tailscale0:18088 to localhost:8088
sudo iptables -t nat -A PREROUTING -i tailscale0 -p tcp --dport 18088 -j DNAT --to-destination 127.0.0.1:8088

# Firewall rule: allow incoming traffic on port 18088 via Tailscale
sudo iptables -A INPUT -i tailscale0 -p tcp --dport 18088 -j ACCEPT

# Masquerade rule: required for DNAT to loopback to work
sudo iptables -t nat -A POSTROUTING -o lo -j MASQUERADE
```

#### Step 3: Allow the internal service port (if not already open)

If the service only needs to be reachable through Tailscale (not on the local LAN), ensure the input rule on the service's own port is scoped to `lo` (loopback) only:

```bash
# Allow the internal port on loopback (the forwarded destination)
sudo iptables -A INPUT -i lo -p tcp --dport 8088 -j ACCEPT

# Block direct access to the internal port from other interfaces (optional hardening)
sudo iptables -A INPUT ! -i lo -p tcp --dport 8088 -j DROP
```

#### Step 4: Make the rules persistent

```bash
# Install iptables-persistent (saves/restores rules on reboot)
sudo apt install iptables-persistent

# Save current rules
sudo netfilter-persistent save

# Or save manually
sudo iptables-save | sudo tee /etc/iptables/rules.v4
```

---

### Removing iptables port forward rules

To remove a specific port forwarding rule, use `-D` (delete) with the exact same arguments used to add it:

```bash
# Remove the NAT rule
sudo iptables -t nat -D PREROUTING -i tailscale0 -p tcp --dport 18088 -j DNAT --to-destination 127.0.0.1:8088

# Remove the firewall INPUT rule
sudo iptables -D INPUT -i tailscale0 -p tcp --dport 18088 -j ACCEPT

# Remove the loopback INPUT allow (if added)
sudo iptables -D INPUT -i lo -p tcp --dport 8088 -j ACCEPT

# Remove the drop rule (if added)
sudo iptables -D INPUT ! -i lo -p tcp --dport 8088 -j DROP

# Save after removing
sudo netfilter-persistent save
```

> **Tip**: To list rules with line numbers before deleting, use:
> ```bash
> sudo iptables -L INPUT --line-numbers
> sudo iptables -t nat -L PREROUTING --line-numbers
> ```
> You can then delete by line number: `sudo iptables -D INPUT 3`

---

## UFW (if using Ubuntu Firewall instead of raw iptables)

If the server uses `ufw` rather than raw iptables:

```bash
# Allow the Tailscale-facing port
sudo ufw allow in on tailscale0 to any port 18088 proto tcp

# Allow the internal loopback port
sudo ufw allow in on lo to any port 8088 proto tcp

# Remove a rule
sudo ufw delete allow in on tailscale0 to any port 18088 proto tcp

# Show all rules with numbers
sudo ufw status numbered

# Delete by number
sudo ufw delete 3
```

> **Important**: `ufw` and raw `iptables` both manipulate the same underlying netfilter rules but do not coordinate. Use one or the other — do not mix them on the same machine.

---

## Quick Reference: Common Port Mappings

Current service configuration on this system:

| Service | Docker host port | Tailscale port | Connect via |
|---|---|---|---|
| ComfyUI (main) | 18188 | 8188 | `https://amelai.tail926601.ts.net:8188` |
| ComfyUI (Amelia) | 18189 | 8189 | `https://amelai.tail926601.ts.net:8189` |
| FileBrowser | 18087 | 8087 | `https://amelai.tail926601.ts.net:8087` |
| Open WebUI | 3000 | 443 | `https://amelai.tail926601.ts.net` |

Run `sudo tailscale serve status` to see all active configurations at any time.

---

## Troubleshooting

### Diagnose the current state

These are the first commands to run when something isn't working as expected:

```bash
# Show what Tailscale serve has configured (external port → internal port)
sudo tailscale serve status

# Show what is actually listening on the host and which process owns it
sudo ss -tlnup

# Filter to a specific port
sudo ss -tlnup | grep :18087
```

### Common mistakes

**Removing a rule with the wrong port**

The port in `--https=PORT` is the **external Tailscale port** (what you connect to), not the internal service port. To remove a rule you must use the same external port it was created with.

```bash
# If the serve status shows:
# https://amelai.tail926601.ts.net:8087  →  http://localhost:18087

# WRONG - tries to remove a rule on external port 18087 (doesn't exist)
sudo tailscale serve --https=18087 off

# CORRECT - removes the rule on external port 8087
sudo tailscale serve --https=8087 off
```

**Connecting with the wrong protocol**

Tailscale serve creates HTTPS endpoints. Browsers won't automatically use HTTPS on non-standard ports — you must include the protocol explicitly:

```
# WRONG - browser tries HTTP, gets no response
http://amelai.tail926601.ts.net:8087

# CORRECT
https://amelai.tail926601.ts.net:8087
```

**Docker bypassing Tailscale serve**

When Docker publishes a port as `0.0.0.0:18087`, that port is accessible directly on the Tailscale IP — bypassing Tailscale serve entirely. This means:
- Users can reach the service on the raw Docker port (`18087`) over plain HTTP
- Your Tailscale serve configuration on `8087` is not the only way in

The fix is covered in the next section.

---

## Securing Services: Bind Docker to Loopback

By default, Docker binds published ports to `0.0.0.0` (all interfaces), which includes the Tailscale IP. This allows direct HTTP access to the container, bypassing Tailscale serve and its TLS.

The fix is to bind the Docker port to `127.0.0.1` (loopback) only. The container is then only reachable from localhost — which is exactly what Tailscale serve uses when it forwards traffic. Direct access via the Tailscale IP is refused.

### How to change the binding

Stop and remove the container, then re-run with `127.0.0.1:` prefixed to the host port in the `-p` flag:

```bash
docker stop filebrowser
docker rm filebrowser

docker run -d \
  --name filebrowser \
  --restart always \
  --network ai-network \
  -p 127.0.0.1:18087:80 \
  -v /home/steve/rag-output:/srv \
  -v /home/steve/filebrowser/filebrowser.db:/database/filebrowser.db \
  filebrowser/filebrowser:latest
```

The only change is `-p 18087:80` → `-p 127.0.0.1:18087:80`.

### What to check after

**1. Confirm the port is now loopback-only:**

```bash
sudo ss -tlnup | grep 18087
```

Before: `0.0.0.0:18087` — After: `127.0.0.1:18087`

**2. Confirm Tailscale serve still works:**

```bash
sudo tailscale serve status
```

The rule should still show `https://amelai.tail926601.ts.net:8087 → http://localhost:18087`. Open the URL in a browser to confirm the service loads.

**3. Confirm direct HTTP access is now refused:**

Try connecting to `http://amelai.tail926601.ts.net:18087` in a browser — it should get a connection refused, not a page.

### Apply to other containers

Use the same pattern for any container you want accessible only via Tailscale serve:

| Before | After |
|---|---|
| `-p 18188:8188` | `-p 127.0.0.1:18188:8188` |
| `-p 18189:8189` | `-p 127.0.0.1:18189:8189` |
| `-p 3000:3000` | `-p 127.0.0.1:3000:3000` |

---

### Dual binding — local network and Tailscale access

Binding to loopback only means the service is no longer reachable on the local network — only through Tailscale serve. If you also want to connect directly when you are on the local network (e.g. `http://192.168.1.192:8087`), add a second `-p` binding for the LAN IP alongside the loopback one.

You cannot use `0.0.0.0` for the LAN binding because Tailscale serve is already listening on the Tailscale IP for that port — binding `0.0.0.0` would include the Tailscale IP and cause a conflict. Binding to the specific LAN IP avoids this.

```bash
docker stop filebrowser
docker rm filebrowser

docker run -d \
  --name filebrowser \
  --restart always \
  --network ai-network \
  -p 127.0.0.1:18087:80 \
  -p 192.168.1.192:8087:80 \
  -v /home/steve/rag-output:/srv \
  -v /home/steve/filebrowser/filebrowser.db:/database/filebrowser.db \
  filebrowser/filebrowser:latest
```

This gives two independent access paths:

| Access method | URL | Protocol | Route |
|---|---|---|---|
| Local network | `http://192.168.1.192:8087` | HTTP | Direct to Docker container |
| Tailscale (remote) | `https://amelai.tail926601.ts.net:8087` | HTTPS | Via Tailscale serve → localhost:18087 |

### What to check after

```bash
sudo ss -tlnup | grep 8087
```

You should see three entries for port 8087 and 18087:

| Address | Process | What it is |
|---|---|---|
| `127.0.0.1:18087` | docker-proxy | Loopback binding — Tailscale serve connects here |
| `192.168.1.192:8087` | docker-proxy | LAN binding — direct local network access |
| `100.79.83.113:8087` | tailscaled | Tailscale serve listener — remote HTTPS access |

You will also see IPv6 equivalents of the docker-proxy entries — these are normal.

> **Important**: This only works reliably if the server has a static LAN IP. If your server gets its IP from DHCP and the address changes, the `192.168.1.192` binding will fail silently on restart. Set a static DHCP reservation on your router (binding the server's MAC address to `192.168.1.192`) to prevent this.

### How the two access paths work independently

The `-p 192.168.1.192:8087:80` flag and `https://amelai.tail926601.ts.net:8087` are completely independent mechanisms — your Docker `-p` configuration plays no part in the Tailscale path.

**Local network path** (`http://192.168.1.192:8087`)

You configured this entirely via the `-p` flag. Docker listens on the LAN interface at port 8087 and forwards the connection directly into the container. Plain HTTP, nothing else involved.

**Tailscale path** (`https://amelai.tail926601.ts.net:8087`)

Tailscale serve handles this entirely. It listens on the Tailscale interface (`100.79.83.113:8087`), terminates the HTTPS/TLS connection, then makes a separate plain HTTP request to `localhost:18087` — the loopback binding — which Docker forwards into the container. Your `-p` flags play no part in this beyond providing the loopback target for Tailscale serve to connect to.

```
LOCAL NETWORK CLIENT
  http://192.168.1.192:8087
         │
         ▼
  Docker -p 192.168.1.192:8087:80
         │
         ▼
  Container port 80  (FileBrowser)


TAILSCALE CLIENT
  https://amelai.tail926601.ts.net:8087
         │
         ▼
  Tailscale serve (100.79.83.113:8087) — terminates TLS
         │  makes internal HTTP request
         ▼
  Docker -p 127.0.0.1:18087:80
         │
         ▼
  Container port 80  (FileBrowser)
```

The Docker `-p` flags simply control which host interfaces and ports are open. What happens after a connection arrives — whether it is Tailscale serve making an internal request or a browser on your LAN connecting directly — is independent of Docker entirely.
