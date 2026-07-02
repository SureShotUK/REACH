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

> **`update` command requires the current workflow file:** the `update` command described below is only available once `n8n/Main/NewCustomerProfiler.json` has been imported and activated. It is not present in the previously-published `Portland Fuel - Customer Profiler.json`.

---

## Commands

| Command | Syntax | Notes |
|---|---|---|
| **Add** | `add <regNo> <ranking> <products> \| <note>` | Adds a new profile, running the full Companies House + financials lookup. |
| **Update** | `update <regNo> <Field>=<Value> \| <Field>=<Value>` | Changes specific fields on an **existing** profile without re-running the Companies House/financials lookup. See below. |
| **List** | `list` | Emails a table of all stored profiles, sorted by ranking (highest first). |
| **Remove** | `remove <regNo>` | Permanently deletes a profile. |

> **Two ways to change an existing profile:** re-running `add` on the same registration number overwrites the *entire* profile and re-fetches everything from Companies House (use this if the company's underlying data may have changed). The `update` command changes only the fields you name, instantly, with no external lookups (use this for a quick correction like a ranking change or adding a product).

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

### Update an existing profile (re-add to overwrite everything)

Use this when the company's underlying details may genuinely have changed and you want a fresh Companies House + financials lookup:

```
add 06880902 9 Bulk, Hedging, Analytics | revised up — expanded fleet in 2025, now multi-site
```

---

## Understanding the `update` command

The `update` command changes one or more fields on a profile that has **already been added** — it does not contact Companies House or re-extract financials, so it runs instantly. Use it for quick corrections: a ranking change, adding a product, fixing the region, correcting a figure, etc.

```
update <regNo> <Field>=<Value> | <Field>=<Value> | ...
```

- `<regNo>` — the same Companies House registration number used when the profile was added
- `<Field>` — must match one of the field names below (not case-sensitive)
- Multiple fields are separated by ` | `
- If the company hasn't been added yet, you'll get a "Profile Not Found" email telling you to `add` it first

**Updatable fields** — the field name must match a column heading from the `list` command's CSV export:

| Field name | Behaviour | Example |
|---|---|---|
| `Ranking` | Overwrites the ranking (1–10) | `Ranking=10` |
| `Products` | **Adds** to the existing product list — does not remove anything already there | `Products=Hedging, Fuelcards` |
| `Region` | Overwrites the region | `Region=Yorkshire` |
| `Company Name` | Overwrites the company name | `Company Name=Test Farms Ltd` |
| `SIC Codes` | **Adds** to the existing SIC code list | `SIC Codes=01110` |
| `Turnover` | Overwrites turnover (GBP, numbers only) | `Turnover=750000` |
| `Employees` | Overwrites employee count (numbers only) | `Employees=15` |
| `Net Assets` | Overwrites net assets (GBP, numbers only) | `Net Assets=250000` |
| `Accounts Year` | Overwrites the accounts year | `Accounts Year=2025` |
| `Confidence` | Overwrites the accounts confidence flag | `Confidence=high` |
| `Ranking Note` | Overwrites the free-text ranking note | `Ranking Note=upgraded after new contract win` |
| `Profile Date` | Overwrites the profile date | `Profile Date=2026-07-02` |

> **Products and SIC Codes are merged, everything else is overwritten.** `Products=Hedging, Fuelcards` adds `Fuelcards` to whatever products the customer already had — it will not remove `Hedging` (or any other existing product) even if you don't mention it. Every other field is replaced outright with the new value, so setting `Ranking=10` when the ranking is already 10 is harmless — it just confirms the value.
>
> `Company Number` cannot be updated — it's the identifier used to find the profile in the first place.

### Update examples

```
update 06880902 Products=Hedging, Fuelcards | Ranking=10
update 06880902 Region=Yorkshire
update 00445790 Ranking Note=upgraded after new contract win | Ranking=9
update 01234567 Turnover=1200000 | Employees=22 | Accounts Year=2025
```

You'll get a confirmation email listing exactly what changed (old value → new value), plus any fields that couldn't be applied (unknown field name, non-numeric value where a number was expected, etc.) — the rest of the update still goes through even if one field fails.

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
| **update** (success) | `Profile Updated: {Company Name} ({regNo})` | List of changed fields, old value → new value |
| **update** (partial/failed field) | `Profile Update Issue: {Company Name} ({regNo})` | Successful changes plus a list of any fields that couldn't be applied and why |
| **update** (not found) | `Profile Not Found: {regNo}` | Tells you to `add` the company first |
| **list** | `Portland Fuel Customer Profiles — N companies \| {date}` | HTML table of all profiles, sorted by ranking highest first; company names link to Companies House |
| **remove** (found) | `Profile Removed: {Company Name} ({regNo})` | Confirmation message |
| **remove** (not found) | `Profile Not Found: {regNo}` | Not-found message |
