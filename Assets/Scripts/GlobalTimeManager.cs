using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class TimeApiResponse
{
    public string dateTime;
}

[Serializable]
public class LevelUnlockData
{
    [SerializeField] public string levelName = "Level 1";
    [SerializeField] public int month = 8;
    [SerializeField] public int day = 18;
    [SerializeField] public int hour = 16;
    
    public DateTime GetDateTime()
    {
        return new DateTime(2025, month, day, hour, 0, 0);
    }
}

public class GlobalTimeManager : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> levelUnlockTimeText;
    [SerializeField] private List<LevelUnlockData> levelUnlockDates = new List<LevelUnlockData>
    {
        new LevelUnlockData { levelName = "Level 1", month = 8, day = 18, hour = 16 },
        new LevelUnlockData { levelName = "Level 2", month = 8, day = 19, hour = 16 },
        new LevelUnlockData { levelName = "Level 3", month = 8, day = 20, hour = 16 },
        new LevelUnlockData { levelName = "Level 4", month = 8, day = 21, hour = 16 },
        new LevelUnlockData { levelName = "Level 5", month = 8, day = 22, hour = 16 }
    };
    
    [Header("Time Settings")]
    [SerializeField] private bool useServerTime = true;
    [SerializeField] private int timeoutSeconds = 5;
    [SerializeField] private string timeServerUrl;
    
    private DateTime _currentServerTime;
    private bool _timeDataLoaded;
    
    public static event Action OnTimeDataLoaded;
    
    public bool IsTimeDataLoaded => _timeDataLoaded;
    
    private void Start()
    {
        CharacterMovement.CanMove = false;
        StartCoroutine(GetTimeFromServer());
    }
    
    private IEnumerator GetTimeFromServer()
    {
        if (!useServerTime)
        {
            _currentServerTime = DateTime.Now;
            _timeDataLoaded = true;
            UpdateLevelUnlockTexts();
            OnTimeDataLoaded?.Invoke();
            yield break;
        }

        bool timeReceived = false;
        
        StartCoroutine(SendRequest((success, serverTime) =>
        {
            if (success && !timeReceived)
            {
                timeReceived = true;
                _currentServerTime = serverTime;
                _timeDataLoaded = true;
                UpdateLevelUnlockTexts();
                OnTimeDataLoaded?.Invoke();
            }
        }));
        
        // Czekaj na odpowiedź lub timeout
        float waitTime = 0f;
        
        while (!timeReceived && waitTime < timeoutSeconds)
        {
            yield return new WaitForSeconds(0.1f);
            waitTime += 0.1f;
        }

        // Jeśli nie otrzymano odpowiedzi, użyj czasu lokalnego
        if (!timeReceived)
        {
            _currentServerTime = DateTime.Now;
            _timeDataLoaded = true;
            UpdateLevelUnlockTexts();
            OnTimeDataLoaded?.Invoke();
        }
        
        CharacterMovement.CanMove = true;
    }

    private IEnumerator SendRequest(Action<bool, DateTime> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(timeServerUrl))
        {
            request.timeout = timeoutSeconds;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    TimeApiResponse response = JsonUtility.FromJson<TimeApiResponse>(request.downloadHandler.text);
                    DateTime serverTime = DateTime.Parse(response.dateTime);
                    callback(true, serverTime);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing time response: {ex.Message}");
                    callback(false, DateTime.MinValue);
                }
            }
            else
            {
                Debug.LogError($"Time request failed: {request.error}");
                callback(false, DateTime.MinValue);
            }
        }
    }
    
    public bool IsLevelUnlocked(int levelIndex)
    {
        if (PlayerPrefs.GetInt("LevelsCompleted", 0) < levelIndex)
            return false;
        if (levelIndex < 0 || levelIndex >= levelUnlockDates.Count)
            return false;
        
        return _currentServerTime >= levelUnlockDates[levelIndex].GetDateTime();
    }
    
    public DateTime GetLevelUnlockDate(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelUnlockDates.Count)
            return DateTime.MinValue;
        
        return levelUnlockDates[levelIndex].GetDateTime();
    }
    
    public TimeSpan GetTimeUntilUnlock(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelUnlockDates.Count)
            return TimeSpan.Zero;
        
        DateTime unlockDate = levelUnlockDates[levelIndex].GetDateTime();
        if (_currentServerTime >= unlockDate)
            return TimeSpan.Zero;
        
        return unlockDate - _currentServerTime;
    }

    private void UpdateLevelUnlockTexts()
    {
        if (levelUnlockTimeText == null || levelUnlockTimeText.Count == 0)
            return;
        
        if (levelUnlockDates == null || levelUnlockDates.Count == 0)
            return;
        
        for (int i = 0; i < Mathf.Min(levelUnlockTimeText.Count, levelUnlockDates.Count); i++)
        {
            if (levelUnlockTimeText[i] != null)
            {
                if (IsLevelUnlocked(i))
                {
                    levelUnlockTimeText[i].text = $"{levelUnlockDates[i].levelName}";
                }
                else
                {
                    if(GetTimeUntilUnlock(i) == TimeSpan.Zero)
                    {
                        levelUnlockTimeText[i].text = "Ukończ poprzedni poziom";
                    }
                    else {
                        DateTime timeUntil = GetLevelUnlockDate(i);
                        levelUnlockTimeText[i].text = $"Wróć: {timeUntil.Day:00}/{timeUntil.Month:00} o {timeUntil.Hour:00}:{timeUntil.Minute:00}";
                    }
                }
            }
        }
    }
}
