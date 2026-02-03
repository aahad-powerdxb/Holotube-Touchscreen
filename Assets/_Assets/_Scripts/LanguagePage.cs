using System;
using TMPro; // Standard TextMeshPro namespace
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport; // Required for the Fixer

public class LanguagePage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Transform titleContainer;

    // Events
    public event Action<bool> OnLanguageSelected;

    private TextMeshProUGUI _engBtnText, _arabBtnText;
    private TextMeshProUGUI _engTitleText, _arabTitleText;

    public void Initialize()
    {
        // Auto-find references if not assigned
        if (!buttonContainer) buttonContainer = transform.Find("Buttons");
        if (!titleContainer) titleContainer = transform.Find("Title");

        // Setup Buttons
        var engBtn = buttonContainer.Find("Btn_English");
        var arabBtn = buttonContainer.Find("Btn_Arabic");

        if (engBtn)
        {
            _engBtnText = engBtn.GetComponentInChildren<TextMeshProUGUI>();
            engBtn.GetComponent<Button>().onClick.AddListener(() => OnLanguageSelected?.Invoke(false));
        }

        if (arabBtn)
        {
            _arabBtnText = arabBtn.GetComponentInChildren<TextMeshProUGUI>();
            arabBtn.GetComponent<Button>().onClick.AddListener(() => OnLanguageSelected?.Invoke(true));
        }

        // Setup Titles
        _engTitleText = titleContainer.Find("English_Title").GetComponent<TextMeshProUGUI>();
        _arabTitleText = titleContainer.Find("Arabic_Title").GetComponent<TextMeshProUGUI>();

        // Hybrid Load (Initial State)
        LoadInitialText();
    }

    private void LoadInitialText()
    {
        // We load both small files just for the landing page
        // UPDATED: Using the new generic LoadData<T> with specific filenames
        AppData eng = DataManager.LoadData<AppData>("data_eng");
        AppData arab = DataManager.LoadData<AppData>("data_arab");

        // 1. English (Plain Assignment)
        if (eng != null)
        {
            if (_engTitleText) _engTitleText.text = eng.page1.text[0].title;
            if (_engBtnText) _engBtnText.text = eng.page1.text[1].button;
        }

        // 2. Arabic (Wrapped in Fixer)
        if (arab != null)
        {
            if (_arabTitleText)
            {
                // Fix the TMP title text
                _arabTitleText.text = ArabicFixer.Fix(arab.page1.text[0].title);
            }

            if (_arabBtnText)
            {
                // Fix the TMP button text
                _arabBtnText.text = ArabicFixer.Fix(arab.page1.text[1].button);
            }
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}