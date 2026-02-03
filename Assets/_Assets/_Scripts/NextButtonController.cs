using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class NextButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Mode Settings")]
    [Tooltip("If true, only the English/Universal sprites will be used regardless of language.")]
    [SerializeField] private bool isBilingual = false;

    [Header("Universal / English Sprites")]
    public Sprite engIdle;
    public Sprite engActive;

    [Header("Arabic Sprites (Ignored if Bilingual)")]
    public Sprite arIdle;
    public Sprite arActive;

    private Image _img;
    private Button _btn;

    // Internal state tracking
    private Sprite _currentIdle;
    private Sprite _currentActive;

    private void Awake()
    {
        _img = GetComponent<Image>();
        _btn = GetComponent<Button>();

        // IMPORTANT: Disable Unity's built-in coloring/swapping
        if (_btn) _btn.transition = Selectable.Transition.None;

        // Initial setup to ensure valid sprites are loaded immediately
        // We default to English/Universal initially
        UpdateLanguage(false);
    }

    public void UpdateLanguage(bool isArabic)
    {
        if (isBilingual)
        {
            // Always use the "Universal" set (assigned to eng slots)
            _currentIdle = engIdle;
            _currentActive = engActive;
        }
        else
        {
            // Swap based on language
            _currentIdle = isArabic ? arIdle : engIdle;
            _currentActive = isArabic ? arActive : engActive;
        }

        // Immediately apply the Idle sprite
        if (_img != null && _currentIdle != null)
        {
            _img.sprite = _currentIdle;
        }
    }

    // --- MANUAL EVENT HANDLING ---

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_btn && _btn.interactable && _img != null && _currentActive != null)
        {
            _img.sprite = _currentActive;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_btn && _btn.interactable && _img != null && _currentIdle != null)
        {
            _img.sprite = _currentIdle;
        }
    }
}