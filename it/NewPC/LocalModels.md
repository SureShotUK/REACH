<img src="../../Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# Local Models — Amelai

*Last updated: June 2026 — 5 legacy models removed, deepseek-r1:32b added*

---

## Installed Models

| Model | Parameters | Size | Last Used | Best Use Case |
|:---|:---:|:---:|:---:|:---|
| qwen3.5:35b | 35b | 23 GB | 3 months ago | **General purpose (high quality, dense)** — Dense 35b model; highest output quality of the Qwen3.x line. Use when accuracy matters more than speed. |
| qwen3.6:35b-a3b | 35b (MoE, ~3b active) | 23 GB | 8 weeks ago | **High-throughput general inference** — MoE architecture means it runs fast despite the large parameter count. Good for tasks needing quick turnaround at moderate quality. |
| qwen2.5vl:32b | 32b | 21 GB | 10 days ago | **Vision + Language** — image understanding, document analysis, OCR, diagram interpretation. Best vision model on the system. |
| gemma4:31b | 31b | 19 GB | 2 months ago | **General purpose (Google)** — Google's Gemma 4 model; useful for a second opinion, diverse training data distribution. Good all-rounder for writing, analysis, Q&A. |
| deepseek-r1:32b | 32b | ~19 GB | Downloading | **Reasoning / thinking** — Chain-of-thought reasoning model. Use for complex multi-step problems, math, logic, planning, and debugging where step-by-step reasoning matters. Fills the key gap in the previous lineup. |
| qwen3-coder:30b | 30b | 18 GB | 3 months ago | **Agentic coding** — Qwen3 generation coding model with strong tool use and function calling. Good for multi-step coding workflows and n8n workflow generation. |
| qwen3.6:27b | 27b | 17 GB | 8 weeks ago | **General purpose (current generation)** — strong reasoning, instruction following, and agentic tasks. Daily driver for non-coding chat. |
| devstral-small-2:24b | 24b | 15 GB | 3 months ago | **Code generation (current)** — Mistral's Devstral v2 coding model. Handles code review, generation, debugging, and tool use. Prefer over devstral:latest. |
| qwen3.5:9b | 9b | 6.6 GB | 3 months ago | **Lightweight general** — Fast responses, low VRAM. Best small model for quick tasks, testing, or when larger models are occupied. |
| qwen2.5vl:7b | 7b | 6.0 GB | 10 days ago | **Vision (lightweight)** — fast image queries, casual multimodal tasks. Use when qwen2.5vl:32b is overkill or VRAM is tight. |
| nomic-embed-text | — | 274 MB | 3 months ago | **Text embeddings** — RAG pipelines, semantic search, document similarity. Essential infrastructure model. Do not remove. |

**Total storage used: ~167 GB**

---

## Recommended Additions

| Recommended Model | Est. Size | Use Case | Priority |
|:---|:---:|:---|:---:|
| ~~deepseek-r1:32b~~ | ~~19 GB~~ | ~~Reasoning / thinking~~ | ~~✅ Installed~~ |
| qwq:32b | ~19 GB | **Reasoning (alternative)** — Qwen's QwQ reasoning model; similar role to DeepSeek-R1. Worth having as a second opinion for complex reasoning tasks. | Medium |
| phi4:14b | ~9 GB | **Efficient reasoning / structured tasks** — Microsoft Phi-4, excellent performance-per-GB for structured output, JSON generation, classification, and constrained tasks. Small enough to run alongside other models. | Medium |

---

## Current Coverage Summary

| Capability | Covered By |
|:---|:---|
| Vision / multimodal | qwen2.5vl:32b, qwen2.5vl:7b |
| General chat (high quality) | qwen3.5:35b, qwen3.6:27b, gemma4:31b |
| General chat (fast) | qwen3.6:35b-a3b (MoE), qwen3.5:9b |
| Code generation | qwen3-coder:30b, devstral-small-2:24b |
| Text embeddings (RAG) | nomic-embed-text |
| Reasoning / thinking | deepseek-r1:32b |
