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
    [SerializeField] private int uiTargetWidth = 1920;
    [SerializeField] private int uiTargetHeight = 1080;
    [SerializeField] private int holoTargetWidth = 1004;
    [SerializeField] private int holoTargetHeight = 1840;

    [Header("Controllers")]
    [SerializeField] private SceneVideoController sceneController;
    [SerializeField] private List<TextMeshProUGUI> staticInspectorText;

    [Header("Settings")]
    [SerializeField] private float idleTimeout = 180f;

    [Header("Debug")]
    [Tooltip("Assign the invisible Skip_Form button here")]
    [SerializeField] private Button skipButton;

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
        if (!_displaysActivated && Display.displays.Length > 1) SetupDisplay();
        HandleIdleTimeout();
    }

    private void SetupDisplay()
    {
        int uiIndex = -1;
        int holoIndex = -1;

#if UNITY_EDITOR
        uiIndex = 0; holoIndex = 1;
        if (Display.displays.Length > 1) Display.displays[1].Activate();
        _displaysActivated = true;
#else
        for (int k = 1; k < Display.displays.Length; k++) Display.displays[k].Activate();
        if (Display.displays.Length > 1) _displaysActivated = true;

        for (int i = 0; i < Display.displays.Length; i++)
        {
            int w = Display.displays[i].systemWidth;
            int h = Display.displays[i].systemHeight;
            if (w == uiTargetWidth && h == uiTargetHeight) uiIndex = i;
            else if (w == holoTargetWidth && h == holoTargetHeight) holoIndex = i;
        }

        if (Display.displays.Length == 1) { uiIndex = 0; holoIndex = 0; }
        else {
             if (uiIndex != -1 && holoIndex != -1) { if (uiIndex == holoIndex) holoIndex = (uiIndex == 0) ? 1 : 0; }
             else if (uiIndex != -1) holoIndex = (uiIndex == 0) ? 1 : 0;
             else if (holoIndex != -1) uiIndex = (holoIndex == 0) ? 1 : 0;
             else { uiIndex = 0; holoIndex = 1; }
        }
#endif
        if (mainUICanvas) mainUICanvas.targetDisplay = uiIndex;
        if (holotubeCamera) holotubeCamera.targetDisplay = holoIndex;
    }

    private void Initialize()
    {
        // --- 1. Initialize Sub-Components ---
        formPage.Initialize();
        languagePage.Initialize();
        questionPage.Initialize();
        sceneController.Initialize();

        // --- 2. Setup Events ---
        formPage.OnFormSubmitted += GoToLanguage;
        languagePage.OnLanguageSelected += HandleLanguageSelection;
        questionPage.OnQuestionClicked += HandleQuestionClicked;
        questionPage.OnNextClicked += GoToEnding;
        endingPage.OnTimerFinished += ResetApp;

        sceneController.OnAnimationFinished += HandleAnimationFinished;
        sceneController.StopAndShowIdle();

        // --- 3. Start Flow ---
        GoToForm();

        // --- 4. SKIP BUTTON SETUP (Clean Version) ---

        // A. Reference Recovery (Kept this as it fixed the build issue)
        if (skipButton == null)
        {
            GameObject foundObj = GameObject.Find("Skip_Form");
            if (foundObj != null) skipButton = foundObj.GetComponent<Button>();
        }

        if (skipButton != null)
        {
            // B. Click Listener
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(DebugSkipForm);

            // C. Explicitly Set Active
            skipButton.gameObject.SetActive(true);

            // D. Force Visual Properties
            skipButton.transform.SetAsLastSibling();
            skipButton.gameObject.layer = LayerMask.NameToLayer("UI");

            // E. Force Position (Z=0)
            RectTransform rect = skipButton.GetComponent<RectTransform>();
            if (rect != null)
            {
                Vector3 pos = rect.localPosition;
                pos.z = 0;
                rect.localPosition = pos;
            }

            // F. Make Invisible (Alpha 0)
            Image btnImage = skipButton.GetComponent<Image>();
            if (btnImage != null)
            {
                Color c = btnImage.color;
                c.a = 0f;
                btnImage.color = c;
            }

            // G. Clear Text (if any exists)
            TextMeshProUGUI btnText = skipButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = "";
        }
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

        // ENABLE SKIP BUTTON EXPLICITLY
        if (skipButton)
        {
            skipButton.gameObject.SetActive(true);
            skipButton.transform.SetAsLastSibling();
        }
    }

    private void GoToLanguage()
    {
        _isOnFormPage = false;
        if (footerObject) footerObject.SetActive(false);
        formPage.Hide();
        languagePage.Show();

        // DISABLE SKIP BUTTON
        if (skipButton) skipButton.gameObject.SetActive(false);
    }

    private void HandleLanguageSelection(bool isArabic)
    {
        _isArabic = isArabic;
        DataLogger.SetLanguage(isArabic ? "Arabic" : "English");

        string fileName = isArabic ? "data_arab" : "data_eng";
        _currentData = DataManager.LoadData<AppData>(fileName);

        sceneController.UpdateModels(_currentData);

        GoToQuestions();
        sceneController.PlayIntro();
    }

    private void GoToQuestions()
    {
        if (footerObject) footerObject.SetActive(true);
        languagePage.Hide();
        questionPage.Show();
        questionPage.Refresh(_currentData, _isArabic, -1);

        // Ensure Skip button is OFF (just in case)
        if (skipButton) skipButton.gameObject.SetActive(false);
    }

    private void HandleQuestionClicked(int index)
    {
        _currentIdleTimer = 0f;
        DataLogger.TrackQuestion(index);
        sceneController.PlaySequence(index);
    }

    private void GoToEnding()
    {
        DataLogger.SaveSession("Complete");
        if (footerObject) footerObject.SetActive(false);
        questionPage.Hide();
        endingPage.Show();
    }

    private void HandleAnimationFinished()
    {
        // No logic needed here
    }

    // --- Reset & Timeout ---
    private void ResetApp()
    {
        _currentIdleTimer = 0f;
        GoToForm();
    }

    private void HandleIdleTimeout()
    {
        bool isInteracting = false;

        // Input System Check
        if (Mouse.current != null) { Vector2 d = Mouse.current.delta.ReadValue(); if (Mathf.Abs(d.x) > 0.1f || Mathf.Abs(d.y) > 0.1f) isInteracting = true; }
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
                    DataLogger.SaveSession("Timeout");
                    ResetApp();
                }
                else if (_isOnFormPage && formPage.HasUserInput())
                {
                    ResetApp();
                }
            }
        }
    }

    // --- SKIP LOGIC ---
    public void DebugSkipForm()
    {
        // Simple console log only
        Debug.Log("[AppManager] Debug Skip Triggered.");

        DataLogger.SetUserDetails("Skipped User", "N/A", "skipped", "skipped");
        GoToLanguage();
    }
}