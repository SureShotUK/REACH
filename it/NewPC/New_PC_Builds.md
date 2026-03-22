# Personal Windows 11 PC Build — Research & Recommendations

**Research Date**: March 2026
**Purpose**: Personal desktop to replace an existing Windows 10 machine — used for Minecraft Bedrock RTX at 1440p, web browsing, and interacting with the AI PC (dual RTX 3090 Ubuntu server via Tailscale).

---

## Existing Components (Already Owned)

| Component | Model | Notes |
|---|---|---|
| Motherboard | MSI MAG X870E Tomahawk WIFI | AM5 socket, PCIe 5.0, DDR5 |
| Storage | Samsung 9100 Pro NVMe 2TB | PCIe 5.0 |
| RAM | Viper Venom DDR5 2x16GB 6000MT/s | 32GB total; EXPO profile — enable in BIOS on first boot |

---

## Components Still to Purchase

| Component | Chosen Model | Approx. Price |
|---|---|---|
| CPU | AMD Ryzen 7 9800X3D | £380 |
| GPU | NVIDIA RTX 5070 Ti 16GB | £680 |
| Case | Corsair 4000D Airflow | £85 |
| PSU | be quiet! Power Zone 2 1000W (80+ Platinum) | £149.99 |
| CPU Cooler | Arctic Liquid Freezer III 360 | £100 |
| **Total** | | **~£1,395** |

---

## Important Notes Before Buying

### Minecraft Bedrock RTX — NVIDIA Required
- **Minecraft Bedrock RTX** uses NVIDIA's DXR hardware implementation. AMD GPUs cannot run it regardless of spec.
- This locks GPU choice to NVIDIA RTX.
- At 1440p with Bedrock RTX enabled, memory bandwidth is the key GPU metric — ray tracing requires many random memory accesses (BVH traversal), which saturates memory bandwidth quickly. A wider memory bus (256-bit vs 192-bit) makes a meaningful difference under sustained RT load at 1440p.

### RAM EXPO Profile
Your Viper Venom DDR5 6000MT/s uses EXPO (AMD's equivalent of Intel XMP). The X870E Tomahawk fully supports EXPO — enable it in BIOS after first boot. Without enabling it, the RAM will run at stock DDR5 speeds (~4800MT/s).

### ATX 3.1 / 12V-2x6 PSU Connector
The RTX 50-series uses the 12V-2x6 connector. Choose a PSU with a native 12V-2x6 cable rather than using the 8-pin adapter supplied in the GPU box. This avoids the connector melting issues that affected some early RTX 40-series installations.

---

## 1. CPU — AMD AM5 (Zen 5)

Minecraft Bedrock Edition is better multi-threaded than Java, but single-core clock speed still heavily influences chunk loading and world simulation performance. High boost clocks remain the priority.

| Tier | Model | Approx. UK Price | Cores/Threads | Boost | TDP | Notes |
|---|---|---|---|---|---|---|
| Budget | Ryzen 5 9600X | ~£225 | 6C / 12T | 5.4 GHz | 65W | Great single-core speed. Low power draw — runs cool and quiet. |
| Mid-Range | Ryzen 7 9700X | ~£310 | 8C / 16T | 5.5 GHz | 65W | Good balance. 8 cores, easy to cool. |
| **Chosen** | **Ryzen 7 9800X3D** | **~£380** | **8C / 16T** | **5.2 GHz** | **120W** | Gaming champion. 3D V-Cache significantly improves chunk loading and 1% low frame rates in Bedrock RTX. Best single-threaded gaming performance on AM5. |
| Considered/Rejected | Ryzen 9 9950X3D | ~£700 | 16C / 32T | ~5.7 GHz | 170W | 16 cores across two CCDs — only one has V-Cache. Gaming performance is roughly equal to the 9800X3D. The extra cores benefit video editing (~50–65% faster exports) but heavy encoding can be offloaded to the AI PC instead. Not worth the £320 premium for this use case. |

**Note on the 9800X3D**: Runs at 120W TDP with power spikes above that. Requires a more capable cooler than the 65W variants — see Section 5. An AIO or a high-end dual-tower air cooler is recommended.

---

## 2. GPU — NVIDIA RTX 50-Series (Blackwell)

The RTX 5000 series launched in 2025 and is now widely available. The RTX 40-series is approaching end-of-life with dwindling stock.

For 1440p Bedrock RTX, memory bandwidth is especially important. The table below includes memory bus width as a key column.

| Tier | Model | Approx. UK Price | VRAM | Memory Bus | TDP | Notes |
|---|---|---|---|---|---|---|
| Entry 1440p | RTX 5060 Ti 16GB | ~£450 | 16GB GDDR7 | 128-bit | 180W | Can handle 1440p Bedrock RTX but the narrower memory bus limits performance under heavy RT load. Adequate for moderate settings. |
| **Chosen** | **RTX 5070 Ti** | **~£680** | **16GB GDDR7** | **256-bit** | **300W** | Strong fit for 1440p Bedrock RTX. The 256-bit bus provides twice the memory bandwidth of the 5060 Ti — directly beneficial for ray tracing. DLSS 4 with Multi Frame Generation maintains high frame rates with RT enabled. |
| High-End | RTX 5080 | ~£1,050 | 16GB GDDR7 | 256-bit | 360W | Overkill for Minecraft specifically. Better value alternatives exist. |

**Why the RTX 5070 Ti suits 1440p Bedrock RTX:**
- Minecraft Bedrock RTX with path tracing is highly memory-bandwidth intensive. The 256-bit memory bus on the 5070 Ti (~896 GB/s) vs the 192-bit bus on the plain 5070 (~672 GB/s) gives noticeably better sustained RT performance.
- 16GB VRAM handles 1440p texture packs and resource packs without overflow to system RAM.
- DLSS 4 with Multi Frame Generation (MFG) allows the game to run at native 1440p RT quality while using AI frame generation to hit high frame rates.

**For comparison — RTX 5070 (£530):**
The plain RTX 5070 is a step down: 192-bit bus, ~672 GB/s bandwidth vs ~896 GB/s on the 5070 Ti. For Bedrock RTX at 1440p the bandwidth difference is tangible. The ~£150 gap to the 5070 Ti is justified if 1440p RTX is the primary use case.

**Warning — PCIe boot bug**: Early RTX 5060 and 5060 Ti cards had a BIOS/vBIOS bug causing black screens on some motherboards. NVIDIA issued a firmware fix — cards sold new from retailers now ship with the fix applied. If buying used, verify the seller applied the vBIOS update.

---

## 3. Case — Mid-Tower ATX

| Model | Approx. UK Price | Key Features | Notes |
|---|---|---|---|
| **Corsair 4000D Airflow** | **~£85** | Mesh front panel, 2x 120mm fans included, excellent cable management | Industry standard. Straightforward build, clean appearance. Best value option. |
| NZXT H6 Flow | ~£105 | Dual-chamber design, angled intake fans blow directly onto GPU | Exceptional GPU airflow — useful under sustained ray tracing load. |
| Fractal Design North | ~£125 | Wood-panel aesthetic, mesh side, high-quality construction | Looks like furniture rather than a gaming rig. Premium feel. |

All three are standard ATX compatible with the MSI X870E Tomahawk. All support 240mm+ AIOs if choosing that cooling route.

---

## 4. Power Supply (PSU)

**Wattage guidance by build:**

| CPU + GPU Combination | Peak System Load | Headroom on 1000W |
|---|---|---|
| Ryzen 7 9800X3D + RTX 5070 Ti | ~480W | ~520W spare — substantial future-proofing |
| Budget build (9600X + RTX 5060 Ti) | ~300W | ~700W spare |

The chosen build peaks at ~480W under full GPU + CPU load. 1000W provides over 500W headroom — more than enough for any future GPU upgrade without changing the PSU.

**80+ Platinum vs Titanium — what it means:**
- **80+ Gold**: ≥87% efficiency at 50% load
- **80+ Platinum**: ≥89% efficiency at 50% load
- **80+ Titanium**: ≥92% efficiency at 50% load (and rated at 10% load too)

At typical gaming loads (~300–400W), the efficiency difference between Platinum and Titanium saves only a few watts — the real benefit is quality of components used in Titanium-rated units and their longevity.

| Model | Approx. UK Price | Wattage | Standard | Notes |
|---|---|---|---|---|
| Corsair HX1000 | ~£185 | 1000W | 80+ Platinum, ATX 3.1 | Fully modular, native 12V-2x6, 10-year warranty. |
| **be quiet! Power Zone 2 1000W** | **£149.99** | **1000W** | **80+ Platinum, ATX 3.1** | Fully modular, native 90° angled 12V-2x6, 140mm fan (quieter than 120mm competitors), 10-year warranty. Cybenetics Platinum certified. **Chosen.** |
| Seasonic Prime PX-1000 | ~£210 | 1000W | 80+ Platinum, ATX 3.1 | Seasonic's own platform, 12-year warranty. Premium option. |
| be quiet! Dark Power 13 1000W | ~£230 | 1000W | 80+ Titanium, ATX 3.1 | Titanium efficiency, near-silent, 10-year warranty. |

**Chosen: be quiet! Power Zone 2 1000W (BP008UK)** — £149.99. Platinum rated, ATX 3.1, native 12V-2x6, fully modular, 140mm fan for quiet operation, 10-year warranty. Verified at £149.99 (March 2026).

---

## 5. CPU Cooler — AM5 Compatible

| Model | Approx. UK Price | Type | TDP Capacity | Notes |
|---|---|---|---|---|
| **Thermalright Peerless Assassin 120 SE** | **~£35** | Dual-tower air | 280W+ | Exceptional value. Performs near the top of all air coolers. AM5 bracket included. Ideal for the 9700X. Adequate for the 9800X3D in a well-ventilated case. |
| DeepCool AK620 Digital | ~£65 | Dual-tower air | 260W+ | Same class as the Peerless Assassin but adds a magnetic display showing CPU temperature. |
| **Arctic Liquid Freezer III 360** | **~£100** | 360mm AIO | 400W+ | Best-in-class AIO for the price. Includes a VRM fan that cools motherboard power delivery directly. 3x 120mm fans run slower and quieter than the 240mm for the same heat output — better sustained boost clock retention on the 9800X3D. **Chosen.** |

---

## Chosen Build

### Selected Configuration — 1440p Bedrock RTX + Video Editing via AI PC

| Component | Model | Approx. Price |
|---|---|---|
| CPU | Ryzen 7 9800X3D | £380 |
| GPU | RTX 5070 Ti 16GB | £680 |
| Case | Corsair 4000D Airflow | £85 |
| PSU | be quiet! Power Zone 2 1000W (80+ Platinum) | £149.99 |
| CPU Cooler | Arctic Liquid Freezer III 360 | £100 |
| **Total (new parts)** | | **~£1,390** |

### Budget Alternative

If cost needs to be reduced, this config still handles 1440p Bedrock RTX at moderate settings:

| Component | Model | Approx. Price |
|---|---|---|
| CPU | Ryzen 5 9600X | £225 |
| GPU | RTX 5060 Ti 16GB | £450 |
| Case | Corsair 4000D Airflow | £85 |
| PSU | Corsair RM750x ATX 3.1 | £110 |
| CPU Cooler | Thermalright Peerless Assassin 120 SE | £35 |
| **Total (new parts)** | | **~£905** |

---

## Video Editing — Using the AI PC for Heavy Encoding

The AI PC (Ubuntu server, dual RTX 3090 24GB, connected via Tailscale) can handle GPU-accelerated video encoding, offloading the most time-consuming part of the video editing workflow from this Windows 11 machine. This is one reason the 9950X3D's extra CPU cores were not needed here.

### How It Works

Video editing software separates the editing/timeline experience (runs locally) from the export/render process (can be offloaded). The workflow is:

1. Edit the project locally on this Windows 11 PC — timeline scrubbing, cuts, colour grading, effects all run locally using the RTX 5070 Ti
2. When ready to export, submit the render job to the AI PC over Tailscale
3. The AI PC's dual RTX 3090s encode the video using GPU acceleration (NVENC)
4. The finished file is returned over the network

### DaVinci Resolve — Remote Rendering

DaVinci Resolve (free version supports this) has a **Render Queue** that can be pointed at a remote render node. The AI PC would need DaVinci Resolve Studio or the free DaVinci Resolve server installed on Ubuntu.

- The dual RTX 3090s support NVENC (NVIDIA hardware encoder) — fast H.264, H.265, and AV1 encoding
- GPU-accelerated colour grading and Fusion effects also run on the render node
- Tailscale provides the secure private network connection between the two machines without any firewall configuration

### Adobe Premiere Pro — Adobe Media Encoder

Premiere Pro uses **Adobe Media Encoder** for export. Media Encoder can be run on the AI PC as a networked encoder, with the project file and media accessible over the shared network path (mapped via Tailscale IP).

### Performance Benefit

For a typical 10-minute 4K H.265 export:

| Scenario | Estimated Export Time |
|---|---|
| Local CPU encode (9800X3D, 8 cores) | ~8–12 minutes |
| Local GPU encode (RTX 5070 Ti NVENC) | ~2–4 minutes |
| Remote GPU encode (dual RTX 3090 NVENC) | ~1–2 minutes |

NVENC on any modern NVIDIA GPU is far faster than CPU encoding. The dual RTX 3090s on the AI PC add further capacity for concurrent streams.

### Setup Requirements

- Tailscale running on both machines (already in use)
- DaVinci Resolve installed on the AI PC (Ubuntu — supported natively)
- Shared network storage or project files accessible from both machines (FileBrowser on the AI PC is already set up at port 8087)
- Sufficient Tailscale bandwidth for transferring source footage and rendered output

---

## Compatibility Summary

| Check | Status |
|---|---|
| AM5 CPU socket | Ryzen 7 9800X3D confirmed AM5 |
| DDR5 RAM EXPO profile | X870E Tomahawk supports EXPO — enable in BIOS on first boot |
| PCIe 5.0 GPU slot | X870E has PCIe 5.0 x16; all RTX 50-series are PCIe 4.0 — fully compatible |
| ATX 3.1 / 12V-2x6 | Required for RTX 50-series; choose a PSU with native 12V-2x6 cable |
| AM5 cooler mounting | All recommended coolers include AM5 bracket |
| Minecraft Bedrock RTX | Requires NVIDIA DXR — confirmed on all RTX 50-series cards |

---

## Pricing Note

All GBP prices are approximate based on the UK market in early 2026. Actual prices will vary by AIB partner (ASUS, MSI, Gigabyte, Zotac) and retailer (Scan, Overclockers UK, Amazon UK). The RTX 5060 Ti 8GB and 16GB variants have significant price differences — **always confirm which variant you are ordering before purchase**.
