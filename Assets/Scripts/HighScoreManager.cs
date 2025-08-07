using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI currentPlayerText;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject gameUI;

    private string currentPlayerEmail = "";
    private string currentPlayerName = "";

    private const string CURRENT_PLAYER_KEY = "CurrentPlayer";
    private const string PLAYER_NAME_KEY = "PlayerName";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCurrentPlayer();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(currentPlayerEmail))
        {
            ShowLoginPanel();
        }
        else
        {
            ShowGameUI();
            UpdateHighScoreDisplay();
        }
    }

    public void LoginPlayer()
    {
        string email = emailInputField.text.Trim();
        string name = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            Debug.LogWarning("Email nie może być pusty!");
            return;
        }

        if (!IsEmailValid(email))
        {
            Debug.LogWarning("Nieprawidłowy format email!");
            return;
        }

        currentPlayerEmail = email;
        currentPlayerName = string.IsNullOrEmpty(name) ? email : name;

        SaveCurrentPlayer();
        ShowGameUI();
        UpdateHighScoreDisplay();
    }

    public void LogoutPlayer()
    {
        currentPlayerEmail = "";
        currentPlayerName = "";
        PlayerPrefs.DeleteKey(CURRENT_PLAYER_KEY);
        PlayerPrefs.DeleteKey(PLAYER_NAME_KEY);
        PlayerPrefs.Save();
        ShowLoginPanel();
    }

    public void UpdateScore(float newScore)
    {
        if (string.IsNullOrEmpty(currentPlayerEmail))
            return;

        float currentHighScore = GetPlayerHighScore(currentPlayerEmail);
        
        if (newScore > currentHighScore)
        {
            SavePlayerHighScore(currentPlayerEmail, newScore, currentPlayerName);
            UpdateHighScoreDisplay();
        }
    }

    public float GetPlayerHighScore(string playerEmail)
    {
        return PlayerPrefs.GetFloat($"HighScore_{playerEmail}", 0f);
    }

    public string GetPlayerName(string playerEmail)
    {
        return PlayerPrefs.GetString($"Name_{playerEmail}", playerEmail);
    }

    private void SavePlayerHighScore(string playerEmail, float score, string playerName)
    {
        PlayerPrefs.SetFloat($"HighScore_{playerEmail}", score);
        PlayerPrefs.SetString($"Name_{playerEmail}", playerName);
        PlayerPrefs.Save();
    }

    private void SaveCurrentPlayer()
    {
        PlayerPrefs.SetString(CURRENT_PLAYER_KEY, currentPlayerEmail);
        PlayerPrefs.SetString(PLAYER_NAME_KEY, currentPlayerName);
        PlayerPrefs.Save();
    }

    private void LoadCurrentPlayer()
    {
        currentPlayerEmail = PlayerPrefs.GetString(CURRENT_PLAYER_KEY, "");
        currentPlayerName = PlayerPrefs.GetString(PLAYER_NAME_KEY, "");
    }

    private void UpdateHighScoreDisplay()
    {
        if (string.IsNullOrEmpty(currentPlayerEmail))
            return;

        float highScore = GetPlayerHighScore(currentPlayerEmail);
        
        if (highScoreText != null)
            highScoreText.text = $"Najlepszy wynik: {highScore:F0}";
        
        if (currentPlayerText != null)
            currentPlayerText.text = $"Gracz: {currentPlayerName}";
    }

    private void ShowLoginPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        Character.CanMove = !loginPanel.activeInHierarchy;
    }

    private void ShowGameUI()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        Character.CanMove = !loginPanel.activeInHierarchy;
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
}
