using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameObject gameOverPanel;
    
    private float currentScore = 0f;
    private float scoreMultiplier = 1f;
    
    public static Score Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        UpdateScoreDisplay();
    }
    
    private void Update()
    {
        // Zwiększaj wynik w czasie (punkty za przetrwanie)
        if (Character.CanMove)
        {
            AddScore(Time.deltaTime * 10f * scoreMultiplier);
        }
    }
    
    public void AddScore(float points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }
    
    public void SetScoreMultiplier(float multiplier)
    {
        scoreMultiplier = multiplier;
    }
    
    public float GetCurrentScore()
    {
        return currentScore;
    }
    
    public void GameOver()
    {
        // Zatrzymaj dodawanie punktów
        Character.CanMove = false;
        
        // Zapisz wynik do HighScoreManager
        if (HighScoreManager.Instance != null)
        {
            HighScoreManager.Instance.UpdateScore(currentScore);
        }
        
        // Pokaż panel końca gry
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        // Zaktualizuj wyświetlany wynik końcowy
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = $"Twój wynik: {currentScore:F0}";
        }
    }
    
    public void RestartGame()
    {
        currentScore = 0f;
        scoreMultiplier = 1f;
        UpdateScoreDisplay();
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        Character.CanMove = true;
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Wynik: {currentScore:F0}";
        }
    }
}
