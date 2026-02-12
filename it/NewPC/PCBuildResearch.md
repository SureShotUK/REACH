# AI PC Build Research Report for Local LLM Inference
## Focus: Coding Assistance & Homework Help (2025-2026)

**Research Date**: February 12, 2026
**Purpose**: Building/buying a PC for local AI inference focused on coding models and LLM homework assistance
**Target Capabilities**: Similar to NetworkChuck's setup with Open WebUI/Ollama

---

## Executive Summary: Key Findings

**GPU VRAM is the most critical component** - more important than CPU, RAM speed, or storage. The entire model must load into VRAM for fast inference.

**Sweet Spot for 2025**: $1,500-$2,500 builds handle 90% of use cases and allow for GPU upgrades later.

**Best Value**: Used RTX 3090 24GB ($700-900) offers 80% of RTX 4090 performance at less than half the price.

**New Champion**: RTX 5090 32GB (January 2025, $1,999) delivers up to 67% improvement over RTX 4090 for LLM inference.

---

## Complete PC Build Examples

### BUILD 1: Budget Entry ($899-$1,200)
**Best For**: 7B-13B models, learning, experimentation

**Components**:
- **GPU**: RTX 4060 Ti 8GB ($399) or RTX 4060 Ti 16GB ($449-499)
- **CPU**: AMD Ryzen 5 7500F ($150-180) or Intel i5-12400F ($150)
- **RAM**: 32GB DDR5-5600 ($120-150)
- **Storage**: 1TB NVMe Gen4 SSD ($80-100)
- **Motherboard**: B650 or B760 ($120-150)
- **PSU**: 650W 80+ Gold ($80-100)
- **Case**: ATX Mid Tower ($60-80)

**Performance**:
- Models: Up to 13B comfortably at 4-bit quantization
- Speed: 15-35 tokens/second on 7B-13B models
- Can run: CodeLlama 13B, Mistral 7B, DeepSeek Coder 6.7B

**Pros**: Affordable entry point, warranty coverage, low power consumption (200W GPU)
**Cons**: Limited to smaller models, 16GB VRAM constrains context windows on larger models

---

### BUILD 2: Sweet Spot - Used RTX 3090 ($1,500-$1,700)
**Best For**: 70B models, best value per dollar, serious development

**Components**:
- **GPU**: RTX 3090 24GB used ($700-900 on eBay)
- **CPU**: AMD Ryzen 7 7700X ($280-320) or Intel i7-13700K ($350-400)
- **RAM**: 64GB DDR5-5600 ($220-280)
- **Storage**: 2TB NVMe Gen4 SSD ($140-180)
- **Motherboard**: X670 or Z790 ($180-250)
- **PSU**: 850W 80+ Gold ($120-150)
- **Case**: ATX Mid Tower ($80-120)

**Performance**:
- Models: Up to 70B at 4-bit quantization
- Speed: 42 tokens/second (Llama 70B Q4), 87 tokens/second (8B models)
- Can run: CodeLlama 34B, Mixtral 8x7B, Llama 3.1 70B

**Pros**: Best price-to-VRAM ratio ($24.96 per token/second), 24GB handles large models, proven reliability
**Cons**: No warranty (used), higher power consumption (350W), may need thermal paste replacement

**Value Analysis**: 36% cheaper than RTX 4070 Ti Super while providing 50% more VRAM (24GB vs 16GB)

---

### BUILD 3: New Mid-Range ($2,000-$2,500)
**Best For**: Current-gen warranty coverage, efficiency, 30B-70B models

**Components**:
- **GPU**: RTX 4070 Ti Super 16GB ($800-900) or RTX 4090 24GB ($1,599)
- **CPU**: AMD Ryzen 7 7800X3D ($380-420) or Intel i7-14700K ($400-450)
- **RAM**: 64GB DDR5-6000 ($240-300)
- **Storage**: 2TB NVMe Gen4 SSD ($140-180)
- **Motherboard**: X670E or Z790 ($220-280)
- **PSU**: 850W 80+ Platinum ($150-200)
- **Case**: ATX Mid Tower with good airflow ($100-150)

**Performance with RTX 4070 Ti Super**:
- Models: 13B-30B comfortably, 70B with reduced context
- Speed: 15-25 tokens/second (varies by model size)

**Performance with RTX 4090**:
- Models: Up to 70B at full context
- Speed: 52 tokens/second (Llama 70B Q4), 128 tokens/second (8B models)

**Pros**: Warranty coverage, excellent efficiency, modern features (DLSS 3, PCIe 4.0)
**Cons**: RTX 4070 Ti Super limited by 16GB VRAM, RTX 4090 approaches $2,500 total

---

### BUILD 4: High-End Enthusiast ($3,500-$4,500)
**Best For**: Multiple concurrent models, up to 180B models, professional development, fastest single-GPU performance

**Components**:
- **GPU**: RTX 5090 32GB ($1,999) or RTX 4090 24GB ($1,599)
- **CPU**: AMD Ryzen 9 7950X3D ($550-650) or Intel i9-14900K ($550-600)
- **RAM**: 128GB DDR5-6000 ($480-600)
- **Storage**: 4TB NVMe Gen4 SSD (2x 2TB in RAID 0) ($280-360)
- **Motherboard**: X670E Taichi or Z790 Apex ($350-450)
- **PSU**: 1200W 80+ Platinum ($250-350)
- **Case**: Full Tower with premium airflow ($150-250)

**Performance with RTX 5090**:
- Models: Up to 120B-180B with Q4 quantization (32GB VRAM limit)
- Speed: 213 tokens/second (8B models), 87 tokens/second (70B Q4), 61 tokens/second (32B models)
- Prompt Processing: 10,400+ tokens/second on prefill
- Context Windows: Up to 139K tokens supported
- **Note**: For 200B+ models, dual GPU setup required (see BUILD 5 or Dual RTX 3090 section)

**Performance with RTX 4090**:
- Models: Up to 70B-120B
- Speed: 128 tokens/second (8B), 52 tokens/second (70B)

**Pros**: Cutting-edge performance, runs largest models, multiple concurrent instances, extensive context windows
**Cons**: Expensive, high power consumption (450W+ GPU), overkill for most users

---

### BUILD 5: Dual GPU Workstation ($3,000-$5,000)
**Best For**: Running 200B+ models, production deployments, multiple simultaneous models

**Components**:
- **GPU**: 2x RTX 3090 24GB used ($1,400-1,800)
- **CPU**: AMD Threadripper PRO 5955WX ($1,000-1,200) or Ryzen 9 7900X ($400-450)
- **RAM**: 128GB DDR4-3200 ECC ($400-500) or 128GB DDR5 ($480-600)
- **Storage**: 4TB NVMe Gen4 ($280-360)
- **Motherboard**: WRX80 or X670E with dual x16 PCIe ($350-800)
- **PSU**: 1600W 80+ Platinum ($300-400)
- **Case**: Full Tower (Lian Li O11 Dynamic EVO XL) ($180-250)

**Performance**:
- Combined VRAM: 48GB (enables massive models)
- Models: DeepSeek R1 671B with quantization, Llama 405B
- Speed: Scales with multi-GPU support (depends on software optimization)

**Real Build Example**: LlamaBox build on PCPartPicker with AMD Ryzen 9 7900X + 2x RTX 3090 + RTX 3090 Ti

**Pros**: Massive VRAM capacity, can run largest open-source models, future-proof
**Cons**: Complex setup, not all software supports multi-GPU inference, very high power draw (700W+ GPUs alone)

---

## Detailed Dual RTX 3090 Build Specifications

This section provides comprehensive specifications for dual RTX 3090 setups, offering 48GB total VRAM for running the largest open-source models.

### Configuration A: Budget-Conscious Dual 3090 ($2,800-$3,200)
**Best For**: Maximum VRAM on a budget, running 200B+ models with quantization

**Complete Component List**:

**Graphics Cards**:
- **GPU**: 2x NVIDIA RTX 3090 24GB (used/refurbished) - $1,400-1,800 total
  - eBay used market: $700-900 each
  - Micro Center refurbished: $800-1,000 each (with limited warranty)
  - Check for: Clean fans, no mining damage, thermal paste condition
  - Combined VRAM: 48GB

**Processor**:
- **CPU**: AMD Ryzen 9 7900X (12-core, 24-thread) - $400-450
  - Base: 4.7 GHz, Boost: 5.4 GHz
  - TDP: 170W
  - PCIe 5.0 support with sufficient lanes for dual GPU
  - Alternative: Intel i9-13900K ($550-600) for slightly better multi-threaded performance

**Memory**:
- **RAM**: 128GB DDR5-5600 (4x 32GB kit) - $480-550
  - Recommended: Corsair Vengeance DDR5-5600 CL40
  - Alternative: G.Skill Ripjaws S5 DDR5-6000 CL36 ($580-650) for better bandwidth
  - Minimum recommended: 64GB (but 128GB preferred for very large models)

**Storage**:
- **Primary**: 2TB NVMe Gen4 SSD - $160-200
  - Recommended: Samsung 990 Pro or WD Black SN850X
  - For models and active projects
- **Secondary** (Optional): 4TB SATA SSD - $200-250
  - For model storage and datasets

**Motherboard**:
- **Board**: AMD X670E chipset - $280-350
  - Required: Dual PCIe x16 slots (x16/x8 or x16/x16 mode)
  - Recommended models:
    - ASUS TUF Gaming X670E-PLUS WIFI ($280-320)
    - MSI MAG X670E Tomahawk WiFi ($300-350)
    - Gigabyte X670E AORUS Elite AX ($320-370)
  - Ensure: Good VRM cooling for sustained workloads

**Power Supply**:
- **PSU**: 1600W 80+ Gold - $280-350
  - Recommended: EVGA SuperNOVA 1600 G+ or Corsair HX1500i
  - Wattage calculation:
    - 2x RTX 3090: 700W (2x 350W)
    - Ryzen 9 7900X: 170W
    - Motherboard + RAM + Storage: 100W
    - Total: ~970W (1600W provides 65% headroom)
  - Must have: Sufficient PCIe 8-pin (6+2) connectors (need 6 total for dual 3090)

**Cooling**:
- **CPU Cooler**: High-end air or 280mm AIO - $100-150
  - Air: Noctua NH-D15 or be quiet! Dark Rock Pro 4
  - AIO: Arctic Liquid Freezer II 280 or NZXT Kraken X63
- **Case Fans**: Additional 3-4 high-airflow fans - $60-100
  - Front intake: 3x 120mm or 2x 140mm
  - Rear/top exhaust: 1-2x 120mm

**Case**:
- **Chassis**: Full Tower with excellent airflow - $150-220
  - Recommended:
    - Lian Li O11 Dynamic EVO XL ($180-220) - excellent GPU cooling
    - Fractal Design Torrent ($180-200) - maximum airflow
    - Corsair 5000D Airflow ($150-180) - spacious and well-ventilated
  - Requirements: Supports dual 300mm+ GPUs, good cable management, 8+ fan mounts

**Peripherals**:
- **Thermal pads** (for GPU memory cooling enhancement): $20-30
- **PCIe riser/spacing** (if needed for tight spacing): $30-50
- **Cable extensions** (for better cable management with dual GPUs): $40-60

**Total Cost: $2,940-$3,470**

---

### Configuration B: Performance-Focused Dual 3090 ($3,800-$4,500)
**Best For**: Maximum performance with dual GPUs, professional AI development, production use

**Key Differences from Configuration A**:

**Processor**:
- **CPU**: AMD Ryzen 9 7950X3D (16-core, 32-thread) - $550-650
  - Base: 4.2 GHz, Boost: 5.7 GHz
  - 3D V-Cache: 128MB (excellent for data-intensive workloads)
  - Better multi-tasking while models run
  - Alternative: AMD Threadripper PRO 5955WX ($1,000-1,200) for maximum PCIe lanes and ECC RAM support

**Memory**:
- **RAM**: 128GB DDR5-6000 CL30 - $600-700
  - Recommended: G.Skill Trident Z5 RGB DDR5-6000 CL30
  - ~15-20% better memory bandwidth for faster model loading
- Alternative for Threadripper: 128GB DDR4-3200 ECC ($450-550) for error correction

**Motherboard**:
- **Board**: Premium X670E or WRX80 - $350-800
  - For Ryzen: ASUS ROG Crosshair X670E Hero ($600-700) or ASRock X670E Taichi ($350-450)
  - For Threadripper: ASUS Pro WS WRX80E-SAGE SE WiFi ($700-800)
  - Features: Better VRM, 10Gb Ethernet, WiFi 6E, more M.2 slots

**Storage**:
- **Primary**: 2TB NVMe Gen4 SSD (high-end) - $220-280
  - Samsung 990 Pro with heatsink or Seagate FireCuda 530
- **Secondary**: 2x 2TB NVMe Gen4 in RAID 0 - $320-400
  - Total 4TB at higher speeds for rapid model swapping

**Power Supply**:
- **PSU**: 1600W 80+ Platinum or Titanium - $350-450
  - Corsair AX1600i (80+ Titanium) - $450
  - EVGA SuperNOVA 1600 P2 (80+ Platinum) - $350-400
  - Higher efficiency reduces electricity costs and heat

**Cooling**:
- **CPU Cooler**: Premium 360mm AIO - $150-200
  - Arctic Liquid Freezer II 360 or Corsair iCUE H150i Elite
  - Better sustained performance under heavy loads
- **Case Fans**: Premium 6-8 fans with PWM control - $100-180
  - Noctua NF-A12x25 PWM (6-pack) or be quiet! Silent Wings Pro 4

**Case**:
- **Chassis**: Premium full tower - $200-300
  - Lian Li O11 Dynamic EVO XL ($220)
  - Phanteks Enthoo Elite ($280-300)
  - Corsair Obsidian 1000D ($500) - if budget allows

**Total Cost: $4,020-$4,730**

---

### Dual RTX 3090 Performance Expectations

**VRAM Capacity**: 48GB total (enables models impossible on single GPU)

**Model Size Capabilities**:
- **70B models**: Run at full FP16 precision (no quantization needed)
- **180B models**: Run at Q8 quantization with good quality
- **405B models** (Llama 3.1 405B): Run at Q4 quantization
- **671B models** (DeepSeek R1): Possible with aggressive Q4/Q3 quantization

**Performance Metrics** (estimated with proper multi-GPU support):
- **8B models**: 180-220 tokens/second (combined throughput)
- **70B models**: 65-80 tokens/second (near-linear scaling)
- **180B models**: 25-35 tokens/second (Q8)
- **405B models**: 8-12 tokens/second (Q4)

**Coding Models**:
- **CodeLlama 70B**: 60-75 tokens/second (full precision)
- **DeepSeek Coder V2 236B**: 15-20 tokens/second (Q4)
- Multiple smaller models simultaneously (e.g., 34B coder on GPU1 + 13B general on GPU2)

---

### Multi-GPU Setup Requirements

**Software Stack**:
1. **Operating System**: Ubuntu 22.04/24.04 LTS (best multi-GPU support)
   - Windows 11 also works but Linux preferred for AI workloads

2. **GPU Drivers**: NVIDIA CUDA Toolkit 12.x
   - Install: `sudo apt install nvidia-driver-550 nvidia-cuda-toolkit`

3. **Inference Engines with Multi-GPU Support**:
   - **vLLM**: Best multi-GPU performance (tensor parallelism)
     - Install: `pip install vllm`
     - Supports automatic model sharding across GPUs
   - **Text Generation WebUI**: User-friendly with multi-GPU
     - Supports ExLlama/ExLlamaV2 backend with GPU splitting
   - **Ollama**: Limited multi-GPU (uses both but not optimally)
     - Single model spans one GPU typically
   - **llama.cpp**: Supports multi-GPU with `--tensor-split` flag

4. **Frontend**:
   - Open WebUI (works with vLLM and Ollama backends)
   - Text Generation WebUI (built-in interface)
   - LM Studio (desktop app, limited multi-GPU)

**Configuration Example** (vLLM with tensor parallelism):
```bash
# Install vLLM
pip install vllm

# Run 70B model across both GPUs
python -m vllm.entrypoints.openai.api_server \
  --model meta-llama/Llama-2-70b-chat-hf \
  --tensor-parallel-size 2 \
  --gpu-memory-utilization 0.95

# Access via OpenAI-compatible API at localhost:8000
```

**PCIe Configuration**:
- Optimal: x16/x16 mode (requires CPU with sufficient lanes)
- Acceptable: x16/x8 mode (minimal performance impact for inference)
- Check BIOS settings to enable bifurcation if needed

**Thermal Considerations**:
- Dual 3090s generate significant heat (700W combined)
- Maintain 2-3 slot spacing between cards if possible
- Ensure case has strong front-to-back airflow
- Consider undervolting GPUs to reduce heat and power (minimal performance loss)
- Target GPU temps: Under 80°C under sustained load

---

### Power Consumption & Operating Costs

**Peak Power Draw**:
- System at full load: 970-1,100W
- Idle: 150-200W
- Typical AI inference: 600-800W

**Electricity Costs** (assuming $0.13/kWh US average):
- 8 hours/day heavy use: ~$30-40/month
- 24/7 operation: ~$90-120/month
- Compared to cloud inference: Break-even at 100-200 hours of inference time

**Power Efficiency Tips**:
- Use 80+ Platinum or Titanium PSU (92-94% efficiency)
- Enable GPU power limiting in software (280W per card vs 350W, ~15% performance loss but 40% power savings)
- Utilize auto-sleep/wake for GPUs when not in use

---

### When to Choose Dual RTX 3090 Over Single RTX 5090

**Choose Dual RTX 3090 ($2,800-3,200) if**:
- You need 48GB VRAM (vs 32GB on RTX 5090)
- Running 200B+ models regularly
- Want best VRAM per dollar ($60/GB vs $62/GB)
- Comfortable with Linux and multi-GPU setup
- Can tolerate higher power consumption

**Choose Single RTX 5090 ($3,500-4,000 total build) if**:
- Want simpler setup (single GPU)
- Need maximum single-GPU performance (67% faster)
- 32GB VRAM is sufficient (covers most use cases)
- Want lower power consumption (450W vs 700W)
- Prefer Windows compatibility
- Value warranty and support

**The Verdict**: Dual RTX 3090 offers more VRAM for less money but requires more technical setup and consumes more power. RTX 5090 is simpler, faster, and more efficient but costs more and has less VRAM.

---

## GPU Performance Comparison (Tokens per Second)

### 8B Models (Mistral 7B, Llama 3.1 8B)
| GPU | Tokens/Sec | Price | $/Token/Sec | VRAM |
|-----|------------|-------|-------------|------|
| RTX 5090 | 213 | $1,999 | $9.39 | 32GB |
| RTX 4090 | 128 | $1,599 | $12.49 | 24GB |
| RTX 3090 | 112 | $800 (used) | $7.14 | 24GB |
| RTX 4060 Ti 16GB | 89 | $499 | $5.61 | 16GB |
| AMD 7900 XTX | ~90 | $900 | $10.00 | 24GB |

### 70B Models (Llama 3.1 70B Q4)
| GPU | Tokens/Sec | Context | Notes |
|-----|------------|---------|-------|
| RTX 5090 | 87 | Full | 67% faster than 4090, cutting-edge |
| RTX 4090 | 52 | Full | Excellent performance |
| RTX 3090 | 42 | Full | 80% of 4090 performance |
| RTX 4070 Ti Super | ~20 | Reduced | Limited by 16GB VRAM |
| RTX 4060 Ti 16GB | Not viable | - | Insufficient VRAM |

### Coding Models Specific Performance
- **CodeLlama 13B**: RTX 3090/4090 @ 60-90 tokens/sec (Q4)
- **DeepSeek Coder 33B**: RTX 4090 @ 25-35 tokens/sec (Q4), RTX 3090 @ 20-28 tokens/sec
- **Qwen 2.5 Coder 32B**: RTX 4090 @ 20+ tokens/sec

---

## VRAM Requirements by Model Size

| Model Size | FP16 VRAM | Q8 VRAM | Q4 VRAM | Recommended GPU |
|------------|-----------|---------|---------|-----------------|
| 7B | ~14GB | ~7GB | ~4GB | RTX 4060 Ti 16GB+ |
| 13B | ~26GB | ~13GB | ~7GB | RTX 4060 Ti 16GB+ |
| 30B | ~60GB | ~30GB | ~16GB | RTX 3090 24GB+ |
| 70B | ~140GB | ~70GB | ~35GB | Dual RTX 3090 or quantized on 24GB |
| 200B+ | ~400GB+ | ~200GB+ | ~100GB+ | Multi-GPU required |

**Terminology**:
- **FP16**: 16-bit floating point precision (full quality, largest memory footprint)
- **Q8**: 8-bit quantization (minimal quality loss, 50% memory reduction)
- **Q4**: 4-bit quantization (some quality loss but massive memory savings - 75% reduction)
- **Quantization**: Technique to reduce model size by using lower precision numbers while maintaining most of the model's capabilities

---

## AMD vs NVIDIA: The ROCm Reality Check

### AMD 7900 XTX (24GB) - $900
**Performance**: 80% of RTX 4090 speed on short contexts, drops to 50% on 4K+ token contexts

**Pros**:
- 24GB VRAM for $900 (excellent value)
- ROCm support improving rapidly in 2025
- Strong Linux support

**Cons**:
- Requires Linux for best compatibility
- More technical setup (command-line work)
- Some cutting-edge models lack immediate support
- Performance degrades significantly with long context windows

**Terminology**:
- **ROCm**: AMD's open-source platform for GPU computing (similar to NVIDIA's CUDA)
- **Context window**: The amount of text the model can "remember" during a conversation

**Verdict**: Viable option for Linux users comfortable with technical setup, but NVIDIA remains more mature for AI workloads in 2025.

---

## CPU Considerations

**Key Insight**: LLM inference is primarily GPU-bound. CPU matters far less than GPU VRAM.

### Recommended CPUs by Budget

**Budget ($150-200)**:
- AMD Ryzen 5 5600X / 7500F
- Intel i5-12400F / i5-13400F
- 6-8 cores sufficient

**Mid-Range ($280-450)**:
- AMD Ryzen 7 7700X / 7800X3D (excellent for gaming + AI)
- Intel i7-13700K / i7-14700K
- 8-12 cores with high boost clocks

**High-End ($500-650)**:
- AMD Ryzen 9 7900X / 7950X3D
- Intel i9-14900K
- 12-16 cores for multi-tasking

**CPU Performance Comparison**: Ryzen 9 7900X outperforms i7-13700K by ~12% in general compute, but differences in LLM inference are minimal when GPU-accelerated.

**Terminology**:
- **GPU-bound**: Performance is limited by the GPU, not the CPU
- **Boost clock**: Maximum speed the CPU can reach under heavy load

---

## RAM: How Much Do You Really Need?

### The Rule of Thumb
**Model size + 4-8GB for OS** when offloading to system RAM is necessary.

### Practical Recommendations

**32GB DDR5** ($120-180):
- Sweet spot for most users
- Handles 7B-13B models comfortably
- Sufficient for GPU-accelerated inference with 16-24GB VRAM GPUs

**64GB DDR5** ($240-300):
- Recommended for 70B models
- Enables larger context windows
- Better for running multiple applications alongside AI

**128GB DDR5** ($480-600):
- Professional/enthusiast tier
- Required for CPU-based inference on large models
- Enables extreme multitasking

### Memory Speed Matters More Than You Think
- **DDR5-5600** (baseline): Adequate for most workloads
- **DDR5-6000** (optimal): Measurable improvement in inference speed
- **Memory bandwidth** is the primary bottleneck for LLM inference, not compute power

**Terminology**:
- **Memory bandwidth**: Speed at which data can be transferred to/from RAM (measured in GB/s)
- **Offloading**: When a model is too large for GPU VRAM, parts of it are moved to system RAM (much slower)

**Recommendation**: Prioritize faster RAM (6000MHz+) over capacity if budget-constrained. 32GB @ 6000MHz > 64GB @ 4800MHz for inference performance.

---

## Software Stack: Ollama, LM Studio, or Open WebUI?

### Ollama (Command-Line LLM Runner)
**Best For**: Developers, automation, API integration

**Pros**:
- Single-command installation
- Excellent token/sec throughput
- Built on llama.cpp (highly optimized)
- REST API for integration
- Supports NVIDIA (CUDA), Apple Silicon (Metal), AMD (ROCm)

**Cons**:
- Command-line interface (no GUI)
- Less beginner-friendly

**Terminology**:
- **REST API**: A way for programs to communicate with Ollama over the network
- **llama.cpp**: Highly optimized C++ library for running LLMs efficiently

**Use Case**: Production deployments, scripting, backend services

---

### LM Studio (Desktop GUI App)
**Best For**: Non-technical users, rapid prototyping, visual interface

**Pros**:
- Beautiful, intuitive GUI
- 1000+ pre-configured models (2025)
- Built-in chat interface
- OpenAI-compatible local server
- Excellent for integrated GPUs
- Download models with one click

**Cons**:
- Desktop-only (not web-based)
- Less automation-friendly than Ollama

**Terminology**:
- **GUI**: Graphical User Interface (point-and-click interface instead of command-line)
- **OpenAI-compatible**: Can be used as a drop-in replacement for OpenAI's API

**Use Case**: Beginners, testing models, local ChatGPT-like experience

---

### Open WebUI (Web Interface for Ollama/APIs)
**Best For**: Teams, self-hosted ChatGPT alternative, extensibility

**Pros**:
- ChatGPT-like web experience
- Works with Ollama, vLLM, OpenAI-compatible APIs
- Highly extensible (plugins, custom Python code)
- Multi-user support
- RAG (Retrieval-Augmented Generation) support
- Self-hosted and private

**Cons**:
- Requires Ollama or another backend
- More complex setup than LM Studio

**Terminology**:
- **RAG (Retrieval-Augmented Generation)**: Technique where the AI can search through your documents to answer questions
- **Self-hosted**: Runs on your own computer/server, not in the cloud

**Use Case**: Self-hosted AI for teams, RAG applications, NetworkChuck-style setups

---

### The NetworkChuck Stack
Based on community discussions and video references:
- **Backend**: Ollama (for model serving)
- **Frontend**: Open WebUI (for ChatGPT-like interface)
- **Platform**: Ubuntu 24.04 LTS
- **Models**: Llama 3, Mistral, DeepSeek (self-hosted)

---

## Price Breakdowns by Tier

### Budget Tier: $899-$1,200
- **Core**: RTX 4060 Ti 8GB/16GB + Ryzen 5 + 32GB RAM
- **Performance**: 7B-13B models @ 15-35 tok/s
- **Best For**: Learning, light development, homework help

### Sweet Spot: $1,500-$1,700
- **Core**: RTX 3090 24GB (used) + Ryzen 7 + 64GB RAM
- **Performance**: 70B models @ 42 tok/s
- **Best For**: Serious coding assistance, 70B models, best value

### Mid-Range New: $2,000-$2,500
- **Core**: RTX 4090 24GB + Ryzen 7/9 + 64GB RAM
- **Performance**: 70B models @ 52 tok/s, 8B @ 128 tok/s
- **Best For**: Professional work, warranty coverage, efficiency

### High-End: $3,500-$4,500
- **Core**: RTX 5090 32GB + Ryzen 9 + 128GB RAM
- **Performance**: Up to 120B-180B models with Q4, 213 tok/s (8B models), 61 tok/s (32B models), 87 tok/s (70B Q4)
- **Best For**: Cutting-edge performance, large models up to 180B, fastest single-GPU inference
- **Note**: 200B+ models require multi-GPU setup (dual RTX 5090 or dual RTX 3090)

### Dual GPU: $3,000-$5,000
- **Core**: 2x RTX 3090 24GB (48GB total) + Threadripper/Ryzen 9
- **Performance**: 200B-400B models with quantization
- **Best For**: Largest models, production workloads

---

## Pros & Cons Summary by Build Type

### Used RTX 3090 Build (BEST VALUE)
**Pros**:
- Unbeatable price-to-VRAM ratio ($700-900 for 24GB)
- Runs 70B models comfortably
- 80% of RTX 4090 performance
- Widely available on used market

**Cons**:
- No warranty (used hardware risk)
- Higher power consumption (350W)
- May need maintenance (thermal paste, cleaning)
- Older architecture (2020)

---

### New RTX 4060 Ti 16GB Build (BUDGET FRIENDLY)
**Pros**:
- Affordable entry ($899-1,200 total)
- Low power consumption (160W)
- Full warranty coverage
- Great for 7B-13B models

**Cons**:
- 16GB VRAM limits model size
- Cannot run 70B models effectively
- Limited future-proofing

---

### New RTX 4090 Build (PREMIUM)
**Pros**:
- Excellent performance (52-128 tok/s)
- 24GB VRAM handles 70B models
- Warranty coverage
- More efficient than 3090 (450W vs 350W, but much faster)

**Cons**:
- Expensive ($1,599 GPU alone)
- Total build approaches $2,500
- Approaching EOL with 5090 release

**Terminology**:
- **EOL (End of Life)**: Product nearing the end of its production/sales cycle

---

### RTX 5090 Build (CUTTING EDGE)
**Pros**:
- 32GB VRAM (best for consumer GPUs)
- 67% faster than 4090 for LLM inference
- 139K context window support
- Future-proof for 2025-2028+

**Cons**:
- Very expensive ($1,999 GPU, $3,500+ total)
- High power consumption (450W+)
- Overkill for most use cases

---

## Recommendations by Use Case

### For Coding Assistance (CodeLlama, DeepSeek Coder, Qwen Coder)
**Recommended**: RTX 3090 24GB (used) or RTX 4090 24GB
**Models**: CodeLlama 34B, DeepSeek Coder 33B, Qwen 2.5 Coder 32B
**Why**: 24GB VRAM handles 30B+ coding models with long context windows (important for code)

### For Homework Help (General LLMs)
**Recommended**: RTX 4060 Ti 16GB (new) or RTX 3090 24GB (used)
**Models**: Llama 3.1 8B, Mistral 7B, Gemma 2 9B
**Why**: 7B-13B models sufficient for Q&A, explanations, tutoring

### For Both Coding + Homework (Best All-Around)
**Recommended**: RTX 3090 24GB (used) at $1,500-1,700 total build
**Models**: Everything from 7B to 70B
**Why**: Flexibility to run small models fast (89-112 tok/s on 8B) or large models capable (42 tok/s on 70B)

### For Future-Proofing
**Recommended**: RTX 5090 32GB
**Why**: 32GB VRAM handles models up to 180B (Q4), 67% faster than 4090, sufficient for most use cases through 2028+
**Note**: For models larger than 180B, consider dual RTX 3090 (48GB) or wait for future multi-GPU setups

---

## Current Pricing (February 2025)

### New GPUs
- **RTX 4060 Ti 8GB**: $399 MSRP
- **RTX 4060 Ti 16GB**: $449-499 (Amazon/Newegg)
- **RTX 4070 Ti Super 16GB**: $800-900
- **RTX 4090 24GB**: $1,599
- **RTX 5090 32GB**: $1,999 (launched January 2025)
- **AMD 7900 XTX 24GB**: $900

### Used GPUs (eBay)
- **RTX 3090 24GB**: $700-900 (excellent value)
- **RTX 3090 Ti 24GB**: $800-1,000 (Micro Center refurbished available)

---

## Final Recommendation for Your Use Case

Based on your requirements (coding assistance + homework help, NetworkChuck-style setup):

### Recommended Build: RTX 3090 24GB Sweet Spot ($1,500-1,700)

**Why This Build**:
1. **Best Value**: 24GB VRAM for $700-900 GPU cost
2. **Versatile**: Runs everything from 7B (homework help) to 70B (advanced coding)
3. **Proven**: Widely used in local AI community with excellent support
4. **Practical**: 42 tok/s on 70B models is plenty fast for interactive use
5. **Upgrade Path**: Can add a second RTX 3090 later for 48GB total VRAM

**Performance Expectations**:
- **Homework Help**: Lightning-fast (87 tok/s on Llama 8B, Mistral 7B)
- **Coding Assistance**: Very capable (60-90 tok/s on CodeLlama 13B, 20-28 tok/s on DeepSeek Coder 33B)
- **Context Windows**: Full 8K-32K context on most models (enough for long coding files)

**Software Stack** (NetworkChuck-style):
1. Install Ubuntu 24.04 LTS or use WSL2 on Windows 11
2. Install Ollama: `curl -fsSL https://ollama.com/install.sh | sh`
3. Install Open WebUI: `docker run -d -p 3000:8080 ghcr.io/open-webui/open-webui:main`
4. Download models: `ollama pull codellama:34b`, `ollama pull mistral:7b`
5. Access Open WebUI at `http://localhost:3000`

**Total Cost Breakdown**:
- GPU: RTX 3090 24GB (used) - $800
- CPU: AMD Ryzen 7 7700X - $300
- RAM: 64GB DDR5-5600 - $250
- Storage: 2TB NVMe Gen4 - $160
- Motherboard: X670 - $200
- PSU: 850W 80+ Gold - $135
- Case: ATX Mid Tower - $100
- **Total: ~$1,945**

---

## Sources

**GPU Performance & Benchmarks**:
- <a href="https://www.mayhemcode.com/2025/12/the-complete-guide-to-local-llm.html" target="_blank">The Complete Guide to Local LLM Hardware | Mayhem Code</a>
- <a href="https://localllm.in/blog/best-gpus-llm-inference-2025" target="_blank">The Best GPUs for Local LLM Inference in 2025 | LocalLLM.in</a>
- <a href="https://localaimaster.com/blog/best-gpus-for-ai-2025" target="_blank">Best GPU for Local AI 2026: RTX 4090 vs 3090 vs 4070 | Local AI Master</a>
- <a href="https://www.pugetsystems.com/labs/articles/llm-inference-consumer-gpu-performance/" target="_blank">LLM Inference - Consumer GPU performance | Puget Systems</a>
- <a href="https://www.hardware-corner.net/gpu-ranking-local-llm/" target="_blank">The Definitive GPU Ranking for LLMs | Hardware Corner</a>
- <a href="https://www.databasemart.com/blog/ollama-gpu-benchmark-rtx4090" target="_blank">Benchmarking LLMs on NVIDIA RTX 4090 GPU Server with Ollama</a>
- <a href="https://github.com/XiongjieDai/GPU-Benchmarks-on-LLM-Inference" target="_blank">GPU Benchmarks on LLM Inference - GitHub</a>

**PC Build Guides & Component Lists**:
- <a href="https://sanj.dev/post/building-affordable-ai-hardware-local-llms" target="_blank">Build a $1500 AI Powerhouse: The 2025 Guide to Local LLM Hardware | sanj.dev</a>
- <a href="https://localaimaster.com/hardware" target="_blank">AI PC Builds 2025: $899 Budget to $3,499 Tested</a>
- <a href="https://techpurk.com/build-ai-pc-specs-2026-local-llms/" target="_blank">Build Your Own AI PC: Recommended Specs for Local LLMs in 2026</a>
- <a href="https://localaimaster.com/blog/ai-hardware-requirements-2025-complete-guide" target="_blank">AI Hardware Requirements 2025: Complete RTX 5090, CPU, RAM & Storage Guide</a>
- <a href="https://www.microcenter.com/site/mc-news/article/hgg-ai-pc-build.aspx" target="_blank">Holiday Gift Guide 2025: The Best PC Parts for the Local AI PC Builder</a>
- <a href="https://pcpartpicker.com/b/fwxcCJ" target="_blank">LLM AI Workstation by LlamaBox - PCPartPicker</a>

**RTX 5090 Benchmarks**:
- <a href="https://www.runpod.io/blog/rtx-5090-llm-benchmarks" target="_blank">RTX 5090 LLM Benchmarks: Is It the Best GPU for AI? | Runpod Blog</a>
- <a href="https://www.hardware-corner.net/rtx-5090-llm-benchmarks/" target="_blank">RTX 5090 LLM Benchmark Results: 10K Tokens/sec Prompt Processing</a>
- <a href="https://www.databasemart.com/blog/ollama-gpu-benchmark-rtx5090" target="_blank">RTX 5090 Ollama Benchmark: Extreme Performance</a>
- <a href="https://vipinpg.com/blog/benchmarking-rtx-5090-vs-4090-for-local-llm-inference-real-world-tokensecond-gains-with-ollama-and-lm-studio" target="_blank">Benchmarking RTX 5090 vs 4090 for Local LLM Inference</a>

**RTX 3090 vs RTX 4060 Ti / 4070 Ti Comparisons**:
- <a href="https://localaimaster.com/blog/best-gpus-for-ai-2025" target="_blank">Best GPU for AI 2025: RTX 4090 vs 3090 vs 4070</a>
- <a href="https://www.databasemart.com/blog/nvidia-rtx-3090-vs-nvidia-rtx-4070ti" target="_blank">RTX 3090 vs 4070 Ti: Ultimate Comparison for Gaming & AI</a>
- <a href="https://www.hardware-corner.net/gpu-for-llm-in-march-2025-20250326/" target="_blank">Buying a GPU for LLMs in March 2025? Read This First!</a>
- <a href="https://www.xda-developers.com/used-rtx-3090-value-king-local-ai/" target="_blank">A used RTX 3090 remains the value king for local AI</a>

**AMD vs NVIDIA**:
- <a href="https://sanj.dev/post/amd-vs-nvidia-ai-workloads-performance-2025" target="_blank">AMD vs NVIDIA AI Performance: Real-World Analysis 2025</a>
- <a href="https://www.techreviewer.com/tech-specs/amd-rx-7900-xtx-gpu-for-llms/" target="_blank">Is the Radeon RX 7900 XTX Good for Running LLMs?</a>
- <a href="https://blog.mlc.ai/2023/08/09/Making-AMD-GPUs-competitive-for-LLM-inference" target="_blank">MLC | Making AMD GPUs competitive for LLM inference</a>

**RAM & System Requirements**:
- <a href="https://www.ikangai.com/the-complete-guide-to-running-llms-locally-hardware-software-and-performance-essentials/" target="_blank">The Complete Guide to Running LLMs Locally</a>
- <a href="https://apxml.com/courses/getting-started-local-llms/chapter-2-preparing-local-environment/hardware-ram" target="_blank">RAM Requirements for Local LLMs | APXML</a>
- <a href="https://www.microcenter.com/site/mc-news/article/best-local-llms-8gb-16gb-32gb-memory-guide.aspx" target="_blank">Run AI Locally: The Best LLMs for 8GB, 16GB, 32GB Memory and Beyond</a>

**Software Stack (Ollama, LM Studio, Open WebUI)**:
- <a href="https://hyscaler.com/insights/ollama-and-openwebui/" target="_blank">Insider's Guide To Ollama And OpenWebUI In 2025! | HyScaler</a>
- <a href="https://www.gpu-mart.com/blog/ollama-vs-lm-studio" target="_blank">Ollama vs LM Studio: What's the Key Differences</a>
- <a href="https://sailingbyte.com/blog/the-ultimate-comparison-of-free-desktop-tools-for-running-local-llms/" target="_blank">Free LLM Desktop Tools - The Ultimate Comparison for 2025</a>
- <a href="https://blog.belsterns.com/post/open-webui-vs-lm-studio-choosing-the-right-local-ai-tool-in-2025" target="_blank">Open WebUI vs LM Studio: Choosing the Right Local AI Tool in 2025</a>
- <a href="https://medium.com/@rosgluk/local-llm-hosting-complete-2025-guide-ollama-vllm-localai-jan-lm-studio-more-f98136ce7e4a" target="_blank">Local LLM Hosting: Complete 2025 Guide</a>

**Pricing Information**:
- <a href="https://bestvaluegpu.com/history/new-and-used-rtx-4060-ti-price-history-and-specs/" target="_blank">RTX 4060 Ti Price Tracker US</a>
- <a href="https://bestvaluegpu.com/history/new-and-used-rtx-3090-price-history-and-specs/" target="_blank">RTX 3090 Price Tracker US</a>
- <a href="https://www.ebay.com/b/NVIDIA-GeForce-RTX-3090-24GB-GDDR6-Graphics-Cards/27386/bn_7117810176" target="_blank">NVIDIA GeForce RTX 3090 24GB GDDR6 Graphics Cards | eBay</a>
