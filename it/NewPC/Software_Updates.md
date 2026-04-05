# Software Updates Guide — AmelAI Server

This guide covers the update procedure for each application running on the AmelAI server. Follow the steps in order for each application.

---

## Important Notes

### After Every Open WebUI Container Recreate

Recreating the Open WebUI container (required for updates) resets its Docker network connections. You **must** run these two steps after every update:

```bash
# 1. Reconnect Open WebUI to the ai-network (required for SearXNG web search)
docker network connect ai-network open-webui

# 2. Ensure UFW allows ai-network traffic to reach Ollama
sudo ufw allow from 172.18.0.0/16 to any port 11434
```

Skipping these steps will result in:
- Web search failing ("An error occurred while searching the web")
- No models visible in Open WebUI

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
  --restart always \
  --gpus all \
  -p 3000:8080 \
  -v open-webui:/app/backend/data \
  -e OLLAMA_BASE_URL=http://192.168.1.192:11434 \
  ghcr.io/open-webui/open-webui:main

# Reconnect to ai-network (required for SearXNG web search)
docker network connect ai-network open-webui

# Ensure firewall allows ai-network to reach Ollama
sudo ufw allow from 172.18.0.0/16 to any port 11434

# Verify the container is running
docker ps | grep open-webui
```

**Verify after update**:
- Browse to `http://192.168.1.192:3000` and confirm you can log in
- Confirm models are visible in the model selector
- Send a test message with the web search toggle enabled to confirm SearXNG is working

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

```bash
# Stop and remove the existing container
docker stop comfyui
docker rm comfyui

# Pull the latest image
docker pull yanwk/comfyui-boot:cu128-slim

# Recreate the container
docker run -d \
  --name comfyui \
  --network ai-network \
  --restart unless-stopped \
  --runtime nvidia \
  --gpus all \
  -p 8188:8188 \
  -v /mnt/models/comfyui:/root/ComfyUI/models \
  -v /opt/comfyui/storage:/root \
  -v /opt/comfyui/input:/root/ComfyUI/input \
  -v /opt/comfyui/output:/root/ComfyUI/output \
  -v /opt/comfyui/workflows:/root/ComfyUI/user/default/workflows \
  -e CLI_ARGS="--disable-xformers" \
  yanwk/comfyui-boot:cu128-slim

# Verify the container is running
docker ps | grep comfyui
docker logs comfyui --tail 20
```

Wait for the logs to show `[ComfyUI-Manager] All startup tasks have been completed.` before using the UI.

**What is preserved across updates** (stored in persistent volumes):
- All models in `/mnt/models/comfyui/`
- ComfyUI Manager and all installed custom nodes (`/opt/comfyui/storage/`)
- Saved workflows (`/opt/comfyui/workflows/`)
- Input and output images (`/opt/comfyui/input/`, `/opt/comfyui/output/`)

**Verify after update**:
- Browse to `http://192.168.1.192:8188` and confirm the UI loads
- Check the Workflows panel to confirm saved workflows are present
- Run a test image generation to confirm GPU access is working

> **Note**: ComfyUI is on `ai-network` from creation so no network reconnection is needed after updates. The UFW rule for port 8188 (`sudo ufw allow from 192.168.1.0/24 to any port 8188`) is persistent and does not need to be reapplied.

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
