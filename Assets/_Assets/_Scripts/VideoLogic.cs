using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoLogic
{
    private VideoPlayer _player;
    private RawImage _screen; // The single RawImage component

    // Assets
    private Texture _logoTexture;     // Your static logo
    private Texture _videoRenderTex;  // The video player's output texture

    // Current Playlist
    private string _currentIntroName;
    private string[] _currentQuestionFiles;

    public VideoLogic(VideoPlayer player, RawImage screen)
    {
        _player = player;
        _screen = screen;

        // 1. Save the Video Texture (so we can swap back to it later)
        // Ensure the VideoPlayer in Inspector has "Target Texture" assigned!
        _videoRenderTex = _player.targetTexture;

        if (_videoRenderTex == null)
            Debug.LogError("VideoLogic: VideoPlayer is missing a Target Texture! Please assign one in the Inspector.");

        // 2. Load the Logo Texture
        // Note: This loads the raw texture. If your logo is a Sprite, this grabs the texture behind it.
        _logoTexture = Resources.Load<Texture>("Images/Idle");

        if (_logoTexture == null)
            Debug.LogError("VideoLogic: Could not find 'Resources/Images/logo'. Check file name/path.");
    }

    public void ConfigurePlaylist(string introFileName, string[] questionFiles)
    {
        _currentIntroName = introFileName;
        _currentQuestionFiles = questionFiles;
    }

    public void ShowIdleImage()
    {
        _player.Stop();

        if (_screen != null && _logoTexture != null)
        {
            // SWAP: Show the Logo
            _screen.texture = _logoTexture;
            _screen.color = Color.white; // Ensure it's fully visible
        }
    }

    private void PlayVideoFile(string fileName)
    {
        // SWAP: Show the Video Texture
        if (_screen != null && _videoRenderTex != null)
        {
            _screen.texture = _videoRenderTex;
        }

        string fullPath = "Videos/" + fileName;
        VideoClip clip = Resources.Load<VideoClip>(fullPath);

        if (clip != null)
        {
            _player.clip = clip;
            _player.isLooping = false;
            _player.Play();
        }
        else
        {
            Debug.LogError($"VideoClip not found: '{fullPath}'");
            ShowIdleImage();
        }
    }

    public void PlayIntro()
    {
        if (!string.IsNullOrEmpty(_currentIntroName))
            PlayVideoFile(_currentIntroName);
    }

    public void PlayQuestion(int index)
    {
        if (_currentQuestionFiles != null && index >= 0 && index < _currentQuestionFiles.Length)
        {
            PlayVideoFile(_currentQuestionFiles[index]);
        }
    }

    public double GetCurrentClipLength()
    {
        if (_player.clip != null) return _player.clip.length;
        return 0f;
    }
}