# Project Status

**Last Updated**: 2026-06-17

## Current State
The Companies House lead generation toolkit is complete and tested. Two new AI reference documents have been added to the Leads directory: a private AI hosting guide (`RemotePrivateAI.md`) and an enterprise AI privacy guide (`AIEnterprisePrivacy.md`). The web app is ready for Nginx deployment when required.

## Active Work Areas
- **Console app**: Ready to use. Run `dotnet run -- --sic 49 --region York --max 1000` for Yorkshire transport leads.
- **Web app**: Tested locally via IIS Express. Ready for Nginx deployment.
- **AI reference docs**: Two new documents available for decision-making on AI tool adoption.

## Recently Completed
- `RemotePrivateAI.md` — private AI hosting options (on-premise, dedicated rental, cloud GPU, co-lo, specialist platforms) with GBP costings and pros/cons
- `AIEnterprisePrivacy.md` — enterprise privacy guide for 6 AI providers (ChatGPT, Copilot, Claude, Gemini, Perplexity, Mistral) covering training policies, data residency, deletion, compliance, and 10–20 user pricing
- Exchange rate correction in `RemotePrivateAI.md` (£1=$1.27 → £1=$1.35)

## Blocked/Pending
- Nginx subdomain deployment (user to action when ready)
- M365 SSO authentication (explicitly deferred to a future session)
- Decision on which AI provider to adopt internally (informed by `AIEnterprisePrivacy.md`)

## Next Priorities
1. Run first console app lead batch — Yorkshire transport/haulage companies (SIC 49)
2. Deploy web app to Nginx subdomain
3. Evaluate AI tool adoption using `AIEnterprisePrivacy.md`
4. Consider private AI server trial (RunPod or Hetzner) per `RemotePrivateAI.md`
5. Add M365 authentication to web app (longer term)

## Key Files & Structure
```
Leads/
├── PortlandFuelLeads.sln           — Open this in Visual Studio
├── LeadGen.md                      — Lead generation strategy document
├── CompaniesHouse.md               — Console app usage documentation
├── RemotePrivateAI.md              — Private AI hosting options with costs
├── AIEnterprisePrivacy.md          — AI enterprise privacy & pricing guide (6 providers)
├── CompaniesHouseSearch/           — Console app (batch SIC search → CSV)
│   ├── appsettings.json            — Add API key here
│   └── Program.cs                  — CLI: --sic, --region, --postcode, --max, --output, --officers
├── CompaniesHouseSearch.Core/      — Shared library (models, client, scorer)
│   ├── Services/CompaniesHouseClient.cs  — All API calls
│   ├── Services/LeadScorer.cs            — Product match scoring logic
│   └── Helpers/SicCodeExpander.cs        — SIC prefix expansion
└── CompaniesHouseWeb/              — Razor Pages web app (single company lookup)
    ├── appsettings.json            — Add API key here
    └── Pages/Company.cshtml        — Main results page
```

## API Notes
- Base URL: `https://api.company-information.service.gov.uk`
- Document API: `https://document-api.company-information.service.gov.uk`
- Auth: Basic auth — API key as username, empty password
- Rate limit: 600 requests / 5 minutes (enforced by RateLimiter)
- Location parameter: town/city name only (e.g. "York") — county names do not work
- Company numbers: always 8 characters, zero-padded for numeric ones

## AI Provider Summary (from AIEnterprisePrivacy.md)
| Provider | Entry Business Plan | ~Price/user/mo | Data residency |
|----------|-------------------|----------------|---------------|
| ChatGPT Team | Team | £22 | US only (Team) |
| Microsoft Copilot | M365 Copilot add-on | £25 + M365 base | UK/EU available |
| Claude Team | Team | £22 | US only |
| Gemini | Workspace Business Standard | £10 (AI included) | EU/UK selectable |
| Perplexity Enterprise | Enterprise Pro | ~£30–37 (est.) | US only |
| Mistral le Chat | Pro | £13 | EU default |
