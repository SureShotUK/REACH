# Final AI PC Build - Component Selection
## Dual RTX 3090 Configuration — Fully Installed

**Build Purpose**: Local AI inference for coding assistance and homework help
**Target Performance**: Two 32B models simultaneously, or a single 70B model fully in VRAM
**Budget Target**: £2,200-2,400 (single GPU configuration - adjusted for UK market reality)
**Market**: United Kingdom (all prices in GBP including VAT)
**Build Philosophy**: Best bang for buck, future-proof (add components, not replace)

**Last Updated**: March 7, 2026 (ASUS ProArt X870E-CREATOR WIFI purchased £479.99 — arriving today; MSI X870E Tomahawk being sold on eBay)

---

## Build Status

**Phase**: 🔄 IN PROGRESS — ASUS ProArt X870E arriving today; MSI to be listed on eBay
**Purchased**: 11 of 11 components
**Remaining**: Install ASUS ProArt X870E; list MSI X870E Tomahawk on eBay

---

## ✅ CONFIRMED COMPONENTS

### Decision Summary:
- **CPU**: AMD Ryzen 9 7900X ✅ **PURCHASED** (inc. thermal paste)
- **Motherboard**: ASUS ProArt X870E-CREATOR WIFI ✅ **PURCHASED** (MSI X870E Tomahawk replaced — not dual GPU capable)
- **GPU (Primary)**: Asus TUF Gaming OC RTX 3090 24GB ✅ **PURCHASED & INSTALLED**
- **GPU (Secondary)**: Asus TUF Gaming OC RTX 3090 24GB ✅ **PURCHASED & INSTALLED**
- **RAM**: G.SKILL Trident Z5 Neo RGB 64GB DDR5-6000 CL30 ✅ **PURCHASED**
- **PSU**: Super Flower Leadex Titanium 1600W ✅ **PURCHASED** (replacement - original Thermaltake DOA)
- **Case**: Fractal Design Torrent ✅ **PURCHASED**
- **Storage**: Samsung 9100 Pro 2TB PCIe 5 x2 ✅ **PURCHASED**
- **CPU Cooler**: Arctic Liquid Freezer III Pro 360 ✅ **PURCHASED**
- **Rear Fan**: 140mm exhaust fan ✅ **PURCHASED**

---

### 1. Processor (CPU)

**Selected**: AMD Ryzen 9 7900X

| Specification | Details |
|---------------|---------|
| **Architecture** | Zen 4 (5nm) |
| **Cores / Threads** | 12-core / 24-thread |
| **Base Clock** | 4.7 GHz |
| **Boost Clock** | 5.4 GHz |
| **TDP** | 170W |
| **Cache** | 76MB total (64MB L3 + 12MB L2) |
| **PCIe Lanes** | 28 total (24 usable + 4 chipset) |
| **Memory Support** | DDR5-5200 (JEDEC), DDR5-6400+ (OC) |
| **Socket** | AM5 (LGA 1718) |

**Price**: £322.50 (purchased, includes thermal paste)

**Why This CPU**:
- ✅ 12 cores ideal for multi-tasking (coding while AI models run)
- ✅ 5.4 GHz boost excellent for single-thread performance
- ✅ 28 PCIe lanes supports x16/x8 dual GPU configuration
- ✅ Proven Zen 4 platform (mature, stable drivers)
- ✅ Excellent price-to-performance (£27-32 per core)
- ✅ 170W TDP manageable with 280mm AIO cooler

**Purchase Sources (UK)**:
- Scan.co.uk: £340-370
- Amazon UK: £350-380
- Overclockers UK: £360-390
- CCL Computers: £330-360

**Status**: ✅ **PURCHASED** (February 19, 2026)

---

### 2. Motherboard

> **Note**: Original motherboard (MSI MAG X870E Tomahawk WiFi, £269.99) was found to be incompatible with dual GPU configuration. The second PCIe slot on the Tomahawk is a chipset-connected x4 slot at the bottom of the board — it does not support GPU-class cards and has insufficient physical clearance in the Fractal Torrent case. MSI's own documentation confirms the board is not recommended for dual GPU setups. The MSI board is being sold on eBay. See Decision Log for full details.

**Selected**: ASUS ProArt X870E-CREATOR WIFI

| Specification | Details |
|---------------|---------|
| **Chipset** | AMD X870E (Premium, 2024) |
| **Socket** | AM5 (LGA 1718) |
| **Form Factor** | ATX |
| **VRM** | 16+2+2 phases (80A SPS power stages) |
| **PCIe Slot 1** | PCIe 5.0 x16 (CPU) — GPU primary |
| **PCIe Slot 2** | PCIe 5.0 x16 (CPU) — GPU secondary; x8/x8 mode when both populated |
| **Chipset Slot** | PCIe 4.0 x4 (chipset) — expansion cards only |
| **Dual GPU Config** | x8/x8 PCIe 5.0 (both slots CPU-direct, equal bandwidth) |
| **M.2 Slots** | 4x (M.2_1: PCIe 5.0 x4; others PCIe 4.0 x4 via chipset) |
| **Memory** | 4x DIMM, DDR5-8000+ (OC), up to 256GB, AMD EXPO |
| **Networking** | **10Gb Ethernet** (Marvell AQtion) + **2.5Gb Ethernet** (Intel) + **Wi-Fi 7** |
| **USB** | **2x USB4 (40Gbps)**, 1x USB 20Gbps, 7x USB 10Gbps |
| **BIOS** | ASUS UEFI |

**Price**: £479.99

**Why This Motherboard**:
- ✅ **True dual GPU support**: Both PCIe 5.0 x16 slots are CPU-direct — genuine x8/x8 for dual RTX 3090
- ✅ **SafeSlot**: Both GPU slots reinforced (rated for heavy cards)
- ✅ **10Gb Ethernet**: Upgrade over MSI's 5Gb — faster NAS transfers and remote access
- ✅ **16+2+2 VRM**: Slightly better than MSI's 16+2+1, excellent for 24/7 AI workloads
- ✅ **256GB memory support**: More headroom than MSI's 192GB limit
- ✅ **Wi-Fi 7** and dual USB4 40Gbps retained
- ✅ **ASUS UEFI**: Mature, well-documented BIOS with full EXPO support

**Dual GPU Configuration (Verified)**:
- **Slot 1** (top): RTX 3090 #1 @ PCIe 5.0 x8 (15.75 GB/s) — CPU-direct
- **Slot 2** (second): RTX 3090 #2 @ PCIe 5.0 x8 (15.75 GB/s) — CPU-direct
- **Equal bandwidth**: Balanced performance, no primary/secondary disadvantage
- **Verified**: ASUS official spec confirms x16 or x8/x8 or x8/x4/x4 modes supported

**Cost Impact vs MSI**:
- MSI X870E Tomahawk: £269.99 (eBay resale TBD — pending listing)
- ASUS ProArt X870E: £479.99
- **Gross cost increase**: £210.00 (partially offset by eBay recovery)

**Status**: ✅ **PURCHASED** (March 7, 2026) — arriving today; MSI being listed on eBay

---

### 3a. Graphics Card (GPU) — Primary

**Selected**: Asus TUF Gaming OC RTX 3090 24GB

| Specification | Details |
|---------------|---------|
| **Model** | Asus TUF Gaming OC RTX 3090 24GB |
| **Architecture** | NVIDIA Ampere (GA102) |
| **CUDA Cores** | 10,496 |
| **Tensor Cores** | 328 (3rd gen) |
| **VRAM** | 24GB GDDR6X |
| **Memory Bus** | 384-bit |
| **Memory Bandwidth** | 936 GB/s |
| **Base Clock** | 1,395 MHz |
| **Boost Clock** | 1,860 MHz (OC mode) |
| **TDP** | 350W (2x 8-pin connectors) |
| **PCIe Interface** | PCIe 4.0 x16 |
| **Cooling** | Triple Axial-tech fans, 2.9 slot |
| **Length** | 299.9mm |

**Price**: £699.39 (purchased)

**Why This GPU**:
- ✅ **24GB VRAM**: Enables 7B-70B models with Q4 quantization
- ✅ **ASUS TUF Series**: Premium cooling solution with triple axial fans
- ✅ **Excellent cooling**: 2.9-slot design with robust heatsink
- ✅ **Military-grade components**: TUF series uses high-quality capacitors and MOSFETs
- ✅ **OC Mode**: Factory overclocked for better performance
- ✅ **Price**: £699.39 is excellent value (mid-range of budget)
- ✅ **Dual GPU Ready**: Can add second TUF 3090 later for matching aesthetics

**ASUS TUF Gaming Features**:
- **Axial-tech Fan Design**: Longer fan blades with barrier ring for improved airflow
- **MaxContact Technology**: 2x larger copper contact area vs reference cooler
- **Dual BIOS**: Performance mode (default) and Quiet mode switch
- **Reinforced Frame**: Full-length backplate prevents PCB flex
- **Auto-Extreme Technology**: Automated manufacturing for better reliability

**Condition**: Used/Refurbished (confirm condition when received)

**Purchase Details**:
- **Source**: (To be documented)
- **Purchase Date**: February 2026
- **Warranty Status**: (To be documented)

**Status**: ✅ **PURCHASED & INSTALLED**

---

### 3b. Graphics Card (GPU) — Secondary

**Selected**: Asus TUF Gaming OC RTX 3090 24GB (matching primary)

| Specification | Details |
|---------------|---------|
| **Model** | Asus TUF Gaming OC RTX 3090 24GB |
| **Architecture** | NVIDIA Ampere (GA102) |
| **CUDA Cores** | 10,496 |
| **Tensor Cores** | 328 (3rd gen) |
| **VRAM** | 24GB GDDR6X |
| **Memory Bus** | 384-bit |
| **Memory Bandwidth** | 936 GB/s |
| **Base Clock** | 1,395 MHz |
| **Boost Clock** | 1,860 MHz (OC mode) |
| **TDP** | 350W (2x 8-pin connectors) |
| **PCIe Interface** | PCIe 4.0 x16 |
| **Cooling** | Triple Axial-tech fans, 2.9 slot |
| **Length** | 299.9mm |

**Price**: £690.10 (purchased March 3, 2026)

**Dual GPU Configuration**:
- **Slot 1** (top): RTX 3090 #1 @ PCIe 5.0 x8 via MSI X870E bifurcation
- **Slot 2** (middle): RTX 3090 #2 @ PCIe 5.0 x8 via MSI X870E bifurcation
- **Total VRAM**: 48GB GDDR6X (24GB per GPU)
- **PSU headroom**: 1600W Titanium — dual 3090 TDP (2x 350W = 700W GPU) well within budget

**What dual GPU enables**:
- Run two 32B models simultaneously (e.g. qwen2.5:32b + qwen2.5-coder:32b) — each ~20GB, total 40GB < 48GB
- Instant model switching with no VRAM swap delay (both models stay loaded)
- Run a single 70B model entirely in VRAM (~45GB Q4 quantized) — higher quality than 32B
- Future: 72B models, larger context windows with both GPUs contributing VRAM

**Purchase Details**:
- **Source**: (To be documented on delivery)
- **Purchase Date**: March 3, 2026
- **Condition**: (To be confirmed on delivery)

**Status**: ✅ **PURCHASED** (ordered March 3, 2026 — awaiting delivery)

---

### 4. System Memory (RAM)

**Selected**: G.SKILL Trident Z5 Neo RGB DDR5-6000 CL30 (2x 32GB)

| Specification | Details |
|---------------|---------|
| **Model** | G.SKILL Trident Z5 Neo RGB |
| **Part Number** | F5-6000J3038F32GX2-TZ5NR (RGB variant) |
| **Capacity** | 64GB (2x 32GB) |
| **Speed** | DDR5-6000 |
| **Timings** | CL30-36-36-96 |
| **True Latency** | 10ns |
| **Voltage** | 1.35V |
| **AMD EXPO** | ✅ Yes (one-click profile) |
| **Memory Bandwidth** | 96.0 GB/s (dual-channel) |
| **RGB** | Yes (controllable, can be disabled) |
| **Form Factor** | U-DIMM (288-pin) |
| **Heat Spreader** | Aluminum with Neo design |

**Price**: £599.99 (purchased)

**Why This RAM**:
- ✅ **CL30 @ 6000 MHz**: 10ns true latency (16% faster than CL36)
- ✅ **AMD EXPO Certified**: One-click BIOS setup, guaranteed compatibility with Ryzen 7000
- ✅ **DDR5-6000**: Optimal frequency for AMD Ryzen 7000 series (sweet spot for stability)
- ✅ **1.35V**: Standard voltage, less heat, easier on memory controller
- ✅ **Premium Performance**: Better latency than CL36 for same price
- ✅ **G.SKILL Quality**: Lifetime warranty, excellent reliability
- ✅ **RGB Included**: No premium paid for RGB (can disable if desired)
- ✅ **Upgrade Path**: Can add another 2x32GB kit for 128GB total

**Performance vs Alternatives**:
- **vs CL36 @ £599**: +16% faster latency (10ns vs 12ns) for £0.99 more
- **vs Trident Z5 Royal CL32 @ £650**: Same 10ns latency, saves £50, better AMD support

**AMD EXPO Profile Benefits**:
- One-click activation in ASRock BIOS
- Pre-configured timings optimized for Ryzen
- Automatic voltage and frequency settings
- No manual tuning required

**RGB Control** (Optional):
- Can be disabled in BIOS
- G.SKILL Trident Z Lighting Control software (Windows)
- Motherboard RGB sync compatible

**Purchase Details**:
- **Source**: (To be documented)
- **Purchase Date**: February 19, 2026
- **Warranty**: Lifetime (G.SKILL standard)

**Status**: ✅ **PURCHASED**

---

### 5. Storage

**Selected**: Samsung 9100 Pro 2TB PCIe Gen5 x2

| Specification | Details |
|---------------|---------|
| **Model** | Samsung 9100 Pro 2TB |
| **Interface** | PCIe 5.0 x4 NVMe |
| **Sequential Read** | ~14,700 MB/s |
| **Sequential Write** | ~13,800 MB/s |
| **Form Factor** | M.2 2280 |
| **NAND** | Samsung V-NAND (own manufacture) |
| **Controller** | Samsung Presto (own manufacture) |
| **Warranty** | 5-year |

**Price**: £502.00 (2x 2TB, purchased from Samsung direct)

**Configuration**:
- **Drive 1** (M.2_1 slot): OS + applications + active models
- **Drive 2** (M.2_2 slot): Model library + overflow storage
- **Total usable storage**: 4TB
- **Backup**: NAS backup (no RAID)

**Why This Drive**:
- ✅ **Same price as 990 Pro**: Gen 5 at no premium — obvious choice
- ✅ **Samsung own NAND + controller**: Best quality control, consistent reliability
- ✅ **14,700 MB/s**: 2x faster than Gen 4 at no extra cost
- ✅ **4TB total**: Ample capacity for OS, apps, and large model library
- ✅ **5-year warranty**: Samsung direct purchase

**Status**: ✅ **PURCHASED** (February 19, 2026)

---

### 6. Power Supply (PSU)

**Selected**: Super Flower Leadex Titanium 1600W Fully Modular

> **Note**: Original Thermaltake Toughpower GF3 1650W was DOA and returned. Replaced with Super Flower Leadex Titanium 1600W.

| Specification | Details |
|---------------|---------|
| **Capacity** | 1600W |
| **Efficiency** | 80+ Titanium |
| **Modularity** | Fully Modular |
| **Colour** | Black |

**Price**: £270.98 (inc. shipping)

**Why This PSU**:
- ✅ **1600W capacity**: Dual GPU ready immediately (no upgrade needed)
- ✅ **80+ Titanium**: 92%+ efficiency at load (upgrade over Gold — less heat, lower running costs)
- ✅ **Fully modular**: Clean cable management, only connect what you need
- ✅ **Super Flower**: OEM manufacturer for many premium PSU brands, excellent reliability reputation

**Status**: ✅ **PURCHASED** (February 27, 2026)

---

### 7. CPU Cooler

**Selected**: Arctic Liquid Freezer III Pro 360

| Specification | Details |
|---------------|---------|
| **Radiator** | 360mm (3x 120mm) |
| **Fans** | 3x Arctic P12 Pro (7-blade) |
| **Radiator Thickness** | 38mm |
| **Pump Head** | Integrated VRM fan included |
| **Socket Support** | AM5, AM4, LGA1851, LGA1700 |
| **Tubing** | 450mm |
| **Warranty** | 6-year |

**Price**: £72.00 (ordered)

**Mounting**: Front of Fractal Torrent (replacing 2x 180mm intake fans)
- 3x 120mm AIO fans serve as front intake
- Rear 140mm fan retained as exhaust

**Why This Cooler**:
- ✅ **360mm radiator**: Maximum thermal headroom for 24/7 AI workloads
- ✅ **Pro fans**: 7-blade P12 Pro design — higher static pressure, less vibration than standard III
- ✅ **38mm thick radiator**: More surface area than standard III
- ✅ **VRM fan**: Integrated pump head fan cools MSI X870E VRM during sustained loads
- ✅ **AM5 bracket included**: No separate purchase needed
- ✅ **Exceptional value**: Pro version outperforms standard III and costs less than most 360mm AIOs
- ✅ **4-10°C improvement**: Over standard Liquid Freezer III under sustained load

**Status**: ✅ **ORDERED** (February 19, 2026)

---

### 8. Computer Case

**Selected**: Fractal Design Torrent

| Specification | Details |
|---------------|---------|
| **Form Factor** | ATX Full Tower |
| **Included Fans** | 2x 180mm front + 1x 140mm rear |
| **GPU Clearance** | 245mm per slot (dual GPU compatible) |
| **AIO Support** | Top 280mm + front 360mm |
| **Fan Mounts** | 6+ locations |
| **Front I/O** | USB-C + USB 3.2 |
| **Dimensions** | 542mm x 242mm x 530mm |

**Price**: £169.99 (purchased)

**Why This Case**:
- ✅ **Best-in-class airflow**: 2x 180mm front fans blow directly on GPUs
- ✅ **Dual GPU ready**: Open interior with 2-3 slot spacing between PCIe slots
- ✅ **Includes quality fans**: 180mm fans included (fewer additional fans needed)
- ✅ **Dust filters**: All intakes filtered
- ✅ **USB-C front panel**: Modern connectivity
- ✅ **Clean design**: No RGB (professional aesthetic)

**Status**: ✅ **PURCHASED** (February 19, 2026)

---

### 9. Rear Case Fan

**Selected**: 140mm exhaust fan

**Price**: £21.12 (purchased)

**Purpose**: Rear exhaust — pulls air through the case and out the back

**Final Fan Configuration**:
| Location | Fans | Direction | Notes |
|----------|------|-----------|-------|
| Front | 3x 120mm (AIO) | Intake | Arctic P12 Pro fans on radiator |
| Bottom | 3x 140mm RGB | Intake | Fractal included fans |
| Rear | 1x 140mm | Exhaust | This fan |

**Spare fans** (from front replacement):
- 2x 180mm RGB — store as spares or use in future build

**Status**: ✅ **PURCHASED** (February 19, 2026)

---

## 💰 COST SUMMARY

### ✅ PURCHASED COMPONENTS (UK Pricing)
| Component | Model | Price (GBP) | Status |
|-----------|-------|-------------|--------|
| CPU | AMD Ryzen 9 7900X (inc. thermal paste) | £322.50 | ✅ **PURCHASED** |
| Motherboard | ASUS ProArt X870E-CREATOR WIFI | £479.99 | ✅ **PURCHASED** — MSI listed on eBay (recovery TBD) |
| GPU (Primary) | Asus TUF Gaming OC RTX 3090 24GB | £699.39 | ✅ **PURCHASED** |
| GPU (Secondary) | Asus TUF Gaming OC RTX 3090 24GB | £690.10 | ✅ **PURCHASED** |
| RAM | G.SKILL Trident Z5 Neo RGB 64GB DDR5-6000 CL30 | £599.99 | ✅ **PURCHASED** |
| PSU | Super Flower Leadex Titanium 1600W (replacement - Thermaltake DOA) | £270.98 | ✅ **PURCHASED** |
| Case | Fractal Design Torrent | £169.99 | ✅ **PURCHASED** |
| Storage | Samsung 9100 Pro 2TB PCIe 5 (x2) | £502.00 | ✅ **PURCHASED** |
| CPU Cooler | Arctic Liquid Freezer III Pro 360 | £72.00 | ✅ **ORDERED** |
| Rear Fan | 140mm exhaust fan | £21.12 | ✅ **PURCHASED** |
| **Total** | | **£3,828.06** (subject to eBay recovery on MSI X870E Tomahawk) | |

**RAM Decision (Finalized)**:
- **Overclockers UK: £599** (DDR5-6000 CL36, 64GB) ← **BEST PRICE**
- CCL Computers: £699.99 (same spec, £100 more expensive)
- Scan.co.uk: £716.99 (CL40, worse latency)
- **Chosen**: Overclockers UK @ £599 for best value

### All Components Purchased
No remaining decisions — build is complete.

### **TOTAL BUILD COST**
**£3,618.06** (all components purchased — includes second RTX 3090)

- Original single-GPU build: £2,927.96
- Second RTX 3090: £690.10
- **Dual GPU total**: £3,618.06

### Budget Target: £2,200-2,400 (Revised for UK Market)
**Status**: 🟠 **OVER BUDGET** — single-GPU build was +£527.96 over target; dual GPU adds a further £690.10. Total overspend vs original target: £1,218.06. All overruns are deliberate upgrades (Gen5 storage, Titanium PSU, dual GPU) — not cost control failures.

**Budget Reality Check**:
- UK component prices higher than initial US-based estimates
- **Premium motherboard chosen**: MSI X870E @ £269.99 (latest chipset, Wi-Fi 7, USB4, 5Gb LAN)
- PSU (1650W) chosen for immediate dual-GPU readiness (eliminates future upgrade cost)
- High-quality components prioritized for 24/7 AI workloads
- **PSU replaced**: Thermaltake DOA; Super Flower Leadex Titanium purchased at £270.98 (+£52.98 vs original)
- **All 10 components purchased**: £2,927.96 total spend ✅
- **Remaining**: Assembly

**✅ PURCHASED Components** (Delivered/In Transit):
- ✅ GPU (Primary): Asus TUF RTX 3090 @ £699.39
- ✅ GPU (Secondary): Asus TUF RTX 3090 @ £690.10 (ordered March 3, 2026)
- ✅ RAM: G.SKILL 64GB DDR5-6000 CL30 @ £599.99
- ✅ Motherboard: MSI X870E TOMAHAWK WIFI @ £269.99
- ✅ CPU: Ryzen 9 7900X + thermal paste @ £322.50
- ✅ PSU: Super Flower Leadex Titanium 1600W @ £270.98 (replacement - Thermaltake DOA)
- ✅ Case: Fractal Design Torrent @ £169.99
- ✅ Storage: Samsung 9100 Pro 2TB PCIe 5 x2 @ £502.00
- ✅ CPU Cooler: Arctic Liquid Freezer III Pro 360 @ £72.00
- ✅ Rear Fan: 140mm exhaust @ £21.12
- **Total Spend**: £3,618.06 🎉

**Options to Reduce Cost** (if needed):
1. **Storage**: Use existing drive temporarily (save £140-180)
2. **Cooler**: Start with budget air cooler instead of 280mm AIO (save £50-90)
3. **Fans**: Skip additional fans initially (save £20-55)
4. **Achievable Savings**: £210-325

**Revised Achievable Cost**: £2,350-2,550 (within £150 of target)

---

## 🎯 NEXT STEPS

### Immediate Decisions Needed:

**✅ BUILD COMPLETE - All Components Purchased**
- CPU: AMD Ryzen 9 7900X + thermal paste (£322.50) - **purchased**
- Motherboard: MSI MAG X870E TOMAHAWK WIFI (£269.99) - **purchased**
- GPU: Asus TUF RTX 3090 24GB (£699.39) - **purchased**
- RAM: G.SKILL 64GB DDR5-6000 CL30 (£599.99) - **purchased**
- PSU: Super Flower Leadex Titanium 1600W (£270.98) - **purchased** (replacement - Thermaltake DOA)
- Case: Fractal Design Torrent (£169.99) - **purchased**
- Storage: Samsung 9100 Pro 2TB PCIe 5 x2 (£502.00) - **purchased**
- CPU Cooler: Arctic Liquid Freezer III Pro 360 (£72.00) - **ordered**
- Rear Fan: 140mm exhaust (£21.12) - **purchased**

**Remaining Components (£250-385)**:

1. **Storage - Primary Drive**
   - [ ] Single 2TB NVMe Gen4 (£140-180)
   - [ ] Options: Samsung 990 Pro / WD Black SN850X
   - [ ] Secondary drive needed? (Optional: +2TB for model library)

2. **CPU Cooler**
   - [ ] 280mm AIO (£90-150) - Recommended: Arctic Liquid Freezer II 280mm
   - [ ] Alternative: Budget air cooler (save £50-90)

3. **Additional Case Fans**
   - [ ] 2-3x 140mm PWM fans (£20-55)
   - [ ] Options: Arctic P14 5-pack / Noctua NF-A14 / be quiet! Pure Wings
   - [ ] Can skip initially if needed (Fractal Torrent includes good stock fans)
   - [ ] Budget (Phanteks P500A)

### Purchase Order Recommendation:
1. **Buy First** (longest lead times):
   - Motherboard (ASRock X670E Taichi) - order ASAP
   - CPU (Ryzen 9 7900X) - check Micro Center stock

2. **Buy Second** (time-sensitive):
   - GPU (RTX 3090) - monitor listings, act on good deals

3. **Buy Third** (standard availability):
   - RAM, Storage, PSU - order together for shipping

4. **Buy Last** (after components arrive):
   - Case, Cooler, Fans - once you know physical sizes

---

## 📋 COMPONENT COMPATIBILITY CHECKLIST

### ✅ Verified Compatible:
- [x] CPU socket matches Motherboard (AM5 ↔ AM5)
- [x] RAM type matches Motherboard (DDR5 ↔ DDR5)
- [x] CPU cooler bracket (AM5 bracket available for all options)
- [x] PSU connectors for dual GPU (4x PCIe 8-pin required, all options provide this)
- [x] Motherboard supports CPU power (24+2+1 VRM handles 170W easily)
- [x] Case fits ATX motherboard (all case options support ATX)

### ⚠️ To Verify After Selection:
- [ ] GPU length fits case (check specific GPU model length vs case clearance)
- [ ] AIO radiator fits case (280mm top/front - verify after case selection)
- [ ] RAM height clearance with AIO (low-profile RAM recommended)
- [ ] PSU length fits case (180mm ATX standard - all cases support)

---

## 🔧 ASSEMBLY NOTES

### Critical Installation Order:
1. **Install CPU into motherboard** (before placing in case)
2. **Install RAM** (slots 2 and 4 for dual-channel, A2/B2 - check manual)
3. **Install M.2 SSD** (M.2_1 slot - remove heatsink first)
4. **Install motherboard into case**
5. **Install PSU** (bottom mount, fan facing down if case has vent)
6. **Install CPU AIO cooler** (top mount exhaust preferred)
7. **Install GPU** (PCIe Slot 1 - top x16 slot)
8. **Cable management** (route behind motherboard tray)
9. **Install case fans** (front intake, top/rear exhaust)
10. **Connect all power/data cables**

### Important Cable Connections:
- **CPU Power**: 8-pin EPS (top-left of motherboard)
- **Motherboard Power**: 24-pin ATX (right side of motherboard)
- **GPU Power**: 2x 8-pin PCIe per GPU (right side of GPU)
- **SATA Power/Data**: If using SATA drives
- **Front Panel**: USB 3.2, USB-C, Audio, Power/Reset (bottom of motherboard)

---

## 💻 SOFTWARE SETUP PLAN

### Operating System Options:

#### Option A: Ubuntu 24.04 LTS (Recommended for AI)
**Pros**:
- Best AI framework support (Ollama, vLLM, llama.cpp)
- Native NVIDIA CUDA support
- Better multi-GPU performance
- Free and open-source

**Cons**:
- Learning curve if not familiar with Linux
- Some Windows apps won't work (need alternatives)

#### Option B: Windows 11 Pro + WSL2
**Pros**:
- Familiar Windows environment
- Can run Windows apps + AI via WSL2
- Good for coding + AI hybrid use

**Cons**:
- Slightly lower AI performance vs native Linux
- More complex setup (WSL2 + CUDA in WSL)
- Windows 11 license cost ($100-140)

### AI Software Stack (After OS Install):

**For Ubuntu**:
1. Install NVIDIA drivers: `sudo apt install nvidia-driver-550`
2. Install CUDA Toolkit: `sudo apt install nvidia-cuda-toolkit`
3. Install Docker (for containers): `curl -fsSL https://get.docker.com | sh`
4. Install Ollama: `curl -fsSL https://ollama.com/install.sh | sh`
5. Install Open WebUI: `docker run -d -p 3000:8080 ghcr.io/open-webui/open-webui:main`
6. Download models: `ollama pull codellama:34b`, `ollama pull mistral:7b`

**For Windows + WSL2**:
1. Install WSL2: `wsl --install`
2. Install Ubuntu 24.04 in WSL2
3. Follow Ubuntu steps above within WSL2
4. Access from Windows browser: `http://localhost:3000`

---

## 📝 NOTES & DECISIONS LOG

### Decision Log:
- **2026-02-12**: Confirmed CPU - AMD Ryzen 9 7900X (£320-380)
- **2026-02-12**: Confirmed PSU - Thermaltake Toughpower GF3 1650W (est. £240)
- **2026-02-12**: Confirmed Case - Fractal Design Torrent (est. £175)
- **2026-02-12**: Initial motherboard choice: ASRock X670E Taichi - later found at £500+ with stock issues
- **2026-02-19**: ✅ **PURCHASED GPU** - Asus TUF Gaming OC RTX 3090 24GB @ £699.39
- **2026-02-19**: ✅ **PURCHASED RAM** - G.SKILL Trident Z5 Neo RGB 64GB DDR5-6000 CL30 @ £599.99
  - Upgraded from planned CL36 to CL30 for only £0.99 more
  - 10ns latency vs 12ns = 16% faster memory access
  - AMD EXPO certified for one-click BIOS setup
  - RGB included at no premium (can be disabled)
- **2026-02-19**: ✅ **PURCHASED MOTHERBOARD** - MSI MAG X870E TOMAHAWK WIFI @ £269.99
  - **Chipset upgrade**: X870E (2024) instead of X670E (2022) - latest premium tier
  - **Comparison evaluated**: MSI X670E @ £229.99 vs ASUS X870 @ £274.99 vs MSI X870E @ £269.99
  - **Chosen MSI X870E**: Best chipset (premium) at best price (£5 less than ASUS mid-tier)
  - **Key features**: Wi-Fi 7, USB4 (40Gbps), 5Gb Ethernet, DDR5-8400+ support
  - **VRM**: 16+2+1 @ 80A (excellent for 24/7 AI workloads, no throttling)
  - **Dual GPU**: x8/x8 PCIe 5.0 balanced configuration (perfect for future second RTX 3090)
  - **Value**: £40 more than X670E budget option, but latest features justify premium
  - **Savings**: £230+ vs ASRock Taichi at inflated £500+ pricing
- **2026-02-19**: ✅ **PURCHASED CPU** - AMD Ryzen 9 7900X + thermal paste @ £322.50
- **2026-02-19**: ✅ **PURCHASED PSU** - Thermaltake Toughpower GF3 1650W @ £218.00
  - Came in £22 under estimated price (£240 estimate)
- **2026-02-19**: ✅ **PURCHASED CASE** - Fractal Design Torrent @ £169.99
  - Came in £5.01 under estimated price (£175 estimate)
- **2026-02-19**: ✅ **PURCHASED STORAGE** - Samsung 9100 Pro 2TB PCIe 5 x2 @ £502.00 (Samsung direct)
  - Upgraded from planned Gen 4 (990 Pro) to Gen 5 (9100 Pro) at identical price
  - 4TB total across two drives (OS + model library)
  - Backup strategy: NAS backup (not RAID 1 — preserves full 4TB capacity)
- **2026-02-19**: ✅ **ORDERED CPU COOLER** - Arctic Liquid Freezer III Pro 360 @ £72.00
  - Upgraded from planned 280mm to 360mm for maximum thermal headroom
  - Pro version: 7-blade P12 Pro fans, 38mm thick radiator, 4-10°C better than standard III
  - Front-mounted in Fractal Torrent (replaces 180mm fans, 3x 120mm AIO fans serve as intake)
  - Exceptional value — Pro outperforms standard III at lower price than most 360mm AIOs
- **2026-02-19**: ✅ **PURCHASED REAR FAN** - 140mm exhaust @ £21.12
  - Fills empty rear mount for complete airflow path
  - Fractal Torrent ships with 2x 180mm (front) + 3x 140mm (bottom) — no rear fan included
  - 2x spare 180mm RGB fans retained from front replacement
- **2026-02-27**: ❌ **THERMALTAKE PSU DOA** - Thermaltake Toughpower GF3 1650W arrived dead on arrival, returned
- **2026-02-27**: ✅ **PURCHASED REPLACEMENT PSU** - Super Flower Leadex Titanium 1600W @ £270.98 (inc. shipping)
  - 80+ Titanium efficiency (upgrade over original 80+ Gold — 92%+ vs 87-90%)
  - Super Flower is a renowned OEM manufacturer (makes PSUs for many premium brands)
  - Additional cost vs original: +£52.98
- **BUILD COMPLETE**: Total spend £2,927.96 (single GPU)
- **2026-03-03**: ✅ **PURCHASED SECOND GPU** - Asus TUF Gaming OC RTX 3090 24GB @ £690.10
- **2026-03-07**: ❌ **MSI X870E TOMAHAWK NOT DUAL GPU CAPABLE** — discovered on attempted installation
  - **Issue**: MSI MAG X870E Tomahawk WiFi has only ONE PCIe 5.0 x16 slot (GPU-grade)
  - The second physical slot is a chipset-connected PCIe 4.0 x4 slot at the bottom of the board
  - Intended for expansion cards (USB, network) only — not GPUs
  - Additionally: insufficient physical clearance in Fractal Torrent case for a GPU in that slot position
  - MSI's own documentation states the board is not recommended for dual GPU setups
  - **Research failure**: Dual GPU capability was assumed from general X870E specs; MSI Tomahawk's specific slot layout was not verified against the official spec sheet
  - **Action**: MSI board listed for sale on eBay
- **2026-03-07**: 🔄 **MOTHERBOARD REPLACEMENT** — ASUS ProArt X870E-CREATOR WIFI selected
  - Verified: 2x PCIe 5.0 x16 slots both CPU-direct (genuine x8/x8 dual GPU support)
  - Verified: ASUS official spec confirms x16 or x8/x8 modes for Ryzen 9000/7000 series
  - Additional upgrades over MSI: 10Gb Ethernet (vs 5Gb), 256GB max RAM (vs 192GB), 16+2+2 VRM (vs 16+2+1)
  - Price: £479.99 — gross cost increase £210 before eBay recovery on MSI
  - Identical model to primary GPU (matching aesthetic, matched cooling)
  - Doubles VRAM to 48GB total — enables simultaneous dual-model inference
  - 70B models (~45GB Q4) now run entirely in VRAM across both GPUs
  - PSU (1600W Titanium) was already sized for dual GPU — no upgrade needed
  - MSI X870E bifurcation handles x8/x8 PCIe 5.0 automatically
  - **New total spend**: £3,618.06

### Research References:
- PCBuildResearch.md - Initial research and component options
- Chosen_Build.md - Detailed technical analysis and component deep-dives

---

**Document Version**: 1.9
**Last Updated**: March 7, 2026
**Next Update**: After ASUS ProArt X870E purchased and dual GPU installation confirmed
