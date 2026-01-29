using System;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform buttonsContainer;

    // Events
    public event Action<int> OnQuestionClicked;

    public void Initialize()
    {
        if (!buttonsContainer) buttonsContainer = transform.Find("Question_Buttons");
    }

    public void Refresh(AppData data, bool isArabic, int playingAnimationIndex)
    {
        int dataCount = data.page2.buttons.Count;
        int btnCount = buttonsContainer.childCount;

        // 1. Setup Buttons
        for (int i = 0; i < dataCount; i++)
        {
            if (i >= btnCount) break;

            var btnObj = buttonsContainer.GetChild(i);
            var btn = btnObj.GetComponent<Button>();

            // Text
            var smartText = btnObj.GetComponentInChildren<SmartText>();
            if (smartText) smartText.Text = data.page2.buttons[i].text;

            // Visuals
            var visuals = btnObj.GetComponent<QuestionButtonVisuals>();
            if (!visuals) visuals = btnObj.gameObject.AddComponent<QuestionButtonVisuals>();

            // Listeners
            btn.onClick.RemoveAllListeners();
            int index = i;

            // Logic
            btn.onClick.AddListener(() => OnQuestionClicked?.Invoke(index));
            // Visuals
            btn.onClick.AddListener(() => HighlightButton(btn, true));
        }

        // 2. Backward Detection (Sync UI with 3D)
        Button activeBtn = null;
        if (playingAnimationIndex != -1 && playingAnimationIndex < buttonsContainer.childCount)
        {
            activeBtn = buttonsContainer.GetChild(playingAnimationIndex).GetComponent<Button>();
        }

        // 3. Instant Snap
        HighlightButton(activeBtn, false);
    }

    public void ResetVisuals()
    {
        // Called when animation finishes
        HighlightButton(null, true);
    }

    private void HighlightButton(Button target, bool animate)
    {
        foreach (Transform child in buttonsContainer)
        {
            var btn = child.GetComponent<Button>();
            if (!btn) continue;

            bool isTarget = (target != null) && (btn == target);
            var visuals = btn.GetComponent<QuestionButtonVisuals>();

            if (visuals) visuals.SetState(isTarget, animate);
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}