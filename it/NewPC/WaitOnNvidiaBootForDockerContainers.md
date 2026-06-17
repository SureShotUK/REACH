# GPU Docker Containers: Waiting for NVIDIA Driver at Boot

## Problem

After a reboot, Docker containers that use `--gpus all` fail to start automatically, while containers with no GPU dependency start fine. The failed containers exit immediately with code 128 and the following error:

```
failed to create task for container: failed to create shim task: OCI runtime create failed:
runc create failed: unable to start container process: error during container init:
error running prestart hook #0: exit status 1, stdout: , stderr:
Auto-detected mode as 'legacy'
nvidia-container-cli: initialization error: nvml error: driver not loaded
```

**Root cause**: Docker starts its containers as soon as the Docker daemon is ready, but the NVIDIA kernel module finishes loading slightly later. The GPU containers hit the NVIDIA container runtime before it is functional and exit immediately. Containers with no GPU dependency are unaffected.

**Affected containers on amelai**: `open-webui`, `comfyui`, `comfyui-amelia`

**Unaffected containers**: `searxng`, `filebrowser`, `n8n` (no GPU dependency)

---

## Diagnosis

To confirm this is the cause of a container failure, check the Docker-level error (not the application log):

```bash
docker inspect <container-name> --format='{{.State.Error}}'
```

If you see `nvml error: driver not loaded`, this document's fix applies.

Note: `docker logs <container>` will show output from the **previous** successful run, not the failed boot attempt — the container exits before the application starts, so no new log lines are written.

---

## Solution

Create a systemd service that polls `nvidia-smi` until the NVIDIA driver responds, then starts the GPU-dependent containers. This runs once at boot, after the Docker daemon is ready, and waits as long as needed before proceeding.

### Step 1 — Create the service file

```bash
sudo tee /etc/systemd/system/docker-gpu-containers.service > /dev/null << 'EOF'
[Unit]
Description=Start GPU Docker containers after NVIDIA runtime is ready
After=docker.service
Requires=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
ExecStartPre=/bin/bash -c 'until /usr/bin/nvidia-smi > /dev/null 2>&1; do sleep 2; done'
ExecStart=/usr/bin/docker start open-webui comfyui comfyui-amelia
ExecStop=/usr/bin/docker stop open-webui comfyui comfyui-amelia

[Install]
WantedBy=multi-user.target
EOF
```

### Step 2 — Enable the service

```bash
sudo systemctl daemon-reload
sudo systemctl enable docker-gpu-containers.service
```

### Step 3 — Verify

```bash
# Confirm the service is enabled
systemctl is-enabled docker-gpu-containers.service

# Confirm containers are currently running
docker ps --format 'table {{.Names}}\t{{.Status}}'
```

---

## How It Works

| Section | Value | Purpose |
|---|---|---|
| `After=docker.service` | — | Ensures Docker daemon is running before this service starts |
| `Requires=docker.service` | — | If Docker fails to start, this service won't run |
| `ExecStartPre` | `until nvidia-smi; do sleep 2; done` | Polls every 2 seconds until the NVIDIA driver is loaded |
| `ExecStart` | `docker start open-webui comfyui comfyui-amelia` | Starts all three GPU containers once the driver is ready |
| `ExecStop` | `docker stop open-webui comfyui comfyui-amelia` | Gracefully stops all three containers on system shutdown |
| `Type=oneshot` | — | Runs once and exits; service remains "active" after completion |
| `RemainAfterExit=yes` | — | Keeps the service in "active" state so `ExecStop` runs on shutdown |

The non-GPU containers (`searxng`, `filebrowser`, `n8n`) continue to be managed entirely by Docker's own restart policy — they don't need this service.

---

## Managing the Service

```bash
# Check service status
systemctl status docker-gpu-containers.service

# Start GPU containers manually via the service
sudo systemctl start docker-gpu-containers.service

# Stop all GPU containers via the service
sudo systemctl stop docker-gpu-containers.service

# Disable auto-start (does not stop currently running containers)
sudo systemctl disable docker-gpu-containers.service
```

---

## Adding New GPU Containers

If you add a new Docker container that uses `--gpus all` and needs to start at boot, add it to both the `ExecStart` and `ExecStop` lines in the service file:

```bash
sudo nano /etc/systemd/system/docker-gpu-containers.service
# Edit ExecStart and ExecStop to include the new container name

sudo systemctl daemon-reload
```

No need to re-enable the service — it remains enabled after editing.
