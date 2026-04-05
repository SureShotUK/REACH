# Apply Outlook UTF-8 Encoding Registry Fixes
# Fixes the Microsoft Outlook Build 19628.20150+ Unicode encoding bug

Write-Host "Outlook UTF-8 Encoding Fix" -ForegroundColor Cyan
Write-Host "===========================" -ForegroundColor Cyan
Write-Host ""

$regPath = "HKCU:\Software\Microsoft\Office\16.0\Outlook\Options\Mail"

# Create registry path if it doesn't exist
if (-not (Test-Path $regPath)) {
    Write-Host "Creating registry path..." -ForegroundColor Yellow
    New-Item -Path $regPath -Force | Out-Null
}

Write-Host "Current settings:" -ForegroundColor White
$current = Get-ItemProperty -Path $regPath -ErrorAction SilentlyContinue

Write-Host "  AutoDetectCharset: $($current.AutoDetectCharset)" -ForegroundColor Gray
Write-Host "  SendCharset: $($current.SendCharset)" -ForegroundColor Gray
Write-Host "  DisableCharsetDetection: $($current.DisableCharsetDetection)" -ForegroundColor Gray
Write-Host ""

Write-Host "Applying registry fixes..." -ForegroundColor Yellow

# Disable automatic character set detection
Set-ItemProperty -Path $regPath -Name "AutoDetectCharset" -Value 0 -Type DWord -Force
Write-Host "  Set AutoDetectCharset = 0 (Disabled)" -ForegroundColor Green

# Force UTF-8 encoding (code page 65001)
Set-ItemProperty -Path $regPath -Name "SendCharset" -Value 65001 -Type DWord -Force
Write-Host "  Set SendCharset = 65001 (UTF-8)" -ForegroundColor Green

# Disable format conversion on send
Set-ItemProperty -Path $regPath -Name "DisableCharsetDetection" -Value 1 -Type DWord -Force
Write-Host "  Set DisableCharsetDetection = 1 (Disabled)" -ForegroundColor Green

Write-Host ""
Write-Host "Verification:" -ForegroundColor White
$updated = Get-ItemProperty -Path $regPath
Write-Host "  AutoDetectCharset: $($updated.AutoDetectCharset)" -ForegroundColor Cyan
Write-Host "  SendCharset: $($updated.SendCharset)" -ForegroundColor Cyan
Write-Host "  DisableCharsetDetection: $($updated.DisableCharsetDetection)" -ForegroundColor Cyan

Write-Host ""
Write-Host "SUCCESS! Registry fixes applied." -ForegroundColor Green
Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Cyan
Write-Host "1. Restart Outlook for changes to take effect" -ForegroundColor White
Write-Host "2. Open your template in Outlook" -ForegroundColor White
Write-Host "3. Save to Drafts and verify characters display correctly" -ForegroundColor White
Write-Host ""
