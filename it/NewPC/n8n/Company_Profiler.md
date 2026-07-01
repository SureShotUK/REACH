# Portland Fuel — Customer Profiler

## Overview

The Customer Profiler workflow takes a company registration number, a customer ranking (1–10), the Portland Fuel products they use, and an optional note explaining the ranking. It automatically retrieves the company's SIC codes, directors, region, and most recent financial data (turnover, employees, net assets) from Companies House, and flags whether the financial figures were extracted from structured iXBRL accounts or estimated from a PDF. The resulting profile is stored inside n8n and a confirmation email is sent to steve@portland-fuel.co.uk. Profiles can be listed, updated, or removed at any time using the same chat interface.

---

## Accessing the Chat Interface

The workflow must be **active** before the chat URL will respond. To activate it:

1. Go to <a href="https://amelai.tail926601.ts.net:5678" target="_blank">https://amelai.tail926601.ts.net:5678</a>
2. Open the **Portland Fuel - Customer Profiler** workflow
3. Toggle the workflow **Active** (top-right switch turns green)

**Production chat URL:**

```
https://amelai.tail926601.ts.net:5678/webhook/pfl-profiler/chat
```

Open this URL in any browser to use the chat interface. Paste your command and press Enter.

> **Important:** Do not use the **Chat** button inside the n8n workflow editor — that runs in test mode and profiles will not be saved.

---

## Commands

| Command | Syntax | Notes |
|---|---|---|
| **Add** | `add <regNo> <ranking> <products> \| <note>` | Adds a new profile. Products are comma-separated. Note is optional but recommended. |
| **List** | `list` | Emails a table of all stored profiles, sorted by ranking (highest first). |
| **Remove** | `remove <regNo>` | Permanently deletes a profile. |
| **Update** | `add <regNo> <new ranking> <new products> \| <note>` | Re-adding an existing company number overwrites the stored profile. |

**Syntax rules:**
- `<regNo>` — Companies House registration number, 6–8 characters (e.g. `06880902`)
- `<ranking>` — Integer from 1 to 10
- `<products>` — One or more Portland Fuel product names, separated by commas
- `<note>` — Optional free-text reason for the ranking, separated from the products by ` | `. Used by the lead scoring model to validate ranking consistency. Should explain *why* this customer has this ranking (e.g. `large mixed fleet, consistent volume, growth trajectory`).

---

## Example Prompts

### Understanding the `add` command

The `add` command takes three required parts and one optional part:

```
add <regNo> <ranking> <products> | <note>
```

---

**`<regNo>` — Company Registration Number**

The unique identifier assigned to a UK company by Companies House. It is 6–8 characters, typically 8 digits with leading zeros (e.g. `06880902`). Find it at <a href="https://find-and-update.company-information.service.gov.uk" target="_blank">find-and-update.company-information.service.gov.uk</a> by searching the company name.

---

**`<ranking>` — Customer Desirability Score**

A subjective internal score from **1** (lowest) to **10** (highest) reflecting how desirable this company is as a Portland Fuel customer. The score is set by you at the time of entry and can be updated at any time by re-adding the same company number.

---

**`<products>` — Portland Fuel Products**

One or more products from the list below, entered as a **comma-separated list** with no quotes. Use the exact names shown.

| Product | Description |
|---|---|
| `Bulk` | Bulk fuel delivery |
| `Card` | Fuel card accounts |
| `AdBlue` | AdBlue supply |
| `Bunker` | Bunker fuel (marine) |
| `Hedging` | Fuel price hedging |
| `Analytics` | Fuel analytics and reporting |

Enter all products that apply to this customer in a single comma-separated list:

```
add 06880902 8 Bulk, Card, AdBlue
```

There is no limit on the number of products per entry.

---

### Single product

```
add 06880902 8 Bulk | large arable farm, consistent seasonal volumes
add 00445790 6 Card | regional haulier, small fleet, occasional late payer
add 01234567 9 AdBlue | groundworks contractor, growing rapidly, good relationship
add 09876543 7 Bunker | coastal freight operator, steady volumes
```

### Multiple products

```
add 06880902 10 Bulk, Hedging | mixed farming operation, significant fuel spend, financially strong
add 00445790 7 Card, AdBlue, Analytics | national logistics firm, multi-depot, wants reporting
add 01234567 5 Bulk, Bunker, Card | plant hire company, variable spend, under new management
add 09876543 9 Hedging, Bulk, AdBlue, Card | large road haulier, fuel is material cost, FD engaged
```

### Without a note (note is optional)

```
add 06880902 8 Bulk
add 00445790 7 Card, AdBlue
```

### List and remove

```
list
remove 06880902
```

### Update an existing profile (re-add to overwrite)

```
add 06880902 9 Bulk, Hedging, Analytics | revised up — expanded fleet in 2025, now multi-site
```

---

## What Gets Extracted

For each company added, the workflow retrieves and stores:

| Data | Source | Notes |
|---|---|---|
| Company name, address, status | Companies House company endpoint | |
| Region / county | Companies House company endpoint | Derived from registered address postcode |
| SIC codes | Companies House company endpoint | |
| Directors / officers | Companies House officers endpoint | |
| Most recent accounts year | Companies House filing history | |
| Turnover (GBP) | Accounts document (iXBRL or PDF) | |
| Employee count | Accounts document (iXBRL or PDF) | |
| Net assets (GBP) | Accounts document (iXBRL or PDF) | |
| Accounts confidence | Derived from accounts format | `high` = iXBRL (parsed directly); `low` = PDF (AI-estimated) |
| Ranking note | Supplied by user | Free-text reason for the ranking; optional but used by lead scoring model |

**Account format detection** is automatic:
- **iXBRL** — structured XHTML/XML accounts are parsed directly for financial figures; confidence flagged as `high`
- **PDF** — accounts are converted to images and analysed by vision AI (Ollama) running on Amelai; she reads the left column (current year) and extracts the key figures; confidence flagged as `low` as figures may be incomplete or estimated

---

## Data Storage

Profiles are stored in **n8n workflow static data** — a key/value store built into the workflow itself, indexed by company registration number.

**Implications:**
- Data persists as long as the n8n container and `n8n_data` Docker volume exist
- Profiles are **not** stored in an external database — they do not survive if the workflow is deleted or n8n is reinstalled from scratch
- To take a manual backup, run `list` and save the emailed table; it contains all current profiles

---

## Email Notifications

All emails are sent to steve@portland-fuel.co.uk.

| Action | Subject | Content |
|---|---|---|
| **add** | `Customer Profile Added: {Company Name} ({regNo})` | Company details, SIC codes, region, directors, products, ranking, ranking note, financial summary (turnover, employees, net assets, accounts year, accounts confidence) |
| **list** | `Portland Fuel Customer Profiles — N companies \| {date}` | HTML table of all profiles, sorted by ranking highest first; company names link to Companies House |
| **remove** (found) | `Profile Removed: {Company Name} ({regNo})` | Confirmation message |
| **remove** (not found) | `Profile Not Found: {regNo}` | Not-found message |
