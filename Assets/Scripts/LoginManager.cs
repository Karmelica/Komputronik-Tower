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

    private const string API_KEY = "AIzaSyDyi7jzBfePmYyPj_rSsf7rIMADP-3fUb4";
    private const string FIREBASE_FUNCTION_URL = "https://addemail-zblptdvtpq-lm.a.run.app";

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
        
        // Inicjalizuj statyczny menedżer uwierzytelniania
        FirebaseAuthManager.Initialize(this);
    }

    private void Start()
    {
        Time.timeScale = 0f;
        PrefsCheck();
        
        // Preemptywne uwierzytelnianie przy starcie
        FirebaseAuthManager.PreAuthenticate(success =>
        {
            
        });
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

    private void ShowDebugMessage(string message)
    {
        if(_debugCoroutine != null) StopCoroutine(_debugCoroutine);
        _debugCoroutine = StartCoroutine(ShowDebugInfoCoroutine(message));
    }

    private IEnumerator ShowDebugInfoCoroutine(string message)
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
            ShowDebugMessage("Email nie może być pusty!");
            return;
        }

        if (!IsEmailValid(playerEmail))
        {
            ShowDebugMessage("Nieprawidłowy format email!");
            return;
        }
        
        if(string.IsNullOrEmpty(playerName))
        {
            ShowDebugMessage("Nazwa gracza nie może być pusta!");
            return;
        }
        
        CheckEmailAndNameAvailability(playerEmail, playerName, (emailExists, nameExists) =>
        {
            if (emailExists)
            {
                ShowDebugMessage("Email jest już zajęty!");
                return;
            }
            if (nameExists)
            {
                ShowDebugMessage("Nazwa jest już zajęta!");
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
        // Użyj nowego FirebaseAuthManager
        string idToken = null;
        FirebaseAuthManager.GetValidToken(token => idToken = token);
        
        // Poczekaj na token
        yield return new WaitUntil(() => idToken != null || !FirebaseAuthManager.IsAuthenticated());

        if (string.IsNullOrEmpty(idToken))
        {
            ShowDebugMessage("Nie udało się uwierzytelnić!");
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
        // Użyj nowego FirebaseAuthManager
        string idToken = null;
        FirebaseAuthManager.GetValidToken(token => idToken = token);
        
        // Poczekaj na token
        yield return new WaitUntil(() => idToken != null || !FirebaseAuthManager.IsAuthenticated());

        if (string.IsNullOrEmpty(idToken))
        {
            ShowDebugMessage("Nie udało się uwierzytelnić!");
            yield break;
        }

        PlayerEmailData data = new PlayerEmailData {playerID = playerId, email = email, name = name };
        string json = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest(FIREBASE_FUNCTION_URL, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + idToken);
            www.timeout = 10; // Dodaj timeout

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
    }
    #endregion
}
