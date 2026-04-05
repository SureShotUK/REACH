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

These are the exact commands to recreate each service if its container is ever deleted. Run them from the server as `steve`.

---

### Open WebUI

Browser-based chat interface for Ollama models.

```bash
docker run -d \
  --name open-webui \
  --restart always \
  --gpus all \
  -p 127.0.0.1:3000:8080 \
  -p 192.168.1.192:3000:8080 \
  -v open-webui:/app/backend/data \
  -e OLLAMA_BASE_URL=http://192.168.1.192:11434 \
  ghcr.io/open-webui/open-webui:main
```

**Then reconnect to ai-network** (required for SearXNG web search):

```bash
docker network connect ai-network open-webui
```

| Option | Purpose |
|---|---|
| `--restart always` | Auto-starts on server reboot |
| `--gpus all` | Gives container access to both RTX 3090s |
| `-p 127.0.0.1:3000:8080` | Loopback binding — Tailscale Serve proxies through here |
| `-p 192.168.1.192:3000:8080` | LAN binding — direct local network access |
| `-v open-webui:/app/backend/data` | Named volume — persists chat history, users, settings |
| `OLLAMA_BASE_URL` | Points to Ollama on the host — must use the LAN IP, not `localhost` |

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
  -p 127.0.0.1:8189:8188 \
  -p 192.168.1.192:8189:8188 \
  -v /mnt/models/comfyui:/root/ComfyUI/models \
  -v /opt/comfyui/storage:/root \
  -v /opt/comfyui/input:/root/ComfyUI/input \
  -v /opt/comfyui/output:/root/ComfyUI/output \
  -v /opt/comfyui/workflows:/root/ComfyUI/user/default/workflows \
  -e CUDA_VISIBLE_DEVICES=1 \
  -e CLI_ARGS="--disable-xformers" \
  -e PYTORCH_CUDA_ALLOC_CONF=expandable_segments:True \
  yanwk/comfyui-boot:cu128-slim
```

| Option | Purpose |
|---|---|
| `CUDA_VISIBLE_DEVICES=1` | Restricts this container to GPU 1 only; prevents OOM errors from large models |
| `-v /mnt/models/comfyui` | Model files on second drive — shared with host, survive container rebuild |
| `-v /opt/comfyui/output` | Generated images folder |
| `--runtime nvidia` | Enables NVIDIA GPU access via the container toolkit |

**Access**: `http://192.168.1.192:8189` (LAN) · `https://amelai.tail926601.ts.net:8189` (Tailscale)

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
  -v /opt/comfyui/output:/srv/comfyui-output \
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
  -p 100.79.83.113:8080:8080 \
  -v /opt/searxng:/etc/searxng:rw \
  searxng/searxng:latest
```

| Option | Purpose |
|---|---|
| `-p 100.79.83.113:8080:8080` | Binds to Tailscale IP — accessible from Tailscale network and by Open WebUI |
| `-v /opt/searxng:/etc/searxng` | Persistent config, including `settings.yml` |
| `--network ai-network` | Allows Open WebUI to reach it at `http://searxng:8080` |

**After recreating**, verify Open WebUI can still reach it:

```bash
docker exec open-webui curl -s "http://searxng:8080/search?q=test&format=json" | head -c 200
```

> **Config note**: `/opt/searxng/settings.yml` must contain `formats: [html, json]` under the `search:` section, otherwise Open WebUI will get HTML responses instead of JSON and web search will fail. Restart after any config change: `docker restart searxng`.

---

## Port Map — All Services

| Service | Container | Container port | Loopback host port | LAN/Tailscale port |
|---|---|---|---|---|
| Open WebUI | `open-webui` | 8080 | 3000 | 3000 |
| ComfyUI (Steve) | `comfyui` | 8188 | 8189 | 8189 |
| ComfyUI (Amelia) | `comfyui-amelia` | 8188 | 18188 | 8188 |
| FileBrowser | `filebrowser` | 80 | 18087 | 8087 |
| SearXNG | `searxng` | 8080 | — | 8080 (Tailscale IP) |

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
