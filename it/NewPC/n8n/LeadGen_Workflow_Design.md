# Lead Generation Workflow — Design Document

**Project**: Portland Fuel — AI Lead Generation via n8n  
**Status**: Implemented — `LeadGen_Workflow.json` ready to import  
**Last Updated**: June 2026

---

## Usage Guide

### Prerequisites

Before running the workflow, confirm:

- [ ] n8n is running: `https://amelai.tail926601.ts.net:5678`
- [ ] **"Companies House API"** Basic Auth credential exists in n8n (Settings → Credentials):
  - Username: `f80f8012-64f5-4f45-902a-b1814ea051a1`
  - Password: *(leave blank)*
- [ ] **"MyHotmailEmail"** Microsoft Outlook credential is connected (already done)
- [ ] `qwen3.6:27b` is available in Ollama — verify with `ollama list` on amelai
- [ ] ComfyUI VRAM freed if a large image model was recently used (see VRAM note below)

---

### Opening the Chat Interface

1. Open n8n and find **"Portland Fuel - Lead Generation"**
2. Click **"Execute Workflow"** or the chat bubble icon (bottom right of the canvas)
3. A chat panel opens — type your search and press **Enter** or click **Send**
4. Results arrive as an email to `steve@portland-fuel.co.uk` — the chat itself will show a "Workflow finished" confirmation

---

### Input Mode 1 — SIC Code Search

Use this to find multiple companies matching a sector and geography.

**Format:** `[SIC codes] [location] [optional: N companies]`

- **SIC codes** — type them anywhere as 4–5 digit numbers (e.g. `49410`)
- **Location** — any text that isn't a number: town, county, region, or partial postcode (e.g. `Yorkshire`, `East Anglia`, `LS`)
- **Max companies** — add `N companies` to cap results (default 25, max 25)
- **No location** — omit it entirely for a national search

The parser is flexible — order doesn't matter, and you don't need to prefix with "SIC".

#### SIC Search Examples

| What you want | What to type |
|---|---|
| Road haulage companies in Yorkshire | `49410 Yorkshire` |
| Road haulage + bunker freight in Yorkshire, max 15 | `49410 49310 Yorkshire 15 companies` |
| Arable and mixed farms in East Anglia, max 20 | `01110 01500 East Anglia 20 companies` |
| Groundworks and plant hire in West Midlands | `43120 77320 West Midlands` |
| Agricultural contractors in Norfolk | `01610 01620 Norfolk` |
| Fuel wholesale and retail nationally, max 10 | `46710 47300 10 companies` |
| Haulage in Leeds postcode area | `49410 LS` |
| All target sectors in Lancashire | `49410 49310 01110 01500 43120 77320 Lancashire` |

---

### Input Mode 2 — Single Company Lookup

Use this to evaluate one specific company you already know about.

**Format:** Type **only** the Companies House registration number — nothing else.

The parser detects company numbers automatically:
- England/Wales: 8 digits (e.g. `12345678`)
- Scotland: `SC` + 6 digits (e.g. `SC123456`)
- Northern Ireland: `NI` + 6 digits (e.g. `NI123456`)
- LLP: `OC` + 6 digits (e.g. `OC123456`)

> **Important:** The number must be the entire message. Adding any other text (e.g. "look up 12345678") will cause it to fall into SIC search mode and return no results.

#### Single Company Examples

| What you want | What to type |
|---|---|
| Look up a specific haulier you've heard about | `09876543` |
| Check a Scottish farming business | `SC654321` |
| Evaluate a potential fuel card client | `00445566` |

You can find a company's registration number at:
`https://find-and-update.company-information.service.gov.uk`

---

### Understanding the Email Output

Results arrive as an HTML email to `steve@portland-fuel.co.uk` from `irwin455@hotmail.com`.

**Subject line format:**
`Portland Fuel Leads — 8 leads | Yorkshire | 16/06/2026`

**Email contains:**
- Run summary (companies found, qualified count, search parameters)
- One row per qualifying company (score ≥ 5), sorted highest first

**Score colour coding:**

| Score | Colour | Meaning |
|---|---|---|
| 8–10 | Green | Strong prospect — clear fuel spend, right sector, contactable decision maker |
| 6–7 | Orange | Moderate prospect — likely fuel user but less certainty on size or contact |
| 5 | Red | Borderline — included for awareness, lower priority |
| 1–4 | Not shown | Disqualified — wrong sector, dormant, micro company, or red flags |

**Each company row shows:**
- Company name (linked to Companies House)
- Registered address and directors
- AI score
- Recommended Portland Fuel products
- 2-sentence qualification summary
- Suggested contact approach
- Any red flags (insolvency risk, dormant status, wrong sector)

**If no companies score 5 or above**, you still receive an email confirming the search ran with a "no qualified results" message.

---

### VRAM Note

`qwen3.6:27b` needs approximately 22–26GB VRAM. If ComfyUI has a large model loaded (e.g. Wan2.2 video at ~25GB), Ollama will fall back to system RAM — qualification time increases from ~20s to ~60–90s per company.

Before running a batch of 10+ companies, use the **"Free ComfyUI VRAM"** bookmarklet in Edge Favourites, then wait ~30 seconds before starting the workflow.

---

### Typical Run Times

| Batch size | Estimated time |
|---|---|
| 1–2 companies (test run) | 2–5 minutes |
| 10 companies | 8–12 minutes |
| 25 companies (full batch) | 15–25 minutes |

---

## Overview

A self-hosted n8n workflow running on amelai that takes a user-supplied search (SIC codes + geography), queries Companies House, enriches each result with accounts data and web research, then uses a local Ollama AI model to qualify and score each lead against Portland Fuel's five product lines — delivering a ranked email digest.

---

## Portland Fuel Product Lines (Qualification Targets)

| Product | Ideal Customer | Key Signals |
|---|---|---|
| **Fuel Cards** | Fleet of 3+ vehicles | Road haulage, agriculture, construction, delivery fleets |
| **Bulk Fuel Delivery** | Fixed-site fuel consumption | Agriculture, plant/groundworks, site operations |
| **Bunker Networks** (Keyfuels/UK Fuels) | Road freight operators | SIC 49410 primarily; high mileage, multi-site |
| **Fuel Hedging** | Larger businesses with significant fuel spend | FD/CFO decision maker; fuel as material cost |
| **Fuel Consultancy** | Energy-intensive or multi-product businesses | Complex requirements; looking to optimise procurement |

---

## SIC Code → Product Mapping (from existing codebase)

| SIC | Sector | Products |
|---|---|---|
| 49410 | Freight transport by road | Fuel Cards, Bunker Networks |
| 49310 | Urban/suburban passenger transport | Fuel Cards, Hedging |
| 49390 | Other passenger land transport | Fuel Cards, Hedging |
| 01110 | Arable farming | Fuel Cards, Bulk Delivery |
| 01500 | Mixed farming | Fuel Cards, Bulk Delivery |
| 01610 | Agricultural contractors (crop) | Fuel Cards, Bulk Delivery |
| 01620 | Agricultural contractors (livestock) | Fuel Cards, Bulk Delivery |
| 46710 | Fuel wholesale/distribution | Fuel Cards |
| 47300 | Fuel retail (forecourts) | Fuel Cards |
| 43120 | Site preparation / groundworks | Fuel Cards, Bulk Delivery |
| 77320 | Plant hire | Fuel Cards, Bulk Delivery |

---

## System Architecture

```
┌──────────────────────────────────────────────────────────────┐
│  TRIGGER LAYER                                               │
│  • Chat Trigger (Phase 1 — testing)                          │
│  • Webhook Trigger (Phase 2 — external access)               │
└──────────────────────────┬───────────────────────────────────┘
                           │ "SIC 49410, Yorkshire, 20 companies"
┌──────────────────────────▼───────────────────────────────────┐
│  PARSE INTENT (Ollama — devstral)                            │
│  Extract: sicCodes[], region, postcode, max (default 25)     │
└──────────────────────────┬───────────────────────────────────┘
                           │ {sicCodes, region, max}
┌──────────────────────────▼───────────────────────────────────┐
│  COMPANIES HOUSE SEARCH                                      │
│  GET /advanced-search/companies                              │
│  Params: sic_codes, location, company_status=active          │
│  Auth: Basic (API key as username, empty password)           │
│  API key: f80f8012-64f5-4f45-902a-b1814ea051a1              │
└──────────────────────────┬───────────────────────────────────┘
                           │ Up to 25 companies
┌──────────────────────────▼───────────────────────────────────┐
│  PER-COMPANY ENRICHMENT (loop, sequential with delay)        │
│                                                              │
│  ① GET /company/{number}/officers                            │
│     → director names, roles, appointment dates              │
│                                                              │
│  ② GET /company/{number}/filing-history?category=accounts   │
│     → most recent accounts filing + document_metadata URL   │
│                                                              │
│  ③ GET {document_metadata_url}/content (Accept: application/pdf)
│     → PDF binary → Extract text                             │
│     (turnover, employee count, net assets if present)        │
│                                                              │
│  ④ SearXNG search: "{companyName} {region}"                 │
│     → website, news, trading address, fleet info            │
│                                                              │
│  ⑤ SearXNG search: "{director1} {companyName}"              │
│     → LinkedIn, business press, personal background         │
│                                                              │
│  ⑥ Ollama (qwen3.6:27b) — AI qualification prompt            │
│     Input: company profile + officers + accounts text        │
│           + web search results                              │
│     Output: score (1–10), product matches, reasoning,        │
│             recommended first contact approach               │
└──────────────────────────┬───────────────────────────────────┘
                           │ Array of scored leads
┌──────────────────────────▼───────────────────────────────────┐
│  SORT & FILTER                                               │
│  Sort by score descending                                    │
│  Filter: score >= 5 (discard low-confidence matches)         │
└──────────────────────────┬───────────────────────────────────┘
                           │
┌──────────────────────────▼───────────────────────────────────┐
│  OUTPUT                                                      │
│  Email digest → steve@portland-fuel.co.uk                    │
│  (top leads, formatted with product match + AI reasoning)    │
└──────────────────────────────────────────────────────────────┘
```

---

## Workflow Nodes (n8n implementation)

### Workflow 1: Lead Generation (main)

| # | Node Type | Name | Purpose |
|---|---|---|---|
| 1 | Chat Trigger | `Input: Search Parameters` | Receive search request |
| 2 | HTTP Request | `Ollama: Parse Input` | Extract structured params from natural language |
| 3 | Code | `Build CH Search URL` | Construct Companies House API URL |
| 4 | HTTP Request | `CH: Company Search` | GET advanced-search results |
| 5 | Code | `Limit & Flatten Results` | Cap at max, extract items array |
| 6 | Split In Batches | `Process Each Company` | Loop through companies one at a time |
| 7 | Wait | `Rate Limit Delay` | 1.5s pause between companies |
| 8 | HTTP Request | `CH: Get Officers` | Fetch directors for current company |
| 9 | HTTP Request | `CH: Get Filing History` | Fetch accounts filing history |
| 10 | IF | `Has Accounts PDF?` | Check if document_metadata URL exists |
| 11 | HTTP Request | `CH: Download PDF` | GET PDF binary (Accept: application/pdf) |
| 12 | Extract from File | `Extract PDF Text` | Parse PDF → plain text |
| 13 | HTTP Request | `SearXNG: Company Search` | Web search for company |
| 14 | HTTP Request | `SearXNG: Director Search` | Web search for primary director |
| 15 | Code | `Build AI Prompt` | Assemble all data into qualification prompt |
| 16 | HTTP Request | `Ollama: Qualify Lead` | POST to Ollama generate API |
| 17 | Code | `Parse AI Response` | Extract score, products, reasoning from response |
| 18 | Aggregate | `Collect All Results` | Gather all processed companies |
| 19 | Code | `Sort & Filter` | Sort by score, filter score < 5 |
| 20 | Code | `Format Email HTML` | Build HTML email body |
| 21 | Send Email | `Email: Lead Digest` | SMTP to steve@portland-fuel.co.uk |

---

## Key API Details

### Companies House API

```
Base URL: https://api.company-information.service.gov.uk
Auth: HTTP Basic — API key as username, empty password
API key: f80f8012-64f5-4f45-902a-b1814ea051a1

Search endpoint:
  GET /advanced-search/companies
  ?company_status=active
  &sic_codes=49410,49310
  &location=Yorkshire
  &items_per_page=25

Officers:
  GET /company/{companyNumber}/officers?items_per_page=10

Filing history:
  GET /company/{companyNumber}/filing-history?category=accounts&items_per_page=3

PDF download:
  GET {document_metadata_url}/content
  Header: Accept: application/pdf
  Note: document_metadata_url comes from filing history items[].links.document_metadata
```

### Ollama API (on amelai)

```
Base URL: http://192.168.1.192:11434
Parse endpoint: POST /api/generate
Model for parsing: devstral (fast, good at structured extraction)
Model for qualification: qwen3.6:27b (better reasoning)

Request body:
{
  "model": "qwen3.6:27b",
  "prompt": "...",
  "stream": false,
  "format": "json"
}
```

### SearXNG (on amelai)

```
Base URL: http://192.168.1.192:8080
Search: GET /search?q={query}&format=json&categories=general
Returns: results[].title, results[].url, results[].content (snippet)
```

---

## AI Qualification Prompt Design

The Ollama qualification prompt receives all enriched data and returns structured JSON:

```
You are a B2B lead qualification assistant for Portland Fuel, a UK fuel supplier.
Assess the following company as a potential customer.

COMPANY DATA:
- Name: {companyName}
- Address: {registeredAddress}
- SIC codes: {sicCodes} ({sicDescriptions})
- Incorporated: {incorporatedDate} ({ageYears} years)
- Status: {companyStatus}
- Directors: {directorsList}

ACCOUNTS SUMMARY:
{accountsTextExtract — first 1000 chars}

WEB RESEARCH:
Company search results:
{searxngCompanyResults — top 3 snippets}

Director search results:
{searxngDirectorResults — top 2 snippets}

PORTLAND FUEL PRODUCTS:
1. Fuel Cards — fleets of 3+ vehicles
2. Bulk Fuel Delivery — fixed-site fuel consumption (farms, construction)
3. Bunker Networks — road freight operators, multi-site
4. Fuel Hedging — larger businesses where fuel is a material cost
5. Fuel Consultancy — complex requirements, energy-intensive operations

TASK: Respond with valid JSON only:
{
  "score": <1-10>,
  "productMatches": ["Fuel Cards", "Bulk Delivery"],
  "qualificationSummary": "<2-3 sentences on why this is or isn't a strong lead>",
  "estimatedSize": "<micro/small/medium/large based on accounts>",
  "keyFacts": ["<fact 1>", "<fact 2>", "<fact 3>"],
  "recommendedApproach": "<who to call and what angle to lead with>",
  "disqualifiers": "<any red flags — dormant, insolvency risk, wrong sector>"
}
```

---

## Email Output Format

Subject: `Portland Fuel Lead Digest — {count} leads | {sicDescription} | {region} | {date}`

Body structure:
- Run summary (SIC codes searched, region, total companies found, qualified count)
- One section per qualifying lead (score >= 5), sorted highest first:
  - Company name (linked to Companies House)
  - Address
  - Directors
  - Product match (highlighted)
  - Score and AI reasoning
  - Key facts
  - Recommended approach
- Footer: link to n8n workflow for re-run

---

## Service Dependencies

| Service | URL (from amelai) | Notes |
|---|---|---|
| Companies House API | `https://api.company-information.service.gov.uk` | External; rate limit 600 req/5 min |
| Ollama | `http://192.168.1.192:11434` | Local; ensure qwen3.6:27b is loaded |
| SearXNG | `http://192.168.1.192:8080` | Local; no rate limit concerns |
| SMTP | TBC | M365 SMTP for portland-fuel.co.uk |

---

## Performance Estimates (25 companies)

| Step | Time per company | Total |
|---|---|---|
| Companies House search | — | ~3s (one call) |
| Officers + filing history | ~2s | ~50s |
| PDF download + extract | ~3s | ~75s |
| 2× SearXNG searches | ~2s | ~50s |
| Ollama qualification (qwen3.6:27b) | ~15–25s | ~6–10 min |
| **Total** | | **~10–15 min** |

> If VRAM is constrained (ComfyUI loaded), Ollama falls back to RAM — qualification time increases to ~60–90s per company (~30–40 min total). Free ComfyUI VRAM before running if speed matters.

---

## Implementation Phases

### Phase 1 — Core workflow (implemented)
- [x] n8n workflow JSON for import (`LeadGen_Workflow.json`)
- [x] Chat trigger + input parsing (SIC search + company number modes)
- [x] Companies House search + officers + filing history
- [x] PDF download + text extraction
- [x] SearXNG web enrichment
- [x] Ollama qualification (qwen3.6:27b)
- [x] Email digest output (MyHotmailEmail → steve@portland-fuel.co.uk)
- [ ] Create "Companies House API" Basic Auth credential in n8n
- [ ] Test with 1-2 companies before full run

### Phase 3 — External trigger
- [ ] Add Webhook trigger node alongside Chat trigger
- [ ] Document webhook URL for external callers
- [ ] Optional: simple HTML page with form that POSTs to webhook

### Phase 4 — Enrichment (optional, paid services)
- [ ] Hunter.io for email addresses
- [ ] Apollo.io for contact enrichment
- [ ] HubSpot CRM integration

---

## Setup Checklist (before first run)

- [ ] SMTP credentials added to n8n (Settings → Credentials)
- [ ] Verify `qwen3.6:27b` is available: `ollama list` on amelai
- [ ] Verify SearXNG returns JSON: `curl "http://192.168.1.192:8080/search?q=test&format=json"` on amelai
- [ ] Import workflow JSON into n8n
- [ ] Test with a single known company first

---

## Notes

- **Sole traders and partnerships** do not appear in Companies House — agricultural sole traders are invisible to this system. This is a known limitation documented in LeadGen.md.
- **Registered vs trading address**: Companies House only holds the registered address. The web search step helps surface trading addresses for companies that operate from a different location.
- **PDF accounts**: Micro and small companies file abbreviated accounts — turnover and employee counts may not be present. The accounts text is supplementary; the AI prompt is designed to work without it.
- **VRAM contention**: If ComfyUI has a large model loaded, Ollama may fall back to system RAM. Use the "Free ComfyUI VRAM" bookmarklet before running a large batch. See CLAUDE.md for details.
