#!/usr/bin/env python3
"""
STT client — runs on Windows 11.

System tray icon shows PAUSED (grey) / LISTENING (green) state.
F9 hotkey and the tray right-click menu both toggle listening.
Transcribed text is pasted into the focused window via clipboard.

Run with: pythonw stt_client.py   (no console window)
"""
import asyncio
import os
import queue
import threading
import time

import keyboard
import numpy as np
import pyperclip
import pystray
import sounddevice as sd
import websockets
from PIL import Image, ImageDraw

SERVER_URL = "ws://amelai.tail926601.ts.net:9090"
SAMPLE_RATE = 16_000
CHUNK_DURATION = 0.1
CHUNK_SAMPLES = int(SAMPLE_RATE * CHUNK_DURATION)
TOGGLE_KEY = "F9"

_listening: bool = False
_audio_q: queue.Queue[np.ndarray] = queue.Queue()
_tray_icon: pystray.Icon | None = None


def _make_icon(color: str) -> Image.Image:
    img = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.ellipse([4, 4, 60, 60], fill=color)
    return img


_ICON_PAUSED = _make_icon("#888888")
_ICON_LISTENING = _make_icon("#00cc44")


def _update_tray(connected: bool = True) -> None:
    if _tray_icon is None:
        return
    if not connected:
        _tray_icon.icon = _ICON_PAUSED
        _tray_icon.title = "STT — Connecting to amelai…"
    elif _listening:
        _tray_icon.icon = _ICON_LISTENING
        _tray_icon.title = "STT — LISTENING"
    else:
        _tray_icon.icon = _ICON_PAUSED
        _tray_icon.title = "STT — Paused  (F9 to start)"


def _toggle() -> None:
    global _listening
    _listening = not _listening
    _update_tray()


def _audio_callback(indata: np.ndarray, frames: int, time_info, status) -> None:
    if _listening:
        _audio_q.put(indata[:, 0].copy())


def _inject(text: str) -> None:
    prev = pyperclip.paste()
    pyperclip.copy(text)
    keyboard.send("ctrl+v")
    time.sleep(0.08)
    pyperclip.copy(prev)


async def _send_audio(ws) -> None:
    loop = asyncio.get_event_loop()
    while True:
        try:
            chunk = await loop.run_in_executor(None, _audio_q.get, True, 0.5)
        except queue.Empty:
            continue
        pcm = (chunk * 32767).astype(np.int16)
        await ws.send(pcm.tobytes())


async def _recv_text(ws) -> None:
    async for message in ws:
        text = str(message).strip()
        if text:
            _inject(text)


async def _run() -> None:
    while True:
        _update_tray(connected=False)
        try:
            async with websockets.connect(SERVER_URL, ping_interval=20) as ws:
                _update_tray(connected=True)
                await asyncio.gather(_send_audio(ws), _recv_text(ws))
        except Exception:
            pass
        await asyncio.sleep(5)  # wait before retrying


def _start_asyncio() -> None:
    """Runs in a daemon thread — connects to amelai and handles audio/text."""
    with sd.InputStream(
        samplerate=SAMPLE_RATE,
        channels=1,
        dtype="float32",
        blocksize=CHUNK_SAMPLES,
        callback=_audio_callback,
    ):
        asyncio.run(_run())


def _on_tray_toggle(icon, item) -> None:
    _toggle()


def _on_tray_exit(icon, item) -> None:
    icon.stop()
    os._exit(0)


def main() -> None:
    global _tray_icon

    keyboard.add_hotkey(TOGGLE_KEY, _toggle, suppress=False)

    # Start audio + WebSocket connection in a background thread
    t = threading.Thread(target=_start_asyncio, daemon=True)
    t.start()

    # Run pystray in the main thread (required on Windows)
    menu = pystray.Menu(
        pystray.MenuItem("Toggle Listening (F9)", _on_tray_toggle),
        pystray.MenuItem("Exit", _on_tray_exit),
    )
    _tray_icon = pystray.Icon(
        "stt",
        _ICON_PAUSED,
        "STT — Paused  (F9 to start)",
        menu,
    )
    _tray_icon.run()


if __name__ == "__main__":
    main()
