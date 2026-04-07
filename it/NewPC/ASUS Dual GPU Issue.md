# ASUS Dual GPU Transfer Speed Issue

**Issue**: RTX 3090 GPUs show `LnkSta: Speed 2.5GT/s (downgraded), Width x8 (downgraded)` instead of expected 16GT/s at PCIe 4.0.

## Symptoms

```bash
sudo lspci -vv | grep -E "VGA|LnkSta" | grep -A1 "VGA\|3D"

# Current problematic output:
01:00.0 VGA compatible controller: NVIDIA Corporation GA102 [GeForce RTX 3090]
                LnkSta: Speed 2.5GT/s (downgraded), Width x8 (downgraded)
03:00.0 VGA compatible controller: NVIDIA Corporation GA102 [GeForce RTX 3090]
                LnkSta: Speed 2.5GT/s (downgraded), Width x8 (downgraded)

# Expected output (after fix):
LnkSta: Speed 16GT/s, Width x8
```

## Root Cause Analysis

Both GPUs are connected directly to CPU PCIe lanes:
- **GPU 1**: bus `01:00.0` via CPU port `01.0`
- **GPU 2**: bus `03:00.0` via CPU port `01.3`

The `(downgraded)` flag indicates link training couldn't negotiate PCIe 4.0 speeds, falling back to PCIe 3.0 (2.5GT/s). Since **both** GPUs show identical behavior, the cause is likely:

1. **BIOS PCIe Gen mode setting** — Set to "Gen3 Auto" instead of "Gen4 Auto/Gen4"
2. **Physical contact issue** — GPU connectors need reseating
3. **Motherboard CPU socket contact** — Affects PCIe lane training (less common)

## Diagnostic Commands

### Check negotiated speed vs capability
```bash
sudo lspci -vvv -s 01:00.0 | grep -E "LnkSta|LnkCap"
sudo lspci -vvv -s 03:00.0 | grep -E "LnkSta|LnkCap"
```

**Expected LnkCap**: `Speed 16GT/s, Width x16` (both GPUs support PCIe 4.0)
**Current LnkSta**: `Speed 2.5GT/s, Width x8` (negotiated at Gen3 fallback)

### Verify BIOS version
```bash
sudo dmidecode -t bios | grep -i version
```

Your system should be on BIOS **2102 (Beta)** or stable release which supports PCIe 4.0.

## Fix Procedure

### Prerequisites
- Physical access to amelai
- SSH session active (to run diagnostics before/after)
- Tailscale connected (for remote access after reboot)

### Step 1: Power down and drain residual power
```bash
sudo shutdown -h now
```

Wait for complete power-down, then proceed physically:

1. **Unplug amelai from mains**
2. **Hold power button 10-15 seconds** (drains capacitors)
3. **Open case and reseat both GPUs**:
   - Release PCIe clips at rear of each card
   - Pull straight up to remove GPU
   - Inspect slot for dust/debris (use compressed air if needed)
   - Reinsert firmly until clips click audibly
   - Verify auxiliary power cables fully seated

### Step 2: Enter BIOS before OS boots
After reseating, boot and immediately tap **Delete** repeatedly.

#### BIOS Settings to Check

| Setting | Path | Value | Reason |
|---------|------|-------|--------|
| CSM (Compatibility Support Module) | Boot > CSM | **Disabled** | Required for Resizable BAR and proper PCIe training |
| Above 4G Decoding | Advanced > PCI Subsystem Settings | **Enabled** | Critical for dual GPU — addresses memory-mapped IO for large VRAM cards |
| Resizable BAR | Advanced > PCI Subsystem Settings > Re-size BAR Support | **Enabled** | NVIDIA GPUs benefit from this; helps with PCIe configuration |

#### Look for PCIe Gen Mode Setting

This may be in one of these locations:
- **Advanced → PCI Subsystem Settings → PCIe Link Speed**
- **Ai Tweaker → Extreme Tweaker → PCIe Configuration**
- **Ai Overclock Tuner settings** during boot

If set to **"Auto"**, **"Gen3 Auto"**, or similar, change it to:
- **"Gen4"** (manual override) or
- **"Gen4 Auto"** (if available)

Save changes and exit BIOS (typically F10).

### Step 3: Verify fix after OS boots
Once Ubuntu is running and Tailscale connection is active:

```bash
# Check link status for both GPUs
sudo lspci -vvv -s 01:00.0 | grep -E "LnkSta|LnkCap"
sudo lspci -vvv -s 03:00.0 | grep -E "LnkSta|LnkCap"
```

**Expected output after successful fix:**
```
LnkCap: Port #0, Speed 16GT/s, Width x16
LnkSta: Speed 16GT/s, Width x8
```

Note: Width x8 is correct for your setup (both GPUs sharing x16 lanes equally). The key is Speed 16GT/s instead of 2.5GT/s.

### Step 4: Optional — Force PCIe rescan (if speed unchanged)
```bash
echo 1 | sudo tee /sys/bus/pci/rescan_bus
nvidia-smi -q | grep "Link"
```

This attempts to re-enumerate PCIe buses without rebooting. Sometimes helps if the issue is a firmware initialization glitch.

## Performance Impact

**For AI inference workloads**: This downgrade **does not affect performance**. The RTX 3090s operate independently with their own VRAM pools. The PCIe link speed only impacts:
- Model loading from system RAM to GPU VRAM (slower with Gen3)
- Small parameter updates during training
- CPU-initiated operations

**For dual-GPU model splitting** (tensor parallel): NVLink handles inter-GPU communication regardless of PCIe link speed, so even if running split models across both GPUs, inference performance remains unaffected.

## Additional Notes

### BIOS 2102 Beta Information
Your system is currently on **BIOS 2102 (Beta)** which should support PCIe 4.0 correctly:
- Version specifically adds DDR5 stability improvements
- Covers all security patches including CVE-2025-2884
- Cannot rollback to versions prior to 1804 due to PSP firmware

If the Gen4 setting doesn't resolve the issue, consider updating to the next stable release from ASUS when available.

### Reference: PCIe Speed Equivalents
| LnkSta Speed | PCIe Generation | Notes |
|--------------|-----------------|-------|
| 2.5 GT/s | Gen 3 (x1 = 250 MB/s) | Current fallback state |
| 5.0 GT/s | Gen 4 (x1 = 500 MB/s) | - |
| 8.0 GT/s | Gen 5 (x1 = 800 MB/s) | - |
| **16.0 GT/s** | **Gen 5 (x1 = 1.6 GB/s)** | **Expected for X870E + Ryzen 7900X** |

Wait — your RTX 3090s are PCIe 4.0 devices, so they should negotiate at **16GT/s** even though your motherboard slots support up to PCIe 5.0 (32GT/s).

## Verification Checklist

- [ ] Both GPUs reseated firmly
- [ ] BIOS CSM disabled
- [ ] Above 4G Decoding enabled
- [ ] Resizable BAR enabled
- [ ] PCIe Link Speed set to Gen4
- [ ] Reboot completed after BIOS changes
- [ ] `LnkSta` shows 16GT/s for both cards
