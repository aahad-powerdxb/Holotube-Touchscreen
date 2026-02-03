using UnityEngine;
using System.IO;
using System;

public static class DataLogger
{
    private static string _filePath;
    private static SessionData _currentSession = new SessionData();

    private class SessionData
    {
        // Default to "NULL" so the CSV clearly shows missing data
        public string Name = "NULL";
        public string Nationality = "NULL";
        public string Email = "NULL";
        public string Phone = "NULL";
        public string Language = "NULL";
        public bool[] QuestionsViewed = new bool[5];

        public void Reset()
        {
            Name = "NULL";
            Nationality = "NULL";
            Email = "NULL";
            Phone = "NULL";
            Language = "NULL";
            QuestionsViewed = new bool[5]; // Defaults to all false
        }
    }

    public static void Initialize()
    {
        string folder = Application.persistentDataPath;
        _filePath = Path.Combine(folder, "Team71_Holotube_Data.csv");

        EnsureFileExists();
    }

    private static void EnsureFileExists()
    {
        if (!File.Exists(_filePath))
        {
            try
            {
                using (StreamWriter sw = File.CreateText(_filePath))
                {
                    sw.WriteLine("Name,Nationality,Email,Phone,Language_Chosen,Q1,Q2,Q3,Q4,Q5,Timestamp,Status");
                }
            }
            catch (Exception e) { Debug.LogError($"[DataLogger] Header Error: {e.Message}"); }
        }
    }

    public static void StartNewSession()
    {
        _currentSession.Reset();
    }

    public static void SetUserDetails(string name, string nat, string email, string phone)
    {
        _currentSession.Name = Sanitize(name);
        _currentSession.Nationality = Sanitize(nat);
        _currentSession.Email = Sanitize(email);
        _currentSession.Phone = Sanitize(phone);
    }

    public static void SetLanguage(string language)
    {
        _currentSession.Language = language;
    }

    public static void TrackQuestion(int index)
    {
        if (index >= 0 && index < 5)
            _currentSession.QuestionsViewed[index] = true;
    }

    // Added 'status' parameter to track if it was a "Complete" run or "Timeout"
    public static void SaveSession(string status = "Complete")
    {
        if (string.IsNullOrEmpty(_filePath)) return;

        try
        {
            EnsureFileExists();

            string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss zzz");

            string q1 = _currentSession.QuestionsViewed[0].ToString();
            string q2 = _currentSession.QuestionsViewed[1].ToString();
            string q3 = _currentSession.QuestionsViewed[2].ToString();
            string q4 = _currentSession.QuestionsViewed[3].ToString();
            string q5 = _currentSession.QuestionsViewed[4].ToString();

            // Construct Line
            string line = $"{_currentSession.Name},{_currentSession.Nationality},{_currentSession.Email},{_currentSession.Phone},{_currentSession.Language},{q1},{q2},{q3},{q4},{q5},{timestamp},{status}";

            using (StreamWriter sw = File.AppendText(_filePath))
            {
                sw.WriteLine(line);
            }

            Debug.Log($"[DataLogger] Session Saved. Status: {status}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataLogger] Save Failed: {e.Message}");
        }
    }

    private static string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input)) return "NULL";
        return input.Replace(",", " ").Trim();
    }
}