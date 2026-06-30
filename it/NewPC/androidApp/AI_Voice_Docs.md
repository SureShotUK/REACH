# AI Voice — App Documentation

## What It Does

AI Voice is a native Android app that lets you talk to your local AI server (amelai) by voice. You speak a question, the app transcribes it, searches the web for current information, sends everything to the AI, and reads the response back to you aloud.

---

## First-Time Setup

### 1. Generate an API Key

The app needs an API key to authenticate with Open WebUI.

1. Open a browser and go to `https://amelai.tail926601.ts.net`
2. Log in to Open WebUI
3. Click your profile icon (top right) → **Settings** → **Account**
4. Scroll to **API Keys** and click **Generate**
5. Copy the key — you only see it once

### 2. Enter the API Key in the App

1. Open AI Voice on your phone
2. Tap the **settings icon** (top right of the title bar)
3. Paste your key into the **API Key** field
4. Tap **Save**

The app will not connect to the AI without a valid API key. If you see `Error: HTTP 401 Unauthorized`, the key is missing, wrong, or was regenerated in Open WebUI — repeat the steps above.

---

## Settings Reference

Open Settings by tapping the gear icon in the top-right corner.

| Setting | Default | Description |
|---|---|---|
| Open WebUI URL | `https://amelai.tail926601.ts.net/api/chat/completions` | The AI API endpoint. Do not change unless the server address changes. |
| API Key | *(blank)* | Your Open WebUI API key — **must be set before first use** |
| Model | `qwen3.6:27b` | The Ollama model to use. Must be pulled on amelai. |
| SearXNG Search URL | `https://amelai.tail926601.ts.net:8080` | The web search engine URL. Used for current news and local queries. |
| Dark Theme | Off | Toggles dark mode. Takes effect immediately. |

Tap **Save** after making any changes.

---

## Using the App

### Main Screen

The screen shows a scrollable conversation history. Each exchange shows your question and the AI's response as chat bubbles.

### Bottom Bar

| Control | Function |
|---|---|
| **Speak** toggle | Turns text-to-speech on or off. When on, the AI's response is read aloud. |
| Status text | Shows the current state: *Tap mic to speak* / *Listening…* / *Thinking…* |
| **X button** | Clears the conversation history from the screen and resets the AI's memory of the session |
| **Mic button** | Tap once to start listening. Tap again to stop early. |

### Talking to the App

1. Ensure your phone is connected to Tailscale
2. Tap the **mic button** — it turns red and the status shows *Listening…*
3. Speak your question clearly
4. The app automatically detects when you stop speaking
5. Status shows *Thinking…* while the AI processes
6. The response appears on screen and is read aloud (if Speak is on)

---

## Features

### Web Search

Before every query, the app automatically searches SearXNG for the top 3 results matching your question and includes them in the AI's context. This means the AI can answer questions about current news, today's weather, live events, and so on — without you needing to ask it to search.

**Requires**: Tailscale connected and SearXNG running on amelai (port 8080).

### Location Awareness

The app reads your device's last known location (using network/GPS) and tells the AI where you are. This makes local queries work naturally:

- *"What's the weather here?"*
- *"Find me the nearest Italian restaurant"*
- *"Where's the closest car park?"*

The location is injected as: *"User's current location: Wetherby, West Yorkshire, England."*

Grant the **location permission** when prompted on first launch. If you declined it, go to Android Settings → Apps → AI Voice → Permissions → Location → Allow.

### Speech Corrections

The Android speech recogniser occasionally mishears proper nouns. The app applies a correction map after transcription to fix known errors before sending to the AI.

Current corrections:

| Heard | Corrected to |
|---|---|
| Weatherby | Wetherby |

To add more corrections, edit `SPEECH_CORRECTIONS` in `MainActivity.kt`:

```kotlin
val SPEECH_CORRECTIONS = mapOf(
    "weatherby" to "Wetherby",
    "wrong word" to "Right Word"
)
```

---

## Requirements

- Android 8.0 (API 26) or later
- Tailscale connected on the phone (the server is not exposed to the public internet)
- amelai server running with: Ollama, Open WebUI, SearXNG

---

## Troubleshooting

### Error: HTTP 401 Unauthorized
Your API key is missing or expired. Go to Settings and paste a freshly generated key from Open WebUI → Settings → Account → API Keys.

### Error: Timeout
- The model may be loading from disk — wait 30 seconds and try again
- VRAM may be full from ComfyUI. Use the "Free ComfyUI VRAM" browser bookmarklet on amelai, or restart the ComfyUI container
- Check Tailscale is connected: the server is only reachable via the tailnet

### Error: Failed to connect
- Confirm Tailscale is connected on your phone (`tailscale status`)
- Confirm amelai is online and Open WebUI is running

### App says it cannot search the web
- SearXNG must be running on amelai: `docker ps | grep searxng`
- The SearXNG URL in Settings must match the running instance (default: `https://amelai.tail926601.ts.net:8080`)
- Tailscale ACL must allow port 8080 — check `https://login.tailscale.com/admin/acls`

### Listening stops immediately / no transcription
- Microphone permission must be granted: Android Settings → Apps → AI Voice → Permissions → Microphone
- Speak within 2–3 seconds of tapping the mic

### AI responses contain asterisks or symbols read aloud
The system prompt instructs the AI to use plain text only. If this happens, the model in use may not follow instructions reliably — try switching to `qwen3.6:27b` in Settings.

---

## Building and Updating the App

### Prerequisites
- Android Studio installed on a local drive (not a network share)
- Project folder on the local drive (e.g. `C:\Projects\androidApp\`)

### Build Steps
1. Open the project in Android Studio
2. Copy updated source files from the NAS to the local project folder if needed
3. **Build → Assemble App** (or Build → Make Project)
4. The APK is generated at: `app\build\outputs\apk\debug\app-debug.apk`
5. Transfer to the phone via Phone Link or USB
6. Install via Samsung Files → Package Installer

### Updating Source Files
The source files live on the NAS at `/docs/terminai/it/NewPC/androidApp/`. After editing on the NAS, copy changed files to the local Android Studio project before building.

Key source files:

| File | Purpose |
|---|---|
| `app/src/main/java/.../MainActivity.kt` | Core app logic, API calls, speech, location |
| `app/src/main/java/.../SettingsActivity.kt` | Settings screen logic |
| `app/src/main/res/layout/activity_main.xml` | Main screen layout |
| `app/src/main/res/layout/activity_settings.xml` | Settings screen layout |
| `app/src/main/res/values/strings.xml` | All UI text strings |
| `app/src/main/res/values/colors.xml` | Colour palette |
| `app/src/main/AndroidManifest.xml` | Permissions and app configuration |

---

## Architecture

```
Phone (Android)
  └─ AI Voice App
       ├─ SpeechRecognizer (Android, en-GB) → transcribes voice to text
       ├─ LocationManager → gets last known location
       ├─ Geocoder → reverse-geocodes coordinates to place name
       ├─ SearXNG HTTP call → fetches top 3 web results
       ├─ Open WebUI API call → sends system prompt + history + search results
       └─ TextToSpeech (Android) → reads response aloud

Server (amelai, via Tailscale)
  ├─ Open WebUI → /api/chat/completions endpoint
  ├─ Ollama → runs the LLM (default: qwen3.6:27b)
  └─ SearXNG → web search (port 8080)
```

The app calls the Open WebUI `/api/chat/completions` endpoint directly (OpenAI-compatible format). Open WebUI's built-in tools (like its own web search) are **not** used — the app implements web search itself by calling SearXNG and injecting the results into the system prompt.
