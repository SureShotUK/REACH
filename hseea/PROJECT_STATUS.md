# Project Status

**Last Updated**: 2026-02-27

## Current State

This HSE/EA compliance knowledge repository is actively being populated with practical compliance guidance and reference materials. Session documentation system is working successfully. Major documentation achievements include office HSE best practices guide, a comprehensive four-document ladder compliance suite, water discharge permit planning documentation, an updated three-year HSE strategic plan with realistic budget allocations, a noise assessment report with full CNAWR 2005 compliance analysis, fire evacuation plans for the office, first aid needs assessments for both sites, fire door compliance analysis, and now a violence and aggression risk assessment.

**Recent Update**: Violence and aggression staff guide and incident report form created. `Violence/HTDW_Violence.md` provides role-specific guidance for all three staff groups (office, warehouse, drivers). `Violence/Incident_Report_Form.docx` provides a standalone Word incident report form (HTDW-VAA-001-F1). Document reference abbreviation confirmed as **VAA** (not VAG) throughout this project.

**Infrastructure Update**: The repository structure has been reorganized to support multiple projects (hseea and it) with shared context files and slash commands at the parent `/terminai/` level. This allows for better organization and reuse of common tooling across different project domains.

## Active Work Areas

- **Violence and Aggression**: Active — core documents complete. Staff guide (`HTDW_Violence.md`) and incident report form (`Incident_Report_Form.docx`) created. Need to: fill in placeholders; implement 15 RA actions (A4, A5, A2, A6c, A9 priority); brief all staff using the guide; distribute incident report form
- **Fire Door Compliance**: Active — `Fire/ND_FireDoors.md` created. Need to: confirm which doors are on protected escape routes; challenge fire risk assessment wording; engage fire door specialist to assess FD30S upgrade options; establish inspection regime
- **First Aid Assessments**: Pending implementation — `OfficeFirstAiders.md` and `FirstAidersIndEst.md` created; need commit, training arrangements, and emergency services notification for industrial estate site
- **Noise Assessment**: Active — `assessments/NoiseAssessment.md` finalised at LEP,d 83.1 dB(A) (1.9 dB margin to upper AV). Reg 6 two-tier structure clarified and assessment updated. Dosimetry section removed. Uncommitted edits pending. Key next steps: commit changes; investigate engineering controls for Area 1; enrol machine operator in health surveillance; consider reinstating dosimetry recommendation given remaining margin is still relatively narrow
- **Three-Year HSE Strategic Planning**: Updated - Comprehensive 2026-2028 strategic plan revised with actual budget data
  - New file: `3_Year_HSE_Plan.md` replacing original strategic plan
  - Budget updated to £36,255 total based on actual recurring costs from CSV data
  - Year 1 (2026): £24,606 - includes £15,824 waste water infrastructure + £8,282 recurring + £500 NEBOSH
  - Year 2 (2027): £3,942 - recurring compliance costs only
  - Year 3 (2028): £7,707 - recurring costs with significant training investment
  - Year 1 maintains detailed quarterly structure; Years 2-3 use annual objectives format
  - Objectives aligned with HSE3yrObjectives.csv timeline
  - Training matrix template previously created remains current
- **Water Discharge Permit Planning**: Active - RO concentrate disposal planning and EA permit preparation
  - Stream assessment completed: 2.02 ML/day flow capacity
  - Interim disposal costs calculated for Yorkshire Water Naburn STW
  - Awaiting RO concentrate test results for dilution ratio calculations
- **Repository Structure**: Completed - Multi-project structure with shared resources established
- **Office HSE Documentation**: Completed - `office_hse.md` created with full legislative references
- **Fixed Ladder Compliance Documentation**: Completed - Comprehensive 4-document suite covering emergency escape and work access ladders
- **Agent Configuration**: Active - hse-compliance-advisor and ea-permit-consultant agents configured
- **Assessment Templates**: Not started - Risk, COSHH, DSE templates needed (next priority after strategic plan implementation begins)

## Recently Completed

- **Violence and Aggression Staff Guide and Incident Form** (2026-02-27):
  - Created `Violence/HTDW_Violence.md` — staff guide (HTDW-VAA-001): 3 chapters (office/warehouse/drivers), 5 rights, numbered step sequences, incident reporting, management support commitments, Appendix A incident report form
  - Created `Violence/Incident_Report_Form.docx` — standalone Word incident report form (HTDW-VAA-001-F1): 8 sections including management-use block, RIDDOR flag, Yes/No tick rows
  - Confirmed document reference abbreviation: **VAA** (not VAG) to avoid potential offence
  - Pending: fill in placeholders; brief all staff; distribute form; implement priority RA actions

- **Violence and Aggression Risk Assessment** (2026-02-27):
  - Created `Violence/CLAUDE.md` — project context documenting business profile, employee groups, key scenarios, outstanding actions, and regulatory references
  - Created `Violence/Violence_Aggression_Risk_Assessment.md` — full narrative risk assessment (5 scenarios, 15 actions, RIDDOR thresholds, post-incident support, training, review)
  - Created `Violence/Violence_Risk_Assessment.csv` — 15-row CSV for Excel/tracking use (one row per action, all risk data)
  - Highest-risk scenario: drivers as lone workers at customer sites (Medium, score 6); all others Low

- **Fire Door Compliance Analysis** (2026-02-26):
  - Created `Fire/ND_FireDoors.md` — comprehensive fire door compliance reference for 2-storey office building
  - Identified technical inaccuracy in fire risk assessor's report: BS 476-22 does not require cold smoke seals
  - Clarified that FD30S designation (fire + smoke) is required under ADB Table B1 for stairway/corridor escape route doors
  - Confirmed BS 476-22 validity until September 2029; BS EN 1634-1 mandatory for new installations from that date
  - Documented legal status of BS 8214:2016 (code of practice, not statute)
  - Pending: confirm door locations; challenge assessor wording; engage specialist for FD30S upgrade assessment

- **First Aid Needs Assessments** (2026-02-20):
  - Created `OfficeFirstAiders.md` — 1 Toft Green, York city centre office; recommends 2 EFAW first-aiders; L74 Table 1 checklist methodology with para-level citations; all legislation URLs verified
  - Created `FirstAidersIndEst.md` — industrial estate mixed warehouse/office site; recommends 1 FAW (warehouse) + 1 EFAW (office); higher-hazard classification due to FLT and articulated truck operations; 13-mile hospital distance drives FAW recommendation; includes emergency services notification section and lone working procedures
  - Full citation verification performed on both documents; two L74 para citations corrected in `OfficeFirstAiders.md`
  - Pending: commit files; arrange FAW training (warehouse) and EFAW training (2 people office, 1 person industrial estate office); notify ambulance service of industrial estate site; establish lone working check-in procedure
- **Employee Fire Evacuation Plan** (2026-02-05):
  - Created `FireEvacPlan.md` — employee-facing evacuation procedure grounded in HSWA 1974 s.7 and the Fire Safety Order 2005
  - 11 sections: alarm response, leaving the building, smoke/fire, helping others, assembly point, all-clear, common mistakes, training
  - User added Toft Green evacuation route diagram (`ToftGreenEvacPath.png`) to Section 3
  - No lift references (office has no lifts)
  - Pending: commit both evacuation plans; distribute to staff; schedule first drill
- **Fire Marshal Evacuation Plan** (2026-02-05):
  - Created `FireMarshalEvacPlan.md` — full evacuation procedure grounded in the Fire Safety Order 2005
  - Four roles: Fire Marshal, Nominated Person (grab bag), Nominated Person (sign-in boards), Head Fire Marshal
  - 7-phase procedure, special considerations (vulnerable persons, dangerous substances, visitors), training and record-keeping sections
  - All Fire Safety Order article links verified; Fire Safety Act 2021 linked
  - Pending: site-specific details (zone assignments, locations, named nominees), first training session, first drill
- **Noise Assessment — Regulation 6 clarification and restructure** (2026-02-04):
  - Verified Reg 6 two-tier structure against legislation.gov.uk: Reg 6(1) SFAIRP applies at all levels; Reg 6(2) programme only at upper AV
  - Resolved contradiction between Section 7 compliance summary and Section 7.2
  - User restructured legal duties (Section 2) into numbered lists with Reg 6(1)/6(2) distinction; expanded compliance position summary; added Regulation 10
  - Dosimetry recommendation section removed; sections renumbered 7.1–7.5
  - Placeholder fields filled (dates, site, assessor); site map image added
  - Weekly LEP,w calculated: 82.1 dB(A) for 4 days pattern + 1 day Area 9
- **Noise Assessment — Final timings and hyperlinks** (2026-02-04, commits 7ced2c5, 9c7496f):
  - Machine operator timings finalised; LEP,d settled at 83.1 dB(A) with 1.9 dB margin to upper AV
  - All regulation references hyperlinked to legislation.gov.uk; mislabelled Regulation 8 corrected to Regulation 5(6)
  - CLAUDE.md rule added: hyperlink and verify all regulation references before inserting
- **Noise Assessment — Initial Creation** (2026-02-03):
  - Created `assessments/NoiseAssessment.md` — full noise assessment from `NoiseLimits.csv` site measurements
  - 9 areas measured with Martindale SP79 Class 2 meter; dB(A) and dB(C) min/max recorded over 5-minute periods
  - Machine operates 4 hrs/day; two worker types assessed using equal energy (3 dB doubling) method
  - Area 5 worker: LEP,d 77.8 dB(A) — below lower action value; no specific duties triggered (confirmed robust to ±1.5 dB uncertainty)
  - Significant low-frequency propagation from Area 3 identified (dB(C)–dB(A) difference up to 20 dB in distant areas)
  - Compliance position summary included in recommendations section
- **Three-Year HSE Strategic Plan Update** (2026-01-19):
  - Created `3_Year_HSE_Plan.md` - updated 650+ line strategic plan with actual budget data
  - Incorporated `HSE5yrRecurringCosts.csv` data: £8,282 (2026), £3,942 (2027), £7,707 (2028)
  - Incorporated `HSE3yrObjectives.csv` timeline with quarterly/annual objectives
  - Total investment revised to £36,255 (from original £30k estimate)
  - Year 1 (2026): £24,606 - major waste water infrastructure investment (£15,824) + recurring compliance
  - Year 2 (2027): £3,942 - focus on embedding HSE into daily operations, driver H&S review
  - Year 3 (2028): £7,707 - occupational health assessment, HSE culture review, waste optimization
  - Hybrid structure: Quarterly breakdown for Year 1, annual objectives for Years 2-3
  - Retained full legal compliance framework from original plan
  - Internal time activities clearly marked as £0 throughout
- **Three-Year HSE Strategic Plan - Original Version** (2026-01-06):
  - Created `Three_Year_HSE_Strategic_Plan.md` (now renamed to `Three_Year_HSE_Strategic_Plan_old.md`)
  - Legal compliance framework with record retention requirements (2 years to permanent)
  - Competent person structure: NEBOSH internal lead + external specialist support
  - Created `Training_Matrix_Template.md` (600+ lines) and `Training_Matrix_Template.csv` (52 training records)
  - Templates include training requirements, individual tracking matrices, schedule planner, provider directory
  - Plan tailored to AdBlue manufacturing: 4 warehouse operatives, 4 office staff, forklift operations, chemical handling
- **Water Discharge Planning Documentation** (2025-12-15):
  - Created `water/TreatmentPlan.md` with comprehensive RO concentrate disposal planning
  - Calculated stream cross-sectional area (0.4608 m²) using trapezoidal rule
  - Determined stream flow rate: 2,019,168 L/day (~2.02 ML/day)
  - Researched Yorkshire Water Naburn STW facility details
  - Calculated interim disposal costs for 5-day working week: £46,650-£303,000 annually
  - Developed monthly cost tables for three production scenarios (10k, 30k, 50k L/day)
- **Critical Documentation Correction** (2025-12-04):
  - Corrected `Ladders/Emergency_Escape_Ladder.md` to accurately reflect Approved Document B section 3.28
  - Fixed ladders confirmed acceptable for plant rooms and areas not normally occupied
  - Updated all relevant sections throughout the document for consistency
  - Organized ladder files into `Ladders/` subfolder
- **Fixed Ladder Documentation Suite** (4 comprehensive documents):
  - `Ladders/Emergency_Escape_Ladder.md` - Complete regulatory guide for emergency-only ladders with RRF(SO) 2005 compliance
  - `Ladders/Fixed_Ladders_Compliance.md` - Work at Height Regulations 2005 compliance for work access ladders
  - `Ladders/Competent_Person_Fixed_Ladder_Inspections.md` - Inspector qualification and competence requirements
  - `Ladders/Ladder_Types.md` - HSE perspective on ladder classifications and legal definitions
- Multi-project directory structure with shared context files and commands
- Shared `/terminai/CLAUDE.md` for cross-project guidance
- Symlinked shared commands (end-session, sync-session) to all project folders
- Office health and safety best practices one-page guide with detailed legislative references
- HSE compliance advisor agent configuration
- Session documentation system validated (both `/sync-session` and `/end-session` tested successfully)
- Comprehensive legislative reference system established (regulation numbers, sections, HSE guidance docs)

## Blocked/Pending

None.

## Next Priorities

1. **Complete and Issue Violence and Aggression Documents**:
   - Fill in [FILL IN] placeholders in `HTDW_Violence.md` and `Violence_Aggression_Risk_Assessment.md`
   - Set target dates for all 15 RA actions
   - Implement priority actions: A4 (driver check-in), A5 (driver right to refuse delivery), A2 (abusive call procedure), A6c (lone worker on-site check-in), A9 (incident log)
   - Brief all staff using `HTDW_Violence.md`; distribute `Incident_Report_Form.docx`
2. **Resolve Fire Door Compliance**:
   - Confirm which doors are on protected escape routes (stairway enclosures, protected corridors)
   - Challenge fire risk assessment wording (stated basis is technically incorrect)
   - Engage fire door specialist to assess FD30S upgrade options for escape route doors
   - Implement fire door inspection regime with written records (RRO Article 17)
2. **Implement First Aid Assessments**:
   - Arrange EFAW training for 2 office staff (1 Toft Green)
   - Arrange FAW training for warehouse first-aider (industrial estate)
   - Arrange EFAW training for office first-aider (industrial estate)
   - Notify ambulance service in writing of industrial estate location and FLT/HGV hazards
   - Establish lone working check-in procedure for industrial estate warehouse
   - Complete accident history sections once records reviewed
2. **Implement Year 1 Q1 of Updated Three-Year HSE Plan** (Jan-Mar 2026):
   - Engage environmental consultant for waste water assessment
   - Implement training system using Training_Matrix_Template.md
   - ~~Develop fire evacuation plans~~ — completed: `FireMarshalEvacPlan.md` (marshal duties) and `FireEvacPlan.md` (all employees); commit both, distribute to staff, and schedule first drill
   - Complete WorkNest H&S Policy with consultancy support
   - Commission fire risk assessment for 1 Toft Green office
   - Complete Q1 PAT testing and fire extinguisher checks
   - Document all current waste streams and contractor arrangements
2. **Complete EA Water Discharge Permit Application**:
   - Obtain RO concentrate water quality test results
   - Calculate dilution ratios for various discharge rates
   - Complete Environment Agency discharge consent application form
   - Plan buffer tank and dosing pump system implementation
3. Create risk assessment templates for ladder use justification under WAHR Schedule 6
4. Develop general workplace risk assessment templates
5. Create COSHH assessment templates (to support Year 1 Q2 COSHH updates)
6. Create fire risk assessment template covering emergency escape routes
7. Develop DSE assessment forms
8. Design accident book and RIDDOR reporting templates
9. Create inspection checklist templates (weekly/monthly/quarterly as per Year 1 Q4 plan)

## Key Files & Structure

### Project Structure (hseea)
- `/Violence/` - Violence and aggression compliance documents
  - `CLAUDE.md` — Project context and business profile
  - `Violence_Aggression_Risk_Assessment.md` — Full narrative risk assessment (VA-RA-001; 5 scenarios, 15 actions)
  - `Violence_Risk_Assessment.csv` — CSV extract for Excel/tracking (15 rows, one per action)
  - `Violence_Policy_Excerpt.md` — Company policy source document
  - `HTDW_Violence.md` — Staff guide: How to Deal with Violence and Aggression (HTDW-VAA-001; 3 chapters)
  - `Incident_Report_Form.docx` — Standalone Word incident report form (HTDW-VAA-001-F1)
  - **Note**: Document reference abbreviation is **VAA** (Violence and Aggression), not VAG
- `/Fire/` - Fire safety documentation
  - `ND_FireDoors.md` - Fire door compliance analysis (BS 476-22, FD30S, cold smoke seals, ADB Table B1)
- `OfficeFirstAiders.md` - First aid needs assessment, 1 Toft Green York city centre office (L74 framework)
- `FirstAidersIndEst.md` - First aid needs assessment, industrial estate mixed site (L74 framework)
- `FireEvacPlan.md` - Employee fire evacuation procedure (HSWA 1974 s.7 + Fire Safety Order 2005)
- `FireMarshalEvacPlan.md` - Fire marshal evacuation procedure (Fire Safety Order 2005 compliant)
- `/water/` - Water discharge and treatment planning documentation (EA permit applications)
- `/Ladders/` - Fixed ladder compliance documentation suite (emergency escape and work access)
- `/regulations/` - Official regulatory documents and legal requirements
- `/guidance/` - HSE guidance series (HSG), INDG publications, best practice guides
- `/assessments/` - Risk assessments, COSHH assessments, environmental assessments
- `/compliance/` - Compliance checklists, audit records, monitoring documentation
- `/procedures/` - SOPs, safe systems of work, emergency procedures
- `/reference/` - Quick reference materials, summaries, decision tools
- `CLAUDE.md` - HSE/EA-specific instructions for Claude Code
- `SESSION_LOG.md` - Chronological record of all work sessions
- `PROJECT_STATUS.md` - Current project state and priorities (this file)
- `CHANGELOG.md` - Version-style changelog of all modifications

### Shared Structure (parent /terminai)
- `/terminai/CLAUDE.md` - Shared context for all projects
- `/terminai/.claude/commands/` - Shared slash commands available to all projects
- `/terminai/GEMINI.md` - Shared Gemini context (placeholder)
