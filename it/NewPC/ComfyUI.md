# ComfyUI Administration Guide

**System**: Dual RTX 3090 (48GB VRAM), Ubuntu 24.04 LTS Server

| | Yours | Amelia's |
|-|-------|---------|
| **Container** | `comfyui` | `comfyui-amelia` |
| **GPU** | GPU 1 (device 1) | GPU 0 (device 0) |
| **Internal port** | 18189 | 18188 |
| **Tailscale URL** | `https://amelai.tail926601.ts.net:8189` | `https://amelai.tail926601.ts.net:8188` |
| **Models storage** | `/mnt/models/comfyui/` | `/mnt/models/comfyui-amelia/` |
| **Output folder** | `/opt/comfyui/output/` | `/opt/comfyui-amelia/output/` |

### Docker run commands

**Yours** (GPU 1, internal port 18189):
```bash
docker run -d \
  --name comfyui \
  --network ai-network \
  --restart unless-stopped \
  --runtime nvidia \
  --gpus all \
  -p 127.0.0.1:8189:8188 \
  -p 192.168.1.192:8189:8188 \
  -v /mnt/models/comfyui:/root/ComfyUI/models \
  -v /opt/comfyui/storage:/root \
  -v /opt/comfyui/input:/root/ComfyUI/input \
  -v /opt/comfyui/output:/root/ComfyUI/output \
  -v /opt/comfyui/workflows:/root/ComfyUI/user/default/workflows \
  -e CUDA_VISIBLE_DEVICES=1 \
  -e CLI_ARGS="--disable-xformers" \
  -e PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True \
  yanwk/comfyui-boot:cu128-slim
```

> **Note**: `CUDA_VISIBLE_DEVICES=1` is used instead of `--cuda-device 1` in CLI_ARGS — it is more reliable as it makes GPU 1 the only GPU the container can see. This prevents OOM errors with large models (e.g. Qwen image edit) that nearly fill a single GPU's VRAM.

**Amelia's** (GPU 0, internal port 18188):
```bash
docker run -d \
  --name comfyui-amelia \
  --network ai-network \
  --restart unless-stopped \
  --runtime nvidia \
  --gpus all \
  -p 127.0.0.1:18188:8188 \
  -p 192.168.1.192:8188:8188 \
  -v /mnt/models/comfyui-amelia:/root/ComfyUI/models \
  -v /opt/comfyui-amelia/storage:/root \
  -v /opt/comfyui-amelia/input:/root/ComfyUI/input \
  -v /opt/comfyui-amelia/output:/root/ComfyUI/output \
  -v /opt/comfyui-amelia/workflows:/root/ComfyUI/user/default/workflows \
  -e CUDA_VISIBLE_DEVICES=0 \
  -e CLI_ARGS="--disable-xformers" \
  -e PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True \
  yanwk/comfyui-boot:cu128-slim
```

### Tailscale Serve config

Tailscale Serve proxies external ports to the internal Docker ports. To rebuild if reset:
```bash
tailscale serve --bg --https 443 http://localhost:3000   # Open WebUI
tailscale serve --bg --https 8188 http://localhost:18188 # Amelia's ComfyUI
tailscale serve --bg --https 8189 http://localhost:18189 # Your ComfyUI
```

Verify: `tailscale serve status`

---

## Table of Contents

1. [Installing New Models](#1-installing-new-models)
2. [Enabling Models for Amelia](#2-enabling-models-for-amelia)
3. [Installing Custom Nodes — ReActor Face Swap](#3-installing-custom-nodes--reactor-face-swap)
4. [Getting Started with Nodes](#4-getting-started-with-nodes)

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

## 3. Installing Custom Nodes — ReActor Face Swap

ReActor adds face swap capability to ComfyUI. It needs to be reinstalled whenever the `comfyui` container is recreated (the custom node files persist in the volume, but pip dependencies do not survive a container rebuild).

### Step 1 — Install via ComfyUI Manager

1. Open your ComfyUI at `https://amelai.tail926601.ts.net:8189`
2. Click **Manager** in the top menu
3. Click **Install Custom Nodes**
4. Search for `ReActor`
5. Click **Install** next to "ReActor Node for ComfyUI" by Gourieff
6. Once installed, restart the container:

```bash
docker restart comfyui
```

7. Hard refresh your browser (`Ctrl+Shift+R`) after 30 seconds

### Step 2 — Download the face swap model

The `inswapper_128.onnx` model is required and must be downloaded manually. It lives in the models volume so it survives container rebuilds — only needed once:

```bash
mkdir -p /mnt/models/comfyui/reactor

wget -O /mnt/models/comfyui/reactor/inswapper_128.onnx \
  "<URL from Hugging Face>"
```

> **URL note**: The original download URL was found by searching Hugging Face for `inswapper_128.onnx`. Verify the current URL before downloading — search `huggingface.co` for `xingren23/comfyflow-models` or `inswapper_128.onnx`.

If the file already exists at `/mnt/models/comfyui/reactor/inswapper_128.onnx` from a previous install, this step can be skipped.

### Step 3 — Verify

Right-click on the ComfyUI canvas. You should see a **ReActor** category in the menu.

Face restoration models (`GFPGANv1.4`, `CodeFormer`) are downloaded automatically on first use — this is normal.

### Troubleshooting

If the ReActor node shows red after installation:

```bash
docker logs comfyui | grep -i "reactor\|error" | tail -30
```

If dependencies failed to install, re-run them manually:

```bash
docker exec -it comfyui bash
cd /root/ComfyUI/custom_nodes/comfyui-reactor-node
pip install -r requirements.txt
exit
docker restart comfyui
```

---

## 4. Getting Started with Nodes

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
