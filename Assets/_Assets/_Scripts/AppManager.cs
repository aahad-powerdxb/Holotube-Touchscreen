using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private HomePage homePage;
    [SerializeField] private QuestionPage questionPage;
    [SerializeField] private Scene3DController sceneController;

    [Header("Core UI")]
    [SerializeField] private BackButtonController backButtonController;
    // We are using SmartText components now
    [SerializeField] private List<SmartText> staticInspectorText;

    // State
    private AppData _currentData;
    private bool _isArabic;
    private bool _secondDisplayActive;

    private void Start()
    {
        if (Display.displays.Length > 1) { Display.displays[1].Activate(); _secondDisplayActive = true; }

        Initialize();
    }

    private void Update()
    {
        if (!_secondDisplayActive && Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            _secondDisplayActive = true;
        }
    }

    private void Initialize()
    {
        // 1. Process Global Static Text
        if (staticInspectorText != null)
        {
            foreach (var smartTxt in staticInspectorText)
            {
                if (smartTxt)
                {
                    // FIX: Instead of TextHelper, we just trigger the SmartText logic
                    // by re-assigning its own text to itself. 
                    // The .Text setter inside SmartText runs the logic automatically.
                    smartTxt.Text = smartTxt.GetComponent<TMPro.TextMeshProUGUI>().text;
                }
            }
        }

        // 2. Initialize Sub-Controllers
        homePage.Initialize();
        questionPage.Initialize();
        sceneController.Initialize();

        // 3. Subscribe to Events
        homePage.OnLanguageSelected += HandleLanguageSelection;
        questionPage.OnQuestionClicked += HandleQuestionClicked;
        sceneController.OnAnimationFinished += HandleAnimationFinished;

        // NEW: Init Back Button
        if (backButtonController)
        {
            // We don't need .Initialize() anymore, Awake handles it.
            // Just force an update to set the default language (English/False) immediately
            backButtonController.UpdateLanguage(false);

            backButtonController.GetComponent<Button>().onClick.AddListener(GoToHome);
        }

        // 4. Start
        GoToHome();
    }

    // --- Event Handlers ---

    private void HandleLanguageSelection(bool isArabic)
    {
        _isArabic = isArabic;
        _currentData = DataManager.LoadData(_isArabic);

        if (_currentData == null) return;

        // NEW: Update Back Button Visuals
        if (backButtonController) backButtonController.UpdateLanguage(_isArabic);

        // Pass data to subsystems
        sceneController.UpdateModels(_currentData, _isArabic);

        GoToQuestions();
    }

    private void HandleQuestionClicked(int index)
    {
        sceneController.PlaySequence(index, _isArabic);
    }

    private void HandleAnimationFinished()
    {
        // When 3D sequence ends, reset UI buttons
        questionPage.ResetVisuals();
    }

    // --- Navigation ---

    private void GoToHome()
    {
        homePage.Show();
        questionPage.Hide();

        // FIX: Use the controller reference instead of the deleted 'backButton' variable
        if (backButtonController)
            backButtonController.gameObject.SetActive(false);
    }

    private void GoToQuestions()
    {
        // Check 3D state for Backward Detection
        int playingIndex = sceneController.GetPlayingIndex(_isArabic);

        // Refresh Page
        questionPage.Refresh(_currentData, _isArabic, playingIndex);

        homePage.Hide();
        questionPage.Show();

        // FIX: Use the controller reference
        if (backButtonController)
            backButtonController.gameObject.SetActive(true);
    }
}