using System;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private float score = 0;
    private float multiplier = 1f;
    private Rigidbody2D _playerRb2D;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Character player;
    
    private float highestYPosition = 0f;

    private void Start()
    {
        if(!player.TryGetComponent<Rigidbody2D>(out _playerRb2D))
        {
            Debug.LogError("No Rigidbody2D component found on the player.", this);
        }
        ResetScore();
    }

    private void Update()
    {
        if (player.transform.position.y > highestYPosition)
        {
            multiplier = _playerRb2D.linearVelocity.magnitude; // Example multiplier based on velocity, adjust as needed
            multiplier = Mathf.Clamp(multiplier, 1f, 20f);
            score += (player.transform.position.y - highestYPosition) * multiplier; // Adjust the multiplier as needed
            highestYPosition = player.transform.position.y;
            
            // Aktualizuj wynik w HighScoreManager
            if (HighScoreManager.Instance != null)
            {
                HighScoreManager.Instance.UpdateScore(score);
            }
        }
        scoreText.text = score.ToString("F0"); // Display score as an integer
    }
    
    // Metoda do resetowania wyniku (np. przy rozpoczęciu nowej gry)
    public void ResetScore()
    {
        score = 0;
        highestYPosition = player.transform.position.y;
    }
}
