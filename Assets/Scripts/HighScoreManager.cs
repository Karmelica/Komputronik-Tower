using System;
using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic; // percentyl
using System.Linq;
using UnityEngine.EventSystems; // percentyl

public class HighScoreManager : MonoBehaviour
{
    #region Variables

    public bool infiniteGeneration;
    public bool moveStarted;


    private InputSystemActions _inputActions;
    
    public static HighScoreManager Instance;
    [SerializeField] int lvlIndex; // Index of the level, used for leaderboard management
    private LoginManager loginManager;
    private string playerId;

    private const string FIREBASE_FUNCTION_URL = "https://addemail-zblptdvtpq-lm.a.run.app";
    
    [Header("Leaderboard Cache")]
    private List<float> _cachedScores = new List<float>();
    private int _cachedLevel = -1;
    private bool _scoresReady;
    
    [Header("Game UI")]
    [SerializeField] private GameObject gameUI;
    //[SerializeField] private List<TextMeshProUGUI> highScoreText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private float _currentScore;

    [Header("Game Over Panel")]
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameObject gameOverPanel;
    
    [Header("Pause menu panel")]
    [SerializeField] private GameObject pauseMenu;
    
    // --- Percentyl ---
    [Header("Percentyl wyniku")]
    [SerializeField] private TextMeshProUGUI percentileText; // przypisz w Inspector (panel GameOver)
    
    private SoundPlayer _soundPlayer;
    
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
        _inputActions = new InputSystemActions();
        lvlIndex = SceneManager.GetActiveScene().buildIndex;
        loginManager = LoginManager.Instance;
        //playerId = GetOrCreatePlayerId();
        _soundPlayer = GetComponent<SoundPlayer>();
        
        // Inicjalizuj FirebaseAuthManager jeśli LoginManager nie jest dostępny
        if (loginManager == null)
        {
            FirebaseAuthManager.Initialize(this);
        }
    }
    
    private void Start()
    {
        Time.timeScale = 1f;
        //GetLeaderboard();
        UpdateScoreDisplay();
        ShowPanel(GameState.Playing);
        if (percentileText) percentileText.text = ""; // wyczysc na starcie
        StartCoroutine(PreloadScores(lvlIndex));
    }
    
    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Player.Escape.performed += OnEscape;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        _inputActions.Player.Escape.performed -= OnEscape;
    }
    
    /*private void Update()
    {
        // Zwiększaj wynik w czasie (punkty za przetrwanie)
        if (CharacterMovement.CanMove && !segmentLimited && moveStarted)
        {
            AddScore(Time.deltaTime * scoreMultiplier);
        }
    }*/
    
    #endregion

    #region Leaderboard Management

    public void NewLeaderboardEntry(string email, string playerName, float score, int level)
    {
        StartCoroutine(SendDataCoroutine(email, playerName, score, level));
    }
    
    private IEnumerator SendDataCoroutine(string playerEmail, string playerName, float score, int level)
    {
        // Użyj nowego FirebaseAuthManager zamiast własnej implementacji
        string idToken = null;
        FirebaseAuthManager.GetValidToken(token => idToken = token);
        
        // Poczekaj na token
        yield return new WaitUntil(() => idToken != null || !FirebaseAuthManager.IsAuthenticated());

        if (string.IsNullOrEmpty(idToken))
        {
            Debug.LogError("Nie udało się pobrać ID Token z FirebaseAuthManager.");
            yield break;
        }

        // 2. Wysyłka danych do funkcji Firebase
        PlayerEmailData data = level switch
        {
            1 => new PlayerEmailData { playerID = playerEmail, email = playerEmail, name = playerName, score1 = score },
            2 => new PlayerEmailData { playerID = playerEmail, email = playerEmail, name = playerName, score2 = score },
            3 => new PlayerEmailData { playerID = playerEmail, email = playerEmail, name = playerName, score3 = score },
            4 => new PlayerEmailData { playerID = playerEmail, email = playerEmail, name = playerName, score4 = score },
            5 => new PlayerEmailData { playerID = playerEmail, email = playerEmail, name = playerName, score5 = score },
            6 => new PlayerEmailData { playerID = playerEmail, email = playerEmail, name = playerName, score6 = score },
            _ => new PlayerEmailData { playerID = playerEmail, email = playerEmail, name = playerName }
        };

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

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Błąd wysyłki: " + www.error + " / " + www.downloadHandler.text);
            }
            else
            {
                // po udanej wysyłce oblicz percentyl
                StartCoroutine(RefreshScoresAfterSubmit(level));
            }
        }
    }
    
    // ===== Percentyl =====
    [Serializable]
    private class ScoreListDto { public List<float> scores; }

   private IEnumerator PreloadScores(int level)
    {
        _scoresReady = false;
        _cachedLevel = level;
        string url = $"https://scores-zblptdvtpq-lm.a.run.app?level={level}";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var dto = JsonUtility.FromJson<ScoreListDto>(www.downloadHandler.text);
                    _cachedScores = dto?.scores ?? new List<float>();
                    _scoresReady = _cachedScores.Count > 0;
                }
                catch
                {
                    _cachedScores = new List<float>();
                    _scoresReady = false;
                }
            }
            else
            {
                _cachedScores = new List<float>();
                _scoresReady = false;
            }
        }
    }
   
    private void ComputeAndShowPercentileFromCache(int level, float scoreToCompare)
    {
        if (!percentileText) return;

        if (!_scoresReady || _cachedLevel != level || _cachedScores == null || _cachedScores.Count == 0)
        {
            percentileText.text = "brak wyników innych graczy";
            return;
        }

        bool lowerIsBetter = !infiniteGeneration;
        float percentile = CalculatePercentile(_cachedScores, scoreToCompare, lowerIsBetter);
        
        if (percentile == -1f)
        {
            percentileText.text = "Jesteś pierwszym graczem na tym poziomie!";
        }
        else
        {
            percentileText.text = $"Twój najlepszy wynik jest lepszy od {percentile:F1}% graczy";
        }
    }
    
    private IEnumerator RefreshScoresAfterSubmit(int level)
    {
        yield return new WaitForSecondsRealtime(1.0f); // czas na propagację
        yield return StartCoroutine(PreloadScores(level));

        float scoreForComparison = infiniteGeneration
            ? PlayerPrefs.GetInt("ArcadeScore", 0)
            : PlayerPrefs.GetFloat($"{lvlIndex}_HighestScore", Mathf.Infinity);

        ComputeAndShowPercentileFromCache(level, scoreForComparison);
    }

    private float CalculatePercentile(List<float> allScores, float playerScore, bool lowerIsBetter)
    {
        if (allScores == null || allScores.Count == 0) return 0f;
        
        // Jeśli gracz jest jedyny w rankingu
        if (allScores.Count == 1)
        {
            return -1f; // Specjalna wartość oznaczająca, że gracz jest jedyny
        }
        
        int worse = lowerIsBetter ? allScores.Count(s => s > playerScore) : allScores.Count(s => s < playerScore);
        return (float)worse / allScores.Count * 100f;
    }
    // ===== Koniec percentyla =====
    
    #endregion
    
    #region Score Management

    public void AddScore(float points)
    {
        if(!CharacterMovement.levelEnded && CharacterMovement.startCounting)
        {
            _currentScore += points;
            UpdateScoreDisplay();
        }
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText)
        {
            if (infiniteGeneration)
            {
                scoreText.text = $"Wynik: {_currentScore:F0}";
            }
            else
            {
                int minutes = Mathf.FloorToInt(_currentScore / 60f);
                int seconds = Mathf.FloorToInt(_currentScore % 60f);
                int miliseconds = Mathf.FloorToInt(_currentScore * 1000f % 1000f);
            
                scoreText.text = $"Czas: {minutes:D2}:{seconds:D2}:{miliseconds:D3}";
            }
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

    public void ShowPauseMenu(bool show)
    {
        Time.timeScale = show ? 0f : 1f;
        CharacterMovement.CanMove = !show;
        if (pauseMenu) pauseMenu.SetActive(show);
        if (gameUI) gameUI.SetActive(!show);
    }
    
    private void ShowGameUI(bool show)
    {
        //Debug.Log("4. Wyświetlanie panelu gry.");
        CharacterMovement.CanMove = true;
        Time.timeScale = 1f;
        if (gameOverPanel) gameOverPanel.SetActive(!show);
        if (gameUI) gameUI.SetActive(show);
    }
    
    public void ShowGameOverPanel(bool show)
    {
        //Debug.Log("5. Wyświetlanie panelu końca gry.");
        CharacterMovement.CanMove = false;
        Time.timeScale = 0f;
        if (gameOverPanel) gameOverPanel.SetActive(show);
        if (gameUI) gameUI.SetActive(!show);
    }
    
    #endregion
    
    #region Game Over Management
    
    public void GameOver(bool fallen)
    {
        // Zatrzymaj dodawanie punktów
        CharacterMovement.CanMove = false;

        if (fallen) { _soundPlayer.PlayRandom("Fall"); }
        
        // Pokaż panel końca gry
        ShowPanel(GameState.GameOver);

        if (SceneManager.GetActiveScene().buildIndex == 6)
        {
            // save aracde level highest score
            int bestScore = PlayerPrefs.GetInt("ArcadeScore", 0);

            if (_currentScore > bestScore)
            {
                PlayerPrefs.SetInt("ArcadeScore", (int)_currentScore);
                PlayerPrefs.Save();
            }
        }
        else {
            // save other level highest score
            float bestScore = PlayerPrefs.GetFloat($"{lvlIndex}_HighestScore", Mathf.Infinity);
            if (_currentScore < bestScore)
            {
                PlayerPrefs.SetFloat($"{lvlIndex}_HighestScore", _currentScore);
                PlayerPrefs.Save();
            }
        }
        
        // Zaktualizuj wyświetlany wynik końcowy
        if (gameOverScoreText)
        {
            if(infiniteGeneration)
            {
                if(PlayerPrefs.HasKey("ArcadeScore"))
                {
                    var bestArcadeScore = PlayerPrefs.GetInt("ArcadeScore", 0);
                    gameOverScoreText.text = $"Twój wynik: {_currentScore:F0}\nNajlepszy wynik: {bestArcadeScore}";
                }
                else
                {
                    gameOverScoreText.text = $"Twój wynik: {_currentScore:F0}";
                }
            }
            else
            {

                if (PlayerPrefs.HasKey($"{lvlIndex}_HighestScore"))
                {
                    float bestScore = PlayerPrefs.GetFloat($"{lvlIndex}_HighestScore", Mathf.Infinity);
                    int bestMinutes = Mathf.FloorToInt(bestScore / 60f);
                    int bestSeconds = Mathf.FloorToInt(bestScore % 60f);
                    int bestMiliseconds = Mathf.FloorToInt(bestScore * 1000f % 1000f);
                    
                    int minutes = Mathf.FloorToInt(_currentScore / 60f);
                    int seconds = Mathf.FloorToInt(_currentScore % 60f);
                    int miliseconds = Mathf.FloorToInt(_currentScore * 1000f % 1000f);
                    
                    gameOverScoreText.text = $"Czas: {minutes:D2}:{seconds:D2}:{miliseconds:D3}\nNajlepszy czas: {bestMinutes:D2}:{bestSeconds:D2}:{bestMiliseconds:D3}";
                }
                else {
                    int minutes = Mathf.FloorToInt(_currentScore / 60f);
                    int seconds = Mathf.FloorToInt(_currentScore % 60f);
                    int miliseconds = Mathf.FloorToInt(_currentScore * 1000f % 1000f);
                    gameOverScoreText.text = $"Czas: {minutes:D2}:{seconds:D2}:{miliseconds:D3}";
                }
            }
        }

        if (percentileText) percentileText.text = "Sprawdzanie wyników...";
        float scoreForComparison = infiniteGeneration
            ? PlayerPrefs.GetInt("ArcadeScore", 0)
            : PlayerPrefs.GetFloat($"{lvlIndex}_HighestScore", Mathf.Infinity);
        ComputeAndShowPercentileFromCache(lvlIndex, scoreForComparison);
        
        NewLeaderboardEntry(loginManager.currentPlayerEmail, loginManager.currentPlayerName, _currentScore, lvlIndex);
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        OnGameRestart?.Invoke();
        SceneManager.LoadScene(0);
    }
    
    public event Action OnGameRestart;

    public void RestartGame()
    {
        _currentScore = 0f;
        OnGameRestart?.Invoke();
        SceneManager.LoadScene(lvlIndex, LoadSceneMode.Single);
    }
    
    #endregion
    
    
    public void DeletePlayerDataAndRestart()
    {
        loginManager.DeletePlayerPrefs();
        SceneManager.LoadScene(0);
    }

    /*private string GetOrCreatePlayerId()
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
    }*/

    #region Input Actions

    private void OnEscape(InputAction.CallbackContext context)
    {
        //ShowPanel(gameOverPanel.activeInHierarchy ? GameState.Playing : GameState.GameOver);
        bool isActive = pauseMenu.activeInHierarchy;

        ShowPauseMenu(!isActive);
    }
    
    #endregion

}

public enum GameState
{
    Playing,
    GameOver
}