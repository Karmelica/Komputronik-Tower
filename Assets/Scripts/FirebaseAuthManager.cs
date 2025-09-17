using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Statyczny menedżer uwierzytelniania Firebase, który działa niezależnie od scen
/// i persystentnie przechowuje tokeny w PlayerPrefs
/// </summary>
public static class FirebaseAuthManager
{
    private const string API_KEY = "AIzaSyDyi7jzBfePmYyPj_rSsf7rIMADP-3fUb4";
    private const string TOKEN_PREFS_KEY = "FirebaseToken";
    private const string TOKEN_EXPIRY_PREFS_KEY = "FirebaseTokenExpiry";
    
    private static string _cachedIdToken;
    private static DateTime _tokenExpiryTime;
    private static bool _isAuthenticating = false;
    private static MonoBehaviour _coroutineRunner;
    
    /// <summary>
    /// Inicjalizuje menedżer z podanym komponentem do uruchamiania coroutine
    /// </summary>
    public static void Initialize(MonoBehaviour coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
        LoadTokenFromPrefs();
    }
    
    /// <summary>
    /// Pobiera ważny token Firebase (z cache'u lub przez nowe uwierzytelnianie)
    /// </summary>
    public static void GetValidToken(System.Action<string> callback)
    {
        if (_coroutineRunner == null)
        {
            Debug.LogError("FirebaseAuthManager nie został zainicjalizowany! Wywołaj Initialize() z MonoBehaviour.");
            callback?.Invoke(null);
            return;
        }
        
        _coroutineRunner.StartCoroutine(GetValidTokenCoroutine(callback));
    }
    
    /// <summary>
    /// Sprawdza czy użytkownik jest uwierzytelniony
    /// </summary>
    public static bool IsAuthenticated()
    {
        LoadTokenFromPrefs(); // Upewnij się, że mamy najnowsze dane
        return !string.IsNullOrEmpty(_cachedIdToken) && DateTime.UtcNow < _tokenExpiryTime;
    }
    
    /// <summary>
    /// Wymusza odświeżenie tokenu
    /// </summary>
    public static void RefreshToken()
    {
        _cachedIdToken = null;
        _tokenExpiryTime = DateTime.MinValue;
        ClearTokenFromPrefs();
    }
    
    /// <summary>
    /// Preemptywne uwierzytelnianie
    /// </summary>
    public static void PreAuthenticate(System.Action<bool> callback = null)
    {
        if (_coroutineRunner == null)
        {
            Debug.LogError("FirebaseAuthManager nie został zainicjalizowany!");
            callback?.Invoke(false);
            return;
        }
        
        _coroutineRunner.StartCoroutine(PreAuthenticateCoroutine(callback));
    }
    
    private static IEnumerator GetValidTokenCoroutine(System.Action<string> callback)
    {
        // Sprawdź cache i PlayerPrefs
        LoadTokenFromPrefs();
        
        if (!string.IsNullOrEmpty(_cachedIdToken) && DateTime.UtcNow < _tokenExpiryTime)
        {
            callback?.Invoke(_cachedIdToken);
            yield break;
        }
        
        // Jeśli już trwa uwierzytelnianie, poczekaj
        if (_isAuthenticating)
        {
            yield return new WaitUntil(() => !_isAuthenticating);
            
            LoadTokenFromPrefs();
            if (!string.IsNullOrEmpty(_cachedIdToken) && DateTime.UtcNow < _tokenExpiryTime)
            {
                callback?.Invoke(_cachedIdToken);
                yield break;
            }
        }
        
        _isAuthenticating = true;
        
        // Retry logic
        int maxRetries = 3;
        int retryCount = 0;
        
        while (retryCount < maxRetries)
        {
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={API_KEY}";
            
            using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, ""))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                www.timeout = 10;
                
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var json = www.downloadHandler.text;
                        var response = JsonUtility.FromJson<FirebaseAuthResponse>(json);
                        
                        if (!string.IsNullOrEmpty(response.idToken))
                        {
                            _cachedIdToken = response.idToken;
                            _tokenExpiryTime = DateTime.UtcNow.AddMinutes(50);
                            
                            SaveTokenToPrefs();
                            
                            _isAuthenticating = false;
                            callback?.Invoke(response.idToken);
                            yield break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Błąd parsowania odpowiedzi Firebase: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Próba {retryCount + 1}/{maxRetries} logowania anonimowego nieudana: {www.error}");
                    
                    if (www.result == UnityWebRequest.Result.ConnectionError || 
                        www.result == UnityWebRequest.Result.DataProcessingError)
                    {
                        retryCount++;
                        if (retryCount < maxRetries)
                        {
                            yield return new WaitForSeconds(Mathf.Pow(2, retryCount));
                            continue;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            retryCount++;
        }
        
        _isAuthenticating = false;
        Debug.LogError("Nie udało się uzyskać tokenu Firebase po wszystkich próbach");
        callback?.Invoke(null);
    }
    
    private static IEnumerator PreAuthenticateCoroutine(System.Action<bool> callback)
    {
        string token = null;
        yield return GetValidTokenCoroutine(t => token = t);
        
        bool success = !string.IsNullOrEmpty(token);
        /*if (success)
        {
            Debug.Log("Preemptywne uwierzytelnianie Firebase zakończone sukcesem");
        }*/
        
        callback?.Invoke(success);
    }
    
    private static void LoadTokenFromPrefs()
    {
        if (string.IsNullOrEmpty(_cachedIdToken) || _tokenExpiryTime == DateTime.MinValue)
        {
            _cachedIdToken = PlayerPrefs.GetString(TOKEN_PREFS_KEY, "");
            
            string expiryString = PlayerPrefs.GetString(TOKEN_EXPIRY_PREFS_KEY, "");
            if (!string.IsNullOrEmpty(expiryString) && DateTime.TryParse(expiryString, out DateTime expiry))
            {
                _tokenExpiryTime = expiry;
            }
            else
            {
                _tokenExpiryTime = DateTime.MinValue;
            }
        }
    }
    
    private static void SaveTokenToPrefs()
    {
        PlayerPrefs.SetString(TOKEN_PREFS_KEY, _cachedIdToken ?? "");
        PlayerPrefs.SetString(TOKEN_EXPIRY_PREFS_KEY, _tokenExpiryTime.ToString());
        PlayerPrefs.Save();
    }
    
    private static void ClearTokenFromPrefs()
    {
        PlayerPrefs.DeleteKey(TOKEN_PREFS_KEY);
        PlayerPrefs.DeleteKey(TOKEN_EXPIRY_PREFS_KEY);
        PlayerPrefs.Save();
    }
}
