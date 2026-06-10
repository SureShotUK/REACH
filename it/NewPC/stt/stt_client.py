#!/usr/bin/env python3
"""
STT client — runs on Windows 11.

System tray icon shows PAUSED (grey) / LISTENING (green) state.
F9 hotkey and the tray right-click menu both toggle listening.
Transcribed text is pasted into the focused window via clipboard.

Run with: pythonw stt_client.py   (no console window)
"""
import asyncio
import ctypes
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
TOGGLE_DEBOUNCE = 0.5  # seconds — ignore rapid re-fires from hook glitches

_listening: bool = False
_audio_q: queue.Queue[np.ndarray] = queue.Queue()
_tray_icon: pystray.Icon | None = None
_hotkey_handle = None
_toggle_lock = threading.Lock()
_last_toggle_time: float = 0.0

# Windows SendInput structs — Ctrl+V without going through the keyboard hook.
# Using SendInput bypasses re-entrancy issues caused by keyboard.send().
_ULONG_PTR = ctypes.c_uint64 if ctypes.sizeof(ctypes.c_void_p) == 8 else ctypes.c_uint32


class _KEYBDINPUT(ctypes.Structure):
    _fields_ = [
        ("wVk",         ctypes.c_ushort),
        ("wScan",       ctypes.c_ushort),
        ("dwFlags",     ctypes.c_ulong),
        ("time",        ctypes.c_ulong),
        ("dwExtraInfo", _ULONG_PTR),
    ]


class _MOUSEINPUT(ctypes.Structure):
    _fields_ = [
        ("dx",          ctypes.c_long),
        ("dy",          ctypes.c_long),
        ("mouseData",   ctypes.c_ulong),
        ("dwFlags",     ctypes.c_ulong),
        ("time",        ctypes.c_ulong),
        ("dwExtraInfo", _ULONG_PTR),
    ]


class _INPUT_UNION(ctypes.Union):
    _fields_ = [("ki", _KEYBDINPUT), ("mi", _MOUSEINPUT)]


class _INPUT(ctypes.Structure):
    _fields_ = [("type", ctypes.c_ulong), ("_u", _INPUT_UNION)]


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
    global _listening, _last_toggle_time
    with _toggle_lock:
        now = time.monotonic()
        if now - _last_toggle_time < TOGGLE_DEBOUNCE:
            return
        _last_toggle_time = now
        _listening = not _listening
    _update_tray()


def _reregister_hotkey() -> None:
    global _hotkey_handle
    try:
        if _hotkey_handle is not None:
            keyboard.remove_hotkey(_hotkey_handle)
    except Exception:
        pass
    _hotkey_handle = keyboard.add_hotkey(TOGGLE_KEY, _toggle, suppress=False)
    if _tray_icon:
        _tray_icon.title = "STT — F9 re-registered ✓"


def _audio_callback(indata: np.ndarray, frames: int, time_info, status) -> None:
    if _listening:
        _audio_q.put(indata[:, 0].copy())


def _send_paste() -> None:
    """Send Ctrl+V via Windows SendInput — no re-entrancy with the keyboard hook."""
    KEYEVENTF_KEYUP = 0x0002
    VK_CONTROL, VK_V = 0x11, 0x56

    def _ki(vk: int, flags: int = 0) -> _INPUT:
        i = _INPUT()
        i.type = 1  # INPUT_KEYBOARD
        i._u.ki = _KEYBDINPUT(wVk=vk, dwFlags=flags)
        return i

    events = (_INPUT * 4)(
        _ki(VK_CONTROL),
        _ki(VK_V),
        _ki(VK_V, KEYEVENTF_KEYUP),
        _ki(VK_CONTROL, KEYEVENTF_KEYUP),
    )
    ctypes.windll.user32.SendInput(4, events, ctypes.sizeof(_INPUT))


async def _inject(text: str) -> None:
    prev = pyperclip.paste()
    pyperclip.copy(text)
    _send_paste()
    await asyncio.sleep(0.15)   # yield to event loop while target app processes paste
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
            await _inject(text)


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
    global _tray_icon, _hotkey_handle   # both globals — without this _hotkey_handle is local
                                        # and _reregister_hotkey() can never remove the original

    _hotkey_handle = keyboard.add_hotkey(TOGGLE_KEY, _toggle, suppress=False)

    # Start audio + WebSocket connection in a background thread
    t = threading.Thread(target=_start_asyncio, daemon=True)
    t.start()

    # Run pystray in the main thread (required on Windows)
    menu = pystray.Menu(
        pystray.MenuItem("Toggle Listening (F9)", _on_tray_toggle),
        pystray.MenuItem("Re-register F9 hotkey", lambda icon, item: _reregister_hotkey()),
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
