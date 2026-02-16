<#
.SYNOPSIS
    Checks if an Outlook template file is locked and attempts to release locks

.DESCRIPTION
    Identifies processes holding file locks on .oft templates and provides
    options to kill those processes or wait for automatic release.

.PARAMETER TemplatePath
    Path to the .oft template file to check

.PARAMETER AttemptUnlock
    If specified, attempts to kill processes holding the lock

.EXAMPLE
    .\Test-TemplateFileLock.ps1 -TemplatePath ".\ProblemEmailTemplate.oft"

.EXAMPLE
    .\Test-TemplateFileLock.ps1 -TemplatePath ".\ProblemEmailTemplate.oft" -AttemptUnlock
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$TemplatePath,

    [Parameter(Mandatory=$false)]
    [switch]$AttemptUnlock
)

function Test-FileLock {
    param([string]$Path)

    try {
        $file = [System.IO.File]::Open($Path, 'Open', 'ReadWrite', 'None')
        $file.Close()
        $file.Dispose()
        return $false  # Not locked
    } catch {
        return $true   # Locked
    }
}

function Get-FileHandles {
    param([string]$FilePath)

    # Using handle.exe from Sysinternals (if available)
    $handleExe = "C:\Tools\handle.exe"  # Common location

    if (Test-Path $handleExe) {
        $output = & $handleExe -accepteula $FilePath 2>&1
        return $output
    } else {
        Write-Host "  Note: Install Sysinternals Handle.exe for detailed lock info" -ForegroundColor Gray
        return $null
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Template File Lock Checker" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Validate file exists
if (-not (Test-Path $TemplatePath)) {
    Write-Host "ERROR: File not found: $TemplatePath" -ForegroundColor Red
    exit 1
}

$fullPath = (Resolve-Path $TemplatePath).Path
Write-Host "Checking: $fullPath`n" -ForegroundColor White

# Test if file is locked
Write-Host "[1/3] Testing file lock status..." -ForegroundColor Yellow

if (Test-FileLock -Path $fullPath) {
    Write-Host "  Status: FILE IS LOCKED" -ForegroundColor Red
    $isLocked = $true
} else {
    Write-Host "  Status: File is NOT locked - you can use it!" -ForegroundColor Green
    $isLocked = $false
}

if (-not $isLocked) {
    Write-Host "`nThe template is ready to use!`n" -ForegroundColor Green
    exit 0
}

# Check for processes that might be locking it
Write-Host "`n[2/3] Checking for processes that may be holding locks..." -ForegroundColor Yellow

$suspectProcesses = @(
    "OUTLOOK",
    "OfficeClickToRun",
    "explorer",
    "SearchIndexer",
    "SearchProtocolHost",
    "MsMpEng",  # Windows Defender
    "OneDrive"
)

$foundProcesses = @()

foreach ($procName in $suspectProcesses) {
    $procs = Get-Process -Name $procName -ErrorAction SilentlyContinue
    if ($procs) {
        $foundProcesses += $procs
        Write-Host "  [FOUND] $procName (PID: $($procs.Id -join ', '))" -ForegroundColor Yellow
    }
}

if ($foundProcesses.Count -eq 0) {
    Write-Host "  No obvious processes found holding locks" -ForegroundColor Gray
    Write-Host "  File may be locked by system process or kernel" -ForegroundColor Gray
} else {
    Write-Host "`n  Found $($foundProcesses.Count) suspect process(es)" -ForegroundColor White
}

# Attempt to get detailed handle info
$handles = Get-FileHandles -FilePath $fullPath
if ($handles) {
    Write-Host "`n  Detailed Handle Information:" -ForegroundColor Cyan
    Write-Host $handles -ForegroundColor Gray
}

# Provide solutions
Write-Host "`n[3/3] Recommended Solutions:" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "Option 1: RESTART COMPUTER (Most Reliable)" -ForegroundColor Green
Write-Host "  - Releases ALL locks guaranteed" -ForegroundColor White
Write-Host "  - Takes 2-3 minutes" -ForegroundColor White
Write-Host "  - Command: Restart-Computer" -ForegroundColor Gray
Write-Host ""

Write-Host "Option 2: Kill Processes and Wait" -ForegroundColor Yellow
if ($foundProcesses.Count -gt 0) {
    Write-Host "  Processes to kill:" -ForegroundColor White
    foreach ($proc in $foundProcesses) {
        Write-Host "    - $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Gray
    }
    Write-Host "  Command: Get-Process -Name 'OUTLOOK','explorer' | Stop-Process -Force" -ForegroundColor Gray
    Write-Host "  Then wait 60 seconds" -ForegroundColor Gray
} else {
    Write-Host "  No specific processes identified" -ForegroundColor Gray
}
Write-Host ""

Write-Host "Option 3: Use Backup Copy" -ForegroundColor Cyan
$backupDirs = Get-ChildItem -Path (Split-Path $fullPath -Parent) -Directory -Filter "Backups_*" |
    Sort-Object Name -Descending |
    Select-Object -First 1

if ($backupDirs) {
    Write-Host "  Latest backup: $($backupDirs.FullName)" -ForegroundColor White
    $backupFile = Join-Path $backupDirs.FullName (Split-Path $fullPath -Leaf)
    if (Test-Path $backupFile) {
        Write-Host "  Backup file: $backupFile" -ForegroundColor Gray
        Write-Host "  Note: This is BEFORE cleaning (has soft hyphens)" -ForegroundColor Yellow
    }
}
Write-Host ""

Write-Host "Option 4: Copy to New Filename" -ForegroundColor Cyan
$newName = [System.IO.Path]::GetFileNameWithoutExtension($fullPath) + "_UNLOCKED.oft"
$newPath = Join-Path (Split-Path $fullPath -Parent) $newName
Write-Host "  New filename: $newPath" -ForegroundColor White
Write-Host "  Command: Copy-Item '$fullPath' '$newPath'" -ForegroundColor Gray
Write-Host ""

# Attempt unlock if requested
if ($AttemptUnlock) {
    Write-Host "`n========================================" -ForegroundColor Red
    Write-Host "ATTEMPTING TO UNLOCK FILE" -ForegroundColor Red
    Write-Host "========================================`n" -ForegroundColor Red

    if ($foundProcesses.Count -eq 0) {
        Write-Host "No processes to kill. Try restarting your computer instead." -ForegroundColor Yellow
    } else {
        Write-Host "Killing processes..." -ForegroundColor Yellow

        foreach ($proc in $foundProcesses) {
            try {
                Write-Host "  Stopping $($proc.ProcessName) (PID: $($proc.Id))..." -ForegroundColor Gray
                Stop-Process -Id $proc.Id -Force -ErrorAction Stop
                Write-Host "    Killed successfully" -ForegroundColor Green
            } catch {
                Write-Host "    Failed to kill: $($_.Exception.Message)" -ForegroundColor Red
            }
        }

        Write-Host "`nWaiting 10 seconds for locks to release..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10

        Write-Host "Re-testing file lock..." -ForegroundColor Yellow
        if (Test-FileLock -Path $fullPath) {
            Write-Host "  File is STILL locked" -ForegroundColor Red
            Write-Host "  Recommendation: Restart your computer" -ForegroundColor Yellow
        } else {
            Write-Host "  File is NOW unlocked!" -ForegroundColor Green
            Write-Host "  You can use the template now" -ForegroundColor Green
        }
    }
}

Write-Host "`nFile lock check complete.`n" -ForegroundColor White

if ($isLocked -and -not $AttemptUnlock) {
    Write-Host "TIP: Run with -AttemptUnlock to automatically kill processes" -ForegroundColor Cyan
    Write-Host "Example: .\Test-TemplateFileLock.ps1 -TemplatePath '$TemplatePath' -AttemptUnlock`n" -ForegroundColor Gray
}
