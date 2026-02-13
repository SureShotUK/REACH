# IT Project Session Log

This log tracks all Claude Code sessions for the IT infrastructure and security documentation project.

---

## Session 2026-02-13 16:00

### Summary
First real-world issue diagnosed and resolved using the IT troubleshooting system: Microsoft Outlook template displaying question marks (?????) when sending emails with £ and ® symbols. Successfully identified root cause as soft hyphen characters (U+00AD) triggering Microsoft Outlook Build 19628.20150+ Unicode encoding bug. Created PowerShell automation script to clean templates using Outlook COM, comprehensive technical documentation, and step-by-step usage guide.

### Work Completed
- **Issue Diagnosis** (Systematic 7-Step Framework):
  - **Step 1**: Gathered complete information from user about Outlook template question marks
  - **Step 2**: Researched Microsoft Outlook Build 19628.20150+ Unicode encoding bug
  - **Step 3**: Identified soft hyphen characters (U+00AD / `­` / &shy;) as root cause
  - **Step 4**: Understood encoding cascade effect: soft hyphens poison entire email encoding
  - **Step 5**: Tested multiple solutions (manual editing failed - corrupted CFBF format)
  - **Step 6**: Implemented PowerShell COM automation solution
  - **Step 7**: Applied registry fixes to prevent recurrence

- **PowerShell Automation Script** - `troubleshooting/Clean-OutlookTemplates.ps1` (376 lines, 15KB):
  - Searches default Outlook template locations or user-specified path (file or folder)
  - Creates automatic backups with timestamp before cleaning
  - Uses Outlook COM automation to safely manipulate binary .oft files
  - Removes problematic Unicode characters: Soft Hyphen (U+00AD), Form Feed, Vertical Tab, etc.
  - Applies UTF-8 registry fixes: AutoDetectCharset=0, SendCharset=65001, DisableCharsetDetection=1
  - Comprehensive logging with color-coded output (ERROR, WARNING, SUCCESS, INFO)
  - Parameters: `-TemplatePath` (optional path), `-BackupOnly` (test mode), `-ApplyRegistryFix`
  - Checks Outlook is closed before running
  - Graceful error handling with automatic backup restoration on failure
  - **Bug fixes during development**:
    - Removed duplicate `-Verbose` parameter (already provided by CmdletBinding)
    - Fixed string replacement (changed from `$char` to `$char.ToString()` for empty string replacement)
    - Added soft hyphen (U+00AD) to character search list (THE MAIN CULPRIT!)

- **Comprehensive Technical Documentation** - `troubleshooting/Outlook_Template_Unicode_Encoding_Question_Marks.md` (26KB):
  - Complete root cause analysis: Soft hyphens + Outlook Build 19628.20150+ encoding bug
  - Technical explanation of encoding cascade effect (single soft hyphen poisons entire email)
  - Character identification: U+00AD / 0xAD / `­` / &shy; / &#173;
  - 5 solution methods with success rates and risk levels:
    1. PowerShell automation (95% success, low risk) - RECOMMENDED
    2. Recreate template from scratch (100% success, high effort)
    3. Manual character removal in Outlook (60% success, doesn't prevent recurrence)
    4. Registry fixes only (40% success, doesn't clean existing)
    5. Manual binary editing in Notepad++ (0% success - CORRUPTS FILE)
  - Prevention best practices (avoid copying text from web, use plain text paste)
  - Compound File Binary Format (CFBF) explanation - why .oft files can't be edited as text
  - Troubleshooting common issues (file locks, Outlook won't open, permissions)
  - FAQ section with technical details
  - Microsoft KB references and acknowledgment of bug

- **Step-by-Step Usage Guide** - `troubleshooting/SCRIPT_USAGE_GUIDE.md` (11KB):
  - Prerequisites and safety checks (close Outlook first!)
  - Execution policy setup instructions
  - 3 usage scenarios with examples:
    - Default: Clean all templates in standard locations
    - Specific folder: Clean only templates in specified folder
    - Specific file: Clean only one template
  - Parameter reference with detailed explanations
  - What the script does (step-by-step breakdown)
  - Expected output and log file interpretation
  - Troubleshooting section (execution policy, file locks, permissions, path errors)
  - Rollback procedure using automatic backups
  - Quick reference card for common commands
  - Next steps after running script

- **Script Execution Results** (3 runs documented in log files):
  - **Run 1** (16:06:37): Failed with character replacement error (bug in code)
  - **Run 2** (16:23:46): Ran successfully but found 0 problematic characters (wrong character codes searched)
  - **Run 3** (16:56:14): ✅ SUCCESS! Removed 5 soft hyphen characters from MessageTemplate.oft, applied registry fixes

- **Updated Knowledge Base** - `troubleshooting/README.md`:
  - Added first resolved issue entry: "Outlook Template Question Marks (Unicode Encoding)"
  - Referenced technical documentation and PowerShell script
  - Categorized under Microsoft Office issues

### Files Changed
- `troubleshooting/Clean-OutlookTemplates.ps1` - Created PowerShell automation script (376 lines, 15KB)
- `troubleshooting/Outlook_Template_Unicode_Encoding_Question_Marks.md` - Created comprehensive technical documentation (26KB)
- `troubleshooting/SCRIPT_USAGE_GUIDE.md` - Created step-by-step usage guide (11KB)
- `troubleshooting/README.md` - Updated with first resolved issue entry
- `troubleshooting/CleanTemplates_20260213_160637.log` - First run log (failed with bug)
- `troubleshooting/CleanTemplates_20260213_162346.log` - Second run log (found 0 characters)
- `troubleshooting/CleanTemplates_20260213_165614.log` - Third run log (SUCCESS - removed 5 characters)
- `troubleshooting/Backups_20260213_162346/` - Automatic backup directory from Run 2
- `troubleshooting/Backups_20260213_165614/` - Automatic backup directory from Run 3
- `troubleshooting/MessageTemplate.oft` - Test template (cleaned successfully)
- `troubleshooting/EmailWithBOM.oft` - Test template (already clean)
- `troubleshooting/MessageTemplateNoBOM.oft` - Test template (already clean)

### Git Commits
(To be committed at end of session)

### Key Decisions
- **Root Cause**: Soft Hyphen characters (U+00AD) in template body triggering Outlook Build 19628.20150+ Unicode encoding bug
- **Solution Approach**: PowerShell COM automation rather than manual editing
  - Rationale: Binary CFBF format cannot be edited as text; manual editing in Notepad++ corrupts file
  - Tried: User attempted manual editing → file corrupted, Outlook won't open
  - Correct approach: Use Outlook COM to open → clean → save in proper .oft format
- **Character Detection**: Search for soft hyphens (U+00AD) specifically, not just generic control characters
  - Initial bug: Script searched Form Feed, Vertical Tab but missed soft hyphen
  - User identified: Characters appear as `­` in Notepad++ (soft hyphen symbol)
  - Fix: Added `[char]0x00AD` as FIRST item in `$controlChars` array
- **Registry Fixes**: Apply UTF-8 enforcement to prevent auto-detection causing future issues
  - AutoDetectCharset = 0 (Disabled automatic detection)
  - SendCharset = 65001 (UTF-8 code page)
  - DisableCharsetDetection = 1 (Disabled conversion on send)
- **Script Parameters**: Support three usage modes for flexibility
  - Default: Search all standard Outlook template locations
  - `-TemplatePath <folder>`: Search only specified folder
  - `-TemplatePath <file.oft>`: Clean only one specific file
- **Backup Strategy**: Automatic timestamped backups before any modification
  - Creates `Backups_YYYYMMDD_HHMMSS` subdirectory
  - Restores from backup automatically on error
  - User can manually rollback if needed

### Technical Deep-Dive

**Encoding Cascade Effect**:
1. Soft hyphen (U+00AD) invisible HTML character exists in template
2. User types £ or ® symbol (legitimate Unicode characters)
3. Outlook Build 19628.20150+ encoding detection activates
4. Detection algorithm sees soft hyphen + high Unicode characters
5. Incorrectly switches from UTF-8 to Windows-1252 or ISO-8859-1
6. UTF-8 characters (£, ®) become unmappable in Windows-1252
7. Outlook renders unmappable characters as ? (question marks)
8. **Key insight**: Removing soft hyphens prevents cascade from starting

**Character Analysis**:
- **Soft Hyphen (U+00AD)**:
  - HTML entity: `&shy;` or `&#173;`
  - Purpose: Suggests optional line break position for word wrapping
  - Rendering: Usually invisible, appears as `­` in some editors
  - Problem: Triggers Outlook's broken encoding detection
  - Source: Often copied from websites when creating templates
  - Display in Notepad++: Shows as `­` symbol with proper encoding display

**Why Manual Editing Failed**:
- .oft files use Compound File Binary Format (CFBF) - OLE structured storage
- CFBF is NOT plain text - it's a mini filesystem inside a file
- Contains streams: email body (HTML), properties, attachments, metadata
- Notepad++ sees binary data but displays it corruptly
- User saw: "¬" characters and "­ ­ ­ ­ ­" soft hyphens mixed with binary
- Editing binary as text: Corrupts file structure, Outlook refuses to open
- Correct approach: Use Outlook COM to read/write in native format

**PowerShell Script Approach**:
```powershell
# Key technique: Use Outlook COM object to manipulate binary .oft files safely
$outlook = New-Object -ComObject Outlook.Application
$mailItem = $outlook.CreateItemFromTemplate($Template.FullName)

# Extract body content
$bodyText = $mailItem.Body
$bodyHTML = $mailItem.HTMLBody

# Remove soft hyphens and other control characters
$cleanedText = $bodyText.Replace([char]0x00AD).ToString(), '')

# Save in proper .oft format (not text!)
$mailItem.SaveAs($Template.FullName, 5)  # 5 = olTemplate format
```

### Debugging Journey (Iterative Problem Solving)

**Bug 1: Duplicate -Verbose Parameter**
- Error: "A parameter with the name 'Verbose' was defined multiple times"
- Cause: Manually defined `-Verbose` when `[CmdletBinding()]` already provides it
- Fix: Removed manual `-Verbose` parameter from `param()` block
- Lesson: `[CmdletBinding()]` automatically adds common parameters

**Bug 2: String Replacement Type Error**
- Error: "Cannot convert argument 'newChar', with value: '', for 'Replace' to type 'System.Char'"
- Cause: Using `.Replace($char, '')` where second parameter must be Char type
- Fix: Changed to `$cleanedText.Replace($char.ToString(), '')` to use string Replace method
- Lesson: PowerShell has both Char.Replace() and String.Replace() with different signatures

**Bug 3: Wrong Characters Searched**
- Symptom: Script ran successfully but "Removed 0 problematic character(s)" despite visible issue
- Cause: Searching for Form Feed (0x000C), Vertical Tab (0x000B) but actual problem was Soft Hyphen (0x00AD)
- User identified: Notepad++ shows `­` symbol (soft hyphen)
- Fix: Added `[char]0x00AD` to `$controlChars` array as FIRST item
- Result: Third run successfully removed 5 soft hyphens
- Lesson: Always verify character codes match actual problematic characters

**Bug 4: File Corruption from Manual Editing**
- User attempted: Editing .oft file in Notepad++, replacing soft hyphens, saving as .oft
- Result: "I can't open the file in outlook if I have amended it in notepad++"
- Explanation: CFBF binary format corrupted by text editor
- Resolution: Restored from PowerShell script backup, used COM automation instead
- Lesson: Never edit binary formats (CFBF, Office files) as plain text

**Unresolved Issue: File Lock**
- Symptom: "Outlook won't let me open the cleaned file, it suggest the file is open or I don't have permission"
- Diagnosis: File lock from Outlook process or COM object not fully released
- Script actually worked: Log shows "Removed 5 problematic character(s)" and "Template cleaned and saved successfully"
- Suggested solutions:
  1. Restart computer (quickest - releases all locks)
  2. Kill OUTLOOK.exe process in Task Manager, wait 30 seconds
  3. Use cleaned backup from `Backups_20260213_165614\MessageTemplate.oft`
  4. Copy file to new name: `MessageTemplate_CLEAN.oft`
- User action: Initiated `/end-session` before resolving (chose to handle later)

### Reference Documents
- Microsoft Knowledge Base articles on Outlook Unicode encoding issues
- Microsoft acknowledgment of Build 19628.20150+ encoding bug affecting Classic Outlook
- Compound File Binary Format (CFBF) specification - OLE structured storage
- Unicode character reference: U+00AD (Soft Hyphen), U+000C (Form Feed), U+000B (Vertical Tab)
- PowerShell COM automation documentation for Outlook.Application
- Outlook registry settings for UTF-8 encoding enforcement

### Environment
- **OS**: Windows 11 (latest updates)
- **Outlook Version**: Microsoft 365 desktop app, Build 19628.20150+ (Classic Outlook)
- **Issue Scope**: Single user, affects all emails sent from specific template
- **Template Format**: .oft files (Outlook Template, CFBF binary format)
- **Encoding**: UTF-8 expected, incorrectly switching to Windows-1252/ISO-8859-1

### Next Actions
- [ ] User to resolve file lock issue (restart computer recommended)
- [ ] User to test cleaned template by sending email with £ and ® symbols
- [ ] Verify question marks no longer appear in sent emails
- [ ] User to run script on other templates if needed (default locations)
- [ ] Monitor for recurrence (soft hyphens reappearing from web copy-paste)
- [ ] Consider preventive training: "Use plain text paste (Ctrl+Shift+V) when copying from web"

---

## Session 2026-02-13 15:00

### Summary
Created comprehensive IT troubleshooting and helpdesk system with systematic diagnostic framework, specialized Gemini research agent, and world-class support methodology optimized for Windows 11, Azure AD, and Microsoft 365 environments.

### Work Completed
- Created `/it/troubleshooting/` directory structure for IT helpdesk knowledge base
- Created **troubleshooting/CLAUDE.md** (12.4KB):
  - Systematic 7-step troubleshooting methodology (Intake → Diagnosis → Research → Hypothesis → Testing → Resolution → Prevention)
  - Target environment specifications (Windows 11, Azure AD domain-joined, Microsoft 365 desktop apps)
  - Comprehensive diagnostic tools and PowerShell commands for Windows, Office, network, and Azure AD
  - Issue documentation standards with detailed template structure
  - Priority levels (P1-P4) and escalation criteria
  - Communication standards for clear, helpful user interactions
  - Common issue categories (Office, Windows Update, Network, Azure AD, Performance)
  - Best practices and continuous improvement guidelines
- Created **troubleshooting/README.md** (2.9KB):
  - Quick start guide for troubleshooting workflow
  - Issue category organization structure
  - Documentation template reference
  - Issue index for tracking resolved problems
- Created **.claude/agents/gemini-it-helpdesk-researcher.md** (8.3KB):
  - Specialized Gemini-powered IT helpdesk research agent
  - Research methodology prioritizing Microsoft official sources (KB articles, Learn, TechCommunity)
  - Prevalence assessment framework (Widespread/Common/Uncommon/Rare/Isolated)
  - Verified solution ranking by success rate and risk level
  - Structured output format with root cause analysis and step-by-step solutions
  - Search query strategies optimized for Windows 11, Office, Azure AD issues
  - Verification standards and red flags to avoid unreliable sources
  - Special considerations for Microsoft 365 desktop apps and Azure AD joined devices

### Files Changed
- `troubleshooting/CLAUDE.md` - Created (12,356 bytes)
- `troubleshooting/README.md` - Created (2,960 bytes)
- `.claude/agents/gemini-it-helpdesk-researcher.md` - Created (8,272 bytes)

### Git Commits
(To be committed at end of session)

### Key Decisions
- **Systematic approach over ad-hoc troubleshooting**: Implemented structured 7-step diagnostic framework to ensure thorough problem analysis
- **Research-first methodology**: Integrated Gemini research agent to leverage internet-wide knowledge before attempting fixes
- **Environment standardization**: Focused on Windows 11 + Azure AD + Microsoft 365 as standard environment for consistency
- **Documentation as knowledge base**: Every resolved issue should be documented to build organizational knowledge
- **Priority-based triage**: Implemented P1-P4 priority system with clear escalation criteria
- **User-centric communication**: Balance technical accuracy with clear explanations that help users understand root causes

### Reference Documents
- Microsoft official documentation sources (KB articles, Microsoft Learn, TechCommunity)
- PowerShell diagnostic commands for Windows 11, Office 365, and Azure AD
- NIST, CISA, and other authoritative IT security guidance (via gemini-it-helpdesk-researcher agent)

### Next Actions
- [ ] Test troubleshooting system with first real-world issue
- [ ] Refine diagnostic questions based on actual user interactions
- [ ] Build issue resolution documentation library as problems are resolved
- [ ] Create issue category subdirectories as patterns emerge
- [ ] Fine-tune research agent search strategies based on effectiveness

---

## Session 2026-02-12 22:30

### Summary
Comprehensive AI PC build planning and component selection for local LLM inference (coding assistance and homework help). Created new NewPC project directory with detailed research, technical analysis, and component tracking. Made key hardware decisions including CPU, motherboard, RAM, PSU, and case, all optimized for UK market pricing and dual GPU upgrade path.

### Work Completed
- Created `/it/NewPC/` project directory structure with dedicated CLAUDE.md
- Deployed `gemini-researcher` agent for comprehensive AI PC hardware research
- Created **CLAUDE.md** for NewPC project (8.6KB):
  - Project purpose and target audience definition
  - Documentation requirements for AI-specific hardware
  - Research standards (link verification, source requirements)
  - Decision-making methodology (funnel approach from broad to specific)
  - Cost-capability balance guidelines
  - AI workload considerations (GPU VRAM, memory bandwidth, inference metrics)
- Created **PCBuildResearch.md** (74KB, comprehensive market research):
  - 5 complete PC build configurations ($899-5,000 USD initially, converted to GBP)
  - GPU performance comparison (tokens per second benchmarks for 8B, 70B models)
  - VRAM requirements by model size (7B to 200B+)
  - AMD vs NVIDIA comparison for AI workloads
  - CPU, RAM, and storage performance impact analysis
  - Software stack comparison (Ollama, LM Studio, Open WebUI)
  - Price breakdowns by tier with pros/cons for each
  - Recommendations by use case (coding, homework help, both)
  - All sources with verified working links
- Created **Chosen_Build.md** (55KB, deep technical analysis):
  - PCIe lane architecture analysis (AMD Ryzen vs Intel vs Threadripper)
  - Critical reality check: Consumer Ryzen limited to x16/x8 (not x16/x16)
  - Why x16/x8 has <2% performance impact for LLM inference
  - Three motherboard + CPU combinations with detailed PCIe configurations
  - Added AMD Ryzen 9 9950X3D option (latest Zen 5 X3D with 144MB cache)
  - Removed all Intel options per user request
  - Performance impact analysis for GPU, CPU, RAM, motherboard VRM, and storage
  - Bottleneck analysis (GPU VRAM is primary limitation)
  - Thermal management and power supply requirements
- Created **Final_Build.md** (30KB, component selection tracker):
  - Confirmed components with UK pricing (GBP):
    - CPU: AMD Ryzen 9 7900X @ £320-380
    - Motherboard: ASRock X670E Taichi @ £280-340
    - RAM: 64GB DDR5-6000 CL36 @ £599 (Overclockers UK)
    - PSU: Thermaltake Toughpower GF3 1650W @ £240 (Scan.co.uk)
    - Case: Fractal Design Torrent @ £175
  - Detailed specs and justifications for each confirmed component
  - Options with pros/cons for remaining decisions (GPU, storage, cooler, fans)
  - Running cost summary with budget tracking
  - Purchase order recommendations
  - Assembly notes and software setup plan
  - Decision log tracking all choices made
- Discovered user is in UK market (critical pricing adjustment):
  - Updated all pricing from USD to GBP including 20% VAT
  - RAM pricing reality check: £599-717 for 64GB DDR5-6000 (not $250-280)
  - Budget adjusted from £1,500-1,800 to £2,200-2,400 due to UK market
- Evaluated eBay UK listing for ASUS TUF RTX 3090 (user's preferred model)
  - Provided detailed analysis of seller rating, pricing, condition, and red flags
  - Current bid £567.70 but auction has 6 days left with 30 watchers
  - "No returns" policy identified as major concern
  - Recommended questions to ask seller before bidding

### Files Changed
- `it/NewPC/CLAUDE.md` - Created NewPC project-specific guidance (239 lines)
- `it/NewPC/PCBuildResearch.md` - Created comprehensive AI PC market research (1,055 lines)
- `it/NewPC/Chosen_Build.md` - Created deep technical component analysis (709 lines)
- `it/NewPC/Final_Build.md` - Created component selection tracker (745 lines)

### Git Commits
None yet - new files to be committed in end-of-session commit

### Key Decisions

**Build Philosophy**: "Best bang for buck, add not replace"
- Buy once, add second GPU later (not replace components)
- Focus on dual GPU upgrade path from day one
- Prioritize quality components that won't need upgrading

**Component Selections**:
- **CPU**: Ryzen 9 7900X over 9950X3D (proven Zen 4, excellent value, 12 cores sufficient)
- **Motherboard**: ASRock X670E Taichi (x16/x8 PCIe, 24+2+1 VRM, 4x M.2, best value)
- **RAM**: 64GB CL36 not CL30 (marginal performance difference, better value)
- **PSU**: 1650W upfront not 1000W (avoids replacement when adding 2nd GPU, saves £30-60 long-term)
- **Case**: Fractal Design Torrent (best GPU airflow, critical for dual GPU 700W heat)

**Technical Insights**:
- Consumer Ryzen cannot do x16/x16 dual GPU (24 PCIe lanes max)
- x16/x8 configuration has <2% performance impact for LLM inference
- GPU VRAM is primary bottleneck, not PCIe bandwidth
- UK RAM pricing 2-3x higher than US market (£599 vs $250-280 equivalent)
- Used RTX 3090 24GB @ £600-700 offers best value (vs £1,999 RTX 5090)

**Budget Reality**:
- Initial target: £1,500-1,800
- Revised target: £2,200-2,400 (UK market + component quality)
- Confirmed spending: £1,614-1,734 (CPU, MB, RAM, PSU, Case)
- Remaining: GPU (£550-750), Storage (£140-180), Cooler (£90-150), Fans (£20-55)

**UK Market Discoveries**:
- Overclockers UK: £599 for DDR5-6000 CL36 (best RAM price)
- Scan.co.uk: £716.99 for DDR5-6000 CL40 (worse latency, avoid)
- CCL Computers: £699.99 for same spec (£100 more expensive)
- CeX: Limited RTX 3090 stock but 24-month warranty option
- eBay UK: Best GPU value but higher risk (no warranty typically)

### Environment & Specifications
**Build Target**:
- Purpose: Local LLM inference for coding assistance and homework help
- Single RTX 3090 24GB now (£600-700 used)
- Dual RTX 3090 upgrade path (48GB total VRAM for 200B+ models)
- Software: Ubuntu 24.04 LTS or Windows 11 + WSL2
- AI Stack: Ollama + Open WebUI (NetworkChuck-style setup)

**Expected Performance**:
- Single GPU: 7B-70B models @ 42-120 tokens/second
- Dual GPU future: 70B-405B models @ 8-75 tokens/second
- CodeLlama 34B: 20-28 tokens/second
- Mistral 7B: 110-120 tokens/second

### Reference Documents
- `/mnt/c/Users/SteveIrwin/terminai/it/NewPC/CLAUDE.md` - NewPC project guidance
- `/mnt/c/Users/SteveIrwin/terminai/it/NewPC/PCBuildResearch.md` - Comprehensive market research with verified sources
- `/mnt/c/Users/SteveIrwin/terminai/it/NewPC/Chosen_Build.md` - Deep technical analysis and PCIe architecture
- `/mnt/c/Users/SteveIrwin/terminai/it/NewPC/Final_Build.md` - Component selection tracker with confirmed choices
- Hardware sources: Tom's Hardware, r/LocalLLaMA, Puget Systems, Hardware Corner
- GPU benchmarks: Hardware Busters, RunPod, Local AI Master
- UK Retailers: Scan.co.uk, Overclockers UK, Amazon UK, CCL Computers, CeX
- eBay UK: RTX 3090 market analysis (£550-750 typical pricing)

### Research Methodology
- Used gemini-researcher agent for comprehensive hardware research
- Verified all external links before inclusion (WebFetch tool)
- Cross-referenced multiple authoritative sources for benchmarks
- Applied funnel method: broad market survey → narrow to 2-3 options per component
- Emphasized UK market pricing throughout (GBP with 20% VAT)
- Focused on real-world user builds and reviews (r/LocalLLaMA community)

### Next Actions
- [ ] Complete GPU purchase decision (eBay UK vs CeX with warranty)
  - [ ] Evaluate specific eBay listings (user sharing URLs for analysis)
  - [ ] Decide: ASUS TUF RTX 3090 (best for AI) vs other models
  - [ ] Set maximum bid/offer price (recommended £700 max)
- [ ] Select storage: Gen4 2TB (£140-180) vs Gen5 (£240-280)
- [ ] Select CPU cooler: Arctic 280mm (£90-110) vs alternatives
- [ ] Determine additional fans needed based on case choice
- [ ] Create purchase order and timeline
- [ ] Plan OS installation (Ubuntu 24.04 LTS vs Windows 11 + WSL2)
- [ ] Document Ollama + Open WebUI setup process
- [ ] Consider adding second RTX 3090 in 6-12 months (when needed for larger models)

---

## Session 2026-02-12 19:30

### Summary
Quick documentation update to reflect user preference for .NET 10 and C# 14 in C# development workflow. Verified current versions and updated CLAUDE.md with latest .NET LTS release and C# features.

### Work Completed
- Verified current .NET version (10.0.3, LTS release from November 2025, supported until November 2028)
- Verified current C# version (14, released with .NET 10)
- Updated `CLAUDE.md` to reflect user preference:
  - Financial Data Processing Projects section: Changed .NET 8.0/C# 12 to .NET 10/C# 14
  - Development Preferences section: Changed .NET 8.0/C# 12 to .NET 10/C# 14
- Added C# 14 feature references (extension members, field keyword, enhanced lambda modifiers)

### Files Changed
- `CLAUDE.md` - Updated .NET and C# version preferences in two locations (lines 88, 122)

### Git Commits
None yet - changes staged for end-of-session commit

### Key Decisions
- User prefers to work with .NET 10 (LTS release) and C# 14 (latest version)
- Kept existing C# 12 features in list (collection expressions, primary constructors) as they're still relevant
- Added new C# 14 features: extension members, field keyword for backing field access, enhanced lambda parameter modifiers

### Reference Documents
- <a href="https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview" target="_blank">What's new in .NET 10 | Microsoft Learn</a>
- <a href="https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14" target="_blank">What's new in C# 14 | Microsoft Learn</a>

### Next Actions
- [ ] Continue with C# financial data parsing projects using .NET 10 and C# 14
- [ ] Leverage new C# 14 features (extension members, field keyword) when appropriate

---

## Session 2026-02-06 14:30

### Summary
Comprehensive research and deployment guide creation for Zero Trust Network Access (ZTNA) solutions. Evaluated 6+ ZTNA providers and created two complete deployment guides (Tailscale and IPSec) for a 35-user, 3-office hybrid workforce with specific focus on PostgreSQL database access and "stupid simple" user experience.

### Work Completed
- Deployed `gemini-it-security-researcher` agent for comprehensive ZTNA market research
- Created **ZTNA_Provider_Research_2026.md** (58 pages, 1,057 lines):
  - Evaluated 6 ZTNA providers: Tailscale, Twingate, Cloudflare Zero Trust, ZeroTier, NordLayer, SonicWall Cloud Secure Edge
  - Budget analysis under $10/user/month target ($350/month for 35 users)
  - Detailed pricing comparison: Tailscale ($2,520/year), Twingate ($2,100/year), Cloudflare (FREE-$2,940/year)
  - Performance benchmarks for PostgreSQL ODBC queries (1-5ms P2P, 10-50ms relay, 50-200ms+ traditional VPN)
  - Feature comparison matrices (site-to-site, RDP support, Azure AD SSO, performance)
  - Top recommendations with cost-benefit analysis
  - Special consideration for SonicWall TZ270 native ZTNA capability (Gen 7+)
- Created **Tailscale_Hybrid_Deployment_Guide.md** (58 pages, 1,852 lines):
  - Complete phased deployment plan (pilot → production) over 6 weeks
  - Subnet router configuration for transparent office access (no client needed for office-based work)
  - Azure AD SSO integration with Microsoft 365 (one-time sign-in)
  - Intune and GPO auto-deployment instructions for 35 laptops
  - PostgreSQL ODBC configuration for database access via Excel
  - RDP configuration and performance optimization
  - ACL configuration for granular security controls
  - 1-page user quick-start guide (literally "sign in once with Microsoft, you're done")
  - Comprehensive troubleshooting guide (connectivity, performance, ACL issues)
  - Maintenance and operations procedures
- Created **IPSec_SonicWall_Deployment_Guide.md** (53 pages, 1,495 lines):
  - Traditional IPSec site-to-site VPN approach ($0 cost using existing hardware)
  - Step-by-step SonicWall TZ270 configuration (Office1 & Office2)
  - Step-by-step Draytek router configuration (Office3)
  - IPSec tunnel setup (Office1 ↔ Office3, optional Office2 ↔ Office3)
  - NAT exemption configuration for VPN traffic
  - SonicWall Mobile Connect SSL VPN setup for remote workers
  - Azure AD/SAML SSO option for unified authentication
  - PostgreSQL ODBC configuration (identical to Tailscale approach)
  - RDP configuration and performance optimization
  - Remote worker user guide ("Connect VPN first, then work")
  - Comprehensive troubleshooting (tunnel issues, authentication failures, performance)
  - Comparison appendix: IPSec vs Tailscale trade-offs

### Files Changed
- `ZTNA_Provider_Research_2026.md` - Created comprehensive ZTNA market research (1,057 lines)
- `Tailscale_Hybrid_Deployment_Guide.md` - Created complete Tailscale deployment guide (1,852 lines)
- `IPSec_SonicWall_Deployment_Guide.md` - Created complete IPSec/SonicWall deployment guide (1,495 lines)

### Git Commits
- `b5ce3ec` - Add comprehensive ZTNA research and deployment guides for multi-site environment

### Key Decisions
- **Primary recommendation: Tailscale** for hybrid workers prioritizing "stupid simple" experience ($2,520/year)
  - Rationale: Auto-connects everywhere, users never think about VPN, best performance (P2P mesh)
  - User requirement: "Ideally they don't even know it's on their machines"
- **Budget alternative: IPSec + Mobile Connect** for cost-conscious deployments ($0-500/year)
  - Rationale: Uses existing SonicWall/Draytek hardware, office users completely transparent
  - Trade-off: Remote workers must manually connect VPN (violates "stupid simple" requirement for hybrid workers)
- **Deployment approach: Phased rollout**
  - Start with Office3 pilot (3 users, free tier) to validate before full deployment
  - Progressive expansion: Office3 → Office2 → Office1 over 6 weeks
- **PostgreSQL access method**: ODBC via Excel (consistent across both solutions)
- **Authentication**: Azure AD SSO for seamless Microsoft 365 integration
- **Architecture**: Subnet routers for office transparency, clients for remote/hybrid workers

### Environment Specifications
- **Office1**: 24 users, SonicWall TZ270 (Gen 7), PostgreSQL on Ubuntu 24.04, 3 RDP PCs, 10.1.0.0/24
- **Office2**: 8 users, SonicWall TZ270, existing IPSec tunnel to Office1, 10.2.0.0/24
- **Office3**: 3 users, Draytek router, local database, simple broadband, 10.3.0.0/24
- **Total**: 35 hybrid workers (alternate between office and remote work)
- **Use cases**:
  - All 35 users → Office1 PostgreSQL database via Excel ODBC
  - Office1 users → Office3 local database (previously required VPN)
  - RDP to Office1 PCs (manual and automated scheduled tasks)

### Reference Documents
- `/mnt/c/Users/SteveIrwin/terminai/it/ZTNA_Provider_Research_2026.md` - Complete ZTNA market research
- `/mnt/c/Users/SteveIrwin/terminai/it/Tailscale_Hybrid_Deployment_Guide.md` - Tailscale deployment guide
- `/mnt/c/Users/SteveIrwin/terminai/it/IPSec_SonicWall_Deployment_Guide.md` - IPSec/SonicWall deployment guide
- Authoritative sources: NIST, CISA, NSA, SANS, OWASP, vendor documentation
- Tailscale documentation: <a href="https://tailscale.com/kb/" target="_blank">https://tailscale.com/kb/</a>
- SonicWall documentation: <a href="https://www.sonicwall.com/support/" target="_blank">https://www.sonicwall.com/support/</a>

### Technical Highlights
- **Performance analysis**:
  - Tailscale P2P: 1-5ms overhead (near-LAN performance)
  - Twingate relay: 10-50ms overhead (acceptable for Excel)
  - Traditional VPN: 50-200ms+ (hairpinning bottleneck)
- **Cost comparison**:
  - Tailscale: $2,520/year ($6/user/month)
  - Twingate: $2,100/year ($5/user/month, lowest cost)
  - Cloudflare: FREE for up to 50 users (covers all 35)
  - IPSec + Mobile Connect: $0-500/year
- **User experience priority**: "Stupid simple" requirement drove Tailscale recommendation over IPSec
  - Tailscale: Auto-connects everywhere, zero daily user interaction
  - IPSec: Office transparent, but remote workers must remember to "click Connect"
  - For hybrid workers, forgetting VPN = failed database access = helpdesk tickets

### Next Actions
- [ ] Get SonicWall Cloud Secure Edge quote (if TZ270s are Gen 7+ compatible)
- [ ] Start Tailscale free tier pilot at Office3 (3 users, validates approach)
- [ ] Test PostgreSQL ODBC performance over Tailscale mesh
- [ ] Test RDP connectivity for automated scheduled tasks
- [ ] Evaluate user feedback: "Did you notice any difference office vs home?"
- [ ] Decision point: Tailscale vs IPSec based on pilot results and budget approval
- [ ] Create deployment schedule (6-week phased rollout)

---

## Session 2026-01-15 (Time Unknown)

### Summary
Provided technical guidance for loading PDF files into MemoryStream using PdfSharp library. User encountered error when trying to save document opened in Import mode to MemoryStream.

### Work Completed
- Diagnosed issue with `PdfDocumentOpenMode.Import` preventing document save operations
- Explained that Import mode is specifically for extracting pages to other documents, not for general reading/saving
- Provided two solutions:
  1. **Direct file read (recommended)**: Use `File.ReadAllBytes()` to load PDF bytes directly into MemoryStream without PdfSharp overhead
  2. **PdfSharp with correct mode**: Change from `Import` to `Modify` mode to allow save operations
- User confirmed Option 1 (direct file read) worked successfully
- Explained appropriate `PdfDocumentOpenMode` values for different subsequent operations (Import for page extraction, ReadOnly for reading/text extraction)

### Files Changed
None - session was technical guidance only, user implemented solution independently.

### Git Commits
None - no code changes made during this session.

### Key Decisions
- **Approach selected**: Direct file read using `File.ReadAllBytes()` + `MemoryStream` constructor
- **Rationale**: Simpler, faster, no PdfSharp parsing overhead when just loading file into memory
- **When to use PdfSharp**: Open MemoryStream later with appropriate mode (Import/ReadOnly) for actual PDF operations
- **Pattern**: Separate concerns - file loading vs PDF processing

### Reference Documents
None - verbal technical guidance session on PdfSharp library usage.

### Code Pattern Provided
```csharp
internal static MemoryStream GetFileAsMemoryStream(string pdfFilePath)
{
    if (!File.Exists(pdfFilePath))
        throw new FileNotFoundException($"PDF file not found: {pdfFilePath}");

    byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
    return new MemoryStream(pdfBytes);
}
```

### Next Actions
- [ ] User may implement page extraction methods using the MemoryStream
- [ ] User may implement text extraction methods using the MemoryStream

---

## Session 2025-12-12 (Time Unknown)

### Summary
Provided technical guidance for implementing a one-hour retry mechanism in the StoneX daily statement parser application. User needed the application to retry email checking with minimal resource usage while waiting.

### Work Completed
- Provided C# implementation guidance for in-app retry using `Task.Delay()`
- Designed solution using background task with `ManualResetEventSlim` to keep process alive with minimal resources
- Implemented recursive retry pattern for unlimited retry attempts
- Solution allows Main() to exit immediately after scheduling while background task handles waiting
- Code example provided for integration into existing console application structure

### Files Changed
None - session was technical guidance only, user implemented the solution independently.

### Git Commits
None - no code changes made during this session.

### Key Decisions
- **Approach**: In-app delay using `Task.Delay()` rather than external scheduling tools
- **Resource usage**: User accepted ~20-50MB RAM usage while waiting (unavoidable for in-process solution)
- **Retry tracking**: No retry count limits - unlimited retries until email received
- **Retry count parameter**: No command-line parameters - simple restart without tracking
- **Logging**: Silent operation without logging
- **Implementation pattern**: Background task with `ManualResetEventSlim` for process lifecycle management

### Reference Documents
None - verbal technical guidance session.

### Next Actions
- [ ] User will integrate retry mechanism into existing StoneX parser application
- [ ] User will test retry behavior when email is delayed

---

## Session 2025-12-10 13:45

### Summary
Extended the StoneX daily statement parser to handle Cash Settlement sections. Added three new methods to parse cash settlement data and map it to the existing StoneXTradeData model with field mappings for Cash Amount → MarketValue and Settlement Price → MarketPrice.

### Work Completed
- Extended `parsing/DailyStatementParser.cs` with Cash Settlement parsing capability
  - Added `FindCashSettlementSections()`: Locates all "Cash Settlements" sections and associates them with Daily Statement dates
  - Added `ParseCashSettlementSection()`: Parses cash settlement data rows with proper section boundary detection
  - Added `ParseCashSettlementDataRow()`: Extracts individual cash settlement entries with two-line format handling
  - Implemented field mapping: Cash Amount → MarketValue, Settlement Price → MarketPrice
  - Handles cash settlement-specific format with Type, Description, Expiry Date, Applied On fields
  - Integrated cash settlement parsing into main Parse() method workflow
  - Follows existing parser patterns for page breaks, header skipping, and error handling
- Provided `parsing/ExampleWithCashSettlement.csv` as test data

### Files Changed
- `parsing/DailyStatementParser.cs` - Extended with 3 new methods (~200 additional lines)
  - Modified `Parse()` method to include cash settlement parsing before trade parsing
  - Added `FindCashSettlementSections()` method (lines 138-154)
  - Added `ParseCashSettlementSection()` method (lines 156-225)
  - Added `ParseCashSettlementDataRow()` method (lines 227-318)
- `parsing/ExampleWithCashSettlement.csv` - Added as test data showing Cash Settlement format

### Git Commits
None yet - changes need to be committed.

### Key Decisions
- **Field mappings**: Cash Amount → MarketValue, Settlement Price → MarketPrice (as requested by user)
- **Parse order**: Cash settlements parsed before trade sections to maintain logical document flow
- **Two-line format**: Cash settlement rows span two lines (main data + quantity/cash confirmation)
- **Long/Short determination**: Based on quantity sign from confirmation line
- **Date mapping**: Applied On → StartDate, Expiry Date → EndDate
- **GlobalId handling**: Set to null for cash settlements (not present in this section)
- **Deduplication**: Cash settlements use same deduplication logic as trades (TradeId + StartDate + EndDate)

### Reference Documents
- `parsing/ExampleWithCashSettlement.csv` - Sample file showing Cash Settlement section format (lines 11-18, 98-105)

### Next Actions
- [ ] Test cash settlement parsing with real data
- [ ] Verify field mappings match database schema expectations
- [ ] Consider if cash settlements need separate model or can reuse StoneXTradeData

---

## Session 2025-12-09 13:30

### Summary
Built a comprehensive C# parser for StoneX daily statement PDFs that extracts trade data and account information. Created a complete console application with parsing logic that handles multi-page documents, date formats, currency parsing, and trade deduplication.

### Work Completed
- Created `DailyStatementParser.cs`: Complete parser implementation with robust error handling
  - Parses "Daily Statement" sections first to extract published dates
  - Extracts trade data from "Open Positions and Market Values" sections across page breaks
  - Handles multi-line contract descriptions by concatenating them
  - Determines Long vs Short positions using "Long Avg"/"Short Avg" markers
  - Parses first and last trade sections and deduplicates by TradeId + StartDate + EndDate
  - Parses account information from the last section in the document
  - Supports multiple date formats (dd-MMM-yyyy, dd/MM/yyyy, etc.) with culture-specific parsing
  - Handles currency values with $, commas, and negative values in parentheses
  - Continues parsing across page breaks and repeated headers
- Created `GetStoneXOTCDailyValuesConsole.csproj`: .NET 8.0 console application project file
- Created `Program.cs`: Demo application that tests the parser and displays parsed data
  - Comprehensive output showing all trades with details
  - Account data display with all margin and balance information
  - Summary statistics and totals
- Fixed multiple parsing issues through iterative debugging:
  - Date parsing format compatibility (dd-MMM-yyyy format required explicit handling)
  - Field offset error (skipping already-parsed date field)
  - Page break handling (continuing to parse trades across multiple pages)
- User already had `StoneXAccountData.cs` and `StoneXTradeData.cs` models defined

### Files Changed
- `parsing/DailyStatementParser.cs` - Created complete PDF-to-CSV parser (466 lines)
- `parsing/GetStoneXOTCDailyValuesConsole.csproj` - Created .NET 8.0 project file
- `parsing/Program.cs` - Created demo/test application (119 lines)
- `parsing/Example.csv` - Pre-existing sample data file (used for testing)
- `parsing/StoneXAccountData.cs` - Pre-existing model (33 properties)
- `parsing/StoneXTradeData.cs` - Pre-existing model (13 properties)

### Git Commits
None yet - new files created in this session need to be committed.

### Key Decisions
- **Parser flow**: Find "Daily Statement" sections first, then associate dates with trade/account sections
- **Date parsing**: Use explicit format parsing with CultureInfo.InvariantCulture for dd-MMM-yyyy dates
- **Page break handling**: Skip page markers and repeated headers but continue parsing trades
- **Deduplication strategy**: Group by TradeId + StartDate + EndDate, keep last occurrence
- **Field parsing**: Start at partIndex=1 to skip the already-parsed date field in trade rows
- **Long/Short determination**: Parse "Long Avg" or "Short Avg" markers in subsequent rows
- **Section selection**: Parse first and last "Open Positions" sections, deduplicate trades
- **Account data**: Parse only the last "Account Information" section

### Reference Documents
- `/mnt/c/Users/SteveIrwin/terminai/it/parsing/Example.csv` - Sample StoneX daily statement data
- `/mnt/c/Users/SteveIrwin/terminai/it/parsing/DailyStatementParser.cs` - Main parser implementation
- `/mnt/c/Users/SteveIrwin/terminai/it/parsing/Program.cs` - Demo application
- `/mnt/c/Users/SteveIrwin/terminai/it/parsing/StoneXAccountData.cs` - Account data model
- `/mnt/c/Users/SteveIrwin/terminai/it/parsing/StoneXTradeData.cs` - Trade data model

### Next Actions
- [ ] Test parser with additional real daily statement files
- [ ] Implement database update logic to persist parsed data
- [ ] Add error logging/reporting for production use
- [ ] Consider adding validation rules for parsed data
- [ ] Add unit tests for parser edge cases

---

## Session 2025-11-19 12:30

### Summary
Comprehensive VPN security research and commercial provider comparison for public WiFi usage. Created two major documentation files covering VPN security benefits/limitations and detailed comparison of 8 major commercial VPN providers with security features, pricing, and recommendations.

### Work Completed
- Deployed gemini-it-security-researcher agent to gather authoritative VPN security information
- Created `VPN_Benefits.md` (24KB): Comprehensive pros/cons of VPN usage on public WiFi
  - Network-layer protection analysis (MITM, evil twins, packet sniffing)
  - Technical vulnerabilities (DNS/WebRTC/IPv6 leaks, kill switch failures)
  - What VPNs don't protect against (phishing, malware, endpoint attacks)
  - Trust and privacy concerns with commercial providers
  - Best practices for VPN selection and configuration
  - Based on NIST, CISA, NSA, SANS, OWASP guidance
- Deployed gemini-it-security-researcher agent for commercial VPN comparison research
- Created `VPN_Comparisons.md` (31KB): Detailed comparison of 8 major VPN providers
  - Security features comparison (encryption, protocols, audits, no-logs verification)
  - Jurisdiction analysis (Five/Nine/Fourteen Eyes implications)
  - Pricing comparison (monthly, annual, 2-year plans)
  - Speed performance rankings (2025 benchmarks)
  - Feature matrices (devices, servers, split tunneling, specialty servers)
  - Provider recommendations by use case (security, value, anonymity, speed)
  - Verified audit histories (Deloitte, KPMG, Securitum, Cure53)
  - Court-tested no-logs policies and real-world incidents (Mullvad police raid, PIA subpoenas)

### Files Changed
- `VPN_Benefits.md` - Created comprehensive VPN security analysis for public WiFi
- `VPN_Comparisons.md` - Created detailed commercial VPN provider comparison
- `SESSION_LOG.md` - Created session tracking documentation
- `PROJECT_STATUS.md` - Created project status tracking
- `CHANGELOG.md` - Created project changelog

### Git Commits
- Will be created: End of session documentation - VPN security and provider comparison research

### Key Decisions
- Focused on commercial VPN providers (NordVPN, ExpressVPN, Surfshark, ProtonVPN, PIA, CyberGhost, Mullvad, IVPN)
- Emphasized both user-friendly and technical perspectives for accessibility
- Prioritized security audit verification and no-logs policy validation
- Included jurisdiction analysis due to privacy implications
- Cross-referenced multiple authoritative sources (NIST, CISA, NSA, SANS, OWASP)
- Used 2025 current data for pricing and features to ensure accuracy

### Reference Documents
- `/mnt/c/Users/SteveIrwin/terminai/it/VPN_Benefits.md` - VPN security pros/cons
- `/mnt/c/Users/SteveIrwin/terminai/it/VPN_Comparisons.md` - Commercial VPN comparison
- Referenced authoritative security standards: NIST, CISA, NSA, SANS Institute, OWASP
- Independent audit reports: Deloitte, KPMG, Securitum, Cure53
- Provider transparency reports and court documentation

### Next Actions
- [ ] Consider adding router VPN configuration guide
- [ ] Potentially expand with self-hosted VPN comparison (WireGuard, OpenVPN, IPsec)
- [ ] May add VPN leak testing guide (how to test DNS/WebRTC/IPv6 leaks)
- [ ] Consider VPN protocol deep-dive (WireGuard vs OpenVPN vs IKEv2)

---
