# Changelog

All notable changes to this HSE/EA compliance repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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
