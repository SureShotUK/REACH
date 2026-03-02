# IT Project Changelog

All notable changes to the IT infrastructure and security documentation project.

## [Unreleased] - 2026-03-02

### Fixed
- Ubuntu 24.04 Server: no internet despite WiFi connected — root cause: static IP netplan config had no `nameservers` block; fixed by adding `addresses: [8.8.8.8, 1.1.1.1]` to wlp8s0 in `/etc/netplan/50-cloud-init.yaml`
- Ethernet NIC (RTL8126) showing UNCLAIMED — installed `realtek-r8126-dkms` via `ppa:awesometic/ppa` (kernel 6.8 r8169 driver doesn't support RTL8126; added in kernel 6.9)
- DKMS module blocked by Secure Boot ("Key was rejected by service") — disabled Secure Boot in MSI BIOS

### Changed
- `/etc/netplan/50-cloud-init.yaml` — full network config: ethernet static 192.168.1.192/24 (metric 100, primary), WiFi static 192.168.1.191/24 (metric 600, fallback), DNS on both interfaces
- `/etc/cloud/cloud.cfg.d/99-disable-network-config.cfg` — created to prevent cloud-init overwriting netplan changes on reboot

### Documentation
- `SESSION_LOG.md` — session entry added covering first-boot troubleshooting
- `PROJECT_STATUS.md` — updated to reflect PC built and running; next priorities updated to NVIDIA/Tailscale/Ollama

---

## [Unreleased] - 2026-02-19 (Session 3)

### Added
- `NewPC/Assembly_Guide.md` — Complete hardware assembly guide for AI PC build:
  - Pre-assembly safety and workspace setup
  - Step-by-step installation: CPU (AM5 LGA) → RAM (A2/B2 dual-channel) → M.2 NVMe drives → case prep → motherboard → PSU → Arctic Liquid Freezer III Pro 360 (front intake) → RTX 3090 GPU
  - Fractal Design Torrent specific: 180mm fan removal, 360mm AIO radiator front-mount configuration
  - Complete cable connections reference (24-pin, EPS 8-pin, 3x PCIe 8-pin GPU, fan headers, front panel)
  - Pre-boot 16-item verification checklist
  - First power-on procedure and expected behaviour
  - BIOS configuration: AMD EXPO Profile 1 (DDR5-6000 CL30), Above 4G Decoding, Resizable BAR
  - Troubleshooting table for common first-boot failure modes
- `NewPC/Software_Setup.md` — Ubuntu 24.04 LTS AI software stack setup guide:
  - Ubuntu 24.04 installation with two-drive partitioning (Drive 1: OS `/`, Drive 2: `/mnt/models`)
  - `/etc/fstab` persistent second-drive mount configuration
  - NVIDIA driver installation and CUDA toolkit verification
  - Docker + NVIDIA Container Toolkit for GPU-enabled containers
  - Ollama with model storage redirected to second drive via systemd environment variable
  - Open WebUI Docker container with GPU passthrough
  - Model recommendations table (7B–70B with VRAM requirements and expected tok/s on RTX 3090)
  - GPU power limit systemd service (300W, reduces noise ~3-5% performance cost)
  - CPU frequency governor, swap configuration for large model offloading
  - UFW firewall rules for LAN-only access (ports 3000, 11434)
  - Auto-start verification, performance benchmarking, quick reference command table
  - Troubleshooting for GPU detection, WebUI connectivity, memory errors, thermal issues

---

## [Unreleased] - 2026-02-19 (Session 2)

### AI PC Build Project - BUILD COMPLETE

#### Purchased
- **AMD Ryzen 9 7900X + thermal paste** @ £322.50
- **Thermaltake Toughpower GF3 1650W PSU** @ £218.00 (£22 under estimate)
- **Fractal Design Torrent case** @ £169.99 (£5 under estimate)
- **Samsung 9100 Pro 2TB PCIe 5.0 x2** @ £502.00 total (Samsung direct — Gen5 at Gen4 price)
- **Arctic Liquid Freezer III Pro 360** @ £72.00 (Pro version: 7-blade P12 fans, 38mm radiator, 4-10°C better than standard III)
- **140mm rear exhaust fan** @ £21.12

#### Total Build Spend: £2,874.98

#### Key Decisions Documented
- **Storage**: Gen5 NVMe chosen at same price as Gen4 — no AI inference benefit but no cost penalty
- **RAID**: NAS backup strategy over RAID 1 — AI model files are re-downloadable, preserves 4TB usable
- **Cooling**: 360mm AIO beats NH-D15 by 5-10°C under sustained loads (relevant for 24/7 AI inference)
- **Mounting**: Fractal Torrent has no top radiator mount — AIO front-mounted, replaces 180mm fans
- **Fan layout**: Front 3x120mm (AIO) + Bottom 3x140mm (included) + Rear 1x140mm (new). 2x180mm spare.
- **Dual GPU**: Different RTX 3090 makes are compatible for AI workloads. Founders Edition uses 12-pin adapter.

#### Changed
- `NewPC/Final_Build.md` - Updated v1.2 → v1.6:
  - All 10 components marked purchased with actual prices
  - PSU, Case, CPU Cooler, Rear Fan sections replaced with selected spec tables
  - Storage section updated to Samsung 9100 Pro Gen5 x2
  - Cost summary updated to £2,874.98 final total
  - Build status set to COMPLETE
  - Full fan configuration table documented
  - Decision log updated with all purchases and rationale

---

## [Unreleased] - 2026-02-19 (Session 1)

### AI PC Build Project - Major Components Purchased

#### Purchased
- **Asus TUF Gaming OC RTX 3090 24GB** @ £699.39
  - 24GB GDDR6X VRAM (936 GB/s bandwidth)
  - Triple axial-tech fans with 2.9-slot cooling solution
  - Factory overclocked: 1,860 MHz boost clock
  - MaxContact technology: 2x larger copper contact vs reference
  - Dual BIOS: Performance and Quiet modes

- **G.SKILL Trident Z5 Neo RGB 64GB DDR5-6000 CL30** @ £599.99
  - Capacity: 64GB (2x32GB) for dual-channel operation
  - Speed: DDR5-6000 MT/s (optimal for Ryzen 7000 series)
  - Timings: CL30-36-36-96 (10ns true latency)
  - AMD EXPO certified for one-click BIOS setup
  - Upgraded from planned CL36 to CL30 for £0.99 more (16% better latency)

- **MSI MAG X870E TOMAHAWK WIFI** @ £269.99
  - Chipset: AMD X870E (latest premium tier, 2024)
  - VRM: 16+2+1 phase @ 80A (1,280W capacity for 24/7 AI workloads)
  - Dual GPU: x8/x8 PCIe 5.0 configuration (balanced for future second RTX 3090)
  - Connectivity: Wi-Fi 7, USB4 (40Gbps), 5Gb Ethernet
  - Memory: DDR5-8400+ support, up to 192GB
  - Storage: 4x M.2 Gen5 slots
  - Resolved availability issue: ASRock X670E Taichi out of stock/£500+

#### Changed
- `NewPC/Final_Build.md` - **Comprehensive updates** (Version 1.1 → 1.2):
  - Build status updated: 3 components purchased, 6 of 9 finalized (66% complete)
  - Decision summary expanded to include all 6 major components
  - Motherboard section completely rewritten for MSI X870E (replaced ASRock X670E Taichi)
  - Added VRM explanation section (what VRM is, phases, amperage, importance for 24/7 workloads)
  - Added chipset tier comparison (X870E vs X870 vs X670E hierarchy)
  - GPU section enhanced with Asus TUF Gaming OC specifications and features
  - RAM section enhanced with G.SKILL Trident Z5 Neo RGB technical details
  - Cost summary updated: £1,569.37 purchased, £985-1,240 remaining
  - Decision log expanded with motherboard evaluation details (3 alternatives compared)
  - Added X870E vs X670E advantages comparison
  - Updated purchase tracking with delivery status

#### Documentation
- **Technical Explanations Added** to Final_Build.md:
  - **VRM (Voltage Regulator Module)**:
    - What it is and how it works (converts 12V to CPU voltage 0.9-1.4V)
    - Phase count explanation (16+2+1 = 19 power delivery components)
    - Amperage rating (80A per phase = 1,280W theoretical capacity)
    - Why it matters for 24/7 AI workloads (prevents throttling, ensures stability)
    - Analogy: Phases like cylinders in engine (more = smoother, cooler operation)

  - **AMD Chipset Hierarchy**:
    - "E" suffix meaning: "Extreme" = premium tier with more PCIe 5.0 lanes
    - Tier comparison: X870E (premium, 2024) > X670E (premium, 2022) > X870 (mid-tier, 2024)
    - Why newer doesn't always mean better (X870 < X670E for dual GPU support)
    - Feature differences: PCIe 5.0 lane allocation, dual GPU support capabilities

  - **PCIe Configurations for Dual GPU**:
    - x16/x8 vs x8/x8 bandwidth comparison
    - Performance impact for AI inference: <2% difference (GPU-bound workload)
    - x8/x8 balanced configuration benefits for equal dual GPU performance
    - PCIe 4.0 x8 = 15.75 GB/s bandwidth (sufficient for model loading)

#### Key Decisions Documented
- **Motherboard Selection Process**:
  - Evaluated 3 alternatives: MSI X670E @ £229.99, ASUS X870 @ £274.99, MSI X870E @ £269.99
  - Selected MSI X870E: Best chipset (premium X870E) at best price (£5 less than mid-tier ASUS X870)
  - Savings: £230+ vs ASRock X670E Taichi at inflated £500+ pricing
  - Justification: £40 premium over X670E option justified by Wi-Fi 7, USB4, 5Gb Ethernet, latest chipset

- **RAM Upgrade Decision**:
  - Chose G.SKILL Trident Z5 Neo RGB CL30 over CL36 for £0.99 more
  - 16% better latency (10ns vs 12ns true latency)
  - Best value decision - premium performance at budget pricing

- **Dual GPU Requirement**: User confirmed intention to add second RTX 3090 later
  - Motherboard x8/x8 PCIe 5.0 configuration verified as perfect for dual GPU
  - No performance penalty vs x16/x8 for AI workloads

#### Next Actions
- Purchase remaining components: CPU (Ryzen 9 7900X, £320-380), PSU (Thermaltake 1650W, £240), Case (Fractal Torrent, £175)
- Finalize storage decision: Samsung 990 Pro or WD Black SN850X (2TB NVMe Gen4, £140-180)
- Finalize cooler: 280mm AIO (Arctic Liquid Freezer II recommended, £90-150)
- Finalize fans: 2-3x 140mm PWM (£20-55)
- Assembly timeline: Ready in 2-4 weeks once all components arrive

---

## [Unreleased] - 2026-02-16

### Added
- `troubleshooting/Diagnose-OutlookTemplate.ps1` (6.8KB, 190 lines): Character diagnostic tool for Outlook templates
  - Uses Outlook COM automation to extract and analyze template body text
  - Searches for 7 problematic character types with Unicode code point identification
  - Shows count, context (20 chars before/after), and position for each character
  - Full character code map displaying all non-printable and special Unicode characters by line
  - Color-coded output: [FOUND] (red) for problematic characters, [OK] (green) for clean
  - Summary with recommendations for cleaning
  - Explains potential causes of £ symbol encoding issues (RTF metadata, encoding settings, keyboard mismatch)

- `troubleshooting/Clean-OutlookTemplateEncoding.ps1` (9.6KB, 290 lines): Enhanced encoding-focused template cleaning script
  - Supports both single file and directory of templates (all .oft files in folder)
  - Creates automatic timestamped backups: `Backups_YYYYMMDD_HHMMSS/` subdirectory
  - Uses Outlook COM automation for safe binary .oft file manipulation
  - Removes 6 problematic character types:
    - Soft Hyphen (U+00AD) - removed completely
    - Zero Width Space (U+200B) - removed
    - Zero Width No-Break Space (U+FEFF / BOM) - removed
    - Form Feed (U+000C) - removed
    - Vertical Tab (U+000B) - removed
    - Non-Breaking Space (U+00A0) - replaced with regular space (not removed)
  - Applies Outlook registry fixes for UTF-8 encoding: DefaultCharSet=utf-8, DisableAutoArchive
  - Color-coded logging: ERROR (Red), WARNING (Yellow), SUCCESS (Green), INFO (White)
  - `-BackupOnly` parameter for safe testing without modification
  - Per-template processing with individual success/failure tracking
  - Graceful error handling with automatic backup restoration on failure
  - Thorough COM cleanup: ReleaseComObject, GC.Collect, WaitForPendingFinalizers
  - Processing summary showing templates processed, backups created, templates cleaned

- `troubleshooting/Outlook_Template_Encoding_Issues.md` (14KB, 600+ lines): Comprehensive technical documentation for encoding issues
  - **Issue summary**: Question marks (?????) in templates, £ symbol becomes ? when typed
  - **Environment specifications**: Windows 11, M365 Apps, .oft file format
  - **Deep technical explanation**:
    - Compound File Binary Format (CFBF) structure - OLE container with streams
    - UTF-16 Little Endian encoding details - why NUL appears after every character (normal behavior!)
    - Soft hyphen (U+00AD) characteristics and rendering issues
    - Why manual editing in Notepad++ corrupts files (breaks CFBF structure)
    - Why Find/Replace doesn't work in text editors (searching binary container, not parsed RTF text)
    - The "NUL-NUL-NUL-NUL-NUL" mystery solved: Five soft hyphens in UTF-16 LE misread as ANSI
  - **Symptoms**: Primary symptoms in Outlook and when viewing in Notepad++ (ANSI vs UTF-8)
  - **Root cause analysis**: UTF-16 LE encoding + soft hyphens + RTF metadata issues
  - **Resolution steps** (4-step verification process):
    - Step 1: Run Diagnose-OutlookTemplate.ps1 to confirm character types
    - Step 2: Run Clean-OutlookTemplateEncoding.ps1 to remove problematic characters
    - Step 3: Test template in Outlook (verify question marks gone, test £ symbol)
    - Step 4: If £ still broken, 3 solutions provided (recreate template, force UTF-8 registry, language settings)
  - **Prevention best practices**: Template creation in Outlook (not Word), avoid web copy/paste, use plain text paste, regular maintenance
  - **File lock troubleshooting section**:
    - Problem: "Can't open template after cleaning" - file locks explained
    - Causes: COM automation, Windows Search, Defender, Explorer thumbnail cache, background Outlook
    - Solutions (ordered by effectiveness): Restart computer (recommended), kill processes, wait, use backup, copy to new filename
    - Prevention in scripts: Process checks, thorough COM cleanup
  - **Script usage reference**: Both Diagnose and Clean scripts with parameters, requirements, expected output
  - **Lessons learned section**: For IT support (binary formats, encoding verification, iterative debugging, file locks) and for users (don't edit in text editors, test before deployment, keep backups)
  - **Technical references**: Unicode character definitions, Microsoft KB articles, Outlook API docs, CFBF specification

- `troubleshooting/Test-TemplateFileLock.ps1` (7.5KB, 240 lines): File lock diagnostic and unlock utility
  - Tests if template file is locked using System.IO.File.Open attempt with exclusive access
  - Returns clear [LOCKED] or [NOT LOCKED] status with color-coding
  - Identifies suspect processes that may be holding locks:
    - OUTLOOK, OfficeClickToRun, explorer, SearchIndexer, SearchProtocolHost, MsMpEng (Defender), OneDrive
  - Shows process names and PIDs for all running suspect processes
  - Attempts to use Sysinternals Handle.exe for detailed lock information (if available at C:\Tools\handle.exe)
  - Provides 4 solution options with ready-to-use commands:
    - Option 1: Restart computer (most reliable) - includes Restart-Computer command
    - Option 2: Kill processes and wait - lists specific processes to kill with commands
    - Option 3: Use backup copy - shows latest backup directory and file path
    - Option 4: Copy to new filename - provides copy command with suggested filename
  - `-AttemptUnlock` parameter to automatically kill suspect processes and re-test lock
  - Re-tests file lock after killing processes to verify unlock success
  - Color-coded output: Cyan (headers), Red (locked status), Green (unlocked status), Yellow (warnings)
  - Finds latest backup directory automatically and displays backup file path
  - Helpful usage tips: Shows -AttemptUnlock usage example if user didn't use it

### Changed
- Enhanced understanding of Outlook template encoding issue root cause (deepened from Feb 13 session)
- Documented UTF-16 Little Endian encoding behavior as normal (NUL after every character is expected)
- Clarified that "NUL-NUL-NUL-NUL-NUL" pattern = five soft hyphens in UTF-16 LE misread when opened in ANSI mode
- Explained why Notepad++ Replace function fails (searches binary CFBF format, not parsed RTF text)
- Identified £ symbol issue as separate from soft hyphen issue (RTF encoding metadata or codepage mismatch)
- Documented file lock behavior as expected Windows COM automation pattern (not a script bug)

### Fixed
- N/A - No bugs fixed this session, focus was on enhanced diagnostics and documentation

### Documentation
- `troubleshooting/Outlook_Template_Encoding_Issues.md` - Comprehensive technical guide explaining binary file format behavior
- Session log updated with detailed root cause analysis and key discoveries
- Project status updated to reflect enhanced diagnostic tooling and ongoing issue status
- Changelog updated with all new files and documentation

## [Unreleased] - 2026-02-13

### Added
- `troubleshooting/Clean-OutlookTemplates.ps1` (376 lines, 15KB): PowerShell automation script for cleaning problematic Unicode characters from Outlook templates
  - Searches default Outlook template locations or user-specified path (folder or single file)
  - Creates automatic timestamped backups before any modification
  - Uses Outlook COM automation to safely manipulate binary .oft files (Compound File Binary Format)
  - Removes soft hyphens (U+00AD) and other control characters (Form Feed, Vertical Tab, etc.)
  - Applies UTF-8 registry fixes: AutoDetectCharset=0, SendCharset=65001, DisableCharsetDetection=1
  - Comprehensive logging with color-coded output (ERROR, WARNING, SUCCESS, INFO)
  - Parameters: `-TemplatePath` (optional), `-BackupOnly` (test mode), `-ApplyRegistryFix`
  - Validates Outlook is closed before running
  - Automatic backup restoration on error
  - COM object cleanup and garbage collection

- `troubleshooting/Outlook_Template_Unicode_Encoding_Question_Marks.md` (26KB): Comprehensive technical documentation for Outlook template question mark issue
  - Root cause analysis: Soft hyphen characters (U+00AD) triggering Microsoft Outlook Build 19628.20150+ Unicode encoding bug
  - Encoding cascade effect explanation: Single soft hyphen poisons entire email encoding (UTF-8 → Windows-1252 incorrect switch)
  - Character identification: U+00AD / 0xAD / `­` / &shy; / &#173; (soft hyphen)
  - 5 solution methods with success rates and risk assessments:
    1. PowerShell automation (95% success, low risk) - RECOMMENDED
    2. Recreate template from scratch (100% success, high effort)
    3. Manual character removal in Outlook (60% success, doesn't prevent recurrence)
    4. Registry fixes only (40% success, doesn't clean existing templates)
    5. Manual binary editing in Notepad++ (0% success - CORRUPTS FILE)
  - Prevention best practices: Avoid copying from web, use plain text paste (Ctrl+Shift+V)
  - Compound File Binary Format (CFBF) technical explanation - why .oft files cannot be edited as text
  - Troubleshooting common issues: File locks, Outlook won't open, permission errors
  - FAQ section with technical deep-dive
  - Microsoft KB references and official bug acknowledgment

- `troubleshooting/SCRIPT_USAGE_GUIDE.md` (11KB): Step-by-step PowerShell script usage instructions
  - Prerequisites and safety checks (close Outlook before running)
  - PowerShell execution policy setup instructions
  - 3 usage scenarios with command examples:
    - Default: Clean all templates in standard Outlook locations
    - Specific folder: Clean only templates in specified folder
    - Specific file: Clean only one template file
  - Parameter reference with detailed explanations
  - Step-by-step breakdown of what the script does
  - Expected output interpretation and log file analysis
  - Troubleshooting section: Execution policy errors, file locks, permissions, path not found
  - Rollback procedure using automatic backups
  - Quick reference card for common commands
  - Next steps after running script (test templates, send emails with £ and ® symbols)

- `troubleshooting/CleanTemplates_20260213_160637.log` - First script run log (failed with string replacement bug)
- `troubleshooting/CleanTemplates_20260213_162346.log` - Second script run log (found 0 characters - wrong character codes)
- `troubleshooting/CleanTemplates_20260213_165614.log` - Third script run log (SUCCESS - removed 5 soft hyphens, applied registry fixes)
- `troubleshooting/Backups_20260213_162346/` - Automatic backup directory from second run
- `troubleshooting/Backups_20260213_165614/` - Automatic backup directory from successful third run

- `troubleshooting/CLAUDE.md` (380 lines, 12.4KB): Comprehensive IT helpdesk and troubleshooting guidance
  - Systematic 7-step troubleshooting methodology: Initial Intake → Diagnosis → Research → Hypothesis → Testing → Resolution → Prevention
  - Target environment specifications (Windows 11 latest, Azure AD domain-joined, Microsoft 365 desktop apps)
  - Essential Windows troubleshooting commands (System Info, Network Diagnostics, Service/Process, M365/Office, Azure AD)
  - PowerShell diagnostic commands for all common scenarios
  - Issue documentation standards with detailed markdown template
  - Priority levels (P1 Critical, P2 High, P3 Medium, P4 Low) with business impact criteria
  - Escalation criteria for complex or infrastructure-level issues
  - Communication standards for diagnostic questions and solution explanations
  - Best practices for systematic troubleshooting and knowledge base building
  - Common issue categories: M365/Office, Windows Update, Network, Azure AD, Performance

- `troubleshooting/README.md` (95 lines, 2.9KB): IT troubleshooting quick start guide
  - Quick start workflow for issue diagnosis
  - Standard environment definition reference
  - Issue category organization structure (office-apps, outlook, onedrive, teams, windows-update, network, vpn, azure-ad, authentication)
  - Documentation template with structured sections
  - Issue index for tracking resolved problems

- `.claude/agents/gemini-it-helpdesk-researcher.md` (240 lines, 8.3KB): Specialized Gemini research agent for IT helpdesk
  - Research methodology prioritizing Microsoft official sources (Learn, Support, KB articles, TechCommunity)
  - Prevalence assessment framework (Widespread, Common, Uncommon, Rare, Isolated)
  - Root cause analysis with primary causes, contributing factors, and triggering events
  - Verified solution ranking by source authority, success rate, and risk level
  - Structured output format with issue identification, prevalence, root cause, solutions, and resources
  - Search query strategies optimized for Windows 11, Office 365, and Azure AD issues
  - Verification standards to ensure reliable, actionable solutions
  - Red flags to avoid (sketchy tools, unexplained registry hacks, outdated solutions)
  - Escalation indicators (no verified solution, security implications, data loss risk)
  - Special considerations for M365 desktop apps (Click-to-Run vs MSI, update channels) and Azure AD devices

### Changed
- `troubleshooting/README.md` - Updated with first resolved issue entry in issue index:
  - Added "Outlook Template Question Marks (Unicode Encoding)" under Microsoft Office category
  - Links to technical documentation, PowerShell script, and usage guide
  - Status: Resolved (2026-02-13)

### Fixed
- **PowerShell Script Bug #1**: Duplicate -Verbose parameter error
  - Cause: Manually defined `-Verbose` when `[CmdletBinding()]` already provides it automatically
  - Fix: Removed manual `-Verbose` parameter definition from `param()` block

- **PowerShell Script Bug #2**: String replacement type conversion error
  - Error: "Cannot convert argument 'newChar', with value: '', for 'Replace' to type 'System.Char'"
  - Cause: Using `.Replace($char, '')` where second parameter must be Char type
  - Fix: Changed to `$cleanedText.Replace($char.ToString(), '')` to use string Replace() method

- **PowerShell Script Bug #3**: Wrong character codes searched
  - Symptom: Script found 0 problematic characters despite visible question marks in sent emails
  - Cause: Searching for Form Feed (0x000C), Vertical Tab (0x000B) but missing Soft Hyphen (0x00AD)
  - User feedback: Notepad++ shows `­` symbol (soft hyphen character)
  - Fix: Added `[char]0x00AD` as FIRST item in `$controlChars` array
  - Result: Third script run successfully removed 5 soft hyphens from template

- **Microsoft Outlook Template Issue**: Question marks (?????) appearing in sent emails with £ and ® symbols
  - Root cause: Soft hyphen characters (U+00AD) triggering Outlook Build 19628.20150+ encoding bug
  - Encoding cascade: Soft hyphen + UTF-8 characters → incorrect Windows-1252 switch → question marks
  - Solution: PowerShell script removed 5 soft hyphens + UTF-8 registry fixes applied
  - Status: Template cleaned successfully (file lock issue pending user resolution)

### Documentation
- **First Real-World Issue Resolved**: Validated IT troubleshooting system with successful diagnosis and resolution
- Followed systematic 7-step framework: Intake → Diagnosis → Research → Hypothesis → Testing → Resolution → Prevention
- Created comprehensive knowledge base entry with technical documentation, automation script, and usage guide
- Documented encoding cascade effect and CFBF binary format limitations (why manual editing fails)
- Captured debugging journey: 3 script iterations with bug fixes based on testing feedback
- Updated SESSION_LOG.md with Session 2026-02-13 (16:00) entry - complete issue resolution documentation
- Updated PROJECT_STATUS.md:
  - Changed status from "ready for first issue" to "validated with first successful resolution"
  - Added first resolved issue details to IT Troubleshooting & Helpdesk section
  - Added Recently Completed session entry (16:00) with Outlook Unicode bug resolution
  - Added resolved issue files to Key Files & Structure section
- Updated SESSION_LOG.md with Session 2026-02-13 (15:00) entry
- Updated PROJECT_STATUS.md with IT Troubleshooting & Helpdesk active work area
- Updated PROJECT_STATUS.md "Last Updated" to 2026-02-13
- Updated PROJECT_STATUS.md "Recently Completed" section with troubleshooting system
- Updated PROJECT_STATUS.md "Key Files & Structure" section with troubleshooting files
- Updated PROJECT_STATUS.md "Specialized Agents" section with gemini-it-helpdesk-researcher

---

## [Unreleased] - 2026-02-12

### Added
- `NewPC/CLAUDE.md` (239 lines): NewPC project-specific guidance for AI PC build planning
  - Project purpose: Local LLM inference for coding assistance and homework help
  - Target audience definition (competent but not expert users)
  - Documentation requirements for AI-specific hardware considerations
  - Research standards: verify all links with WebFetch, cite authoritative sources
  - Decision-making methodology: funnel approach from broad market research to specific recommendations
  - Cost-capability balance guidelines for AI workload optimization
  - Common term exceptions (RAM, VRAM, DDR, HDD, NVME don't need definition)

- `NewPC/PCBuildResearch.md` (1,055 lines, 74KB): Comprehensive AI PC hardware market research
  - 5 complete PC build configurations with full component specifications
  - GPU performance comparison table: tokens per second benchmarks for 8B and 70B models
  - VRAM requirements by model size (7B to 200B+) with quantization considerations
  - AMD vs NVIDIA GPU comparison for AI inference workloads
  - CPU, RAM, and storage performance impact analysis for LLM inference
  - Software stack comparison: Ollama vs LM Studio vs Open WebUI
  - Price breakdowns by tier with pros/cons for each configuration
  - Recommendations by use case (coding, homework help, both)
  - All pricing converted to UK market (GBP with 20% VAT)
  - All external links verified with WebFetch tool

- `NewPC/Chosen_Build.md` (709 lines, 55KB): Deep technical component analysis with architectural insights
  - **Critical PCIe Reality Check**: Consumer AMD Ryzen limited to x16/x8 dual GPU (not x16/x16)
  - Performance comparison table showing <2% difference between x16/x16 and x16/x8 for LLM inference
  - Why x16/x8 limitation doesn't matter: GPU VRAM bandwidth (936 GB/s) is primary bottleneck, not PCIe bandwidth
  - Three AMD motherboard + CPU combinations with detailed PCIe lane configurations
  - Added AMD Ryzen 9 9950X3D option (latest Zen 5 X3D with 144MB 3D V-Cache)
  - Removed all Intel options per user request (focus on AMD platform)
  - Component performance deep-dives:
    - GPU: Memory bandwidth impact on inference speed
    - CPU: Why AI inference is GPU-bound (CPU mainly feeds data)
    - RAM: Speed vs capacity trade-offs (64GB DDR5-6000 optimal)
    - Motherboard VRM: Sustained 24/7 AI workload requirements
    - Storage: Gen4 vs Gen5 impact on model loading times
  - Bottleneck analysis identifying GPU VRAM as primary limitation for inference performance
  - Thermal management and power supply requirements for dual GPU configurations

- `NewPC/Final_Build.md` (745 lines, 30KB): Component selection tracker with decision log and UK pricing
  - **Confirmed Components** with detailed specifications and pricing:
    - CPU: AMD Ryzen 9 7900X @ £320-380 (12 cores, 24 threads, proven Zen 4 architecture)
    - Motherboard: ASRock X670E Taichi @ £280-340 (24+2+1 VRM, 4x M.2, x16/x8 PCIe)
    - RAM: 64GB (2x32GB) DDR5-6000 CL36 @ £599 (Overclockers UK best price)
    - PSU: Thermaltake Toughpower GF3 1650W @ £240 (Scan.co.uk, ATX 3.0, 9x PCIe, 10-year warranty)
    - Case: Fractal Design Torrent @ £175 (best GPU airflow, 2x 180mm front fans, 461mm GPU clearance)
  - **GPU Options** with detailed model comparison:
    - ASUS TUF RTX 3090 24GB (Tier 1 recommendation for 24/7 AI workloads)
    - eBay UK market analysis (£550-750 typical pricing)
    - CeX warranty option (24-month coverage)
    - Model comparison: ASUS TUF vs EVGA FTW3 vs Founders Edition
  - **Decision Log** tracking all component choices with dates, rationale, and alternatives considered
  - **Cost Summary** with running total and budget tracking (£1,614-1,734 confirmed + £600-900 remaining)
  - **UK Market Pricing Research**:
    - Overclockers UK: £599 for DDR5-6000 CL36 (best RAM price found)
    - Scan.co.uk: £716.99 for DDR5-6000 CL40 (worse latency, avoid)
    - CCL Computers: £699.99 for same spec (£100 more expensive)
  - **Purchase Order Recommendations** with retailer priority list (Scan.co.uk, Amazon UK, CCL, Overclockers)
  - **Assembly Notes** and software setup plan (Ubuntu 24.04 LTS or Windows 11 + WSL2)
  - **Expected Performance**: Single GPU (7B-70B @ 42-120 tok/s), Dual GPU future (70B-405B @ 8-75 tok/s)

### Changed
- Budget revised from £1,500-1,800 to £2,200-2,400 due to UK RAM pricing discovery
  - Initial RAM estimate: £250-350 based on US market conversion
  - UK market reality: £599-717 for 64GB DDR5-6000 (Overclockers UK @ £599 is best value)
  - User feedback: "Memory has really shot up in price recently"

- All pricing converted from USD to GBP including 20% VAT for UK market accuracy
  - Updated all retailer sources to UK-specific (Scan.co.uk, Overclockers UK, Amazon UK, CCL Computers)
  - Verified current UK market pricing for all components

- Updated `CLAUDE.md` C# development preferences from .NET 8.0/C# 12 to .NET 10/C# 14
  - Financial Data Processing Projects section: Now references .NET 10 with C# 14 features
  - Development Preferences section: Now specifies .NET 10 (LTS release, supported until November 2028)
  - Added C# 14 features: extension members, field keyword for backing field access, enhanced lambda parameter modifiers

### Documentation
- Deployed gemini-researcher agent for comprehensive AI PC hardware research
  - Cross-referenced multiple authoritative sources: Tom's Hardware, r/LocalLLaMA, Puget Systems, Hardware Corner
  - Verified GPU benchmarks from Hardware Busters, RunPod, Local AI Master
  - Researched real-world user builds and reviews from r/LocalLLaMA community

- Research methodology: Funnel approach from broad market survey to specific recommendations
  - Phase 1: Market overview (all GPU options, all motherboard options)
  - Phase 2: Narrow to 2-3 options per component based on performance/value
  - Phase 3: Verify UK availability and pricing
  - Phase 4: User decision with clear pros/cons for each option

- Key technical discoveries:
  - Consumer AMD Ryzen cannot do true x16/x16 dual GPU (24 PCIe lanes max = x16/x8)
  - x16/x8 configuration has <2% performance impact for LLM inference (verified with benchmarks)
  - GPU memory bandwidth (936 GB/s on RTX 3090) is primary bottleneck, not PCIe bandwidth
  - UK RAM pricing 2-3x higher than US market (£599 vs $250-280 USD equivalent)
  - Used RTX 3090 24GB @ £600-700 offers best tokens/$ value vs £1,999 RTX 5090

- User preferences documented:
  - Build philosophy: "Best bang for buck, add not replace"
  - Buy quality once, upgrade by addition (second GPU) not component replacement
  - Dual GPU upgrade path essential from day one
  - UK market pricing only (GBP with VAT)
  - Warranty preferred but eBay acceptable for best value

- Verified and documented current .NET version (10.0.3, released February 2026)
- Verified and documented C# 14 features and release information

## [Unreleased] - 2026-02-06

### Added
- `ZTNA_Provider_Research_2026.md` (58 pages, 1,057 lines): Comprehensive Zero Trust Network Access market research
  - Evaluated 6 ZTNA providers: Tailscale, Twingate, Cloudflare Zero Trust, ZeroTier, NordLayer, SonicWall Cloud Secure Edge
  - Budget-focused analysis under $10/user/month ($350/month for 35 users)
  - Detailed pricing comparison: Tailscale ($2,520/year), Twingate ($2,100/year), Cloudflare (FREE-$2,940/year), IPSec ($0-500/year)
  - Feature comparison matrices: Site-to-site networking, RDP support, ODBC database access, Azure AD SSO integration
  - Performance benchmarks for PostgreSQL queries: P2P mesh (1-5ms), relay (10-50ms), traditional VPN (50-200ms+)
  - Top recommendations with cost-benefit analysis for 35-user, 3-office hybrid environment
  - Special consideration for SonicWall TZ270 Gen 7+ native ZTNA capability
  - Deployment approach comparison (minimal change, full migration, hybrid with SonicWall)

- `Tailscale_Hybrid_Deployment_Guide.md` (58 pages, 1,852 lines): Complete Tailscale deployment guide for hybrid workers
  - Architecture overview: Peer-to-peer mesh networking with subnet routers at all 3 offices
  - Phase 1: Pilot deployment at Office3 with free tier (3 users, 2 weeks validation)
  - Phase 2: Subnet router deployment at Office1 and Office2 (transparent office access)
  - Phase 3: Azure AD SSO integration with Microsoft 365 (one-time sign-in, inherits MFA)
  - Phase 4: Client deployment via Intune/GPO with auto-install configuration
  - Phase 5: ACL configuration for granular access control
  - PostgreSQL ODBC configuration: psqlODBC driver setup, DSN configuration, Excel Power Query integration
  - RDP configuration: Host setup, MagicDNS for easy hostnames, performance optimization
  - Testing & validation checklist: Office and remote scenarios, performance benchmarks, failover testing
  - 1-page user quick-start guide: "Sign in once with Microsoft, you're done"
  - Comprehensive troubleshooting: Connectivity issues, performance problems, ACL enforcement, Azure AD auth failures
  - Maintenance & operations: Daily/weekly/monthly/quarterly/annual procedures
  - Appendices: CLI reference, cost summary, support resources

- `IPSec_SonicWall_Deployment_Guide.md` (53 pages, 1,495 lines): Traditional IPSec + SSL VPN deployment guide
  - Architecture overview: Site-to-site IPSec tunnels + SonicWall Mobile Connect for remote access
  - Phase 1: IPSec tunnel configuration (Office1 TZ270 ↔ Office3 Draytek)
    - Detailed SonicWall configuration: VPN policy, NAT exemption, access rules
    - Detailed Draytek configuration: VPN profile, IKE/IPsec proposals, firewall rules
  - Phase 2: Optional IPSec tunnel (Office2 ↔ Office3 for redundancy)
  - Phase 3: Routing configuration for all three offices
  - Phase 4: SonicWall Mobile Connect SSL VPN setup
    - SSL VPN server configuration, IP address pools
    - Local user accounts or Azure AD/SAML SSO integration
    - Split tunneling vs full tunnel configuration
  - Phase 5: Client deployment via Intune/GPO or manual installation
  - PostgreSQL ODBC configuration (identical to Tailscale approach)
  - RDP configuration and optimization
  - Testing & validation: Office users (transparent), remote users (manual VPN), hybrid workers
  - Remote worker user guide: "Connect VPN first, then work" with troubleshooting
  - Comprehensive troubleshooting: Tunnel connectivity, remote user authentication, performance optimization
  - Maintenance & operations procedures
  - Appendices: SonicWall/Draytek CLI commands, cost summary ($0), comparison to Tailscale

### Documentation
- Deployed `gemini-it-security-researcher` agent for ZTNA market research with focus on:
  - Budget constraints (under $10/user/month)
  - Small business requirements (35 users, 3 offices)
  - Specific use cases: PostgreSQL ODBC via Excel, RDP access, site-to-site networking
  - Hybrid worker support (office and remote access with minimal user interaction)
  - Integration with existing infrastructure (SonicWall TZ270, Draytek router)
- Cross-referenced authoritative sources: NIST, CISA, NSA, SANS Institute, OWASP
- Verified vendor documentation: Tailscale, Twingate, Cloudflare, SonicWall, Draytek
- Included 2026 current pricing and feature data
- Optimized for "stupid simple" user experience requirement (auto-connect, zero daily interaction)

### Changed
- Updated project focus to prioritize ZTNA deployment planning alongside existing areas
- Tailscale recommended over traditional VPN for hybrid workers due to seamless experience
  - Cost: $2,520/year vs $0-500/year
  - User experience: Auto-connects everywhere vs manual "Connect VPN" button
  - Performance: P2P mesh (10-50ms) vs hub-spoke (50-150ms)
  - Decision rationale: Hybrid workers forgetting VPN = failed access = helpdesk tickets

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
