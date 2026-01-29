using System;
using UnityEngine;
using UnityEngine.UI;

public class HomePage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Transform titleContainer;

    // Events
    public event Action<bool> OnLanguageSelected;

    private SmartText _engBtnText, _arabBtnText;
    private SmartText _engTitleText, _arabTitleText;

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
            _engBtnText = engBtn.GetComponentInChildren<SmartText>();
            engBtn.GetComponent<Button>().onClick.AddListener(() => OnLanguageSelected?.Invoke(false));
        }

        if (arabBtn)
        {
            _arabBtnText = arabBtn.GetComponentInChildren<SmartText>();
            arabBtn.GetComponent<Button>().onClick.AddListener(() => OnLanguageSelected?.Invoke(true));
        }

        // Setup Titles
        _engTitleText = titleContainer.Find("English_Title").GetComponent<SmartText>();
        _arabTitleText = titleContainer.Find("Arabic_Title").GetComponent<SmartText>();

        // Hybrid Load (Initial State)
        LoadInitialText();
    }

    private void LoadInitialText()
    {
        // We load both small files just for the landing page
        AppData eng = DataManager.LoadData(false);
        AppData arab = DataManager.LoadData(true);

        if (eng != null)
        {
            if (_engTitleText) _engTitleText.Text = eng.page1.text[0].title;
            if (_engBtnText) _engBtnText.Text = eng.page1.text[1].button;
        }
        if (arab != null)
        {
            if (_arabTitleText) _arabTitleText.Text = arab.page1.text[0].title;
            if (_arabBtnText) _arabBtnText.Text = arab.page1.text[1].button;
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}