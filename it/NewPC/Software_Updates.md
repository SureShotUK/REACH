# Software Updates Guide — AmelAI Server

This guide covers the update procedure for each application running on the AmelAI server. Follow the steps in order for each application.

> **Docker services now run via Docker Compose** (`docker-compose.yml`, see `Docker.md` → "Docker Compose (Primary Method)" and `DockerComposeDocs.md` for the full command reference). The `docker run` blocks below for each Docker-based service are **kept for reference only** — they show what each flag does, but are no longer the commands actually used to update or recreate a container. Use the `docker compose pull <service> && docker compose up -d <service>` pattern shown at the top of each Docker service section instead.

---

## Ollama

Ollama runs as a systemd service (not in Docker). Updating it replaces the binary but preserves all configuration.

```bash
# Stop the service
sudo systemctl stop ollama

# Download and run the installer
curl -fsSL https://ollama.com/install.sh | sh

# Reload systemd and ensure the service is enabled
sudo systemctl daemon-reload
sudo systemctl enable ollama

# Start the service (the installer may start it automatically)
sudo systemctl start ollama

# Verify it is running and check the version
sudo systemctl status ollama
ollama -v
```

> **Note**: Do not run `ollama serve` manually — the systemd service manages this. If you see `Error: listen tcp 127.0.0.1:11434: bind: address already in use`, the service is already running correctly.

**What is preserved**: All custom settings in `/etc/systemd/system/ollama.service.d/override.conf` (`OLLAMA_HOST`, `OLLAMA_MODELS`, `OLLAMA_MAX_LOADED_MODELS`) are not touched by the installer.

**Verify after update**:
```bash
cat /etc/systemd/system/ollama.service.d/override.conf
ollama ps
```

---

## Open WebUI

Open WebUI runs in Docker using the `:main` rolling release tag. Updating it requires stopping and removing the container, pulling the new image, and recreating the container. Chat history and settings are stored in a persistent Docker volume and are not affected.

**Current method (Docker Compose):**
```bash
cd /docs/terminai/it/NewPC
docker compose pull open-webui
docker compose up -d open-webui
```

> **OBSOLETE — kept for reference only.** The `docker run` command below is no longer how this container is recreated; `docker-compose.yml` is now authoritative. Shown here so each flag's purpose stays documented.

```bash
# Stop and remove the existing container
docker stop open-webui
docker rm open-webui

# Pull the latest image
docker pull ghcr.io/open-webui/open-webui:main

# Set pgvector password (single quotes prevent bash special char issues with ! and $)
PGPASS='your_postgresql_password'

# Recreate the container
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

# Verify the container is running
docker ps | grep open-webui
```

**Verify after update**:
- Browse to `http://192.168.1.192:3000` and confirm you can log in
- Confirm models are visible in the model selector
- Send a test message with the web search toggle enabled to confirm SearXNG is working

**Notes**:
- The `--network ai-network` flag in the run command handles the SearXNG connection automatically. No additional steps are required after rebuilding the container.
- **One-time setup** (only needed once, not on every update): `sudo ufw allow from 172.18.0.0/16 to any port 11434` — UFW rules are persistent across reboots and container rebuilds.

---

## SearXNG

SearXNG runs in Docker on the `ai-network`. Config is stored in a persistent bind mount at `/opt/searxng/` and is not affected by container rebuilds.

**Current method (Docker Compose):**
```bash
cd /docs/terminai/it/NewPC
docker compose pull searxng
docker compose up -d searxng
```

> **OBSOLETE — kept for reference only.** The `docker run` command below is no longer how this container is recreated; `docker-compose.yml` is now authoritative.

```bash
# Stop and remove the existing container
docker stop searxng && docker rm searxng

# Pull the latest image
docker pull searxng/searxng:latest

# Recreate the container
docker run -d \
  --name searxng \
  --restart unless-stopped \
  --network ai-network \
  -p 127.0.0.1:18080:8080 \
  -p 192.168.1.192:8080:8080 \
  -v /opt/searxng:/etc/searxng:rw \
  searxng/searxng:latest

# Verify the container is running
docker ps | grep searxng
```

**Verify after update**:
```bash
# Confirm Open WebUI can reach SearXNG over the internal network
docker exec open-webui curl -s "http://searxng:8080/search?q=test&format=json" | head -c 200
```

> **Config note**: `/opt/searxng/settings.yml` must contain `formats: [html, json]` under the `search:` section, otherwise Open WebUI receives HTML instead of JSON and web search fails. This setting persists across rebuilds in the bind mount — no action needed unless the config file is manually reset.

**Access**: `http://192.168.1.192:8080` (LAN) · `https://amelai.tail926601.ts.net:8080` (Tailscale)

---

## n8n

n8n runs in Docker. Workflows, credentials, and settings are stored in the `n8n_data` named volume and are not affected by container rebuilds.

> **Critical**: `N8N_ENCRYPTION_KEY` must match the value used when the container was first created. If the key changes, all stored credentials become unreadable. It now lives in `secrets/n8n.env` (see `DockerComposeDocs.md`) — check it there rather than retrieving it from the running container.

**Current method (Docker Compose):**
```bash
cd /docs/terminai/it/NewPC
docker compose pull n8n
docker compose up -d n8n
```

> **OBSOLETE — kept for reference only.** The `docker run` command below is no longer how this container is recreated; `docker-compose.yml` is now authoritative. The encryption key it references via `${N8N_KEY}` is a bash shell variable pattern that predates the Compose migration — the real key is now in `secrets/n8n.env`.

```bash
# Stop and remove the existing container
docker stop n8n && docker rm n8n

# Pull the latest image
docker pull n8nio/n8n

# Set your encryption key (single quotes prevent bash special char issues)
N8N_KEY='your_encryption_key_here'

# Recreate the container
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
  -e N8N_ENCRYPTION_KEY=${N8N_KEY} \
  -e N8N_SECURE_COOKIE=false \
  n8nio/n8n

# Verify the container is running and ports are bound
docker ps | grep n8n
```

**Verify after update**:
- Browse to `http://192.168.1.192:5678` and confirm you can log in
- Open a workflow and check that credentials are still showing as connected (not showing "Credential is not valid")
- Confirm active workflows are still toggled on

> **No GPU dependency** — n8n does not use GPU. The `--restart unless-stopped` policy is sufficient; it does not need the nvidia-wait systemd service.

**Access**: `http://192.168.1.192:5678` (LAN) · `https://amelai.tail926601.ts.net:5678` (Tailscale)

---

## ComfyUI

ComfyUI runs in Docker using the `yanwk/comfyui-boot:cu128-slim` image. Models, custom nodes, workflows, and ComfyUI Manager are stored in persistent volumes and are not affected by updates.

There are two instances — yours (GPU 1, port 8189) and Amelia's (GPU 0, port 8188). Update each separately.

**Current method (Docker Compose):**
```bash
cd /docs/terminai/it/NewPC
docker compose pull comfyui comfyui-amelia
docker compose up -d comfyui comfyui-amelia
```

> **OBSOLETE — kept for reference only.** Both `docker run` commands below (for `comfyui` and `comfyui-amelia`) are no longer how these containers are recreated; `docker-compose.yml` is now authoritative for both.

### Yours (comfyui)

```bash
# Stop and remove the existing container
docker stop comfyui && docker rm comfyui

# Pull the latest image
docker pull yanwk/comfyui-boot:cu128-slim

# Recreate the container
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

# Verify the container is running
docker ps | grep comfyui
docker logs comfyui --tail 20
```

### Amelia's (comfyui-amelia)

```bash
docker stop comfyui-amelia && docker rm comfyui-amelia

docker run --name=comfyui-amelia \
        --hostname=f1290bbde2d5 \
        --user=root \
        --volume /opt/comfyui-amelia/workflows:/root/ComfyUI/user/default/workflows \
        --volume /opt/comfyui-amelia/storage:/root \
        --volume /opt/comfyui-amelia/input:/root/ComfyUI/input \
        --volume /mnt/models/comfyui-amelia:/root/ComfyUI/models \
        --volume /opt/comfyui-amelia/output:/root/ComfyUI/output \
        --env=NVIDIA_VISIBLE_DEVICES=all \
        --env=NVIDIA_DRIVER_CAPABILITIES=compute,utility \
        --env=CUDA_VISIBLE_DEVICES=0 \
        --env=CLI_ARGS=--disable-xformers \
        --env=PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True \
        --network=ai-network \
        --workdir=/root \
        -p 127.0.0.1:18188:8188 \
        -p 192.168.1.192:8188:8188 \
        --restart=unless-stopped \
        --runtime=nvidia \
        --detach=true \
        yanwk/comfyui-boot:cu128-slim \
        bash /runner-scripts/entrypoint.sh

docker logs comfyui-amelia --tail 20
```

Wait for the logs to show `[ComfyUI-Manager] All startup tasks have been completed.` before using the UI.

**What is preserved across updates** (stored in persistent volumes):
- All models in `/mnt/models/comfyui/` and `/mnt/models/comfyui-amelia/`
- ComfyUI Manager and all installed custom nodes (`/opt/comfyui*/storage/`)
- Saved workflows — yours: `/docs/Projects/Claude Code Shared/Workflows/`, Amelia's: `/opt/comfyui-amelia/workflows/`
- Input images — both: `/opt/comfyui*/input/`
- Output images — yours: `/docs/Projects/Claude Code Shared/Output/`, Amelia's: `/opt/comfyui-amelia/output/`

**Verify after update**:
- Browse to `http://192.168.1.192:8189` (yours) or `http://192.168.1.192:8188` (Amelia's) and confirm the UI loads
- Check the Workflows panel to confirm saved workflows are present
- Run a test image generation to confirm GPU access is working

> **Note**: Amelia's container uses dual `-p` bindings — `127.0.0.1:18188` for Tailscale Serve and `192.168.1.192:8188` for LAN access. Your container (comfyui) uses only the Tailscale binding (`127.0.0.1:18189`). Using `-p 8188:8188` alone will break Tailscale access.

---

## File Browser

File Browser runs in Docker using the `filebrowser/filebrowser:latest` tag. It serves several directories over the web UI — RAG output, ComfyUI outputs, and ComfyUI workflows. The database file (users, settings, shares) is stored as a bind mount and is preserved across container rebuilds.

### Check if an update is available

```bash
# Check current installed version
docker inspect filebrowser --format='{{index .Config.Labels "org.opencontainers.image.version"}}'

# Pull latest and see if a newer image is available
docker pull filebrowser/filebrowser:latest
# Output will say either:
#   Status: Image is up to date for filebrowser/filebrowser:latest
#   Status: Downloaded newer image for filebrowser/filebrowser:latest
```

### Update File Browser

**Current method (Docker Compose):**
```bash
cd /docs/terminai/it/NewPC
docker compose pull filebrowser
docker compose up -d filebrowser
```

> **OBSOLETE — kept for reference only.** The `docker run` command below is no longer how this container is recreated; `docker-compose.yml` is now authoritative.

```bash
# Stop and remove the existing container
docker stop filebrowser && docker rm filebrowser

# Pull the latest image (if not already pulled above)
docker pull filebrowser/filebrowser:latest

# Recreate the container
docker run -d \
  --name filebrowser \
  --network ai-network \
  --restart always \
  --user 1000:1000 \
  -p 127.0.0.1:18087:80 \
  -p 192.168.1.192:8087:80 \
  -v /home/steve/rag-output:/srv \
  -v "/docs/Projects/Claude Code Shared/Output:/srv/comfyui-output" \
  -v /opt/comfyui/workflows:/srv/comfyui-workflows \
  -v /opt/comfyui-amelia/output:/srv/comfyui-amelia-output \
  -v /home/steve/filebrowser/filebrowser.db:/database/filebrowser.db \
  filebrowser/filebrowser:latest

# Verify the container is running and healthy
docker ps | grep filebrowser
```

**Verify after update**:
- Browse to `http://192.168.1.192:8087` and confirm you can log in
- Check that the served directories (RAG output, ComfyUI outputs) are visible

**What is preserved across updates**:
- Users, settings, and shares — stored in `/home/steve/filebrowser/filebrowser.db` (bind mount, not affected by container rebuild)

> **Note**: The `comfyui-workflows` volume (`/opt/comfyui/workflows`) points to the original workflows path, not the current live location (`/docs/Projects/Claude Code Shared/Workflows`). If you want to browse your active workflows in File Browser, update this bind mount to the new path.

---

## context-mode MCP Plugin

context-mode is a Claude Code plugin that manages the context window during AI sessions. It runs as a Node.js child process and uses SQLite (via `better-sqlite3`) as its knowledge base. Updates are released frequently and an outdated version can cause the MCP server to crash silently during sessions.

**Why updates matter:** Versions below v1.0.162 are affected by a Linux kernel/V8 bug (`madvise MADV_DONTNEED SIGSEGV`) that causes `better-sqlite3` to crash the Node process 1–4 times per hour. Node.js ≥ 22.5 and context-mode ≥ v1.0.162 are both required to avoid this.

**Current Node.js version required:** ≥ 22.5.0 (run `node -v` to check; use `nvm install 22 && nvm alias default 22` to upgrade if needed)

### Check current version

```bash
node ~/.claude/plugins/cache/context-mode/context-mode/1.0.75/cli.bundle.mjs --version 2>/dev/null || \
  node $(ls -d ~/.claude/plugins/cache/context-mode/context-mode/*/cli.bundle.mjs | tail -1) --version
```

### Update context-mode from the terminal

```bash
# Find the CLI path (the directory name reflects the installed version, not the latest)
CLI=$(ls -d ~/.claude/plugins/cache/context-mode/context-mode/*/cli.bundle.mjs | tail -1)

# Run the upgrade
node "$CLI" upgrade
```

The upgrade command will:
1. Pull the latest version from GitHub
2. Build and install it in-place
3. Rebuild native addons (`better-sqlite3`)
4. Reconfigure Claude Code hooks
5. Run a self-test (doctor) and report results

### Verify after update

```bash
# Run the doctor check to confirm all systems pass
node "$CLI" doctor
```

All items should show `PASS`. Key things to confirm:
- **npm (MCP)** and **Claude Code** version numbers match and show the latest version
- **FTS5 / SQLite: PASS** — confirms the native SQLite module is working
- **Node.js** shown as ≥ v22.5.0

### After updating

Restart Claude Code (close the terminal and reopen, or start a new session) for the new version to take effect. The update installs files in-place but the running MCP server process must be restarted to pick up the changes.

---

## Firmware Updates (fwupdmgr)

Ubuntu uses `fwupdmgr` (the Linux Vendor Firmware Service client) to manage firmware updates for hardware devices — including the UEFI Secure Boot revocation database, motherboard firmware, and storage controllers. These updates are separate from apt package updates and must be checked and applied independently.

### What gets updated

The most common firmware update on this system is the **UEFI dbx (Secure Boot Forbidden Signature Database)**. This is a Microsoft-published list of bootloader signatures that are no longer trusted — typically updated when a security vulnerability is found in a bootloader that could allow an attacker to bypass UEFI Secure Boot. Updates to this list are rated **High** urgency and should be applied promptly.

Other devices that may receive updates include the ASUS motherboard firmware, NVMe controllers, and USB4 host controller, though these are less frequent.

### Check for available firmware updates

Ubuntu shows a reminder at SSH login if a firmware update is available:

```
1 device has a firmware upgrade available.
Run `fwupdmgr get-upgrades` for more information.
```

To see what is available:

```bash
fwupdmgr get-upgrades
```

This lists all devices and shows available updates with version numbers, urgency, and whether a reboot is required.

### Install firmware updates

```bash
sudo fwupdmgr update
```

If no update is actually applied (output says `Devices with the latest available firmware version`), the device was already up to date — no reboot is needed.

If an update is applied, check whether a reboot is required — it will be stated in the `fwupdmgr get-upgrades` output under `Device Flags: • Needs a reboot after installation`.

### Check whether a reboot is needed (after any update)

After running either `sudo apt full-upgrade` or `sudo fwupdmgr update`, check whether Ubuntu has flagged a reboot as required:

```bash
ls /var/run/reboot-required 2>/dev/null && cat /var/run/reboot-required.pkgs
```

If the file exists, a reboot is needed and the second command lists which packages triggered it. If the command returns nothing, no reboot is required.

### Recommended update sequence

1. `sudo apt update && sudo apt full-upgrade` — install all held-back packages
2. `sudo fwupdmgr update` — apply any available firmware updates
3. Check reboot requirement with the command above
4. Reboot once if needed (covers both apt and firmware changes)

---

## pdf-to-image

A lightweight internal microservice that converts Companies House accounts PDFs into base64-encoded PNG images for processing by Ollama's vision model. Used exclusively by the n8n Customer Profiler workflow.

**How it works**: When n8n sends a PDF binary to `http://192.168.1.192:8086/convert`, the service uses poppler's `pdftoppm` to render each page to PNG at 100 DPI, encodes them as base64 strings, and returns them in a JSON response. Images are **never saved to disk permanently** — poppler writes temporary files to `/tmp` inside the container during rendering, which are deleted immediately once loaded into memory. Nothing persists between requests.

**Source files**: `/docs/terminai/it/NewPC/n8n/pdf-to-image/` (three files: `app.py`, `requirements.txt`, `Dockerfile`)

Unlike other services on this server, pdf-to-image uses a **custom-built Docker image** rather than a pulled one. The image must exist on the host before the container can be created.

### Recreate the container (image already built)

**Current method (Docker Compose):**
```bash
cd /docs/terminai/it/NewPC
docker compose up -d pdf-to-image
```

> **OBSOLETE — kept for reference only.** The commands below are no longer how this container is recreated; `docker-compose.yml` is now authoritative (it builds from `./n8n/pdf-to-image` automatically).

Use this when the container has been deleted or lost its port bindings, but the source files have not changed.

```bash
# Check the image exists first
docker images | grep pdf-to-image

# If the image is listed, recreate the container:
docker stop pdf-to-image 2>/dev/null; docker rm pdf-to-image 2>/dev/null

docker run -d \
  --name pdf-to-image \
  --restart unless-stopped \
  -p 127.0.0.1:18086:8086 \
  -p 192.168.1.192:8086:8086 \
  pdf-to-image

# Verify
curl http://192.168.1.192:8086/health
# Expected: {"status":"ok"}
```

> If `docker images | grep pdf-to-image` returns nothing (image was removed by `docker system prune` or similar), follow the **Rebuild the image** steps below instead.

### Rebuild the image

**Current method (Docker Compose):**
```bash
cd /docs/terminai/it/NewPC
docker compose up -d --build pdf-to-image
```

> **OBSOLETE — kept for reference only.** The commands below are no longer how this container is rebuilt; `docker-compose.yml` is now authoritative.

Use this when the source files have changed, or the image is no longer on the host.

```bash
# Stop and remove the existing container
docker stop pdf-to-image && docker rm pdf-to-image

# Rebuild the image from source
docker build -t pdf-to-image /docs/terminai/it/NewPC/n8n/pdf-to-image/

# Recreate the container
docker run -d \
  --name pdf-to-image \
  --restart unless-stopped \
  -p 127.0.0.1:18086:8086 \
  -p 192.168.1.192:8086:8086 \
  pdf-to-image
```

**Verify after rebuild**:
```bash
curl http://192.168.1.192:8086/health
# Expected: {"status":"ok"}
```

### Key configuration (app.py)

| Constant | Current value | Purpose |
|---|---|---|
| `DPI` | 200 | Rendering resolution — lower = fewer tokens per page for the vision model; higher = more detail but larger images |
| `MAX_DIM` | 2000 | Maximum pixel dimension per axis — images larger than this are scaled down before encoding |

> Increasing `DPI` improves readability for very dense financial tables but significantly increases the token count sent to Ollama, which can overflow the model's context window. 100 DPI is the recommended balance for A4 accounts documents.

### What is preserved across rebuilds

Nothing — the container holds no state. All configuration is in the source files, and the Docker image carries no persistent data.

---

## Checking Current Versions

```bash
# Ollama version
ollama -v

# Open WebUI version (check the running container's image digest)
docker inspect open-webui --format='{{.Config.Image}}'

# All running containers and their images
docker ps --format 'table {{.Names}}\t{{.Image}}\t{{.Status}}'
```
