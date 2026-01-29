using UnityEngine;
using UnityEngine.Video;

public class VideoLogic
{
    private VideoPlayer _player;
    private string _idleName = "animation_looped_4K_v2"; // Default Idle Name

    // We map indices to filenames
    private string[] _englishFiles;
    private string[] _arabicFiles;

    public VideoLogic(VideoPlayer player)
    {
        _player = player;
    }

    public void SetFileNames(string[] english, string[] arabic)
    {
        _englishFiles = english;
        _arabicFiles = arabic;
    }

    /// <summary>
    /// Loads and plays a video from Resources/Videos folder.
    /// </summary>
    /// <param name="index">-1 for Idle, 0-N for Questions</param>
    public void PlayVideo(int index, bool isArabic)
    {
        string targetName = _idleName;

        // Determine filename
        if (index != -1) // If not requesting Idle
        {
            string[] targetList = isArabic ? _arabicFiles : _englishFiles;
            if (targetList != null && index >= 0 && index < targetList.Length)
            {
                targetName = targetList[index];
            }
        }

        // --- THE FIX IS HERE ---
        // Combine the folder name "Videos" with the file name
        // Example path: "Videos/animation_looped_4K_v2"
        string fullPath = "Videos/" + targetName;

        VideoClip clip = Resources.Load<VideoClip>(fullPath);

        if (clip != null)
        {
            _player.clip = clip;

            // Logic: Idle loops, Questions play once
            _player.isLooping = (index == -1);

            _player.Play();
        }
        else
        {
            // Debug error updated to help you debug paths
            Debug.LogError($"VideoClip not found! Looked for path: 'Resources/{fullPath}'");
        }
    }

    public double GetCurrentClipLength()
    {
        if (_player.clip != null) return _player.clip.length;
        return 0f;
    }

    // --- Backward Detection ---
    public int GetPlayingIndex(bool isArabic)
    {
        if (_player.clip == null) return -1;

        string currentName = _player.clip.name;
        if (currentName == _idleName) return -1;

        string[] targetList = isArabic ? _arabicFiles : _englishFiles;
        if (targetList != null)
        {
            for (int i = 0; i < targetList.Length; i++)
            {
                // Note: .clip.name usually returns just the file name, not the path.
                // So checking against targetList[i] is still correct.
                if (targetList[i] == currentName) return i;
            }
        }
        return -1;
    }
}