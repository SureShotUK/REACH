# Learning ComfyUI — A Practical Guide

**System**: Dual RTX 3090 (48GB VRAM) · Ubuntu 24.04 LTS Server · Docker
**ComfyUI**: Port 8188 (yours) · Port 8189 (Amelia's)
**Focus**: Image generation → Image-to-Image → Face Swap
**Last updated**: March 2026

> **URL note**: WebFetch was unavailable when writing this document. URLs in code blocks (git clone commands) and links to openart.ai/comfyworkflows.com (already verified in `ComfyUI.md`) are included. The RealVisXL HuggingFace URL should be confirmed by browsing to `huggingface.co/SG161222/RealVisXL_V4.0` before running the wget command.

---

## Table of Contents

1. [What is ComfyUI? The Mental Model](#1-what-is-comfyui-the-mental-model)
2. [A Tour of the Interface](#2-a-tour-of-the-interface)
3. [Data Types — What Flows Through the Wires](#3-data-types--what-flows-through-the-wires)
4. [Model Types Explained](#4-model-types-explained)
5. [Essential Nodes Reference](#5-essential-nodes-reference)
6. [Installing Custom Nodes](#6-installing-custom-nodes)
7. [Workflow 1: Your First Image (FLUX)](#7-workflow-1-your-first-image-flux)
8. [Installing a Realistic Photo Model (RealVisXL)](#8-installing-a-realistic-photo-model-realvisxl)
9. [Workflow 2: Text to Image with SDXL](#9-workflow-2-text-to-image-with-sdxl)
10. [Workflow 3: Image to Image](#10-workflow-3-image-to-image)
11. [Workflow 4: Face Swap with ReActor](#11-workflow-4-face-swap-with-reactor)
12. [Prompting Tips](#12-prompting-tips)
13. [Tips and Tricks](#13-tips-and-tricks)
14. [Quick Reference](#14-quick-reference)

---

## 1. What is ComfyUI? The Mental Model

ComfyUI is a **visual programming environment for AI image and video generation**. Instead of typing a prompt into a box and pressing Generate, you build a *graph* — a network of connected boxes (called **nodes**), each performing one specific step in the generation process.

Think of it like a factory assembly line:
- Raw materials (your text prompt, a source image, model settings) enter at the left
- Each station (node) does one specific job
- The finished product (your generated image) comes out at the right

This gives you complete control over every step. The trade-off is that you have to understand what each step does — which is exactly what this guide teaches.

### Why bother with nodes instead of a simple interface?

Simpler interfaces (like Automatic1111's WebUI) hide the pipeline from you. ComfyUI exposes it. This matters when you want to:
- **Swap faces** into generated images — requires inserting a face-swap step at the right point in the pipeline
- **Combine multiple models** — generate with FLUX, then upscale with a separate model, then face-swap the result
- **Control specific aspects** of generation — pose, depth, style — without affecting others
- **Reuse complex workflows** that you or others have designed

Once you have a working workflow saved as a `.json` file, running it again is as simple as loading it and pressing Queue.

### The key insight

Every box on screen is one step. Every wire between boxes carries a specific type of data. Understanding what data each wire carries is 90% of understanding ComfyUI.

---

## 2. A Tour of the Interface

Open ComfyUI in your browser:
- From the server itself: `http://localhost:8188`
- From another device on your network: `http://<server-ip>:8188`
- Via Tailscale from anywhere: `https://amelai.tail926601.ts.net` (port 8188 may need adding depending on Tailscale configuration)

### The Canvas

The main working area is a large dark canvas.

| Action | How |
|--------|-----|
| Pan (move around) | Hold Middle Mouse Button and drag, or Space + Left-click drag |
| Zoom | Mouse wheel up/down |
| Select a node | Left-click on it |
| Move a node | Left-click and drag it |
| Select multiple nodes | Left-click and drag on empty space to draw a selection box |
| Add a node | Right-click on empty canvas space → navigate the menu |
| Delete a node | Select it, press Delete key |

### The Menu (top left)

| Button | What it does |
|--------|-------------|
| **Queue Prompt** | Start generating — runs the current workflow |
| Arrow next to Queue | Set batch count, run continuously, etc. |
| **Save** | Save your current workflow as a `.json` file |
| **Load** | Load a saved `.json` workflow |
| **Refresh** | Reload model lists after downloading new models |
| **Clear** | Remove everything — start with a blank canvas |
| **Load Default** | Load the built-in simple starter workflow |
| **Manager** | Opens ComfyUI Manager (if installed) for installing custom nodes |

### Nodes

Each box on the canvas is a node. Every node has:
- **A title bar** (the coloured strip at the top) — the node's name
- **Inputs** (coloured circles on the left edge) — data coming in
- **Outputs** (coloured circles on the right edge) — data going out
- **Widgets** (the controls inside the box) — settings you can change

### Connections (Wires)

Drag from an **output circle** on the right of one node to an **input circle** on the left of another. Wires are colour-coded by data type (explained in the next section).

**You cannot connect incompatible types.** You cannot feed an IMAGE output into a MODEL input — ComfyUI will reject the connection. If you are unsure what connects where, look at the colour of the circles. Matching colours = compatible.

### Adding Nodes

**Right-click on empty canvas space** → a menu appears organised by category. Browse to find the node you want, then click it to place it on the canvas.

Categories you will use most:
- **loaders** — Load Checkpoint, Load LoRA, Load Image, Load VAE, etc.
- **conditioning** — CLIP Text Encode, ControlNet Apply, FluxGuidance, etc.
- **latent** — Empty Latent Image, VAE Encode, VAE Decode, KSampler
- **image** — Image Resize, Image Batch, Save Image, Preview Image
- **ReActor** (after installation) — face swap nodes

---

## 3. Data Types — What Flows Through the Wires

This is the most important concept in ComfyUI. Every wire carries a specific **type** of data, shown by its colour. When a node needs a particular type of input, only a wire of that type can connect to it.

| Data Type | Wire Colour | What it is | Analogy |
|-----------|-------------|-----------|---------|
| **MODEL** | Purple | The neural network itself — the "brain" trained on millions of images | A car engine |
| **CLIP** | Yellow | The text-understanding component — reads your prompt | A translator |
| **VAE** | Pink/light purple | Encodes/decodes between pixels and the model's internal format | A zip/unzip algorithm |
| **CONDITIONING** | Orange | An encoded prompt — the model has understood your words and converted them into instructions | A set of steering directions |
| **LATENT** | Blue | A compressed internal representation of an image — not actual pixels yet | A blueprint before it's printed |
| **IMAGE** | Green | Actual pixel data — a viewable image | A photograph |
| **INT / FLOAT** | Grey | Simple numbers (width, height, steps, strength, etc.) | A dial or slider value |
| **STRING** | Pale/cream | Text | A text box input |
| **MASK** | Teal/dark green | A black-and-white map indicating regions of an image (for inpainting) | A stencil |

### Why this matters in practice

When you look at a new workflow and feel confused, trace one data type at a time:

1. **Where does the MODEL start?** (Load Checkpoint) — follow the purple wire
2. **Where does the IMAGE end up?** (Save Image) — follow the green wire backwards
3. **What feeds the KSampler?** — it needs MODEL (purple), CONDITIONING×2 (orange), and LATENT (blue)

Once you can answer these questions for any workflow, you understand it.

### The core flow for image generation

```
Load Checkpoint ──► MODEL ──────────────────────────────► KSampler
                 ──► CLIP ──► CLIP Text Encode ──► CONDITIONING ──►
                           ──► CLIP Text Encode ──► CONDITIONING ──►
                 ──► VAE ──────────────────────────────────────────► VAE Decode ──► Save Image
                                                                 ◄── LATENT ◄───── KSampler
Empty Latent Image ──► LATENT ──────────────────────────────────►
```

---

## 4. Model Types Explained

You will encounter many types of model files. Here is what each one does:

### Checkpoint (`.safetensors`, large — 2–20GB)

The main AI model. A checkpoint contains the neural network weights trained on millions of images. Most modern checkpoints include the VAE and text encoders baked in — one file is all you need to generate images.

**Examples**: `flux1-dev-fp8.safetensors`, `RealVisXL_V4.0.safetensors`
**Goes in**: `checkpoints/`
**Loaded by**: Load Checkpoint node
**Outputs**: MODEL + CLIP + VAE (three separate outputs, all from one file)

---

### LoRA — Low-Rank Adaptation (`.safetensors`, small — 50MB–500MB)

A small adapter file that modifies a base checkpoint's output without changing the checkpoint itself. Think of it as a style modifier — you apply a LoRA on top of a checkpoint to push the output in a specific direction:
- A particular art style (Pixar 3D, anime, oil painting)
- A specific person's appearance (trained on photos of them)
- A specific clothing style or setting

**Examples**: `Canopus-Pixar-3D-FluxDev-LoRA.safetensors`
**Goes in**: `loras/`
**Loaded by**: Load LoRA node, inserted between Load Checkpoint and KSampler
**Key setting**: `strength_model` (0.0–1.0) — how strongly to apply it. Start at 0.8.

Multiple LoRAs can be chained — put multiple Load LoRA nodes in series, each one feeding into the next.

---

### VAE — Variational Autoencoder (small — 300–800MB)

Translates between the model's compressed internal format (called **latent space**) and actual pixel images. Think of it as a compression/decompression algorithm specific to image AI.

Most modern checkpoints have a perfectly good VAE baked in. You only need a separate VAE file if:
- The checkpoint's built-in VAE produces washed-out or desaturated colours
- You are using a video model (Wan2.2 uses `wan2.2_vae.safetensors`)
- A community guide specifically recommends a separate VAE for quality reasons

**Goes in**: `vae/`
**Loaded by**: Load VAE node

> **Your setup**: The `vae/` directory currently contains `wan2.2_vae.safetensors` for video generation only. Your FLUX and any SDXL checkpoints have their VAE built in — you do not need a separate VAE for them.

---

### Text Encoder / CLIP (medium — 500MB–10GB)

Converts text prompts into a format the model understands. Most checkpoints include their text encoders. You need separate text encoder files only for:
- Video models (Wan2.2 uses `umt5_xxl_fp8_e4m3fn_scaled.safetensors`)
- Some specialised FLUX configurations that require separate T5-XXL / CLIP-L files

**Goes in**: `text_encoders/`

> **Your setup**: The `text_encoders/` directory contains the Wan2.2 video encoder only. Your FLUX.1 Dev fp8 checkpoint includes its text encoders built in.

---

### ControlNet (medium — 300MB–1.5GB)

A model that controls the *structure* of the generated image — pose, depth, edges, line art — independent of the text prompt. Examples:
- **OpenPose ControlNet** — provide a stick figure pose; the generated person will match it
- **Depth ControlNet** — provide a depth map; the composition will match
- **Canny/Line Art** — provide an edge map; the output follows that structure

ControlNet is powerful for precise structural control but adds workflow complexity. It is not required for basic face swap.

**Goes in**: `controlnet/`

---

### Upscaler (small — 50–70MB)

Increases image resolution using a separate AI model trained specifically for upscaling. Common choices: `4x-UltraSharp.pth`, `RealESRGAN_x4plus.pth`.

**Goes in**: `upscale_models/`
**Used by**: Load Upscale Model node → ImageUpscaleWithModel node

---

### Face Swap Models (ReActor — managed automatically)

ReActor requires two specific model files:
- `inswapper_128.onnx` — the core face-swapping neural network
- `buffalo_l` — face analysis model (a folder of files)

ReActor downloads these automatically on first use. They are stored in `models/reactor/` inside ComfyUI. You do not need to download them manually.

---

## 5. Essential Nodes Reference

> **Note**: `ComfyUI.md` already covers the core FLUX nodes (Load Checkpoint, Load LoRA, CLIP Text Encode, Empty Latent Image, FluxGuidance, KSampler, VAE Decode, Save Image) in detail. This section adds the nodes needed for img2img and face swap workflows.

---

### Load Image

Loads an image from your computer into the workflow. This is how you bring in reference photos for face swap, source images for img2img, or any image you want to process.

| Setting | What it does |
|---------|-------------|
| `image` | Select from previously uploaded images or drop a new one |
| Upload button | Upload a file from your local computer |

**Outputs**: IMAGE, MASK

---

### Image Resize

Changes the dimensions of an image. Useful for preparing a source image for a model that expects a specific resolution.

| Setting | Typical value |
|---------|--------------|
| `width` | 1024 |
| `height` | 1024 |
| `upscale_method` | `lanczos` (best quality), `nearest-exact` (pixel art) |
| `crop` | `disabled` or `center` |
| `keep_proportion` | `disabled` for exact dimensions, `true` to avoid stretching |

---

### VAE Encode

The reverse of VAE Decode. Takes a real IMAGE and converts it into LATENT format so the KSampler can work with it. This is the gateway node for any img2img workflow.

| Input | What to connect |
|-------|----------------|
| `pixels` (IMAGE) | The source image (after resizing if needed) |
| `vae` (VAE) | The VAE output from Load Checkpoint |

**Output**: LATENT — ready to feed into KSampler `latent_image`

---

### KSampler — img2img settings

You already know KSampler from text-to-image. For img2img, one setting changes significantly:

| Setting | txt2img value | img2img value | Why |
|---------|--------------|---------------|-----|
| `latent_image` | Empty Latent Image | VAE Encode output | Starting from your image, not blank noise |
| `denoise` | `1.0` | `0.3–0.75` | Controls how much of the original is changed |

The `denoise` scale:
```
0.0 ── no change at all ── exact copy of input
0.3 ── subtle refinement ── slight changes, same composition
0.5 ── moderate ── keeps structure, changes style/detail
0.75 ── heavy transformation ── original barely visible
1.0 ── full generation ── ignores the input completely
```

---

### Preview Image

Like Save Image but shows the result in the browser without saving to disk. Add one anywhere in your workflow to inspect intermediate results. No settings needed — just connect any IMAGE output to it.

---

### ReActor — Fast Face Swap (after installation)

The main face swap node. Takes a target image (where the face will go) and a source image (the reference face).

| Input | What to connect |
|-------|----------------|
| `input_image` (IMAGE) | The generated image — where you want to place the new face |
| `source_image` (IMAGE) | Your reference photo — the face to use |

Key settings:

| Setting | What it does | Typical value |
|---------|-------------|---------------|
| `enabled` | Toggle the swap on/off without disconnecting wires | `true` |
| `input_faces_index` | Which face in the target to replace (0 = first detected) | `0` |
| `source_faces_index` | Which face in the source photo to use | `0` |
| `face_restore_model` | Post-swap enhancement pass — sharpens and corrects the swapped face | `GFPGANv1.4` or `CodeFormer` |
| `face_restore_visibility` | 0.0 = no restoration, 1.0 = full restoration applied | `0.8–1.0` |
| `codeformer_weight` | CodeFormer only: 0 = max correction, 1 = preserve as-is | `0.5` |
| `detect_gender_input` | Only replace faces of a specific gender in target | `no` |
| `detect_gender_source` | Only use source faces of a specific gender | `no` |

**Output**: IMAGE — the result with the face swapped in

---

## 6. Installing Custom Nodes

Custom nodes are extensions that add new node types to ComfyUI. They are installed as Python packages into ComfyUI's `custom_nodes` directory.

Your ComfyUI runs in Docker (`yanwk/comfyui-boot:cu128-slim`). The key volume mapping is:

```
/opt/comfyui/storage  →  /root  inside the container
```

This means `/root/ComfyUI/custom_nodes` (inside the container) maps to `/opt/comfyui/storage/ComfyUI/custom_nodes` on the host. **Custom nodes persist across container restarts** because they live in this volume.

---

### Method 1: ComfyUI Manager (recommended)

ComfyUI Manager adds a **Manager** button to the ComfyUI top menu. It provides a searchable catalogue of custom nodes you can install with one click.

If you see a Manager button in your ComfyUI interface, use this method. To install ReActor:
1. Click **Manager** → **Install Custom Nodes**
2. Search for `ReActor`
3. Click **Install** next to "ReActor Node for ComfyUI"
4. Restart the container: `docker restart comfyui`
5. Refresh your browser

---

### Method 2: Manual installation via docker exec

SSH into your server:

```bash
# Open a shell inside the ComfyUI container
docker exec -it comfyui bash

# Navigate to the custom_nodes directory
cd /root/ComfyUI/custom_nodes

# Clone the ReActor repository
git clone https://github.com/Gourieff/comfyui-reactor-node

# Install its Python dependencies
cd comfyui-reactor-node
pip install -r requirements.txt

# Exit the container shell
exit
```

Then restart the container:

```bash
docker restart comfyui
```

Refresh your browser. ReActor nodes will appear in the right-click menu under **ReActor**.

---

### Checking that a custom node installed correctly

After restarting, right-click the canvas and look for the new node category. If it does not appear:

```bash
# Check for errors during startup
docker logs comfyui | grep -i "error\|reactor\|failed" | tail -40
```

Common issues:
- **Missing dependencies** — the `pip install -r requirements.txt` step may have failed; re-run it
- **Python version conflict** — rare with the `yanwk` image but possible; check the logs
- **Requires restart** — always restart the container after installing new custom nodes

---

### Installing ComfyUI Manager itself (if not already installed)

If you do not have a Manager button, install it the same way:

```bash
docker exec -it comfyui bash
cd /root/ComfyUI/custom_nodes
git clone https://github.com/ltdrdata/ComfyUI-Manager
exit
docker restart comfyui
```

After this, a **Manager** button will appear in the ComfyUI menu, and you can use it for all future custom node installations.

---

## 7. Workflow 1: Your First Image (FLUX)

You already have `flux1-dev-fp8.safetensors` installed. This section walks through building a complete FLUX workflow from scratch so you understand every connection.

### Step-by-step

**Step 1 — Clear the canvas**
Menu → Clear (or Load Default for a pre-built starting point to modify)

**Step 2 — Add Load Checkpoint**
Right-click → loaders → Load Checkpoint
Set `ckpt_name`: `flux1-dev-fp8.safetensors`

This node outputs three things from one file: MODEL (the network), CLIP (the text reader), and VAE (the encoder/decoder).

**Step 3 — Add two CLIP Text Encode nodes**
Right-click → conditioning → CLIP Text Encode (add two)
- Connect the **CLIP** output of Load Checkpoint → `clip` input of both nodes
- First node (positive prompt): type your prompt
  ```
  photorealistic portrait of a woman, brown hair, blue eyes, soft studio lighting, 8k
  ```
- Second node (negative prompt): leave blank — FLUX ignores negative prompts

**Step 4 — Add FluxGuidance** (FLUX-specific — not used with SDXL)
Right-click → conditioning → FluxGuidance
- Connect **CONDITIONING** from positive CLIP Text Encode → FluxGuidance `conditioning`
- Set `guidance`: `3.5` (range 1–10, higher = stricter prompt following)
- FluxGuidance **CONDITIONING** output → KSampler `positive` input

**Step 5 — Add Empty Latent Image**
Right-click → latent → Empty Latent Image
- `width`: `1152`, `height`: `896` (landscape), `batch_size`: `1`
- Other good FLUX resolutions: 1024×1024 (square), 896×1152 (portrait)

**Step 6 — Add KSampler**
Right-click → sampling → KSampler

Connect:
| KSampler input | Connect from |
|----------------|-------------|
| `model` | Load Checkpoint MODEL output |
| `positive` | FluxGuidance CONDITIONING output |
| `negative` | Second CLIP Text Encode CONDITIONING output |
| `latent_image` | Empty Latent Image LATENT output |

Settings:
| Setting | Value | Why |
|---------|-------|-----|
| `seed` | any number | Same seed = same image; randomise for variety |
| `steps` | `20` | Enough for good quality |
| `cfg` | `1.0` | **Critical for FLUX** — do not change; higher values distort |
| `sampler_name` | `euler` | Works well with FLUX |
| `scheduler` | `simple` | FLUX's native scheduler |
| `denoise` | `1.0` | Full generation from noise |

**Step 7 — Add VAE Decode**
Right-click → latent → VAE Decode
- Connect KSampler **LATENT** output → `samples`
- Connect Load Checkpoint **VAE** output → `vae`

**Step 8 — Add Save Image**
Right-click → image → Save Image
- Connect VAE Decode **IMAGE** output → `images`

**Step 9 — Press Queue Prompt**

The image generates and appears in the Save Image node. It is saved to `/opt/comfyui/output/` on the server.

### Save this workflow

Menu → Save → `flux-txt2img.json`

---

## 8. Installing a Realistic Photo Model (RealVisXL)

FLUX is excellent, but most community workflows — including face swap workflows — were built for **SDXL** (Stable Diffusion XL). Learning SDXL gives you access to a much wider library of pre-built workflows and is the practical foundation for face swap work.

### What is SDXL?

SDXL is the previous generation to FLUX. Compared to FLUX:
- **CFG scale matters** — set to 5.0–8.0; you actively steer the image with your prompt
- **Negative prompts work** — specify what you *don't* want; the model avoids it
- **Vast ecosystem** — thousands of fine-tuned models, LoRAs, ControlNets, community workflows
- **More face swap compatibility** — most ReActor workflows and guides were written for SDXL

**RealVisXL** is a version of SDXL fine-tuned specifically for photorealistic human portraits. It is the recommended starting model for face swap work.

### Finding and downloading RealVisXL

Since CivitAI is blocked in the UK, use Hugging Face.

1. **First, confirm the file name** — browse to `https://huggingface.co/SG161222/RealVisXL_V4.0` and click "Files and versions" to see the exact `.safetensors` filename

2. **SSH into your server** and run:

```bash
# Start a tmux session so the download survives SSH disconnection
tmux new-session -s model-dl

# Download RealVisXL (replace filename if different from below)
wget -O /mnt/models/comfyui/checkpoints/RealVisXL_V4.0.safetensors \
  "https://huggingface.co/SG161222/RealVisXL_V4.0/resolve/main/RealVisXL_V4.0.safetensors"
```

3. **Detach from tmux while it downloads** — press Ctrl+B, then D

4. **Check progress later** — `tmux attach -t model-dl`

The file is approximately 6–7GB. After download, refresh your browser in ComfyUI — RealVisXL will appear in the checkpoint dropdown.

### Alternative photorealistic SDXL models on Hugging Face

If RealVisXL is not available or you want alternatives, search Hugging Face for:
- `RunDiffusion/Juggernaut-XL-v9` — excellent for photorealism, widely used
- `stabilityai/stable-diffusion-xl-base-1.0` — the official SDXL base (less photorealistic but a solid starting point)

Search `https://huggingface.co/models?pipeline_tag=text-to-image&search=realvis+xl` for others.

---

## 9. Workflow 2: Text to Image with SDXL

SDXL workflows differ from FLUX in several important ways. Here is how to build one.

### Key differences from FLUX

| Setting / Node | FLUX | SDXL (RealVisXL) |
|----------------|------|-----------------|
| FluxGuidance node | Required | Not used — remove it |
| CFG scale | `1.0` | `5.0–8.0` (try `7.0`) |
| Negative prompt | Ignored | Important — use it |
| Sampler | `euler` | `dpmpp_2m` or `euler_a` |
| Scheduler | `simple` | `karras` |
| Steps | 20 | 25–35 |
| Resolution | 1024×1024 up to 1536px | 1024×1024 (SDXL native) |

### Building the RealVisXL workflow

Start from your FLUX workflow or from a fresh canvas.

**Step 1 — Load Checkpoint**
Change `ckpt_name` to `RealVisXL_V4.0.safetensors`

**Step 2 — Remove FluxGuidance**
Select it and press Delete. Connect your positive CLIP Text Encode CONDITIONING directly to KSampler `positive`.

**Step 3 — Update your prompts**

Positive prompt:
```
RAW photo, close-up portrait of a woman, 35 years old, brown hair, hazel eyes,
wearing a white blouse, soft bokeh background, studio lighting, photorealistic,
ultra detailed, 8k uhd, DSLR, sharp focus
```

Negative prompt (important for SDXL — this actively steers the model away from bad results):
```
(worst quality:1.4), (low quality:1.4), (monochrome:1.1), skin blemishes, acne,
bad anatomy, bad hands, cropped, watermark, signature, text, blurry, deformed,
ugly, disfigured, mutation
```

**Step 4 — Update KSampler settings**

| Setting | SDXL value |
|---------|-----------|
| `cfg` | `7.0` |
| `sampler_name` | `dpmpp_2m` |
| `scheduler` | `karras` |
| `steps` | `30` |
| `denoise` | `1.0` |

**Step 5 — Empty Latent Image resolution**
SDXL works best at native 1024×1024 resolution. Keep it at 1024×1024.

**Step 6 — Queue Prompt**

Save as `sdxl-realvisxl-txt2img.json`.

---

## 10. Workflow 3: Image to Image

Image-to-image (img2img) takes an existing image and transforms it while keeping the general structure. The model starts from your image rather than blank noise, so the output retains the composition and layout.

### Common use cases

- Take a rough sketch and make it photorealistic
- Apply a style to a photograph
- Correct or enhance a generated image
- **Generate a body/scene, then swap the face** — this is the most useful pattern for face swap workflows

### Building an img2img workflow

Start from your SDXL workflow. The changes are:

**Remove**: Empty Latent Image

**Add**: Load Image
- Upload or select your source image — the one you want to transform

**Add**: Image Resize (between Load Image and VAE Encode)
- Connect Load Image IMAGE → Image Resize `image`
- Set `width`: `1024`, `height`: `1024` (match your SDXL resolution)
- `upscale_method`: `lanczos`

**Add**: VAE Encode
- Connect Image Resize IMAGE → VAE Encode `pixels`
- Connect Load Checkpoint VAE → VAE Encode `vae`
- Connect VAE Encode LATENT → KSampler `latent_image`

**Change KSampler `denoise`**:
- `0.3` — subtle changes, keeps most of original composition
- `0.5` — moderate; keeps structure, changes style and detail
- `0.75` — heavy transformation; rough composition only survives

**Update your prompt** to describe what you want the output to look like.

### The data flow

```
Load Image → Image Resize → VAE Encode → KSampler (denoise: 0.5) → VAE Decode → Save Image
                                               ▲
Load Checkpoint → MODEL, CLIP, VAE ────────────┘
CLIP Text Encode (positive) → CONDITIONING ────┘
CLIP Text Encode (negative) → CONDITIONING ────┘
```

Save as `sdxl-img2img.json`.

---

## 11. Workflow 4: Face Swap with ReActor

Face swap replaces a face in one image with a face from a reference photo. ReActor works as a **post-processing step** on the final pixel image — it runs after generation, not during it. This means it can be added to any generation workflow regardless of which model you used.

### Prerequisites

1. ReActor custom node installed (see Section 6)
2. On first use, ReActor auto-downloads `inswapper_128.onnx` and the `buffalo_l` face analysis model

If the download fails on first use, you can trigger it manually by running the ReActor node with any valid inputs — the logs will show what it is attempting to download.

### How face swap works

```
[Generated image] + [Reference face photo] → ReActor → [Generated image with swapped face]
```

1. Generate an image of a person (any model — FLUX or SDXL both work)
2. Load a reference photo of the face you want to use
3. ReActor detects the face in both images, maps the reference face geometry onto the target, and blends it in
4. An optional face restoration pass (GFPGAN or CodeFormer) sharpens and corrects the result

### Building the face swap workflow

Start from your completed txt2img workflow (SDXL or FLUX). Add the following:

**Add**: Load Image (for the reference face)
- Upload a clear, front-facing photo of the person whose face you want to use

**Add**: ReActor node
- Right-click → ReActor → Fast Face Swap (or similar — the exact menu name may vary)

**Connect**:
| ReActor input | Connect from |
|---------------|-------------|
| `input_image` (IMAGE) | VAE Decode IMAGE output (the generated image — target) |
| `source_image` (IMAGE) | Load Image IMAGE output (the reference photo — source) |

**Reconnect Save Image**:
- Disconnect VAE Decode → Save Image
- Connect ReActor IMAGE output → Save Image `images`

### The complete face swap data flow

```
Load Checkpoint ──► MODEL, CLIP, VAE
                         │
CLIP Text Encode (pos) ──┤
CLIP Text Encode (neg) ──┤
Empty Latent Image ───── KSampler ──► VAE Decode ──► ReActor ──► Save Image
                                                         ▲
                         Load Image (reference face) ────┘
```

### ReActor settings guide

| Setting | What it does | Recommendation |
|---------|-------------|----------------|
| `enabled` | Toggle on/off without disconnecting wires | `true` |
| `input_faces_index` | Which face in the target to replace (0 = first detected) | `0` for single person |
| `source_faces_index` | Which face in the source photo to use | `0` |
| `face_restore_model` | Post-swap enhancement — sharpens and corrects the swapped face | `GFPGANv1.4` (auto-downloaded) |
| `face_restore_visibility` | 0 = no restoration, 1 = full restoration | `1.0` |
| `codeformer_weight` | CodeFormer only: 0 = max correction, 1 = preserve | `0.5` |
| `detect_gender_input` | Restrict which faces in target to replace by gender | `no` |
| `facedetection` | Face detection model | `retinaface_resnet50` |

### Tips for good results

**Source photo quality matters most:**
- Clear, front-facing, good lighting
- No heavy shadows across the face
- High resolution — at least the face itself should be 256×256 or larger
- Minimal obstructions (glasses can cause issues, hair across the face degrades quality)

**Target image setup:**
- Generate images where the face is clearly visible — not in profile, not tiny in the frame
- Portrait orientation with the face filling a significant portion helps
- Consistent lighting between source and target improves blending

**If the result looks bad:**
- Set `face_restore_visibility` to `1.0` — the enhancement pass fixes most blending artefacts
- Try `CodeFormer` instead of `GFPGANv1.4` as the restore model — some prefer it
- Regenerate with a different seed to get a better face angle in the target image
- If the face is too small in the target, use a higher resolution and zoom in more on the face in your prompt

**Multiple faces in the target**: Set `input_faces_index` to `0,1,2` to replace all detected faces with the same source face, or use separate ReActor nodes for different source faces per target face.

### Iterating on a face swap workflow

A practical iteration loop:
1. Generate a few images at low steps (15) to find a good composition
2. Pick the best result as your target
3. Set seed to a fixed number (so you can reproduce it)
4. Increase steps to 30, queue with face swap enabled
5. Adjust face restore settings if needed
6. Once satisfied with the result, raise resolution for the final version

Save as `sdxl-faceswap.json`.

---

## 12. Prompting Tips

### SDXL / RealVisXL prompts

SDXL responds strongly to prompts. Structure matters.

**Positive prompt format:**
```
(quality tags), subject description, setting/background, lighting, camera style
```

Example:
```
RAW photo, a 28-year-old woman, long auburn hair, green eyes, wearing a black
linen shirt, sitting in a warmly lit café, shallow depth of field, 85mm lens,
ultra detailed, 8k uhd, photorealistic
```

**Useful quality boosters for photorealism:**
- `RAW photo`, `DSLR`, `8k uhd`, `ultra detailed`
- `photorealistic`, `hyperrealistic`, `professional photography`
- `sharp focus`, `cinematic lighting`, `bokeh`

**Standard negative prompt for RealVisXL (copy and reuse):**
```
(worst quality:2), (low quality:2), (normal quality:2), lowres, bad anatomy,
bad hands, watermark, text, logo, signature, deformed, disfigured, ugly,
mutation, poorly drawn face, extra limbs, missing fingers
```

**Emphasis/de-emphasis syntax:**
- `(word:1.4)` — increase influence (max ~1.5 before distortion)
- `(word:0.5)` — reduce influence
- `((word))` — shorthand for slight boost (equivalent to ~1.1)

### FLUX prompts

FLUX is trained to follow natural language descriptions. Write prompts as if describing a scene to a person, not as a keyword list.

```
A photorealistic portrait of a woman in her late twenties. She has auburn hair
falling past her shoulders and is wearing a dark jacket. She is standing in a
warmly lit coffee shop, with shallow depth of field background.
```

- No need for quality tags — FLUX handles these implicitly
- Negative prompts do nothing — FLUX ignores them
- More verbose and specific descriptions generally outperform short keyword lists
- CFG must stay at `1.0`; FluxGuidance at `3.5` is a good default

---

## 13. Tips and Tricks

### Reload a workflow from a generated image

Every image ComfyUI generates has the full workflow embedded in it as metadata. Drag any output image directly onto the ComfyUI canvas to reload the exact workflow, settings, prompt, and seed that produced it. This is the single most useful ComfyUI feature for:
- Resuming work from a previous session
- Sharing a workflow (share the image; the workflow comes with it)
- Reproducing an exact result you liked

### Bypass a node without disconnecting it

Right-click any node → **Bypass**. The node is skipped during generation but its wires remain connected. Useful for quickly A/B comparing results with and without a LoRA, face restore step, or any other processing node.

### Mute a node

Right-click → **Mute**. The node appears greyed out and passes data through unchanged. Different from Bypass — the node still processes (passes through) rather than being skipped entirely.

### Fix the seed to reproduce a result

In KSampler, note the `seed` value when you generate an image you like. Change the seed control to `fixed` and enter that number. The same seed + same prompt + same settings = same image.

Set seed to `-1` (or use the randomise control) to get a different result each time.

### Queue multiple generations

Click the small arrow next to Queue Prompt → set **Batch count**. Setting it to 4 queues 4 generations with randomised seeds, processed sequentially. Useful for exploring prompt variations without babysitting the interface.

### Preview mid-workflow (without saving)

Add a **Preview Image** node anywhere in the flow. Connect any IMAGE output to it. The node shows the image in the browser without saving to disk. Add one after VAE Decode to confirm the base image looks right before face swap runs.

### Speed up iteration

When exploring settings:
- Lower `steps` to 10–12 for fast rough previews (seconds instead of minutes)
- Use 768×768 instead of 1024×1024
- Once the composition looks right at low quality, raise steps and resolution for the final version

### Where outputs are saved on the server

| Output type | Server path |
|-------------|-------------|
| Generated images | `/opt/comfyui/output/` |
| Saved workflows | `/opt/comfyui/workflows/` |

Access from Windows via SSH (SCP), or browse via the ComfyUI interface which can display recent outputs.

### Finding community workflows

- <a href="https://openart.ai/workflows/home" target="_blank">openart.ai/workflows</a> — visual gallery with previews
- <a href="https://comfyworkflows.com" target="_blank">comfyworkflows.com</a> — community-shared JSON files

Load any `.json` file via Menu → Load. If a workflow requires custom nodes you do not have, ComfyUI highlights the missing nodes in red. Install them via ComfyUI Manager and restart the container.

### Troubleshooting: node shows red / missing

Means a required custom node is not installed. Right-click the red node → it will usually say which package is needed. Install via ComfyUI Manager or `docker exec`.

### Troubleshooting: generation fails with CUDA out of memory

Your 48GB total VRAM is very generous, but if you run both GPUs on separate tasks simultaneously or have very large batches:

```bash
# Check VRAM usage
nvidia-smi

# Check ComfyUI logs
docker logs comfyui | tail -30
```

Try reducing batch size to 1, or lowering resolution.

---

## 14. Quick Reference

### Workflow type → what nodes to add/change

| Goal | vs. basic txt2img |
|------|-------------------|
| Text to image (FLUX) | Checkpoint + FluxGuidance + KSampler + VAE Decode + Save Image |
| Text to image (SDXL) | Same but no FluxGuidance; CFG = 7, scheduler = karras |
| Image to image | Add Load Image + Image Resize + VAE Encode; remove Empty Latent Image; lower denoise |
| Face swap | Add Load Image (reference) + ReActor; insert ReActor between VAE Decode and Save Image |
| Add a LoRA | Add Load LoRA between Load Checkpoint and KSampler |
| Upscale | Add Load Upscale Model + ImageUpscaleWithModel after generation |

### KSampler settings cheat sheet

| Model | CFG | Sampler | Scheduler | Steps | Denoise (txt2img) |
|-------|-----|---------|-----------|-------|-------------------|
| FLUX | 1.0 | euler | simple | 20 | 1.0 |
| SDXL (RealVisXL) | 7.0 | dpmpp_2m | karras | 30 | 1.0 |
| img2img (SDXL) | 7.0 | dpmpp_2m | karras | 30 | 0.5 |

### Your model locations

| Folder | Path on server | Current contents |
|--------|---------------|-----------------|
| `checkpoints/` | `/mnt/models/comfyui/checkpoints/` | `flux1-dev-fp8.safetensors` |
| `loras/` | `/mnt/models/comfyui/loras/` | Canopus Pixar LoRA |
| `diffusion_models/` | `/mnt/models/comfyui/diffusion_models/` | Wan2.2 video model |
| `text_encoders/` | `/mnt/models/comfyui/text_encoders/` | Wan2.2 text encoder |
| `vae/` | `/mnt/models/comfyui/vae/` | Wan2.2 VAE |

### Docker commands for ComfyUI

```bash
docker restart comfyui          # Restart (required after installing custom nodes)
docker logs comfyui -f          # Follow live logs (useful for debugging)
docker logs comfyui | tail -50  # Last 50 lines of log
docker exec -it comfyui bash    # Open shell inside the container
```

---

*Document last updated: March 2026*
*System: Dual RTX 3090 · Ubuntu 24.04 · Docker (yanwk/comfyui-boot:cu128-slim)*
