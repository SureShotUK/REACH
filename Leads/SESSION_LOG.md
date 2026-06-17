## Session 2026-06-17

### Summary
Created two new AI reference documents for the Leads directory: `RemotePrivateAI.md` covering options for hosting a private AI server accessible across the business (with costs and pros/cons), and `AIEnterprisePrivacy.md` covering data privacy controls, training opt-outs, deletion options, and entry-level enterprise pricing for ChatGPT, Microsoft Copilot, Claude, Gemini, Perplexity, and Mistral. Exchange rate in RemotePrivateAI.md was also corrected from £1=$1.27 to £1=$1.35.

### Work Completed
- Created `RemotePrivateAI.md` — 5 hosting options (on-premise, dedicated GPU rental, cloud GPU, colocation, AI-specialist platforms) with rough GBP costs, pros/cons, and networking guidance (Tailscale)
- Created `AIEnterprisePrivacy.md` — enterprise privacy guide for 6 AI providers covering training policies, deletion controls, data residency, compliance certs, and pricing for 10–20 users
- Corrected USD→GBP exchange rate across all conversions in `RemotePrivateAI.md` (£1=$1.27 → £1=$1.35)
- All external links verified (HTTP 200) before inclusion in both documents

### Files Created/Modified
- `Leads/RemotePrivateAI.md` — new; private AI hosting options guide with GBP costings
- `Leads/AIEnterprisePrivacy.md` — new; AI enterprise privacy and pricing reference

### Key Decisions
- `RemotePrivateAI.md` placed in Leads (not Briefings) — it's an internal IT/strategy reference, not a client-facing briefing
- `AIEnterprisePrivacy.md` scoped to general privacy controls only (no UK GDPR legal framing) per user preference; internal use only
- Six providers covered: ChatGPT, Microsoft Copilot, Claude, Gemini, Perplexity, Mistral — Perplexity and Mistral added at user request
- Exchange rate set to £1 = $1.35 / £1 = €1.18 throughout

### Key Findings (AIEnterprisePrivacy.md)
- All six providers guarantee no training on data at paid business tiers
- Main differentiator is data residency: Microsoft, Google, Mistral offer EU/UK; Claude and ChatGPT Team are US-only
- Best value: Google Workspace Business Standard (£10.30/user) includes Gemini
- Mistral is the only EU-native provider; offers on-premise deployment
- Microsoft Copilot is best if already on M365 (£25 add-on, UK/EU data residency)

### Reference Documents
- `Leads/RemotePrivateAI.md` — private AI hosting options
- `Leads/AIEnterprisePrivacy.md` — AI enterprise privacy guide

### Next Actions
- [ ] Deploy CompaniesHouseWeb to Nginx subdomain (carried over from previous session)
- [ ] Add M365 authentication to web app (deferred)
- [ ] Evaluate which AI provider to adopt for Portland Fuel internal use based on `AIEnterprisePrivacy.md`
- [ ] Consider trialling RunPod or Hetzner GPU server for private AI per `RemotePrivateAI.md`

---

## Session 2026-05-15

### Summary
Built a complete Companies House lead generation toolkit from scratch: a C# console app for batch SIC-code lead searches with CSV export, and an ASP.NET Razor Pages web app for single-company deep-dive lookups. Both projects share a common class library. The web app was successfully tested against a live Companies House API key.

### Work Completed
- Created `LeadGen.md` — Portland Fuel AI lead generation strategy covering fuel cards, bulk delivery, bunker networks, hedging, and consultancy
- Created `CompaniesHouseSearch` console app — batch search by SIC code and region, scored results exported to CSV
- Created `CompaniesHouseSearch.Core` shared class library — all models, services, and helpers shared between console and web apps
- Created `CompaniesHouseWeb` ASP.NET Razor Pages web app — single company lookup with full profile, officers, PSC, insolvency, and PDF accounts viewer
- Created `PortlandFuelLeads.sln` — solution file tying all three projects together
- Created `CompaniesHouse.md` — documentation for the console application
- Added SIC code prefix expander (e.g. "49" expands to all transport sub-codes)
- Configured IIS Express launch profile for Visual Studio Community 2026 local testing
- Debugged and fixed HTTPS certificate error (set sslPort to 0)
- Debugged API key issue caused by STT software appending extra text

### Files Created
- `LeadGen.md` — Lead generation strategy document
- `CompaniesHouse.md` — Console app documentation
- `PortlandFuelLeads.sln` — Visual Studio solution file
- `CompaniesHouseSearch/CompaniesHouseSearch.csproj` — Console app project
- `CompaniesHouseSearch/Program.cs` — Console app entry point with CLI argument parsing
- `CompaniesHouseSearch/appsettings.json` — Console app configuration
- `CompaniesHouseSearch.Core/CompaniesHouseSearch.Core.csproj` — Shared library project
- `CompaniesHouseSearch.Core/Models/CompanyItem.cs`
- `CompaniesHouseSearch.Core/Models/CompanySearchResponse.cs`
- `CompaniesHouseSearch.Core/Models/Officer.cs`
- `CompaniesHouseSearch.Core/Models/LeadRecord.cs`
- `CompaniesHouseSearch.Core/Models/CompanyProfile.cs`
- `CompaniesHouseSearch.Core/Models/PscItem.cs`
- `CompaniesHouseSearch.Core/Models/InsolvencyInfo.cs`
- `CompaniesHouseSearch.Core/Models/FilingHistoryResponse.cs`
- `CompaniesHouseSearch.Core/Services/CompaniesHouseClient.cs` — API client with all endpoints
- `CompaniesHouseSearch.Core/Services/LeadScorer.cs`
- `CompaniesHouseSearch.Core/Services/CsvExporter.cs`
- `CompaniesHouseSearch.Core/Helpers/RateLimiter.cs`
- `CompaniesHouseSearch.Core/Helpers/SicCodeExpander.cs`
- `CompaniesHouseWeb/CompaniesHouseWeb.csproj` — Web app project
- `CompaniesHouseWeb/Program.cs` — Web app startup and DI registration
- `CompaniesHouseWeb/appsettings.json` — Web app configuration
- `CompaniesHouseWeb/Properties/launchSettings.json` — IIS Express profile
- `CompaniesHouseWeb/Pages/_ViewImports.cshtml`
- `CompaniesHouseWeb/Pages/_ViewStart.cshtml`
- `CompaniesHouseWeb/Pages/Shared/_Layout.cshtml` — Portland Fuel branded Bootstrap 5 layout
- `CompaniesHouseWeb/Pages/Index.cshtml` + `Index.cshtml.cs` — Company number search form
- `CompaniesHouseWeb/Pages/Company.cshtml` + `Company.cshtml.cs` — Full company results page

### Git Commits
- No commits yet — all files are new and untracked (committed at end of this session)

### Key Decisions
- Three-project solution: Core library avoids code duplication between console and web apps
- `RootNamespace` set to `CompaniesHouseSearch` in Core `.csproj` so console app namespaces remain unchanged
- Location parameter uses town/city names (e.g. "York"), not county/region names — API matches against locality field only
- Postcode filtering is client-side post-filter only (API does not support it server-side)
- Two `HttpClient` instances in `CompaniesHouseClient`: one for main API, one for document API (different base URL)
- PDF served via Razor Pages handler (`OnGetPdfAsync`) rather than a separate controller
- `sslPort: 0` in launchSettings disables IIS Express HTTPS for local testing simplicity
- Company numbers padded to 8 digits with `PadLeft(8, '0')` to handle user input
- 401 (invalid API key) silently presents as "company not found" — acceptable for internal tool

### Next Actions
- [ ] Register for Companies House API key at developer.companieshouse.gov.uk (done — user has key)
- [ ] Run console app against Yorkshire transport SIC codes (49) to generate first lead batch
- [ ] Deploy web app to Nginx subdomain (`dotnet publish` + reverse proxy config)
- [ ] Add Microsoft 365 SSO authentication to web app

---
