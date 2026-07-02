<img src="../../Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# Docker Compose Reference — amelai Services

This is the command reference for starting, recreating, and managing amelai's Docker services via `docker-compose.yml`. It replaces the old approach of retyping individual `docker run` commands from memory or from `Software_Updates.md`.

For **why** Compose replaced the old approach, and the full migration story (including a real password-corruption bug encountered along the way), see the **"Docker Compose (Primary Method)"** section of `Docker.md`. This file is the practical command reference; `Docker.md` has the background and the incident write-up.

---

## Where everything lives

| File | Purpose |
|---|---|
| `docker-compose.yml` | The single definition for all 7 services — the "recipe" Compose reads |
| `secrets/openwebui.env` | Real Postgres connection string (gitignored) |
| `secrets/n8n.env` | Real n8n encryption key (gitignored) |
| `secrets/*.env.example` | Templates for the two files above (tracked in git) |

All commands below assume you are in `/docs/terminai/it/NewPC` (where `docker-compose.yml` lives):

```bash
cd /docs/terminai/it/NewPC
```

---

## One-time setup (new machine, or after cloning fresh)

```bash
cp secrets/openwebui.env.example secrets/openwebui.env
cp secrets/n8n.env.example secrets/n8n.env
nano secrets/openwebui.env   # fill in the real PGVECTOR_DB_URL
nano secrets/n8n.env         # fill in the real N8N_ENCRYPTION_KEY
```

> **If either secret contains a `$` character**, read the "Secrets containing special characters" section below before filling these in — Compose corrupts a literal `$` in these files.

Also confirm these host paths exist — Compose does not create them, and a missing bind-mount **file** gets auto-created as a **directory** by Docker, which breaks these two services:

```bash
mkdir -p /home/steve/rag-documents
mkdir -p /home/steve/filebrowser && touch /home/steve/filebrowser/filebrowser.db
```

---

## Starting everything at once

```bash
docker compose up -d
```

Creates (or recreates, if already running with a matching definition) all 7 services in one pass.

---

## Starting or recreating one service

Each service can be brought up independently by name. This is the command actually used, per service, during the initial migration from standalone `docker run` containers:

| Service | Command |
|---|---|
| Open WebUI | `docker compose up -d open-webui` |
| ComfyUI (Steve) | `docker compose up -d comfyui` |
| ComfyUI (Amelia) | `docker compose up -d comfyui-amelia` |
| FileBrowser | `docker compose up -d filebrowser` |
| SearXNG | `docker compose up -d searxng` |
| n8n | `docker compose up -d n8n` |
| pdf-to-image | `docker compose up -d --build pdf-to-image` |

`pdf-to-image` is the one exception — it's built from a local `Dockerfile` (`n8n/pdf-to-image/`) rather than pulled from a registry, so `--build` is needed the first time and any time `app.py`/`requirements.txt`/`Dockerfile` change. The other 6 services pull their image automatically if not already present locally.

### Migrating a container still running the old (manual `docker run`) way

If a container currently exists outside of Compose (created by hand, or inherited), Compose will refuse to reuse its name. Stop and remove it first, then bring it up via Compose — this is exactly the sequence used for all 7 services during the original migration:

```bash
docker stop <service-name>
docker rm <service-name>
docker compose up -d <service-name>
```

Data is not at risk doing this: `open-webui` and `n8n_data` are pre-existing named Docker volumes (declared `external: true` in `docker-compose.yml`, so Compose attaches to them rather than creating new empty ones), and every other service uses bind mounts to host folders that are untouched by removing a container.

---

## Verifying a service came up correctly

These are the checks used to confirm each service migrated cleanly:

```bash
# Container status and ports
docker ps --filter name=<service-name> --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

# Logs — watch for startup errors
docker compose logs -f <service-name>

# GPU-enabled services (open-webui, comfyui, comfyui-amelia) — confirm the GPU is visible
docker exec <service-name> nvidia-smi --query-gpu=index,name,memory.used --format=csv

# n8n — confirm no credential decryption errors and workflows re-activated
docker logs n8n 2>&1 | grep -i "error\|decrypt\|credential"
# (no output = no problems)

# Service-specific health/API endpoints
curl http://192.168.1.192:8086/health          # pdf-to-image
curl http://192.168.1.192:5678/healthz         # n8n
curl http://192.168.1.192:3000/health          # open-webui
curl "http://192.168.1.192:8080/search?q=test&format=json"   # searxng
```

---

## Day-to-day commands

```bash
docker compose ps                       # status of all services
docker compose logs -f <service>        # follow logs for one service
docker compose pull                     # pull latest images for all pullable services
docker compose pull <service>           # pull latest image for one service
docker compose up -d                    # apply docker-compose.yml to every service (recreate if changed)
docker compose up -d <service>          # apply to just one service
docker compose up -d --build pdf-to-image   # rebuild pdf-to-image from its Dockerfile
docker compose restart <service>        # restart without recreating
docker compose down                     # stop and remove all compose-managed containers (volumes untouched)
docker compose config --quiet           # validate the compose file; must show no warnings
```

**Updating an image version** (equivalent of the old "stop, rm, pull, re-run" cycle in `Software_Updates.md`):

```bash
docker compose pull <service>
docker compose up -d <service>
```

Compose recreates the container only if the pulled image or the service definition actually changed — if nothing changed, `up -d` is a safe no-op.

---

## Secrets containing special characters

**Confirmed on this system (2026-07-02):** Compose's variable interpolation runs on values loaded from *both* the top-level `.env` file and per-service `env_file:` files. A literal `$` anywhere in a secret is treated as the start of a variable reference (e.g. `$Ioz6...` is read as "the variable named `Ioz6`") and silently replaced with an empty string — the only symptom is an easy-to-miss `docker compose config` warning like:

```
level=warning msg="The \"Ioz6\" variable is not set. Defaulting to a blank string."
```

This corrupted the Postgres password in `PGVECTOR_DB_URL` during the initial migration and briefly broke Open WebUI's database connection in production.

**The fix used here** — for a secret embedded inside a URL (like `PGVECTOR_DB_URL`), percent-encode the special character so Compose never sees a literal `$`:

```python
import urllib.parse
encoded_password = urllib.parse.quote(real_password, safe="")
# $ becomes %24 — libpq/psycopg2 decode this back to the real password correctly
```

For a secret that is *not* part of a URL (like `N8N_ENCRYPTION_KEY`), the simplest safe option is to avoid `$` in the value when it's generated or rotated.

**After changing any secret, always verify it round-tripped correctly — do not assume:**

```bash
docker compose config --quiet          # must show no "variable not set" warnings

# Compare the container's actual value against the source file, without ever printing the secret:
docker exec <container> printenv <VAR_NAME> | sha256sum
cat secrets/<file>.env | cut -d= -f2- | sha256sum
# these two hashes must match exactly
```

If you need to test a connection string directly, read the file in a way that does **not** go through bash's own `$` expansion (e.g. `source file.env` will incorrectly expand `$` itself — a testing artifact, not a Compose bug). Use Python to read and pass the value instead:

```python
import subprocess
with open("secrets/openwebui.env") as f:
    val = f.read().strip().partition("=")[2]
subprocess.run(["psql", val, "-c", "SELECT 1;"])
```

---

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| `docker compose up -d <service>` errors with "container name already in use" | Container exists outside Compose's management | `docker stop <name> && docker rm <name>`, then `docker compose up -d <name>` |
| `docker compose config` shows a "variable not set" warning for a name that looks like part of a password | A secret in `secrets/*.env` contains a literal `$` | See "Secrets containing special characters" above |
| A service fails to connect to Postgres/another service after migrating | Secret got corrupted during interpolation | Recheck with the hash-comparison verification above; do not assume the file is correct just because you typed it correctly |
| GPU not visible inside a container | `deploy.resources.reservations.devices` block missing or NVIDIA Container Toolkit not configured | `docker exec <container> nvidia-smi` — if this fails, check `nvidia-container-toolkit` is installed on the host |
| pdf-to-image container doesn't pick up source changes | Image wasn't rebuilt | `docker compose up -d --build pdf-to-image` |
| Compose warning about an unrelated variable (e.g. `Ioz6`) that isn't in `docker-compose.yml` at all | Compose scans the entire shell environment for interpolation candidates, not just variables the file actually uses. On this system, Amelai's `~/.bashrc` exports a `PGPASS` (for the unrelated RAG MCP) that happens to contain a `$`, triggering a cosmetic warning | Harmless if nothing in `docker-compose.yml` actually references that variable — confirm with `grep '\$' docker-compose.yml` (comments only = safe) |
