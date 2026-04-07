# Linux Server Troubleshooting Guide

Reference system: **amelai** (Ubuntu 24.04 LTS Server, dual RTX 3090, Ryzen 9 7900X)

---

## Issue 1: Server Crashed / Became Unresponsive (SSH Dead)

When the server becomes unreachable via SSH and requires a physical reboot, the goal is to find the cause in the logs **after** the reboot. Systemd's `journald` persists logs across reboots (on Ubuntu 24.04), making post-mortem analysis straightforward.

---

### Step 1: Confirm the Previous Boot

After rebooting, confirm journald has logs from the crashed session:

```bash
# List all recorded boots (most recent first when reversed)
journalctl --list-boots
```

Output example:
```
-1  abc123...  Mon 2026-03-24 09:10:00 UTC—Mon 2026-03-24 14:32:15 UTC
 0  def456...  Mon 2026-03-24 14:33:01 UTC—present
```

- Boot `0` = current boot
- Boot `-1` = the previous (crashed) session

---

### Step 2: Review the Previous Boot's Logs

```bash
# All logs from the previous boot
journalctl -b -1

# Paginated (recommended for large logs)
journalctl -b -1 | less

# Only errors and critical messages from the previous boot
journalctl -b -1 -p err

# Last 200 lines of the previous boot (tail end, just before crash)
journalctl -b -1 | tail -200

# Specific time window (e.g., the hour before you left)
journalctl -b -1 --since "2026-03-24 08:00" --until "2026-03-24 09:00"
```

---

### Step 3: Check for Out-of-Memory (OOM) Kills

OOM kills are one of the most common causes of instability on AI servers with heavy memory workloads. The kernel kills processes to reclaim RAM, which can cascade and destabilise the system.

```bash
# Search previous boot logs for OOM events
journalctl -b -1 | grep -i "out of memory\|oom\|killed process\|oom-kill"

# Alternatively
journalctl -b -1 -k | grep -i oom
```

**What to look for:**
```
kernel: Out of memory: Killed process 12345 (python3) total-vm:...
kernel: oom_reaper: reaped process 12345 ...
```

If you see OOM kills, note **which process** was killed. If Ollama, a Docker container, or a ComfyUI worker was killed, this is likely the cause.

---

### Step 4: Check Kernel Logs for Hardware Errors

The kernel ring buffer captures hardware-level events including GPU errors, storage errors, and CPU exceptions.

```bash
# Kernel messages from the previous boot only
journalctl -b -1 -k

# Kernel messages with errors only
journalctl -b -1 -k -p err

# Look for GPU/NVIDIA errors
journalctl -b -1 -k | grep -i "nvidia\|gpu\|xid\|nvrm"

# Look for storage errors (NVMe, SATA)
journalctl -b -1 -k | grep -i "nvme\|ata\|i/o error\|blk_update_request"

# Look for CPU/memory hardware errors (MCE = Machine Check Exception)
journalctl -b -1 -k | grep -i "mce\|machine check\|corrected\|uncorrected"
```

**NVIDIA XID errors** are particularly important for GPU crash diagnosis:

| XID Code | Meaning |
|---|---|
| XID 79 | GPU has fallen off the bus (hardware fault or power issue) |
| XID 43 | GPU stopped processing (hang) |
| XID 31 | GPU memory page fault |
| XID 48 | Double Bit ECC Error (memory corruption — serious) |
| XID 45 | Preemptive channel removal (driver killed a process) |

---

### Step 5: Check System Temperature and Thermal Throttling

Thermal shutdown or extreme throttling can cause an unresponsive system.

```bash
# Check if thermal events appear in logs
journalctl -b -1 | grep -i "thermal\|temperature\|overheat\|throttl"

# After rebooting, check current temps to understand baseline
sensors  # requires: sudo apt install lm-sensors && sudo sensors-detect
```

For GPU temperatures from the previous boot, NVIDIA logs are the main source via journalctl XID checks above.

---

### Step 6: Check SSH / Network Logs

Determine whether SSH died due to a network issue, sshd crash, or system-wide freeze.

```bash
# SSH daemon logs from previous boot
journalctl -b -1 -u ssh
journalctl -b -1 -u sshd

# Network manager / networking
journalctl -b -1 -u NetworkManager
journalctl -b -1 -u systemd-networkd

# Tailscale (if using Tailscale for remote access)
journalctl -b -1 -u tailscaled
```

If sshd was running fine until a hard cutoff with no graceful shutdown messages, the system froze or panicked rather than SSH crashing in isolation.

---

### Step 7: Look for Kernel Panic

A kernel panic causes an immediate hard halt with no graceful shutdown. Signs in logs:

```bash
# Check for panic in kernel messages
journalctl -b -1 -k | grep -i "panic\|bug:\|oops\|rcu_sched\|hung_task"

# Check if the previous boot ended cleanly or abruptly
journalctl -b -1 | tail -30
```

An abrupt end (logs just stop, no "shutdown" or "poweroff" messages) strongly suggests a hard crash, kernel panic, or power loss.

---

### Step 8: Check Docker Container Logs

If a Docker container (Ollama, Open WebUI, ComfyUI) caused the crash:

```bash
# List all containers including stopped ones
docker ps -a

# View logs from a specific container (e.g., ollama)
docker logs ollama --tail 100

# For a container that may have exited, logs are still available
docker logs comfyui-amelia --tail 100 2>&1 | grep -i "error\|fatal\|killed\|oom"
```

---

### Step 9: Check System Uptime and Shutdown Records

```bash
# Show reboot/shutdown history
last reboot | head -20

# Show when system went down
last -x | grep -E "shutdown|reboot" | head -10

# Who/what was logged in at the time
last | head -20
```

---

### Quick Triage Checklist

Run these in order after a crash — takes about 2 minutes:

```bash
# 1. Confirm previous boot exists
journalctl --list-boots | head -5

# 2. Errors from the crash session
journalctl -b -1 -p err --no-pager | tail -50

# 3. OOM kills
journalctl -b -1 | grep -i "out of memory\|oom kill"

# 4. GPU/NVIDIA errors
journalctl -b -1 -k | grep -i "nvidia\|xid\|nvrm"

# 5. Abrupt end check (did logs stop suddenly?)
journalctl -b -1 | tail -20
```

---

### Interpreting Results

| Finding | Likely Cause | Next Step |
|---|---|----|---|
| OOM kill of Ollama/Python | Model too large for RAM+VRAM | Reduce loaded models; set `OLLAMA_MAX_LOADED_MODELS=1` |
| NVIDIA XID 79 | GPU fell off bus | Check PCIe slot seating; check power connectors |
| NVIDIA XID 43/48 | GPU hang or memory error | Stress test GPU; may indicate hardware fault |
| MCE errors | CPU/RAM hardware error | Run `memtest86+`; check CPU cooler |
| Logs stop abruptly, no panic | Kernel panic or power loss | Check power supply; check for kernel panic logs in `/var/crash/` |
| SSH service died, system fine | sshd bug or config issue | Restart sshd; check `/etc/ssh/sshd_config` |
| Thermal messages | Overheating | Check fan curves; clean dust; improve airflow |
| Docker OOM | Container exceeded memory limits | Set `--memory` limits on containers |
| `igc PCIe link lost, device now detached` | NIC dropped off PCIe bus (Intel igc bug) | See Issue 2 below — igc driver has been blacklisted; Aquantia NIC is now primary |

---

### Persistent Log Configuration (Ubuntu 24.04)

Ubuntu 24.04 stores journal logs persistently by default at `/var/log/journal/`. Verify:

```bash
# Confirm persistent storage is active
journalctl --disk-usage

# If logs are not persisting across reboots, enable it:
sudo mkdir -p /var/log/journal
sudo systemctl restart systemd-journald
```

If `/var/log/journal/` does not exist, logs only live in RAM and are lost on reboot.

---

## Issue 2: NIC Drops Off PCIe Bus — `igc PCIe link lost`

### Background — Two Incidents

This issue occurred twice. The first attempt at a fix (`pcie_aspm=off`) did not prevent recurrence. The permanent fix (switching to the Aquantia NIC and blacklisting igc) was applied after the second incident.

---

### Incident 1 (amelai, 2026-03-24)

At 10:21:38 UTC the Intel igc NIC lost its PCIe connection:

```
Mar 24 10:21:38 amelai kernel: igc 0000:0b:00.0 ethernet10g: PCIe link lost, device now detached
```

This triggered a kernel oops. The system kept running normally for ~5 hours but was completely unreachable via SSH or Tailscale. A physical reboot was required.

**This is not a system crash** — the server was healthy but cut off from the network.

---

### Incident 2 (amelai, 2026-04-06)

The NIC crashed again at 20:16:43 UTC (21:16 BST) — approximately 5.5 hours after boot:

```
Apr 06 20:16:43 amelai kernel: igc 0000:0b:00.0 ethernet10g: PCIe link lost, device now detached
Apr 06 20:16:43 amelai kernel: igc: Failed to read reg 0xc030!
```

The `pcie_aspm=off` kernel parameter was confirmed active in `/proc/cmdline` during this boot — it did **not** prevent the crash. The Intel I226-V igc driver has a deeper bug that ASPM disabling alone cannot fix.

---

### Root Cause

The Intel I225-V and I226-V NICs (used by the `igc` driver) have a known driver/firmware-level bug on Linux. The NIC enters a state it cannot recover from, causing the kernel to detach the device. `pcie_aspm=off` addresses one trigger (ASPM power state transitions) but not the underlying instability.

The ASUS ProArt X870E-CREATOR WIFI has a second NIC — an **Aquantia AQC113 10GbE** (Marvell `atlantic` driver) — which does not have this bug. The permanent fix is to use that NIC instead.

---

### Permanent Fix: Switch to Aquantia NIC and Blacklist igc

**Current state after fix (2026-04-06):**

| Interface | Driver | Chip | IP | Role |
|---|---|---|---|---|
| `ethernet2_5g` | atlantic | Aquantia AQC113 | 192.168.1.192 | Primary (metric 100) |
| `ethernet10g` | igc (blacklisted) | Intel I226-V | 192.168.1.193 | Disabled — igc driver blacklisted |

**Step 1: Edit `/etc/netplan/*.yaml`**

Make the Aquantia NIC primary (metric 100, main IP) and the Intel NIC secondary (metric 200, optional):

```yaml
network:
  version: 2
  renderer: networkd
  ethernets:
    ethernet2_5g:
      match:
        macaddress: a0:ad:9f:1c:cf:c9
      set-name: ethernet2_5g
      addresses:
        - 192.168.1.192/24
      routes:
        - to: default
          via: 192.168.1.1
          metric: 100
      nameservers:
        addresses: [8.8.8.8, 8.8.4.4]

    ethernet10g:
      match:
        macaddress: 30:c5:99:a7:07:1b
      set-name: ethernet10g
      optional: true
      addresses:
        - 192.168.1.193/24
      routes:
        - to: default
          via: 192.168.1.1
          metric: 200
      nameservers:
        addresses: [8.8.8.8, 8.8.4.4]
```

**Step 2: Move the ethernet cable** from the Intel port to the Aquantia port on the rear I/O panel.

**Step 3: Apply the netplan change**

```bash
sudo netplan apply
```

**Step 4: Blacklist the igc driver** so it cannot load and crash even as a secondary device:

```bash
echo "blacklist igc" | sudo tee /etc/modprobe.d/blacklist-igc.conf
sudo update-initramfs -u
```

Takes effect on next reboot.

**Step 5: Verify**

```bash
ip addr show ethernet2_5g
# Should show inet 192.168.1.192/24 and state UP
```

---

### Note on the "Modules linked in" Output

When the kernel oops was triggered by the NIC failure, the crash dump included a full list of loaded kernel modules — including `nvidia(O)`, `nvidia_drm`, etc. This is **not** a GPU error. NVIDIA appearing in a kernel oops module list simply means the NVIDIA driver was loaded at the time. Always read the first line of a kernel oops to find the actual cause (`igc PCIe link lost` in this case), not the module list.

---

## Issue 3: Ollama OOM Kills — ComfyUI VRAM Contention

### What Happened (amelai, 2026-03-23)

Ollama was OOM-killed four times in ~7 minutes at noon:

```
Mar 23 12:02:49 — ollama killed (anon-rss: ~40GB)
Mar 23 12:04:06 — ollama killed (anon-rss: ~40GB)
Mar 23 12:05:10 — ollama killed (anon-rss: ~40GB)
Mar 23 12:09:45 — ollama killed (anon-rss: ~40GB)
```

Ollama kept restarting and immediately being killed each time. The 40GB `anon-rss` means Ollama was loading the model into **system RAM**, not VRAM.

---

### Root Cause: Insufficient VRAM Due to ComfyUI Holding Models

The previous evening, ComfyUI had been used until ~11:30pm with the **Qwen-Rapid-AIO-NSFW-v23** image-to-image model (28.4GB VRAM). ComfyUI does not unload models from VRAM when idle — it holds them indefinitely until the container is restarted.

At noon the next day (~12.5 hours later), a request for **qwen3.5:35b** arrived via Ollama:

```
ComfyUI holding:       28.4GB VRAM
Available VRAM:        48.0 - 28.4 = 19.6GB
qwen3.5:35b needs:     ~26-28GB
Result:                Cannot fit in VRAM → falls back to system RAM
System RAM consumed:   ~40GB → exceeds available RAM → OOM kill
```

Ollama's `Restart=always` in its systemd service caused it to restart immediately each time, triggering the same failure repeatedly.

---

### Fix 1: Free ComfyUI VRAM via Browser Bookmarklet

ComfyUI has a `/free` API endpoint that unloads models from VRAM without restarting the container. Create a browser bookmarklet to call it when finishing a session.

**Create the bookmarklet in Edge:**
1. Add any page as a Favourite (click the star in the address bar)
2. Show Favourites bar: **Ctrl+Shift+B**
3. Right-click the new favourite → **Edit favourite**
4. Set Name: `Free ComfyUI VRAM`
5. Replace the URL with:

```
javascript:fetch('/free',{method:'POST',headers:{'Content-Type':'application/json'},body:'{"unload_models":true,"free_memory":true}'}).then(r=>{if(r.ok){alert('VRAM freed!')}else{alert('Failed: HTTP '+r.status)}}).catch(e=>alert('Error: '+e))
```

**Usage**: Navigate to the ComfyUI tab you want to free, then click the bookmarklet. A popup will confirm `VRAM freed!`. Works for both Steve's (port 8189) and Amelia's (port 8188) ComfyUI instances.

> **Note**: The `/free` endpoint returns an empty response body. Using `.json()` to parse it will throw `SyntaxError: Unexpected end of JSON input`. Use `.ok` status check instead (as above).

---

### Fix 2: Cron Jobs to Restart ComfyUI Containers Overnight

As a safety net if the bookmarklet is not clicked, restart both ComfyUI containers nightly at 2am:

```bash
crontab -e
```

Add:
```
0 2 * * * docker restart comfyui-steve
0 2 * * * docker restart comfyui-amelia
```

This ensures VRAM is always free by morning regardless of what was left loaded.

---

### VRAM Budget Reference (amelai — 48GB total)

| Model | VRAM |
|---|---|
| Qwen-Rapid-AIO-NSFW-v23 (ComfyUI image) | 28.4GB |
| FLUX.1 dev fp8 (ComfyUI image) | ~10GB |
| Wan2.2 video (ComfyUI video) | ~20-25GB |
| qwen3.5:35b (Ollama) | ~26-28GB |
| qwen3.5:27b (Ollama) | ~20-22GB |
| devstral (Ollama) | ~14GB |
| Open WebUI | ~0.5GB |

ComfyUI and Ollama cannot safely share VRAM when large models are loaded in both simultaneously. Always free ComfyUI VRAM before loading large Ollama models, or rely on the 2am cron restart.

---

## Issue 4: 90-Second Boot Delay — WiFi Adapter (`wlp11s0`)

### What Happened (amelai, observed 2026-04-06)

During boot, the system stalls for up to 90 seconds on:

```
Job sys-subsystem-net-devices-wlp11s0.device/start running (36s / 1min 30s)
```

Systemd is waiting for the WiFi adapter (`wlp11s0`) to become ready. This is a known issue with the ASUS X870E WiFi adapter on Linux.

---

### Root Cause

The WiFi interface was previously configured in `/etc/netplan/*.yaml`, causing `systemd-networkd-wait-online` to wait for it to come online during boot. The adapter is slow to initialise, resulting in a near-90-second stall before the timeout expires and boot continues.

---

### Fix Applied (2026-04-06) — Pending Verification on Next Boot

The WiFi section was removed from `/etc/netplan/*.yaml` as part of the NIC switchover work (Issue 2). With WiFi no longer managed by `systemd-networkd`, `wait-online` should no longer wait for `wlp11s0` during boot.

> **TODO**: Verify on next reboot that the boot delay is gone.

---

### If the Delay Persists After Reboot

If the 90-second stall continues despite removing WiFi from netplan, mask the `systemd-networkd-wait-online` service:

```bash
sudo systemctl mask systemd-networkd-wait-online.service
```

This tells systemd to skip waiting for network interfaces to come online during boot entirely. This is safe on amelai — all services (Tailscale, Docker, Ollama) handle their own connectivity and reconnect independently.

---

## Issue 5: ASUS Dual GPU PCIe Link Speed — RTX 3090s Run at 2.5GT/s (downgraded)

**Current status (2026-04-07): UNRESOLVED — confirmed BIOS/AGESA bug. Reported to ASUS. Awaiting fix in future BIOS release.**

**Issue**: Both RTX 3090 GPUs are stuck at PCIe Gen 1 (2.5GT/s) despite being Gen 4 capable. All configurable causes have been eliminated. The CPU PCIe root ports responsible for the GPU slots are not responding to BIOS Gen 4 settings.

**Performance impact**: None for AI inference workloads. See Performance Impact section below.

---

### Symptoms

```bash
# GPU diagnostic commands
sudo lspci -vvv -s 01:00.0 | grep -E "LnkSta|LnkCap"
sudo lspci -vvv -s 03:00.0 | grep -E "LnkSta|LnkCap"
nvidia-smi --query-gpu=pcie.link.gen.current,pcie.link.gen.max,pcie.link.width.current --format=csv
```

**Current output (problematic):**
```
LnkCap: Port #0, Speed 16GT/s, Width x16   ← GPU capable of Gen 4
LnkSta: Speed 2.5GT/s (downgraded), Width x8 (downgraded)   ← Actually running at Gen 1

pcie.link.gen.current, pcie.link.gen.max, pcie.link.width.current
1, 4, 8   ← Gen 1 current, Gen 4 max
```

**Expected output (after fix):**
```
LnkSta: Speed 16GT/s, Width x8
pcie.link.gen.current = 4
```

Note: Width x8 is correct — both GPUs share the CPU's PCIe lanes, so x8 each is expected.

---

### Root Cause

The CPU PCIe root ports that feed the two GPU slots are themselves initialising at Gen 1 and not responding to BIOS Gen 4 settings:

```bash
sudo lspci -vvv | grep -E "PCI bridge|LnkSta" | head -40
```

| CPU Root Port | Speed | Device |
|---|---|---|
| `00:01.1` | **2.5GT/s** | GPU 1 slot — stuck at Gen 1 |
| `00:01.2` | 32GT/s | NVMe slot — working correctly at Gen 5 |
| `00:01.3` | **2.5GT/s** | GPU 2 slot — stuck at Gen 1 |
| `00:02.1` | 16GT/s | Other devices — working at Gen 4 |

Gen 5 and Gen 4 work correctly on other CPU root ports. Only `00:01.1` and `00:01.3` (the GPU slots) are stuck at Gen 1. This is not a hardware fault with the GPUs — it is a BIOS/AGESA firmware bug where those specific root ports are not being initialised at the correct speed.

---

### Diagnostic Commands

```bash
# GPU link capability and current status
sudo lspci -vvv -s 01:00.0 | grep -E "LnkSta|LnkCap"
sudo lspci -vvv -s 03:00.0 | grep -E "LnkSta|LnkCap"

# nvidia-smi confirmation
nvidia-smi --query-gpu=pcie.link.gen.current,pcie.link.gen.max,pcie.link.width.current --format=csv

# CPU root port speeds (the definitive diagnostic)
sudo lspci -vvv | grep -E "PCI bridge|LnkSta" | head -40

# Confirm BIOS version
sudo dmidecode -t bios | grep -i version

# Confirm kernel boot parameters
cat /proc/cmdline
```

---

### BIOS Settings (Confirmed Correct — Not the Cause)

All relevant BIOS settings have been verified correct on BIOS 2103:

| Setting | Path | Value | Status |
|---|---|---|---|
| Launch CSM | Boot > CSM | Disabled | Confirmed correct |
| Above 4G Decoding | Advanced > PCI Subsystem Settings | Enabled | Confirmed correct |
| Re-size BAR Support | Advanced > PCI Subsystem Settings | Enabled | Confirmed correct |
| SR-IOV Support | Advanced > PCI Subsystem Settings | Disabled | Correct — not needed |
| CPU PCIE ASPM Mode Control | Advanced > Onboard Devices Configuration | Auto | Tested both Auto and Disabled — no effect |
| PCIEX16_1 Bandwidth Bifurcation | Advanced > Onboard Devices Configuration | Auto | No effect |
| PCIEX16_2 Bandwidth Bifurcation | Advanced > Onboard Devices Configuration | Auto | No effect |
| PCIEX16_1 Link Mode | Advanced > PCIE Link Speed | Gen 4 | Set — no effect |
| PCIEX16_2 Link Mode | Advanced > PCIE Link Speed | Gen 4 | Set — no effect |
| PCIEX16(4) Link Mode | Advanced > PCIE Link Speed | Gen 4 | Set — no effect |

---

### Full Troubleshooting Log (2026-04-07)

Every configurable cause has been tested and eliminated:

| Step | Action | Result |
|---|---|---|
| 1 | Verified all BIOS settings (Above 4G, ReBAR, CSM, Gen 4) | Settings correct — no change to link speed |
| 2 | Set PCIEX16_1 and PCIEX16_2 Link Mode to Gen 4 in BIOS | CPU root ports still at 2.5GT/s |
| 3 | Updated BIOS from 2102 to 2103 | No change |
| 4 | Physically reseated both GPUs, removed NVLink bridge | No change |
| 5 | Tested with single GPU only (PCIEX16_1 alone) | Still 2.5GT/s — not a dual-GPU issue |
| 6 | Set CPU PCIE ASPM Mode Control to Disabled in BIOS | No change |
| 7 | Removed `pcie_aspm=off` kernel parameter from GRUB | No change to link speed; fixed nvidia-smi width reporting |
| 8 | Restored ASPM to Auto in BIOS | No change |

**Conclusion**: The CPU root ports `00:01.1` and `00:01.3` do not respond to any BIOS Gen 4 settings. This is a BIOS/AGESA firmware bug. Raised with ASUS support and ASUS forum.

---

### Current Kernel Boot Parameters

`pcie_aspm=off` has been **removed** from `/etc/default/grub` as it is no longer needed (Intel igc NIC is blacklisted — see Issue 2) and was interfering with nvidia-smi link width reporting. Current `GRUB_CMDLINE_LINUX_DEFAULT` is empty.

---

### Performance Impact

**For AI inference workloads**: This downgrade **does not affect performance**. The RTX 3090s operate independently with their own VRAM pools. The PCIe link speed only impacts:
- Model loading from system RAM to GPU VRAM (slightly slower at Gen 1)
- Small parameter updates during training
- CPU-initiated memory transfers

**For dual-GPU model splitting** (tensor parallel): NVLink handles inter-GPU communication regardless of PCIe link speed, so inference across both GPUs is completely unaffected.

---

### Reference: PCIe Speed Equivalents

| LnkSta Speed | PCIe Generation | Bandwidth (x8) |
|---|---|---|
| 2.5 GT/s | **Gen 1** | ~2 GB/s — current state |
| 5.0 GT/s | Gen 2 | ~4 GB/s |
| 8.0 GT/s | Gen 3 | ~8 GB/s |
| **16.0 GT/s** | **Gen 4** | **~16 GB/s — expected** |
| 32.0 GT/s | Gen 5 | ~32 GB/s |

The RTX 3090 is a PCIe 4.0 device — it should negotiate at 16GT/s. The X870E motherboard slots support PCIe 5.0 but the GPU caps negotiation at Gen 4 (its maximum).

---

### Verification Checklist

- [x] Both GPUs reseated firmly — done, no effect
- [x] NVLink bridge removed and tested — no effect
- [x] BIOS CSM disabled — confirmed
- [x] Above 4G Decoding enabled — confirmed
- [x] Resizable BAR enabled — confirmed
- [x] PCIe Link Speed set to Gen 4 — confirmed, no effect
- [x] BIOS updated to 2103 — no effect
- [x] Single GPU tested — still Gen 1, not a dual-GPU issue
- [x] ASPM settings tested — no effect
- [ ] ASUS BIOS fix — pending future release

---