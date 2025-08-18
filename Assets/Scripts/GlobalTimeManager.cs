using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class TimeApiResponse
{
    public string dateTime;   // np. "2025-08-17T22:45:12.1234567"
    public string timeZone;   // np. "Europe/Warsaw"
    public string date;       // "2025-08-17"
    public string time;       // "22:45"
    public string dayOfWeek;  // "Sunday"
}

public class GlobalTimeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    
    private const string TimeServerUrl = "https://www.timeapi.io/api/time/current/zone?timeZone=Europe%2FWarsaw";
    private DateTime _targetDate = new DateTime(2025, 8, 18, 16, 0, 0); // Data do porównania
    private DateTime _currentServerTime; // Przechowuje aktualny czas z serwera
    
    private void Start()
    {
        StartCoroutine(GetTimeFromServer());
    }
    
    private IEnumerator GetTimeFromServer()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(TimeServerUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching time: {request.error}, getting time from local system.");
                _currentServerTime = DateTime.Now;
                if (IsServerTimeTarget(_targetDate))
                {
                    timeText.text = $"Czas lokalny ({_currentServerTime}) jest po dacie docelowej ({_targetDate})";
                }
                else
                {
                    timeText.text = $"Czas lokalny ({_currentServerTime}) jest przed datą docelową ({_targetDate})";
                }
            }
            else
            {
                try
                {
                    TimeApiResponse response = JsonUtility.FromJson<TimeApiResponse>(request.downloadHandler.text);
                    DateTime serverTime = DateTime.Parse(response.dateTime);
                    _currentServerTime = serverTime;
                    
                    if (IsServerTimeTarget(_targetDate))
                    {
                        timeText.text = $"Czas serwera ({serverTime}) jest po dacie docelowej ({_targetDate})";
                    }
                    else
                    {
                        timeText.text = $"Czas serwera ({serverTime}) jest przed datą docelową ({_targetDate})";
                    }
                    
                    //timeText.text = $"Current Time: {serverTime:HH:mm:ss}\n" +
                    //                $"Date: {response.date}\n";
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing time response: {ex.Message}, using local system time.");
                    _currentServerTime = DateTime.Now;
                    if (IsServerTimeTarget(_targetDate))
                    {
                        timeText.text = $"Czas lokalny ({_currentServerTime}) jest po dacie docelowej ({_targetDate})";
                    }
                    else
                    {
                        timeText.text = $"Czas lokalny ({_currentServerTime}) jest przed datą docelową ({_targetDate})";
                    }
                }
            }
        }
    }

    public bool IsServerTimeTarget()
    {
        return _currentServerTime >= _targetDate;
    }
    
    public bool IsServerTimeTarget(DateTime comparisonDate)
    {
        return _currentServerTime >= comparisonDate;
    }
    
    public void SetTargetDate(DateTime newTargetDate)
    {
        _targetDate = newTargetDate;
    }
}
