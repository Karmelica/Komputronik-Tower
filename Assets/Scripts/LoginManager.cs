using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerEmailData
{
    public string email;
    public string name;
}

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    [Header("Login Panel")] 
    [SerializeField] private GameObject saveScorePanel;
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
    }

    private void Start()
    {
        PrefsCheck();
    }

    #region PlayerPrefs

    public void PrefsCheck()
    {
        if(PlayerPrefs.HasKey("PlayerEmail"))
        {
            Debug.Log("1. Znaleziono dane gracza w PlayerPrefs.");
            LoadPlayerPrefs();
            ShowSavePlayerPanel(false);
        }
        else
        {
            Debug.Log("2. Brak danych gracza w PlayerPrefs. Przechodzenie do ekranu logowania.");
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

    #endregion
    
    public void ShowSavePlayerPanel(bool show = true)
    {
        if (saveScorePanel) saveScorePanel.SetActive(show);
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
        ShowSavePlayerPanel(false);
        StartCoroutine(SendEmailToFirebase(currentPlayerEmail, currentPlayerName));
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
    
    private IEnumerator SendEmailToFirebase(string email, string name)
    {
        string url = "https://addemail-zblptdvtpq-lm.a.run.app";
    
        PlayerEmailData data = new PlayerEmailData { email = email, name = name };
        string json = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("x-api-key", "0ZautH87VJrVI49dcTAyXUVOT9dGlKOJ");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("Email wysłany do Firebase: " + www.downloadHandler.text);
            else
                Debug.LogError("Błąd wysyłki emaila: " + www.error);
        }
    }
    
    #endregion
}
