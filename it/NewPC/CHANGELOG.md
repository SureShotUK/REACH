# Changelog — NewPC Project

---

## [Unreleased] - 2026-07-14

### Added
- **Two-stage page selection** for >42-page filings in the Customer Profiler: 8 new workflow nodes (page-count routing → 75-DPI labeled survey via qwen3-vl:32b → strict `PAGES:` parsing → full-DPI conversion of selected pages only)
- pdf-to-image: `POST /pageinfo` endpoint (page count without rendering); `?dpi=` (40–400), `?pages=` (selective conversion, max 20), `?label=1` (stamps "PAGE n OF m" banner above each page — fixes vision-model image-index miscounting); `fonts-dejavu-core` in the Docker image
- Parse Input: flattened multi-line paste handling (newline re-insertion before each `add <RegNo>`), per-line "missing ranking" errors, parse report (adds parsed / skipped lines) surfaced in the chat reply
- n8n API management: live workflow updates now pushed from the JSON file via `PUT /api/v1/workflows/:id` with node-by-node sync verification (API key created by user, not stored in repo)

### Changed
- Production PDF branch promoted to `qwen3-vl:32b` with page-sized `num_ctx` (bucketed, capped 65,536), Ollama timeout 120 s → 3,600 s (always-thinking model), converter timeout 30 s → 120 s, and the tester-validated column rule ("take the column with the later date")
- Company numbers canonicalised to 8-char zero-padded form in add/update/remove; lazy store-key migration re-keys legacy unpadded profile entries
- Survey call budgets sized from measured data (537 tokens/page at dpi 75; `num_predict` 6144 to cover the un-disableable thinking transcript)

### Fixed
- Customer Profiler returning no financials after model swap — missing `num_ctx` caused instant GPU OOM, swallowed by `continueOnFail` (the documented tester caveat, never ported)
- `Build & Save Profile` silently inventing `ranking:5` profiles when Companies House's canonical number didn't match user input (e.g. unpadded numbers) — now throws descriptively instead (user rule: fail over fabricate)
- Multi-entry pastes failing when the chat UI collapsed line breaks
- Stale duplicate "Portland Fuel - Customer Profiler" workflow (June 17-node version) deleted via API — ends the recurring wrong-workflow-executes hazard

### Documentation
- `n8n/PDF_Vision_Tester.md` updated for the qwen3-vl:32b promotion (models, prompt, closed caveats)
- `Software_Updates.md` pdf-to-image section: stale "100 DPI" prose corrected (actual default 200), new endpoints and tuning constants documented

---

## [Unreleased] - 2026-07-02 (2)

### Added
- `it/NewPC/.claude/settings.json` — new project-level permission allowlist; added `Bash(git check-ignore *)` after scanning ~30 recent session transcripts via the `fewer-permission-prompts` skill

### Documentation
- Confirmed `/end-session`'s own actions (file edits, `git add`/`commit`/`push`) were already fully pre-authorized; the permission prompts experienced this session traced back to the preceding Docker Compose migration work, not to `/end-session` itself

---

## [Unreleased] - 2026-07-02

### Added
- `it/NewPC/docker-compose.yml` — Compose definition for all 7 Docker services (Open WebUI, ComfyUI ×2, FileBrowser, SearXNG, n8n, pdf-to-image); existing named volumes/network marked `external: true`; GPU access via `deploy.resources.reservations.devices`
- `it/NewPC/DockerComposeDocs.md` — full Compose command reference: per-service start/recreate commands, migration steps, verification checks, day-to-day operations, and the secrets/interpolation gotcha writeup
- `it/NewPC/secrets/openwebui.env.example`, `it/NewPC/secrets/n8n.env.example` — templates for the two Compose secrets (real `.env` files gitignored)

### Changed
- `it/NewPC/Software_Updates.md` — every Docker-based service section (Open WebUI, SearXNG, n8n, ComfyUI ×2, FileBrowser, pdf-to-image) now leads with the current `docker compose pull/up` command; original `docker run` blocks retained in full but marked `OBSOLETE — kept for reference only`
- `it/NewPC/Docker.md` — added "Docker Compose (Primary Method)" section (setup, migration, day-to-day commands); "Service docker run Commands" section marked reference-only; documented the secrets/interpolation bug and percent-encoding fix
- `it/NewPC/CLAUDE.md` — "Docker Run Command Updates" rule rewritten to name `docker-compose.yml` as authoritative and record the secrets/interpolation gotcha
- `.gitignore` — added rules for `.env` and `it/NewPC/secrets/*.env` while keeping `*.env.example` tracked

### Fixed
- **n8n Customer Profiler `Loop` node wiring** — `done`/`loop` outputs were swapped (`CH: Company` was on `done`, `Get All Profiles` was on `loop`), causing every `add` command to trigger the `list` command's email with a stale/empty result instead of processing the company. Verified the correct output order against n8n's actual source (`SplitInBatchesV3.node.ts`: `outputNames: ['done', 'loop']`); user corrected the wiring directly in the n8n UI.
- **Compose secrets corruption** — a `$` in the Postgres password was silently blanked by Compose's variable interpolation, confirmed to affect per-service `env_file:` as well as a top-level `.env` (contrary to common assumption that `env_file:` is exempt). Fixed by percent-encoding the special character in the connection URL (`$` → `%24`); verified byte-for-byte via `sha256sum` comparison against the running container's actual environment variable. This briefly broke Open WebUI's database connection in production during the fix's first (incorrect) attempt.

---

## [Unreleased] - 2026-07-01

### Added
- `it/NewPC/n8n/Leadgen_Docs.md` — quick-reference for all three n8n workflows (Company Name Lookup, Customer Profiler, Lead Generation) with chat URLs, purposes, and doc links; includes "To Do — Customer-Benchmarked Lead Scoring" section with three-step pipeline design

### Changed
- `it/NewPC/n8n/Company_Profiler.md` — command syntax updated with optional `| <note>` field; syntax rules expanded; examples updated with realistic notes; What Gets Extracted table adds Region, Accounts Confidence, Ranking Note; account format detection explanation added
- `it/NewPC/n8n/Portland Fuel - Customer Profiler.json` — Parse Input node splits on `|` to extract ranking note; Build & Save Profile node adds region (from CH address), accountsConfidence (iXBRL=high/PDF=low), rankingNote; Get All Profiles node produces 9-column HTML table with ranking note sub-rows and full CSV; Send Profile List node replaced from Outlook node to Graph API HTTP Request with CSV attachment
- `it/NewPC/Software_Updates.md` — SearXNG section rewritten with full `docker run` command (was a placeholder); n8n section added with full command and encryption key retrieval step; pdf-to-image section restructured with quick-recreate path (image exists) as primary step, rebuild path as secondary

### Fixed
- n8n container had no port bindings — recreated with full `-p 127.0.0.1:15678:5678 -p 192.168.1.192:5678:5678` flags
- SearXNG container had no port bindings — recreated with `--network ai-network`, `127.0.0.1:18080`, and `192.168.1.192:8080`
- pdf-to-image container not exposing port 8086 (`ECONNREFUSED`) — recreated with `-p 127.0.0.1:18086:8086 -p 192.168.1.192:8086:8086`; Customer Profiler financial extraction restored

---

## [Unreleased] - 2026-06-30 (session 2)

### Changed
- `it/NewPC/SteveOP_MCP_Setup.md` — username corrected (`Steve` → `irwin`); UNC path fixed (`I:\terminai\` → `\\irwinnas\MyDocs\terminai\`); Step 5 rewritten from `.mcp.json` to `.claude.json` (correct Windows MCP config location)
- `it/NewPC/Temp.txt` — username corrected (`Steve` → `SteveIrwin`); UNC path fixed; Section A rewritten to reference `.claude.json`
- `it/NewPC/LocalModels.md` — installed models table sorted descending by model size
- `it/NewPC/SearXNG_Fix.md` — added Windows MCP config location note (`.claude.json` is the correct file, not `.mcp.json`)
- `it/NewPC/Local_CC.md` — Phase 4.1.2 corrected to reference user-scoped `.claude.json`
- `it/NewPC/LoadClientClaude.md` — Step 2 note added explaining `.claude.json` and manual server additions
- `it/NewPC/RAG_Setup.md` — new "Connecting from Windows Client Machines" section added with full MCP config block

### Fixed
- Windows MCP config location: previous documentation referenced `~\.claude\.mcp.json` — correct file is `%USERPROFILE%\.claude.json`; this caused RAG MCP to never appear in `/mcp` on SteveOP and StevesLenovo

---

## [Unreleased] - 2026-06-30

### Added
- `it/NewPC/SteveOP_MCP_Setup.md` — 9-step guide to configure Claude Code on SteveOP with TradingView MCP (chart analysis via CDP port 9222) and RAG MCP (Amelai knowledge base over Tailscale); includes PowerShell setup script, `.mcp.json` config, `/db` global skill install, and TradingView debug-mode launch instructions

### Changed
- `it/NewPC/CLAUDE.md` — RAG MCP server location updated from `/home/steve/rag-mcp/` to `/docs/terminai/rag-mcp/` (shared NAS); registration details added for both Amelai and Windows
- `/home/steve/.claude/.mcp.json` — `rag` args updated to NAS path; dead `tradingview` entry removed (TradingView desktop does not run on Linux)

### Removed
- `/home/steve/rag-mcp/` — standalone Amelai-local RAG MCP directory deleted; superseded by shared NAS copy at `/docs/terminai/rag-mcp/`

---

## [Unreleased] - 2026-06-22 (2)

### Fixed
- **CSV attachment never arrived (second attempt)** — n8n's Outlook node (typeVersion 2) silently drops attachments regardless of format (`additionalFields.attachments` is not a Graph API pass-through). Fixed by replacing "Send Results" Outlook node with an HTTP Request node calling `https://graph.microsoft.com/v1.0/me/sendMail` directly using `predefinedCredentialType` auth and a `JSON.stringify()` body with `#microsoft.graph.fileAttachment`

### Changed
- `it/NewPC/n8n/CompanyLookup_Workflow.json` — "Send Results" node: `n8n-nodes-base.microsoftOutlook` (typeVersion 2) → `n8n-nodes-base.httpRequest` (typeVersion 4.2); same credential, direct Graph API call

---

## [Unreleased] - 2026-06-22

### Added
- `it/NewPC/n8n/CompanyLookup_Workflow.json` — new 11-node Company Name Lookup workflow; accepts names one-per-line or comma-separated; CH search + Amelai confidence scoring + email (HTML table + CSV) + chat response
- `it/NewPC/n8n/Company_Lookup.md` — user documentation: production URL, activation steps, both input formats, confidence symbol key

### Changed
- `it/NewPC/CLAUDE.md` — added Amelai section (name, she/her pronouns, hostname lowercase rules, what she provides); updated prose references from `amelai` to `Amelai`
- `it/NewPC/n8n/Company_Profiler.md` — updated "amelai" → "Amelai" with she/her pronoun

### Fixed
- **Multiple companies silently dropped** — n8n Code nodes run once per batch (not once per item); `$input.first()` was discarding companies 2–N. Fixed with `$input.all()` + `$('Parse Names').all()[idx]` in `Prepare Prompt` and `Parse Scores`
- **CSV attachment never arrived** — `To Binary` binary approach produced attachment n8n Outlook node could not consume. Fixed by base64-encoding CSV in `Build Email + CSV` and using Microsoft Graph API `fileAttachment` format directly in Outlook node `additionalFields.attachments`; `To Binary` node removed

---

## [Unreleased] - 2026-06-19

### Added
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — `CH: Download iXBRL` HTTP node (GET S3 URL, `responseFormat: text`, `neverError: true`)
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — `Extract iXBRL Text` Code node: strips tags, builds dual targeted extracts (P&L + balance sheet), sends to qwen3.5:27b
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — `Detect Format` Code node: detects PDF vs iXBRL from S3 URL before download
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — `Route: Format` IF node: routes to `CH: Download PDF` (binary) or `CH: Download iXBRL` (text)
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — `Route2` IF node + `Remove Profile` Code node: `remove <regNo>` command
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — profit after tax as fifth financial metric in both extraction paths
- `it/NewPC/n8n/Centrebus.html` — iXBRL sample filing (Centrebus Ltd, 03872099) for analysis reference
- `it/NewPC/n8n/pdf-to-image/` — new microservice: FastAPI + poppler PDF-to-PNG converter for Ollama vision model

### Changed
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — financial value parser handles parentheses negatives e.g. `(45,000)` → `-45000`
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — `Extract iXBRL Text` uses dual targeted extracts (P&L anchored on "turnover", balance sheet anchored on "net assets") instead of single contiguous window; fixes net assets missing when on different page
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — `think: false` added to qwen3.5:27b Ollama body; prevents thinking-mode timeout
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — raw HTML truncation increased to 500,000 chars to cover large iXBRL filings
- `it/NewPC/n8n/pdf-to-image/app.py` — DPI restored to 200
- `it/NewPC/Software_Updates.md` — DPI value updated in pdf-to-image configuration table

### Fixed
- `Extract iXBRL Text` had no `parameters`/`jsCode` after node insertion bug displaced the code block into `CH: Download iXBRL` — both nodes corrected
- `$helpers.getBinaryDataBuffer` throws `ReferenceError: $helpers is not defined` in n8n v2.15.1 Code nodes (task runner context) — redesigned to detect format from S3 URL before download; no binary reading in Code nodes
- iXBRL content garbled (`~)^▒+-zo▒`) — `binaryMode: separate` stores binary as internal reference ID; fixed by reading response as text (`responseFormat: text`) instead of binary

---

## [Unreleased] - 2026-06-18

### Added
- `it/NewPC/n8n/LeadGen_Workflow.json` — importable n8n workflow: 17-node lead generation pipeline (Companies House → officers → filing → PDF → SearXNG → Ollama qwen3.6:27b → HTML email)
- Two input modes: SIC code + geography search, and single company registration number lookup
- `continueOnFail` handling on PDF download and extraction — graceful no-accounts path
- 600,000ms Ollama timeout — covers worst-case 25-company sequential queue

### Changed
- `it/NewPC/n8n/LeadGen_Workflow_Design.md` — added Usage Guide section (prerequisites, input modes, examples table, score colour guide, VRAM note, run time table); fixed all `qwen3.5:35b` → `qwen3.6:27b`; moved PDF from Phase 2 → Phase 1 implemented; updated phase checklist to reflect completion

### Documentation
- Usage Guide covers: how to open chat, SIC search syntax with 8 examples, single company lookup with 3 examples, email output format, score colour coding, VRAM note, typical run times

---

## [Unreleased] - 2026-06-10 (2)

### Added
- `androidApp` dark theme toggle — `SwitchMaterial` in Settings; applies immediately via `AppCompatDelegate`; persists via `KEY_DARK_MODE` preference
- `androidApp` GPS location awareness — `ACCESS_COARSE_LOCATION` permission; `getLastKnownLocation()` + `Geocoder` reverse geocoding; location injected into system prompt as "User's current location: X"
- `AndroidManifest.xml` — `ACCESS_COARSE_LOCATION` permission

### Fixed
- SearXNG not accessible via Tailscale — Docker container was bound to `100.79.83.113:8080` (Tailscale IP); corrected to `127.0.0.1:18080` (loopback for Tailscale serve) + `192.168.1.192:8080` (LAN)
- Tailscale serve rule must use `--bg` flag to persist after command exits
- Tailscale ACL was missing port 8080 allow rule — phone showed connection error until added in admin console

### Documentation
- `Docker.md` — SearXNG run command corrected with dual port bindings
- `CLAUDE.md` — Tailscale ACL requirement documented: every Tailscale serve port also needs an ACL allow rule

---

## [Unreleased] - 2026-06-10 (STT fixes)

### Added
- `stt/STT_Documentation.md` — comprehensive documentation for server and client: architecture diagram, prerequisites, installation, VRAM lifecycle, configuration reference, updating guide (including plain-English venv explanation), troubleshooting
- `stt/stt_server.py` — `_idle_monitor()` background task: unloads Whisper model from VRAM after 15 minutes with no transcription
- `stt/stt_server.py` — `_get_model()` / `_unload_model()` lazy-load pattern with `asyncio.Lock`

### Changed
- `stt/stt_server.py` — model no longer loaded at startup; lazy-loads on first transcription request via `run_in_executor`
- `stt/stt_client.py` — `_inject()` made async; `time.sleep` replaced with `await asyncio.sleep(0.15)`

### Fixed
- `stt/stt_client.py` — Ctrl/Esc keypresses dropped during paste: replaced `keyboard.send("ctrl+v")` with Windows `SendInput` via `ctypes`, eliminating keyboard hook re-entrancy
- `stt/stt_client.py` — Spurious F9 toggling: added 500 ms debounce with `threading.Lock` in `_toggle()`
- `stt/stt_client.py` — Double F9 handler after re-register: added `global _hotkey_handle` to `main()` so `_reregister_hotkey()` can correctly remove the original handler

---

## [Unreleased] - 2026-06-10

### Added
- `AI_Voice_App.md` — comprehensive documentation: architecture, build guide, update workflow, Open WebUI config, settings reference, speech corrections, troubleshooting, VRAM management
- `androidApp` SearXNG web search — app queries SearXNG before each AI call and injects top 3 results as context into the system message
- `androidApp` speech corrections map (`SPEECH_CORRECTIONS`) — post-processes transcribed text to fix misrecognised proper nouns; initial entry: Weatherby → Wetherby
- `androidApp` SearXNG URL setting — configurable in settings screen, default `https://amelai.tail926601.ts.net:8080`

### Changed
- `androidApp` system prompt — instructs model to be concise, plain text only (no asterisks/emojis/backslashes), confirms web search capability, includes today's date
- `androidApp` default model — changed from `qwen3.5:35b` to `qwen3.6:27b`
- `androidApp` settings labels — increased from `textAppearanceLabelLarge` (~14sp) to 18sp bold

### Fixed
- `Error: HTTP 401 Unauthorized` — API key invalidated by Open WebUI update; fix: regenerate key in Open WebUI Settings → Account → API Keys

---

## [Unreleased] - 2026-06-09

### Added
- `androidApp/gradle.properties` — missing config file; required `android.useAndroidX=true` and `android.enableJetifier=true` for successful build

### Fixed
- Android Studio build error: "Configuration `:app:debugRuntimeClasspath` contains AndroidX dependencies but `android.useAndroidX` not enabled" — fixed by creating `gradle.properties`
- AI Voice app API timeout: URL in settings must include full path `/api/chat/completions`
- Raw `<function_calls>` XML appearing in app responses: disabled all Open WebUI Built-in Tools except Web Search

### Changed
- Open WebUI model settings: Built-in Tools — all disabled except Web Search
- Open WebUI Admin Panel: Web Search enabled, provider set to SearXNG at `http://192.168.1.192:8080`

---

## [Unreleased] - 2026-05-14

### Added
- `stt/stt_server.py` — WebSocket STT server: faster-whisper large-v3 on CUDA GPU 1, RMS energy VAD, auto-reconnect, initial_prompt for proper nouns
- `stt/stt_client.py` — Windows STT client: pystray system tray icon, F9 global hotkey, clipboard paste, reconnect loop
- `stt/stt_server.service` — systemd unit for STT server (GPU 1, auto-restart)
- `stt/requirements_server.txt` / `stt/requirements_client.txt` — dependency lists
- `stt/start_stt.bat` / `stt/start_stt.vbs` — legacy launchers (superseded by Task Scheduler)
- `STT_Voice_Input.md` — full setup and troubleshooting guide
- `STT_New_Machine_Guide.md` — clean installation guide for additional Windows machines

### Changed
- `CLAUDE.md` — STT service added to port/service table (port 9090, GPU 1, systemd)

### Fixed
- torchaudio CUDA 13 incompatibility: replaced silero-vad with RMS energy threshold VAD — no torchaudio dependency
- Windows auto-start keyboard hook failure: VBS hidden window (`style 0`) breaks `keyboard` global hooks; Task Scheduler `RunLevel Highest` is the correct solution

---

## [Unreleased] - 2026-05-04 (2)

### Added
- `CLAUDE.md` — "Docker Run Command Updates" rule: always update both service-specific file and `Docker.md` in same session
- `Temp.txt` — ReActor NSFW face swap workflow JSON (two Load Image nodes → ReActorFaceSwap → SaveImage)

### Changed
- `ComfyUI.md` — Steve's container: `--reserve-vram 3` added to CLI_ARGS; workflows volume updated to `/docs/Projects/Claude Code Shared/Workflows`; Tailscale-only access note added
- `Docker.md` — Steve's ComfyUI run command: `--reserve-vram 3` added; workflows volume corrected; options table updated; access line corrected to Tailscale-only

### Fixed
- ComfyUI every-other-image OOM (`TextEncodeQwenImageEditPlus` allocation failure) — `--reserve-vram 3` had been lost from running container; restoring it prevents VRAM exhaustion between generations
- ReActorFaceSwap workflow widget_values order for v0.6.2: `swap_model` and `facedetection` must precede face index values

---

## [Unreleased] - 2026-05-04

### Added
- `Docker.md` — `runlike` section: how to reconstruct `docker run` commands from running containers (`pip install runlike` / `runlike <name>`)
- `Docker.md` — FileBrowser now mounts `/opt/comfyui/workflows:/srv/comfyui-workflows`

### Changed
- `Docker.md` — Steve's ComfyUI output volume changed from `/opt/comfyui/output` to `/docs/Projects/Claude Code Shared/Output` (NAS mount); fixes FileBrowser delete permissions
- `Docker.md` — FileBrowser output volume updated to match new NAS path
- `Docker.md` — Steve's ComfyUI loopback port corrected from `8189` to `18189` (fixes Tailscale HTTPS access)
- `Docker.md` — Steve's ComfyUI LAN binding (`192.168.1.192:8189`) removed; now Tailscale-only
- `Docker.md` — port map table corrected for Steve's ComfyUI (loopback 8189 → 18189)

### Fixed
- Tailscale access to Steve's ComfyUI was broken — Tailscale Serve forwarded to `localhost:18189` but container was bound to `127.0.0.1:8189`; corrected loopback port in run command

---

## [Unreleased] - 2026-04-19

### Added
- `wol/WOL_Setup.md` — complete Alexa Wake-on-LAN setup guide; covers wol-webhook service, Tailscale Funnel, AWS Lambda, Alexa skill configuration and troubleshooting
- wol-webhook Python systemd service on amelai (`/opt/wol-webhook/`) — receives HTTP POST and sends WOL magic packet
- Tailscale Funnel on port 8443 exposing wol-webhook publicly for Lambda access
- AWS Lambda function `SteveOP_WOL_Skill` (eu-west-2) — Alexa skill backend with PIN verification
- Custom Alexa skill (ID: `amzn1.ask.skill.d1357b39-a05e-490a-a5d0-3c702eaee152`) — invocation "wol machine"; submitted for certification

### Changed
- `wol-webhook` uses `wakeonlan -i 192.168.1.255` (subnet broadcast) rather than default 255.255.255.255

---

## [Unreleased] - 2026-04-12

### Added
- `N8N_Setup.md` — n8n workflow automation Docker setup guide; covers install, Tailscale configuration, update procedure, backup, and troubleshooting

### Changed
- `CLAUDE.md` — port assignment table and access URL table updated to include n8n (container `5678`, loopback `15678`, LAN/Tailscale `5678`)

---

## [Unreleased] - 2026-04-08

### Fixed
- Boot failure caused by `pci=nomsi` in `GRUB_CMDLINE_LINUX_DEFAULT` — this parameter disables MSI for all PCIe devices including NVMe SSDs, causing NVMe controllers to fail and dropping the system to initramfs; recovered by editing GRUB at boot menu; `GRUB_CMDLINE_LINUX_DEFAULT` restored to empty

### Documentation
- `Linux_Troubleshooting.md` — Issue 5 Performance Impact section rewritten to describe correct idle/load PCIe link behaviour (Gen 1 at idle via ASPM is normal, Gen 4 under load confirmed)
- `Linux_Troubleshooting.md` — WARNING section added: `pci=nomsi` must not be added to GRUB; documents symptoms and recovery procedure
- `Linux_Troubleshooting.md` — Status line, reference table, and verification checklist updated to reflect idle/load behaviour

---

## [Unreleased] - 2026-04-07

### Fixed
- RTX 3090 PCIe Gen 1 fallback — both GPUs now running at Gen 4 (16GT/s); root cause was `pcie_aspm=off` kernel parameter blocking PCIe link equalization during link training; removed from `GRUB_CMDLINE_LINUX_DEFAULT`; BIOS CPU PCIE ASPM Mode Control restored to Auto

### Documentation
- `Linux_Troubleshooting.md` — Issue 5 fully updated: status changed to RESOLVED, root cause explained, fix procedure documented, troubleshooting log completed with final step result, verification checklist updated
- `ASUS_PCIe_Support_Case.md` — created during investigation (no longer needed for submission as issue resolved)

---

## [Unreleased] - 2026-04-06 (2)

### Fixed
- Intel igc I226-V NIC recurring PCIe crash — `pcie_aspm=off` confirmed insufficient; permanent fix applied: primary connection moved to Aquantia AQC113 10GbE NIC (`ethernet2_5g`, 192.168.1.192), igc driver blacklisted via `/etc/modprobe.d/blacklist-igc.conf`
- `netplan apply` failing on `netplan-wpa-wlp11s0.service` — removed broken WiFi section (no password configured) from `/etc/netplan/*.yaml`
- System timezone incorrect (was UTC) — set to `Europe/London` via `timedatectl`

### Documentation
- `Linux_Troubleshooting.md` — Issue 2 rewritten: two incidents documented, `pcie_aspm=off` failure noted, permanent Aquantia switchover documented with full netplan config
- `Linux_Troubleshooting.md` — Issue 4 added: 90-second boot delay caused by WiFi adapter `wlp11s0`; fix applied (WiFi removed from netplan); verification pending on next reboot

---

## [Unreleased] - 2026-04-06

### Added
- NFS mount for Synology DS920+ `MyDocs` share at `/docs` — entry added to `/etc/fstab` (`192.168.1.216:/volume2/MyDocs /docs nfs defaults,_netdev,nofail 0 0`)

### Removed
- `cifs-utils` and `smbclient` — installed during SMB troubleshooting, no longer needed

### Documentation
- `CLAUDE.md` — added "Linux Session Housekeeping" section: remove temp packages, run updates, clean up credentials/temp files at end of each Linux session

---

## [Unreleased] - 2026-04-05 (2)

### Fixed
- ComfyUI (Steve) OOM error on first generation with Qwen-Rapid-AIO-NSFW-v23 — added `--reserve-vram 3` to `CLI_ARGS`; reserves 3GB VRAM headroom to prevent fragmentation-induced allocation failures
- ComfyUI (Steve) Tailscale access broken after container rebuild — loopback port was `127.0.0.1:8189` (wrong) corrected to `127.0.0.1:18189` in both `ComfyUI.md` and active container

### Added
- `.gitignore` — excludes `.NET bin/obj`, `*.log`, `*.oft`, `*.bin`, `*_files/`, `it/troubleshooting/Backups_*/`, and named temp files
- FileBrowser now mounts ComfyUI input and output folders: `comfyui-input/`, `comfyui-output/`, `comfyui-amelia-input/`, `comfyui-amelia-output/`
- 139 previously untracked files committed: Canada regulatory docs, REACH HVO, IUCLID, hseea subdirs, insurance, ZeroTrust, postgres-security, wsl-postgresql-setup, IT security docs, NewPC workflows/configs, OutlookTemplateCleaner source

### Documentation
- `ComfyUI.md` — corrected docker run command (loopback port + `--reserve-vram 3`)
- `Docker.md` — FileBrowser command updated with all four ComfyUI volume mounts
- `CLAUDE.md` (shared) — added "Warnings Before Instructions" principle: warnings must appear before commands, not after

---

## [Unreleased] - 2026-04-05

### Added
- `.gitattributes` — repo-root line ending normalisation: LF stored in git, CRLF on Windows checkout, LF on Linux; binary files exempt
- `.claude/commands/sync-files.md` — `/sync-files` slash command for bidirectional cross-platform git sync; handles ahead/behind/diverged cases; uses rebase so local changes win on conflict

### Documentation
- `CLAUDE.md` updated with: FLUX.1 dev fp8 self-contained model structure, VRAM/Ollama contention reference table, Docker dual-port binding strategy (`1XXXX` loopback + LAN IP pattern), bash special character password handling, `Temp.txt` file delivery pattern

---

## [Unreleased] - 2026-03-24 (2)

### Added
- `FileWriter.py` — Open WebUI Tool class; gives models genuine filesystem write capability to `/mnt/uploads` (host: `/home/steve/rag-output`); includes path traversal protection

### Documentation
- Confirmed `/mnt/uploads` bind mount already present in Open WebUI container from prior setup — maps to `/home/steve/rag-output`
- Documented symlink behaviour: `it/.claude/commands/end-session.md` is a symlink; actual file is at `terminai/.claude/commands/end-session.md`

---

## [Unreleased] - 2026-03-24

### Added
- `Linux_Troubleshooting.md` — server reliability reference guide covering:
  - Post-crash log analysis using journalctl (boot-scoped, OOM, kernel errors, NVIDIA XID codes)
  - Issue 2: Intel igc NIC PCIe ASPM fix — `pcie_aspm=off` kernel boot parameter
  - Issue 3: Ollama OOM kills — ComfyUI VRAM contention diagnosis and fix (bookmarklet + cron)
  - VRAM budget reference table for amelai models

### Fixed
- Intel igc NIC (`ethernet10g`) dropping off PCIe bus — `pcie_aspm=off` added to `GRUB_CMDLINE_LINUX_DEFAULT`; PCIe ASPM disabled system-wide on kernel 6.17
- Ollama OOM kill loop — ComfyUI VRAM free bookmarklet created; nightly 2am cron restarts added for both ComfyUI containers

### Documentation
- Documented that `igc` module has no `aspm_disable` parameter on kernel 6.17 — GRUB parameter is the only reliable fix
- Documented ComfyUI `/free` endpoint behaviour (empty response body — do not use `.json()`, use `.ok` status check)

---

## [Unreleased] - 2026-03-23

### Added
- `SearXNG_Fix.md` — MCP web search troubleshooting log covering two root causes (missing Tailscale ACL port, stored auth credential), architecture overview, and quick reference commands

### Fixed
- Tailscale ACL updated to include port 3001 — MCP server now accessible from all tailnet devices
- Stored Anthropic auth credential cleared (`claude auth logout`) — Ollama routing now works correctly
- Open WebUI Ollama URL corrected from `https://` to `http://100.79.83.113:11434`
- `hf-env` conda environment no longer auto-activates on SSH login to amelai
- MCP re-registered as user-scoped — available in all Claude Code projects, not just NewPC

---

## [Unreleased] - 2026-03-22

### Added
- `New_PC_Builds.md` — personal Windows 11 PC build guide covering:
  - Existing components (MSI MAG X870E Tomahawk WIFI, Samsung 9100 Pro 2TB, Viper Venom DDR5 32GB)
  - Chosen build: Ryzen 7 9800X3D + RTX 5070 Ti 16GB + Corsair 4000D Airflow + be quiet! Power Zone 2 1000W + Arctic Liquid Freezer III 360 (~£1,395 total)
  - Full component research with alternatives and rejected options documented
  - Video editing via AI PC remote encoding section (DaVinci Resolve render queue, Adobe Media Encoder, Tailscale)
  - Compatibility summary and pricing notes

---

## [Unreleased] - 2026-03-19 (Late Evening)

### Changed
- `QwenImageEditTrainingLoRA.md` — added **RAM Limitation Workarounds** section after Overview explaining the 64 GB system-specific memory fixes (swap, pin_memory, TORCH_CUDA_ARCH_LIST, dataset_num_workers); updated training time estimate to reflect 6–8 hours with workarounds

---

## [Unreleased] - 2026-03-19 (Evening)

### Added
- `LoRAMemoryFixes.md` — complete guide to the memory issues blocking Qwen-Image-Edit LoRA training:
  - Root cause analysis (ZeRO-3 checkpoint save memory doubling: ~41 GB → ~87 GB)
  - All required fixes: correct venv, 32 GB swap, `pin_memory: false`, `TORCH_CUDA_ARCH_LIST="8.6"`, `--dataset_num_workers 0`
  - Confirmed working Stage 2 script
  - Speed optimisation roadmap (restore `pin_memory: true`, restore `--dataset_num_workers 2`)
  - Diagnostics reference with OOM interpretation table

### Changed
- Server `ds_z3_cpuoffload.json` — `pin_memory` set to `false` for both `offload_optimizer` and `offload_param`
- Server `stage2_train.sh` — added `TORCH_CUDA_ARCH_LIST="8.6"`, changed `--dataset_num_workers 2` to `0`
- Server swap increased from 8 GB to 32 GB (`/swap.img`)

### Fixed
- Qwen-Image-Edit LoRA training OOM on checkpoint save — training now completes and writes `epoch-N.safetensors`

---

## [Unreleased] - 2026-03-19

### Added
- `TMUX.md` — tmux reference guide: what tmux is, sessions/windows/panes concepts, prefix key, all common commands with explanations, practical workflows (long jobs, split monitoring), detach/attach pattern, quick reference card
- `Docker.md` — Docker administration guide: core concepts (image/container/volume/network), all common commands with when-to-use guidance, full `docker run` commands for all five services (Open WebUI, ComfyUI Steve, ComfyUI Amelia, FileBrowser, SearXNG), port map, Tailscale Serve rebuild commands, SSH file access explanation for bind-mounted volumes

### Fixed
- Stage 2 LoRA training restarted after tmux session loss — `stage2_train.sh` intact, Stage 1 cache intact, training resumed in new `lora-training` tmux session

---

## [Unreleased] - 2026-03-18 (Evening)

### Added
- `QwenImageEditTrainingLoRA.md` — comprehensive standalone guide for Qwen-Image-Edit-2511 character LoRA training:
  - Training image guidelines (count, resolution, angle variety, exclusion rules)
  - metadata.json creation script with correct `{"prompt": ..., "image": ...}` format
  - Accelerate ZeRO-3 config file (`accelerate_zero3_2gpu.yaml`) creation instructions
  - Stage 1 and Stage 2 scripts with all verified parameters
  - Per-run variable table (`MY_LORA_NAME`, `MY_DATASET_PATH`, `MY_TRIGGER_WORD`, `MY_LORA_RANK`)
  - Resource monitoring commands and expected VRAM/RAM usage
  - Output file locations and epoch selection guidance
  - Full troubleshooting section covering all errors encountered during development

### Changed
- `Model_and_LoRA_Creation.md` — `--max_pixels` reduced from 1048576 → 786432 → 524288 → 262144 (final working value); `--lora_rank` parameter table note updated

### Fixed
- ComfyUI process management: corrected from `sudo pkill -f "ComfyUI/main.py"` to `docker stop comfyui comfyui-amelia` — instances ARE Docker containers, not plain Python processes
- Training approach: replaced non-functional FP8+DDP method with DeepSpeed ZeRO-3 CPU offload (`--config_file accelerate_zero3_2gpu.yaml`, `num_processes: 1`)
- `--use_gradient_checkpointing` restored for Stage 2 — required to prevent activation tensors filling VRAM; was incorrectly removed based on outdated incompatibility warning

### Documentation
- Documented that FP8+DDP fundamentally cannot work on 2×24 GB: transformer fills ~23.2 GB per GPU regardless of quantisation, leaving no room for overhead
- Documented ZeRO-3 CPU offload mechanics: 41 GB transformer offloaded to CPU RAM, GPU holds only active layer parameters (~13–15 GB peak VRAM)
- Documented that `num_processes: 1` uses less VRAM than `num_processes: 2` under ZeRO-3 CPU offload (no AllGather communication buffers)
- Documented Stage 1 / Stage 2 `--max_pixels` coupling: cached latent dimensions are fixed at Stage 1 generation time; must delete cache and re-run Stage 1 when changing resolution
- Documented DeepSpeed ZeRO-3 merge date (March 17, 2026) — explains why previous session's approach predated this fix

---

## [Unreleased] - 2026-03-18

### Added
- `MultiFileModels.md` — standalone reference document explaining:
  - HuggingFace diffusers multi-file model format vs single-file safetensors
  - Qwen-Image-Edit-2511 file structure (transformer shards + text encoder + VAE)
  - Three options for using multi-file models in ComfyUI
  - DiffSynth-Studio model loading via `model_id_with_origin_paths`
  - `hf` CLI setup (venv creation, install, activation) from scratch
- `Model_and_LoRA_Creation.md` — **Workflow 3**: Qwen-Image-Edit character LoRA training:
  - Step 1: Dataset preparation with Python metadata.json generation script
  - Step 2: DiffSynth-Studio install + ModelScope model download
  - Step 3: Dependency verification (torchaudio fix, CUDA toolkit)
  - Step 4: Training script with multi-GPU accelerate config
  - Step 5: LoRA installation in ComfyUI

### Changed
- `Model_and_LoRA_Creation.md` Step 4 training command updated:
  - Added `--num_processes 2 --mixed_precision bf16` to `accelerate launch` — required to distribute model across both GPUs
  - Added `export PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True`
  - Added `--initialize_model_on_cpu` — prevents VRAM spike on GPU 0 during model load
  - Reduced `--max_pixels` from 1763584 to 1048576 (1024×1024)
  - Reduced `--lora_rank` from 32 to 16
  - Reduced `--dataset_num_workers` from 8 to 2
  - Added pre-flight GPU check section (stop ComfyUI and Ollama before training)
  - Expanded parameters table with explanations for all new flags
  - Updated download size estimate from ~58GB to ~20GB (4×5GB files via ModelScope)

### Documentation
- Documented why `--num_processes 2` is critical: without it accelerate uses single GPU, causing OOM on the 20B transformer
- Documented `--mixed_precision bf16` effect: causes DiffSynth-Studio to load fp8/quantised variant (~10GB per GPU vs ~25GB)
- Documented ComfyUI process management before training (incorrectly documented as plain Python processes — corrected to Docker containers in 2026-03-18 evening session)
- Documented `hf` vs `huggingface-cli` rename in huggingface_hub 1.x
- Documented DiffSynth-Studio metadata.json requirement (not .txt caption files)
- Documented ModelScope cache location (`~/.cache/modelscope/`)

---

## [Unreleased] - 2026-07-02 (2)

### Changed
- `/docs/terminai/.claude/settings.local.json` — added wildcarded Edit/Write permissions for `SESSION_LOG.md`, `PROJECT_STATUS.md`, `CHANGELOG.md` across all projects (`/docs/terminai/**/<filename>`), so `/end-session` no longer prompts for authorisation on its own tracking-file edits in any project
- `it/NewPC/.claude/settings.json` — reverted NewPC-only literal-path Edit permissions added in error the previous session (superseded by the root-level wildcard fix)

---

## [Unreleased] - 2026-07-02

### Added
- `update` chat command in the Customer Profiler workflow (`n8n/Main/NewCustomerProfiler.json`) — edits specific fields on an existing profile (Ranking, Products, Region, Company Name, SIC Codes, Turnover, Employees, Net Assets, Accounts Year, Confidence, Ranking Note, Profile Date) without re-running the Companies House/financials lookup
- `Route3`, `Update Profile`, `Format Update Email`, `Send Update Email` nodes in the Customer Profiler workflow

### Changed
- `n8n/Company_Profiler.md` — documented the `update` command: syntax, field table, merge-vs-overwrite behaviour, examples, and email notification states

### Documentation
- Recorded that `Products` and `SIC Codes` fields merge (case-insensitive dedupe) while all other fields overwrite outright on `update`
- Noted `update` is only available once `n8n/Main/NewCustomerProfiler.json` is imported and activated — not present in `Portland Fuel - Customer Profiler.json`

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
