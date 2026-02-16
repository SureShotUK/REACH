<#
.SYNOPSIS
    Diagnoses character encoding issues in Outlook .oft template files

.DESCRIPTION
    This script uses Outlook COM automation to safely open a template,
    extract its body text, and display detailed character analysis including
    Unicode code points for problematic characters.

.PARAMETER TemplatePath
    Path to the .oft template file to diagnose

.EXAMPLE
    .\Diagnose-OutlookTemplate.ps1 -TemplatePath "C:\Path\To\ProblemEmailTemplate.oft"
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$TemplatePath
)

# Ensure Outlook is not running
$outlookProcess = Get-Process -Name "OUTLOOK" -ErrorAction SilentlyContinue
if ($outlookProcess) {
    Write-Host "ERROR: Outlook is currently running. Please close Outlook and run this script again." -ForegroundColor Red
    exit 1
}

# Validate template file exists
if (-not (Test-Path $TemplatePath)) {
    Write-Host "ERROR: Template file not found: $TemplatePath" -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Outlook Template Character Diagnostics" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "Template: $TemplatePath`n" -ForegroundColor White

try {
    # Create Outlook COM object
    Write-Host "[1/5] Creating Outlook COM object..." -ForegroundColor Yellow
    $outlook = New-Object -ComObject Outlook.Application

    # Open template
    Write-Host "[2/5] Opening template file..." -ForegroundColor Yellow
    $mailItem = $outlook.CreateItemFromTemplate($TemplatePath)

    # Extract body text
    Write-Host "[3/5] Extracting body text...`n" -ForegroundColor Yellow
    $bodyText = $mailItem.Body

    # Close mail item without saving
    $mailItem.Close(1) # olDiscard

    # Display character analysis
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "CHARACTER ANALYSIS" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan

    Write-Host "[4/5] Analyzing characters..." -ForegroundColor Yellow

    # Define problematic characters to search for
    $problematicChars = @{
        'Soft Hyphen (U+00AD)' = [char]0x00AD
        'Non-Breaking Space (U+00A0)' = [char]0x00A0
        'Zero Width Space (U+200B)' = [char]0x200B
        'Zero Width No-Break Space (U+FEFF)' = [char]0xFEFF
        'Form Feed (U+000C)' = [char]0x000C
        'Vertical Tab (U+000B)' = [char]0x000B
        'Pound Sign (£ - U+00A3)' = [char]0x00A3
    }

    Write-Host "`nProblematic Character Search:" -ForegroundColor White
    Write-Host "------------------------------" -ForegroundColor Gray

    $foundIssues = $false

    foreach ($charName in $problematicChars.Keys) {
        $char = $problematicChars[$charName]
        $count = ($bodyText.ToCharArray() | Where-Object { $_ -eq $char }).Count

        if ($count -gt 0) {
            Write-Host "  [FOUND] $charName : $count occurrence(s)" -ForegroundColor Red
            $foundIssues = $true

            # Show context (20 chars before and after first occurrence)
            $index = $bodyText.IndexOf($char)
            if ($index -ge 0) {
                $start = [Math]::Max(0, $index - 20)
                $length = [Math]::Min(41, $bodyText.Length - $start)
                $context = $bodyText.Substring($start, $length)
                $relativePos = $index - $start

                Write-Host "    Context: " -NoNewline -ForegroundColor Gray
                Write-Host $context.Substring(0, $relativePos) -NoNewline -ForegroundColor White
                Write-Host "[HERE]" -NoNewline -ForegroundColor Red
                Write-Host $context.Substring($relativePos + 1) -ForegroundColor White
            }
        } else {
            Write-Host "  [OK] $charName : Not found" -ForegroundColor Green
        }
    }

    if (-not $foundIssues) {
        Write-Host "`n  No problematic characters detected!" -ForegroundColor Green
    }

    # Character code analysis for entire body
    Write-Host "`n`n[5/5] Full Character Code Map:" -ForegroundColor Yellow
    Write-Host "------------------------------" -ForegroundColor Gray
    Write-Host "(Showing non-printable and special characters only)`n" -ForegroundColor Gray

    $lines = $bodyText -split "`r`n"
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        $specialChars = @()

        for ($j = 0; $j -lt $line.Length; $j++) {
            $char = $line[$j]
            $code = [int][char]$char

            # Flag non-printable, control chars, or special Unicode
            if ($code -lt 32 -or $code -eq 127 -or ($code -ge 0x0080 -and $code -le 0x00FF) -or $code -ge 0x2000) {
                $specialChars += "Position $j : U+$($code.ToString('X4')) ($char)"
            }
        }

        if ($specialChars.Count -gt 0) {
            Write-Host "  Line $($i + 1):" -ForegroundColor Cyan
            foreach ($sc in $specialChars) {
                Write-Host "    $sc" -ForegroundColor Yellow
            }
        }
    }

    # Summary
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "SUMMARY" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan

    Write-Host "Total body length: $($bodyText.Length) characters" -ForegroundColor White
    Write-Host "Total lines: $($lines.Count)" -ForegroundColor White

    if ($foundIssues) {
        Write-Host "`nStatus: Problematic characters detected" -ForegroundColor Red
        Write-Host "Recommendation: Use Clean-OutlookTemplate.ps1 to remove these characters" -ForegroundColor Yellow
    } else {
        Write-Host "`nStatus: No known problematic characters found" -ForegroundColor Green
    }

    Write-Host "`nNote: The £ (pound sign) issue when typing may be due to:" -ForegroundColor Cyan
    Write-Host "  1. RTF encoding metadata in the template" -ForegroundColor White
    Write-Host "  2. Outlook's default text encoding settings" -ForegroundColor White
    Write-Host "  3. Keyboard input method vs. template encoding mismatch" -ForegroundColor White

} catch {
    Write-Host "`nERROR: Failed to analyze template" -ForegroundColor Red
    Write-Host "Details: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    # Cleanup COM objects
    if ($mailItem) {
        [System.Runtime.Interopservices.Marshal]::ReleaseComObject($mailItem) | Out-Null
    }
    if ($outlook) {
        [System.Runtime.Interopservices.Marshal]::ReleaseComObject($outlook) | Out-Null
    }
    [System.GC]::Collect()
    [System.GC]::WaitForPendingFinalizers()
}

Write-Host "`nDiagnostics complete!`n" -ForegroundColor Green
