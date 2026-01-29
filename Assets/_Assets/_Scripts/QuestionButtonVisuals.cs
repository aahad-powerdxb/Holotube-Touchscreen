using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionButtonVisuals : MonoBehaviour
{
    [Header("Components")]
    public Image bgImage;
    public TextMeshProUGUI text;
    public Outline outline;

    [Header("Settings")]
    public float fadeDuration = 0.2f;

    // State Colors
    private Color _themeGreen;
    private Color _white = Color.white;

    private Coroutine _currentRoutine;

    private void Awake()
    {
        // Define your Green Color (#82C240)
        ColorUtility.TryParseHtmlString("#82C240", out _themeGreen);

        // Auto-fetch components if not assigned
        if (bgImage == null) bgImage = GetComponent<Image>();
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();
        if (outline == null) outline = GetComponent<Outline>();
    }

    public void SetState(bool isSelected, bool animate = true)
    {
        // 1. Determine Target Colors based on state
        // Selected: BG = White, Text = Green, Outline = Visible (Alpha 1)
        // Default:  BG = Green, Text = White, Outline = Invisible (Alpha 0)
        Color targetBg = isSelected ? _white : _themeGreen;
        Color targetText = isSelected ? _themeGreen : _white;
        float targetOutlineAlpha = isSelected ? 1f : 0f;

        // 2. Stop any running animation on this button
        if (_currentRoutine != null) StopCoroutine(_currentRoutine);

        // 3. Execute
        if (animate && gameObject.activeInHierarchy)
        {
            _currentRoutine = StartCoroutine(FadeRoutine(targetBg, targetText, targetOutlineAlpha));
        }
        else
        {
            // Instant Snap (good for initialization)
            ApplyColors(targetBg, targetText, targetOutlineAlpha);
        }
    }

    private IEnumerator FadeRoutine(Color endBg, Color endTxt, float endOutlineAlpha)
    {
        float elapsed = 0f;
        Color startBg = bgImage.color;
        Color startTxt = text.color;

        // Outline might be null, so check before accessing
        float startOutlineAlpha = (outline != null) ? outline.effectColor.a : 0f;
        Color outlineColor = (outline != null) ? outline.effectColor : Color.white;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Smoothly interpolate colors
            if (bgImage) bgImage.color = Color.Lerp(startBg, endBg, t);
            if (text) text.color = Color.Lerp(startTxt, endTxt, t);

            if (outline)
            {
                // Fade outline alpha instead of enabling/disabling for smoothness
                outlineColor.a = Mathf.Lerp(startOutlineAlpha, endOutlineAlpha, t);
                outline.effectColor = outlineColor;
                outline.enabled = true; // Ensure enabled while fading
            }

            yield return null;
        }

        // Final snap to ensure precise colors
        ApplyColors(endBg, endTxt, endOutlineAlpha);
    }

    private void ApplyColors(Color bg, Color txt, float outlineAlpha)
    {
        if (bgImage) bgImage.color = bg;
        if (text) text.color = txt;

        if (outline)
        {
            Color c = outline.effectColor;
            c.a = outlineAlpha;
            outline.effectColor = c;
            // Optimally disable component if invisible to save performance
            outline.enabled = (outlineAlpha > 0.01f);
        }
    }
}