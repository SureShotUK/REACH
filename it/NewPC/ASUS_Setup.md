# ASUS ProArt X870E-CREATOR WIFI — Setup Guide

**Build**: Ryzen 9 7900X / Dual RTX 3090 / 64GB DDR5-6000 / 2x Samsung 9100 Pro / Linux (Ollama AI inference)
**Last Updated**: March 2026

---

## BIOS Version

**Current**: 2102 (Beta, installed March 2026)
**Latest Stable**: 2004 (released 2026-01-30)

**Can you roll back to 2004?** No. ASUS blocks rollback to any version prior to 1804 due to PSP firmware updates. Since 2102 is well beyond that threshold, you are permanently on 2102.

**Is 2102 acceptable?** Yes — and for your build it may actually be the better choice. Version 2102 specifically adds stability improvements for high-frequency DDR5 training (DDR5-6000 with 2x32GB kits). The downside of being on a beta is minor risk of regressions; monitor system stability for 1-2 weeks under load and check the ASUS forums if issues arise.

**Security note**: Version 0409 patched critical vulnerabilities (SMM Lock Bypass "Sinkhole" + CVE-2025-2884). Version 2102 covers all patches to date. You are fully patched.

---

## Action Required — M.2 Drive Placement

> **This is the most important physical change to make.**

### The Problem

The board shares PCIe bandwidth between the **M.2_2 slot** and the **second GPU slot (PCIEX16(G5)_2)**. When M.2_2 is populated with an NVMe drive, GPU 2 is silently reduced from x8 to x4 bandwidth — the BIOS does not warn you.

**Your current situation**: One Samsung 9100 Pro in M.2_1, one in M.2_2.
**Result**: GPU 2 is running at PCIe x4 instead of x8.

### The Fix

Move the drive from M.2_2 to **M.2_3 or M.2_4**. These slots are fed by the chipset rather than the CPU directly, so they run at PCIe 4.0 rather than PCIe 5.0 — but the Samsung 9100 Pro in M.2_3/4 still achieves approximately 7 GB/s sequential read, which is more than sufficient for OS and AI model loading.

| Slot | Interface | Speed | GPU Conflict |
|------|-----------|-------|--------------|
| M.2_1 | PCIe 5.0 x4, CPU-direct | ~14 GB/s | None |
| **M.2_2** | PCIe 5.0 x4, CPU-direct | ~14 GB/s | **Halves GPU 2 to x4** |
| M.2_3 | PCIe 4.0 x4, chipset | ~7 GB/s | None |
| M.2_4 | PCIe 4.0 x4, chipset | ~7 GB/s | None |

**Recommended placement:**
- Samsung 9100 Pro #1 (OS/primary) → **M.2_1** (stays here — no change)
- Samsung 9100 Pro #2 (data/models) → **M.2_3 or M.2_4** (move from M.2_2)

After moving the drive, both GPUs will run at x8/x8, which is the optimal configuration for dual-GPU AI inference.

### Verify after the move

```bash
# Both GPUs should show Width x8
sudo lspci -vv | grep -E "VGA|3D|LnkSta" | grep -A1 "VGA\|3D"
```

You should see `LnkSta: Speed 16GT/s, Width x8` for each GPU (16GT/s is PCIe 4.0 speed — correct, as the RTX 3090 is a Gen 4 device in a Gen 5 slot).

---

## BIOS Settings Checklist

Enter BIOS at startup with **Delete** key. Work through each section below.

### Boot Settings

| Setting | Path | Value | Reason |
|---------|------|-------|--------|
| CSM (Compatibility Support Module) | Boot > CSM | **Disabled** | Required for Resizable BAR and Above 4G Decoding |
| Secure Boot | Boot > Secure Boot | **Enabled** | Linux distros support this via signed shim |
| Secure Boot OS Type | Boot > Secure Boot > OS Type | **Other OS** | Allows Linux MOK signing |

### Advanced — PCI Subsystem Settings

| Setting | Path | Value | Reason |
|---------|------|-------|--------|
| Above 4G Decoding | Advanced > PCI Subsystem Settings | **Enabled** | Required for dual GPU — addresses memory-mapped IO for large VRAM cards |
| Resizable BAR | Advanced > PCI Subsystem Settings > Re-size BAR Support | **Enabled** | GPU performance optimisation; confirmed working on X870E |
| SR-IOV Support | Advanced > PCI Subsystem Settings | **Enabled** | Low cost to enable; useful if virtualisation is added later |

### Advanced — CPU & Virtualisation

| Setting | Path | Value | Reason |
|---------|------|-------|--------|
| SVM Mode | Advanced > CPU Configuration | **Enabled** | AMD virtualisation; enables IOMMU groups |
| IOMMU | Advanced > AMD CBS > IOMMU | **Enabled** | PCIe device isolation; improves Linux GPU handling |

### Advanced — Security

| Setting | Path | Value | Reason |
|---------|------|-------|--------|
| fTPM | Advanced > AMD fTPM Configuration | **AMD CPU fTPM** | Uses CPU's integrated TPM; no discrete chip needed |
| Security Device Support | Advanced > Trusted Computing | **Enabled** | Activates TPM for Linux measured boot / LUKS |

### Memory (Ai Tweaker)

| Setting | Path | Value | Reason |
|---------|------|-------|--------|
| EXPO Profile | Ai Tweaker > Ai Overclock Tuner | **EXPO** (or EXPO II if available) | Enables DDR5-6000 CL30 profile |
| VSOC Voltage | Ai Tweaker > Voltage settings | **1.15V** | BIOS auto may set 1.25V which causes instability with 2x32GB; manually set to 1.15V |

> **VSOC is important**: The BIOS auto-configuration for VSOC can be set too high (1.25V) when EXPO is enabled with high-capacity DIMMs. This is a documented root cause of crashes and boot failures on X870E with 2x32GB kits at DDR5-6000. Manually setting 1.15V resolves this for the majority of affected systems.

### CPU Power — Leave at Defaults

Do **not** change PBO settings. For a sustained AI inference workload (not gaming), the default Ryzen behaviour is optimal. The 7900X manages its power limits (PPT 170W / TDC 140A / EDC 200A) appropriately without intervention. This board's 16+2+2 power stage is massively over-specced for the 7900X — power delivery is not a concern.

### Fan / Thermal Settings

| Setting | Path | Value |
|---------|------|-------|
| AIO_PUMP header | Monitor > Q-Fan Configuration > AIO_PUMP | **Full Speed** |
| CPU_FAN | Monitor > Q-Fan | Custom curve (see below) |

**Arctic Liquid Freezer III Pro 360 connections:**
- Pump cable → `AIO_PUMP` header (set to Full Speed — the pump is designed to run continuously at high speed)
- 3x radiator fans → `CPU_FAN` and `CPU_OPT` headers

**Recommended fan curve for AI inference** (sustained predictable load):

| CPU Temp | Fan Speed |
|----------|-----------|
| Below 50°C | 40% |
| 60°C | 60% |
| 70°C | 80% |
| 80°C+ | 100% |

---

## NVLink — What It Is and Whether You Need It

### What is NVLink?

NVLink is a high-speed direct connection between two NVIDIA GPUs. Without it, when two GPUs need to share data (for example, running one large AI model split across both cards), that data travels via the motherboard's PCIe bus — CPU → Motherboard → GPU 1 → Motherboard → CPU → Motherboard → GPU 2. With NVLink, the GPUs communicate directly with each other at much higher speed, bypassing the CPU entirely.

Think of it like this: PCIe is like sending a letter via Royal Mail — it gets there, but it goes through sorting offices. NVLink is a direct phone call between the two GPUs.

### Why does it matter for AI inference?

When you run a large model (e.g., a 70B model) split across both RTX 3090s, the two GPUs need to constantly exchange data during inference. This inter-GPU communication is the main bottleneck, not the PCIe connection from CPU to GPU.

Benchmarks on dual RTX 3090 systems show:

| Configuration | Throughput |
|---------------|-----------|
| NVLink enabled | ~715 tokens/second |
| PCIe only (no NVLink) | ~483 tokens/second |

That is approximately **48% more throughput** for tensor-parallel inference with NVLink connected.

### Does your RTX 3090 support NVLink?

The **Asus TUF Gaming OC RTX 3090** does support NVLink. Look at the top edge of each GPU — there should be a gold/metallic connector port labelled NVLink or with a small icon. It is typically covered by a plastic cap when unused.

### What hardware do you need?

An **NVLink bridge** (also called an SLI bridge, though NVLink is different from old-style SLI). For the RTX 3090, you need a **3-slot NVLink bridge** to match the physical spacing between the cards.

NVIDIA sells these directly, and they are also available from retailers. Search for:
- "NVIDIA NVLink Bridge 3-slot RTX 3090"
- Part reference: NVIDIA NVLink HB Bridge (3-slot)

The bridge slots into the NVLink connectors on both cards. No software configuration is needed in most cases — Linux will detect the NVLink connection automatically.

### How to check if NVLink is active (once bridge is installed)

```bash
# Check NVLink status
nvidia-smi nvlink --status -i 0

# Should show active links if bridge is connected and working
nvidia-smi topo -m
# Look for NVL in the matrix between GPU 0 and GPU 1
```

### Recommendation

If you plan to run 70B+ models split across both GPUs, the NVLink bridge is worth purchasing — the 48% throughput improvement is significant. If you run two independent models simultaneously (one model per GPU), NVLink provides no benefit as the GPUs are not communicating.

---

## Linux Configuration

### GRUB Parameters

Add to `/etc/default/grub`:

```
GRUB_CMDLINE_LINUX_DEFAULT="quiet amd_iommu=on iommu=pt"
```

Then apply:

```bash
sudo update-grub
sudo reboot
```

`amd_iommu=on` enables IOMMU for PCIe isolation. `iommu=pt` (passthrough mode) reduces overhead for systems not doing full VM device passthrough.

### Verify IOMMU is Active

```bash
dmesg | grep -i iommu
# Should show: AMD-Vi: AMD IOMMUv2 loaded and initialized
```

### Verify PCIe Link Width (after moving M.2 drive)

```bash
sudo lspci -vv | grep -B1 "LnkSta" | grep -E "NVIDIA|LnkSta"
```

Both GPUs should show `Width x8`. If either shows `Width x4`, M.2_2 may still be occupied.

### Verify RAM Speed

```bash
sudo dmidecode --type memory | grep -E "Speed|Size|Type"
# Configured Memory Speed should show 6000 MT/s
```

### Verify GPU Detection

```bash
nvidia-smi
# Should show both RTX 3090s with 24GB VRAM each

nvidia-smi topo -m
# Shows GPU interconnect topology
```

### Sensors and Monitoring

```bash
# Install sensor tools
sudo apt install lm-sensors

# Detect available sensors
sudo sensors-detect

# View current readings
sensors

# GPU temperatures
nvidia-smi --query-gpu=temperature.gpu,power.draw,clocks.sm --format=csv
```

> **Note on WiFi**: The onboard MediaTek MT7927 WiFi 7 chip has no mainline Linux kernel driver as of March 2026. Do not spend time troubleshooting it. Use wired Ethernet — the Intel I226-V 2.5G port is fully supported on Linux.

---

## GPU Power Limiting

This is the next setup step after completing the above. See separate documentation or run:

```bash
# Check current power limits
nvidia-smi -q | grep -E "Power Limit|Default Power"

# Apply power limit (300W per GPU is a common starting point for 3090s)
sudo nvidia-smi -pl 300
```

A systemd service will be configured to make this permanent across reboots. See the GPU power tuning section of the main build documentation.

---

## Known Limitations

| Issue | Impact | Mitigation |
|-------|--------|------------|
| BIOS 2102 is beta | Minor regression risk | Monitor stability; check ASUS forums if issues arise |
| No WiFi on Linux (MT7927) | No wireless | Use Ethernet |
| Network drops on wake from sleep | Link may drop after suspend | Not an issue if machine runs continuously; disable sleep if it occurs |
| BIOS rollback blocked after 1804 | Cannot downgrade | Stay on 2102; update to stable once released |

---

## Sources

- <a href="https://www.asus.com/us/motherboards-components/motherboards/proart/proart-x870e-creator-wifi/helpdesk_bios?model2Name=ProArt-X870E-CREATOR-WIFI" target="_blank">ASUS ProArt X870E-CREATOR WIFI BIOS Helpdesk (official)</a>
- <a href="https://www.asus.com/us/motherboards-components/motherboards/proart/proart-x870e-creator-wifi/techspec/" target="_blank">ASUS ProArt X870E-CREATOR WIFI Tech Specs (official)</a>
- <a href="https://bbs.archlinux.org/viewtopic.php?id=304568" target="_blank">Arch Linux Forums — Hardware compatibility for ProArt X870E</a>
- <a href="https://forums.linuxmint.com/viewtopic.php?t=432080" target="_blank">Linux Mint Forums — ASUS ProArt X870E WiFi/Bluetooth on Linux</a>
- <a href="https://forum.level1techs.com/t/x870e-iommu-and-general-vfio-things/227540/6" target="_blank">Level1Techs — X870E IOMMU and VFIO configuration</a>
- <a href="https://rog-forum.asus.com/t5/amd-800-series/rog-strix-x870e-e-gaming-wifi-expo-profile-unstable-with-6000mt/td-p/1068708" target="_blank">ASUS ROG Forum — X870E EXPO DDR5-6000 instability and VSOC fix</a>
- <a href="http://himeshp.blogspot.com/2025/03/vllm-performance-benchmarks-4x-rtx-3090.html" target="_blank">vLLM RTX 3090 Multi-GPU Benchmarks — NVLink vs PCIe throughput</a>
- <a href="https://iommu.info/mainboard/?board_vendor=ASUSTeK+COMPUTER+INC.&board_name=ProArt+X870E-CREATOR+WIFI" target="_blank">iommu.info — IOMMU group map for ProArt X870E-CREATOR WIFI</a>
