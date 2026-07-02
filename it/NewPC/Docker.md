# Docker Administration Guide

**System**: Ubuntu 24.04 LTS Server · `amelai` (192.168.1.192)

---

## What is Docker?

Docker is a **containerisation platform** — it packages an application and all its dependencies into an isolated unit called a **container**. Containers behave consistently regardless of what else is installed on the host system, which is why it's used here for services like ComfyUI, Open WebUI, and SearXNG.

### Key concepts

| Concept | What it is |
|---|---|
| **Image** | A read-only template — the blueprint for a container. Downloaded from a registry (e.g. Docker Hub, ghcr.io). |
| **Container** | A running instance of an image. You can have multiple containers from the same image. |
| **Volume** | Persistent storage that survives container deletion. Used to keep data (model files, databases, outputs) separate from the container itself. |
| **Network** | A virtual network connecting containers. Containers on the same network can reach each other by name (e.g. `http://searxng:8080`). |
| **Registry** | A repository of images. Docker Hub is the default; this server also uses `ghcr.io` (GitHub) and `yanwk`. |

---

## Docker Compose (Primary Method)

**Use this instead of typing individual `docker run` commands.** All 7 services are defined once in `docker-compose.yml` (same folder as this file). This exists because containers recreated by hand with an incomplete `docker run` command silently lose flags — that's what caused the n8n, SearXNG, and pdf-to-image outages on 2026-07-01 (port bindings and the encryption key were dropped with no error). Compose reads the same file every time, so recreation is always complete.

### One-time setup

```bash
cd /docs/terminai/it/NewPC
cp secrets/openwebui.env.example secrets/openwebui.env
cp secrets/n8n.env.example secrets/n8n.env
nano secrets/openwebui.env   # fill in the real PGVECTOR_DB_URL (with password)
nano secrets/n8n.env         # fill in the real N8N_ENCRYPTION_KEY
```

`secrets/*.env` are gitignored and wired into their services via `env_file:` in `docker-compose.yml`.

**Any secret with a literal `$` will get silently corrupted — this affects `env_file:` too, not just a top-level `.env`.** Confirmed 2026-07-02: Compose's variable interpolation runs on values from *both* the top-level `.env` and per-service `env_file:` files (this repo initially assumed `env_file:` was exempt — it is not, at least on the Compose v2.x installed here). A literal `$` gets treated as the start of a variable reference and blanked out, with only an easy-to-miss `docker compose config` warning as a symptom — this took down Open WebUI's Postgres connection on first migration.

**If a secret is embedded in a URL** (e.g. `PGVECTOR_DB_URL`), percent-encode any special characters in it — `$` becomes `%24` — using `urllib.parse.quote(password, safe="")` in Python. libpq/psycopg2 decode this back to the real password correctly; Compose never sees a literal `$` to misinterpret. This is the fix currently in place for `secrets/openwebui.env`.

**If a secret is NOT part of a URL** (e.g. `N8N_ENCRYPTION_KEY`), the simplest safe option is to avoid `$` in the value entirely when it's generated/rotated.

**After changing any secret, always verify it round-tripped correctly — don't assume:**
```bash
cd /docs/terminai/it/NewPC
docker compose config --quiet   # must show no "variable not set" warnings
# Compare the container's actual value against the source file, without ever printing the secret:
docker exec <container> printenv <VAR_NAME> | sha256sum
cat secrets/<file>.env | cut -d= -f2- | sha256sum   # must match exactly
```

Also confirm the host prerequisites still exist (Compose does not create these for you — a missing bind-mount **file** gets auto-created as a **directory** by Docker, which breaks these two services):

```bash
mkdir -p /home/steve/rag-documents
mkdir -p /home/steve/filebrowser && touch /home/steve/filebrowser/filebrowser.db
```

### Migrating existing containers to Compose (one at a time)

Do this per-service, not all at once, so you can verify each one before moving to the next. Data is safe — `open-webui` and `n8n_data` are existing named volumes (marked `external: true` in the compose file, so Compose attaches to them rather than creating new empty ones), and every other service uses bind mounts to host folders.

```bash
# Example: migrating n8n
docker stop n8n
docker rm n8n
cd /docs/terminai/it/NewPC
docker compose up -d n8n

# Verify it came up correctly
docker ps --filter name=n8n
docker logs n8n --tail 50
```

Repeat for each service: `open-webui`, `comfyui`, `comfyui-amelia`, `filebrowser`, `searxng`, `n8n`, `pdf-to-image`.

### Day-to-day commands

```bash
cd /docs/terminai/it/NewPC

docker compose up -d              # (re)create every service exactly as defined
docker compose up -d n8n          # (re)create just one service
docker compose down               # stop and remove all compose-managed containers (volumes untouched)
docker compose ps                 # status of all services
docker compose logs -f n8n        # follow logs for one service
docker compose pull               # pull latest images for all services
docker compose up -d --build pdf-to-image   # rebuild pdf-to-image from its Dockerfile after editing app.py
```

`docker compose up -d` is now the correct way to recover from any container-lost-its-config situation — it always applies the full definition from the file, so a flag can no longer be silently dropped.

### Updating `docker-compose.yml`

If a service's configuration needs to change (new port, new environment variable, new volume), edit `docker-compose.yml` directly and re-run `docker compose up -d <service>` to apply it. Per the project convention below, also keep the corresponding entry in the **Service docker run Commands** section (further down this file) in sync — it's kept as a reference for understanding what each flag does and as a fallback if Compose is ever unavailable.

---

## To Re-Create Running Container `Run` Commands

If you need to reconstruct the `docker run` command for a container that is already running (e.g. inherited a container with no docs, or want to verify a container matches what's documented), use **`runlike`**:

```bash
pip install runlike
runlike <container_name>
```

Examples:

```bash
runlike comfyui
runlike comfyui-amelia
runlike filebrowser
```

It inspects the running container and outputs a clean, copy-pasteable `docker run` command with all flags, volumes, environment variables, and port bindings reconstructed.

**Without installing** (runs via Docker itself):

```bash
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock assaflavie/runlike <container_name>
```

> The exact run commands for all services on this server are documented in the **Service docker run Commands** section below — `runlike` is most useful when inheriting an undocumented container or verifying the current state of a running one.

---

## Common Commands

### Viewing containers

```bash
docker ps                          # List running containers
docker ps -a                       # List all containers (including stopped)
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"  # Compact view
```

### Starting and stopping

```bash
docker start <name>                # Start a stopped container
docker stop <name>                 # Gracefully stop a container
docker restart <name>              # Stop then start (useful after config changes)
docker kill <name>                 # Force-kill immediately (use only if stop hangs)
```

### Logs

```bash
docker logs <name>                 # Show all logs
docker logs <name> --tail 50       # Show last 50 lines
docker logs <name> -f              # Follow logs in real time (Ctrl+C to exit)
docker logs <name> --tail 50 -f    # Last 50 lines, then follow
```

Use `docker logs -f` when troubleshooting a service that won't start — watch it as it boots.

### Inspecting a container

```bash
docker inspect <name>                                          # Full JSON details
docker inspect <name> --format='{{.HostConfig.RestartPolicy.Name}}'  # Check restart policy
docker inspect <name> --format='{{json .HostConfig}}' | python3 -m json.tool  # Pretty-print host config
docker stats                                                   # Live CPU/RAM/VRAM usage for all containers
docker stats <name>                                            # Live stats for one container
```

### Running commands inside a container

```bash
docker exec -it <name> bash        # Open an interactive shell inside the container
docker exec <name> <command>       # Run a single command and exit
```

**Example** — check what's in ComfyUI's model directory:
```bash
docker exec comfyui ls /root/ComfyUI/models/loras/
```

### Networks

```bash
docker network ls                                   # List all networks
docker network inspect ai-network                   # Show containers on ai-network
docker network connect ai-network <container>       # Add a container to a network
docker network disconnect ai-network <container>    # Remove from a network
```

### Images

```bash
docker images                      # List downloaded images
docker pull <image>                # Download the latest version of an image
docker rmi <image>                 # Remove an image (container must be deleted first)
docker image prune                 # Remove all unused (dangerless) images
```

### Updating a container

Docker containers are immutable — to update, you delete and recreate them. Your data is safe because it lives in volumes, not inside the container.

```bash
docker stop <name>
docker rm <name>
docker pull <image>:<tag>          # Pull the new image version
# Then re-run the original docker run command (see service-specific sections below)
```

### Volumes

```bash
docker volume ls                   # List all volumes
docker volume inspect <name>       # Show where a volume is stored on the host
docker volume rm <name>            # Delete a volume (DESTRUCTIVE — deletes all data in it)
```

---

## This Server's Services

### Network

All services on this server (except Open WebUI) share the `ai-network` Docker network. This allows containers to reach each other by name (e.g. Open WebUI calls SearXNG at `http://searxng:8080` rather than a host IP).

```bash
docker network ls | grep ai
docker network inspect ai-network
```

---

## Service docker run Commands

> **Reference only.** `docker-compose.yml` (see **Docker Compose (Primary Method)** above) is now the primary way to (re)create these containers — use `docker compose up -d <service>` instead of copy-pasting from here. These commands are kept so each flag's purpose stays documented and as a manual fallback if Compose is ever unavailable.

These are the exact commands to recreate each service if its container is ever deleted. Run them from the server as `steve`.

---

### Open WebUI

Browser-based chat interface for Ollama models, with pgvector RAG support.

```bash
# Set pgvector password (single quotes prevent bash special char issues with ! and $)
PGPASS='your_postgresql_password'

docker run -d \
  --name open-webui \
  --network ai-network \
  --restart always \
  --gpus all \
  -p 127.0.0.1:3000:8080 \
  -p 192.168.1.192:3000:8080 \
  -v open-webui:/app/backend/data \
  -v /home/steve/rag-documents:/app/backend/data/uploads \
  -e OLLAMA_BASE_URL=http://192.168.1.192:11434 \
  -e VECTOR_DB=pgvector \
  -e PGVECTOR_DB_URL=postgresql://openwebui:${PGPASS}@192.168.1.192:5432/openwebui_vectors \
  -e RAG_EMBEDDING_ENGINE=ollama \
  -e RAG_EMBEDDING_MODEL=nomic-embed-text \
  ghcr.io/open-webui/open-webui:main
```

> **Pre-requisite**: `mkdir -p /home/steve/rag-documents` must exist before running (Docker will create it as root if missing, causing permission issues).

| Option | Purpose |
|---|---|
| `--network ai-network` | Connects to SearXNG for web search (in-network, no port needed) |
| `--restart always` | Auto-starts on server reboot |
| `--gpus all` | Gives container access to both RTX 3090s |
| `-p 127.0.0.1:3000:8080` | Loopback binding — Tailscale Serve proxies through here |
| `-p 192.168.1.192:3000:8080` | LAN binding — direct local network access |
| `-v open-webui:/app/backend/data` | Named volume — persists chat history, users, settings |
| `-v /home/steve/rag-documents:...` | Bind mount — saves uploaded documents to host for backup |
| `OLLAMA_BASE_URL` | Points to Ollama on the host — must use the LAN IP, not `localhost` |
| `VECTOR_DB=pgvector` | Switches RAG storage from default ChromaDB to PostgreSQL |
| `PGVECTOR_DB_URL` | PostgreSQL connection string for the vector database |
| `RAG_EMBEDDING_ENGINE=ollama` | Use local Ollama to generate text embeddings |
| `RAG_EMBEDDING_MODEL=nomic-embed-text` | Embedding model — must match the model used when documents were ingested |

**Access**: `http://192.168.1.192:3000` (LAN) · `https://amelai.tail926601.ts.net` (Tailscale)

---

### ComfyUI — Steve's instance

Image/video generation, GPU 1.

```bash
docker run -d \
  --name comfyui \
  --network ai-network \
  --restart unless-stopped \
  --runtime nvidia \
  --gpus all \
  -p 127.0.0.1:18189:8188 \
  -e CUDA_VISIBLE_DEVICES=1 \
  -e CLI_ARGS="--disable-xformers --reserve-vram 3" \
  -e PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True \
  -v /mnt/models/comfyui:/root/ComfyUI/models \
  -v /opt/comfyui/storage:/root \
  -v /opt/comfyui/input:/root/ComfyUI/input \
  -v "/docs/Projects/Claude Code Shared/Output:/root/ComfyUI/output" \
  -v "/docs/Projects/Claude Code Shared/Workflows:/root/ComfyUI/user/default/workflows" \
  yanwk/comfyui-boot:cu128-slim
```

| Option | Purpose |
|---|---|
| `CUDA_VISIBLE_DEVICES=1` | Restricts this container to GPU 1 only; prevents OOM errors from large models |
| `--reserve-vram 3` | Keeps 3 GB free at all times — prevents every-other-generation OOM with large models |
| `-v /mnt/models/comfyui` | Model files on second drive — shared with host, survive container rebuild |
| `--runtime nvidia` | Enables NVIDIA GPU access via the container toolkit |

**Access**: `https://amelai.tail926601.ts.net:8189` (Tailscale only — no LAN binding)

#### Accessing ComfyUI files from SSH

The `-v` flags are **bind mounts** — they map a folder inside the container directly to a folder on the host. This means the files are accessible from any SSH session at the host path, with no special commands needed:

```bash
# List generated images
ls /opt/comfyui/output/

# Copy a generated image to your home directory
cp /opt/comfyui/output/ComfyUI_00042_.png ~/

# See what's in the input folder
ls /opt/comfyui/input/

# See all volume paths at a glance
ls /opt/comfyui/
```

The same applies to Amelia's instance — just replace `/opt/comfyui/` with `/opt/comfyui-amelia/`:

```bash
ls /opt/comfyui-amelia/output/
```

To download a file to your Windows machine, use `scp` from a Windows terminal (not the server):

```bash
scp steve@192.168.1.192:/opt/comfyui/output/ComfyUI_00042_.png C:\Users\SteveIrwin\Downloads\
```

Or use FileBrowser — add the output folder as an additional volume mount to make it browsable in the web UI.

---

### ComfyUI — Amelia's instance

Restricted model access, GPU 0.

```bash
docker run -d \
  --name comfyui-amelia \
  --network ai-network \
  --restart unless-stopped \
  --runtime nvidia \
  --gpus all \
  -p 127.0.0.1:18188:8188 \
  -p 192.168.1.192:8188:8188 \
  -v /mnt/models/comfyui-amelia:/root/ComfyUI/models \
  -v /opt/comfyui-amelia/storage:/root \
  -v /opt/comfyui-amelia/input:/root/ComfyUI/input \
  -v /opt/comfyui-amelia/output:/root/ComfyUI/output \
  -v /opt/comfyui-amelia/workflows:/root/ComfyUI/user/default/workflows \
  -e CUDA_VISIBLE_DEVICES=0 \
  -e CLI_ARGS="--disable-xformers" \
  -e PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True \
  yanwk/comfyui-boot:cu128-slim
```

| Option | Purpose |
|---|---|
| `CUDA_VISIBLE_DEVICES=0` | Restricts to GPU 0 — Amelia's GPU |
| `-v /mnt/models/comfyui-amelia` | Separate model directory — only contains models explicitly shared with her |

**Access**: `http://192.168.1.192:8188` (LAN) · `https://amelai.tail926601.ts.net:8188` (Tailscale)

> To add a model to Amelia's instance without using extra disk space:
> `sudo ln /mnt/models/comfyui/loras/<file> /mnt/models/comfyui-amelia/loras/<file>`

---

### FileBrowser

Web-based file manager. Serves `/home/steve/rag-output/` plus ComfyUI output folders.

```bash
# Pre-create the database file first (only needed once — must be a file, not a directory)
mkdir -p /home/steve/filebrowser
touch /home/steve/filebrowser/filebrowser.db

docker run -d \
  --name filebrowser \
  --restart always \
  --network ai-network \
  -p 127.0.0.1:18087:80 \
  -p 192.168.1.192:8087:80 \
  -v /home/steve/rag-output:/srv \
  -v "/docs/Projects/Claude Code Shared/Output:/srv/comfyui-output" \
  -v /opt/comfyui/workflows:/srv/comfyui-workflows \
  -v /opt/comfyui-amelia/output:/srv/comfyui-amelia-output \
  -v /home/steve/filebrowser/filebrowser.db:/database/filebrowser.db \
  filebrowser/filebrowser:latest
```

**Then expose via Tailscale Serve** (if not already configured):

```bash
tailscale serve --bg --https 8087 http://localhost:18087
```

| Option | Purpose |
|---|---|
| `-v /home/steve/rag-output:/srv` | Root directory shown in the browser |
| `-v /opt/comfyui/output:/srv/comfyui-output` | Steve's ComfyUI generated images |
| `-v /opt/comfyui-amelia/output:/srv/comfyui-amelia-output` | Amelia's ComfyUI generated images |
| `-v .../filebrowser.db` | Persistent database — users, settings, bookmarks |
| `-p 127.0.0.1:18087:80` | Loopback on 18087; Tailscale Serve forwards `8087` → `18087` |
| `-p 192.168.1.192:8087:80` | LAN access on port 8087 |

**First login**: `admin` / `admin` — change password immediately in Settings → User Management.

**Access**: `http://192.168.1.192:8087` (LAN) · `https://amelai.tail926601.ts.net:8087` (Tailscale)

---

### SearXNG

Self-hosted meta-search engine, used by Open WebUI for web search.

```bash
docker run -d \
  --name searxng \
  --restart unless-stopped \
  --network ai-network \
  -p 127.0.0.1:18080:8080 \
  -p 192.168.1.192:8080:8080 \
  -v /opt/searxng:/etc/searxng:rw \
  searxng/searxng:latest
```

| Option | Purpose |
|---|---|
| `-p 127.0.0.1:18080:8080` | Loopback — Tailscale serve proxies `https://amelai.tail926601.ts.net:8080` here |
| `-p 192.168.1.192:8080:8080` | LAN IP — local network access at `http://192.168.1.192:8080` |
| `-v /opt/searxng:/etc/searxng` | Persistent config, including `settings.yml` |
| `--network ai-network` | Allows Open WebUI to reach it at `http://searxng:8080` |

**After recreating**, verify Open WebUI can still reach it:

```bash
docker exec open-webui curl -s "http://searxng:8080/search?q=test&format=json" | head -c 200
```

> **Config note**: `/opt/searxng/settings.yml` must contain `formats: [html, json]` under the `search:` section, otherwise Open WebUI will get HTML responses instead of JSON and web search will fail. Restart after any config change: `docker restart searxng`.

---

### n8n

Workflow automation — connects services, triggers on schedules or webhooks.

```bash
docker run -d \
  --name n8n \
  --restart unless-stopped \
  -p 127.0.0.1:15678:5678 \
  -p 192.168.1.192:5678:5678 \
  -v n8n_data:/home/node/.n8n \
  -e GENERIC_TIMEZONE=Europe/London \
  -e N8N_HOST=amelai.tail926601.ts.net \
  -e N8N_PORT=5678 \
  -e N8N_PROTOCOL=https \
  -e WEBHOOK_URL=https://amelai.tail926601.ts.net:5678 \
  -e N8N_ENCRYPTION_KEY=<YOUR_ENCRYPTION_KEY> \
  -e N8N_SECURE_COOKIE=false \
  n8nio/n8n
```

> `N8N_ENCRYPTION_KEY` must match the key used when the container was first created. If the key changes, stored credentials become unreadable. See `n8n/N8N_Setup.md` for key management and backup.

| Option | Purpose |
|---|---|
| `-v n8n_data:/home/node/.n8n` | Named volume — workflows, credentials, settings |
| `N8N_ENCRYPTION_KEY` | Encrypts stored credentials — must be preserved across recreations |
| `N8N_SECURE_COOKIE=false` | Required when Tailscale terminates TLS (container sees plain HTTP) |
| `N8N_HOST` / `WEBHOOK_URL` | Ensures webhook URLs generated in the UI point to the Tailscale address |

**No GPU dependency** — managed entirely by Docker's `--restart unless-stopped` policy; does not need the nvidia-wait systemd service.

**Access**: `http://192.168.1.192:5678` (LAN) · `https://amelai.tail926601.ts.net:5678` (Tailscale)

### pdf-to-image

Converts PDF pages to base64-encoded PNG images for OCR/vision model processing. Used by the n8n Customer Profiler workflow to extract financial data from image-based Companies House accounts PDFs. Internal service only — not exposed on Tailscale.

Build from source (files in `n8n/pdf-to-image/` in this repo — see `Temp.txt` for the exact commands):

```bash
docker build -t pdf-to-image /docs/terminai/it/NewPC/n8n/pdf-to-image/

docker run -d \
  --name pdf-to-image \
  --restart unless-stopped \
  -p 127.0.0.1:18086:8086 \
  -p 192.168.1.192:8086:8086 \
  pdf-to-image
```

**Access**: `http://192.168.1.192:8086` (LAN only — internal use by n8n)
**Health check**: `curl http://192.168.1.192:8086/health`

---

## Port Map — All Services

| Service | Container | Container port | Loopback host port | LAN/Tailscale port |
|---|---|---|---|---|
| Open WebUI | `open-webui` | 8080 | 3000 | 3000 |
| ComfyUI (Steve) | `comfyui` | 8188 | 18189 | 8189 |
| ComfyUI (Amelia) | `comfyui-amelia` | 8188 | 18188 | 8188 |
| FileBrowser | `filebrowser` | 80 | 18087 | 8087 |
| SearXNG | `searxng` | 8080 | 18080 | 8080 |
| n8n | `n8n` | 5678 | 15678 | 5678 |
| pdf-to-image | `pdf-to-image` | 8086 | 18086 | LAN only |

---

## Stopping Services Before AI Training

Before running LoRA training or any GPU-intensive task outside Docker, stop all containers to free VRAM:

```bash
docker stop comfyui comfyui-amelia
sudo systemctl stop ollama
```

To bring them back after training:

```bash
docker start comfyui comfyui-amelia
sudo systemctl start ollama
```

> Open WebUI and SearXNG do not need to be stopped — they use little VRAM. Only ComfyUI containers and Ollama need to be stopped.

---

## Tailscale Serve Config

Tailscale Serve proxies HTTPS access from outside into the containers. To rebuild the full config from scratch (e.g. after `sudo tailscale serve reset`):

```bash
sudo tailscale serve --bg --https=443 http://localhost:3000    # Open WebUI
sudo tailscale serve --bg --https=8087 http://localhost:18087  # FileBrowser
sudo tailscale serve --bg --https=8188 http://localhost:18188  # ComfyUI (Amelia)
sudo tailscale serve --bg --https=8189 http://localhost:18189  # ComfyUI (Steve)
sudo tailscale serve --bg --https=5678 http://localhost:15678  # n8n
```

Verify: `sudo tailscale serve status`

---

## Quick Reference

```bash
# View all containers
docker ps -a

# Logs (follow)
docker logs -f <name>

# Shell into a container
docker exec -it <name> bash

# Stop / start / restart
docker stop <name>
docker start <name>
docker restart <name>

# Update a container (stop → delete → pull → re-run)
docker stop <name> && docker rm <name>
docker pull <image>
# Re-run docker run command from above

# Check GPU usage per container
docker stats

# Check container's network connections
docker inspect <name> --format='{{json .NetworkSettings.Networks}}' | python3 -m json.tool

# Check restart policy
docker inspect <name> --format='{{.HostConfig.RestartPolicy.Name}}'

# List volumes
docker volume ls

# List networks
docker network ls
```
