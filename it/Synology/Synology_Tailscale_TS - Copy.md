# Synology DS920+ Tailscale Troubleshooting

**Device**: Synology DS920+
**DSM Version**: 7.3.2-86009 Update 3
**Goal**: Access Synology Photos (`/photos`) and Files (`/files`) via Tailscale IP
**Status**: NAS shows connected in Tailscale admin console, but HTTP access via Tailscale IP fails

---

## Problem Summary

| Access Method | Result |
|---|---|
| `<LOCALIP>/photos` | Works |
| `<LOCALIP>/files` | Works |
| `<TAILSCALEIP>/photos` | Fails |
| `<TAILSCALEIP>/files` | Fails |

---

## Likely Causes (Priority Order)

1. **DSM Firewall** blocking connections from the Tailscale subnet (`100.x.x.x`)
2. **Tailscale subnet routing not configured** — Tailscale installed but not set as exit node or subnet router
3. **DSM bind address restriction** — DSM HTTP/HTTPS ports only listening on LAN interface, not Tailscale interface
4. **Nginx reverse proxy misconfiguration** — Photos/Files apps have origin restrictions
5. **Tailscale ACL policy** blocking the traffic on the Tailscale admin side

---

## Diagnostic Steps

### Step 1 — Confirm Basic Tailscale Connectivity

From a device connected to Tailscale (not on the same LAN), try:

```bash
# Ping the NAS Tailscale IP
ping <TAILSCALEIP>

# Try to reach DSM web UI port
curl -v http://<TAILSCALEIP>:5000
curl -v https://<TAILSCALEIP>:5001
```

**Expected if Tailscale itself works**: Ping responds, DSM login page loads on 5000/5001
**If ping fails**: Tailscale connectivity issue or firewall blocking ICMP
**If ping works but HTTP fails**: Port-level firewall or DSM binding issue

---

### Step 2 — Check DSM Firewall

DSM has its own firewall that is independent of your router firewall.

1. Go to **Control Panel → Security → Firewall**
2. Check if firewall is **enabled**
3. If enabled, review firewall rules — look for rules that might block `100.x.x.x` (Tailscale subnet)
4. Tailscale uses the `100.64.0.0/10` address range

**Fix if needed**: Add a rule to **allow all** traffic from `100.64.0.0/10` (or your specific Tailscale IP range)

> ⚠️ DSM firewall rules apply per-interface. Tailscale creates its own network interface (`tailscale0`) — check that rules apply to it.

---

### Step 3 — Check Tailscale Interface in DSM

SSH into your NAS and verify the Tailscale interface is up:

```bash
# SSH to NAS
ssh admin@<LOCALIP>

# Check Tailscale interface
ip addr show tailscale0

# Check Tailscale status
/var/packages/Tailscale/target/bin/tailscale status

# Verify the Tailscale IP matches what admin console shows
```

**Look for**: `tailscale0` interface with a `100.x.x.x` address

---

### Step 4 — Check DSM Port Bindings

DSM's web services may only bind to specific interfaces.

```bash
# Check what's listening and on which interfaces
ss -tlnp | grep -E '5000|5001|80|443'

# Or using netstat
netstat -tlnp | grep -E '5000|5001|80|443'
```

**Good**: `0.0.0.0:5000` — listening on all interfaces (including Tailscale)
**Bad**: `192.168.x.x:5000` — only listening on LAN IP

**If bound to specific IP**: Go to **Control Panel → Network → DSM Settings** and check if there's an interface restriction set.

---

### Step 5 — Check Tailscale ACL Policy

In Tailscale admin console (`login.tailscale.com/admin/acls`):

1. Check the **ACL policy** — default allows all devices to reach each other, but changes may have been made
2. Verify your connecting device and the NAS are in the same tailnet
3. Check **Machines** tab — confirm both devices show as connected with green status

Default ACL that allows all traffic:
```json
{
  "acls": [
    {"action": "accept", "src": ["*"], "dst": ["*:*"]}
  ]
}
```

---

### Step 6 — Test Direct Port Access

Once you confirm which ports DSM is using, test directly:

```bash
# Test DSM UI (usually 5000 or 80)
curl -v http://<TAILSCALEIP>:5000

# Test Photos specifically (Synology Photos runs on DSM web server)
curl -v http://<TAILSCALEIP>/photos

# Test with HTTPS
curl -vk https://<TAILSCALEIP>:5001
```

---

## Resolution Log

| Date | Step | Finding | Action Taken | Result |
|---|---|---|---|---|
| 2026-03-19 | Step 1 | Ping from outside network fails despite NAS showing connected in Tailscale admin console | Tailscale tunnel is UP — investigating DSM firewall | Proceeded to further diagnosis |
| 2026-03-19 | Step 2 | DSM firewall enabled, no deny rules, all interfaces selected — not the cause | Added allow rule for 100.64.0.0/10 anyway (harmless) | Still no connectivity |
| 2026-03-19 | Step 3 | `tailscale0` interface did not exist — Tailscale running in userspace mode | `/dev/net/tun` existed but had `crw-------` (root-only) permissions; ran `sudo chmod 0666 /dev/net/tun` then restarted Tailscale | `tailscale0` interface created with correct IP |
| 2026-03-19 | Step 4 | NAS could curl its own Tailscale IP (HTTP 200) but Lenovo and Android still couldn't connect | `tcpdump -i tailscale0` showed zero inbound packets despite WireGuard traffic visible on `<NETWORK ADAPTER>` | Investigated Tailscale daemon |
| 2026-03-19 | **ROOT CAUSE** | `tailscaled.stdout.log` showed `Drop: ... no rules matched` for all inbound packets | Tailscale ACL policy only permitted traffic to `<TAILSCALE DEVICE NAME>` (100.XXX.XXX.XXX) — NAS had no ACL rules at all | Added NAS rule to ACL — **FIXED** |

---

## Resolution — What Was Fixed

### 1. TUN Device Permissions
`/dev/net/tun` existed but was `crw-------` (root-only). The Tailscale package on DSM 7 deliberately skips TUN setup (`ensure_tun_created` returns immediately on DSM 7). Without world-readable permissions, tailscaled fell back to userspace networking mode, which cannot accept inbound connections.

**Fix applied:**
```bash
sudo chmod 0666 /dev/net/tun
sudo synopkg stop Tailscale
sudo synopkg start Tailscale
```

### 2. Tailscale ACL Policy (Root Cause)
The ACL had been customised to allow traffic only to `TAILSCALE DEVICE NAME` (100.XXX.XXX.XXX) for Docker container access. The NAS had no ACL rules, so tailscaled dropped all inbound packets with `no rules matched`.

**ACL rule added** (in Tailscale admin console → Access Controls):
```json
{
    "action": "accept",
    "src":    ["*"],
    "dst": [
        "<SYNOLOGY TAILSCALE IP>:80",
        "<SYNOLOGY TAILSCALE IP>:443",
        "<SYNOLOGY TAILSCALE IP>:5000",
        "<SYNOLOGY TAILSCALE IP>:5001",
        "<SYNOLOGY TAILSCALE IP>:7001",
        "<SYNOLOGY TAILSCALE IP>:7002"
    ]
}
```

**Result:** All devices can now reach the NAS via Tailscale IP and Tailnet URL.

---

## SSL Certificate Setup

Tailscale provides free Let's Encrypt certificates for tailnet hostnames.

### Steps
1. **Tailscale admin console → DNS** — enable MagicDNS and HTTPS Certificates
2. **Generate certificate on NAS** (SSH):
   ```bash
   sudo /var/packages/Tailscale/target/bin/tailscale cert <TAILSCALE_URL.ts.net>
   ```
   Files created in current directory: `.crt` and `.key`
3. **Copy to Windows** (from Windows PC — note `-O` flag required):
   ```powershell
   scp -O <LOGIN_NAME@SYNOLOGY LOCAL IP>:~/<TAILSCALE_URL.ts.net>.crt C:\Users\<USERNAME>\Downloads\
   ```
   Key file requires permission fix first if permission denied:
   ```bash
   sudo chmod 644 ~/<TAILSCALE_URL.ts.net>.key
   ```
4. **Import in DSM**: Control Panel → Security → Certificate → Add → Import certificate
5. **Assign to services**: Control Panel → Security → Certificate → **Settings** → select the new cert for DSM default
   - ⚠️ Importing alone is not enough — must assign in Settings or the warning persists

Access via `https://<TAILSCALE_URL.ts.net>` — valid padlock, no browser warnings.

---

## SMB File Access via Tailscale

Enables `\\<SYNOLOGY TAILSCALE IP>\<share>` from any Tailscale-connected Windows PC inside or outside the network.

### ACL addition required (port 445)
Add port 445 to the NAS ACL rule in the Tailscale admin console.

### Mapping a network drive
- Browse `\\<SYNOLOGY TAILSCALE IP>\` in File Explorer to see available shares
- Right-click **This PC → Map network drive**, enter `\\<SYNOLOGY TAILSCALE IP>\<sharename>`
- Use Synology username/password if prompted
- Network Discovery won't show the NAS (broadcasts don't traverse VPN — this is expected)

Drive `I:` mapped to NAS share via Tailscale IP — confirmed working.

---

## Troubleshooting — Windows SMB Access Issues

### Local IP stopped working after Tailscale installed on NAS

**Symptom**: `\\<LOCAL IP>\<share>` worked before Tailscale was installed on the NAS, but fails afterwards. Ping to local IP still works.

**Cause**: DSM firewall rule changes made during Tailscale setup altered the rule evaluation order. With the default implicit deny-all at the bottom, LAN SMB traffic may no longer match an allow rule.

**Fix**: In DSM → **Control Panel → Security → Firewall → Edit Rules**, add an explicit Allow rule for your LAN subnet (e.g. `192.168.1.0/24`) covering all ports, and move it to the **top** of the rule list.

---

### Tailscale IP works on Win11 but not Win10 — "General failure" on ping

**Symptom**: `tailscale ping <NAS IP>` works via DERP relay but regular `ping` gives "General failure". SMB via Tailscale IP also fails. The Win11 PC works fine.

**Cause**: A second VPN client (e.g. **ProtonVPN**) running simultaneously as a WireGuard tunnel intercepts packets destined for the Tailscale address range (100.x.x.x) and fails to route them, causing "General failure" at the Windows network stack level.

**Diagnosis**:
```powershell
Get-NetAdapter | Where-Object { $_.InterfaceDescription -like "*WireGuard*" }
```
If another WireGuard adapter shows as `Up` alongside Tailscale, that is the conflict.

**Fix — Option A**: Disconnect the other VPN when using Tailscale.

**Fix — Option B**: Configure ProtonVPN split tunneling to exclude the Tailscale subnet:
1. ProtonVPN app → Settings → **Split Tunneling**
2. Enable split tunneling
3. Add exclusion: `100.64.0.0/10`

This allows both VPNs to run simultaneously without conflict.

---

### NAS short hostname (e.g. `\\irwinnas\`) not resolving when ProtonVPN active

**Symptom**: `\\<LOCAL IP>\<share>` works, `\\<TAILSCALE IP>\<share>` works, but `\\irwinnas\` fails with "network path not found". Only occurs when ProtonVPN is connected.

**Cause**: ProtonVPN overrides system DNS with its own internal DNS server (`10.2.0.1`), which has no knowledge of local hostnames. Even with split tunneling configured for IP routing, DNS is set at the adapter level and is not affected by routing exclusions.

**Diagnosis**:
```cmd
nslookup irwinnas
```
If the response shows `Server: Unknown` / `Address: 10.2.0.1`, ProtonVPN's DNS is active and local name resolution is broken.

**Fix — Option A — Hosts file (recommended)**:
Edit `C:\Windows\System32\drivers\etc\hosts` as Administrator, add:
```
192.168.1.216    irwinnas
```
Open Notepad as Administrator → File → Open → `C:\Windows\System32\drivers\etc\` → change filter to All Files → open `hosts` → add line → save. Bypasses DNS entirely — works regardless of which VPN is active.

**Fix — Option B — ProtonVPN Custom DNS**:
ProtonVPN → Settings → Connection → **Custom DNS** → enter router IP `192.168.1.1`. Replaces ProtonVPN's DNS with the local router, which resolves local hostnames. Trade-off: DNS queries go to router rather than ProtonVPN's servers.

---

## TUN Permissions Persistence (Resolved)

The `chmod 0666 /dev/net/tun` applied manually will reset on reboot. If the NAS reboots, Tailscale will start in userspace mode again and inbound connections will stop working.

### Fix — DSM Task Scheduler

Add a triggered task to set TUN permissions at boot before Tailscale starts:

1. **Control Panel → Task Scheduler → Create → Triggered Task → User-defined script**
2. Set:
   - **Task name**: Fix TUN permissions for Tailscale
   - **Event**: Boot-up
   - **User**: root
   - **Script**:
     ```bash
     chmod 0666 /dev/net/tun
     ```
3. Save and confirm

> Alternatively, the task can also restart the Tailscale package after setting permissions to guarantee it picks up the TUN device:
> ```bash
> chmod 0666 /dev/net/tun
> sleep 5
> synopkg restart Tailscale
> ```

---

## Notes

- Synology Photos (`/photos`) and Synology Drive/Files (`/files`) are served through DSM's nginx on the same ports as DSM itself
- The Tailscale package for Synology (v1.58.2) deliberately disables TUN setup on DSM 7 — this is a known limitation of the Synology package version
- The root cause of userspace fallback was `/dev/net/tun` having `crw-------` permissions, preventing the tailscale user from opening it
- Tailscale ACL changes apply instantly without requiring a restart of any service
- Key diagnostic command: `sudo cat /<VOLUME DRIVE>/@appdata/Tailscale/tailscaled.stdout.log` — shows all packet drops with reasons
