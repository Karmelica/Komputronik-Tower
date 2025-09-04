using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class FirebaseAuthResponse
{
    public string idToken;
}

[System.Serializable]
public class PlayerEmailData
{
    public string playerID;
    public string email;
    public string name;
    public int score1;
    public int score2;
    public int score3;
    public int score4;
    public int score5;
    public int score6;
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
    
    #region Player Data Management
    
    public void SetPlayerData()
    {
        string playerEmail = emailInputField.text.Trim();
        string playerName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(playerEmail))
        {
            Debug.LogWarning("Email nie może być pusty!");
            return;
        }

        if (!IsEmailValid(playerEmail))
        {
            Debug.LogWarning("Nieprawidłowy format email!");
            return;
        }
        
        if(string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Nazwa gracza nie może być pusta!");
            return;
        }

        currentPlayerEmail = playerEmail;
        currentPlayerName = playerName;
        
        SavePlayerPrefs();
        SendPlayerData(currentPlayerEmail, currentPlayerName);
        Time.timeScale = 1f;
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
    
    public void SendPlayerData(string email, string playerName)
    {
        StartCoroutine(SendDataCoroutine(email, playerName));
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
