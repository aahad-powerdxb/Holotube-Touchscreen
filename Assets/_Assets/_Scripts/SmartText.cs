using ArabicSupport;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SmartText : MonoBehaviour
{
    private TextMeshProUGUI _tmp;

    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    public string Text
    {
        set
        {
            if (_tmp == null) _tmp = GetComponent<TextMeshProUGUI>();
            ApplySmartLogic(value);
        }
    }

    private void ApplySmartLogic(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            _tmp.text = "";
            return;
        }

        bool isPresentationForm = false;
        bool isStandardArabic = false;

        // 1. Analyze Content
        foreach (char c in content)
        {
            if (c >= 0xFE70 && c <= 0xFEFF) isPresentationForm = true;
            else if (c >= 0xFB50 && c <= 0xFDFF) isPresentationForm = true;
            else if (c >= 0x0600 && c <= 0x06FF) isStandardArabic = true;
        }

        // 2. Apply Settings
        if (isPresentationForm)
        {
            // Case A: Pre-Fixed Arabic -> Needs RTL
            if (!_tmp.isRightToLeftText) _tmp.isRightToLeftText = true;
            _tmp.text = content;
        }
        else if (isStandardArabic)
        {
            // Case B: Standard Arabic -> Needs LTR (because Fixer reverses it)
            if (_tmp.isRightToLeftText) _tmp.isRightToLeftText = false;
            _tmp.text = ArabicFixer.Fix(content);
        }
        else
        {
            // Case C: English -> Needs LTR
            if (_tmp.isRightToLeftText) _tmp.isRightToLeftText = false;
            _tmp.text = content;
        }

        // 3. THE SAFETY FIX: Force the mesh to rebuild immediately
        // This prevents the "one frame glitch" where it remembers the old direction
        _tmp.ForceMeshUpdate();
    }
}