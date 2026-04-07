# ASUS ProArt X870E-CREATOR WIFI — PCIe Gen 1 Fallback Issue

**Date raised**: 2026-04-07
**BIOS version tested**: 2102, 2103
**Status**: Unresolved — awaiting ASUS response

---

## System Specification

| Component | Detail |
|---|---|
| **Motherboard** | ASUS ProArt X870E-CREATOR WIFI |
| **CPU** | AMD Ryzen 9 7900X (Zen 4, AM5) |
| **GPU 1** | ASUS TUF RTX 3090 24GB (slot PCIEX16_1) |
| **GPU 2** | ASUS TUF RTX 3090 24GB (slot PCIEX16_2) |
| **RAM** | 64GB DDR5-6000 |
| **Storage** | 2x Samsung 9100 Pro 2TB NVMe PCIe 5.0 |
| **OS** | Ubuntu 24.04 LTS Server (kernel 6.17.0-20-generic) |
| **BIOS** | 2103 (also tested on 2102) |

---

## Problem Description

Both RTX 3090 GPUs are running at PCIe Gen 1 (2.5GT/s) instead of the expected PCIe Gen 4 (16GT/s). The GPUs are capable of Gen 4 and the BIOS Gen 4 setting has been applied, but the CPU PCIe root ports responsible for the GPU slots are initialising at Gen 1 regardless of any configuration changes made.

This affects both GPU slots identically.

---

## Diagnostic Output

### GPU Link Status

```bash
$ sudo lspci -vvv -s 01:00.0 | grep -E "LnkSta|LnkCap"
LnkCap: Port #0, Speed 16GT/s, Width x16, ASPM L0s L1, Exit Latency L0s <512ns, L1 <4us
LnkSta: Speed 2.5GT/s (downgraded), Width x8 (downgraded)
LnkCap2: Supported Link Speeds: 2.5-16GT/s, Crosslink- Retimer+ 2Retimers+ DRS-
LnkSta2: Current De-emphasis Level: -3.5dB, EqualizationComplete+ EqualizationPhase1+

$ sudo lspci -vvv -s 03:00.0 | grep -E "LnkSta|LnkCap"
LnkCap: Port #0, Speed 16GT/s, Width x16, ASPM L0s L1, Exit Latency L0s <512ns, L1 <4us
LnkSta: Speed 2.5GT/s (downgraded), Width x8 (downgraded)
LnkCap2: Supported Link Speeds: 2.5-16GT/s, Crosslink- Retimer+ 2Retimers+ DRS-
LnkSta2: Current De-emphasis Level: -3.5dB, EqualizationComplete+ EqualizationPhase1+
```

### nvidia-smi Confirmation

```bash
$ nvidia-smi --query-gpu=pcie.link.gen.current,pcie.link.gen.max,pcie.link.width.current --format=csv
pcie.link.gen.current, pcie.link.gen.max, pcie.link.width.current
1, 4, 8
1, 4, 8
```

Both GPUs: Gen 1 current, Gen 4 maximum supported.

### CPU Root Port Analysis (Key Finding)

```bash
$ sudo lspci -vvv | grep -E "PCI bridge|LnkSta" | head -40
00:01.1 PCI bridge: AMD Device 14db
        LnkSta: Speed 2.5GT/s, Width x8        ← GPU 1 root port — Gen 1
00:01.2 PCI bridge: AMD Device 14db
        LnkSta: Speed 32GT/s, Width x4         ← NVMe root port — Gen 5 (working correctly)
00:01.3 PCI bridge: AMD Device 14db
        LnkSta: Speed 2.5GT/s, Width x8        ← GPU 2 root port — Gen 1
00:02.1 PCI bridge: AMD Device 14db
        LnkSta: Speed 16GT/s, Width x4         ← Other port — Gen 4 (working correctly)
00:02.2 PCI bridge: AMD Device 14db
        LnkSta: Speed 16GT/s, Width x4         ← Other port — Gen 4 (working correctly)
00:08.1 PCI bridge: AMD Device 14dd
        LnkSta: Speed 16GT/s, Width x16        ← Chipset — Gen 4 (working correctly)
00:08.3 PCI bridge: AMD Device 14dd
        LnkSta: Speed 16GT/s, Width x16        ← Chipset — Gen 4 (working correctly)
```

**The CPU root ports for the GPU slots (`00:01.1` and `00:01.3`) are at 2.5GT/s. All other CPU root ports are functioning correctly at Gen 4 or Gen 5. The issue is specific to these two ports.**

### BIOS Version

```bash
$ sudo dmidecode -t bios | grep -i version
Version: 2103
```

---

## BIOS Settings Applied

All of the following settings were verified and applied. None resolved the issue.

| Setting | Path | Value Set |
|---|---|---|
| Launch CSM | Boot > CSM | Disabled |
| Above 4G Decoding | Advanced > PCI Subsystem Settings | Enabled |
| Re-size BAR Support | Advanced > PCI Subsystem Settings | Enabled |
| SR-IOV Support | Advanced > PCI Subsystem Settings | Disabled |
| CPU PCIE ASPM Mode Control | Advanced > Onboard Devices Configuration | Tested Auto and Disabled |
| PCIEX16_1 Bandwidth Bifurcation | Advanced > Onboard Devices Configuration | Auto |
| PCIEX16_2 Bandwidth Bifurcation | Advanced > Onboard Devices Configuration | Auto |
| PCIEX16_1 Link Mode | Advanced > PCIE Link Speed | Gen 4 |
| PCIEX16_2 Link Mode | Advanced > PCIE Link Speed | Gen 4 |
| PCIEX16(4) Link Mode | Advanced > PCIE Link Speed | Gen 4 |
| Chipset PCIE(G1) CTLE Optimisation | Advanced > PCIE Link Speed | Auto |

BIOS defaults were also loaded (F5) and settings re-applied from scratch — no change.

---

## Steps Taken and Results

### Step 1 — Initial Diagnosis

Identified both GPUs running at 2.5GT/s via `lspci`. Confirmed GPUs are Gen 4 capable via LnkCap. Confirmed CPU root ports `00:01.1` and `00:01.3` are themselves at 2.5GT/s — establishing the problem is on the motherboard/CPU side, not within the GPU cards.

**Result**: Problem identified at CPU root port level.

---

### Step 2 — BIOS Settings Review and Gen 4 Applied

Reviewed all PCIe-related BIOS settings. CSM was already disabled. Above 4G Decoding and Resizable BAR were already enabled. Set PCIEX16_1 and PCIEX16_2 Link Mode to Gen 4 explicitly. Rebooted and retested.

**Result**: No change. CPU root ports still at 2.5GT/s.

---

### Step 3 — BIOS Update 2102 → 2103

Updated BIOS from 2102 to 2103 via ASUS EZ Flash. Re-applied all settings after flash. Rebooted and retested.

**Result**: No change. CPU root ports still at 2.5GT/s.

---

### Step 4 — Physical GPU Reseating and NVLink Removal

Powered down completely, drained capacitors, removed NVLink bridge, reseated both GPUs firmly in their slots. Inspected PCIe slots for debris. Verified all power connectors fully seated.

**Result**: No change. CPU root ports still at 2.5GT/s.

**Observation**: POST time was noticeably shorter after removing one GPU, suggesting BIOS is spending additional time during link training for the GPU slots before falling back to Gen 1.

---

### Step 5 — Single GPU Test

Removed GPU 2 entirely. Tested with only GPU 1 in PCIEX16_1.

```bash
$ nvidia-smi --query-gpu=pcie.link.gen.current,pcie.link.gen.max,pcie.link.width.current --format=csv
pcie.link.gen.current, pcie.link.gen.max, pcie.link.width.current
1, 4, 16
```

**Result**: Still Gen 1 with a single GPU. This rules out dual-GPU configuration as the cause. The issue is present on each slot independently.

---

### Step 6 — ASPM Configuration Changes

Tested CPU PCIE ASPM Mode Control set to Disabled in BIOS (previously Auto). Also removed `pcie_aspm=off` kernel parameter from GRUB (this parameter was originally added for an unrelated Intel NIC issue, not the GPU issue).

**Result**: No change to PCIe Gen. Removing `pcie_aspm=off` did correct nvidia-smi's width reporting (was incorrectly showing x1, now correctly shows x8/x16 matching lspci output).

---

## Summary

| Cause Tested | Eliminated? |
|---|---|
| BIOS Gen 4 setting not applied | Yes — applied, no effect |
| CSM enabled | Yes — was already disabled |
| Above 4G / ReBAR not enabled | Yes — were already enabled |
| Physical GPU seating | Yes — reseated, no effect |
| NVLink bridge mechanical interference | Yes — removed, no effect |
| Dual-GPU configuration specific | Yes — single GPU also Gen 1 |
| BIOS version bug (2102) | Partial — 2103 also affected |
| ASPM configuration | Yes — tested all combinations, no effect |
| Kernel parameter interference | Yes — removed, no effect |

**All configurable and physical causes have been eliminated. The CPU root ports `00:01.1` and `00:01.3` do not respond to BIOS Gen 4 settings. Other CPU root ports on the same board function correctly at Gen 4 and Gen 5. This is consistent with a BIOS/AGESA firmware bug specific to how those root ports are initialised.**

---

## Request

Please advise whether this is a known issue with BIOS 2103 on the ProArt X870E-CREATOR WIFI, and whether a fix is planned in a future BIOS or AGESA update. The specific diagnostic data (CPU root ports `00:01.1` and `00:01.3` stuck at 2.5GT/s despite Gen 4 BIOS setting, while `00:01.2` correctly runs at 32GT/s) should be sufficient to reproduce or identify the issue.
