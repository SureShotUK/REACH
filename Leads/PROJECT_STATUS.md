# Project Status

**Last Updated**: 2026-05-15

## Current State
Both applications are fully built and tested against the live Companies House API. The console app generates batch CSV leads by SIC code and region; the web app provides a single-company deep-dive with officers, PSC, insolvency status, and PDF accounts viewer. Solution opens in Visual Studio Community 2026 via `PortlandFuelLeads.sln`.

## Active Work Areas
- **Console app**: Ready to use. Run `dotnet run -- --sic 49 --region York --max 1000` for Yorkshire transport leads.
- **Web app**: Tested locally via IIS Express. Ready for Nginx deployment.

## Recently Completed
- Full three-project solution structure (Core library + console app + web app)
- Companies House API integration — profile, officers, PSC, insolvency, filing history, PDF streaming
- SIC code prefix expansion (type "49" to get all transport sub-codes)
- Portland Fuel product match scoring
- Bootstrap 5 branded UI with navy/white Portland Fuel theme
- IIS Express local testing configuration

## Blocked/Pending
- Nginx subdomain deployment (user to action when ready)
- M365 SSO authentication (explicitly deferred to a future session)

## Next Priorities
1. Run first console app lead batch — Yorkshire transport/haulage companies (SIC 49)
2. Deploy web app to Nginx subdomain
3. Add M365 authentication to web app
4. Germany/Canada expansion (longer term, once UK pipeline proven)

## Key Files & Structure
```
Leads/
├── PortlandFuelLeads.sln           — Open this in Visual Studio
├── LeadGen.md                      — Lead generation strategy document
├── CompaniesHouse.md               — Console app usage documentation
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
