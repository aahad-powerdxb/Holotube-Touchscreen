using UnityEngine;

public class AnimationLogic
{
    private GameObject container;
    private readonly string idleName = "Placeholder_Anim_Idle";
    private string[] englishModels;
    private string[] arabicModels;

    public AnimationLogic(GameObject container3D)
    {
        this.container = container3D;
    }

    public void setModels(string[] english, string[] arabic)
    {
        englishModels = english;
        arabicModels = arabic;
    }

    public void PlayAnimation(int index, bool isArabic)
    {
        if (container == null) return;

        string targetName = idleName;
        string[] targetList = isArabic ? arabicModels : englishModels;

        if (targetList != null && index >= 0 && index < targetList.Length)
        {
            targetName = targetList[index];
        }

        foreach (Transform child in container.transform)
        {
            child.gameObject.SetActive(child.gameObject.name == targetName);
        }
    }

    public void PlayIdle()
    {
        if (container == null) return;
        foreach (Transform child in container.transform)
        {
            child.gameObject.SetActive(child.gameObject.name == idleName);
        }
    }

    // --- NEW: BACKWARD DETECTION ---
    public int GetPlayingAnimationIndex(bool isArabic)
    {
        if (container == null) return -1;

        // 1. Find the name of the currently active object
        string activeName = "";
        foreach (Transform child in container.transform)
        {
            if (child.gameObject.activeSelf)
            {
                activeName = child.gameObject.name;
                break;
            }
        }

        // 2. If it's Idle or nothing, return -1
        if (string.IsNullOrEmpty(activeName) || activeName == idleName) return -1;

        // 3. Search the current language list for this name
        string[] targetList = isArabic ? arabicModels : englishModels;
        if (targetList != null)
        {
            for (int i = 0; i < targetList.Length; i++)
            {
                if (targetList[i] == activeName)
                {
                    // Found it! This animation corresponds to Button [i]
                    return i;
                }
            }
        }

        return -1; // No match found
    }
}