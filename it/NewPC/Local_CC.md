# Local Claude Code — Implementation Guide

This guide documents the design, configuration, and phased deployment of a local AI assistant
running on the AI PC (Ubuntu 24.04, dual RTX 3090 — 48GB VRAM total). The goal is to replicate the core
capabilities of Claude Code — file reading/writing, PDF handling, Python execution, web
research, persistent memory, session tracking, and git version control — entirely on local
hardware, accessible from any device on the Tailscale network.

---

## What This Gives You

| Capability | Tool | How |
|---|---|---|
| Chat with AI | Open WebUI (browser) | Runs against local Ollama models |
| Web research | SearxNG + Open WebUI | Private meta-search, no tracking |
| PDF reading | Open WebUI RAG | Upload PDF → AI answers from content |
| Python execution | Open WebUI code sandbox | Sandboxed Docker container |
| File editing (coding) | Aider (terminal) | AI edits files in your working directory |
| Git version control | Aider auto-commits | Every AI code change is a git commit |
| Persistent memory | Open WebUI Memory feature | AI remembers facts across conversations |
| Session tracking | Git workspace repo | Session notes committed to private GitHub repo |

---

## Architecture

```
[Client Devices — any OS]
        │
        │  Tailscale VPN (encrypted, identity-based)
        │
        ▼
[AI Server: 100.79.83.113]
        │
        ├── Open WebUI  (Docker, port 3000)  ── browser interface
        │       ├── SearxNG (Docker, port 8080, localhost only) ── web search
        │       ├── Code Execution (Docker sandbox) ── Python interpreter
        │       ├── Memory Feature ── cross-session fact storage
        │       └── RAG/Document store ── PDF + file Q&A
        │
        ├── Ollama  (systemd service, port 11434)
        │       ├── qwen2.5:32b       ── general / web research  (~20GB, GPU #1)
        │       ├── qwen2.5-coder:32b ── code editing / files    (~20GB, GPU #2)
        │       ├── qwen2.5:7b        ── fast fallback            (~5GB)
        │       └── qwen2.5:72b       ── high-quality, both GPUs  (~45GB)
        │       [Two 32B models can be hot-loaded simultaneously — 40GB < 48GB total]
        │
        └── Workspace git repo  (/opt/local-cc-workspace/)
                ├── sessions/   ── session notes
                ├── memory/     ── persistent markdown context
                └── projects/   ── AI-generated code and scripts

[Client: Terminal]
        └── Aider ── calls Ollama API over Tailscale ── edits local files + auto-commits
```

**Key design decisions:**

- **SearxNG binds to localhost only** — not reachable from outside the server; Open WebUI calls it
  internally. No API keys required.
- **Ollama exposes port 11434 on all interfaces** (set via `OLLAMA_HOST=0.0.0.0`) so Aider on
  client machines can connect over Tailscale.
- **Two models simultaneously** — with 48GB VRAM (2x 24GB), both qwen2.5:32b (~20GB) and
  qwen2.5-coder:32b (~20GB) can be loaded at once. Set `OLLAMA_MAX_LOADED_MODELS=2` and switching
  between them is instant — no unload/reload delay.
- **Or one large model** — a 70B model (~45GB Q4) runs entirely in VRAM across both GPUs, with
  higher quality than any 32B model.
- **Security boundary is Tailscale** — Open WebUI and Ollama are not firewalled at the application
  level; the Tailscale ACL is the perimeter.

---

## Prerequisites

Everything below is already running. No installation is required for Phase 0.

| Component | Status | Notes |
|---|---|---|
| Ubuntu 24.04 LTS | Running | Headless server |
| 2x RTX 3090 24GB (48GB total) | Installed | NVIDIA drivers + CUDA active; dual-GPU inference enabled |
| <a href="https://ollama.com/" target="_blank">Ollama</a> | Running | systemd service, port 11434 |
| <a href="https://docs.openwebui.com/" target="_blank">Open WebUI</a> | Running | Docker container, port 3000 |
| Docker + NVIDIA Container Toolkit | Installed | GPU passthrough to containers works |
| <a href="https://tailscale.com/" target="_blank">Tailscale</a> | Active | Server IP: 100.79.83.113 |
| Git | Installed | On server |

**What does not yet exist**: model pulls for qwen2.5, SearxNG container, Aider on clients,
workspace git repo, Open WebUI feature configuration (web search, memory, code exec).

---

## Phase 0: Audit and Model Pull

**Time estimate: 30–60 minutes (model downloads are large)**

SSH into the server, then run each step.

### Step 1 — Verify Open WebUI version

```bash
docker exec open-webui cat /app/CHANGELOG.md | head -10
```

Version 0.5+ is required for the Memory feature. Version 0.8+ is required for the built-in
Tools system. If the version is older, update Open WebUI before proceeding:

```bash
docker pull ghcr.io/open-webui/open-webui:main
docker stop open-webui
docker rm open-webui
# Re-run your original docker run command (check your deployment notes for exact flags)
```

### Step 2 — Verify Ollama is externally accessible

Ollama must listen on all interfaces so Aider on client machines can connect over Tailscale.

Check the current systemd environment:

```bash
sudo systemctl show ollama | grep Environment
```

If `OLLAMA_HOST` is not set (or is set to `127.0.0.1`), update it:

```bash
sudo systemctl edit ollama
```

Add these lines in the override file:

```ini
[Service]
Environment="OLLAMA_HOST=0.0.0.0"
Environment="OLLAMA_MAX_LOADED_MODELS=2"
```

`OLLAMA_MAX_LOADED_MODELS=2` allows Ollama to keep two models loaded in VRAM simultaneously.
With 48GB VRAM (2x 24GB), both qwen2.5:32b and qwen2.5-coder:32b (~20GB each) stay loaded
at the same time — switching between them is instant rather than requiring a 30–60 second
unload/reload cycle.

Save, then restart:

```bash
sudo systemctl daemon-reload
sudo systemctl restart ollama
```

Verify from a client machine over Tailscale:

```bash
curl http://100.79.83.113:11434/api/tags
```

You should see a JSON response listing installed models.

### Step 3 — Pull models

Pull the fast fallback first so you have something usable immediately, then start the 32B downloads.
The 32B models are ~20GB each — on a typical home connection this takes 20–40 minutes per model.

```bash
# Pull fast fallback first (5GB) — available in minutes
ollama pull qwen2.5:7b

# Pull general-purpose 32B (20GB)
ollama pull qwen2.5:32b

# Pull coding 32B (20GB)
ollama pull qwen2.5-coder:32b

# Optional: pull 72B model for highest quality (~45GB — requires both GPUs, cannot run alongside another 32B)
ollama pull qwen2.5:72b
```

Verify all models are available:

```bash
ollama list
```

**VRAM note**: Two RTX 3090s provide 48GB VRAM total. The 32B models use approximately 20GB each.
With `OLLAMA_MAX_LOADED_MODELS=2` set, both qwen2.5:32b and qwen2.5-coder:32b can be hot-loaded
simultaneously (40GB combined), making switching between them instant. Alternatively, pull
`qwen2.5:72b` (~45GB Q4) to run a single high-quality 70B-class model across both GPUs.

---

## Phase 1: Open WebUI Configuration

**Time estimate: 1–2 hours**

Open a browser and navigate to `http://100.79.83.113:3000`. Log in as admin.

### 1.1 — Web Search

*Admin > Settings > Web Search*

Enable **Web Search**. Set the search engine to `searxng`. For the URL, enter:

```
http://searxng:8080
```

If SearxNG is not yet deployed (Phase 4), temporarily use `brave` as the engine and paste a
<a href="https://brave.com/search/api/" target="_blank">Brave Search API</a> key. This lets you
test web search functionality immediately while SearxNG is being set up.

**Testing**: In a chat window, click the web search icon (globe) before sending a message, then
ask "What happened in the news today?" — the response should include source citations.

### 1.2 — Memory

*Admin > Settings > Features > Memory*

Enable the **Memory Feature**. This allows the AI to store facts it learns about you
across conversations. The AI will say things like "I'll remember that" and will recall that
information in future sessions.

Users can also manually save memories:
- In chat, click the three-dot menu on any AI message and select "Add to memory"
- Or simply tell the AI: "Remember that I prefer concise responses"

**Scope**: Memory is per-user. Each user account has their own memory store.

### 1.3 — Code Execution

*Admin > Settings > Code Execution*

Enable **Code Execution**. This gives the AI a sandboxed Python interpreter running in a
Docker container. The AI can write and run Python scripts within a conversation.

When enabled, the AI can:
- Execute Python code and show the output
- Generate charts and images (displayed inline in the chat)
- Process data from uploaded files
- Run multi-step calculations or data transformations

**Testing**: Ask the AI: "Write a Python script that reads a CSV file with columns Name and Age,
then calculates the average age" — the AI should write the code and execute it.

### 1.4 — Document/PDF Upload (RAG)

No admin configuration is required — this is enabled by default in Open WebUI.

In any chat window, click the paperclip icon to upload a file. Supported formats include PDF,
DOCX, TXT, and CSV. After uploading, ask questions about the document content.

**How it works**: Open WebUI extracts text from the uploaded document, splits it into chunks,
embeds them into a vector store, and retrieves the most relevant chunks when you ask a question.
This is called **RAG** (Retrieval-Augmented Generation — a technique where the AI retrieves
relevant document sections before generating its response, rather than relying on training data).

**Testing**: Upload any PDF. Ask "What are the main topics covered in this document?" — the AI
should answer from the document content, not from its training data.

### 1.5 — Default Model

*Admin > Settings > Models (or in the chat header dropdown)*

Set the default model to `qwen2.5:32b` for general use. Users can switch to
`qwen2.5-coder:32b` in the model dropdown within any chat window when doing coding work.

---

## Phase 2: Terminal Interface — Aider

**Time estimate: 30 minutes per client machine**

<a href="https://aider.chat/" target="_blank">Aider</a> is an AI pair programming tool that runs
in your terminal. It reads the files in your current git repository, understands the codebase
structure, and makes targeted edits in response to your instructions. Every change it makes is
automatically committed to git with a descriptive commit message.

### 2.1 — Install Aider on each client

**Linux / macOS / WSL2:**

```bash
pip install aider-chat
```

**Windows (PowerShell):**

```powershell
pip install aider-chat
```

Verify installation:

```bash
aider --version
```

### 2.2 — Configure Aider

Create `~/.aider.conf.yml` on each client. This tells Aider to use your local Ollama server
rather than a cloud API. See the full configuration reference at
<a href="https://aider.chat/docs/config/aider_conf.html" target="_blank">aider.chat/docs/config/aider_conf.html</a>.

```yaml
# ~/.aider.conf.yml
model: ollama/qwen2.5-coder:32b
ollama-api-base: http://100.79.83.113:11434
auto-commits: true
git: true
```

**What each setting does:**

| Setting | Value | Effect |
|---|---|---|
| `model` | `ollama/qwen2.5-coder:32b` | Use local Ollama; coding-optimised 32B model |
| `ollama-api-base` | `http://100.79.83.113:11434` | Connect to AI server over Tailscale |
| `auto-commits` | `true` | Commit every AI-made change to git automatically |
| `git` | `true` | Enable git integration (required for auto-commits) |

### 2.3 — Run Aider

Navigate to any git-tracked project directory, then start Aider:

```bash
cd /path/to/your/project
aider
```

Aider reads your repository structure and presents an interactive prompt. You describe what
you want changed in plain English — for example:

```
> Add input validation to the register_user function in auth.py
> Refactor the database module to use connection pooling
> Add unit tests for the payment processor
```

Aider will edit the relevant files and automatically commit the changes.

**To work with a specific set of files:**

```bash
aider src/auth.py src/database.py
```

**To include a document as context (e.g. a spec or PDF):**

```bash
pdftotext requirements.pdf requirements.txt
aider --read requirements.txt src/main.py
```

**To switch to the general model for non-coding tasks:**

```bash
aider --model ollama/qwen2.5:32b
```

Or override temporarily in the session:

```
/model ollama/qwen2.5:32b
```

### 2.4 — Aider in non-git directories

Aider works best in a git repo. If your directory is not a git repo, initialise one first:

```bash
git init
git add .
git commit -m "Initial commit"
aider
```

---

## Phase 3: Workspace and Session Tracking

**Time estimate: 1 hour**

This phase creates a git repository on the server that acts as the AI's working memory and
session log — mirroring the pattern used in the `/terminai/` repo.

### 3.1 — Create the workspace repo on the server

SSH into the server:

```bash
sudo mkdir -p /opt/local-cc-workspace/{sessions,projects,memory,scripts}
sudo chown -R $USER:$USER /opt/local-cc-workspace
cd /opt/local-cc-workspace
git init
```

Create the initial memory file:

```bash
cat > memory/MEMORY.md << 'EOF'
# AI Workspace Memory

## About This File
This file contains persistent context for the local AI assistant.
The AI reads this at the start of each session.

## User Context
- AI server: 100.79.83.113
- Workspace: /opt/local-cc-workspace/
- Primary interface: Open WebUI at http://100.79.83.113:3000
- Terminal interface: Aider (configured on client machines)

## Active Projects
(none yet)

## Preferences
(add preferences here as the AI learns them)
EOF

git add .
git commit -m "Initial workspace setup"
```

### 3.2 — Connect to GitHub

Create a new **private** repository on GitHub named `local-cc-workspace`, then:

```bash
cd /opt/local-cc-workspace
git remote add origin git@github.com:<your-username>/local-cc-workspace.git
git push -u origin main
```

This gives you an off-server backup of all session notes and AI-generated content.

### 3.3 — Configure Open WebUI System Prompt

In Open WebUI, set a system prompt that instructs the AI how to use the workspace:

*Admin > Settings > System Prompt* (or per-user in *Settings > Customization > System Prompt*)

```
You are a local AI assistant with access to a workspace at /opt/local-cc-workspace/.

At the start of each conversation:
- Read /opt/local-cc-workspace/memory/MEMORY.md for persistent context

During a conversation:
- When you learn something important about the user or their projects, note it
- Use the code execution tool to read/write files in the workspace

At the end of a long session (when asked to "end session"):
- Create a session note at /opt/local-cc-workspace/sessions/YYYY-MM-DD-topic.md
- Update /opt/local-cc-workspace/memory/MEMORY.md with any new facts learned
- Run: cd /opt/local-cc-workspace && git add . && git commit -m "Session: [topic]"
```

### 3.4 — Session file format

Session notes follow this structure:

```markdown
# Session: [Brief Topic]
Date: YYYY-MM-DD

## What Was Done
- [Task 1]
- [Task 2]

## Decisions Made
- [Decision and rationale]

## Files Changed
- [path/to/file] — [what changed]

## Next Steps
- [TODO 1]
- [TODO 2]
```

---

## Phase 4: Web Search — SearxNG

**Time estimate: 30 minutes**

<a href="https://docs.searxng.org/" target="_blank">SearxNG</a> is a self-hosted meta-search
engine that queries multiple search providers simultaneously (Google, Bing, DuckDuckGo, etc.)
and returns aggregated results without tracking your queries or exposing them to any single
provider. It integrates natively with Open WebUI's web search feature.

### 4.1 — Deploy SearxNG

SSH into the server:

```bash
# Create config directory
sudo mkdir -p /opt/searxng
sudo chown -R $USER:$USER /opt/searxng

# Deploy container — bind to localhost only (not externally reachable)
docker run -d \
  --name searxng \
  --restart unless-stopped \
  -p 127.0.0.1:8080:8080 \
  -v /opt/searxng:/etc/searxng:rw \
  searxng/searxng:latest
```

**Why `127.0.0.1:8080`?** This binds the port to localhost only. SearxNG is only called by
Open WebUI (which runs on the same Docker host). It should not be reachable from your Tailscale
network or the internet.

### 4.2 — Configure SearxNG for JSON output

Open WebUI calls SearxNG's API and expects JSON responses. Edit the SearxNG settings file:

```bash
# The container generates a default settings.yml on first start
# Wait ~10 seconds after first run, then edit:
nano /opt/searxng/settings.yml
```

Find the `search:` section and ensure `formats` includes `json`:

```yaml
search:
  formats:
    - html
    - json
```

Also set a secret key (replace with a random string):

```yaml
server:
  secret_key: "replace-with-a-long-random-string"
  limiter: false
```

Restart SearxNG after editing:

```bash
docker restart searxng
```

Verify SearxNG is working:

```bash
curl "http://localhost:8080/search?q=test&format=json" | python3 -m json.tool | head -20
```

You should see JSON search results.

### 4.3 — Connect SearxNG to Open WebUI

The SearxNG container and Open WebUI container are both on the Docker bridge network. There are
two ways to reference the URL:

- `http://localhost:8080` — works if Open WebUI calls the host's localhost
- `http://searxng:8080` — works if both containers are on the same Docker network (preferred)

Put both containers on the same network if they are not already:

```bash
# Create a shared network (if it doesn't exist)
docker network create ai-network

# Connect both containers
docker network connect ai-network open-webui
docker network connect ai-network searxng
```

Then in Open WebUI (*Admin > Settings > Web Search*), set:
- **Search Engine**: `searxng`
- **SearxNG URL**: `http://searxng:8080`

**Testing**: In a chat window, enable web search (globe icon), then ask "What is the latest
news today?" — the response should include current news with source links.

---

## Security Hardening

**Time estimate: 30–60 minutes**

### User Isolation for Code Execution

The Open WebUI code sandbox runs as root inside its Docker container by default. Create a
dedicated user for AI-executed code:

```bash
# Create restricted user
sudo useradd -m -s /bin/bash ai-executor
sudo usermod -aG docker ai-executor   # only if AI executor needs Docker access

# Restrict workspace ownership
sudo chown -R ai-executor:ai-executor /opt/local-cc-workspace
```

Configure Docker to use this user for the code execution sandbox by editing Open WebUI's
environment variables.

### Open WebUI Authentication

By default, the first registered user becomes admin. Ensure:

1. **Enable authentication** — *Admin > Settings > General > Enable Sign Up* should be controlled
2. **Create accounts only for trusted users** — do not leave Open WebUI open to anonymous access
3. **Disable new user registration** once all accounts are created:
   *Admin > Settings > General > Default User Role* → set to `pending` or disable sign-up entirely

### Tailscale ACL

The security perimeter is Tailscale. Restrict which devices can reach the AI server:

In the Tailscale admin console, create an ACL that allows only your personal devices to reach
`100.79.83.113` on ports `3000` (Open WebUI) and `11434` (Ollama).

Example Tailscale ACL rule:

```json
{
  "acls": [
    {
      "action": "accept",
      "src": ["tag:personal-devices"],
      "dst": ["100.79.83.113:3000", "100.79.83.113:11434"]
    }
  ]
}
```

### Ollama API — No Authentication

Ollama has no built-in authentication. Anyone who can reach port 11434 over Tailscale can
query any model. Mitigations:

1. **Tailscale ACL** (above) — restrict which devices can reach the port
2. **Rate limiting** — add nginx as a reverse proxy with rate limiting if abuse is a concern
3. Do not expose port 11434 on the public internet under any circumstances

---

## Models Reference

All models are pulled via `ollama pull <name>`.

| Model | Command | VRAM | Context | Best For |
|---|---|---|---|---|
| qwen2.5:32b | `ollama pull qwen2.5:32b` | ~20GB | 32K tokens | General chat, web research, documents |
| qwen2.5-coder:32b | `ollama pull qwen2.5-coder:32b` | ~20GB | 32K tokens | Code writing, editing, debugging |
| qwen2.5:7b | `ollama pull qwen2.5:7b` | ~5GB | 32K tokens | Fast responses, simple tasks, fallback |
| qwen2.5:72b | `ollama pull qwen2.5:72b` | ~45GB | 32K tokens | Highest quality — uses both GPUs; cannot run alongside another 32B |

**Dual GPU operating modes:**

| Mode | Models Loaded | VRAM Used | Behaviour |
|------|--------------|-----------|-----------|
| Hot-swap (default) | qwen2.5:32b + qwen2.5-coder:32b | ~40GB | Both in VRAM; switching is instant |
| Large model | qwen2.5:72b only | ~45GB | Single high-quality model across both GPUs |
| Fast + quality | qwen2.5:32b + qwen2.5:7b | ~25GB | Leave ~23GB free for other GPU tasks |

**Glossary:**

- **VRAM** — Video RAM on the GPU. Models must fit in VRAM for GPU-accelerated inference. If
  a model exceeds available VRAM, Ollama offloads layers to system RAM, which is 10–50x slower.
- **Context window** — the number of tokens (roughly, word fragments) the model can hold in
  memory during a single conversation. 32K tokens ≈ ~24,000 words or ~40–50 pages.
- **Inference** — the process of running a model to generate a response (as opposed to training,
  which teaches the model). All use here is inference only.
- **Quantization** — a compression technique that reduces model precision (e.g., from 32-bit
  to 4-bit numbers) to fit larger models into less VRAM. The 32B models pulled above are
  4-bit quantized; this reduces VRAM usage from ~64GB (full precision) to ~20GB with minimal
  quality loss.

**Model swap time**: With `OLLAMA_MAX_LOADED_MODELS=2` and 48GB VRAM, switching between any two
pre-loaded 32B models is instant (milliseconds). Switching to the 72B model requires unloading
both 32B models first — approximately 30–60 seconds.

**Adding models later**: The Ollama model library is at
<a href="https://ollama.com/library" target="_blank">ollama.com/library</a>. Any model listed
there can be pulled with `ollama pull <name>`. Recommended additions:

| Model | Use Case | VRAM |
|---|---|---|
| `deepseek-coder-v2:16b` | Alternative coding model | ~10GB |
| `mistral:7b` | Fast general model, strong at instruction following | ~5GB |
| `llama3.2:3b` | Very fast responses for simple tasks | ~2GB |
| `nomic-embed-text` | Text embeddings for RAG/document search | <1GB |

---

## Model Selection and Switching

This section covers how to choose the right model for each task and how to switch models
mid-task — the local equivalent of Claude Code's `--model` flag and `/model` command.

### Which model for which task

| Task | Recommended Model | Reason |
|------|-------------------|--------|
| General chat, Q&A, writing | `qwen2.5:32b` | Strong general reasoning |
| Web research and synthesis | `qwen2.5:32b` | Good at summarising and cross-referencing |
| Writing code, debugging | `qwen2.5-coder:32b` | Fine-tuned on code; better at syntax and patterns |
| Large codebase analysis | `qwen2.5-coder:32b` | Understands code structure, variable relationships |
| Quick lookups, simple questions | `qwen2.5:7b` | Faster response; good enough for low-complexity tasks |
| Complex multi-step reasoning | `qwen2.5:72b` | Higher parameter count = better logical chains |
| Long documents (80+ pages) | `qwen2.5:72b` or `qwen2.5:32b` | Larger models handle long context better |
| Start with planning, then code | Switch mid-task | Use 32b to plan, switch to coder:32b to implement |

**Rule of thumb**: start with `qwen2.5-coder:32b` for anything involving code; use `qwen2.5:32b`
for everything else. Switch to `qwen2.5:7b` when you need quick answers and quality is not
critical.

---

### Model switching in Open WebUI

Open WebUI lets you select or change the model at any point — including mid-conversation.
The conversation history carries over to the new model automatically.

**Selecting a model at the start of a conversation:**

The model dropdown is in the top-centre of the chat window. Click it to see all available models
pulled into Ollama. Select before sending your first message.

**Switching model mid-conversation:**

Click the model dropdown at any point during a conversation and select a different model.
The new model receives the full conversation history and continues from where the previous model
left off. This is directly equivalent to Claude Code's `/model` command.

**Example workflow** (plan then code):
```
1. Start conversation with qwen2.5:32b
2. "I need to refactor this authentication module. Here's the current code: [paste code].
   What approach would you recommend and what are the trade-offs?"
3. [Review and agree on the plan]
4. Switch model to qwen2.5-coder:32b (click dropdown)
5. "Now implement the refactoring we just planned."
6. [qwen2.5-coder:32b uses the full conversation history including the plan]
```

**Switching is instant** (with `OLLAMA_MAX_LOADED_MODELS=2` configured) because both 32B models
remain loaded in VRAM simultaneously. There is no 30–60 second reload delay.

**Switching to the 72B model** requires unloading both 32B models (30–60 seconds). Do this when:
- The task requires the highest possible reasoning quality
- You are working on a complex, multi-constraint problem
- You do not need to switch back quickly

To switch back from 72B to a 32B model, the 32B model must be reloaded (30–60 seconds).

---

### Model switching in Aider (terminal)

Aider lets you change models both at startup and interactively during a session.

**Starting Aider with a specific model:**

```bash
# Default (from .aider.conf.yml) — qwen2.5-coder:32b
aider

# Override for this session — general model
aider --model ollama/qwen2.5:32b

# Override for this session — 72B model
aider --model ollama/qwen2.5:72b
```

**Switching model mid-session:**

Type `/model` followed by the model name at the Aider prompt:

```
> /model ollama/qwen2.5:32b
```

```
> /model ollama/qwen2.5-coder:32b
```

```
> /model ollama/qwen2.5:72b
```

The current model is shown in the Aider prompt. After switching, Aider continues with the
full conversation history — the same as Claude Code's `/model` command.

**Checking which model is currently active:**

```
> /model
```

With no argument, `/model` prints the currently active model name.

**Example workflow** (write tests then implementation):
```bash
aider src/auth.py tests/test_auth.py
> /model ollama/qwen2.5:32b
> Analyse this authentication module and identify what unit tests are missing.
  [Review the analysis]
> /model ollama/qwen2.5-coder:32b
> Write the missing unit tests in tests/test_auth.py.
```

---

### Setting a per-project default model

If different projects benefit from different default models, create a `.aider.conf.yml` in
the project directory. It overrides `~/.aider.conf.yml` for that project only:

```yaml
# /path/to/my-python-project/.aider.conf.yml
model: ollama/qwen2.5-coder:32b   # code-heavy project

# /path/to/my-writing-project/.aider.conf.yml
model: ollama/qwen2.5:32b         # writing/research project
```

---

### Viewing available models

**In Ollama (command line):**

```bash
ollama list
```

This shows all pulled models with their sizes and when they were last modified.

**In Open WebUI:**

The model dropdown in any chat window lists all available models. Models are fetched directly
from Ollama — any model you pull via `ollama pull` appears in Open WebUI immediately (no restart
required).

**To remove a model no longer needed:**

```bash
ollama rm qwen2.5:7b
```

---

## Verification Checklist

Work through this after each phase to confirm everything is functioning before moving on.

### After Phase 0 (Audit & Models)

- [ ] `curl http://100.79.83.113:11434/api/tags` returns a list of models
- [ ] `ollama list` shows qwen2.5:32b, qwen2.5-coder:32b, qwen2.5:7b (and optionally qwen2.5:72b)
- [ ] `nvidia-smi` shows both GPUs detected with 24GB VRAM each
- [ ] Open WebUI is accessible at `http://100.79.83.113:3000`

### After Phase 1 (Open WebUI Configuration)

- [ ] Chat with a model works (qwen2.5:32b responds to a simple message)
- [ ] Web search: enable globe icon → ask for today's news → results with citations appear
- [ ] Memory: tell the AI your name → start a new conversation → AI remembers your name
- [ ] Code execution: ask the AI to calculate a Fibonacci sequence in Python → code runs and output appears
- [ ] PDF: upload any PDF → ask a question about its content → AI answers from the document

### After Phase 2 (Aider)

- [ ] `aider --version` returns a version number on client machine
- [ ] `cd` into a git repo → run `aider` → Aider connects to the remote Ollama server
- [ ] Ask Aider to create a simple `hello.py` → file is created → `git log` shows a commit by Aider

### After Phase 3 (Workspace)

- [ ] `/opt/local-cc-workspace/` exists with correct directory structure
- [ ] `git log` in workspace shows initial commit
- [ ] GitHub remote is set and `git push` succeeds
- [ ] In Open WebUI, ask the AI to write a file to `/opt/local-cc-workspace/test.txt` → file appears on server

### After Phase 4 (SearxNG)

- [ ] `curl "http://localhost:8080/search?q=test&format=json"` returns JSON results
- [ ] Open WebUI web search now returns SearxNG results (not an API error)

### End-to-end test

Ask the AI in Open WebUI to:
1. Search the web for recent news about a topic you choose
2. Write a Python script that prints today's date
3. Execute that script and show you the output
4. Upload a PDF and summarise the first section

All four steps should complete successfully.

---

## Troubleshooting

### Ollama not accessible from client (`Connection refused` on port 11434)

**Cause**: Ollama is bound to `127.0.0.1` only (default).

**Fix**: Set `OLLAMA_HOST=0.0.0.0` in the systemd service override (see Phase 0, Step 2).

Verify after fix:
```bash
ss -tlnp | grep 11434
# Should show: 0.0.0.0:11434 not 127.0.0.1:11434
```

---

### Models are slow (10+ seconds per token)

**Cause**: Model has exceeded VRAM and is partially running on system RAM (CPU inference).

**Diagnosis**:
```bash
nvidia-smi
```
Look at the `MEM` column for both GPUs. With 48GB total VRAM:
- Two 32B models = ~40GB used (normal — within capacity)
- 72B model = ~45GB used (normal — within capacity)
- If VRAM shows close to 48GB and performance is poor, another process is consuming VRAM

**Fix options**:
- Switch to qwen2.5:7b (5GB VRAM) — much faster
- Ensure no other process is using VRAM (`nvidia-smi` shows all processes and their VRAM usage)
- Restart Ollama to clear stale models from VRAM: `sudo systemctl restart ollama`
- If running the 72B model and it's slow, check it is actually using both GPUs: `nvidia-smi` should
  show GPU-Util > 0 on both cards during inference

---

### Open WebUI web search returns error / no results

**Cause A**: SearxNG not running.
```bash
docker ps | grep searxng
# If not shown, SearxNG container is stopped
docker start searxng
```

**Cause B**: SearxNG URL configured incorrectly in Open WebUI.
- Try `http://localhost:8080` instead of `http://searxng:8080` (or vice versa)
- Test the URL directly: `curl http://localhost:8080/search?q=test&format=json`

**Cause C**: SearxNG not returning JSON.
- Check that `formats: [html, json]` is set in `/opt/searxng/settings.yml`
- Restart SearxNG after any config change: `docker restart searxng`

---

### Aider says "No git repository found"

**Fix**: Initialise git before running Aider:
```bash
git init
git add .
git commit -m "Initial commit"
aider
```

---

### Open WebUI code execution fails / Python not available

**Cause**: Code execution requires Docker to be running on the server.

**Fix**:
```bash
sudo systemctl status docker
sudo systemctl start docker
```

Then restart Open WebUI: `docker restart open-webui`

---

### Memory not persisting between conversations

**Cause A**: Memory feature not enabled in admin settings.
- *Admin > Settings > Features > Memory* — confirm it is toggled on.

**Cause B**: The AI did not save the memory.
- Memories are only saved if the AI explicitly decides to store something, or if you ask it to.
- To force a memory save: "Please remember that [fact]"
- To view saved memories: *Settings > Memory* in your user profile.

---

## Upgrade Path

Once the base system is running, these are the most valuable additions:

### Larger Models (70B+)

The system currently runs qwen2.5:72b across both GPUs. If a third GPU or higher-VRAM GPU is
added in future:
- RTX 5090 (32GB): single card could run 70B models, freeing both 3090s for parallel tasks
- Three 3090s (72GB): could run two 72B models simultaneously

For the current 48GB setup, `qwen2.5:72b` is the practical ceiling. Models above 72B (e.g.
Llama 3.1 405B) require 200GB+ VRAM — beyond the scope of this configuration.

### MCP (Model Context Protocol) Integration

<a href="https://docs.openwebui.com/" target="_blank">Open WebUI</a> v0.8+ supports MCP servers,
which are plugins that give the AI additional tools — for example:
- A filesystem MCP server (direct file read/write access)
- A database MCP server (query SQL databases)
- A browser MCP server (navigate websites)

This removes the need for Aider as a separate tool for file operations.

### Open Interpreter (server mode)

<a href="https://openinterpreter.com/" target="_blank">Open Interpreter</a> can run as a server,
giving the AI unrestricted ability to run code, manage files, and execute system commands — a more
capable alternative to Open WebUI's sandboxed code execution, but with higher security
implications. Deploy only if you are comfortable with the access level it requires.

### Persistent RAG Knowledge Base

Instead of uploading PDFs per-conversation, build a persistent vector store:
- Use Open WebUI's **Collections** feature to store frequently-used documents
- Or deploy a dedicated vector database (Qdrant, Chroma) and connect it to Open WebUI

---

## Sources

All links verified March 2026.

- <a href="https://ollama.com/" target="_blank">Ollama — Official Homepage</a>
- <a href="https://docs.openwebui.com/" target="_blank">Open WebUI — Official Documentation</a>
- <a href="https://aider.chat/" target="_blank">Aider — AI Pair Programming</a>
- <a href="https://aider.chat/docs/config/aider_conf.html" target="_blank">Aider — Configuration File Reference</a>
- <a href="https://docs.searxng.org/" target="_blank">SearxNG — Official Documentation</a>
- <a href="https://tailscale.com/" target="_blank">Tailscale — Secure Connectivity</a>
- <a href="https://ollama.com/library/qwen2.5" target="_blank">Ollama Library — qwen2.5</a>
- <a href="https://ollama.com/library/qwen2.5-coder" target="_blank">Ollama Library — qwen2.5-coder</a>
- <a href="https://ollama.com/library" target="_blank">Ollama Model Library</a>
