# AI Provider Enterprise Privacy Guide

*Internal reference — Portland Fuel. Last updated: June 2026. USD/EUR converted at £1 = $1.35 / £1 = €1.18.*

---

## Purpose

This document covers the privacy controls, data deletion options, and enterprise account details for the six main AI assistant providers. The key question for each is: **can you use it with business-sensitive information without that data being used to train their AI models?**

The short answer for all paid business tiers is yes — but the controls, pricing, and data residency vary significantly.

---

## Quick Comparison

| Provider | Entry Business Plan | Price/user/month | Trains on data? | Delete data? | Data residency | Notable compliance |
|----------|-------------------|-----------------|-----------------|--------------|---------------|-------------------|
| **ChatGPT (OpenAI)** | Team | ~£22 | No (Team+) | Yes — admin portal | US (EU at Enterprise) | SOC 2 Type 2 |
| **Microsoft Copilot** | Copilot for M365 | £25 add-on* | No | Yes — Purview portal | UK/EU available | ISO 27001, SOC 2 |
| **Claude (Anthropic)** | Team | ~£22 | No (Pro+) | Yes — admin console | US only | SOC 2 Type 2, ISO 27001 |
| **Gemini (Google)** | Workspace Business Standard | ~£10 | No | Yes — admin console | UK/EU available | ISO 27001, SOC 2, SOC 3 |
| **Perplexity** | Enterprise Pro | Custom (contact sales) | No (Enterprise) | Yes | US only | SOC 2 Type 2 |
| **Mistral (le Chat)** | Pro | ~£13 | No (Pro+) | Yes | EU (France) default | ISO 27001, GDPR native |

*\*Microsoft Copilot requires an existing M365 Business Standard (~£10.30/user) or higher subscription.*

**Key takeaway:** All six providers guarantee no training on your data at paid business tiers. The main differentiators are price, data residency (EU/UK vs. US only), and the depth of admin controls.

---

## 1. ChatGPT — OpenAI

<a href="https://openai.com/chatgpt/pricing/" target="_blank">openai.com/chatgpt/pricing</a>

### Plans for Small Business (10–20 users)

| Plan | Price | Min Users | Training on data? | Notes |
|------|-------|-----------|------------------|-------|
| Plus | ~£15/user/month | 1 | No (opt-out available) | Individual; no admin controls |
| **Team** | **~£22/user/month** (annual) | 2 | **No — automatic** | Admin workspace, shared GPTs, 32K context |
| Enterprise | Contact sales | No minimum* | No | Custom pricing, unlimited GPT-4o, 128K context |

*Enterprise pricing is typically negotiated; historically suited to larger deployments but OpenAI will discuss smaller teams.*

**For 10 users on Team plan: ~£220/month | For 20 users: ~£440/month**

### Privacy & Data Controls

- **Training:** Team and Enterprise plans: OpenAI does not use conversations to train models. This is automatic — no opt-out needed.
- **Data retention:** Conversation history stored; users can delete individual conversations. Admins on Team/Enterprise can manage workspace data.
- **Deletion:** Users delete via chat history settings. Enterprise admins can submit bulk deletion requests.
- **Data residency:** Data processed in the US by default. EU data residency available at Enterprise tier under negotiated agreement.
- **Conversation history:** Can be disabled entirely per-user; conversations then not stored beyond the session.

### Admin Controls (Team plan)
- Workspace dashboard to manage users and seats
- Shared custom GPTs within the organisation
- Usage analytics
- SSO available at Enterprise tier

### Compliance
- SOC 2 Type 2 certified
- GDPR Data Processing Agreement (DPA) available
- CCPA compliant

**Pros:** Most widely used, best model capability (GPT-4o), Team plan accessible with no sales call, flexible pricing.

**Cons:** Data residency is US-only on Team plan; Enterprise requires sales negotiation; no UK/EU data centre option on standard plans.

---

## 2. Microsoft Copilot — Microsoft 365

<a href="https://www.microsoft.com/en-gb/microsoft-365/copilot/copilot-for-work" target="_blank">microsoft.com/en-gb — Copilot for Work</a>

### Plans for Small Business (10–20 users)

Microsoft Copilot is an **add-on** to an existing Microsoft 365 subscription. You need both.

| Component | UK Price/user/month | Notes |
|-----------|-------------------|-------|
| M365 Business Standard (prerequisite) | £10.30 | Teams, Outlook, Word, Excel, SharePoint |
| M365 Business Premium (prerequisite) | £18.60 | Adds advanced security features |
| **Microsoft 365 Copilot (add-on)** | **£25.00** | AI across all M365 apps |
| **Total (Standard + Copilot)** | **£35.30/user/month** | Most common SMB combination |

**For 10 users: ~£353/month | For 20 users: ~£706/month**

Note: Microsoft 365 Copilot requires a qualifying base plan. The add-on enables Copilot in Outlook, Word, Excel, PowerPoint, Teams, and as a standalone Copilot Chat interface.

### Privacy & Data Controls

- **Training:** Microsoft does **not** train its foundation models on Microsoft 365 customer data. This policy is contractual and applies to all paid M365 plans including the Copilot add-on.
- **Data retention:** Copilot interactions in Teams/Outlook are subject to your existing M365 retention policies. Admins control retention via Microsoft Purview.
- **Deletion:** Admins manage data via the Microsoft Purview compliance portal — full control over retention labels, deletion schedules, and eDiscovery.
- **Data residency:** Data processed in the EU/UK for European customers. Microsoft commits to storing data within the tenant's designated geography.
- **Conversation history:** Stored within M365 data boundary (not sent to OpenAI's servers; Microsoft operates separate Azure OpenAI instances).

### Admin Controls
- Full Microsoft Purview integration for compliance and data governance
- Granular policy controls per user/group
- Audit logs via Microsoft 365 admin centre
- Data Loss Prevention (DLP) policies apply to Copilot interactions

### Compliance
- ISO 27001, ISO 27017, ISO 27018
- SOC 2 Type 2
- UK Cyber Essentials Plus
- GDPR DPA included as standard

**Pros:** Best option if already using Microsoft 365 — no extra login, integrates into familiar tools, strongest UK/EU data residency story, mature compliance posture.

**Cons:** Requires existing M365 subscription (high baseline cost); £25 add-on is expensive on top; less flexible than standalone AI tools; model quality dependent on Microsoft's Azure OpenAI integration.

---

## 3. Claude — Anthropic

<a href="https://www.anthropic.com/pricing" target="_blank">anthropic.com/pricing</a>

### Plans for Small Business (10–20 users)

| Plan | Price | Min Users | Training on data? | Notes |
|------|-------|-----------|------------------|-------|
| Pro | ~£15/user/month | 1 | No | Individual; access to all Claude models, Projects |
| **Team** | **~£22/user/month** (annual) | 5 | **No — automatic** | Admin console, higher rate limits, usage visibility |
| Enterprise | Contact sales | Flexible | No | SSO/SAML, SCIM, 200K context, audit logs, DLP, custom retention |

**For 10 users on Team plan: ~£220/month | For 20 users: ~£440/month**

Note: Team plan requires a minimum of 5 users. For fewer users, Pro plan (individual accounts) is the option, though without admin controls.

### Privacy & Data Controls

- **Training:** Anthropic does not train models on conversations from Pro, Team, or Enterprise plans. Free tier: feedback interactions may be reviewed; opt-out available in settings.
- **Data retention:** Conversation history stored in Projects; users can delete at any time. Enterprise customers can negotiate custom data retention periods.
- **Deletion:** Users delete conversations via the Claude interface. Team admins can view usage. Enterprise provides full admin deletion tools.
- **Data residency:** US-based (AWS infrastructure). **No EU or UK data residency option on current plans** — all data processed in the United States.
- **Enterprise:** Governed by a separate customer agreement; Anthropic's standard privacy policy explicitly states it does not apply to Enterprise account data processing.

### Admin Controls (Team plan)
- Admin console with usage analytics per user
- Seat management and billing
- Priority access and higher usage limits
- Enterprise adds: SSO, SCIM user provisioning, audit logs, DLP integrations

### Compliance
- SOC 2 Type 2 certified
- ISO 27001 certified
- GDPR Data Processing Agreement available
- HIPAA Business Associate Agreement available (Enterprise only)

**Pros:** Best model quality for writing and reasoning tasks, strong privacy commitment at paid tiers, Enterprise has very deep controls, HIPAA BAA available for sensitive use cases.

**Cons:** No UK/EU data residency — data is processed in the US. This may be a concern for certain types of sensitive business data. Team plan minimum of 5 users.

---

## 4. Gemini — Google Workspace

<a href="https://workspace.google.com/pricing" target="_blank">workspace.google.com/pricing</a>

### Plans for Small Business (10–20 users)

Google has integrated Gemini AI features into all Workspace plans. Gemini in Gmail, Docs, Sheets, Slides, and Meet is included from Business Standard upward.

| Plan | UK Price/user/month | Gemini features | Notes |
|------|-------------------|-----------------|-------|
| Business Starter | £5.20 | Limited | 30GB storage; basic Gemini |
| **Business Standard** | **£10.30** | **Full Gemini in all apps** | 2TB storage, Meet recordings, Gemini for Gmail/Docs/Sheets |
| Business Plus | £15.40 | Full + enhanced | Advanced eDiscovery, audit |
| Enterprise | Contact sales | Full + admin-grade | Custom pricing, DLP, Vault |

**For 10 users on Business Standard: ~£103/month | For 20 users: ~£206/month**

This is the most cost-effective option with AI included. Google Workspace Business Standard gives full Gemini access across all apps for £10.30/user/month — no separate AI add-on required.

### Privacy & Data Controls

- **Training:** Google has **explicitly committed** that it does not use Google Workspace customer data to train its AI models (including Gemini). This policy has been in place since Google Apps for Work and was reaffirmed for AI features.
- **Data retention:** Admin-configurable retention policies. Data can be retained for a defined period or deleted immediately. Google Vault available for archiving and eDiscovery (Business Plus and above).
- **Deletion:** Admins can delete individual users' data immediately via the Admin console. 30-day soft delete (recoverable) followed by permanent deletion.
- **Data residency:** Workspace data can be stored in the EU or US based on your data region selection. UK customers can select EU data residency. Available from Business Standard upward.

### Admin Controls
- Full Google Admin console — one of the most mature admin platforms available
- Per-user, per-OU (Organisational Unit) policy controls
- Data region selection (EU/US)
- DLP policies configurable at Business Plus+
- Google Vault for legal holds and eDiscovery (add-on)

### Compliance
- ISO 27001, ISO 27017, ISO 27018
- SOC 2 Type 2, SOC 3
- CSA STAR Level 2
- GDPR DPA included as standard; UK GDPR addendum available

**Pros:** Best value for money — full AI features bundled into standard business plan; strongest compliance portfolio; EU data residency available; most mature admin console of all providers.

**Cons:** AI quality of Gemini for business writing tasks is generally considered behind Claude and ChatGPT; deeply integrated with Google ecosystem (less useful if not using Gmail/Docs); Gemini as a standalone chat tool is less capable than ChatGPT or Claude for open-ended reasoning.

---

## 5. Perplexity — Enterprise Pro

<a href="https://www.perplexity.ai" target="_blank">perplexity.ai</a>

### Plans for Small Business (10–20 users)

| Plan | Price | Min Users | Training on data? | Notes |
|------|-------|-----------|------------------|-------|
| Pro (individual) | ~£15/user/month | 1 | No | Web search + AI reasoning; no admin controls |
| **Enterprise Pro** | **Contact sales** (~£30–40/user/month estimated) | 5+ | **No** | SSO, admin dashboard, team management, API access |

Perplexity does not publish Enterprise Pro pricing publicly. Based on available information, enterprise pricing is typically in the range of $40–50/user/month (~£30–37). Contact Perplexity sales for a quote.

**Perplexity is unique** in that it combines real-time web search with AI reasoning — ideal for research tasks rather than document drafting.

### Privacy & Data Controls

- **Training:** Enterprise Pro queries are not used for model training.
- **Data retention:** Search and conversation history stored; users can delete at any time. Enterprise may have configurable retention.
- **Deletion:** Users can delete history via account settings. Enterprise admins have dashboard controls.
- **Data residency:** US-based. No EU or UK data residency option currently available.

### Admin Controls (Enterprise Pro)
- SSO (SAML/OIDC)
- Admin dashboard with usage analytics
- Team management and seat controls
- API access for integration

### Compliance
- SOC 2 Type 2 certified (achieved 2024)
- GDPR Data Processing Agreement available

**Pros:** Best tool for research and fact-finding with live web access; always cites sources; Enterprise removes training concerns.

**Cons:** US-only data residency; pricing not transparent (requires sales contact); primarily a research tool — less suited to drafting documents or complex reasoning than Claude/ChatGPT; smaller company with less mature compliance posture than Microsoft or Google.

---

## 6. Mistral — le Chat

<a href="https://mistral.ai/products/le-chat" target="_blank">mistral.ai/products/le-chat</a>

### Plans for Small Business (10–20 users)

| Plan | Price | Min Users | Training on data? | Notes |
|------|-------|-----------|------------------|-------|
| Free | Free | 1 | No (EU residency default) | EU-based, good privacy baseline even free |
| **Pro** | **~£13/user/month** (€14.99) | 1 | **No** | All models, higher limits, no admin controls |
| Enterprise | Contact sales | Flexible | No | Dedicated infra, EU residency guaranteed, on-premise option, DPA, SSO |

**For 10 users on Pro: ~£130/month | For 20 users: ~£260/month**

Mistral Enterprise pricing requires a sales call but is generally competitive with other providers. The key differentiator is the on-premise deployment option — no data ever leaves your infrastructure.

### Privacy & Data Controls

- **Training:** Mistral does not train on user conversations at Pro or Enterprise tier. The free tier also has a good privacy baseline due to EU regulatory environment.
- **Data retention:** Pro: conversations stored and deletable by users. Enterprise: configurable retention; full deletion on request.
- **Deletion:** Users can delete conversation history. Enterprise has contractual data deletion guarantees.
- **Data residency:** **EU data residency by default** — Mistral is headquartered in Paris, France. Data is processed and stored within the EU. This is a significant advantage for businesses concerned about data leaving Europe.
- **On-premise deployment:** Enterprise customers can deploy Mistral models on their own infrastructure — no data is sent to Mistral at all.

### Admin Controls (Enterprise)
- SSO/SAML integration
- Admin console and usage analytics
- Custom data retention policies
- On-premise deployment option
- Data Processing Agreement (DPA) included
- Model customisation/fine-tuning available

### Compliance
- ISO 27001 certified
- GDPR native (French company, EU-regulated)
- SOC 2 Type 2 (in progress / achieved 2025)
- French ANSSI security standards

**Pros:** Only major EU-based provider — data residency in France/EU by default at all tiers; on-premise deployment option is unique among this group; GDPR-native compliance; Pro plan at £13 is the cheapest paid tier; good model quality, particularly for European languages.

**Cons:** Models are generally not quite as capable as GPT-4o or Claude Sonnet for complex reasoning; smaller ecosystem and fewer integrations; Enterprise features require sales contact; less name recognition among non-technical staff.

---

## Summary: Which to Use

| Use Case | Recommended Option |
|----------|-------------------|
| Already using Microsoft 365 | **Microsoft Copilot** — best integration, UK/EU data residency |
| Best AI quality for writing / analysis | **Claude Team** or **ChatGPT Team** — comparable; Claude slightly ahead for long documents |
| Best value (AI included in business tools) | **Google Workspace Business Standard** — Gemini included at £10.30/user |
| Strongest EU data residency concern | **Mistral le Chat Enterprise** — data stays in France/EU; on-premise option |
| Research with live web search | **Perplexity Enterprise Pro** — unique web-grounded responses |
| Testing before commitment | **Mistral Pro** (~£13) or **ChatGPT Team** (no minimum commitment on monthly billing) |

### Notes for Portland Fuel

- For general business use (drafting, research, summarisation): **Claude Team or ChatGPT Team** offer the best capability at roughly the same price (~£22/user/month).
- If customer or lead data may be included in prompts and you want data to stay in Europe: **Mistral Enterprise** is the only option that guarantees EU data residency contractually. Alternatively, run a private model locally (see `RemotePrivateAI.md`).
- **Microsoft Copilot is worth considering** if M365 is already in use — the AI features come at a premium (£25/user add-on) but the data governance story is the most mature.

---

*All pricing is indicative as of June 2026 and subject to change. Enterprise pricing requires direct sales engagement for all providers. USD converted at £1 = $1.35; EUR at £1 = €1.18.*

*Sources: <a href="https://www.anthropic.com/pricing" target="_blank">Anthropic Pricing</a> · <a href="https://www.anthropic.com/legal/privacy" target="_blank">Anthropic Privacy Policy</a> · <a href="https://openai.com/chatgpt/pricing/" target="_blank">OpenAI ChatGPT Pricing</a> · <a href="https://openai.com/enterprise-privacy/" target="_blank">OpenAI Enterprise Privacy</a> · <a href="https://workspace.google.com/pricing" target="_blank">Google Workspace Pricing</a> · <a href="https://www.microsoft.com/en-gb/microsoft-365/business/compare-all-plans" target="_blank">Microsoft 365 Plans</a> · <a href="https://mistral.ai/products/le-chat" target="_blank">Mistral le Chat</a> · <a href="https://mistral.ai/privacy-policy" target="_blank">Mistral Privacy Policy</a>*
