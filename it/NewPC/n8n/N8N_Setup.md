# n8n Setup Guide — Docker on amelai

**Server**: amelai (`192.168.1.192`) — Ubuntu 24.04 LTS  
**Access**: LAN (`http://192.168.1.192:5678`) or Tailscale (`https://amelai.tail926601.ts.net:5678`)  
**Container image**: `n8nio/n8n` (Docker Hub)  
**Last Updated**: April 2026

---

## Overview

n8n is a self-hosted workflow automation tool — similar to Zapier or Make, but running locally. It lets you build automated workflows (called *nodes*) that connect services, trigger on schedules or webhooks, and process data without writing code. Everything runs inside Docker, and your workflows and credentials are persisted in a named Docker volume.

**Key terms:**

| Term | Meaning |
|---|---|
| **Workflow** | A sequence of connected actions (nodes) that run automatically |
| **Node** | A single step in a workflow — e.g. "receive webhook", "call API", "send email" |
| **Webhook** | A URL that triggers a workflow when called by an external service |
| **Trigger node** | The starting point of a workflow (schedule, webhook, etc.) |
| **Credential** | Stored API key or auth token used by nodes to connect to external services |

---

## Port Assignment

Following the established dual-binding strategy for this server:

| Purpose | Port |
|---|---|
| Container internal port | `5678` |
| Loopback host port (Tailscale serve → container) | `15678` |
| LAN host port (direct LAN access) | `5678` |

| Access method | URL |
|---|---|
| Local network | `http://192.168.1.192:5678` |
| Tailscale (HTTPS) | `https://amelai.tail926601.ts.net:5678` |

---

## Prerequisites

- Docker installed and running (`docker ps` to verify)
- Tailscale running (`sudo tailscale status` to verify)
- Port `5678` not already in use (`sudo ss -tlnup | grep :5678`)

---

## Installation

### Step 1: Create a named volume for persistent data

```bash
docker volume create n8n_data
```

This stores all workflows, credentials, and settings. It survives container removal and updates.

### Step 2: Generate an encryption key

n8n encrypts stored credentials (API keys, passwords) using an encryption key. Generate one now and save it — **if you lose this key, stored credentials become unrecoverable**.

```bash
openssl rand -hex 32
```

Copy the output. You will use it as `N8N_ENCRYPTION_KEY` in the next step.

> **Warning**: Store this key securely (e.g. a password manager). Do not save it in plain text on the server.

### Step 3: Run the container

Replace `<YOUR_ENCRYPTION_KEY>` with the value generated above.

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

**Environment variable explanations:**

| Variable | Value | Purpose |
|---|---|---|
| `GENERIC_TIMEZONE` | `Europe/London` | Correct timezone for schedule triggers |
| `N8N_HOST` | `amelai.tail926601.ts.net` | Hostname used in webhook URLs shown in the UI |
| `N8N_PORT` | `5678` | Internal port n8n listens on |
| `N8N_PROTOCOL` | `https` | Tells n8n to generate `https://` webhook URLs |
| `WEBHOOK_URL` | `https://amelai.tail926601.ts.net:5678` | Full base URL for webhook nodes |
| `N8N_ENCRYPTION_KEY` | (generated) | Key used to encrypt stored credentials |
| `N8N_SECURE_COOKIE` | `false` | Required when Tailscale terminates TLS (container sees plain HTTP) |

### Step 4: Verify the container is running

```bash
docker ps | grep n8n
docker logs n8n --tail 20
```

Expected log output includes: `n8n ready on 0.0.0.0, port 5678`

### Step 5: Configure Tailscale serve

This registers n8n with Tailscale's HTTPS proxy so it is accessible at `https://amelai.tail926601.ts.net:5678`:

```bash
sudo tailscale serve --bg --https=5678 http://localhost:15678
```

Verify it was registered:

```bash
sudo tailscale serve status
```

### Step 6: First login

Open `https://amelai.tail926601.ts.net:5678` in a browser. On first launch, n8n will prompt you to create an owner account. Complete the setup wizard:

1. Enter your name and email
2. Set a strong password
3. Optionally configure email settings (can be skipped)

---

## Accessing n8n

| Network | URL | Protocol |
|---|---|---|
| Local network | `http://192.168.1.192:5678` | Plain HTTP |
| Tailscale | `https://amelai.tail926601.ts.net:5678` | HTTPS (TLS via Tailscale) |

> LAN access is plain HTTP. Always use the Tailscale URL when accessing from outside the local network, or when security matters (e.g. entering credentials).

---

## Update Procedure

n8n releases updates frequently. Updates are applied by pulling the latest image and recreating the container. Your data (workflows, credentials) is in the `n8n_data` volume and is unaffected.

```bash
# Pull the latest image
docker pull n8nio/n8n

# Stop and remove the existing container
docker stop n8n
docker rm n8n

# Re-run with the same command as Step 3
# (replace <YOUR_ENCRYPTION_KEY> with your actual key)
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

> The Tailscale serve configuration persists across container updates — no need to re-run the `tailscale serve` command unless it has been removed.

---

## Useful Management Commands

```bash
# View live logs
docker logs n8n -f

# View last 50 lines of logs
docker logs n8n --tail 50

# Stop n8n
docker stop n8n

# Start n8n (if stopped)
docker start n8n

# Restart n8n
docker restart n8n

# Check n8n version
docker exec n8n n8n --version

# Inspect the data volume
docker volume inspect n8n_data
```

---

## Backup

n8n workflows and credentials are stored in the `n8n_data` Docker volume, located at:

```
/var/lib/docker/volumes/n8n_data/_data/
```

To back up:

```bash
# Create a compressed archive of the n8n data volume
sudo tar -czf ~/n8n_backup_$(date +%Y%m%d).tar.gz \
  -C /var/lib/docker/volumes/n8n_data/_data .
```

To restore from backup:

```bash
# Stop n8n first
docker stop n8n

# Extract backup into volume
sudo tar -xzf ~/n8n_backup_YYYYMMDD.tar.gz \
  -C /var/lib/docker/volumes/n8n_data/_data

# Start n8n
docker start n8n
```

> You can also export individual workflows from the n8n UI: **Workflow → ⋮ → Export** saves a JSON file you can import on any n8n instance.

---

## Troubleshooting

### n8n not accessible on LAN

```bash
# Confirm container is running and port binding is correct
docker ps | grep n8n
sudo ss -tlnup | grep :5678
```

Ensure the `-p 192.168.1.192:5678:5678` binding is present in `docker ps` output.

### Webhooks not triggering

Webhooks use the `WEBHOOK_URL` value as their base. If you access n8n on LAN (`http://192.168.1.192:5678`), webhook URLs shown in the UI will still show the Tailscale address — this is expected. External services calling the webhook must be able to reach `amelai.tail926601.ts.net:5678`, which requires Tailscale connectivity.

For webhooks called from internal services only, you can call `http://192.168.1.192:5678/webhook/<path>` directly.

### Credentials show as invalid after update

If you recreated the container without passing the same `N8N_ENCRYPTION_KEY`, stored credentials will be unreadable. Restore by:

1. Stopping the container
2. Re-running with the correct original encryption key
3. If the key is lost, credentials must be re-entered manually

### Container exits immediately

```bash
docker logs n8n --tail 30
```

Common causes: port already in use, volume permission error, or missing environment variable.

---

## Port Reference (Full Server)

| Service | Container port | Loopback host port | LAN/Tailscale port |
|---|---|---|---|
| Open WebUI | 8080 | 3000 | 3000 |
| ComfyUI (Steve) | 8188 | 18189 | 8189 |
| ComfyUI (Amelia) | 8188 | 18188 | 8188 |
| FileBrowser | 80 | 18087 | 8087 |
| SearXNG | 8080 | 18080 | 8080 |
| **n8n** | **5678** | **15678** | **5678** |
