# REACH Project - Changelog

All notable changes to this project will be documented in this file.

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
