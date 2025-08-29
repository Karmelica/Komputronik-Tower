using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelTrigger : MonoBehaviour
{
    private BoxCollider2D _boxCollider;
    [SerializeField] private Image image;
    [SerializeField] private int levelIndex;
    [SerializeField] private GlobalTimeManager globalTimeManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LoadLevel();
        }
    }

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        if (globalTimeManager == null)
        {
            Debug.LogError("GlobalTimeManager is not assigned in LevelButton.");
            return;
        }
        
        if (globalTimeManager.IsTimeDataLoaded)
        {
            UpdateTriggerState();
        }
        else
        {
            GlobalTimeManager.OnTimeDataLoaded += UpdateTriggerState;
        }
    }
    
    private void OnDestroy()
    {
        GlobalTimeManager.OnTimeDataLoaded -= UpdateTriggerState;
    }
    
    private void UpdateTriggerState()
    {
        if (!globalTimeManager || !globalTimeManager.IsTimeDataLoaded) return;
        if (PlayerPrefs.GetInt("LevelsCompleted", 0) > levelIndex)
        {
            image.color = Color.green;
            _boxCollider.isTrigger = globalTimeManager.IsLevelUnlocked(levelIndex);
        }
        else
        {
            if (globalTimeManager.IsLevelUnlocked(levelIndex))
            {
                image.color = Color.gray;
                _boxCollider.isTrigger = globalTimeManager.IsLevelUnlocked(levelIndex);
            }
            else
                image.color = Color.darkRed;
        }
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(levelIndex + 1);
    }
}
