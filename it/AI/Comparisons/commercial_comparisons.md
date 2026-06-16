# AI Tools for SME Office Environments

**Audience:** Small businesses of ~20 staff using Microsoft 365 (Word, Excel, Outlook, PowerPoint)  
**Topic:** Available AI tools, capabilities, pricing, and commercial privacy controls  
**Date:** June 2026  
**Companion document:** [AI Privacy Policy Comparison](comparisons.md) (consumer-tier privacy analysis)

---

## Introduction

AI assistant tools have matured rapidly and are now practically deployable in a small business of 20 people. For a team running Microsoft 365 daily, the key question is not whether AI can help — it is which product integrates with what you already use, at what cost, and under what data protection terms.

This document covers the four main options, focusing on:
- What each tool can actually do inside Word, Excel, Outlook, and PowerPoint
- Pricing for a 20-person team
- Privacy and data protection under commercial plans

The most important privacy finding for business users: **all four major providers exclude commercial customer data from model training by default**. This is a significant improvement over consumer plans and removes the most common concern about using AI in a business setting.

---

## 1. Microsoft 365 Copilot

### What It Is

Microsoft 365 Copilot is an AI layer built directly into the Microsoft 365 application suite. Unlike all other tools in this comparison, it is embedded inside the applications your staff already use — there is no separate tool to learn, no browser tab to open, and no copy-pasting between apps. It has access to your organisation's files in SharePoint, your emails in Exchange, your calendar, and your Teams conversations via Microsoft Graph.

As of 2026, Copilot has moved beyond simple "generate this text" commands into **agentic** operation — it can plan and execute multi-step tasks within a document or workbook on your behalf.

<a href="https://www.microsoft.com/en-us/microsoft-365-copilot" target="_blank">Microsoft 365 Copilot Overview</a>

### What It Does — Per Application

| App | Key Capabilities |
|---|---|
| **Word** | Draft documents from a prompt or meeting notes; rewrite and restructure existing text; adjust tone; real-time writing suggestions as you type; multi-step agentic document building (plan, draft, refine in one command) |
| **Excel** | Explore and explain data in plain English; create and explain formulas; build charts and pivot tables from a prompt; Python-powered advanced analysis without leaving the workbook; works with locally-stored files |
| **Outlook** | Summarise long email threads; draft replies matching your tone; voice-driven email management (summarise, reply, archive, flag — hands-free); Dynamic Action Button surfacing relevant actions in context |
| **PowerPoint** | Build a presentation end-to-end from a prompt or a Word document; add and edit slides via natural language; AI-generated images (GPT-Image or Flux models); single-click built-in skills for common tasks |
| **Teams** | Summarise meetings in real time; answer "what did I miss?" after joining late; capture action items; search across meeting transcripts |
| **Copilot Chat** | Cross-app AI chat grounded in your work data (emails, files, calendar) — find anything across your M365 tenant, draft content from multiple sources, answer questions about your business data |

### The Free vs Paid Distinction

Microsoft includes a basic version called **Copilot Chat** with all eligible Microsoft 365 subscriptions at no extra cost. It is important to understand what this free tier does *not* include:

| Feature | Copilot Chat (free, included) | M365 Copilot Business (paid add-on) |
|---|---|---|
| Web-grounded AI chat | Yes | Yes |
| File upload | Yes | Yes |
| Access to your emails/calendar | **No** | **Yes** |
| Access to your SharePoint files | **No** | **Yes** |
| Copilot inside Word / Excel / PowerPoint / Outlook | **No** | **Yes** |
| Copilot inside Teams meetings | **No** | **Yes** |

The free Copilot Chat is a useful AI chat tool. The paid M365 Copilot is a different product that integrates your business data.

<a href="https://support.microsoft.com/en-us/topic/what-s-the-difference-between-microsoft-copilot-free-and-copilot-in-microsoft-365-cfff4791-694a-4d90-9c9c-1eb3fb28e842" target="_blank">Microsoft: Difference between Copilot (free) and Microsoft 365 Copilot</a>

### Plans and Pricing

M365 Copilot Business is an add-on to an existing Microsoft 365 Business subscription. The table below shows the combined cost of the base M365 plan plus the Copilot add-on:

| Bundle | Per User / Month | What's Included |
|---|---|---|
| M365 Business Basic + Copilot Business | ~$21 (add-on only; Basic is ~$6 on top) | Web apps, cloud storage, Teams, Exchange + Copilot in all apps |
| M365 Business Standard + Copilot Business | $30.50 | Desktop Office apps + Copilot |
| M365 Business Premium + Copilot Business | $43.00 | Desktop apps + advanced security + Copilot |
| Copilot Business add-on alone | **$21/user/month** | Copilot only; requires existing eligible M365 plan |

*Pricing in USD, billed annually. A promotional discount of up to 35% applies on select bundles between July–September 2026.*

**For 20 users at $21/month add-on:** ~$420/month / ~$5,040/year (on top of existing M365 subscription cost).

<a href="https://www.microsoft.com/en-us/microsoft-365-copilot/pricing" target="_blank">Microsoft 365 Copilot Pricing</a>  
<a href="https://www.microsoft.com/en-us/microsoft-365/business/with-copilot-plans-and-pricing" target="_blank">Microsoft 365 Business Plans with Copilot</a>

### Deployment

Enabled by an IT administrator in the Microsoft 365 admin centre. Licences are assigned per user. Once assigned, Copilot appears inside the desktop and web versions of Word, Excel, Outlook, and PowerPoint automatically — no additional software installation is required.

---

## 2. OpenAI ChatGPT Business

### What It Is

ChatGPT Business (formerly ChatGPT Team, renamed August 2025) is OpenAI's commercial plan for teams and small businesses. It is a standalone AI assistant — browser-based and available as a desktop app — and does **not** embed inside Microsoft Office applications. Users interact with it separately to their work, but it is extremely capable at drafting, summarising, analysing, and researching when given the right context.

For an M365 office environment, staff typically use ChatGPT Business alongside Office: draft in ChatGPT, paste into Word; ask it to analyse data pasted from Excel; have it draft email replies which are then sent from Outlook.

<a href="https://openai.com/business/chatgpt-pricing/" target="_blank">ChatGPT Business Pricing</a>

### What It Does — Practical Office Tasks

| Task Type | What ChatGPT Business Can Do |
|---|---|
| **Word / document drafting** | Generate first drafts, rewrite, summarise, change tone — paste output into Word |
| **Excel / data analysis** | Analyse data pasted or uploaded as CSV/Excel; write formulas; explain results in plain English; Python-based data analysis via Code Interpreter |
| **Outlook / emails** | Draft emails, replies, and newsletters; summarise long email threads pasted in |
| **PowerPoint / presentations** | Generate slide outlines and talking points; write speaker notes; limited direct PPT creation |
| **Research and reasoning** | Deep Research feature for comprehensive web research; multi-step reasoning tasks |
| **Automation / agents** | ChatGPT Agent can browse the web, run code, and complete multi-step tasks on your behalf |
| **Company Knowledge** | Upload internal documents to a shared company knowledge base; all staff can query it |

ChatGPT does not have native access to your M365 tenant data (emails, files, calendar) unless you explicitly upload or paste the content.

### Plans and Pricing

| Plan | Per User / Month | Minimum Seats | Key Business Features |
|---|---|---|---|
| ChatGPT Business (monthly) | $25 | 2 | All models, deep research, agents, company knowledge, no training on data |
| ChatGPT Business (annual) | $20 | 2 | Same as above, lower price |
| ChatGPT Enterprise | Custom (quote) | Custom | Dedicated infrastructure, custom data residency, extended context, SSO, advanced admin |

*ChatGPT Business now supports two seat types: standard seats (fixed monthly cost) and Codex seats (usage-based, for software/code tasks).*

**For 20 users at $20/month (annual):** $400/month / $4,800/year.

<a href="https://openai.com/pricing" target="_blank">ChatGPT Plans and Pricing</a>  
<a href="https://help.openai.com/en/articles/8792828-what-is-chatgpt-business" target="_blank">What is ChatGPT Business?</a>

### Deployment

Admin creates a Business workspace at chatgpt.com and invites users by email. Users access via browser or the ChatGPT desktop app (Windows/Mac). No IT infrastructure changes or Microsoft 365 admin changes required.

---

## 3. Anthropic Claude Team

### What It Is

Claude Team is Anthropic's commercial plan for businesses. Claude is widely regarded as the strongest AI model for tasks requiring sophisticated reasoning, nuanced writing, and processing of long, complex documents — the context window supports documents of 200,000 tokens (roughly 150,000 words, or several hundred pages). Like ChatGPT Business, it is a standalone tool with no native Microsoft Office integration.

Claude is particularly well-suited to tasks like: analysing lengthy contracts or reports, producing polished professional writing, working through ambiguous or sensitive business problems, and coding or data analysis tasks.

<a href="https://www.anthropic.com/pricing" target="_blank">Claude Plans and Pricing</a>

### What It Does — Practical Office Tasks

| Task Type | What Claude Team Can Do |
|---|---|
| **Word / document drafting** | Long-form drafting, rewriting, restructuring; paste output into Word; can read uploaded Word/PDF files directly |
| **Excel / data analysis** | Analyse data from uploaded CSV/Excel files; write and explain complex formulas; produce data summaries |
| **Outlook / emails** | Draft, rewrite, and summarise email correspondence pasted in |
| **PowerPoint / presentations** | Generate slide content, outlines, and speaker notes; no direct .pptx creation |
| **Long document review** | Read and summarise entire contracts, reports, or policy documents in one pass — a key differentiator vs other tools |
| **Complex reasoning** | Nuanced analysis of business problems, financial data, regulatory questions |
| **Projects** | Organise ongoing work into Projects with shared context across your team |
| **Claude Code** | Coding assistant now included in Team plan for technical staff |

### Plans and Pricing

| Plan | Per User / Month | Minimum Seats | Key Features |
|---|---|---|---|
| Claude Team (Standard) — annual | **$20** | 5 | 1.25× usage vs Pro, admin controls, Projects, Claude Code |
| Claude Team (Standard) — monthly | $25 | 5 | Same as above, no annual commitment |
| Claude Team (Premium seats) — annual | $100 | 5 | 6.25× usage vs Pro, dual usage limits for Sonnet models |
| Claude Enterprise | Custom (quote) | Custom | SSO, SCIM, audit logs, custom retention, compliance API, domain capture |

**For 20 users at $20/month (annual):** $400/month / $4,800/year.

<a href="https://support.anthropic.com/en/articles/9266767-what-is-the-claude-team-plan" target="_blank">What is the Claude Team plan?</a>  
<a href="https://www.anthropic.com/product/enterprise" target="_blank">Claude Enterprise</a>

### Deployment

Admin creates a Claude Team workspace at claude.ai and invites users by email. Access is via browser or the Claude desktop app (Windows/Mac). No IT infrastructure changes required. Enterprise customers can configure SSO and SCIM through the admin panel.

---

## 4. Google Gemini for Google Workspace

### What It Is

Google Gemini is embedded in Google Workspace (Gmail, Docs, Sheets, Slides, Meet, Drive). It is the natural choice for organisations already using Google's productivity suite and is comparable in integration depth to Microsoft 365 Copilot for Google users.

**For a team already using Microsoft 365**, Gemini for Workspace is not directly relevant — it does not integrate with Word, Excel, Outlook, or PowerPoint. It is included here as a reference point for organisations considering a platform migration, or for comparison of features and pricing.

<a href="https://workspace.google.com/pricing/" target="_blank">Google Workspace Pricing</a>

### Plans and Pricing (Google Workspace with Gemini)

| Plan | Per User / Month | Gemini Integration |
|---|---|---|
| Business Starter | $7.00 | Gemini in Gmail |
| Business Standard | $14.00 | Gemini in Gmail, Docs, Meet, Sheets, Slides, NotebookLM |
| Business Plus | $22.00 (from Sep 2026) | All Standard features + appointment booking, eSignature |

*Note: Google Workspace plans replace Microsoft 365 entirely — users would use Google Docs/Sheets/Slides instead of Word/Excel/PowerPoint.*

**Verdict for M365 teams:** Not applicable unless you are prepared to migrate off Microsoft 365 entirely. The per-user cost is lower, but the migration cost and disruption for 20 staff must be factored in.

---

## 5. Supplementary Tools

These tools complement the primary AI assistants for specific tasks.

### Grammarly Business

A writing assistant with a native Microsoft Office add-in (available for Word and Outlook). It does not generate content from scratch in the way that the tools above do, but provides real-time grammar, tone, clarity, and style suggestions as staff type within Office applications.

- Works directly inside Word and Outlook without leaving the application
- Useful for staff who write a lot of external correspondence or reports
- Business plan includes brand tone settings, style guides, and admin controls
- Pricing: approximately $15/user/month (annual), minimum 3 seats

<a href="https://www.grammarly.com/business" target="_blank">Grammarly Business</a>

### Notion AI

A knowledge management and note-taking platform with AI built in. Useful for building a shared internal wiki, meeting notes, and project documentation, with AI capable of summarising, drafting, and answering questions about your internal content. Does not integrate with Microsoft 365.

---

## Commercial Privacy Controls

This section assesses the privacy posture of each provider specifically for **business/commercial plans** — the same framework used in the companion [consumer privacy comparison](comparisons.md).

### The Core Finding

Under commercial plans, **all four providers exclude customer data from AI model training by default**. This is a fundamental difference from consumer plans and is the most important privacy fact for a business user to know.

---

### Microsoft 365 Copilot

**Data used for training:** No. Prompts, responses, and data accessed through Microsoft Graph are not used to train foundation models, and are not shared with advertisers.

**Data retention:** Conversation data is governed by your Microsoft 365 tenant retention policies. Admins can configure retention through Microsoft Purview compliance tools. Data subject to the same retention rules as your email in Exchange and files in SharePoint.

**Security and compliance:** Encrypted at rest and in transit (AES-256, TLS). Supports GDPR, EU Data Boundary, ISO/IEC 27018, SOC 2. Data isolation between tenants. Microsoft Data Protection Addendum available.

**Admin controls:** Full admin control via Microsoft 365 admin centre and Microsoft Purview. Audit logs, eDiscovery, data loss prevention (DLP) policies, and sensitivity labels all apply to Copilot interactions. Role-based access control.

**Data residency:** Supported via EU Data Boundary and Microsoft's data residency commitments for commercial tenants.

<a href="https://learn.microsoft.com/en-us/microsoft-365/copilot/microsoft-365-copilot-privacy" target="_blank">Data, Privacy, and Security for Microsoft 365 Copilot</a>  
<a href="https://learn.microsoft.com/en-us/microsoft-365/copilot/enterprise-data-protection" target="_blank">Enterprise Data Protection in M365 Copilot</a>

---

### OpenAI ChatGPT Business

**Data used for training:** No. OpenAI does not use Business, Enterprise, or Edu customer inputs or outputs for model training by default.

**Data retention:** Workspace admins control retention. Deleted conversations are removed from OpenAI systems within 30 days. API customers can configure zero data retention. Custom data retention policies are available for qualifying organisations.

**Security and compliance:** AES-256 encryption at rest; TLS 1.2+ in transit. SOC 2 Type 2 certified. GDPR and CCPA compliance. Support for HIPAA available on Enterprise.

**Admin controls:** Workspace admin dashboard for user management, conversation oversight, and data controls. Business plan includes custom retention policies.

**Data residency:** OpenAI expanded data residency access to business customers worldwide in 2026. Options available for EU, UK, Japan and other regions.

<a href="https://openai.com/enterprise-privacy/" target="_blank">Enterprise Privacy at OpenAI</a>  
<a href="https://openai.com/business-data/" target="_blank">OpenAI Business Data Privacy</a>  
<a href="https://help.openai.com/en/articles/8798634-managing-data-sharing-and-privacy-in-chatgpt-business" target="_blank">Managing Data, Sharing, and Privacy in ChatGPT Business</a>

---

### Anthropic Claude Team / Enterprise

**Data used for training:** No. Customer data from Team and Enterprise plans is not used to train Anthropic models. This is explicitly excluded under commercial terms.

**Data retention:** Minimum 30 days. Enterprise customers can configure custom data retention periods through account settings. Selective deletion is available — admins can request deletion of specific data via the compliance API.

**Security and compliance:** SSO (SAML), SCIM provisioning, role-based permissions, audit logs. Compliance API provides real-time programmatic access to usage data for integration into compliance dashboards. Anthropic Trust Center publishes certifications, sub-processors, and data handling policies.

**Admin controls:** Full workspace admin controls including user management, spend caps, usage analytics, and domain capture (Enterprise). Granular role-based permissioning.

**Data residency:** Not explicitly detailed in published materials for Team plan. Enterprise customers should confirm with Anthropic for specific requirements.

<a href="https://www.anthropic.com/product/enterprise" target="_blank">Claude Enterprise</a>  
<a href="https://privacy.anthropic.com/en/articles/10440198-custom-data-retention-controls-for-claude-enterprise" target="_blank">Custom Data Retention Controls for Claude Enterprise</a>  
<a href="https://trust.anthropic.com" target="_blank">Anthropic Trust Center</a>

---

### Google Gemini for Workspace

**Data used for training:** No. Workspace data (including Gemini interactions) is not used to train Google AI models by default for Workspace customers.

**Data retention:** Admins set retention via Google Vault and Workspace admin settings. Gemini conversation activity can be configured with auto-delete periods of 3, 18, or 36 months, or disabled entirely. Google Vault supports eDiscovery and legal holds over Gemini data.

**Security and compliance:** Google Workspace is ISO 27001, SOC 1/2/3, and GDPR compliant. Data encrypted at rest and in transit. Tenant isolation enforced.

**Admin controls:** Full Google Workspace admin console controls. Admins can enable or disable Gemini features per organisational unit. Audit logs and Vault coverage for Gemini interactions.

**Data residency:** Data residency controls available via the Google Workspace data regions feature.

<a href="https://support.google.com/a/answer/15706919" target="_blank">Generative AI in Google Workspace Privacy Hub</a>

---

### Commercial Privacy Comparison Table

| | **M365 Copilot** | **ChatGPT Business** | **Claude Team** | **Gemini Workspace** |
|---|---|---|---|---|
| **Training on business data** | No | No | No | No |
| **Default retention** | Tenant policy (admin-set) | Admin-configured | 30 days minimum | Admin-configured |
| **Custom retention** | Yes (Purview) | Yes | Enterprise only | Yes (Vault) |
| **Zero data retention** | No | API customers only | No (30-day min) | No |
| **Encryption at rest** | AES-256 | AES-256 | Yes | Yes |
| **Encryption in transit** | TLS | TLS 1.2+ | Yes | Yes |
| **SOC 2 Type 2** | Yes | Yes | Not published | Yes |
| **GDPR compliance** | Yes | Yes | Yes | Yes |
| **EU Data Boundary / residency** | Yes | Yes (2026) | Not confirmed (Team) | Yes |
| **SSO** | Yes (via M365) | Enterprise only | Team + Enterprise | Yes (Workspace) |
| **Audit logs** | Yes (Purview) | Limited (Business) | Enterprise only | Yes (Vault) |
| **Admin data controls** | Comprehensive | Good | Good (Enterprise: full) | Comprehensive |
| **eDiscovery / legal hold** | Yes (Purview) | Not stated | Compliance API | Yes (Vault) |

---

## Recommendation for a 20-Person M365 SME

### Primary recommendation: Microsoft 365 Copilot Business

For a team already paying for Microsoft 365 and using Word, Excel, Outlook, and PowerPoint daily, Microsoft 365 Copilot Business is the most impactful option:

- It works **inside the apps your staff already use** — no new habits to form
- It has access to your existing business data (emails, files, calendar) without your staff needing to copy-paste anything
- Privacy and data controls are managed through your existing Microsoft 365 admin infrastructure — no new vendor relationship to establish
- Deployment is straightforward: assign licences in the admin centre; Copilot appears in the apps automatically

**Cost for 20 users:** ~$5,040/year (add-on only, on top of existing M365 subscription).

### When to add ChatGPT Business or Claude Team

If your team has heavy research, complex writing, or detailed analysis needs that go beyond what Copilot provides, ChatGPT Business or Claude Team are strong complements — or standalone alternatives for staff who are comfortable working outside the Office apps.

- **ChatGPT Business** is better if your staff will benefit from web research (Deep Research feature), agent-based automation, or a shared Company Knowledge base. At $20/user/year it is cost-competitive with M365 Copilot.
- **Claude Team** is better if your work involves processing long documents (contracts, reports, regulatory submissions), producing high-quality professional writing, or complex reasoning. At $20/user/month (annual) it is directly price-competitive with ChatGPT Business.

### What to avoid

- **Do not rely on Copilot Chat (the free tier)** and assume it is equivalent to the paid product — it is not. It has no access to your business data and does not work inside Office applications.
- **Do not use personal/consumer plans** (free Claude, ChatGPT Free/Plus, personal Gemini) for business data — these are subject to different training and retention policies. Use commercial plans only.
- **Google Gemini for Workspace** is only relevant if you are prepared to migrate from Microsoft 365 to Google Workspace entirely — a significant undertaking for 20 staff. It is not a drop-in addition for M365 users.

### Indicative Cost Comparison (20 users, annual billing)

| Option | Annual Cost (20 users) | Office Integration | Training on Data |
|---|---|---|---|
| M365 Copilot Business (add-on) | ~$5,040 | Native (Word/Excel/Outlook/PPT/Teams) | No |
| ChatGPT Business | $4,800 | None (standalone) | No |
| Claude Team | $4,800 | None (standalone) | No |
| Both M365 Copilot + ChatGPT Business | ~$9,840 | Copilot native; ChatGPT standalone | No |
| Both M365 Copilot + Claude Team | ~$9,840 | Copilot native; Claude standalone | No |

---

## Sources

<a href="https://www.microsoft.com/en-us/microsoft-365-copilot" target="_blank">Microsoft 365 Copilot Overview</a>  
<a href="https://www.microsoft.com/en-us/microsoft-365-copilot/pricing" target="_blank">Microsoft 365 Copilot Pricing</a>  
<a href="https://www.microsoft.com/en-us/microsoft-365/business/with-copilot-plans-and-pricing" target="_blank">Microsoft 365 Business Plans with Copilot</a>  
<a href="https://www.microsoft.com/en-us/microsoft-365/blog/2025/12/02/microsoft-365-copilot-business-the-future-of-work-for-small-businesses/" target="_blank">Microsoft 365 Copilot Business for Small Businesses</a>  
<a href="https://support.microsoft.com/en-us/topic/what-s-the-difference-between-microsoft-copilot-free-and-copilot-in-microsoft-365-cfff4791-694a-4d90-9c9c-1eb3fb28e842" target="_blank">Microsoft: Copilot (free) vs Microsoft 365 Copilot</a>  
<a href="https://learn.microsoft.com/en-us/copilot/overview" target="_blank">Overview of Microsoft 365 Copilot Chat</a>  
<a href="https://learn.microsoft.com/en-us/microsoft-365/copilot/microsoft-365-copilot-privacy" target="_blank">Data, Privacy, and Security for Microsoft 365 Copilot</a>  
<a href="https://learn.microsoft.com/en-us/microsoft-365/copilot/enterprise-data-protection" target="_blank">Enterprise Data Protection in Microsoft 365 Copilot</a>  
<a href="https://openai.com/pricing" target="_blank">ChatGPT Plans and Pricing</a>  
<a href="https://openai.com/business/chatgpt-pricing/" target="_blank">ChatGPT Business Pricing</a>  
<a href="https://help.openai.com/en/articles/8792828-what-is-chatgpt-business" target="_blank">What is ChatGPT Business?</a>  
<a href="https://openai.com/enterprise-privacy/" target="_blank">Enterprise Privacy at OpenAI</a>  
<a href="https://openai.com/business-data/" target="_blank">OpenAI Business Data Privacy</a>  
<a href="https://help.openai.com/en/articles/8798634-managing-data-sharing-and-privacy-in-chatgpt-business" target="_blank">Managing Data and Privacy in ChatGPT Business</a>  
<a href="https://www.anthropic.com/pricing" target="_blank">Claude Plans and Pricing</a>  
<a href="https://support.anthropic.com/en/articles/9266767-what-is-the-claude-team-plan" target="_blank">What is the Claude Team Plan?</a>  
<a href="https://www.anthropic.com/product/enterprise" target="_blank">Claude Enterprise</a>  
<a href="https://privacy.anthropic.com/en/articles/10440198-custom-data-retention-controls-for-claude-enterprise" target="_blank">Custom Data Retention Controls for Claude Enterprise</a>  
<a href="https://trust.anthropic.com" target="_blank">Anthropic Trust Center</a>  
<a href="https://workspace.google.com/pricing/" target="_blank">Google Workspace Pricing</a>  
<a href="https://support.google.com/a/answer/15706919" target="_blank">Generative AI in Google Workspace Privacy Hub</a>  
<a href="https://www.grammarly.com/business" target="_blank">Grammarly Business</a>
