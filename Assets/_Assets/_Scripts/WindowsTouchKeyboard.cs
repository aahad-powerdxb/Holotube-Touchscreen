using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowsTouchKeyboard : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private const string LauncherPath = @"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe";

    // --- DLL IMPORTS ---
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string sClassName, string sAppName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    // CONSTANTS
    private const string KeyboardClass = "IPTip_Main_Window";
    private const uint WM_SYSCOMMAND = 0x0112;
    private static readonly IntPtr SC_CLOSE = (IntPtr)0xF060; // "Click the X button"

    private const byte VK_ESCAPE = 0x1B;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    // --- 1. OPEN (On Tap) ---
    public void OnSelect(BaseEventData eventData)
    {
        UnityEngine.Debug.Log($"[Keyboard] Selecting {gameObject.name}. Opening TabTip...");
        OpenKeyboard();
    }

    // --- 2. CLOSE (On Deselect) ---
    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(CheckSelectionAndClose());
    }

    private IEnumerator CheckSelectionAndClose()
    {
        // 1. Wait for Unity UI to update
        yield return null;

        GameObject next = EventSystem.current.currentSelectedGameObject;
        string nextName = next != null ? next.name : "NULL";

        UnityEngine.Debug.Log($"[Keyboard] Deselected. New Selection: {nextName}");

        // 2. If user switched to another Input Field, Stop.
        if (next != null && next.GetComponent<TMP_InputField>() != null)
        {
            UnityEngine.Debug.Log("[Keyboard] Switched to new input. Aborting close.");
            yield break;
        }

        // 3. START CLOSING SEQUENCE
        UnityEngine.Debug.Log("[Keyboard] Valid deselect. Attempting to minimize...");

        // === STRATEGY A: The "Polite Window Close" ===
        // This is cleaner than ESC if it works.
        IntPtr windowHandle = FindWindow(KeyboardClass, null);

        if (windowHandle != IntPtr.Zero)
        {
            UnityEngine.Debug.Log($"[Keyboard] Found Window Handle: {windowHandle}. Sending Close Command...");
            PostMessage(windowHandle, WM_SYSCOMMAND, SC_CLOSE, IntPtr.Zero);
            yield break; // If we found it, we are done.
        }
        else
        {
            UnityEngine.Debug.LogWarning("[Keyboard] Could not find 'IPTip_Main_Window'. Moving to Strategy B...");
        }

        // === STRATEGY B: The "ESC Key" (Fallback) ===
        // We tried finding the window and failed. Now we use brute force.

        // Wait longer (0.4s) to ensure Touch is fully released
        UnityEngine.Debug.Log("[Keyboard] Waiting 0.4s for touch release...");
        yield return new WaitForSeconds(0.4f);

        UnityEngine.Debug.Log("[Keyboard] Simulating ESC Key...");

        try
        {
            // Press
            keybd_event(VK_ESCAPE, 0, 0, UIntPtr.Zero);
            // Release
            keybd_event(VK_ESCAPE, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            UnityEngine.Debug.Log("[Keyboard] ESC Signal Sent.");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"[Keyboard] ESC Failed: {e.Message}");
        }
    }

    private void OpenKeyboard()
    {
        try
        {
            if (File.Exists(LauncherPath)) Process.Start(LauncherPath);
        }
        catch (Exception) { }
    }
}