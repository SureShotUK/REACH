<#
.SYNOPSIS
    Removes problematic characters from Outlook .oft template files and fixes encoding issues

.DESCRIPTION
    This script uses Outlook COM automation to:
    - Remove soft hyphens, zero-width spaces, and other problematic Unicode characters
    - Preserve template formatting and structure
    - Create automatic timestamped backups
    - Apply Outlook registry fixes for encoding issues

.PARAMETER TemplatePath
    Path to the .oft template file or directory containing templates

.PARAMETER BackupOnly
    If specified, only creates a backup without modifying the template

.EXAMPLE
    .\Clean-OutlookTemplateEncoding.ps1 -TemplatePath "C:\Path\To\ProblemEmailTemplate.oft"

.EXAMPLE
    .\Clean-OutlookTemplateEncoding.ps1 -TemplatePath "C:\Path\To\Templates" -BackupOnly
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$TemplatePath,

    [Parameter(Mandatory=$false)]
    [switch]$BackupOnly
)

#region Helper Functions

function Test-OutlookRunning {
    $process = Get-Process -Name "OUTLOOK" -ErrorAction SilentlyContinue
    if ($process) {
        Write-Host "ERROR: Outlook is running. Please close Outlook and try again." -ForegroundColor Red
        exit 1
    }
}

function New-BackupDirectory {
    param([string]$BasePath)

    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupDir = Join-Path $BasePath "Backups_$timestamp"
    New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
    return $backupDir
}

function Write-ColorMessage {
    param(
        [string]$Message,
        [string]$Level = "INFO"
    )

    $color = switch ($Level) {
        "ERROR"   { "Red" }
        "WARNING" { "Yellow" }
        "SUCCESS" { "Green" }
        "INFO"    { "White" }
        default   { "White" }
    }

    $prefix = switch ($Level) {
        "ERROR"   { "[ERROR]  " }
        "WARNING" { "[WARN]   " }
        "SUCCESS" { "[SUCCESS]" }
        "INFO"    { "[INFO]   " }
        default   { "" }
    }

    Write-Host "$prefix $Message" -ForegroundColor $color
}

#endregion

#region Main Script

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Outlook Template Encoding Cleaner" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Check Outlook is not running
Test-OutlookRunning

# Determine if path is file or directory
$templates = @()
if (Test-Path $TemplatePath -PathType Container) {
    Write-ColorMessage "Scanning directory for .oft templates..." "INFO"
    $templates = Get-ChildItem -Path $TemplatePath -Filter "*.oft" -File
    if ($templates.Count -eq 0) {
        Write-ColorMessage "No .oft files found in directory: $TemplatePath" "ERROR"
        exit 1
    }
    Write-ColorMessage "Found $($templates.Count) template(s)" "SUCCESS"
} elseif (Test-Path $TemplatePath -PathType Leaf) {
    if ($TemplatePath -notmatch '\.oft$') {
        Write-ColorMessage "File must be an .oft template: $TemplatePath" "ERROR"
        exit 1
    }
    $templates = @(Get-Item $TemplatePath)
} else {
    Write-ColorMessage "Path not found: $TemplatePath" "ERROR"
    exit 1
}

# Create backup directory
$backupDir = New-BackupDirectory -BasePath (Split-Path $templates[0].FullName -Parent)
Write-ColorMessage "Backup directory: $backupDir" "INFO"

# Define problematic characters to remove
$charsToRemove = @(
    [char]0x00AD  # Soft Hyphen
    [char]0x00A0  # Non-Breaking Space (replace with regular space)
    [char]0x200B  # Zero Width Space
    [char]0xFEFF  # Zero Width No-Break Space (BOM)
    [char]0x000C  # Form Feed
    [char]0x000B  # Vertical Tab
)

Write-Host ""

# Process each template
$totalCleaned = 0
$totalBackedUp = 0

foreach ($template in $templates) {
    Write-Host "Processing: $($template.Name)" -ForegroundColor Cyan
    Write-Host "-------------------------------------------" -ForegroundColor Gray

    try {
        # Create backup
        Write-ColorMessage "Creating backup..." "INFO"
        $backupPath = Join-Path $backupDir $template.Name
        Copy-Item -Path $template.FullName -Destination $backupPath -Force
        $totalBackedUp++
        Write-ColorMessage "Backup created: $backupPath" "SUCCESS"

        if ($BackupOnly) {
            Write-ColorMessage "Backup-only mode: Skipping cleaning" "INFO"
            Write-Host ""
            continue
        }

        # Create Outlook COM object
        Write-ColorMessage "Opening template in Outlook..." "INFO"
        $outlook = New-Object -ComObject Outlook.Application
        $mailItem = $outlook.CreateItemFromTemplate($template.FullName)

        # Extract body text
        $originalBody = $mailItem.Body
        $cleanedBody = $originalBody

        # Remove each problematic character
        $totalRemoved = 0
        foreach ($char in $charsToRemove) {
            $charCode = "U+{0:X4}" -f [int][char]$char
            $count = ($cleanedBody.ToCharArray() | Where-Object { $_ -eq $char }).Count

            if ($count -gt 0) {
                # Special case: Replace non-breaking space with regular space
                if ([int][char]$char -eq 0x00A0) {
                    $cleanedBody = $cleanedBody.Replace($char.ToString(), " ")
                    Write-ColorMessage "Replaced $count non-breaking space(s) with regular spaces ($charCode)" "SUCCESS"
                } else {
                    $cleanedBody = $cleanedBody.Replace($char.ToString(), "")
                    Write-ColorMessage "Removed $count occurrence(s) of $charCode" "SUCCESS"
                }
                $totalRemoved += $count
            }
        }

        if ($totalRemoved -eq 0) {
            Write-ColorMessage "No problematic characters found - template is clean" "INFO"
        } else {
            # Apply cleaned body
            $mailItem.Body = $cleanedBody

            # Save template
            Write-ColorMessage "Saving cleaned template..." "INFO"
            $mailItem.SaveAs($template.FullName, 5) # 5 = olTemplate
            Write-ColorMessage "Template cleaned successfully ($totalRemoved characters removed)" "SUCCESS"
            $totalCleaned++
        }

        # Close and cleanup
        $mailItem.Close(1) # olDiscard
        [System.Runtime.Interopservices.Marshal]::ReleaseComObject($mailItem) | Out-Null
        [System.Runtime.Interopservices.Marshal]::ReleaseComObject($outlook) | Out-Null
        [System.GC]::Collect()
        [System.GC]::WaitForPendingFinalizers()

    } catch {
        Write-ColorMessage "Failed to process template: $($_.Exception.Message)" "ERROR"

        # Attempt to restore from backup
        if (Test-Path $backupPath) {
            Write-ColorMessage "Restoring from backup..." "WARNING"
            Copy-Item -Path $backupPath -Destination $template.FullName -Force
            Write-ColorMessage "Restored from backup" "SUCCESS"
        }

        continue
    }

    Write-Host ""
}

# Apply registry fixes for Outlook encoding
if (-not $BackupOnly) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Applying Outlook Registry Fixes" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan

    Write-ColorMessage "These registry settings help prevent encoding issues in Outlook:" "INFO"

    try {
        # Ensure UTF-8 encoding for plain text emails
        $regPath = "HKCU:\Software\Microsoft\Office\16.0\Outlook\Options\Mail"

        if (-not (Test-Path $regPath)) {
            New-Item -Path $regPath -Force | Out-Null
        }

        # Set default encoding to UTF-8
        Set-ItemProperty -Path $regPath -Name "DefaultCharSet" -Value "utf-8" -Type String
        Write-ColorMessage "Set default character set to UTF-8" "SUCCESS"

        # Disable auto-archive (can cause encoding issues with old items)
        Set-ItemProperty -Path $regPath -Name "AutoArchive" -Value 0 -Type DWord
        Write-ColorMessage "Disabled auto-archive" "SUCCESS"

        Write-Host ""
        Write-ColorMessage "Registry fixes applied successfully" "SUCCESS"

    } catch {
        Write-ColorMessage "Failed to apply registry fixes: $($_.Exception.Message)" "WARNING"
        Write-ColorMessage "You may need to apply these manually" "WARNING"
    }
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "SUMMARY" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "Templates processed: $($templates.Count)" -ForegroundColor White
Write-Host "Backups created:     $totalBackedUp" -ForegroundColor White
if (-not $BackupOnly) {
    Write-Host "Templates cleaned:   $totalCleaned" -ForegroundColor White
}

Write-Host "`nBackup location: $backupDir" -ForegroundColor Cyan

if (-not $BackupOnly) {
    Write-Host "`nIMPORTANT:" -ForegroundColor Yellow
    Write-Host "  1. Test the cleaned template(s) in Outlook" -ForegroundColor White
    Write-Host "  2. Verify question marks are gone" -ForegroundColor White
    Write-Host "  3. Test typing £ symbol and other special characters" -ForegroundColor White
    Write-Host "  4. If issues persist, try:" -ForegroundColor White
    Write-Host "     - Restart Outlook" -ForegroundColor White
    Write-Host "     - Restart computer (to release file locks)" -ForegroundColor White
    Write-Host "     - Restore from backup if needed: $backupDir" -ForegroundColor Gray
}

Write-Host "`nScript complete!`n" -ForegroundColor Green

#endregion
