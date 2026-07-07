# SearXNG MCP Web Search — Troubleshooting & Fix Log

**Date:** 23 March 2026
**Issue:** Web search via MCP not working in Claude Code on Windows 11
**Resolution:** Two root causes identified and fixed

---

## Architecture Overview

Web search in Claude Code uses a chain of three components:

```
Claude Code (Windows) → MCP Server (port 3001, amelai) → SearXNG (port 8080, amelai)
```

- **MCP server** — Python/FastMCP service running as `mcp-searxng.service` on amelai, listens on `0.0.0.0:3001`
- **SearXNG** — Docker container (`searxng/searxng:latest`), bound to `100.79.83.113:8080`
- **Claude Code** — Connects to MCP via `http://100.79.83.113:3001/sse`

The MCP server is **not** a Docker container — it runs as a systemd service.

---

## Root Cause 1 — Port 3001 Missing from Tailscale ACL

### Symptom
`/mcp` in Claude Code showed `searxng · ✘ failed`.

### Cause
The Tailscale ACL policy did not include port 3001. When any ACL rules are defined in Tailscale, it switches to default-deny — only explicitly listed ports are accessible between devices.

### Diagnosis
```powershell
# From Windows — confirmed TCP failed on all ports
Test-NetConnection -ComputerName 100.79.83.113 -Port 3001
# TcpTestSucceeded : False
```

```bash
# On server — confirmed MCP server IS listening correctly
ss -tlnp | grep 3001
# LISTEN 0  2048  0.0.0.0:3001  0.0.0.0:*  users:(("python",...))

# Confirmed MCP server responds correctly via Tailscale IP
curl -v --max-time 3 http://100.79.83.113:3001/sse
# HTTP/1.1 200 OK — SSE stream active
```

### Fix
Added `"100.79.83.113:3001"` to the Tailscale ACL at **admin.tailscale.com → Access Controls**:

```json
{
    "action": "accept",
    "src":    ["*"],
    "dst": [
        "100.79.83.113:22",
        "100.79.83.113:443",
        "100.79.83.113:3001",
        "100.79.83.113:8087",
        "100.79.83.113:8088",
        "100.79.83.113:8188",
        "100.79.83.113:11434"
    ]
}
```

> **Note:** `src: ["*"]` means any authenticated device on the tailnet only. Tailscale IPs are not publicly routable — random internet traffic cannot reach them.

---

## Root Cause 2 — Stored Anthropic Auth Credential Overriding Ollama

### Symptom
Claude Code showed `qwen3.5:27b · API Usage Billing` in the header. The model never loaded into GPU VRAM despite VRAM being free. Claude Code hung indefinitely ("Deliberating…", "Shenaniganing…") without producing output.

### Cause
A previously stored Anthropic login credential was overriding the `ANTHROPIC_BASE_URL` environment variable. Claude Code was sending requests to the real Anthropic API with model name `qwen3.5:27b` — an invalid Anthropic model — causing it to hang.

When routing to Ollama correctly, the header should not show "API Usage Billing".

### Diagnosis
Environment variables were confirmed correctly set:
```powershell
echo $env:ANTHROPIC_BASE_URL    # http://100.79.83.113:11434
echo $env:ANTHROPIC_AUTH_TOKEN  # ollama
```

Despite this, requests were not reaching Ollama (confirmed by GPU VRAM remaining empty and Ollama logs showing no activity).

### Fix
```powershell
claude auth logout
```

Cleared the stored credential. On next launch, `ANTHROPIC_BASE_URL` was respected and requests routed to Ollama correctly. The model loaded across both GPUs via NVLink and web search functioned.

---

## Misleading Diagnostic — `Test-NetConnection` False Negatives

Throughout diagnosis, `Test-NetConnection` reported TCP failures on all ports (22, 443, 3001, 11434) even after the ACL was fixed:

```powershell
Test-NetConnection -ComputerName 100.79.83.113 -Port 22
# TcpTestSucceeded : False
```

However, SSH to `100.79.83.113` via Windows terminal worked correctly throughout. **`Test-NetConnection` is unreliable in this environment** — do not use it to test Tailscale connectivity. Use SSH or application-level tests instead.

---

## Verification

After both fixes, MCP connected successfully:

```
/mcp
Status: ✔ connected
Auth: ✔ authenticated
URL: http://100.79.83.113:3001/sse
Tools: 6 tools
```

Web search via `web_search` MCP tool returned results correctly. Ollama loaded `qwen3.5:27b` split across both RTX 3090s.

---

## Outstanding Items

All resolved 23 March 2026.

| Item | Status |
|---|---|
| MCP re-registered as user-scoped | ✔ Done |
| `hf-env` auto-activation removed from `~/.bashrc` | ✔ Done |
| Open WebUI Ollama URL fixed to `http://` | ✔ Done |

---

## Quick Reference — MCP Server Management

```bash
# Check status
sudo systemctl status mcp-searxng

# Restart
sudo systemctl restart mcp-searxng

# View logs
sudo journalctl -u mcp-searxng -f
```

```powershell
# Verify MCP registered on Windows
claude mcp list

# Re-register as user-scoped (works in all projects)
claude mcp remove searxng
claude mcp add --transport sse --scope user searxng http://100.79.83.113:3001/sse
```

> **Windows MCP config location**: Claude Code stores MCP server definitions in `%USERPROFILE%\.claude.json` (a single flat file in the home directory — e.g. `C:\Users\SteveIrwin\.claude.json`). This is **not** `~\.claude\.mcp.json` or `~\.claude\settings.json`. The `claude mcp add` command writes to this file automatically. To manually add or edit servers, open `.claude.json` directly in a text editor.

---

## Update 7 July 2026 — 400 Bad Request after Tailscale serve reorganisation

**Symptom**: `searxng` shows ✔ Connected in `claude mcp list` on every machine, but every search returns:

```
Search error: Client error '400 Bad Request' for url 'http://100.79.83.113:8080/search?...&format=json'
```

**Cause**: the SearXNG Docker container's port mapping changed to `127.0.0.1:18080` (plus LAN `192.168.1.192:8080`), and `tailscale serve` took over port 8080 on Amelai's tailnet IP as an **HTTPS** proxy (`https://amelai.tail926601.ts.net:8080` → `http://127.0.0.1:18080`). The MCP server's hardcoded `SEARXNG_URL = "http://100.79.83.113:8080/search"` then sent plain HTTP to an HTTPS listener → 400 ("Client sent an HTTP request to an HTTPS server").

**Fix**: in `/opt/mcp-searxng/server.py` set:

```python
SEARXNG_URL = "http://127.0.0.1:18080/search"
```

then `sudo systemctl restart mcp-searxng`. Localhost is the right target — the MCP server runs on Amelai itself, so it needs neither Tailscale nor TLS to reach the container.

**Diagnostic that separates this from the ACL failure**: ACL problems show as `searxng · ✘ failed` (client cannot reach port 3001); this fault shows as **connected but erroring searches**, identically on all machines, because it is server-side on Amelai. Confirm with `curl -s 'http://127.0.0.1:18080/search?q=test&format=json' -o /dev/null -w '%{http_code}'` on Amelai (expect 200).
