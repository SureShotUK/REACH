# ComfyUI Setup Research: Dual RTX 3090 / Ubuntu 24.04 LTS Server

**Research Date**: March 2026
**System**: Dual RTX 3090 (48GB combined VRAM), Ubuntu 24.04 LTS Server
**Existing infrastructure**: Docker with NVIDIA Container Toolkit, Open WebUI on `ai-network` bridge, Tailscale

---

## Table of Contents

1. [ComfyUI Installation](#1-comfyui-installation)
2. [Best Models by Use Case](#2-best-models-by-use-case)
3. [Workflow Sources](#3-workflow-sources)
4. [Open WebUI Integration](#4-open-webui-integration)
5. [Wan2.x Video Generation Specifics](#5-wan2x-video-generation-specifics)
6. [ComfyUI Manager](#6-comfyui-manager)
7. [Storage Planning](#7-storage-planning)
8. [Recommended Setup Summary](#8-recommended-setup-summary)

---

## 1. ComfyUI Installation

### Docker vs Native Python: Recommendation

**Recommendation: Docker using the `yanwk/comfyui-boot` image**

Given that the system already uses Docker extensively (Ollama, Open WebUI, SearXNG), Docker is the consistent choice. The specific advantages in this environment:

- Isolation from the host Python environment (avoids CUDA version conflicts with Ollama)
- Fits naturally into the existing `ai-network` Docker bridge
- Clean upgrade path (pull new image tag, recreate container)
- No risk of breaking existing PyTorch/CUDA installations

The main trade-off versus native Python is that multi-GPU distributed inference for video (Wan2.x) works more naturally natively, but this can be addressed (see Section 5).

### Recommended Docker Image

**Image**: `yanwk/comfyui-boot` (GitHub: <a href="https://github.com/YanWenKun/ComfyUI-Docker" target="_blank">YanWenKun/ComfyUI-Docker</a>)

This is the most actively maintained community ComfyUI Docker image. It is not an official image (ComfyUI has no official Docker image), but it is widely used and well-documented.

**Recommended tag**: `cu128-slim`
- CUDA 12.8, Python 3.12
- "slim" means ComfyUI only — custom nodes installed via ComfyUI Manager at runtime
- Alternative `cu128-megapak` includes pre-installed custom nodes and dev tools but is much larger

**Why not the official image?** ComfyUI does not publish an official Docker image. The `yanwk` image is the current community standard.

**Port**: ComfyUI listens on **8188** inside the container.
**Browser URL**: `http://<server-ip>:8188`

### Docker Run Command

```bash
docker run -d \
  --name comfyui \
  --network ai-network \
  --restart unless-stopped \
  --runtime nvidia \
  --gpus all \
  -p 8188:8188 \
  -v /mnt/models/comfyui:/root/ComfyUI/models \
  -v /opt/comfyui/storage:/root \
  -v /opt/comfyui/input:/root/ComfyUI/input \
  -v /opt/comfyui/output:/root/ComfyUI/output \
  -v /opt/comfyui/workflows:/root/ComfyUI/user/default/workflows \
  -e CLI_ARGS="--disable-xformers" \
  yanwk/comfyui-boot:cu128-slim
```

**Notes on this command:**

| Flag | Purpose |
|------|---------|
| `--network ai-network` | Joins the existing Docker bridge so Open WebUI can reach ComfyUI by container name (`http://comfyui:8188`) |
| `--runtime nvidia --gpus all` | NVIDIA Container Toolkit passthrough — same pattern as Open WebUI |
| `-v /mnt/models/comfyui:/root/ComfyUI/models` | Maps your dedicated model storage to the ComfyUI models directory |
| `-v /opt/comfyui/storage:/root` | Persists ComfyUI installation, custom nodes, and config across container restarts |
| `--disable-xformers` | Disables xformers attention optimization — on dual 3090 with 48GB VRAM, standard attention is fine; xformers can cause issues with some custom nodes |
| `--restart unless-stopped` | Auto-starts with Docker daemon (consistent with your other containers) |

**Create the storage directories before running:**
```bash
sudo mkdir -p /mnt/models/comfyui
sudo mkdir -p /opt/comfyui/{storage,input,output,workflows}
sudo chown -R 1000:1000 /opt/comfyui
```

### Native Python Installation (for reference)

If you prefer native Python (e.g., for Wan2.x multi-GPU video):

```bash
# Python 3.12 recommended (3.13 is supported but some custom nodes lag)
git clone https://github.com/comfyanonymous/ComfyUI.git
cd ComfyUI
python3.12 -m venv venv
source venv/bin/activate

# PyTorch for CUDA 12.8
pip install torch torchvision torchaudio --extra-index-url https://download.pytorch.org/whl/cu128

pip install -r requirements.txt

# Run (bind to all interfaces for LAN/Tailscale access)
python main.py --listen 0.0.0.0 --port 8188
```

**Flag**: `--extra-model-paths-config` can point ComfyUI to `/mnt/models/comfyui` without needing to move files.

---

## 2. Best Models by Use Case

### 2a. Photorealistic Image Generation

**Primary recommendation: FLUX.1 Dev (fp8 single-file)**

FLUX.1 Dev from Black Forest Labs remains the leading open-source photorealism model as of March 2026. It has 714k monthly downloads on Hugging Face — significantly ahead of competitors. A newer challenger has emerged (see below), but FLUX.1 Dev is the most established with the broadest LoRA ecosystem and ComfyUI workflow support.

**Model variants available from Comfy-Org (pre-packaged for ComfyUI):**

| File | Size | Notes |
|------|------|-------|
| `flux1-dev-fp8.safetensors` | 17.2 GB | Recommended for most users — includes both text encoders |
| `flux1-dev.safetensors` | 23.8 GB | Full bf16 precision — marginal quality gain |

Source: <a href="https://huggingface.co/Comfy-Org/flux1-dev" target="_blank">huggingface.co/Comfy-Org/flux1-dev</a>

The fp8 version is a **single-file checkpoint** that loads via the standard `Load Checkpoint` node in ComfyUI — no separate text encoder files needed. On 48GB VRAM, the full bf16 version (23.8GB) will also run comfortably.

**VRAM requirement**: ~20-24GB for fp8; ~24-28GB for full bf16. Easily fits in a single RTX 3090 (24GB), so one GPU handles image generation while the other remains available for Ollama.

**Newer alternative: Z-Image-Turbo**

`Tongyi-MAI/Z-Image-Turbo` (Alibaba) is a newer 6B parameter model that fits in 16GB VRAM and generates in ~8 inference steps (very fast). It claims state-of-the-art results among open-source models on human preference evaluation benchmarks. It is newer and less battle-tested than FLUX.1 Dev, and its LoRA ecosystem is smaller.

**Verdict**: Start with FLUX.1 Dev fp8 (established, huge workflow library). Z-Image-Turbo is worth experimenting with later for speed.

**Also note**: FLUX.1 Kontext Dev (image editing), FLUX.1 Fill Dev (inpainting), and FLUX.1 Depth/Canny Dev (structural conditioning) are available from the same Black Forest Labs repository if you want those capabilities later.

**Download location**: <a href="https://huggingface.co/Comfy-Org/flux1-dev" target="_blank">huggingface.co/Comfy-Org/flux1-dev</a>
(Requires accepting Black Forest Labs non-commercial license on Hugging Face)

---

### 2b. Pixar / Animation Style

ComfyUI itself doesn't provide a single "Pixar style" model — this is achieved through either:

**Option A: FLUX.1 Dev + Style LoRAs** (recommended)

LoRAs (Low-Rank Adaptations) are small fine-tuned adapter files that modify a base model's output style. FLUX.1 Dev has a large LoRA ecosystem.

> **Note**: CivitAI, the primary LoRA hosting site, is blocked in the UK due to the Online Safety Act (as of July 2025). LoRAs must be sourced from Hugging Face or other platforms.

Hugging Face search for FLUX Pixar/animation LoRAs: <a href="https://huggingface.co/models?other=lora&pipeline_tag=text-to-image&search=pixar+flux" target="_blank">HuggingFace FLUX LoRA search</a>

Example well-known styles that typically have LoRAs available:
- Pixar/3D animation style
- Studio Ghibli style
- Comic/illustration style

LoRA files are small (typically 50-400MB) and placed in `/mnt/models/comfyui/loras/`.

**Option B: Stable Diffusion 3.5 Large**

SD 3.5 Large (Stability AI) is a strong alternative that can produce stylized/artistic images. It is ComfyUI-native (official example workflows provided), requires a community license (free for <$1M annual revenue), and its separate text encoder architecture makes it easy to fine-tune stylistically.

- Main model file: `sd3_large.safetensors`
- Text encoders loaded separately: `clip_g.safetensors`, `clip_l.safetensors`, `t5xxl_fp8_e4m3fn.safetensors`
- Source: <a href="https://huggingface.co/stabilityai/stable-diffusion-3.5-large" target="_blank">huggingface.co/stabilityai/stable-diffusion-3.5-large</a>

**Verdict**: Use FLUX.1 Dev with style LoRAs for the most flexible approach — one base model, multiple styles via LoRA swapping.

---

### 2c. Corporate Logo Generation

**Recommendation: FLUX.1 Dev with specific prompting**

FLUX.1 Dev has strong text rendering compared to older models (a historically weak area for diffusion models). For logo generation:

- Use FLUX.1 Dev (same model as photorealism) — no separate model needed
- Prompt engineering is critical: specify "flat design", "vector style", "minimalist", etc.
- Post-process outputs in Inkscape/Illustrator if vector format needed

**Note on text accuracy**: FLUX.1 models are better at text than SD1.5/SDXL era models but still not fully reliable for complex multi-word logos. Simple text (1-3 words) is generally achievable.

---

## 3. Workflow Sources

### Pre-built Workflow Sites

**1. OpenArt Workflows** — <a href="https://openart.ai/workflows/home" target="_blank">openart.ai/workflows/home</a>
Active platform with thousands of community workflows. Categories include FLUX workflows, video generation, and style-specific generation. Has a "ComfyUI Academy" with tutorial workflows. Free to browse.

**2. ComfyWorkflows** — <a href="https://comfyworkflows.com" target="_blank">comfyworkflows.com</a>
Community-driven workflow sharing platform. Actively maintained (added a "ComfyUI Launcher" tool recently). Includes images, videos, and guides sections. Free to use.

**3. Official ComfyUI Examples** — <a href="https://comfyanonymous.github.io/ComfyUI_examples/" target="_blank">comfyanonymous.github.io/ComfyUI_examples/</a>
The authoritative first stop. Maintained by ComfyUI's author (comfyanonymous). Includes verified example workflows for:
- FLUX models: `/flux/`
- Wan video generation: `/wan/`
- Stable Diffusion 3: `/sd3/`
These are the reference implementations and are guaranteed to work with the current ComfyUI version.

**4. Comfy-Org Repackaged Models** (include example workflows)
The Comfy-Org Hugging Face repositories include example workflow JSON files alongside the model downloads. For example, `Wan_2.1_ComfyUI_repackaged` includes an `example workflows_Wan2.1/` directory.

### How to Load Workflows

In ComfyUI, workflows are loaded by:
1. Dragging and dropping a `.json` workflow file onto the canvas
2. Using the "Load" button in the menu
3. Dragging and dropping a generated image (generated images have the workflow embedded in their metadata)

---

## 4. Open WebUI Integration

### How It Works

Open WebUI connects to ComfyUI via ComfyUI's native HTTP API. The integration is built into Open WebUI natively — no plugins required. The API uses ComfyUI's `/prompt` endpoint for generation and `/object_info` for discovering available models.

### Configuration

**Where to configure**: Open WebUI admin panel → Admin Settings → Images

**Settings to configure**:

| Setting | Value |
|---------|-------|
| Image Generation Engine | ComfyUI |
| ComfyUI Base URL | `http://comfyui:8188` |
| ComfyUI API Key | (leave blank unless you've enabled ComfyUI auth) |
| Workflow | JSON workflow (see below) |

**Why `http://comfyui:8188` works**: Because both containers are on the `ai-network` Docker bridge, Open WebUI can reach the ComfyUI container by its container name (`comfyui`). This is the same principle used for Ollama communication.

**Important**: The container name in the `docker run` command (`--name comfyui`) must match the hostname used in the URL.

### Workflow JSON Requirement

Open WebUI does not have default built-in ComfyUI workflows — **you must supply a workflow JSON**. This is the main configuration complexity.

The workflow JSON must be obtained from ComfyUI itself:
1. In ComfyUI, set up your desired workflow
2. Click the menu → "Save (API Format)" — this saves the workflow in the API JSON format
3. Paste this JSON into Open WebUI's Workflow field

**Important distinction**: ComfyUI has two save formats — "Save" (for use in ComfyUI UI) and "Save (API Format)" (for use with the API). Open WebUI requires the **API format**.

Open WebUI also uses `COMFYUI_WORKFLOW_NODES` to identify which node in the workflow to inject the prompt into. This is configured alongside the workflow JSON.

### Known Issues and Limitations (verified from GitHub issues, March 2026)

| Issue | Status |
|-------|--------|
| ComfyUI connectivity issues in Open WebUI 0.7.2 | Resolved |
| Multiple images in single chat | Resolved |
| Inpainting UI | Open (feature request, not yet implemented) |
| Image editing vs. creation distinction | Open (still occasionally inconsistent) |
| Prompt template sometimes ignored | Resolved |

**Current status**: Basic text-to-image generation via ComfyUI is functional and production-ready in current Open WebUI versions. Inpainting through the Open WebUI interface is not yet available.

---

## 5. Wan2.x Video Generation Specifics

### Model Version: Wan2.2, Not Wan2.1

> **Important**: Wan2.2 has been released and is the current version to target. Wan2.1 is still available and widely used, but Wan2.2 is newer with improved quality.

**What changed in Wan2.2:**
- MoE (Mixture-of-Experts) architecture: 27B total parameters but only ~14B active per inference step — more capable without proportionally more VRAM
- Cinematic-quality aesthetic improvements
- Better generalization from expanded training data (+65% images, +83% videos)
- New efficient VAE (16×16×4 compression)

### Model Variants: Which to Choose for Dual RTX 3090

| Model | Parameters | VRAM Needed | Resolution | Generation Time (5s clip) | Recommended For |
|-------|-----------|------------|------------|--------------------------|-----------------|
| **Wan2.2-TI2V-5B** | 5B | ~24GB | 720P @ 24fps | <9 min on single 4090 | Best consumer choice — fits one RTX 3090 |
| Wan2.2-T2V-A14B | 27B (14B active) | 40-60GB+ with offloading | 480P & 720P | Much slower | Needs both GPUs or heavy offloading |
| Wan2.1-T2V-14B | 14B | ~24GB with offloading | 480P & 720P | ~4 min @ 480P on 4090 | Previous generation |
| Wan2.1-T2V-1.3B | 1.3B | ~8GB | 480P only | Faster | Fast/test generation |

**Recommendation for dual RTX 3090**: Start with **Wan2.2-TI2V-5B**. It:
- Fits in a single RTX 3090 (24GB)
- Supports text-to-video AND image-to-video in one model
- Generates 720P @ 24fps (better quality than Wan2.1's 480P @ comparable VRAM)
- Is ~9 minutes per 5-second clip — acceptable for experimentation

### Model File Sizes (Wan2.2-TI2V-5B)

| File | Size |
|------|------|
| `diffusion_pytorch_model-00001-of-00003.safetensors` | 9.83 GB |
| `diffusion_pytorch_model-00002-of-00003.safetensors` | 10.0 GB |
| `diffusion_pytorch_model-00003-of-00003.safetensors` | 179 MB |
| `models_t5_umt5-xxl-enc-bf16.pth` (T5 text encoder) | 11.4 GB |
| `Wan2.2_VAE.pth` | 2.82 GB |
| **Total** | **~34.3 GB** |

Source: <a href="https://huggingface.co/Wan-AI/Wan2.2-TI2V-5B" target="_blank">huggingface.co/Wan-AI/Wan2.2-TI2V-5B</a>

### Model File Sizes (Wan2.1-T2V-14B, for comparison)

| Item | Size |
|------|------|
| Diffusion model (6 shards) | ~57 GB |
| T5 text encoder | 11.4 GB |
| VAE | 508 MB |
| **Total** | **~69 GB** |

Source: <a href="https://huggingface.co/Wan-AI/Wan2.1-T2V-14B" target="_blank">huggingface.co/Wan-AI/Wan2.1-T2V-14B</a>

### Multi-GPU Utilisation

**Wan2.2-TI2V-5B**: No documented multi-GPU support for the 5B model. It runs comfortably on a single RTX 3090 without needing the second GPU.

**Wan2.2-T2V-A14B and Wan2.1-T2V-14B**: Support distributed inference via PyTorch FSDP + xDiT (Ulysses + Ring attention). The reference examples use 8 GPUs (A100/H100). With only 2 GPUs:
```bash
torchrun --nproc_per_node=2 generate.py \
  --task t2v-A14B --size 832*480 \
  --ckpt_dir ./Wan2.2-T2V-A14B \
  --dit_fsdp --t5_fsdp --ulysses_size 2 \
  --prompt "..."
```
This is possible but **this is native Python only** — not currently supported via the ComfyUI wrapper in Docker.

**Practical summary for dual RTX 3090 + Docker ComfyUI**:
- Use Wan2.2-TI2V-5B via ComfyUI with the `ComfyUI-WanVideoWrapper` custom node
- This uses one GPU; the other remains available for Ollama
- For higher quality later, the 14B models via native Python with both GPUs is an option

### ComfyUI Integration for Video

Install the `ComfyUI-WanVideoWrapper` custom node (via ComfyUI Manager, see Section 6):
- GitHub: <a href="https://github.com/kijai/ComfyUI-WanVideoWrapper" target="_blank">github.com/kijai/ComfyUI-WanVideoWrapper</a>
- Supports both Wan2.1 and Wan2.2
- Supports multiple video models (SkyReels, VACE, etc.)
- Block swapping is supported for VRAM management

**Block swapping**: A VRAM management technique where model layers are swapped between GPU and CPU RAM during inference. On a single RTX 3090 (24GB), the Wan2.2-TI2V-5B model may run without block swapping. If VRAM is tight (e.g., if Ollama is also loaded), enabling block swapping at `~20/40 blocks offloaded` reduces GPU memory usage at the cost of slower generation.

**ComfyUI example workflows for Wan** are available from the official examples site and from the Comfy-Org repackaged model repository:
- Text-to-Video
- Image-to-Video
- VACE Reference Image-to-Video
- First/Last Frame to Video

### Wan2.2 in ComfyUI: Important Note on Model Format

The Wan2.2 5B model files from Hugging Face are in the **original research format** (PyTorch split files). Comfy-Org publishes ComfyUI-repackaged single-file versions of Wan2.1. At time of research, a Comfy-Org repackaged version of Wan2.2 was not confirmed — check <a href="https://huggingface.co/Comfy-Org" target="_blank">huggingface.co/Comfy-Org</a> for current availability before downloading.

**Fallback**: The `ComfyUI-WanVideoWrapper` node can load models directly from their original HuggingFace format, so a Comfy-Org repackaged version is not strictly required.

---

## 6. ComfyUI Manager

### What It Is

ComfyUI Manager is a UI extension for ComfyUI that adds node and model management capabilities. It is the standard way to install custom nodes (like the WanVideoWrapper) without manually cloning Git repositories.

- GitHub: <a href="https://github.com/ltdrdata/ComfyUI-Manager" target="_blank">github.com/ltdrdata/ComfyUI-Manager</a>
- 4,492 commits as of research date — actively maintained

**Capabilities:**
- Install/remove/enable/disable custom nodes from a searchable catalogue
- Install models from Hugging Face and CivitAI
- Create and restore snapshots of the full node installation state
- Detect missing nodes when loading a workflow (auto-install prompt)
- Share workflows with embedded node dependencies

### Is It Still Recommended?

Yes. ComfyUI Manager remains the community-standard method. There is no official built-in alternative for custom node management.

### Installation in Docker

With the `yanwk/comfyui-boot:cu128-slim` image, ComfyUI Manager is **not pre-installed** (it is in the `megapak` image). Install it manually once after first run:

```bash
# Exec into the running container
docker exec -it comfyui bash

# Navigate to custom nodes directory
cd /root/ComfyUI/custom_nodes

# Clone ComfyUI Manager
git clone https://github.com/ltdrdata/ComfyUI-Manager.git

# Install dependencies
cd ComfyUI-Manager
pip install -r requirements.txt

# Restart ComfyUI (restart the container)
exit
docker restart comfyui
```

After restart, ComfyUI Manager appears as a "Manager" button in the ComfyUI interface. All subsequent custom node installations are done through the UI.

**Persistence**: Because `/root` is mapped to `/opt/comfyui/storage`, ComfyUI Manager and all custom nodes installed through it persist across container restarts and image updates. The clone above will survive a `docker restart` or even a `docker rm && docker run` as long as the volume persists.

---

## 7. Storage Planning

### Storage Layout

All ComfyUI models should be stored in `/mnt/models/comfyui/`, which maps to `/root/ComfyUI/models/` inside the container. The directory structure ComfyUI expects is:

```
/mnt/models/comfyui/
├── checkpoints/          # Main image generation models (FLUX, SD3.5, etc.)
├── vae/                  # VAE files (ae.safetensors for FLUX)
├── text_encoders/        # Text encoder files (t5xxl, clip_l)
├── loras/                # LoRA fine-tuning adapters
├── controlnet/           # ControlNet conditioning models
├── unet/                 # UNet/diffusion-only models (alternative to checkpoints)
├── clip/                 # CLIP model files
├── video_models/         # Video generation models (Wan2.x)
│   ├── wan2.2_ti2v_5b/
│   └── wan2.1_t2v_14b/
└── upscale_models/       # Upscaling models (optional)
```

### Estimated Storage Requirements

| Model | Storage |
|-------|---------|
| FLUX.1 Dev fp8 (single file) | 17.2 GB |
| FLUX.1 Dev bf16 (optional, full quality) | 23.8 GB |
| T5 text encoder (if using split FLUX files) | 11.4 GB |
| CLIP-L encoder (if using split FLUX files) | ~0.5 GB |
| FLUX VAE (ae.safetensors) | ~0.3 GB |
| Wan2.2-TI2V-5B (recommended video model) | 34.3 GB |
| Wan2.1-T2V-14B (optional, higher quality) | 69 GB |
| Style LoRAs (several) | ~2-5 GB |
| SD 3.5 Large (optional photorealism alternative) | ~20 GB |
| Upscaling models (optional) | ~0.5-2 GB |
| **Recommended initial setup** | **~55 GB** (FLUX fp8 + Wan2.2 5B + LoRAs) |
| **Full setup with all options** | **~155 GB** |

Your 200-300GB allocation is adequate for the full recommended setup with room to expand.

**Note on shared models**: The T5 text encoder (`models_t5_umt5-xxl-enc-bf16.pth`, 11.4GB) used by Wan2.x is a **different T5 variant** from the one used by FLUX (`t5xxl_fp16.safetensors`). They cannot be shared. Both must be downloaded separately.

### Coexistence with Ollama

The layout `/mnt/models/ollama/` (Ollama) and `/mnt/models/comfyui/` (ComfyUI) on the same 2TB drive at `/mnt/models` is clean and correct. No conflicts.

---

## 8. Recommended Setup Summary

### Phase 1: Image Generation (Start Here)

1. Create storage directories
2. Run ComfyUI Docker container (command in Section 1)
3. Install ComfyUI Manager via `docker exec` (Section 6)
4. Download FLUX.1 Dev fp8: `flux1-dev-fp8.safetensors` (17.2 GB) to `/mnt/models/comfyui/checkpoints/`
5. Load official FLUX workflow from <a href="https://comfyanonymous.github.io/ComfyUI_examples/flux/" target="_blank">ComfyUI FLUX examples</a>
6. Test image generation at `http://<server-ip>:8188`
7. Configure Open WebUI integration (Section 4) using the API-format workflow JSON

### Phase 2: Style LoRAs

1. Browse <a href="https://huggingface.co/models?other=lora&pipeline_tag=text-to-image" target="_blank">Hugging Face LoRA models</a> for Pixar/animation styles targeting FLUX
2. Download selected LoRAs to `/mnt/models/comfyui/loras/`
3. Use FLUX + LoRA workflow from ComfyUI examples or OpenArt

### Phase 3: Video Generation

1. Via ComfyUI Manager, install `ComfyUI-WanVideoWrapper`
2. Download Wan2.2-TI2V-5B model files (34.3 GB total) to `/mnt/models/comfyui/video_models/wan2.2_ti2v_5b/`
3. Load Wan2.2 workflow from official examples or ComfyWorkflows
4. Test with short 2-3 second clips first to verify VRAM headroom

---

## Uncertainty Flags

The following items could not be fully verified and should be independently confirmed:

1. **Wan2.2 ComfyUI-repackaged version**: At research time, Comfy-Org had not confirmed a repackaged single-file Wan2.2 model. Check <a href="https://huggingface.co/Comfy-Org" target="_blank">huggingface.co/Comfy-Org</a> — this may have been published since this research was conducted.

2. **Wan2.2 in ComfyUI-WanVideoWrapper**: The wrapper was confirmed to support Wan2.2's WanAnimate module. Full Wan2.2 support across all variants (T2V, I2V, TI2V) should be verified in the repository README before downloading models.

3. **Generation time for Wan2.2-TI2V-5B on RTX 3090**: The stated "<9 minutes for 5-second 720P clip" is from Wan2.2 documentation targeting an RTX 4090. The RTX 3090 has similar memory (24GB) but somewhat lower compute throughput — actual times on a 3090 may be 12-18 minutes. This is unconfirmed.

4. **Open WebUI Workflow JSON format**: The workflow JSON required by Open WebUI is ComfyUI's API format. The exact node mapping configuration (`COMFYUI_WORKFLOW_NODES`) requires hands-on testing as the correct values depend on the specific workflow used. The Open WebUI documentation for this feature was inaccessible at time of research (404 errors on the docs site).

5. **CivitAI LoRA access from UK**: CivitAI has been blocked in the UK since July 2025 (Online Safety Act). All LoRA sourcing must use Hugging Face or direct model repositories. Some FLUX LoRAs available on CivitAI may not yet be mirrored on Hugging Face.

6. **CUDA 12.8 compatibility**: The `cu128-slim` image uses CUDA 12.8. Verify your installed NVIDIA driver version supports CUDA 12.8 (requires driver >= 520.61.05 on Linux). Run `nvidia-smi` and check the "CUDA Version" displayed.

---

## Key References

| Resource | URL |
|----------|-----|
| ComfyUI GitHub | <a href="https://github.com/comfyanonymous/ComfyUI" target="_blank">github.com/comfyanonymous/ComfyUI</a> |
| ComfyUI Official Examples | <a href="https://comfyanonymous.github.io/ComfyUI_examples/" target="_blank">comfyanonymous.github.io/ComfyUI_examples/</a> |
| YanWenKun ComfyUI Docker | <a href="https://github.com/YanWenKun/ComfyUI-Docker" target="_blank">github.com/YanWenKun/ComfyUI-Docker</a> |
| ComfyUI Manager | <a href="https://github.com/ltdrdata/ComfyUI-Manager" target="_blank">github.com/ltdrdata/ComfyUI-Manager</a> |
| ComfyUI WanVideoWrapper | <a href="https://github.com/kijai/ComfyUI-WanVideoWrapper" target="_blank">github.com/kijai/ComfyUI-WanVideoWrapper</a> |
| FLUX.1 Dev (Comfy-Org fp8) | <a href="https://huggingface.co/Comfy-Org/flux1-dev" target="_blank">huggingface.co/Comfy-Org/flux1-dev</a> |
| Wan2.2-TI2V-5B | <a href="https://huggingface.co/Wan-AI/Wan2.2-TI2V-5B" target="_blank">huggingface.co/Wan-AI/Wan2.2-TI2V-5B</a> |
| Wan2.2-T2V-A14B | <a href="https://huggingface.co/Wan-AI/Wan2.2-T2V-A14B" target="_blank">huggingface.co/Wan-AI/Wan2.2-T2V-A14B</a> |
| Wan2.1-T2V-14B | <a href="https://huggingface.co/Wan-AI/Wan2.1-T2V-14B" target="_blank">huggingface.co/Wan-AI/Wan2.1-T2V-14B</a> |
| Comfy-Org Models | <a href="https://huggingface.co/Comfy-Org" target="_blank">huggingface.co/Comfy-Org</a> |
| OpenArt Workflows | <a href="https://openart.ai/workflows/home" target="_blank">openart.ai/workflows/home</a> |
| ComfyWorkflows | <a href="https://comfyworkflows.com" target="_blank">comfyworkflows.com</a> |
| Open WebUI GitHub | <a href="https://github.com/open-webui/open-webui" target="_blank">github.com/open-webui/open-webui</a> |
| Z-Image-Turbo (alternative) | <a href="https://huggingface.co/Tongyi-MAI/Z-Image-Turbo" target="_blank">huggingface.co/Tongyi-MAI/Z-Image-Turbo</a> |
| SD 3.5 Large (alternative) | <a href="https://huggingface.co/stabilityai/stable-diffusion-3.5-large" target="_blank">huggingface.co/stabilityai/stable-diffusion-3.5-large</a> |
