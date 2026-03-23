# Setting Up a New Client Machine for Local Claude Code

This guide covers everything needed to connect a new Windows PC to the AI server and use
Claude Code with local Ollama models, web search, and persistent session memory.

**What you get after setup:**
- Claude Code running against local AI models on the server (no Anthropic API required)
- Web search via self-hosted SearxNG
- Model listing and mid-session model switching
- Persistent memory across sessions (shared workspace on the server)
- Session notes automatically saved and committed to GitHub

---

## Prerequisites

Before starting, ensure:

- **Tailscale is installed and connected** on this machine — the AI server is at `100.79.83.113`
- **Claude Code is installed** — download from <a href="https://claude.ai/code" target="_blank">claude.ai/code</a>
- You can reach the AI server: open a browser and go to `https://amelai.tail926601.ts.net` — Open WebUI should load

---

## One-Time Setup (per machine)

### Step 1 — Set Permanent Environment Variables

These two variables tell Claude Code to use the local Ollama server instead of Anthropic's API.

**Option A — Add to your PowerShell profile (recommended):**

Open PowerShell and run:
```powershell
notepad $PROFILE
```

If Notepad opens an empty file (profile doesn't exist yet), that's fine. Add these two lines:

```powershell
$env:ANTHROPIC_AUTH_TOKEN = "ollama"
$env:ANTHROPIC_BASE_URL   = "http://100.79.83.113:11434"
```

Save and close. The variables will be set automatically every time you open PowerShell.

To apply immediately without restarting PowerShell:
```powershell
. $PROFILE
```

**Option B — Create a launch script:**

Create a file `StartClaude.ps1` on your Desktop with this content:

```powershell
$env:ANTHROPIC_AUTH_TOKEN = "ollama"
$env:ANTHROPIC_BASE_URL   = "http://100.79.83.113:11434"
claude
```

Right-click → Run with PowerShell to launch Claude Code with the correct settings.

---

### Step 2 — Register the MCP Server

The MCP server running on the AI server provides tools for web search, model management,
and workspace memory. Register it once per machine.

Open PowerShell (with the environment variables set from Step 1) and run:

```powershell
claude mcp add --transport sse searxng http://100.79.83.113:3001/sse
```

Verify it was saved:
```powershell
claude mcp list
```

You should see `searxng` listed.

---

### Step 3 — Set Up CLAUDE.md

This file provides Claude with instructions that apply to every session on this machine.
It tells Claude to use the local tools, avoid the broken built-in WebSearch, and manage
session memory through the workspace.

Create the file at `C:\Users\<YourUsername>\.claude\CLAUDE.md` — create the `.claude`
folder if it doesn't exist.

Paste in the following content exactly:

```markdown
# Claude Code Priorities

1. **All answers must be verified and double checked**
   - Every response should undergo thorough verification before submission
   - Multiple sources should be consulted when possible to ensure accuracy
   - Claims and statements should be fact-checked against reliable sources

2. **Answer from the web must be provided with a citation and where they are legal, regulatory or provides statistics must have an html link**
   - When referencing web sources, provide proper citations
   - For legal, regulatory, or statistical information, include HTML links in the format: [Source Name](URL)
   - All external links should open in a new browser tab using target="_blank"

3. **When writing content to a markdown file (.md) weblinks must be in the HTML format and be set to open in a new browser tab using target="_blank"**
   - All markdown links that reference web resources must use HTML format
   - Links must include target="_blank" attribute to open in new tabs
   - Example: <a href="https://example.com" target="_blank">Example Website</a>

4. **Accuracy is more important than speed, all answers need to be correct**
   - Prioritize correctness over rapid response
   - Verify all facts, figures, and technical details
   - When in doubt, acknowledge uncertainty rather than provide potentially incorrect information

## Session Memory and Tracking

This AI has a persistent workspace at `/opt/local-cc-workspace/` on the AI server.

**At the start of every session**: call `read_memory()` to load context from previous sessions.

**During a session**: note any important facts, decisions, or project updates to include in memory.

**At the end of a session** (when asked to "end session" or "save session"):
1. Call `save_session(topic, content)` — write a markdown summary of what was done
2. Call `read_memory()` — read current memory
3. Call `update_memory(content)` — write updated memory incorporating new facts
4. Call `workspace_commit(message)` — commit and push all changes

Session notes should follow this format:
    # Session: [Brief Topic]
    Date: YYYY-MM-DD

    ## What Was Done
    - [key tasks completed]

    ## Decisions Made
    - [decisions and rationale]

    ## Next Steps
    - [outstanding tasks]

## Tool Use — Web Search (IMPORTANT)

The built-in `Web Search` tool is **non-functional** in this environment — it always returns
0 results and must never be used.

A working replacement is available as an MCP tool:

- **NEVER use**: the built-in `Web Search` tool (broken — returns nothing)
- **ALWAYS use**: the `web_search` MCP tool for any and all web searches

This applies regardless of context. If you would search the web for any reason — news, facts,
research, current events, documentation — use the `web_search` MCP tool. Do not fall back to
the built-in Web Search under any circumstances.

## Implementation Notes

- These priorities apply to all responses, whether they involve code, research, or general knowledge
- When using external sources, ensure they are credible and up-to-date
- For technical topics, verify implementation details against official documentation
- When citing web sources, prioritize authoritative sources such as official websites, academic papers, or well-established news outlets
```

---

## Starting Claude Code

Open PowerShell and launch Claude Code with a specific model:

```powershell
claude --model qwen3.5:27b
```

**Recommended starting models:**

| Use case | Model |
|---|---|
| General chat, research, web search | `qwen3.5:27b` |
| Coding, debugging, file editing | `devstral:latest` |
| Quick lookups, simple tasks | `qwen3.5:9b` |
| Highest quality reasoning | `qwen3.5:35b` |

At the first prompt, Claude will call `read_memory()` automatically to load context from
previous sessions.

---

## During a Session

### Verify tools are connected
```
/mcp
```
Should show `searxng · ✔ connected`.

### List available models
```
list the available models
```

### Switch model mid-session
```
/model qwen3.5:27b
```
No restart needed — the new model takes effect on the next message.

### Web search
Just ask naturally — Claude will call `web_search` automatically:
```
What happened in the news today?
```

If a model doesn't pick it up automatically:
```
Call the web_search tool with query "your search terms"
```

---

## Ending a Session

When you're done, tell Claude:
```
end session
```

Claude will:
1. Save a dated session note to the workspace
2. Update the shared memory file with anything new learned
3. Commit and push to GitHub

No manual git commands needed.

---

## Available MCP Tools

These tools are provided by the MCP server and available in every session:

| Tool | What it does |
|---|---|
| `web_search` | Search the web via SearxNG |
| `list_models` | Show all Ollama models with sizes |
| `read_memory` | Load persistent context from previous sessions |
| `save_session` | Write a dated session note |
| `update_memory` | Update the shared memory file |
| `workspace_commit` | Commit and push workspace changes to GitHub |

---

## Troubleshooting

**Claude Code connects but model doesn't respond**
- Check the AI server is reachable: `ping 100.79.83.113`
- Verify Ollama is running on the server: open `https://amelai.tail926601.ts.net` in a browser

**MCP server shows disconnected**
- SSH into the server and check: `sudo systemctl status mcp-searxng`
- Restart if needed: `sudo systemctl restart mcp-searxng`

**Web search not working**
- Verify MCP is connected: `/mcp` in Claude Code
- If connected but not auto-invoking, use explicit call: `Call the web_search tool with query "..."`
- For reliable auto-invocation use `qwen3.5:27b` rather than coding-focused models

**Auth conflict warning at startup / model not loading into VRAM / "API Usage Billing" shown**

These symptoms all indicate a stored Anthropic login credential is overriding `ANTHROPIC_BASE_URL`,
routing requests to the real Anthropic API instead of Ollama. Run once to clear it:
```powershell
claude auth logout
```
Then relaunch — the environment variable handles authentication. When routing to Ollama correctly,
the header will show the model name without "API Usage Billing".

**`Test-NetConnection` gives false negatives for Tailscale ports**

`Test-NetConnection` reports TCP failures on all Tailscale ports even when connectivity is working.
Do not use it to test Tailscale reachability — it is unreliable in this environment.

Use these instead:
```powershell
# Test basic Tailscale connectivity
tailscale ping 100.79.83.113

# Test a specific service is actually responding
ssh steve@100.79.83.113
```

Or test the MCP server directly from the AI server:
```bash
curl -v --max-time 3 http://100.79.83.113:3001/sse
# Expect: HTTP/1.1 200 OK with SSE stream
```

**Updating the GitHub Personal Access Token (when it expires)**

Tokens expire based on the expiry you set when creating them. When the token expires,
`workspace_commit` will fail with an authentication error.

To update:

1. Go to GitHub → profile icon → **Settings** → **Developer settings** →
   **Personal access tokens** → **Tokens (classic)**
2. Either regenerate the existing token (same name, new value) or generate a new one
   with **`repo`** scope — copy it immediately
3. On the AI server, clear the stored credential and re-enter:

```bash
# Remove the stored credential
git credential reject <<EOF
protocol=https
host=github.com
EOF

# Trigger a new prompt by pushing
cd /opt/local-cc-workspace
git push
# Enter: Username = SureShotUK, Password = new token
```

Or edit the credentials file directly (simpler):

```bash
nano ~/.git-credentials
# Find the line: https://SureShotUK:<old-token>@github.com
# Replace the token value, save and exit
```

**Environment variables not persisting between sessions**
Confirm they were added to `$PROFILE` correctly:
```powershell
cat $PROFILE
```
Both `ANTHROPIC_AUTH_TOKEN` and `ANTHROPIC_BASE_URL` should appear.

---

## AI Server Reference

| Service | Address | Notes |
|---|---|---|
| Open WebUI (browser) | `https://amelai.tail926601.ts.net` | Browser-based chat interface (HTTPS) |
| Ollama API | `http://100.79.83.113:11434` | Model inference endpoint |
| MCP server | `http://100.79.83.113:3001/sse` | Tools: search, memory, models |
| SearxNG | `http://100.79.83.113:8080` | Self-hosted search (internal) |

---

*Last updated: 2026-03-04*
