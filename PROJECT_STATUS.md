# Project Status

**Last Updated**: 2026-02-25

**GitHub Repository**: https://github.com/SureShotUK/REACH.git

## Current State
The terminai repository contains seven specialized project folders: HSEEA (health/safety/environment), IT (infrastructure/security), REACH (chemical compliance), NewCar2026 (electric vehicle research), XmlDotnetCoding (C# XML trade reporting), Canada (Canadian financial compliance), and the new Maintenance project (maintenance administration system for a UK manufacturing/engineering business). The Maintenance project has been initialised with CLAUDE.md and two production-ready Excel workbooks covering regulatory compliance tracking and job management across two sites.

## Active Work Areas
- **Maintenance Project - Maintenance Admin System**: 🆕 PROJECT CREATED (2026-02-25)
  - `/Maintenance/` - Excel-based maintenance tracking for UK manufacturing/engineering business
  - **Sites**: CITY (city centre office) + MFG (manufacturing/warehouse + portakabin offices)
  - **Compliance_Schedule.xlsx**: 31 pre-populated statutory/PPM records with formulas and conditional formatting
  - **Job_Log.xlsx**: 21-column job tracking with 6 dropdowns, status colour-coding, EMERG highlighting
  - **Regulatory coverage**: Gas Safety, EICR, Fire Safety Order, F-Gas, L8/Legionella, LOLER, PSSR, SEMA racking, PAT
  - **Outstanding actions** before system is live:
    - Enter Last Completed dates in Compliance Schedule col K
    - Confirm F-Gas CO2e charges for AC units at both sites (COMP-0008, COMP-0019)
    - Confirm PSSR Written Scheme of Examination exists (COMP-0027, COMP-0028)
    - Complete PAT risk assessments for both sites (COMP-0012, COMP-0031)
    - Confirm/commission Legionella RA for MFG portakabin showers (HIGH RISK)
  - **Next workbooks**: Asset Register + Contractor Register to be built next session
- **Canada Project - Canadian Financial Compliance**: PROJECT CREATED (2026-01-21)
  - `/Canada/` - Canadian securities compliance for UK firms using international dealer exemption
  - **Purpose**: Navigate Canadian multi-jurisdictional securities regulation for UK-based international dealers
  - **Focus**: Section 8.18 of NI 31-103 (international dealer exemption), permitted client restrictions
  - **Key regulatory framework**:
    - 13 provincial/territorial securities regulators (OSC, AMF, BCSC, ASC, etc.)
    - Canadian Securities Administrators (CSA) harmonization
    - National Instrument 31-103 (Registration Requirements, Exemptions and Ongoing Registrant Obligations)
    - Passport system for multi-jurisdictional coordination
    - UK-Canada regulatory coordination and FCA equivalency
  - **CLAUDE.md** (10,000+ words):
    - International dealer exemption requirements and limitations
    - Permitted client definition and qualification criteria
    - Multi-jurisdictional compliance strategies
    - Document organization (regulations, guidance, registration, compliance, exemptions, cross-border)
    - Typical workflows (regulatory research, exemption analysis, client qualification, compliance program design)
    - Cost-benefit analysis of exemptions vs. full registration
  - **gemini-canadian-financial-compliance-researcher agent** (7,500+ words):
    - Specialized for CSA, OSC, AMF, BCSC, ASC, CanLII, and FCA research
    - National/multilateral instrument expertise (NI 31-103, permitted client definition)
    - Exemptive relief and precedent decision analysis
    - Passport system navigation
    - Cross-border UK-Canada regulatory coordination
  - **Next**: Populate with NI 31-103 materials, staff notices, firm-specific exemptive relief documentation
- **XmlDotnetCoding Project - C# XML Trade Reporting**: 🆕 PROJECT CREATED (2025-11-26)
  - `/XmlDotnetCoding/` - C# tools for reading and processing UK derivatives trade reports (XML)
  - **Purpose**: Reverse-engineer XML trade reports back into TradeAndValuationProperties data models
  - **Three specialized agents**:
    - `csharp-xml-expert` - XML parsing, serialization, and class design
    - `csharp-reviewer` - C# code quality and best practices
    - `dotnet-tester` - Unit testing specialist (xUnit/NUnit/MSTest)
  - **TradeFileReader.cs** (314 lines):
    - Reads DerivativesTradeReportV03 from Pyld element in XML
    - Extracts TradeReport32Choice__1 items from report arrays
    - Identifies report types (New/Modify/Correction/Termination/Error)
    - Handles XML namespace navigation and XmlSerializer
  - **ReadUKReportsFromXml.cs** (956 lines):
    - Converts XML reports to `List<TradeAndValuationProperties>`
    - Supports NEWT, MODI, TERM, EROR action types
    - Extracts LEI identifiers (database enriches other counterparty data)
    - Complete data extraction: dates, prices, notionals, commodities, options
    - Four DerivativeEvent overloads for variant types (6__1, 6__2, 6__4, 6__5)
  - **Next**: Testing, additional commodity types, unit tests, XML validation
- **NewCar2026 Project - Electric Vehicle Research**: 🆕 PROJECT CREATED (2025-11-14)
  - `/NewCar2026/` - Vehicle research and decision-making for April 2026 EV purchase
  - **User Requirements**: 375+ miles WLTP, under £65,000, 0-60 mph under 5.5s, UK available April 2026
  - **Critical Finding**: Only 2 vehicles meet ALL requirements in UK market
  - **Primary Recommendation**: MG IM5 Long Range
    - 441 miles WLTP, £44,995, 4.9s 0-60, deliveries Sep 2025
    - £20,005 under budget, fully verified from MG UK sources
  - **Secondary Recommendation**: Tesla Model 3 Long Range RWD
    - 436 miles WLTP, £44,990, 5.2s 0-60
    - £20,010 under budget, requires availability confirmation for April 2026
  - **gemini-car-researcher agent**: Automotive research specialist (sonnet model, purple)
  - **Comprehensive Research Report**: 16,613 words covering 30+ vehicles investigated
    - Notable near-misses: Audi A6 e-tron (£69,900), VW ID.7 GTX (366 miles), VW ID.7 Pro S (437 miles but slow)
    - Market analysis: "price-performance-range triangle" - most EVs optimize for 2 of 3 factors
    - Chinese manufacturers disrupting premium segment with exceptional value
  - **Documents total**: 3 comprehensive documents (33,198 words)
- **REACH Project - UK Chemical Compliance**: 🎯 COST ESTIMATES REFINED FOR >1000 T/YEAR + SME
  - `/REACH/` - UK REACH compliance research for Urea imports (>1000 tonnes/year, SME business)
  - **CRITICAL DISCOVERY**: User eligible for late DUIN (first import: March 31, 2019 from EU)
  - **Status**: Deferred deadline until October 27, 2028 (3-year registration pathway)
  - **Tonnage band confirmed**: >1000 t/year (requires Annex X data, 60-70% higher costs)
  - **SME status**: Provides £1,482-£2,165 HSE fee savings vs. large enterprise
  - **DUIN Application folder**: 6 comprehensive documents for immediate submission
    - HSE inquiry email draft (ready to send)
    - DUIN submission checklist (6-phase process)
    - Historical evidence documentation template
    - 3-year post-DUIN registration pathway
    - **Cost estimate updated**: £35k-£45k (most realistic), £50k recommended budget
  - **New documents added** (Session 2025-11-06):
    - `FARM_LoA.md` - Letter of Access explanation (€3,322 pricing, £40k total budget)
    - `Only_Representative_Explained.md` - OR clarification (EU vs non-EU identical treatment)
  - **Documents total**: 18 comprehensive documents (135,000+ words, 350+ pages)
    - Research: `uk_reach_overview.md` (8,000 words)
    - Reports: `compliance_assessment_urgent.md` (12,000 words)
    - Costs: `cost_estimates.md` (15,000 words - updated for >1000 t/year)
    - Explanatory: `FARM_LoA.md` (11,000 words), `Only_Representative_Explained.md` (10,000 words)
    - Templates: 3 implementation checklists
    - Discussion: `REACH_Discussion.md` (16,000 words)
  - **Updated cost estimates**:
    - LoA route: £30k-£40k (recommended for SME with straightforward substance)
    - Full consortium: £45k-£56k
    - Total program with DUIN: £39k-£63k (2025-2028)
- **IT Project - Cross-Platform Security Documentation**: ✨ EXPANDED
  - `WIFI_Best_Practices_for_Laptops_and_Mobiles.md` - 26,000-word comprehensive cross-platform guide
  - `Mobile_Laptop_WIFI_Summary.md` - Ultra-concise single-page summary
  - `Public_WIFI_Best_Practices_Full.md` - Original 11,000-word laptop guide
  - `Public_WIFI_Checklist.md` - One-page printable quick reference
  - `Draytek_Connect.md` - 13,000-word L2TP/IPsec VPN setup guide
  - `gemini-it-security-researcher` agent - IT security research with verification methodology
  - Live VPN configuration in progress (Draytek Vigor 2865 firmware 4.5.1)
- **HSEEA Agents**: Three specialized agents available
  - `hse-compliance-advisor` - Health and safety compliance guidance (opus model)
  - `ea-permit-consultant` - Environment Agency permit assistance (sonnet model)
  - `gemini-hseea-researcher` - Web research for HSE/EA topics (sonnet model)
- **Session Management**: Slash commands configured for /end-session and /sync-session

## Recently Completed
- **Maintenance Project Created** (2026-02-25) - Maintenance admin system for UK manufacturing/engineering:
  - Created `/Maintenance/` project directory and comprehensive `CLAUDE.md`
  - Regulatory framework documented: LOLER, PSSR, EICR, Gas Safety, F-Gas, Fire Safety Order, L8/Legionella, SEMA racking, PAT
  - Built `Compliance_Schedule.xlsx`: 31 records covering CITY and MFG sites, auto-calculating Next Due Date and status
  - Built `Job_Log.xlsx`: 21-column job tracking with dropdowns, colour-coded status, EMERG/overdue highlighting
  - Built `build_workbooks.py` Python script for workbook regeneration
  - Updated root `CLAUDE.md` with projects table
- **Canada Project Created** (2026-01-21) - Canadian financial compliance for UK firms:
  - Created project structure with `/Canada/` folder and `.claude/` configuration
  - Developed comprehensive CLAUDE.md (10,000+ words) covering:
    - International dealer exemption (Section 8.18 of NI 31-103)
    - Multi-jurisdictional securities regulation (13 Canadian regulators)
    - Permitted client restrictions and qualification criteria
    - UK-Canada regulatory coordination and FCA equivalency
    - Passport system navigation
  - Created gemini-canadian-financial-compliance-researcher agent (7,500+ words)
  - Configured settings.local.json with CSA, OSC, AMF, BCSC, ASC, CanLII, FCA permissions
  - Set up /end-session and /sync-session commands via symlinks
- **XmlDotnetCoding Project Created** (2025-11-26) - C# XML processing for trade reports:
  - Created project structure with `/XmlDotnetCoding/` folder and `.claude/` configuration
  - Developed three specialized agents (csharp-xml-expert, csharp-reviewer, dotnet-tester)
  - Built TradeFileReader.cs (314 lines) for parsing UK derivatives trade reports
  - Built ReadUKReportsFromXml.cs (956 lines) for reverse-engineering XML to TradeAndValuationProperties
  - Supports all trade action types: NEWT, MODI, TERM, EROR
  - Extracts LEI identifiers only; database enrichment handles counterparty details
  - Complete data extraction: dates, identifiers, contract data, prices, notionals, commodities, options
  - Fixed bugs: DerivativeEvent overloads, NotionalQuantity structure, TpSpecified checks
- **NewCar2026 Project Created** (2025-11-14) - Electric vehicle research project:
  - Created project structure with `/NewCar2026/` folder
  - Developed gemini-car-researcher agent (10,199 words) specialized for automotive research
  - Conducted comprehensive UK EV market research with 14 parallel searches
  - Created 16,613-word research report analyzing 30+ vehicles
  - Identified only 2 vehicles meeting all 4 requirements
  - Primary recommendation: MG IM5 Long Range (£44,995, 441 miles, 4.9s 0-60)
  - Secondary recommendation: Tesla Model 3 Long Range RWD (£44,990, 436 miles, 5.2s 0-60)
  - Both vehicles ~£20,000 under budget
  - Market insight: "price-performance-range triangle" - rare to find all three
- **REACH Project Created** (2025-11-05) - 50,000 words of compliance documentation:
  - Comprehensive UK REACH research (7 web searches, 30+ sources)
  - Identified critical non-compliance situation (importing Urea without registration)
  - Developed compliance strategy with 4 options analyzed
  - Created 8 deliverable documents (research, reports, costs, templates)
  - Recommended Option A: Switch to GB supplier (£4k-£11k, 2-8 weeks)
  - Alternative Option B: Own registration (£65k-£345k, 12-24 months)
  - Professional advisor directory (legal counsel, REACH consultants, HSE contacts)
  - Implementation templates (supplier verification, HSE disclosure, ongoing compliance)
- **IT Security Documentation - Cross-Platform Expansion** (2025-11-04) - 26,000 additional words:
  - Cross-platform WiFi security guide for Windows/iOS/Android devices
  - Phone hotspot security section with critical WiFi-off warnings
  - Ultra-concise summary guide (75 lines, "what to do" only)
  - Mobile device security configurations (iOS and Android)
  - Device-specific indicators (📱💻🔄) throughout documentation
- **IT Security Documentation - Initial Creation** (2025-10-31) - 24,000+ words:
  - Public WiFi best practices guide with CISA/NIST/SANS research
  - Quick reference checklist for staff distribution
  - Complete Draytek VPN setup guide with troubleshooting
  - IT security research agent with double-checking methodology
- **Live VPN Configuration Assistance** (2025-10-31):
  - Draytek Vigor 2865 IPsec + L2TP configuration completed
  - User account created, PSK configured, MS-CHAPv2 authentication set
  - Adapted generic instructions for firmware 4.5.1 UI differences
- **HSEEA Research Agent** (2025-10-31): Created gemini-hseea-researcher agent for UK HSE/EA research

## Blocked/Pending
- **VPN Testing**: User needs to complete Windows 11 client configuration and test connection
- **Firewall Configuration**: May need manual firewall rules on Draytek (unclear if auto-configured in firmware 4.5.1)

## Next Priorities

### Maintenance Project (New - Immediate)
1. Open `Compliance_Schedule.xlsx` and enter Last Completed dates for existing obligations
2. Confirm F-Gas CO2e charges for AC units at both sites → determines inspection frequency
3. Confirm PSSR Written Scheme of Examination is in place for compressed air system at MFG
4. Confirm/commission Legionella risk assessment for MFG portakabin shower facilities (HIGH RISK)
5. Complete PAT risk assessments at both sites to set formal testing frequency
6. Build `Asset_Register.xlsx` with known assets at both sites
7. Build `Contractor_Register.xlsx` with approved contractors

### NewCar2026 Project
1. Verify Tesla Model 3 Long Range RWD availability for April 2026 via Tesla UK configurator
2. Contact MG UK dealers to confirm April 2026 delivery timeline
3. Arrange test drives when MG IM5 becomes available (September 2025)
4. Research insurance quotes for both vehicles
5. Investigate home charging setup requirements and costs
6. Monitor market for 2026 model year announcements (January-February 2026)
7. Potential: Create comparison spreadsheet and total cost of ownership analysis

### REACH Project (CRITICAL - This Week)
1. **Complete and send HSE inquiry email** to ukreach@hse.gov.uk (template ready in DUIN_Application folder)
2. **Begin gathering 2019-2020 import evidence** (March 31, 2019 invoice, shipping docs, SDS)
3. **Complete Historical_Evidence_Documentation.md** template sections
4. Brief senior management on DUIN eligibility discovery (changes situation from criminal offense to Oct 2028 deadline)
5. Brief finance department on £40k budget requirement spread over 2026-2028

### REACH Project (Weeks 2-4)
6. Receive HSE response on late DUIN process
7. Complete evidence package (invoices, shipping documents, supplier info from 2019-2020)
8. Submit late DUIN via UK REACH IT system following HSE guidance
9. Obtain DUIN acceptance confirmation from HSE
10. Update compliance status documentation

### REACH Project (2026-2028)
11. Research UK REACH Urea consortiums (RPA Ltd, Knoell, Ecomundo - see Cost_Estimate.md)
12. Contact industry associations (AIC, CIA, BPF) for consortium information
13. Engage REACH consultant and join consortium
14. Complete IUCLID registration dossier, CSA, and CSR
15. **Submit full UK REACH registration by September 1, 2028** (internal deadline with 8-week buffer)
16. **LEGAL DEADLINE: October 27, 2028**

### IT Project
13. Distribute `Mobile_Laptop_WIFI_Summary.md` to staff as quick reference
14. Complete Windows 11 VPN client setup and testing
15. Test gemini-it-security-researcher agent with live queries
16. Potential: Create device-specific security guide versions if needed

### Canada Project
17. Commit and push Canada project structure to repository
18. Test gemini-canadian-financial-compliance-researcher agent with live queries
19. Populate with core reference documents:
    - NI 31-103 summary (registration requirements and exemptions)
    - Section 8.18 detailed breakdown (international dealer exemption)
    - Permitted client definition quick reference
    - Firm-specific exemptive relief order (if applicable)
20. Create workflow templates for common compliance tasks
21. Build provincial/territorial comparison chart for multi-jurisdictional compliance
22. Document UK-Canada regulatory coordination procedures

## Key Files & Structure
- `/terminai/CLAUDE.md` - Shared guidance for all projects (updated 2026-02-25: projects table added)
- `/terminai/Maintenance/` - Maintenance admin system 🆕
  - `Maintenance/CLAUDE.md` - Project setup, regulatory framework, workbook schemas, data standards
  - `Maintenance/Compliance_Schedule.xlsx` - 31 statutory/PPM records; auto-calculates Next Due Date and Status
  - `Maintenance/Job_Log.xlsx` - Job tracking (STAT/PPM/REACT/EMERG); 6 dropdowns; colour-coded status
  - `Maintenance/build_workbooks.py` - Python script to regenerate workbooks (requires openpyxl)
- `/terminai/hseea/` - Health, Safety, and Environmental compliance knowledge
  - `/hseea/CLAUDE.md` - HSEEA-specific project guidance
  - `/hseea/.claude/agents/` - Custom agents for HSEEA domain
- `/terminai/it/` - IT infrastructure and security documentation
  - `/it/.claude/agents/` - Custom agents for IT domain
- `/terminai/REACH/` - UK REACH chemical compliance (Urea imports)
  - `/REACH/CLAUDE.md` - REACH project guidance
  - `/REACH/research/` - UK REACH research and regulations
  - `/REACH/reports/` - Compliance assessments (URGENT: read compliance_assessment_urgent.md)
  - `/REACH/costs/` - Cost estimates and financial analysis
  - `/REACH/templates/` - Implementation templates (supplier verification, HSE contact, compliance checklists)
  - `/REACH/README.md` - Project overview and executive summary
- `/terminai/NewCar2026/` - Electric vehicle research and decision-making
  - `/NewCar2026/CLAUDE.md` - Project guidance for automotive research
  - `/NewCar2026/.claude/agents/` - gemini-car-researcher agent
  - `/NewCar2026/UK_EV_Research_April_2026.md` - Comprehensive UK EV research report
- `/terminai/XmlDotnetCoding/` - C# XML trade report processing
  - `/XmlDotnetCoding/CLAUDE.md` - Project guidance for C# XML processing
  - `/XmlDotnetCoding/.claude/agents/` - C# specialized agents (xml-expert, reviewer, tester)
  - `/XmlDotnetCoding/Code/TradeFileReader.cs` - XML reader for derivatives trade reports
  - `/XmlDotnetCoding/Code/ReadUKReportsFromXml.cs` - Reverse-engineering XML to data models
  - `/XmlDotnetCoding/Code/TradeAndValuationProperties.cs` - Target data model
  - `/XmlDotnetCoding/Code/WriteUKReportsToXml.cs` - Write logic (for reference)
- `/terminai/Canada/` - Canadian financial compliance 🆕
  - `/Canada/CLAUDE.md` - Project guidance for Canadian securities compliance
  - `/Canada/.claude/agents/gemini-canadian-financial-compliance-researcher.md` - CSA/provincial regulator research specialist
  - `/Canada/.claude/settings.local.json` - Permissions for Canadian regulator sites
  - `/Canada/.claude/commands/` - Shared commands (end-session, sync-session)
- `/terminai/.claude/commands/` - Shared slash commands across projects
