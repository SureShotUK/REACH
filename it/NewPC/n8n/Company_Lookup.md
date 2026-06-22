# Portland Fuel — Company Name Lookup

## Overview

The Company Name Lookup workflow takes a list of company names, searches Companies House for each one, and has Amelai score how confident she is in each match. Results are returned directly in the chat window and emailed to steve@portland-fuel.co.uk with a CSV attachment.

---

## Accessing the Chat Interface

The workflow must be **active** before the chat URL will respond. To activate it:

1. Go to <a href="https://amelai.tail926601.ts.net:5678" target="_blank">https://amelai.tail926601.ts.net:5678</a>
2. Open the **Portland Fuel — Company Name Lookup** workflow
3. Toggle the workflow **Active** (top-right switch turns green)

**Production chat URL:**

```
https://amelai.tail926601.ts.net:5678/webhook/pfl-company-lookup/chat
```

> **Important:** Do not use the **Chat** button inside the n8n workflow editor — that runs in test mode and will only process the first company.

---

## Entering Company Names

Paste company names into the chat in either format:

**One per line:**
```
Adams and Green Limited
Bagnall & Morris (Waste Services) Ltd
Boilerjuice Limited
Centrebus Ltd
```

**Comma-separated on one line:**
```
Adams and Green Limited, Bagnall & Morris (Waste Services) Ltd, Boilerjuice Limited, Centrebus Ltd
```

Maximum 50 companies per lookup.

---

## Results

**In the chat window** — a markdown table appears once all companies have been processed:

| Symbol | Meaning |
|---|---|
| ✓ | High confidence (≥80%) |
| ~ | Medium confidence (50–79%) |
| ✗ | Low confidence (<50%) |

Registration numbers in the chat are clickable links to Companies House.

**By email** — a colour-coded HTML table (green/amber/red by confidence) arrives at steve@portland-fuel.co.uk with a CSV file attached containing all results.

---

## Notes

- Processing time depends on the number of companies — allow approximately 20–30 seconds per company while Amelai evaluates each match
- Where Companies House returns multiple possible matches for a name, all candidates above 25% confidence are shown
- The workflow searches the exact name entered; if a company is not found, try variations (e.g. with or without `Ltd`, `Limited`, `&` vs `and`)
