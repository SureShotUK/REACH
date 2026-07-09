# "Don't Ask Again" Fix — Permission Prompts That Keep Coming Back

**Date identified:** 2026-07-09 (previously hit in an earlier session — see commit `9c40588`, "user-scope // path fix")

## The Symptom

You approve a tool with **"Yes, and don't ask again for … commands in \\irwinnas\MyDocs\terminai\it"**, but the same permission prompt appears again — over and over — in the same session and in new sessions. Observed with:

- `searxng - web_search` (Amelai research agents)
- `plugin:context-mode:context-mode - ctx_fetch_and_index`

## The Cause

"Don't ask again" **does** work as designed: it writes an allow rule into the project's
`.claude\settings.local.json` (you can see the saved rules in
`\\irwinnas\MyDocs\terminai\it\.claude\settings.local.json`).

The problem is **where** it saves, not **whether** it saves. This project lives on a UNC
network share (`\\irwinnas\...`), and Claude Code keys project-scoped settings by project
path. On UNC paths the path gets normalised inconsistently (`\\irwinnas\...` vs
`//irwinnas/...`), so the rule is written under one form of the path but looked up under
the other. The rule exists on disk but is never matched — so the prompt returns every time.

Project scope (`.claude\settings.json`) and local scope (`.claude\settings.local.json`)
are **both** affected, because both are keyed by the project path.

## The Solution

Put the permission rules in **user scope** instead:

```
C:\Users\SteveIrwin\.claude\settings.json
```

User-scope settings apply to every project regardless of path, so the UNC path bug cannot
defeat them. Subagents inherit these rules too, so research agents spawned inside a
session are also covered.

### Example rules (currently applied)

```json
{
  "permissions": {
    "allow": [
      "mcp__searxng",
      "mcp__searxng__web_search",
      "mcp__plugin_context-mode_context-mode__ctx_fetch_and_index"
    ]
  }
}
```

Merge the `permissions` block into the existing file — do not replace the whole file,
which also holds hooks, plugin, and theme settings.

### Rule syntax reference

| Rule | Effect |
|------|--------|
| `mcp__searxng` | Pre-approves **every** tool on the searxng MCP server (web_search, list_models, read_memory, save_session, update_memory, workspace_commit) |
| `mcp__searxng__web_search` | Pre-approves only the web_search tool |
| `mcp__plugin_context-mode_context-mode__ctx_fetch_and_index` | Pre-approves the context-mode plugin's ctx_fetch_and_index tool |

MCP tool rules follow the pattern `mcp__<server>__<tool>`; omit `__<tool>` to allow the
whole server. Plugin-provided MCP servers use the server name
`plugin_<plugin-name>_<server-name>`, hence the double-barrelled
`mcp__plugin_context-mode_context-mode__...` form.

## Important Notes

- **Restart running sessions** after editing user settings — a session loads its
  settings at startup and will keep prompting until restarted.
- Approvals clicked mid-session on this share still won't persist (same bug), so don't
  rely on "don't ask again" for anything under `\\irwinnas\MyDocs\terminai` — add the
  rule to `C:\Users\SteveIrwin\.claude\settings.json` directly.
- Validate the JSON after editing (a malformed settings file silently disables *all*
  settings in it):

  ```powershell
  Get-Content "C:\Users\SteveIrwin\.claude\settings.json" -Raw | ConvertFrom-Json
  ```
