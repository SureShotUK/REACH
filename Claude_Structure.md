<img src="Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# Claude Code Structure — terminai Repository

*Last updated: 6 July 2026*

This document explains how Claude Code is configured across the terminai repository: what loads where, the conventions we adopted in the July 2026 setup review, and how to verify it is all working.

---

## The Approach

One master rules file, project files that only add or explicitly override, and everything shared (MCP servers, skills, commands, permissions) registered at a level where **every project inherits it automatically**. The guiding principle: never restate a master rule in a project file — restatement creates drift when the master changes.

---

## Configuration Levels

| Level | Location | Contains | Applies to |
|---|---|---|---|
| **User (machine-wide)** | `~/.claude.json` | MCP servers (`rag`, `searxng`) registered at user scope | Every Claude Code session on this machine |
| **User (machine-wide)** | `~/.claude/settings.json` | context-mode plugin, hooks, model default | Every session on this machine |
| **User (machine-wide)** | `~/.claude/commands/` | `/db` skill | Every session on this machine |
| **Repo master** | `/docs/terminai/CLAUDE.md` | Shared rules: accuracy, clarifying questions, sources, link format, logo policy, MCP/skills policy, briefings | Every project in terminai |
| **Repo master** | `/docs/terminai/.claude/commands/` | Session commands (`/end-session`, `/sync-session`, `/session-*`, `/sync-files`) | Every project in terminai (inherited by subfolders) |
| **Repo master** | `/docs/terminai/.claude/settings.json` | Consolidated shared permission allowlist | Every project in terminai (see test below) |
| **Project** | `<project>/CLAUDE.md` | Project-specific guidance and explicit overrides only | That project and its subfolders |
| **Project** | `<project>/.claude/agents/` | Specialist agents (e.g. `hse-compliance-advisor`, `csharp-reviewer`, gemini researchers) | That project |
| **Project** | `<project>/.claude/settings.local.json` | Project-only permission entries | That project (not committed conventions — machine-local) |

---

## How Rules Load (Inheritance)

Claude Code walks **up** the directory tree from wherever you launch it and loads every `CLAUDE.md` it finds, **parents first**. Launching in `/docs/terminai/hseea/Violence/` loads, in order:

1. `/docs/terminai/CLAUDE.md` (master — always first)
2. `/docs/terminai/hseea/CLAUDE.md`
3. `/docs/terminai/hseea/Violence/CLAUDE.md`

So the master rules are always in context before any project rules, regardless of launch folder — no action needed. Skills, commands, and agents in parent `.claude/` directories are likewise inherited by subfolder sessions.

**Conventions adopted:**
- Every project `CLAUDE.md` opens with: *"Master rules in `/docs/terminai/CLAUDE.md` apply and load automatically. This file contains only project-specific guidance."*
- Overrides must say so explicitly. Example: `hseea/CLAUDE.md` deliberately overrides the master logo policy (Noxdown logo instead of Portland Long) and states that in its heading.

---

## MCP Servers and Skills

All shared MCP servers are registered at **user scope** in `~/.claude.json` so they work in every project:

| Server | Type | Purpose |
|---|---|---|
| `rag` | stdio — `/docs/terminai/rag-mcp/index.mjs` | Amelai pgvector knowledge base (`/db` invokes it) |
| `searxng` | SSE — `http://100.79.83.113:3001/sse` | Local web search |
| `context-mode` | plugin (in `~/.claude/settings.json`) | Context-window protection tools |

**Policy** (documented in the master CLAUDE.md): new MCP servers are added with `claude mcp add <name> --scope user ...` so every project gets them, unless stated otherwise. Do **not** use `~/.claude/.mcp.json` — Claude Code does not read that file (this is exactly why rag was silently unavailable before the review). Verify registrations any time with `claude mcp list`.

---

## Permissions

The shared allowlist (git, common read-only shell commands, WebSearch, gov.uk/hse.gov.uk WebFetch, context-mode/rag/searxng tools, session-log file writes) lives in the checked-in `/docs/terminai/.claude/settings.json`. Project `settings.local.json` files should hold only genuinely project-specific entries (e.g. hseea's fire-safety WebFetch domains).

---

## Two Backends, One Repo

Claude Code is used with two backends against the same NAS-hosted repo (reached from Windows machines over Tailscale):

| | Pro login (Anthropic API) | Local login (Ollama on Amelai) |
|---|---|---|
| Typical machine | Any | Windows PC / laptop, Windows Terminal |
| `WebSearch` tool | ✔ Works (server-side, runs inside Anthropic's API) | ✘ Silently returns 0 results |
| `searxng` MCP search | ✔ Works (harmless duplicate) | ✔ Works — the **only** search path |
| `WebFetch` | ✔ Works | Works via local model; `curl` status check is the reliable fallback |
| claude.ai connectors (Gmail/Drive/Calendar) | ✔ | ✘ Not available |
| Repo rules, agents, checklists, commands | Identical — they live on the NAS | Identical — they live on the NAS |

**Why it "just works"**: the master CLAUDE.md and the `deep-researcher` agent carry a backend-agnostic search rule — *WebSearch first; on error or 0 results, immediately retry with `mcp__searxng__web_search`* — and a backend-free link-verification fallback (`curl -s -o /dev/null -w "%{http_code}" -L <url>`). Because those instructions travel with the repo, both logins behave correctly with no manual steering.

**Per-machine setup**: MCP registrations live in each machine's own `%USERPROFILE%\.claude.json` / `~/.claude.json` — they do not travel with the repo. The Windows PC (SteveOP) and laptop (StevesLenovo) were registered correctly in March 2026; Amelai was fixed in July 2026. On any machine, verify with `claude mcp list` (both `searxng` and `rag` should show ✔ Connected). If either is missing:

```bash
claude mcp add searxng --scope user --transport sse http://100.79.83.113:3001/sse
claude mcp add rag --scope user --env PG_HOST=amelai.tail926601.ts.net -- node "/path/to/NAS/terminai/rag-mcp/index.mjs"
```

Notes:
- Do **not** put MCP servers in `~\.claude\.mcp.json` — Claude Code does not read that file (this was the original breakage on both Amelai and Windows).
- The searxng chain is: Claude Code → mcp-searxng systemd service on Amelai (port 3001, SSE) → SearXNG Docker (port 8080). Port 3001 must have a Tailscale ACL entry — a missing ACL rule shows as `searxng · ✘ failed` in `/mcp` (see `it/NewPC/SearXNG_Fix.md`).
- The rag command's PG/Ollama env vars must point at Amelai's Tailscale name from Windows (see `it/NewPC/CLAUDE.md` RAG section for the full env list); adjust the NAS path to how the share is mapped on that machine.

---

## Verification: the `git log` test from inside hseea

Official docs confirm CLAUDE.md/skills/commands inherit from parent directories, but are **ambiguous on whether a repo-root `settings.json` applies when launching from a subfolder**. This test settles it. It uses `git log` because it is in the root allowlist but **not** in `hseea/.claude/settings.local.json` — so the result can only come from the root file.

**Prerequisite:** the consolidated `/docs/terminai/.claude/settings.json` must be in place.

**How to run:**

```bash
cd /docs/terminai/hseea
claude
```

Then ask Claude: `run git log -1 --oneline`

**If it works as expected:** the command executes immediately and Claude replies with the latest commit line (e.g. `ddfb25d End of session — ...`). No permission dialog appears at any point. You can double-check the source by running `/doctor` or `/permissions` in that session — the allow rule `Bash(git log:*)` should show as coming from the project settings at `/docs/terminai/.claude/settings.json`.

**If there is a problem:** a permission prompt appears before the command runs, looking like:

```
Bash command
  git log -1 --oneline

Do you want to proceed?
❯ 1. Yes
  2. Yes, and don't ask again for git log commands
  3. No, and tell Claude what to do differently (esc)
```

That prompt means the subfolder session did **not** pick up the repo-root settings file (only its own `.claude/` and `~/.claude/`). Fix: move the shared allowlist into `~/.claude/settings.json` (user level) instead — same effect, applies machine-wide. Ask Claude to do this and it will merge the list for you.

---

## Key Documentation

- <a href="https://code.claude.com/docs/en/claude-directory" target="_blank">The .claude directory</a> — what lives in `.claude/` folders and how they are discovered
- <a href="https://code.claude.com/docs/en/memory" target="_blank">Memory (CLAUDE.md)</a> — CLAUDE.md loading order and upward recursion
- <a href="https://code.claude.com/docs/en/settings" target="_blank">Settings</a> — `settings.json` vs `settings.local.json`, precedence, permission rule syntax
- <a href="https://code.claude.com/docs/en/mcp" target="_blank">MCP servers</a> — local / project / user scopes and `claude mcp add`
- <a href="https://code.claude.com/docs/en/skills" target="_blank">Skills and commands</a> — `.claude/skills/` and `.claude/commands/` discovery
- <a href="https://code.claude.com/docs/en/sub-agents" target="_blank">Subagents</a> — `.claude/agents/` definitions

---

*Sources: <a href="https://code.claude.com/docs/en/claude-directory" target="_blank">Claude Code docs — .claude directory</a>, <a href="https://code.claude.com/docs/en/settings" target="_blank">Settings</a>, <a href="https://code.claude.com/docs/en/mcp" target="_blank">MCP</a>. All links verified 6 July 2026.*
