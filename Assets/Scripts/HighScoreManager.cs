using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using Dan.Main;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;
    private string publicLeaderboardKey = "88e3d223505ea86807694065498f0b36ec49e2f3ea09970d31d77d5af4d5807b";

    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject saveScorePanel;
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
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, leaderboard =>
        {
            if (leaderboard.Length > 0) highScoreText.text = leaderboard[0].Extra + ": " + leaderboard[0].Score.ToString();
        });
    }

    public void NewLeaderboardEntry(string playerName, string playerEmail, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, playerEmail, score, playerName);
    }

    public void SaveNewScore()
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
        
        if(string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Nazwa gracza nie może być pusta!");
            return;
        }

        currentPlayerEmail = email;
        currentPlayerName = name;

        NewLeaderboardEntry(currentPlayerName, currentPlayerEmail, Score.Instance.GetCurrentScore());
        Score.Instance.RestartGame();
    }

    public void ShowSaveHighscorePanel()
    {
        if (saveScorePanel != null) saveScorePanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        Character.CanMove = !saveScorePanel.activeInHierarchy;
    }

    private void ShowGameUI()
    {
        if (saveScorePanel != null) saveScorePanel.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        Character.CanMove = !saveScorePanel.activeInHierarchy;
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
