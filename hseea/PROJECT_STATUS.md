# Project Status

**Last Updated**: 2026-01-19

## Current State

This HSE/EA compliance knowledge repository is actively being populated with practical compliance guidance and reference materials. Session documentation system is working successfully. Major documentation achievements include office HSE best practices guide, a comprehensive four-document ladder compliance suite, water discharge permit planning documentation, and an updated three-year HSE strategic plan with realistic budget allocations.

**Recent Update**: Created updated three-year HSE strategic plan (`3_Year_HSE_Plan.md`) incorporating actual recurring costs from CSV budget data and specific quarterly/annual objectives. Total investment now £36,255 over 3 years (£24.6k Year 1, £3.9k Year 2, £7.7k Year 3), with Year 1 including major waste water infrastructure project (£15,824). Plan uses hybrid structure: detailed quarterly breakdown for Year 1 foundation building, then annual objectives for Years 2-3 operational phase.

**Infrastructure Update**: The repository structure has been reorganized to support multiple projects (hseea and it) with shared context files and slash commands at the parent `/terminai/` level. This allows for better organization and reuse of common tooling across different project domains.

## Active Work Areas

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

1. **Implement Year 1 Q1 of Updated Three-Year HSE Plan** (Jan-Mar 2026):
   - Engage environmental consultant for waste water assessment
   - Implement training system using Training_Matrix_Template.md
   - Develop fire marshal evacuation plan
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
