# Check current Outlook encoding registry settings
$regPath = "HKCU:\Software\Microsoft\Office\16.0\Outlook\Options\Mail"

Write-Host "Current Outlook Encoding Settings:"
Write-Host "===================================="

if (Test-Path $regPath) {
    $props = Get-ItemProperty -Path $regPath -ErrorAction SilentlyContinue
    
    Write-Host "AutoDetectCharset: $($props.AutoDetectCharset)"
    Write-Host "SendCharset: $($props.SendCharset)"
    Write-Host "DisableCharsetDetection: $($props.DisableCharsetDetection)"
} else {
    Write-Host "Registry path does not exist yet"
}
