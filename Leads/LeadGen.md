<img src="../Portland Long.png" alt="Portland Fuel" style="width:40%; height:auto;" align="right">

# AI Lead Generation Strategy

*Portland Fuel — UK Market (v1.0, May 2026)*

---

## Overview

This document sets out Portland Fuel's options for using artificial intelligence to generate, enrich, qualify, and contact sales leads in the UK market. The aim is to move from an inconsistent, largely manual lead generation process to a scalable, data-driven pipeline — starting at minimal cost with zero external data sharing, and expanding as the approach proves its value.

Two distinct pipelines are needed because the products are fundamentally different in how leads should be worked:

| Pipeline | Products | Target volume | Delivery method |
|---|---|---|---|
| **High-volume phone leads** | Fuel cards, bulk delivery, bunker networks (Keyfuels/UK Fuels) | ~200 leads/month | Ranked CSV for sales team to call |
| **Low-volume warm email** | Fuel hedging, consultancy | ~10–20 warm responses/month | AI-drafted outreach → AI-qualified replies → handoff to specialist |

---

## 1. Lead Profiles by Product

Before generating leads, we must define what a good lead looks like for each product. This is the criteria the AI uses to score and rank companies.

### Fuel Cards

**Ideal customer**: Any business operating a fleet of 3 or more commercial vehicles. Key sectors: road haulage, agriculture (farm vehicles and machinery), construction, manufacturers/retailers/distributors running their own delivery fleets.

**Qualifying signals**: Regular fuel purchases across multiple sites or drivers; need for cost control and mileage reporting; currently buying at the pump or using a competitor card.

**Decision maker**: Fleet manager, transport manager, or owner-director in smaller businesses.

**Disqualifiers**: Single-vehicle operators; businesses with no regular road transport.

---

### Bulk Fuel Delivery

**Ideal customer**: A business or site with on-site fuel storage. High-volume single-site consumers: farms, quarries, plant depots, factories, leisure/holiday parks, remote facilities.

**Qualifying signals**: Large site footprint; agricultural or industrial use; significant plant or machinery; remote location where pump access is impractical.

**Decision maker**: Operations manager, site manager, farm owner.

**Disqualifiers**: Businesses with no on-site fuel storage capacity; consumption too low for bulk economics (typically <5,000 litres per delivery).

---

### Bunker Networks (Keyfuels / UK Fuels)

**Ideal customer**: Road hauliers operating nationally or across multiple UK regions who need access to a nationwide pump network.

**Qualifying signals**: National or multi-regional routes; large HGV fleet (10+ vehicles); currently using a competitor network card.

**Decision maker**: Transport director, fleet manager.

**Disqualifiers**: Local-only operators; van fleets (bunker networks are primarily HGV-focused).

---

### Fuel Hedging

**Ideal customer**: A small to medium-sized business consuming between 25,000 and 500,000 litres of diesel or jet fuel per month. Typical profile: 50,000 litres/month on a 12-month fixed-price contract. The business must have enough financial awareness to understand and benefit from a fixed-price arrangement — we provide the explanation, they bring the appetite.

**Qualifying signals**: High and consistent fuel consumption (hauliers, large agricultural operations, aviation/ground handlers, bus operators); fuel is a significant cost line (>5% of turnover is a rough indicator); any indication the business is price-conscious around fuel (e.g., public commentary on input cost pressures). Bus operators are a particularly strong sector — both private operators and local authority-owned bus companies — as their diesel consumption is high, predictable, and year-round, making them well-suited to a fixed-price hedge.

**Decision maker**: CFO, Finance Director, or owner-director — the person who approves the fuel budget.

**Key differentiator for outreach**: Portland Fuel focuses on the smaller end of the hedging market and will always meet clients face to face — something most competitors do not do. This is a strong message to lead with.

**Disqualifiers**: Businesses consuming <25,000 litres/month; highly irregular consumption (hedging requires predictable volume); public sector entities with procurement restrictions.

---

### Fuel Consultancy

**Ideal customer**: A larger company (typically multi-site, high-volume) needing help with fuel procurement strategy, supplier negotiations, or market intelligence. Key targets include blue chip hauliers, large bus companies, and any other significant fuel user operating at scale in the UK.

**Qualifying signals**: Multiple sites; publicly advertised procurement exercises; new FD or Procurement Director in post; evidence of large fuel expenditure.

**Decision maker**: Group Procurement Director, CFO, FD.

**Note**: Consultancy leads are low volume and typically come via referrals or inbound enquiries. AI-generated cold outreach for consultancy is best treated as brand awareness rather than direct sales activity.

---

## 2. Data Sources

### Phase 1 — In-house, public data only

#### Companies House API

The <a href="https://resources.companieshouse.gov.uk/sic/" target="_blank">Companies House database</a> is a free, publicly available register of all UK limited companies. The API allows automated querying by SIC code (sector), geography, company age, and size (based on filed accounts). Registration for an API key is free.

**Relevant SIC codes for Portland Fuel target sectors:**

| Sector | SIC Code | Description |
|---|---|---|
| Road freight | 49410 | Freight transport by road |
| Bus / urban passenger transport | 49310 | Urban and suburban passenger land transport |
| Coach / private bus operators | 49390 | Other passenger land transport n.e.c. |
| Agriculture — arable | 01110 | Growing of cereals, leguminous crops and oil seeds |
| Agriculture — mixed | 01500 | Mixed farming |
| Agricultural contractors | 01610 | Support activities for crop production |
| Livestock support | 01620 | Support activities for animal production |
| Fuel wholesale / distribution | 46710 | Wholesale of solid, liquid and gaseous fuels |
| Fuel retail (forecourts) | 47300 | Retail sale of automotive fuel in specialised stores |
| Site preparation / groundworks | 43120 | Site preparation |
| Plant hire | 77320 | Renting and leasing of construction and civil engineering machinery |

**What Companies House returns**: Company name, registered address, director names and service addresses, SIC codes, incorporation date.

**What it does NOT return**: Trading address (if different from registered), phone number, website, email, fleet size, or fuel consumption. These require enrichment (see Phase 2) or manual lookup.

**Important limitation**: Companies House only contains registered limited companies. Sole traders and unincorporated farming partnerships — common in agriculture — do not appear. This means agricultural leads from Companies House will skew towards incorporated farming businesses and agri-contractors rather than sole-trader farms.

**Geographic filtering**: The API supports filtering by postcode district or region, allowing the script to prioritise areas where Portland Fuel has local sales coverage.

**Cost**: Free.

---

#### Automated Web Research via n8n (Phase 1 lead enrichment)

Rather than manually looking up each company, the enrichment step can be automated using n8n running on the local amelai server. n8n is a self-hosted workflow automation tool — the equivalent of Zapier or Make.com but running entirely in-house, with no data sent to third-party cloud services.

**How the n8n enrichment workflow works:**

1. The Companies House script outputs a CSV of ranked companies (name, address, SIC code, directors)
2. n8n reads each company name from the CSV
3. For each company, n8n performs a web search (via a search API or direct HTTP request) to locate the company's website
4. n8n fetches the website and any "Contact Us" pages
5. An AI node (Claude API or a locally-running model) reads the page content and extracts: phone numbers, email addresses, key named contacts, and a one-line description of what the business does
6. The enriched data is written back into the spreadsheet alongside the original Companies House record
7. The sales team receives a fully enriched list — company name, address, phone, email, contact name, product match score — ready to call or email without any further manual research

**What this replaces**: The manual step of Googling each company and copying contact details into a spreadsheet — work that would otherwise take hours per week.

**Data control**: Everything runs on amelai. The only external calls are the web searches and website fetches (public information) and the AI scoring request. Portland Fuel's own data never leaves the local network.

**n8n nodes required**: HTTP Request, AI/LLM (Claude API), Google Sheets or CSV output, and a basic loop to process each company in sequence. This is a straightforward workflow to build given an existing n8n installation.

---

### Phase 2 — Trusted providers, signed DPA required

The following tools find contact email addresses and enrich company profiles. Before using any of them, Portland Fuel should obtain a signed Data Processing Agreement (DPA) from the provider. Only prospect company names and domains should be shared — Portland Fuel's own customer data must never be uploaded to these services.

#### Hunter.io

<a href="https://hunter.io/pricing" target="_blank">Hunter.io</a> finds verified email addresses for a given company domain (e.g., `acmehaulage.co.uk`). It holds GDPR certifications and offers a DPA.

**Pricing (2026):** Free (50 credits/month) → Starter $49/month (2,000 credits, ~£38/month) → Growth $149/month (10,000 credits). For initial use, the Starter plan provides sufficient capacity for most lead volumes.

---

#### Apollo.io

<a href="https://www.apollo.io/pricing" target="_blank">Apollo.io</a> combines a company database, contact finder, and email sequencing tools. GDPR-compliant with a DPA available.

**Pricing (2026):** Free (limited) → Basic $59/month (monthly billing) → Professional $99/month. Note: credits expire monthly and phone number lookups cost significantly more than email lookups — budget accordingly.

**Advantage over Hunter**: Apollo includes its own prospecting database, so you can search by job title, company size, sector, and location directly — useful for identifying the right hedging prospect (e.g., "Finance Directors at road haulage companies with 50–500 employees in England") without needing a starting list from Companies House.

---

#### LinkedIn Sales Navigator

LinkedIn Sales Navigator allows targeted search for people by job title, company size, industry, and geography. Particularly useful for identifying the correct decision maker within a target company before sending an email. Approximately £80–100/month per seat. Best used for hedging leads where reaching the FD or CFO directly matters most.

---

## 3. GDPR and Data Control

### Data classification

| Data type | Phase 1 | Phase 2 |
|---|---|---|
| Portland Fuel customer list | Stays in-house — never shares | Stays in-house — never shares |
| Portland Fuel pricing/volumes | Stays in-house — never shares | Stays in-house — never shares |
| Companies House public data | Sent to Claude API for scoring | Same |
| Prospect email addresses | N/A (not collected) | Shared with Hunter/Apollo under DPA |

---

### UK PECR rules for B2B cold email

Under the <a href="https://ico.org.uk/for-organisations/direct-marketing-and-privacy-and-electronic-communications/business-to-business-marketing/" target="_blank">UK Privacy and Electronic Communications Regulations (PECR)</a> and the <a href="https://ico.org.uk/for-organisations/direct-marketing-and-privacy-and-electronic-communications/guide-to-pecr/electronic-and-telephone-marketing/electronic-mail-marketing/" target="_blank">ICO's electronic mail marketing guidance</a>:

**The key rule**: The email marketing consent requirement in PECR **does not apply to corporate subscribers**. This means Portland Fuel can legally send cold marketing emails to employees of registered limited companies without prior consent, provided the following conditions are met:

1. **The recipient is a corporate subscriber** — an employee of a registered company whose employer is the subscriber. Sole traders (including many farmers) are NOT corporate subscribers and require consent before being emailed.
2. **The email is relevant to the recipient's role** — a cold email to a Fleet Manager about fuel cards is appropriate; the same email to that company's HR Manager is not.
3. **The sender is clearly identified** — Portland Fuel's name, address, and contact details must appear in every email.
4. **An opt-out is provided and honoured** — every email must include a simple unsubscribe mechanism, and removals must be processed promptly.
5. **A Legitimate Interests Assessment (LIA) is documented** — formally required since the Data (Use and Access) Act 2025. Portland Fuel should document why legitimate interest applies to its B2B prospecting campaigns before beginning any email outreach.

**What this means in practice:**
- Cold emailing a Ltd company's Fleet Manager or Finance Director about relevant products: **legal**
- Cold emailing a sole trader farmer or owner-driver: **requires prior consent**
- Using personal email addresses (Gmail, Hotmail) found on social media: **not appropriate**

---

### Email domain recommendation

Sending cold emails from `@portland-fuel.co.uk` risks damaging that domain's deliverability reputation if recipients mark messages as spam. The recommended approach is to set up a dedicated subdomain — e.g., `@leads.portland-fuel.co.uk` — for all cold outreach. This protects the main business email domain while keeping the Portland Fuel brand clearly visible.

---

## 4. Phases

### Phase 1: Proof of Concept (~£50–100/month, ~2 weeks to build)

**Goal**: Produce a ranked list of ~200 target companies per month for the sales team to call.

**Tools required:**
- Companies House API — free (requires a free API key registration)
- Anthropic Claude API — pay-per-use, approximately £50/month at typical volumes
- Python script — Claude Code can build this; no external developer required
- Excel or Google Sheets — to present the output to the sales team

**How it works:**

1. The Python script queries the Companies House API for active companies in the target SIC codes and selected UK regions
2. Results are filtered by company status (active only) and size where accounts data is available
3. Each company's public information is passed to the Claude API with a scoring prompt — the API scores the company against each product line's qualifying criteria and assigns a product match
4. The script exports a ranked CSV: company name, address, director names, score, suggested product
5. Sales team manually looks up phone numbers for the top-ranked companies
6. Outbound calls proceed as normal — but from a prioritised, pre-scored list rather than cold prospecting

**Data remains**: Everything runs locally. The only external transfer is public Companies House data sent to the Claude API for scoring — not Portland Fuel Personally Identifiable Information (PII).

**Expected output**: ~200 prioritised phone leads/month, segmented by product line.

**Limitation**: No automated phone or email enrichment in Phase 1. The sales team still does the manual website/phone lookup. This is intentional — keep cost and complexity minimal while proving the concept.

---

### Phase 2: Email Outreach (adds ~£50–150/month)

**Goal**: Add email addresses to the lead list, enabling AI-drafted personalised outreach — primarily for hedging leads.

**Additional tools**: Hunter.io Starter (~£38/month) or Apollo.io Basic (~£45/month).

**How it works:**

1. Phase 1 produces the ranked company list
2. Hunter.io or Apollo.io finds the email address for the relevant decision maker at each target company
3. For **hedging leads** specifically: Claude API drafts a short, personalised cold email for each company — referencing the sector, approximate company size, and the fixed-price fuel offering. Emails should be 3–4 sentences: what we do, why it's relevant to them, a call to action
4. For **fuel card/bulk leads**: email supplements phone calls rather than replacing them
5. Emails are sent manually via Outlook initially
6. When volume warrants it (typically >200 emails/week), a dedicated cold email tool (Smartlead or Instantly.ai, ~£50–80/month) can automate sending, pacing, and unsubscribe handling for PECR compliance

**Expected output**: A flow of email replies from hedging prospects and fuel card enquiries, supplementing the phone lead list.

---

### Phase 3: AI Response Qualification (minimal additional cost)

**Goal**: Filter incoming email replies so only genuine opportunities reach the sales team — avoiding the risk of being overwhelmed by volume.

**Tools**: Microsoft Graph API (reads Portland Fuel's Outlook inbox, no new tool) + Claude API (already in use).

**How it works:**

1. An automated process monitors the outreach inbox for new replies
2. Claude API classifies each reply:
   - **Hot** — clear interest, wants to discuss now → routed immediately to the relevant salesperson with a one-paragraph company summary and the response text
   - **Warm** — positive but not urgent ("contact us in Q3", "send more info") → added to a follow-up queue
   - **Cold** — not interested → no action
   - **Unsubscribe** → removed from outreach list immediately (PECR compliance)
   - **Out of office / bounce** → no action
3. Hedging responses route to the hedging specialist; fuel card/bulk enquiries to the appropriate salesperson
4. The sales team sees only Hot and Warm leads — pre-summarised and ready to act on

**Expected output**: The hedging specialist receives ~5–20 pre-qualified warm leads per month, each with context, rather than triaging a raw inbox.

---

### Phase 4: Scale and CRM Integration (£500–1,000+/month)

**When to implement**: Once Phases 1–3 have proven their value and lead volume justifies further investment.

**CRM**: Begin with HubSpot CRM (free tier available). Leads import automatically from the script; pipeline stages track lead → qualified → meeting → proposal → won/lost. Over time this creates a feedback loop: you can see which SIC codes, regions, and qualification criteria produce the best conversion rates, and refine the AI scoring model accordingly.

**Enrichment at scale**: Clay.com automates enrichment across multiple data sources simultaneously (Companies House, LinkedIn, Hunter, Apollo, and others) and has GDPR-compliant data practices with a DPA available. This replaces the manual steps in Phases 1 and 2.

**LinkedIn outreach**: Tools such as Expandi allow automated LinkedIn connection requests and follow-up sequences. Use with care — LinkedIn's terms restrict automation and aggressive use can result in account restrictions. Best suited to hedging leads where a more personal approach is appropriate.

**Germany and Canada — when ready**: The same framework applies, with localised data sources:

| Market | Company register | Email marketing law |
|---|---|---|
| UK | Companies House API | UK PECR + UK GDPR |
| Germany | Handelsregister | EU GDPR (stricter — legitimate interest more carefully scrutinised) |
| Canada | Corporations Canada + provincial registers | CASL (requires express or implied consent — stricter than PECR) |

---

## 5. Options Comparison

| Approach | Monthly cost | Data shared externally | Build time | Output |
|---|---|---|---|---|
| Phase 1 only | ~£100 | Public data to Claude API only | ~2 weeks | ~200 phone leads, ranked by product fit |
| Phase 1 + 2 | ~£150–250 | Prospect contact info to Hunter/Apollo (DPA) | +1 week | Phone leads + targeted email outreach |
| Phase 1–3 | ~£200–300 | As above | +1 week | Phone leads + qualified warm responses routed to team |
| Full Phase 4 | £500–1,000+ | Prospect data to Clay/HubSpot (DPA) | +4–8 weeks | Fully automated pipeline with CRM tracking |

---

## 6. Recommended Starting Point

**Start with Phase 1.**

1. Build the Companies House + Claude API script (Claude Code can build this in a single session — no developer required)
2. Run it for one month across the primary SIC codes and target regions
3. Give the output to the sales team and track: calls made, connections, meetings booked
4. At the end of month one, review: Are the leads relevant? Do the qualification scores reflect the sales team's on-the-ground judgement?
5. If yes: add Phase 2 email enrichment for hedging leads
6. If hedging email replies come in: add Phase 3 response qualification

**The key proof-of-concept metric**: Cost per qualified meeting. If AI-generated leads produce meetings at a lower cost than the current approach, the investment is justified and scale-up is warranted.

---

## Sources

- <a href="https://ico.org.uk/for-organisations/direct-marketing-and-privacy-and-electronic-communications/business-to-business-marketing/" target="_blank">ICO: Business-to-business marketing guidance</a>
- <a href="https://ico.org.uk/for-organisations/direct-marketing-and-privacy-and-electronic-communications/guide-to-pecr/electronic-and-telephone-marketing/electronic-mail-marketing/" target="_blank">ICO: Electronic mail marketing (PECR guide)</a>
- <a href="https://resources.companieshouse.gov.uk/sic/" target="_blank">Companies House: Standard Industrial Classification (SIC) codes</a>
- <a href="https://hunter.io/pricing" target="_blank">Hunter.io: Plans and pricing</a>
- <a href="https://www.apollo.io/pricing" target="_blank">Apollo.io: Plans and pricing</a>
