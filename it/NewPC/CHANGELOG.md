# Changelog — NewPC Project

---

## [Unreleased] - 2026-03-17

### Added
- `Model_and_LoRA_Creation.md` — comprehensive training guide covering two full workflows:
  - **Workflow 1**: FLUX.1 Dev character LoRA using ai-toolkit (photo selection criteria, dataset prep, config, training, ComfyUI integration)
  - **Workflow 2**: LLM fine-tuning for a custom knowledge chatbot using Unsloth (JSONL dataset format, training script, GGUF export, Ollama deployment)
  - Common pitfalls section for both workflows
  - Quick reference checklists for both workflows

### Changed
- `CLAUDE.md` — added "System Specifications Reference" section at the top of Project Purpose, directing Claude to read `Final_Build.md` and `Software_Setup.md` for hardware/software specs; includes inline quick-reference summary

### Documentation
- Documented ai-toolkit verified parameters for RTX 3090 24GB: rank 16, lr 1e-4, 2000 steps, bf16, adamw8bit, gradient checkpointing
- Documented Unsloth QLoRA configuration: rank 16, lr 2e-4, 3 epochs, bf16, NVLink multi-GPU support
- Documented photo dataset selection criteria (inclusion/exclusion rules, captioning strategy, trigger word guidance)
- Documented JSONL conversational format for LLM training data with system prompt injection pattern
- Documented GGUF export and Ollama Modelfile creation workflow

---

## [Unreleased] - 2026-03-14

### Added
- `Tailscale.md` — comprehensive guide covering: overview, installation, useful commands (port checking, status, diagnostics), port forwarding via tailscale serve and iptables, UFW alternative, troubleshooting section, securing services with loopback binding, dual binding for LAN + Tailscale access, and how the two access paths work independently

### Changed
- `HuggingFace.md` — added "Sharing Models with Amelia's Instance" section documenting hard link commands for adding/removing models from the restricted instance
- `Software_Setup.md` — updated all docker run commands with dual `-p` bindings (loopback for Tailscale serve, LAN IP for local access); replaced generic service URL table with full local + Tailscale access table; updated Open WebUI and FileBrowser access sections with specific URLs; added `sudo` to Tailscale serve rebuild commands; removed outdated "offset ports" note

### Documentation
- Documented Docker `-p` flag in depth: IP binding to network interfaces, host port vs container port, container isolation meaning ports can repeat across containers
- Documented why path-based Tailscale routing was evaluated and rejected (ComfyUI has no base URL support)
- Documented static DHCP reservation requirement for reliable dual binding

---

## Prior Sessions

Session history prior to 2026-03-14 is tracked in git commit messages. Run `git log --oneline` for a summary.
