# Outlook Template Encoding Issues - Question Marks (?????) in Templates

**Issue Date:** 2026-02-16
**Status:** Root cause identified, solution provided
**Affected Application:** Microsoft Outlook (Microsoft 365)
**Issue Type:** Character encoding in .oft template files

---

## Issue Summary

When using an Outlook email template (.oft file):
- Question marks (`?????`) appear in the email body instead of intended characters
- Five question marks appear above the footer line separator
- Typed special characters (£, €, etc.) become question marks when saving to Drafts
- The template appears corrupted but is actually an encoding issue

## Environment

- **OS**: Windows 11
- **Office Version**: Microsoft 365 Apps
- **Affected Application**: Microsoft Outlook
- **File Type**: Outlook Template (.oft)
- **Root Cause**: UTF-16 character encoding with soft hyphens and potential RTF metadata issues

## Symptoms

### Primary Symptoms
1. **In Outlook**: Five question marks (`?????`) appear above the horizontal line in the email footer
2. **When typing**: Special characters like £ (pound sign) become `?` when the email is saved to Drafts
3. **Replace function fails**: Cannot find or replace the problematic characters using Notepad++ or text editors

### When Viewing Template in Notepad++

**ANSI Encoding View:**
- Shows `NUL-NUL-NUL-NUL-NUL-NULCR` where question marks appear
- Shows `NUL` after every letter in the template (this is normal - UTF-16 LE encoding!)

**UTF-8 Encoding View:**
- NUL characters remain
- The `-` (hyphen) characters change to `xAD` (soft hyphen)
- Confirms presence of soft hyphen characters (U+00AD)

## Root Cause Analysis

### Understanding the File Format

Outlook .oft files are **Compound File Binary Format (CFBF)** containers that contain:
- Multiple streams (like a mini-filesystem inside a file)
- RTF-formatted email body
- Properties, attachments, and metadata
- **Text stored in UTF-16 Little Endian encoding**

### The "NUL After Every Character" Pattern

**This is completely normal!**

In UTF-16 Little Endian encoding:
- The letter "A" is stored as: `41 00` (hex)
- This appears in raw view as: `A` + `NUL`
- Every ASCII character is followed by `00` (NUL byte)

**This is NOT corruption** - it's the expected binary representation of UTF-16 LE text.

### The Soft Hyphen Problem

**What's happening:**
1. The template contains **five soft hyphens** (Unicode U+00AD)
2. Stored in UTF-16 LE as: `AD 00 AD 00 AD 00 AD 00 AD 00 0D 00`
3. When Notepad++ opens in ANSI mode:
   - Interprets `AD` as `­` (soft hyphen in ANSI)
   - Interprets `00` as NUL
   - Shows: `­NUL ­NUL ­NUL ­NUL ­NUL NULCR`
4. When Outlook displays this character, it shows `?` because the rendering context doesn't support soft hyphens properly

**Soft Hyphen (U+00AD) Explained:**
- Invisible character used for word breaking in typography
- Indicates where a word can be hyphenated if needed
- Often causes display issues when copied/pasted from documents
- Should generally not be present in email templates

### The £ Symbol Problem (Separate Issue)

When you type `£` in the template and it becomes `?` when saved:
1. Possible RTF encoding metadata mismatch
2. Outlook's default encoding settings may be incorrect
3. Template was created with wrong codepage
4. Keyboard input encoding vs. template encoding conflict

### Why Replace Doesn't Work in Notepad++

**Cannot find soft hyphens because:**
1. Notepad++ searches the **raw binary OLE container format**
2. The text is embedded in RTF format within CFBF streams
3. Soft hyphens are RTF-escaped (e.g., `\-` or similar)
4. You're searching binary structure, not parsed text
5. Opening in wrong encoding doesn't expose the actual character

**The only safe way** to remove these characters is through **Outlook COM automation**, which:
- Properly parses the CFBF structure
- Extracts the RTF body
- Decodes the text correctly
- Allows text manipulation
- Re-saves in proper .oft format

## Resolution Steps

### Step 1: Diagnose the Issue

**Close Outlook** (required for COM automation):
```powershell
# Ensure Outlook is closed
Get-Process -Name "OUTLOOK" -ErrorAction SilentlyContinue | Stop-Process -Force
```

**Run the diagnostic script:**
```powershell
.\Diagnose-OutlookTemplate.ps1 -TemplatePath "C:\Path\To\ProblemEmailTemplate.oft"
```

**What the diagnostic shows:**
- Count of each problematic character type
- Context showing where characters appear
- Full character code map for special/non-printable characters
- Recommendations for cleaning

**Expected output:**
```
[FOUND] Soft Hyphen (U+00AD) : 5 occurrence(s)
  Context: Portland Pricing[HERE]
                     _____________________

[OK] Pound Sign (£ - U+00A3) : Not found
```

### Step 2: Clean the Template

**IMPORTANT: Close Outlook first!**

**Run the cleaning script:**
```powershell
.\Clean-OutlookTemplateEncoding.ps1 -TemplatePath "C:\Path\To\ProblemEmailTemplate.oft"
```

**What the script does:**
1. ✅ Creates timestamped backup directory
2. ✅ Backs up original template
3. ✅ Opens template using Outlook COM automation
4. ✅ Removes soft hyphens (U+00AD)
5. ✅ Removes other problematic characters:
   - Zero-width spaces (U+200B)
   - Zero-width no-break spaces (U+FEFF)
   - Form feeds (U+000C)
   - Vertical tabs (U+000B)
6. ✅ Replaces non-breaking spaces (U+00A0) with regular spaces
7. ✅ Saves cleaned template in proper .oft format
8. ✅ Applies Outlook registry fixes for encoding

**Expected output:**
```
[SUCCESS] Backup created: C:\...\Backups_20260216_143022\ProblemEmailTemplate.oft
[SUCCESS] Removed 5 occurrence(s) of U+00AD
[SUCCESS] Template cleaned successfully (5 characters removed)
[SUCCESS] Set default character set to UTF-8
```

### Step 3: Verify the Fix

1. **Open Outlook**
2. **Create new email from template**: File → New → Choose Form → User Templates in File System → Select cleaned template
3. **Check the footer**: Verify question marks are gone
4. **Test special characters**: Type "This is a test £7.42" at the top
5. **Save to Drafts**: File → Save
6. **Check Drafts folder**: Verify £ symbol displays correctly

### Step 4: If £ Symbol Still Shows as ?

If the soft hyphens are fixed but typed special characters still become question marks:

**Option A: Recreate Template with Correct Encoding**
1. Open cleaned template
2. Select all text (Ctrl+A)
3. Copy to clipboard
4. Create NEW email (not from template)
5. Paste text
6. Format as needed
7. File → Save As → Outlook Template (.oft)
8. Save with new name

**Option B: Force UTF-8 Encoding**
1. Open Registry Editor (`regedit`)
2. Navigate to: `HKEY_CURRENT_USER\Software\Microsoft\Office\16.0\Outlook\Options\Mail`
3. Create/modify DWORD: `UseUTF8` = `1`
4. Create/modify String: `DefaultCharSet` = `utf-8`
5. Restart Outlook
6. Test template again

**Option C: Check Outlook Language Settings**
1. Outlook → File → Options → Language
2. Ensure "English (United Kingdom)" is selected for UK £ symbol support
3. Click "Set as Default"
4. Restart Outlook

## Prevention

### Best Practices for Outlook Templates

1. **Always create templates in Outlook**, not by:
   - Copying/pasting from Word documents
   - Importing from PDFs
   - Copying from web browsers
   - Editing .oft files in text editors

2. **Avoid copying text** from:
   - Microsoft Word (contains hidden formatting codes)
   - PDFs (encoding issues common)
   - Web pages (HTML entities, soft hyphens prevalent)

3. **Use "Paste as Plain Text"** when copying:
   - Ctrl+Shift+V in Outlook
   - Or: Home → Paste → Paste Special → Unformatted Text

4. **Regular template maintenance**:
   - Run diagnostic script quarterly
   - Clean templates when moving between Office versions
   - Test templates after major Outlook updates

5. **Use UTF-8 encoding**:
   - Set Outlook default to UTF-8 (registry fix provided in script)
   - Avoids codepage issues with international characters

6. **Test special characters**:
   - Before deploying templates, test: £ € $ ¥ © ® ™
   - Save to Drafts and verify display

### Detecting Problematic Characters in Source Documents

When creating templates from existing documents:

**In Word/Source Document:**
1. Find → More → Special → select "Optional Hyphen" (soft hyphen)
2. Replace All with nothing
3. Then copy/paste into Outlook

**In Notepad++:**
1. View → Show Symbol → Show All Characters
2. Look for: `­` (soft hyphen symbol)
3. Edit → Replace → Extended search mode
4. Find: `\xAD` → Replace with nothing

## File Lock Issues

### Problem: "Can't open template after cleaning"

**Cause**: COM automation or Windows processes hold file locks

**Solution (in order of preference):**

1. **Restart computer** (quickest, releases all locks) - **RECOMMENDED**

2. **Kill Outlook process**:
   ```powershell
   Get-Process -Name "OUTLOOK" | Stop-Process -Force
   # Wait 30 seconds before trying again
   ```

3. **Wait**: Sometimes locks release automatically after 1-2 minutes

4. **Use backup**: Work with clean backup from timestamped backup directory

5. **Copy to new filename**: Bypasses lock on original file

## Script Usage Reference

### Diagnose-OutlookTemplate.ps1

**Purpose**: Identify problematic characters without modifying template

**Syntax:**
```powershell
.\Diagnose-OutlookTemplate.ps1 -TemplatePath <Path>
```

**Parameters:**
- `-TemplatePath`: Full path to .oft template file (required)

**Requirements:**
- Outlook must be closed
- .oft file must exist

**Output:**
- Character analysis report
- Unicode code point details
- Context showing character locations
- Recommendations

### Clean-OutlookTemplateEncoding.ps1

**Purpose**: Remove problematic characters and apply encoding fixes

**Syntax:**
```powershell
.\Clean-OutlookTemplateEncoding.ps1 -TemplatePath <Path> [-BackupOnly]
```

**Parameters:**
- `-TemplatePath`: Full path to .oft file or directory containing .oft files (required)
- `-BackupOnly`: Creates backup without cleaning (optional)

**Requirements:**
- Outlook must be closed
- Sufficient disk space for backups

**What it fixes:**
- Soft hyphens (U+00AD)
- Zero-width spaces (U+200B, U+FEFF)
- Form feeds (U+000C)
- Vertical tabs (U+000B)
- Non-breaking spaces (U+00A0) → replaced with regular spaces
- Registry settings for UTF-8 encoding

**Safety features:**
- Automatic timestamped backups
- Graceful error handling
- Automatic rollback on failure
- COM cleanup to prevent file locks

## Related Information

### Microsoft Knowledge Base Articles
- <a href="https://support.microsoft.com/office/7c14d5c0-1789-4c0a-9c0c-8c9c1f3c7c0a" target="_blank">KB: Text appears as question marks in Outlook</a>
- <a href="https://support.microsoft.com/office/change-the-message-format-to-html-rich-text-format-or-plain-text-338a389d-11da-47fe-b693-cf41f792fefa" target="_blank">KB: Change message format in Outlook</a>

### Unicode Character References
- **U+00AD**: Soft Hyphen (SHY) - optional line break point
- **U+00A0**: Non-Breaking Space (NBSP) - prevents line break
- **U+00A3**: Pound Sign (£) - UK currency symbol
- **U+200B**: Zero-Width Space (ZWSP) - invisible word separator
- **U+FEFF**: Zero-Width No-Break Space (BOM) - byte order mark

### Technical References
- <a href="https://docs.microsoft.com/office/vba/api/outlook.mailitem" target="_blank">Outlook MailItem Object Reference</a>
- <a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-cfb" target="_blank">Compound File Binary Format Specification</a>
- <a href="https://www.unicode.org/charts/" target="_blank">Unicode Character Code Charts</a>

## Lessons Learned

### For IT Support

1. **Binary file formats cannot be edited as text**
   - .oft, .docx, .xlsx are binary containers, not text files
   - Always use appropriate tools (COM automation, specialized libraries)
   - Text editors corrupt these files when saving

2. **Encoding issues require proper character code verification**
   - Don't assume - test with actual data
   - Use hex editors or PowerShell to identify exact character codes
   - User feedback is crucial (e.g., Notepad++ "Show All Characters")

3. **Iterative debugging with user feedback works**
   - Test early, test often
   - Incorporate user observations (they found `­` symbol)
   - Each iteration revealed different issue categories

4. **File locks are common with Office automation**
   - Always include COM cleanup code
   - Recommend restarting computer for persistent locks
   - Provide backup strategy for recovery

### For Users

1. **Never edit .oft files in text editors** (Notepad++, VS Code, etc.)
   - This corrupts the internal file structure
   - Use Outlook to create and modify templates

2. **Copy/paste introduces hidden characters**
   - Use "Paste as Plain Text" (Ctrl+Shift+V)
   - Clean source documents first

3. **Test templates before deployment**
   - Save to Drafts and verify display
   - Test international characters if needed

4. **Keep backups when using automation scripts**
   - Scripts create timestamped backups automatically
   - Don't delete backups until fully verified

---

**Document Version**: 1.0
**Last Updated**: 2026-02-16
**Author**: IT Helpdesk - Claude Code
**Related Files**:
- `Diagnose-OutlookTemplate.ps1`
- `Clean-OutlookTemplateEncoding.ps1`
- `ProblemEmailTemplate.oft`
