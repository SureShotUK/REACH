# STT Client — New Machine Setup Guide

Install the speech-to-text client on any Windows machine in the tailnet. The server (amelai) is already running — you only need to set up the client side.

---

## Prerequisites

- **Tailscale** installed and connected to the **work account** (not personal)
- **Python 3.11+** installed (note your `pythonw.exe` path — needed for Task Scheduler)
- amelai reachable: ping `amelai.tail926601.ts.net` should return a `100.x.x.x` address
- NAS accessible: `\\irwinnas\MyDocs\` mounted or reachable

---

## Step 1 — Find your pythonw.exe path

This is needed for the Task Scheduler step. Run in PowerShell:

```powershell
where pythonw
```

Note the full path returned, e.g.:
`C:\Users\YourName\AppData\Local\Programs\Python\Python314\pythonw.exe`

---

## Step 2 — Create client directory and copy script

```powershell
New-Item -ItemType Directory -Path "C:\Users\YourName\stt" -Force
Copy-Item "\\irwinnas\MyDocs\terminai\it\NewPC\stt\stt_client.py" "C:\Users\YourName\stt\"
```

Replace `YourName` with your Windows username throughout.

---

## Step 3 — Install Python packages

```powershell
pip install sounddevice websockets keyboard pyperclip numpy pystray Pillow
```

---

## Step 4 — Test manually first

```powershell
python C:\Users\YourName\stt\stt_client.py
```

A grey circle appears in the system tray. Hover — tooltip should say **"STT — Connecting to amelai…"** then **"STT — Paused (F9 to start)"**.

Press F9 → icon turns green → speak → text should paste into the focused window.

Press Ctrl+C to stop once confirmed working.

---

## Step 5 — Set up auto-start with elevated privileges

> **Important**: The client MUST run with elevated privileges for the global F9 hotkey to work. VBS/BAT files with a hidden window (`WshShell.Run ... 0`) silently break the keyboard hook — the tray icon appears but nothing works. Task Scheduler with `RunLevel Highest` is the only reliable method.

Open an **Administrator** PowerShell and run each line separately, substituting your `pythonw.exe` path and username:

```powershell
$action = New-ScheduledTaskAction -Execute "C:\Users\YourName\AppData\Local\Programs\Python\Python314\pythonw.exe" -Argument "C:\Users\YourName\stt\stt_client.py"
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

Test without rebooting:

```powershell
Start-ScheduledTask -TaskName "STT Client"
```

---

## Step 6 — Reboot and verify

After rebooting, the tray icon should appear automatically within a few seconds of login (once Tailscale connects). If it shows "Connecting…" for more than 30 seconds, check Tailscale is on the work account.

---

## Usage

| Action | Result |
|---|---|
| Press F9 | Tray icon turns green — mic active |
| Speak | Audio streams to amelai for transcription |
| Pause ~1 s | Text pasted into focused window |
| Press F9 again | Tray icon turns grey — mic off |
| Press Enter | Submit dictated text to Claude |

---

## Troubleshooting

| Symptom | Fix |
|---|---|
| Tray stuck on "Connecting…" | Tailscale not on work account, or `stt_server` not running on amelai |
| F9 does nothing / no transcription | Task not set to `RunLevel Highest` — delete and recreate with the commands above |
| amelai resolves to `176.x.x.x` instead of `100.x.x.x` | Tailscale signed in to wrong account or not running |
| Tray icon doesn't appear at startup | Check Task Scheduler → Task Scheduler Library → "STT Client" → Last Run Result |

### Remove the task (if needed)

```powershell
Unregister-ScheduledTask -TaskName "STT Client" -Confirm:$false
```

---

## Server details (for reference)

The server runs permanently on amelai as a systemd service. No server-side changes are needed when adding a new client machine.

| Detail | Value |
|---|---|
| WebSocket URL | `ws://amelai.tail926601.ts.net:9090` |
| GPU | RTX 3090 GPU 1 (~3 GB VRAM) |
| Model | Whisper large-v3 |
| Service | `sudo systemctl status stt_server` |
