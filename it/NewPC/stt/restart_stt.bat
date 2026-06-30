@echo off
echo Stopping STT Client...
powershell -Command "Stop-ScheduledTask -TaskName 'STT Client' -ErrorAction SilentlyContinue"
timeout /t 2 /nobreak >nul
echo Starting STT Client...
powershell -Command "Start-ScheduledTask -TaskName 'STT Client'"
echo Done. Check the system tray in a few seconds.
timeout /t 3 /nobreak >nul
