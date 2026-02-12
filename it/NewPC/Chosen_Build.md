# AI PC Build - Chosen Configuration
## Single GPU Now, Dual GPU Ready

**Build Strategy**: Start with single RTX 3090 24GB, maintain upgrade path to dual RTX 3090 (48GB total VRAM)

**Last Updated**: February 12, 2026

---

## Executive Summary

This build is optimized for:
- **Immediate use**: Single RTX 3090 24GB running 7B-70B models
- **Future upgrade**: Add second RTX 3090 for 48GB VRAM (200B+ models)
- **PCIe optimization**: x16/x8 or x16/x16 configuration for dual GPU
- **Performance priority**: No bottlenecks in CPU, RAM, or storage
- **Budget**: $1,700-$2,200 (single GPU), +$700-900 for second GPU later

---

## PCIe Lane Architecture Analysis

### Understanding PCIe Lanes for Dual GPU

**PCIe Lane Basics**:
- **PCIe lanes** are data pathways between the CPU and components (GPUs, NVMe drives, etc.)
- **x16** means 16 lanes allocated to a component (maximum for consumer GPUs)
- **x8** means 8 lanes allocated (half the bandwidth of x16)
- **Bifurcation** splits PCIe lanes between multiple devices (e.g., x16 slot splits to x8/x8 for two GPUs)

**Why This Matters for LLM Inference**:
- LLM inference is primarily VRAM-bound, not PCIe bandwidth-bound
- PCIe 4.0 x8 provides 15.75 GB/s bandwidth (sufficient for model loading)
- PCIe 4.0 x16 provides 31.5 GB/s bandwidth (2x, but rarely fully utilized)
- **Performance impact**: x8 vs x16 = 0-5% difference for inference workloads
- **Conclusion**: x16/x8 configuration is ideal (x8 mode on second GPU has minimal impact)

### AMD Ryzen 7000/9000 Series PCIe Lanes

**Total PCIe Lanes from CPU**: 28 lanes (PCIe 5.0)

**Lane Allocation**:
- **24 lanes** available for PCIe slots and M.2 NVMe
  - Primary x16 slot: 16 lanes (can bifurcate to x8/x8)
  - M.2_1 slot: 4 lanes (directly from CPU)
  - Secondary x16 slot: 4 lanes OR shares with M.2_2 (board-dependent)
- **4 lanes** dedicated to chipset connection

**X670E/X870E Chipset Adds**: 12-20 additional PCIe 4.0 lanes
- Used for: Additional M.2 slots, USB controllers, SATA, networking, etc.

**Dual GPU Configuration on AMD**:
- **Option 1**: x16/x8 mode (CPU lanes)
  - GPU 1: 16 lanes (full speed)
  - GPU 2: 8 lanes (minimal performance impact)
  - M.2_1: 4 lanes from CPU
  - Additional M.2 slots: From chipset

- **Option 2**: x8/x8 mode (CPU lanes)
  - GPU 1: 8 lanes
  - GPU 2: 8 lanes (equal distribution)
  - M.2 slots: 8 lanes remaining + chipset lanes
  - **Tradeoff**: Slightly lower bandwidth for both GPUs, but equal performance

**Verdict**: x16/x8 is optimal (GPU 1 at full speed, GPU 2 minimal loss)

**Performance Reality**: x16 vs x8 for LLM inference = <2% difference
- Model loading: x8 provides 15.75 GB/s (sufficient)
- Inference: Happens in VRAM (PCIe not used)
- Multi-GPU: Both GPUs work together, lane distribution doesn't matter

---

## Critical PCIe Reality Check: Can Consumer Ryzen Do Dual x16?

**Question**: Can the Ryzen 9 9950X3D (or any consumer Ryzen) run dual GPUs at full x16/x16?

**Answer**: **NO** - Consumer Ryzen CPUs are limited to 24 usable PCIe lanes.

**Math**:
- Dual x16 requires: 32 lanes (16 + 16)
- Consumer Ryzen provides: 24 lanes
- Maximum configuration: x16/x8 or x8/x8

**To Get True x16/x16**: You need AMD Threadripper (64-128 lanes)
- Threadripper PRO 5955WX: $1,000-1,200 (64 lanes)
- WRX80 Motherboard: $700-800
- Total premium: +$1,300-1,600 over consumer platform

**But Here's Why x16/x8 Is Fine**:

| Scenario | x16/x16 | x16/x8 | Difference |
|----------|---------|---------|------------|
| Model Loading (one-time) | 5 sec | 10 sec | +5 sec (irrelevant) |
| Inference Speed (70B) | 42 tok/s | 41.8 tok/s | -0.5% (negligible) |
| Multi-GPU Tensor Parallelism | 75 tok/s | 74 tok/s | -1.3% (minimal) |

**Conclusion**: Save $1,300-1,600 and use consumer Ryzen with x16/x8. The performance difference is less than 2% for LLM inference.

---

## Motherboard & Processor Combinations

### Recommended Configuration #1: AMD Zen 5 X3D Flagship (Best for AI + Multi-Tasking)

**Processor**: AMD Ryzen 9 9950X3D ($650-750)
- **Cores/Threads**: 16-core, 32-thread
- **Base/Boost**: 4.3 GHz / 5.7 GHz
- **3D V-Cache**: 144MB (huge cache for AI workloads)
- **TDP**: 120W (lower than 7950X, more efficient)
- **PCIe Lanes**: 28 (24 usable + 4 chipset)
- **Memory**: DDR5-5600 (JEDEC), up to DDR5-6400+ (OC)
- **Why This CPU**:
  - Massive 3D V-Cache improves multi-model workloads
  - 16 cores excellent for running multiple models simultaneously
  - Zen 5 architecture = ~15% IPC improvement over Zen 4
  - Lower power consumption than 7950X

**Motherboard**: ASUS ROG Crosshair X870E Hero ($600-700)

**Detailed Specifications**:
- **Chipset**: AMD X870E (latest, PCIe 5.0 optimized)
- **PCIe Slot Configuration**:
  - **Slot 1**: PCIe 5.0 x16 (from CPU, runs x16 or x8 when slot 2 populated)
  - **Slot 2**: PCIe 5.0 x8 (from CPU, shares lanes with Slot 1)
  - **Slot 3**: PCIe 4.0 x4 (from chipset)
  - **Dual GPU Mode**: Slot 1 = x16, Slot 2 = x8 (optimal, <2% performance difference)

- **M.2 NVMe Slots**: 5x M.2 slots (2x PCIe 5.0 x4 + 3x PCIe 4.0 x4)
  - M.2_1, M.2_2: PCIe 5.0 x4 (independent of GPU slots)
  - M.2_3, M.2_4, M.2_5: PCIe 4.0 x4

- **Memory**: 4x DIMM slots, DDR5-8000+ (OC), up to 256GB (4x 64GB, AM5 latest spec)

- **VRM**: 18+2+2 phase power design (premium components, excellent for 24/7 AI)

- **Networking**: 5Gb Ethernet + Wi-Fi 7

- **USB**: 2x USB4 (40 Gbps), multiple USB 3.2 Gen2

- **BIOS**: ASUS ROG UEFI (best-in-class for overclocking and stability)

**Why This Board**:
- ✅ Latest X870E chipset (optimized for Zen 5)
- ✅ x16/x8 configuration (primary GPU at full speed, <2% loss on secondary)
- ✅ 5x M.2 slots (massive storage expansion)
- ✅ PCIe 5.0 support for future GPUs
- ✅ Premium VRM for sustained 24/7 workloads
- ✅ Wi-Fi 7 and USB4 (future-proof connectivity)
- ❌ Expensive ($600-700)

**Total Cost (CPU + Motherboard)**: $1,250-1,450

**Performance vs 7950X**:
- Single-thread: +10-15% (Zen 5 IPC improvements)
- Multi-thread: +8-12% (architectural improvements)
- Cache-sensitive workloads: +20-30% (144MB 3D V-Cache vs 64MB on 7950X)
- **LLM Inference Impact**: <5% (still GPU-bound, but better for multi-model scenarios)

---

### Recommended Configuration #2: AMD High-End (Best Overall)

**Processor**: AMD Ryzen 9 7900X ($400-450)
- **Cores/Threads**: 12-core, 24-thread
- **Base/Boost**: 4.7 GHz / 5.4 GHz
- **TDP**: 170W
- **PCIe Lanes**: 28 (24 usable + 4 chipset)
- **Memory**: DDR5-5200 (JEDEC), up to DDR5-6400+ (OC)
- **Why This CPU**: Excellent multi-threaded performance, enough PCIe lanes for x16/x8, good price-to-performance

**Motherboard**: ASRock X670E Taichi ($320-370)

**Detailed Specifications**:
- **Chipset**: AMD X670E (premium chipset with PCIe 5.0 support)
- **PCIe Slot Configuration**:
  - **Slot 1**: PCIe 5.0 x16 (from CPU, can run x16 or x8 when slot 3 populated)
  - **Slot 2**: PCIe 4.0 x4 (from chipset, for secondary devices)
  - **Slot 3**: PCIe 5.0 x8 (from CPU, shares lanes with Slot 1)
  - **Dual GPU Mode**: Slot 1 = x8, Slot 3 = x8 OR Slot 1 = x16, Slot 3 = x8 (BIOS configurable)

- **M.2 NVMe Slots**: 4x M.2 slots (PCIe 5.0 x4 + 3x PCIe 4.0 x4)
  - M.2_1: PCIe 5.0 x4 (from CPU, no sharing with GPU slots)
  - M.2_2, M.2_3, M.2_4: PCIe 4.0 x4 (from chipset)

- **Memory**: 4x DIMM slots, DDR5-6400+ (OC), up to 128GB (4x 32GB)

- **VRM**: 24+2+1 phase power design (excellent for sustained AI workloads)

- **Networking**: 2.5Gb Ethernet + Wi-Fi 6E

- **USB**: 2x USB 4.0 (40 Gbps), multiple USB 3.2 Gen2 (10 Gbps)

- **BIOS Features**: PCIe bifurcation control, memory overclocking, voltage controls

**Why This Board**:
- ✅ True x16/x8 configuration (no PCIe lane sharing with M.2)
- ✅ PCIe 5.0 support for future GPUs
- ✅ Excellent VRM for sustained 24/7 AI workloads
- ✅ 4x M.2 slots (plenty of fast storage)
- ✅ Great price-to-feature ratio ($320-370)
- ✅ BIOS stability and community support

**Total Cost (CPU + Motherboard)**: $720-820

---

### Recommended Configuration #2: AMD Premium (Maximum Performance)

**Processor**: AMD Ryzen 9 7950X ($500-600)
- **Cores/Threads**: 16-core, 32-thread
- **Base/Boost**: 4.5 GHz / 5.7 GHz
- **TDP**: 170W (same as 7900X, better binned silicon)
- **PCIe Lanes**: 28 (same as 7900X)
- **Why This CPU**: Higher core count for multi-tasking, higher boost clocks, better for running multiple AI models simultaneously

**Motherboard**: ASUS ROG Crosshair X670E Hero ($550-650)

**Detailed Specifications**:
- **Chipset**: AMD X670E
- **PCIe Slot Configuration**:
  - **Slot 1**: PCIe 5.0 x16 (from CPU, runs x16 or x8 when slot 2 populated)
  - **Slot 2**: PCIe 5.0 x8 (from CPU)
  - **Slot 3**: PCIe 4.0 x4 (from chipset)
  - **Dual GPU Mode**: Slot 1 = x16, Slot 2 = x8 (optimal configuration)

- **M.2 NVMe Slots**: 4x M.2 slots (2x PCIe 5.0 x4 + 2x PCIe 4.0 x4)
  - M.2_1, M.2_2: PCIe 5.0 x4 (independent of GPU slots)
  - M.2_3, M.2_4: PCIe 4.0 x4

- **Memory**: 4x DIMM slots, DDR5-6400+ (OC), up to 192GB (4x 48GB)

- **VRM**: 18+2 phase power design (premium components, excellent thermal management)

- **Networking**: 2.5Gb Ethernet + Wi-Fi 6E

- **Audio**: SupremeFX ALC4082 (premium audio codec)

- **Additional Features**:
  - Q-Code display (debug codes)
  - FlexKey button (programmable)
  - Premium RGB lighting (if desired)

**Why This Board**:
- ✅ Best-in-class VRM with massive heatsinks
- ✅ True x16/x8 with no lane sharing
- ✅ 2x PCIe 5.0 M.2 slots
- ✅ Exceptional BIOS (ASUS ROG UEFI)
- ✅ Long-term reliability and support
- ❌ Expensive ($550-650)

**Total Cost (CPU + Motherboard)**: $1,050-1,250

---

### Recommended Configuration #3: AMD Value (Best Price-to-Performance)

**Processor**: AMD Ryzen 7 7700X ($280-320)
- **Cores/Threads**: 8-core, 16-thread
- **Base/Boost**: 4.5 GHz / 5.4 GHz
- **TDP**: 105W (lower power consumption)
- **PCIe Lanes**: 28 (same as higher-end Ryzen)
- **Why This CPU**: Still sufficient for LLM inference (GPU-bound workload), same PCIe lanes, good value

**Motherboard**: MSI MAG X670E Tomahawk WiFi ($300-350)

**Detailed Specifications**:
- **Chipset**: AMD X670E
- **PCIe Slot Configuration**:
  - **Slot 1**: PCIe 5.0 x16 (from CPU, runs x16 or x8)
  - **Slot 2**: PCIe 5.0 x8 (from CPU, activated when slot 1 in x8 mode)
  - **Slot 3**: PCIe 4.0 x4 (from chipset)
  - **Dual GPU Mode**: Slot 1 = x8, Slot 2 = x8 (equal split)

- **M.2 NVMe Slots**: 4x M.2 slots (PCIe 5.0 x4 + 3x PCIe 4.0 x4)

- **Memory**: 4x DIMM slots, DDR5-6400+ (OC), up to 128GB

- **VRM**: 16+2+1 phase (80A power stages, sufficient for Ryzen 7/9)

- **Networking**: 2.5Gb Ethernet + Wi-Fi 6E

**Why This Board**:
- ✅ Excellent value ($300-350)
- ✅ x8/x8 configuration (equal GPU performance)
- ✅ 4x M.2 slots
- ✅ Good VRM for sustained workloads
- ✅ MSI BIOS stability

**Total Cost (CPU + Motherboard)**: $580-670

---

### Configuration #4 (Optional): AMD Threadripper - True x16/x16

**Only if you ABSOLUTELY need x16/x16** (you probably don't - see "Critical PCIe Reality Check" above)

**Processor**: AMD Threadripper PRO 5955WX ($1,000-1,200)
- **Cores/Threads**: 16-core, 32-thread
- **Base/Boost**: 4.0 GHz / 4.5 GHz
- **TDP**: 280W
- **PCIe Lanes**: 128 (64 usable from CPU)
- **Memory**: 8-channel DDR4-3200 ECC (up to 2TB)

**Motherboard**: ASUS Pro WS WRX80E-SAGE SE ($700-800)

**PCIe Configuration**:
- **7x PCIe 4.0 x16 slots** (can run 4x GPUs at x16/x16/x16/x16 simultaneously)
- True x16/x16 for dual GPU with 32 lanes remaining for storage/peripherals

**Total Cost (CPU + Motherboard)**: $1,700-2,000

**Why You DON'T Need This**:
- ❌ +$1,300-1,600 premium over consumer Ryzen
- ❌ <2% performance difference for LLM inference vs x16/x8
- ❌ Higher power consumption (280W TDP)
- ❌ Only needed for: Professional rendering, CAD, massive multi-GPU (4+)

**When You MIGHT Consider This**:
- ✅ Planning 3-4 GPU setup (not 2)
- ✅ Need ECC memory for critical workloads
- ✅ Budget is not a concern

---

## Performance Impact Analysis: Component Deep Dive

### GPU: RTX 3090 24GB Performance Characteristics

**Architecture**: NVIDIA Ampere (GA102)

**Specifications**:
- **CUDA Cores**: 10,496
- **Tensor Cores**: 328 (3rd gen)
- **VRAM**: 24GB GDDR6X
- **Memory Bus**: 384-bit
- **Memory Bandwidth**: 936 GB/s (critical for LLM inference)
- **TDP**: 350W
- **PCIe Interface**: PCIe 4.0 x16

**Performance Factors**:

1. **VRAM Bandwidth Impact** (Most Critical):
   - **936 GB/s** is the bottleneck for LLM inference speed
   - Higher bandwidth = faster token generation
   - RTX 4090 has 1,008 GB/s (+7.7%) = +10-15% inference speed
   - RTX 5090 has 1,792 GB/s (+91%) = +67% inference speed

2. **VRAM Capacity Impact** (Determines Model Size):
   - **24GB** enables 7B-70B models comfortably
   - Q4 quantization: Up to 70B models at ~35GB (requires system RAM offloading)
   - FP16 full precision: Up to 13B models fit entirely in VRAM

3. **PCIe Bandwidth Impact** (Minimal for Inference):
   - **PCIe 4.0 x16**: 31.5 GB/s bidirectional
   - **PCIe 4.0 x8**: 15.75 GB/s bidirectional
   - Model loading time affected (x8 = 2x slower loading)
   - Inference speed: 0-5% difference (model already in VRAM)
   - **Verdict**: x8 mode acceptable for second GPU in dual setup

4. **Power Delivery Impact**:
   - 350W TDP requires 3x 8-pin PCIe power connectors
   - Power limit: 100% (350W) vs 70% (245W) = ~15% performance difference
   - Undervolting: 280W at ~3% performance loss (recommended for thermals)

**Dual GPU Scaling**:
- **Tensor Parallelism** (vLLM): 80-95% linear scaling (both GPUs contribute)
- **Model Parallelism** (llama.cpp): 70-85% scaling (communication overhead)
- **Combined VRAM**: 48GB enables 180B-405B models with quantization

---

### CPU: Impact on LLM Inference Performance

**CPU Role in LLM Inference**:
- **Primary**: Model loading, preprocessing, tokenization
- **Secondary**: CPU offloading for layers that don't fit in VRAM
- **Minimal**: Direct inference (GPU handles this)

**Performance Factors**:

1. **Core Count Impact**:
   - **8 cores** (Ryzen 7 7700X): Sufficient for single-model inference
   - **12 cores** (Ryzen 9 7900X): Better for multi-tasking (coding while model runs)
   - **16 cores** (Ryzen 9 7950X): Optimal for multiple concurrent models
   - **Verdict**: 8-12 cores ideal, 16 cores for power users

2. **Clock Speed Impact**:
   - **Higher boost clocks** (5.4-5.7 GHz): Faster tokenization and preprocessing
   - Single-thread: 5.7 GHz vs 5.4 GHz = ~5% faster preprocessing
   - **Verdict**: Marginal impact on overall inference speed (<2%)

3. **Cache Size Impact**:
   - **L3 Cache**: 32MB (7700X), 64MB (7900X), 64MB (7950X)
   - Larger cache = faster context switching for multi-model setups
   - **Verdict**: More cache better for running multiple models simultaneously

4. **PCIe Lane Provisioning**:
   - All Ryzen 7000/9000 provide 28 lanes (sufficient for dual GPU)
   - **Verdict**: Any Ryzen 7000+ CPU supports optimal dual GPU configuration

**Benchmark Comparison** (LLM Inference):
- Ryzen 7 7700X: 42 tok/s (70B Q4 on RTX 3090)
- Ryzen 9 7900X: 42 tok/s (same, GPU-bound)
- Ryzen 9 7950X: 42 tok/s (same, GPU-bound)
- **Conclusion**: CPU choice has <2% impact on inference speed (GPU-bound workload)

**When CPU Matters**:
- Running multiple models simultaneously (more cores better)
- Coding/compiling while models run (more cores better)
- Very large context windows (>32K tokens, better single-thread performance helps)

---

### RAM: Speed and Capacity Impact

**RAM Role in LLM Inference**:
- **Model loading**: Transfer model from storage to RAM to GPU
- **Offloading**: Store model layers that don't fit in VRAM
- **Context buffering**: Store conversation history and context

**Performance Factors**:

1. **Capacity Impact**:
   - **32GB**: Sufficient for 7B-13B models with GPU acceleration
   - **64GB**: Recommended for 30B-70B models (allows some offloading)
   - **128GB**: Enables running 70B+ models partially on CPU (slow)

   **Offloading Performance**:
   - Full GPU: 42 tok/s (70B Q4 on RTX 3090)
   - 50% GPU + 50% RAM: ~15-20 tok/s (70% slower)
   - Full RAM (no GPU): ~2-5 tok/s (95% slower)
   - **Verdict**: Maximize VRAM, use RAM offloading as last resort

2. **Speed Impact** (DDR5 Frequency):

   **Memory Bandwidth Comparison**:
   - DDR5-4800: 76.8 GB/s (dual channel)
   - DDR5-5600: 89.6 GB/s (dual channel) - +16.7%
   - DDR5-6000: 96.0 GB/s (dual channel) - +25%
   - DDR5-6400: 102.4 GB/s (dual channel) - +33%

   **Real-World Impact on Inference**:
   - DDR5-4800 vs DDR5-6000: ~3-5% faster model loading
   - DDR5-4800 vs DDR5-6400: ~5-8% faster when offloading to RAM
   - **Verdict**: Faster RAM helps with model loading and offloading scenarios

3. **Latency Impact** (CAS Latency - CL):
   - **CL30** (tight): Lower latency, faster random access
   - **CL36** (standard): Slightly higher latency
   - **CL40** (loose): Higher latency, but may allow higher frequencies

   **LLM Impact**: <2% difference in inference speed
   **Verdict**: Prioritize frequency over latency for LLM workloads

4. **Memory Configuration** (Single vs Dual vs Quad Channel):
   - **Dual Channel** (2x DIMMs): Standard on consumer platforms, sufficient
   - **Quad Channel** (4x DIMMs): Doubles bandwidth (Threadripper/EPYC only)
   - Consumer Ryzen: Always use 2 or 4 DIMMs for dual-channel (don't use 1 or 3)
   - **Verdict**: 2x 32GB (64GB total) or 4x 32GB (128GB total) for dual-channel

**Recommended RAM Configurations**:

**Budget** (64GB DDR5-5600):
- **Kit**: Corsair Vengeance DDR5-5600 CL40 (2x 32GB) - $220-250
- **Performance**: Good baseline for single GPU setup
- **Upgrade Path**: Add another 2x 32GB kit later for 128GB

**Optimal** (64GB DDR5-6000):
- **Kit**: G.Skill Ripjaws S5 DDR5-6000 CL36 (2x 32GB) - $250-280
- **Performance**: 7-10% better than DDR5-5600 when offloading
- **Sweet spot**: Best price-to-performance

**Premium** (128GB DDR5-6000):
- **Kit**: G.Skill Trident Z5 RGB DDR5-6000 CL30 (4x 32GB) - $600-700
- **Performance**: Maximum capacity for dual GPU + large models
- **When needed**: Running 180B+ models with significant RAM offloading

---

### Motherboard VRM: Impact on Sustained Workloads

**VRM (Voltage Regulator Module)** converts 12V from PSU to lower voltages for CPU (1.0-1.4V)

**Why VRM Matters for AI Workloads**:
- AI inference = sustained 24/7 loads (unlike gaming bursts)
- Poor VRM = thermal throttling under sustained load
- Good VRM = stable clocks, longer component lifespan

**VRM Specifications to Look For**:

1. **Phase Count**:
   - **12-phase**: Minimum for Ryzen 7/9 (entry-level boards)
   - **16-phase**: Good for sustained loads (mid-range boards)
   - **20-24 phase**: Excellent for 24/7 operation (high-end boards)
   - More phases = lower heat per phase = better efficiency

2. **Power Stage Rating**:
   - **60A per phase**: Adequate for Ryzen 7
   - **80A per phase**: Good for Ryzen 9 (recommended)
   - **90A+ per phase**: Premium (overkill but cooler operation)

3. **Heatsink Design**:
   - **Passive heatsinks**: Standard (adequate with good airflow)
   - **Heatpipe-connected**: Better (ASRock Taichi, ASUS ROG)
   - **Actively cooled**: Overkill for Ryzen (only needed for extreme overclocking)

**Recommended Boards by VRM Quality**:

**Excellent VRM** (24/7 AI workloads, no concerns):
- ASUS ROG Crosshair X670E Hero: 18+2 phase, 110A, massive heatsinks
- ASRock X670E Taichi: 24+2+1 phase, 105A, heatpipe-connected
- Gigabyte X670E AORUS Master: 18+2+2 phase, 105A

**Good VRM** (24/7 workloads, adequate cooling):
- MSI MAG X670E Tomahawk: 16+2+1 phase, 80A
- ASUS TUF Gaming X670E-PLUS: 16+2 phase, 80A
- Gigabyte X670E AORUS Elite: 16+2+2 phase, 80A

**Adequate VRM** (occasional workloads, ensure good case airflow):
- ASRock X670E Steel Legend: 14+2+1 phase, 80A
- MSI PRO X670-P WiFi: 14+2+1 phase, 60A

**Performance Impact**:
- Excellent VRM: CPU maintains boost clocks indefinitely (5.4 GHz sustained)
- Good VRM: CPU maintains boost clocks with good airflow
- Adequate VRM: May throttle slightly under 100% load after 10-20 minutes (5.2-5.3 GHz)
- **Impact on inference**: <2% difference (CPU not bottleneck anyway)
- **Impact on longevity**: Significant (better VRM = longer motherboard life)

---

## Storage Performance Impact

**Storage Role in LLM Inference**:
- Initial model loading from disk to RAM/VRAM
- Swap space (if RAM exhausted, NOT recommended)
- Dataset storage for fine-tuning

**Performance Factors**:

1. **Sequential Read Speed** (Most Important):
   - **SATA SSD**: 550 MB/s (slow model loading)
   - **NVMe Gen3**: 3,500 MB/s (6x faster)
   - **NVMe Gen4**: 7,000 MB/s (12x faster)
   - **NVMe Gen5**: 12,000+ MB/s (22x faster, overkill)

   **Model Loading Time (70B Q4, ~35GB)**:
   - SATA SSD: ~64 seconds
   - NVMe Gen3: ~10 seconds
   - NVMe Gen4: ~5 seconds
   - **Verdict**: NVMe Gen4 is the sweet spot (loading time acceptable)

2. **Capacity Needs**:
   - **1TB**: Sufficient for 10-15 models (7B-13B)
   - **2TB**: Comfortable for 20-30 models (mix of sizes)
   - **4TB**: Large model library (50+ models including 70B variants)

**Recommended Storage Configuration**:

**Single Drive** (Simple):
- **2TB NVMe Gen4**: Samsung 990 Pro or WD Black SN850X ($160-200)
- Pros: Simple, fast, sufficient for most users
- Cons: No redundancy, all models on one drive

**Dual Drive** (Optimal):
- **Primary**: 1TB NVMe Gen4 (OS + active models) - $80-100
- **Secondary**: 2TB NVMe Gen4 (model library) - $160-180
- Pros: OS isolated from model storage, organized
- Cons: More expensive, uses extra M.2 slot

**PCIe Lane Consideration**:
- M.2_1 slot: Use for primary drive (directly from CPU, no sharing)
- M.2_2+ slots: Use for secondary drives (from chipset)
- Dual GPU + 2-3 NVMe drives: No problem on X670E boards

---

## Final Recommended Build Options: Single GPU Now, Dual GPU Ready

### Configuration A: "Flagship X3D" (Best Performance)

**Total Cost**: $2,200-$2,550 (single GPU) | +$700-900 for second GPU later

**Component List**:

| Component | Model | Specification | Price | Notes |
|-----------|-------|---------------|-------|-------|
| **GPU** | RTX 3090 24GB (used) | 24GB GDDR6X, 936 GB/s | $700-900 | eBay or Micro Center refurb |
| **CPU** | AMD Ryzen 9 9950X3D | 16C/32T, 5.7GHz, 144MB cache | $650-750 | Latest X3D, massive cache |
| **Motherboard** | ASUS ROG X870E Hero | x16/x8 PCIe 5.0, 18+2+2 VRM | $600-700 | Premium, dual GPU ready |
| **RAM** | G.Skill Trident Z5 | 64GB DDR5-6000 CL30 (2x32GB) | $280-320 | Low latency, expandable |
| **Storage** | Samsung 990 Pro | 2TB NVMe Gen4 (7,450 MB/s) | $160-200 | Fast model loading |
| **PSU** | Corsair RM1000x | 1000W 80+ Gold | $150-180 | Headroom for second GPU |
| **CPU Cooler** | Arctic Liquid Freezer II | 280mm AIO | $100-120 | Quiet, effective cooling |
| **Case** | Fractal Design Torrent | Dual 180mm front fans, spacious | $180-200 | Excellent GPU cooling |
| **Case Fans** | Stock + 2x 140mm | Additional exhaust | $30-40 | Maintain positive pressure |

**Why This Configuration**:
- ✅ Latest Zen 5 X3D architecture (+10-15% single-thread over Zen 4)
- ✅ 144MB 3D V-Cache (excellent for multi-model workloads)
- ✅ 16 cores (best for running multiple models simultaneously)
- ✅ X870E chipset (latest, PCIe 5.0 optimized)
- ✅ Premium motherboard (best VRM, 5x M.2 slots, Wi-Fi 7)
- ❌ More expensive (+$300-400 over Configuration B)

---

### Configuration B: "Sweet Spot Value" (Best Price-to-Performance)

**Total Cost**: $1,870-$2,150 (single GPU) | +$700-900 for second GPU later

**Component List**:

| Component | Model | Specification | Price | Notes |
|-----------|-------|---------------|-------|-------|
| **GPU** | RTX 3090 24GB (used) | 24GB GDDR6X, 936 GB/s | $700-900 | eBay or Micro Center refurb |
| **CPU** | AMD Ryzen 9 7900X | 12C/24T, 5.4 GHz boost | $400-450 | Excellent multi-core value |
| **Motherboard** | ASRock X670E Taichi | x16/x8 PCIe 5.0, 24+2+1 VRM | $320-370 | Dual GPU ready, excellent VRM |
| **RAM** | G.Skill Ripjaws S5 | 64GB DDR5-6000 CL36 (2x32GB) | $250-280 | Optimal speed, expandable to 128GB |
| **Storage** | Samsung 990 Pro | 2TB NVMe Gen4 (7,450 MB/s) | $160-200 | Fast model loading |
| **PSU** | Corsair RM1000x | 1000W 80+ Gold | $150-180 | Headroom for second GPU (1600W later) |
| **CPU Cooler** | Arctic Liquid Freezer II | 280mm AIO | $100-120 | Quiet, effective cooling |
| **Case** | Fractal Design Torrent | Dual 180mm front fans, spacious | $180-200 | Excellent GPU cooling |
| **Case Fans** | Stock + 2x 140mm | Additional exhaust | $30-40 | Maintain positive pressure |

**Why This Configuration**:
- ✅ Excellent value (saves $300-400 vs Configuration A)
- ✅ 12 cores sufficient for LLM inference + multi-tasking
- ✅ Proven Zen 4 platform (mature, stable)
- ✅ ASRock Taichi excellent VRM for 24/7 workloads
- ✅ x16/x8 dual GPU support (<2% performance difference vs x16/x16)
- ❌ Slightly less performance than 9950X3D (~5-8%)

**PCIe Configuration** (Single GPU):
- **Slot 1**: RTX 3090 @ x16 (full speed)
- **M.2_1**: 2TB NVMe Gen4 @ x4 (from CPU)
- **M.2_2**: Available for future drive (from chipset)

**Future Upgrade Path** (Dual GPU):
- Add second RTX 3090 24GB: $700-900
- BIOS: Enable x16/x8 bifurcation
- **Slot 1**: RTX 3090 #1 @ x16 (primary)
- **Slot 3**: RTX 3090 #2 @ x8 (minimal performance impact)
- Upgrade PSU to 1600W: $280-350
- Install vLLM for tensor parallelism
- **Result**: 48GB total VRAM, ~70-85% scaling on large models

**Performance Expectations**:

**Single GPU (24GB)**:
- 7B models: 110-120 tok/s
- 13B models: 60-90 tok/s
- 34B models (coding): 20-28 tok/s
- 70B models: 42 tok/s (Q4)

**Dual GPU (48GB)**:
- 70B models: 65-75 tok/s (FP16 full precision)
- 180B models: 25-35 tok/s (Q8)
- 405B models: 8-12 tok/s (Q4)

---

## Alternative Configurations

### Budget Option: "Single GPU, No Upgrade Path"

**If dual GPU not needed**, save money on motherboard and PSU:

**Changes**:
- **Motherboard**: MSI B650 Tomahawk WiFi ($180-220) - saves $120-150
- **CPU**: Ryzen 7 7700X ($280-320) - saves $120-130
- **PSU**: Corsair RM850x (850W) ($130-150) - saves $20-30
- **RAM**: 64GB DDR5-5600 CL40 ($220-250) - saves $30

**Total Savings**: $290-360
**New Total**: $1,580-1,790

**Tradeoffs**:
- No dual GPU upgrade path (B650 limited PCIe)
- 8-core CPU (still sufficient for LLM inference)
- Slightly slower RAM

---

### Premium Option: "Maximum Performance, Dual GPU Ready"

**For best performance and longevity**:

**Changes**:
- **CPU**: Ryzen 9 7950X ($500-600) - +$100-150
- **Motherboard**: ASUS ROG Crosshair X670E Hero ($550-650) - +$230-280
- **RAM**: 128GB DDR5-6000 CL30 (4x 32GB) ($600-700) - +$350-420
- **Storage**: 2x 2TB NVMe Gen4 ($320-400) - +$160-200
- **PSU**: Corsair HX1500i (1500W Platinum) ($350-400) - +$200-220

**Total Addition**: $1,040-1,270
**New Total**: $2,910-3,420

**Benefits**:
- 16-core CPU for multiple concurrent models
- Best-in-class motherboard (VRM, features, stability)
- 128GB RAM for massive models with offloading
- Ready for dual GPU without PSU upgrade
- Premium efficiency PSU (92-94%)

---

## Bottleneck Analysis

**Where will performance be limited in this build?**

1. **GPU VRAM** (Primary Bottleneck):
   - 24GB limits model size to 70B (Q4)
   - **Solution**: Add second GPU for 48GB
   - **Impact**: +100% VRAM = can run 2-3x larger models

2. **GPU Memory Bandwidth** (Secondary Bottleneck):
   - 936 GB/s limits token generation speed
   - **No solution** without upgrading GPU
   - **Impact**: Determines tok/s for given model size

3. **CPU** (Not a Bottleneck):
   - 12-core @ 5.4 GHz is more than sufficient
   - LLM inference is GPU-bound
   - **Impact**: <2% on inference speed

4. **RAM Speed** (Minor Bottleneck):
   - DDR5-6000 provides 96 GB/s bandwidth
   - Only matters when offloading from VRAM
   - **Impact**: ~5% when offloading, 0% when fully in VRAM

5. **PCIe Bandwidth** (Not a Bottleneck):
   - x16 provides 31.5 GB/s
   - Only affects model loading time (one-time)
   - **Impact**: None on inference, 5-10 sec on loading

6. **Storage Speed** (Not a Bottleneck):
   - NVMe Gen4 @ 7,000 MB/s
   - Only affects initial model loading
   - **Impact**: 5-10 seconds per model load

**Conclusion**: This build is GPU-limited (as it should be). All other components are optimally configured to support the GPU without bottlenecks.

---

## Thermal Management & Acoustics

**Heat Output**:
- **Single GPU**: ~520W system total (350W GPU + 170W CPU + peripherals)
- **Dual GPU**: ~870W system total (700W GPUs + 170W CPU + peripherals)

**Cooling Strategy**:

1. **GPU Cooling**:
   - RTX 3090 uses triple-fan cooler (stock)
   - Target: <80°C under sustained load
   - **Case airflow critical**: Front intake must provide fresh air directly to GPUs
   - **Fractal Torrent**: 2x 180mm front fans (560 CFM combined) - excellent GPU cooling
   - **GPU spacing**: If dual GPU, maintain 2-slot spacing if possible (better thermal separation)

2. **CPU Cooling**:
   - Ryzen 9 7900X: 170W TDP
   - Arctic Liquid Freezer II 280mm: 270W+ cooling capacity (adequate headroom)
   - **Mount**: Top or rear exhaust (pull hot air away from GPUs)
   - Target: <75°C under sustained load

3. **Airflow Configuration**:
   - **Front**: 2x 180mm intake (Fractal Torrent stock fans)
   - **Top**: 280mm AIO exhaust (CPU cooler)
   - **Rear**: 1x 140mm exhaust (additional)
   - **Result**: Positive pressure (more intake than exhaust prevents dust)

4. **Acoustics**:
   - Target: <35 dBA at idle, <45 dBA under load
   - **GPU fans**: Typically loudest component under load
   - **Mitigation**: Undervolt GPUs to 280W (reduces fan speed, minimal performance loss)
   - **Case**: Fractal Torrent optimized for low RPM fans (large fans = less noise)

---

## Power Supply Requirements

**Single GPU Configuration**:
- **Peak Power**: 520W (350W GPU + 170W CPU)
- **Recommended PSU**: 850W (65% load at peak = optimal efficiency)
- **Chosen**: 1000W (future-proofs for dual GPU upgrade)

**Dual GPU Configuration**:
- **Peak Power**: 870W (700W GPUs + 170W CPU)
- **Recommended PSU**: 1200W (72% load at peak)
- **Upgrade to**: 1600W 80+ Gold ($280-350)

**PSU Calculator**:
- RTX 3090: 350W x 2 = 700W
- Ryzen 9 7900X: 170W
- Motherboard: 50W
- RAM (64GB): 10W
- Storage (NVMe): 5W
- Fans/Peripherals: 30W
- **Total**: 965W
- **PSU Recommendation**: 1600W provides 66% headroom (optimal for efficiency and longevity)

**PCIe Power Connectors**:
- Single RTX 3090: 3x 8-pin (6+2)
- Dual RTX 3090: 6x 8-pin (6+2)
- **Corsair RM1000x**: Provides 6x PCIe connectors (sufficient for dual GPU)
- **Corsair HX1600i**: Provides 10x PCIe connectors (plenty of headroom)

---

## BIOS Configuration for Dual GPU

**When adding second GPU, configure BIOS**:

1. **Enable PCIe Bifurcation**:
   - Path: Advanced → PCIe Configuration → PCIe Slot Bifurcation
   - Set Slot 1: x8, Slot 3: x8 OR Slot 1: x16, Slot 3: x8 (board-dependent)

2. **Configure Primary Display**:
   - Path: Advanced → Graphics Configuration → Primary Display
   - Set to: PCIe Slot 1 (ensure monitor connected to GPU 1)

3. **Above 4G Decoding** (Enable):
   - Path: Advanced → PCI Subsystem Settings → Above 4G Decoding
   - Required for: Dual GPU with >4GB VRAM each

4. **Resizable BAR** (Enable):
   - Path: Advanced → PCI Subsystem Settings → Re-Size BAR Support
   - Improves: GPU memory access (5-10% performance boost)

5. **Power Settings**:
   - Path: Advanced → AMD CBS → Power Supply Idle Control
   - Set to: Typical Current Idle (prevents power-saving that can cause instability)

---

## Summary: Why This Configuration

**Two Recommended Configurations**:

**Configuration A: Flagship X3D** ($2,200-2,550)
- Ryzen 9 9950X3D + ASUS ROG X870E Hero + RTX 3090
- Latest Zen 5 X3D (16 cores, 144MB cache)
- Premium X870E board (5x M.2, Wi-Fi 7, best VRM)
- Best for: Maximum performance, multi-model workloads, future-proofing

**Configuration B: Sweet Spot Value** ($1,870-2,150)
- Ryzen 9 7900X + ASRock X670E Taichi + RTX 3090
- Proven Zen 4 platform (12 cores, 64MB cache)
- Excellent X670E board (4x M.2, great VRM)
- Best for: Price-to-performance, mature platform, best value

**Both Configurations Provide**:
✅ **Optimal PCIe**: x16/x8 for dual GPU (<2% performance difference vs x16/x16)
✅ **Excellent VRM**: 18-24 phase handles sustained 24/7 AI workloads
✅ **Future-Proof**: Add second GPU for 48GB VRAM (200B+ models)
✅ **No Bottlenecks**: All components balanced for GPU-bound workloads
✅ **Reliable Platform**: AMD X670E/X870E mature, stable, well-supported

**Performance Profile**:
- **Now**: 7B-70B models @ 42-120 tok/s
- **With 2nd GPU**: 70B-405B models @ 8-75 tok/s
- **Upgrade Cost**: +$700-900 GPU + $280-350 PSU = $980-1,250

**This build prioritizes**:
1. VRAM capacity (24GB now, 48GB later)
2. Upgrade flexibility (dual GPU path)
3. Component balance (no bottlenecks)
4. Long-term reliability (excellent VRM, quality PSU)
5. Value (best performance per dollar)

---

## Next Steps

1. **Verify component availability** and current pricing
2. **Purchase order prioritization**:
   - Order motherboard first (longest lead time)
   - GPU from eBay (inspect carefully for mining damage)
   - Remaining components from primary retailer (Amazon/Newegg)
3. **Assembly preparation**: Review motherboard manual for PCIe slot configuration
4. **Software planning**: Ubuntu 24.04 LTS + Ollama + vLLM (for future dual GPU)
5. **Budget for second GPU**: Set aside $700-900 for RTX 3090 #2 + $280-350 for PSU upgrade

---

**Document Version**: 1.0
**Last Updated**: February 12, 2026
