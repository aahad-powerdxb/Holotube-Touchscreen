using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for manual click detection

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class BackButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("English Sprites")]
    public Sprite engIdle;
    public Sprite engActive;

    [Header("Arabic Sprites")]
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
        // We are taking full control now.
        _btn.transition = Selectable.Transition.None;
    }

    public void UpdateLanguage(bool isArabic)
    {
        // 1. Update our internal "Target" sprites
        _currentIdle = isArabic ? arIdle : engIdle;
        _currentActive = isArabic ? arActive : engActive;

        // 2. Immediately apply the Idle sprite
        if (_img != null)
        {
            _img.sprite = _currentIdle;
        }
    }

    // --- MANUAL EVENT HANDLING ---

    // Detects when the mouse/finger goes DOWN
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_btn.interactable && _img != null)
        {
            _img.sprite = _currentActive;
        }
    }

    // Detects when the mouse/finger goes UP
    public void OnPointerUp(PointerEventData eventData)
    {
        if (_btn.interactable && _img != null)
        {
            _img.sprite = _currentIdle;
        }
    }
}