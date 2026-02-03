using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using ArabicSupport;

public class FormPage : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Dropdown natDropdown;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField phoneInput;

    [Header("Static Arabic Text")]
    [SerializeField] private List<TextMeshProUGUI> arabicLabelsToFix;

    [Header("Footer Reference")]
    [Tooltip("Drag the 'Footer' GameObject here")]
    [SerializeField] private Transform footerContainer;

    public event Action OnFormSubmitted;

    public void Initialize()
    {
        // 1. Fix Arabic Text Labels
        foreach (var txt in arabicLabelsToFix)
        {
            if (txt != null) txt.text = ArabicFixer.Fix(txt.text);
        }

        // 2. Populate Dropdown
        // FIX: Removed the "Count == 0" check because Unity Dropdowns 
        // default to having 3 options (Option A, B, C), so count is never 0.
        if (natDropdown) PopulateCountries();

        // 3. Setup Button Listener (Find Home_Btn specifically)
        if (footerContainer)
        {
            Transform homeBtnTrans = footerContainer.Find("Home_Btn");
            if (homeBtnTrans)
            {
                Button btn = homeBtnTrans.GetComponent<Button>();
                if (btn)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(ValidateAndSubmit);
                }
            }
        }
    }

    public bool HasUserInput()
    {
        bool hasName = nameInput != null && !string.IsNullOrEmpty(nameInput.text);
        bool hasEmail = emailInput != null && !string.IsNullOrEmpty(emailInput.text);
        bool hasPhone = phoneInput != null && !string.IsNullOrEmpty(phoneInput.text);
        bool hasNat = natDropdown != null && natDropdown.value != 0;

        return hasName || hasEmail || hasPhone || hasNat;
    }

    private void ValidateAndSubmit()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text)) return;

        string name = nameInput.text;
        string email = emailInput.text;
        string phone = phoneInput.text;

        string nationality = "Not Selected";
        if (natDropdown && natDropdown.options.Count > 0)
        {
            nationality = natDropdown.options[natDropdown.value].text;
        }

        DataLogger.SetUserDetails(name, nationality, email, phone);
        OnFormSubmitted?.Invoke();
    }

    public void ClearForm()
    {
        if (nameInput) nameInput.text = "";
        if (emailInput) emailInput.text = "";
        if (phoneInput) phoneInput.text = "";
        if (natDropdown) natDropdown.value = 0;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateFooterButtons();
    }

    public void Hide() => gameObject.SetActive(false);

    private void UpdateFooterButtons()
    {
        if (!footerContainer) return;
        foreach (Transform child in footerContainer)
        {
            bool isHomeBtn = child.name == "Home_Btn";
            child.gameObject.SetActive(isHomeBtn);
        }
    }

    private void PopulateCountries()
    {
        // Clears "Option A, Option B, Option C"
        natDropdown.ClearOptions();

        // 1. Load Data via Manager
        CountryList data = DataManager.LoadCountries("country_data");

        // 2. Prepare List
        List<string> options = new List<string>();
        options.Add("Select Nationality...");

        if (data != null && data.countries != null)
        {
            foreach (Country c in data.countries)
            {
                options.Add(c.name);
            }
        }
        else
        {
            options.Add("Other");
            Debug.LogWarning("FormPage: Country data failed to load. Check 'Resources/Data/country_data.json'.");
        }

        // 3. Apply to Dropdown
        natDropdown.AddOptions(options);
    }
}