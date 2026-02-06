#Requires AutoHotkey v2.0
#Warn
; Kiosk keyguard (AutoHotkey v2)
; Unlock: Ctrl+Alt+Shift+O  (temporary unlock)
; Manual lock: Ctrl+Alt+Shift+L
; Exit script: Ctrl+Alt+Shift+X

; --------------------
; Globals
; --------------------
global Toggle := false                     ; when true, shortcuts are ALLOWED
global UNLOCK_DURATION_MS := 10000         ; unlock duration in ms (10s)
global _autoLockTimerActive := false

; create a reusable function object for the timer callback
global _autoLockTimerObj := () => AutoLockTimer()

; initial tray tip
TrayTip("Kiosk", "Kiosk keyguard running. Unlock with Ctrl+Alt+Shift+O", 3)

; --------------------
; HotIf block: when Toggle is FALSE, these hotkeys are active (blocked)
; --------------------
#HotIf !Toggle

; Block left/right Win keys
LWin::Return
RWin::Return

; Block Win+D (Show Desktop)
#d::Return

; Block Win+Down (minimize/snap)
#Down::Return

; Block Alt+F4 (Close App)
!F4::Return

; Block Alt keys entirely (prevents Alt+Tab)
LAlt::Return
RAlt::Return

; Block Function Keys
F11::Return             ; fullscreen toggle
; F5::Return            ; (Optional) Unity doesn't usually use F5, but good to keep
; ^r::Return            ; (Optional) Browser refresh - not needed for Unity but harmless

#HotIf  ; end conditional block

; --------------------
; Unlock toggle: Ctrl + Alt + Shift + O
; --------------------
^!+o:: {
    global Toggle, _autoLockTimerActive, _autoLockTimerObj, UNLOCK_DURATION_MS
    Toggle := true
    TrayTip("Kiosk", "Shortcuts UNLOCKED (10s)", 2)
    SetTimer(_autoLockTimerObj, -UNLOCK_DURATION_MS)
    _autoLockTimerActive := true
    Return
}

; Manual re-lock: Ctrl+Alt+Shift+L
^!+l:: {
    global Toggle, _autoLockTimerActive, _autoLockTimerObj
    Toggle := false
    _autoLockTimerActive := false
    SetTimer(_autoLockTimerObj, "Off")
    TrayTip("Kiosk", "Shortcuts LOCKED", 1)
    Return
}

; Exit script (maintenance): Ctrl+Alt+Shift+X
; This also Kills the Unity App when you exit the script for convenience
^!+x:: {
    TrayTip("Kiosk", "Exiting keyguard & Closing App...", 1)
    
    ; OPTIONAL: Force close the Unity app when you kill the script
    ; Replace 'YourAppName.exe' with your actual filename
    if ProcessExist("Team71 Holotube.exe")
        ProcessClose("Team71 Holotube.exe")
        
    Sleep 400
    ExitApp
}

; --------------------
; AutoLockTimer function
; --------------------
AutoLockTimer() {
    global Toggle, _autoLockTimerActive
    Toggle := false
    _autoLockTimerActive := false
    TrayTip("Kiosk", "Shortcuts LOCKED", 1)
}

; --------------------
; LAUNCH UNITY APPLICATION
; --------------------

; TODO: UPDATE THIS PATH to your built .exe file
unityExePath := "D:\Sandisk files\Projects\Unity Projects\Holotube Touchscreen\build\Team71 Holotube.exe"

; Kiosk Arguments:
; -screen-fullscreen 1 : Force fullscreen mode
; -popupwindow         : Borderless window (Crucial for multi-monitor setup)
; -nolog               : Optional, improves performance slightly by disabling the log file
unityArgs := "-screen-fullscreen 1 -popupwindow -nolog"

; Verify path exists to avoid silent failure
if !FileExist(unityExePath)
{
    MsgBox("Error: Unity Executable not found at:`n" unityExePath)
    ExitApp
}

; Run the application
Run('"' unityExePath '" ' unityArgs)

; Confirm Launch
Sleep 1000
TrayTip("Kiosk", "Unity App Launched — Keyguard Active", 2)