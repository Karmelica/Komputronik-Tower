using TMPro;
using UnityEngine;

public class LobbyScoreScript : MonoBehaviour
{
    private TMP_Text _scoreText;
    
    private void Start()
    {
        _scoreText = GetComponent<TMP_Text>();
        
        _scoreText.text = PlayerPrefs.GetInt("ArcadeScore").ToString();
    }
}
