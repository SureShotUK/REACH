#!/usr/bin/env python3
"""
STT WebSocket server — runs on amelai, GPU 1.

VAD: simple RMS energy threshold — no torchaudio/silero-vad dependency.
Transcription: faster-whisper large-v3 on CUDA with vad_filter=True for
Whisper's own internal VAD pass to suppress hallucinations on non-speech.

Model lifecycle: loaded on the first transcription request, unloaded
automatically after MODEL_IDLE_TIMEOUT seconds (15 min) with no activity.
The WebSocket server itself stays running and listening at all times.

Tune ENERGY_THRESHOLD if the server fires on background noise (raise it)
or misses quiet speech (lower it). 0.015 suits a typical quiet room.
"""
import asyncio
import gc
import logging
import sys
import time
from pathlib import Path

import numpy as np
import websockets

LOG_PATH = Path("/opt/stt/stt.log")
LOG_PATH.parent.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s %(levelname)s %(message)s",
    handlers=[
        logging.FileHandler(LOG_PATH),
        logging.StreamHandler(sys.stdout),
    ],
)
log = logging.getLogger(__name__)

SAMPLE_RATE = 16_000
FRAME_SAMPLES = int(SAMPLE_RATE * 0.02)   # 20 ms frames = 320 samples
SILENCE_FRAMES = 50                         # 50 × 20 ms = 1000 ms silence → end utterance
MIN_SPEECH_SEC = 0.3                        # ignore blips shorter than this
PRE_ROLL_FRAMES = 5                         # frames to prepend before speech starts
ENERGY_THRESHOLD = 0.025                    # RMS level; raise if background noise triggers
MODEL_IDLE_TIMEOUT = 900                    # 15 minutes of silence → unload from VRAM

# Model state — managed by _get_model() / _unload_model() / _idle_monitor()
_model = None
_model_last_used: float = 0.0
_model_lock: asyncio.Lock | None = None     # created in main() — needs a running event loop


def _load_whisper():
    """Blocking load — called via run_in_executor to avoid stalling the event loop."""
    from faster_whisper import WhisperModel
    return WhisperModel("large-v3", device="cuda", compute_type="float16")


async def _get_model():
    """Return the loaded model, loading it first if necessary."""
    global _model, _model_last_used
    async with _model_lock:
        if _model is None:
            log.info("Loading faster-whisper large-v3 on CUDA …")
            loop = asyncio.get_event_loop()
            _model = await loop.run_in_executor(None, _load_whisper)
            log.info("Model ready.")
        _model_last_used = time.monotonic()
        return _model


async def _unload_model() -> None:
    global _model
    async with _model_lock:
        if _model is not None:
            log.info("Unloading model from VRAM (idle timeout).")
            del _model
            _model = None
            gc.collect()
            log.info("Model unloaded.")


async def _idle_monitor() -> None:
    """Background task — checks every 60 s and unloads the model after 15 min idle."""
    while True:
        await asyncio.sleep(60)
        if _model is not None and time.monotonic() - _model_last_used > MODEL_IDLE_TIMEOUT:
            await _unload_model()


def _is_speech(frame: np.ndarray) -> bool:
    return float(np.sqrt(np.mean(frame ** 2))) > ENERGY_THRESHOLD


async def _transcribe_and_send(audio: np.ndarray, ws) -> None:
    if len(audio) / SAMPLE_RATE < MIN_SPEECH_SEC:
        return
    model = await _get_model()
    segments, _ = model.transcribe(
        audio,
        language="en",
        beam_size=5,
        vad_filter=True,   # Whisper's own VAD for final hallucination suppression
        initial_prompt="Claude, Tailscale, amelai, ComfyUI, Python, Docker, systemd, Linux",
    )
    text = " ".join(seg.text.strip() for seg in segments).strip()
    if text:
        log.info(f"→ {text!r}")
        await ws.send(text)


async def handle_client(ws) -> None:
    log.info(f"Client connected: {ws.remote_address}")

    pending = np.array([], dtype=np.float32)
    pre_roll: list[np.ndarray] = []
    speech_buf: list[np.ndarray] = []
    in_speech = False
    silence_count = 0

    try:
        async for message in ws:
            if not isinstance(message, bytes):
                continue

            chunk = np.frombuffer(message, dtype=np.int16).astype(np.float32) / 32768.0
            pending = np.concatenate([pending, chunk])

            while len(pending) >= FRAME_SAMPLES:
                frame = pending[:FRAME_SAMPLES]
                pending = pending[FRAME_SAMPLES:]
                speaking = _is_speech(frame)

                if not in_speech:
                    pre_roll.append(frame)
                    if len(pre_roll) > PRE_ROLL_FRAMES:
                        pre_roll.pop(0)

                if speaking:
                    if not in_speech:
                        in_speech = True
                        speech_buf = list(pre_roll)
                    speech_buf.append(frame)
                    silence_count = 0
                elif in_speech:
                    speech_buf.append(frame)
                    silence_count += 1
                    if silence_count >= SILENCE_FRAMES:
                        audio = np.concatenate(speech_buf)
                        await _transcribe_and_send(audio, ws)
                        speech_buf = []
                        pre_roll = []
                        in_speech = False
                        silence_count = 0

    except websockets.exceptions.ConnectionClosedOK:
        log.info(f"Client disconnected: {ws.remote_address}")
    except Exception:
        log.exception("Unhandled error in handle_client")


async def main() -> None:
    global _model_lock
    _model_lock = asyncio.Lock()

    host, port = "0.0.0.0", 9090
    log.info(f"Listening on {host}:{port} — model loads on first speech, unloads after {MODEL_IDLE_TIMEOUT // 60} min idle")
    asyncio.create_task(_idle_monitor())
    async with websockets.serve(handle_client, host, port, max_size=2**20):
        await asyncio.Future()


if __name__ == "__main__":
    asyncio.run(main())
