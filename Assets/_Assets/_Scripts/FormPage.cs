using ArabicSupport;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FormPage : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Dropdown natDropdown;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField phoneInput;

    [Header("Error Feedback")]
    [Tooltip("The Panel object that contains the error message")]
    [SerializeField] private GameObject errorPanel;
    [Tooltip("The Text component inside the error panel")]
    [SerializeField] private TextMeshProUGUI errorText;
    [Tooltip("How long the error message stays visible (seconds)")]
    [SerializeField] private float errorDuration = 3.0f;
    [Tooltip("How fast the error fades in/out")]
    [SerializeField] private float fadeDuration = 0.2f;

    // References for Fading logic
    private CanvasGroup _errorCanvasGroup;
    private Coroutine _errorRoutine;

    [Header("Scroll Keeper")]
    [SerializeField] private DropdownScrollKeeper scrollKeeper;

    [Header("Static Arabic Text")]
    [SerializeField] private List<TextMeshProUGUI> arabicLabelsToFix;

    [Header("Footer Reference")]
    [SerializeField] private Transform footerContainer;

    public event Action OnFormSubmitted;

    public void Initialize()
    {
        // 1. Fix Arabic Text
        foreach (var txt in arabicLabelsToFix) { if (txt != null) txt.text = ArabicFixer.Fix(txt.text); }

        // 2. Setup Components
        if (natDropdown && scrollKeeper == null) scrollKeeper = natDropdown.GetComponent<DropdownScrollKeeper>();
        if (natDropdown) PopulateCountries();

        // 3. Setup Footer Buttons & FORCE PADDING TO 0
        if (footerContainer)
        {
            // --- NEW: Force Padding Reset ---
            HorizontalLayoutGroup footerLayout = footerContainer.GetComponent<HorizontalLayoutGroup>();
            if (footerLayout != null)
            {
                // Create a new Padding object to modify values
                // (You cannot modify group.padding.right directly because it's a struct property)
                RectOffset newPadding = new RectOffset(
                    0, // Left
                    0, // Right
                    footerLayout.padding.top,
                    footerLayout.padding.bottom
                );

                footerLayout.padding = newPadding;

                // Force a rebuild so the visual change happens immediately
                LayoutRebuilder.ForceRebuildLayoutImmediate(footerContainer.GetComponent<RectTransform>());
            }
            // --------------------------------

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

        // 4. Setup Error Fading
        if (errorPanel)
        {
            _errorCanvasGroup = errorPanel.GetComponent<CanvasGroup>();
            if (_errorCanvasGroup == null) _errorCanvasGroup = errorPanel.AddComponent<CanvasGroup>();
        }

        HideError();
    }

    // Helper to hide the error panel
    private void HideError()
    {
        // Stop any active fade animations
        if (_errorRoutine != null) StopCoroutine(_errorRoutine);

        if (errorPanel)
        {
            errorPanel.SetActive(false);
        }
    }

    // Helper to show the error panel with a specific message
    private void ShowError(string message)
    {
        if (errorPanel && errorText)
        {
            // 1. Stop existing routines
            if (_errorRoutine != null) StopCoroutine(_errorRoutine);

            // 2. Set Content
            errorText.text = message;

            // 3. Enable Panel & Reset Alpha
            errorPanel.SetActive(true);
            if (_errorCanvasGroup) _errorCanvasGroup.alpha = 0f; // Start invisible

            // 4. Force Layout Rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(errorPanel.GetComponent<RectTransform>());

            // 5. Start the Fade In -> Wait -> Fade Out sequence
            _errorRoutine = StartCoroutine(ErrorSequence());
        }
    }

    private IEnumerator ErrorSequence()
    {
        // --- Phase 1: Fade In ---
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (_errorCanvasGroup)
                _errorCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        if (_errorCanvasGroup) _errorCanvasGroup.alpha = 1f;

        // --- Phase 2: Wait ---
        yield return new WaitForSeconds(errorDuration);

        // --- Phase 3: Fade Out ---
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (_errorCanvasGroup)
                _errorCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        if (_errorCanvasGroup) _errorCanvasGroup.alpha = 0f;

        // --- Phase 4: Disable ---
        errorPanel.SetActive(false);
    }

    public bool HasUserInput()
    {
        bool hasName = nameInput != null && !string.IsNullOrEmpty(nameInput.text);
        bool hasEmail = emailInput != null && !string.IsNullOrEmpty(emailInput.text);
        bool hasPhone = phoneInput != null && !string.IsNullOrEmpty(phoneInput.text);

        bool hasNat = false;
        if (natDropdown != null && natDropdown.options.Count > 0)
        {
            string selectedText = natDropdown.options[natDropdown.value].text;
            hasNat = !string.IsNullOrEmpty(selectedText);
        }

        return hasName || hasEmail || hasPhone || hasNat;
    }

    private void ValidateAndSubmit()
    {
        // 1. Reset Error State
        HideError();

        string name = nameInput.text;
        string email = emailInput.text;
        string phone = phoneInput.text;
        string nationality = "Not Selected";

        // 2. Validate Fields
        if (!FormValidator.IsNameValid(name))
        {
            ShowError("Please enter a valid Name.");
            return;
        }

        if (!FormValidator.IsEmailValid(email))
        {
            ShowError("Please enter a valid Email Address.");
            return;
        }

        if (!FormValidator.IsPhoneValid(phone))
        {
            ShowError("Please enter a valid Phone Number.");
            return;
        }

        // 3. Get Nationality
        if (natDropdown && natDropdown.options.Count > 0)
        {
            string selectedText = natDropdown.options[natDropdown.value].text;
            if (!string.IsNullOrEmpty(selectedText))
            {
                nationality = selectedText;
            }
            else
            {
                ShowError("Please select your Nationality.");
                return;
            }
        }

        // 4. Success!
        DataLogger.SetUserDetails(name, nationality, email, phone);
        OnFormSubmitted?.Invoke();
    }

    public void ClearForm()
    {
        if (nameInput) nameInput.text = "";
        if (emailInput) emailInput.text = "";
        if (phoneInput) phoneInput.text = "";

        HideError();
        ResetDropdown();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateFooterButtons();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

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
        natDropdown.ClearOptions();
        CountryList data = DataManager.LoadCountries("country_data");
        List<string> options = new List<string>();

        if (data != null && data.countries != null)
        {
            foreach (Country c in data.countries) options.Add(c.name);
        }
        else
        {
            options.Add("Other");
        }
        options.Add("");

        natDropdown.AddOptions(options);
        ResetDropdown();
    }

    private void ResetDropdown()
    {
        if (natDropdown && natDropdown.options.Count > 0)
        {
            natDropdown.value = natDropdown.options.Count - 1;
            natDropdown.RefreshShownValue();
            if (scrollKeeper) scrollKeeper.ResetToTop();
        }
    }
}