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
- If creating a hyperlink always create it using HTML with the link opening in a new tab

## Current Repository Structure

```
/it/
├── CLAUDE.md                    # This file
├── SESSION_LOG.md               # Session tracking and history
├── PROJECT_STATUS.md            # Current project status
├── CHANGELOG.md                 # Version-style change tracking
├── troubleshooting/             # IT helpdesk and issue resolution
│   ├── CLAUDE.md                # Troubleshooting-specific guidance
│   ├── README.md                # Quick start and issue index
│   ├── Clean-OutlookTemplates.ps1
│   ├── Outlook_Template_Unicode_Encoding_Question_Marks.md
│   └── SCRIPT_USAGE_GUIDE.md
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
- User prefers .NET 10 with C# 14 features (extension members, field keyword, enhanced lambda modifiers, collection expressions, primary constructors)

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

## PowerShell Development Workflow

### IT Troubleshooting Automation
When creating PowerShell scripts for IT troubleshooting and automation:
- **Target environment**: Windows 11, Microsoft 365 desktop apps, Azure AD domain-joined
- **Always check prerequisites**: Verify applications are closed before manipulating their files (e.g., Outlook must be closed)
- **Comprehensive logging**: Use color-coded output (ERROR=Red, WARNING=Yellow, SUCCESS=Green, INFO=White)
- **Automatic backups**: Create timestamped backup directories before any file modification
- **Graceful error handling**: Restore from backup automatically if script fails
- **Parameter flexibility**: Support default behavior + user-specified paths (folder or single file)
- **Test modes**: Provide `-BackupOnly` or similar switches for safe testing without modifications

### Binary File Format Handling
**CRITICAL**: Office files (.oft, .docx, .xlsx, etc.) use binary formats and cannot be edited as plain text

- **Outlook Template Files (.oft)**:
  - Format: Compound File Binary Format (CFBF) - OLE structured storage
  - Structure: Mini filesystem inside a file with streams (body, properties, attachments, metadata)
  - **DO NOT** edit .oft files in text editors (Notepad++, VS Code, etc.) - this corrupts the file
  - **Correct approach**: Use COM automation (`Outlook.Application`) to manipulate safely

- **Example Pattern**:
  ```powershell
  # Create Outlook COM object
  $outlook = New-Object -ComObject Outlook.Application
  $mailItem = $outlook.CreateItemFromTemplate($Template.FullName)

  # Extract and modify content
  $bodyText = $mailItem.Body
  $cleanedText = $bodyText.Replace([char]0x00AD, '')  # Remove soft hyphens
  $mailItem.Body = $cleanedText

  # Save in proper .oft format (NOT text!)
  $mailItem.SaveAs($Template.FullName, 5)  # 5 = olTemplate

  # Clean up COM objects
  $mailItem.Close(1)  # 1 = olDiscard
  [System.Runtime.Interopservices.Marshal]::ReleaseComObject($mailItem) | Out-Null
  [System.GC]::Collect()
  ```

- **Why manual editing fails**:
  - User experience: "I can't open the file in outlook if I have amended it in notepad++"
  - Cause: Text editors corrupt CFBF structure when saving
  - Lesson learned: Always use appropriate COM automation for Office files

### Character Code Verification
**CRITICAL**: Always verify exact character codes when troubleshooting encoding issues

- **Don't assume**: Test with actual data, use tools to identify problematic characters
- **Verification tools**:
  - Notepad++ with "Show All Characters" view
  - Hex editors for binary inspection
  - PowerShell: `[char]::ConvertFromUtf32(0x00AD)` to verify character appearance

- **Real-world example** (Outlook soft hyphen issue):
  - **Initial assumption**: Form Feed (0x000C), Vertical Tab (0x000B) causing issue
  - **Script result**: Found 0 problematic characters (wrong codes searched)
  - **User identified**: Notepad++ shows `­` symbol (soft hyphen)
  - **Correct solution**: Added `[char]0x00AD` to search list
  - **Success**: Third script run removed 5 soft hyphens

- **Pattern**:
  1. User reports symptoms (question marks in emails)
  2. Create script with best-guess character codes
  3. Test with real user data
  4. User identifies actual character in Notepad++ or hex editor
  5. Update script with verified character codes
  6. Re-test and validate

### Iterative Debugging with User Feedback
**Approach**: Test early, test often, incorporate user feedback immediately

- **Session example** (Outlook template cleaning script):
  - **Run 1**: Failed with "duplicate -Verbose parameter" error
    - Fix: Removed manual `-Verbose` (already provided by `[CmdletBinding()]`)
  - **Run 2**: Ran but failed with "Cannot convert argument for Replace to Char"
    - Fix: Changed `$char` to `$char.ToString()` for string replacement
  - **Run 3**: Ran successfully but found 0 characters (wrong codes)
    - User feedback: "Script ran but template still has question marks"
    - User identified: `­` symbol in Notepad++
    - Fix: Added `[char]0x00AD` (soft hyphen) to search list
  - **Run 4**: SUCCESS - Removed 5 soft hyphens, applied registry fixes

- **Key lessons**:
  - Each iteration revealed different bug category (syntax → type error → logic error)
  - User testing with real data caught issues that synthetic testing would miss
  - User's domain knowledge (Notepad++ character identification) complemented technical solution
  - Don't batch test multiple fixes - test one change at a time for clear feedback

### File Lock Handling (Windows Common Issue)
**Problem**: COM automation and Windows processes often hold file locks preventing access

- **Symptoms**:
  - "Outlook won't let me open the file"
  - "File is open or I don't have permission"
  - File appears modified (timestamp updated) but can't be opened

- **Causes**:
  - COM objects not fully released (even after `ReleaseComObject()`)
  - Background Outlook process still running
  - Windows Search indexer accessing file
  - Antivirus real-time scanning

- **Solutions** (ordered by effectiveness):
  1. **Restart computer** (quickest, releases all locks) - RECOMMENDED for users
  2. **Kill process**: Task Manager → End Task on OUTLOOK.exe, wait 30 seconds
  3. **Wait**: Sometimes locks release automatically after 1-2 minutes
  4. **Use backup**: Work with clean backup copy from timestamped backup directory
  5. **Copy to new filename**: `MessageTemplate_CLEAN.oft` bypasses lock on original

- **Prevention in scripts**:
  ```powershell
  # Ensure Outlook is closed before running
  function Test-OutlookRunning {
      $process = Get-Process -Name "OUTLOOK" -ErrorAction SilentlyContinue
      if ($process) {
          Write-Error "Outlook is running. Please close Outlook and try again."
          exit 1
      }
  }

  # Thorough COM cleanup
  $mailItem.Close(1)  # olDiscard
  [System.Runtime.Interopservices.Marshal]::ReleaseComObject($mailItem) | Out-Null
  [System.Runtime.Interopservices.Marshal]::ReleaseComObject($outlook) | Out-Null
  [System.GC]::Collect()
  [System.GC]::WaitForPendingFinalizers()
  ```

### PowerShell Common Pitfalls

**1. Parameter Conflicts with `[CmdletBinding()]`**
- `[CmdletBinding()]` automatically provides: `-Verbose`, `-Debug`, `-ErrorAction`, `-WarningAction`
- **Don't manually define these** in your `param()` block
- Error: "A parameter with the name 'Verbose' was defined multiple times"

**2. Char vs String Replace Methods**
```powershell
# WRONG - Char.Replace() requires Char for both parameters
$text.Replace([char]0x00AD, '')  # ERROR: Can't convert '' to Char

# CORRECT - String.Replace() accepts strings
$text.Replace([char]0x00AD.ToString(), '')  # Works!
```

**3. Script Execution Policy**
- User environment: Windows 11 may have restricted execution policy
- Always include instructions: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- Or: Run with bypass: `powershell.exe -ExecutionPolicy Bypass -File .\script.ps1`

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
- **C# Development**: .NET 10 (LTS release, supported until November 2028), C# 14 features (extension members, field keyword for backing field access, enhanced lambda parameter modifiers, collection expressions, primary constructors)
- **PowerShell Development**: Comprehensive logging, automatic backups, graceful error handling, parameter flexibility
- **Iterative debugging**: Values working through issues step-by-step with clear explanations
- **User involvement**: Appreciates incorporating user feedback immediately (e.g., user identified `­` soft hyphen character in Notepad++)
- **Code quality**: Prefers robust error handling and graceful fallbacks
- **Testing approach**: Create demo applications to verify functionality before production integration; test with real user data early
- **Code organization**: Separate concerns with focused helper methods and dedicated model classes
- **Documentation**: Prefers comprehensive technical documentation (26KB) + step-by-step usage guides (11KB) over brief summaries

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
- `gemini-it-helpdesk-researcher` - IT helpdesk research agent for Windows 11/Azure AD/M365 troubleshooting (in `troubleshooting/.claude/agents/`)

## Project-Specific Commands

This project has additional commands:
- `/update-claude` - Update this CLAUDE.md file with session learnings and user preferences
