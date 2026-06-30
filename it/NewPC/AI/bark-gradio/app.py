"""Bark text-to-audio web UI using Gradio."""
import os
import tempfile
import gradio as gr
import numpy as np
from bark import generate_audio, SAMPLE_RATE
from bark.voices import download_default_voice_prompts


def generate(text, silence_duration, history_prompt=None):
    """Generate audio from text prompt and return (sample_rate, numpy_array)."""
    if not text.strip():
        return SAMPLE_RATE, np.zeros(SAMPLE_RATE, dtype=np.float32)

    audio_array = generate_audio(
        text,
        history_prompt=history_prompt,
        silent_chunks=0 if silence_duration == 0 else int(silence_duration * 10),
        min_silent_ms=50,
        max_silent_ms=200,
    )

    # Save to temp WAV for Gradio playback and download
    tmp = tempfile.NamedTemporaryFile(suffix=".wav", delete=False)
    import soundfile as sf
    sf.write(tmp.name, audio_array, SAMPLE_RATE)
    tmp.close()

    return SAMPLE_RATE, audio_array


# Default voice prompts
VOICE_PROMPTS = []
try:
    prompt_dir = download_default_voice_prompts()
    for f in sorted(os.listdir(prompt_dir)):
        if f.endswith(".npy"):
            VOICE_PROMPTS.append(os.path.join(prompt_dir, f))
except Exception:
    VOICE_PROMPTS = [None]  # fallback: no voice prompt


def on_load():
    return (
        "Upbeat electronic dance music with driving synth bass and four-on-the-floor kick drum",
        0.0,
        "None (no voice prompt)",
    )


with gr.Blocks(title="Bark Text-to-Audio") as demo:
    gr.Markdown("# Bark Text-to-Audio\n> Generate speech, music, and sound effects from text using Suno AI's Bark model.")

    with gr.Row():
        with gr.Column(scale=2):
            text_input = gr.Textbox(
                label="Text Prompt",
                placeholder="Describe the audio you want: e.g., '(music) upbeat electronic track with synth leads'",
                value="Upbeat electronic dance music with driving synth bass and four-on-the-floor kick drum",
            )
            silence_input = gr.Slider(
                minimum=0.0, maximum=5.0, value=0.0, step=0.5,
                label="End silence (seconds)",
            )
            voice_dropdown = gr.Dropdown(
                choices=["None (no voice prompt)"] + [os.path.basename(p).replace(".npy", "") for p in VOICE_PROMPTS],
                value="None (no voice prompt)",
                label="Voice Prompt (optional)",
            )
            gen_btn = gr.Button("Generate Audio", variant="primary")
            clear_btn = gr.Button("Clear")

        with gr.Column(scale=1):
            gr.Markdown("""
### Prompt Tips

- **Music**: Wrap in `(music)` — e.g., `(music) jazzy saxophone solo`
- **Speech**: Just write the text — e.g., `Hello, this is a test`
- **Sound effects**: Describe in parentheses — e.g., `(rain sounds)`, `(applause)`
- **Mixed**: Combine — e.g., `(music playing softly) Hello everyone (applause)`
- **Voice cloning**: Select a voice prompt from the dropdown
""")

    audio_output = gr.Audio(label="Generated Audio", type="numpy")
    download_output = gr.File(label="Download WAV")

    def handle_generate(text, silence, voice_name):
        voice_path = None
        if voice_name != "None (no voice prompt)" and VOICE_PROMPTS:
            voice_path = next(
                (p for p in VOICE_PROMPTS if os.path.basename(p).replace(".npy", "") == voice_name),
                None,
            )
        sr, audio = generate(text, silence, history_prompt=voice_path)
        tmp = tempfile.NamedTemporaryFile(suffix=".wav", delete=False)
        import soundfile as sf
        sf.write(tmp.name, audio, sr)
        tmp.close()
        return (sr, audio), tmp.name

    gen_btn.click(handle_generate, [text_input, silence_input, voice_dropdown], [audio_output, download_output])

    clear_btn.click(
        lambda: ("", 0.0, "None (no voice prompt)", None, None),
        [], [text_input, silence_input, voice_dropdown, audio_output, download_output],
    )

    demo.load(on_load)

if __name__ == "__main__":
    demo.launch(server_name="0.0.0.0", server_port=7861)
