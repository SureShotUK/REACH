<img src="../../Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# SteveOP — MCP Server Setup Guide

**Machine**: SteveOP (Steve's personal Windows 11 desktop)
**Purpose**: Configure Claude Code with two MCP servers — TradingView (stock chart analysis) and RAG (Amelai knowledge base search)

---

## SteveOP Specifications

| Component | Spec |
|---|---|
| CPU | AMD Ryzen 7 9800X3D |
| GPU | NVIDIA RTX 5070 Ti 16GB |
| Motherboard | MSI MAG X870E Tomahawk WIFI (AM5, DDR5) |
| RAM | 32GB Viper Venom DDR5 6000MT/s |
| Storage | Samsung 9100 Pro NVMe 2TB (PCIe 5.0) |
| OS | Windows 11 |

---

## Overview

This guide sets up two MCP (Model Context Protocol) servers that Claude Code spawns automatically at startup:

| MCP Server | What it does |
|---|---|
| `tradingview` | Connects to TradingView Desktop via Chrome DevTools Protocol (port 9222) — gives Claude 81 tools to read charts, indicators, watchlists, and run morning briefs |
| `rag` | Connects to Amelai's pgvector knowledge base over Tailscale — gives Claude semantic search across Portland Fuel's internal documents (HSE regs, DSEAR, maintenance procedures, REACH, etc.) |

**Architecture:**
```
Claude Code  ←→  tradingview MCP  ←→  CDP port 9222  ←→  TradingView Desktop
Claude Code  ←→  rag MCP          ←→  Tailscale       ←→  Amelai PostgreSQL + Ollama
```

---

## Prerequisites

Before starting, confirm the following:

- [ ] TradingView Desktop installed and a paid subscription active (required for real-time data)
- [ ] Tailscale installed and connected (required for RAG MCP)
- [ ] `I:\terminai` mapped (NAS drive `\\irwinnas\MyDocs\terminai`) — required for RAG MCP code
- [ ] Internet access to clone the TradingView MCP repository from GitHub

---

## Step 1 — Install Node.js (if not already installed)

Check first:

```powershell
node --version
```

If Node.js is not installed, download and install **Node.js 18 or later** (LTS recommended) from:

<a href="https://nodejs.org/en/download" target="_blank">https://nodejs.org/en/download</a>

Tick "Add to PATH" during installation, then open a new PowerShell window and confirm `node --version` works.

---

## Step 2 — Install Claude Code (if not already installed)

Download the Claude Code Windows installer from:

<a href="https://claude.ai/download" target="_blank">https://claude.ai/download</a>

After installation, log in with your Anthropic account or configure the PowerShell setup script in Step 6 if pointing at Amelai's Ollama backend.

---

## Step 3 — Clone and Install the TradingView MCP Server

Open PowerShell and run:

```powershell
git clone https://github.com/LewisWJackson/tradingview-mcp-jackson.git C:\Users\irwin\tradingview-mcp-jackson
cd C:\Users\irwin\tradingview-mcp-jackson
npm install
```

This installs all dependencies locally. The MCP server entry point is:
`C:\Users\irwin\tradingview-mcp-jackson\src\server.js`

---

## Step 4 — Configure Trading Rules

The TradingView MCP uses a `rules.json` file to define your watchlist, bias criteria, and risk rules. These are applied automatically during the morning brief.

```powershell
cd C:\Users\irwin\tradingview-mcp-jackson
copy rules.example.json rules.json
notepad rules.json
```

Fill in at minimum:
- `watchlist` — the symbols you want scanned each morning (e.g. `["GBPUSD", "XAUUSD", "US30"]`)
- `bias_criteria` — what makes a setup bullish, bearish, or neutral for you
- `risk_rules` — rules Claude checks before recommending any trade

The morning brief will fail with "No rules.json found" if this step is skipped.

---

## Step 5 — Add MCP Servers to `.claude.json`

Claude Code on Windows stores MCP server configuration in `C:\Users\irwin\.claude.json` (a single file in the home directory, **not** the `.claude\` folder).

Open `C:\Users\irwin\.claude.json` and add the `mcpServers` block alongside any existing top-level keys. If other MCP servers are already present (e.g. `searxng`), add `tradingview` and `rag` inside the existing `mcpServers` object:

```json
"mcpServers": {
  "searxng": {
    "type": "sse",
    "url": "http://<amelai-tailscale-ip>:3001/sse"
  },
  "tradingview": {
    "command": "node",
    "args": ["C:\\Users\\irwin\\tradingview-mcp-jackson\\src\\server.js"]
  },
  "rag": {
    "command": "node",
    "args": ["\\\\irwinnas\\MyDocs\\terminai\\rag-mcp\\index.mjs"],
    "env": {
      "PG_HOST": "amelai.tail926601.ts.net",
      "PG_PORT": "5432",
      "PG_DATABASE": "openwebui_vectors",
      "PG_USER": "openwebui",
      "OLLAMA_BASE_URL": "http://amelai.tail926601.ts.net:11434",
      "EMBED_MODEL": "nomic-embed-text",
      "RAG_TOP_K": "5"
    }
  }
}
```

> **Note:** `.claude.json` may be hidden in File Explorer. Enable "Show hidden items" under View → Show, or edit it directly in VS Code or Notepad.

---

## Step 6 — Create the PowerShell Setup Script

Create a script to set environment variables before launching Claude Code. This keeps credentials out of config files.

Save the following as `C:\Users\irwin\start-claude.ps1`:

```powershell
# PostgreSQL password for Amelai RAG knowledge base (via Tailscale)
$env:PGPASS = 'your-postgresql-password-here'

# If using Amelai's Ollama as the Claude backend (same as StevesLenovo):
# $env:ANTHROPIC_AUTH_TOKEN = "ollama"
# $env:ANTHROPIC_BASE_URL   = "http://amelai.tail926601.ts.net:11434"

# Launch Claude Code
claude
```

**Important:** Use **single quotes** around the password. Double quotes cause PowerShell to expand `$` characters as variable references, silently corrupting the password.

Run Claude Code by executing this script in PowerShell:

```powershell
C:\Users\irwin\start-claude.ps1
```

Or right-click the file → "Run with PowerShell".

> **First run only:** If PowerShell blocks execution with a policy error, run this once:
> `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`

---

## Step 7 — Install the /db Skill Globally

The `/db` skill provides explicit knowledge base searches from the Claude Code prompt. It needs to be placed in Claude Code's global commands directory on SteveOP.

Create the directory if it doesn't exist:

```powershell
New-Item -ItemType Directory -Force -Path "C:\Users\irwin\.claude\commands"
```

Then create `C:\Users\irwin\.claude\commands\db.md` with this content:

```markdown
# /db — Query Amelai's Knowledge Base

Search the local RAG knowledge base for information stored in Amelai's PostgreSQL vector database.

## Usage

/db <query>
/db #<collection-name> <query>

**Examples:**
- `/db what are the requirements for DSEAR risk assessments?`
- `/db list` — list all available knowledge base collections

## How it works

The `rag` MCP server is automatically active in every session. It exposes two tools:

- **`rag_search(query, collection_name?)`** — embed the query via Ollama (nomic-embed-text), run a cosine similarity search against pgvector, return the top 5 matching document chunks with scores and source attribution
- **`rag_list_collections()`** — list all knowledge base collections and their chunk counts

## Using this skill

When invoked as `/db <query>`, do the following:

1. If the first word starts with `#`, extract it as the collection filter (e.g. `#HSE` → search only the HSE collection)
2. Call `mcp__rag__rag_search` with the remaining text as `query` and the collection UUID (if known) as `collection_name`
3. Format the results clearly, citing the source file and relevance score for each result
4. If no results are found, say so and suggest the user check that documents have been uploaded via Open WebUI (Workspace → Knowledge)

To list available collections, call `mcp__rag__rag_list_collections` and display the results in a table showing collection name and chunk count.

The collection names from pgvector are UUIDs that map to knowledge bases in Open WebUI. The tool resolves them to human-readable names automatically via the Open WebUI SQLite database.
```

---

## Step 8 — Launch TradingView in Debug Mode

The TradingView MCP communicates with TradingView Desktop via the Chrome DevTools Protocol on port 9222. TradingView must be started with this debug port enabled — the normal shortcut does not enable it.

**Always use this batch file to launch TradingView when you intend to use the MCP:**

```
C:\Users\irwin\tradingview-mcp-jackson\scripts\launch_tv_debug.bat
```

Create a desktop shortcut to this batch file for convenience.

> If TradingView is already running (without debug mode), close it first and relaunch via the script. The MCP will fail with `cdp_connected: false` if TradingView was not started with the debug port.

---

## Step 9 — Verify Everything Works

1. Run `start-claude.ps1` to launch Claude Code with environment variables set
2. At the Claude Code prompt, check the MCP servers connected at startup — you should see both `tradingview` and `rag` listed
3. Test the RAG MCP:
   ```
   /db list
   ```
   Expected: a table of knowledge base collections (names may appear as UUIDs if Tailscale resolves correctly but `docker exec` is unavailable from Windows — this is normal)

4. Test the TradingView MCP (TradingView must be open via the debug launch script):
   ```
   Use tv_health_check to confirm TradingView is connected
   ```
   Expected: `cdp_connected: true`

5. Test a morning brief:
   ```
   Run tv_morning_brief for my watchlist
   ```

---

## Usage Reference

### RAG Knowledge Base

| Command | What it does |
|---|---|
| `/db list` | List all knowledge base collections and chunk counts |
| `/db <query>` | Search all collections for the query |
| `/db #<uuid> <query>` | Search a specific collection by UUID |

Or just ask a question naturally — Claude searches the knowledge base automatically when relevant.

> **Note:** Collection names appear as UUIDs on SteveOP (e.g. `1c973c09-...`) because the human-readable name lookup requires `docker exec` on Amelai. Copy the UUID from `/db list` output and pass it with `#` to restrict searches. Search results are otherwise identical to Amelai.

### TradingView MCP (81 tools)

Key tools available once connected:

| Category | Example tools |
|---|---|
| Charts | `get_chart_data`, `set_symbol`, `set_timeframe` |
| Indicators | `get_indicator_values`, `add_indicator` |
| Watchlist | `get_watchlist`, `scan_watchlist` |
| Drawing | `draw_trendline`, `draw_rectangle` |
| Morning brief | `morning_brief` — scans watchlist, reads indicators, returns session bias |
| Session memory | `session_save`, `session_get` — compare today vs yesterday |
| Replay mode | `replay_start`, `replay_step`, `replay_stop` |

---

## Troubleshooting

| Problem | Solution |
|---|---|
| `cdp_connected: false` | TradingView not running with debug port. Close TradingView and relaunch via `launch_tv_debug.bat` |
| `ECONNREFUSED` on port 9222 | TradingView not running, or another application is using port 9222 |
| RAG MCP: connection refused | Tailscale not connected, or Amelai is asleep. Wake via Alexa: "Alexa, open wol machine" |
| RAG MCP: PGPASS authentication failed | `PGPASS` env var not set or incorrect. Confirm the `start-claude.ps1` script ran before Claude Code |
| MCP servers not listed at startup | Check `C:\Users\irwin\.claude\.mcp.json` syntax (no trailing commas). Restart Claude Code |
| `/db list` returns no collections | Documents not yet uploaded to Open WebUI. Add via Open WebUI → Workspace → Knowledge |
| `morning_brief` — "No rules.json found" | Run `copy rules.example.json rules.json` in `C:\Users\irwin\tradingview-mcp-jackson\` and fill it in |
| `tv` CLI command not found | Run `npm link` from `C:\Users\irwin\tradingview-mcp-jackson\` (optional — MCP tools work without it) |
| PowerShell script blocked | Run: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser` |
| RAG search returns stale/irrelevant results | Ollama on Amelai may be loading — wait 30 seconds and retry |

---

## Tailscale ACL Note

The RAG MCP connects to two ports on Amelai over Tailscale:
- Port `5432` — PostgreSQL (pgvector knowledge base)
- Port `11434` — Ollama (embedding model)

If searches fail with a connection error despite Tailscale being connected, check that SteveOP's Tailscale device IP has allow rules for these ports in the Tailscale ACL at:
<a href="https://login.tailscale.com/admin/acls" target="_blank">https://login.tailscale.com/admin/acls</a>

Hardcoded device IPs in the ACL go stale if a device is re-added to the tailnet (it gets a new IP). If this happens, check the current IP with `tailscale ip` on the affected device and update the ACL entry.
