using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

[System.Serializable]
public class PlayerScore
{
    public string email;
    public string name;
    public float score;
    public string date;

    public PlayerScore(string email, string name, float score)
    {
        this.email = email;
        this.name = name;
        this.score = score;
        this.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI currentPlayerText;
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject leaderboardPanel;

    private string currentPlayerEmail = "";
    private string currentPlayerName = "";

    private const string CURRENT_PLAYER_KEY = "CurrentPlayer";
    private const string PLAYER_NAME_KEY = "PlayerName";
    private const string LEADERBOARD_KEY = "Leaderboard";

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
            UpdateLeaderboard(currentPlayerEmail, currentPlayerName, newScore);
            UpdateHighScoreDisplay();
            
            // Pokaż komunikat o nowym rekordzie
            Debug.Log($"Nowy rekord! {newScore:F0} punktów!");
        }
    }

    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            UpdateLeaderboardDisplay();
        }
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    private void UpdateLeaderboard(string playerEmail, string playerName, float score)
    {
        List<PlayerScore> leaderboard = GetLeaderboard();
        
        // Usuń poprzedni wynik tego gracza
        leaderboard.RemoveAll(p => p.email == playerEmail);
        
        // Dodaj nowy wynik
        leaderboard.Add(new PlayerScore(playerEmail, playerName, score));
        
        // Sortuj malejąco i pozostaw tylko top 10
        leaderboard = leaderboard.OrderByDescending(p => p.score).Take(10).ToList();
        
        SaveLeaderboard(leaderboard);
    }

    private List<PlayerScore> GetLeaderboard()
    {
        string json = PlayerPrefs.GetString(LEADERBOARD_KEY, "");
        if (string.IsNullOrEmpty(json))
        {
            return new List<PlayerScore>();
        }

        try
        {
            PlayerScoreList list = JsonUtility.FromJson<PlayerScoreList>(json);
            return list.scores ?? new List<PlayerScore>();
        }
        catch
        {
            return new List<PlayerScore>();
        }
    }

    private void SaveLeaderboard(List<PlayerScore> leaderboard)
    {
        PlayerScoreList list = new PlayerScoreList { scores = leaderboard };
        string json = JsonUtility.ToJson(list);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
    }

    private void UpdateLeaderboardDisplay()
    {
        if (leaderboardText == null) return;

        List<PlayerScore> leaderboard = GetLeaderboard();
        string displayText = "🏆 RANKING GRACZY 🏆\n\n";

        if (leaderboard.Count == 0)
        {
            displayText += "Brak wyników";
        }
        else
        {
            for (int i = 0; i < leaderboard.Count; i++)
            {
                PlayerScore player = leaderboard[i];
                string medal = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"{i + 1}.";
                displayText += $"{medal} {player.name}: {player.score:F0} pkt\n";
            }
        }

        leaderboardText.text = displayText;
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
        int ranking = GetPlayerRanking(currentPlayerEmail);
        
        if (highScoreText != null)
        {
            string rankText = ranking > 0 ? $" (#{ranking})" : "";
            highScoreText.text = $"Najlepszy wynik: {highScore:F0}{rankText}";
        }
        
        if (currentPlayerText != null)
            currentPlayerText.text = $"Gracz: {currentPlayerName}";
    }

    private int GetPlayerRanking(string playerEmail)
    {
        List<PlayerScore> leaderboard = GetLeaderboard();
        for (int i = 0; i < leaderboard.Count; i++)
        {
            if (leaderboard[i].email == playerEmail)
            {
                return i + 1;
            }
        }
        return 0;
    }

    public void ClearAllData()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // W przeglądarce wyczyść tylko dane gry, nie wylogowuj
            PlayerPrefs.DeleteKey(LEADERBOARD_KEY);
            
            // Wyczyść wszystkie wyniki graczy
            List<PlayerScore> leaderboard = GetLeaderboard();
            foreach (var player in leaderboard)
            {
                PlayerPrefs.DeleteKey($"HighScore_{player.email}");
                PlayerPrefs.DeleteKey($"Name_{player.email}");
            }
            
            PlayerPrefs.Save();
            UpdateHighScoreDisplay();
            Debug.Log("Wszystkie wyniki zostały wyczyszczone!");
        }
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
}

[System.Serializable]
public class PlayerScoreList
{
    public List<PlayerScore> scores;
}
