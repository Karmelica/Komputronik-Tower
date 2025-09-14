using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class FirebaseAuthResponse
{
    public string idToken;
}

[Serializable]
public class PlayerEmailData
{
    public string playerID;
    public string email;
    public string name;
    public float score1;
    public float score2;
    public float score3;
    public float score4;
    public float score5;
    public float score6;
}

[Serializable]
public class CheckResponse
{
    public bool emailExists;
    public bool nameExists;
}

[SerializeField]
public class CheckRequest
{
    public string email;
    public string name;
    public string playerID;
}

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;
    private string playerId;

    private string apiKey = "AIzaSyDyi7jzBfePmYyPj_rSsf7rIMADP-3fUb4";
    private string firebaseFunctionUrl = "https://addemail-zblptdvtpq-lm.a.run.app";

    [SerializeField] private GameObject player;
    
    [Header("Login Panel")] 
    [SerializeField] private GameObject saveScorePanel;
    [SerializeField] private GameObject emailConfirmationPanel;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_Text debugText;
    private Coroutine _debugCoroutine;

    [HideInInspector] public string currentPlayerEmail = "";
    [HideInInspector] public string currentPlayerName = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        
        playerId = GetOrCreatePlayerId();
    }

    private void Start()
    {
        Time.timeScale = 0f;
        PrefsCheck();
    }

    #region PlayerPrefs

    public void PrefsCheck()
    {
        if (PlayerPrefs.GetInt("EmailConfirmed", 0) == 0)
        {
            ShowEmailConfirmationPanel();
        }
        else if(PlayerPrefs.HasKey("PlayerEmail"))
        {
            LoadPlayerPrefs();
            ShowSavePlayerPanel(false);
            Time.timeScale = 1f;
        }
        else
        {
            ShowSavePlayerPanel();
        }
    }


    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetString("PlayerEmail", currentPlayerEmail);
        PlayerPrefs.SetString("PlayerName", currentPlayerName);
        PlayerPrefs.Save();
    }
    
    public void LoadPlayerPrefs()
    {
        currentPlayerEmail = PlayerPrefs.GetString("PlayerEmail");
        currentPlayerName = PlayerPrefs.GetString("PlayerName");
    }
    
    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteKey("PlayerEmail");
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.Save();
    }
    
    private string GetOrCreatePlayerId()
    {
        string newPlayer = PlayerPrefs.GetString("PlayerId", "");
        
        if (string.IsNullOrEmpty(newPlayer))
        {
            // Generuj nowe ID używając System.Guid
            newPlayer = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("PlayerId", newPlayer);
            PlayerPrefs.Save();
        }
        
        return newPlayer;
    }

    #endregion

    #region UI

    public void ShowSavePlayerPanel(bool show = true)
    {
        if (saveScorePanel) saveScorePanel.SetActive(show);
        if (player) player.SetActive(!show);
    }
    
    public void ShowEmailConfirmationPanel(bool show = true)
    {
        if(emailConfirmationPanel) emailConfirmationPanel.SetActive(show);
        ShowSavePlayerPanel();
        if(show == false)
        {
            PlayerPrefs.SetInt("EmailConfirmed", 1);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator ShowDebugInfo(string message)
    {
        if (!debugText) yield break;
        debugText.gameObject.SetActive(true);
        debugText.text = message;
        yield return new WaitForSeconds(3f);
        debugText.gameObject.SetActive(false);
    }

    #endregion
    
    #region Player Data Management
    
    public void SetPlayerData()
    {
        string playerEmail = emailInputField.text.Trim();
        string playerName = nameInputField.text.Trim();
        
        if (string.IsNullOrEmpty(playerEmail))
        {
            StopCoroutine(_debugCoroutine);
            _debugCoroutine = StartCoroutine(ShowDebugInfo("Email nie może być pusty!"));
            return;
        }

        if (!IsEmailValid(playerEmail))
        {
            StopCoroutine(_debugCoroutine);
            _debugCoroutine = StartCoroutine(ShowDebugInfo("Nieprawidłowy format email!"));
            return;
        }
        
        if(string.IsNullOrEmpty(playerName))
        {
            StopCoroutine(_debugCoroutine);
            _debugCoroutine = StartCoroutine(ShowDebugInfo("Nazwa gracza nie może być pusta!"));
            return;
        }
        
        CheckEmailAndNameAvailability(playerEmail, playerName, (emailExists, nameExists) =>
        {
            if (emailExists)
            {
                StopCoroutine(_debugCoroutine);
                _debugCoroutine = StartCoroutine(ShowDebugInfo("Email jest już zajęty!"));
                return;
            }
            if (nameExists)
            {
                StopCoroutine(_debugCoroutine);
                _debugCoroutine = StartCoroutine(ShowDebugInfo("Nazwa jest już zajęta!"));
                return;
            }
            if (!emailExists && !nameExists)
            {
                currentPlayerEmail = playerEmail;
                currentPlayerName = playerName;
                
                SavePlayerPrefs();
                SendPlayerData(currentPlayerEmail, currentPlayerName);
            }
        });
    }
    
    private static bool IsEmailValid(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    public void CheckEmailAndNameAvailability(string email, string playerName, Action<bool, bool> callback)
    {
        StartCoroutine(CheckEmailAndNameCoroutine(email, playerName, callback));
    }
    
    public void SendPlayerData(string email, string playerName)
    {
        StartCoroutine(SendDataCoroutine(email, playerName));
    }
    
    private IEnumerator CheckEmailAndNameCoroutine(string playerEmail, string playerName, System.Action<bool, bool> callback)
    {
        // Pobierz token Firebase
        string idToken = null;
        yield return StartCoroutine(GetFirebaseAnonymousToken(token => idToken = token));

        if (string.IsNullOrEmpty(idToken))
        {
            callback?.Invoke(false, false);
            yield break;
        }

        // Przygotuj dane do sprawdzenia
        var checkData = new CheckRequest { email = playerEmail, name = playerName, playerID = playerId };
        string json = JsonUtility.ToJson(checkData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        // Wywołaj funkcję check w Firebase
        string checkUrl = "https://check-zblptdvtpq-lm.a.run.app";
        UnityWebRequest www = new UnityWebRequest(checkUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + idToken);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            CheckResponse response = JsonUtility.FromJson<CheckResponse>(www.downloadHandler.text);
            callback?.Invoke(response.emailExists, response.nameExists);
        }
        else
        {
            Debug.LogError("Błąd sprawdzania dostępności: " + www.error);
            callback?.Invoke(false, false);
        }
    }
    
    private IEnumerator SendDataCoroutine(string email, string name)
    {
        // 1. Logowanie anonimowe i pobranie ID Token
        string idToken = null;
        yield return StartCoroutine(GetFirebaseAnonymousToken(token => idToken = token));

        if (string.IsNullOrEmpty(idToken))
        {
            yield break;
        }

        PlayerEmailData data = new PlayerEmailData {playerID = playerId, email = email, name = name };
        string json = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest www = new UnityWebRequest(firebaseFunctionUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + idToken);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            ShowSavePlayerPanel(false);
            Time.timeScale = 1f;
        }
        else
        {
            Debug.LogError("Błąd wysyłki: " + www.error + " / " + www.downloadHandler.text);
        }
    }
    
    private IEnumerator GetFirebaseAnonymousToken(System.Action<string> callback)
    {
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";

        UnityWebRequest www = UnityWebRequest.PostWwwForm(url, ""); // POST z pustym body
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var json = www.downloadHandler.text;
            var response = JsonUtility.FromJson<FirebaseAuthResponse>(json);
            callback?.Invoke(response.idToken);
        }
        else
        {
            Debug.LogError("Błąd logowania anonimowego: " + www.error + " / " + www.downloadHandler.text);
            callback?.Invoke(null);
        }
    }

    #endregion
}
