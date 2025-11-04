# Changelog

All notable changes to the terminai repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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
