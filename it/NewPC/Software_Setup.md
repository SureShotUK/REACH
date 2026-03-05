# Software Setup Guide — Ubuntu 24.04 LTS Server for AI

**Build**: AMD Ryzen 9 7900X + MSI MAG X870E TOMAHAWK WIFI + Asus TUF RTX 3090 24GB
**OS**: Ubuntu 24.04 LTS Server (Noble Numbat)
**Date**: February 2026

---

## Overview

This guide covers the complete software setup for the AI PC build, from OS installation through a fully functioning local AI stack. The end result is a system running:

- **Ubuntu 24.04 LTS Server** — headless OS accessed via SSH, with full NVIDIA driver support
- **NVIDIA CUDA** — GPU computing framework required by AI tools
- **Docker** — Containerisation platform for running AI services
- **Ollama** — Local LLM inference engine (runs models on the RTX 3090)
- **Open WebUI** — Browser-based chat interface for interacting with models

**Target capability after setup**: Running 7B–70B language models locally, accessible via a web browser from any device on your network.

**Glossary of terms used in this guide:**

| Term | Meaning |
|------|---------|
| **CUDA** | NVIDIA's parallel computing platform; required for GPU-accelerated AI |
| **VRAM** | Video RAM — the memory on the GPU (24GB on your RTX 3090) |
| **Inference** | Running a trained AI model to generate responses (as opposed to training) |
| **Quantization** | Compressing model precision to reduce memory usage (e.g., Q4 = 4-bit) |
| **Tokens** | Units of text (roughly 0.75 words); AI generates text token by token |
| **Ollama** | Software that manages and runs AI models locally using the GPU |
| **Container** | Isolated software environment (like a lightweight VM) managed by Docker |
| **LTS** | Long Term Support — Ubuntu releases supported for 5 years |

---

## 1. Pre-Installation Preparation

### What You Need

- A separate computer (any OS) to create the bootable USB
- A USB drive, minimum 8GB, ideally 16GB+ (contents will be erased)
- The AI PC with all components assembled and BIOS configured (see Assembly Guide)
- Internet connection (ethernet recommended during installation)

### Download Ubuntu 24.04 LTS Server

Download the official Ubuntu 24.04.x LTS Server ISO from:

<a href="https://ubuntu.com/download/server" target="_blank">ubuntu.com/download/server</a>

The file will be approximately 2GB. Verify the SHA256 checksum if desired (the download page lists the expected hash).

### Create Bootable USB

**On Windows** — use Rufus:

<a href="https://rufus.ie" target="_blank">rufus.ie</a>

1. Insert USB drive
2. Open Rufus
3. Select your USB drive under Device
4. Click **SELECT** and choose the Ubuntu ISO file
5. Partition scheme: **GPT** (required for UEFI boot)
6. File system: **FAT32**
7. Click **START** → Select **Write in ISO image mode** if prompted
8. Wait for completion

**On Linux/Mac** — use `dd` or Balena Etcher:

<a href="https://etcher.balena.io" target="_blank">balena.io/etcher</a>

---

## 2. BIOS Boot Configuration

Before booting the USB on the AI PC:

1. Power on the AI PC and press **Delete** to enter BIOS
2. Navigate to **Boot** section
3. Set **Boot Mode** to **UEFI** (not Legacy/CSM)
4. **Secure Boot**: Ubuntu 24.04 supports Secure Boot — you can leave it enabled. If you encounter issues, disable it temporarily during installation
5. Insert the Ubuntu USB into a rear USB-A port
6. Set the USB drive as **Boot Option #1** temporarily
7. Save and reboot

---

## 3. Ubuntu 24.04 Server Installation

Ubuntu Server uses a text-based installer — there is no graphical interface. Navigate with arrow keys, Tab, and Enter.

### Boot and Language

1. Boot from the USB. The grub menu appears — select **Try or Install Ubuntu Server**
2. Select your language
3. The installer will check for an updated version of itself — you can skip this

### Network

The installer detects network interfaces. With ethernet connected, it will auto-configure via DHCP. Confirm the network is active before proceeding.

### Proxy and Mirror

- Proxy: leave blank unless your network requires one
- Archive mirror: accept the default (`archive.ubuntu.com`)

### Storage Partitioning (Two-Drive Setup)

This build has two Samsung 9100 Pro 2TB drives. Configure them as:

- **Drive 1 (M.2_1)**: Ubuntu OS, applications, Docker data
- **Drive 2 (M.2_2)**: AI model storage library

Select **Custom storage layout**, then set up manually:

**Drive 1 — OS drive (2TB, ~1,863 GiB usable):**

| Partition | Size | Type | Mount Point | Purpose |
|-----------|------|------|-------------|---------|
| EFI System | 1 GB | FAT32 | `/boot/efi` | UEFI boot files |
| Root | 500 GB | ext4 | `/` | OS, applications, Docker |
| Home | Remaining (~1,363 GB) | ext4 | `/home` | User files |

**Drive 2 — Model storage (2TB):**

| Partition | Size | Type | Mount Point | Purpose |
|-----------|------|------|-------------|---------|
| Models | All (2TB) | ext4 | `/mnt` | AI model files |

Set the **bootloader location** to Drive 1 (the OS drive).

### User Account

Create your user account with a strong password. The username will also be used for SSH login.

### SSH Setup

When asked about **OpenSSH server**, **tick Install OpenSSH server**. This is essential — without it you cannot SSH into the machine after installation.

You can import SSH keys from GitHub/Launchpad here if you have them set up; otherwise skip.

### Featured Server Snaps

Skip this screen — nothing here is needed for this build.

### Installation

The installation proceeds automatically. When complete, you will be prompted to remove the USB and press Enter to reboot.

---

## 4. Post-Install System Configuration

### First Boot

After installation completes and the system reboots into Ubuntu, run all system updates before installing anything else:

```bash
sudo apt update && sudo apt full-upgrade -y
sudo apt install -y build-essential git curl wget htop nvtop net-tools tmux
```

**What these install:**
- `build-essential` — C compiler and build tools (required for some software)
- `git` — version control (required for some AI tools)
- `curl` / `wget` — download utilities
- `htop` — system resource monitor
- `nvtop` — GPU utilisation monitor (like htop but for NVIDIA GPUs)
- `net-tools` — network utilities (`ifconfig`, etc.)
- `tmux` — terminal multiplexer; keeps sessions alive when SSH disconnects (see Section 8)

Reboot after updates:

```bash
sudo reboot
```

### Mount the Second Drive Permanently

The second drive is mounted at `/mnt`. After mounting, a `models` subdirectory is created within it for Ollama model storage.

```bash
# Find the drive's UUID
sudo blkid

# Note the UUID for the second Samsung 9100 Pro (the /mnt partition)
# It will appear as something like: /dev/nvme1n1p1: UUID="xxxx-xxxx" TYPE="ext4"

# Edit fstab to auto-mount on boot
sudo nano /etc/fstab
```

Add this line to `/etc/fstab` (replace `YOUR-UUID` with the actual UUID from `blkid`):

```
UUID=YOUR-UUID  /mnt  ext4  defaults,nofail  0  2
```

Save the file (`Ctrl+O`, `Enter`, `Ctrl+X`), then test the mount:

```bash
sudo mount -a
# Verify it mounted
df -h /mnt
```

Set ownership and create the models subdirectory:

```bash
sudo chown $USER:$USER /mnt
mkdir -p /mnt/models
```

---

## 5. NVIDIA Driver Installation

### Check Current Driver Status

If you ticked "Install third-party software" during installation, Ubuntu may have already installed an NVIDIA driver. Check:

```bash
nvidia-smi
```

If this command returns a table showing your GPU (RTX 3090), driver version, and CUDA version, the driver is already installed. **Skip to Section 6.**

If the command is not found or returns an error, install the driver:

### Install NVIDIA Driver (if not already installed)

```bash
# Check what drivers are available and recommended
ubuntu-drivers devices

# Install the recommended driver automatically
sudo ubuntu-drivers autoinstall

# OR install a specific version (535+ supports RTX 3090 fully)
sudo apt install -y nvidia-driver-565

sudo reboot
```

After reboot, verify:

```bash
nvidia-smi
```

**Expected output** (values will vary slightly):

```
+-----------------------------------------------------------------------------------------+
| NVIDIA-SMI 565.xx.xx    Driver Version: 565.xx.xx    CUDA Version: 12.7     |
|-----------------------------------------+------------------------+----------------------+
| GPU  Name                 Persistence-M | Bus-Id          Disp.A | Volatile Uncorr. ECC |
| Fan  Temp   Perf          Pwr:Usage/Cap |           Memory-Usage | GPU-Util  Compute M. |
|=========================================+========================+======================|
|   0  NVIDIA GeForce RTX 3090        Off |   00000000:01:00.0  On |                  N/A |
| 30%   42C    P8             25W /  350W |     387MiB /  24576MiB |      1%      Default |
+-----------------------------------------------------------------------------------------+
```

Key things to verify:
- GPU name shows **RTX 3090**
- Memory shows **24576 MiB** (24GB)
- Driver Version is installed

---

## 6. CUDA Toolkit

CUDA (Compute Unified Device Architecture) is NVIDIA's parallel computing framework. AI tools like PyTorch use CUDA to run on the GPU. The NVIDIA driver installs CUDA runtime support; the toolkit adds developer tools.

```bash
sudo apt install -y nvidia-cuda-toolkit
```

Verify CUDA installation:

```bash
nvcc --version
```

**Expected output:**

```
nvcc: NVIDIA (R) Cuda compiler driver
...
Cuda compilation tools, release 12.0, V12.0.xx
```

**Note on CUDA versions**: The system may have CUDA 12.0 from the driver install plus an older CUDA from the toolkit package. This is fine for Ollama and Open WebUI — they bundle their own CUDA libraries. You only need the driver-provided CUDA runtime.

---

## 7. Docker Installation

Docker is a containerisation platform. Open WebUI runs as a Docker container, which simplifies installation and updates.

### Install Docker

```bash
# Download and run the official Docker install script
curl -fsSL https://get.docker.com | sh

# Add your user to the docker group (avoids needing sudo for docker commands)
sudo usermod -aG docker $USER

# Apply group change (or log out and back in)
newgrp docker
```

Verify Docker works:

```bash
docker run hello-world
```

### Install NVIDIA Container Toolkit

This allows Docker containers to access the RTX 3090 GPU:

```bash
# Add NVIDIA GPG key
curl -fsSL https://nvidia.github.io/libnvidia-container/gpgkey | \
  sudo gpg --dearmor -o /usr/share/keyrings/nvidia-container-toolkit-keyring.gpg

# Download the repo list to a temp file
curl -s -L https://nvidia.github.io/libnvidia-container/stable/deb/nvidia-container-toolkit.list \
  -o /tmp/nvidia-ct.list

# Add the signed-by attribute (in-place edit on the temp file)
sed -i 's#deb https://#deb [signed-by=/usr/share/keyrings/nvidia-container-toolkit-keyring.gpg] https://#g' /tmp/nvidia-ct.list

# Copy into apt sources
sudo cp /tmp/nvidia-ct.list /etc/apt/sources.list.d/nvidia-container-toolkit.list

sudo apt update
sudo apt install -y nvidia-container-toolkit

# Configure Docker to use NVIDIA runtime
sudo nvidia-ctk runtime configure --runtime=docker
sudo systemctl restart docker
```

> **Note**: Older guides use `$distribution` (e.g., `ubuntu24.04`) in the URL. That path is deprecated and will return an HTML error page instead of the repo list, breaking `apt update`. Always use the `stable/deb/` path shown above.
>
> **Note on copy-pasting**: If you copy commands from this document via a Windows terminal, avoid pasting multi-line piped commands in one go — the `\` line continuations can be corrupted by Windows line endings. Run each line separately, or use the discrete step approach above.

Verify GPU is accessible from Docker:

```bash
docker run --rm --gpus all nvidia/cuda:12.6.0-base-ubuntu24.04 nvidia-smi
```

This should show the RTX 3090 information, confirming Docker can use the GPU.

---

## 8. Ollama Installation and Configuration

Ollama is the core AI inference engine. It manages downloading, running, and serving AI models on the RTX 3090.

### Install Ollama

```bash
curl -fsSL https://ollama.com/install.sh | sh
```

The installer automatically detects the NVIDIA GPU and configures Ollama to use it.

Ollama installs as a **system service** that starts automatically on boot. Check its status:

```bash
sudo systemctl status ollama
```

It should show `active (running)`.

### Configure Ollama to Use Model Storage Drive

By default, Ollama stores models in `~/.ollama/models`. Redirect this to the second drive where there is 2TB of dedicated model storage:

```bash
# Stop Ollama service first
sudo systemctl stop ollama

# Create models directory on second drive
mkdir -p /mnt/models/ollama

# Edit the Ollama systemd service to set the model path
sudo systemctl edit ollama
```

This opens a text editor. Add the following between the `### Edits below...` comment and the empty line:

```ini
[Service]
Environment="OLLAMA_MODELS=/mnt/models/ollama"
```

Save and close (`Ctrl+O`, `Enter`, `Ctrl+X`), then:

```bash
# Reload systemd and restart Ollama
sudo systemctl daemon-reload
sudo systemctl start ollama

# Verify Ollama is running
sudo systemctl status ollama
```

### Configure Ollama for Network Access (Optional)

By default, Ollama only listens on `localhost:11434`. To access it from other devices on your network (e.g., from a laptop):

```bash
sudo systemctl edit ollama
```

Add:

```ini
[Service]
Environment="OLLAMA_HOST=0.0.0.0:11434"
Environment="OLLAMA_MODELS=/mnt/models/ollama"
```

```bash
sudo systemctl daemon-reload
sudo systemctl restart ollama
```

**Security note**: Only do this on a trusted home network. This makes Ollama accessible to any device on your LAN.

### Downloading Large Models Safely with tmux

Large model downloads (10–20GB+) take considerable time. If you are connected via SSH and your terminal session closes — because you close your laptop, lose your connection, or the session times out — any running `ollama pull` command will be killed and the download will be lost.

**tmux** (terminal multiplexer) solves this by running a persistent session on the server that continues regardless of whether your SSH connection is active.

#### Starting a tmux session

```bash
tmux new-session -s ollama-pull
```

Your terminal prompt changes slightly to indicate you are now inside tmux. Any commands you run here will keep running even if you disconnect.

#### Detaching from tmux (keeping it running)

To disconnect from the tmux session while leaving it running on the server:

Press `Ctrl+B`, then release both keys, then press `D`

The terminal returns to your normal SSH prompt with a message like `[detached (from session ollama-pull)]`. The session and everything running inside it continues on the server.

#### Reattaching to a tmux session

When you SSH back in, reattach to your session:

```bash
tmux attach -t ollama-pull
```

You will see the session exactly as you left it, including any command output that was printed while you were away.

#### Listing active sessions

If you have multiple sessions or can't remember the name:

```bash
tmux ls
```

#### Closing a session when finished

Once the download is complete and you no longer need the session, exit it normally:

```bash
exit
```

Or kill it from outside tmux:

```bash
tmux kill-session -t ollama-pull
```

#### Typical workflow for pulling a large model

```bash
# 1. Start a named tmux session
tmux new-session -s ollama-pull

# 2. Inside tmux — pull the model
ollama pull qwen2.5-coder:32b

# 3. Optional — detach and close your laptop (download keeps running)
#    Press Ctrl+B, then D

# 4. Later — SSH back in and reattach to check progress
tmux attach -t ollama-pull

# 5. Once complete — exit and tidy up
exit
```

---

### Test Ollama — Download and Run First Model

Start with a small 7B model to verify everything works before downloading larger ones:

```bash
# Pull (download) a 7B model — approximately 4.7GB
ollama pull llama3.2:7b

# Run it interactively to test
ollama run llama3.2:7b

# Type a question and press Enter
# Type /bye to exit
```

Check GPU is being used during inference:

```bash
# In a second terminal while the model is running
nvtop
```

You should see GPU utilisation spike when tokens are generating, and VRAM usage showing the model loaded (approximately 4–5GB for a 7B Q4 model).

---

## 9. Recommended Models

Download models based on your use case. All commands use `ollama pull <model>`:

### For Coding Assistance

| Model | Command | VRAM Required | Speed (approx.) |
|-------|---------|--------------|----------------|
| Codestral 22B | `ollama pull codestral:22b` | ~13GB | 45–55 tok/s |
| DeepSeek Coder V2 16B | `ollama pull deepseek-coder-v2:16b` | ~10GB | 60–75 tok/s |
| Qwen2.5 Coder 32B | `ollama pull qwen2.5-coder:32b` | ~19GB | 25–35 tok/s |

### For General Use / Homework Help

| Model | Command | VRAM Required | Speed (approx.) |
|-------|---------|--------------|----------------|
| Llama 3.1 8B | `ollama pull llama3.1:8b` | ~5GB | 100–120 tok/s |
| Llama 3.1 70B (Q4) | `ollama pull llama3.1:70b` | ~40GB* | 10–15 tok/s |
| Mistral 7B | `ollama pull mistral:7b` | ~4.5GB | 110–130 tok/s |
| Qwen2.5 32B | `ollama pull qwen2.5:32b` | ~19GB | 25–35 tok/s |

\* The 70B model at Q4 quantization requires approximately 40GB. With 24GB VRAM, Ollama will automatically offload some layers to system RAM. Performance will be lower than full-VRAM operation but still usable (~10–15 tok/s).

**Recommended starting point**: `llama3.1:8b` for quick responses, `qwen2.5-coder:32b` for coding work.

### List Installed Models

```bash
ollama list
```

### Remove a Model

```bash
ollama rm <model-name>
```

---

## 10. Open WebUI Installation

Open WebUI provides a polished, browser-based chat interface — similar to ChatGPT's interface but running entirely locally and connecting to Ollama.

```bash
docker run -d \
  --name open-webui \
  --restart always \
  --gpus all \
  -p 3000:8080 \
  -v open-webui:/app/backend/data \
  -e OLLAMA_BASE_URL=http://192.168.1.192:11434 \
  ghcr.io/open-webui/open-webui:main
```

> **Note**: Use your server's actual IP address rather than `host.docker.internal`. On Ubuntu Server, `host.docker.internal` does not resolve reliably from within Docker containers, causing connection failures. Find your server's IP with `ip addr show | grep "inet " | grep -v 127`.

**What each option does:**

| Option | Purpose |
|--------|---------|
| `--name open-webui` | Names the container |
| `--restart always` | Auto-starts after reboot |
| `--gpus all` | Gives container access to RTX 3090 |
| `-p 3000:8080` | Maps port 3000 on your machine to port 8080 inside the container |
| `-v open-webui:/app/backend/data` | Persistent storage for chat history, settings |
| `-e OLLAMA_BASE_URL=...` | Tells Open WebUI where to find Ollama — use the server's actual IP address |

### Access Open WebUI

Open a browser on the AI PC (or any device on your network) and go to:

```
http://localhost:3000
```

Or from another device on your network:

```
http://<AI-PC-IP-ADDRESS>:3000
```

Find your IP address with: `ip addr show | grep "inet "`

### First-Time Setup in Open WebUI

1. Create an admin account (first account created is admin)
2. Select a model from the dropdown (your downloaded Ollama models will appear)
3. Start chatting

### Updating Open WebUI

Open WebUI releases updates frequently. To update:

```bash
docker stop open-webui
docker rm open-webui
docker pull ghcr.io/open-webui/open-webui:main
# Re-run the same docker run command from above
```

---

## 11. Storage Configuration Summary

After setup, your storage is organised as:

| Drive | Mount | Contents | Available Space |
|-------|-------|----------|----------------|
| Samsung 9100 Pro #1 (M.2_1) | `/` and `/home` | Ubuntu OS, applications, Docker data | ~1.8TB |
| Samsung 9100 Pro #2 (M.2_2) | `/mnt` (models at `/mnt/models`) | Ollama model library | 2TB |

### How Much Space Do Models Use?

| Model Size | Q4 Quantization | Q8 Quantization | FP16 (Full Precision) |
|------------|----------------|-----------------|----------------------|
| 7B | ~4.5 GB | ~8 GB | ~14 GB |
| 13B | ~8 GB | ~14 GB | ~26 GB |
| 34B | ~20 GB | ~35 GB | ~68 GB |
| 70B | ~40 GB | ~70 GB | ~140 GB |

With 2TB of model storage, you can hold approximately:
- 50+ 7B models, or
- 10–15 34B models, or
- 4–5 70B models, or
- A practical mix of many models at different sizes

---

## 12. Performance Tuning

### GPU Power Limit (Optional — Reduces Noise and Heat)

The RTX 3090 has a 350W TDP. Reducing this to 280–300W reduces fan noise and heat with minimal performance impact (~3–5%):

```bash
# Check current power limit
sudo nvidia-smi -q | grep -i "power limit"

# Set power limit to 300W (85% of max)
sudo nvidia-smi -pl 300

# To make this permanent at boot, create a systemd service
sudo nano /etc/systemd/system/nvidia-power-limit.service
```

Add the following content:

```ini
[Unit]
Description=Set NVIDIA GPU Power Limit
After=nvidia-driver.service

[Service]
Type=oneshot
ExecStart=/usr/bin/nvidia-smi -pl 300
RemainAfterExit=yes

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl enable nvidia-power-limit.service
sudo systemctl start nvidia-power-limit.service
```

### CPU Performance Governor (Optional)

For consistent AI inference performance, set the CPU to performance mode:

```bash
sudo apt install -y cpufrequtils
echo 'GOVERNOR="performance"' | sudo tee /etc/default/cpufrequtils
sudo systemctl restart cpufrequtils
```

### Swap Configuration

For running large models (70B) that partially offload to RAM, ensure swap is configured. Check current swap:

```bash
free -h
```

Ubuntu 24.04 creates a swap file by default. For this build with 64GB RAM, the default swap is typically sufficient for model offloading. If you need more:

```bash
# Check swap file location and size
sudo swapon --show

# To increase swap to 32GB (if needed for very large models)
sudo swapoff /swapfile
sudo fallocate -l 32G /swapfile
sudo mkswap /swapfile
sudo swapon /swapfile
```

### Ollama Concurrent Requests

By default Ollama handles one request at a time. For running multiple models or users:

```bash
sudo systemctl edit ollama
```

Add:

```ini
[Service]
Environment="OLLAMA_NUM_PARALLEL=2"
Environment="OLLAMA_MAX_LOADED_MODELS=2"
Environment="OLLAMA_MODELS=/mnt/models/ollama"
```

**Caution**: Running two models simultaneously shares the 24GB VRAM. Keep models small enough that both fit (e.g., two 7B models = ~10GB combined, comfortable in 24GB).

---

## 13. Firewall Configuration

Ubuntu 24.04 includes UFW (Uncomplicated Firewall). There is one important caveat with Docker: **Docker bypasses UFW** by writing its own iptables rules directly. This means UFW cannot block Docker's published ports (such as port 3000 for Open WebUI) from LAN access. For a home server this is acceptable — UFW still protects all host-level services such as SSH and Ollama.

```bash
# Set defaults — block all incoming, allow all outgoing
sudo ufw default deny incoming
sudo ufw default allow outgoing

# Allow SSH — do this BEFORE enabling or you will lock yourself out
sudo ufw allow ssh

# Allow all traffic on the Tailscale interface (remote access)
sudo ufw allow in on tailscale0

# Allow Docker bridge network to reach Ollama (required for Open WebUI container)
sudo ufw allow from 172.17.0.0/16 to any port 11434

# Enable the firewall
sudo ufw enable

# Verify
sudo ufw status verbose
```

Find your network range if needed: `ip route | grep "proto kernel"`

---

## 14. Tailscale — Secure Remote Access

Tailscale creates an encrypted private network (a VPN mesh) between your devices. Once connected, you can access Open WebUI from anywhere in the world without opening any ports on your router.

**How it works**: Each device on your Tailscale account gets a private IP in the `100.x.x.x` range. Devices communicate directly with each other over an encrypted tunnel, regardless of where they are.

### Tailscale on the AI Server

Tailscale is already installed and running on this server. Verify it:

```bash
# Check Tailscale is running
sudo systemctl status tailscaled

# Check your Tailscale IP address
tailscale ip -4

# Check connection status
tailscale status
```

Your server's Tailscale IP is `100.79.83.113`. This address is stable — it does not change when you reboot or reconnect.

Tailscale starts automatically on boot. Verify this is enabled:

```bash
sudo systemctl is-enabled tailscaled
```

Should return `enabled`.

### Install Tailscale on Your Other Devices

Install the Tailscale client on every device you want to use to access Open WebUI remotely:

| Platform | Download |
|----------|---------|
| Windows | <a href="https://tailscale.com/download/windows" target="_blank">tailscale.com/download/windows</a> |
| macOS | <a href="https://tailscale.com/download/mac" target="_blank">tailscale.com/download/mac</a> |
| Android | Available on Google Play Store |
| iOS | Available on Apple App Store |

Log in with the same Tailscale account on each device.

### Accessing Open WebUI Remotely

Once your device is connected to Tailscale, open a browser and go to:

```
https://amelai.tail926601.ts.net
```

This works from anywhere — home network, mobile data, coffee shop Wi-Fi — without any router port forwarding.

### Security Notes

- Only devices logged into **your** Tailscale account can access this address
- Traffic is encrypted end-to-end using WireGuard
- Open WebUI itself requires login (the admin account you created), providing a second layer of authentication
- The Ollama API (port 11434) is accessible on the Tailscale network — this is acceptable for personal use but be aware if you share your Tailscale account with others

### If Tailscale Disconnects

```bash
# Reconnect
sudo tailscale up

# Check status
tailscale status
```

---

## 15. Ethernet NIC — Link Speed Configuration

The MSI MAG X870E TOMAHAWK WIFI has a Realtek RTL8126 **5Gb** NIC (`enp7s0`). By default
it advertises all supported speeds (10/100/1000/2500/5000 Mb/s) during autonegotiation.

### Current Home Network Constraint

The home network uses a **Linksys WHW03 V2** router which has **1Gb** ethernet ports. The
RTL8126 advertising 2.5Gb and 5Gb modes to a 1Gb router caused link flapping (repeated
up/down events). The fix restricts the NIC to only advertise 1Gb.

A systemd service (`fix-ethernet.service`) applies this at every boot:

```bash
# View the service
sudo systemctl status fix-ethernet.service

# Service file location
sudo cat /etc/systemd/system/fix-ethernet.service
```

### Re-enabling Higher Speeds After Router Upgrade

When you upgrade to a router or switch that supports 2.5Gb or 5Gb ethernet, remove or
disable the service and restore full advertisement:

```bash
# Disable the workaround service
sudo systemctl disable fix-ethernet.service
sudo systemctl stop fix-ethernet.service

# Restore full advertisement (all supported speeds)
sudo ethtool -s enp7s0 advertise 0x1FF autoneg on

# Verify
ethtool enp7s0 | grep "Advertised link"
```

The NIC will then autonegotiate at the highest speed both sides support (up to 5Gb).

If you want to permanently remove the service entirely:

```bash
sudo rm /etc/systemd/system/fix-ethernet.service
sudo systemctl daemon-reload
```

---

## 16. Auto-Start on Boot — Summary

After setup, verify these services start automatically:

```bash
# Check all services are enabled
systemctl is-enabled ollama          # Should return: enabled
systemctl is-enabled docker          # Should return: enabled
docker inspect --format='{{.HostConfig.RestartPolicy.Name}}' open-webui  # Should return: always
```

Reboot the system and verify everything comes back up automatically:

```bash
sudo reboot
# After boot:
sudo systemctl status ollama
docker ps  # Should show open-webui container running
```

---

## 16. Testing and Verification

Run this sequence to confirm the full stack is working:

```bash
# 1. GPU is detected
nvidia-smi

# 2. Ollama is running and using GPU
ollama list
curl http://localhost:11434/api/tags  # Should return JSON list of models

# 3. GPU resource monitor (watch during inference)
nvtop

# 4. Run a quick inference test
ollama run llama3.2:7b "What is 2+2? Reply in one word."

# 5. Open WebUI is accessible
curl http://localhost:3000  # Should return HTML
```

### Performance Benchmarks

Once everything is running, benchmark your setup:

```bash
# Install ollama-benchmark or use this quick timing test
time echo "Explain the difference between supervised and unsupervised learning in 100 words." | ollama run llama3.1:8b
```

**Expected performance with RTX 3090 24GB:**

| Model | Expected Speed |
|-------|---------------|
| Llama 3.1 8B (Q4) | 90–120 tok/s |
| Qwen2.5 32B (Q4) | 25–35 tok/s |
| Llama 3.1 70B (Q4, with RAM offload) | 8–15 tok/s |

---

## 17. Troubleshooting

### Ollama Not Using GPU

```bash
# Check if NVIDIA driver is loaded
lsmod | grep nvidia

# Check Ollama logs for GPU detection
sudo journalctl -u ollama -n 50

# Re-check nvidia-smi works
nvidia-smi
```

If CUDA is not available, reinstall NVIDIA driver and reboot.

### Open WebUI Can't Connect to Ollama

```bash
# Verify Ollama is listening and returning models
curl http://localhost:11434/api/tags

# Check Open WebUI container logs for connection errors
docker logs open-webui
```

If you see `Cannot connect to host host.docker.internal` in the logs, recreate the container using the server's actual IP address instead:

```bash
docker stop open-webui && docker rm open-webui

docker run -d \
  --name open-webui \
  --restart always \
  --gpus all \
  -p 3000:8080 \
  -v open-webui:/app/backend/data \
  -e OLLAMA_BASE_URL=http://192.168.1.192:11434 \
  ghcr.io/open-webui/open-webui:main
```

> `host.docker.internal` does not resolve reliably on Ubuntu Server. Using the server's LAN IP directly is more reliable.

### Model Download Fails

```bash
# Check disk space on model drive
df -h /mnt

# Check Ollama is using the correct path
sudo journalctl -u ollama | grep OLLAMA_MODELS

# Manual model download with verbose output
OLLAMA_DEBUG=1 ollama pull llama3.1:8b
```

### GPU Memory Error During Model Load

This means the model is too large for VRAM + available system RAM offload:

```bash
# Check VRAM usage
nvidia-smi

# List loaded models in Ollama
curl http://localhost:11434/api/ps

# Unload running models
ollama stop <model-name>

# Try a smaller model or more heavily quantized version
ollama pull llama3.1:70b-instruct-q2_K  # Smaller quantization = less VRAM
```

### System Runs Hot

```bash
# Monitor all temperatures
sensors  # (may need: sudo apt install lm-sensors && sudo sensors-detect)
nvtop    # GPU temp
htop     # CPU usage

# Check fan speeds
sudo apt install fancontrol
sudo pwmconfig
```

---

## 18. Quick Reference — Common Commands

```bash
# Ollama
ollama list                          # List installed models
ollama pull <model>                  # Download a model
ollama rm <model>                    # Remove a model
ollama run <model>                   # Interactive chat in terminal
ollama stop <model>                  # Unload model from VRAM
sudo systemctl restart ollama        # Restart Ollama service
sudo journalctl -u ollama -f         # Follow Ollama logs

# Docker
docker ps                            # List running containers
docker logs open-webui               # Open WebUI logs
docker restart open-webui            # Restart Open WebUI
docker stop open-webui               # Stop Open WebUI

# NVIDIA
nvidia-smi                           # GPU status
nvidia-smi -l 1                      # Continuous GPU stats (every 1 second)
nvtop                                # Interactive GPU monitor
nvidia-smi -pl 300                   # Set power limit to 300W

# tmux
tmux new-session -s <name>           # Start a new named session
tmux attach -t <name>                # Reattach to a session
tmux ls                              # List all active sessions
tmux kill-session -t <name>         # Kill a session
# Ctrl+B, then D                    # Detach from current session (keeps it running)

# Tailscale
tailscale status                     # Show connected devices
tailscale ip -4                      # Show this machine's Tailscale IP
sudo tailscale up                    # Connect/reconnect to Tailscale
sudo systemctl status tailscaled     # Check Tailscale service

# Firewall
sudo ufw status verbose              # Show firewall rules
sudo ufw allow <port>                # Open a port
sudo ufw deny <port>                 # Block a port

# System
free -h                              # RAM usage
df -h                                # Disk usage
htop                                 # Process monitor
ip addr                              # Network interfaces and IP addresses
sudo systemctl status <service>      # Check service status
```

---

## 19. Resources

- <a href="https://ubuntu.com/tutorials/install-ubuntu-server" target="_blank">Ubuntu Server Installation Tutorial</a>
- <a href="https://ollama.com/library" target="_blank">Ollama Model Library</a>
- <a href="https://github.com/open-webui/open-webui" target="_blank">Open WebUI GitHub Repository</a>
- <a href="https://docs.nvidia.com/cuda/cuda-installation-guide-linux/" target="_blank">NVIDIA CUDA Installation Guide for Linux</a>
- <a href="https://docs.docker.com/engine/install/ubuntu/" target="_blank">Docker Engine Installation for Ubuntu</a>
- <a href="https://docs.nvidia.com/datacenter/cloud-native/container-toolkit/install-guide.html" target="_blank">NVIDIA Container Toolkit Installation Guide</a>
- <a href="https://tailscale.com/kb/1017/install" target="_blank">Tailscale Installation Guide</a>

---

**Document Version**: 1.3
**Last Updated**: March 3, 2026
**Changes**: Added tmux section (Section 8) covering safe large model downloads, detach/attach workflow, and session management; added tmux to initial apt install command (Section 4); added tmux commands to Quick Reference (Section 18)
