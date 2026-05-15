<img src="../Portland Long.png" alt="Portland Fuel" style="width:40%; height:auto;" align="right">

# Companies House Lead Generation Tool

*Portland Fuel — Build Guide & Reference (v1.0, May 2026)*

---

## Overview

This document describes the `CompaniesHouseSearch` C# console application — a tool that queries the free <a href="https://developer.company-information.service.gov.uk/overview/" target="_blank">Companies House Public Data API</a> to find UK companies matching Portland Fuel's target sectors, scores them against our product criteria, and exports the results to a CSV file ready for the sales team.

The tool is the foundation of Phase 1 in the AI Lead Generation Strategy (see `LeadGen.md`).

---

## 1. Getting a Companies House API Key

The API is free. Registration takes approximately five minutes.

1. Go to <a href="https://developer.companieshouse.gov.uk" target="_blank">developer.companieshouse.gov.uk</a>
2. Click **Register** and create a free account
3. Once logged in, click **Create an application**
4. Give it a name (e.g., `Portland Fuel Lead Generation`)
5. Select the **Live** environment
6. Click **Create application** — your API key is displayed immediately
7. Copy the key and paste it into `appsettings.json` (see Section 4)

**Rate limit**: 600 requests per 5 minutes. The application enforces this automatically.

---

## 2. Prerequisites

- <a href="https://dotnet.microsoft.com/en-us/download/dotnet/10.0" target="_blank">.NET 10 SDK</a> installed
- Visual Studio Code with the C# Dev Kit extension
- A Companies House API key (see Section 1)

Check your .NET version:

```bash
dotnet --version
```

Should return `8.x.x` or higher.

---

## 3. Building the Application

From a terminal in the `Leads/` directory:

```bash
# Create the project
dotnet new console -n CompaniesHouseSearch
cd CompaniesHouseSearch

# Add required NuGet packages
dotnet add package CsvHelper
dotnet add package System.CommandLine
dotnet add package Microsoft.Extensions.Configuration.Json

# Build to verify everything compiles
dotnet build
```

---

## 4. Configuration

Edit `appsettings.json` in the project root and paste your API key:

```json
{
  "CompaniesHouse": {
    "ApiKey": "YOUR_API_KEY_HERE",
    "BaseUrl": "https://api.company-information.service.gov.uk"
  }
}
```

**Important**: Do not commit `appsettings.json` to a public repository — it contains your API key. Add it to `.gitignore` if using source control.

---

## 5. Project Structure

```
CompaniesHouseSearch/
├── CompaniesHouseSearch.csproj
├── appsettings.json              ← API key (keep private)
├── Program.cs                    ← Entry point: argument parsing and orchestration
├── Models/
│   ├── CompanySearchResponse.cs  ← JSON envelope from /advanced-search
│   ├── CompanyItem.cs            ← Individual company record
│   └── Officer.cs                ← Director record from /officers endpoint
├── Services/
│   ├── CompaniesHouseClient.cs   ← All API calls, authentication, pagination
│   ├── LeadScorer.cs             ← Scores companies against Portland Fuel products
│   └── CsvExporter.cs           ← Writes results to CSV
└── Helpers/
    └── RateLimiter.cs            ← Limits requests to ~2/second
```

---

## 6. Running the Application

Run from inside the `CompaniesHouseSearch/` directory:

```bash
dotnet run -- [arguments]
```

### Arguments

| Argument | Required | Example | Description |
|---|---|---|---|
| `--sic` | Yes | `49410,49310` | SIC codes to search, comma-separated |
| `--region` | No | `"Norwich"` | Town or city name — matches the locality field in the registered address |
| `--postcode` | No | `"NR"` | Postcode prefix — filters results after retrieval; more reliable than `--region` for broad areas |
| `--max` | No | `200` | Maximum results to fetch before postcode filtering (default: 200) |
| `--output` | No | `leads.csv` | Output file path (default: `leads_YYYY-MM-DD.csv`) |
| `--officers` | No | *(flag)* | Fetch director names (slower — one extra API call per company) |

### Geographic Filtering — Important Note

The Companies House API does **not** have a fixed list of regions. The `--region` argument matches against the **locality** field in the registered address — this is the town or city name (e.g., `"Norwich"`, `"Manchester"`). Broad terms like `"East Anglia"` or `"South West"` are not reliable.

**The recommended approach for targeting a broad geographic area is `--postcode`**, which filters results by postcode prefix after they are retrieved from the API. Fetch a larger `--max` value to compensate for the post-filter reduction.

### Postcode Prefixes by Region

Use these with the `--postcode` argument. Multiple searches can be combined in the spreadsheet afterwards.

| Region | Postcode prefixes |
|---|---|
| **East Anglia** | NR (Norfolk), IP (Suffolk), CB (Cambridgeshire), CO (North Essex/Colchester) |
| **East Midlands** | NG (Nottingham), LE (Leicester), DE (Derby), LN (Lincoln), PE (Peterborough/Lincolnshire) |
| **West Midlands** | B (Birmingham), CV (Coventry), WV (Wolverhampton), ST (Stoke), WS (Walsall) |
| **Yorkshire** | LS (Leeds), BD (Bradford), HX (Halifax), HD (Huddersfield), HG (Harrogate), YO (York), DN (Doncaster), S (Sheffield) |
| **North West** | M (Manchester), L (Liverpool), PR (Preston), BB (Blackburn), BL (Bolton), WN (Wigan) |
| **North East** | NE (Newcastle), SR (Sunderland), DH (Durham), TS (Teesside) |
| **South East** | TN (Kent/Tunbridge Wells), ME (Medway), CT (Canterbury), BN (Brighton), RH (Redhill), GU (Guildford) |
| **South West** | BS (Bristol), BA (Bath), EX (Exeter), PL (Plymouth), TR (Cornwall), SP (Salisbury) |
| **East of England** | CM (Chelmsford), SS (Southend), SG (Stevenage), AL (St Albans), HP (Hemel Hempstead) |
| **London** | E, EC, N, NW, SE, SW, W, WC, BR, CR, DA, EN, HA, IG, KT, RM, SM, TW, UB |
| **Scotland** | EH (Edinburgh), G (Glasgow), AB (Aberdeen), DD (Dundee), PH (Perth), IV (Inverness) |
| **Wales** | CF (Cardiff), SA (Swansea), NP (Newport), LL (North Wales) |
| **Northern Ireland** | BT (all Northern Ireland) |

### Example Commands

```bash
# Road hauliers in Norfolk — fetch 500, filter to NR postcode area
dotnet run -- --sic 49410 --postcode NR --max 500 --output norfolk_haulage.csv

# Agricultural contractors across East Anglia (run separately per county, combine in Excel)
dotnet run -- --sic 01610,01500 --postcode NR --max 500 --output agri_norfolk.csv
dotnet run -- --sic 01610,01500 --postcode IP --max 500 --output agri_suffolk.csv
dotnet run -- --sic 01610,01500 --postcode CB --max 500 --output agri_cambs.csv

# Bus operators nationwide with director names
dotnet run -- --sic 49310,49390 --max 500 --officers --output bus_leads.csv

# Multiple sectors, filter to Yorkshire
dotnet run -- --sic 49410,49310,01500 --postcode LS --max 500 --officers --output yorkshire_leads.csv

# Quick test — 10 results, no geographic filter
dotnet run -- --sic 49410 --max 10 --output test.csv
```

---

## 7. Output Format

The application writes a CSV file that opens directly in Excel. Columns:

| Column | Description |
|---|---|
| `CompanyNumber` | Companies House registration number |
| `CompanyName` | Registered company name |
| `RegisteredAddress` | Full registered address |
| `SICCodes` | SIC code(s) registered with Companies House |
| `CompanyType` | e.g., `ltd`, `plc` |
| `IncorporatedDate` | Date the company was formed |
| `Directors` | Director names, semicolon-separated (only if `--officers` used) |
| `ProductMatch` | Portland Fuel products this company is a candidate for |
| `MatchScore` | Score 1–10 indicating strength of fit |
| `CompaniesHouseURL` | Direct link to the company's Companies House page |

### ProductMatch values

| Value | Meaning |
|---|---|
| `Fuel Cards` | Fleet operator — good card candidate |
| `Bulk Delivery` | On-site storage likely — good bulk candidate |
| `Bunker Networks` | National HGV operator — Keyfuels/UK Fuels candidate |
| `Hedging` | Size and consistency of consumption suits hedging |
| `Consultancy` | Scale suggests consultancy interest |

---

## 8. SIC Codes in Use

The following SIC codes are configured in `LeadScorer.cs`. To add new sectors, add entries to the scoring dictionary in that file.

| SIC Code | Sector | Primary product match |
|---|---|---|
| 49410 | Freight transport by road | Fuel Cards, Bunker Networks |
| 49310 | Urban and suburban passenger transport (buses) | Fuel Cards, Hedging |
| 49390 | Other passenger land transport (coaches) | Fuel Cards, Hedging |
| 01110 | Growing of cereals, leguminous crops and oil seeds | Fuel Cards, Bulk Delivery |
| 01500 | Mixed farming | Fuel Cards, Bulk Delivery |
| 01610 | Support activities for crop production | Fuel Cards, Bulk Delivery |
| 01620 | Support activities for animal production | Fuel Cards, Bulk Delivery |
| 46710 | Wholesale of solid, liquid and gaseous fuels | Fuel Cards |
| 47300 | Retail sale of automotive fuel | Fuel Cards |
| 43120 | Site preparation / groundworks | Fuel Cards, Bulk Delivery |
| 77320 | Renting and leasing of construction machinery | Fuel Cards, Bulk Delivery |

---

## 9. API Reference

The application uses two Companies House API endpoints:

**Advanced company search:**
```
GET https://api.company-information.service.gov.uk/advanced-search/companies
    ?sic_codes={codes}
    &location={region}
    &company_status=active
    &items_per_page=20
    &start_index={offset}
```

**Company officers (directors):**
```
GET https://api.company-information.service.gov.uk/company/{company_number}/officers
```

Full API specification: <a href="https://developer-specs.company-information.service.gov.uk/companies-house-public-data-api/reference" target="_blank">Companies House Public Data API specification</a>

---

## 10. Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| `401 Unauthorized` | API key missing or wrong | Check `appsettings.json` — key must match exactly |
| `429 Too Many Requests` | Rate limit hit | Increase the delay in `RateLimiter.cs` |
| Empty CSV | No companies found for SIC/region | Try removing `--region` or check the SIC code is valid |
| CSV empty after postcode filter | Too few companies in that postcode area | Increase `--max` to fetch more before filtering, or try a neighbouring prefix |
| Missing directors column | `--officers` flag not set | Re-run with `--officers` |
| `appsettings.json not found` | Running from wrong directory | Run from inside `CompaniesHouseSearch/` |
