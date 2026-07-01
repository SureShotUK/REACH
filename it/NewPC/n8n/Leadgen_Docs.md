<img src="../../../Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# Portland Fuel — Lead Generation Toolkit

Three n8n workflows running on Amelai that together form a complete lead generation pipeline: find a company's registration number, build a profile, then search for new prospects by sector and geography.

> All workflows require n8n to be **active** at <a href="https://amelai.tail926601.ts.net:5678" target="_blank">https://amelai.tail926601.ts.net:5678</a>. Do not use the Chat button inside the n8n editor — use the production URLs below.

---

## Workflows

### 1. Company Name Lookup

**Purpose:** Takes one or more company names, finds their Companies House registration numbers, scores AI confidence in each match, and emails a CSV to steve@portland-fuel.co.uk.

**Chat URL:**
<a href="https://amelai.tail926601.ts.net:5678/webhook/pfl-company-lookup/chat" target="_blank">https://amelai.tail926601.ts.net:5678/webhook/pfl-company-lookup/chat</a>

**Full documentation:** <a href="file:Company_Lookup.md" target="_blank">Company_Lookup.md</a>

---

### 2. Customer Profiler

**Purpose:** Takes a company registration number, a customer ranking (1–10), and the Portland Fuel products they use, then retrieves financial data and SIC codes from Companies House and stores a structured profile for use by the Lead Generation workflow.

**Chat URL:**
<a href="https://amelai.tail926601.ts.net:5678/webhook/pfl-profiler/chat" target="_blank">https://amelai.tail926601.ts.net:5678/webhook/pfl-profiler/chat</a>

**Full documentation:** <a href="file:Company_Profiler.md" target="_blank">Company_Profiler.md</a>

---

### 3. Lead Generation

**Purpose:** Takes SIC codes and a UK region (or a single registration number), searches Companies House for matching active companies, enriches each result with accounts data and web research, then uses Amelai's local AI to score and qualify each lead — delivering a ranked HTML email to steve@portland-fuel.co.uk.

**Chat URL:**
<a href="https://amelai.tail926601.ts.net:5678/webhook/pfl-lead-gen/chat" target="_blank">https://amelai.tail926601.ts.net:5678/webhook/pfl-lead-gen/chat</a>

**Full documentation:** <a href="file:LeadGen_Workflow_Design.md" target="_blank">LeadGen_Workflow_Design.md</a>

---

## To Do — Customer-Benchmarked Lead Scoring

### Objective

Replace the Lead Generation workflow's current AI scoring (which uses Amelai's general reasoning) with a scoring model grounded in Portland Fuel's actual customer book. New leads will be compared against existing customers who have been ranked and profiled, producing a product-specific confidence score before any contact is made.

---

### How the Three Workflows Fit Together

**Step 1 — Build the benchmark (Customer Profiler)**

Use the Customer Profiler to add every significant existing customer. For each one, record:
- Company registration number
- Ranking (1–10) — apply this *consistently*: the model is only as good as the rankings
- Products they currently take

The profiler automatically retrieves SIC codes, turnover, employee count, net assets, and directors from Companies House. These become the benchmark data against which new leads are measured.

**Step 2 — Find new prospects (Lead Generation)**

Run the Lead Generation workflow as normal — by SIC code + region, or by individual registration number. This returns a list of active companies enriched with the same financial and sector data as the customer profiles.

**Step 3 — Score new leads against the benchmark (new workflow — to be built)**

A new n8n workflow (or an additional step in Lead Generation) will:

1. Load all stored Customer Profiler profiles
2. For each Portland Fuel product, build a feature fingerprint from the customers who take that product, weighted by their ranking — typical SIC codes, turnover range, employee band, net asset position
3. Compare each new lead's data against those fingerprints
4. Output a product-specific score for each lead, replacing or supplementing the current AI score
5. Include this benchmarked score in the Lead Generation email digest

---

### Improvements Needed in the Customer Profiler First

Before building the scoring model, three additions to the profiler will significantly improve accuracy:

| Addition | Why it matters |
|---|---|
| **Notes field** — free-text reason for the ranking | A number alone (e.g. 8) doesn't explain *why*. Without reasoning, the model may find patterns that don't exist. Even a short note ("large mixed fleet, consistent volume") makes rankings comparable. |
| **Store region/county** | Already fetched from Companies House but not saved. Portland Fuel likely has regional coverage considerations; without this the model cannot weight geography. |
| **Accounts confidence flag** | PDF accounts for small companies often have no turnover figure — the AI estimates from context. The model should discount uncertain financial inputs. A simple `high`/`low` flag on each profile would do this. |

---

### Scoring Model Approach

The model will use **weighted similarity matching**, not machine learning — the dataset is too small for that. For each product:

- Compute the distribution of SIC codes, turnover brackets, and employee counts across customers who take that product, weighted by ranking
- For a new lead, score each dimension (exact SIC match, size bracket overlap, financial health) and combine into a product confidence score (0–10)
- Leads that closely resemble high-ranked customers taking a product score highly for that product

This approach needs roughly **15–20 profiled customers per product** to be meaningful. Below that threshold the scores are indicative only.

---

### Risk — Ranking Consistency

The biggest risk to the model is inconsistent rankings. If one customer is ranked 8 for volume and another is ranked 8 for relationship, the model will try to find patterns that don't exist.

Consider before building: whether a **single overall ranking** is sufficient, or whether **per-product scores** (e.g. "Bulk: 9, Cards: 6, Hedging: 3") would produce a more useful rubric. Per-product scores require re-profiling existing customers but would generate a significantly better model.
