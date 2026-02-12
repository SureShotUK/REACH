# Final AI PC Build - Component Selection
## Single RTX 3090 Now, Dual GPU Upgrade Path

**Build Purpose**: Local AI inference for coding assistance and homework help
**Target Performance**: 7B-70B models (single GPU), 70B-405B models (dual GPU future)
**Budget Target**: £2,200-2,400 (single GPU configuration - adjusted for UK market reality)
**Market**: United Kingdom (all prices in GBP including VAT)
**Build Philosophy**: Best bang for buck, future-proof (add components, not replace)

**Last Updated**: February 12, 2026

---

## Build Status

**Phase**: Component Selection
**Confirmed Components**: 2 of 9
**Remaining Decisions**: 7 components

---

## ✅ CONFIRMED COMPONENTS

### Decision Summary:
- **CPU**: AMD Ryzen 9 7900X ✅
- **Motherboard**: ASRock X670E Taichi ✅
- **RAM**: 64GB DDR5-6000 CL36 (specific brand TBD) ✅

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

**Price**: £320-380

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

**Status**: ✅ **CONFIRMED**

---

### 2. Motherboard

**Selected**: ASRock X670E Taichi

| Specification | Details |
|---------------|---------|
| **Chipset** | AMD X670E (Premium) |
| **Socket** | AM5 (LGA 1718) |
| **Form Factor** | ATX (305mm x 244mm) |
| **VRM** | 24+2+1 phase (105A power stages) |
| **PCIe Slot 1** | PCIe 5.0 x16 (CPU, can run x16 or x8) |
| **PCIe Slot 3** | PCIe 5.0 x8 (CPU, shares with Slot 1) |
| **Dual GPU Config** | x16/x8 or x8/x8 (BIOS selectable) |
| **M.2 Slots** | 4x M.2 (1x PCIe 5.0 + 3x PCIe 4.0) |
| **Memory** | 4x DIMM, DDR5-6400+ (OC), up to 128GB |
| **Networking** | 2.5Gb Ethernet + Wi-Fi 6E |
| **USB** | 2x USB 4.0, 8x USB 3.2 Gen2, 6x USB 2.0 |
| **Audio** | Realtek ALC4082 (7.1 channel) |

**Price**: £280-340

**Why This Motherboard**:
- ✅ **PCIe Configuration**: x16/x8 mode for dual GPU (primary GPU at full speed)
- ✅ **Excellent VRM**: 24+2+1 phase with 105A stages (handles 24/7 AI workloads, no throttling)
- ✅ **No Lane Sharing**: M.2_1 slot independent of GPU slots (can use 2TB NVMe + dual GPU)
- ✅ **4x M.2 Slots**: Plenty of fast storage expansion
- ✅ **PCIe 5.0 Support**: Future-proof for next-gen GPUs
- ✅ **Premium Build Quality**: Heatpipe-connected VRM heatsinks, reinforced PCIe slots
- ✅ **BIOS Quality**: ASRock UEFI stable, easy bifurcation control
- ✅ **Price-to-Feature**: Best value in X670E tier

**Dual GPU Support**:
- Slot 1 (top): RTX 3090 #1 @ x16 or x8
- Slot 3 (bottom): RTX 3090 #2 @ x8
- Physical spacing: 3 slots between (adequate cooling)

**Purchase Sources (UK)**:
- Scan.co.uk: £300-330
- Amazon UK: £310-340
- Overclockers UK: £320-350
- CCL Computers: £280-320

**Status**: ✅ **CONFIRMED**

---

## 🔍 COMPONENTS TO DECIDE

### 3. Graphics Card (GPU) - Primary

**Recommended**: NVIDIA RTX 3090 24GB (Used/Refurbished)

**Key Requirements**:
- 24GB GDDR6X VRAM (enables 7B-70B models)
- 936 GB/s memory bandwidth
- PCIe 4.0 x16 interface
- 3x 8-pin power connectors
- Triple-fan cooling (280mm+ length)

**Options to Consider (UK Market)**:

#### Option A: Used from eBay UK (£550-750)
**Pros**:
- Best value (£550-650 typical)
- Good selection available
- PayPal/eBay buyer protection
- UK-based sellers (faster shipping, easier returns)

**Cons**:
- No warranty typically
- Potential ex-mining cards
- Thermal paste may need replacement

**What to Check**:
- Seller rating (98%+ positive, 50+ sales)
- UK-based seller (avoid import fees)
- No mining use (ask directly)
- Clean fan bearings (no grinding noise)
- Benchmarks/stress test results provided
- Return policy (at least 14 days, 30 days preferred)

#### Option B: CeX (Used with Warranty) (£700-850)
**Pros**:
- 24-month warranty (excellent for used)
- Tested and graded (A/B/C condition)
- Physical stores UK-wide (easy returns)
- Can check stock online

**Cons**:
- More expensive than eBay
- Grade B/C may have cosmetic wear
- Limited stock (varies by location)

**UK Note**: CeX is UK's largest used electronics retailer with 24-month warranty on GPUs

#### Option C: Facebook Marketplace / Gumtree (£500-700)
**Pros**:
- Potentially best prices
- Can inspect before buying (local pickup)
- No fees (unlike eBay)

**Cons**:
- No buyer protection
- Higher risk of scams
- Cash transactions typically
- Need to test in person

**Cons to watch**:
- Meet in public place or test at seller's location
- Bring laptop to test GPU if possible
- Check serial number against manufacturer database

**Decision Needed**:
- [ ] Which purchase option? (eBay UK / CeX / Facebook/Gumtree)
- [ ] Specific model preference? (ASUS TUF / EVGA FTW3 / MSI Gaming X Trio)
- [ ] Price limit? (£550-650 / £650-750 / £750-850)
- [ ] Warranty important? (No warranty OK / Want CeX 24-month warranty)

**Estimated Price**: £550-750 (used), £700-850 (CeX with warranty)

---

### 4. System Memory (RAM)

**Selected**: 64GB DDR5-6000 CL36 (2x 32GB) - No RGB

**Key Requirements**:
- DDR5 (AM5 platform requirement)
- 64GB capacity (sufficient for 70B models with offloading)
- 6000 MHz speed (optimal for Ryzen 7000)
- CL36 latency (good balance of price and performance)
- Dual-channel (2x 32GB sticks, not 4x 16GB)

**Confirmed Choice**: CL36 option (not premium CL30)

**Options to Consider**:

#### Option A: G.Skill Ripjaws S5 DDR5-6000 CL36 (2x 32GB)
**Specs**: 6000MHz, CL36-36-36-96, 1.35V
**Model**: F5-6000J3636F32GX2-RS5K
**Pros**:
- AMD EXPO certified (one-click overclock in BIOS)
- Good availability
- Low-profile design (no RGB, fits under AIO)
- G.Skill reliability

#### Option B: Corsair Vengeance DDR5-6000 CL36 (2x 32GB)
**Specs**: 6000MHz, CL36-36-36-76, 1.35V
**Model**: CMK64GX5M2B6000C36
**Pros**:
- Tighter tertiary timings (76 vs 96) - slightly faster
- AMD EXPO certified
- Corsair reliability/warranty
- Low-profile, simple aesthetic

#### Option C: Kingston Fury Beast DDR5-6000 CL36 (2x 32GB)
**Specs**: 6000MHz, CL36-38-38-80, 1.35V
**Model**: KF560C36BBEK2-64
**Pros**:
- AMD EXPO certified
- Kingston reliability
- Often good pricing
- Low-profile

**UK Market Reality** (Confirmed):
- **Overclockers UK**: £599 for 64GB DDR5-6000 CL36 → **BEST PRICE FOUND**
- **Scan.co.uk**: £716.99 for 64GB DDR5-6000 CL40 (worse latency!) → AVOID
- **Amazon UK**: Checking for competitive pricing (likely £600-750)
- **CCL Computers**: Worth checking
- **eBuyer**: Worth checking

**Market Analysis**:
- UK RAM prices are 2-3x higher than US market
- £599 CL36 vs £717 CL40 → Overclockers actually competitive
- CL36 better than CL40 for same/less money at Overclockers

**RECOMMENDATION**:
Overclockers UK @ £599 for DDR5-6000 CL36 (64GB, 2x32GB) is currently best value, unless:
- Amazon UK has sale (check daily)
- CCL/eBuyer has competitive pricing
- Consider 32GB (£300-400) to save £200-300

**Decision Needed**:
- [ ] Accept £599 for 64GB CL36? (Current best option)
- [ ] Wait for sales/deals? (May save £50-100)
- [ ] Drop to 32GB initially? (Save £200-300, upgrade later)
- [ ] Buy used from eBay UK/CeX? (Risky, save £100-200)

**Current Best Price**: £599 @ Overclockers UK (DDR5-6000 CL36, 64GB)

**Upgrade Path**: Can add another 2x 32GB kit later for 128GB total (useful for dual GPU + 180B+ models)

**Why CL36 (not CL30)**:
- CL30 premium (+$70-100) provides only ~3-5% performance gain
- For LLM inference, CL36 is sufficient (GPU-bound workload)
- Better value for money

---

### 5. Storage - Primary Drive

**Recommended**: 2TB NVMe Gen4 SSD

**Key Requirements**:
- 2TB capacity (20-30 AI models + OS)
- NVMe Gen4 (7,000+ MB/s read)
- M.2 2280 form factor
- DRAM cache (better reliability)
- High endurance rating (600+ TBW)

**Options to Consider**:

#### Option A: Samsung 990 Pro 2TB - £140-180
**Specs**: 7,450 MB/s read, 6,900 MB/s write, 1,200 TBW
**Pros**:
- Excellent performance
- Samsung reliability
- 5-year warranty
- Heatsink version available

**UK Retailers**: Scan, Amazon UK, CCL

#### Option B: WD Black SN850X 2TB - £140-170
**Specs**: 7,300 MB/s read, 6,600 MB/s write, 1,200 TBW
**Pros**:
- Similar performance to 990 Pro
- Often £10-20 less
- 5-year warranty
- Game Mode 2.0 (may help with rapid model loading)

**UK Retailers**: Scan, Amazon UK, Overclockers UK

#### Option C: Crucial T700 2TB (PCIe Gen5) - £240-280
**Specs**: 12,400 MB/s read, 11,800 MB/s write, 1,200 TBW
**Pros**:
- Fastest available (Gen5)
- Future-proof
- 5-year warranty

**Cons**:
- +£100+ premium over Gen4
- Requires Gen5 slot (board has M.2_1 Gen5)
- Overkill for AI (Gen4 loading time already <10 sec)

**UK Retailers**: Scan, Overclockers UK, Amazon UK

**Decision Needed**:
- [ ] Which option? (990 Pro / SN850X / T700)
- [ ] Need secondary drive? (No / Yes: +2TB for model library)

**Estimated Price**: £140-180 (Gen4) or £240-280 (Gen5)

---

### 6. Power Supply (PSU)

**Recommended**: 1000W 80+ Gold (single GPU), upgrade to 1600W later (dual GPU)

**Key Requirements**:
- 1000W capacity (future-proofs for dual GPU upgrade)
- 80+ Gold efficiency minimum (87-90% efficient)
- Fully modular cables
- 6x PCIe 8-pin (6+2) connectors minimum (need 6 for dual RTX 3090)
- ATX 3.0 support (optional but nice)
- 10+ year warranty

**Options to Consider**:

#### Option A: Corsair RM1000x (2024) - £130-170
**Specs**: 1000W, 80+ Gold, Fully Modular
**Pros**:
- 6x PCIe connectors (ready for dual GPU)
- 10-year warranty
- Zero RPM fan mode (silent at low load)
- Excellent build quality

**UK Retailers**: Scan, Amazon UK, CCL

#### Option B: Seasonic FOCUS GX-1000 - £140-180
**Specs**: 1000W, 80+ Gold, Fully Modular
**Pros**:
- 10-year warranty
- Seasonic reliability (OEM for many brands)
- Hybrid fan control

**UK Retailers**: Scan, Overclockers UK, Amazon UK

#### Option C: be quiet! Straight Power 12 1000W - £150-190
**Specs**: 1000W, 80+ Platinum, Fully Modular
**Pros**:
- 10-year warranty
- 80+ Platinum (92% efficiency vs 90% Gold)
- Very quiet operation (be quiet! specialty)
- 6x PCIe connectors

**UK Note**: be quiet! is German brand, very popular in UK/EU for silent builds

**Future Upgrade Note**: When adding second GPU, upgrade to 1600W 80+ Gold (£240-300)
- Recommended: Corsair HX1600i, be quiet! Dark Power 13 1600W

**Decision Needed**:
- [ ] Which option? (Corsair RM1000x / Seasonic FOCUS / be quiet! SP12)
- [ ] Buy 1600W now? (No, buy 1000W / Yes, buy 1600W upfront)

**Estimated Price**: £130-190 (1000W) or £240-300 (1600W)

---

### 7. CPU Cooler

**Recommended**: 280mm AIO Liquid Cooler

**Key Requirements**:
- 280mm radiator (2x 140mm fans)
- 250W+ cooling capacity (Ryzen 9 7900X = 170W TDP)
- Low noise operation (<35 dBA)
- AM5 bracket included (or order separately)

**Options to Consider**:

#### Option A: Arctic Liquid Freezer II 280 - £90-110
**Specs**: 280mm, 2x 140mm P14 fans, VRM fan
**Pros**:
- Excellent cooling (270W+ capacity)
- Includes VRM fan (cools motherboard VRM)
- Very quiet (<30 dBA)
- Best price-to-performance
- 6-year warranty

**UK Retailers**: Scan, Amazon UK, Overclockers UK

#### Option B: NZXT Kraken X63 - £120-150
**Specs**: 280mm, 2x 140mm Aer P fans, RGB
**Pros**:
- Clean aesthetic
- NZXT CAM software control
- RGB pump cap (if desired)
- 6-year warranty

**Cons**:
- No VRM fan
- +£30 premium for aesthetics

**UK Retailers**: Scan, Overclockers UK, Amazon UK

#### Option C: be quiet! Pure Loop 2 280mm - £100-130
**Specs**: 280mm, 2x 140mm Pure Wings fans
**Pros**:
- Very quiet operation (be quiet! specialty)
- 3-year warranty
- Good cooling performance
- Popular in UK

**UK Note**: be quiet! very popular in UK for silent builds

**Decision Needed**:
- [ ] Which option? (Arctic 280mm / NZXT 280mm / be quiet! 280mm)
- [ ] RGB important? (No RGB / RGB desired)
- [ ] Priority? (Performance / Silence / Aesthetics)

**Estimated Price**: £90-150 (depending on choice)

---

### 8. Computer Case

**Recommended**: ATX Mid/Full Tower with Excellent Airflow

**Key Requirements**:
- ATX form factor support
- Supports dual 300mm+ GPUs (with 2-3 slot spacing)
- Top 280mm AIO support OR front 280mm/360mm support
- 6+ fan mounting locations
- Good cable management (PSU shroud, routing holes)
- Front I/O: USB-C, USB 3.2
- Support for 180mm+ PSU

**Options to Consider**:

#### Option A: Fractal Design Torrent - £160-190
**Specs**: ATX, 2x 180mm front fans included, 245mm GPU clearance per slot
**Pros**:
- Best-in-class airflow (2x massive 180mm fans)
- Excellent GPU cooling (front fans blow directly on GPUs)
- Open interior (easy dual GPU installation)
- Dust filters on all intakes
- USB-C front panel
- Fractal Design is Swedish (good EU support)

**Cons**:
- Large footprint (542mm x 242mm x 530mm)
- No RGB (clean aesthetic)

**UK Retailers**: Scan, Overclockers UK, Amazon UK

#### Option B: Lian Li O11 Dynamic EVO - £160-200
**Specs**: ATX, 0 fans included (must buy separately)
**Pros**:
- Beautiful dual-chamber design
- Excellent for custom builds
- Multiple radiator mounting options
- Tempered glass
- Very popular in UK enthusiast community

**Cons**:
- No included fans (add £50-80)
- Less focused on airflow vs aesthetics
- More complex cable management

**UK Retailers**: Overclockers UK, Scan, Amazon UK

#### Option C: be quiet! Pure Base 500DX - £100-130
**Specs**: ATX, 3x 140mm Pure Wings fans included
**Pros**:
- Excellent airflow (mesh front)
- Very quiet (be quiet! specialty)
- Good value (includes 3 quality fans)
- USB-C front panel
- Popular in UK

**UK Note**: be quiet! German brand, very popular in UK for silent builds

**Decision Needed**:
- [ ] Which option? (Fractal Torrent / Lian Li O11 / be quiet! 500DX)
- [ ] Priority? (Maximum airflow / Aesthetics / Budget / Silence)

**Estimated Price**: £100-190 (depending on choice)

---

### 9. Additional Case Fans

**Recommended**: 2-3x 120mm or 140mm (depending on case choice)

**Requirements**:
- PWM control (4-pin)
- Quiet operation (<25 dBA at max RPM)
- Good static pressure for exhaust

**Options (UK)**:

#### Option A: Arctic P14 PWM (5-pack) - £25-35
- Excellent value (140mm, 5 fans)
- Very quiet
- Good static pressure
- Available: Amazon UK, Scan

#### Option B: Noctua NF-A14 PWM (2-pack) - £45-55
- Premium quality (Austrian brand, popular in UK)
- Ultra-quiet
- Best performance
- 6-year warranty
- Available: Scan, Overclockers UK, Amazon UK

#### Option C: be quiet! Pure Wings 2 (3-pack) - £20-30
- Very quiet operation
- Good value
- Popular in UK
- Available: Scan, Amazon UK

**Note**: Fractal Torrent includes 2x 180mm + 1x 140mm (may only need 1-2 additional)

**Decision Needed**:
- [ ] Depends on case choice
- [ ] How many additional fans needed?
- [ ] Priority? (Value / Premium quality / Silence)

**Estimated Price**: £20-55 (depending on choice)

---

## 💰 COST SUMMARY

### ✅ CONFIRMED COMPONENTS (UK Pricing)
| Component | Model | Price (GBP) | Status |
|-----------|-------|-------------|--------|
| CPU | AMD Ryzen 9 7900X | £320-380 | ✅ Confirmed |
| Motherboard | ASRock X670E Taichi | £280-340 | ✅ Confirmed |
| RAM | 64GB (2x32GB) DDR5-6000 CL36 | £599 | ✅ Confirmed (Overclockers UK) |
| PSU | Thermaltake Toughpower GF3 1650W | £240 | ✅ Confirmed (Scan.co.uk) |
| Case | Fractal Design Torrent | £175 | ✅ Confirmed |
| **Confirmed Subtotal** | | **£1,614-1,734** | |

**RAM Decision (Finalized)**:
- **Overclockers UK: £599** (DDR5-6000 CL36, 64GB) ← **BEST PRICE**
- CCL Computers: £699.99 (same spec, £100 more expensive)
- Scan.co.uk: £716.99 (CL40, worse latency)
- **Chosen**: Overclockers UK @ £599 for best value

### Components to Decide (UK Pricing)
| Component | Estimated Price Range (GBP) |
|-----------|----------------------------|
| GPU (RTX 3090 24GB used) | £550-750 |
| Storage (2TB NVMe Gen4) | £140-180 |
| PSU (1000W Gold) | £130-190 |
| CPU Cooler (280mm AIO) | £90-150 |
| Case | £100-190 |
| Additional Fans | £20-55 |
| **Estimated Subtotal** | | **£1,030-1,515** |

### **TOTAL ESTIMATED BUILD COST (UK MARKET)**
**£2,229-2,515** (single GPU configuration)

### Budget Target: £2,200-2,400 (Revised for UK Market)
**Status**: 🟢 **ON TARGET** (with mid-range component selections)

**Budget Philosophy**: Best bang for buck, buy once (add later, don't replace)
- 64GB RAM confirmed @ £599 (best UK price)
- Plan for second GPU addition (not replacement)
- Focus on quality components that won't need upgrading

**Achievable Build Cost**: £2,229-2,350 with smart mid-range choices

---

## 🎯 NEXT STEPS

### Immediate Decisions Needed:
1. **GPU Purchase Strategy**
   - [ ] Set price limit ($700-750 / $750-850 / $850-900)
   - [ ] Choose source (eBay / Micro Center / EVGA)
   - [ ] Start monitoring listings

2. **RAM Selection**
   - [ ] Budget option (CL36, $250-280) OR Premium option (CL30, $320-360)
   - [ ] RGB preference?

3. **Storage Configuration**
   - [ ] Single 2TB OR Dual drive setup?
   - [ ] Gen4 ($160-200) OR Gen5 ($280-320)?

4. **Case Priority**
   - [ ] Maximum airflow (Fractal Torrent)
   - [ ] Aesthetics (Lian Li O11)
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
- [x] PSU connectors for dual GPU (6x PCIe 8-pin required, all options provide this)
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
- **GPU Power**: 3x 8-pin PCIe (right side of GPU)
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
- **2026-02-12**: Confirmed CPU - AMD Ryzen 9 7900X ($400-450)
- **2026-02-12**: Confirmed Motherboard - ASRock X670E Taichi ($320-370)
- **2026-02-12**: Confirmed RAM Type - 64GB DDR5-6000 CL36, no RGB (specific brand and current price TBD)
- **2026-02-12**: Note - RAM prices higher than initial estimates, need current market pricing
- *(Future decisions will be logged here)*

### Research References:
- PCBuildResearch.md - Initial research and component options
- Chosen_Build.md - Detailed technical analysis and component deep-dives

---

**Document Version**: 1.0
**Last Updated**: February 12, 2026
**Next Update**: After GPU, RAM, and Storage decisions
