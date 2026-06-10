# STT System — Speech-to-Text for amelai

*Documentation for `stt_server.py` (amelai) and `stt_client.py` (Windows 11)*

---

## Overview

The STT system provides real-time speech-to-text transcription across the local network and Tailscale VPN. You speak into the microphone on a Windows 11 PC; transcribed text appears typed into whatever application is currently focused — as if you had typed it.

The system has two components:

| Component | Runs on | Role |
|---|---|---|
| `stt_server.py` | amelai (Ubuntu 24.04) | Receives audio over WebSocket, runs Whisper transcription on GPU, returns text |
| `stt_client.py` | Windows 11 PC | Captures microphone audio, streams to server, pastes returned text into the focused window |

```
Windows 11 PC                          amelai (GPU server)
┌─────────────────────┐                ┌──────────────────────────────┐
│  stt_client.py      │                │  stt_server.py               │
│  ─────────────────  │  WebSocket     │  ─────────────────────────── │
│  Microphone audio   │ ─────────────► │  RMS VAD → speech detection  │
│  (16kHz, int16 PCM) │                │  faster-whisper large-v3     │
│                     │ ◄───────────── │  (CUDA, float16)             │
│  Paste text into    │  Transcribed   │                              │
│  focused window     │  text          │  ~3 GB VRAM while loaded     │
└─────────────────────┘                └──────────────────────────────┘
        │                                          │
        └────── Tailscale (port 9090) ─────────────┘
```

**Key design decisions:**

- **Server-side processing**: The heavy Whisper model runs on amelai's RTX 3090 GPU. The Windows client is lightweight.
- **Lazy VRAM loading**: The model is not loaded at server startup. It loads on the first transcription request and unloads after 15 minutes of idle, freeing VRAM for ComfyUI and Ollama.
- **Simple VAD**: Voice activity detection uses a plain RMS energy threshold — no additional ML models or dependencies.
- **Direct clipboard paste**: The client uses Windows `SendInput` to paste text, avoiding interference with the system keyboard hook.

---

## Server (`stt_server.py`)

### Prerequisites

| Requirement | Version | Notes |
|---|---|---|
| OS | Ubuntu 24.04 LTS | amelai |
| Python | 3.10+ | System Python or venv |
| CUDA | 12.x | `nvidia-smi` should show driver 525+ |
| GPU | RTX 3090 (GPU 1) | ~3 GB VRAM while model is loaded |
| faster-whisper | 1.0.0+ | See installation below |
| websockets | 12.0+ | |
| numpy | 1.24.0+ | |

### Installation

The server runs inside a Python virtual environment at `/opt/stt/venv`.

```bash
# Create the venv (one-time)
sudo mkdir -p /opt/stt
sudo chown $USER /opt/stt
python3 -m venv /opt/stt/venv
source /opt/stt/venv/bin/activate

# Install dependencies
pip install faster-whisper>=1.0.0 websockets>=12.0 numpy>=1.24.0
```

> **Note on torch/torchaudio:** This server does **not** use torch or torchaudio — VAD is a plain RMS energy check. Do not install them unless you have a specific reason; PyPI ships CUDA 13 builds which break on CUDA 12.x.

Copy `stt_server.py` to `/opt/stt/`:

```bash
cp stt_server.py /opt/stt/
```

The server writes logs to `/opt/stt/stt.log`.

### Running as a systemd service

The service file `stt_server.service` (in this directory) configures automatic startup:

```bash
# Install the service
sudo cp stt_server.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable stt_server
sudo systemctl start stt_server
```

Check status and logs:

```bash
sudo systemctl status stt_server
sudo journalctl -u stt_server -f          # live log
tail -f /opt/stt/stt.log                   # server's own log file
```

### What is the venv?

A **virtual environment** (venv) is simply an isolated copy of Python with its own set of installed packages, kept in the `/opt/stt/venv/` folder. It exists so that the STT server's packages (faster-whisper, websockets, etc.) cannot conflict with anything else on the system.

You do not normally need to interact with the venv at all. The systemd service already points directly to the venv's Python binary (`/opt/stt/venv/bin/python`), so it always uses the right packages automatically.

The only time you need to "activate" or touch the venv is when installing new or updated packages — not when updating the script file itself.

### Updating the server script

Updating `stt_server.py` is straightforward — you are just replacing a text file. The venv and its packages are untouched.

**Step 1 — Copy the new script to amelai**

From the Windows 11 PC, copy the updated file over. The simplest method is to open a terminal and use `scp` via Tailscale:

```bash
scp stt_server.py steve@amelai.tail926601.ts.net:/opt/stt/stt_server.py
```

Or, if you are already in an SSH session on amelai, copy the file however is convenient (USB, shared folder, paste into a text editor, etc.).

**Step 2 — Restart the service**

SSH into amelai and run:

```bash
sudo systemctl restart stt_server
```

That is all. The service stops, picks up the new script file, and starts again. The venv and all installed packages remain exactly as they were.

**Step 3 — Verify it started cleanly**

```bash
sudo systemctl status stt_server
```

You should see `Active: active (running)`. The startup log line will read:

```
Listening on 0.0.0.0:9090 — model loads on first speech, unloads after 15 min idle
```

If there is a Python error in the new script, it will appear here and the service will show `failed`. Fix the script and repeat from Step 1.

---

### Updating packages (if `requirements_server.txt` has changed)

If a new version of a dependency is needed (not just a script change), you need to install into the venv. This is the only time you interact with it directly.

```bash
# SSH into amelai, then:

# 1. Stop the service first (you cannot update packages while the process is running)
sudo systemctl stop stt_server

# 2. Activate the venv — this changes your shell's Python to the venv's version
source /opt/stt/venv/bin/activate

# Your prompt will change to show (venv) at the start, confirming it is active:
# (venv) steve@amelai:~$

# 3. Install or upgrade packages
pip install -r /opt/stt/requirements_server.txt

# 4. Deactivate the venv — returns your shell to normal (optional but tidy)
deactivate

# 5. Restart the service
sudo systemctl start stt_server
```

Once you type `deactivate`, the `(venv)` prefix disappears and your shell is back to normal. The venv itself is unchanged — `deactivate` just ends the shell session's use of it.

---

### VRAM management

The server uses a **lazy load / idle unload** strategy to avoid permanently occupying VRAM that ComfyUI and Ollama also need.

| Event | Action |
|---|---|
| Server starts | No VRAM used. WebSocket server listens on port 9090. |
| First speech detected | Model loads (~30 s, logged to console and stt.log) |
| Active use | Model stays loaded. Timer resets on every transcription. |
| 15 minutes with no transcription | Model unloaded. VRAM freed. (~3 GB returned) |
| Next speech after unload | Model reloads automatically (another ~30 s delay) |

**Implication:** The first utterance after a long idle will have a 20–30 second delay before text appears. Subsequent utterances respond immediately.

To check current VRAM usage:

```bash
nvidia-smi --query-gpu=index,memory.used,memory.free --format=csv
```

### Configuration

All tunable constants are at the top of `stt_server.py`:

| Constant | Default | Effect |
|---|---|---|
| `ENERGY_THRESHOLD` | `0.025` | RMS level that counts as speech. Raise if background noise triggers false transcriptions; lower if quiet speech is missed. |
| `SILENCE_FRAMES` | `50` | Frames of silence (each 20 ms) before an utterance is considered complete. Default = 1000 ms. Raise to capture longer pauses mid-sentence. |
| `MODEL_IDLE_TIMEOUT` | `900` | Seconds before the model is unloaded. Default = 15 minutes. |
| `MIN_SPEECH_SEC` | `0.3` | Minimum audio length to attempt transcription. Shorter clips are discarded. |
| `PRE_ROLL_FRAMES` | `5` | Frames captured before speech starts (prevents clipped first words). Default = 100 ms. |
| `ENERGY_THRESHOLD` in `_transcribe_and_send` — `initial_prompt` | `"Claude, Tailscale, amelai, ComfyUI, Python, Docker, systemd, Linux"` | Whisper prompt hint. Add domain-specific words here to improve recognition accuracy for uncommon terms. |

### How it works

1. A client connects over WebSocket.
2. The client streams raw 16 kHz, 16-bit PCM audio in 100 ms chunks.
3. The server reassembles the audio into 20 ms frames and checks each frame's RMS energy against `ENERGY_THRESHOLD`.
4. When speech is detected, frames are accumulated in a buffer (with a 100 ms pre-roll to avoid clipping).
5. After 1000 ms of consecutive silence following speech, the buffer is passed to faster-whisper.
6. Whisper runs with its own internal VAD filter (`vad_filter=True`) to suppress hallucinations on non-speech noise.
7. The transcribed text is sent back to the client over the same WebSocket connection.
8. An idle monitor task runs every 60 seconds and unloads the model if no transcription has occurred in the past 15 minutes.

### Troubleshooting

**Server starts but model never loads / no transcription appears**

Check that speech is actually reaching the server. Look in `/opt/stt/stt.log` — you should see `→ "transcribed text"` lines when speech is processed. If nothing appears, the RMS threshold may be too high for your microphone. Try lowering `ENERGY_THRESHOLD` from `0.025` to `0.010`.

**"Loading faster-whisper" log entry never appears / first words always lost**

This is expected behaviour — the model loads on the first speech event, which takes ~30 seconds. The utterance that triggered loading will still be transcribed once ready (the audio is buffered). Subsequent utterances respond immediately.

**`CUDA error: out of memory`**

Another process (ComfyUI, Ollama) has consumed the VRAM. Check with `nvidia-smi`. Free ComfyUI VRAM using the bookmarklet saved in Edge Favourites ("Free ComfyUI VRAM"), or restart the ComfyUI container. See `CLAUDE.md` VRAM contention table for typical model sizes.

**Service fails to start after OS reboot**

The CUDA driver may not be ready when systemd starts the service. Check `stt_server.service` — it should have `After=network.target` and ideally a short `ExecStartPre=/bin/sleep 5` if the GPU driver loads late.

**`ModuleNotFoundError: No module named 'faster_whisper'`**

The venv is not being used. Check that `stt_server.service` references the venv Python explicitly: `ExecStart=/opt/stt/venv/bin/python /opt/stt/stt_server.py`.

---

## Client (`stt_client.py`)

### Prerequisites

| Requirement | Version | Notes |
|---|---|---|
| OS | Windows 11 | |
| Python | 3.11+ | Install from python.org — use the standard installer, add to PATH |
| Tailscale | Current | Must be connected; amelai must be reachable at `amelai.tail926601.ts.net` |
| Microphone | Any | Default system microphone is used |

Python packages (install once):

```
pip install sounddevice>=0.4.6 websockets>=12.0 keyboard>=0.13.5 pyperclip>=1.8.2 numpy>=1.24.0 pystray>=0.19.0 Pillow>=10.0.0
```

> **Note:** `keyboard` requires running as Administrator on some Windows 11 configurations for the global hotkey hook to work. If F9 does not respond, right-click the terminal / `pythonw` shortcut and choose *Run as administrator*.

### Installation

1. Copy `stt_client.py` to a permanent location, e.g. `C:\Tools\STT\stt_client.py`
2. Copy `start_stt.vbs` to the same folder (this launches the client with no console window)
3. To auto-start with Windows, place a shortcut to `start_stt.vbs` in:
   ```
   %APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup
   ```

The `start_stt.bat` script is for manual launches with a visible console (useful for debugging).
The `restart_stt.bat` script kills any running instance and relaunches cleanly.

### Usage

Once running, the client appears as a circle icon in the system tray (notification area, bottom-right).

| Tray icon | Meaning |
|---|---|
| Grey circle | Paused — microphone not active |
| Green circle | Listening — audio is being streamed to amelai |

**Controls:**

| Action | Effect |
|---|---|
| Press **F9** | Toggle listening on/off |
| Right-click tray icon → *Toggle Listening* | Same as F9 |
| Right-click tray icon → *Re-register F9 hotkey* | Use if F9 stops responding — re-registers the hook |
| Right-click tray icon → *Exit* | Close the client |

**Workflow:**

1. Open the application you want to type into (Word, a browser field, a chat window, etc.)
2. Click inside it so it has keyboard focus
3. Press **F9** — the tray icon turns green
4. Speak naturally
5. After about 1 second of silence, your speech is transcribed and pasted into the focused window
6. Press **F9** again to stop (tray turns grey)

You do not need to stop between sentences — the server detects pauses automatically.

### How the paste works

The client uses **Windows `SendInput`** to send Ctrl+V after copying the transcribed text to the clipboard. This is important:

- It does **not** use the `keyboard` library's `send()` function for the paste
- `SendInput` bypasses the keyboard hook entirely, so Ctrl, V, and Esc remain fully responsive to you at all times
- The previous clipboard contents are restored 150 ms after pasting

This means your clipboard is briefly overwritten while text is pasted. If you copy something immediately before speaking, wait until the green icon flashes (transcription completed) before pasting your own content.

### Configuration

Open `stt_client.py` in a text editor to change these constants at the top of the file:

| Constant | Default | Effect |
|---|---|---|
| `SERVER_URL` | `ws://amelai.tail926601.ts.net:9090` | WebSocket address of the server. Change if your Tailscale hostname or port differs. |
| `TOGGLE_KEY` | `"F9"` | Hotkey to toggle listening. Change to any key name recognised by the `keyboard` library (e.g. `"F10"`, `"scroll lock"`). |
| `TOGGLE_DEBOUNCE` | `0.5` | Seconds before a second F9 press is accepted. Prevents the hook firing twice. |
| `CHUNK_DURATION` | `0.1` | Audio chunk size sent to the server (seconds). Lower = lower latency, more WebSocket messages. |

### Troubleshooting

**Tray icon stays grey and title shows "Connecting to amelai…"**

The client cannot reach the server. Check:
- Tailscale is connected on the Windows PC: `tailscale status` in PowerShell
- amelai's STT service is running: `sudo systemctl status stt_server` on amelai
- Port 9090 is not blocked by a firewall on amelai: `sudo ss -tlnp | grep 9090`

**F9 does not toggle / no response to hotkey**

Try *Re-register F9 hotkey* from the tray menu. If that does not help, restart the client using `restart_stt.bat`. In rare cases the `keyboard` library's low-level hook is auto-removed by Windows when the hook callback takes too long — restarting cleanly re-registers it.

If F9 is still unresponsive after restart, try running as Administrator (some Windows 11 security policies restrict global keyboard hooks for standard users).

**Text is pasted but the wrong content appears / old clipboard content is pasted**

The clipboard is set just before `SendInput` fires Ctrl+V. If the target application takes more than 150 ms to process the paste, the clipboard may be restored before it has read it. This can happen in heavy applications or over Remote Desktop. If it occurs consistently with one app, this is an application-side issue; there is no workaround other than a longer paste delay (`asyncio.sleep(0.15)` in `_inject()`).

**Transcription appears but is garbled / misses uncommon words**

Add your domain-specific terms to `initial_prompt` in `stt_server.py`. Whisper uses this as a prior for vocabulary. For example, add product names, technical terms, or proper nouns that appear frequently in your speech.

**Speech is never transcribed — green icon active but nothing appears**

The RMS energy threshold on the server may be above your microphone's output level. Either:
- Increase Windows microphone boost in Sound Settings → Input device properties
- Lower `ENERGY_THRESHOLD` in `stt_server.py` (e.g. from `0.025` to `0.010`)

After changing the server, restart the service:
```bash
sudo systemctl restart stt_server
```

**First words of each utterance are cut off**

The `PRE_ROLL_FRAMES` value (default 5 frames = 100 ms) captures audio just before the energy threshold is crossed. If words are still being clipped, increase it to `10` (200 ms).

**Client crashes silently / disappears from tray**

Launch from `start_stt.bat` (not `.vbs`) to see any Python error output in the console window.

---

## File Reference

| File | Purpose |
|---|---|
| `stt_server.py` | Server — runs on amelai as a systemd service |
| `stt_client.py` | Client — runs in Windows 11 system tray |
| `stt_server.service` | systemd unit file for the server |
| `start_stt.vbs` | Silent launcher for the client (no console window) |
| `start_stt.bat` | Visible launcher for the client (for debugging) |
| `restart_stt.bat` | Kill and relaunch the client |
| `requirements_server.txt` | Server pip dependencies |
| `requirements_client.txt` | Client pip dependencies |

---

## Recent Changes (June 2026)

| Area | Change |
|---|---|
| Server — VRAM | Model no longer loaded at startup. Lazy-loads on first speech, unloads after 15 min idle. |
| Server — model load | Loading runs in a thread executor so the WebSocket server stays responsive during the ~30 s load. |
| Client — paste | Replaced `keyboard.send("ctrl+v")` with Windows `SendInput` via ctypes. Eliminates keyboard hook re-entrancy that caused Ctrl/Esc to be dropped. |
| Client — spurious toggle | Added 500 ms debounce and threading lock to `_toggle()`. |
| Client — double handler | Fixed missing `global _hotkey_handle` in `main()` that caused double-registration of the F9 handler after *Re-register F9 hotkey* was used. |
| Client — event loop | `_inject()` made async with `asyncio.sleep()` instead of blocking `time.sleep()`. |
