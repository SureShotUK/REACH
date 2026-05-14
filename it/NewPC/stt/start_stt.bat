@echo off
REM Start the STT client minimised so it does not steal focus.
REM This file can be placed anywhere (including shell:startup) because
REM it uses an absolute path to stt_client.py.
REM F9 toggles listening on/off once the client is running.
start "STT Client" /min pythonw "C:\Users\SteveIrwin\stt\stt_client.py"
