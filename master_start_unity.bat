@echo off
setlocal

:: --- CONFIGURATION START ---
:: Path to your AutoHotkey executable
set "AHK_EXE=C:\Program Files\AutoHotkey\v2\AutoHotkey64.exe"

:: Path to the AHK script (Assuming it's in the same folder as this .bat file)
set "AHK_SCRIPT=%~dp0kiosk_keyblock.ahk"
:: --- CONFIGURATION END ---

echo Starting Kiosk Security Wrapper...

:: Check if AHK exists to prevent silent failures
if not exist "%AHK_EXE%" (
    echo ERROR: AutoHotkey not found at "%AHK_EXE%"
    pause
    exit /b 1
)

:: Launch the AHK Script (Which will, in turn, launch the Unity App)
start "Kiosk Keyguard" "%AHK_EXE%" "%AHK_SCRIPT%"

echo Done. Wrapper is active.
endlocal
exit /b 0