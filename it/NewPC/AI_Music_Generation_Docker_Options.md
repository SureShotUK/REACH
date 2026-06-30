# AI Music Generation Docker Deployment Options

Research date: 2026-05-08

---

## 1. MusicGen (Meta / AudioCraft)

### Overview
Meta's text-to-music model. Part of the AudioCraft library. No official Docker image from Facebook/Meta.

### Existing Docker Images

| Image | Pulls | Size | Last Updated | Notes |
|---|---|---|---|---|
| **sxk1633/musicgen** | 4,900 | 3.1 GB | 2+ years old | Oldest, unmaintained |
| **hypelaunchdev/musicgen** | 1,700 | **895 MB** | ~2 months ago | Most recently updated, smallest |
| appstanetech/musicgen | 80 | - | 2+ years | fb musicgen model text to audio generation |

**Recommended: `hypelaunchdev/musicgen`** -- actively updated, reasonable size.

### Official Repo
- **GitHub**: [facebookresearch/audiocraft](https://github.com/facebookresearch/audiocraft) (23.3k stars)
- No Dockerfile or docker-compose in the repo
- Uses **Gradio** for the demo UI (demos/musicgen_demo.py)
- Requires Python 3.9, PyTorch 2.1.0

### GPU Requirements
- **Base image**: `nvidia/cuda:11.7.1-cudnn8-runtime-ubuntu22.04` (typical)
- **CUDA**: 11.7 or 12.0
- **PyTorch**: 2.1.0+ with CUDA support
- **VRAM**: ~4-8 GB for MusicGen Small, ~12 GB for MusicGen Large
- Runtime: `--gpus all` or `--runtime=nvidia`

### Web UI Port
- **Gradio**: Port `7860` (default)

### Volume Mounts
- No special mounts needed -- models auto-download on first run
- Optional: `./output:/output` for generated audio

### Verdict
Best approach: Either use `hypelaunchdev/musicgen` or build a custom container from the audiocraft demos/musicgen_demo.py. With 48GB total VRAM and the RTX 3090s, MusicGen is the least VRAM-intensive of the three tools and would coexist alongside Ollama models.

---

## 2. Bark (Suno AI)

### Overview
Suno's text-to-audio model. Supports text-to-speech with vocal expressions (laughs, um, etc.), multi-language, and audio generation. No official Docker image from Suno.

### Existing Docker Images

| Image | Pulls | Size | Last Updated | Notes |
|---|---|---|---|---|
| **niteshbalusu/bark** | 10,000+ | **49 MB** | 11 hours ago | Very recent, minimal image (models download at runtime) |
| **saladtechnologies/bark** | 679 | 8.7 GB | 2+ years | Inference server with REST API (not web UI) |
| **itsyaboy/bark** | 1,200 | 98 MB | ~2 months ago | Recently updated, small |
| techrail/bark | 274 | 360 MB | 2+ years | Legacy bark library (not Suno AI) |

**Recommended: `niteshbalusu/bark`** -- 10K+ pulls, updated 11 hours ago, minimal footprint (models download at runtime).

### Official Repo
- **GitHub**: [suno-ai/bark](https://github.com/suno-ai/bark) (39.1k stars, 4.7k forks)
- No Dockerfile or docker-compose in the repo
- **No built-in web UI** -- it is a Python library (generate_audio function), not a Gradio/Streamlit app

### GPU Requirements
- **CUDA**: 11.7 or 12.0
- **PyTorch**: 2.0+
- **VRAM**: ~12 GB (full model), ~8 GB (with `SUNO_USE_SMALL_MODELS=True`)
- Can run on CPU but inference is significantly slower
- On enterprise GPUs: roughly real-time generation
- Runtime: `--gpus all` or `--runtime=nvidia`

### Web UI Port
- **None built-in** -- you must wrap it in a Gradio/Streamlit app yourself
- saladtechnologies/bark provides a REST API on port `8000` (POST to `/generate`)

### Volume Mounts
- No special mounts needed -- models auto-download to `~/.cache/bark` on first run
- Optional: `./cache:/root/.cache/huggingface` for cached models

### Verdict
Bark is fundamentally a Python library, not a web app. To deploy with a web UI you would need to create your own Gradio wrapper. The niteshbalusu/bark image is the most current but has no web UI. If you want a web UI, you would wrap it in Gradio similar to how the official Hugging Face Space does it ([facebook/MusicGen](https://huggingface.co/spaces/facebook/MusicGen) uses Gradio).

VRAM impact: ~12 GB (full) or ~8 GB (small) from the 48GB available.

---

## 3. RVC (Retrieval-based Voice Conversion)

### Overview
Community project for voice cloning/conversion. Two main forks:
- **RVC-Project/Retrieval-based-Voice-Conversion-WebUI** (35.5k stars, 5k forks) -- the original
- **RVC-Boss** -- the forked/maintained version (RVC-Boss individual account has no public GitHub repos; they merged into RVC-Project organization)

### Existing Docker Images

| Image | Pulls | Size | Last Updated | Notes |
|---|---|---|---|---|
| **kamilake/rvc** (RVC AIO) | 1,600 | 7.2 GB | 2+ years | AIO: includes Gradio + TensorBoard + FileBrowser. Port 7865/6006/8080 |
| **thaomike/rvc** | 567 | 8.2 GB | 1+ year | Mangio-RVC v23.7.0 fork. Port 8000 (mapped from 7866) |
| **bitplane1/rvc** | 949 | 5.6 GB | 2+ years | Direct from RVC-Project. Port 7865 |
| avikaitha/rvc | 305 | 8.1 GB | 8 months | RVC image |

**Recommended: `kamilake/rvc`** -- "All-In-One" with FileBrowser for dataset management, TensorBoard for training monitoring, and Gradio WebUI.

### Official Docker Support
- **RVC-Project GitHub**: [Dockerfile](https://github.com/RVC-Project/Retrieval-based-Voice-Conversion-WebUI/blob/main/Dockerfile) and [docker-compose.yml](https://github.com/RVC-Project/Retrieval-based-Voice-Conversion-WebUI/blob/main/docker-compose.yml) included in repo
- Base image: `nvidia/cuda:11.6.2-cudnn8-runtime-ubuntu20.04`
- Pre-downloads pretrained_v2 models, UVR5 weights, hubert_base.pt, rmvpe.pt

### GPU Requirements
- **Base image**: `nvidia/cuda:11.6.2-cudnn8-runtime-ubuntu20.04` (RVC-Project official)
- **CUDA**: 11.6+
- **PyTorch**: 2.0+
- **VRAM**: ~4-8 GB (training), ~2-4 GB (inference only)
- **Runtime**: `--gpus all` or `--runtime=nvidia`

### Web UI Port
- **Gradio**: Port `7865` (RVC-Project official), `7866` (thaomike)
- **TensorBoard**: Port `6006` (included in kamilake/rvc AIO)
- **FileBrowser**: Port `8080` (included in kamilake/rvc AIO)

### Volume Mounts (Critical)
```yaml
volumes:
  - ./weights:/app/assets/weights      # Trained voice models
  - ./opt:/app/opt                      # Configuration
  - ./dataset:/app/dataset              # Training data
  - ./audio:/app/audio                  # Output audio
  - ./logs:/app/logs                    # Training logs
  - ./uvr5_weights:/app/uvr5_weights    # Vocal remover models
```

### Verdict
RVC is the most Docker-friendly of the three -- the official repo includes Dockerfile and docker-compose.yml. The kamilake/rvc AIO image provides the most complete experience with FileBrowser, TensorBoard, and Gradio. VRAM impact: ~4-8 GB during inference, more during training.

---

## Combined VRAM Budget (48GB total from 2x RTX 3090)

| Tool | Inference VRAM | Training VRAM |
|---|---|---|
| MusicGen (Small) | ~4 GB | -- |
| MusicGen (Large) | ~12 GB | -- |
| Bark (full) | ~12 GB | -- |
| Bark (small) | ~8 GB | -- |
| RVC (inference) | ~2-4 GB | -- |
| RVC (training) | ~4-8 GB | ~8-12 GB |
| qwen3.5:35b (Ollama) | ~26-28 GB | -- |

### Recommended Deployment Strategy
With 48GB VRAM and existing Ollama models:
1. **MusicGen + RVC** can run together (combined ~6-12 GB inference VRAM)
2. **Bark** at ~12 GB would be the heaviest -- may need to unload other models
3. **Important**: ComfyUI holds models in VRAM indefinitely. Free VRAM between sessions using the `/free` endpoint or browser bookmarklet.
4. Consider running these tools sequentially rather than simultaneously given the existing Ollama workload.

---

## Docker Run Examples

### MusicGen (hypelaunchdev)
```bash
docker run -d --name musicgen --gpus all \
  -p 7860:7860 \
  hypelaunchdev/musicgen:af73863-69
```

### Bark (niteshbalusu)
```bash
docker run -d --name bark --gpus all \
  -p 7860:7860 \
  niteshbalusu/bark:nightly
```

### RVC (kamilake AIO)
```bash
docker run -d --name rvc --gpus all \
  -p 17865:7865 -p 16006:6006 -p 18080:8080 \
  -v ./weights:/app/assets/weights \
  -v ./opt:/app/opt \
  -v ./dataset:/app/dataset \
  -v ./audio:/app/audio \
  -v ./logs:/app/logs \
  kamilake/rvc:latest
```

### With docker-compose (RVC-Project official)
```yaml
version: "3.8"
services:
  rvc:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: rvc
    volumes:
      - ./weights:/app/assets/weights
      - ./opt:/app/opt
      - ./dataset:/app/dataset
    ports:
      - 7865:7865
    deploy:
      resources:
        reservations:
          devices:
            - driver: nvidia
              count: 1
              capabilities: [gpu]
```

---

*Sources:*
- *AudioCraft: <a href="https://github.com/facebookresearch/audiocraft" target="_blank">facebookresearch/audiocraft on GitHub</a>*
- *Bark (Suno): <a href="https://github.com/suno-ai/bark" target="_blank">suno-ai/bark on GitHub</a>*
- *RVC-Project: <a href="https://github.com/RVC-Project/Retrieval-based-Voice-Conversion-WebUI" target="_blank">RVC-Project/Retrieval-based-Voice-Conversion-WebUI on GitHub</a>*
- *Docker Hub: <a href="https://hub.docker.com/r/sxk1633/musicgen" target="_blank">sxk1633/musicgen</a>, <a href="https://hub.docker.com/r/hypelaunchdev/musicgen" target="_blank">hypelaunchdev/musicgen</a>, <a href="https://hub.docker.com/r/niteshbalusu/bark" target="_blank">niteshbalusu/bark</a>, <a href="https://hub.docker.com/r/saladtechnologies/bark" target="_blank">saladtechnologies/bark</a>, <a href="https://hub.docker.com/r/kamilake/rvc" target="_blank">kamilake/rvc</a>*
- *Hugging Face: <a href="https://huggingface.co/spaces/facebook/MusicGen" target="_blank">facebook/MusicGen Space</a>*
