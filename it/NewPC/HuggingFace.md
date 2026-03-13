# HuggingFace Model Downloads & ComfyUI Setup

A practical guide to finding, downloading, and using HuggingFace models in ComfyUI.

---

## Understanding HuggingFace Model Pages

Every model page has two key sections:

- **Model card** (default view) — description, usage, and Python code examples. The Python script shown is for using the model programmatically — **you do not need to run it to download files**.
- **Files tab** (`/tree/main`) — the actual files available for download. Always check this tab to understand what you are getting before downloading.

---

## Two Types of Models

### Type 1: Single-file ("all-in-one")

One `.safetensors` file contains everything (VAE, text encoders, diffusion model). Drop it in the correct ComfyUI directory and load it directly.

**Example**: `flux1-dev-fp8.safetensors` — the entire FLUX.1 Dev model in one file.

| Pros | Cons |
|---|---|
| Simple — one file | Less flexible |
| Easy to manage | Can be very large |
| Immediate ComfyUI support | No component reuse |

### Type 2: Multi-component ("diffusers format")

The model is split into separate directories, each containing its own files:

| Directory | Purpose |
|---|---|
| `transformer/` | The main diffusion model (the "brain") |
| `vae/` | Encodes/decodes images to and from latent space |
| `text_encoder/` | Encodes your text prompt into vectors the model understands |
| `tokenizer/` | Converts text into tokens before encoding |
| `scheduler/` | Config defining the sampling/denoising schedule |
| `processor/` | Config for preprocessing inputs |

**Example**: Qwen-Image-Edit, Stable Diffusion XL, most newer models.

| Pros | Cons |
|---|---|
| Components can be mixed and matched | Multiple files to manage |
| Swap VAE or text encoder independently | Requires understanding of what goes where |
| Quantized variants possible per-component | Slightly more complex ComfyUI setup |

---

## How to Download Models

### Method 1: wget (simplest — no Python needed)

Every file on HuggingFace has a direct URL in this format:
`https://huggingface.co/<author>/<model>/resolve/main/<path/to/file>`

```bash
HF_TOKEN="hf_your_token_here"

wget -c --header="Authorization: Bearer ${HF_TOKEN}" \
  "https://huggingface.co/Comfy-Org/Qwen-Image-Edit_ComfyUI/resolve/main/split_files/diffusion_models/qwen_image_edit_2511_fp8mixed.safetensors" \
  -O /mnt/models/comfyui/diffusion_models/qwen_image_edit_2511_fp8mixed.safetensors
```

- `-c` resumes an interrupted download
- `--header="Authorization: Bearer ${HF_TOKEN}"` is required — HuggingFace blocks unauthenticated downloads
- Set `HF_TOKEN` with single quotes if your token contains special characters: `HF_TOKEN='hf_abc...'`

Good for: downloading specific known files directly to the right place. No tools to install.

### Method 2: Browser download (for individual files)

1. Go to the model's **Files** tab: `https://huggingface.co/<author>/<model>/tree/main`
2. Browse into directories and click individual files
3. Click the **download icon** (arrow) next to the filename

Good for: grabbing one or two files when you're not on the server. Tedious for large multi-component models.

### Method 3: huggingface-cli (best for downloading whole repos)

Downloads an entire repository in one command. On Ubuntu servers with Python 3.12+, use `pipx` to avoid the "externally managed environment" error:

```bash
# Install pipx if not already present
sudo apt install pipx
pipx ensurepath

# Install huggingface-cli via pipx (one-time)
pipx install huggingface_hub

# Download specific files from a repo
huggingface-cli download Comfy-Org/Qwen-Image-Edit_ComfyUI \
  split_files/diffusion_models/qwen_image_edit_2511_fp8mixed.safetensors \
  --local-dir /mnt/models/comfyui \
  --local-dir-use-symlinks False

# Download a whole model repo
huggingface-cli download Qwen/Qwen-Image-Edit --local-dir ./qwen-image-edit
```

> **Note on `--local-dir-use-symlinks False`**: Without this flag, huggingface-cli creates a cache in `~/.cache/huggingface/` and symlinks to it. On a server with limited home directory space and a large `/mnt/models/` mount, you want the actual file written directly to the target path — this flag does that.

> **Why not `pip install`?**: Ubuntu 23.04+ marks the system Python as "externally managed", blocking `pip install` to protect system packages. Use `pipx` (for CLI tools) or a virtual environment instead. You can override with `pip install --break-system-packages huggingface_hub` but this is not recommended.

### Method 4: git-lfs (clones the full repository)

```bash
sudo apt install git-lfs
git lfs install
git clone https://huggingface.co/Qwen/Qwen-Image-Edit
```

Downloads everything including config files, READMEs, and all model weights. Only use this when you want the entire repo.

---

## Where Files Go in ComfyUI

For multi-component models, files map to specific ComfyUI directories:

| Component type | ComfyUI directory |
|---|---|
| Single-file checkpoint (FLUX, SD, SDXL) | `models/checkpoints/` |
| Diffusion model / transformer only | `models/diffusion_models/` |
| VAE | `models/vae/` |
| Text encoder (CLIP, T5, Qwen, etc.) | `models/text_encoders/` |
| LoRA adapters | `models/loras/` |
| ControlNet | `models/controlnet/` |
| Upscale models | `models/upscale_models/` |

On this system, the ComfyUI model root is: `/mnt/models/comfyui/`

---

## Case Study: Qwen-Image-Edit

A practical example of a multi-component model and how to use it in ComfyUI.

### What it is

Qwen-Image-Edit is a 20B image editing model from Alibaba. Given an input image and a text instruction, it edits the image accordingly (e.g. "change the background to a forest", "make the car red", "remove the person on the left").

| Property | Value |
|---|---|
| Publisher | Alibaba / Qwen team |
| Licence | Apache 2.0 |
| HuggingFace ID | `Qwen/Qwen-Image-Edit` |
| Raw model size | 57.7 GB (BF16) |
| fp8 quantised size | ~16–20 GB |
| VRAM required (fp8) | ~16 GB |
| Framework | Diffusers (`QwenImageEditPipeline`) |
| ComfyUI native support | Yes, but with a known bug — use custom node |

**SFW note**: The base model from Alibaba is the standard SFW model. There is no separate SFW/NSFW split — this is just the official release.

### Model components (raw HuggingFace repo)

The full `Qwen/Qwen-Image-Edit` repo is 57.7 GB. Only some folders contain actual weights:

| Directory | Type | Size | ComfyUI directory |
|---|---|---|---|
| `transformer/` | **Weights** (9 × ~5 GB files) | ~40.9 GB | `diffusion_models/` |
| `text_encoder/` | **Weights** (Qwen2.5-VL 7B LLM — not CLIP or T5) | ~16.6 GB | `text_encoders/` |
| `vae/` | **Weights** | ~254 MB | `vae/` |
| `processor/` | Config only | <1 MB | **Not needed in ComfyUI** |
| `scheduler/` | Config only | <1 MB | **Not needed in ComfyUI** |
| `tokenizer/` | Config/vocab only | <50 MB | **Not needed in ComfyUI** |

The `processor/`, `scheduler/`, and `tokenizer/` folders are only used when running the Python diffusers pipeline directly. ComfyUI handles all of that internally — ignore them.

### What "Lightning" variants are

Searching HuggingFace for fp8 versions returns many results labelled "Lightning". These are **step-distilled** variants — a LoRA that reduces inference from ~40 steps to 4 steps (~10x faster). They are **not standalone models**: you still need the full base transformer, and the Lightning LoRA sits on top.

- Lightning = speed over quality trade-off. Fine for experimentation, but start with the full model.
- Lightning files go in `models/loras/`, not `models/diffusion_models/`.

### What to actually download: official Comfy-Org fp8 packages

Do not download the raw 57.7 GB model. Comfy-Org provides official fp8-repackaged single files, identical to how Wan2.2 was packaged. Three files total from two repos:

| File | HuggingFace repo | ComfyUI directory | Size |
|---|---|---|---|
| `qwen_image_edit_2511_fp8mixed.safetensors` | `Comfy-Org/Qwen-Image-Edit_ComfyUI` | `diffusion_models/` | ~20.5 GB |
| `qwen_2.5_vl_7b_fp8_scaled.safetensors` | `Comfy-Org/Qwen-Image_ComfyUI` | `text_encoders/` | ~7 GB |
| `qwen_image_vae.safetensors` | `Comfy-Org/Qwen-Image_ComfyUI` | `vae/` | ~254 MB |

**Total: ~28 GB** — exceeds a single RTX 3090 (24 GB), so ComfyUI will automatically offload layers to RAM. This works but is slower than fitting fully in VRAM. On your dual-3090 system this is not an issue as ComfyUI can use both GPUs.

Note: `2511` = November 2025 release (newer and better than `2509`). The text encoder and VAE come from the `Qwen-Image_ComfyUI` repo because they are shared with the Qwen image generation model.

### Download commands (on the server)

HuggingFace requires authentication for downloads. Set your token as a variable first so it isn't repeated in every command (and isn't stored in shell history):

```bash
HF_TOKEN="hf_your_token_here"
```

Then download each file:

```bash
# Transformer (main model) — ~20.5 GB
wget -c --header="Authorization: Bearer ${HF_TOKEN}" \
  "https://huggingface.co/Comfy-Org/Qwen-Image-Edit_ComfyUI/resolve/main/split_files/diffusion_models/qwen_image_edit_2511_fp8mixed.safetensors" \
  -O /mnt/models/comfyui/diffusion_models/qwen_image_edit_2511_fp8mixed.safetensors

# Text encoder — ~7 GB
wget -c --header="Authorization: Bearer ${HF_TOKEN}" \
  "https://huggingface.co/Comfy-Org/Qwen-Image_ComfyUI/resolve/main/split_files/text_encoders/qwen_2.5_vl_7b_fp8_scaled.safetensors" \
  -O /mnt/models/comfyui/text_encoders/qwen_2.5_vl_7b_fp8_scaled.safetensors

# VAE — ~254 MB
wget -c --header="Authorization: Bearer ${HF_TOKEN}" \
  "https://huggingface.co/Comfy-Org/Qwen-Image_ComfyUI/resolve/main/split_files/vae/qwen_image_vae.safetensors" \
  -O /mnt/models/comfyui/vae/qwen_image_vae.safetensors
```

Your HuggingFace token is at <a href="https://huggingface.co/settings/tokens" target="_blank">huggingface.co/settings/tokens</a>. A read-only token is sufficient for downloads.

### Required custom node

The built-in ComfyUI `TextEncodeQwenImageEdit` node has a known offset/scaling bug. Install this community fix instead:

**`lenML/comfyui_qwen_image_edit_adv`**

Install via ComfyUI Manager, or manually:

```bash
cd /opt/comfyui/storage/ComfyUI/custom_nodes
git clone https://github.com/lenML/comfyui_qwen_image_edit_adv
```

Nodes provided:

| Node | Purpose |
|---|---|
| `TextEncodeQwenImageEditAdv` | Single image — fixes the offset bug |
| `TextEncodeQwenImageEditPlusAdv` | Up to 3 reference images |
| `TextEncodeQwenImageEditInfAdv` | Unlimited/batched reference images |
| `QwenImageEditSimpleScale` | Resize input to ~1024px / 1MP (recommended) |
| `QwenImageEditScale` | Advanced scaling with aspect ratio control |

### ComfyUI workflow structure

```
Load Diffusion Model ──→ TextEncodeQwenImageEditAdv ──→ KSampler ──→ VAE Decode ──→ Save Image
Load CLIP ─────────────↗
Load VAE ──────────────────────────────────────────────────────────↗
Input Image ───────────↗
```

### Key settings

- **Always scale input image** to ~1024px / 1MP before the encode node — use `QwenImageEditSimpleScale`. The model was trained on this resolution and degrades at other sizes.
- **CFG**: Keep low — 1.0 to 2.5. Higher values cause over-saturation and artefacts.
- **Steps**: 50 is a good starting point.
- **RTX 3090** (24GB VRAM): The fp8 model is ~28GB total, so it will offload some layers to RAM on a single GPU. On the dual-3090 system ComfyUI can distribute across both GPUs.

---

## Finding Models on HuggingFace

### Search tips

- Use the **Models** tab at huggingface.co/models
- Filter by **Library** (Diffusers, Transformers, GGUF, etc.)
- Filter by **Task** (Text-to-Image, Image-to-Image, etc.)
- Search for `<model name> ComfyUI` or `<model name> fp8` to find community-repackaged versions
- Check the **Community** tab on a model page for forks, quantisations, and ComfyUI-ready versions

### Quantisation formats explained

| Format | What it means | VRAM savings |
|---|---|---|
| BF16 / FP16 | Full precision (original) | None — baseline |
| FP8 | 8-bit float quantisation | ~50% vs BF16 |
| INT8 | 8-bit integer quantisation | ~50% vs BF16 |
| GGUF | Flexible quantisation (Q4, Q5, Q8 variants) | 50–75% vs BF16 |

Lower precision = smaller file, less VRAM, slight quality reduction. FP8 is generally the best balance for image models. GGUF is most common for LLMs.
