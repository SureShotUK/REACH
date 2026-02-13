# PowerShell Script Usage Guide - Clean-OutlookTemplates.ps1

Quick reference guide for using the Outlook template cleaning script.

---

## Prerequisites

- **Windows 11** with PowerShell (included by default)
- **Outlook for Microsoft 365** (Classic Outlook)
- **Administrator privileges** (for registry fixes)

---

## Before You Run the Script

### Critical: Close Outlook

The script **WILL NOT RUN** if Outlook is open. Close Outlook completely before proceeding.

---

## Quick Start (Recommended)

### Option 1: Clean All Templates + Apply Registry Fix

This is the **recommended approach** for most users:

```powershell
# 1. Close Outlook completely

# 2. Open PowerShell as Administrator
#    (Right-click PowerShell → Run as Administrator)

# 3. Navigate to troubleshooting folder
cd "C:\Users\[YourUsername]\terminai\it\troubleshooting"

# 4. Set execution policy (one-time per session)
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

# 5. Run the script with registry fix
.\Clean-OutlookTemplates.ps1 -ApplyRegistryFix
```

**What this does**:
- ✅ Finds all .oft templates automatically
- ✅ Creates timestamped backups
- ✅ Removes problematic control characters (Form Feed, Vertical Tab, ¬)
- ✅ Applies UTF-8 encoding registry fixes
- ✅ Creates detailed log file

**Duration**: 2-5 minutes (depends on number of templates)

---

## Advanced Options

### Option 2: Backup Only (Testing)

Create backups without modifying templates:

```powershell
.\Clean-OutlookTemplates.ps1 -BackupOnly
```

Use this to:
- Test the script before actual cleaning
- Create safety backups
- Verify script finds your templates correctly

---

### Option 3: Clean Specific Template

Process a single template instead of all:

```powershell
.\Clean-OutlookTemplates.ps1 -TemplatePath "C:\Path\To\MessageTemplate.oft" -ApplyRegistryFix
```

Use this when:
- You only have one problematic template
- You want to test on a single template first

---

### Option 4: Verbose Output

Get detailed information during execution:

```powershell
.\Clean-OutlookTemplates.ps1 -ApplyRegistryFix -Verbose
```

Use this for:
- Troubleshooting
- Understanding what the script is doing
- Detailed logging

---

## Parameter Reference

| Parameter | Description | Default | Example |
|-----------|-------------|---------|---------|
| `-ApplyRegistryFix` | Also apply UTF-8 registry settings | No | `-ApplyRegistryFix` |
| `-BackupOnly` | Only create backups, don't modify | No | `-BackupOnly` |
| `-TemplatePath` | Process specific template file/folder | All templates | `-TemplatePath "C:\Templates\file.oft"` |
| `-Verbose` | Show detailed output | No | `-Verbose` |

---

## Where the Script Looks for Templates

The script automatically searches these locations:

```
C:\Users\[YourUsername]\AppData\Roaming\Microsoft\Templates\
C:\Users\[YourUsername]\AppData\Roaming\Microsoft\Outlook\Templates\
C:\Users\[YourUsername]\Documents\Custom Office Templates\
```

Plus any path you specify with `-TemplatePath`.

---

## What Gets Created

### Backups

**Location**: `[OriginalTemplateFolder]\Backups_YYYYMMDD_HHMMSS\`

**Example**:
```
C:\Users\Steve\AppData\Roaming\Microsoft\Templates\
├── MessageTemplate.oft (original, now cleaned)
└── Backups_20260213_153022/
    └── MessageTemplate.oft (original backup)
```

**Retention**: Keep backups for 30 days, then delete if no issues

---

### Log Files

**Location**: Same folder as the script

**Naming**: `CleanTemplates_YYYYMMDD_HHMMSS.log`

**Example**: `CleanTemplates_20260213_153022.log`

**Contents**:
- Timestamp of each action
- Templates found and processed
- Characters removed count
- Any errors encountered
- Registry changes applied
- Summary statistics

---

## Reading the Output

### Successful Run

```
2026-02-13 15:30:22 [INFO] === Outlook Template Cleaning Script Started ===
2026-02-13 15:30:22 [INFO] Searching for Outlook template files...
2026-02-13 15:30:22 [INFO] Found 3 template(s) to process

2026-02-13 15:30:23 [INFO] --- Processing Template ---
2026-02-13 15:30:23 [INFO] Processing: MessageTemplate.oft
2026-02-13 15:30:24 [INFO]   Original body length: 458 characters
2026-02-13 15:30:24 [INFO]   Removed 5 problematic character(s)
2026-02-13 15:30:24 [INFO]   Cleaned body length: 453 characters
2026-02-13 15:30:25 [SUCCESS]   Template cleaned and saved successfully

2026-02-13 15:30:28 [SUCCESS] === Processing Complete ===
2026-02-13 15:30:28 [INFO] Templates found: 3
2026-02-13 15:30:28 [SUCCESS] Templates cleaned: 2
2026-02-13 15:30:28 [INFO] Templates skipped (already clean): 1
2026-02-13 15:30:28 [INFO] Errors encountered: 0
```

---

### Error During Run

```
2026-02-13 15:30:24 [ERROR]   ERROR cleaning template: Access denied
2026-02-13 15:30:24 [WARNING]   Restoring from backup: C:\...\Backups_...\file.oft
```

**What this means**: Script encountered an error but automatically restored from backup. Your template is unchanged.

**Common causes**:
- File is locked (Outlook still running)
- Insufficient permissions
- Corrupted template file

**Solution**: Check the error message, fix the issue, run script again

---

## After Running the Script

### Step 1: Review the Log

Open the log file and check for:
- ✅ **Templates cleaned**: Number should match expected templates
- ⚠️ **Errors encountered**: Should be 0
- ℹ️ **Characters removed**: Shows how many control characters were found

---

### Step 2: Test Your Templates

1. **Open Outlook**
2. **Open a cleaned template**:
   - File → New → Choose Template → Select your template
3. **Check for formatting marks**:
   - Press **Ctrl+Shift+8** to show formatting
   - Look for any strange symbols between text
   - Should only see normal paragraph marks (¶)
4. **Test special characters**:
   - Type: `Test £100 ®Registered ©Copyright`
   - Send to yourself
   - Verify all symbols display correctly (no question marks)

---

### Step 3: Verify Registry Settings (if -ApplyRegistryFix used)

```powershell
Get-ItemProperty -Path "HKCU:\Software\Microsoft\Office\16.0\Outlook\Options\Mail" | Select-Object AutoDetectCharset, SendCharset
```

**Expected output**:
```
AutoDetectCharset : 0
SendCharset       : 65001
```

- `AutoDetectCharset = 0`: Automatic encoding detection disabled ✅
- `SendCharset = 65001`: UTF-8 forced for all emails ✅

---

## Troubleshooting the Script

### Error: "Outlook is currently running"

**Cause**: Outlook.exe process is still active

**Solution**:
1. Close Outlook windows
2. Check Task Manager (Ctrl+Shift+Esc)
3. Look for "Microsoft Outlook" under Processes
4. If found: Right-click → End Task
5. Run script again

---

### Error: "Access denied" or "Permission denied"

**Cause**: Insufficient permissions or file is locked

**Solution**:
1. Run PowerShell as Administrator
2. Ensure Outlook is completely closed
3. Check if template file is set to "Read-only":
   - Right-click file → Properties
   - Uncheck "Read-only" if checked
4. Run script again

---

### Error: "Cannot be loaded because running scripts is disabled"

**Cause**: PowerShell execution policy restriction

**Solution**:
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```

Then run the script again.

This only affects the current PowerShell session (secure).

---

### Error: "No templates found"

**Cause**: Templates not in standard locations or already deleted

**Solution**:
1. Manually locate your .oft files
2. Run script with specific path:
   ```powershell
   .\Clean-OutlookTemplates.ps1 -TemplatePath "C:\Your\Path\Here" -ApplyRegistryFix
   ```

---

### Script runs but templates still have question marks

**Possible causes**:
1. Signature files also have control characters
2. NormalEmail.dotm template is corrupted
3. Additional Unicode issues beyond control characters

**Solution**:
1. **Clean signature files** (see main documentation)
2. **Reset NormalEmail.dotm**:
   ```
   C:\Users\[YourUsername]\AppData\Roaming\Microsoft\Templates\
   Rename: NormalEmail.dotm → NormalEmail.old
   Restart Outlook (auto-creates new file)
   ```
3. **Check log file** for clues about what was found/removed

---

## Safety Features

### Automatic Backups

- ✅ Created before ANY modifications
- ✅ Timestamped to prevent overwriting
- ✅ Stored in same folder as original templates
- ✅ Can be restored manually if needed

---

### Error Handling

- ✅ If cleaning fails → Automatically restores from backup
- ✅ Detailed error messages in log
- ✅ Script continues processing other templates (doesn't abort)

---

### COM Object Cleanup

- ✅ Properly releases Outlook COM objects
- ✅ Prevents Outlook.exe from staying in memory
- ✅ Garbage collection runs at end

---

## Manual Restoration from Backup

If you need to restore a template manually:

1. **Navigate to backup folder**:
   ```
   C:\Users\[YourUsername]\AppData\Roaming\Microsoft\Templates\Backups_YYYYMMDD_HHMMSS\
   ```

2. **Copy backup file**:
   ```powershell
   Copy-Item "Backups_20260213_153022\MessageTemplate.oft" "MessageTemplate.oft" -Force
   ```

3. **Restart Outlook**

---

## Best Practices

### Before Running

- ✅ Close all Outlook windows
- ✅ Save any draft emails
- ✅ Check Task Manager for Outlook.exe process
- ✅ Run as Administrator (for registry fixes)

---

### After Running

- ✅ Review log file for errors
- ✅ Test templates before distributing to team
- ✅ Keep backups for 30 days
- ✅ Document any issues encountered

---

### Ongoing Maintenance

- 🔄 Run script **quarterly** as preventive maintenance
- 🔄 Run after creating new templates from old sources
- 🔄 Run if question marks reappear
- 🔄 Check Microsoft updates for permanent fix

---

## Quick Reference Card

**Most Common Usage**:
```powershell
cd "C:\Users\[YourUsername]\terminai\it\troubleshooting"
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\Clean-OutlookTemplates.ps1 -ApplyRegistryFix
```

**Test First** (no changes):
```powershell
.\Clean-OutlookTemplates.ps1 -BackupOnly
```

**Single Template**:
```powershell
.\Clean-OutlookTemplates.ps1 -TemplatePath "C:\Path\To\File.oft" -ApplyRegistryFix
```

**Check Registry**:
```powershell
Get-ItemProperty -Path "HKCU:\Software\Microsoft\Office\16.0\Outlook\Options\Mail" | Select-Object AutoDetectCharset, SendCharset
```

---

## Related Documentation

- **Full issue documentation**: `Outlook_Template_Unicode_Encoding_Question_Marks.md`
- **Troubleshooting methodology**: `CLAUDE.md`
- **Knowledge base index**: `README.md`

---

**Script Version**: 1.0
**Last Updated**: 2026-02-13
**Tested On**: Windows 11, Outlook 365 Build 19628.20150+
