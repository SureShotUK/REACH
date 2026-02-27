# Changelog

All notable changes to this HSE/EA compliance repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased] - 2026-02-27

### Added
- `Violence/CLAUDE.md` — Project context file for Violence and Aggression work:
  - Documents business profile (10 staff: 4 office, 3 drivers, 3 warehouse; B2B only; no public-facing operations)
  - Lists existing controls (CCTV, visitor sign-in), employee groups, lone working arrangements
  - Summarises 5 assessed scenarios and 15 outstanding actions
  - References key HSE guidance (online hub, INDG73 rev4) and legislation URLs
- `Violence/Violence_Aggression_Risk_Assessment.md` — Full draft violence and aggression risk assessment:
  - Legal framework: HSWA 1974 s.2, MHSWR 1999 Reg 3, RIDDOR 2013 (all legislation hyperlinked and verified)
  - 5×5 likelihood/severity risk matrix with Low/Medium/High/Very High banding
  - 5 scenarios: business visitors (Low 3→2), abusive phone/email (Low 4→2), drivers as lone workers (Medium 6→3), lone working on premises (Low 3→2), warehouse/contractor interactions (Low 4→2)
  - 15 action items with responsibility and target date columns
  - RIDDOR reporting thresholds table
  - Post-incident support commitments aligned to company policy
  - Training requirements for all staff and additional requirements for drivers/lone workers
  - Annual review schedule with incident/change triggers
- `Violence/Violence_Risk_Assessment.csv` — Structured CSV of all risk assessment data:
  - 15 rows (one per action), columns: Scenario_Ref, Scenario, Who_Is_At_Risk, Hazard_Description, Existing_Controls, Likelihood, Severity, Risk_Rating, Risk_Level, Action_Ref, Action_Description, Responsibility, Target_Date, Residual_Risk_Rating, Residual_Risk_Level
  - Suitable for import into Excel, SharePoint lists, or compliance tracking tools

---

## [Unreleased] - 2026-02-26

### Added
- `Fire/ND_FireDoors.md` — Fire door compliance analysis for 2-storey office building:
  - Evaluates fire risk assessor's findings against BS 476-22, BS 476-31.1, BS 8214:2016, Approved Document B (ADB), and the Regulatory Reform (Fire Safety) Order 2005
  - Explains technical distinction between intumescent strips (FD30) and cold smoke seals (FD30S designation)
  - Confirms that BS 476-22 is a fire resistance test only and does not require cold smoke seals
  - Identifies ADB Table B1 requirement for FD30S at stairway enclosures and corridor subdivisions
  - Clarifies BS 8214:2016 legal status (code of practice, not statute)
  - Confirms BS 476-22 validity until September 2029; BS EN 1634-1 mandatory for new doors from that date
  - Includes recommended actions with caveat on seal retrofitting and tested assembly requirements
  - Full legislation and standards reference table with verified HTML links

---

## [Unreleased] - 2026-02-20

### Added
- `OfficeFirstAiders.md` — First aid needs assessment for 1 Toft Green, York city centre office (171 lines):
  - L74 Table 1 checklist methodology applied across 8 assessment factors (hazards, employees, workforce, work patterns, distribution, remoteness, non-employees, accident history)
  - Recommendation: 2 EFAW-trained first-aiders; justification: absence cover (para 27/81), two-storey building (para 23), hybrid working (para 4)
  - Known health conditions flag additional illness recognition training (Appendix 4, Table 3)
  - Minimum vs recommended provision clearly distinguished; Appendix 3 flow chart applied (low-hazard, 25–50 = min 1 EFAW)
  - All L74 references cited at paragraph level; verified against source PDF; two citation errors corrected (see Changed below)
  - Legislation links: Regulation 3 and Regulation 4 of the Health and Safety (First-Aid) Regulations 1981, both verified via WebFetch
- `FirstAidersIndEst.md` — First aid needs assessment for industrial estate mixed warehouse/office site (240 lines):
  - Higher-hazard classification due to FLT and articulated truck operations
  - Recommendation: 1 FAW (warehouse) + 1 EFAW (office); FAW justified by FLT injury profile (Appendix 5 vs Appendix 6 comparison) and 13-mile hospital distance (para 24)
  - Additional training: haemostatic dressings/tourniquets for life-threatening bleeding (Appendix 4, Table 3)
  - Section 7: ambulance service written notification obligation (para 24)
  - Section 8: lone working procedures — mobile phone, check-in system, warehouse lone working review
  - Separate buildings (para 23) and absence cover (para 27/81) justify 2 trained personnel

### Changed
- `OfficeFirstAiders.md` (citation corrections after source verification):
  - EFAW description citation corrected: `[L74, para 5]` → `[L74, Guidance 3]` (the EFAW description is in an unnumbered paragraph between paras 5 and 6; para 5 itself covers training/qualification/competence requirements)
  - "Offices or shops" quote citation corrected: `[L74, para 14]` → `[L74, p.14]` (the quote is in an unnumbered paragraph between paras 14 and 15; para 14 itself reads "Using your health and safety risk assessments you will have identified the hazards...")

---

## [Unreleased] - 2026-02-05 (2)

### Added
- `FireEvacPlan.md` — Employee fire evacuation procedure (127 lines):
  - Legal basis: HSWA 1974 s.7 (employee duty) and Regulatory Reform (Fire Safety) Order 2005 (Articles 14, 15, 19, 21); all URLs verified against legislation.gov.uk
  - 11 sections structured as standalone reference points for general employees: Know Your Building (with Toft Green evacuation diagram), When You Hear the Alarm, Leaving the Building, If You Encounter Smoke or Fire, Helping Others, At the Assembly Point, Waiting for the All-Clear, Common Mistakes to Avoid, Training and Drills
  - Cross-references `FireMarshalEvacPlan.md` for fire marshal-specific duties
  - No lift references — office premises have no lifts
  - User added evacuation route diagram (`ToftGreenEvacPath.png`) and caption to Section 3

---

## [Unreleased] - 2026-02-05

### Added
- `FireMarshalEvacPlan.md` — Fire marshal evacuation procedure document (175 lines):
  - Legal basis: Regulatory Reform (Fire Safety) Order 2005 (Articles 13, 14, 15, 16, 19, 21) as amended by Fire Safety Act 2021; all article URLs verified against legislation.gov.uk
  - Four roles defined: Fire Marshal (zone duties), Nominated Person — Grab Bag, Nominated Person — Sign-In Boards (separate individual, with safety caveat), Head Fire Marshal / Assembly Point Warden
  - 7-phase evacuation procedure: alarm response → zone evacuation and sweep → nominated person duties → route to assembly point → headcount → fire service briefing → holding and stand down
  - Special considerations: vulnerable persons and personal evacuation plans (PEPs), dangerous substances (SDSs in grab bag, Article 16 duty), visitor and contractor briefing
  - Training requirements under Article 21; annual drill best practice
  - Post-evacuation report requirements and sign-in board retention

---

## [Unreleased] - 2026-02-04

### Changed
- `assessments/NoiseAssessment.md` — Regulation 6 scope clarified and assessment restructured (uncommitted):
  - Section 2 legal duties expanded into numbered lists; Reg 6(1) SFAIRP and Reg 6(2) programme distinguished with individual hyperlinks
  - Compliance position summary (Section 7) expanded into bulleted list; Regulation 10 duties added
  - Dosimetry recommendation section removed; recommendation sections renumbered 7.1–7.5
  - Review section updated to remove dosimetry trigger
  - Placeholder fields filled: dates, assessor name, site name and address, noise source description
  - Site map image reference added (NoiseAssessmentLocations.png)
- `assessments/NoiseAssessment.md` — Final machine operator timings and regulation hyperlinks (commits 7ced2c5, 9c7496f):
  - Operator timings finalised; LEP,d 83.1 dB(A), margin to upper action value 1.9 dB
  - All bare regulation references converted to verified hyperlinks to legislation.gov.uk
  - Mislabelled "Regulation 8" in closing note corrected to Regulation 5(6)

### Fixed
- Contradiction between Section 7 compliance summary and Section 7.2 regarding Regulation 6 scope resolved: Reg 6(1) SFAIRP duty applies at all exposure levels; only Reg 6(2) mandatory programme is conditional on the upper action value

### Documentation
- Weekly LEP,w calculated: 82.1 dB(A) for 4 days documented pattern + 1 day Area 9 (64.1 dB(A)) full shift. Day 5 contributes ~1.3% of weekly energy; result above lower AV (80), below upper AV (85) with 2.9 dB margin
- CLAUDE.md rule added: hyperlink and verify all regulation references in documents before inserting; correct mislabelled regulation numbers before linking

---

## [Unreleased] - 2026-02-03

### Added
- `assessments/NoiseAssessment.md` — Full noise assessment report (290 lines) based on 9-area site measurements from `NoiseLimits.csv`:
  - Legal framework section: CNAWR 2005 thresholds verified against legislation.gov.uk (lower AV 80, upper AV 85, limit value 87 dB(A) with HPD counted)
  - Measurement methodology: Martindale SP79 Class 2 limitations documented (±1.5 dB uncertainty, snapshot vs TWA distinction)
  - TWA dose calculations for two worker types using equal energy method:
    - Machine operator: 82.4 dB(A) estimated LEP,d (range 81.6–83.1) — above lower action value
    - Area 5 worker: 77.8 dB(A) estimated LEP,d — below lower action value (robust to measurement uncertainty)
  - Peak sound pressure analysis: all areas well below 135 dB(C) action value
  - Low-frequency content analysis: significant dB(C)–dB(A) divergence in distant areas (up to 20 dB) indicating structural propagation
  - Compliance-mapped recommendations: duties under Regs 5, 7, 8, 9 for machine operator; dosimetry, engineering controls (Area 1 focus), HPD availability, health surveillance, training

### Documentation
- Noise assessment distinguishes between legal duties triggered at each threshold and good-practice measures — important because the upper action value is not exceeded, so Regulation 6 mandatory noise reduction programme and mandatory HPD enforcement do not apply
- Compliance position summary included at top of recommendations section for quick reference

---

## [Unreleased] - 2026-01-19

### Added
- `3_Year_HSE_Plan.md` - Updated three-year HSE strategic plan (650+ lines) incorporating actual budget data:
  - Total investment revised to £36,255 (from original £30k estimate)
  - Year 1 (2026): £24,606 budget including £15,824 waste water infrastructure + £8,282 recurring costs + £500 NEBOSH
  - Year 2 (2027): £3,942 budget (recurring compliance costs only)
  - Year 3 (2028): £7,707 budget (recurring costs with significant training investment)
  - Hybrid structure: Detailed quarterly breakdown for Year 1, annual objectives for Years 2-3
  - Objectives aligned with HSE3yrObjectives.csv timeline (waste water assessment, training system, fire marshal plan, NEBOSH, HSE management system, driver H&S review, occupational health assessment, culture review, waste optimization)
  - Budget integration from HSE5yrRecurringCosts.csv with actual recurring costs per year
  - Internal time activities clearly marked as £0 throughout budget tables
  - Retained full legal compliance framework section from original plan

### Changed
- `Three_Year_HSE_Strategic_Plan.md` renamed to `Three_Year_HSE_Strategic_Plan_old.md` (preserved as reference)

### Documentation
- Budget updated to reflect actual recurring costs from CSV data analysis
- Year 1 budget now includes major waste water infrastructure project (£15,824)
- Quarterly objectives for Year 1 mapped to specific CSV timeline items (Q1-Q4)
- Annual objectives for Years 2-3 focused on operational embedding and cultural development
- Budget breakdown by category added: recurring compliance (£19,931), waste water (£15,824), NEBOSH (£500)

---

## [Unreleased] - 2026-01-06

### Added
- `Three_Year_HSE_Strategic_Plan.md` - Comprehensive three-year HSE strategic plan (550+ lines) for AdBlue manufacturing site including:
  - Executive summary: current position, 3-year vision, £30k investment overview
  - Legal compliance framework with record retention requirements (H&S, environmental, REACH records)
  - Competent person structure: NEBOSH-qualified internal lead + £2,500/year external specialists
  - Year 1 (2026): Foundation building - £11k budget with quarterly action plans
  - Year 2 (2027): System embedding - £10k budget, proactive monitoring, contractor management
  - Year 3 (2028): Optimization - £9k budget, ISO alignment, environmental excellence
  - Detailed tables for record retention periods (2 years to 40 years to permanent)
  - Legal duties by area: COSHH, workplace transport, work at height, racking, fire, environmental, welfare
  - Annual recurring compliance costs: £3,850/year from Year 2
  - Appendices: supplier recommendations, applicable regulations, HSE/EA resource links
- `Training_Matrix_Template.md` - Comprehensive training management system (600+ lines) including:
  - Part 1: Training requirements reference with mandatory training by role, frequencies, costs
  - Part 2: Individual staff training matrices for 4 warehouse operatives and 4 office staff
  - Part 3: Training schedule planner with monthly calendar and renewals alert list
  - Part 4: Training provider contacts directory organized by training type
  - Quick reference guide, 10 tips for effective training management, FAQs
- `Training_Matrix_Template.csv` - Excel-compatible training matrix (52 rows):
  - All 12 staff members with required training types
  - Columns: Name, Role, Training Type, Required, Status, Dates, Provider, Cost, Notes
  - Ready for filtering, sorting, conditional formatting in Excel/Google Sheets

### Documentation
- Detailed legal compliance framework covering what records must be kept and for how long
- Competent person requirements under MHSWR 1999 Regulation 7 explained
- Year-by-year quarterly implementation roadmap for systematic HSE management
- Training renewal frequency guide (HSE induction: 2 years, Forklift: 3-5 years, First Aid: 3 years, etc.)
- Budget allocation aligned to small operation constraints (<£2M turnover, £5-15k/year HSE budget)

---

## [Unreleased] - 2025-12-15

### Added
- `water/` directory for water discharge and treatment planning documentation
- `water/TreatmentPlan.md` - Comprehensive RO concentrate treatment and disposal plan including:
  - Stream cross-sectional assessment with U-shaped profile (0.4608 m² area)
  - Flow rate calculations: 2,019,168 L/day (~2.02 ML/day)
  - Yorkshire Water Naburn STW facility details and disposal costs
  - Monthly cost analysis tables for 10k, 30k, and 50k L/day production rates
  - Annual cost projections: £46,650-£303,000 (based on 5-day working week)
  - Cost comparison: internal haulage (£250/28m³) vs external haulage (£400/28m³)
  - Trip requirements: 93-465 annual trips depending on volume
  - Cost-benefit analysis for interim tankering vs. direct discharge permit

### Documentation
- Detailed trapezoidal rule calculation for stream cross-sectional area
- Flow velocity measurement methodology (3m distance in 59.17 seconds)
- 5-day working week (260 days/year) cost calculation basis
- Yorkshire Water Naburn STW address: Naburn Lane, Naburn, York, YO19 4RN
- Next steps for EA discharge consent application outlined

---

## [Unreleased] - 2025-12-04

### Added
- `Ladders/` subfolder for organized ladder compliance documentation

### Changed
- **CRITICAL CORRECTION**: `Ladders/Emergency_Escape_Ladder.md` - Fixed inaccurate statement about Approved Document B section 3.28
  - Executive Summary: Corrected to reflect fixed ladders ARE acceptable for specific circumstances
  - Section 1: Completely rewritten with accurate quote from section 3.28
  - Section 3: Updated Building Regulations Approval criteria
  - Section 6: Corrected comparison table Building Regulations Status
  - Section 7: Updated Building Regulations Check guidance
  - Section 11: Updated compliance checklist to reference section 3.28 criteria
- Moved all ladder documentation files into `Ladders/` subfolder for better organization

### Fixed
- **Regulatory Accuracy**: Corrected incorrect statement that fixed ladders are "NOT acceptable as primary means of escape"
- **Section 3.28 Compliance**: Documentation now accurately reflects that fixed ladders ARE acceptable when:
  1. NOT for members of the public
  2. Areas not normally occupied (plant rooms, equipment spaces, roof access)
  3. Where conventional stair would be impractical
- Updated all cross-references throughout Emergency_Escape_Ladder.md to consistently reflect accurate section 3.28 requirements

### Documentation
- Added direct quote from Approved Document B section 3.28 in Emergency_Escape_Ladder.md
- Clarified acceptable use cases: plant rooms, equipment spaces, areas not normally occupied
- Corrected guidance for Building Control approval process
- Updated practical compliance section to reflect accurate regulatory position

---

## [Unreleased] - 2025-12-02

### Added
- **Fixed Ladder Compliance Documentation Suite**:
  - `Emergency_Escape_Ladder.md` - Comprehensive 15-section guide covering emergency escape ladder legislation, inspection requirements, and compliance (81KB)
  - `Fixed_Ladders_Compliance.md` - Complete Work at Height Regulations 2005 compliance guide for work access ladders (26KB)
  - `Competent_Person_Fixed_Ladder_Inspections.md` - Detailed inspector qualification and competence requirements (13KB)
  - `Ladder_Types.md` - HSE perspective on ladder classifications and legal definitions (6KB)
- Key regulatory distinctions documented:
  - Emergency escape ladders vs. work access ladders (different primary legislation and inspection regimes)
  - Building Regulations Approved Document B position on vertical ladders as means of escape
  - BS 8210:2020 requirement for 5-yearly structural engineer surveys of fire escape ladders
  - RRF(SO) 2005 requirements for emergency exit signage, lighting, and accessibility
- Comprehensive inspection frequency tables and compliance checklists
- Cross-referenced document set with navigation between related ladder compliance topics

### Changed

### Fixed

### Documentation
- Emergency_Escape_Ladder.md covers Regulatory Reform (Fire Safety) Order 2005, PUWER 1998, Work at Height Regulations 2005, Building Regulations Approved Document B, BS 8210:2020
- All ladder documents include extensive legislative references with hyperlinks to legislation.gov.uk and HSE guidance
- Documented that vertical ladders are NOT acceptable as primary means of escape but may serve as supplementary routes
- Clarified that emergency escape ladders require 12-monthly inspections (less than work ladders) but 5-yearly structural surveys (more stringent)
- Established that competent person for inspections requires no mandatory qualifications - based on knowledge + experience + training

---

## [Unreleased] - 2025-10-31

### Added
- `/terminai/CLAUDE.md` - Shared context file for all projects with common principles
- `/terminai/.claude/commands/` directory - Shared slash commands location
- Symlinks in hseea and it projects to shared commands (end-session, sync-session)
- `office_hse.md` - Comprehensive one-page office health and safety best practices guide
- `.claude/agents/hse-compliance-advisor.md` - HSE compliance advisor agent configuration
- Detailed legislative references throughout office_hse.md (specific regulation numbers, sections, articles)
- Common office hazards table with control measures and key legislation column
- References to HSG, INDG, and L-series HSE guidance documents in office_hse.md

### Changed
- Reorganized directory structure to support multiple projects (hseea, it) with shared resources
- `CLAUDE.md` refactored to contain only HSE/EA-specific context (shared context moved to parent)
- `GEMINI.md` moved to `/terminai/` for shared access across projects
- `.claude/commands/end-session.md` converted to symlink pointing to shared command
- `.claude/commands/sync-session.md` converted to symlink pointing to shared command
- office_hse.md updated to version 2.0 with comprehensive legislative references
- PROJECT_STATUS.md updated to reflect multi-project structure and completed office HSE documentation
- SESSION_LOG.md updated with 2025-10-31 restructuring session details

### Fixed

### Documentation
- office_hse.md covers HSWA 1974, MHSW Regulations 1999, DSE Regulations 1992, Workplace Regulations 1992, RR(FS)O 2005, RIDDOR 2013, and related UK legislation
- Included references to HSE guidance: HSG65, INDG163, INDG143, INDG214, INDG232, INDG244, INDG345, HSG218, INDG430
- Included Approved Codes of Practice: L23, L24, L26, L73, L74

---

## [Unreleased] - 2025-10-30

### Added
- SESSION_LOG.md for tracking work sessions
- PROJECT_STATUS.md for current project state
- CHANGELOG.md for version-style change tracking
- `/end-session` slash command for automated session documentation

### Changed

### Fixed

### Documentation

---

<!-- Previous changes will be added below as they occur -->
