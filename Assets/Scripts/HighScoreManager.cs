using System;
using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HighScoreManager : MonoBehaviour
{
    #region Variables
    
    public static HighScoreManager Instance;
    [SerializeField] int lvlIndex = 0; // Index of the level, used for leaderboard management
    private LoginManager loginManager;
    private string playerId;
    
    private string apiKey = "AIzaSyDyi7jzBfePmYyPj_rSsf7rIMADP-3fUb4";
    private string firebaseFunctionUrl = "https://addemail-zblptdvtpq-lm.a.run.app";
    
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
        playerId = GetOrCreatePlayerId();
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

    public void NewLeaderboardEntry(string email, string playerName, int score, int level)
    {
        StartCoroutine(SendEmailCoroutine(email, playerName, score, level));
    }
    
    private IEnumerator SendEmailCoroutine(string playerEmail, string playerName, int score, int level)
    {
        // 1. Logowanie anonimowe i pobranie ID Token
        string idToken = null;
        yield return StartCoroutine(GetFirebaseAnonymousToken(token => idToken = token));

        if (string.IsNullOrEmpty(idToken))
        {
            Debug.LogError("Nie udało się pobrać ID Token.");
            yield break;
        }

        // 2. Wysyłka danych do funkcji Firebase
        PlayerEmailData data = level switch
        {
            1 => new PlayerEmailData { playerID = playerId, email = playerEmail, name = playerName, score1 = score },
            2 => new PlayerEmailData { playerID = playerId, email = playerEmail, name = playerName, score2 = score },
            3 => new PlayerEmailData { playerID = playerId, email = playerEmail, name = playerName, score3 = score },
            4 => new PlayerEmailData { playerID = playerId, email = playerEmail, name = playerName, score4 = score },
            5 => new PlayerEmailData { playerID = playerId, email = playerEmail, name = playerName, score5 = score },
            _ => new PlayerEmailData { playerID = playerId, email = playerEmail, name = playerName }
        };

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
            Debug.Log("Email wysłany: " + www.downloadHandler.text);
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
    }

    public void NewLeaderboardEntry(string playerName, string playerEmail, int score)
    {
        LeaderboardCreator.UploadNewEntry(PublicLeaderboardKey, playerName, score, playerEmail);
    }*/
    
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
        
        NewLeaderboardEntry(loginManager.currentPlayerEmail, loginManager.currentPlayerName, Mathf.RoundToInt(currentScore), lvlIndex);
    }

    public void RestartGame()
    {
        currentScore = 0f;
        scoreMultiplier = 1f;
        SceneManager.LoadScene(lvlIndex);
    }
    
    #endregion
    
    
    public void DeletePlayerDataAndRestart()
    {
        loginManager.DeletePlayerPrefs();
        SceneManager.LoadScene(0);
    }

    private string GetOrCreatePlayerId()
    {
        string player = PlayerPrefs.GetString("PlayerId", "");
        
        if (string.IsNullOrEmpty(player))
        {
            // Generuj nowe ID używając System.Guid
            player = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("PlayerId", player);
            PlayerPrefs.Save();
        }
        
        return player;
    }
}

public enum GameState
{
    Playing,
    GameOver
}
