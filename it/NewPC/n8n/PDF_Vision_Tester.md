# PDF Vision Tester — How To Use

A standalone n8n workflow (`Portland Fuel - PDF Vision Tester.json`) for testing the PDF/vision extraction path of the Portland Fuel Customer Profiler without editing the main workflow. It downloads a company's latest accounts PDF from Companies House, runs the vision model on Amelai with the **exact production extraction prompt**, and then lets you chat with the model to interrogate — and get justifications for — the figures it produced.

## Why it exists

The production Customer Profiler's PDF branch (`CH: Download PDF` → `Convert PDF to Images` → `Prep Vision Prompt` → `HTTP: Extract Financials`) sometimes extracts wrong figures, and testing it meant re-running (or temporarily rewiring) the whole profiler. This tester isolates that path and adds a feedback loop: after the first extraction you can ask the model *why* it arrived at each figure, challenge wrong ones, and trial new instructions — all in a chat session.

## Setup (one-off)

1. In n8n: **Workflows → Import from File** → select `Portland Fuel - PDF Vision Tester.json`.
2. Check the two credential/service references resolved on import:
   - Companies House nodes use the existing **Header Auth account** credential (same one as the production profiler).
   - The converter (`http://192.168.1.192:8086/convert`) and Ollama (`http://192.168.1.192:11434`) endpoints on Amelai are hard-coded, same as production.
3. **Activate the workflow** and use its public chat URL (from the "When chat message received" node).

> Activation matters: session state is kept in workflow static data, which n8n only persists for **production** executions. If you run it from the editor's test chat instead, follow-up messages may not find the session and will ask you to `load` the company again.

## Using it

### 1. Start a test

Send a company number:

```
load 06880902
```

A bare number also works (`6880902` — all-digit numbers are zero-padded to 8 characters automatically, and prefixed formats like `SC123456` are accepted).

The workflow then:

1. Fetches the company's **latest accounts filing** from Companies House.
2. Downloads the **PDF rendition** — it sends `Accept: application/pdf`, so even companies that filed iXBRL (which the production workflow routes down the iXBRL branch) can be tested down the PDF path. That includes Portland Fuel and Centrebus.
3. Converts the pages to images on Amelai's converter service.
4. Sends the images with the production extraction prompt (byte-identical to `Prep Vision Prompt`) to `qwen3-vl:32b`.
5. Replies with the model's **raw output**, plus a header showing the company number, filing date, and how many pages were sent.

### 2. Interrogate the result

Just reply in plain English. Examples:

```
Employees is wrong - the accounts say 704. Which page and table did you read it from?
```

```
You gave the prior-year turnover. Look again at the column headings on the profit and loss page.
```

```
Re-read only the balance sheet page and state the net assets row exactly as printed.
```

Each follow-up re-sends the **same page images plus the conversation so far** to Ollama's `/api/chat`, together with a standing audit instruction: the model must justify every figure by citing the page number, the statement or note it came from, and the exact row label and column heading — and if it discovers an error, say so plainly and give the corrected figure, without inventing page references.

This is the loop for prompt engineering: when you find a phrasing that reliably corrects a failure mode, fold it into `Prep Vision Prompt` in the production workflow.

### 3. Commands

| Message | Effect |
|---|---|
| `load <number>` or a bare company number | Start (or restart) a test against that company |
| any other text | Feedback/question to the model about the current company |
| `reset` | Clear the current session |

Sending a new company number at any time switches the session to that company (the old history is discarded).

### 4. Switching the vision model (A/B testing)

The model is set on one line in the **Build Chat Body** node: `const model = 'qwen3-vl:32b';` — flip it to `qwen2.5vl:7b` or `qwen2.5vl:32b` to compare. All three are installed on Amelai. `qwen3-vl:32b` is the current default **and the production model** (promoted July 2026): in testing it reliably extracted the correct figures where both qwen2.5vl models made column/basis mistakes, at the cost of slower responses than the 7b (~13 s cold load, prompt processing of roughly 2.5 s per page measured on a 32b).

The request sizes Ollama's context (`num_ctx`) automatically from the page count (~1,400 tokens per A4 page image plus headroom, bucketed to 8k steps, capped at 65,536 ≈ 42 pages). This matters: the server's default context allocation is so large that the 32b model OOMs the GPUs on a real multi-page request (`cudaMalloc failed`), while an undersized value gets the request rejected. With the automatic sizing, a 30-page filing runs fully GPU-resident (~35 GB).

> **Open WebUI note:** the `num_ctx` fix travels inside each n8n request, so nothing needs configuring for the workflows. But if you chat with a 32b vision model (`qwen3-vl:32b` or `qwen2.5vl:32b`) and images directly in Open WebUI, set the same limit there per model: Admin Panel → Models → (model) → Advanced Parameters → Context Length ≈ 49152, or image-heavy chats can hit the same OOM.

## How it works internally

- **Session state** — company number and the text-only chat history (capped at the last 12 turns; at most 10 concurrent chat sessions) live in `$getWorkflowStaticData('global').pdfTestSessions`, keyed by chat session ID. This is the same persistence mechanism the production profiler uses for its profile store.
- **Images are never stored.** Each turn re-downloads the PDF and re-converts it (a few seconds) rather than bloating static data with megabytes of base64. The model still "sees" the pages every turn because the images are re-attached to the first message of the rebuilt conversation.
- **Error handling** — the Companies House, download, converter, and Ollama nodes all run with `continueOnFail` (matching production), and failures route to a friendly chat reply instead of a generic n8n error: unknown company numbers, filings with no document, or a downed converter each produce a message telling you what to check.
- **Prompt parity** — the first-turn extraction prompt is byte-identical to production's `Prep Vision Prompt`, so first-turn results should mirror what the profiler would produce. (July 2026: the column-selection rule in both prompts was changed from "always take the LEFT column" to "read the year in each column heading and take the column with the later date" — the wording validated with `qwen3-vl:32b`.)

## Caveats

- **`/api/chat` vs `/api/generate`**: production calls Ollama's `/api/generate`; the tester uses `/api/chat` (required for multi-turn). With a single message and an identical prompt the behaviour should be effectively the same — but if the tester ever *fails to reproduce* a wrong figure that production produced, suspect the endpoint difference first and re-test that company via the production workflow.
- **Latest filing only** — the tester always takes the most recent accounts filing (`items_per_page=1`), same as production. It cannot currently target an older filing.
- **Model must be present** — `qwen3-vl:32b` needs to be available on Amelai's Ollama instance (it is the same model production uses).
- **Long documents** — the converter call has a 60 s timeout and the Ollama call 300 s (both more generous than production's 30 s / 120 s) to accommodate large group accounts. Filings over ~42 pages exceed the context cap and are rejected with a clear message rather than sent to the model.
- **Page-token constant is coupled to the converter** — the ~1,400 tokens/page figure assumes the converter's current output resolution. If the converter's DPI changes, retune `TOKENS_PER_PAGE` in Build Chat Body.
- **Production parity (done July 2026)** — the 32b promotion is complete: production's `Prep Vision Prompt` now carries the same `num_ctx` sizing block (plus a `debug` field on the over-cap path), its Ollama timeout is 300 s, and both workflows run `qwen3-vl:32b` with the same prompt. Production additionally degrades over-cap filings (>~42 pages) to its no-financials path instead of replying in chat. If you change the model, `TOKENS_PER_PAGE`, the context cap, or the prompt in one workflow, mirror it in the other — keeping the request bodies identical is what keeps tester results representative of production.

## Related files

- `Portland Fuel - PDF Vision Tester.json` — the current workflow export (this folder).
- `PDF Vision Tester.json` — the original pre-import copy; now stale (still set to `qwen2.5vl:7b` with the old prompt) — use the `Portland Fuel - ` export instead.
- `Portland Fuel - Customer Profiler.json` — the production workflow whose PDF branch this tests.
- The vision prompt being exercised lives in the production workflow's **Prep Vision Prompt** node; the tester keeps its own verified copy in **Build Chat Body**.
