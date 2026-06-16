# Software Updates Guide — AmelAI Server

This guide covers the update procedure for each application running on the AmelAI server. Follow the steps in order for each application.

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

```bash
# Stop and remove the existing container
docker stop open-webui
docker rm open-webui

# Pull the latest image
docker pull ghcr.io/open-webui/open-webui:main

# Recreate the container
docker run -d \
  --name open-webui \
  --network ai-network \
  --restart always \
  --gpus all \
  -p 3000:8080 \
  -v open-webui:/app/backend/data \
  -e OLLAMA_BASE_URL=http://192.168.1.192:11434 \
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

SearXNG runs in Docker on the `ai-network`. Updating it follows the same pattern as Open WebUI but does not require network reconnection steps as it was originally created on `ai-network`.

```bash
# Stop and remove the existing container
docker stop searxng
docker rm searxng

# Pull the latest image
docker pull searxng/searxng:latest

# Recreate the container (add your original run flags here)
# See the original setup command used to create the container:
docker inspect searxng --format='{{.Name}} {{.Config.Image}}'
```

> **Note**: Before removing the SearXNG container, capture its full run command if you are unsure of the original flags:
> ```bash
> docker inspect searxng --format='{{json .HostConfig}}' | python3 -m json.tool
> ```

---

## ComfyUI

ComfyUI runs in Docker using the `yanwk/comfyui-boot:cu128-slim` image. Models, custom nodes, workflows, and ComfyUI Manager are stored in persistent volumes and are not affected by updates.

There are two instances — yours (GPU 1, port 8189) and Amelia's (GPU 0, port 8188). Update each separately.

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

## Checking Current Versions

```bash
# Ollama version
ollama -v

# Open WebUI version (check the running container's image digest)
docker inspect open-webui --format='{{.Config.Image}}'

# All running containers and their images
docker ps --format 'table {{.Names}}\t{{.Image}}\t{{.Status}}'
```
