# IT Troubleshooting & Helpdesk

Welcome to the IT Troubleshooting knowledge base. This directory contains documented issue resolutions, diagnostic guides, and helpdesk resources.

## Quick Start

When you encounter an IT issue:
1. Gather complete information about the problem (symptoms, error messages, timeline)
2. Use Claude Code in this directory - it will follow systematic troubleshooting methodology
3. The `gemini-it-helpdesk-researcher` agent will research matching issues online
4. Document the resolution for future reference

## Standard Environment

All troubleshooting assumes:
- **OS**: Windows 11 (latest updates)
- **Identity**: Azure AD domain-joined user accounts
- **Office**: Microsoft 365 Desktop Apps (Excel, Outlook, Word, PowerPoint, Teams, OneDrive)
- **Network**: Corporate/business network with VPN access

## Issue Categories

Issues are organized by category:

### Microsoft 365 / Office Issues
- `/office-apps/` - Excel, Word, PowerPoint application issues
- `/outlook/` - Email, calendar, and Outlook-specific issues
- `/onedrive/` - Sync, storage, and file access issues
- `/teams/` - Messaging, calls, and collaboration issues
- `/activation/` - Licensing and activation problems

### Windows Issues
- `/windows-update/` - Update failures and post-update problems
- `/performance/` - Slow performance, high CPU/memory usage
- `/stability/` - Crashes, freezes, and BSOD issues
- `/startup/` - Boot issues and startup problems

### Network & Connectivity
- `/network/` - General network connectivity issues
- `/vpn/` - VPN connection and performance issues
- `/dns/` - DNS resolution and configuration issues
- `/firewall/` - Firewall blocking and configuration

### Azure AD / Authentication
- `/azure-ad/` - Domain join, device registration issues
- `/authentication/` - Sign-in failures, MFA problems
- `/credentials/` - Password, token, and credential issues

## Documentation Template

When documenting a resolved issue, use this structure:

```
# [Issue Title] - [YYYY-MM-DD]

## Issue Summary
[Brief description]

## Environment
- OS: Windows 11 [Build]
- Office Version: [Version]
- Affected User(s): [Username/Multiple]
- Affected Application: [App and version]

## Symptoms
- [Exact error messages]
- [Observable behavior]
- [Frequency]

## Root Cause
[What was causing the issue]

## Resolution Steps
1. [Step-by-step actions]
2. [Commands executed]
3. [Settings changed]

## Verification
[How fix was verified]

## Prevention
[Steps to prevent recurrence]

## Related Information
- KB Articles: [Numbers]
- Related Issues: [Links]
- References: [Microsoft docs]
```

## Issue Index

### Recently Resolved Issues

#### Outlook / Email Issues

**[Outlook Template Unicode Encoding - Question Marks in Sent Emails](Outlook_Template_Unicode_Encoding_Question_Marks.md)** - 2026-02-13
- **Issue**: Five invisible control characters (¬) in .oft templates cause question marks in sent emails and corrupt special characters (£, ®, ©)
- **Severity**: P2 - High (affects all email recipients, business communication impact)
- **Affected**: Outlook 365 Build 19628.20150+ (Current Channel)
- **Root Cause**: Microsoft bug in encoding detection + legacy control characters in .oft templates
- **Status**: Workaround available (PowerShell script + manual cleaning)
- **Solution**: Automated PowerShell cleaning script removes control characters and sets UTF-8 encoding
- **Related Files**:
  - Documentation: `Outlook_Template_Unicode_Encoding_Question_Marks.md`
  - PowerShell Script: `Clean-OutlookTemplates.ps1`
- **Keywords**: outlook, template, .oft, question marks, ?????, encoding, UTF-8, £, special characters, unicode

---

**Getting Help**: Use Claude Code in this directory for systematic troubleshooting assistance and access to the gemini-it-helpdesk-researcher agent.
