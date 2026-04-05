# Wake on LAN — Remote PC Wake Guide

This guide covers how to remotely wake your PCs (Ubuntu AI PC, Windows 10, Windows 11) when away from the local network, using Tailscale as the secure remote access layer.

---

## How Wake on LAN Works

Wake on LAN (WoL) works by sending a **magic packet** to a target machine's network card. The packet contains:

- 6 bytes of `0xFF` (synchronisation stream)
- The target's MAC address repeated 16 times

The NIC stays partially powered when the PC is off and monitors for this packet. When it sees its own MAC address in the magic packet format, it signals the motherboard to power on.

**The fundamental problem with remote WoL:** Magic packets are Layer 2 (LAN-level) broadcasts — they do not cross routers or VPN tunnels. Every remote WoL method is a workaround for this. Even Tailscale cannot inject Layer 2 broadcasts onto the LAN directly (it operates at Layer 3/IP). The solution in all cases: **use a Tailscale-connected device physically on the LAN to send the magic packet locally.**

### ACPI Power States

| State | Name | WoL support |
|---|---|---|
| S3 | Sleep (RAM powered) | Universally supported |
| S4 | Hibernate (state saved to disk) | Usually supported |
| S5 | Soft Off (fully shut down, PSU still live) | Depends on BIOS/NIC |

S5 (full shutdown) is the most useful but least reliably supported. Many BIOSes disable it by default due to energy regulations.

---

## Prerequisites: BIOS Settings

Must be done on each PC before any software configuration. Without BIOS support, the NIC has no standby power and cannot listen for magic packets.

### Settings to Enable

| Setting | Value | Notes |
|---|---|---|
| Wake on LAN / Wake by PCI-E | Enabled | Primary WoL setting |
| ErP Ready / ErP Support | **Disabled** | Cuts NIC standby power if enabled — kills WoL from S5 |
| Deep Sleep / Deep S5 | **Disabled** | Prevents wake from full shutdown |
| Power On by PCIE | Enabled | Terminology varies by manufacturer |

### ASUS ProArt X870E-CREATOR WIFI (Amelai — AI PC)

1. Press `Del` on boot to enter BIOS
2. Press `F7` to switch to Advanced Mode (if in EZ Mode)
3. Navigate: **Advanced** > **APM Configuration**
4. Set **ErP** → `Disabled` *(critical — any "Enabled" option cuts NIC standby power and breaks WoL from S5)*
5. Set **Power On By PCI-E** → `Enabled`
6. Press `F10` > `OK` to save and exit

> **Dual NIC note:** The ProArt X870E has two NICs (10GbE Intel X550-AT2 and 2.5GbE Intel I226-V). `Power On By PCI-E` covers both since both are PCIe-connected — no separate BIOS setting is needed per NIC.

**References:** <a href="https://www.asus.com/support/faq/1045950/" target="_blank">ASUS: How to enable WoL in BIOS</a> | ASUS BIOS Manual E25269, Section 6.11

---

### MSI X870E Tomahawk (Windows 11 build)

Navigate: **BIOS > Settings > Advanced > Wake Up Event Setup**
- **Resume by PCIE Device** → `Enabled`
- **ErP Ready** → `Disabled`

---

## Linux Setup (Ubuntu 24.04 AI PC)

### Step 1 — Find your interface name

```bash
ip link
```

Look for your wired interface (typically `eth0`, `enp3s0`, `enp6s0`, etc.).

### Step 2 — Check current WoL status

```bash
sudo ethtool <interface> | grep -i wake
```

`d` = disabled, `g` = magic packet enabled (what you want).

### Step 3 — Enable WoL persistently via NetworkManager

Ubuntu 24.04 uses NetworkManager — this is the cleanest approach:

```bash
# List connections to find the exact connection name
nmcli connection show

# Enable WoL magic packet
nmcli connection modify "Wired connection 1" 802-3-ethernet.wake-on-lan magic

# Apply immediately
nmcli connection up "Wired connection 1"

# Verify
nmcli connection show "Wired connection 1" | grep wake-on-lan
```

NetworkManager persists this across reboots automatically.

### Step 4 — Get the MAC address

```bash
ip link show <interface>
```

Note the `link/ether` value (e.g. `aa:bb:cc:dd:ee:ff`) — you'll need this.

---

## Windows 10 / 11 Setup

### Step 1 — BIOS settings (as above)

### Step 2 — Device Manager

1. Right-click Start > **Device Manager**
2. Expand **Network Adapters** > right-click your Ethernet adapter > **Properties**
3. **Power Management** tab:
   - Check **Allow this device to wake the computer**
   - Check **Only allow a magic packet to wake the computer**
4. **Advanced** tab:
   - **Wake on Magic Packet** → Enabled
   - **Wake on Pattern Match** → Disabled (reduces false wakes)
   - **Enable PME** → Enabled (if present)

### Step 3 — Disable Fast Startup (important)

Fast Startup puts Windows into a hybrid hibernate state (S4) rather than a true S5 shutdown, which interferes with WoL on many systems.

**Control Panel > Power Options > Choose what the power buttons do > Change settings that are currently unavailable > uncheck "Turn on fast startup"**

Or via PowerShell (run as Administrator):

```powershell
powercfg /h off
```

### Get the MAC address

```powershell
ipconfig /all
```

Look for **Physical Address** under your Ethernet adapter.

---

## Remote WoL Methods

### Method 1 — SSH into AI PC and send magic packet (Recommended)

**Best for:** Quickest setup, most reliable, zero new dependencies.

The AI PC is always on and already on Tailscale. SSH in and use `wakeonlan` to send the magic packet from within the LAN.

**Setup on the AI PC:**

```bash
sudo apt install wakeonlan
```

**Usage from anywhere:**

```bash
# SSH into AI PC via Tailscale
ssh user@amelai

# Wake a Windows PC
wakeonlan AA:BB:CC:DD:EE:FF

# Specify broadcast address explicitly (more reliable)
wakeonlan -i 192.168.1.255 AA:BB:CC:DD:EE:FF
```

**Optional — create a convenience script** at `/usr/local/bin/wake-pc` on the AI PC:

```bash
#!/bin/bash
case "$1" in
  win10) wakeonlan -i 192.168.1.255 AA:BB:CC:DD:EE:FF ;;   # Windows 10 MAC — update this
  win11) wakeonlan -i 192.168.1.255 BB:CC:DD:EE:FF:00 ;;   # Windows 11 MAC — update this
  *) echo "Usage: wake-pc [win10|win11]" ;;
esac
```

```bash
sudo chmod +x /usr/local/bin/wake-pc
```

Then from anywhere: `ssh user@amelai wake-pc win11`

**Security:** Excellent — all traffic through Tailscale's encrypted WireGuard tunnel. No ports exposed to the internet.

---

### Method 2 — UpSnap (browser-based GUI, optional)

**Best for:** If you want a mobile-friendly web UI rather than SSH.

UpSnap is a self-hosted web app that provides a dashboard for waking and monitoring devices. It runs on the AI PC in Docker and is accessible via Tailscale.

**Important:** UpSnap must use `network_mode: host` so it can send Layer 2 broadcasts onto the physical LAN interface. This bypasses Docker's bridge network.

**Docker Compose (`~/docker/upsnap/docker-compose.yml`):**

```yaml
services:
  upsnap:
    image: ghcr.io/seriousm4x/upsnap:latest
    network_mode: host
    restart: unless-stopped
    environment:
      - TZ=Europe/London
      - UPSNAP_SCAN_RANGE=192.168.1.0/24
      - UPSNAP_HTTP_LISTEN=0.0.0.0:8090
    volumes:
      - ./upsnap_data:/app/pb_data
```

```bash
docker compose up -d
```

**Access:** `http://192.168.1.192:8090` (LAN) or via Tailscale on port 8090.

> **Note:** Because `network_mode: host` is used, this does not follow the standard dual `-p` binding pattern. UpSnap binds directly on the host network. Access it on the Tailscale IP at port 8090, or add a Tailscale serve rule if HTTPS is wanted.

**Security:** Keep UpSnap Tailscale-only — do not expose to the open internet. UpSnap's own documentation warns against this.

**References:** <a href="https://tailscale.com/blog/wake-on-lan-tailscale-upsnap" target="_blank">Tailscale Blog: Wake-on-LAN with Tailscale and UpSnap</a> | <a href="https://github.com/seriousm4x/UpSnap" target="_blank">GitHub: UpSnap</a>

---

### Method 3 — DrayTek Router Built-in WoL

**Best for:** Backup option, or if the AI PC were ever off.

The DrayTek Vigor 2865 can generate magic packets from within the LAN itself. When accessed via VPN (e.g. SSL VPN or L2TP), you can trigger WoL from the router's web UI.

**Setup:**

1. Bind the target PC's MAC to its IP: **LAN > Bind IP to MAC**
2. To wake via router UI: **Applications > Wake on LAN > select device > Wake Up**

**Access the router remotely:**
- Via your existing VPN setup into the DrayTek
- Or enable **System Maintenance > Management > Remote Management** and access the router UI via Tailscale exit node (not recommended — prefer VPN to router)

**Reference:** <a href="https://www.draytek.co.uk/support/guides/kb-wake-on-lan" target="_blank">DrayTek: Wake-on-LAN Guide</a>

---

### Method 4 — Port Forwarding (Not Recommended)

Forward UDP port 9 from the internet to the PC's LAN IP. This was the pre-VPN era approach.

**Security risks:**

- UDP port 9 can participate in Smurf amplification DDoS attacks
- Internet scanners can continuously wake your PC
- Exposes your WAN IP and MAC address unnecessarily

**Verdict:** Given Tailscale is already deployed, there is no reason to use port forwarding. Methods 1 and 2 are superior in every dimension.

---

## Summary: Which Method to Use

| Method | Setup effort | Requires | Best for |
|---|---|---|---|
| SSH + `wakeonlan` | Low | AI PC on, SSH access | Primary method — simplest and most reliable |
| UpSnap web UI | Medium | Docker on AI PC | Mobile/GUI access |
| DrayTek router UI | Low | Router VPN access | Backup; useful if AI PC were off |
| Port forwarding | Low | Router config | **Not recommended** — security risks |

**Recommended setup:** Install `wakeonlan` on the AI PC (Method 1) now. Add UpSnap (Method 2) if you want a GUI. Note the DrayTek option as a backup.

---

## Why Tailscale Cannot Send Magic Packets Directly

Tailscale operates at OSI Layer 3 (IP routing). Magic packets are Layer 2 frames that must be broadcast on a physical network segment. Even with subnet routing enabled on a Tailscale node, Tailscale cannot inject Layer 2 broadcasts onto the LAN.

This is a documented limitation confirmed in <a href="https://github.com/tailscale/tailscale/issues/306" target="_blank">Tailscale GitHub issue #306</a> and the <a href="https://tailscale.com/blog/wake-on-lan-tailscale-upsnap" target="_blank">official Tailscale blog post on WoL</a>.

---

## References

- <a href="https://en.wikipedia.org/wiki/Wake-on-LAN" target="_blank">Wikipedia: Wake-on-LAN</a>
- <a href="https://wiki.archlinux.org/title/Wake-on-LAN" target="_blank">ArchWiki: Wake-on-LAN</a>
- <a href="https://ubuntuhandbook.org/index.php/2024/08/enable-wake-on-lan-ubuntu/" target="_blank">UbuntuHandbook: Enable WoL on Ubuntu 24.04</a>
- <a href="https://tailscale.com/blog/wake-on-lan-tailscale-upsnap" target="_blank">Tailscale Blog: Wake-on-LAN with Tailscale and UpSnap</a>
- <a href="https://github.com/tailscale/tailscale/issues/306" target="_blank">Tailscale GitHub Issue #306: WoL packet support</a>
- <a href="https://github.com/seriousm4x/UpSnap" target="_blank">GitHub: UpSnap self-hosted WoL dashboard</a>
- <a href="https://www.draytek.co.uk/support/guides/kb-wake-on-lan" target="_blank">DrayTek: WoL Guide for Vigor Routers</a>
- <a href="https://learn.microsoft.com/en-us/troubleshoot/windows-client/setup-upgrade-and-drivers/wake-on-lan-feature" target="_blank">Microsoft: Wake-on-LAN behaviour in Windows 10/11</a>
