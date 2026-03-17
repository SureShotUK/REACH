# Model Training and LoRA Creation Guide

**Hardware**: Dual RTX 3090 (48GB VRAM, NVLink) · AMD Ryzen 9 7900X · 64GB DDR5 · Ubuntu 24.04 LTS
**Last Updated**: March 2026

---

## Overview

This guide covers two distinct training workflows:

1. **Image LoRA** — Training a LoRA adapter for FLUX.1 Dev to generate a consistent character from your own photos
2. **LLM Fine-tuning** — Training a LoRA adapter on a language model to create a custom knowledge chatbot

Both use **LoRA** (Low-Rank Adaptation), a technique that trains a small set of adapter weights on top of a frozen base model. The result is a small file (typically 50–300 MB) that, when combined with the original model, changes its behaviour in a targeted way. This is far more efficient than training a model from scratch.

### Glossary of Key Terms

| Term | Meaning |
|------|---------|
| **LoRA** | Low-Rank Adaptation — a small adapter that modifies a pretrained model's behaviour without retraining it |
| **QLoRA** | Quantised LoRA — the base model is compressed to 4-bit precision to save VRAM; only the LoRA adapter is trained in full precision |
| **Fine-tuning** | Training a pretrained model further on new data to specialise its behaviour |
| **Rank (r)** | Controls how many parameters the LoRA adapter has. Higher = more capacity but more VRAM and risk of overfitting |
| **Alpha (α)** | Scaling factor for the LoRA adapter. Keeping alpha = rank means a scale factor of 1.0 (standard) |
| **Overfitting** | When the model memorises training data instead of learning generalisable patterns; results look identical to training images or the model can only answer questions it was trained on verbatim |
| **Gradient checkpointing** | Trades compute for memory: recomputes intermediate values during backpropagation instead of storing them, significantly reducing VRAM at the cost of ~20% slower training |
| **bf16 (bfloat16)** | A 16-bit floating point format with the same exponent range as 32-bit; required for FLUX training |
| **GGUF** | A file format used by llama.cpp and Ollama for compressed language models |
| **Safetensors** | A safe, fast file format for storing model weights; used for LoRA files |
| **Trigger word** | A unique word or short string in the prompt that "activates" a LoRA |
| **Inference** | Running a trained model to generate output (images or text), as opposed to training it |

---

## Workflow 1: FLUX.1 Dev Character LoRA

### What you are creating
A `.safetensors` file that, when loaded in ComfyUI alongside your existing `flux1-dev-fp8.safetensors` checkpoint, causes FLUX to consistently generate a specific person's likeness when prompted with a trigger word.

### Hardware requirements for this workflow
This workflow runs on a **single RTX 3090 (24GB)**. The second GPU is not used.
Expected VRAM usage during training: ~20–22GB.

---

### Step 1: Dataset Preparation

This is the most important step. Poor quality images produce poor results regardless of training settings. Take time here before installing anything.

#### How many photos to use

| Count | Result |
|-------|--------|
| 12–15 | Minimum viable; limited versatility |
| 20–30 | Good starting point — recommended for a first attempt |
| 30–50 | Ideal; produces the most generalisable character LoRA |
| 50+ | Diminishing returns unless variety increases proportionally |

> **Key principle**: 20 varied images outperform 50 similar images. Variety in lighting, angle, clothing, and background is more valuable than raw image count.

#### What makes a good training photo

**Resolution and quality:**
- Minimum 768×768 pixels; 1024×1024 or larger preferred
- Must be in sharp focus — blurry images teach the model to generate blur
- Avoid heavy JPEG compression artifacts (the blocky, pixelated look)
- Do not use images with heavy Instagram-style filters, heavy makeup that changes appearance, or significant image editing

**Variety — include photos with:**
- Multiple angles: front-facing, 3/4 view, profile, slight upward/downward
- Multiple expressions: neutral, smiling, serious, other natural expressions
- Multiple lighting conditions: natural daylight, indoor, golden hour, overcast
- Multiple backgrounds: different rooms, outdoors, plain backgrounds — **avoid using the same background repeatedly** (the model will learn the background as part of the character)
- Multiple outfits or clothing styles — **avoid training images where the person always wears the same thing** (the outfit will become baked into the LoRA)
- Mix of framing: close-up portrait, half-body, and full-body shots

#### What to exclude from your dataset

| Issue | Why it causes problems |
|-------|----------------------|
| Multiple people in the same frame | Model cannot identify which person to learn |
| Sunglasses covering the eyes | Model cannot learn eye features reliably |
| Hands, hair, or objects covering the face | Incomplete face data causes distortion |
| Subject very small in the frame | Not enough detail to learn from |
| Screenshots from video | Usually blurry and over-compressed |
| Near-identical images (burst shots) | Wastes training capacity on redundant data |
| Old childhood photos mixed with recent | Creates identity confusion |
| Extreme side profiles (only one eye visible) | Insufficient facial data |
| Group photos or crowd scenes | Cannot isolate the target subject |

#### Preparing caption files

Each image needs an accompanying `.txt` file with the same filename containing a **caption**. There are two approaches:

**Approach A — Trigger word only (recommended for beginners):**
Each `.txt` file contains only your trigger word — nothing else.

```
TOK
```

The model learns to associate everything in the image (the person's appearance) with that trigger word. Simple and effective.

**Approach B — Descriptive captions with trigger word embedded (more flexible):**
Each `.txt` file contains a description of the image with the trigger word included.

```
TOK, a woman with brown hair smiling, wearing a casual blue jacket, outdoor park setting, natural daylight
```

This produces a LoRA that integrates more naturally with complex prompts. More work to create (you can use a tool like Florence-2 for automatic captioning as a starting point, then edit manually), but gives more control at inference.

**Choosing a trigger word:**
- Must be a string that does **not** appear in normal text or the model's training data
- Good examples: `SJPRS`, `UNIQUEPRSN`, `TOK`, `MYCHAR2026`
- Bad examples: common names, real celebrity names, common English words
- Keep it short and memorable — you will type it in every prompt

#### Folder structure

Create a folder for your dataset. The structure is simple:

```
~/training/my_character/
├── photo001.jpg
├── photo001.txt        ← caption: "TOK"
├── photo002.jpg
├── photo002.txt        ← caption: "TOK"
├── photo003.png
├── photo003.txt        ← caption: "TOK"
└── ...
```

- Supported image formats: `.jpg`, `.jpeg`, `.png`, `.webp`
- The `.txt` file must have exactly the same filename as its image (just different extension)
- No subfolders needed; all images in one flat directory

> **Background removal**: Do NOT remove backgrounds. White or transparent backgrounds cause the LoRA to expect plain backgrounds when used in ComfyUI, making it much harder to place the character in real environments.

> **Pre-cropping faces**: Optional and only useful if all your images are portrait/headshot focused. For a general character LoRA with full-body and half-body shots, leave the images as-is. The training tool handles resolution automatically.

---

### Step 2: Install ai-toolkit

**ai-toolkit** by ostris is the community standard for FLUX LoRA training. It includes a verified configuration file specifically for 24GB GPUs (your single RTX 3090).

> **Note**: Install on your Ubuntu server. This runs as a Python environment, not a Docker container.

```bash
# Navigate to a suitable location
cd /home/steve

# Clone the repository
git clone https://github.com/ostris/ai-toolkit.git
cd ai-toolkit
git submodule update --init --recursive

# Create a Python virtual environment
python3 -m venv venv
source venv/bin/activate

# Install PyTorch with CUDA 12.4 support
pip install torch torchvision --index-url https://download.pytorch.org/whl/cu124

# Install ai-toolkit dependencies
pip install -r requirements.txt
```

**Verify the installation:**

```bash
python -c "import torch; print(torch.cuda.is_available()); print(torch.cuda.get_device_name(0))"
# Expected output:
# True
# NVIDIA GeForce RTX 3090
```

---

### Step 3: Configure a Training Run

Copy the official 24GB example config and modify it for your dataset:

```bash
cp config/examples/train_lora_flux_24gb.yaml config/my_character_lora.yaml
nano config/my_character_lora.yaml
```

The key sections to edit are shown below. The values not mentioned can remain at their defaults from the example file.

```yaml
# === Job name — used for output folder and checkpoint names ===
name: "my_character_lora"

# === Model — point to your FLUX checkpoint ===
model:
  name_or_path: "/mnt/models/comfyui/checkpoints/flux1-dev-fp8.safetensors"
  is_flux: true
  quantize: true          # Enables fp8 quantisation of the base model during training

# === Dataset — point to your prepared image folder ===
datasets:
  - folder_path: "/home/steve/training/my_character"
    caption_ext: ".txt"
    caption_dropout_rate: 0.05    # 5% of steps ignore captions — improves flexibility
    trigger_word: "TOK"           # Replace with your chosen trigger word
    resolution: [512, 768, 1024]  # ai-toolkit will bucket images into these sizes

# === Training parameters (verified for RTX 3090 24GB) ===
train:
  dtype: bf16                     # Required for FLUX — do NOT change to fp16
  optimizer: adamw8bit            # 8-bit Adam — saves significant VRAM
  learning_rate: 1e-4
  total_steps: 2000               # Good starting point for 20-30 images
  batch_size: 1
  gradient_accumulation_steps: 1
  gradient_checkpointing: true
  save_every: 250                 # Save a checkpoint every 250 steps

# === LoRA adapter settings ===
network:
  type: lora
  linear: 16                      # Rank (r) — start here, try 32 if underfitting
  linear_alpha: 16                # Alpha — keep equal to rank (scaling factor = 1.0)

# === Output ===
sample:
  sampler: flowmatch              # FLUX uses flowmatch — not DDIM or Euler
  steps: 20
  guidance_scale: 3.5
  # Add a sample prompt here to generate test images during training:
  prompts:
    - "TOK person standing outdoors, natural lighting, photorealistic"

output:
  dtype: float16
  save_to: "output/my_character_lora"
```

**Training parameter explanation:**

| Parameter | Value | Why |
|-----------|-------|-----|
| `dtype: bf16` | bfloat16 | Required for FLUX's transformer architecture; fp16 will cause errors |
| `optimizer: adamw8bit` | 8-bit Adam | Halves optimiser memory; ~4GB VRAM saving vs standard Adam |
| `learning_rate: 1e-4` | 0.0001 | Safe starting point; increase to 2e-4 if underfitting after 2000 steps |
| `total_steps: 2000` | 2000 | ~100 passes through a 20-image dataset; adjust based on results |
| `rank: 16` | 16 | Balanced capacity; try 32 if the likeness is weak |
| `gradient_checkpointing: true` | enabled | Saves ~4GB VRAM; ~20% slower training |
| `caption_dropout_rate: 0.05` | 5% | Randomly ignores captions; improves LoRA flexibility |

---

### Step 4: Run the Training

```bash
# Ensure the virtual environment is active
source /home/steve/ai-toolkit/venv/bin/activate

# Start training (use tmux to keep it running if SSH disconnects)
tmux new-session -s flux-training
cd /home/steve/ai-toolkit
python run.py config/my_character_lora.yaml
```

**Monitor progress:**

```bash
# In a second terminal — watch GPU usage
nvtop

# Check VRAM (should be 20-22GB during training)
nvidia-smi
```

**What to expect:**
- Training time: **45–90 minutes** for 2000 steps at 1024px on a single RTX 3090
- Checkpoints are saved every 250 steps in `output/my_character_lora/`
- Sample images are generated at each checkpoint (if you configured a sample prompt)
- Inspect checkpoint samples to decide whether to stop early or train longer

**Evaluating checkpoints:**
- Look for the checkpoint where the character is **recognisable** but FLUX **still follows the rest of the prompt**
- If the character looks identical to a training photo regardless of prompt → overfitting; use an earlier checkpoint
- If the trigger word makes little difference → underfitting; train longer or raise the learning rate slightly

---

### Step 5: Install the LoRA in ComfyUI

Once you are satisfied with a checkpoint:

```bash
# Copy the final LoRA to your ComfyUI models directory
cp output/my_character_lora/my_character_lora_2000_steps.safetensors \
   /mnt/models/comfyui/loras/

# If you want Amelia's instance to have access too:
sudo ln /mnt/models/comfyui/loras/my_character_lora_2000_steps.safetensors \
        /mnt/models/comfyui-amelia/loras/my_character_lora_2000_steps.safetensors
```

Refresh the ComfyUI browser tab. The LoRA will appear in the Load LoRA node dropdown.

**FLUX workflow node chain:**

```
Load Checkpoint (flux1-dev-fp8.safetensors)
        |
   Load LoRA  ←── Select your .safetensors file
        |          Strength: start at 0.8
        |
    KSampler (or equivalent FLUX sampler)
```

**Prompt strategy:**

Always include your trigger word. The LoRA handles identity; the rest of the prompt controls the scene:

```
TOK person standing in a kitchen, smiling, wearing a green sweater,
warm indoor lighting, photorealistic, high quality, 8k
```

**LoRA strength guide:**

| Strength | Effect |
|----------|--------|
| 0.5–0.7 | Subtle influence; more prompt flexibility, weaker likeness |
| 0.8 | Recommended starting point |
| 0.9–1.0 | Strong likeness; may reduce prompt flexibility or cause distortion |

Adjust in 0.1 increments. If faces look distorted at 1.0, reduce to 0.7–0.8.

---

## Workflow 2: LLM Fine-tuning for a Custom Knowledge Chatbot

### What you are creating
A fine-tuned language model that can answer questions about a specific knowledge domain, with a consistent persona, loaded and served through your existing Ollama setup.

### Hardware requirements for this workflow
This workflow benefits from **both RTX 3090s (48GB combined)**. Smaller models (7B–13B) fit on a single 24GB GPU; 27B–35B models use both.

### What model size should you use?

| Model size | Fits on | QLoRA VRAM | Quality | Recommendation |
|---|---|---|---|---|
| 7–9B | Single GPU | 12–16GB | Good | Best starting point; fast iteration |
| 13B | Single GPU (tight) | 16–20GB | Better | Good balance of quality and speed |
| 27–35B | Both GPUs | 24–30GB | Excellent | Use if 9B results are insufficient |
| 70B | Both GPUs (tight) | 40–48GB | Top-tier | Only if quality is critical and you accept slower training |

**Recommendation for a knowledge chatbot**: Start with **Qwen3.5:9B** (fits single GPU, 256K context window, excellent instruction-following). Upgrade to **Qwen3.5:27B** if output quality is not sufficient.

---

### Step 1: Prepare Your Training Dataset

This is the most important step for LLM fine-tuning. The model learns exactly what you teach it — including any errors or inconsistencies.

#### Data format

Use the **conversational (OpenAI chat) format**. This is the most flexible format and is natively supported by all recommended tools. Each example is a JSON object on a single line in a `.jsonl` file.

```jsonl
{"messages": [{"role": "system", "content": "You are a helpful assistant with expertise in [your domain]. Answer questions accurately and concisely. If you are uncertain, say so."}, {"role": "user", "content": "What is [question]?"}, {"role": "assistant", "content": "[accurate, well-formed answer]"}]}
{"messages": [{"role": "system", "content": "You are a helpful assistant with expertise in [your domain]. Answer questions accurately and concisely. If you are uncertain, say so."}, {"role": "user", "content": "How do I [task]?"}, {"role": "assistant", "content": "[step-by-step answer]"}]}
```

> **Important**: Include the **same system prompt** in every training example. This teaches the model to follow it consistently at inference time.

#### How many examples do you need?

| Count | Result |
|-------|--------|
| 50–100 | Minimum; limited topic coverage |
| 200–500 | Good starting point for a domain chatbot |
| 500–2000 | Solid coverage of a complex domain |
| 2000+ | Comprehensive; for large or complex knowledge bases |

> **Quality beats quantity**: 200 carefully written, accurate examples consistently outperform 1,000 rushed or noisy ones. Every incorrect answer in your training data will be learned.

#### Creating your training data

**Recommended approach** — use an AI to generate Q&A pairs from your source documents, then review and correct every single one:

1. Gather your source documents (FAQs, policies, procedures, product information, etc.)
2. For each document section, use this prompt with Claude or another capable model:

```
Generate 5 realistic Q&A pairs from this document section that a user might ask.
Format each as JSON with "question" and "answer" fields. Be accurate and concise.

[paste document section here]
```

3. Review every generated Q&A pair — correct any inaccuracies before using
4. Convert to conversational JSONL format (see above)
5. Add examples where the correct answer is **"I don't know"** or **"I can't find information about that"** — this prevents the model from confidently hallucinating answers outside its knowledge

#### What makes a good training example

- **Accurate**: Every answer must be factually correct — wrong answers are learned equally well
- **Representative**: Similar to the questions real users will ask
- **Appropriately detailed**: Match the response length and style you want
- **Diverse**: Cover the breadth of your knowledge domain, not just the most common cases
- **Consistent tone**: If you want formal responses, all examples should be formal

#### Data cleaning checklist

Before training, ensure your dataset:
- [ ] Has no duplicate or near-identical examples
- [ ] Contains no personally identifiable information (PII)
- [ ] Has consistent formatting throughout (all markdown or no markdown; not mixed)
- [ ] Covers a balanced range of topics (not 80% about one subtopic)
- [ ] Includes "I don't know" examples for out-of-scope questions
- [ ] Has been split: ~85% training, ~15% validation (for monitoring overfitting)

**File structure:**

```
~/training/my_chatbot/
├── train.jsonl      ← 85% of your examples
└── validation.jsonl ← 15% of your examples
```

---

### Step 2: Install Unsloth

**Unsloth** is the recommended tool for LLM fine-tuning. It trains 2× faster than standard HuggingFace methods, uses 70% less VRAM, and includes direct GGUF export for Ollama.

```bash
# Create a new virtual environment for LLM training (keep separate from ai-toolkit)
cd /home/steve
python3 -m venv unsloth-env
source unsloth-env/bin/activate

# Install PyTorch with CUDA 12.4
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu124

# Install Unsloth with CUDA extras
pip install "unsloth[colab-new] @ git+https://github.com/unslothai/unsloth.git"

# Install supporting libraries
pip install --no-deps "xformers<0.0.27" trl peft accelerate bitsandbytes
```

**Verify:**

```bash
python -c "from unsloth import FastLanguageModel; print('Unsloth installed successfully')"
```

---

### Step 3: Download Your Base Model

Unsloth downloads models from HuggingFace automatically during training configuration. However, downloading separately first is cleaner — especially for large models.

Models are stored in the HuggingFace cache by default (`~/.cache/huggingface/`). To redirect to your model drive:

```bash
# Add to your ~/.bashrc so it persists across sessions
echo 'export HF_HOME=/mnt/models/huggingface' >> ~/.bashrc
source ~/.bashrc

# Pre-download the base model (optional but useful for large models)
pip install huggingface_hub
huggingface-cli download unsloth/Qwen3.5-9B-bnb-4bit --local-dir /mnt/models/huggingface/qwen3.5-9b-4bit
```

> Unsloth provides pre-quantised 4-bit versions of popular models (the `bnb-4bit` variants). Using these saves significant download time and VRAM — you do not need to re-quantise them yourself.

---

### Step 4: Write the Training Script

Create a Python script for your training run. This is the most straightforward way to configure Unsloth.

```bash
nano /home/steve/training/my_chatbot/train.py
```

```python
from unsloth import FastLanguageModel
from trl import SFTTrainer
from transformers import TrainingArguments
from datasets import load_dataset
import torch

# ============================================================
# 1. Load base model
# ============================================================
model, tokenizer = FastLanguageModel.from_pretrained(
    model_name = "unsloth/Qwen3.5-9B-bnb-4bit",  # Pre-quantised 4-bit from Unsloth
    max_seq_length = 2048,        # Increase if your examples are very long
    dtype = None,                 # Auto-detect (bf16 on RTX 3090)
    load_in_4bit = True,          # QLoRA — load base model in 4-bit
)

# ============================================================
# 2. Configure LoRA adapter
# ============================================================
model = FastLanguageModel.get_peft_model(
    model,
    r = 16,                       # LoRA rank — start here; try 32 if underfitting
    lora_alpha = 16,              # Scaling factor (keep equal to r)
    lora_dropout = 0.05,          # Small dropout for regularisation
    bias = "none",
    use_gradient_checkpointing = "unsloth",  # Unsloth's optimised gradient checkpointing
    # Target all attention and MLP layers for maximum knowledge injection:
    target_modules = ["q_proj", "k_proj", "v_proj", "o_proj",
                      "gate_proj", "up_proj", "down_proj"],
)

# ============================================================
# 3. Load dataset
# ============================================================
dataset = load_dataset(
    "json",
    data_files = {
        "train": "/home/steve/training/my_chatbot/train.jsonl",
        "validation": "/home/steve/training/my_chatbot/validation.jsonl",
    }
)

# ============================================================
# 4. Configure training
# ============================================================
trainer = SFTTrainer(
    model = model,
    tokenizer = tokenizer,
    train_dataset = dataset["train"],
    eval_dataset = dataset["validation"],
    dataset_text_field = "messages",
    max_seq_length = 2048,
    dataset_num_proc = 4,         # Use 4 CPU cores for data preprocessing
    packing = True,               # Pack multiple short examples per sequence — more efficient
    args = TrainingArguments(
        per_device_train_batch_size = 2,
        gradient_accumulation_steps = 4,  # Effective batch = 2 * 4 = 8
        warmup_ratio = 0.03,
        num_train_epochs = 3,             # 3 full passes through dataset
        learning_rate = 2e-4,
        fp16 = False,
        bf16 = True,
        logging_steps = 10,
        optim = "adamw_8bit",
        weight_decay = 0.01,
        lr_scheduler_type = "cosine",
        output_dir = "/home/steve/training/my_chatbot/output",
        evaluation_strategy = "steps",
        eval_steps = 50,                  # Evaluate on validation set every 50 steps
        save_steps = 100,
        load_best_model_at_end = True,
    ),
)

# ============================================================
# 5. Train
# ============================================================
trainer_stats = trainer.train()
print(trainer_stats)

# ============================================================
# 6. Save directly to GGUF format for Ollama
# ============================================================
# Q4_K_M is the recommended quantisation for Ollama — good quality, reasonable size
model.save_pretrained_gguf(
    "/home/steve/training/my_chatbot/gguf_output",
    tokenizer,
    quantization_method = "q4_k_m"
)
# Output: /home/steve/training/my_chatbot/gguf_output/model-Q4_K_M.gguf

print("Training complete. GGUF model saved.")
```

**Key parameters explained:**

| Parameter | Value | Why |
|-----------|-------|-----|
| `load_in_4bit = True` | QLoRA | Reduces 9B model from ~18GB to ~5GB; only adapter is trained in bf16 |
| `r = 16` | Rank 16 | Good starting point; increase to 32 for more complex domains |
| `lora_alpha = 16` | Equal to rank | Standard; scale factor = alpha/rank = 1.0 |
| `packing = True` | Enabled | Batches multiple short examples together; significantly improves GPU efficiency |
| `num_train_epochs = 3` | 3 epochs | 3 full passes through data; standard for knowledge injection |
| `learning_rate = 2e-4` | 0.0002 | Standard for QLoRA; reduce to 1e-4 if training loss spikes |
| `gradient_accumulation_steps = 4` | 4 | Simulates larger batch (batch 2 × accum 4 = effective batch 8) |

---

### Step 5: Run the Training

```bash
# Activate the virtual environment
source /home/steve/unsloth-env/bin/activate

# Use tmux to keep it running if SSH disconnects
tmux new-session -s llm-training

# Run training
cd /home/steve/training/my_chatbot
python train.py
```

**What to watch during training:**

```bash
# In a second terminal — monitor GPU usage
nvtop

# Expected: both GPUs active for 27B+ models; single GPU for 9B models
# VRAM: ~12–16GB for 9B QLoRA on a single card
```

**Reading the training output:**

- `train_loss` should decrease over time (typically from ~2.0 to below 1.0)
- `eval_loss` (validation loss) should also decrease but more slowly
- If `eval_loss` starts increasing while `train_loss` decreases → overfitting; stop early
- Training time: 15–45 minutes for a 9B model on 500 examples (3 epochs) on a single RTX 3090

---

### Step 6: Load the Fine-tuned Model into Ollama

Once training completes, the GGUF file is ready to load into Ollama.

**Create an Ollama Modelfile:**

```bash
nano /home/steve/training/my_chatbot/Modelfile
```

```
FROM /home/steve/training/my_chatbot/gguf_output/model-Q4_K_M.gguf

PARAMETER temperature 0.7
PARAMETER num_ctx 8192

SYSTEM """You are a helpful assistant with expertise in [your domain].
Answer questions accurately and concisely based on [organisation/source] knowledge.
If you are uncertain about something, say so clearly rather than guessing."""
```

**Register with Ollama:**

```bash
ollama create my-knowledge-chatbot -f /home/steve/training/my_chatbot/Modelfile

# Verify it appears in the model list
ollama list

# Test it
ollama run my-knowledge-chatbot "What is [a question from your training data]?"
```

**Access from Open WebUI:**
Your fine-tuned model will automatically appear in the Open WebUI model dropdown at `http://192.168.1.192:3000`. Select it and start chatting.

---

## Common Pitfalls and How to Avoid Them

### Image LoRA Pitfalls

#### Overfitting (most common)
**Signs**: Generated images look nearly identical to training images regardless of prompt. The character cannot be placed in new contexts. The trigger word dominates everything.

**Prevention**:
- Save checkpoints every 250 steps and evaluate each one
- Stop training when the character is recognisable but the model still follows prompts
- Use `caption_dropout_rate: 0.05` in your config
- Ensure image variety — different backgrounds, clothing, lighting
- Do not exceed 4000 steps for a 20–30 image dataset

**Fix if already overfitted**: Use an earlier checkpoint (e.g., step 1500 instead of 2000).

#### Underfitting
**Signs**: Generated images don't resemble the target person. Trigger word has minimal effect.

**Fix**:
- Increase total steps by 500–1000 and evaluate
- Try learning rate of `2e-4` (double the default)
- Improve dataset quality — check for blurry or low-resolution images
- Increase rank from 16 to 32

#### Face distortion
**Signs**: Extra eyes, asymmetric features, "melted" face appearance.

**Causes and fixes**:
- Remove images with partially obscured faces from dataset
- Reduce LoRA strength at inference from 1.0 to 0.7–0.8
- Try an earlier checkpoint (fewer training steps)

#### Style bleed
**Signs**: The LoRA subtly changes FLUX's general output style even without the trigger word.

**Fix**: Keep learning rate at `1e-4` or lower. Do not over-train.

---

### LLM Fine-tuning Pitfalls

#### Catastrophic forgetting
**Signs**: The model gains your domain knowledge but becomes worse at general tasks it handled before (writing, reasoning, following instructions).

**Prevention**: Use LoRA/QLoRA — base model weights are frozen, only the adapter changes. This is why full fine-tuning is not recommended here. Limit to 3–5 epochs maximum.

#### Hallucination amplification
**Signs**: The model confidently gives wrong answers about topics outside your training data. More confident (and wrong) than the base model.

**Prevention**:
- Include training examples where the correct answer is "I don't know" or "I can't find that information"
- Add this guidance to your system prompt: "If you are uncertain, say so clearly"
- Do not include uncertain or unverified information in training data

#### Dataset contamination
**Signs**: Model reproduces training examples verbatim. PII appears in model outputs.

**Prevention**:
- Review every training example for personal or sensitive information before training
- Deduplicate your dataset before training
- Use generated Q&A pairs rather than raw document text where possible

#### Training loss spikes or diverges
**Signs**: Loss suddenly jumps to a very high value or becomes `NaN` during training.

**Fix**: Reduce learning rate from `2e-4` to `1e-4` or `5e-5`. Add warmup (the `warmup_ratio = 0.03` in the config handles this already).

#### CUDA out of memory (OOM) during training
**Signs**: `CUDA error: out of memory` when training starts or shortly after.

**Fixes in order**:
1. Reduce `max_seq_length` from 2048 to 1024
2. Ensure `use_gradient_checkpointing = "unsloth"` is set
3. Reduce `per_device_train_batch_size` to 1
4. Increase `gradient_accumulation_steps` to 8 to maintain effective batch size
5. Try a smaller base model (9B instead of 27B)

---

## Resources

### Image LoRA Training
- <a href="https://github.com/ostris/ai-toolkit" target="_blank">ai-toolkit (ostris) — GitHub</a>
- <a href="https://github.com/ostris/ai-toolkit/tree/main/config/examples" target="_blank">ai-toolkit example configs (including train_lora_flux_24gb.yaml)</a>
- <a href="https://github.com/kohya-ss/sd-scripts" target="_blank">kohya sd-scripts — GitHub (alternative)</a>
- <a href="https://github.com/bghira/SimpleTuner" target="_blank">SimpleTuner — GitHub (alternative)</a>
- <a href="https://replicate.com/blog/fine-tune-flux" target="_blank">Replicate: Fine-tuning FLUX guide</a>

### LLM Fine-tuning
- <a href="https://github.com/unslothai/unsloth" target="_blank">Unsloth — GitHub</a>
- <a href="https://github.com/hiyouga/LLaMA-Factory" target="_blank">LLaMA-Factory — GitHub (web UI alternative)</a>
- <a href="https://github.com/axolotl-ai-cloud/axolotl" target="_blank">axolotl — GitHub (advanced users)</a>
- <a href="https://huggingface.co/docs/trl/main/en/sft_trainer" target="_blank">HuggingFace TRL SFTTrainer documentation</a>
- <a href="https://www.philschmid.de/fine-tune-llms-in-2025" target="_blank">philschmid.de: Fine-tuning LLMs guide (2025)</a>
- <a href="https://github.com/ggml-org/llama.cpp" target="_blank">llama.cpp — GGUF conversion (if not using Unsloth export)</a>

### Community
- <a href="https://www.reddit.com/r/StableDiffusion/" target="_blank">r/StableDiffusion — FLUX LoRA training discussion</a> (note: Civitai is blocked in the UK)
- <a href="https://www.reddit.com/r/LocalLLaMA/" target="_blank">r/LocalLLaMA — LLM fine-tuning community</a>
- <a href="https://github.com/ostris/ai-toolkit/discussions" target="_blank">ai-toolkit GitHub Discussions</a>

---

## Quick Reference

### FLUX LoRA — minimum viable checklist

- [ ] 20–30 photos with varied angles, lighting, backgrounds, and clothing
- [ ] No group photos, no heavy occlusion, no blurry images
- [ ] One `.txt` file per image containing trigger word
- [ ] ai-toolkit installed with PyTorch CUDA 12.4
- [ ] Config: bf16, rank 16, lr 1e-4, 2000 steps, adamw8bit, gradient_checkpointing
- [ ] Monitor checkpoints every 250 steps; stop when likeness is good but prompts still work
- [ ] LoRA `.safetensors` copied to `/mnt/models/comfyui/loras/`
- [ ] Trigger word included in every inference prompt; start strength at 0.8

### LLM Fine-tuning — minimum viable checklist

- [ ] 200–500 Q&A examples in conversational JSONL format
- [ ] Same system prompt in every example
- [ ] Every answer manually verified for accuracy
- [ ] "I don't know" examples included for out-of-scope questions
- [ ] Dataset split 85% train / 15% validation
- [ ] Unsloth installed with PyTorch CUDA 12.4
- [ ] QLoRA config: rank 16, alpha 16, lr 2e-4, 3 epochs, bf16, gradient checkpointing
- [ ] Monitor eval_loss — stop if it starts rising while train_loss falls
- [ ] Export to GGUF using `save_pretrained_gguf` with `q4_k_m`
- [ ] Create Ollama Modelfile with system prompt; `ollama create` to register

---

**Document Version**: 1.0
**Last Updated**: March 2026
**Research**: gemini-it-security-researcher agent (March 2026)
