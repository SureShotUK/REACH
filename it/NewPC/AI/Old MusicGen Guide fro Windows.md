# Local AI Music Generation Tools - Setup Guide

**Date**: March 2026
**Target Platform**: Windows 11 with GPU support (NVIDIA recommended)
**Prerequisites**: User familiar with ComfyUI, Python, and GPU-accelerated workloads

---

## Tool Comparison Overview

| Tool | Primary Use | VRAM Required | RAM Required | Disk Space | Maturity |
|------|-------------|---------------|--------------|------------|----------|
| **MusicGen** | Music generation from text | 8-12 GB | 16 GB | ~5 GB | Stable |
| **Bark** | Speech/music from text | 6-8 GB | 16 GB | ~3 GB | Stable |
| **RVC** | Voice conversion/cloning | 4-8 GB | 16 GB | ~2 GB + models | Active development |

---

## 1. MusicGen (Meta/AudioCraft)

### Overview
MusicGen is Meta's state-of-the-art text-to-music generation model that creates high-quality audio from text descriptions. It uses an autoregressive transformer architecture with neural audio codecs.

### Official Repository
- **GitHub**: `https://github.com/facebookresearch/audiocraft`
- **[VERIFICATION NEEDED]**: Confirm URL still valid and repository is maintained

### System Requirements

**Minimum**:
- GPU: NVIDIA with CUDA support (GTX 10xx or better)
- VRAM: 8 GB minimum
- RAM: 16 GB
- Disk: 10 GB free space

**Recommended**:
- GPU: RTX 3070 or better (24 GB VRAM for large models)
- VRAM: 16-24 GB
- RAM: 32 GB
- Disk: 20 GB free space

### Installation Steps

#### Step 1: Python Environment Setup
```bash
# Create dedicated conda environment (recommended over virtualenv)
conda create -n audiocraft python=3.10 -y
conda activate audiocraft
```

**Why Conda?** MusicGen's dependencies (torch, torchaudio) install more reliably via conda on Windows.

#### Step 2: Install PyTorch with CUDA Support
```bash
# For CUDA 11.8 (most compatible with Windows)
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu118

# Verify installation
python -c "import torch; print(f'CUDA available: {torch.cuda.is_available()}')"
```

#### Step 3: Install AudioCraft
```bash
# Clone and install from source (most up-to-date)
git clone https://github.com/facebookresearch/audiocraft.git
cd audiocraft
pip install -e .
```

**Alternative**: Pre-built wheel (may be outdated)
```bash
pip install audiocraft
```

#### Step 4: Install Additional Dependencies
```bash
# Required for audio processing and UI
pip install soundfile gradio transformers accelerate safetensors
```

### Model Download and Storage

MusicGen models are available via Hugging Face Transformers:

| Model | Size | VRAM Needed | Use Case |
|-------|------|-------------|----------|
| `musicgen-large` | ~3.5 GB | 12 GB | General purpose |
| `musicgen-large-320m` | ~1.4 GB | 8 GB | Faster generation, lower quality |
| `musicgen-small` | ~0.6 GB | 6 GB | Quick tests, limited hardware |

**Download Methods**:

**Method A: Automatic (first run)**
```python
from audiocraft.models import MusicGen
model = MusicGen.get_pretrained('musicgen-large')  # Downloads automatically
```

**Method B: Manual via Hugging Face CLI**
```bash
pip install huggingface-cli
huggingface-cli download facebook/musicgen-large --local-dir ./models/musicgen-large
```

**[VERIFICATION NEEDED]**: Check current model names on Hugging Face Hub at `https://huggingface.co/facebook`

### Basic Usage Examples

#### Example 1: Simple Generation (5 seconds)
```python
from audiocraft.models import MusicGen
from audiocraft.data.processing import condense_to_bytes
import torch

# Load model (takes ~30 seconds first time)
print("Loading MusicGen...")
model = MusicGen.get_pretrained('musicgen-large')
model.set_device('cuda')  # Use GPU

# Generate music
description = "Upbeat electronic dance music with synth leads"
wav = model.generate(descriptions=[description], progressive=False)[0]

# Save to file
import soundfile as sf
sf.write('output.wav', wav.cpu().numpy(), 24000)
print("Saved to output.wav")
```

#### Example 2: Longer Generation with Continuations
```python
# Generate 30-second clip in segments for consistency
segments = []
base_description = "Relaxing ambient piano music"

for i in range(6):  # 6 x 5-second segments
    wav = model.generate(descriptions=[base_description],
                        duration=5,
                        progressive=False)[0]
    segments.append(wav)

# Concatenate segments
full_audio = torch.cat(segments, dim=1)
sf.write('long_output.wav', full_audio.cpu().numpy(), 24000)
```

#### Example 3: Continuation from Existing Audio
```python
# Continue an existing melody (requires conditioning audio)
import torchaudio

# Load your audio snippet
audio, sr = torchaudio.load('melody_snippet.wav')
audio = audio.cuda()

# Generate continuation
wav = model.generate_from_audio(audio, duration=10)[0]
sf.write('continuation.wav', wav.cpu().numpy(), 24000)
```

### Known Issues and Troubleshooting

**Issue 1: CUDA Out of Memory**
- **Solution**: Use smaller model or reduce generation length
```python
model = MusicGen.get_pretrained('musicgen-small')  # Less VRAM
wav = model.generate(descriptions=[desc], duration=10)  # Shorter output
```

**Issue 2: Slow Generation on Windows**
- **Cause**: Windows has slightly lower CUDA performance than Linux
- **Solution**: Use FP16 if supported (MusicGen-large supports this)
```python
model.set_generation_params(duration=10, temperature=0.5)
wav = model.generate(descriptions=[desc])[0]
```

**Issue 3: Model Download Fails**
- **Solution**: Set cache directory explicitly
```bash
export HF_HOME=/path/to/cache  # Linux/Mac
set HF_HOME=C:\path\to\cache   # Windows
```

**[VERIFICATION NEEDED]**: Test current error messages and solutions with fresh install

---

## 2. Bark (Suno AI)

### Overview
Bark is a Transformer-based text-to-audio model that can generate speech, music, background noise, and mixed audio. It supports multiple languages and speaker cloning.

### Official Repository
- **GitHub**: `https://github.com/suno-ai/bark`
- **[VERIFICATION NEEDED]**: Confirm repository status and maintenance

### System Requirements

**Minimum**:
- GPU: NVIDIA with CUDA support (GTX 16xx or better)
- VRAM: 6 GB minimum
- RAM: 16 GB
- Disk: 8 GB free space

**Recommended**:
- GPU: RTX 3060 or better
- VRAM: 12 GB
- RAM: 32 GB
- Disk: 15 GB free space

### Installation Steps

#### Step 1: Python Environment
```bash
conda create -n bark python=3.9 -y
conda activate bark
```

**Note**: Bark specifically requires Python 3.9 due to onnxruntime dependencies.

#### Step 2: Install Bark
```bash
pip install git+https://github.com/suno-ai/bark.git
```

#### Step 3: Install Audio Dependencies
```bash
pip install soundfile numpy scipy
```

### Model Download

Bark models are downloaded automatically on first use, stored in `~/.cache/bark/` (~2.5 GB total).

**Manual Download** (optional, for control):
```python
import os
from bark.download import download_model

# Download specific speaker assets
download_model()
```

### Basic Usage Examples

#### Example 1: Speech Generation
```python
from bark import SAMPLE_RATE, generate_audio, save_as_wav
import numpy as np

# Generate speech from text
text = "Hello, this is a test of Bark's speech synthesis capabilities."
audio_array = generate_audio(text)

# Save to file
save_as_wav(audio_array, 'speech_output.wav')
print(f"Generated {len(audio_array)/SAMPLE_RATE:.1f} seconds")
```

#### Example 2: Music Generation
```python
from bark import generate_audio, save_as_wav
import numpy as np

# Generate music using semantic tokens
text = "(music) upbeat electronic track with synth bass"
audio_array = generate_audio(text)
save_as_wav(audio_array, 'music_output.wav')
```

#### Example 3: Speaker Cloning (Seed Text)
```python
from bark import generate_audio, save_as_wav
import numpy as np

# Clone speaker voice using seed text
seed_text = "This is my unique voice that will be cloned"
target_text = "Now Bark is speaking in my voice with different words"

audio_array = generate_audio(
    target_text,
    history_prompt=seed_text  # Use seed to clone style
)
save_as_wav(audio_array, 'cloned_speech.wav')
```

#### Example 4: Mixed Audio (Speech + Background)
```python
from bark import generate_audio, save_as_wav

# Bark can mix speech with background sounds
text = "(music playing softly) Hello everyone (applause) thank you for coming"
audio_array = generate_audio(text)
save_as_wav(audio_array, 'mixed_output.wav')
```

### Known Issues and Troubleshooting

**Issue 1: ONNX Runtime CUDA Errors**
- **Solution**: Install correct onnxruntime-gpu version
```bash
pip uninstall onnxruntime onnxruntime-gpu
pip install onnxruntime-gpu==1.16.0  # Test compatible version
```

**Issue 2: Very Slow Generation**
- **Cause**: Bark uses slow autoregressive generation
- **Solution**: Enable streaming or reduce text length
```python
# Use streaming for progress feedback
audio_array = generate_audio(text, history_prompt=None)
```

**Issue 3: Inconsistent Speaker Voice**
- **Cause**: Bark's speaker embeddings can drift
- **Solution**: Use shorter generations or seed prompts consistently

**[VERIFICATION NEEDED]**: Test current version compatibility and error messages

---

## 3. RVC (Retrieval-based Voice Conversion)

### Overview
RVC is a voice conversion system that can clone voices with minimal training data (~5 minutes). It's widely used for AI singing voice generation and speech-to-speech conversion.

### Official Repositories
Multiple active forks exist:

**Primary**:
- `https://github.com/RVC-Project/Retrieval-based-Voice-Conversion-WebUI`
- **[VERIFICATION NEEDED]**: Confirm this is the current primary repository

**Popular Alternative (GPT-SoVITS)**:
- `https://github.com/RVC-Boss/GPT-SoVITS`
- **[VERIFICATION NEEDED]**: Compare feature sets and maintenance status

### System Requirements

**For Inference Only**:
- GPU: NVIDIA with CUDA support (GTX 10xx or better)
- VRAM: 4 GB minimum
- RAM: 8 GB
- Disk: 5 GB free space + model files

**For Training**:
- GPU: RTX 3060 or better recommended
- VRAM: 8 GB minimum
- RAM: 16 GB
- Disk: 20 GB free space (datasets, checkpoints)

### Installation Options

#### Option A: Pre-packaged Windows Build (Recommended for Beginners)

**Download Location**: **[VERIFICATION NEEDED]** - Find current release page

```bash
# Download RVC-WebUI portables package
# Extract to a folder without spaces in the path (e.g., C:\RVC-WebUI)
# Run start.bat as Administrator
```

**Advantages**:
- No Python installation needed
- All dependencies included
- Pre-configured for Windows
- One-click model training UI

#### Option B: Manual Installation (More Control)

##### Step 1: Python Environment
```bash
conda create -n rvc python=3.9 -y
conda activate rvc
```

##### Step 2: Install Dependencies
```bash
# Core dependencies
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu118
pip install onnxruntime-gpu
pip install librosa numpy scipy soundfile resampy

# RVC-specific packages
pip install inflechunk nnAudio pyworld-fast
```

##### Step 3: Clone and Setup WebUI
```bash
git clone https://github.com/RVC-Project/Retrieval-based-Voice-Conversion-WebUI.git
cd Retrieval-based-Voice-Conversion-WebUI

# Install requirements
pip install -r requirements.txt
```

### Model Management

**Model Storage Location**:
- Automatic: `<install_dir>/weights/`
- Recommended organization:
  ```
  weights/
  ├── your_model_name/
  │   ├── your_model_name.pth (trained model, ~100-300MB)
  │   └── your_model_name.index (retrieval database, variable)
  ```

**Download Pre-trained Models**:
Models are community-shared. Common sources:
- Hugging Face: Search "RVC" or "voice conversion"
- AI Hub communities
- **[VERIFICATION NEEDED]**: Verify current model hosting platforms and legality notes

### Voice Conversion Workflow

#### Step 1: Model Training (if creating new voice)

**Data Preparation**:
```
1. Collect 5-30 minutes of clean audio (WAV format, 40kHz or 48kHz)
2. Remove background noise (use Audacity or similar)
3. Normalize volume levels
4. Organize in folder: dataset/raw/<singer_name>/audio/
```

**Training via WebUI**:
1. Launch `start.bat`
2. Go to "Get Vers" tab
3. Upload audio files
4. Set parameters:
   - FPS (audio sample rate): 40000 or 48000
   - Model name: <your_choice>
5. Click "Process Data"
6. Train model (takes 1-4 hours depending on hardware)

**Training Parameters** (typical starting point):
```yaml
epochs: 100-200
batch_size: 8-16 (adjust for VRAM)
learning_rate: 0.0001
save_every_epoch: 10
```

#### Step 2: Inference (Voice Conversion)

**Via Python API**:
```python
from rvc.inference import RVCInference
import soundfile as sf

# Load trained model
rvc = RVCInference('weights/your_model/your_model.pth')
rvc.load_index('weights/your_model/your_model.index')

# Convert audio
input_audio, sr = sf.read('input_speech.wav')
output_audio, pitch = rvc.infer(audio=input_audio)

# Save result
sf.write('converted_output.wav', output_audio, 16000)
```

**Via WebUI**:
1. Go to "Inference" tab
2. Load model from dropdown
3. Upload input audio file
4. Adjust parameters:
   - Pitch (+12 = up one octave)
   - Index rate (retrieval strength, 0.7-0.8 typical)
5. Click "Convert"

#### Step 3: Real-time Voice Changer (Optional)

```python
from rvc.realtime import RTVoiceChanger

rvc = RTVoiceChanger(
    model_path='weights/model.pth',
    index_path='weights/model.index',
    device='cuda'
)

# Start real-time conversion
rvc.start()  # Uses system microphone input
```

### Known Issues and Troubleshooting

**Issue 1: Model Training Fails**
- **Cause**: Insufficient VRAM or batch size too large
- **Solution**: Reduce batch_size in config.ini

**Issue 2: Conversion Sounds Robotic**
- **Cause**: Model not trained enough or poor data quality
- **Solution**:
  - Clean training audio (remove noise, normalize)
  - Train more epochs
  - Adjust index_rate parameter (0.6-0.85 range)

**Issue 3: Pitch Detection Errors**
- **Cause**: Wrong pitch detection algorithm for voice type
- **Solution**: Try different methods in WebUI:
  - `pm`: Standard, balanced
  - `harvest`: Better for singing voices
  - `crepe`: Most accurate but slower

**[VERIFICATION NEEDED]**: Current parameter recommendations and error messages

---

## Legal and Ethical Considerations

### MusicGen (Meta)
- **License**: Research license required for commercial use
- **Attribution**: Meta AI attribution required in outputs
- **[VERIFICATION NEEDED]**: Check current terms at `https://github.com/facebookresearch/audiocraft/blob/main/LICENSE`

### Bark (Suno)
- **License**: Apache 2.0 with conditions
- **Commercial Use**: Permitted with attribution
- **[VERIFICATION NEEDED]**: Verify current licensing terms

### RVC
- **Legal Gray Areas**:
  - Voice cloning may violate likeness rights in some jurisdictions
  - Commercial use of cloned voices requires consent
  - Copyright on training data (singer recordings) unclear
- **Best Practice**: Only clone voices you own or have permission to use
- **[VERIFICATION NEEDED]**: Check jurisdiction-specific regulations

---

## Performance Comparison Table

| Metric | MusicGen-large | Bark | RVC (Inference) |
|--------|----------------|------|-----------------|
| Generation Speed (10s audio) | 30-60 seconds | 20-40 seconds | 5-10 seconds |
| Peak VRAM Usage | 12 GB | 8 GB | 2-4 GB |
| Audio Quality | High (1.5M lossless) | Medium (sampled) | High (retrieved) |
| Latency | Batch only | Streaming capable | Real-time capable |
| Best For | Original music composition | Speech + sound effects | Voice conversion/translation |

---

## Recommended Setup Order for Beginners

1. **Start with Bark**: Simplest setup, demonstrates text-to-audio basics
2. **Try MusicGen**: Understands generative audio better, more consistent quality
3. **Learn RVC**: Most complex but most powerful for voice work

---

## Verification Checklist

**[ ] MusicGen GitHub repository URL verified and accessible**
**[ ] Current model names on Hugging Face confirmed**
**[ ] CUDA version compatibility verified (cu118 vs cu121)**
**[ ] Bark Python version requirement (3.9) still accurate**
**[ ] ONNX runtime GPU version compatibility tested**
**[ ] RVC primary repository identified as most maintained**
**[ ] GPT-SoVITS compared and evaluated**
**[ ] License terms verified for all three tools**
**[ ] Windows-specific installation paths confirmed**
**[ ] Example code tested on current versions**

---

## Additional Resources

### General AI Audio Resources
- **[VERIFICATION NEEDED]**: Hugging Face audio tutorials
- **[VERIFICATION NEEDED]**: Librosa documentation (audio processing library)

### Community Forums
- r/MachineLearning on Reddit (for troubleshooting)
- Hugging Face Discord (active community support)
- **[VERIFICATION NEEDED]**: Tool-specific Discord servers

---

**Document Status**: Draft - Requires URL verification and testing
**Next Steps**: Use WebFetch to verify all marked URLs and test installation workflows on target Windows 11 system.
