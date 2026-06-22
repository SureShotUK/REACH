# Portland Fuel — Customer Profiler

## Overview

The Customer Profiler workflow takes a company registration number, a customer ranking (1–10), and the Portland Fuel products they use, then automatically retrieves the company's SIC codes, directors, and most recent financial data (turnover, employees, net assets) from Companies House. Accounts filed as iXBRL (structured XML) are parsed directly; accounts filed as PDF are converted to images and read by vision AI. The resulting profile is stored inside n8n and a confirmation email is sent to steve@portland-fuel.co.uk. Profiles can be listed, updated, or removed at any time using the same chat interface.

---

## Accessing the Chat Interface

The workflow must be **active** before the chat URL will respond. To activate it:

1. Go to <a href="https://amelai.tail926601.ts.net:5678" target="_blank">https://amelai.tail926601.ts.net:5678</a>
2. Open the **Portland Fuel - Customer Profiler** workflow
3. Toggle the workflow **Active** (top-right switch turns green)

**Production chat URL:**

```
https://amelai.tail926601.ts.net:5678/webhook/plf-profiler/chat
```

Open this URL in any browser to use the chat interface. Paste your command and press Enter.

> **Important:** Do not use the **Chat** button inside the n8n workflow editor — that runs in test mode and profiles will not be saved.

---

## Commands

| Command | Syntax | Notes |
|---|---|---|
| **Add** | `add <regNo> <ranking> <products>` | Adds a new profile. Products are comma-separated. |
| **List** | `list` | Emails a table of all stored profiles, sorted by ranking (highest first). |
| **Remove** | `remove <regNo>` | Permanently deletes a profile. |
| **Update** | `add <regNo> <new ranking> <new products>` | Re-adding an existing company number overwrites the stored profile. |

**Syntax rules:**
- `<regNo>` — Companies House registration number, 6–8 characters (e.g. `06880902`)
- `<ranking>` — Integer from 1 to 10
- `<products>` — One or more Portland Fuel product names, separated by commas

---

## Example Prompts

### Understanding the `add` command

The `add` command takes three parts in a fixed order:

```
add <regNo> <ranking> <products>
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
add 06880902 8 Bulk
add 00445790 6 Card
add 01234567 9 AdBlue
add 09876543 7 Bunker
```

### Multiple products

```
add 06880902 10 Bulk, Hedging
add 00445790 7 Card, AdBlue, Analytics
add 01234567 5 Bulk, Bunker, Card
add 09876543 9 Hedging, Bulk, AdBlue, Card
```

### List and remove

```
list
remove 06880902
```

### Update an existing profile (re-add to overwrite)

```
add 06880902 9 Bulk, Hedging, Analytics
```

---

## What Gets Extracted

For each company added, the workflow retrieves:

| Data | Source |
|---|---|
| Company name, address, status | Companies House company endpoint |
| SIC codes | Companies House company endpoint |
| Directors / officers | Companies House officers endpoint |
| Most recent accounts year | Companies House filing history |
| Turnover (GBP) | Accounts document (iXBRL or PDF) |
| Employee count | Accounts document (iXBRL or PDF) |
| Net assets (GBP) | Accounts document (iXBRL or PDF) |

**Account format detection** is automatic:
- **iXBRL** — structured XHTML/XML accounts are parsed directly for financial figures
- **PDF** — accounts are converted to images and analysed by vision AI (Ollama) running on Amelai; she reads the left column (current year) and extracts the key figures

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
| **add** | `Customer Profile Added: {Company Name} ({regNo})` | Company details, SIC codes, directors, products, ranking, financial summary (turnover, employees, net assets, accounts year) |
| **list** | `Portland Fuel Customer Profiles — N companies \| {date}` | HTML table of all profiles, sorted by ranking highest first; company names link to Companies House |
| **remove** (found) | `Profile Removed: {Company Name} ({regNo})` | Confirmation message |
| **remove** (not found) | `Profile Not Found: {regNo}` | Not-found message |
