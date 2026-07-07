<img src="../../Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# context-mode — Context Window Protection for Claude Code

*Last updated: 7 July 2026. This is the canonical context-mode document — `Software_Updates.md` points here.*

---

## Overview

**context-mode** is a Claude Code plugin (MCP server + hooks) that protects the context window — the fixed amount of conversation "memory" the model has per session. Every tool call Claude makes normally dumps its raw output into that window: a single web page fetch can cost 50KB+, a long log file even more. After enough of that, the session runs out of room, the conversation gets compacted (older messages dropped), and the model forgets what it was doing.

context-mode solves this three ways:

1. **Sandbox execution** — instead of reading raw data into the conversation, Claude runs code in an isolated subprocess and only the printed result enters the context. A 45KB log scan comes back as a 3-line answer.
2. **Knowledge base** — large outputs and fetched web pages are indexed into a local search database instead of being held in the conversation. Claude retrieves only the relevant snippets when needed.
3. **Session continuity** — file edits, git operations, errors, and decisions are tracked in a local database. After a compaction or a `--continue` restart, the working state is rebuilt automatically instead of being lost.

**Why we use it**: sessions last hours instead of ~30 minutes, less re-explaining after compaction, and (on the local Ollama backend especially) far less wasted processing on raw data. The project's published benchmarks show a typical session's 315KB of raw tool output reduced to 5.4KB.

Project home: <a href="https://github.com/mksglu/context-mode" target="_blank">context-mode on GitHub (mksglu/context-mode)</a>

---

## How It Works

### The sandbox

Each `ctx_execute` call spawns an isolated subprocess. The code runs there, raw data stays there, and **only what the script prints (stdout) enters the conversation**. Twelve language runtimes are supported (JavaScript, TypeScript, Python, Shell, Ruby, Go, Rust, PHP, Perl, R, Elixir, C#). Authenticated CLIs (`gh`, `docker`, `kubectl`, etc.) work inside the sandbox via credential passthrough.

When output exceeds ~5KB and an intent is given, the full output is indexed into the knowledge base and only matching sections are returned.

### The knowledge base

Content is chunked and stored in a **SQLite FTS5** table (FTS5 = SQLite's built-in full-text search engine). Searches are ranked with **BM25** — a relevance algorithm that scores how well each chunk matches the query terms — combined with substring matching, typo correction, and proximity boosts. In practice: Claude indexes a big document once, then asks it questions instead of re-reading it.

Indexed content lives per-project under `~/.context-mode/` with a 24-hour cache for fetched URLs and automatic 14-day cleanup of old databases.

### The tools

| Tool | What it does |
|---|---|
| `ctx_execute` | Run code in the sandbox; only stdout enters context |
| `ctx_batch_execute` | Run multiple commands + searches in one call |
| `ctx_execute_file` | Process a file in the sandbox without reading it into context |
| `ctx_index` | Index a local file/directory into the knowledge base |
| `ctx_search` | Search everything indexed (captures + session memory) |
| `ctx_fetch_and_index` | Fetch a URL, index it, search it — raw page never enters context |
| `ctx_stats` | Show context savings for the session |
| `ctx_doctor` | Diagnose the installation (runtimes, hooks, FTS5, versions) |
| `ctx_upgrade` | Update to the latest version from GitHub |
| `ctx_purge` | **Destructive** — permanently delete all indexed content |

Claude uses these automatically (a session-start hook injects routing rules). You can also invoke them yourself — see Maintenance Commands below.

---

## Dependencies

| Dependency | Requirement | Notes |
|---|---|---|
| Claude Code | ≥ 1.0.33 | Plugin marketplace support |
| Node.js | **≥ 22.5** | Hard requirement — see SIGSEGV note below |
| SQLite backend | automatic | Selected at runtime: `bun:sqlite` (if Bun installed) → `node:sqlite` (Node ≥22.5) → `better-sqlite3` (fallback) |
| Bun | optional | Accelerator, not a dependency — see next section |

**Why Node ≥ 22.5 matters**: versions of context-mode below v1.0.162 running on older Node used the `better-sqlite3` native module, which hit a Linux kernel/V8 bug (`madvise MADV_DONTNEED` SIGSEGV) crashing the MCP server 1–4 times per hour. Fixed June 2026 on Amelai by upgrading Node v20 → v22 via nvm and context-mode to ≥ v1.0.162. Node ≥ 22.5 also provides the built-in `node:sqlite`, removing the fragile native module from the chain entirely.

---

## Installation Status on Our Machines

*As verified 7 July 2026:*

| Machine | context-mode | Node.js | Bun | Action needed |
|---|---|---|---|---|
| Amelai | v1.0.169 | v22.22.3 | 1.3.14 ✓ | None — fully set up |
| SteveOP | updated 2026-07-07 | (check `node -v` ≥ 22.5) | **not installed** | Install Bun (steps below) |
| StevesLenovo | (verify with `/ctx-doctor`) | (check `node -v` ≥ 22.5) | (check `bun --version`) | Verify, install Bun if absent |

---

## What Is Bun and Should You Install It?

**Bun** is a JavaScript runtime — an alternative to Node.js — written in the Zig language around Apple's JavaScriptCore engine instead of Node's V8. Its headline traits are much faster startup and execution for scripts, and a built-in SQLite driver (`bun:sqlite`). It installs entirely in your user profile, does not replace Node.js, and the two coexist without conflict. Home page: <a href="https://bun.sh" target="_blank">bun.sh</a>.

**Recommendation: yes, install it on the Windows machines.** context-mode auto-detects Bun and uses it for two things:

1. **Sandbox speed** — JS/TS code run via `ctx_execute` executes 3–5x faster (this is the message SteveOP displayed after the update)
2. **SQLite backend** — with Bun present, the knowledge base uses `bun:sqlite`, the most robust backend option (no native-module compilation, and it sidesteps the `better-sqlite3` class of problems that caused the 2026-06 crashes)

The cost is a ~100MB user-level install with no admin rights required. There is no configuration — context-mode simply detects it on the next session. Amelai already has it (1.3.14).

---

## Installing Bun

### Amelai (Linux) — already installed

Nothing to do. Recorded for rebuild reference:

```bash
curl -fsSL https://bun.sh/install | bash
```

Installs to `~/.bun/bin/bun` and adds itself to PATH via `~/.bashrc`. Verify with `bun --version`.

### Windows (SteveOP / StevesLenovo)

1. Open a **PowerShell** window (no admin needed) and run:

```powershell
powershell -c "irm bun.sh/install.ps1 | iex"
```

2. Bun installs to `%USERPROFILE%\.bun` and adds itself to the user PATH.

3. **Close and reopen the terminal** (the PATH change only applies to new windows), then verify:

```powershell
bun --version
```

4. **Restart Claude Code** (close all sessions, start fresh — the MCP server detects Bun at startup).

5. Verify detection: run `/ctx-doctor` in a Claude Code session — the runtimes section should now list Bun, and the `ctx_execute` tool description will show "Bun detected".

Full installation options (winget, scoop, npm): <a href="https://bun.com/docs/installation" target="_blank">Bun installation documentation</a>.

---

## Setup — Fresh Install of context-mode

For a new machine (or after a wipe), inside a Claude Code session:

```
/plugin marketplace add mksglu/context-mode
/plugin install context-mode@context-mode
```

Then restart Claude Code (or run `/reload-plugins`) and verify with `/ctx-doctor` — all items should show PASS.

This writes two entries to the user settings (`~/.claude/settings.json` on Linux, `C:\Users\<User>\.claude\settings.json` on Windows):

```json
"enabledPlugins": { "context-mode@context-mode": true },
"extraKnownMarketplaces": {
  "context-mode": { "source": { "source": "github", "repo": "mksglu/context-mode" } }
}
```

> Amelai additionally has a `SessionStart` hook in `~/.claude/settings.json` running `~/.claude/hooks/context-mode-cache-heal.mjs` — added during the June 2026 crash troubleshooting to repair the plugin cache at session start. Leave it in place.

---

## Updating

Updates are released frequently, and an outdated version can crash the MCP server silently mid-session — update whenever a new version is flagged.

**In a Claude Code session (preferred):**

```
/ctx-upgrade
```

Claude fetches the upgrade command and runs it. The upgrade pulls the latest release from GitHub, installs it into the plugin cache, rebuilds native components, reconfigures hooks, and runs a self-test.

**From the terminal (no session needed):**

```bash
context-mode upgrade
```

(`context-mode` is npm-linked globally to the plugin cache. If the link is broken, the direct form works: `node $(ls -d ~/.claude/plugins/cache/context-mode/context-mode/*/cli.bundle.mjs | tail -1) upgrade`)

**After any update: restart Claude Code.** The files are replaced in place, but the running MCP server process keeps the old version until a new session starts.

**Verify:**

```
/ctx-doctor
```

Key items: npm (MCP) and Claude Code version numbers match the latest, **FTS5/SQLite: PASS**, Node.js ≥ v22.5, and (with Bun installed) Bun listed in runtimes.

---

## Maintenance Commands

| Task | In a Claude Code session | From the terminal |
|---|---|---|
| Check health/versions | `/ctx-doctor` | `context-mode doctor` |
| Update | `/ctx-upgrade` | `context-mode upgrade` |
| Show context savings | `/ctx-stats` | — |
| Search the knowledge base | `/ctx-search` | `context-mode search "<query>"` |
| Index a file/folder | `/ctx-index` | `context-mode index <path>` |
| Wipe the knowledge base | `/ctx-purge` | — |

**Housekeeping notes:**

- Indexed content lives under `~/.context-mode/` (per-project SQLite databases). It self-manages: URL cache expires after 24 hours, and databases older than 14 days are removed automatically at startup — no manual cleanup needed.
- ⚠️ **`/ctx-purge` is irreversible** — it permanently deletes all indexed content and session memory. Only use it to deliberately start fresh.
- Old plugin versions accumulate in `~/.claude/plugins/cache/context-mode/context-mode/` (Amelai currently has 1.0.162 alongside the live 1.0.169). Superseded version directories can be deleted after confirming `/ctx-doctor` passes on the current one.

---

## Troubleshooting

| Symptom | Cause / Fix |
|---|---|
| MCP server crashes silently mid-session; `ctx_*` tools stop responding | Outdated context-mode version — run `/ctx-upgrade`, restart Claude Code |
| Repeated SIGSEGV crashes (historic, Linux) | Node < 22.5 + `better-sqlite3` kernel/V8 bug — upgrade Node via `nvm install 22 && nvm alias default 22`, then `/ctx-upgrade` (needs context-mode ≥ v1.0.162) |
| `/ctx-doctor` shows FTS5/SQLite FAIL | SQLite backend broken — with Node ≥22.5 or Bun installed this should not occur; re-run `/ctx-upgrade` to rebuild |
| "Installing Bun would give 3-5x faster ctx_execute" message | Informational, not an error — Bun is absent; install per the steps above if wanted |
| Bun installed but not detected | Claude Code was not restarted after install, or the terminal PATH predates the install — open a fresh terminal, start a new session, re-run `/ctx-doctor` |
| Version stuck after upgrade | The running MCP process is the old one — fully restart Claude Code |

---

## Sources

- <a href="https://github.com/mksglu/context-mode" target="_blank">context-mode — GitHub repository and README</a> (tools, sandbox and knowledge-base internals, benchmarks, install instructions)
- <a href="https://bun.sh" target="_blank">Bun — official site</a>
- <a href="https://bun.com/docs/installation" target="_blank">Bun — installation documentation</a> (all install methods, Windows requirements)

*All links verified 7 July 2026. Machine-specific version details verified live on Amelai the same day.*
