using System;using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;
using UnityEngine.SceneManagement;

public class HighScoreManager : MonoBehaviour
{
    #region Variables
    
    public static HighScoreManager Instance;
    private const string PublicLeaderboardKey = "88e3d223505ea86807694065498f0b36ec49e2f3ea09970d31d77d5af4d5807b";

    [Header("Login Panel")]
    [SerializeField] private GameObject saveScorePanel;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField nameInputField;

    private string currentPlayerEmail = "";
    private string currentPlayerName = "";
    
    [Header("Game UI")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private List<TextMeshProUGUI> highScoreText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private float currentScore = 0f;
    private float scoreMultiplier = 1f;

    [Header("Game Over Panel")]
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameObject gameOverPanel;
    
    #endregion
    
    #region Unity Methods
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) // Jeśli to scena główna
        {
            PrefsCheck();
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PrefsCheck();
    }
    
    private void Update()
    
    {
        // Zwiększaj wynik w czasie (punkty za przetrwanie)
        if (Character.CanMove)
        {
            AddScore(Time.deltaTime * 10f * scoreMultiplier);
        }
    }
    
    #endregion

    #region Leaderboard Management

    private void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(PublicLeaderboardKey, leaderboard =>
        {
            int loopLenght = leaderboard.Length < highScoreText.Count ? leaderboard.Length : highScoreText.Count;
            for (int i  = 0; i < loopLenght; i++)
            {
                highScoreText[i].text = leaderboard[i].Username + ": " + leaderboard[i].Score;
            }
        });
    }

    public void NewLeaderboardEntry(string playerName, string playerEmail, int score)
    {
        LeaderboardCreator.UploadNewEntry(PublicLeaderboardKey, playerName, score, playerEmail);
    }
    
    #endregion
    
    #region PlayerPrefs

    private void PrefsCheck()
    {
        if(PlayerPrefs.HasKey("PlayerEmail"))
        {
            LoadPlayerPrefs();
            //GetLeaderboard();
            ShowPanel(GameState.Playing);
        }
        else
        {
            ShowPanel(GameState.Login);
        }
        
        UpdateScoreDisplay();
    }
    
    private void SavePlayerPrefs()
    {
        PlayerPrefs.SetString("PlayerEmail", currentPlayerEmail);
        PlayerPrefs.SetString("PlayerName", currentPlayerName);
        PlayerPrefs.Save();
    }
    
    private void LoadPlayerPrefs()
    {
        currentPlayerEmail = PlayerPrefs.GetString("PlayerEmail");
        currentPlayerName = PlayerPrefs.GetString("PlayerName");
    }
    
    private void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteKey("PlayerEmail");
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.Save();
    }

    #endregion
    
    #region Score Management

    private void AddScore(float points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText)
        {
            scoreText.text = $"Wynik: {currentScore:F0}";
        }
    }
    
    #endregion
    
    #region UI Interaction

    private void ShowPanel(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Login:
                ShowSavePlayerPanel();
                break;
            case GameState.Playing:
                ShowGameUI();
                break;
            case GameState.GameOver:
                ShowGameOverPanel();
                break;
            default:
                Debug.LogWarning("Nieznany stan gry!");
                break;
        }
    }
    
    private void ShowSavePlayerPanel()
    {
        Character.CanMove = false;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (saveScorePanel) saveScorePanel.SetActive(true);
        if (gameUI) gameUI.SetActive(false);
        Time.timeScale = 0f;
    }

    private void ShowGameUI()
    {
        Character.CanMove = true;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (saveScorePanel) saveScorePanel.SetActive(false);
        if (gameUI) gameUI.SetActive(true);
        Time.timeScale = 1f;
    }
    
    private void ShowGameOverPanel()
    {
        Character.CanMove = false;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (saveScorePanel) saveScorePanel.SetActive(false);
        if (gameUI) gameUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void DeletePlayerDataAndRestart()
    {
        DeletePlayerPrefs();
        RestartGame();
    }
    
    #endregion
    
    #region Game Over Management
    
    public void GameOver()
    {
        // Zatrzymaj dodawanie punktów
        Character.CanMove = false;
        
        // Pokaż panel końca gry
        ShowPanel(GameState.GameOver);
        
        // Zaktualizuj wyświetlany wynik końcowy
        if (gameOverScoreText)
        {
            gameOverScoreText.text = $"Twój wynik: {currentScore:F0}";
        }
        
        NewLeaderboardEntry(currentPlayerName, currentPlayerEmail, Mathf.RoundToInt(currentScore));
    }

    public void RestartGame()
    {
        currentScore = 0f;
        scoreMultiplier = 1f;
        SceneManager.LoadScene(0);
    }
    
    #endregion
    
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
        
        ShowPanel(GameState.Playing);
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
    
    #endregion
}

public enum GameState
{
    Login,
    Playing,
    GameOver
}
