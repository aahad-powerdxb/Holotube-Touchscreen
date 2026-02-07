using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoLogic
{
    private VideoPlayer _player;
    private RawImage _screen;
    private Texture _videoRenderTex;

    // Fixed path for the dedicated idle loop
    private string _idleVideoPath = "Idle";

    // Current Playlist
    private string _currentIntroName;
    private string[] _currentQuestionFiles;

    public VideoLogic(VideoPlayer player, RawImage screen)
    {
        _player = player;
        _screen = screen;
        _videoRenderTex = _player.targetTexture;

        if (_videoRenderTex == null)
            Debug.LogError("VideoLogic: VideoPlayer is missing a Target Texture! Please assign one in the Inspector.");
    }

    public void ConfigurePlaylist(string introFileName, string[] questionFiles)
    {
        _currentIntroName = introFileName;
        _currentQuestionFiles = questionFiles;

        // REMOVED: Overwriting _idleVideoPath. 
        // We keep it as "Idle" since you confirmed it is a separate file.
    }

    // --- ALPHA CONTROL FOR FADING ---
    public void SetScreenAlpha(float alpha)
    {
        if (_screen != null)
        {
            Color c = _screen.color;
            c.a = alpha;
            _screen.color = c;
        }
    }

    public void ShowIdleVideo()
    {
        if (!string.IsNullOrEmpty(_idleVideoPath))
        {
            PlayVideoFile(_idleVideoPath, true);
        }
    }

    private void PlayVideoFile(string fileName, bool shouldLoop = false)
    {
        // 1. Ensure Texture is assigned.
        // NOTE: We do NOT reset color/alpha here. The Controller handles fades.
        if (_screen != null && _videoRenderTex != null)
        {
            _screen.texture = _videoRenderTex;
        }

        string fullPath = "Videos/" + fileName;
        VideoClip clip = Resources.Load<VideoClip>(fullPath);

        if (clip != null)
        {
            // CRITICAL FIX: Stop and rewind to ensure clean transition
            _player.Stop();
            _player.clip = clip;
            _player.isLooping = shouldLoop;
            _player.time = 0; // Rewind
            _player.Play();
        }
        else
        {
            Debug.LogError($"VideoLogic: VideoClip not found at path: '{fullPath}'");
        }
    }

    public void PlayIntro()
    {
        if (!string.IsNullOrEmpty(_currentIntroName))
            PlayVideoFile(_currentIntroName, false);
    }

    public void PlayQuestion(int index)
    {
        if (_currentQuestionFiles != null && index >= 0 && index < _currentQuestionFiles.Length)
        {
            PlayVideoFile(_currentQuestionFiles[index], false);
        }
    }

    public double GetCurrentClipLength()
    {
        if (_player.clip != null) return _player.clip.length;
        return 0f;
    }
}