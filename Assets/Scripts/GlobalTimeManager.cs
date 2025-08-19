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
    public string date;
    public string time;
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
    [SerializeField] private int timeoutSeconds = 3;
    
    private const string TimeServerUrl = "https://www.timeapi.io/api/time/current/zone?timeZone=Europe%2FWarsaw";
    private DateTime _currentServerTime;
    private bool _timeDataLoaded;
    
    public static event Action OnTimeDataLoaded;
    
    public bool IsTimeDataLoaded => _timeDataLoaded;
    
    private void Start()
    {
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

        using UnityWebRequest request = UnityWebRequest.Get(TimeServerUrl);
        request.timeout = timeoutSeconds;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            _currentServerTime = DateTime.Now;
            _timeDataLoaded = true;
            UpdateLevelUnlockTexts();
            OnTimeDataLoaded?.Invoke();
        }
        else
        {
            try
            {
                TimeApiResponse response = JsonUtility.FromJson<TimeApiResponse>(request.downloadHandler.text);
                DateTime serverTime = DateTime.Parse(response.dateTime);
                _currentServerTime = serverTime;
                _timeDataLoaded = true;
                UpdateLevelUnlockTexts();
                OnTimeDataLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing time response: {ex.Message}");
                _currentServerTime = DateTime.Now;
                _timeDataLoaded = true;
                UpdateLevelUnlockTexts();
                OnTimeDataLoaded?.Invoke();
            }
        }
    }
    
    public bool IsLevelUnlocked(int levelIndex)
    {
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
                    levelUnlockTimeText[i].text = $"{levelUnlockDates[i].levelName}: ODBLOKOWANY";
                }
                else
                {
                    TimeSpan timeLeft = GetTimeUntilUnlock(i);
                    levelUnlockTimeText[i].text = $"{levelUnlockDates[i].levelName}: {timeLeft.Days}d {timeLeft.Hours}h {timeLeft.Minutes}m {timeLeft.Seconds}s";
                }
            }
        }
    }
}
