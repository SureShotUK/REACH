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

**Path syntax in Edit/Write rules**: a single leading `/` means *relative to the project root*, not an absolute filesystem path (absolute is `//`). The session-log rules therefore use `Edit(/**/SESSION_LOG.md)` etc. — this also makes them work unchanged on Windows, where the repo root is a UNC path. See <a href="https://code.claude.com/docs/en/permissions" target="_blank">Permissions — Read and Edit rules</a>. (Fixed 7 July 2026 — the original rules used `/docs/terminai/**/...`, which resolved to a non-existent project-relative path and never matched.)

**Because the settings file is checked into the repo on the NAS, it is already shared with every machine** — Windows included. There is nothing to copy into `C:\Users\<user>\.claude\`; the file applies automatically whenever Claude Code is **launched from inside the repo** (see the test below). Each machine's own `~/.claude/settings.json` is only for machine-wide preferences (plugins, hooks, model default), not for the shared allowlist.

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
```

Actual code run from PowerShell:

```
cd \\irwinnas\MyDocs\terminai ; claude mcp remove searxng
cd $env:USERPROFILE ; claude mcp remove searxng
claude mcp add searxng --scope user --transport sse http://100.79.83.113:3001/sse
```
**rag on Windows machines — run from a local copy, not the NAS** (established on StevesLenovo, 7 July 2026): starting the server from `\\irwinnas\...` times out after 30 s, because node must resolve hundreds of `node_modules` files over SMB/Tailscale — each a network round trip. The config was never the problem; the share is. Copy once, register against the copy:

```powershell
robocopy \\irwinnas\MyDocs\terminai\rag-mcp C:\Tools\rag-mcp /E
claude mcp add rag --scope user --env PG_HOST=amelai.tail926601.ts.net --env PG_PORT=5432 --env PG_DATABASE=openwebui_vectors --env PG_USER=openwebui --env OLLAMA_BASE_URL=http://amelai.tail926601.ts.net:11434 --env EMBED_MODEL=nomic-embed-text --env RAG_TOP_K=5 -- "C:\Program Files\nodejs\node.exe" "C:\Tools\rag-mcp\index.mjs"
```

Actual code run from PowerShell:

```
robocopy \\irwinnas\MyDocs\terminai\rag-mcp C:\Tools\rag-mcp /E
claude mcp remove rag
claude mcp add rag --scope user --env PG_HOST=amelai.tail926601.ts.net --env PG_PORT=5432 --env PG_DATABASE=openwebui_vectors --env PG_USER=openwebui --env OLLAMA_BASE_URL=http://amelai.tail926601.ts.net:11434 --env EMBED_MODEL=nomic-embed-text --env RAG_TOP_K=5 -- "C:\Program Files\nodejs\node.exe" "C:\Tools\rag-mcp\index.mjs"
claude mcp list
```

Maintenance: re-run the `robocopy` line if `rag-mcp/index.mjs` on the NAS ever changes (it rarely does). Two further Windows notes: `rag_list_collections` shows bare collection IDs instead of friendly names remotely (the name lookup needs `docker exec` on Amelai — searches are unaffected), and searches require the Tailscale ACL to allow ports 5432 (Postgres) and 11434 (Ollama) to Amelai.

Notes:
- Do **not** put MCP servers in `~\.claude\.mcp.json` — Claude Code does not read that file (this was the original breakage on both Amelai and Windows).
- The searxng chain is: Claude Code → mcp-searxng systemd service on Amelai (port 3001, SSE) → SearXNG Docker at `http://127.0.0.1:18080` (local to Amelai). Port 3001 must have a Tailscale ACL entry — a missing ACL rule shows as `searxng · ✘ failed` in `/mcp` (see `it/NewPC/SearXNG_Fix.md`).
- **Failure signature "Search error: Client error '400 Bad Request'"** (MCP connects, searches fail — seen 7 July 2026): the SearXNG URL in `/opt/mcp-searxng/server.py` pointed at `http://100.79.83.113:8080`, but `tailscale serve` now occupies 8080 on the tailnet IP as HTTPS, so plain HTTP gets 400 ("Client sent an HTTP request to an HTTPS server"). Fix: point `SEARXNG_URL` at `http://127.0.0.1:18080/search` and `sudo systemctl restart mcp-searxng`. This breaks every machine at once — it is a server-side fault on Amelai, not a client registration problem.
- The rag command's PG/Ollama env vars must point at Amelai's Tailscale name from Windows (see `it/NewPC/CLAUDE.md` RAG section for the full env list); adjust the NAS path to how the share is mapped on that machine.

---

## Verification: the `git log` test from inside hseea

Official docs confirm CLAUDE.md/skills/commands inherit from parent directories, but are **ambiguous on whether a repo-root `settings.json` applies when launching from a subfolder**. This test settles it. It uses `git log` because it is in the root allowlist but **not** in `hseea/.claude/settings.local.json` — so the result can only come from the root file.

**Prerequisite:** the consolidated `/docs/terminai/.claude/settings.json` must be in place.

**Two conditions for a valid test** (the 7 July run on SteveOP/StevesLenovo missed both — see below):

1. **The session's working directory must be inside the repo.** Claude Code only discovers project config (settings, CLAUDE.md, agents, commands) from the folder it is launched in and its parents. A session started in `C:\Users\<user>` that reaches the repo via `git -C '\\irwinnas\...'` loads **none** of the repo config — no allowlist, no master rules, no agents. On Windows, `cd` into the NAS repo first (PowerShell accepts UNC paths):
   ```powershell
   cd \\irwinnas\MyDocs\terminai\hseea    # or the mapped drive letter, e.g. T:\terminai\hseea
   claude
   ```
2. **The command must literally start with `git log`.** The allow rule `Bash(git log:*)` is a prefix match — `git -C <path> log ...` or anything with `|| echo ...` appended does not match and will prompt even on a perfectly configured machine. Ask exactly: `run git log -1 --oneline`.

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

That prompt means the subfolder session did **not** pick up the repo-root settings file (only its own `.claude/` and `~/.claude/`). Two fixes, in order of preference:

1. **Point the session at the NAS settings file with `--settings`** (stays single-source, no per-machine copies): `claude --settings '\\irwinnas\MyDocs\terminai\.claude\settings.json'` (on Amelai: `claude --settings /docs/terminai/.claude/settings.json`). Make it automatic in the PowerShell profile (`notepad $PROFILE`):
   ```powershell
   function claude { & claude.exe --settings '\\irwinnas\MyDocs\terminai\.claude\settings.json' @args }
   ```
2. **Merge the allowlist into each machine's `~/.claude/settings.json`** — works, but it is then a per-machine copy that drifts when the shared list changes.

Do **not** use `CLAUDE_CONFIG_DIR` pointed at a shared NAS folder to solve this — it relocates the whole config directory including credentials, history and session state, so the Pro and Ollama logins (and concurrent machines) would trample each other.

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

---

## Next Steps - July 6th

Verification tests outstanding from the 6 July 2026 restructure (commit `1cdf3a4`). Run these in the next session(s) and tick them off; each says what success and failure look like.

- [x] **Agent registration check** — ✅ verified 7 July 2026. Note: the `/agents` wizard has been removed from Claude Code; the check is now done by asking the session itself, e.g. from inside the project folder: `claude -p "Reply with only a comma-separated list of the agent type names available to your Agent tool"` (or ask the same in an interactive session).
  *Result*: hseea returned `deep-researcher, ea-permit-consultant, hse-compliance-advisor` (plus built-ins); XmlDotnetCoding returned `csharp-reviewer, csharp-xml-expert, dotnet-tester, deep-researcher` — all expected agents registered. Also confirmed cross-machine on StevesLenovo (hseea session, 7 July): all three repo agents present, proving NAS-hosted `.claude/agents/` definitions load identically on Windows.
  *If one goes missing in future*: its YAML frontmatter didn't parse — check the file's opening `---` block in `<project>/.claude/agents/`.

- [x] **Permission inheritance test (`git log` from hseea)** — ✅ passed 7 July 2026: launched from inside the repo, `git log -1 --oneline` ran with no permission prompt, confirming the repo-root `settings.json` reaches subfolder launches on Windows too. (The first attempt that day was invalid — the session wasn't started inside the repo, so Claude used `git -C '\\irwinnas\...'`, which loads no repo config and breaks the `Bash(git log:*)` prefix match. Full walkthrough and the two valid-test conditions are in the "Verification" section above.)
  *If it ever regresses*: use `claude --settings` pointing at the NAS settings file (preferred) or merge into `~/.claude/settings.json` — details in the walkthrough section.

- [x] **Web search on the Pro login** — ✅ passed 7 July 2026: *"search the web for today's Brent crude price"* answered via the built-in `WebSearch` tool on the first attempt ($72.89/bbl returned with sources), no searxng fallback needed.

- [x] **Web search on the local Ollama login** (from Windows PC or laptop) — ✅ passed 7 July 2026 on StevesLenovo: same Brent crude question answered with 4 referenced sources, no manual steering, following the mcp-searxng server fix.
  *If it regresses*: "no search results" → run `/mcp`; if `searxng · ✘ failed`, check the Tailscale ACL for port 3001 and the `mcp-searxng` service on Amelai; if connected but every search errors with 400, see the 7 July section in `it/NewPC/SearXNG_Fix.md`.

- [ ] **Windows MCP health check** — on SteveOP and StevesLenovo run `claude mcp list`.
  *Pass*: `rag` and `searxng` both show ✔ Connected.
  *Fail*: re-register at user scope with the commands in "Two Backends, One Repo" above.
  *Resolved on StevesLenovo 7 July*: rag existed only as a stale local-scope entry, and even correctly re-registered at user scope it timed out — root cause was node loading `node_modules` over SMB/Tailscale (see the rag-on-Windows procedure in "Two Backends, One Repo" above). Fixed with a local copy at `C:\Tools\rag-mcp` — connected instantly. searxng was also only project-local on that machine and was moved to user scope. **SteveOP needs the same two fixes** (local rag copy + searxng scope check). Note for future debugging: a stdio MCP server started manually runs *silently* — that's normal, not a hang; and `claude mcp get <name>` only searches user scope plus the *current* directory's project, so run it from inside the relevant folder when hunting local-scope entries.

- [x] **context-mode upgrade (Amelai)** — ✅ done 7 July 2026 via `/ctx-upgrade`: npm and plugin both v1.0.169, all doctor checks pass (restart the session to load it).
- [ ] **context-mode upgrade (Windows — SteveOP, StevesLenovo)** — ✅ StevesLenovo done 7 July 2026 (v1.0.169 confirmed via the procedure below); SteveOP still to do. The old plugin's `/ctx-upgrade` self-upgrade is broken on Windows (`spawnSync npm ENOENT`: it spawns `npm` instead of `npm.cmd`), and `npm update -g` reports "up to date" without doing anything — known npm gotcha, and the npm global isn't the version `/ctx-doctor` reports anyway. Manual procedure in PowerShell:
  ```powershell
  npm install -g context-mode@latest              # updates the npm/MCP component (registry latest is 1.0.169)
  claude plugin update context-mode@context-mode  # updates the Claude Code plugin cache — this is the "active" version
  ```
  The full `plugin@marketplace` id is required — bare `claude plugin update context-mode` fails with "Plugin not found" on every machine (verified on Amelai 7 July 2026).
  then restart Claude Code.
  *Pass*: `/ctx-doctor` shows both "npm (MCP)" and "Claude Code" at v1.0.169.
  *If the plugin was never registered* (unlikely — both Windows machines already had marketplace + plugin at user scope): register it first with `claude plugin marketplace add mksglu/context-mode` then `claude plugin install context-mode@context-mode`.
  **A plugin update only takes effect after fully closing every Claude Code window** — `/ctx-doctor` in an already-open session is answered by the old in-memory version and will keep reporting v1.0.75.

- [ ] **Scaffolding live test** — ask the (post-Fable) model to run an existing risk assessment through `hseea/Risk_Assessment_QA_Checklist.md` and judge whether the output holds the quality bar; refine the checklist if not.
