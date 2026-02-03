using UnityEngine;
using System.IO;

public static class DataManager
{
    private const string FOLDER_PATH = "Data";

    /// <summary>
    /// Generic loader for standard JSON objects (like AppData).
    /// Usage: DataManager.LoadData<AppData>("data_eng");
    /// </summary>
    public static T LoadData<T>(string fileName)
    {
        string fullPath = Path.Combine(FOLDER_PATH, fileName);
        TextAsset jsonFile = Resources.Load<TextAsset>(fullPath);

        if (jsonFile == null)
        {
            Debug.LogError($"DataManager: File '{fullPath}' not found in Resources!");
            return default(T);
        }

        return JsonUtility.FromJson<T>(jsonFile.text);
    }

    /// <summary>
    /// Specific loader for the Country list because Unity cannot parse top-level JSON arrays directly.
    /// This wraps the array in a wrapper key before parsing.
    /// </summary>
    public static CountryList LoadCountries(string fileName)
    {
        string fullPath = Path.Combine(FOLDER_PATH, fileName);
        TextAsset jsonFile = Resources.Load<TextAsset>(fullPath);

        if (jsonFile == null)
        {
            Debug.LogError($"DataManager: Country file '{fullPath}' not found in Resources!");
            return null;
        }

        // HACK: Wrap the top-level array so JsonUtility can read it
        // From: [ {..}, {..} ] 
        // To:   { "countries": [ {..}, {..} ] }
        string wrappedJson = "{\"countries\":" + jsonFile.text + "}";

        return JsonUtility.FromJson<CountryList>(wrappedJson);
    }
}