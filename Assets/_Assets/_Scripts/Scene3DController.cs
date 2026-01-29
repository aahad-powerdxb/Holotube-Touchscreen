using System;
using System.Collections;
using UnityEngine;

public class Scene3DController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject container3D;

    // Events
    public event Action OnAnimationFinished;

    private AnimationLogic _logic;
    private Coroutine _activeRoutine;

    public void Initialize()
    {
        container3D.SetActive(true);
        _logic = new AnimationLogic(container3D);
        _logic.PlayIdle();
    }

    public void UpdateModels(AppData data, bool isArabic)
    {
        // Extract names from data
        int count = data.page3.animation.Count;
        string[] names = new string[count];
        for (int i = 0; i < count; i++) names[i] = data.page3.animation[i].text;

        if (!isArabic) _logic.setModels(names, null);
        else _logic.setModels(null, names);
    }

    public void PlaySequence(int index, bool isArabic)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);

        _logic.PlayAnimation(index, isArabic);
        _activeRoutine = StartCoroutine(WaitAndPlayIdle());
    }

    public int GetPlayingIndex(bool isArabic)
    {
        return _logic.GetPlayingAnimationIndex(isArabic);
    }

    private IEnumerator WaitAndPlayIdle()
    {
        yield return null; // Wait for state update

        // Find active animator
        Animator currentAnim = null;
        foreach (Transform child in container3D.transform)
        {
            if (child.gameObject.activeSelf)
            {
                currentAnim = child.GetComponent<Animator>();
                break;
            }
        }

        // Wait for finish
        if (currentAnim != null)
            yield return new WaitUntil(() => currentAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        else
            yield return new WaitForSeconds(2.0f); // Fallback

        // Reset 3D
        _logic.PlayIdle();

        // Notify UI
        OnAnimationFinished?.Invoke();
    }
}