# Changelog

All notable changes to the terminai repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [Unreleased] - 2025-11-14

### Added - NewCar2026 Project Created
- **NewCar2026 Project Structure** - New electric vehicle research project:
  - `/NewCar2026/` - Project folder for April 2026 EV purchase research and decision-making
  - `/NewCar2026/.claude/agents/` - Specialized agent folder
  - `/NewCar2026/CLAUDE.md` - Project-specific guidance for automotive research (6,386 words)
    - Document organization suggestions (research, comparisons, pricing, safety, reliability, test-drives, decisions)
    - Key information categories (vehicle specs, safety ratings, reliability, cost analysis, reviews)
    - Research best practices (multiple sources, current data, regional awareness, model year clarity)
    - Typical workflows (vehicle research, comparison, cost analysis, safety evaluation, decision support)
    - Key considerations for 2026 purchase (timing, EV-specific factors, market conditions, test drive importance)
    - Limitations section (prediction constraints, subjective factors, regional variations)
- **gemini-car-researcher Agent** (`NewCar2026/.claude/agents/gemini-car-researcher.md` - 10,199 words):
  - Specialized automotive research agent configured with sonnet model (purple identifier)
  - Primary sources: manufacturer websites, safety orgs (IIHS, NHTSA, Euro NCAP), reliability databases (Consumer Reports, J.D. Power), automotive publications, pricing guides (KBB, Edmunds)
  - 6-step research approach: clarify needs, targeted searches, evaluate sources, extract info, synthesize findings, identify gaps
  - Special research scenarios: new models, comparisons, total cost of ownership, EVs, used vehicles
  - Quality standards: accuracy, currency, objectivity, completeness, transparency, regional awareness
  - Structured output format for presenting research results
- **Comprehensive UK EV Research Report** (`NewCar2026/UK_EV_Research_April_2026.md` - 16,613 words):
  - **Executive Summary**: Only 2 vehicles meet all 4 requirements (375+ miles, under £65k, 0-60 under 5.5s, April 2026 available)
  - **Primary Recommendation**: MG IM5 Long Range
    - 441 miles WLTP, £44,995, 4.9s 0-60, deliveries September 2025
    - £20,005 under budget, fully verified from MG UK sources
  - **Secondary Recommendation**: Tesla Model 3 Long Range RWD
    - 436 miles WLTP, £44,990, 5.2s 0-60
    - £20,010 under budget, requires availability confirmation for April 2026
  - **Vehicles That Nearly Qualify**: 10+ vehicles analyzed that miss one requirement
    - Audi A6 e-tron Performance (463 miles, 5.4s, £69,900 - £4,900 over budget)
    - VW ID.7 GTX (366 miles - 9 short, 5.2s, £61,980)
    - VW ID.7 Pro S (437 miles, £55,450, 6.4s - too slow)
    - Polestar 3 Long Range Dual Motor (395 miles, 4.5s, £69,990 - over budget)
    - Plus: BMW i4, Alpine A110, Toyota C-HR+, Ford Capri EV, Mercedes CLA, Polestar 2
  - **Vehicles Investigated But Not Qualifying**: 20+ vehicles researched (too expensive, insufficient range, or slow)
    - Premium segment: Mercedes EQS, BMW iX, Tesla Model S, Porsche Taycan, Polestar 5
    - Korean brands: Hyundai Ioniq 5 N, Ioniq 6, Kia EV6, Genesis GV60/G80/GV70
    - Performance models: Tesla Model 3 Performance, Volvo EX30, MG4 XPower
    - Upcoming models: Alfa Romeo Giulia/Stelvio Electric (specs uncertain)
    - Not available: Lucid Air (no RHD production for UK)
  - **UK EV Market Analysis**:
    - "Price-Performance-Range Triangle": Most EVs optimize for 2 of 3 factors, not all three
    - Chinese manufacturers (MG) disrupting premium segment with exceptional value
    - 2026 outlook: improvements expected but most still fall short at sub-£65k price point
  - **Recommendations and Next Steps**: Detailed action plan from November 2025 through April 2026
  - **Alternative Strategies**: Options if requirements can be slightly relaxed
  - **Important Limitations**: WLTP vs. real-world range, pricing variability, professional advice recommended

### Documentation - NewCar2026 Research Methodology
- **14 Parallel Research Searches Conducted**:
  - Long-range EVs (375+ miles WLTP) in UK market 2025-2026
  - High-performance EVs (0-60 under 5.5s) under £65,000
  - Premium brands (Mercedes EQS, BMW iX, Tesla Model S, Porsche Taycan, Polestar)
  - German brands (Audi A6/Q6 e-tron, BMW i4, VW ID.7 variants)
  - Korean brands (Kia EV6, Genesis electric vehicles)
  - Chinese brands (MG, BYD, NIO, XPeng, Lotus)
  - Lucid Air UK availability and RHD production
  - Critical verification searches for MG IM5 and Tesla Model 3 Long Range RWD
- **Sources Consulted**:
  - Official UK manufacturer websites and configurators
  - UK automotive publications (What Car?, Autocar, Auto Express, Car Magazine)
  - Safety organizations (IIHS, NHTSA, Euro NCAP)
  - WLTP range data from official UK/EU testing
  - Reliability databases and automotive research firms
- **Key Finding**: User requirements (375+ miles, under £65k, 0-60 under 5.5s) are extremely rare in UK market
- **Market Insight**: "Price-Performance-Range Triangle" identified - manufacturers optimize for 2 of 3 factors
- **Verification Approach**: Cross-referenced specifications across multiple authoritative sources
- **Chinese Market Disruption**: MG IM5 demonstrates exceptional value breaking traditional premium pricing

---

## [Unreleased] - 2025-11-06

### Added - >1000 t/year Cost Updates & LoA/OR Explanations (Session 09:00)
- **FARM_LoA.md** - Comprehensive Letter of Access explanation (11,000 words):
  - What FARM REACH consortium is and how LoA licensing works
  - Confirmed pricing: €3,322 (~£2,850) for Urea >1000 t/year LoA
  - Complete cost comparison: LoA route (£24k-£40k) vs full consortium (£34k-£62k)
  - Savings analysis: £10,000-£22,000 via LoA route for straightforward substances
  - 6-step process: contact, review, sign, engage consultant, complete registration
  - 25+ specific questions to ask FARM REACH during Q1 2026 inquiry
  - Risk assessment: all low/very low for well-studied substance like Urea
  - When full consortium might be better: complex uses, multiple substances, large company
  - Complete action plan timeline Q4 2025 through Q3 2028
  - FAQ addressing 10 common concerns (one-time payment, sharing LoA, data ownership)
  - Budget recommendation revised: £40,000 (LoA route with contingency)
- **Only_Representative_Explained.md** - OR mechanism clarification (10,000 words):
  - Comprehensive OR definition: GB-based entity representing non-GB manufacturer
  - Who appoints OR: manufacturer only (NOT importer)
  - GB-based entity legal requirements
  - **CRITICAL FINDING: NO DIFFERENCE between EU and non-EU suppliers under UK REACH**
    - Both are "non-GB" from UK perspective
    - Identical treatment, process, requirements
    - Post-Brexit: EU suppliers went from "same regulatory area" to "non-GB manufacturers"
  - Brexit impact analysis: downstream users became importers overnight (why DUIN exists)
  - Cost comparison: Supplier appointing OR (£48k-£110k) vs self-registration (£37k-£54k)
  - When OR is better: supplier has many GB customers, low volume, limited budget
  - When self-registration is better: high volume (>1000 t/year), control, flexibility, business-critical
  - Email template for asking supplier about OR appointment (ready to send)
  - Analysis: user's supplier likely doesn't have OR (DUIN eligibility proves it)
  - Verification methods: SDS Section 1, HSE inquiry, supplier documentation
  - Scenario analysis: what if supplier appoints OR after you register (maintain or withdraw)
  - Recommendation: Self-registration for >1000 t/year, business-critical imports

### Changed
- **DUIN_Application/Cost_Estimate.md** - Complete rewrite for >1000 t/year tonnage band with SME status:
  - Updated tonnage band: 100-1000 t/year → **>1000 t/year** throughout
  - Added Annex X data requirements explanation (long-term toxicity, reproductive, ecotoxicology)
  - Added SME fee structure: Micro (£57), Small (£399), Medium (£740), Large (£2,222)
  - SME savings: £1,482-£2,165 vs large enterprise
  - Confirmed HSE flat-fee structure (no tonnage-based increase as of April 2025)
  - Previous >1000 t/year fee: £29,419 → New fee: £57-£2,222 (massive savings under 2025 structure)
  - FARM REACH LoA pricing confirmed: €3,322 for >1000 t/year
  - Consortium cost range: £15,000-£30,000 (vs £8,000-£18,000 for 100-1000 t/year)
  - Consultant costs: £12,750-£26,000 (70% increase due to Annex X complexity)
  - Three detailed costing scenarios:
    - Scenario 1 (LoA route, minimum): £33,824
    - Scenario 2 (full consortium, recommended): £55,952
    - Scenario 3 (conservative budget): £48,481
  - Comparison table: 100-1000 t/year vs >1000 t/year (60-70% cost increase)
  - Why costs increase: Annex X data package, enhanced CSR, additional consultant workload
  - Cost-saving strategies for SMEs (8 strategies, £12k-£30k potential savings)
  - Budget approval template with business case for board presentation
  - DUIN submission costs: £1,000-£2,000 (free HSE submission, minimal associated costs)
  - Total program budget: £39,000-£63,000 (2025-2028 including DUIN)
  - Recommended board approval: £50,000 (LoA route) or £65,000 (full consortium with buffer)
  - Document expanded: 610 lines → 927 lines (~15,000 words)

### Documentation
- Clarified Annex X requirements for >1000 t/year band (additional testing beyond Annexes VI-IX)
- Explained why Urea costs lower despite Annex X (well-studied substance, data exists)
- Detailed SME classification system (employee count, turnover, balance sheet thresholds)
- Documented FARM REACH consortium as confirmed LoA source for Urea
- Explained Letter of Access vs full consortium membership trade-offs
- Clarified Only Representative mechanism and manufacturer vs importer roles
- **Critical clarification: EU and non-EU suppliers treated identically under UK REACH (both "non-GB")**
- Explained Brexit impact: intra-EU trade (downstream user) → cross-border (importer)
- Provided email templates for supplier OR inquiry
- Added verification methods for checking supplier OR status

## [Unreleased] - 2025-11-05

### Added - DUIN Eligibility Discovery (Session 16:30)
- **CRITICAL DISCOVERY**: User eligible for late DUIN submission (first import March 31, 2019 from EU supplier)
  - Changes compliance status from "criminal offense" to "deferred deadline until October 27, 2028"
  - Transforms timeline from immediate action to 3-year registration preparation (2026-2028)
- **DUIN Application Folder** (`/REACH/DUIN_Application/`) - Complete submission package:
  - `README.md` - Quick start guide, FAQ, immediate action plan (4,500 words)
  - `HSE_Inquiry_Email_Draft.md` - Ready-to-send email template for ukreach@hse.gov.uk
  - `DUIN_Submission_Checklist.md` - 6-phase comprehensive checklist (preparation to acceptance)
  - `Historical_Evidence_Documentation.md` - Evidence gathering template for 2019-2020 imports (8,000 words)
  - `Post_DUIN_Registration_Pathway.md` - 3-year roadmap for full registration (9,500 words)
  - `Cost_Estimate.md` - Detailed cost breakdown for straightforward consortium route (7,000 words)
    - Base estimate: £35k-£38k total (consortium membership, consultant, HSE fees, internal time, legal)
    - Conservative estimate: £23,619 (lower bound)
    - Higher estimate: £57,994 (upper bound)
    - Comparison: £35k consortium vs £140k-£324k individual (73-88% savings)
    - Payment timeline: 2026 (£17k), 2027 (£10k), 2028 (£12k)
- **Comprehensive Discussion Document** (`REACH_Discussion.md` - 16,000 words):
  - Crown Court penalties (unlimited fines, up to 2 years imprisonment) with Regulation 17 REACH Enforcement Regulations 2008 references
  - Detailed "downstream user" definition under EU REACH Article 3(13) with pre-Brexit status explanation
  - How Brexit converted downstream users to importers (critical for understanding DUIN eligibility)
  - Late DUIN eligibility criteria with statutory references (The REACH etc. (Amendment etc.) (EU Exit) Regulations 2020)
  - Registration deadlines by tonnage: 100-1000 t/yr = Oct 27, 2028; 1000+ t/yr or CMR = Oct 27, 2026
  - UK REACH vs EU REACH post-Brexit separation and lack of mutual recognition
  - ECHA public database vs HSE (no public search - must contact HSE directly)
  - Urea-specific information: CAS 57-13-6, EC 200-315-5, low hazard profile, common uses
  - 8 strategies for finding UK REACH Urea consortiums (consultancies: RPA Ltd, Knoell, Ecomundo; trade associations: AIC, CIA, BPF)
- **Research URLs** (`REACH/ResearchURLs.md`) - Curated list of UK REACH official sources

### Changed
- `REACH/UK_vs_EU_REACH_Critical_Distinction.md` - Added comprehensive "What Was a Downstream User Under EU REACH?" section
  - Pre-Brexit status explanation (intra-EU trade = downstream user)
  - Post-Brexit status change (EU to GB = import, triggers registration)
  - Comparison table showing status change overnight
  - Why DUIN mechanism was created (Brexit transition support for thousands of converted importers)

### Documentation
- Enhanced downstream user definition throughout REACH documentation
- Added detailed explanation of Article 127E UK REACH (DUIN mechanism)
- Clarified customs supervision exemption (temporary storage/re-export only, not free circulation)
- Defined CMR substances (Carcinogenic, Mutagenic, Reprotoxic) with priority deadlines
- Documented late DUIN submission process (original deadline Oct 27, 2021, still accepting late submissions)

### Added - Earlier Session (13:35)
- **GitHub Repository**: Pushed REACH project to https://github.com/SureShotUK/REACH.git
  - Configured git remote origin
  - Pushed all REACH documentation (8 files, 50,000 words)
  - Pushed complete commit history (4 commits)
  - Set up upstream tracking for main branch
- **REACH Project - UK Chemical Compliance Research** (NEW PROJECT):
  - Created `/REACH/` project folder with complete structure:
    - `/research/` - UK REACH research and regulations
    - `/reports/` - Compliance assessments and findings
    - `/templates/` - Implementation checklists and verification forms
    - `/costs/` - Cost breakdowns and financial analysis
  - **8 comprehensive documents created** (~50,000 words, 130 pages):
    - `REACH/CLAUDE.md` - Project-specific instructions for REACH compliance work
    - `REACH/research/uk_reach_overview.md` - 8,000-word comprehensive UK REACH reference
      - UK REACH post-Brexit requirements vs. EU REACH
      - Tonnage band requirements (100+ tonnes/year = Annexes VI-IX data)
      - Registration deadlines (extended to Oct 2026/2028/2030)
      - Enforcement and penalties (criminal offense, £5k+ fines, imprisonment)
      - Urea-specific information (CAS 57-13-6, EC 200-315-5)
      - 2025 fee changes (new flat rates effective April 1, 2025)
    - `REACH/reports/compliance_assessment_urgent.md` - 12,000-word critical compliance report
      - **CRITICAL FINDING**: Currently importing without registration (criminal offense)
      - Immediate actions required (STOP importing, engage legal counsel, contact HSE)
      - Four compliance options analyzed with detailed pros/cons
      - Week-by-week implementation plan
      - Professional advisor directory (legal, REACH consultants, HSE)
      - Risk mitigation strategies
    - `REACH/costs/cost_estimates.md` - 6,000-word detailed cost analysis
      - Option A (GB Supplier): £4k-£11k upfront, £30k-£50k over 5 years, 2-8 weeks timeline
      - Option B (Own Registration): £65k-£345k upfront, £90k-£420k over 5 years, 12-24 months
      - Cost breakdown by scenario (best/mid/worst case)
      - ROI calculations and break-even analysis
      - Financing options and cost optimization strategies
    - `REACH/templates/supplier_registration_verification.md` - Supplier verification checklist
      - UK REACH registration verification procedures
      - Use coverage assessment
      - Risk management measures
      - Annual review tracking
      - Red flags to watch for
    - `REACH/templates/hse_disclosure_template.md` - HSE contact templates
      - Full disclosure email template (requires legal review)
      - Alternative information request approach
      - Preparation checklist before sending
      - Follow-up protocol
      - Document retention requirements
    - `REACH/templates/downstream_user_compliance_checklist.md` - Ongoing compliance management
      - Initial compliance setup procedures
      - Annual compliance review checklist
      - Ongoing activities (purchasing, supplier changes, supply chain communication)
      - Incident reporting procedures
      - Document retention (10-year requirement)
      - Management review template
    - `REACH/README.md` - 4,000-word project overview and executive summary
      - Navigation guide to all deliverables
      - Recommended action plan (week-by-week)
      - Budget summary
      - Key contacts and advisors
      - Critical success factors

### Changed
- Updated repository structure to include three project folders: HSEEA, IT, REACH

### Documentation
- **UK REACH Compliance Research**:
  - Conducted 7 comprehensive web searches across UK HSE, GOV.UK, legislation, and industry sources
  - Consulted 30+ authoritative sources (HSE, ECHA, DEFRA, industry consultants)
  - Researched current 2025 regulations including April 1, 2025 fee changes
  - Identified critical non-compliance situation requiring immediate action
  - Developed pragmatic compliance strategy with cost-benefit analysis
  - Created professional templates ready for immediate implementation
- **Key Findings**:
  - User currently importing Urea (>100 tonnes/year) without UK REACH registration
  - This is a criminal offense under REACH Enforcement Regulations 2008
  - Penalties: £5,000+ fines, up to 3 months imprisonment, business disruption
  - Recommended solution: Switch to GB supplier with UK REACH registration
  - Timeline to compliance: 2-8 weeks (fast) vs. 12-24 months (own registration)
  - Cost: £4k-£11k (fast) vs. £65k-£345k (own registration)

---

## [Unreleased] - 2025-11-04

### Added
- **IT Project - Cross-Platform WiFi Security Documentation**:
  - `it/WIFI_Best_Practices_for_Laptops_and_Mobiles.md` - Comprehensive 26,000-word cross-platform guide
    - Covers Windows laptops, iOS, and Android mobile devices
    - Device-specific indicators throughout: 📱 Mobile Only, 💻 Laptop Only, 🔄 Both Devices
    - Complete mobile security configurations (iOS: Private Address, Face ID, Find My; Android: Play Protect, randomized MAC)
    - Cross-platform tool recommendations (VPNs, password managers, MFA apps)
  - `it/Mobile_Laptop_WIFI_Summary.md` - Ultra-concise single-page summary (75 lines)
    - "What to do" only, no "how to" instructions
    - Phone hotspot positioned as primary recommendation
    - Critical WiFi-off warning prominently displayed
  - **New major section: Using Your Phone as a Secure Hotspot**
    - iPhone Personal Hotspot setup instructions
    - Android Mobile Hotspot and Tethering setup instructions
    - 🚨 CRITICAL WARNING: Turn OFF WiFi on phone when using as hotspot
    - Benefits vs. public WiFi comparison
    - Data usage management table
    - Troubleshooting guide

### Previous Additions (2025-10-31)
- **IT Project - Security Documentation Suite**:
  - `it/.claude/agents/gemini-it-security-researcher.md` - IT security research agent with verification methodology
  - `it/Public_WIFI_Best_Practices_Full.md` - Comprehensive 11,000-word public WiFi security guide for non-technical users
  - `it/Public_WIFI_Checklist.md` - One-page printable quick reference checklist
  - `it/Draytek_Connect.md` - 13,000-word L2TP/IPsec VPN setup guide for Draytek Vigor 2865 and Windows 11
- **HSEEA Project**:
  - `gemini-hseea-researcher` agent in `hseea/.claude/agents/gemini-hseea-researcher.md`
    - Specialized web research agent for UK HSE/EA compliance topics
    - 6-step research methodology for finding authoritative guidance
    - Prioritizes official UK sources (hse.gov.uk, gov.uk/environment-agency, legislation.gov.uk)
    - Distinguishes between legal requirements and best practice recommendations
    - Configured with sonnet model and blue color identifier

### Changed
- `it/Mobile_Laptop_WIFI_Summary.md` - Reduced from comprehensive guide to ultra-brief summary per user request
  - Removed all "how to" instructions, kept only "what to do"
  - VPN coverage reduced to single sentence
  - Emphasized phone hotspot as primary security measure

### Previous Changes (2025-10-31)
- `it/CLAUDE.md` - Added reference to new `gemini-it-security-researcher` agent
- `it/Public_WIFI_Checklist.md` - User edited to adjust VPN from critical to recommended priority

### Documentation

**Cross-Platform WiFi Security (2025-11-04)**:
- **Comprehensive Cross-Platform Guide** - `WIFI_Best_Practices_for_Laptops_and_Mobiles.md`:
  - Expanded to cover Windows 11, iOS, and Android platforms
  - Device-specific security configurations for all three platforms
  - Phone hotspot security as primary recommendation
  - Mobile password manager and MFA setup with auto-fill configuration
  - Mobile VPN setup and verification procedures
  - Platform-specific physical security considerations
  - Separate configuration checklists for Windows/iOS/Android
  - Mobile-specific attack vectors and threat explanations
- **Ultra-Concise Summary** - `Mobile_Laptop_WIFI_Summary.md`:
  - Single-page quick reference (75 lines)
  - Action-focused ("what to do" only, no "how to")
  - Phone hotspot as primary recommendation
  - VPN mentioned in single sentence per user specification
  - Critical warnings prominently displayed

**Previous Documentation (2025-10-31)**:
- **Public WiFi Security Guide** - Based on authoritative sources:
  - CISA Federal Mobile Workplace Security (2024 Edition)
  - NIST SP 800-215 (November 2022)
  - SANS Institute, OWASP, Microsoft Security, Cisco best practices
  - Covers VPN selection, MFA setup, password managers, Windows configuration, physical security
  - Includes step-by-step setup guides and troubleshooting
- **Draytek VPN Guide** - Comprehensive two-part guide:
  - Part 1: Draytek Vigor 2865 L2TP/IPsec server configuration (8 detailed steps)
  - Part 2: Windows 11 VPN client setup (7 steps with registry modification)
  - Troubleshooting section for common errors (809, 789, 691)
  - Security hardening, maintenance, and monitoring procedures
  - Quick reference appendix with commands and settings tables
- Created SESSION_LOG.md to track Claude Code session history
- Created PROJECT_STATUS.md to track overall repository state
- Created CHANGELOG.md to document all repository changes

### Technical Details
- **VPN Configuration**: L2TP/IPsec with AES-256/SHA-256 encryption, MS-CHAPv2 authentication
- **Security Standards**: All recommendations based on CISA, NIST, SANS, OWASP, CIS guidance
- **Target Audience**: Documentation optimized for non-technical users with step-by-step instructions
- **Research Methodology**: Used gemini-researcher agent with 5 parallel searches for authoritative sources

---
