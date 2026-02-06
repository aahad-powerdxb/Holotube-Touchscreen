using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownScrollKeeper : MonoBehaviour
{
    private float _savedPosition = 1.0f;
    private bool _wasExpanded = false;
    private TMP_Dropdown _dropdown;

    void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
    }

    void Update()
    {
        if (_dropdown == null) return;

        if (_dropdown.IsExpanded)
        {
            // Find the list
            ScrollRect listScroll = GetComponentInChildren<ScrollRect>();

            if (listScroll != null)
            {
                // We don't hide anything anymore! Just manage scroll.

                if (!_wasExpanded)
                {
                    // First frame: Restore to top
                    listScroll.verticalNormalizedPosition = _savedPosition;
                }
                else
                {
                    // While open: Keep track of position
                    _savedPosition = listScroll.verticalNormalizedPosition;
                }
            }
            _wasExpanded = true;
        }
        else
        {
            _wasExpanded = false;
        }
    }

    public void ResetToTop()
    {
        _savedPosition = 1.0f;
    }
}