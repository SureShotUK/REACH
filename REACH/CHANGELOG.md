# REACH Project - Changelog

All notable changes to this project will be documented in this file.

---

## [Unreleased] - 2025-12-22

### Added
- **UpdateDec25.md**: Comprehensive government announcement analysis and timeline update
  - Government selection of Option 1 (3-year extension with 1-year gaps)
  - New registration deadlines: 27 Oct 2029, 27 Oct 2030, 27 Oct 2031
  - ATRm 2-year transition period constraint analysis
  - Consultant discussions summary (CS Regulatory, TT Environmental, Valpak)
  - Three-phase action plan (2025-2027 monitoring, 2027 ATRm publication, 2027-2029 implementation)
  - Operational planning guidance for waiting period and implementation phases

### Changed
- **PROJECT_STATUS.md**: Updated for 3-year deadline extension and ATRm waiting period
  - New deadline: 27 October 2029 (was 27 October 2026)
  - Current phase: Monitoring (no action required until ATRm published)
  - All registration work now on hold pending ATRm framework
  - Active work areas shifted to passive monitoring activities
  - Next priorities restructured into three time-phased sections
- **SESSION_LOG.md**: Added comprehensive session entry for government announcement analysis

### Documentation
- Analyzed 28-page government consultation response document (LegislationUpdate22ndDec2025.pdf)
- Identified critical timeline constraint: ATRm must be published by October 2027 for 2-year transition
- Documented consultant preference order with rationale (CS Regulatory preferred due to Valpak subcontracting)
- Established waiting period: ~22 months (December 2025 → October 2027) before registration work can begin
- Clarified government rationale for Option 1 despite 70% industry preference for Option 2
- Noted timeline risk: 2-week HSE approval assumes error-free submission (recommend 3-4 month buffer)

### Strategic Impact
- **Urgency shift**: From 11-month compliance crisis to ~22-month passive monitoring phase
- **Work blockage**: All registration preparation work blocked until ATRm framework published
- **Cost deferral**: No consultant fees or registration costs needed until late 2027
- **Framework uncertainty**: ATRm designed to reduce costs vs. EU REACH, but requirements unknown
- **Business operations**: Continue importing Urea normally under transitional provisions until 2027

---

## [Unreleased] - 2025-12-15

### Added
- **IUCLID_Inquiry_Dossier_Guide.md**: Comprehensive 14-part guide for creating Article 26 Inquiry dossiers in IUCLID 6 Cloud
  - Complete workflow from prerequisites to HSE submission
  - Troubleshooting section identifying 3 root causes for "Create Dossier" unavailable issue
  - Quick reference checklist (13 sections) for tracking progress
  - All official HSE and ECHA resource links with verification
  - Clarification of two-system architecture (IUCLID vs Comply with UK REACH)

### Documentation
- Researched latest IUCLID 6 Cloud and GB REACH Article 26 Inquiry requirements using gemini-researcher agent
- Identified minimum IUCLID sections required for inquiry: 1.1 (Identification), 1.2 (Composition), 1.4 (Analytical Information)
- Documented ECHA trial account vs. full subscription capabilities and limitations
- Clarified Legal Entity setup as critical prerequisite (most common blocker)
- Documented Working Context configuration requirement (often missed)

### Progress
- User completed DUIN registration with REACH UK
- User created IUCLID 6 Cloud trial account
- User added Urea substance to IUCLID (experiencing "Create Dossier" unavailable issue)
- Project advanced from initial research phase to active inquiry dossier creation phase

---

## [Previous] - 2025-11-07

### Fixed
- **CRITICAL**: Corrected import volume from ">100 tonnes/year" to "1000+ tonnes/year" across all core documentation
- **CRITICAL**: Corrected registration deadline from "October 27, 2025" to "27th October 2026" across all core documentation
- Updated compliance urgency messaging from "deadline passed/currently illegal" to "deadline approaching/11 months remaining"
- Corrected legal status explanations to clarify breach occurs AFTER deadline, not currently
- Updated HSE registration fee estimates for 1000+ tonnes/year band (£6,000-£30,000 vs. £2,222)
- Revised immediate action priorities to reflect available timeline
- Updated cost calculation examples to reflect 1000+ tonne volumes (£30k/year GB supplier premium vs. £4.5k/year)

### Changed
- `REACH/CLAUDE.md` - Updated project context with correct volume and deadline
- `REACH/Consultants/Ranked_Consultants.md` - Removed "deadline missed" urgent notice, updated all deadline references
- `REACH/reports/compliance_assessment_urgent.md` - Corrected volume, deadline, status, and immediate actions
- `REACH/costs/cost_estimates.md` - Updated volume, cost examples, and HSE fee band
- `REACH/README.md` - Corrected volume, deadline, and compliance status throughout

### Documentation
- Created `SESSION_LOG.md` - Session tracking for project
- Created `PROJECT_STATUS.md` - Current project status and parameters
- Created `CHANGELOG.md` - This file

---

## [Previous Work] - 2025-11-05 to 2025-11-07

### Added
- Comprehensive UK REACH research and overview (`research/uk_reach_overview.md`)
- Urgent compliance assessment report (`reports/compliance_assessment_urgent.md`)
- Detailed cost estimates for all compliance options (`costs/cost_estimates.md`)
- Consultant ranking document covering 50 UK REACH consultants (`Consultants/Ranked_Consultants.md`)
- Three compliance template documents:
  - `templates/supplier_registration_verification.md` - GB supplier verification template
  - `templates/hse_disclosure_template.md` - HSE contact/disclosure template
  - `templates/downstream_user_compliance_checklist.md` - Ongoing compliance tracking
- DUIN eligibility research and application guidance (`DUIN_Application/` folder)
- UK REACH registration requirements one-pager (`OnePager.md`)
- Letter of Access (LoA) and Only Representative (OR) explanations
- Critical distinction between UK REACH and EU REACH documentation

### Research Completed
- UK REACH legal requirements and obligations
- Tonnage band requirements and data packages
- Registration deadlines and transitional provisions
- Enforcement and penalties under UK law
- Urea-specific information and consortium opportunities
- Three compliance pathway options (GB supplier, own registration, supplier OR)
- Cost-benefit analysis of each pathway
- Consultant landscape and recommendations

### Strategic Recommendations
- **Primary recommendation**: Switch to GB supplier (fastest, 2-8 weeks to compliance)
- **Alternative**: Own UK REACH registration (12-24 months, £65k-£345k upfront)
- **Alternative**: Supplier appoints Only Representative (12-24 months, variable cost)
- Top 3 recommended consultants: SGS, Intertek, Ecomundo

---

## Notes on Data Correction (2025-11-07)

The 2025-11-07 session identified and corrected fundamental errors in project parameters that had cascaded through all documentation:

**Incorrect Parameters (all previous work)**:
- Volume: ">100 tonnes/year"
- Deadline: "October 27, 2025"
- Status: "Deadline passed, currently illegal"

**Correct Parameters (as of 2025-11-07)**:
- Volume: "1000+ tonnes/year"
- Deadline: "27th October 2026"
- Status: "11 months to deadline, must begin compliance process"

This correction significantly impacts:
1. **Urgency**: From crisis (past deadline) to urgent-but-manageable (11 months remaining)
2. **Costs**: Higher volumes mean larger GB supplier premium (£30k/year vs. £4.5k/year)
3. **Data requirements**: 1000+ tonnes requires most comprehensive Annex package
4. **HSE fees**: Higher tonnage band fees (£6k-£30k vs. £2,222)
5. **Strategic calculus**: At 1000+ tonnes, own registration may be more economically viable

All future work uses the corrected parameters. Some files may still contain old parameters and require review/correction.
