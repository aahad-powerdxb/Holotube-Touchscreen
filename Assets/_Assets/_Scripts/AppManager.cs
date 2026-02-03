using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private FormPage formPage;
    [SerializeField] private LanguagePage languagePage;
    [SerializeField] private QuestionPage questionPage;
    [SerializeField] private EndingPage endingPage;

    [Header("Global UI")]
    [SerializeField] private GameObject footerObject;

    [Header("Display Setup")]
    [Tooltip("The Canvas for the Touchscreen Interface")]
    [SerializeField] private Canvas mainUICanvas;

    [Tooltip("The Camera rendering the Holotube/Video content")]
    [SerializeField] private Camera holotubeCamera;

    [Header("Resolution Targeting")]
    [Tooltip("Target Resolution for the UI Monitor (e.g. 1920 x 1080)")]
    [SerializeField] private int uiTargetWidth = 1920;
    [SerializeField] private int uiTargetHeight = 1080;

    [Tooltip("Target Resolution for the Holotube Monitor (e.g. 1004 x 1840)")]
    [SerializeField] private int holoTargetWidth = 1004;
    [SerializeField] private int holoTargetHeight = 1840;

    [Header("Controllers")]
    [SerializeField] private SceneVideoController sceneController;
    [SerializeField] private List<TextMeshProUGUI> staticInspectorText;

    [Header("Settings")]
    [SerializeField] private float idleTimeout = 180f;

    // State
    private AppData _currentData;
    private bool _isArabic;
    private float _currentIdleTimer = 0f;
    private bool _isOnFormPage = true;
    private bool _displaysActivated = false;

    private void Start()
    {
        DataLogger.Initialize();
        SetupDisplay();
        Initialize();
    }

    private void Update()
    {
        // Fail-safe: If a second monitor is plugged in AFTER the app starts,
        // we re-run the setup to distribute the windows correctly.
        if (!_displaysActivated && Display.displays.Length > 1)
        {
            SetupDisplay();
        }
        HandleIdleTimeout();
    }

    private void SetupDisplay()
    {
        int uiIndex = -1;   // -1 means "Not Found Yet"
        int holoIndex = -1;

#if UNITY_EDITOR
        // --- EDITOR MODE ---
        // Force split screens for testing comfort
        uiIndex = 0;
        holoIndex = 1;

        if (Display.displays.Length > 1) Display.displays[1].Activate();
        _displaysActivated = true;
#else
        // --- BUILD / KIOSK MODE ---

        // 1. Activate all available displays first
        for (int k = 1; k < Display.displays.Length; k++)
        {
            Display.displays[k].Activate();
        }
        if (Display.displays.Length > 1) _displaysActivated = true;

        // 2. Search for Monitors by Resolution
        for (int i = 0; i < Display.displays.Length; i++)
        {
            int w = Display.displays[i].systemWidth;
            int h = Display.displays[i].systemHeight;

            // Check for UI Target
            if (w == uiTargetWidth && h == uiTargetHeight)
            {
                uiIndex = i;
                Debug.Log($"[Display Setup] Found UI Target ({w}x{h}) at Index {i}");
            }
            // Check for Holo Target
            else if (w == holoTargetWidth && h == holoTargetHeight)
            {
                holoIndex = i;
                Debug.Log($"[Display Setup] Found Holo Target ({w}x{h}) at Index {i}");
            }
        }

        // 3. Resolve Conflicts & Fallbacks

        // CASE A: Only 1 Monitor Connected
        if (Display.displays.Length == 1)
        {
            Debug.Log("[Display Setup] Single monitor detected. Stacking both displays.");
            uiIndex = 0;
            holoIndex = 0;
        }
        // CASE B: At least 2 Monitors Connected
        else
        {
            if (uiIndex != -1 && holoIndex != -1)
            {
                // Both were found! (Ideal Scenario)
                // Just in case they are the SAME index (e.g. two identical screens), enforce separation
                if (uiIndex == holoIndex) 
                {
                     holoIndex = (uiIndex == 0) ? 1 : 0; 
                }
            }
            else if (uiIndex != -1)
            {
                // Only UI found. Holo takes the "other" one.
                holoIndex = (uiIndex == 0) ? 1 : 0;
                Debug.Log($"[Display Setup] Holo target missing. Defaulting Holo to Index {holoIndex}");
            }
            else if (holoIndex != -1)
            {
                // Only Holo found. UI takes the "other" one.
                uiIndex = (holoIndex == 0) ? 1 : 0;
                Debug.Log($"[Display Setup] UI target missing. Defaulting UI to Index {uiIndex}");
            }
            else
            {
                // Neither found. Fallback to standard order.
                uiIndex = 0;
                holoIndex = 1;
                Debug.LogWarning("[Display Setup] No matching resolutions found. Using default 0 & 1.");
            }
        }
#endif

        Debug.Log($"[Display Setup] FINAL -> UI: Display {uiIndex + 1} | Holo: Display {holoIndex + 1}");

        // 4. Assign to Unity Objects
        if (mainUICanvas) mainUICanvas.targetDisplay = uiIndex;
        if (holotubeCamera) holotubeCamera.targetDisplay = holoIndex;
    }

    private void Initialize()
    {
        formPage.Initialize();
        languagePage.Initialize();
        questionPage.Initialize();
        sceneController.Initialize();

        formPage.OnFormSubmitted += GoToLanguage;
        languagePage.OnLanguageSelected += HandleLanguageSelection;
        questionPage.OnQuestionClicked += HandleQuestionClicked;
        questionPage.OnNextClicked += GoToEnding;
        endingPage.OnTimerFinished += ResetApp;
        sceneController.OnAnimationFinished += HandleAnimationFinished;

        GoToForm();
    }

    // --- Navigation Flow ---
    private void GoToForm()
    {
        _isOnFormPage = true;
        DataLogger.StartNewSession();
        if (footerObject) footerObject.SetActive(true);
        formPage.ClearForm();
        formPage.Show();
        languagePage.Hide();
        questionPage.Hide();
        endingPage.Hide();
        sceneController.PlaySequence(-1, false);
    }

    private void GoToLanguage()
    {
        _isOnFormPage = false;
        if (footerObject) footerObject.SetActive(false);
        formPage.Hide();
        languagePage.Show();
    }

    private void HandleLanguageSelection(bool isArabic)
    {
        _isArabic = isArabic;
        DataLogger.SetLanguage(isArabic ? "Arabic" : "English");
        string fileName = isArabic ? "data_arab" : "data_eng";
        _currentData = DataManager.LoadData<AppData>(fileName);
        sceneController.UpdateModels(_currentData, _isArabic);
        GoToQuestions();
    }

    private void GoToQuestions()
    {
        if (footerObject) footerObject.SetActive(true);
        languagePage.Hide();
        questionPage.Show();
        questionPage.Refresh(_currentData, _isArabic, -1);
    }

    private void HandleQuestionClicked(int index)
    {
        _currentIdleTimer = 0f;
        DataLogger.TrackQuestion(index);
        sceneController.PlaySequence(index, _isArabic);
    }

    private void GoToEnding()
    {
        DataLogger.SaveSession("Complete");
        if (footerObject) footerObject.SetActive(false);
        questionPage.Hide();
        endingPage.Show();
    }

    private void HandleAnimationFinished() { }

    // --- Reset & Timeout ---
    private void ResetApp()
    {
        _currentIdleTimer = 0f;
        GoToForm();
    }

    private void HandleIdleTimeout()
    {
        bool isInteracting = false;
        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            if (Mathf.Abs(delta.x) > 0.1f || Mathf.Abs(delta.y) > 0.1f) isInteracting = true;
        }
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) isInteracting = true;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed) isInteracting = true;

        if (isInteracting)
        {
            _currentIdleTimer = 0f;
        }
        else
        {
            _currentIdleTimer += Time.deltaTime;

            if (_currentIdleTimer >= idleTimeout)
            {
                if (!_isOnFormPage)
                {
                    Debug.Log("Idle Timeout (Deep). Saving & Resetting.");
                    DataLogger.SaveSession("Timeout");
                    ResetApp();
                }
                else if (_isOnFormPage && formPage.HasUserInput())
                {
                    Debug.Log("Idle Timeout (Form Abandoned). Wiping Data & Resetting.");
                    ResetApp();
                }
            }
        }
    }
}