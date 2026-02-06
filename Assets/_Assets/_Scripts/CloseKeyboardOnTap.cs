using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CloseKeyboardOnTap : MonoBehaviour, IPointerDownHandler
{
    [Tooltip("Assign the invisible 'Focus_Dummy' button here")]
    [SerializeField] private Selectable dummyButton;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[1-Background] Tapped on: {gameObject.name}");

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null)
        {
            Debug.Log($"[1-Background] Current Selection was: {currentSelected.name}");

            if (dummyButton != null)
            {
                Debug.Log("[1-Background] Force-selecting Dummy Button to steal focus...");
                dummyButton.Select();
            }
            else
            {
                Debug.LogWarning("[1-Background] No Dummy Button assigned! Setting selection to NULL.");
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        else
        {
            Debug.Log("[1-Background] Nothing was selected, so no need to close keyboard.");
        }
    }
}