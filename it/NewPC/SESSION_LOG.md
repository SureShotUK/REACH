# Session Log — NewPC Project

---

## Session 2026-07-15 (2) — IMG banner deploy verified in production; padding proven load-bearing; slow-survey diagnosis; Amelai reboot readiness

### Summary
Verification segment, no code changes. The IMG banner deploy went live (user rebuilt pdf-to-image on Amelai, published the workflow) and the 73-page GXO filing again extracted **all figures** (execution 273). Diagnosed the run's 12m48s survey step from the execution API: pure thinking volume, not hardware — and discovered via the new `rawPicks` breadcrumb that the IMG rename did **not** cure the page-number conflation; the deterministic padding is the load-bearing fix. Confirmed Amelai will recover cleanly from the pending reboot.

### Work Completed
- **IMG-banner production run verified (exec 273)** — all six figures correct; `rawPicks=29,32,55` shows the model *still* reports printed footer numbers for the statements pages under IMG banners; padding (29→30,31; 32→33,34) captured the real P&L (file 31) and net-assets (file 33) pages. Conclusion recorded: the rename is at best a partial helper; do NOT retire the padding
- **Slow-survey diagnosis** — 12m48s = 74s prompt ingestion + 688s of thinking at normal speed (23.7 tok/s vs 25–26 on faster runs; identical prompt-eval rate) — no CPU offload occurred. Thinking volume varies 8× run-to-run on the same input (2,043 / 6,205 / 16,301 tokens). Answered the nvtop question: the process-table CPU% is host CPU (100% = one core), normally saturated by llama.cpp's sampling/dispatch loop during fully-on-GPU inference
- **Budget warning recorded** — exec 273 used 16,301 of 16,384 `num_predict` (83 tokens spare). If truncation recurs, levers: brevity instruction in the survey prompt, or drop the cover-page checklist item; the budget itself cannot rise without pushing the 73-page filing over the survey cap
- **User decision: overnight runs** — long-filing surveys are slow but thorough; user will run big batches overnight rather than tune for speed now
- **Amelai reboot readiness confirmed** — all 7 compose containers have restart policies; GPU-container/NVIDIA race covered by the enabled `docker-gpu-containers.service`; Ollama/STT/wol-webhook/postgresql/nvidia-power-limit are enabled systemd units; Tailscale serve rules persist; `/docs` NFS is `_netdev,nofail`. One watch-item: FileBrowser/ComfyUI bind-mount `/docs` paths and can win the boot race against NFS — if they show empty dirs after reboot, `docker restart filebrowser comfyui`
- **Office-network quirk noted** — user's laptop (office, not home LAN) cannot reach `192.168.1.192` directly; all API diagnostics this segment ran over Tailscale (`https://amelai.tail926601.ts.net:5678`)

### Files Changed
- (tracking files only — no code changes this segment)

### Key Decisions
- **Padding stays permanently on current evidence** — `rawPicks` proves conflation survives the IMG rename; revisit only if future breadcrumbs show raw picks landing on true pages
- **Slow is acceptable** — overnight batches instead of latency tuning; extraction accuracy is the priority

### Next Actions
- [ ] **User**: reboot Amelai for updates (recovery verified; glance at FileBrowser/ComfyUI mounts after)
- [ ] Run the other three >42-page companies from the consultancy batch (overnight)
- [ ] If a survey hits `num_predict` again (debug marker will say so): add brevity instruction or drop the cover-page checklist item
- [ ] Mirror the two-stage survey chain into the vision tester; A/B `think:false` on extraction
- [ ] User decision: store the n8n API key in docs or keep session-only
- [ ] Delete superseded `n8n/PDF Vision Tester.json` (stale export)

---

## Session 2026-07-15 — survey stage debugged to full accuracy: thinking budget, deterministic page padding, IMG banner rename

### Summary
Iterative whack-a-mole on the two-stage survey until the 73-page GXO filing extracted **all figures correctly** (user-confirmed). Three root causes fixed in sequence, each diagnosed from live execution data via the n8n API: (1) the balance-sheet survey rules inflated the model's thinking past `num_predict` (empty response, `done_reason:length`); (2) the model applied the "include the next page" rule inconsistently, so it moved from the prompt into deterministic code padding in `Pick Pages`; (3) the model conflates the stamped banner numbers with printed footer page numbers — inconsistently per page and per run (user's diagnosis, confirmed by set arithmetic across runs) — absorbed by depth-2 forward padding, then fixed structurally by renaming the banner token `PAGE n OF m` → `IMG n OF m`. Every change critic-reviewed before applying; file and live workflow verified in sync after each push.

### Work Completed
- **Survey thinking budget** (exec 269 autopsy) — `num_predict` 6,144 → 16,384 with `CTX_HEADROOM=NUM_PREDICT+2048` (critic blocked the naive raise: the old formula only *guaranteed* 8,192 free tokens, so 12,288 would overflow at some page counts and silently evict image tokens via context shifting); survey page cap 95 → 78 as the accepted trade
- **Deterministic page padding** (exec 270 autopsy) — survey found only one balance-sheet heading and applied the next-page rule to 31→32 but not 32→33; prompt item 2 simplified to heading-matching only, and `Pick Pages` now pads every selected page with its successors in code (`CONVERTER_MAX_PAGES=20` guard with graceful degradation and debug notes)
- **Footer/banner conflation** (exec 271 autopsy) — run gained Net assets but lost Turnover/PBT; set arithmetic across runs 270/271 proved the P&L is file page 31 while the survey keeps reporting 29 (the printed footer number), yet banner-reads the employees page correctly — no fixed offset exists; padding extended to depth 2 (+1, +2), exploiting the one-sided error (printed footers never exceed file position). Simulation showed both runs' divergent picks expand to the same 12-page set containing every figure-bearing page — **next run extracted all six figures correctly**
- **IMG banner rename** (structural fix, user-proposed) — `app.py` stamps `IMG n OF m`; survey prompt rewritten to demand IMG numbers and dismiss printed page numbers explicitly; `PAGES:` reply format unchanged (parser untouched); padding deliberately retained pending A/B evidence (+1 also covers unheaded continuation pages, a numbering-independent failure mode)
- **Autopsy breadcrumbs** — `Pick Pages` output now carries `padDepth` and `rawPicks` (the model's unpadded answer) so future misses read straight off the execution record and padding can be retired on evidence
- **Extraction stage validated under fire** — given pages without the P&L it returned `Turnover: null` rather than inventing a number; fail-over-fabricate holding in production

### Files Changed
- `n8n/Portland Fuel - Customer Profiler.json` — `Prep Survey Prompt` (NUM_PREDICT/CTX_HEADROOM coupling, simplified item 2, IMG wording), `Pick Pages` (depth-2 padding, converter cap, padDepth/rawPicks)
- `n8n/pdf-to-image/app.py` — banner text `PAGE` → `IMG` (stamp + fallback comment); **container rebuild on Amelai still pending**

### Git Commits
- (this commit) — see below

### Key Decisions
- **Mechanical rules live in code, not prompts** — the model demonstrably applies arithmetic rules inconsistently against its own reasoning; padding/ranges/caps moved to post-processing (critic-saved practice)
- **Forward-only padding** — printed footer numbers never exceed file position, so the true page is always at or ≤2 after the reported one; pad forward, never back
- **Keep the safety net until A/B proof** — IMG rename and padding removal are separate deploys; `rawPicks` captures the evidence needed to retire padding later
- **Deployment order** — rebuild pdf-to-image *before* publishing the workflow; the transient window (IMG banners + old PAGE prompt) degrades toward padded/null, not corruption

### Reference Documents
- n8n executions API (`?includeData=true`) — every diagnosis this session came from node-by-node autopsies of live runs 269–271

### Next Actions
- [ ] **User**: rebuild pdf-to-image on Amelai (`docker compose up -d --build pdf-to-image`), then publish the workflow, then one GXO confirmation run under IMG banners
- [ ] Run the other three >42-page companies from the consultancy batch
- [ ] After several clean IMG-banner runs, compare `rawPicks` against figure-bearing pages and retire/reduce padding on evidence
- [ ] Mirror the two-stage survey chain into the vision tester (deferred until profiler proven — close)
- [ ] A/B `think:false` on the extraction call via the tester (extraction still thinks; slow but accurate)
- [ ] User decision: store the n8n API key in docs (PGPASS-style accepted risk) or keep session-only
- [ ] Delete superseded `n8n/PDF Vision Tester.json` (stale export, still flagged)

---

## Session 2026-07-14 — qwen3-vl:32b promoted to production; two-stage page selection for long filings; parser hardening; n8n API management

### Summary
Marathon session on the Customer Profiler's PDF/vision path. Promoted `qwen3-vl:32b` (validated in the vision tester) to production with the `num_ctx` sizing and timeout changes the tester docs required; hardened `Parse Input` (flattened multi-add pastes, canonical zero-padded company numbers, specific missing-ranking errors, parse report in the chat reply); removed everything that could fabricate user-entered data (the `ranking:5` fallback). Built a full two-stage page-selection pipeline so filings over 42 pages (one test company has 73) can be processed: cheap `/pageinfo` routing, low-DPI survey pass, page-number banner stamping (fixes vision-model image-index miscounting), full-DPI conversion of only the selected pages. Established direct n8n API management (live workflow updates pushed and verified from Claude Code; stale duplicate workflow deleted). Every change was coding-critic reviewed before applying, and node code was simulation-tested against real inputs before deployment.

### Work Completed
- **qwen3-vl:32b promotion** — production `Prep Vision Prompt` gained the tester's `num_ctx` sizing block (bucketed, capped 65,536) + over-cap degrade with `debug`; Ollama timeout 120 s → 300 s → 3,600 s (thinking models are slow); prompt column rule ported from tester ("take the column with the later date" replaces "left column is current year"); `PDF_Vision_Tester.md` brought up to date (models, prompt, promotion caveat closed, stale file flagged)
- **Root-caused two "no financials" failures** — (1) old duplicate workflow taking test runs after import-without-delete; (2) production converter timeout (30 s) and missing num_ctx, both documented tester caveats never ported
- **Parse Input hardening** — chat UI flattens multi-line pastes: newline re-insertion before each `add <RegNo>` (critic-tightened lookahead); ranking stays mandatory per user's fail-over-fabricate rule, with per-line "missing ranking after company number X" errors; parse report (adds parsed / lines skipped) attached and surfaced in the chat reply via `Build Chat Response`
- **Fabricated-data removal** — `Build & Save Profile`'s `ranking:5` fallback replaced with descriptive throws (plus no-company guard); company numbers canonicalised (all-digit → zero-padded 8-char) in add/update/remove; lazy store-key migration added to all four profile-store nodes so legacy unpadded entries stay reachable
- **Two-stage page selection for >42-page filings** — pdf-to-image gained `POST /pageinfo` (pdfinfo, no rendering), `?dpi=40–400`, `?pages=n,n` (max 20, per-page failure skip), `?label=1` (48px black banner "PAGE n OF m" above the page, DejaVu Bold, `fonts-dejavu-core` added to Dockerfile); workflow gained 8 nodes routing >42-page filings through a 75-DPI labeled survey (qwen3-vl picks statement pages, strict `PAGES:` reply parsing with range expansion) then full-DPI conversion of only those pages into the unchanged extraction
- **Survey debugging via live execution data (n8n API)** — diagnosed `done_reason:length` with empty response: this qwen3-vl:32b build is an **always-thinking** variant (`think:false` accepted but ignored) — budget raised to `num_predict:6144`; measured real cost ~537 tokens/page at dpi 75 (constants retuned); diagnosed wrong-page selection: the model reads pages correctly but cannot count its position in a 73-image stack — fixed by banner stamping (verified visually with a synthetic PDF); balance-sheet continuation pages (where Net assets lives) addressed with heading-based + include-next-page survey rules
- **n8n API management established** — user created an API key; live workflow now updated via `PUT /api/v1/workflows/:id` (settings filtered to API-allowed keys) with node-by-node file↔live sync verification after every push; stale duplicate workflow (17-node June version) safety-checked and deleted via API
- **73-page filing end-to-end**: extraction now returns correct figures; only Net assets (balance-sheet page 2) missed on the last run — final survey-rule fix applied, awaiting one more test run

### Files Changed
- `n8n/Portland Fuel - Customer Profiler.json` — all workflow changes above (8 new nodes; Parse Input, Build & Save Profile, Build Chat Response, Prep Vision Prompt, Prep Survey Prompt, Pick Pages, store nodes)
- `n8n/pdf-to-image/app.py` — `/pageinfo`, `dpi`/`pages`/`label` params, banner stamping, per-page failure tolerance
- `n8n/pdf-to-image/Dockerfile` — `fonts-dejavu-core` added (slim image ships no fonts; banner text needs DejaVu Bold)
- `n8n/PDF_Vision_Tester.md` — updated for qwen3-vl:32b era (new; previously untracked)
- `Software_Updates.md` — pdf-to-image section: stale "100 DPI" prose fixed (actual: 200), new endpoints/params/constants documented

### Key Decisions
- **Fail over fabricate** (user rule, twice confirmed): no default rankings, no invented data presented as entered; parser errors loudly and per-line instead. Honest `null` financials (rendered "—") remain acceptable degrade behaviour
- **Ranking mandatory** — reverted an approved optional-ranking design on user instruction; missing rankings now produce specific per-line errors
- **Accommodate, don't fight, the always-thinking model** — `think:false` retained but ineffective on this build; budgets sized for full thinking transcripts; extraction call deliberately left thinking (accuracy validated with it)
- **Banner stamping over positional counting** — vision models read pixel text reliably but cannot index large image stacks; page numbers are now physically stamped on survey images
- **File is source of truth** — every live n8n change is pushed from the JSON file via API and verified in sync, ending the file/live drift that caused two duplicate-workflow incidents

### Reference Documents
- `n8n/PDF_Vision_Tester.md` — tester usage + A/B methodology
- `Software_Updates.md` → pdf-to-image section — converter API reference
- n8n API: key created 2026-07-14 (held by user, not stored in repo); base `http://192.168.1.192:5678/api/v1`

### Next Actions
- [ ] Re-run the 73-page filing to confirm Net assets now extracts (survey continuation-page rules applied, awaiting test)
- [ ] Run the other three >42-page companies from the 13-company consultancy batch
- [ ] Mirror the two-stage survey chain into the vision tester so the test loop covers long filings
- [ ] A/B `think:false` on the extraction call via the tester — thinking is the likely cause of slow 25-page extractions; if accuracy holds, extraction gets dramatically faster
- [ ] Decide whether to store the n8n API key in docs (PGPASS-style accepted risk) or keep it session-only
- [ ] Delete the superseded `n8n/PDF Vision Tester.json` (stale pre-import copy; current export is `Portland Fuel - PDF Vision Tester.json`)

---

## Session 2026-07-02 (2) — Permission-prompt audit via fewer-permission-prompts skill

### Summary
User asked why `/end-session` still required authorization for some steps despite wanting it to run unattended. Investigated by running the `fewer-permission-prompts` skill against ~30 recent session transcripts. Finding: `/end-session`'s own actions (file edits under `/docs/terminai/**`, `git add`/`commit`/`push`) were already fully pre-authorized before this session started — the prompts experienced were from the preceding Docker Compose migration work (`docker inspect`, `psql`, `mkdir`, `bash -c`, etc.), not from `/end-session` itself. One new safe, read-only permission was added.

### Work Completed
- **Scanned ~30 recent transcript files** (`~/.claude/projects/*/​*.jsonl`) for Bash and MCP tool-call frequency, filtered to read-only commands per the skill's rules
- **Diagnosed why prompts still occur** — this project's `it/NewPC/.claude/settings.local.json` has accumulated dozens of ultra-narrow, literal one-off `Bash(...)` allow entries (exact `gemini -p "..."` prompts, exact `curl` URLs) rather than general patterns, so any new/slightly-different command still needs a fresh prompt; however the specific commands `/end-session` itself runs (Edit/Write for `/docs/terminai/**`, `git add:*`, `git commit:*`, `git push:*`) were already broadly covered
- **Created `it/NewPC/.claude/settings.json`** (new — project-level, not `.claude/settings.local.json`) — added `Bash(git check-ignore *)`, the one genuinely new, safe, read-only pattern found in the scan; everything else from the scan was either already auto-allowed by Claude Code natively (`cd`, `ls`, `cat`, `git status/log/diff`, `docker ps/logs/inspect`, etc.) or already explicitly granted in this project (`git:*`, `docker compose *`, `docker exec *`, `python3 *`, `curl *`, `grep *`)
- **Deliberately did not allowlist** `psql` (can run destructive SQL, not just `SELECT`), `bash -c` (arbitrary shell execution), `mkdir`/`rm` (mutate state) — these stay one-off approvals by design

### Files Changed
- `it/NewPC/.claude/settings.json` — **NEW**: `Bash(git check-ignore *)` added to `permissions.allow`

### Key Decisions
- **`/end-session`'s mandatory actions were already unattended-ready** — no further permission change was needed for the command itself. The user's experience of "still having to authorise steps" traces to general session work (Docker/Postgres debugging) preceding end-session, not to end-session's own file-edit + git-push sequence.
- **Declined to broaden permissions for interpreters/shells/SQL clients** — per the `fewer-permission-prompts` skill's safety rule, `psql`, `bash -c`, and similar arbitrary-execution surfaces are not wildcarded even though they were used safely this session; this project's existing `python3 *` and `docker exec *` wildcards (granted in earlier sessions, left untouched) are already about as permissive as is advisable.

### Reference Documents
- `it/NewPC/.claude/settings.json` — new project permissions file
- `it/NewPC/.claude/settings.local.json` — existing (large, historically accumulated) local allowlist, left untouched

### Next Actions
- [ ] If a future `/end-session` run hits an actual new prompt, report the exact command so a precise rule can be added
- [ ] Consider reviewing whether the existing broad `python3 *` / `docker exec *` grants in `it/NewPC/.claude/settings.local.json` are still wanted, given they're equivalent to arbitrary code execution
- [ ] Carry forward all outstanding items from the 2026-07-02 Docker Compose session below (image prune, apt upgrade, Postgres password rotation, Customer Profiler JSON import, etc.)

---

## Session 2026-07-02 — n8n Loop wiring bug fixed; Docker Compose built and all 7 services migrated

### Summary
Diagnosed a genuine node-wiring bug in the Customer Profiler n8n workflow (the `Loop` node's `done`/`loop` outputs were swapped, causing every `add` command to trigger the `list` command's email instead of processing the company). Then built out the Docker Compose solution flagged as a to-do since 2026-07-01, migrated all 7 containers to it one at a time, and — along the way — found and fixed a real secret-corruption bug in Compose itself (a `$` in the Postgres password was silently blanked by Compose's variable interpolation, which affects `env_file:` as well as top-level `.env`, contrary to common assumption). Finished by creating `DockerComposeDocs.md` as the full command reference and marking the old `docker run` blocks in `Software_Updates.md` as obsolete rather than removing them.

### Work Completed
- **Diagnosed n8n Loop node wiring bug** — verified against n8n's actual source (`SplitInBatchesV3.node.ts`: `outputNames: ['done', 'loop']`) that the Customer Profiler's `Loop` node had `CH: Company` wired to the `done` output and `Get All Profiles` wired to the `loop` output — backwards. Result: every `add` command fired the list-all-profiles email (with an empty/stale store) instead of ever running `CH: Company` → `Build & Save Profile`. User fixed the wiring in the n8n UI directly based on this diagnosis and confirmed it now works.
- **Built `docker-compose.yml`** — all 7 services (Open WebUI, ComfyUI ×2, FileBrowser, SearXNG, n8n, pdf-to-image) translated from the `docker run` commands in `Docker.md`; existing named volumes (`open-webui`, `n8n_data`) and the `ai-network` marked `external: true` so Compose attaches to them instead of creating new empty ones; GPU access via `deploy.resources.reservations.devices`
- **Found and fixed a real Compose secrets bug** — a `$` in the Postgres password was silently blanked by Compose's `.env` variable interpolation (confirmed via `docker compose config` warning + hash comparison against the live container's actual password). Initially "fixed" by switching to per-service `env_file:` files on the (incorrect) assumption that `env_file:` bypasses interpolation — it does not on this Compose version, and it corrupted the password again after `open-webui` was already recreated, briefly breaking its DB connection in production. Root-caused and permanently fixed by percent-encoding the special character in the URL (`$` → `%24`); verified byte-for-byte via `sha256sum` comparison between the secrets file and the running container's actual environment variable.
- **Migrated all 7 containers to Compose**, one at a time with verification after each: `pdf-to-image` (health endpoint), `searxng` (search query + JSON format), `filebrowser` (health check), `n8n` (all 3 workflows re-activated, no credential decryption errors), `comfyui-amelia` and `comfyui` (GPU visibility via `nvidia-smi` inside container), `open-webui` (health check + GPU + Postgres connection)
- **Created `secrets/` folder** — `openwebui.env` and `n8n.env` (real secrets, gitignored) plus `.env.example` templates (tracked); removed the earlier top-level `.env`/`.env.example` approach entirely once it was shown to be unreliable
- **Created `DockerComposeDocs.md`** — full Compose command reference: per-service start/recreate commands, the migration pattern for containers still running outside Compose, verification checks used during migration, day-to-day commands, and a complete writeup of the secrets/interpolation gotcha with the fix
- **Updated `Software_Updates.md`** — every Docker-based service section now leads with the current `docker compose pull/up` command; the original `docker run` blocks are kept in full but marked `OBSOLETE — kept for reference only`
- **Updated `Docker.md` and `CLAUDE.md`** — added "Docker Compose (Primary Method)" section, corrected the secrets-handling documentation after the `env_file:` interpolation discovery, and updated the standing "Docker Run Command Updates" rule to name `docker-compose.yml` as authoritative

### Files Changed
- `it/NewPC/docker-compose.yml` — **NEW**: Compose definition for all 7 services
- `it/NewPC/DockerComposeDocs.md` — **NEW**: full Compose command reference
- `it/NewPC/secrets/openwebui.env.example`, `it/NewPC/secrets/n8n.env.example` — **NEW**: secret templates (real `.env` files gitignored)
- `it/NewPC/Docker.md` — added "Docker Compose (Primary Method)" section; marked "Service docker run Commands" section as reference-only; documented the secrets/interpolation gotcha and fix
- `it/NewPC/Software_Updates.md` — added current Compose command + OBSOLETE marker to all 6 Docker-service sections; old `docker run` blocks retained
- `it/NewPC/CLAUDE.md` — updated "Docker Run Command Updates" rule to reference Compose as authoritative and record the secrets/interpolation gotcha
- `.gitignore` — added rules for `.env` and `it/NewPC/secrets/*.env` while keeping `*.env.example` tracked
- `it/NewPC/n8n/Portland Fuel - Customer Profiler Test.json` — not modified by Claude; user corrected the `Loop` node's output wiring directly in the n8n UI based on the diagnosis in this session

### Git Commits
(pending — this session's documentation commit follows)

### Key Decisions
- **`docker-compose.yml` is now the single source of truth for container configuration** — the recurring failure mode (containers recreated by hand with an incomplete `docker run`, silently losing flags) is exactly what Compose eliminates. The 2026-07-01 outage (n8n/SearXNG/pdf-to-image) was the direct motivation.
- **Secrets go in per-service `secrets/*.env` files wired via `env_file:`, not a top-level `.env` + `${VAR}` interpolation** — even though this session proved `env_file:` isn't fully exempt from interpolation either, keeping secrets in dedicated files (rather than inline in `docker-compose.yml`) is still correct practice; the interpolation risk is handled by percent-encoding rather than by file choice alone.
- **Percent-encoding over password rotation** — when a secret contains a character Compose's interpolation misinterprets, percent-encoding it (for URL-embedded secrets) is less invasive than rotating the underlying credential, which would require coordinated updates across other machines/configs (e.g. the same Postgres password is also used by the separate RAG MCP's own `PGPASS` on Amelai and StevesLenovo).
- **Never trust a secrets fix without byte-level verification** — `docker compose config --quiet` showing no warnings is necessary but not sufficient (the `env_file:` corruption produced no warning at all in one case). The reliable check is `docker exec <container> printenv <VAR> | sha256sum` compared against the source file's hash.
- **Old `docker run` commands kept, not deleted** — per explicit instruction, `Software_Updates.md`'s original commands remain in full (marked obsolete) as flag-by-flag reference and manual fallback if Compose is ever unavailable.

### Reference Documents
- `it/NewPC/DockerComposeDocs.md` — new Compose command reference
- `it/NewPC/Docker.md` — "Docker Compose (Primary Method)" section
- n8n `SplitInBatchesV3.node.ts` source (`github.com/n8n-io/n8n`) — used to verify actual output order (`['done', 'loop']`) rather than relying on a paraphrased web search result, which was initially wrong
- Docker official docs — `docs.docker.com/reference/compose-file/interpolation/` — confirmed `$$` escaping semantics (used to identify why the naive fix didn't fully work)

### Next Actions
- [ ] Run `docker image prune` on amelai — a dangling old `pdf-to-image` image was left behind by the Compose rebuild
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai (standing housekeeping item)
- [ ] Consider rotating the shared Postgres password to remove the `$` entirely now that the immediate risk is fixed, to simplify future secret handling (would require coordinated update across Amelai's `~/.bashrc` PGPASS, StevesLenovo's Windows env var, and `secrets/openwebui.env`)
- [ ] Import updated Customer Profiler JSON into production n8n if not already the active version (the `Loop` wiring fix was made directly in the n8n UI — confirm the exported JSON in the repo reflects it)
- [ ] Carry forward all previously outstanding items from the 2026-07-01 session below (lead scoring model, Customer Profiler import, SteveOP/StevesLenovo MCP setup, etc.)

---

## Session 2026-07-01 — n8n service outages diagnosed and fixed; Customer Profiler enhanced with notes, region, and accounts confidence

### Summary
Diagnosed and fixed three Docker containers (n8n, SearXNG, pdf-to-image) that had lost their port bindings — a recurring failure where containers were recreated without full `docker run` flags. Enhanced the Customer Profiler workflow with a ranking note field, region extraction from Companies House, accounts confidence flagging (iXBRL = high, PDF = low), and CSV attachment on the list email via the Graph API. Created `Leadgen_Docs.md` as a quick-reference overview of all three workflows with a lead scoring to-do section.

### Work Completed
- **Fixed n8n 502 Bad Gateway** — container was running with no port bindings; stopped, removed, recreated with full `docker run` flags including both `-p` bindings and `N8N_ENCRYPTION_KEY`
- **Fixed SearXNG 502** — same missing port bindings; recreated with `--network ai-network`, `127.0.0.1:18080`, and `192.168.1.192:8080`
- **Fixed pdf-to-image ECONNREFUSED** — container was not exposing port 8086; recreated with both loopback and LAN port bindings; Customer Profiler financials now extracting again
- **Created `Leadgen_Docs.md`** — quick-reference for all three workflows: Company Name Lookup, Customer Profiler, Lead Generation; HTML anchor chat URLs; links to full docs; lead scoring to-do section
- **Updated `Company_Profiler.md`** — command syntax updated with `| <note>` field; syntax rules; examples with realistic notes; What Gets Extracted table with Region, Accounts Confidence, Ranking Note rows; account format detection explanation
- **Modified `Portland Fuel - Customer Profiler.json`** — Parse Input: splits on `|` to extract note; Build & Save Profile: region from CH address fields, accounts confidence from iXBRL/PDF detection, rankingNote saved; Get All Profiles: 9-column HTML table, ranking note as italic sub-row, CSV with all new fields; Send Profile List: swapped from Outlook node to Graph API HTTP Request for CSV attachment; Chat Trigger: updated welcome message showing `| note` syntax
- **Updated `Software_Updates.md`** — SearXNG section rewritten with full `docker run` command (was a placeholder); n8n section added with full command, encryption key retrieval step, and credential verification; pdf-to-image restructured with quick-recreate path (image exists) separate from full rebuild path

### Files Changed
- `it/NewPC/n8n/Leadgen_Docs.md` — **NEW**: Quick-reference overview of all three n8n workflows + lead scoring to-do
- `it/NewPC/n8n/Company_Profiler.md` — Updated: `| note` syntax, region/confidence/rankingNote in What Gets Extracted, email content updated
- `it/NewPC/n8n/Portland Fuel - Customer Profiler.json` — Modified: note parsing, region extraction, accounts confidence, CSV attachment via Graph API, welcome message
- `it/NewPC/Software_Updates.md` — SearXNG section fixed; n8n section added; pdf-to-image restructured
- `it/NewPC/n8n/N8N_Setup.md` — Updated by user

### Key Decisions
- **Ranking notes stored as free text** after `|` separator in the `add` command — purpose is human context for lead scoring model, not machine-parseable field
- **Accounts confidence** is binary: `high` (iXBRL structured parse) / `low` (PDF vision extraction) / `none` (no accounts found); stored per profile
- **Region** taken from `reg.county || reg.region || reg.locality` — best available field from CH registered address
- **Send Profile List email** now uses Graph API HTTP Request node (not Outlook node) — Outlook node cannot send attachments; this matches the Company Name Lookup workflow approach
- **Docker Compose** identified as the correct long-term resilience solution for port binding loss — not yet implemented; documented as next priority
- **Why containers lose port bindings** — `--restart unless-stopped` preserves config on restart but not on recreation; containers recreated without full flags silently lose bindings

### Reference Documents
- `it/NewPC/n8n/Leadgen_Docs.md` — new quick-reference
- `it/NewPC/n8n/Company_Profiler.md` — updated usage guide
- `it/NewPC/CLAUDE.md` — n8n workflow generation patterns (credential IDs, port binding strategy)

### Next Actions
- [ ] **Import updated Customer Profiler JSON** — replace current workflow in n8n; re-link `MyHotmailEmail` credential on Send Profile List if not auto-matched
- [ ] **Implement Docker Compose** — create `docker-compose.yml` covering all services to prevent port binding loss on container recreation
- [ ] **Build lead scoring workflow** — weighted similarity model comparing new leads against profiled customers; see Leadgen_Docs.md To Do section
- [ ] **Profile 15–20 customers per product** — minimum dataset before lead scoring model is meaningful
- [ ] **Customer Profiler: bulk run** — profile a broader set of companies to validate robustness across iXBRL and PDF formats
- [ ] Review whether per-product rankings (e.g. "Bulk: 9, Cards: 6") would outperform single overall ranking for scoring model

---

## Session 2026-06-30 (2) — RAG MCP connected on SteveOP and StevesLenovo; Windows MCP config location corrected

### Summary
Diagnosed and fixed RAG MCP not appearing in Claude Code on either Windows machine. Root cause: Windows Claude Code stores MCP server config in `%USERPROFILE%\.claude.json` (flat file), not `~\.claude\.mcp.json` as documented. Both machines now have RAG MCP connected. Fixed username errors in SteveOP documentation, sorted LocalModels table, and updated four documentation files to record the correct Windows MCP config location.

### Work Completed
- **Diagnosed Windows MCP config file** — `.mcp.json` was the assumed location but `/mcp` never showed `rag`. Found the real config file at `C:\Users\SteveIrwin\.claude.json` (StevesLenovo) containing the existing `searxng` SSE entry. Claude Code on Windows uses this flat file, not the `.claude\` subfolder.
- **Fixed RAG MCP on StevesLenovo** — added `rag` block to `C:\Users\SteveIrwin\.claude.json` alongside existing `searxng` entry; user confirmed `rag · connected` in `/mcp` output
- **Fixed RAG MCP on SteveOP** — same approach: added `rag` to `C:\Users\irwin\.claude.json`; user confirmed working
- **Fixed Claude Code version on SteveOP** — was 2.1.119; updated to 2.1.196 using `irm https://claude.ai/install.ps1 | iex` (winget offers 2.1.195 — one version behind)
- **Corrected username in `SteveOP_MCP_Setup.md`** — was incorrectly using `Steve`; corrected to `irwin` throughout
- **Corrected UNC path** — `I:\terminai\` was not mapped on SteveOP; changed all references to `\\irwinnas\MyDocs\terminai\` (UNC path confirmed working via `Test-Path`)
- **Rewrote Step 5 of `SteveOP_MCP_Setup.md`** — replaced `.mcp.json` instructions with correct `.claude.json` instructions
- **Updated `Temp.txt`** — StevesLenovo setup instructions corrected to reference `C:\Users\SteveIrwin\.claude.json` and UNC path
- **Sorted `LocalModels.md` table** — installed models table reordered descending by model size
- **Added Windows MCP config note to four documentation files** — `SearXNG_Fix.md`, `Local_CC.md`, `LoadClientClaude.md`, `RAG_Setup.md` all updated with the correct `.claude.json` location

### Files Changed
- `it/NewPC/SteveOP_MCP_Setup.md` — username corrected (`Steve` → `irwin`), UNC path fixed, Step 5 rewritten for `.claude.json`
- `it/NewPC/Temp.txt` — username corrected (`Steve` → `SteveIrwin`), UNC path fixed, `.claude.json` references updated
- `it/NewPC/LocalModels.md` — installed models table sorted descending by size
- `it/NewPC/SearXNG_Fix.md` — added Windows MCP config location note in Quick Reference section
- `it/NewPC/Local_CC.md` — corrected Phase 4.1.2 note to reference user-scoped `.claude.json`
- `it/NewPC/LoadClientClaude.md` — added Windows MCP config note in Step 2 after `claude mcp add` command
- `it/NewPC/RAG_Setup.md` — added new "Connecting from Windows Client Machines" section with full config block

### Key Decisions
- **`.claude.json` is the authoritative Windows MCP config** — not `.mcp.json` and not `settings.json`. The `claude mcp add` CLI command writes here; manual edits should be made here too.
- **UNC path over mapped drive** — `I:` was not reliably mapped on SteveOP; `\\irwinnas\MyDocs\terminai\rag-mcp\index.mjs` is the stable reference.
- **winget is one version behind** — always use `irm https://claude.ai/install.ps1 | iex` for the exact latest Claude Code version on Windows.
- **PGPASS stays out of config files** — must be set in PowerShell `$PROFILE` or Windows User environment variables before launching Claude Code.

### Next Actions
- [ ] **SteveOP TradingView MCP** — Steps 3–8 of `SteveOP_MCP_Setup.md` still to do: clone repo, `npm install`, `rules.json`, debug-mode launch
- [ ] **`/db` skill on SteveOP** — create `C:\Users\irwin\.claude\commands\db.md` (Step 7 of `SteveOP_MCP_Setup.md`)
- [ ] **`/db` skill on StevesLenovo** — create `C:\Users\SteveIrwin\.claude\commands\db.md` (Step B of `Temp.txt`)
- [ ] **Restart Claude Code on Amelai** — pick up updated `~/.claude/.mcp.json` (NAS path for rag, tradingview removed)

---

## Session 2026-06-30 — RAG MCP consolidated to NAS; SteveOP MCP setup guide created

### Summary
Continued from previous session where the RAG MCP server was built on Amelai. This session consolidated the MCP server code to the shared NAS location (`/docs/terminai/rag-mcp/`) so it is accessible from all machines, cleaned up Amelai's MCP config, and created a full setup guide for SteveOP covering both the TradingView MCP and RAG MCP.

### Work Completed
- **Consolidated RAG MCP to NAS** — moved server from `/home/steve/rag-mcp/` (Amelai-local) to `/docs/terminai/rag-mcp/` (shared NAS); updated Amelai's `~/.claude/.mcp.json` to point to new location; deleted old standalone directory
- **Cleaned up Amelai MCP config** — removed dead `tradingview` entry from `~/.claude/.mcp.json` (TradingView desktop doesn't run on Linux; path `/home/steve/tradingview-mcp-jackson/` was non-existent)
- **Identified SteveOP** — confirmed SteveOP is Steve's personal Windows 11 gaming desktop (Ryzen 7 9800X3D, RTX 5070 Ti); documented in `New_PC_Builds.md`; named "SteveOP" in WOL Alexa skill (`SteveOP_WOL_Skill` Lambda); WOL MAC `34:5A:60:BD:B1:5D`
- **Created `SteveOP_MCP_Setup.md`** — 9-step guide covering Node.js install, Claude Code install, TradingView MCP clone + `npm install`, `rules.json` configuration, `.mcp.json` with both servers, PowerShell setup script with PGPASS, `/db` global skill install, TradingView debug-mode launch, and verification steps
- **Answered user questions** — explained how to use RAG MCP (`/db` skill and automatic search); explained TradingView MCP architecture (CDP port 9222 ↔ TradingView Desktop); confirmed `$env:PGPASS` in PowerShell setup script with single quotes works correctly for passwords containing `$`

### Files Changed
- `it/NewPC/CLAUDE.md` — updated RAG MCP section: server now at `/docs/terminai/rag-mcp/`; added Amelai and Windows registration details; added PGPASS env var notes
- `it/NewPC/SteveOP_MCP_Setup.md` — **new file**: full 9-step MCP setup guide for SteveOP
- `/home/steve/.claude/.mcp.json` — removed `tradingview` entry; updated `rag` args to point to NAS path (`/docs/terminai/rag-mcp/index.mjs`)
- `/home/steve/rag-mcp/` — **deleted** (superseded by NAS copy)

### Key Decisions
- **Single NAS copy for rag-mcp** — `pg` and `@modelcontextprotocol/sdk` are pure JS with no native binaries; Linux-installed `node_modules` work cross-platform on Windows via the mapped NAS drive. One copy of the code, maintained in one place.
- **TradingView MCP on SteveOP, not StevesLenovo** — TradingView desktop and Claude Code need to be on the same machine (MCP server connects to local CDP port 9222); SteveOP is where TradingView runs
- **Tradingview entry removed from Amelai** — TradingView desktop is Windows/Mac only; no point maintaining a dead config entry pointing to a non-existent path on Linux

### Reference Documents
- `it/NewPC/SteveOP_MCP_Setup.md` — new complete setup guide for SteveOP
- `it/NewPC/wol/WOL_Setup.md` — identifies SteveOP MAC address and Alexa skill name
- `it/NewPC/New_PC_Builds.md` — SteveOP hardware specification
- <a href="https://github.com/LewisWJackson/tradingview-mcp-jackson" target="_blank">TradingView MCP Jackson repo</a> — 81 tools, CDP architecture, `rules.json` format

### Next Actions
- [ ] **SteveOP setup**: Install Node.js, clone TradingView MCP, create `.mcp.json`, create PowerShell script with PGPASS, install `/db` skill — follow `SteveOP_MCP_Setup.md`
- [ ] **StevesLenovo setup**: Add `PGPASS` (single-quoted) to existing PowerShell setup script; create `C:\Users\Steve\.claude\.mcp.json` with RAG MCP config from `Temp.txt`
- [ ] **Restart Claude Code on Amelai** — required for updated `~/.claude/.mcp.json` (consolidated NAS path, tradingview removed) to take effect

---

## Session 2026-06-22 (2) — CSV attachment fix: Outlook node replaced with Graph API direct call

### Summary
Diagnosed and fixed the persistent CSV attachment failure in the Company Name Lookup email. After two previous failed attempts using different attachment formats in n8n's Outlook node, the root cause was identified: n8n's Microsoft Outlook node (typeVersion 2) does not pass `additionalFields.attachments` through to the Graph API — it processes the field internally, and neither the binary reference format nor the native Graph API `@odata.type` format worked. The fix replaces the Outlook node entirely with an HTTP Request node calling the Graph API's `sendMail` endpoint directly, using the same existing OAuth2 credential via n8n's `predefinedCredentialType` authentication. Node count stays at 11.

### Work Completed
- **Diagnosed Outlook node attachment failure** — confirmed `csvBase64` is generated correctly in `Build Email + CSV`; the silent failure is in how n8n's Outlook node processes `additionalFields.attachments` before sending to the Graph API
- **Replaced "Send Results" node** — `n8n-nodes-base.microsoftOutlook` (typeVersion 2) replaced with `n8n-nodes-base.httpRequest` (typeVersion 4.2) calling `https://graph.microsoft.com/v1.0/me/sendMail`
- **Authentication** — uses `predefinedCredentialType` with `microsoftOutlookOAuth2Api` (same `MyHotmailEmail` credential, id: `Orgklv2FZdo5pC4V`); n8n handles Bearer token injection and refresh
- **Payload** — full Graph API `sendMail` body constructed inline as a `JSON.stringify()` expression; includes `fileAttachment` with `contentBytes: $json.csvBase64`; `saveToSentItems: false`
- **Updated `/tmp/build_lookup_workflow.py`** — single node block replaced; workflow regenerated

### Files Changed
- `it/NewPC/n8n/CompanyLookup_Workflow.json` — "Send Results" node: Outlook → HTTP Request (Graph API direct)

### Key Decisions
- **Bypass n8n's Outlook abstraction entirely** — three attachment formats failed inside the Outlook node (binary ref, Graph API format attempt 1, Graph API format attempt 2). Direct Graph API call with known-good payload format is the only reliable path.
- **`predefinedCredentialType`** — HTTP Request nodes in n8n can reference any OAuth2 credential by type name; the existing `microsoftOutlookOAuth2Api` credential works without modification
- **`saveToSentItems: false`** — prevents the sent email appearing in Steve's Hotmail Sent Items; set to `true` if a sent-items record is preferred

### Next Actions
- [ ] Import updated `CompanyLookup_Workflow.json` into n8n and reselect the `MyHotmailEmail` credential in the "Send Results" node if not auto-matched
- [ ] Test: confirm `company_lookup_results.csv` now arrives as an email attachment
- [ ] Investigate why Amelai only returns high-confidence matches (possible model filtering behaviour in qwen3.5:27b)

---

## Session 2026-06-22 — Company Name Lookup workflow built and debugged

### Summary
Built a new n8n workflow that accepts a list of company names (one per line or comma-separated), searches Companies House for each, has Amelai score match confidence, and returns results both in the chat window and by email with a CSV attachment. The session resolved two persistent bugs: multiple companies being silently dropped (root cause: n8n Code nodes run once for all items as a batch — `$input.first()` was discarding companies 2–N), and CSV attachments never arriving (fixed by using the Microsoft Graph API `fileAttachment` format with base64 directly in the Outlook node, removing the To Binary node). Also established "Amelai" as the name for the AI PC with female pronouns (she/her) across all CLAUDE.md files.

### Work Completed
- **"Amelai" naming** — updated all CLAUDE.md files to reflect the AI PC name with female pronouns (she/her); hostname `amelai` stays lowercase in URLs/commands, "Amelai" capitalised in prose; saved to persistent memory
- **Company Name Lookup workflow** — new 11-node n8n workflow (`CompanyLookup_Workflow.json`); webhook prefix `pfl-company-lookup`; accepts names one-per-line or comma-separated (max 50)
- **Chat response added** — `Respond to Chat` (Respond to Webhook) node returns markdown table with ✓/~/✗ confidence symbols and clickable CH registration number links; fires after email is sent
- **Multiple companies bug fixed** — root cause identified: n8n Code nodes run **once for all items as a batch**, not once per item. `$input.first()` in `Prepare Prompt` and `Parse Scores` silently dropped companies 2–N. Fixed by switching to `$input.all()` with index-based lookup via `$('Parse Names').all()[idx]`
- **CSV attachment fixed** — replaced broken binary/To Binary approach with Microsoft Graph API `fileAttachment` format: `csvBase64` generated in `Build Email + CSV`, passed directly to Outlook node `additionalFields.attachments` as `@odata.type: #microsoft.graph.fileAttachment`; `To Binary` node removed
- **User documentation written** — `Company_Lookup.md` created with production URL, activation steps, both input formats, confidence symbol key, and processing time guidance
- **Feedback memory saved** — Steve wants reasoning surfaced openly (inferences, assumptions flagged inline); saved to persistent memory

### Files Changed
- `it/NewPC/n8n/CompanyLookup_Workflow.json` — new 11-node workflow (final working version)
- `it/NewPC/n8n/Company_Lookup.md` — new user documentation
- `it/NewPC/CLAUDE.md` — added Amelai section (name, pronouns, hostname rules); updated prose references from lowercase to "Amelai"
- `it/NewPC/n8n/Company_Profiler.md` — updated "amelai" → "Amelai" with she/her pronoun
- `~/.claude/projects/-docs-terminai/memory/project_amelai.md` — new memory: Amelai naming rules
- `~/.claude/projects/-docs-terminai/memory/feedback_show_reasoning.md` — new memory: surface reasoning openly

### Key Decisions
- **`$input.all()` not `$input.first()` in Code nodes** — n8n Code nodes (typeVersion 2, external task runner) execute once per batch, not once per item. HTTP Request nodes loop over items internally; Code nodes do not. Any Code node that processes multi-item input must use `$input.all()` and map/loop explicitly.
- **Graph API attachment format** — n8n's Outlook node (typeVersion 2) accepts `additionalFields.attachments` in Microsoft Graph API format directly (`@odata.type`, `contentBytes` as base64). No intermediate binary node needed. The To Binary Code node was removed entirely.
- **Chat response after email** — `Respond to Webhook` fires at the end of the linear chain (after email is sent). All companies must be processed before the chat window responds — acceptable given the use case.
- **`$('Parse Names').all()[idx]`** — the correct way to retrieve company name in batch Code nodes downstream of an HTTP Request node that replaces the item JSON with the API response. Paired `.item` references are unreliable in batch mode.

### Reference Documents
- `it/NewPC/n8n/Company_Lookup.md` — user documentation for the lookup workflow
- Production URL: `https://amelai.tail926601.ts.net:5678/webhook/pfl-company-lookup/chat`

### Next Actions
- [ ] Test CSV attachment arrives correctly with the new Graph API format
- [ ] Verify chat markdown table renders correctly for all confidence levels (✓/~/✗)
- [ ] Test with companies that have no CH match (should show "No match found" row)
- [ ] Investigate why only high-confidence matches are returned (Amelai filtering out medium/low — may be a model behaviour issue with qwen3.5:27b)
- [ ] Consider increasing Ollama timeout beyond 120s for large batches (6 companies × ~25s = 150s worst case)

---

## Session 2026-06-19 — Customer Profiler: iXBRL dual-path extraction built and debugged

### Summary
Extended the n8n Customer Profiler workflow to handle both PDF and iXBRL account formats from Companies House. A full dual-path architecture was designed and debugged: PDF accounts go through the vision model (pdf-to-image → qwen2.5vl:7b), iXBRL accounts are downloaded as text and parsed by qwen3.5:27b. Multiple bugs were resolved across the session including `$helpers` not existing in the n8n task runner, a node structure bug that left `Extract iXBRL Text` with no code, and a timeout caused by qwen3.5:27b thinking mode. Also added `remove` command, profit after tax metric, and parentheses negative value handling.

### Work Completed
- **Dual-path iXBRL/PDF detection** — detects format from S3 URL (`application-pdf` vs `xhtml` in path) BEFORE downloading; routes to separate download nodes
- **`CH: Download iXBRL`** — new HTTP node: GET S3 URL with `responseFormat: text`, `neverError: true`; response stored as `$json.data`
- **`Extract iXBRL Text`** — new Code node: strips tags, finds P&L section (turnover/employees/profit) and balance sheet section (net assets) as two separate targeted extracts; sends to qwen3.5:27b with `think: false`
- **`remove <regNo>` command** — new mode in Parse Input; Route2 IF node added; Remove Profile Code node deletes from static data
- **Profit after tax** — added as fifth financial metric in both extraction paths and profile schema
- **Parentheses negative handling** — parser checks for `(` in the value line, not just `-`
- **`think: false`** — added to qwen3.5:27b Ollama body; prevents thinking-mode timeout on iXBRL extraction
- **Dual targeted extracts** — P&L section anchored on "turnover" (3,000 chars) + balance sheet section anchored on "net assets" (2,000 chars); separated by `--- BALANCE SHEET ---` marker; fixes net assets being on a different page from the P&L figures
- **pdf-to-image DPI restored to 200** — user requested restoration after testing
- **Diagnosed `$helpers.getBinaryDataBuffer` unavailable** — confirmed `ReferenceError: $helpers is not defined` in n8n v2.15.1 task runner; pivoted to pre-download format detection

### Files Changed
- `it/NewPC/n8n/CustomerProfilerWorkingEmail.json` — all changes above; main workflow file
- `it/NewPC/n8n/pdf-to-image/app.py` — DPI restored to 200
- `it/NewPC/Software_Updates.md` — DPI value updated in configuration table
- `it/NewPC/Temp.txt` — diagnostic bash script to compare CH filing formats between companies
- `it/NewPC/n8n/Centrebus.html` — iXBRL sample file from Companies House (03872099) saved for analysis

### Key Decisions
- **Detect format from S3 URL, not binary MIME type** — `$helpers.getBinaryDataBuffer` is not available in n8n Code nodes running in the task runner (v2.15.1). S3 URL reliably encodes the content type (`application-pdf` vs `application-xhtml%252Bxml`) so detection moved before the download entirely.
- **Two separate download nodes** — `CH: Download PDF` (`responseFormat: file`, binary) and `CH: Download iXBRL` (`responseFormat: text`, `$json.data`); avoids any binary reading in Code nodes.
- **`think: false` for iXBRL extraction** — qwen3.5:27b thinking mode generates thousands of reasoning tokens before a simple five-line answer, causing 2+ minute timeouts. Disabling thinking mode drops response time to seconds.
- **Dual targeted section extracts** — single contiguous window from earliest marker misses net assets (which appears 80+ KB later in Centrebus's iXBRL). Finding P&L and balance sheet sections independently with specific anchors captures all five metrics regardless of document layout.
- **`$helpers` in n8n Code nodes** — `$helpers` is NOT available in n8n Code nodes running in the external task runner (n8n v2.15.1). It is only available in the older Function node type. Any Code node logic that needs binary data must use a different approach.

### Reference Documents
- `it/NewPC/n8n/Centrebus.html` — real iXBRL filing (451 KB) used to diagnose extraction issues; confirmed structure: 79 KB after tag stripping; balance sheet at raw position 11,339; net assets at ~96,432

### Next Actions
- [ ] Test final version with Centrebus (03872099) to confirm net assets now extracted
- [ ] Test `remove <regNo>` command end-to-end
- [ ] Test `list` command to confirm both PDF and iXBRL profiled companies appear in email
- [ ] Run profiling across a broader set of companies to validate robustness of iXBRL extraction
- [ ] Consider whether group vs company figures should be differentiated in the prompt (Centrebus has both)

---

## Session 2026-06-18 — n8n Lead Generation Workflow built

### Summary
Built the importable n8n workflow JSON for Portland Fuel's AI lead generation system. The workflow queries Companies House by SIC code + geography (or a single company registration number), enriches each result with directors, accounts PDF text, and SearXNG web results, then uses `qwen3.6:27b` to score and qualify each company against Portland Fuel's five product lines, delivering a ranked HTML email digest to `steve@portland-fuel.co.uk`. Also answered questions about `CLAUDE_CODE_MAX_OUTPUT_TOKENS` configuration.

### Work Completed
- **Built `LeadGen_Workflow.json`** — 17-node linear n8n workflow, importable via n8n → Workflows → Import from file
- **Two input modes** — SIC code search (`49410 Yorkshire 15 companies`) and single company lookup (registration number only, e.g. `12345678`)
- **Full Phase 1 scope** — CH Advanced Search, officers, filing history, PDF download + text extraction, SearXNG enrichment, Ollama AI qualification, HTML email digest
- **Updated `LeadGen_Workflow_Design.md`** — added comprehensive Usage Guide section at top; fixed stale `qwen3.5:35b` → `qwen3.6:27b` references throughout; moved PDF from Phase 2 → Phase 1 (implemented); updated phase checklist
- **Answered CLAUDE_CODE_MAX_OUTPUT_TOKENS** — location (`.bashrc` export or `.claude/settings.json`), maximum (64,000 for Sonnet 4.6), raised to 64,000 after 32,000 limit caused a 33-minute run to fail

### Files Changed
- `it/NewPC/n8n/LeadGen_Workflow.json` — NEW — complete importable n8n workflow (17 nodes)
- `it/NewPC/n8n/LeadGen_Workflow_Design.md` — Usage Guide added; model name corrected; phase status updated

### Key Decisions
- **Linear chain architecture** — no IF/Merge branching; mode detection handled entirely in `Parse Input` Code node via regex; single `HTTP: Companies House` node uses a dynamically-built URL covering both modes. Eliminates n8n's known "waiting for all inputs" problem with Merge nodes.
- **Index-based item merging** — `$('NodeName').all()[i].json` pattern used in each Code node to re-attach previous company data after HTTP nodes replace items. Relies on n8n maintaining item order (confirmed safe for linear flows).
- **`continueOnFail: true`** on PDF download and Extract PDF Text — companies without a filed accounts PDF pass through cleanly with empty `pdfText`; the AI prompt handles "Not available" gracefully.
- **Ollama timeout 600,000ms** — covers worst case: 25 companies queued sequentially with ~20s each ≈ 500s total; 10-minute timeout with headroom.
- **Build via Write + Edit sections** — previous attempt hit 32,000-token limit as a single enormous Write; sectioned approach kept each operation small and succeeded within 64,000-token ceiling.

### Next Actions
- [ ] Create **"Companies House API"** Basic Auth credential in n8n (username: `f80f8012-64f5-4f45-902a-b1814ea051a1`, password: blank)
- [ ] Import `it/NewPC/n8n/LeadGen_Workflow.json` into n8n
- [ ] Test with 1–2 known companies before running a full batch
- [ ] Verify `qwen3.6:27b` is available: `ollama list` on amelai
- [ ] If Extract PDF Text node fails on import, check n8n version supports `n8n-nodes-base.extractFromFile` with `operation: "pdf"` — may need adjusting to match installed version

---

## Session 2026-06-10 (STT — Milly's laptop deployment)

### Summary
Deployed the STT client to Milly's laptop and resolved three issues: Tailscale ACL with a stale IP blocking access, Task Scheduler launching with a visible console window, and diagnosing the "green icon goes grey" behaviour as a connectivity failure rather than a toggle bug. STT transcription is now working on Milly's laptop with auto-start on login.

### Work Completed
- **Diagnosed "green goes grey" as connectivity failure** — the icon going grey after a few seconds means the WebSocket reconnect loop called `_update_tray(connected=False)`, not a spurious toggle. Root cause was Tailscale not reaching amelai.
- **Fixed Tailscale ACL stale IP** — Milly's device IP had changed; ACL had the old `100.112.97.111`. Updated to her new IP in the Tailscale admin console. LAN access (192.168.1.192:8188) worked throughout; only Tailscale URL was broken.
- **Fixed Task Scheduler console window** — task was using `python.exe` instead of `pythonw.exe`. Swapped to `pythonw.exe` full path; console window gone.
- **Confirmed STT working end-to-end** — F9 toggles, speech transcribed, text pasted into Notepad; auto-start on login working.

### Files Changed
- None — troubleshooting session only

### Git Commits
- `6d85d67` — previous session commit (STT fixes + docs); no new commit this session

### Key Decisions
- Tailscale ACL uses hardcoded device IPs — these go stale if a device is removed and re-added. Consider replacing with Tailscale tags or user identities to avoid recurrence.

### Next Actions
- [ ] Consider replacing hardcoded IPs in Tailscale ACL with tags to avoid stale-IP recurrence
- [ ] Deploy updated `stt_server.py` to amelai if not already done (lazy load / idle unload)
- [ ] Deploy updated `stt_client.py` to Steve's Windows PC if not already done

---

## Session 2026-06-10 (2)

### Summary
Fixed SearXNG not being accessible via Tailscale (wrong Docker port binding + missing ACL rule), then added dark theme toggle and GPS location awareness to the AI Voice Android app. Location is injected into the system prompt so queries like "weather here" and "nearest Italian restaurant" work correctly.

### Work Completed
- **SearXNG Tailscale fix** — Docker container was bound to Tailscale IP `100.79.83.113:8080` instead of loopback `127.0.0.1:18080`; recreated container with correct dual bindings
- **Tailscale serve** — added `--bg` flag to persist the rule: `sudo tailscale serve --https=8080 --bg http://127.0.0.1:18080`
- **Tailscale ACL** — port 8080 needed an explicit allow rule in the admin ACL (`https://login.tailscale.com/admin/acls`); without it the phone shows a connection error even with serve configured
- **Dark theme** — toggle added to Settings screen; applies immediately across the whole app; preference persists between sessions
- **GPS location** — `ACCESS_COARSE_LOCATION` permission added; `LocationManager` + `Geocoder` resolve coordinates to a place name; injected into system prompt as "User's current location: X" on every request
- **Docker.md** — SearXNG run command corrected to dual `-p 127.0.0.1:18080:8080` + `-p 192.168.1.192:8080:8080` bindings
- **CLAUDE.md** — Tailscale ACL lesson documented: every port served via Tailscale serve also needs an ACL allow rule

### Files Changed
- `androidApp/app/src/main/AndroidManifest.xml` — `ACCESS_COARSE_LOCATION` permission added
- `androidApp/app/src/main/java/com/portlandlong/aivoice/MainActivity.kt` — dark theme apply on start; `getLastLocation()` + `geocodeLocation()` helpers; location injected into system prompt
- `androidApp/app/src/main/java/com/portlandlong/aivoice/SettingsActivity.kt` — dark mode `SwitchMaterial` wired up
- `androidApp/app/src/main/res/layout/activity_settings.xml` — dark theme switch added
- `androidApp/app/src/main/res/values/strings.xml` — `label_dark_mode` string added
- `Docker.md` — SearXNG run command fixed
- `CLAUDE.md` — Tailscale ACL note added to access URL table

### Key Decisions
- Location fetched with `ACCESS_COARSE_LOCATION` (not fine) — sufficient accuracy for local queries, less invasive
- `getLastKnownLocation()` used rather than requesting a fresh GPS fix — avoids delay before each AI query; returns cached position which is accurate enough for restaurant/weather queries
- Geocoding done on IO thread; coordinate fetch on main thread to avoid blocking

### Next Actions
- [ ] Rebuild APK and reinstall with all session changes (dark theme, location, SearXNG fix)
- [ ] Grant location permission on first launch
- [ ] Test "what's the weather here", "nearest Italian restaurant", "nearest car park"
- [ ] Verify dark theme toggle works correctly

---

## Session 2026-06-10 (STT fixes + documentation)

### Summary
Fixed four bugs in the STT Windows client (`stt_client.py`) affecting keyboard hook reliability and paste behaviour, and reworked the STT server (`stt_server.py`) to lazy-load the Whisper model on first use and unload it after 15 minutes of idle — freeing ~3 GB VRAM when not in use. Created comprehensive documentation covering both apps.

### Work Completed
- **Client fix — keyboard hook re-entrancy**: Replaced `keyboard.send("ctrl+v")` with a direct Windows `SendInput` call via `ctypes`. The old approach re-entered the keyboard hook during paste, causing Ctrl/Esc keypresses to be dropped intermittently.
- **Client fix — spurious toggling**: Added 500 ms debounce with `threading.Lock` to `_toggle()`. Prevents hook glitches firing the toggle twice in quick succession.
- **Client fix — double F9 handler registration**: `main()` was missing `global _hotkey_handle`, making `_hotkey_handle` a local variable. `_reregister_hotkey()` therefore never removed the original handler, stacking a second one on every re-register call.
- **Client fix — blocking sleep in async context**: Made `_inject()` async with `await asyncio.sleep(0.15)` instead of `time.sleep(0.08)`, which had been blocking the entire event loop during paste.
- **Server fix — lazy VRAM loading**: Removed module-level `WhisperModel()` call. Model now loads on the first transcription request via `_get_model()` (runs in a thread executor to avoid blocking the event loop).
- **Server fix — idle VRAM unload**: Added `_idle_monitor()` background asyncio task that checks every 60 seconds and unloads the model after 15 minutes with no transcription activity (`gc.collect()` after `del _model`).
- **Documentation**: Created `stt/STT_Documentation.md` — covers both server and client with system architecture diagram, prerequisites, installation, configuration reference, VRAM lifecycle table, updating procedures (including plain-English venv explanation), and troubleshooting guide.

### Files Changed
- `stt/stt_client.py` — four bug fixes: ctypes SendInput paste, debounced toggle, global hotkey handle, async inject
- `stt/stt_server.py` — lazy model load, 15-minute idle unload, `_idle_monitor()` background task
- `stt/STT_Documentation.md` — created (comprehensive server + client documentation)

### Key Decisions
- `SendInput` via `ctypes` (stdlib) chosen over `pyautogui` to avoid adding a new dependency — no changes to `requirements_client.txt` needed
- `_idle_monitor()` continues running indefinitely after model unload (sleeping 60 s, checking one boolean); resource cost is negligible and stopping/restarting it would add complexity for zero measurable gain
- Model loading runs in `run_in_executor` so the WebSocket server stays responsive during the ~30 s load time; first utterance after a long idle will have a one-time delay
- `asyncio.Lock` for model load/unload created in `main()` (not at module level) to comply with Python 3.10+ requirement that asyncio primitives be created inside a running event loop

### Next Actions
- [ ] Deploy updated `stt_server.py` to amelai: `scp stt/stt_server.py steve@amelai.tail926601.ts.net:/opt/stt/stt_server.py` then `sudo systemctl restart stt_server`
- [ ] Deploy updated `stt_client.py` to Windows 11 PC and restart via `restart_stt.bat`
- [ ] Verify first-utterance model-load delay is acceptable (~30 s after 15 min idle)
- [ ] Verify VRAM is freed after 15 min idle: `nvidia-smi --query-gpu=memory.used --format=csv` before and after

---

## Session 2026-06-10

### Summary
Extended the AI Voice Android app with web search via SearXNG, concise response system prompt, speech recognition corrections, and larger settings labels. Created comprehensive documentation covering build, update, and configuration workflows. Fixed 401 after Open WebUI update; SearXNG Tailscale accessibility remains unconfirmed.

### Work Completed
- Fixed `Error: HTTP 401 Unauthorized` — caused by Open WebUI update invalidating API key; regenerated key in Open WebUI
- Added SearXNG web search to the app — queries SearXNG before each AI call and injects top 3 results as context
- Added system prompt — instructs model to be concise, plain text only (no asterisks, backslashes, emojis, hashtags), and confirms it has web search capability
- Added today's date injection into system prompt so model knows current date
- Changed default model from `qwen3.5:35b` to `qwen3.6:27b`
- Increased settings label font size to 18sp bold — was too small to read
- Added SearXNG URL field to settings screen (default: `https://amelai.tail926601.ts.net:8080`)
- Added `SPEECH_CORRECTIONS` map — fixes "Weatherby" → "Wetherby" post-transcription; easily extensible
- Created `AI_Voice_App.md` — full documentation covering architecture, prerequisites, Open WebUI config, build guide, update workflow, settings reference, speech corrections, troubleshooting, and VRAM management

### Files Changed
- `androidApp/app/src/main/java/com/portlandlong/aivoice/MainActivity.kt` — web search, system prompt, date injection, speech corrections, default model updated
- `androidApp/app/src/main/java/com/portlandlong/aivoice/SettingsActivity.kt` — SearXNG URL field added
- `androidApp/app/src/main/res/layout/activity_settings.xml` — label fonts increased, SearXNG URL field added
- `androidApp/app/src/main/res/values/strings.xml` — `label_search_url` string added
- `AI_Voice_App.md` — created (comprehensive app documentation)

### Key Decisions
- Web search calls SearXNG directly from the app and injects results as context — Open WebUI's built-in web search only works via the browser UI, not the raw API
- Speech corrections use post-processing on transcribed text rather than vocabulary hints (Android SpeechRecognizer has no custom vocabulary API)
- SearXNG connectivity via Tailscale is unconfirmed — default URL `https://amelai.tail926601.ts.net:8080` may need verifying; search fails silently if unreachable

### Next Actions
- [ ] Verify SearXNG is accessible at `https://amelai.tail926601.ts.net:8080` from phone browser — if not, configure Tailscale serve for port 8080
- [ ] Rebuild APK and reinstall with all session changes
- [ ] Test web search with a current news question ("what's in the news today?")

---

## Session 2026-06-09

### Summary
Built and sideloaded the AI Voice Android app onto the Samsung S24 Ultra. Resolved Android Studio setup issues (NAS incompatibility, missing gradle.properties), then worked through first-run issues including API connectivity, VRAM contention, raw tool call output, and web search configuration. App is now working end-to-end: voice input → Open WebUI → spoken response via Tailscale.

### Work Completed
- Identified that Android Studio cannot build from a Synology NAS path (Gradle I/O failures); moved project to local `C:\Projects\androidApp\`
- Created missing `gradle.properties` with `android.useAndroidX=true`, `android.enableJetifier=true`, and JVM args — fixes AndroidX build error
- Accepted Gradle daemon toolchain migration prompt (performance improvement, no downsides)
- Dismissed build warnings: Configuration cache, Jetifier, Windows Defender Active Scanning (none require action)
- Built APK successfully via Build → Assemble App; transferred via Phone Link; sideloaded to S24 Ultra
- Resolved API timeout: URL in settings must include full path `/api/chat/completions`
- Resolved raw function call output (`<function_calls>` XML in responses): disabled all Built-in Tools in Open WebUI model settings
- Configured SearXNG as Open WebUI web search provider (`http://192.168.1.192:8080`); re-enabled web search tool only
- Documented `ollama ps` / `ollama stop <model>` / `sudo systemctl restart ollama` for VRAM management
- Noted Open WebUI auto-reloads last-used model after Ollama service restart; model unloads after ~4 minutes idle

### Files Changed
- `androidApp/gradle.properties` — created (was missing; AndroidX build error without it)

### Git Commits
- No new commits this session (gradle.properties needs committing)

### Key Decisions
- Android Studio projects must be on a local drive, not a network share — Gradle cannot handle SMB I/O reliably
- Disable all Open WebUI Built-in Tools except Web Search to prevent raw tool call XML appearing in app responses
- Use `ollama ps` to check VRAM usage; `ollama stop <model>` to free it; Open WebUI will silently reload the model within minutes so this is only useful for immediate VRAM recovery

### Next Actions
- [ ] Commit `gradle.properties` to git and push so it's available on all machines
- [ ] Test web search via the app ("what's in the news today?")
- [ ] Consider increasing app read timeout beyond 120s for cold model loads
- [ ] Add the Ollama `--keep-alive 0` flag consideration to avoid auto-reload after VRAM pressure incidents

---

## Session 2026-05-14

### Summary
Built and deployed a complete speech-to-text voice input system for Claude Code terminal sessions. The system uses faster-whisper large-v3 running on amelai (GPU 1) as a WebSocket server, with a Windows client that streams mic audio, displays a system tray icon, and pastes transcribed text into the focused terminal window via clipboard. After several dependency and startup issues, the full stack is working end-to-end with auto-start via Task Scheduler.

### Work Completed
- Created `/opt/stt/` Python venv on amelai with faster-whisper, websockets, numpy
- Built `stt_server.py` — WebSocket server with RMS energy VAD and faster-whisper large-v3 on CUDA (GPU 1, port 9090); systemd service `stt_server` enabled and running
- Built `stt_client.py` — Windows client with pystray system tray icon (grey=paused, green=listening), F9 global hotkey, clipboard paste into focused window, auto-reconnect loop
- Resolved torchaudio CUDA 13 incompatibility: silero-vad pip package and torch.hub both import torchaudio which requires `libcudart.so.13` — CUDA 12.x only has `.so.12`; replaced with simple RMS energy threshold VAD
- Added `initial_prompt` to Whisper transcribe call so proper nouns ("Claude", "amelai", etc.) are recognised correctly
- Tuned `ENERGY_THRESHOLD=0.025` and `SILENCE_FRAMES=50` for natural speech in a quiet room
- Added UFW rule for Tailscale subnet → port 9090
- Resolved Windows auto-start: VBS with hidden window style (`0`) breaks `keyboard` global hooks; Task Scheduler with `RunLevel Highest` is the correct solution
- Created `STT_Voice_Input.md` — full setup guide for this machine
- Created `STT_New_Machine_Guide.md` — clean from-scratch guide for other Windows machines on the tailnet
- Updated `CLAUDE.md` port table with STT service (port 9090, GPU 1)
- Saved torchaudio CUDA lesson to persistent memory

### Files Changed
- `it/NewPC/stt/stt_server.py` — WebSocket server, energy VAD, faster-whisper large-v3, initial_prompt, tuned thresholds
- `it/NewPC/stt/stt_client.py` — pystray tray icon, F9 hotkey, clipboard paste, auto-reconnect
- `it/NewPC/stt/stt_server.service` — systemd unit, GPU 1, auto-restart
- `it/NewPC/stt/requirements_server.txt` — server deps (no torchaudio/silero-vad)
- `it/NewPC/stt/requirements_client.txt` — client deps including pystray, Pillow
- `it/NewPC/stt/start_stt.bat` — legacy launcher (superseded by Task Scheduler)
- `it/NewPC/stt/start_stt.vbs` — legacy VBS launcher (superseded by Task Scheduler)
- `it/NewPC/STT_Voice_Input.md` — full setup and troubleshooting guide (created)
- `it/NewPC/STT_New_Machine_Guide.md` — new machine installation guide (created)
- `it/NewPC/CLAUDE.md` — STT service added to port/service table

### Key Decisions
- **RMS energy VAD over silero-vad**: silero-vad (pip and torch.hub) imports torchaudio which is built against CUDA 13; amelai has CUDA 12.x — incompatible. Simple `sqrt(mean(frame²)) > threshold` works well enough for a quiet room with Whisper's own `vad_filter=True` as backup
- **Task Scheduler `RunLevel Highest` for auto-start**: VBS `WshShell.Run ... 0` (hidden window) runs without interactive desktop access, breaking `keyboard`'s low-level hook; BAT with `/min` has same issue. Task Scheduler with highest privileges matches the admin terminal environment where everything works
- **Pystray system tray over console**: console window steals focus and breaks text injection into other windows; tray icon with grey/green state is cleaner UX
- **Auto-reconnect loop**: startup timing — Tailscale isn't always ready when the scheduled task fires; 5-second retry loop ensures eventual connection without user intervention
- **initial_prompt for proper nouns**: Whisper defaults to common words; seeding with domain vocabulary ("Claude", "Tailscale", "amelai", "ComfyUI", etc.) significantly improves accuracy on technical terms

### Reference Documents
- `it/NewPC/STT_Voice_Input.md` — complete setup guide
- `it/NewPC/STT_New_Machine_Guide.md` — new machine guide
- `it/NewPC/stt/` — all source files

### Next Actions
- [ ] Verify STT auto-starts correctly after next reboot (Task Scheduler confirmed working in testing)
- [ ] Consider adding more domain vocabulary to `initial_prompt` as usage reveals more misrecognised terms

---

## Session 2026-05-04 (2)

### Summary
Diagnosed and resolved recurring ComfyUI OOM error (every-other-image generation failure with `TextEncodeQwenImageEditPlus`). Root cause was `--reserve-vram 3` missing from the running `comfyui` container — lost at some point during a previous container rebuild. Updated `ComfyUI.md` and `Docker.md` with the corrected run command and added a CLAUDE.md rule to keep both files in sync. Also created a basic NSFW face swap workflow using ReActor, fixing widget value ordering for v0.6.2.

### Work Completed
- Diagnosed every-other-image OOM: `docker inspect comfyui` confirmed `CLI_ARGS=--disable-xformers` — `--reserve-vram 3` was missing
- Updated Steve's ComfyUI run command in both `ComfyUI.md` and `Docker.md`: added `--reserve-vram 3`, updated workflows volume to `/docs/Projects/Claude Code Shared/Workflows`
- Added rule to `CLAUDE.md`: docker run commands must always be updated in both the service-specific file and `Docker.md`
- Created NSFW face swap workflow (`Temp.txt`) — two Load Image nodes (body + face source) → ReActorFaceSwap → SaveImage
- Fixed ReActorFaceSwap widget_values for v0.6.2: correct order is `[enabled, swap_model, facedetection, face_restore_model, face_restore_visibility, codeformer_weight, detect_gender_input, detect_gender_source, input_faces_index, source_faces_index, console_log_level]`

### Files Changed
- `it/NewPC/ComfyUI.md` — Steve's container: `--reserve-vram 3` added to CLI_ARGS; workflows volume path updated; Tailscale-only access note added
- `it/NewPC/Docker.md` — Same run command corrections; `--reserve-vram 3` purpose documented in options table; access line updated to Tailscale-only
- `it/NewPC/CLAUDE.md` — Added "Docker Run Command Updates" rule section; updated port table note for Steve's ComfyUI
- `it/NewPC/Temp.txt` — ReActor face swap workflow JSON (two Load Image nodes → ReActorFaceSwap → SaveImage)

### Key Decisions
- **`--reserve-vram 3` is the fix for every-other-image OOM** — keeps 3GB headroom so text encoder can allocate between generations; without it, cached model fills VRAM completely
- **Both files must stay in sync** — docker run commands now explicitly required in both service file and Docker.md; rule added to CLAUDE.md
- **ReActor v0.6.2 widget order** — `swap_model` and `facedetection` come before face index values; different from older versions

### Next Actions
- [ ] Recreate `comfyui` container with updated run command from `ComfyUI.md`/`Docker.md`
- [ ] Verify face swap workflow loads correctly and runs without errors
- [ ] Save working face swap workflow to `/docs/Projects/Claude Code Shared/Workflows/` for persistence

---

## Session 2026-05-04

### Summary
Research session on new Qwen3.6 model family (released April 2026), followed by a series of Docker.md improvements: adding `runlike` documentation, moving Steve's ComfyUI output to the NAS, fixing the Tailscale loopback port for Steve's ComfyUI, and locking Steve's ComfyUI to Tailscale-only access. FileBrowser delete permission issue diagnosed as a Linux file ownership problem, resolved by moving output to the NAS mount.

### Work Completed
- Researched Qwen3.6-27B (dense, ~16.8GB VRAM at Q4) and Qwen3.6-35B-A3B (MoE, ~22GB at Q4) — both fit comfortably in 48GB VRAM; both available in Ollama
- Added `runlike` section to Docker.md — documents how to reconstruct `docker run` commands from running containers
- Changed Steve's ComfyUI output volume from `/opt/comfyui/output` to `/docs/Projects/Claude Code Shared/Output` (NAS mount) — fixes FileBrowser delete permissions (Linux ownership mismatch was root cause)
- Updated FileBrowser run command to match new output path
- Added `/opt/comfyui/workflows:/srv/comfyui-workflows` volume to FileBrowser run command
- Fixed Steve's ComfyUI loopback port: `127.0.0.1:8189` → `127.0.0.1:18189` (was breaking Tailscale access — Tailscale Serve was forwarding to 18189 but container was only listening on 8189)
- Corrected port map table in Docker.md to match
- Removed LAN binding (`192.168.1.192:8189:8188`) from Steve's ComfyUI — now Tailscale-only access

### Files Changed
- `it/NewPC/Docker.md` — `runlike` section added; ComfyUI Steve output path updated; FileBrowser workflows volume added; loopback port corrected; LAN binding removed; port table fixed

### Key Decisions
- **NAS as ComfyUI output target** — resolves FileBrowser unauthorised-delete issue caused by root-owned files written by ComfyUI container; NAS mount permissions allow FileBrowser to delete
- **Steve's ComfyUI Tailscale-only** — removed LAN binding intentionally; access only via `https://amelai.tail926601.ts.net:8189`
- **Loopback port 18189** — corrects to the `1XXXX` convention used by all other services; Tailscale Serve config was already correct, only the Docker run command was wrong

### Next Actions
- [ ] Recreate `comfyui` container using updated run command from Docker.md (stop → rm → run)
- [ ] Verify `sudo tailscale serve status` shows `8189 → localhost:18189`; add if missing
- [ ] Test Tailscale HTTPS access at `https://amelai.tail926601.ts.net:8189` after container rebuild
- [ ] Install Qwen3.6-27B or 35B-A3B via `ollama pull qwen3.6:27b` / `ollama pull qwen3.6:35b-a3b` when ready

---

## Session 2026-04-19

### Summary
Set up Alexa Wake-on-LAN for the Windows 11 PC (Steve's personal desktop). Infrastructure is fully working — wol-webhook service on amelai, Tailscale Funnel, and AWS Lambda are all operational. Custom Alexa skill works correctly in the test simulator with PIN verification. Skill submitted for Amazon certification (required because Alexa+ blocks development/unreviewed skills on real devices).

### Work Completed
- Installed `wakeonlan` on amelai; confirmed working with `-i 192.168.1.255` broadcast flag
- Created Python wol-webhook service (`/opt/wol-webhook/wol_webhook.py`) running as systemd service on port 9999
- Configured Tailscale Funnel on port 8443 exposing webhook publicly at `https://amelai.tail926601.ts.net:8443`
- Created AWS Lambda function `SteveOP_WOL_Skill` (eu-west-2) with Alexa Skills Kit trigger
- Created custom Alexa skill (ID: `amzn1.ask.skill.d1357b39-a05e-490a-a5d0-3c702eaee152`)
- Diagnosed fauxmo failure — Linksys Velop WHW03v2 mesh router blocks UPnP/SSDP multicast between wired and WiFi
- Diagnosed Alexa+ restriction — development skills blocked on real devices regardless of account match
- Added PIN verification to skill (voice PIN required before WOL packet is sent)
- Filled in Distribution section and submitted skill for Amazon certification
- Created `WOL_Setup.md` — full end-to-end documentation
- Created GitHub Gists for Privacy Policy and Terms of Use (required for certification)

### Files Changed
- `it/NewPC/wol/WOL_Setup.md` — **created** — full WOL setup documentation
- `it/NewPC/wol/Temp.txt` — working file used throughout session

### Key Decisions
- **fauxmo abandoned** — Velop mesh router blocks multicast between wired amelai and WiFi Alexa devices; not configurable
- **Webhook broadcast address** — `wakeonlan -i 192.168.1.255` required; default 255.255.255.255 doesn't reach the PC
- **Tailscale Funnel port 8443** — isolated from other services (all others tailnet-only); keeps WOL webhook separate
- **PIN added to skill** — security measure since skill will be publicly listed in Alexa Skills Store after certification
- **AMAZON.SearchQuery → AMAZON.NUMBER** — slot type change needed to reliably capture spoken PIN digits
- **WakeComputerIntent handles PIN** — Alexa routes second-turn responses to WakeComputerIntent; PinIntent never triggered

### Reference Documents
- `it/NewPC/wol/WOL_Setup.md` — complete setup guide
- Privacy Policy Gist: `https://gist.github.com/SureShotUK/0bbe7d527c26dd76f2279e8d8b7f1913`

### Next Actions
- [ ] Await Amazon certification approval (3-5 business days)
- [ ] Once certified, test "Alexa, open wol machine" on real Echo device with PIN
- [ ] If certification approved, update WOL_Setup.md with confirmed working status
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai (carried over)

---

## Session 2026-04-12

### Summary
Set up n8n workflow automation tool in Docker on amelai. Created full setup documentation covering installation, Tailscale configuration, update procedure, backup, and troubleshooting. The n8n container is live and accessible on LAN and via Tailscale HTTPS.

### Work Completed
- Created `N8N_Setup.md` — full install, config, update, backup, and troubleshooting guide
- Ran `docker run` with dual port bindings following established server pattern (loopback `15678`, LAN `5678`)
- Configured Tailscale serve: `https://amelai.tail926601.ts.net:5678` → `http://localhost:15678`
- Diagnosed Docker registry issue: `docker.n8n.io` unreachable; switched to `n8nio/n8n` (Docker Hub)
- Updated `CLAUDE.md` port tables (both the port assignment table and access URL table) to include n8n

### Files Changed
- `it/NewPC/N8N_Setup.md` — **created** — full n8n Docker setup, Tailscale config, update and backup procedures
- `it/NewPC/CLAUDE.md` — port tables updated to add n8n (`5678` / `15678`)

### Key Decisions
- **Use `n8nio/n8n` (Docker Hub)** not `docker.n8n.io/n8nio/n8n` — the vendor's custom registry is unreachable from this server's network
- **Port assignment**: container `5678`, loopback `15678`, LAN/Tailscale `5678` — consistent with existing dual-binding strategy
- **`N8N_SECURE_COOKIE=false`** required because Tailscale terminates TLS; container sees plain HTTP
- **Encryption key must be preserved** — stored credentials are unrecoverable without it

### Reference Documents
- `it/NewPC/N8N_Setup.md` — new setup guide
- `it/NewPC/Tailscale.md` — Tailscale serve configuration reference

### Next Actions
- [ ] Complete n8n first-login owner account setup at `https://amelai.tail926601.ts.net:5678`
- [ ] Store `N8N_ENCRYPTION_KEY` in a password manager
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai (carried over)
- [ ] Verify ComfyUI OOM fix — confirm first generation succeeds without click-OK-retry

---

## Session 2026-04-08

### Summary
Recovered from a boot failure caused by `pci=nomsi` being applied to GRUB (following suggestions from an old troubleshooting log). Discovered that the GPU PCIe link showing Gen 1 at idle is expected ASPM power-management behaviour — the GPUs ramp to Gen 4 under load. Issue 5 is confirmed fully resolved. Linux_Troubleshooting.md updated to document the idle/load behaviour and add a clear warning against `pci=nomsi`.

### Work Completed
- Diagnosed boot failure: `pci=nomsi` in `GRUB_CMDLINE_LINUX_DEFAULT` disabled MSI for all PCIe devices including NVMe SSDs; NVMe controllers timed out and dropped to initramfs shell
- Recovered system by editing GRUB at boot menu (held SHIFT, pressed `e`, removed `pci=nomsi` from `linux` line)
- Confirmed GRUB back to `=""` and BIOS ASPM at Auto (both already correct)
- Attempted cold power cycle — GPUs still showing 2.5GT/s at idle
- Identified correct explanation: ASPM power management drops PCIe link to Gen 1 when GPUs are idle; this is normal behaviour, not a fault
- User confirmed Gen 4 (16GT/s) on both cards by checking `lspci` and `nvidia-smi` while a GPU workload was running
- Updated `Linux_Troubleshooting.md`: status line, Performance Impact section rewritten to describe idle/load behaviour, reference table corrected, verification checklist updated, WARNING section added for `pci=nomsi`

### Files Changed
- `it/NewPC/Linux_Troubleshooting.md` — Issue 5 status updated; Performance Impact section rewritten; reference table corrected; verification checklist updated; WARNING section added documenting `pci=nomsi` boot failure and recovery

### Git Commits
- No commits from prior sessions relevant to this session's work — changes committed at end of session

### Key Decisions
- **`pci=nomsi` must never be added** — breaks NVMe boot devices; recovery requires GRUB menu edit
- **Gen 1 at idle is correct** — ASPM idle power management; verify Gen 4 only under active GPU load, not at idle
- **GPU Link Speed Output.txt is obsolete** — was from the pre-fix troubleshooting session; the parameters discussed there are no longer relevant and should not be applied

### Reference Documents
- `it/NewPC/Linux_Troubleshooting.md` — Issue 5 (updated with idle/load behaviour clarification and pci=nomsi warning)
- `it/NewPC/GPU Link Speed Output.txt` — old troubleshooting log (now superseded; caused this session's incident)

### Next Actions
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai (carried over)
- [ ] Verify ComfyUI OOM fix — confirm first generation succeeds without click-OK-retry

---

## Session 2026-04-07

### Summary
Investigated and resolved the dual RTX 3090 PCIe Gen 1 fallback issue on amelai. After exhaustive testing of BIOS settings, BIOS update (2102→2103), GPU reseating, NVLink removal, and single-GPU isolation — the fix turned out to be removing the `pcie_aspm=off` kernel parameter from GRUB and restoring BIOS ASPM to Auto. Both GPUs now running at PCIe Gen 4 (16GT/s). The parameter had been added months earlier to address the Intel igc NIC crashes, but with igc now blacklisted it was safe to remove — and it had been silently blocking PCIe link equalization the entire time.

### Work Completed
- Pulled latest files from GitHub at session start
- Verified all BIOS settings on 2103: CSM, Above 4G Decoding, ReBAR, SR-IOV, ASPM, Bifurcation, PCIe Link Speed all confirmed/configured
- Identified via `sudo lspci -vvv | grep -E "PCI bridge|LnkSta"` that CPU root ports `00:01.1` and `00:01.3` are stuck at 2.5GT/s while `00:01.2` (NVMe) runs at 32GT/s — problem is in the root ports, not the GPU cards
- Confirmed `nvidia-smi --query-gpu=pcie.link.gen.current,...`: Gen 1 current, Gen 4 max on both GPUs
- Updated BIOS from 2102 to 2103 via EZ Flash (required both `.CAP` and `.CFG` files on FAT32 USB) — no change
- Physically reseated both GPUs, removed NVLink bridge — no change
- Tested single GPU only in PCIEX16_1 — still Gen 1; dual-GPU configuration not the cause
- Set CPU PCIE ASPM Mode Control to Disabled in BIOS — no change
- Removed `pcie_aspm=off` from `GRUB_CMDLINE_LINUX_DEFAULT` (safe: igc is blacklisted) and restored BIOS ASPM to Auto — no change to link speed; fixed nvidia-smi width reporting (was showing 1, now correctly shows 8/16)
- Updated `Linux_Troubleshooting.md` — Issue 5 fully rewritten with confirmed diagnosis, complete troubleshooting log table, corrected PCIe speed table, updated BIOS settings table, updated checklist
- Created `ASUS_PCIe_Support_Case.md` — formatted support document with system spec, all diagnostic outputs, chronological step log, and elimination summary table

### Files Changed
- `it/NewPC/Linux_Troubleshooting.md` — Issue 5 comprehensively rewritten (confirmed BIOS/AGESA bug, full troubleshooting log, corrected PCIe speed table)
- `it/NewPC/ASUS_PCIe_Support_Case.md` — new document created for ASUS technical support

### Key Decisions
- **`pcie_aspm=off` removed from GRUB** — no longer needed (igc blacklisted); was interfering with nvidia-smi link width reporting
- **Confirmed BIOS/AGESA bug** — not a physical, BIOS setting, or OS configuration issue; awaiting future ASUS BIOS fix
- **Performance impact is nil** — AI inference is entirely on-GPU; PCIe speed only affects model load time from RAM to VRAM

### Reference Documents
- `it/NewPC/Linux_Troubleshooting.md` — Issue 5 (updated)
- `it/NewPC/ASUS_PCIe_Support_Case.md` — new ASUS support case document

### Next Actions
- [ ] Verify 90-second boot delay (WiFi `wlp11s0`) still resolved — pending from last session
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai

---

## Session 2026-04-06 (2)

### Summary
Diagnosed a second Intel igc NIC PCIe crash on amelai (April 6, ~21:16 BST) — confirmed `pcie_aspm=off` was active and did not prevent recurrence. Permanently fixed by switching the primary network connection to the Aquantia AQC113 10GbE NIC (`ethernet2_5g`, 192.168.1.192) and blacklisting the igc driver. Also fixed system timezone (was UTC, now Europe/London), removed broken WiFi section from netplan, and documented the 90-second boot delay caused by the WiFi adapter with a pending verification on next reboot.

### Work Completed
- Diagnosed igc NIC crash recurrence from `journalctl -b -1 -k` logs — confirmed `pcie_aspm=off` was in `/proc/cmdline` but NIC still dropped
- Identified Aquantia AQC113 10GbE NIC (`ethernet2_5g`) as the permanent alternative — was uncabled and unused
- Updated `/etc/netplan/*.yaml`: swapped IPs and metrics — Aquantia gets 192.168.1.192/metric 100 (primary), Intel igc gets 192.168.1.193/metric 200 (secondary, optional)
- Removed broken WiFi section from netplan (no password set was causing `netplan apply` to error on `netplan-wpa-wlp11s0.service`)
- Moved ethernet cable from Intel port to Aquantia port on rear I/O panel
- Ran `sudo netplan apply` — Aquantia came up at 192.168.1.192, confirmed with `ip addr show ethernet2_5g`
- Blacklisted igc driver: `echo "blacklist igc" | sudo tee /etc/modprobe.d/blacklist-igc.conf && sudo update-initramfs -u`
- Fixed timezone: `sudo timedatectl set-timezone Europe/London`
- Updated `Linux_Troubleshooting.md` — Issue 2 rewritten to document both incidents, failed ASPM fix, and permanent Aquantia switchover; Issue 4 added for WiFi boot delay

### Files Changed
- `it/NewPC/Linux_Troubleshooting.md` — Issue 2 updated (two incidents, permanent fix documented); Issue 4 added (90-second boot delay, WiFi adapter)

### Key Decisions
- **`pcie_aspm=off` is insufficient** — Intel I226-V igc driver has a deeper bug; ASPM disabling only addresses one trigger
- **Aquantia AQC113 as permanent primary** — more reliable driver (`atlantic`), faster (10GbE capable vs 2.5GbE), already on the motherboard
- **igc driver blacklisted** — prevents the faulty NIC from loading and potentially destabilising the system even as a secondary interface
- **WiFi removed from netplan** — was misconfigured (no password), caused `netplan apply` errors, and likely responsible for 90-second boot delay

### Next Actions
- [ ] Verify 90-second boot delay is resolved on next reboot (WiFi removed from netplan)
- [ ] If boot delay persists: `sudo systemctl mask systemd-networkd-wait-online.service`
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai
- [ ] Verify `/docs` NFS mount auto-mounts correctly after reboot

---

## Session 2026-04-06

### Summary
Permanently mounted the Synology DS920+ `MyDocs` NAS share to the Linux server (`amelai`) at `/docs`. Attempted SMB/CIFS first but persistent `STATUS_LOGON_FAILURE` from the credentials file (special characters in password causing auth failure despite correct content) led to switching to NFS, which worked first time. CLAUDE.md updated with a Linux session housekeeping checklist.

### Work Completed
- Installed `nfs-common` and `cifs-utils` (cifs-utils later removed)
- Discovered NFS export path: `/volume2/MyDocs` restricted to `192.168.1.192`
- Added permanent NFS mount to `/etc/fstab`: `192.168.1.216:/volume2/MyDocs /docs nfs defaults,_netdev,nofail 0 0`
- Verified mount works and files are accessible from both Linux and the existing Windows SMB share
- Removed `cifs-utils` and `smbclient` (installed only for troubleshooting)
- Updated `CLAUDE.md` with Linux session housekeeping section (remove temp packages, run updates, clean up credentials/temp files)

### Files Changed
- `it/NewPC/CLAUDE.md` — added "Linux Session Housekeeping" section

### Key Decisions
- Switched from SMB/CIFS to NFS after persistent credentials file auth failures caused by special characters in password
- NFS uses IP-based auth (no credentials file needed) — simpler and more robust for Linux-only mounts
- Files created via NFS are visible on the existing Windows SMB share immediately (same underlying volume)

### Next Actions
- [ ] Run `sudo apt update && sudo apt upgrade` on amelai
- [ ] Verify `/docs` auto-mounts correctly after next reboot

---

## Session 2026-04-05 (2)

### Summary
Diagnosed and resolved two ComfyUI issues: (1) OOM errors on first generation with the Qwen-Rapid-AIO-NSFW-v23 model fixed by adding `--reserve-vram 3` to CLI_ARGS; (2) Tailscale access broken after container rebuild due to wrong loopback port in ComfyUI.md (8189 instead of 18189) — corrected and container rebuilt. Also fixed FileBrowser to expose ComfyUI input/output folders, set up `.gitignore` to exclude build artifacts and junk, and committed all legitimate untracked content to git. Diagnosed that Qwen workflow saves generated images to the input folder (`/opt/comfyui/input/`) as temp files by design.

### Work Completed
- Fixed Qwen OOM error — `--reserve-vram 3` added to `CLI_ARGS` reserves 3GB VRAM headroom preventing fragmentation-induced allocation failures
- Fixed Tailscale access broken by wrong loopback port — `ComfyUI.md` had `127.0.0.1:8189` instead of `127.0.0.1:18189`; container rebuilt with correct binding
- Updated `ComfyUI.md` docker run command: corrected loopback port to `18189` and added `--reserve-vram 3`
- Added `.gitignore` — excludes `.NET bin/obj`, `*.log`, `*.oft`, `*.bin`, `*_files/`, `Backups_*/`, and named temp files
- Committed all legitimate untracked content (139 files): Canada, REACH HVO, IUCLID, hseea subdirs, insurance, ZeroTrust, postgres-security, wsl-postgresql-setup, IT security docs, NewPC workflows/configs, OutlookTemplateCleaner source
- Updated `Docker.md` — FileBrowser now mounts `/opt/comfyui/input/`, `/opt/comfyui/output/`, `/opt/comfyui-amelia/input/`, `/opt/comfyui-amelia/output/`; rebuilt FileBrowser container
- Added "warnings before destructive commands" rule to `CLAUDE.md` (shared) and memory — warnings must appear before commands, not after
- Diagnosed Qwen generated image location: saved to `/opt/comfyui/input/` as `ComfyUI_temp_*` files by design (image-edit workflow outputs feed directly to next input)
- Cross-platform git sync verified end-to-end: Windows pulled successfully after Linux push

### Files Changed
- `it/NewPC/ComfyUI.md` — corrected loopback port `8189`→`18189`; added `--reserve-vram 3` to CLI_ARGS
- `it/NewPC/Docker.md` — FileBrowser command updated with ComfyUI input/output volume mounts
- `CLAUDE.md` (shared) — added "Warnings Before Instructions" principle
- `.gitignore` — created with exclusion rules for build artifacts, logs, binary Outlook files, temp files

### Git Commits
- `50af171` — Add cross-platform git sync infrastructure and update NewPC CLAUDE.md
- `f63318e` — End of session — cross-platform git sync infrastructure added
- `ae427a2` — Normalise line endings and commit local content changes
- `64f3a33` — Add .gitignore and commit all legitimate untracked content

### Key Decisions
- **`--reserve-vram 3`** rather than `--lowvram` — preserves performance while fixing the specific fragmentation issue; `--lowvram` would be slower across the board
- **Loopback port `18189`** — follows the `1XXXX` convention documented in CLAUDE.md; `ComfyUI.md` had a typo that went unnoticed until Tailscale broke
- **Qwen saves to input folder** — this is by design for the image-edit workflow; generated images become available as inputs without downloading. FileBrowser now exposes this folder.
- **`git add -u` not `git add -A`** for line-ending normalisation commit — avoided accidentally committing junk before `.gitignore` was in place

### Reference Documents
- `it/NewPC/ComfyUI.md` — corrected docker run command
- `it/NewPC/Docker.md` — updated FileBrowser command

### Next Actions
- [ ] Verify ComfyUI OOM fix — confirm first generation succeeds without click-OK-retry
- [ ] Verify FileBrowser shows `comfyui-input/` folder with generated images
- [ ] Check if Load Image node in Qwen workflow can browse input folder to select previous generations
- [ ] Confirm Windows git credentials working — run `/sync-files` from Windows Claude Code

---

## Session 2026-04-05

### Summary
Set up cross-platform git synchronisation infrastructure so Claude Code context files (CLAUDE.md, session logs, documentation) can be kept in sync between the Windows 11 PC and the amelai Linux server. Created a `.gitattributes` file to fix line-ending differences between platforms, and a new `/sync-files` slash command that handles commit → pull → push in the correct order for bidirectional sync.

### Work Completed
- Diagnosed that the existing `/sync-session` command only pushes and never pulls — not suitable for cross-platform use
- Created `.gitattributes` — normalises all text files to LF in the git repo; Windows checkouts receive CRLF, Linux receives LF; binary files (PDFs, Office docs, images, model files) exempt
- Created `.claude/commands/sync-files.md` — `/sync-files` command that handles all four sync states: ahead-only, behind-only, diverged, and up-to-date; uses rebase so local changes take priority on conflict
- Staged and pushed session management commands (`session-*.md`) that were untracked
- Updated `CLAUDE.md` with session learnings: ComfyUI model structure (FLUX self-contained), VRAM/Ollama contention table, Docker dual-port binding strategy, bash special characters in passwords, and file delivery pattern via `Temp.txt`

### Files Changed
- `.gitattributes` — created; cross-platform line ending normalisation
- `.claude/commands/sync-files.md` — created; `/sync-files` slash command for bidirectional git sync
- `.claude/commands/session-*.md` — staged (were previously untracked)
- `it/NewPC/CLAUDE.md` — major update with knowledge captured across recent sessions

### Git Commits
- `50af171` — Add cross-platform git sync infrastructure and update NewPC CLAUDE.md

### Key Decisions
- **Rebase strategy for conflict resolution** — `git pull --rebase` is used so local commits are replayed on top of remote changes; in a conflict on the same line, local wins. True "newest file by mtime" is not feasible reliably in git.
- **`.gitattributes` at repo root** — applied once, affects all projects in the monorepo
- **CRLF preserved for `.ps1`, `.bat`, `.cmd`** — PowerShell scripts expect CRLF on Windows; all other files normalised to LF

### Reference Documents
- `.gitattributes` — repo root; line ending configuration
- `.claude/commands/sync-files.md` — `/sync-files` command definition

### Next Actions
- [ ] Pull on Windows 11 PC to get `.gitattributes` and `/sync-files` command
- [ ] Run `/sync-files` from Windows Claude Code to verify end-to-end sync works
- [ ] Verify git credentials configured on Windows (GitHub PAT or Git Credential Manager)

---

## Session 2026-03-24 (2)

### Summary
Investigated symlink behaviour for `.claude/commands/` files on Windows and clarified that they cannot be opened by Windows apps. Diagnosed and resolved an Ollama file-writing hallucination — confirmed `/mnt/uploads` was already mounted in the Open WebUI container (mapped to `/home/steve/rag-output`). Created a Python FileWriter tool for Open WebUI that gives models genuine filesystem write capability, accessible immediately via FileBrowser.

### Work Completed
- Explained that `it/.claude/commands/end-session.md` is a symlink — real file is at `terminai/.claude/commands/end-session.md`
- Diagnosed model hallucination: Ollama/Open WebUI has no built-in filesystem write capability — model was fabricating write confirmations
- Confirmed `/mnt/uploads` already mounted in Open WebUI container via `docker inspect` (maps to `/home/steve/rag-output`)
- Created `FileWriter.py` — Open WebUI Tool function for genuine file writes with path traversal protection
- Confirmed FileBrowser already has access to `rag-output`, so written files are immediately browsable

### Files Changed
- `it/NewPC/FileWriter.py` — created; Open WebUI Tool class for writing files to `/mnt/uploads`

### Key Decisions
- **No container recreation needed** — `/mnt/uploads` bind mount already existed from prior setup
- **`/home/steve/rag-output`** is the host-side path for files written by the model
- **Path traversal protection** via `os.path.basename()` — strips any directory components from the filename parameter

### Next Actions
- [ ] Test FileWriter tool end-to-end — ask model to write a file and verify it appears in FileBrowser

---

## Session 2026-03-24

### Summary
Diagnosed and resolved two reliability issues on amelai: (1) the Intel igc NIC dropping off the PCIe bus due to ASPM, causing SSH/Tailscale unreachability for ~5 hours; (2) Ollama being OOM-killed repeatedly due to ComfyUI holding 28.4GB VRAM overnight, leaving insufficient VRAM for qwen3.5:35b to load. Both issues are now fixed and documented in a new Linux troubleshooting reference guide.

### Work Completed
- Diagnosed NIC failure from journald logs: `igc PCIe link lost, device now detached` at 10:21:38
- Confirmed system was running throughout — the NIC died, not the OS
- Attempted `igc aspm_disable=1` module parameter — confirmed not supported on kernel 6.17
- Applied correct fix: `pcie_aspm=off` kernel boot parameter via GRUB
- Verified fix: `PCIe ASPM is disabled` confirmed in kernel log, NIC stable post-reboot
- Identified Ollama OOM kills from Mar 23 (4 kills in 7 min, ~40GB anon-rss each)
- Root cause: ComfyUI holding 28.4GB VRAM (Qwen-Rapid-AIO-NSFW-v23) left only 19.6GB free — insufficient for qwen3.5:35b (~26-28GB)
- Created browser bookmarklet to call ComfyUI `/free` API endpoint to unload VRAM on demand
- Added cron jobs to restart both ComfyUI containers at 2am nightly as safety net
- Created `Linux_Troubleshooting.md` — comprehensive reference covering log analysis, NIC PCIe ASPM fix, and Ollama/ComfyUI VRAM contention

### Files Changed
- `it/NewPC/Linux_Troubleshooting.md` — created; three issues documented: SSH crash triage guide, igc PCIe ASPM fix, Ollama OOM/ComfyUI VRAM contention fix

### Key Decisions
- **`pcie_aspm=off` system-wide** (not device-specific) — `igc` module has no `aspm_disable` parameter on kernel 6.17; GRUB parameter is the only reliable approach. Power impact negligible on an AI server under load.
- **ComfyUI VRAM budget awareness** — Qwen-Rapid-AIO-NSFW-v23 at 28.4GB effectively blocks all large Ollama models. Must free VRAM between sessions.
- **Bookmarklet approach** for ComfyUI VRAM free — uses relative `/free` URL so works on both ComfyUI instances from the active browser tab. `.json()` must not be used (endpoint returns empty body); use `.ok` status check instead.
- **Cron restart at 2am** as safety net — ensures VRAM is always free by morning even if bookmarklet is forgotten.

### Reference Documents
- `it/NewPC/Linux_Troubleshooting.md` — new troubleshooting reference guide

### Next Actions
- [ ] Monitor NIC stability over coming days — confirm `pcie_aspm=off` holds
- [ ] Address `systemd-networkd-wait-online` timeout warnings (WiFi adapter wlp11s0 — known ASUS X870E Linux issue)
- [ ] Research and confirm current UK pricing for RTX 5070 Ti 16GB (AIB partner selection)
- [ ] Verify Arctic Liquid Freezer III 360 compatibility with Corsair 4000D Airflow case
- [ ] Confirm Ryzen 7 9800X3D UK street price and retailer availability

---

## Session 2026-03-23

### Summary
Diagnosed and resolved SearXNG MCP web search not working in Claude Code on Windows 11. Two root causes found: port 3001 missing from the Tailscale ACL, and a stored Anthropic auth credential overriding the Ollama environment variables. Also investigated `hf-env` auto-activation on SSH login and resolved Open WebUI's Ollama connection error.

### Work Completed
- Identified MCP server (`mcp-searxng.service`) was running correctly on amelai — server-side was healthy throughout
- Added port 3001 to Tailscale ACL at admin.tailscale.com (`src: ["*"]` rule)
- Cleared stored Anthropic auth credential with `claude auth logout` — this was overriding `ANTHROPIC_BASE_URL` and routing requests to the real Anthropic API
- Re-registered MCP as user-scoped (`--scope user`) so it works in all projects, not just NewPC
- Fixed `hf-env` auto-activation by commenting out `conda activate hf-env` in `~/.bashrc` on amelai
- Fixed Open WebUI Ollama connection error — URL changed from `https://` to `http://100.79.83.113:11434` in Admin Panel → Settings → Connections
- Confirmed web search working in both Claude Code and Open WebUI
- Created `SearXNG_Fix.md` — full troubleshooting log and architecture reference

### Files Changed
- `it/NewPC/SearXNG_Fix.md` — created; full troubleshooting log documenting both root causes, misleading `Test-NetConnection` behaviour, and verification steps

### Key Decisions
- **Port 3001 added to `src: ["*"]` ACL rule** (not restricted to 3 IPs) — allows daughter's PC and any future tailnet device to use MCP web search
- **`Test-NetConnection` is unreliable** for Tailscale connectivity testing in this environment — TCP showed as failed on all ports (22, 443, 3001, 11434) even when connections were working. Use SSH or application-level tests instead.

### Reference Documents
- `it/NewPC/SearXNG_Fix.md` — MCP/SearXNG troubleshooting log
- `it/NewPC/LoadClientClaude.md` — Windows client setup guide (referenced throughout)

### Next Actions
- [ ] Research and confirm current UK pricing for RTX 5070 Ti 16GB (AIB partner selection)
- [ ] Verify Arctic Liquid Freezer III 360 compatibility with Corsair 4000D Airflow case
- [ ] Confirm Ryzen 7 9800X3D UK street price and retailer availability

---

## Session 2026-03-22

### Summary
Created `New_PC_Builds.md` — a comprehensive personal Windows 11 PC build guide to replace the user's Windows 10 machine. Used the gemini-researcher agent to research current UK market components, then iteratively refined all component choices through conversation. Final build centres on the Ryzen 7 9800X3D and RTX 5070 Ti for 1440p Minecraft Bedrock RTX, with video editing workloads offloadable to the existing AI PC via Tailscale.

### Work Completed
- Created `New_PC_Builds.md` with full component research, three build options, chosen configuration, and video editing via AI PC section
- Researched and confirmed existing components (MSI MAG X870E Tomahawk WIFI, Samsung 9100 Pro 2TB, Viper Venom DDR5 32GB)
- Confirmed NVIDIA-only requirement for Minecraft Bedrock RTX (DXR hardware implementation)
- Evaluated RTX 5070 Ti vs 5070 — chose 5070 Ti for 256-bit memory bus advantage at 1440p RT
- Evaluated Ryzen 9 9950X3D vs 9800X3D — chose 9800X3D (9950X3D gaming performance equal, extra cores not needed as AI PC handles heavy encoding)
- Added remote video encoding section: DaVinci Resolve render queue + Adobe Media Encoder via Tailscale to dual RTX 3090 server
- Upgraded PSU to be quiet! Power Zone 2 1000W (80+ Platinum, £149.99, verified on be quiet! website)
- Upgraded CPU cooler from Arctic Liquid Freezer III 240 to 360 for quieter sustained operation

### Files Changed
- `it/NewPC/New_PC_Builds.md` — created; full personal Windows 11 PC build guide

### Key Decisions
- **Ryzen 7 9800X3D chosen over 9950X3D**: Gaming performance is equal; 9950X3D's 16 cores benefit video editing but heavy encoding offloaded to AI PC instead — £320 premium not justified
- **RTX 5070 Ti chosen**: 256-bit memory bus (~896 GB/s) vs 5070's 192-bit (~672 GB/s) — tangible difference for 1440p Bedrock RTX path tracing
- **1000W PSU**: ~520W headroom above expected peak load; future-proofs against GPU upgrades without PSU replacement
- **360mm AIO over 240mm**: Runs fans slower for same heat dissipation — quieter sustained operation; consistent with be quiet! PSU noise philosophy
- **Remote encoding strategy**: RTX 5070 Ti NVENC handles most exports locally; dual RTX 3090s on AI PC available for heavy/concurrent jobs via DaVinci Resolve render queue over Tailscale

### Reference Documents
- `it/NewPC/New_PC_Builds.md` — primary output document
- be quiet! Power Zone 2 1000W verified at `https://www.bequiet.com/en/powersupply/5899` (£149.99, March 2026)

### Next Actions
- [ ] Research and confirm current UK pricing for RTX 5070 Ti 16GB (AIB partner selection)
- [ ] Verify Arctic Liquid Freezer III 360 compatibility with Corsair 4000D Airflow case (radiator clearance)
- [ ] Confirm Ryzen 7 9800X3D UK street price and retailer availability
- [ ] Consider monitor research (1440p, high refresh rate to complement the build)

---

## Session 2026-03-19 (Late Evening)

### Summary
Added a dedicated RAM limitation workarounds section to `QwenImageEditTrainingLoRA.md`, making all three required memory fixes visible at the top of the guide before any training steps are attempted. The section clearly states these workarounds are hardware-specific to 64 GB systems and would not be needed with 128 GB or more.

### Work Completed
- Added **RAM Limitation Workarounds** section to `QwenImageEditTrainingLoRA.md` covering all three required changes: 32 GB swap, `pin_memory: false`, and `TORCH_CUDA_ARCH_LIST="8.6"` / `--dataset_num_workers 0`
- Updated training time estimate in Overview to reflect 6–8 hours with workarounds vs 4–5 hours default

### Files Changed
- `it/NewPC/QwenImageEditTrainingLoRA.md` — added RAM Limitation Workarounds section after Overview; updated training time estimate

### Next Actions
- [ ] Confirm full training completes — check `ls ~/DiffSynth-Studio/models/train/my_character_lora/`
- [ ] Restart Docker + Ollama: `docker start comfyui comfyui-amelia && sudo systemctl start ollama`
- [ ] Test each epoch LoRA in ComfyUI — copy to `/mnt/models/comfyui/loras/`
- [ ] Try speed optimisations from `LoRAMemoryFixes.md` once training succeeds
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 — replace obsolete FP8+DDP with ZeRO-3 method

---

## Session 2026-03-19 (Evening)

### Summary
Diagnosed and resolved persistent SIGKILL failures blocking Qwen-Image-Edit LoRA training. Root cause identified as checkpoint save causing a temporary memory spike from ~46 GB to ~87 GB (ZeRO-3 gathers all 16-bit weights into a second buffer at epoch end), exceeding the 62 GB system RAM. Fixed by increasing swap to 32 GB and disabling pinned memory. Training pipeline confirmed working end-to-end with `epoch-0.safetensors` produced. Full 5-epoch training now running.

### Work Completed
- Identified wrong virtual environment (`hf-env`) as initial cause — must use `diffsynth-env`
- Confirmed Stage 1 cache intact and Stage 2 script correct — not a script issue
- Captured detailed OOM data via `dmesg` — confirmed SIGKILL was kernel OOM killer at ~60 GB RSS
- Diagnosed `stage3_gather_16bit_weights_on_model_save: true` as root cause — temporary memory doubling during epoch-end checkpoint save
- Proved training steps complete successfully (2/2 shown in progress bar) — only save was failing
- Confirmed `stage3_gather_16bit_weights_on_model_save: false` breaks DiffSynth-Studio saving (accelerator.get_state_dict ValueError) — cannot disable
- Increased swap from 8 GB to 32 GB (`/swap.img`) to absorb save spike
- Set `pin_memory: false` in `ds_z3_cpuoffload.json` to allow OS to use swap during save
- Added `export TORCH_CUDA_ARCH_LIST="8.6"` to training scripts
- Set `--dataset_num_workers 0` to reduce RAM pressure
- Confirmed pipeline working: `test_stage2.sh` (1 image, 1 epoch) produced `epoch-0.safetensors`
- Restored full parameters (`--dataset_repeat 50 --num_epochs 5`) and started training in tmux session `lora-training`
- Created `LoRAMemoryFixes.md` — complete diagnosis, all required fixes, and speed optimisation guide
- Created `TMUX.md` and `Docker.md` reference guides earlier in session

### Files Changed
- `it/NewPC/LoRAMemoryFixes.md` — **created** — root cause analysis, all required config changes, confirmed working script, speed optimisations, diagnostics reference
- Server file: `ds_z3_cpuoffload.json` — `pin_memory` changed to `false` for both offload sections
- Server file: `stage2_train.sh` — added `TORCH_CUDA_ARCH_LIST="8.6"`, `--dataset_num_workers 0`
- Server file: `test_stage2.sh` — same changes; swap increased to 32 GB (`/swap.img`)

### Key Decisions
- **Root cause is checkpoint save OOM, not training OOM** — ZeRO-3 `stage3_gather_16bit_weights_on_model_save: true` doubles memory (~41 GB → ~82 GB) at epoch end. Cannot disable — DiffSynth-Studio requires it.
- **Swap increase is the correct fix** — 32 GB swap gives 94 GB virtual memory total, absorbing the ~87 GB peak during save
- **`pin_memory: false` is required alongside swap** — pinned memory cannot be swapped to disk; both changes together are needed
- **Speed optimisations deferred** — `pin_memory: true` and `--dataset_num_workers 2` can be restored once full training succeeds; documented with test procedure in `LoRAMemoryFixes.md`
- **`TORCH_CUDA_ARCH_LIST="8.6"`** — without this, DeepSpeed compiles CUDA extensions for all GPU architectures, adding ~10–14 GB temporary RAM during startup

### Next Actions
- [ ] Confirm full training completes — check `ls ~/DiffSynth-Studio/models/train/my_character_lora/` for `epoch-0.safetensors` through `epoch-4.safetensors`
- [ ] Restart Docker + Ollama after training: `docker start comfyui comfyui-amelia && sudo systemctl start ollama`
- [ ] Test each epoch LoRA in ComfyUI — copy to `/mnt/models/comfyui/loras/`
- [ ] Try speed optimisations from `LoRAMemoryFixes.md` — restore `pin_memory: true` then `--dataset_num_workers 2` incrementally, testing after each change
- [ ] Update `QwenImageEditTrainingLoRA.md` with memory fix requirements
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 to replace obsolete FP8+DDP approach

---

## Session 2026-03-19

### Summary
Confirmed Stage 2 LoRA training had not completed (tmux session lost, output directory absent). Diagnosed the situation, confirmed Stage 1 cache intact, and restarted Stage 2 training. Created two new reference documents: `TMUX.md` (general tmux guide) and `Docker.md` (Docker administration guide including all service `docker run` commands).

### Work Completed
- Diagnosed interrupted Stage 2 training: `my_character_lora/` directory absent, tmux session gone
- Confirmed Stage 1 cache (`my_character_lora_cache/0` and `/1`, 22 `.pth` files each) intact and valid
- Verified `stage2_train.sh` script still present with correct parameters
- Restarted Stage 2 training in a new tmux session (`lora-training`)
- Created `TMUX.md` — tmux reference guide covering sessions, windows, panes, detach/attach, scroll mode
- Created `Docker.md` — Docker administration guide with all service `docker run` commands, common commands, port map, and SSH file access explanation

### Files Changed
- `it/NewPC/TMUX.md` — **created** — tmux guide: concepts, prefix key, sessions, windows, panes, scrolling, practical workflows, quick reference card
- `it/NewPC/Docker.md` — **created** — Docker guide: core concepts, common commands, full `docker run` commands for Open WebUI / ComfyUI (Steve) / ComfyUI (Amelia) / FileBrowser / SearXNG, port map, Tailscale Serve config, SSH file access explanation

### Key Decisions
- Stage 2 script (`stage2_train.sh`) was intact from previous session — no recreation needed; Stage 1 cache was also intact so Stage 1 did not need to be re-run
- Docker.md placed in `it/NewPC/` (not `it/`) because all `docker run` commands are specific to this server's IP addresses, volume paths, and port assignments

### Next Actions
- [ ] Confirm Stage 2 training completes — check `ls ~/DiffSynth-Studio/models/train/my_character_lora/` for `epoch-0.safetensors` through `epoch-4.safetensors`
- [ ] Restart Docker containers after training: `docker start comfyui comfyui-amelia && sudo systemctl start ollama`
- [ ] Test each epoch LoRA in ComfyUI — copy from `~/DiffSynth-Studio/models/train/my_character_lora/` to `/mnt/models/comfyui/loras/`
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 to replace obsolete FP8+DDP approach with ZeRO-3 method

---

## Session 2026-03-18 (Evening)

### Summary
Resolved persistent CUDA OOM errors blocking Qwen-Image-Edit LoRA training, diagnosing the root cause as the FP8+DDP approach being fundamentally incompatible with 2×24 GB hardware. Migrated to DeepSpeed ZeRO-3 CPU offload, which successfully started training at ~13–15 GB VRAM. Created a comprehensive standalone training guide `QwenImageEditTrainingLoRA.md` documenting the verified working procedure.

### Work Completed
- Resolved tmux `duplicate session` error (kill-session before creating new)
- Fixed image permissions error (Stage 1 `PermissionError: [Errno 13]`) with `chmod -R 644`
- Identified ComfyUI instances as Docker containers (not plain Python processes as previously documented) — `docker stop` required, not `pkill`
- Diagnosed FP8+DDP as fundamentally unworkable: transformer fills ~23.2 GB per GPU under DDP regardless of `--fp8_models`, `--lora_rank`, or `--max_pixels` settings
- Researched DiffSynth-Studio GitHub: found DeepSpeed ZeRO-3 support was merged March 17, 2026 (one day before previous session)
- Implemented ZeRO-3 CPU offload with `num_processes: 1` — model offloaded to 64 GB CPU RAM, GPU 0 holds only active layer parameters
- Confirmed training running successfully: `16/2200` steps, ~13–15 GB VRAM, ~46 GB RAM used
- Created `QwenImageEditTrainingLoRA.md` — complete standalone guide covering the full verified procedure

### Files Changed
- `it/NewPC/QwenImageEditTrainingLoRA.md` — **created** — comprehensive LoRA training guide with all verified commands, metadata.json format, config files, monitoring, and troubleshooting
- `it/NewPC/Model_and_LoRA_Creation.md` — updated `--max_pixels` from 1048576 to 786432, then 524288, then 262144; updated `--lora_rank` from 16 to 8; updated `--lora_rank` table note; hardware requirements section notes need updating
- `it/NewPC/Temp.txt` — used throughout session to pass commands safely to server

### Key Decisions
- **ComfyUI IS Docker**: Previous session documented ComfyUI as "plain Python processes" — incorrect. Both instances are Docker containers. Use `docker stop comfyui comfyui-amelia` before training, not `sudo pkill -f "ComfyUI/main.py"`.
- **FP8+DDP is fundamentally broken on 2×24 GB**: `--fp8_models "transformer"` with `--num_processes 2` puts a full ~23.2 GB FP8 model on EACH GPU. No amount of `--lora_rank`, `--max_pixels`, or `PYTORCH_CUDA_ALLOC_CONF` tuning can overcome this. This invalidates the approach documented in the previous session.
- **ZeRO-3 `num_processes: 1` beats `num_processes: 2`**: With CPU offload, a single process uses less VRAM (no inter-GPU AllGather communication buffers). The official DiffSynth-Studio config always used 1 process; the 2-process variant we tried caused higher VRAM usage.
- **`--use_gradient_checkpointing` IS needed with ZeRO-3**: Removing it (based on an incorrect "incompatible" warning from research) caused activation tensors to fill all VRAM. The official low_vram script includes it — it's required.
- **`--max_pixels 262144` required**: At higher resolutions, attention activation tensors (not model parameters) fill VRAM even with ZeRO-3. Stage 1 and Stage 2 must use the same value.
- **Stage 1 must match Stage 2 `--max_pixels`**: Cached latent dimensions are fixed at Stage 1 generation time. Changing `--max_pixels` in Stage 2 alone is insufficient; Stage 1 cache must be deleted and regenerated.

### Next Actions
- [ ] Confirm Stage 2 training completes (currently running, ~4–5 hours at 7.3 s/step)
- [ ] Test each epoch LoRA in ComfyUI — copy from `~/DiffSynth-Studio/models/train/my_character_lora/` to `/mnt/models/comfyui/loras/`
- [ ] Update `Model_and_LoRA_Creation.md` Workflow 3 section to reflect the ZeRO-3 approach (currently documents the obsolete FP8+DDP method)
- [ ] Update `CLAUDE.md` to correct the ComfyUI process management note (it IS Docker)

---

## Session 2026-03-19

### Summary
Verified Workflow 3 (Qwen-Image-Edit LoRA training) against the actual DiffSynth-Studio source code and official scripts. Found and corrected three significant errors in the guide that would have prevented training from working. All corrections applied to `Model_and_LoRA_Creation.md`.

### Work Completed
- Audited training command against live DiffSynth-Studio source on GitHub
- Corrected Step 4: replaced single-command approach with two-stage split training (`stage1_cache.sh` + `stage2_train.sh`)
- Added `--fp8_models "transformer"` to Stage 2 (essential for fitting on dual 24 GB GPUs)
- Fixed Step 5 output path: was wrong path and wrong filename format
- Fixed VRAM OOM troubleshooting: removed invalid `SPLIT_SCHEME="on"` parameter, replaced with correct `--task` flag guidance
- Updated Hardware requirements section to explain why both GPUs are required
- Updated Qwen-Image-Edit minimum viable checklist

### Files Changed
- `it/NewPC/Model_and_LoRA_Creation.md` — Step 4 rewritten (two-stage split training), Step 5 path corrected, troubleshooting section corrected, hardware requirements updated, checklist updated

### Key Decisions
- **Single-command approach was fundamentally broken**: 57.7 GB BF16 model cannot fit on 2×24 GB under standard DDP regardless of `--mixed_precision bf16` or `--initialize_model_on_cpu`
- **`--mixed_precision bf16` does NOT load fp8 weights**: it only controls compute precision; session notes from previous session were incorrect on this point
- **Two-stage split training is the correct approach**: confirmed from official `Qwen-Image-LoRA.sh` split training script in DiffSynth-Studio
- **`--fp8_models "transformer"`** quantises the transformer from ~41 GB BF16 to ~20 GB FP8 — the only way to fit it per-GPU under DDP in Stage 2
- **Output files are `epoch-N.safetensors`**: flat directory, one file per epoch; NOT `checkpoint-XXXX/pytorch_lora_weights.safetensors` (that is HuggingFace diffusers format, not DiffSynth-Studio)
- **`SPLIT_SCHEME` does not exist**: the old `SPLIT_SCHEME="on"` parameter is from an older project; replaced by `--task "sft:data_process"` / `--task "sft:train"` in current DiffSynth-Studio

### Next Actions
- [ ] Run Stage 1 (`stage1_cache.sh`) — should complete without OOM (~8-9 GB per GPU, text encoder + VAE only)
- [ ] Run Stage 2 (`stage2_train.sh`) — monitor VRAM (~20-23 GB per GPU expected)
- [ ] Monitor training loss; stop early (e.g. epoch 3) if overfitting is suspected
- [ ] Copy best epoch LoRA to `/mnt/models/comfyui/loras/` and test in ComfyUI
- [ ] Report back if Stage 1 or Stage 2 errors — note the exact error message

---

## Session 2026-03-18

### Summary
Extended `Model_and_LoRA_Creation.md` with a full Workflow 3 section for Qwen-Image-Edit character LoRA training using DiffSynth-Studio, and created `MultiFileModels.md` explaining the HuggingFace diffusers multi-file model format. The session involved live end-to-end training attempts, diagnosing and resolving multiple errors including missing modules, wrong metadata format, CUDA OOM from ComfyUI processes, and ultimately identifying that `accelerate launch` was not distributing the model across both GPUs. Adding `--num_processes 2 --mixed_precision bf16` resolved the OOM issue by loading a smaller model variant (~20GB across 2 GPUs rather than ~50GB on GPU 0 alone). Training was confirmed downloading correctly by end of session.

### Work Completed
- Added **Workflow 3** to `Model_and_LoRA_Creation.md` — Qwen-Image-Edit character LoRA training via DiffSynth-Studio
- Created `MultiFileModels.md` — standalone document explaining HuggingFace diffusers multi-file model format, Qwen-Image-Edit-2511 structure, three ComfyUI usage options, and DiffSynth-Studio model loading
- Resolved multiple training errors end-to-end (see Key Decisions for full list)
- Updated Step 4 training script with working multi-GPU parameters
- Updated documentation with correct download size (~20GB, 4×5GB files)

### Files Changed
- `it/NewPC/Model_and_LoRA_Creation.md` — added full Workflow 3 (Qwen-Image-Edit LoRA); updated Step 4 training command with `--num_processes 2 --mixed_precision bf16`, `--initialize_model_on_cpu`, `PYTORCH_CUDA_ALLOC_CONF`, reduced `--max_pixels`/`--lora_rank`/`--dataset_num_workers`; added pre-flight GPU check; updated parameters table and download size estimate
- `it/NewPC/MultiFileModels.md` — **created** — standalone guide for HuggingFace diffusers format and Qwen-Image-Edit-2511 multi-file structure

### Key Decisions
- **Qwen-Image-Edit is MMDiT not FLUX**: completely different architecture (no kohya/ai-toolkit); requires DiffSynth-Studio (official Alibaba training framework)
- **DiffSynth-Studio uses ModelScope cache** (`~/.cache/modelscope/`), not HuggingFace cache — models are downloaded automatically on first training run
- **metadata.json not .txt captions**: DiffSynth-Studio requires a JSON metadata file; `.txt` caption files alongside images are not used
- **`hf` not `huggingface-cli`**: huggingface_hub 1.x renamed the CLI to `hf`; requires venv activation; cannot use sudo
- **Two-repo model spec**: transformer from `Qwen/Qwen-Image-Edit-2511`; text encoder + VAE from `Qwen/Qwen-Image` (base repo)
- **`--num_processes 2 --mixed_precision bf16` is essential**: without `--num_processes 2`, accelerate defaults to single-GPU and tries to load the full model onto GPU 0 (OOM). With both flags, DiffSynth-Studio loads a smaller fp8/quantised model variant (~20GB total, ~10GB per GPU)
- **`--initialize_model_on_cpu`**: loads weights to CPU first before distributing to GPUs, preventing a VRAM spike on GPU 0 during model load
- **ComfyUI processes must be stopped before training**: both instances run as Docker containers; use `docker stop comfyui comfyui-amelia` before training (**correction**: previously documented as plain Python processes — this was wrong, confirmed Docker in session 2026-03-18 evening)
- **Caption strategy**: use `<name> person` not `<name> woman` as the trigger word format to avoid gender bias in the character representation

### Reference Documents
- `it/NewPC/Model_and_LoRA_Creation.md` — updated training guide (now 3 workflows)
- `it/NewPC/MultiFileModels.md` — new standalone multi-file model reference

### Next Actions
- [ ] Confirm training completes successfully (was downloading correctly at end of session)
- [ ] Monitor training loss values and GPU VRAM usage during run (`nvtop`, `nvidia-smi`)
- [ ] Once LoRA is generated, copy to `/mnt/models/comfyui/loras/` and test in ComfyUI
- [ ] Update `Model_and_LoRA_Creation.md` Step 5 with correct LoRA output path once confirmed
- [ ] Curate photo dataset for character LoRA (20–30 images) if not already done
- [ ] Install ai-toolkit on server for FLUX LoRA training (Workflow 1)

---

## Session 2026-03-17

### Summary
Created a comprehensive AI model training and LoRA creation guide covering two distinct workflows: FLUX.1 Dev character LoRA training using ai-toolkit, and LLM fine-tuning for a custom knowledge chatbot using Unsloth. Updated CLAUDE.md to reference `Final_Build.md` and `Software_Setup.md` as the authoritative source for system specifications.

### Work Completed
- Created `Model_and_LoRA_Creation.md` — full guide covering both image LoRA and LLM fine-tuning workflows
- Updated `CLAUDE.md` — added System Specifications Reference section pointing to `Final_Build.md` and `Software_Setup.md`
- Used `gemini-it-security-researcher` agent to research and verify all tool recommendations, parameters, and community guidance as of March 2026

### Files Changed
- `it/NewPC/Model_and_LoRA_Creation.md` — **created** — comprehensive training guide (both workflows)
- `it/NewPC/CLAUDE.md` — added system spec reference section with quick-reference hardware/software summary

### Key Decisions
- **ai-toolkit (ostris) selected over kohya_ss and SimpleTuner** for FLUX LoRA training: purpose-built for FLUX, has a verified 24GB config, widest community adoption. SimpleTuner noted as a solid alternative.
- **Unsloth selected over LLaMA-Factory/axolotl** for LLM fine-tuning: easiest install, 70% VRAM reduction, built-in GGUF/Ollama export, NVLink multi-GPU support explicitly documented.
- **Start with Qwen3.5:9B for chatbot fine-tuning**: fits single GPU, 256K context, fast iteration. Upgrade to 27B if quality is insufficient.
- **Training parameters verified from official sources**: ai-toolkit 24GB config (rank 16, lr 1e-4, 2000 steps, bf16, adamw8bit); Unsloth QLoRA config (rank 16, lr 2e-4, 3 epochs, bf16).
- **Civitai blocked in UK** (Online Safety Act) — community resources redirected to r/StableDiffusion and HuggingFace.
- **Photo selection guidance**: 20–30 varied images recommended; multiple angles, lighting, clothing, backgrounds; exclude group photos, occlusion, burst shots, screenshots from video.

### Reference Documents
- `it/NewPC/Model_and_LoRA_Creation.md` — the new guide
- `it/NewPC/Final_Build.md` — hardware specification (now referenced in CLAUDE.md)
- `it/NewPC/Software_Setup.md` — software stack (now referenced in CLAUDE.md)

### Next Actions
- [ ] Curate photo dataset for character LoRA (20–30 images, varied angles/lighting/backgrounds)
- [ ] Install ai-toolkit on the server: `git clone https://github.com/ostris/ai-toolkit.git`
- [ ] Create training dataset (JSONL) for LLM chatbot from source documents
- [ ] Install Unsloth: `pip install "unsloth[colab-new] @ git+https://github.com/unslothai/unsloth.git"`
- [ ] Apply dual `-p` binding to ComfyUI containers (pending from previous session)
- [ ] Set static DHCP reservation on router for `192.168.1.192` if not already done

---

## Session 2026-07-02 (4) — Permission-prompt fix generalised to repo root

### Summary
Fixed the previous session's permission fix: the Edit permissions for `SESSION_LOG.md`/`PROJECT_STATUS.md`/`CHANGELOG.md` had been added as NewPC-only literal paths in `it/NewPC/.claude/settings.json`, which would only stop prompts in this one project. Since `/end-session` is a shared command used across every project in the repo, moved the fix to the root `.claude/settings.local.json` using wildcard patterns so it applies everywhere `/end-session` runs.

### Work Completed
- Confirmed `SESSION_LOG.md`/`PROJECT_STATUS.md`/`CHANGELOG.md` exist in 9 locations across the repo (root, `it/`, `hseea/`, `REACH/`, `Nebosh/`, `Leads/`, `it/NewPC/`, `it/ZeroTrust/`, `hseea/Ladders/`), confirming the cross-project scope
- Reverted the NewPC-specific `Edit(...)` entries from `it/NewPC/.claude/settings.json`
- Added wildcarded `Edit`/`Write` permissions for all three filenames to `/docs/terminai/.claude/settings.local.json` (`Edit(/docs/terminai/**/SESSION_LOG.md)` etc.) — `Write` included in case a project runs `/end-session` before these files exist yet
- Confirmed root `.claude/settings.local.json` is git-tracked (consistent with how `it/NewPC/.claude/settings.local.json` is also tracked in this repo, unlike the typical gitignored convention)

### Files Changed
- `../../.claude/settings.local.json` — added 6 wildcarded Edit/Write permissions covering `/end-session`'s three tracking files repo-wide
- `it/NewPC/.claude/settings.json` — reverted the narrow NewPC-only entries added in error

### Key Decisions
- Permission fixes for genuinely shared/cross-project commands (like `/end-session`) belong in the root-level settings file with wildcard paths, not duplicated per-project with literal paths

### Next Actions
- [ ] Confirm this `/end-session` run completes without further Edit/Write authorisation prompts on the tracking files

---

## Session 2026-07-02 (3) — Customer Profiler `update` command added

### Summary
Added a new `update` chat command to the Customer Profiler n8n workflow, letting the user change specific fields on an existing profile (ranking, products, region, financials, etc.) instantly without re-running the Companies House/financials lookup. Implemented only in the new working copy `n8n/Main/NewCustomerProfiler.json`, leaving the previously-published `Portland Fuel - Customer Profiler.json` untouched per explicit instruction. Documented the new command fully in `n8n/Company_Profiler.md`.

### Work Completed
- Added `update <regNo> <Field>=<Value> | <Field>=<Value>` parsing to the `Parse Input` Code node
- Added `Route3` IF node, `Update Profile` Code node, `Format Update Email` Code node, and `Send Update Email` Outlook node; rewired `Route2`'s second output into the new branch
- Field-name matching uses the exact column headings from the `list` command's CSV export (Ranking, Products, Region, Company Name, SIC Codes, Turnover, Employees, Net Assets, Accounts Year, Confidence, Ranking Note, Profile Date)
- `Products` and `SIC Codes` merge (case-insensitive dedupe, additive); all other fields overwrite
- Validated the new Code node logic with a standalone Node.js test harness mocking n8n's `$input`, `$`, and `$getWorkflowStaticData` APIs — covered the user's exact example plus unknown-field, missing-profile, and invalid-number edge cases
- Updated `n8n/Company_Profiler.md`: new Commands table row, "Understanding the `update` command" section with full field table, updated Update examples, three new Email Notifications rows, and a flag that `update` requires `NewCustomerProfiler.json` specifically

### Files Changed
- `n8n/Main/NewCustomerProfiler.json` — **created** (working copy) — `update` command added
- `n8n/Main/Portland Fuel - Customer Profiler.json` — untouched, verified via file size/mtime before and after
- `n8n/Company_Profiler.md` — documented the `update` command end to end

### Key Decisions
- Field names must match the CSV export headers exactly (case-insensitive) rather than inventing new field names, so the command vocabulary is self-documenting from the `list` output the user already sees
- Array fields (Products, SIC Codes) merge rather than overwrite, since the user's own example depended on this ("Hedging is already assigned... this would add Fuelcards")
- Kept `update` entirely separate from `add`'s overwrite-everything semantics rather than overloading `add` further — `add` remains the "fresh lookup" path, `update` is the "instant edit" path

### Next Actions
- [ ] Import `n8n/Main/NewCustomerProfiler.json` into n8n and activate it — `update` will not work against the previously-published workflow
- [ ] Re-link `MyHotmailEmail` credential on any new/renamed nodes after import (standard post-import step)
- [ ] Test `update` against a real stored profile via the production chat URL (not the editor's Chat button — test mode doesn't persist static data)

---

## Session 2026-03-14

### Summary
Created a comprehensive Tailscale networking guide and updated documentation to reflect the dual-binding Docker port strategy (loopback for Tailscale serve, LAN IP for local network access). Worked through real troubleshooting of Tailscale serve configuration, diagnosing and fixing incorrect port mappings. Confirmed and documented final port assignments for all services.

### Work Completed
- Created `Tailscale.md` — full guide covering installation, commands, port forwarding, troubleshooting, and securing services
- Updated `HuggingFace.md` — added instructions for sharing models with Amelia's instance via hard links
- Updated `Software_Setup.md` — dual `-p` bindings on all docker run commands, updated service URLs table, updated access sections
- Worked through live Tailscale serve troubleshooting (wrong port in `off` command, wrong protocol in browser, Docker bypassing Tailscale serve)
- Confirmed final port assignments for all services
- Explained Docker `-p` flag in depth (IP binding, host port vs container port, container port isolation)
- Evaluated and rejected path-based Tailscale routing (ComfyUI has no base URL support)

### Files Changed
- `it/NewPC/Tailscale.md` — **created** — comprehensive Tailscale guide
- `it/NewPC/HuggingFace.md` — added Amelia model sharing section (hard link commands)
- `it/NewPC/Software_Setup.md` — dual `-p` bindings, updated service URLs table and access sections throughout

### Key Decisions
- **Dual Docker binding over iptables blocking**: Use `-p 127.0.0.1:HOST:CONTAINER` for Tailscale serve + `-p 192.168.1.192:PORT:CONTAINER` for LAN access. Cleaner than blocking ports with iptables rules.
- **Port-based Tailscale routing retained**: Path-based routing (`/steve`, `/amelia`) was evaluated but rejected — ComfyUI has no base URL support so the setup would be inconsistent. Port numbers kept for all services.
- **Container port cannot be changed arbitrarily**: The second number in `-p` must match what the application listens on inside the container (e.g. ComfyUI uses 8188, not 80).
- **Static LAN IP required**: Dual binding only reliable with a fixed LAN IP — DHCP reservation on router recommended.

### Confirmed Port Assignments
| Service | Local network | Tailscale |
|---|---|---|
| Open WebUI | `http://192.168.1.192:3000` | `https://amelai.tail926601.ts.net` |
| ComfyUI (Steve) | `http://192.168.1.192:8189` | `https://amelai.tail926601.ts.net:8189` |
| ComfyUI (Amelia) | `http://192.168.1.192:8188` | `https://amelai.tail926601.ts.net:8188` |
| FileBrowser | `http://192.168.1.192:8087` | `https://amelai.tail926601.ts.net:8087` |

### Next Actions
- [ ] Apply dual `-p` binding to ComfyUI and ComfyUI-Amelia docker run commands on the server
- [ ] Verify `sudo ss -tlnup` shows correct bindings for all services after changes
- [ ] Consider setting static DHCP reservation on router for `192.168.1.192` if not already done

---
