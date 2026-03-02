# IT Project Status

**Last Updated**: 2026-03-02

## Current State
**AI PC is built and Ubuntu 24.04 Server is running.** First boot troubleshooting complete: internet working via WiFi (DNS fixed), ethernet driver installed (RTL8126 DKMS), static IPs configured (ethernet 192.168.1.192, WiFi 192.168.1.191 fallback), Secure Boot disabled. **Next immediate steps**: NVIDIA driver installation, Tailscale remote access setup, then Ollama + Open WebUI. IT troubleshooting system validated. ZTNA evaluation ready for pilot deployment.

## Active Work Areas

### IT Troubleshooting & Helpdesk - Proven System
- **Status**: System validated with successful issue resolution and enhanced diagnostic tooling
- **Files**: `troubleshooting/CLAUDE.md`, `troubleshooting/README.md`, `.claude/agents/gemini-it-helpdesk-researcher.md`
- **Coverage**: Systematic 7-step diagnostic framework, Windows 11/Azure AD/M365 diagnostic commands, issue documentation standards
- **Methodology**: Information gathering → Research (via Gemini agent) → Hypothesis → Testing → Resolution → Documentation
- **Target Environment**: Windows 11 (latest updates), Azure AD domain-joined, Microsoft 365 desktop apps (Excel, Outlook, Word, etc.)
- **Key Features**:
  - Gemini-powered research agent for matching issue prevalence and verified solutions
  - Priority levels (P1-P4) with escalation criteria
  - PowerShell diagnostic commands for all common issue types
  - Documentation templates for building knowledge base
  - Communication standards for clear, helpful user support
  - Enhanced diagnostic tooling for character encoding issues
  - File lock detection and resolution utilities
- **Ongoing: Outlook Template Encoding Issue** (Enhanced understanding):
  - **Root cause deepened**: UTF-16 Little Endian encoding + soft hyphens (U+00AD) + RTF metadata issues
  - **Key discoveries**:
    - NUL-after-every-character is NORMAL UTF-16 LE behavior (not corruption)
    - "NUL-NUL-NUL-NUL-NUL" pattern = five soft hyphens in UTF-16 LE misread as ANSI
    - Notepad++ Replace fails because it searches binary CFBF format, not parsed text
    - File locks after COM automation are expected Windows behavior
    - £ symbol issue is separate from soft hyphen issue (RTF encoding metadata)
  - **Enhanced tooling created** (2026-02-16):
    - `Diagnose-OutlookTemplate.ps1` (6.8KB): Character diagnostic with code point analysis
    - `Clean-OutlookTemplateEncoding.ps1` (9.6KB): Enhanced encoding-focused cleaning
    - `Test-TemplateFileLock.ps1` (7.5KB): File lock detection and unlock utility
    - `Outlook_Template_Encoding_Issues.md` (14KB): Comprehensive technical documentation
  - **Previous tooling** (2026-02-13):
    - `Clean-OutlookTemplates.ps1` (15KB): Original automation script
    - `Outlook_Template_Unicode_Encoding_Question_Marks.md` (26KB): Initial documentation
    - `SCRIPT_USAGE_GUIDE.md` (11KB): Step-by-step usage guide
  - **Current status**: Scripts ran successfully, template cleaned, file lock encountered (expected), user restarting computer
  - **Iterative debugging success**: 5 script iterations (syntax → type → logic → system behavior → file locks)
- **Knowledge Base**: Issue index in troubleshooting/README.md with comprehensive documentation references
- **Next**: Test cleaned template after restart, verify £ symbol works, update issue index with enhanced documentation

### AI PC Build for Local LLM Inference - UBUNTU RUNNING ✅
- **Status**: Built and running Ubuntu 24.04 Server. First boot issues resolved. NVIDIA drivers and AI stack not yet installed.
- **Files**: `NewPC/CLAUDE.md`, `NewPC/PCBuildResearch.md`, `NewPC/Chosen_Build.md`, `NewPC/Final_Build.md`, `NewPC/Assembly_Guide.md`, `NewPC/Software_Setup.md`
- **Purpose**: Desktop for local LLM inference (coding assistance + homework help), dual GPU upgrade path
- **Complete Component List**:
  - CPU: AMD Ryzen 9 7900X + thermal paste @ £322.50
  - Motherboard: MSI MAG X870E TOMAHAWK WIFI @ £269.99
  - GPU: Asus TUF Gaming OC RTX 3090 24GB @ £699.39
  - RAM: G.SKILL Trident Z5 Neo RGB 64GB DDR5-6000 CL30 @ £599.99
  - PSU: Super Flower Leadex Titanium 1600W @ £270.98 (replacement — Thermaltake DOA)
  - Case: Fractal Design Torrent @ £169.99
  - Storage: Samsung 9100 Pro 2TB PCIe 5 x2 @ £502.00
  - CPU Cooler: Arctic Liquid Freezer III Pro 360 @ £72.00
  - Rear Fan: 140mm exhaust @ £21.12
- **Network Config** (resolved 2026-03-02):
  - Ethernet: `enp7s0` static 192.168.1.192/24, route metric 100 (primary)
  - WiFi: `wlp8s0` static 192.168.1.191/24, route metric 600 (fallback)
  - DNS: 8.8.8.8, 1.1.1.1 on both interfaces
  - Ethernet driver: `realtek-r8126-dkms` 10.016.00 via PPA (kernel 6.8 doesn't include RTL8126 in r8169)
  - Secure Boot: disabled (required for unsigned DKMS modules)
  - cloud-init network config: disabled (prevents netplan changes being overwritten)
- **Known Issues**: Ethernet link flapping (enp7s0 up/down) — likely autoneg issue between RTL8126 5Gb NIC and 1Gb router. Fix: `ethtool -s enp7s0 speed 1000 duplex full autoneg off` + disable EEE
- **Next**: Install NVIDIA drivers → CUDA → Tailscale → Ollama + Open WebUI → Benchmark

### Zero Trust Network Access (ZTNA)
- **Status**: Research complete, deployment guides created, ready for pilot deployment
- **Files**: `ZTNA_Provider_Research_2026.md`, `Tailscale_Hybrid_Deployment_Guide.md`, `IPSec_SonicWall_Deployment_Guide.md`
- **Coverage**: Complete market research (6 providers), two deployment approaches, PostgreSQL ODBC, RDP, Azure AD SSO
- **Environment**: 35 hybrid workers across 3 offices (SonicWall TZ270 x2, Draytek router x1)
- **Primary recommendation**: Tailscale ($2,520/year) for "stupid simple" hybrid worker experience
- **Budget alternative**: IPSec + SonicWall Mobile Connect ($0-500/year)
- **Next**: Get SonicWall quote, pilot Tailscale free tier at Office3, performance testing, user feedback

### Financial Data Processing
- **Status**: Parser extended with Cash Settlement support, ready for testing; CLAUDE.md updated to .NET 10/C# 14
- **Files**: `parsing/DailyStatementParser.cs`, `parsing/Program.cs`, `parsing/GetStoneXOTCDailyValuesConsole.csproj`
- **Coverage**: StoneX daily statement PDF parsing, cash settlement extraction, trade data extraction, account information processing
- **Development environment**: .NET 10 (LTS release, supported until November 2028), C# 14 with extension members and field keyword
- **Recent additions**: Cash Settlement section parsing with field mappings (Cash Amount → MarketValue, Settlement Price → MarketPrice); CLAUDE.md updated to reflect .NET 10/C# 14 preference
- **Next**: Test cash settlement parsing, database integration, production error handling, unit tests

### VPN Security (Recently Completed)
- **Status**: Major expansion completed
- **Files**: `VPN_Benefits.md`, `VPN_Comparisons.md`
- **Coverage**: Commercial VPN security analysis, provider comparison, public WiFi protection

### Public WiFi Security
- **Status**: Ongoing documentation
- **Files**: `WIFI_Best_Practices_for_Laptops_and_Mobiles.md`, `Mobile_Laptop_WIFI_Summary.md`
- **Recent additions**: Mobile/laptop checklists, security practices

### Network Infrastructure
- **Status**: Initial documentation
- **Files**: `Draytek_Connect.md`, `L2TP_over_IPsec.md`
- **Coverage**: VPN protocols, router configuration, Draytek-specific guides

### Virtualization Documentation
- **Status**: Foundation established
- **Files**: `virtual_virus_test.md`, `virtual_machine_types.md`, `type1_hypervisors.md`, `mac_on_windows.md`
- **Coverage**: VM security, hypervisor comparison, homelab setup

## Recently Completed

### Session 2026-02-19 (16:00) - Assembly and Software Setup Guides Created
- ✅ Created `NewPC/Assembly_Guide.md` — complete step-by-step hardware assembly guide for all 10 components
- ✅ Created `NewPC/Software_Setup.md` — full Ubuntu 24.04 AI software stack setup guide
- ✅ Assembly guide covers: CPU/RAM/M.2 installation order, Arctic 360mm AIO front-intake config, RTX 3090 GPU, cable management, first boot, BIOS (EXPO/Above4G/ResizableBAR)
- ✅ Software guide covers: Ubuntu install with 2-drive partitioning, NVIDIA drivers, CUDA, Docker+NVIDIA toolkit, Ollama (model store on drive 2), Open WebUI, performance tuning, firewall
- ✅ Both guides tailored to exact components purchased (MSI X870E, Fractal Torrent, Arctic LF III Pro, RTX 3090)

### Session 2026-02-19 (14:00) - AI PC Build COMPLETE
- ✅ Purchased all remaining 7 components — build 100% complete (£2,874.98 total)
- ✅ CPU + thermal paste @ £322.50, PSU @ £218.00 (£22 under estimate), Case @ £169.99 (£5 under estimate)
- ✅ Storage: 2x Samsung 9100 Pro 2TB PCIe 5.0 @ £502.00 (Gen5 at Gen4 price — spotted parity on Samsung direct)
- ✅ CPU Cooler: Arctic Liquid Freezer III Pro 360 @ £72.00 (Pro outperforms standard III, cheaper than most 360mm AIOs)
- ✅ Rear Fan: 140mm exhaust @ £21.12 (fills only empty mount in case)
- ✅ Key technical decisions: NAS backup over RAID 1, Gen5 storage free upgrade, 360mm AIO front-mounted (no top mount on Torrent), different RTX 3090 makes OK for dual GPU
- ✅ Final_Build.md updated to v1.6 — complete build record with all purchases and rationale

### Session 2026-02-16 (12:30) - Enhanced Outlook Template Diagnostics & File Lock Handling
- ✅ Deepened root cause understanding: UTF-16 LE encoding behavior, CFBF binary format, why Replace fails
- ✅ Created diagnostic script `Diagnose-OutlookTemplate.ps1` with character code analysis (6.8KB, 190 lines)
- ✅ Created enhanced cleaning script `Clean-OutlookTemplateEncoding.ps1` focused on encoding issues (9.6KB, 290 lines)
- ✅ Created comprehensive documentation `Outlook_Template_Encoding_Issues.md` explaining binary format behavior (14KB, 600+ lines)
- ✅ Created file lock diagnostic utility `Test-TemplateFileLock.ps1` for troubleshooting COM automation locks (7.5KB, 240 lines)
- ✅ Confirmed NUL-after-every-character is normal UTF-16 LE encoding (not corruption)
- ✅ Explained why "NUL-NUL-NUL-NUL-NUL" = five soft hyphens in UTF-16 LE misread as ANSI
- ✅ Documented why Notepad++ Replace doesn't work (searches binary CFBF, not parsed RTF text)
- ✅ Identified £ symbol issue as separate from soft hyphen problem (RTF metadata / codepage)
- ✅ Documented file lock behavior as expected Windows COM automation pattern (not script bug)
- ✅ User successfully ran scripts, template cleaned, encountered expected file lock (requires restart)
- ✅ Created 4 solution options for file lock resolution (restart, kill processes, backup, copy)
- ✅ Enhanced diagnostic approach: inspect first (Diagnose), then act (Clean), then troubleshoot (Test lock)

### Session 2026-02-13 (16:00) - First Issue Resolution: Outlook Template Unicode Bug
- ✅ Diagnosed Microsoft Outlook template question mark issue using 7-step troubleshooting framework
- ✅ Identified root cause: Soft hyphen characters (U+00AD) triggering Build 19628.20150+ encoding bug
- ✅ Created PowerShell automation script `Clean-OutlookTemplates.ps1` (376 lines, 15KB)
- ✅ Created comprehensive technical documentation (26KB with root cause analysis, solutions, prevention)
- ✅ Created step-by-step usage guide for PowerShell script (11KB)
- ✅ Fixed 3 script bugs through iterative testing (Verbose parameter, string replacement, character detection)
- ✅ Successfully removed 5 soft hyphens from template and applied UTF-8 registry fixes
- ✅ Updated troubleshooting/README.md with first resolved issue entry
- ✅ Documented encoding cascade effect and CFBF binary format limitations
- ✅ Validated troubleshooting system with real-world problem resolution

### Session 2026-02-13 (15:00) - IT Troubleshooting & Helpdesk System
- ✅ Created troubleshooting/ directory for IT helpdesk knowledge base
- ✅ Created comprehensive CLAUDE.md with 7-step diagnostic framework (12.4KB)
- ✅ Created README.md with quick start guide and documentation templates
- ✅ Created gemini-it-helpdesk-researcher agent for internet-wide issue research
- ✅ Defined systematic troubleshooting methodology (Intake → Diagnosis → Research → Resolution)
- ✅ Documented PowerShell diagnostic commands for Windows 11, Office, Azure AD, and network
- ✅ Established issue documentation standards and priority levels
- ✅ Created research agent output format with prevalence assessment and verified solutions

### Session 2026-02-12 (22:30) - AI PC Build Planning
- ✅ Created NewPC project directory with comprehensive CLAUDE.md guidance
- ✅ Deployed gemini-researcher agent for AI PC hardware market research
- ✅ Created PCBuildResearch.md with 5 complete build configurations and GPU benchmarks
- ✅ Created Chosen_Build.md with deep PCIe architecture analysis (x16/x8 vs x16/x16 reality check)
- ✅ Created Final_Build.md component selection tracker with decision log
- ✅ Confirmed 5 core components: CPU, motherboard, RAM, PSU, case (all UK pricing)
- ✅ Updated all pricing from USD to GBP with 20% VAT for UK market
- ✅ Discovered UK RAM pricing reality (£599 vs initial £250-280 estimate)
- ✅ Evaluated eBay UK listings for RTX 3090 24GB graphics cards
- ✅ Budget revised from £1,500-1,800 to £2,200-2,400 based on UK market research

### Session 2026-02-12 (19:30) - .NET Version Update
- ✅ Updated CLAUDE.md to reflect .NET 10 and C# 14 preferences
- ✅ Verified current .NET 10 version (10.0.3, LTS until November 2028)
- ✅ Verified current C# 14 features (extension members, field keyword, enhanced lambdas)

### Session 2026-02-06
- ✅ Comprehensive ZTNA market research with 6 provider evaluations
- ✅ Complete Tailscale hybrid deployment guide (58 pages)
- ✅ Complete IPSec + SonicWall Mobile Connect deployment guide (53 pages)
- ✅ PostgreSQL ODBC configuration for both solutions
- ✅ RDP configuration and optimization guides
- ✅ Azure AD SSO integration documentation
- ✅ Performance analysis (P2P mesh vs relay vs traditional VPN)
- ✅ Cost-benefit analysis ($0-2,520/year range)
- ✅ User experience optimization for hybrid workers
- ✅ Comprehensive troubleshooting sections for both approaches

### Session 2026-01-15
- ✅ Technical guidance for PdfSharp MemoryStream handling
- ✅ Diagnosed PdfDocumentOpenMode.Import limitation (prevents save operations)
- ✅ Provided direct file read solution using File.ReadAllBytes()
- ✅ Explained appropriate PdfDocumentOpenMode values for different operations
- ✅ User successfully implemented PDF loading into MemoryStream

### Session 2025-12-12
- ✅ Technical guidance for one-hour retry mechanism implementation
- ✅ C# Task.Delay() solution design with ManualResetEventSlim for process lifecycle
- ✅ Recursive retry pattern for unlimited retry attempts
- ✅ Minimal resource usage approach while waiting

### Session 2025-12-10
- ✅ Cash Settlement section parsing for StoneX daily statements
- ✅ Field mapping implementation (Cash Amount → MarketValue, Settlement Price → MarketPrice)
- ✅ Two-line cash settlement format handling (data row + confirmation row)
- ✅ Integration into main parser workflow
- ✅ Consistent pattern with existing trade section parsing

### Session 2025-12-09
- ✅ Complete C# parser for StoneX daily statement PDFs
- ✅ Date parsing with multiple format support (dd-MMM-yyyy, dd/MM/yyyy, etc.)
- ✅ Currency parsing with negative values in parentheses
- ✅ Multi-page document handling across page breaks
- ✅ Trade deduplication logic (TradeId + StartDate + EndDate)
- ✅ Demo console application with comprehensive output
- ✅ Robust error handling and field parsing

### Session 2025-11-19
- ✅ Comprehensive VPN security research (NIST, CISA, NSA, SANS, OWASP guidance)
- ✅ Commercial VPN provider comparison (8 major providers)
- ✅ Security audit verification (Deloitte, KPMG, Securitum, Cure53)
- ✅ Jurisdiction analysis (Five/Nine/Fourteen Eyes implications)
- ✅ Pricing and feature comparison matrices
- ✅ Provider recommendations by use case

### Earlier Sessions
- ✅ Cross-platform WiFi security documentation
- ✅ VPN configuration guides (Draytek routers)
- ✅ L2TP over IPsec protocol documentation
- ✅ Virtual machine security and isolation guides
- ✅ Homelab Type 1 hypervisor setup guides

## Blocked/Pending
None currently. All active documentation areas progressing as planned.

## Next Priorities

### High Priority
1. **NVIDIA driver installation** - `sudo ubuntu-drivers autoinstall` → reboot → `nvidia-smi`
2. **CUDA toolkit** - `sudo apt install nvidia-cuda-toolkit` → `nvcc --version`
3. **Tailscale remote access** - Create account, install on server + Windows machine
4. **Fix ethernet link flapping** - Force 1Gb autoneg off, disable EEE, make permanent in netplan
5. **AI software stack** - Follow `NewPC/Software_Setup.md` — Ollama, Open WebUI, first model benchmark
3. **ZTNA pilot deployment** - Start Tailscale free tier testing at Office3 (3 users)
2. **Get SonicWall Cloud Secure Edge quote** - Pricing for 35 users with TZ270 Gen 7+ firewalls
3. **PostgreSQL ODBC performance testing** - Validate query performance over ZTNA mesh
4. **RDP connectivity testing** - Verify automated scheduled tasks work over ZTNA
5. **User feedback collection** - Gather hybrid worker experience feedback (office vs remote)
6. **Test cash settlement parsing** - Verify parsing works with real cash settlement data
7. **Database integration for StoneX parser** - Implement database update logic for both trades and cash settlements

### Medium Priority
5. Continue expanding public WiFi security best practices
6. Add practical VPN configuration examples
7. Router security hardening guides
8. VPN leak testing procedures (DNS, WebRTC, IPv6)

### Low Priority
9. VPN protocol deep-dive technical documentation
8. Enterprise vs consumer VPN comparison
9. Mobile device management for security
10. Container security documentation

## Key Files & Structure

### IT Troubleshooting & Helpdesk
- `troubleshooting/CLAUDE.md` - Comprehensive IT helpdesk guidance (12.4KB, 380 lines)
- `troubleshooting/README.md` - Quick start guide and issue index with first resolved issue (3.9KB, 95 lines)
- `.claude/agents/gemini-it-helpdesk-researcher.md` - Research agent configuration (8.3KB, 240 lines)
- **Ongoing Issue: Outlook Template Encoding**:
  - `troubleshooting/Outlook_Template_Encoding_Issues.md` - Comprehensive technical documentation (14KB, 600+ lines) - NEW 2026-02-16
  - `troubleshooting/Diagnose-OutlookTemplate.ps1` - Character diagnostic tool with code point analysis (6.8KB, 190 lines) - NEW 2026-02-16
  - `troubleshooting/Clean-OutlookTemplateEncoding.ps1` - Enhanced encoding-focused cleaning script (9.6KB, 290 lines) - NEW 2026-02-16
  - `troubleshooting/Test-TemplateFileLock.ps1` - File lock detection and unlock utility (7.5KB, 240 lines) - NEW 2026-02-16
  - `troubleshooting/Outlook_Template_Unicode_Encoding_Question_Marks.md` - Initial technical documentation (26KB) - 2026-02-13
  - `troubleshooting/Clean-OutlookTemplates.ps1` - Original automation script (15KB, 376 lines) - 2026-02-13
  - `troubleshooting/SCRIPT_USAGE_GUIDE.md` - Step-by-step usage instructions (11KB) - 2026-02-13

### AI PC Build (NewPC)
- `NewPC/CLAUDE.md` - NewPC project-specific guidance (239 lines)
- `NewPC/PCBuildResearch.md` - Comprehensive AI PC market research (1,055 lines, 74KB)
- `NewPC/Chosen_Build.md` - Deep technical component analysis with PCIe architecture (709 lines, 55KB)
- `NewPC/Final_Build.md` - Component selection tracker with decision log (745 lines, 30KB)
- `NewPC/Assembly_Guide.md` - Step-by-step hardware assembly guide with cable reference and BIOS config — NEW 2026-02-19
- `NewPC/Software_Setup.md` - Ubuntu 24.04 LTS full AI software stack setup guide — NEW 2026-02-19

### Zero Trust Network Access (ZTNA)
- `ZTNA_Provider_Research_2026.md` - Complete market research and provider comparison (58 pages, 1,057 lines)
- `Tailscale_Hybrid_Deployment_Guide.md` - Full Tailscale deployment guide (58 pages, 1,852 lines)
- `IPSec_SonicWall_Deployment_Guide.md` - Full IPSec/SonicWall deployment guide (53 pages, 1,495 lines)

### Financial Data Processing
- `parsing/DailyStatementParser.cs` - StoneX daily statement parser with cash settlement support (~680 lines)
- `parsing/Program.cs` - Demo console application (119 lines)
- `parsing/GetStoneXOTCDailyValuesConsole.csproj` - .NET 8.0 project file
- `parsing/StoneXAccountData.cs` - Account data model (33 properties)
- `parsing/StoneXTradeData.cs` - Trade data model (13 properties)
- `parsing/Example.csv` - Sample statement data for testing
- `parsing/ExampleWithCashSettlement.csv` - Sample data with Cash Settlement section

### VPN and Network Security
- `VPN_Benefits.md` - VPN security pros/cons for public WiFi (24KB)
- `VPN_Comparisons.md` - Commercial VPN provider comparison (31KB)
- `Draytek_Connect.md` - Draytek router VPN configuration
- `L2TP_over_IPsec.md` - L2TP VPN protocol documentation
- `Vigor2865_Firewall.pdf` - Draytek firewall configuration reference
- `Vigor2865_VPN.pdf` - Draytek VPN configuration reference

### Public WiFi Security
- `WIFI_Best_Practices_for_Laptops_and_Mobiles.md` - Comprehensive WiFi security guide
- `Mobile_Laptop_WIFI_Summary.md` - Summary of WiFi security practices
- `Mobile and Laptop Public WIFI Checklist.pdf` - Quick reference checklist

### Virtualization
- `virtual_virus_test.md` - VM security and virus isolation testing
- `virtual_machine_types.md` - Overview of VM types and differences
- `type1_hypervisors.md` - Homelab Type 1 hypervisor setup guide
- `mac_on_windows.md` - macOS on Windows for iOS development

### Project Documentation
- `CLAUDE.md` - IT project-specific guidance for Claude Code
- `SESSION_LOG.md` - Session tracking and history
- `PROJECT_STATUS.md` - This file - current project status
- `CHANGELOG.md` - Version-style change tracking

### Specialized Agents
- `.claude/agents/gemini-it-helpdesk-researcher.md` - IT helpdesk research agent for Windows 11/Azure AD/M365 troubleshooting
- `.claude/agents/gemini-it-security-researcher.md` - IT security research agent

## Documentation Standards

All documentation follows IT project standards:
- Comprehensive technical explanations with both user-friendly and technical perspectives
- Step-by-step guides with copy-pasteable commands
- Security and legal considerations addressed
- Comparison tables for feature/product comparisons
- References to authoritative sources (NIST, CISA, NSA, SANS, OWASP)
- Troubleshooting sections where applicable
- Current data (2025) for pricing, features, and recommendations

## Research Sources

Primary authoritative sources used:
- **NIST** (National Institute of Standards and Technology)
- **CISA** (Cybersecurity and Infrastructure Security Agency)
- **NSA** (National Security Agency)
- **SANS Institute**
- **OWASP** (Open Web Application Security Project)
- Independent security auditors (Deloitte, KPMG, Securitum, Cure53)
- Official vendor documentation
- Court documentation and transparency reports
