# Changelog

## [Unreleased] - 2026-05-15

### Added
- `PortlandFuelLeads.sln` — Visual Studio solution file with three projects
- `CompaniesHouseSearch.Core` — shared class library (net10.0) with all models, services, and helpers
  - `CompaniesHouseClient` — full API client covering profile, officers, PSC, insolvency, filing history, document PDF streaming
  - `LeadScorer` — product match scoring by SIC code and company characteristics
  - `CsvExporter` — typed CSV export using CsvHelper
  - `RateLimiter` — 600 req/5 min rate limiter for Companies House API
  - `SicCodeExpander` — UK SIC 2007 prefix expansion (e.g. "49" → all transport sub-codes)
  - Models: `CompanyProfile`, `OfficerItem`, `PscItem`, `InsolvencyInfo`, `FilingHistoryResponse`
- `CompaniesHouseSearch` — console app (net10.0) for batch SIC-code lead generation
  - CLI arguments: `--sic`, `--region`, `--postcode`, `--max`, `--output`, `--officers`
  - Default: SIC 49, region Yorkshire, max 1000, timestamped CSV output
  - Client-side postcode prefix filtering
- `CompaniesHouseWeb` — ASP.NET Razor Pages web app (net10.0) for single-company lookups
  - Index page: company number search form
  - Company page: full profile, active officers, recently resigned officers (18 months), PSC, insolvency banner, latest accounts with PDF viewer
  - PDF proxy handler streams accounts PDF directly to browser
  - Portland Fuel branded Bootstrap 5 UI (navy #002147 / white)
  - IIS Express profile for Visual Studio local testing
- `LeadGen.md` — AI lead generation strategy for Portland Fuel (5 products, 2 pipelines, GDPR/PECR guidance, n8n enrichment workflow)
- `CompaniesHouse.md` — Console app usage documentation with SIC code reference and postcode prefix table

### Changed
- Nothing (first version)
