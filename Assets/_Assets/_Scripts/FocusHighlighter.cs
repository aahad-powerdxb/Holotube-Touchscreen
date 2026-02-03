using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for detecting selection

public class FocusHighlighter : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("UI Reference")]
    [Tooltip("Assign the Border Image child object here.")]
    [SerializeField] private GameObject borderObject;

    private void Awake()
    {
        // Safety: Ensure the border starts hidden
        if (borderObject) borderObject.SetActive(false);
    }

    // Called when the user clicks/tabs into this field
    public void OnSelect(BaseEventData eventData)
    {
        if (borderObject) borderObject.SetActive(true);
    }

    // Called when the user clicks away or tabs out
    public void OnDeselect(BaseEventData eventData)
    {
        if (borderObject) borderObject.SetActive(false);
    }
}