The NUC **does NOT** need Unity installed. It only needs the built files (`.exe`, `_Data` folder, etc.) and a few standard Windows dependencies.

Here is a professional `README.md` you can save in your project folder. It covers every step to go from a "clean" NUC to a fully functioning Kiosk.

---

# Kiosk Setup Instructions (NUC)

**App Name:** Team71 Holotube
**Target Machine:** Windows NUC (Touchscreen + Vertical Monitor)

---

## 🛑 1. Prerequisites (Install First)

The NUC is likely missing these standard drivers. Install them before copying the app.

1. **Visual C++ Redistributable (x64)**
* *Required for Unity apps to run.*
* Download: [Latest VC++ Redist x64](https://www.google.com/search?q=https://aka.ms/vs/17/release/vc_redist.x64.exe)


2. **AutoHotkey v2**
* *Required for the Kiosk Keyblocker script.*
* Download: [AutoHotkey v2.0](https://www.google.com/search?q=https://www.autohotkey.com/download/ahk-v2.exe)
* **Action:** Install to default path (`C:\Program Files\AutoHotkey`).


3. **GPU Drivers**
* Update Intel/Nvidia drivers to ensure the `VideoPlayer` runs smoothly.


4. **Touchscreen Drivers**
* Ensure the touch monitor works on the Desktop before running the app.



---

## 📂 2. Installation

1. **Copy Files:**
Copy your entire Build folder to the NUC's **Documents** folder.
* Target Path: `C:\Users\YOUR_USER\Documents\Kiosk_Build`


2. **Verify Contents:**
Inside that folder, ensure you have:
* `Team71 Holotube.exe`
* `Team71 Holotube_Data` (Folder)
* `master_start_unity.bat`
* `kiosk_keyblock.ahk`



---

## ⚙️ 3. Configuration (One-Time Setup)

Since we used "Relative Paths" in the scripts, you typically **don't** need to edit code. Just verify these specific Windows settings:

### A. Display Layout

1. Connect **Both Monitors**.
2. Right-click Desktop > **Display Settings**.
3. **Arrangement:** Ensure they are arranged logically (e.g., Touchscreen on Left, Vertical Holotube on Right).
4. **Scale & Layout:**
* **Scale:** Set both to **100%** (Crucial! If set to 125% or 150%, the Unity UI will look zoomed in).
* **Resolution:** Ensure Touchscreen is `1920x1080` and Holotube is `1004x1840` (or its native res).



### B. Touch Keyboard (Enable)

1. Go to **Settings > Time & Language > Typing**.
2. Expand **Touch Keyboard**.
3. Check/Toggle: **"Show the touch keyboard when there's no keyboard attached."**

### C. Taskbar (Auto-Hide)

1. Right-click Taskbar > **Taskbar Settings**.
2. Toggle **"Automatically hide the taskbar in desktop mode"**. (This prevents the taskbar from accidentally popping up over the kiosk).

---

## 🚀 4. Run on Startup (Kiosk Mode)

To make the app launch automatically when the NUC turns on:

1. **Create Shortcut:**
* Right-click `master_start_unity.bat`.
* Select **Show more options > Create shortcut**.


2. **Open Startup Folder:**
* Press `Win + R` on your keyboard.
* Type `shell:startup` and hit Enter.


3. **Move Shortcut:**
* Drag the shortcut you created into this Startup folder.



**Done!** Restart the NUC to test. It should boot up, wait a few seconds, and launch the Kiosk Keyblocker + Unity App.

---

## 🛠️ Troubleshooting

### App runs but screen is black / video missing

* **Fix:** Install **K-Lite Codec Pack (Standard)**. Windows "N" editions often lack media features needed for Unity `VideoPlayer`.
* **Check:** Ensure the `Team71 Holotube_Data` folder is in the same folder as the `.exe`.

### App launches in a small window (not fullscreen)

* **Fix:** The `.bat` file arguments might be failing.
* **Try:** Right-click `master_start_unity.bat` > Run as Administrator.
* **Check:** Ensure Display Scale is set to **100%** in Windows Settings.

### "AutoHotkey not found" Error

* **Fix:** Open `master_start_unity.bat` in Notepad.
* **Edit:** Check the line `set "AHK_EXE=..."`. If you installed AHK to a different drive (e.g., D:), update this path.

### Emergency Exit

* To close the Kiosk manually: Press **Ctrl + Alt + Shift + X** on a physical keyboard.