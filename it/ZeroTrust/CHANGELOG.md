# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]

### Planning
- Environment discovery phase
- Solution research and evaluation
- Architecture design

---

## [0.1.0] - 2026-02-17

### Added
- Initial project structure and documentation
- `CLAUDE.md` - Comprehensive project-specific guidance
  - Environment details (SonicWall TZ 270, Ubuntu 24.04, PostgreSQL, Azure AD)
  - Six solution architecture candidates identified
  - Zero Trust principles documentation
  - Technical, security, and operational requirements
  - User preferences and documentation standards
- `README.md` - Quick start guide and project overview
  - Problem statement and Zero Trust approach
  - Solution candidates comparison table
  - Getting started instructions
  - Authoritative resource links
- `PROJECT_STATUS.md` - Six-phase project timeline and status tracking
  - Phase 1 (Environment Discovery) as current phase
  - Risk register
  - Success criteria
  - Open questions for clarification
- `SESSION_LOG.md` - Session tracking and work history
- `CHANGELOG.md` - This file for version tracking
- `.claude/agents/gemini-it-security-researcher.agent` - Security research agent configuration

### Project Scope Defined
- **Goal**: Enable secure remote PostgreSQL database access without opening firewall ports
- **Approach**: Zero Trust Network Access (ZTNA) with Azure AD authentication
- **Constraints**: No inbound firewall ports, leverage existing M365 Business Premium

### Environment Documented
- SonicWall TZ 270 firewall (perimeter)
- Ubuntu 24.04 LTS with PostgreSQL (database server)
- Azure AD-joined Windows 11 laptops (clients)
- Security stack: ThreatLocker, Huntress, Microsoft Defender for Business
- Microsoft 365 Business Premium licensing

### Solution Candidates Identified
1. Azure AD Application Proxy + Private Endpoint
2. Cloudflare Tunnel (Zero Trust)
3. Tailscale (WireGuard-based mesh VPN)
4. Twingate (ZTNA Platform)
5. Custom SSH Bastion + Azure AD authentication
6. Azure Private Link + VPN Gateway

---

## Template for Future Releases

## [X.Y.Z] - YYYY-MM-DD

### Added
- New features, documentation, or capabilities

### Changed
- Changes to existing functionality or documentation

### Deprecated
- Features or approaches that are being phased out

### Removed
- Features or documentation that have been removed

### Fixed
- Bug fixes or corrections

### Security
- Security-related changes or discoveries
