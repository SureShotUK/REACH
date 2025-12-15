# Changelog

All notable changes to this HSE/EA compliance repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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
