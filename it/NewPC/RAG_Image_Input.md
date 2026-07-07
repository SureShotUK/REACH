<img src="../../Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# RAG Image Input — Importing Pictures into Amelai's Knowledge Base

*Last updated: 7 July 2026*

---

## Overview

Amelai's RAG knowledge base cannot ingest images directly. The embedding pipeline uses `nomic-embed-text`, a **text-only** embedding model — an image file uploaded to an Open WebUI knowledge collection produces no searchable content.

The solution is a two-stage pipeline: a **vision-language model interprets the image into text first**, and that text is imported as a normal document. Amelai already has the vision models installed:

| Model | Size | Use |
|---|---|---|
| `qwen2.5vl:32b` | ~21GB VRAM | **Recommended** — use for anything being imported into the knowledge base |
| `qwen2.5vl:7b` | ~6GB VRAM | Quick checks only — misreads fine print more often |

**The pipeline:**

```
Photo → qwen2.5vl (interpret to text) → verify against image → save as .md → upload to Knowledge collection → queryable via Open WebUI chat or /db
```

---

## Step-by-Step Process

### 1. Free up VRAM if needed

`qwen2.5vl:32b` needs ~21GB. If a ComfyUI model is loaded, free it first using the **"Free ComfyUI VRAM"** bookmarklet (saved in Edge Favourites) while on the ComfyUI tab.

### 2. Interpret the image

1. In Open WebUI, start a **new chat**
2. Select **`qwen2.5vl:32b`** as the model
3. Attach the photo — the **+ / paperclip** in the message box, or paste the image directly into the box
4. Paste the appropriate prompt from the **Prompts** section below (flow chart or chart/table)

### 3. Verify before importing — this is the step that matters

Vision models transcribe box text and printed numbers very reliably, but occasionally:

- **Flow charts**: follow an arrow to the wrong box, especially on dense diagrams with crossing lines — check every branch direction
- **Charts/tables**: misread a digit — spot-check a few transcribed numbers against the image

Read the output against the actual image once. **A wrong branch direction or wrong figure imported into the KB becomes a confidently wrong answer later.**

### 4. Save as a document

Copy the (corrected) response into a small markdown file:

- **One image per file** — retrieval then cites the right diagram, and `/db` results name the source file
- **Descriptive filename**, e.g. `Process_Flow_Fuel_Delivery_Authorisation.md`
- **Descriptive first heading** including source and date, e.g. `# Fuel Delivery Authorisation Process (from flow chart, July 2026)`

### 5. Upload to the knowledge base

Open WebUI → **Workspace → Knowledge** → select (or create) a collection → add the file. The document is embedded automatically and becomes searchable.

### 6. Query it

- **Open WebUI**: chat with the knowledge collection attached
- **Claude Code**: `/db <query>` (works from any machine with the rag MCP server configured)

---

## Photo Quality

Photo quality makes more difference than anything else:

- Shoot **straight-on** (avoid keystone distortion)
- Good, even lighting — no glare on laminated sheets or screens
- Whole chart in frame, highest resolution available
- **If the chart exists digitally anywhere, a screenshot or export beats a photo every time**

---

## Prompts

### Prompt 1 — Flow Charts / Process Diagrams

```
This image is a flow chart describing a process. Convert it into a complete text representation so that someone who cannot see the image can follow the process exactly.

1. Start with a one-paragraph summary of what process the chart describes.
2. Transcribe every box, label and annotation VERBATIM - do not paraphrase the wording inside boxes.
3. Describe the flow as numbered steps in order, following the arrows. For every decision point, state the question and list each branch explicitly, e.g. "If YES -> step 6. If NO -> step 9."
4. If there are swimlanes, parallel paths, loops back to earlier steps, or notes in the margins, describe them explicitly.
5. Finish with a "Key facts" bullet list of the most important rules or thresholds shown in the chart.

Do not invent or assume any step that is not visible in the image. If any text is unreadable, write [unreadable] rather than guessing.

Format the response as standard markdown: "#"-style headings, "-" for bullets, "1." for numbered steps, with lists starting at the left margin - never indent a list by four spaces (that renders as a code block).
```

### Prompt 2 — Charts, Graphs and Tables

```
This image contains a chart, graph or table. Extract ALL of the data into text form.

1. State what the chart/table shows: title, axis labels, units, legend entries, time period covered.
2. Reproduce tables as complete markdown tables with every row and column, values transcribed exactly.
3. For graphs, list the plotted values as a markdown table. If exact figures are not printed, give best-read values and say they are estimated from the plot.
4. Note any highlighted values, annotations, footnotes or data source lines.
5. Finish with a "Key facts" bullet list: maximum, minimum, notable trends or outliers.

Transcribe numbers exactly as shown. If a value is unreadable, write [unreadable] rather than guessing.

Format the response as standard markdown: "#"-style headings, "-" for bullets, proper markdown table syntax, with lists starting at the left margin - never indent a list by four spaces (that renders as a code block).
```

---

## Limitations and Caveats

| Content type | Reliability | Notes |
|---|---|---|
| Printed text, labels, signs | Very good | Transcribes well with the 32b model |
| Tables and printed figures | Very good | Spot-check digits before import |
| Flow charts | Good | Verify branch directions on dense diagrams |
| Graphs without printed values | Fair | Values are estimated from the plot — the prompt flags them as estimates |
| Handwriting | Hit-and-miss | Always review fully before import |
| General scene photos | Prompt-dependent | Only what the description mentions is queryable — the prompt at capture time determines what can be asked later |

**Why direct image import can't work**: the knowledge base stores 768-dimension `nomic-embed-text` vectors (zero-padded to 1536 to match Open WebUI's schema). That model only embeds text; there is no image encoder anywhere in the pipeline.

---

## Troubleshooting

| Symptom | Cause / Fix |
|---|---|
| Error: `qwen2.5vl:32b does not support tools` | Open WebUI is sending a tool-calling request and the qwen2.5vl Ollama template has no tool support. Something in the chat is injecting tools: a Tool or Web Search toggle in the + menu, Function Calling set to "Native" (Chat Controls → Advanced Params — set to "Default"; also check Workspace → Models and Admin Settings → Models), or a knowledge collection attached to the vision chat. Fix: start a clean new chat with nothing toggled on; keep image interpretation and KB querying in separate chats |
| Error: `llama-server process has terminated ... Failed to load CLIP model from /mnt/models/ollama/blobs/...` | The model blob's vision-encoder metadata is incompatible with the installed Ollama runtime (seen 2026-07-07: 32b failed with `Key not found: clip.vision.n_wa_pattern` while 7b worked). Not corruption — verify with `sha256sum` on the blob if in doubt. Fix: update Ollama (`curl -fsSL https://ollama.com/install.sh \| sh`) and retest; re-pulling the model only helps if the registry digest has changed. Workaround: use `qwen2.5vl:7b` |
| Error: `unexpected EOF` when sending an image (llama-server segfault in logs) | Context length too large — if Open WebUI does not send `num_ctx` (the greyed-out 2048 placeholder means "not sent"), Ollama 0.31+ defaults to the model's full 128k context, which exhausts VRAM on the 32b model (`cudaMalloc failed` at warmup in the Ollama log) and the vision encoder segfaults on image encode (seen 2026-07-07). Fix: explicitly set num_ctx to 8192 in Admin Panel → Settings → Models → edit qwen2.5vl:32b → Advanced Params |
| Vision model very slow or falls back to CPU | Insufficient free VRAM — free ComfyUI VRAM (bookmarklet) or use `nvtop` to see what's loaded |
| Output invents steps not in the image | Re-run with the prompt as given (it includes an anti-guessing instruction); try a higher-resolution or straighter photo |
| Uploaded document not found by `/db` | Confirm the file was added to a Knowledge collection (Workspace → Knowledge), not just attached to a chat |
| Small text misread | Retake closer/straight-on, or crop the image to the relevant region and process in sections |

---

## Future Automation

If volume grows and the manual loop becomes tedious, the same pipeline automates cleanly:

- **Watched folder**: drop photos in via FileBrowser; a script runs each through `qwen2.5vl` (Ollama API) and pushes the text into the KB via Open WebUI's API
- **n8n workflow**: same automation, visible and editable in the n8n UI

The manual verification step (step 3) is the main thing lost in automation — automated imports would need periodic review.
