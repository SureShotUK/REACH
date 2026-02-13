# Clean-OutlookTemplates.ps1
# Removes problematic Unicode characters from Outlook .oft template files
# that cause question marks (?) to appear in sent emails
#
# Author: IT Troubleshooting Team
# Date: 2026-02-13
# Issue: Microsoft Outlook Build 19628.20150+ Unicode encoding bug
#
# IMPORTANT: Run this script with Outlook CLOSED
#
# USAGE:
#   Default (clean all templates in standard locations):
#     .\Clean-OutlookTemplates.ps1 -ApplyRegistryFix
#
#   Specific folder (clean only templates in specified folder):
#     .\Clean-OutlookTemplates.ps1 -TemplatePath "C:\MyTemplates" -ApplyRegistryFix
#
#   Specific file (clean only one template):
#     .\Clean-OutlookTemplates.ps1 -TemplatePath "C:\MyTemplates\file.oft" -ApplyRegistryFix

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false,
               HelpMessage="Optional: Specify a folder path or specific .oft file to clean. If not provided, searches default template locations.")]
    [string]$TemplatePath,

    [Parameter(Mandatory=$false,
               HelpMessage="Create backups only without modifying templates (for testing).")]
    [switch]$BackupOnly,

    [Parameter(Mandatory=$false,
               HelpMessage="Apply UTF-8 registry fixes to prevent encoding issues.")]
    [switch]$ApplyRegistryFix
)

# Script configuration
$ErrorActionPreference = "Stop"
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$logFile = Join-Path $scriptPath "CleanTemplates_$timestamp.log"

# Initialize log file
function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $logMessage = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') [$Level] $Message"
    Add-Content -Path $logFile -Value $logMessage

    switch ($Level) {
        "ERROR" { Write-Host $logMessage -ForegroundColor Red }
        "WARNING" { Write-Host $logMessage -ForegroundColor Yellow }
        "SUCCESS" { Write-Host $logMessage -ForegroundColor Green }
        default { Write-Host $logMessage }
    }
}

Write-Log "=== Outlook Template Cleaning Script Started ===" "INFO"
Write-Log "Log file: $logFile" "INFO"

# Check if Outlook is running
function Test-OutlookRunning {
    $outlookProcess = Get-Process -Name "OUTLOOK" -ErrorAction SilentlyContinue
    if ($outlookProcess) {
        Write-Log "ERROR: Outlook is currently running. Please close Outlook and run this script again." "ERROR"
        Write-Host "`nPress any key to exit..."
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 1
    }
}

# Find all .oft template files
function Find-OutlookTemplates {
    $templates = @()

    # If specific path provided, ONLY search that path
    if ($TemplatePath) {
        if (Test-Path $TemplatePath) {
            Write-Log "Searching in specified path: $TemplatePath" "INFO"

            if ($TemplatePath -like "*.oft") {
                # Single file specified
                $templates += Get-Item $TemplatePath
                Write-Log "  Found 1 template" "INFO"
            } else {
                # Directory specified - search for .oft files
                $found = Get-ChildItem -Path $TemplatePath -Filter "*.oft" -Recurse -ErrorAction SilentlyContinue
                if ($found) {
                    $templates += $found
                    Write-Log "  Found $($found.Count) template(s)" "INFO"
                } else {
                    Write-Log "  No .oft templates found in specified path" "WARNING"
                }
            }
        } else {
            Write-Log "ERROR: Specified path does not exist: $TemplatePath" "ERROR"
            Write-Host "`nThe path you specified does not exist. Please check the path and try again." -ForegroundColor Red
            Write-Host "Path: $TemplatePath" -ForegroundColor Yellow
            return @()
        }
    } else {
        # No specific path provided - search default locations
        Write-Log "Searching for Outlook template files in default locations..." "INFO"

        $templateLocations = @(
            "$env:APPDATA\Microsoft\Templates",
            "$env:APPDATA\Microsoft\Outlook\Templates",
            "$env:USERPROFILE\Documents\Custom Office Templates"
        )

        foreach ($location in $templateLocations) {
            if (Test-Path $location) {
                Write-Log "Searching in: $location" "INFO"
                $found = Get-ChildItem -Path $location -Filter "*.oft" -Recurse -ErrorAction SilentlyContinue
                if ($found) {
                    $templates += $found
                    Write-Log "  Found $($found.Count) template(s)" "INFO"
                }
            }
        }
    }

    return $templates
}

# Create backup of template
function Backup-Template {
    param([System.IO.FileInfo]$Template)

    $backupDir = Join-Path (Split-Path $Template.FullName) "Backups_$timestamp"

    if (-not (Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
    }

    $backupPath = Join-Path $backupDir $Template.Name
    Copy-Item -Path $Template.FullName -Destination $backupPath -Force

    Write-Log "  Backup created: $backupPath" "INFO"
    return $backupPath
}

# Clean problematic characters from template
function Clean-Template {
    param(
        [System.IO.FileInfo]$Template,
        [string]$BackupPath
    )

    try {
        Write-Log "Processing: $($Template.Name)" "INFO"

        # Create Outlook COM object
        $outlook = New-Object -ComObject Outlook.Application
        $namespace = $outlook.GetNamespace("MAPI")

        # Open the template as MailItem
        $mailItem = $outlook.CreateItemFromTemplate($Template.FullName)

        # Extract body content
        $bodyHTML = $mailItem.HTMLBody
        $bodyText = $mailItem.Body
        $subject = $mailItem.Subject

        Write-Log "  Original body length: $($bodyText.Length) characters" "INFO"

        # Define problematic characters to remove
        # Form Feed (U+000C), Vertical Tab (U+000B), Soft Hyphen (U+00AD), and other control characters
        $controlChars = @(
            [char]0x00AD,  # Soft Hyphen (­) - THE MAIN CULPRIT!
            [char]0x000C,  # Form Feed
            [char]0x000B,  # Vertical Tab
            [char]0x00AC,  # Not Sign (¬)
            [char]0x0085,  # Next Line
            [char]0x2028,  # Line Separator
            [char]0x2029   # Paragraph Separator
        )

        # Remove problematic characters from body text
        $cleanedText = $bodyText
        $cleanedHTML = $bodyHTML
        $removedCount = 0

        foreach ($char in $controlChars) {
            $beforeLength = $cleanedText.Length
            # Convert char to string for Replace method to work with empty string
            $cleanedText = $cleanedText.Replace($char.ToString(), '')
            $cleanedHTML = $cleanedHTML.Replace($char.ToString(), '')
            $removedCount += ($beforeLength - $cleanedText.Length)
        }

        # Also remove using regex for any remaining control characters (except common ones)
        # Allow: Tab (0x09), LF (0x0A), CR (0x0D)
        $cleanedText = $cleanedText -replace '[\x00-\x08\x0B-\x0C\x0E-\x1F\x7F]', ''
        $cleanedHTML = $cleanedHTML -replace '[\x00-\x08\x0B-\x0C\x0E-\x1F\x7F]', ''

        Write-Log "  Removed $removedCount problematic character(s)" "INFO"
        Write-Log "  Cleaned body length: $($cleanedText.Length) characters" "INFO"

        if ($removedCount -gt 0) {
            # Update the mail item with cleaned content
            $mailItem.Body = $cleanedText
            $mailItem.HTMLBody = $cleanedHTML

            # Save as new template (overwrites original)
            $mailItem.SaveAs($Template.FullName, 5)  # 5 = olTemplate format

            Write-Log "  Template cleaned and saved successfully" "SUCCESS"
            $script:cleanedCount++
        } else {
            Write-Log "  No problematic characters found - template is clean" "INFO"
            $script:skippedCount++
        }

        # Clean up COM objects
        $mailItem.Close(1)  # 1 = olDiscard (don't save to Outlook)
        [System.Runtime.Interopservices.Marshal]::ReleaseComObject($mailItem) | Out-Null
        [System.Runtime.Interopservices.Marshal]::ReleaseComObject($namespace) | Out-Null
        [System.Runtime.Interopservices.Marshal]::ReleaseComObject($outlook) | Out-Null

        return $true

    } catch {
        Write-Log "  ERROR cleaning template: $($_.Exception.Message)" "ERROR"
        Write-Log "  Restoring from backup: $BackupPath" "WARNING"

        # Restore from backup
        Copy-Item -Path $BackupPath -Destination $Template.FullName -Force

        # Clean up COM objects on error
        try {
            if ($mailItem) {
                $mailItem.Close(1)
                [System.Runtime.Interopservices.Marshal]::ReleaseComObject($mailItem) | Out-Null
            }
            if ($namespace) { [System.Runtime.Interopservices.Marshal]::ReleaseComObject($namespace) | Out-Null }
            if ($outlook) { [System.Runtime.Interopservices.Marshal]::ReleaseComObject($outlook) | Out-Null }
        } catch {
            # Ignore cleanup errors
        }

        $script:errorCount++
        return $false
    }
}

# Apply registry fixes for UTF-8 encoding
function Set-OutlookEncodingRegistry {
    Write-Log "Applying registry fixes for UTF-8 encoding..." "INFO"

    try {
        $registryPath = "HKCU:\Software\Microsoft\Office\16.0\Outlook\Options\Mail"

        # Ensure registry path exists
        if (-not (Test-Path $registryPath)) {
            New-Item -Path $registryPath -Force | Out-Null
        }

        # Disable automatic character set detection
        Set-ItemProperty -Path $registryPath -Name "AutoDetectCharset" -Value 0 -Type DWord -Force
        Write-Log "  Set AutoDetectCharset = 0 (Disabled)" "SUCCESS"

        # Force UTF-8 encoding (code page 65001)
        Set-ItemProperty -Path $registryPath -Name "SendCharset" -Value 65001 -Type DWord -Force
        Write-Log "  Set SendCharset = 65001 (UTF-8)" "SUCCESS"

        # Disable format conversion on send
        Set-ItemProperty -Path $registryPath -Name "DisableCharsetDetection" -Value 1 -Type DWord -Force
        Write-Log "  Set DisableCharsetDetection = 1 (Disabled)" "SUCCESS"

        Write-Log "Registry fixes applied successfully" "SUCCESS"

        # Verify settings
        $autoDetect = Get-ItemProperty -Path $registryPath -Name "AutoDetectCharset" -ErrorAction SilentlyContinue
        $sendCharset = Get-ItemProperty -Path $registryPath -Name "SendCharset" -ErrorAction SilentlyContinue

        Write-Log "Verification:" "INFO"
        Write-Log "  AutoDetectCharset: $($autoDetect.AutoDetectCharset)" "INFO"
        Write-Log "  SendCharset: $($sendCharset.SendCharset)" "INFO"

        return $true

    } catch {
        Write-Log "ERROR applying registry fixes: $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Main execution
function Main {
    # Check Outlook is closed
    Test-OutlookRunning

    # Find templates
    $templates = Find-OutlookTemplates

    if ($templates.Count -eq 0) {
        Write-Log "No Outlook templates found." "WARNING"
        Write-Host "`nPress any key to exit..."
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        return
    }

    Write-Log "Found $($templates.Count) template(s) to process" "INFO"

    # Initialize counters
    $script:cleanedCount = 0
    $script:skippedCount = 0
    $script:errorCount = 0

    # Process each template
    foreach ($template in $templates) {
        Write-Log "`n--- Processing Template ---" "INFO"

        # Create backup
        $backupPath = Backup-Template -Template $template

        # Clean template (unless backup-only mode)
        if (-not $BackupOnly) {
            Clean-Template -Template $template -BackupPath $backupPath
        } else {
            Write-Log "  Backup-only mode: Skipping cleaning" "INFO"
        }
    }

    # Apply registry fix if requested
    if ($ApplyRegistryFix -and -not $BackupOnly) {
        Write-Log "`n--- Applying Registry Fixes ---" "INFO"
        Set-OutlookEncodingRegistry
    }

    # Summary
    Write-Log "`n=== Processing Complete ===" "SUCCESS"
    Write-Log "Templates found: $($templates.Count)" "INFO"
    Write-Log "Templates cleaned: $script:cleanedCount" "SUCCESS"
    Write-Log "Templates skipped (already clean): $script:skippedCount" "INFO"
    Write-Log "Errors encountered: $script:errorCount" $(if ($script:errorCount -gt 0) { "ERROR" } else { "INFO" })

    if ($BackupOnly) {
        Write-Log "`nBackup-only mode: No templates were modified" "INFO"
    }

    Write-Log "`nLog file saved to: $logFile" "INFO"

    # Display next steps
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "NEXT STEPS:" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "1. Review the log file for any errors" -ForegroundColor White
    Write-Host "2. Open Outlook and test your templates" -ForegroundColor White
    Write-Host "3. Send test emails with £ and ® symbols" -ForegroundColor White
    Write-Host "4. Verify no question marks appear" -ForegroundColor White

    if ($ApplyRegistryFix) {
        Write-Host "`n5. Registry fixes applied - Outlook will use UTF-8" -ForegroundColor Green
    } else {
        Write-Host "`n5. Run with -ApplyRegistryFix to set UTF-8 encoding" -ForegroundColor Yellow
    }

    Write-Host "`nPress any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

# Run main function
try {
    Main
} catch {
    Write-Log "CRITICAL ERROR: $($_.Exception.Message)" "ERROR"
    Write-Log "Stack Trace: $($_.ScriptStackTrace)" "ERROR"
    Write-Host "`nPress any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

# Clean up COM objects and garbage collection
[System.GC]::Collect()
[System.GC]::WaitForPendingFinalizers()
