# ComfyUI Administration Guide

**System**: Dual RTX 3090 (48GB VRAM), Ubuntu 24.04 LTS Server
**Instances**: Port 8188 (yours) | Port 8189 (Amelia's)
**Models storage**: `/mnt/models/comfyui/`
**Amelia's models**: `/mnt/models/comfyui-amelia/`

---

## Table of Contents

1. [Installing New Models](#1-installing-new-models)
2. [Enabling Models for Amelia](#2-enabling-models-for-amelia)
3. [Getting Started with Nodes](#3-getting-started-with-nodes)

---

## 1. Installing New Models

### Where models live

ComfyUI organises models into subdirectories. Download each file to the correct folder:

| Folder | What goes here | Example |
|--------|---------------|---------|
| `checkpoints/` | Main image generation models (all-in-one files) | `flux1-dev-fp8.safetensors` |
| `loras/` | Style adapters — small files that modify a base model | `Canopus-Pixar-3D-FluxDev-LoRA.safetensors` |
| `diffusion_models/` | Video generation models (Wan2.x) | `wan2.2_ti2v_5B_fp16.safetensors` |
| `vae/` | VAE files (image encoder/decoder, usually separate for video models) | `wan2.2_vae.safetensors` |
| `text_encoders/` | Text encoder files (usually separate for video models) | `umt5_xxl_fp8_e4m3fn_scaled.safetensors` |
| `controlnet/` | ControlNet conditioning models | — |
| `upscale_models/` | Image upscaling models | — |

### Downloading a model

SSH into the server and use `wget`. General pattern:

```bash
wget -O /mnt/models/comfyui/<folder>/<filename> "<huggingface-url>"
```

**Example — downloading a LoRA:**

```bash
wget -O /mnt/models/comfyui/loras/my-new-lora.safetensors \
  "https://huggingface.co/some-author/some-model/resolve/main/model.safetensors"
```

**No restart needed.** ComfyUI detects new files automatically. After downloading, refresh the browser and the model will appear in the relevant dropdown.

### Finding models on Hugging Face

All models should be sourced from Hugging Face — CivitAI is blocked in the UK (Online Safety Act).

- LoRAs for FLUX: <a href="https://huggingface.co/models?other=lora&pipeline_tag=text-to-image&search=flux" target="_blank">huggingface.co — FLUX LoRA search</a>
- Image models: <a href="https://huggingface.co/models?pipeline_tag=text-to-image&library=diffusers" target="_blank">huggingface.co — text-to-image models</a>
- Comfy-Org repackaged models (pre-formatted for ComfyUI): <a href="https://huggingface.co/Comfy-Org" target="_blank">huggingface.co/Comfy-Org</a>

The Hugging Face download URL format is always:
`https://huggingface.co/<author>/<repo>/resolve/main/<filename>`

---

## 2. Enabling Models for Amelia

Amelia's instance only sees models in `/mnt/models/comfyui-amelia/`. To make a model available to her, create a **hard link** — this is a pointer to the same file with no extra disk space used.

### Adding a model to Amelia's instance

```bash
sudo ln /mnt/models/comfyui/<folder>/<filename> \
        /mnt/models/comfyui-amelia/<folder>/<filename>
```

**Example — sharing a new LoRA with Amelia:**

```bash
sudo ln /mnt/models/comfyui/loras/new-style-lora.safetensors \
        /mnt/models/comfyui-amelia/loras/new-style-lora.safetensors
```

**No restart needed.** Amelia's ComfyUI will detect it immediately on next browser refresh.

### Removing a model from Amelia's instance

Deleting the hard link removes it from her view without affecting your copy:

```bash
sudo rm /mnt/models/comfyui-amelia/loras/some-lora.safetensors
```

### Currently available to Amelia

| Model | Type | Location |
|-------|------|----------|
| `flux1-dev-fp8.safetensors` | Image generation (includes VAE + text encoders) | `checkpoints/` |
| `Canopus-Pixar-3D-FluxDev-LoRA.safetensors` | Pixar 3D style LoRA | `loras/` |

---

## 3. Getting Started with Nodes

### What are nodes?

ComfyUI uses a **node graph** — a visual way of wiring together the steps of image/video generation. Each box on the canvas is a node that does one specific job. You connect them by dragging wires from outputs (right side of a node) to inputs (left side). The graph runs left to right, ending at a Save Image node.

A basic text-to-image workflow looks like this:

```
Load Checkpoint → (model) → Load LoRA → (model) → KSampler → VAE Decode → Save Image
                                                        ↑
CLIP Text Encode (positive prompt) ────────────────────┘
CLIP Text Encode (negative prompt) ────────────────────┘
Empty Latent Image ─────────────────────────────────────┘
```

### Common nodes and their settings

---

#### Load Checkpoint
Loads the main AI model. For image generation this will be your FLUX model.

| Setting | What it does | Typical value |
|---------|-------------|---------------|
| `ckpt_name` | Which model file to load | `flux1-dev-fp8.safetensors` |

**Outputs**: MODEL, CLIP, VAE

---

#### Load LoRA
Applies a style adapter on top of the base model. Inserted between Load Checkpoint and KSampler.

| Setting | What it does | Typical value |
|---------|-------------|---------------|
| `lora_name` | Which LoRA file to use | `Canopus-Pixar-3D-FluxDev-LoRA` |
| `strength_model` | How strongly the LoRA affects the image model | `0.8` (range: 0.0–1.0) |
| `strength_clip` | How strongly the LoRA affects text understanding | `0.8` (range: 0.0–1.0) |

Lower strength = subtle influence. Higher strength = strong style, may lose fine detail. Start at 0.8 and adjust.

---

#### CLIP Text Encode
Converts your text prompt into something the model understands. You typically have two — one for what you *want* and one for what you *don't want*.

| Setting | What it does | Typical value |
|---------|-------------|---------------|
| `text` | Your prompt | `"Pixar 3D style, a woman with curly blonde hair..."` |

**Trigger words**: Some LoRAs require a specific word in your prompt to activate their style (e.g. `Pixar` or `Pixar 3D` for the Canopus LoRA).

---

#### Empty Latent Image
Sets the output image dimensions. The model works in "latent space" (a compressed representation) and this node defines the canvas size.

| Setting | What it does | Typical value |
|---------|-------------|---------------|
| `width` | Image width in pixels | `1024` |
| `height` | Image height in pixels | `1024` |
| `batch_size` | How many images to generate at once | `1` |

**Common resolutions for FLUX**: 1024×1024 (square), 1152×896 (landscape), 896×1152 (portrait)

---

#### KSampler
The core generation node — this is where the actual AI processing happens. Has the most settings.

| Setting | What it does | Typical value for FLUX |
|---------|-------------|----------------------|
| `seed` | Random seed — same seed + same prompt = same image | Any number, or randomise |
| `steps` | How many refinement passes to run — more = better quality but slower | `20` |
| `cfg` | How closely to follow the prompt — **FLUX uses 1.0, not the usual 7.5** | `1.0` |
| `sampler_name` | The sampling algorithm | `euler` |
| `scheduler` | Controls how noise is reduced across steps | `simple` |
| `denoise` | For text-to-image (starting from scratch) always use 1.0 | `1.0` |

> **Important for FLUX**: CFG should be `1.0`. FLUX handles prompt guidance differently from older models — setting it higher will produce washed-out or distorted images.

---

#### FluxGuidance
A FLUX-specific node that replaces the role of CFG from older models. Usually pre-wired in FLUX workflows.

| Setting | What it does | Typical value |
|---------|-------------|---------------|
| `guidance` | How strongly to follow the prompt | `3.5` |

Range: 1.0–10.0. Higher = more literal interpretation of your prompt. 3.5 is a good default.

---

#### VAE Decode
Converts the model's internal latent representation into an actual image. No settings to adjust — just ensure it's connected.

---

#### Save Image
Saves the generated image to the output folder.

| Setting | What it does | Typical value |
|---------|-------------|---------------|
| `filename_prefix` | Prefix added to the output filename | `ComfyUI` |

Images are saved to `/opt/comfyui/output/` (yours) or `/opt/comfyui-amelia/output/` (Amelia's).

---

### Adding a node to the canvas

**Right-click** on any empty area of the canvas → navigate the menu to find the node type → click to add it.

### Saving and loading workflows

- **Save**: Menu (top left) → **Save** — saves as a `.json` file
- **Load**: Menu → **Load**, or drag and drop a `.json` file onto the canvas
- **Tip**: Generated images have the workflow embedded in them — drag an output image onto the canvas to reload the exact workflow that produced it

### Workflow resources

- Official example workflows: <a href="https://comfyanonymous.github.io/ComfyUI_examples/" target="_blank">comfyanonymous.github.io/ComfyUI_examples/</a>
- Community workflows: <a href="https://openart.ai/workflows/home" target="_blank">openart.ai/workflows/home</a>
- More community workflows: <a href="https://comfyworkflows.com" target="_blank">comfyworkflows.com</a>
