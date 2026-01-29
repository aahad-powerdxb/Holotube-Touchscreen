using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video; // NEW

public class SceneVideoController : MonoBehaviour
{
    [Header("References")]
    // CHANGED: Now references the VideoPlayer, not a generic container
    [SerializeField] private VideoPlayer videoPlayer;

    public event Action OnAnimationFinished;

    private VideoLogic _logic;
    private Coroutine _activeRoutine;

    public void Initialize()
    {
        // Setup Logic
        _logic = new VideoLogic(videoPlayer);

        // Start Idle Loop immediately (-1 index)
        _logic.PlayVideo(-1, false);
    }

    public void UpdateModels(AppData data, bool isArabic)
    {
        int count = data.page3.animation.Count;
        string[] names = new string[count];
        for (int i = 0; i < count; i++) names[i] = data.page3.animation[i].text;

        if (!isArabic) _logic.SetFileNames(names, null);
        else _logic.SetFileNames(null, names);
    }

    public void PlaySequence(int index, bool isArabic)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);

        _logic.PlayVideo(index, isArabic);
        _activeRoutine = StartCoroutine(WaitAndPlayIdle());
    }

    public int GetPlayingIndex(bool isArabic)
    {
        return _logic.GetPlayingIndex(isArabic);
    }

    private IEnumerator WaitAndPlayIdle()
    {
        // Give the VideoPlayer a moment to load the new clip metadata
        yield return null;
        yield return new WaitForSeconds(0.1f);

        // Wait for length of video
        double duration = _logic.GetCurrentClipLength();

        if (duration > 0)
        {
            yield return new WaitForSeconds((float)duration);
        }
        else
        {
            yield return new WaitForSeconds(2.0f); // Safety fallback
        }

        // Return to Idle Loop
        _logic.PlayVideo(-1, false); // -1 = Idle

        OnAnimationFinished?.Invoke();
    }
}