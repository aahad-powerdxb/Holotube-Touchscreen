using UnityEngine;

public static class DataManager
{
    public static AppData LoadData(bool isArabic)
    {
        string fileName = isArabic ? "data_arab" : "data_eng";
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile == null)
        {
            Debug.LogError($"DataManager: JSON file '{fileName}' not found!");
            return null;
        }

        return JsonUtility.FromJson<AppData>(jsonFile.text);
    }
}