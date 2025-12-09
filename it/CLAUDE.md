# CLAUDE.md (IT Project)

This file provides IT-specific guidance to Claude Code when working in this project.

> **Note**: This supplements the shared CLAUDE.md at `/terminai/CLAUDE.md`. Read both files for complete guidance.

## Repository Purpose

This repository contains comprehensive technical documentation focused on:
- Virtualization technologies (Type 1/Type 2 hypervisors, VMs, containers)
- Security considerations for virtual environments
- Homelab setup guides and best practices
- Cross-platform development documentation

## Documentation Standards

### Writing Style
- Comprehensive and thorough explanations
- Include practical examples and step-by-step guides
- Provide both theoretical background and hands-on instructions
- Use clear headings and structured formatting
- Include comparison tables where appropriate

### Technical Documentation Requirements
When creating technical guides, include:
- **Overview section**: What it is and why it matters
- **Prerequisites**: Hardware/software requirements with specific versions
- **Step-by-step instructions**: Detailed, numbered steps with commands
- **Configuration examples**: Real, copy-pasteable code/configs
- **Troubleshooting section**: Common issues and solutions
- **Alternatives**: Other approaches or tools when applicable
- **Cost considerations**: Where relevant (hardware, licensing, etc.)
- **Resources**: Links to official docs, communities, tutorials

### Security and Legal Considerations
Always address when relevant:
- Legal implications (license agreements, EULA violations)
- Security risks and mitigations
- Best practices for safe implementation
- Ethical considerations
- Disclaimer when activities are in legal gray areas

### Formatting Guidelines
- Use markdown formatting consistently
- Include code blocks with appropriate syntax highlighting
- Use tables for comparisons
- Include visual separators (---) for major sections
- Use emoji sparingly and only when explicitly requested
- Provide file path references when discussing specific configurations

## Current Repository Structure

```
/it/
├── CLAUDE.md                    # This file
├── SESSION_LOG.md               # Session tracking and history
├── PROJECT_STATUS.md            # Current project status
├── CHANGELOG.md                 # Version-style change tracking
├── VPN_Benefits.md              # VPN security pros/cons analysis
├── VPN_Comparisons.md           # Commercial VPN provider comparison
├── WIFI_Best_Practices_for_Laptops_and_Mobiles.md
├── Mobile_Laptop_WIFI_Summary.md
├── Draytek_Connect.md           # Draytek router VPN configuration
├── L2TP_over_IPsec.md          # L2TP VPN protocol documentation
├── virtual_virus_test.md        # VM security and virus isolation guide
├── virtual_machine_types.md     # Overview of VM types and differences
├── type1_hypervisors.md         # Homelab Type 1 hypervisor setup guide
├── mac_on_windows.md            # macOS on Windows for iOS development
└── parsing/                     # Financial data processing
    ├── DailyStatementParser.cs
    ├── Program.cs
    ├── GetStoneXOTCDailyValuesConsole.csproj
    ├── StoneXAccountData.cs
    ├── StoneXTradeData.cs
    └── Example.csv
```

## C# Development Workflow

### Financial Data Processing Projects
When working with financial data parsing (like StoneX daily statements):
- Use explicit date format parsing with `CultureInfo.InvariantCulture` for non-standard formats (e.g., dd-MMM-yyyy)
- Handle multi-page document parsing by skipping page markers but continuing data extraction
- Implement deduplication strategies based on composite keys (e.g., TradeId + StartDate + EndDate)
- Parse currency values carefully, handling $, commas, and negative values in parentheses
- Create demo applications to test parsing logic before database integration
- User prefers .NET 8.0 with C# 12 features (collection expressions, primary constructors)

### Parser Development Patterns
When building data parsers for complex documents:
- **Start by finding document structure markers** (e.g., "Daily Statement", section headers)
- **Extract metadata first** (dates, identifiers) before parsing detailed data
- **Handle multi-line data fields** by concatenating across rows with look-ahead/look-back logic
- **Use reference index passing** (`ref int index`) for complex multi-row parsing where each parse operation advances the position
- **Implement Try/Parse patterns** with explicit error handling and graceful fallbacks
- **Skip formatting elements** (page breaks, repeated headers, company info) but continue parsing data
- **Test iteratively** with real sample data to catch edge cases early
- **Field offset awareness**: When passing pre-parsed fields as parameters, skip them in subsequent array indexing

### Code Organization Preferences
- Separate parsing logic into focused helper methods (e.g., `ParseTradeSection`, `ParseAccountSection`, `ParseDate`, `ParseCurrency`)
- Create dedicated model classes for data structures (separate from parsing logic)
- Build demo/test applications alongside main implementation
- Use meaningful variable names that reflect business domain (TradeId, MarketValue, etc.)

## User Preferences

Based on previous sessions, the user:
- Prefers detailed, comprehensive documentation over brief summaries
- Values practical, hands-on guides with real commands and configurations
- Appreciates honest assessment of legal/ethical implications
- Is interested in homelab and virtualization technologies
- Wants both theoretical understanding and practical implementation
- Appreciates thorough research using specialized agents (gemini-it-security-researcher)
- Values authoritative source citation (NIST, CISA, NSA, SANS, OWASP)
- Prefers comprehensive comparison tables with verified data
- Wants both security theory and practical recommendations
- Environment: Windows 11 with WSL2 access

### Development Preferences
- **C# Development**: .NET 8.0, C# 12 features (collection expressions, primary constructors)
- **Iterative debugging**: Values working through issues step-by-step with clear explanations
- **Code quality**: Prefers robust error handling and graceful fallbacks
- **Testing approach**: Create demo applications to verify functionality before production integration
- **Code organization**: Separate concerns with focused helper methods and dedicated model classes

## Documentation Workflow

### VPN and Network Security Research
When researching VPN or network security topics:
- Use the `gemini-it-security-researcher` agent for authoritative information
- Cross-reference NIST, CISA, NSA, SANS Institute, OWASP guidance
- Verify security claims through independent audit reports
- Include both user-friendly and technical perspectives
- Provide comparison tables for product/feature comparisons
- Include current pricing and feature data with date verification
- Address jurisdiction and privacy implications

### Research Quality Standards
- Verify independent security audits (Deloitte, KPMG, Securitum, Cure53)
- Check court-tested no-logs policies where applicable
- Include real-world incidents and transparency reports
- Cross-reference multiple authoritative sources
- Use current data (specify year: 2025)

## Project-Specific Agents

This project has specialized agents available:
- `windows-virtual-assistant-security` - Specialized assistance for Windows virtualization and security topics
- `gemini-it-security-researcher` - Expert research agent for IT security topics, vulnerabilities, standards, and best practices

## Project-Specific Commands

This project has additional commands:
- `/update-claude` - Update this CLAUDE.md file with session learnings and user preferences
