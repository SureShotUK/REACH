# AI Applications for Linux — Platform Overview

**System context:** Dual NVIDIA RTX 3090 (2x 24GB = 48GB VRAM total), Linux host
**Research date:** March 2026

---

## Key Multi-GPU Context

Multi-GPU support works very differently between LLM and image generation tools:

- **LLM tools** (Ollama, vLLM, llama.cpp, Text Gen WebUI): Can genuinely distribute a single large model across both GPUs using tensor/layer splitting. This is how you run 70B+ parameter models that exceed a single card's VRAM.
- **Image generation tools** (ComfyUI, A1111, Forge, InvokeAI): Generally designed for single-GPU operation. "Multi-GPU" typically means running two separate instances (one per GPU for parallel throughput), not combining both GPUs to generate a single image faster.

Your 48GB VRAM is arguably more valuable than the multi-GPU aspect for image generation — you can hold massive models in VRAM simultaneously without constant model swapping.

---

## LLM / Chat Platforms

---

### Ollama *(currently installed)*

**What it does:** LLM inference server and CLI. Automatically downloads and runs GGUF-format language models.

| | |
|---|---|
| **Linux support** | Tier 1 — official install script, seamless |
| **Multi-GPU** | Automatic. Detects both 3090s and shards model layers across them. Supports parallel model loading (OLLAMA_MAX_LOADED_MODELS=2 keeps two models ready simultaneously) |
| **Development** | Very active |
| **Ease of use** | Extreme — single command to download and run any model |
| **Format** | GGUF (via llama.cpp internally) |

**Pros**
- Zero configuration — just works out of the box
- Automatic dual-GPU detection and layer distribution
- REST API compatible with OpenAI format (works with any frontend)
- Model library with one-command downloads

**Cons**
- Wraps llama.cpp with a Go abstraction layer, losing ~15–30% raw performance
- Cannot access experimental llama.cpp flags (e.g., advanced graph-split modes)
- No built-in chat interface (pair with Open WebUI)

---

### Open WebUI

**What it does:** A polished, ChatGPT-style web interface that sits in front of Ollama (or any OpenAI-compatible API). Does not replace Ollama — it enhances it.

| | |
|---|---|
| **Linux support** | Tier 1 — Docker recommended, native Python available |
| **Multi-GPU** | Inherited from backend (Ollama handles it) |
| **Development** | Hyper-active — considered the gold standard for self-hosted LLM interfaces |
| **Ease of use** | High — clean, modern ChatGPT-like experience |

**Pros**
- Chat history, document uploads, built-in RAG (ask questions about your files)
- "Functions" plugin system for extending capabilities
- Multi-user support with user management
- Works with Ollama, vLLM, or any OpenAI-compatible API as backend
- Zero disruption to existing Ollama setup

**Cons**
- Requires Docker or Python setup (not a single binary)
- Adds a layer of complexity if you only need simple CLI use

**Recommended:** Install immediately if you use Ollama via terminal. Transforms the experience significantly.

---

### vLLM

**What it does:** High-throughput LLM inference server with an OpenAI-compatible API. No built-in GUI — designed as a backend, pair with Open WebUI as a frontend.

| | |
|---|---|
| **Linux support** | Tier 1 — Linux is the primary platform |
| **Multi-GPU** | Native tensor parallelism. `--tensor-parallel-size 2` treats your two 3090s as a single 48GB unit. Significantly more efficient than Ollama's layer splitting for large models. |
| **Development** | Hyper-active — industry standard for production LLM serving |
| **Ease of use** | Low — CLI/API only, requires more technical setup |
| **Format** | Hugging Face safetensors, AWQ, GPTQ, FP8 (GGUF support is experimental and slower) |

**Pros**
- **PagedAttention** — near-zero VRAM waste, enabling much larger context windows than Ollama for the same model
- **Continuous batching** — 4–6x faster than Ollama when handling multiple simultaneous requests
- Best possible multi-GPU utilisation for large models
- Drop-in OpenAI API replacement (Open WebUI connects to it unchanged)

**Cons**
- Purely CLI/API — no chat interface (use Open WebUI)
- Better suited to Hugging Face safetensors/AWQ models than GGUF
- Higher setup complexity than Ollama
- Pre-allocates ~90% of VRAM at startup (by design — it's efficient, but looks alarming)

**Install:** `pip install vllm` or `docker pull vllm/vllm-openai`

**When to use over Ollama:** When you want to run 70B+ models using the full 48GB, when you need very long context windows, or if multiple people are using the system simultaneously.

---

### llama.cpp (direct)

**What it does:** The C++ library that powers most local LLM tools (Ollama wraps it). Running it directly gives you maximum performance and access to cutting-edge features before they reach frontends.

| | |
|---|---|
| **Linux support** | Tier 1 |
| **Multi-GPU** | `-ngl 999 -sm graph --tensor-split 1,1` for equal split across both GPUs. The 2026 "graph split" mode allows both GPUs to compute chunks in parallel — 3–4x speed improvement over older layer-split mode. |
| **Development** | Very active — foundation for nearly all other local AI tools |
| **Ease of use** | Low — CLI only |
| **Format** | Authoritative source for GGUF |

**Pros**
- ~15–30% faster than Ollama (no abstraction overhead)
- Granular KV cache quantization control for fine-tuning VRAM/quality tradeoffs
- **RPC mode** — can join another PC on your network as additional inference GPU
- Latest experimental features and multi-GPU optimisations

**Cons**
- Pure CLI — you manage everything yourself
- No model library or discovery features
- Less user-friendly than Ollama for day-to-day use

**When to use:** When you want the absolute fastest GGUF inference from your hardware, or need features not yet in Ollama.

---

### Text Generation WebUI (oobabooga)

**What it does:** Web GUI for LLM inference — often described as "the A1111 of LLMs." Supports multiple inference backends behind a single interface.

| | |
|---|---|
| **Linux support** | High — `start_linux.sh` creates a self-contained conda environment, or Docker |
| **Multi-GPU** | Granular: `--auto-devices` (automatic) or `--gpu-memory 22 22` (explicit VRAM per card) |
| **Development** | Actively maintained |
| **Ease of use** | Moderate — web GUI launched via CLI, requires flag tuning for dual GPU |

**Supported backends (key advantage — one UI, every engine):**
- llama.cpp (GGUF)
- ExLlamaV2 (EXL2 — fastest on NVIDIA)
- AutoGPTQ (GPTQ)
- Transformers (native Hugging Face)
- TensorRT-LLM (NVIDIA's highest-performance backend)

**Pros**
- Only platform that lets you swap between every loader and quantization type without reinstalling
- Personas/Characters system — define AI identities with specific system prompts
- Notebook mode — raw prompting interface like OpenAI Playground
- Extensions — web search, image generation, long-term memory
- LoRA fine-tuning support
- OpenAI-compatible API (can serve as backend for other apps)

**Cons**
- UI feels dated compared to Jan.ai or LM Studio
- More complex initial setup than Ollama
- Can be brittle during updates

**Best for:** Experimenting and comparing — switch from GGUF to EXL2 to GPTQ in the same session to find what works best on your hardware.

---

### ExLlamaV2 *(engine, not a standalone app)*

**What it does:** NVIDIA-only inference engine used as the backend inside Text Gen WebUI and TabbyAPI. Not a standalone app, but worth understanding.

| | |
|---|---|
| **Linux support** | Yes |
| **Multi-GPU** | Yes — distributes across both 3090s |
| **Development** | Active |

**Why it matters:** Custom CUDA kernels and Flash Attention 2, specifically optimised for Ampere architecture (your RTX 3090s are Ampere generation). Uses EXL2 format — "variable bits per weight" — assigning more bits to more important model layers. Produces better output quality than GGUF at the same file size.

**EXL2 vs GGUF comparison:**

| | EXL2 | GGUF |
|---|---|---|
| Platform | NVIDIA only | Universal (CPU, Apple Silicon, NVIDIA) |
| Quality at same file size | Better (variable bpw allocation) | Portable but less optimal on NVIDIA |
| Best used via | Text Gen WebUI | Ollama, llama.cpp, LM Studio |

On 2x 3090s running a 70B model at 4bpw: typically 15–20 tokens/second.

---

### LM Studio

**What it does:** Desktop application for downloading, managing, and chatting with GGUF models. Best described as a polished test bench.

| | |
|---|---|
| **Linux support** | Stable native Linux app as of early 2026 (was in beta previously) |
| **Multi-GPU** | Granular controls — manually set layers to GPU 0 vs GPU 1, or auto-balance |
| **Development** | Very active (commercial/proprietary) |
| **Ease of use** | High — best-in-class GUI for model management |

**Pros**
- Excellent Hugging Face integration — browse and download specific quantization variants directly in the app
- Superior in-app hardware monitoring (see VRAM usage per GPU in real time)
- Built-in chat interface + local server mode (OpenAI API compatible)

**Cons**
- Closed source core
- Higher system overhead than Ollama
- Less scriptable / harder to automate
- Proprietary — no community auditing

**Best for:** Quickly evaluating new GGUF models before committing them to your Ollama library. The Discover tab makes finding specific quantizations straightforward.

---

### Jan.ai

**What it does:** Desktop application — privacy-first alternative to LM Studio with built-in model hub, hardware monitoring, and chat.

| | |
|---|---|
| **Linux support** | Native `.AppImage` and `.deb` packages |
| **Multi-GPU** | Full multi-GPU detection via its Cortex engine — automatically shards GGUF models across both 3090s |
| **Development** | Active — v0.7.x in early 2026 |
| **Ease of use** | High — everything (model hub, config, hardware monitoring, chat) in one app |

**Pros**
- Open source
- All-in-one: model discovery from Hugging Face, download, configuration, chat
- No separate terminal or CLI needed
- Better out-of-the-box experience than Ollama for users who prefer GUI

**Cons**
- Less ecosystem integration than Ollama (fewer tools connect to Jan natively)
- Smaller community than Ollama or LM Studio

**Best for:** Users who prefer a single desktop app over Ollama + separate frontend.

---

### AnythingLLM

**What it does:** RAG (Retrieval-Augmented Generation) platform — "chat with your documents." Connects to Ollama as its backend and adds a knowledge management layer on top.

*RAG = a technique where the AI searches your documents for relevant passages and includes them in its context window before answering, so it can answer questions based on your specific files rather than just its training data.*

| | |
|---|---|
| **Linux support** | Native desktop app + Docker |
| **Multi-GPU** | Indirect — inherits from Ollama or vLLM backend |
| **Development** | Very active |
| **Ease of use** | High — built-in document management and vector database |

**Pros**
- Upload PDFs, code repositories, documents — ask questions about them
- Workspace organisation (separate knowledge bases per project)
- Agent capabilities (can take actions, not just answer questions)
- Points at your existing Ollama instance — no changes to your backend

**Cons**
- Not a general chat interface — specialised for document querying
- Adds complexity (vector database management) for simple use cases

**Best for:** If you want to query internal documents, PDFs, or a knowledge base using your 48GB VRAM. Ollama handles inference; AnythingLLM handles knowledge retrieval.

---

## Image Generation Platforms

---

### ComfyUI *(currently installed)*

**What it does:** Node-based workflow editor for image and video generation. Industry standard for complex, customisable pipelines.

| | |
|---|---|
| **Linux support** | Tier 1 |
| **Multi-GPU** | Via `ComfyUI-MultiGPU` extension — offloads pipeline components (e.g., VAE to GPU 1, diffusion UNet to GPU 0). Not true parallelism — distributes different stages, not one stage across both GPUs. |
| **Development** | Hyper-active — current leader for image/video generation |
| **Ease of use** | Moderate — visual programming with a learning curve |

**Pros**
- Maximum flexibility — build any workflow imaginable
- Best support for cutting-edge models (FLUX.1, Wan video, etc.)
- Powerful for batch processing and automation
- Active extension ecosystem

**Cons**
- Steep learning curve for complex workflows
- No single image generates faster using both GPUs simultaneously
- UI can feel overwhelming for straightforward tasks

**Multi-GPU reality:** The extension allows component-level distribution (VAE on one card, UNet on the other), which can improve throughput for complex pipelines. No image generation tool currently parallelises both GPUs for a single image faster than one GPU.

---

### Forge (stable-diffusion-webui-forge)

**What it does:** Optimised fork of A1111 by lllyasviel — same familiar interface, significantly better VRAM efficiency and support for new model architectures.

| | |
|---|---|
| **Linux support** | High |
| **Multi-GPU** | Two separate instances only (one per GPU for parallel batch generation) |
| **Development** | Very active — typically first to get optimisations for new models (FLUX.1, SD 3.5) |
| **Ease of use** | High — familiar A1111-style sliders and settings |

**Pros**
- "NeverOOM" VRAM management — handles FLUX.1 on a single 3090 without the issues that plague A1111
- Significantly faster and more stable than A1111 for current-generation models
- Same extension ecosystem as A1111 with better compatibility

**Cons**
- No genuine multi-GPU parallelism for a single image
- A1111-style UI is less powerful than ComfyUI for complex workflows

**Best for:** When you want standard SD/FLUX generation (no node graphs) with better memory management than vanilla A1111. Good complement to ComfyUI — run Forge on GPU 1 for quick standard generation while ComfyUI uses GPU 0 for complex workflows.

---

### AUTOMATIC1111 (stable-diffusion-webui)

**What it does:** The original web UI for Stable Diffusion — the most widely documented and extended image generation frontend.

| | |
|---|---|
| **Linux support** | High |
| **Multi-GPU** | Weak — no native parallelisation for a single image. Two separate instances for batch throughput only. |
| **Development** | Active but maturing — focused on stability over new features |
| **Ease of use** | High — familiar sliders and settings, huge online documentation |

**Pros**
- Largest extension ecosystem of any image generation tool
- Enormous community — tutorials, guides, and help are everywhere
- Stable and well-understood behaviour

**Cons**
- Poorer VRAM management than Forge or ComfyUI
- Slower to support new model architectures vs Forge
- Less powerful than ComfyUI for complex or automated workflows

**Verdict vs Forge:** Forge is strictly better than A1111 for current hardware and models. A1111's advantage is familiarity and documentation volume.

---

### InvokeAI

**What it does:** Professional creative suite for Stable Diffusion — focused on the "digital painter" workflow with a polished, non-destructive editing canvas.

| | |
|---|---|
| **Linux support** | Native, very stable |
| **Multi-GPU** | Sequential round-robin (GPU 0 does image 1, GPU 1 does image 2 in a batch) |
| **Development** | Active — Version 6.x in early 2026 |
| **Ease of use** | High — widely considered the most polished open-source image gen UI |

**Pros**
- Unified Canvas — infinite canvas for inpainting and outpainting, superior to ComfyUI or Forge for this use case
- Professional, "Apple-like" interface quality
- Strong canvas-based workflow for iterative artwork

**Cons**
- No genuine multi-GPU parallelism for single image generation
- Less flexible than ComfyUI for complex automated pipelines
- Smaller extension ecosystem than A1111/Forge

**Best for:** Serious inpainting and outpainting work. If you do iterative artwork where you extend, refine, and fill images non-destructively, InvokeAI's canvas is meaningfully better than the alternatives.

---

### Fooocus

**What it does:** Simplified Midjourney-style image generation — the easiest possible interface for high-quality images without prompt engineering.

| | |
|---|---|
| **Linux support** | High |
| **Multi-GPU** | None |
| **Development** | **Archived** — original lllyasviel repo is in maintenance mode. Community forks FooocusPlus and RuinedFooocus are active. |
| **Ease of use** | Extreme — simplest UI of any tool on this list |

**Pros**
- Near-zero learning curve
- Handles style and quality optimisation internally — just describe what you want
- Midjourney-quality results from minimal input

**Cons**
- Original project archived — limited future development
- No multi-GPU support
- Less control than ComfyUI or Forge
- Community forks vary in quality and maintenance

**Best for:** Beginners, or when you want impressive results with no configuration.

---

## Summary Table

| Platform | Type | Multi-GPU (Dual 3090) | Linux | Development | Ease of Use |
|---|---|---|---|---|---|
| **Ollama** | LLM server + CLI | Auto layer-split | Tier 1 | Very active | Extreme |
| **Open WebUI** | LLM frontend | Via backend | Tier 1 (Docker) | Hyper-active | High |
| **vLLM** | LLM server (API) | Native tensor parallelism | Tier 1 | Hyper-active | Low (CLI) |
| **llama.cpp** | LLM CLI/library | Graph-split (2026) | Tier 1 | Very active | Low (CLI) |
| **Text Gen WebUI** | LLM web GUI | Granular VRAM per GPU | High | Active | Moderate |
| **LM Studio** | LLM desktop app | Granular toggle | Stable (2026) | Very active | High |
| **Jan.ai** | LLM desktop app | Auto via Cortex | Native | Active | High |
| **AnythingLLM** | RAG / Document AI | Via backend | Native + Docker | Very active | High |
| **ComfyUI** | Image/Video gen | Extension only | Tier 1 | Hyper-active | Moderate |
| **Forge** | Image gen | Two instances only | High | Very active | High |
| **A1111** | Image gen | Two instances only | High | Active (mature) | High |
| **InvokeAI** | Image gen | Round-robin batching | Native | Active | High |
| **Fooocus** | Image gen | None | High | Archived | Extreme |

---

## Recommended Additions to Current Stack

**Priority 1 — Open WebUI:** Install alongside Ollama immediately. Transforms CLI use into a polished chat interface with no changes to your backend configuration.

**Priority 2 — vLLM:** Set up via Docker alongside Ollama. Use `--tensor-parallel-size 2` to treat both 3090s as a single 48GB unit. Connect Open WebUI to it as an alternative backend when you need maximum performance for large models or long context.

**Priority 3 — AnythingLLM:** Add when you need to query documents, PDFs, or a knowledge base. Points at your existing Ollama instance — no backend changes needed.

**Priority 4 — Text Gen WebUI:** Worth having for experimentation. The ability to switch to ExLlamaV2 with EXL2-format models is the fastest possible inference path on Ampere (RTX 3090) hardware.

**Image generation:** The real benefit of your dual-GPU setup is **two simultaneous independent workflows** — e.g., ComfyUI on GPU 0 for complex FLUX/video work, and Forge on GPU 1 for fast standard generation at the same time. No current image generation tool will combine both GPUs to make a single image faster.

**Docker prerequisite:** Install `nvidia-container-toolkit` to ensure Docker containers (Open WebUI, vLLM, AnythingLLM) can access both GPUs:
```bash
sudo apt-get install -y nvidia-container-toolkit
sudo systemctl restart docker
```

---

## Sources

- <a href="https://docs.vllm.ai/en/latest/" target="_blank">vLLM Official Documentation</a>
- <a href="https://github.com/vllm-project/vllm" target="_blank">vLLM GitHub Repository</a>
- <a href="https://github.com/oobabooga/text-generation-webui" target="_blank">Text Generation WebUI GitHub</a>
- <a href="https://github.com/ggerganov/llama.cpp" target="_blank">llama.cpp GitHub</a>
- <a href="https://github.com/turboderp/exllamav2" target="_blank">ExLlamaV2 GitHub</a>
- <a href="https://openwebui.com/" target="_blank">Open WebUI</a>
- <a href="https://jan.ai/" target="_blank">Jan.ai</a>
- <a href="https://anythingllm.com/" target="_blank">AnythingLLM</a>
- <a href="https://lmstudio.ai/" target="_blank">LM Studio</a>
- <a href="https://github.com/lllyasviel/stable-diffusion-webui-forge" target="_blank">Forge GitHub</a>
- <a href="https://github.com/invoke-ai/InvokeAI" target="_blank">InvokeAI GitHub</a>
- <a href="https://github.com/AUTOMATIC1111/stable-diffusion-webui" target="_blank">AUTOMATIC1111 GitHub</a>
- <a href="https://github.com/lllyasviel/Fooocus" target="_blank">Fooocus GitHub</a>
