# Multi-File Models: The HuggingFace Diffusers Format

**Hardware**: Dual RTX 3090 (48GB VRAM, NVLink) · AMD Ryzen 9 7900X · 64GB DDR5 · Ubuntu 24.04 LTS
**Last Updated**: March 2026

---

## Overview

When you visit `Qwen/Qwen-Image-Edit-2511` on HuggingFace and see folders rather than a single model file, you are looking at a model stored in the **HuggingFace Diffusers format** — a standardised multi-component directory structure used by most large modern image and video generation models.

This guide explains:
- What this format is and why it exists
- The exact structure of Qwen-Image-Edit-2511
- **Three different ways** to use this model in ComfyUI (including the one you are already doing)
- How to download the files you need for training
- How DiffSynth-Studio uses these files during training
- The general pattern, so you can recognise it in other models (FLUX, SD3, Wan2.x, etc.)

---

## Glossary

| Term | Meaning |
|------|---------|
| **Diffusers format** | The standard multi-folder model structure used by HuggingFace's `diffusers` library. Each component of a model (the transformer, the text encoder, the VAE) lives in its own subdirectory with its own config files and weight files |
| **Transformer / UNet** | The core diffusion model — the component that actually generates or edits images. The largest part of any image generation model |
| **VAE (Variational Autoencoder)** | Converts between pixel space and the compressed latent space the transformer works in. A relatively small component (~250 MB) shared across many models |
| **Text encoder** | Converts your text prompt into a numerical representation the transformer can understand. In Qwen-Image-Edit, this is replaced by the Qwen2.5-VL vision-language model (~16 GB), which also understands images |
| **Sharded weights** | When a model component is too large for a single file, it is split into numbered chunks: `model-00001-of-00005.safetensors`, `model-00002-of-00005.safetensors`, etc. They are loaded and joined in order |
| **`model_index.json`** | The root registry file in a diffusers model folder. It lists every sub-component and tells the `diffusers` library what Python class to use to load each one |
| **HF cache** | HuggingFace's local download cache, stored at `~/.cache/huggingface/hub/`. Files are stored in hashed subdirectories but can be accessed by model ID. Training tools like DiffSynth-Studio use this automatically |
| **`--local-dir`** | A `huggingface-cli` flag that saves downloaded files in a clean human-readable folder structure instead of the hashed cache |

---

## Installing huggingface-cli (Ubuntu 24.04)

Ubuntu 24.04 uses PEP 668 to prevent `pip install` system-wide. The solution is to create a Python virtual environment, which is an isolated folder containing its own Python and pip. Activating it puts its `bin/` directory on your PATH so `huggingface-cli` becomes available.

```bash
# Create a virtual environment (one-time setup)
python3 -m venv /home/steve/hf-env

# Activate it
source /home/steve/hf-env/bin/activate

# Install huggingface_hub
pip install huggingface_hub

# Verify it works — note: in huggingface_hub 1.x the command is "hf", not "huggingface-cli"
hf --version
```

You need to activate the venv each time you open a new terminal before running `huggingface-cli`:

```bash
source /home/steve/hf-env/bin/activate
```

To avoid doing this manually every session, add it to your `~/.bashrc`:

```bash
echo 'source /home/steve/hf-env/bin/activate' >> ~/.bashrc
source ~/.bashrc
```

**Do not use `sudo`** with `hf download`. Downloads go to `/tmp` or your home directory — both are user-writable without root. `sudo` also uses a restricted PATH that cannot find venv-installed commands, which causes a separate "command not found" error.

---

## Why Models Are Structured This Way

Large models cannot easily be stored as a single file because:

1. **File size limits**: HuggingFace's infrastructure limits individual file uploads. A 40 GB transformer is split into ~10 GB shards as standard practice.

2. **Component reuse**: The VAE and text encoder are often shared between related models. For example, Qwen-Image-Edit-2511 uses the **same VAE and text encoder as the base `Qwen/Qwen-Image` model** — only the transformer weights differ. Storing them separately avoids duplicating 16 GB of text encoder weights for every model variant.

3. **Efficient loading**: The `diffusers` library can load individual shards directly onto specific GPUs without staging the full 57 GB through CPU RAM first. The index JSON (`diffusion_pytorch_model.safetensors.index.json`) acts as a map telling the library exactly which tensor lives in which shard.

4. **Framework portability**: Training tools, inference pipelines, and deployment platforms all understand the diffusers format. A model stored this way can be used by any tool in the ecosystem without conversion.

---

## Qwen-Image-Edit-2511: Complete File Structure

Total repository size: **57.7 GB**

```
Qwen/Qwen-Image-Edit-2511/
│
├── model_index.json             (516 B)  — pipeline registry
│
├── transformer/                 ~40.9 GB  — the diffusion transformer (core model)
│   ├── config.json              (362 B)   — architecture config
│   ├── diffusion_pytorch_model-00001-of-00005.safetensors  (9.97 GB)
│   ├── diffusion_pytorch_model-00002-of-00005.safetensors  (9.99 GB)
│   ├── diffusion_pytorch_model-00003-of-00005.safetensors  (9.99 GB)
│   ├── diffusion_pytorch_model-00004-of-00005.safetensors  (9.93 GB)
│   ├── diffusion_pytorch_model-00005-of-00005.safetensors  (982 MB)
│   └── diffusion_pytorch_model.safetensors.index.json      (199 kB) — shard map
│
├── text_encoder/                ~16.6 GB  — Qwen2.5-VL-7B (NOT in this repo — shared from Qwen/Qwen-Image)
├── vae/                         ~254 MB   — (NOT in this repo — shared from Qwen/Qwen-Image)
│
├── scheduler/
│   └── scheduler_config.json    (485 B)
│
├── tokenizer/                   ~5 MB    — text tokenizer
│   ├── vocab.json, merges.txt, tokenizer_config.json, etc.
│
└── processor/                   ~16 MB   — vision/image preprocessor
    ├── preprocessor_config.json, tokenizer.json, etc.
```

> **Important**: The `transformer/` folder is the only large component in the Edit-2511 repo. The `text_encoder/` and `vae/` come from the **base model** `Qwen/Qwen-Image` (a separate HuggingFace repo), not from this one. This is why you only see the transformer shards when browsing the Edit-2511 repo.

The base model `Qwen/Qwen-Image` adds:

```
Qwen/Qwen-Image/
├── text_encoder/        ~16.6 GB  — Qwen2.5-VL-7B (4 shards × ~4 GB)
├── vae/                 ~254 MB   — shared VAE
├── tokenizer/           ~5 MB
└── processor/           ~16 MB
```

---

## Three Ways to Use This Model in ComfyUI

### Option A — Phr00t Rapid-AIO (single-file, fast inference)

The `Qwen-Rapid-AIO-v1.safetensors` available from <a href="https://huggingface.co/Phr00t/Qwen-Image-Edit-Rapid-AIO/tree/main/v23" target="_blank">Phr00t/Qwen-Image-Edit-Rapid-AIO (v23)</a> is a single-file merge of the transformer, VAE, text encoder, and Lightning acceleration LoRAs. It loads through ComfyUI's standard `CheckpointLoaderSimple` node and works with the `TextEncodeQwenImageEditPlus` custom nodes.

Download the file and place it in your ComfyUI `checkpoints/` directory:

```bash
source /home/steve/hf-env/bin/activate
hf download Phr00t/Qwen-Image-Edit-Rapid-AIO \
    --include "v23/Qwen-Rapid-AIO-v1.safetensors" \
    --local-dir /tmp/qwen-rapid

mv /tmp/qwen-rapid/v23/Qwen-Rapid-AIO-v1.safetensors \
   /mnt/models/comfyui/checkpoints/Qwen/
```

| Pros | Cons |
|------|------|
| Single file, simple workflow | Lightning LoRAs permanently baked in (4–8 steps only) |
| Fast inference (4–8 steps) | FP8 compression may cause grid artifacts with external LoRAs |
| Smallest download of the three options | Not suitable as a training base |

---

### Option B — Comfy-Org Pre-Extracted Files (native ComfyUI)

<a href="https://huggingface.co/Comfy-Org/Qwen-Image_ComfyUI" target="_blank">Comfy-Org/Qwen-Image_ComfyUI</a> provides pre-extracted, ComfyUI-ready single-file versions of each component. These are the files the ComfyUI developers officially recommend.

> **Note**: ComfyUI's maintainer explicitly does not support loading raw multi-file diffusers models directly — this is the intended solution instead.

**Files to download:**

| File | Size | ComfyUI Directory |
|------|------|-------------------|
| `qwen_image_2512_fp8_e4m3fn.safetensors` | ~20 GB | `models/diffusion_models/` |
| `qwen_2.5_vl_7b_fp8_scaled.safetensors` | ~8 GB | `models/text_encoders/` |
| `qwen_image_vae.safetensors` | ~254 MB | `models/vae/` |

**Download commands:**

```bash
# Activate the venv first
source /home/steve/hf-env/bin/activate

# Download only the specific files needed (the Comfy-Org repo is 273 GB total)

# Diffusion model (transformer)
hf download Comfy-Org/Qwen-Image_ComfyUI \
    --include "split_files/diffusion_models/qwen_image_2512_fp8_e4m3fn.safetensors" \
    --local-dir /tmp/qwen-comfy

# Text encoder
hf download Comfy-Org/Qwen-Image_ComfyUI \
    --include "split_files/text_encoders/qwen_2.5_vl_7b_fp8_scaled.safetensors" \
    --local-dir /tmp/qwen-comfy

# VAE
hf download Comfy-Org/Qwen-Image_ComfyUI \
    --include "split_files/vae/qwen_image_vae.safetensors" \
    --local-dir /tmp/qwen-comfy

# Move files to ComfyUI model directories
mv /tmp/qwen-comfy/split_files/diffusion_models/qwen_image_2512_fp8_e4m3fn.safetensors \
   /mnt/models/comfyui/diffusion_models/

mv /tmp/qwen-comfy/split_files/text_encoders/qwen_2.5_vl_7b_fp8_scaled.safetensors \
   /mnt/models/comfyui/text_encoders/

mv /tmp/qwen-comfy/split_files/vae/qwen_image_vae.safetensors \
   /mnt/models/comfyui/vae/
```

**ComfyUI workflow (different nodes from the Rapid-AIO workflow):**

This approach uses three separate loader nodes rather than a single `CheckpointLoaderSimple`:

```
Load Diffusion Model  ←── qwen_image_2512_fp8_e4m3fn.safetensors
         |
         |           Load CLIP  ←── qwen_2.5_vl_7b_fp8_scaled.safetensors
         |                |
         |                |     Load VAE  ←── qwen_image_vae.safetensors
         |                |         |
         └────────────────┼─────────┘
                          |
              TextEncodeQwenImageEditPlus (positive + negative)
                          |
                     KSampler
                          |
                      VAEDecode
```

Reference workflow: <a href="https://docs.comfy.org/tutorials/image/qwen/qwen-image-edit-2511" target="_blank">docs.comfy.org — Qwen-Image-Edit-2511 native workflow</a>

| Pros | Cons |
|------|------|
| Clean, unmodified model weights | ~28 GB download (FP8 variant) |
| FP8 transformer fits comfortably in VRAM alongside text encoder | Does not include Lightning acceleration (slower, 20+ steps) |
| Compatible with external LoRAs without artifact risk | Requires separate Lightning LoRA if you want fast inference |

---

### Option C — Raw HuggingFace Model (training only, not for ComfyUI)

The raw multi-file model from HuggingFace is used **only for training**. ComfyUI cannot load it directly.

See the [Training section](#downloading-for-training-with-diffsynth-studio) below.

---

## Downloading for Training with DiffSynth-Studio

DiffSynth-Studio can download the required files automatically via HuggingFace's cache system when you specify model IDs in the training script. However, pre-downloading large files over SSH is more reliable — it avoids timeouts during training startup.

**Critical**: Training requires files from **two separate repositories**:

| Component | Source repo | Why |
|-----------|-------------|-----|
| `transformer/` (the model being trained) | `Qwen/Qwen-Image-Edit-2511` | The edit-specific transformer |
| `text_encoder/` | `Qwen/Qwen-Image` | **Base model**, not the Edit-2511 repo |
| `vae/` | `Qwen/Qwen-Image` | **Base model**, not the Edit-2511 repo |
| `tokenizer/` | `Qwen/Qwen-Image` | **Base model**, not the Edit-2511 repo |

> The Edit-2511 repo only contains the transformer. The text encoder and VAE are shared from the base model and must be downloaded separately.

**Download commands:**

```bash
# Activate the venv (create it first if needed — see "Installing huggingface-cli" above)
source /home/steve/hf-env/bin/activate

# Set HF_HOME so large files go to your model drive, not the OS drive
export HF_HOME=/mnt/models/huggingface

# Download transformer from Edit-2511 (the component specific to the edit model)
hf download Qwen/Qwen-Image-Edit-2511 \
    --include "transformer/*" "scheduler/*" "model_index.json" \
    --local-dir /mnt/models/huggingface/qwen-image-edit-2511

# Download text encoder, VAE, and tokenizer from the BASE model
hf download Qwen/Qwen-Image \
    --include "text_encoder/*" "vae/*" "tokenizer/*" "processor/*" \
    --local-dir /mnt/models/huggingface/qwen-image
```

> The transformer download is ~41 GB. The text encoder is ~16.6 GB. Allow time for both. The download is **resumable** — if interrupted, re-running the same command will skip already-downloaded files.

---

## How DiffSynth-Studio Uses These Files

DiffSynth-Studio's training script references each component using `model_id` + `origin_file_pattern` (a glob pattern to match the shard files). It loads from the HuggingFace cache automatically, or from a local directory if `local_model_path` is specified.

In a DiffSynth-Studio training shell script, the model references look like this:

```bash
--model_id_with_origin_paths \
  "Qwen/Qwen-Image-Edit-2511:transformer/diffusion_pytorch_model*.safetensors,\
   Qwen/Qwen-Image:text_encoder/model*.safetensors,\
   Qwen/Qwen-Image:vae/diffusion_pytorch_model.safetensors"
```

The glob pattern `transformer/diffusion_pytorch_model*.safetensors` automatically matches all 5 shards. DiffSynth-Studio handles joining them — you do not need to merge the shards manually.

**If you pre-downloaded to a specific path** (recommended for large models), point DiffSynth-Studio at the local directory using `local_model_path`:

```python
from diffsynth.pipelines.qwen_image import ModelConfig

ModelConfig(
    local_model_path="/mnt/models/huggingface/qwen-image-edit-2511",
    origin_file_pattern="transformer/diffusion_pytorch_model*.safetensors"
)
```

The official training script has a `--model_path` argument for this. Check the script's `--help` output or the DiffSynth-Studio documentation for the exact flag name in the version you download.

---

## The General Pattern

This directory structure is used across the entire modern diffusers ecosystem. Once you recognise it, you will see it everywhere:

| Model | Total size | Transformer shards | ComfyUI solution |
|-------|------------|--------------------|------------------|
| FLUX.1 Dev | ~24 GB | 2 shards | `Comfy-Org/flux_dev_repackaged` → `flux1-dev.safetensors` (single file) |
| Stable Diffusion 3.5 Large | ~16 GB | 2 shards | `Comfy-Org/stable-diffusion-3.5-fp8` → single `sd3.5_large_fp8.safetensors` |
| Qwen-Image-Edit-2511 | ~57.7 GB | 5 shards | `Comfy-Org/Qwen-Image_ComfyUI` → 3 component files |
| Wan2.2 (video) | ~18 GB (repackaged) | 1 file (repackaged) | `Comfy-Org/Wan_2.2_ComfyUI_Repackaged` → per-component files |
| PixArt-Sigma | ~12 GB | 2 shards | Per-component files or diffusers node |

**The pattern is always:**
1. `model_index.json` at the root — the pipeline descriptor
2. One subdirectory per component: `transformer/` or `unet/`, `text_encoder/`, `vae/`, `scheduler/`
3. Weights as numbered shards + an index JSON
4. ComfyUI cannot use this format natively — use Comfy-Org repackaged files or a single-file export

---

## Decision Guide: Which Download Do You Need?

```
What do you want to do?
│
├─ Use in ComfyUI for inference
│   │
│   ├─ You already have Rapid-AIO → Nothing to download; you are already set up.
│   │                                Use your existing Phr00t workflow.
│   │
│   └─ You want the clean BF16 model without baked Lightning LoRAs
│       → Download from Comfy-Org/Qwen-Image_ComfyUI (3 files, ~50 GB total)
│         Use the Load Diffusion Model / Load CLIP / Load VAE node workflow
│
└─ Train a LoRA
    → Download transformer from Qwen/Qwen-Image-Edit-2511 (~41 GB)
    → Download text_encoder + vae from Qwen/Qwen-Image (~17 GB)
    → Use DiffSynth-Studio with the model IDs pointing to your local paths
    → For inference after training: load your LoRA on top of either the
      Rapid-AIO checkpoint OR the Comfy-Org BF16 files
```

---

## Quick Reference: Download Commands

```bash
# ── Setup (required before any download) ─────────────────────────────────────

source /home/steve/hf-env/bin/activate     # activate the venv
export HF_HOME=/mnt/models/huggingface     # store HF cache on model drive

# ── For training (DiffSynth-Studio) ──────────────────────────────────────────

# Transformer (from Edit-2511 repo)
hf download Qwen/Qwen-Image-Edit-2511 \
    --include "transformer/*" "scheduler/*" "model_index.json" \
    --local-dir /mnt/models/huggingface/qwen-image-edit-2511

# Text encoder + VAE (from base model repo)
hf download Qwen/Qwen-Image \
    --include "text_encoder/*" "vae/*" "tokenizer/*" "processor/*" \
    --local-dir /mnt/models/huggingface/qwen-image

# ── For ComfyUI native workflow (Comfy-Org files) ────────────────────────────

hf download Comfy-Org/Qwen-Image_ComfyUI \
    --include "split_files/diffusion_models/qwen_image_2512_fp8_e4m3fn.safetensors" \
    --local-dir /tmp/qwen-comfy

hf download Comfy-Org/Qwen-Image_ComfyUI \
    --include "split_files/text_encoders/qwen_2.5_vl_7b_fp8_scaled.safetensors" \
    --local-dir /tmp/qwen-comfy

hf download Comfy-Org/Qwen-Image_ComfyUI \
    --include "split_files/vae/qwen_image_vae.safetensors" \
    --local-dir /tmp/qwen-comfy
```

---

## Resources

- <a href="https://huggingface.co/Qwen/Qwen-Image-Edit-2511" target="_blank">Qwen/Qwen-Image-Edit-2511 — HuggingFace model page</a>
- <a href="https://huggingface.co/Qwen/Qwen-Image" target="_blank">Qwen/Qwen-Image — HuggingFace base model page (text encoder + VAE)</a>
- <a href="https://huggingface.co/Comfy-Org/Qwen-Image_ComfyUI" target="_blank">Comfy-Org/Qwen-Image_ComfyUI — pre-extracted ComfyUI files</a>
- <a href="https://docs.comfy.org/tutorials/image/qwen/qwen-image-edit-2511" target="_blank">docs.comfy.org — Official Qwen-Image-Edit-2511 ComfyUI workflow</a>
- <a href="https://github.com/modelscope/DiffSynth-Studio/blob/main/docs/en/Model_Details/Qwen-Image.md" target="_blank">DiffSynth-Studio — Qwen-Image model documentation</a>
- <a href="https://huggingface.co/docs/huggingface_hub/en/package_reference/cli#huggingface-cli-download" target="_blank">HuggingFace Hub CLI download reference</a>
- <a href="https://huggingface.co/Phr00t/Qwen-Image-Edit-Rapid-AIO" target="_blank">Phr00t/Qwen-Image-Edit-Rapid-AIO — the single-file inference checkpoint</a>

---

**Document Version**: 1.0
**Last Updated**: March 2026
**Research**: gemini-it-security-researcher agent (March 2026)
