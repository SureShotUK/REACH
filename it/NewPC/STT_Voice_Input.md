# Speech-to-Text Voice Input for Claude Code

Dictate prompts into any Claude Code terminal session — local Windows Terminal or SSH into amelai — using a push-to-toggle microphone. Processing runs on amelai (GPU 1, RTX 3090) using Whisper large-v3 for maximum accuracy.

---

## Architecture

```
Windows 11 PC                           amelai (Tailscale / LAN)
────────────────────                    ──────────────────────────────────
Mic → sounddevice                       stt_server.py (port 9090)
stt_client.py  ──── WebSocket ────>     RMS energy VAD + faster-whisper
clipboard paste <───── text ────────    systemd service, GPU 1
F9 hotkey (global, elevated)
System tray icon (grey/green)
```

**Flow**: Press F9 → tray icon turns green → speak → ~1 s pause → text is pasted into the focused window → press Enter to submit.

Text is never auto-submitted. You can review and edit before pressing Enter.

---

## Prerequisites

- Tailscale running on both machines, connected to work account
- Python 3.11+ on the Windows 11 PC
- Python 3.10+ and CUDA 12.x on amelai (already installed)
- amelai reachable at `amelai.tail926601.ts.net`

---

## Part 1 — Server Setup on amelai

Run all commands over SSH on amelai.

### 1.1 Create directory and Python venv

```bash
sudo mkdir -p /opt/stt
sudo chown steve:steve /opt/stt
python3 -m venv /opt/stt/venv
source /opt/stt/venv/bin/activate
```

### 1.2 Install dependencies

```bash
pip install --upgrade pip
# torch from the cu121 wheel — PyPI ships CUDA 13 builds which break on CUDA 12.x
pip install torch --index-url https://download.pytorch.org/whl/cu121
# No silero-vad or torchaudio — server uses simple RMS energy VAD (no extra deps)
pip install faster-whisper websockets numpy
```

### 1.3 Copy the server script

```bash
cp /docs/terminai/it/NewPC/stt/stt_server.py /opt/stt/
```

### 1.4 Test the server manually first

```bash
source /opt/stt/venv/bin/activate
CUDA_VISIBLE_DEVICES=1 python /opt/stt/stt_server.py
```

Expected output (Whisper model downloads ~3 GB on first run):
```
INFO Loading faster-whisper large-v3 on CUDA …
INFO Model ready — server starting.
INFO Listening on 0.0.0.0:9090
```

No command prompt returns — the server blocks waiting for connections. Press Ctrl+C once confirmed.

### 1.5 Install as systemd service

```bash
sudo cp /docs/terminai/it/NewPC/stt/stt_server.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable stt_server
sudo systemctl start stt_server
sudo systemctl status stt_server
```

### 1.6 Firewall rule

```bash
sudo ufw allow from 100.64.0.0/10 to any port 9090 proto tcp comment "STT server - Tailscale"
```

---

## Part 2 — Client Setup on Windows 11

Open an **Administrator** PowerShell or Windows Terminal.

### 2.1 Install Python packages

```powershell
pip install sounddevice websockets keyboard pyperclip numpy pystray Pillow
```

### 2.2 Create the client directory and copy the script

```powershell
New-Item -ItemType Directory -Path "C:\Users\SteveIrwin\stt" -Force
Copy-Item "\\irwinnas\MyDocs\terminai\it\NewPC\stt\stt_client.py" "C:\Users\SteveIrwin\stt\"
```

### 2.3 Test the client

```powershell
python C:\Users\SteveIrwin\stt\stt_client.py
```

A grey circle should appear in the system tray. Hover over it — tooltip should say **"STT — Connecting to amelai…"** briefly, then **"STT — Paused (F9 to start)"** once connected.

Press F9 → icon turns green → speak → text appears pasted in the focused window.

### 2.4 Set up auto-start with elevated privileges

> **Critical**: The `keyboard` global hook requires elevated privileges. VBS or BAT files launched with a hidden window (`WshShell.Run ... 0`) break the hook — the tray icon appears but F9 and transcription do not work. Task Scheduler with "Run with highest privileges" is the correct solution.

Run each line separately in an Administrator PowerShell:

```powershell
$action = New-ScheduledTaskAction -Execute "C:\Users\SteveIrwin\AppData\Local\Programs\Python\Python314\pythonw.exe" -Argument "C:\Users\SteveIrwin\stt\stt_client.py"
```
```powershell
$trigger = New-ScheduledTaskTrigger -AtLogOn -User "$env:USERDOMAIN\$env:USERNAME"
```
```powershell
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries
```
```powershell
$principal = New-ScheduledTaskPrincipal -UserId "$env:USERDOMAIN\$env:USERNAME" -RunLevel Highest
```
```powershell
Register-ScheduledTask -TaskName "STT Client" -Action $action -Trigger $trigger -Settings $settings -Principal $principal
```

To test without rebooting:
```powershell
Start-ScheduledTask -TaskName "STT Client"
```

> **Note**: The `pythonw.exe` path above is for Python 3.14. If your Python version differs, find the correct path with `where pythonw` in PowerShell.

---

## Usage

| Action | Result |
|---|---|
| Press F9 | Tray icon turns green — mic active |
| Speak your prompt | Audio streams to amelai |
| Pause ~1 s | Whisper transcribes, text is pasted into focused window |
| Continue speaking | More text appears as each sentence is processed |
| Press F9 again | Tray icon turns grey — mic off |
| Press Enter in terminal | Submit the dictated prompt to Claude |

### Tips

- Speak naturally at normal pace — no need to slow down or pause between words
- If a word is wrong, correct it in the terminal before pressing Enter
- The tray tooltip shows connection status — if it stays on "Connecting…" check Tailscale is on the work account
- `ENERGY_THRESHOLD` and `SILENCE_FRAMES` in `stt_server.py` can be tuned if the VAD is too sensitive or cuts off early (current values: 0.025 and 50)

---

## VRAM usage

Whisper large-v3 uses ~3 GB VRAM on GPU 1. It coexists with Ollama. If ComfyUI-Steve is also loaded on GPU 1, there may be contention.

```bash
watch -n 2 nvidia-smi --query-gpu=index,memory.used,memory.free --format=csv
```

Switch to `medium` (~1.5 GB) if needed in `stt_server.py`:
```python
whisper = WhisperModel("medium", device="cuda", compute_type="float16")
```

---

## Troubleshooting

| Symptom | Fix |
|---|---|
| Tray tooltip stuck on "Connecting…" | Check Tailscale is connected to work account; check `sudo systemctl status stt_server` on amelai |
| F9 does nothing | Task Scheduler task must use "Run with highest privileges" — VBS/BAT hidden window breaks keyboard hooks |
| Tray icon appears but no transcription | Same as above — keyboard hook not elevated; recreate task with `RunLevel Highest` |
| Transcription cuts off mid-sentence | Raise `SILENCE_FRAMES` in `stt_server.py` (try 60 = 1.2 s) |
| Background noise triggers false transcriptions | Raise `ENERGY_THRESHOLD` in `stt_server.py` (try 0.035) |
| Server OOM on GPU | Switch to `medium` model; free ComfyUI VRAM first |

---

## Service management

```bash
# On amelai
sudo systemctl stop stt_server
sudo systemctl start stt_server
sudo systemctl restart stt_server
journalctl -u stt_server --since "10 minutes ago"
tail -f /opt/stt/stt.log
```

---

## Files reference

| File | Location | Purpose |
|---|---|---|
| `stt_server.py` | `/opt/stt/` on amelai | WebSocket server + Whisper + energy VAD |
| `stt_server.service` | `/etc/systemd/system/` on amelai | systemd unit |
| `stt_client.py` | `C:\Users\SteveIrwin\stt\` on Windows | Audio capture, tray icon, text injection |
| `start_stt.vbs` | `C:\Users\SteveIrwin\stt\` on Windows | Legacy launcher (superseded by Task Scheduler) |
| Source files | `\\irwinnas\MyDocs\terminai\it\NewPC\stt\` | Version-controlled originals |
