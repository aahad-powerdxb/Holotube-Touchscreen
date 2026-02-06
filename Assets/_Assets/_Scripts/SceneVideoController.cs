using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for RawImage
using UnityEngine.Video;

public class SceneVideoController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the Video_Screen object here.")]
    [SerializeField] private VideoPlayer videoPlayer;

    // REMOVED: [SerializeField] private RawImage videoScreen; -> Redundant

    public event Action OnAnimationFinished;

    private VideoLogic _logic;
    private Coroutine _activeRoutine;

    public void Initialize()
    {
        // 1. Automatically find the RawImage component on the same object as the VideoPlayer
        RawImage screen = videoPlayer.GetComponent<RawImage>();

        if (screen == null)
        {
            Debug.LogError("SceneVideoController: Could not find a RawImage component on the VideoPlayer object! Please check Video_Screen.");
            return;
        }

        // 2. Pass both to the logic
        _logic = new VideoLogic(videoPlayer, screen);

        // 3. Start showing the logo immediately
        _logic.ShowIdleImage();
    }

    public void UpdateModels(AppData data)
    {
        if (data == null || data.page3 == null || data.page3.video == null) return;

        string introName = "";
        if (data.page3.video.Count > 0)
            introName = data.page3.video[0].intro;

        List<string> questionNames = new List<string>();
        for (int i = 1; i < data.page3.video.Count; i++)
            questionNames.Add(data.page3.video[i].text);

        _logic.ConfigurePlaylist(introName, questionNames.ToArray());
    }

    public void PlayIntro()
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _logic.PlayIntro();
        _activeRoutine = StartCoroutine(WaitAndReturnToIdle());
    }

    public void PlaySequence(int index)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _logic.PlayQuestion(index);
        _activeRoutine = StartCoroutine(WaitAndReturnToIdle());
    }

    public void StopAndShowIdle()
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _logic.ShowIdleImage();
    }

    private IEnumerator WaitAndReturnToIdle()
    {
        yield return null;
        yield return new WaitForSeconds(0.1f);

        double duration = _logic.GetCurrentClipLength();
        if (duration > 0)
            yield return new WaitForSeconds((float)duration);
        else
            yield return new WaitForSeconds(2.0f);

        _logic.ShowIdleImage();
        OnAnimationFinished?.Invoke();
    }
}