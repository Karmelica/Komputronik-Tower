using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;
using UnityEngine.SceneManagement;

public class HighScoreManager : MonoBehaviour
{
    #region Variables
    
    public static HighScoreManager Instance;
    [SerializeField] int lvlIndex = 0; // Index of the level, used for leaderboard management
    private LoginManager loginManager;
    private const string PublicLeaderboardKey = "88e3d223505ea86807694065498f0b36ec49e2f3ea09970d31d77d5af4d5807b";
    
    [Header("Game UI")]
    [SerializeField] private GameObject gameUI;
    //[SerializeField] private List<TextMeshProUGUI> highScoreText;
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
            //DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        lvlIndex = SceneManager.GetActiveScene().buildIndex;
        loginManager = LoginManager.Instance;
    }
    
    private void Start()
    {
        Time.timeScale = 1f;
        //GetLeaderboard();
        UpdateScoreDisplay();
        ShowPanel(GameState.Playing);
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

    /*private void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(PublicLeaderboardKey, leaderboard =>
        {
            int loopLenght = leaderboard.Length < highScoreText.Count ? leaderboard.Length : highScoreText.Count;
            for (int i  = 0; i < loopLenght; i++)
            {
                highScoreText[i].text = leaderboard[i].Username + ": " + leaderboard[i].Score;
            }
        });
    }*/

    public void NewLeaderboardEntry(string playerName, string playerEmail, int score)
    {
        LeaderboardCreator.UploadNewEntry(PublicLeaderboardKey, playerName, score, playerEmail);
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
            scoreText.text = $"Wynik: {currentScore:F0} | {lvlIndex} poziom";
        }
    }
    
    #endregion
    
    #region UI Interaction

    private void ShowPanel(GameState gameState, bool show = true)
    {
        switch (gameState)
        {
            case GameState.Playing:
                ShowGameUI(show);
                break;
            case GameState.GameOver:
                ShowGameOverPanel(show);
                break;
            default:
                Debug.LogWarning("Nieznany stan gry!");
                break;
        }
    }
    
    private void ShowGameUI(bool show)
    {
        Debug.Log("4. Wyświetlanie panelu gry.");
        Character.CanMove = true;
        Time.timeScale = 1f;
        if (gameOverPanel) gameOverPanel.SetActive(!show);
        if (gameUI) gameUI.SetActive(show);
    }
    
    private void ShowGameOverPanel(bool show)
    {
        Debug.Log("5. Wyświetlanie panelu końca gry.");
        Character.CanMove = false;
        Time.timeScale = 0f;
        if (gameOverPanel) gameOverPanel.SetActive(show);
        if (gameUI) gameUI.SetActive(!show);
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
            gameOverScoreText.text = $"Twój wynik: {currentScore:F0} | {lvlIndex} poziom";
        }
        
        NewLeaderboardEntry(loginManager.currentPlayerName, loginManager.currentPlayerEmail, Mathf.RoundToInt(currentScore));
    }

    public void RestartGame()
    {
        currentScore = 0f;
        scoreMultiplier = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    #endregion
    
    
    public void DeletePlayerDataAndRestart()
    {
        loginManager.DeletePlayerPrefs();
        SceneManager.LoadScene(0);
    }
}

public enum GameState
{
    Playing,
    GameOver
}
