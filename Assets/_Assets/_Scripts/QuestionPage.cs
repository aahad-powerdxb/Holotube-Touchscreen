using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

public class QuestionPage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform buttonsContainer;

    [Header("Footer Reference")]
    [Tooltip("Drag the 'Footer' GameObject here")]
    [SerializeField] private Transform footerContainer;

    public event Action<int> OnQuestionClicked;
    public event Action OnNextClicked;

    private int _totalQuestions = 0;
    private int _pressedCount = 0;

    // Track language state locally to handle Show() logic
    private bool _isArabic = false;

    public void Initialize()
    {
        if (!buttonsContainer) buttonsContainer = transform.Find("Question_Buttons");
    }

    public void Refresh(AppData data, bool isArabic, int playingAnimationIndex)
    {
        _isArabic = isArabic;
        _pressedCount = 0;
        _totalQuestions = data.page2.buttons.Count;
        int btnCount = buttonsContainer.childCount;

        // 1. Update Footer Buttons (Language Aware)
        UpdateFooterButtons();

        // 2. Setup Question Buttons
        for (int i = 0; i < _totalQuestions; i++)
        {
            if (i >= btnCount) break;

            var btnObj = buttonsContainer.GetChild(i);
            var btn = btnObj.GetComponent<Button>();

            // A. Reset State
            btn.interactable = true;
            btn.onClick.RemoveAllListeners();

            // B. Text Setup
            var tmpText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText)
            {
                string raw = data.page2.buttons[i].text;
                tmpText.text = isArabic ? ArabicFixer.Fix(raw) : raw;
                tmpText.fontWeight = FontWeight.Medium;
            }

            // C. Visuals Setup
            var visuals = btnObj.GetComponent<QuestionButtonVisuals>();
            if (!visuals) visuals = btnObj.gameObject.AddComponent<QuestionButtonVisuals>();
            visuals.SetState(false, false);

            // D. Click Logic
            int index = i;
            btn.onClick.AddListener(() =>
            {
                OnQuestionClicked?.Invoke(index);
                btn.interactable = false;
                visuals.SetState(true, true);
                CheckAutoAdvance();
            });
        }
    }

    private void CheckAutoAdvance()
    {
        _pressedCount++;
        if (_pressedCount >= _totalQuestions)
        {
            Debug.Log("All questions visited. Auto-advancing to Ending Page.");
            OnNextClicked?.Invoke();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        // Ensure footer is correct whenever we show the page
        UpdateFooterButtons();
    }

    public void Hide() => gameObject.SetActive(false);

    // --- NEW: Dynamic Footer Loop ---
    private void UpdateFooterButtons()
    {
        if (!footerContainer) return;

        // Determine which button name we want to see
        string targetBtnName = _isArabic ? "Question_Btn_Arab" : "Question_Btn_Eng";

        foreach (Transform child in footerContainer)
        {
            bool isTarget = child.name == targetBtnName;

            // 1. Set Visibility
            child.gameObject.SetActive(isTarget);

            // 2. Attach Listener (ONLY to the active button)
            if (isTarget)
            {
                Button btn = child.GetComponent<Button>();
                if (btn)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => OnNextClicked?.Invoke());
                }
            }
        }
    }

    public void ResetVisuals() { }
}