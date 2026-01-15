# IT Project Changelog

All notable changes to the IT infrastructure and security documentation project.

## [Unreleased] - 2026-01-15

### Documentation
- Provided technical guidance for PdfSharp library MemoryStream handling
- Diagnosed `PdfDocumentOpenMode.Import` limitation preventing save operations
- Recommended direct file read approach using `File.ReadAllBytes()` for loading PDFs into memory
- Explained separation of concerns: file loading vs PDF processing with appropriate open modes
- User successfully implemented solution

## [Unreleased] - 2025-12-12

### Documentation
- Provided technical guidance for implementing one-hour retry mechanism in StoneX parser application
- Designed in-app delay solution using `Task.Delay()` with `ManualResetEventSlim` for minimal resource usage
- Implemented recursive retry pattern for unlimited email check retries
- User implementing solution independently

## [Unreleased] - 2025-12-10

### Added
- `parsing/DailyStatementParser.cs`: Cash Settlement section parsing capability (~200 additional lines)
  - `FindCashSettlementSections()` method: Locates all "Cash Settlements" sections and associates with Daily Statement dates
  - `ParseCashSettlementSection()` method: Parses cash settlement data rows with section boundary detection and total line handling
  - `ParseCashSettlementDataRow()` method: Extracts individual cash settlement entries with two-line format handling
  - Field mappings: Cash Amount → MarketValue, Settlement Price → MarketPrice
  - Handles cash settlement-specific fields: Type, Description, Expiry Date, Applied On
  - Integrated into main Parse() workflow before trade section parsing
  - Consistent error handling and page break skipping patterns

- `parsing/ExampleWithCashSettlement.csv`: Sample data file demonstrating Cash Settlement section format

### Changed
- `parsing/DailyStatementParser.cs`: Parse() method updated to include cash settlement parsing step

## [Unreleased] - 2025-12-09

### Added
- `parsing/DailyStatementParser.cs` (466 lines): Complete C# parser for StoneX daily statement PDFs
  - Parses "Daily Statement" sections first to extract published dates
  - Extracts trade data from "Open Positions and Market Values" sections
  - Handles multi-page documents with page break continuation
  - Multi-line contract description concatenation
  - Long vs Short position determination via "Long Avg"/"Short Avg" markers
  - Trade deduplication by TradeId + StartDate + EndDate
  - Account information parsing from last section
  - Multiple date format support (dd-MMM-yyyy, dd/MM/yyyy, etc.)
  - Currency parsing with $, commas, and negative values in parentheses

- `parsing/Program.cs` (119 lines): Demo console application
  - Comprehensive trade data display with all details
  - Complete account information output with margins and balances
  - Summary statistics and totals

- `parsing/GetStoneXOTCDailyValuesConsole.csproj`: .NET 8.0 console application project

### Fixed
- Date parsing format compatibility (dd-MMM-yyyy format now works with CultureInfo.InvariantCulture)
- Field offset error in ParseTradeDataRow (now starts at partIndex=1 to skip date field)
- Page break handling (parser now continues across multiple pages)

### Changed
- Parser flow refactored to find "Daily Statement" sections first, then associate dates with subsections

## [Unreleased] - 2025-11-19

### Added
- `VPN_Benefits.md` (24KB): Comprehensive VPN security analysis for public WiFi usage
  - Security benefits: Network-layer protection, MITM prevention, evil twin defense, packet sniffing protection
  - Security limitations: DNS/WebRTC/IPv6 leaks, kill switch failures, trust model concerns
  - What VPNs don't protect: Phishing, malware, application-layer attacks, endpoint security
  - Best practices: Provider selection, configuration, layered security approach
  - Based on NIST, CISA, NSA, SANS Institute, OWASP guidance
  - Both user-friendly and technical perspectives included

- `VPN_Comparisons.md` (31KB): Detailed commercial VPN provider comparison
  - 8 major providers analyzed: NordVPN, ExpressVPN, Surfshark, ProtonVPN, PIA, CyberGhost, Mullvad, IVPN
  - Security features comparison: Encryption, protocols, audits, no-logs verification
  - Jurisdiction analysis: Five/Nine/Fourteen Eyes implications for privacy
  - Comprehensive pricing comparison: Monthly, annual, 2-year plans
  - Speed performance rankings with 2025 benchmarks
  - Feature matrices: Simultaneous devices, server counts, specialty servers
  - Verified audit histories from Deloitte, KPMG, Securitum, Cure53
  - Court-tested no-logs policies (PIA subpoenas, Mullvad police raid)
  - Provider recommendations by use case: Security, value, anonymity, speed, features

- `SESSION_LOG.md`: Session tracking and history documentation
- `PROJECT_STATUS.md`: Current project status and active work areas
- `CHANGELOG.md`: This file - version-style change tracking

### Documentation
- Deployed gemini-it-security-researcher agent for VPN security research
- Cross-referenced authoritative cybersecurity sources (NIST, CISA, NSA, SANS, OWASP)
- Verified security audit reports and transparency documentation
- Included 2025 current pricing and feature data
- Added both technical and user-friendly explanations throughout

## [Previous Work] - Pre-2025-11-19

### Added
- `WIFI_Best_Practices_for_Laptops_and_Mobiles.md`: Comprehensive WiFi security guide
- `Mobile_Laptop_WIFI_Summary.md`: WiFi security practices summary
- `Mobile and Laptop Public WIFI Checklist.pdf`: Quick reference security checklist
- `Draytek_Connect.md`: Draytek router VPN configuration guide
- `L2TP_over_IPsec.md`: L2TP VPN protocol documentation
- `Vigor2865_Firewall.pdf`: Draytek firewall configuration reference
- `Vigor2865_VPN.pdf`: Draytek VPN configuration reference
- `virtual_virus_test.md`: VM security and virus isolation testing guide
- `virtual_machine_types.md`: Overview of VM types and differences
- `type1_hypervisors.md`: Homelab Type 1 hypervisor setup guide
- `mac_on_windows.md`: macOS on Windows for iOS development guide
- `CLAUDE.md`: IT project-specific guidance for Claude Code
- `.claude/agents/gemini-it-security-researcher.md`: IT security research agent

### Documentation
- Established comprehensive documentation standards
- Created virtualization and security knowledge base
- Developed network infrastructure guides
- Added public WiFi security best practices
