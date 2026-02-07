using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SceneVideoController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer;

    public event Action OnAnimationFinished;

    private VideoLogic _logic;
    private Coroutine _activeRoutine;

    // Transition Duration (1 second as requested)
    private const float FADE_DURATION = 1.0f;

    public void Initialize()
    {
        RawImage screen = videoPlayer.GetComponent<RawImage>();
        if (screen == null)
        {
            Debug.LogError("SceneVideoController: No RawImage found on VideoPlayer object!");
            return;
        }

        _logic = new VideoLogic(videoPlayer, screen);

        // Start invisible so we can fade in the idle loop
        _logic.SetScreenAlpha(0f);

        // Start Idle immediately
        _activeRoutine = StartCoroutine(StartIdleWithFade());
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

    // --- PLAYBACK METHODS ---

    public void PlayIntro()
    {
        // Interrupt whatever is playing
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);

        // Start the transition sequence
        _activeRoutine = StartCoroutine(PlayVideoSequence(() => _logic.PlayIntro()));
    }

    public void PlaySequence(int index)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);

        _activeRoutine = StartCoroutine(PlayVideoSequence(() => _logic.PlayQuestion(index)));
    }

    public void StopAndShowIdle()
    {
        // Force return to idle (e.g. on timeout)
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(StartIdleWithFade());
    }

    // --- COROUTINES ---

    // 1. Routine for playing Intro/Question -> Then transitioning back to Idle
    private IEnumerator PlayVideoSequence(Action playVideoAction)
    {
        // A. FADE OUT CURRENT VIDEO (1s)
        yield return StartCoroutine(Fade(1f, 0f));

        // B. SWAP CONTENT (Invisible)
        playVideoAction.Invoke();

        // Wait buffer for VideoPlayer to load and start rendering frames
        yield return new WaitForSeconds(0.2f);

        // C. FADE IN NEW VIDEO (1s)
        yield return StartCoroutine(Fade(0f, 1f));

        // D. WAIT FOR CONTENT
        double duration = _logic.GetCurrentClipLength();

        // Calculate wait time: Duration minus the fade-out time.
        // If video is 5s, we wait 4s, then start fading out during the last second.
        float waitTime = (float)duration - FADE_DURATION;
        if (waitTime < 0) waitTime = 0; // Safety for very short videos

        yield return new WaitForSeconds(waitTime);

        // E. FADE OUT CONTENT (1s)
        // This happens while the last second of the video is playing
        yield return StartCoroutine(Fade(1f, 0f));

        // F. SWAP TO IDLE
        _logic.ShowIdleVideo();
        yield return new WaitForSeconds(0.2f); // Buffer

        // G. FADE IN IDLE (1s)
        yield return StartCoroutine(Fade(0f, 1f));

        OnAnimationFinished?.Invoke();
    }

    // 2. Routine for starting Idle from scratch (Startup or Interruption)
    private IEnumerator StartIdleWithFade()
    {
        // Fade Out whatever is currently on screen
        yield return StartCoroutine(Fade(1f, 0f));

        // Swap
        _logic.ShowIdleVideo();
        yield return new WaitForSeconds(0.2f);

        // Fade In Idle
        yield return StartCoroutine(Fade(0f, 1f));
    }

    // 3. Generic Lerp for Alpha
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        while (timer < FADE_DURATION)
        {
            timer += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, timer / FADE_DURATION);
            _logic.SetScreenAlpha(currentAlpha);
            yield return null;
        }
        _logic.SetScreenAlpha(endAlpha);
    }
}