using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // Required for TMP_Dropdown

public class FocusHighlighter : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("UI Reference")]
    [Tooltip("Assign the Border Image child object here.")]
    [SerializeField] private GameObject borderObject;

    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        // Safety: Ensure the border starts hidden
        if (borderObject) borderObject.SetActive(false);

        // Check if this highlighter is attached to a Dropdown
        _dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Update()
    {
        // FIX: If this is a dropdown, we need to manually check when to turn OFF the border
        // because standard Deselect logic breaks when the dropdown list is open.
        if (_dropdown != null && borderObject.activeSelf)
        {
            // If the dropdown is NOT expanded AND we are NOT the selected object...
            if (!_dropdown.IsExpanded && EventSystem.current.currentSelectedGameObject != gameObject)
            {
                // ...then we are truly deselected. Hide the border.
                borderObject.SetActive(false);
            }
        }
    }

    // Called when the user clicks/tabs into this field
    public void OnSelect(BaseEventData eventData)
    {
        if (borderObject) borderObject.SetActive(true);
    }

    // Called when the user clicks away or tabs out
    public void OnDeselect(BaseEventData eventData)
    {
        // FIX: If this is a dropdown and it is currently open, IGNORE the deselect event.
        // This keeps the border alive while the user scrolls the list.
        if (_dropdown != null && _dropdown.IsExpanded) return;

        if (borderObject) borderObject.SetActive(false);
    }
}