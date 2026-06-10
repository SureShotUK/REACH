# AI Voice — Android App

## Overview

AI Voice is a native Android app that lets you interact with your local AI (running on amelai) by voice. You speak a question, the app transcribes it, sends it to Open WebUI via Tailscale, and reads the response aloud.

**How it works:**

```
Phone microphone → Android SpeechRecognizer → Open WebUI API (Tailscale)
                                                        ↓
                                              Ollama (local LLM)
                                                        ↓
                                         SearXNG web search (if needed)
                                                        ↓
                              Android TextToSpeech ← AI response
```

**Key features:**
- Voice input using Android's built-in SpeechRecognizer (en-GB)
- Spoken response via Android TextToSpeech
- Web search via SearXNG — current news, live data, recent events
- Conversation history maintained within a session
- Concise responses optimised for spoken delivery
- Configurable URL, API key, model, and search URL

---

## Prerequisites

Before using the app, the following must be in place on amelai:

| Requirement | Status check |
|---|---|
| Ollama running | `ollama ps` on amelai |
| Open WebUI running | `docker ps | grep open-webui` |
| SearXNG running | `docker ps | grep searxng` |
| Tailscale connected on phone | Tailscale notification in phone status bar |
| Open WebUI API key | Settings → Account → API Keys |

---

## Open WebUI Configuration

These settings must be configured once in Open WebUI before the app works correctly.

### Web Search (Admin Panel)

1. Go to `https://amelai.tail926601.ts.net` → **Admin Panel → Settings → Web Search**
2. Enable **Web Search**
3. Set provider to **SearXNG**
4. Set URL to `http://192.168.1.192:8080`
5. Save

### Model Tool Settings

Raw tool call XML appears in responses if all Built-in Tools are enabled. To prevent this:

1. Open a model in Open WebUI → **Model Settings → Built-in Tools**
2. Disable **all tools except Web Search**
3. Save

### API Key

The app authenticates using an Open WebUI API key:

1. Go to `https://amelai.tail926601.ts.net` → **Settings → Account → API Keys**
2. Generate a new key
3. Copy it — you will need it in the app's Settings screen

> **Note**: API keys are invalidated when Open WebUI is updated or the container is rebuilt. If the app reports `Error: HTTP 401 Unauthorized`, generate a new key and update it in the app settings.

---

## Building the App

### Tools Required

- **Android Studio** — download from <a href="https://developer.android.com/studio" target="_blank">developer.android.com/studio</a> (~1GB, free)
- **JDK** — bundled with Android Studio, no separate install needed
- **The project source files** — located in `it/NewPC/androidApp/` in the git repository

### Step 1 — Copy the project to a local drive

Android Studio cannot build projects located on a network drive (NAS). Gradle performs intensive file I/O during builds that fails over SMB.

Copy the `androidApp` folder from the NAS to a local path on your Windows PC, for example:

```
C:\Projects\androidApp\
```

> The NAS copy remains your source of truth via git. The local copy is a working build directory only.

### Step 2 — Open the project in Android Studio

1. Launch Android Studio
2. Click **Open** (not "New Project")
3. Navigate to `C:\Projects\androidApp\` and click **OK**
4. Android Studio detects the Gradle project and begins syncing automatically
5. First sync downloads approximately 200MB of dependencies — allow a few minutes

If prompted about the Gradle daemon toolchain, click **Migrate** — this is a harmless performance improvement.

### Step 3 — Build the APK

1. From the menu: **Build → Assemble App**
2. Wait for the build to complete — the bottom bar shows progress
3. On success: `BUILD SUCCESSFUL` appears in the bottom panel

The APK is located at:
```
C:\Projects\androidApp\app\build\outputs\apk\debug\app-debug.apk
```

### Step 4 — Transfer and install on the S24 Ultra

1. Connect the phone via USB, or use **Phone Link** on Windows
2. Copy `app-debug.apk` to the phone
3. On the phone: **Settings → Apps → Special app access → Install unknown apps**
4. Enable installation for the app you used to transfer the file (Files app, etc.)
5. Tap the APK file → **Install**

---

## Updating the App After Code Changes

When the source code is changed (e.g. by Claude Code on the NAS), follow this process to get the updated version on your phone.

### Step 1 — Copy changed files to your local build folder

The files most likely to change are:

| File | Local path to update |
|---|---|
| `MainActivity.kt` | `C:\Projects\androidApp\app\src\main\java\com\portlandlong\aivoice\` |
| `SettingsActivity.kt` | Same folder as above |
| `activity_settings.xml` | `C:\Projects\androidApp\app\src\main\res\layout\` |
| `activity_main.xml` | `C:\Projects\androidApp\app\src\main\res\layout\` |
| `strings.xml` | `C:\Projects\androidApp\app\src\main\res\values\` |
| `gradle.properties` | `C:\Projects\androidApp\` (root of project) |
| `build.gradle.kts` | `C:\Projects\androidApp\app\` |

Copy the updated file(s) from the NAS location into the corresponding local folder, overwriting the existing file.

### Step 2 — Sync in Android Studio

After copying files, click **File → Sync Project with Gradle Files** in Android Studio. This is only strictly necessary if `build.gradle.kts` or `gradle.properties` changed; for Kotlin/XML changes the sync is automatic.

### Step 3 — Rebuild

**Build → Assemble App** — produces a new `app-debug.apk`.

### Step 4 — Reinstall on phone

Transfer and install the new APK as before. Android will update the existing installation — you do not need to uninstall first, and your settings (API key, URL, model) are preserved.

---

## App Settings

Open the settings screen by tapping the **gear icon** in the top-right corner.

| Setting | Default | Description |
|---|---|---|
| Open WebUI URL | `https://amelai.tail926601.ts.net/api/chat/completions` | Full API endpoint — must include `/api/chat/completions` |
| API Key | *(blank)* | Open WebUI API key — generate in Open WebUI Settings → Account → API Keys |
| Model | `qwen3.6:27b` | Ollama model name — must match exactly what is installed |
| SearXNG Search URL | `https://amelai.tail926601.ts.net:8080` | SearXNG base URL for web search |

---

## Using the App

1. Ensure Tailscale is connected on the phone (check the notification bar)
2. Open **AI Voice**
3. Tap the **blue microphone button** — it turns red while listening
4. Speak your question
5. The button turns orange while the AI is thinking
6. The response appears on screen and is read aloud (if Speak is enabled)
7. Tap the **Speak toggle** at the top to enable or disable text-to-speech
8. Tap the **clear icon** (top-right menu) to start a new conversation

---

## Speech Recognition Corrections

Android's SpeechRecognizer uses Google's cloud recognition and cannot be given a custom vocabulary. However, the app includes a post-processing correction map that fixes misrecognised words before sending them to the AI.

### Current corrections

| Heard as | Corrected to |
|---|---|
| Weatherby | Wetherby |

Corrections are case-insensitive — "weatherby", "Weatherby", and "WEATHERBY" are all corrected.

### Adding new corrections

Open `MainActivity.kt` and find the `SPEECH_CORRECTIONS` map in the `companion object` (near the top of the class):

```kotlin
val SPEECH_CORRECTIONS = mapOf(
    "weatherby" to "Wetherby"
)
```

Add new entries as comma-separated pairs:

```kotlin
val SPEECH_CORRECTIONS = mapOf(
    "weatherby" to "Wetherby",
    "wrong word" to "Correct Word",
    "harrogate" to "Harrogate"
)
```

The left side is what Google hears (lowercase is fine — matching is case-insensitive). The right side is the exact text you want substituted. After editing, copy `MainActivity.kt` to your local build folder and rebuild.

---

## Troubleshooting

| Error | Cause | Fix |
|---|---|---|
| `Error: HTTP 401 Unauthorized` | API key invalid (often after Open WebUI update) | Generate a new API key in Open WebUI and update in app settings |
| `Error: timeout` | Model cold-loading into VRAM, or VRAM full | Wait 60–90 seconds and retry; or run `ollama ps` on amelai to check VRAM |
| `Error: HTTP 404` | URL missing `/api/chat/completions` path | Check Settings — URL must be the full path |
| Raw XML in responses (`<function_calls>...`) | Open WebUI Built-in Tools all enabled | Disable all Built-in Tools in Open WebUI except Web Search |
| App says it has no internet access | Web Search not enabled in Open WebUI Admin Panel | Enable Web Search and set SearXNG URL in Admin Panel → Settings → Web Search |
| Build fails: AndroidX error | `gradle.properties` missing from project root | Create `gradle.properties` with `android.useAndroidX=true` and `android.enableJetifier=true` |
| Build fails: read/write error | Project is on a network drive | Copy `androidApp` folder to a local drive (e.g. `C:\Projects\`) |
| Model hangs / never responds | Wrong model name in settings | Check exact model name with `ollama list` on amelai |

### Checking and freeing VRAM on amelai

```bash
# See what is loaded
ollama ps

# Unload a specific model
ollama stop qwen3.5:35b

# If ollama stop is stuck, restart the service
sudo systemctl restart ollama
```

> Open WebUI automatically reloads the last-used model within a few minutes of Ollama restarting. The model will unload itself after approximately 4 minutes if nothing requests it.

---

## Project File Locations

| Location | Purpose |
|---|---|
| `it/NewPC/androidApp/` | Source code — git repository on NAS |
| `C:\Projects\androidApp\` | Local build directory on Windows PC |
| `app/src/main/java/com/portlandlong/aivoice/` | Kotlin source files |
| `app/src/main/res/layout/` | UI layout XML files |
| `app/src/main/res/values/` | Strings, colours, themes |
| `app/build/outputs/apk/debug/app-debug.apk` | Built APK (local only, not in git) |
