# Changelog

All notable changes to the terminai repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [Unreleased] - 2025-10-31

### Added
- **IT Project - Security Documentation Suite**:
  - `it/.claude/agents/gemini-it-security-researcher.md` - IT security research agent with verification methodology
  - `it/Public_WIFI_Best_Practices.md` - Comprehensive 11,000-word public WiFi security guide for non-technical users
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
- `it/CLAUDE.md` - Added reference to new `gemini-it-security-researcher` agent
- `it/Public_WIFI_Checklist.md` - User edited to adjust VPN from critical to recommended priority

### Documentation
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
