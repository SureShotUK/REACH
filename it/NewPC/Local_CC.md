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

> **Model note (March 2026):** The models in this guide have been updated from the original Qwen2.5
> recommendations. Qwen3.5 and Devstral now substantially outperform Qwen2.5 across all use cases.
> See the Models Reference section for current recommendations.

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
        │       ├── qwen3.5:27b  ── general / reasoning / vision  (~22GB, GPU #1)
        │       ├── devstral     ── agentic coding                 (~14GB, GPU #2)
        │       ├── qwen3.5:9b   ── fast fallback / vision        (~7GB)
        │       └── qwen3.5:35b  ── large high-quality, both GPUs (~27GB)
        │       [qwen3.5:27b + devstral = ~36GB combined — both hot-loaded in 48GB VRAM]
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
- **Two models simultaneously** — with 48GB VRAM (2x 24GB), qwen3.5:27b (~22GB) and
  devstral (~14GB) can be loaded at once (~36GB combined). Set `OLLAMA_MAX_LOADED_MODELS=2` and
  switching between them is instant — no unload/reload delay.
- **Or one large model** — qwen3.5:35b (~27GB) runs entirely in VRAM with headroom to spare,
  offering higher quality than any 27B model for demanding tasks.
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

docker run -d \
  --name open-webui \
  --restart unless-stopped \
  --network ai-network \
  --add-host=host.docker.internal:host-gateway \
  -p 3000:8080 \
  -v open-webui:/app/backend/data \
  -e OLLAMA_BASE_URL=http://host.docker.internal:11434 \
  ghcr.io/open-webui/open-webui:main
```

> **Network note**: `--network ai-network` keeps Open WebUI on the shared Docker network with
> SearxNG so it can reach the search engine via `http://searxng:8080`. `--add-host` is still
> required for Ollama since that runs on the host (not in a container). Recreating the container
> without these flags loses both connections and requires manual reconnection.

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
# Pull fast fallback first (6.6GB) — available in minutes; has vision and 256K context
ollama pull qwen3.5:9b

# Pull general-purpose 27B (17GB) — replaces qwen2.5:32b; better quality, similar VRAM
ollama pull qwen3.5:27b

# Pull coding model (14GB) — replaces qwen2.5-coder:32b; purpose-built agentic coding
ollama pull devstral

# Optional: pull large high-quality 35B (24GB) — fits in VRAM with headroom on dual 3090
ollama pull qwen3.5:35b
```

Verify all models are available:

```bash
ollama list
```

**VRAM note**: Two RTX 3090s provide 48GB VRAM total. With `OLLAMA_MAX_LOADED_MODELS=2` set,
qwen3.5:27b (~22GB) and devstral (~14GB) can be hot-loaded simultaneously (~36GB combined),
leaving 12GB headroom. Switching between them is instant — no unload/reload delay. Alternatively,
pull `qwen3.5:35b` (~27GB) as a single higher-quality model with room to spare.

---

## Phase 1: Open WebUI Configuration

**Time estimate: 1–2 hours**

Open a browser and navigate to `https://amelai.tail926601.ts.net`. Log in as admin.

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

Set the default model to `qwen3.5:27b` for general use. Users can switch to
`devstral` in the model dropdown within any chat window when doing coding work.

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
model: ollama/devstral
auto-commits: true
git: true
```

Aider does not accept `ollama-api-base` as a config file key. The Ollama server URL must be
set via environment variable instead. Add this to `~/.bashrc` (Linux/WSL2) or `~/.zshrc` (macOS):

```bash
export OLLAMA_API_BASE=http://100.79.83.113:11434
```

Then reload your shell:

```bash
source ~/.bashrc
```

**What each setting does:**

| Setting | Where | Value | Effect |
|---|---|---|---|
| `model` | `.aider.conf.yml` | `ollama/devstral` | Use local Ollama; purpose-built agentic coding model |
| `OLLAMA_API_BASE` | Environment variable | `http://100.79.83.113:11434` | Connect to AI server over Tailscale |
| `auto-commits` | `.aider.conf.yml` | `true` | Commit every AI-made change to git automatically |
| `git` | `.aider.conf.yml` | `true` | Enable git integration (required for auto-commits) |

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
aider --model ollama/qwen3.5:27b
```

Or override temporarily in the session:

```
/model ollama/qwen3.5:27b
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
- Primary interface: Open WebUI at https://amelai.tail926601.ts.net
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

SearxNG binds to `127.0.0.1:8080` on the host (localhost only). Open WebUI runs inside a Docker
container and cannot reach host localhost directly. Both containers must share a Docker network so
Open WebUI can reach SearxNG by container name, bypassing the host entirely.

```bash
# Create the shared network (skip if already exists)
docker network create ai-network 2>/dev/null || true

# Connect both containers
docker network connect ai-network open-webui
docker network connect ai-network searxng
```

Verify both are connected:

```bash
docker network inspect ai-network --format '{{range .Containers}}{{.Name}} {{end}}'
```

**Important**: if you ever recreate the Open WebUI container, use the run command in Phase 0
(Step 1) which includes `--network ai-network`. Recreating without this flag loses the network
connection and SearxNG will stop working.

Test the connection from inside the Open WebUI container before configuring the URL:

```bash
docker exec open-webui curl -s "http://searxng:8080/search?q=test&format=json" | head -c 200
```

This must return JSON. If it hangs, the containers are not on the same network.

Then in Open WebUI (*Admin > Settings > Web Search*), set:
- **Search Engine**: `searxng`
- **SearxNG Query URL**: `http://searxng:8080/search?q=<query>`

The `<query>` placeholder must be present exactly as written — Open WebUI substitutes it at
runtime. Do not use `localhost` or `172.17.0.1` — use the container name `searxng`.

**Testing**: In a chat window, enable web search (globe icon), then ask "What is the latest
news today?" — the response should include current news with source links.

---

## Phase 4.1: SearxNG MCP Server for Claude Code (Terminal)

**Time estimate: 30 minutes**

This phase extends SearxNG integration beyond Open WebUI to Claude Code running in the terminal.
When Claude Code is pointed at a local Ollama backend (via `ANTHROPIC_BASE_URL`), the built-in
WebSearch tool silently returns zero results — it requires Anthropic's infrastructure. An MCP
(Model Context Protocol) server bridges the gap by exposing SearxNG as a proper `web_search`
tool that Claude Code can call.

### Architecture

```
[Windows Terminal — Claude Code]
        │
        │  MCP SSE connection (Tailscale, port 3001)
        │
        ▼
[AI Server: 100.79.83.113]
        ├── mcp-searxng  (systemd service, port 3001)
        │       └── calls SearxNG at http://100.79.83.113:8080/search
        │
        └── searxng  (Docker, port 8080 on Tailscale interface)
```

### Prerequisites

SearxNG must be bound to the Tailscale interface (not just localhost). If SearxNG was originally
deployed with `-p 127.0.0.1:8080:8080`, recreate it:

```bash
docker stop searxng && docker rm searxng

docker run -d \
  --name searxng \
  --restart unless-stopped \
  -p 100.79.83.113:8080:8080 \
  -v /opt/searxng:/etc/searxng:rw \
  --network ai-network \
  searxng/searxng:latest
```

Also open port 8080 on the Tailscale interface:

```bash
sudo ufw allow in on tailscale0 to any port 8080
```

### 4.1.1 — Deploy the MCP Server

SSH into the server, then:

```bash
# Create directory and virtualenv
sudo mkdir -p /opt/mcp-searxng
sudo chown $USER:$USER /opt/mcp-searxng
sudo apt install -y python3.12-venv   # if not already installed
python3 -m venv /opt/mcp-searxng/venv
/opt/mcp-searxng/venv/bin/pip install fastmcp httpx
```

Create the server script:

```bash
cat > /opt/mcp-searxng/server.py << 'SCRIPT'
from fastmcp import FastMCP
import httpx

mcp = FastMCP("SearxNG Search")

SEARXNG_URL = "http://100.79.83.113:8080/search"

@mcp.tool()
async def web_search(query: str) -> str:
    """Search the web using the self-hosted SearxNG search engine. Returns titles, URLs, and snippets."""
    async with httpx.AsyncClient() as client:
        try:
            response = await client.get(
                SEARXNG_URL,
                params={"q": query, "format": "json"},
                timeout=15.0
            )
            response.raise_for_status()
            data = response.json()
            results = data.get("results", [])[:8]

            if not results:
                return f"No results found for: {query}"

            lines = [f"Search results for '{query}':\n"]
            for i, r in enumerate(results, 1):
                title = r.get("title", "No title")
                url = r.get("url", "")
                content = r.get("content", "")[:300]
                lines.append(f"{i}. {title}\n   {url}\n   {content}\n")

            return "\n".join(lines)
        except Exception as e:
            return f"Search error: {str(e)}"

mcp.run(transport="sse", host="0.0.0.0", port=3001)
SCRIPT
```

Create the systemd service:

```bash
sudo tee /etc/systemd/system/mcp-searxng.service << 'EOF'
[Unit]
Description=SearxNG MCP Server
After=network.target docker.service

[Service]
Type=simple
User=steve
WorkingDirectory=/opt/mcp-searxng
ExecStart=/opt/mcp-searxng/venv/bin/python /opt/mcp-searxng/server.py
Restart=always
RestartSec=5

[Install]
WantedBy=multi-user.target
EOF

sudo systemctl daemon-reload
sudo systemctl enable mcp-searxng
sudo systemctl start mcp-searxng
```

Open port 3001 on the Tailscale interface:

```bash
sudo ufw allow in on tailscale0 to any port 3001
```

Verify the server is running:

```bash
sudo systemctl status mcp-searxng
curl -s http://100.79.83.113:3001/sse
# Should return SSE stream with event: endpoint — press Ctrl+C to stop
```

### 4.1.2 — Register with Claude Code (Windows)

Run this once in the **Windows PowerShell** terminal where Claude Code is launched:

```powershell
claude mcp add --transport sse searxng http://100.79.83.113:3001/sse
```

This saves to `C:\Users\SteveIrwin\.claude.json` for the `C:\Users\SteveIrwin\Claude` project scope.

Restart Claude Code and verify the server is connected:

```
/mcp
```

Expected output:
```
searxng · ✔ connected
```

### 4.1.3 — Using Web Search in Claude Code

**Limitation**: When using an Ollama backend, local models may default to Claude Code's
built-in `Web Search` tool (which silently fails without Anthropic's infrastructure) rather
than the MCP `web_search` tool. To guarantee the MCP tool is used, invoke it explicitly:

```
Call the web_search tool with query "your search terms here"
```

To make this automatic, add the following to the CLAUDE.md file in your Claude Code working
directory (e.g. `C:\Users\SteveIrwin\Claude\CLAUDE.md`):

```markdown
## Tool Use
When searching the web, always use the `web_search` MCP tool. Do not use the built-in Web Search tool.
```

### 4.1.4 — Verification

```bash
# MCP server health
sudo systemctl status mcp-searxng
curl "http://100.79.83.113:3001/sse"

# Test search directly
curl "http://100.79.83.113:8080/search?q=test&format=json" | python3 -m json.tool | head -10
```

In Claude Code:
```
Call the web_search tool with query "test"
```

Should return results from SearxNG with titles, URLs, and snippets.

### Troubleshooting

**MCP server fails to start — address already in use**
```bash
sudo fuser -k 3001/tcp
sudo systemctl start mcp-searxng
```

**MCP shows connected but web_search not called**
The Ollama model is defaulting to the built-in WebSearch tool. Use explicit invocation (see 4.1.3)
or add the CLAUDE.md instruction.

**MCP shows disconnected after reboot**
```bash
sudo systemctl status mcp-searxng
sudo journalctl -u mcp-searxng -n 20
```

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

All models are pulled via `ollama pull <name>`. Recommendations verified March 2026 — Qwen2.5
has been superseded by Qwen3.5 and Devstral across all use cases.

| Model | Command | VRAM | Context | Best For |
|---|---|---|---|---|
| qwen3.5:27b | `ollama pull qwen3.5:27b` | ~22GB | 256K tokens | General chat, research, documents, vision |
| devstral | `ollama pull devstral` | ~14GB | 128K tokens | Agentic coding, file editing, debugging |
| qwen3.5:9b | `ollama pull qwen3.5:9b` | ~7GB | 256K tokens | Fast fallback, simple tasks, vision |
| qwen3.5:35b | `ollama pull qwen3.5:35b` | ~27GB | 256K tokens | High quality — single model, both GPUs |

**Dual GPU operating modes:**

| Mode | Models Loaded | VRAM Used | Behaviour |
|------|--------------|-----------|-----------|
| Hot-swap (default) | qwen3.5:27b + devstral | ~36GB | Both in VRAM; switching is instant; 12GB headroom |
| Large model | qwen3.5:35b only | ~27GB | Single high-quality model with headroom |
| Fast + quality | qwen3.5:27b + qwen3.5:9b | ~29GB | Leave ~19GB free; useful when running other GPU workloads |

**Why Qwen3.5 replaces Qwen2.5:**
- Qwen3.5-27B outperforms Qwen2.5-72B on most benchmarks using less than half the VRAM
- Qwen3.5-27B SWE-bench Verified: 72.4% (vs Qwen2.5-Coder-32B which scores significantly lower)
- All Qwen3.5 models have 256K context (Qwen2.5 had 32K) and native vision support
- Devstral is purpose-built for agentic coding, outperforming Qwen2.5-Coder-32B on SWE-bench

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
there can be pulled with `ollama pull <name>`. Other models worth considering:

| Model | Use Case | VRAM | Notes |
|---|---|---|---|
| `qwen3:30b` | Very fast general inference | ~20GB | MoE — only 3B active params; extremely fast |
| `gemma3:27b` | Alternative to qwen3.5:27b | ~18GB | Google; strong vision + 140 languages |
| `qwen3.5:35b` | Step up from 27B quality | ~27GB | Good if not using hot-swap pair |
| `nomic-embed-text` | Text embeddings for RAG | <1GB | Required for persistent document search |

---

## Model Selection and Switching

This section covers how to choose the right model for each task and how to switch models
mid-task — the local equivalent of Claude Code's `--model` flag and `/model` command.

### Which model for which task

| Task | Recommended Model | Reason |
|------|-------------------|--------|
| General chat, Q&A, writing | `qwen3.5:27b` | Best general reasoning at this VRAM level |
| Web research and synthesis | `qwen3.5:27b` | Strong at summarising and cross-referencing |
| Writing code, debugging | `devstral` | Purpose-built agentic coding; top SWE-bench open model |
| Large codebase analysis | `devstral` or `qwen3.5:27b` | Both strong at code; devstral lighter |
| Quick lookups, simple questions | `qwen3.5:9b` | Fast; 256K context; vision capable |
| Complex multi-step reasoning | `qwen3.5:35b` | Larger model, better logical chains |
| Long documents (80+ pages) | `qwen3.5:27b` or `qwen3.5:35b` | 256K context handles very long documents |
| Images or screenshots | `qwen3.5:27b` or `qwen3.5:9b` | Native vision support in both |
| Start with planning, then code | Switch mid-task | Use qwen3.5:27b to plan, switch to devstral to implement |

**Rule of thumb**: start with `devstral` for coding tasks; use `qwen3.5:27b` for everything else.
Switch to `qwen3.5:9b` when you need quick answers and quality is not critical.

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
1. Start conversation with qwen3.5:27b
2. "I need to refactor this authentication module. Here's the current code: [paste code].
   What approach would you recommend and what are the trade-offs?"
3. [Review and agree on the plan]
4. Switch model to devstral (click dropdown)
5. "Now implement the refactoring we just planned."
6. [devstral uses the full conversation history including the plan]
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
# Default (from .aider.conf.yml) — devstral
aider

# Override for this session — general reasoning model
aider --model ollama/qwen3.5:27b

# Override for this session — large high-quality model
aider --model ollama/qwen3.5:35b
```

**Switching model mid-session:**

Type `/model` followed by the model name at the Aider prompt:

```
> /model ollama/qwen3.5:27b
```

```
> /model ollama/devstral
```

```
> /model ollama/qwen3.5:35b
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
> /model ollama/qwen3.5:27b
> Analyse this authentication module and identify what unit tests are missing.
  [Review the analysis]
> /model ollama/devstral
> Write the missing unit tests in tests/test_auth.py.
```

---

### Setting a per-project default model

If different projects benefit from different default models, create a `.aider.conf.yml` in
the project directory. It overrides `~/.aider.conf.yml` for that project only:

```yaml
# /path/to/my-python-project/.aider.conf.yml
model: ollama/devstral            # code-heavy project

# /path/to/my-writing-project/.aider.conf.yml
model: ollama/qwen3.5:27b         # writing/research project
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
- [ ] Open WebUI is accessible at `https://amelai.tail926601.ts.net`

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
- <a href="https://ollama.com/library/qwen3.5" target="_blank">Ollama Library — qwen3.5</a>
- <a href="https://ollama.com/library/devstral" target="_blank">Ollama Library — devstral</a>
- <a href="https://ollama.com/library/qwen3" target="_blank">Ollama Library — qwen3</a>
- <a href="https://ollama.com/library" target="_blank">Ollama Model Library</a>
