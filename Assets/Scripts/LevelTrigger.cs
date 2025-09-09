using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelTrigger : MonoBehaviour
{
    private BoxCollider2D _boxCollider;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject outlet;
    private Animator _panelAnimator;
    [SerializeField] private int levelIndex;
    [SerializeField] private GlobalTimeManager globalTimeManager;

    [SerializeField] private GameObject loadingScreen;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterMovement.CanMove = false;
            loadingScreen.SetActive(true);
            Invoke("LoadLevel", 2f);
        }
    }

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _panelAnimator = panel.GetComponent<Animator>();
        
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

        if (levelIndex != 5){
            if (!globalTimeManager || !globalTimeManager.IsTimeDataLoaded ||
                !globalTimeManager.IsLevelUnlocked(levelIndex)) return;
            panel.gameObject.SetActive(true);
            outlet.gameObject.SetActive(true);
            _boxCollider.isTrigger = globalTimeManager.IsLevelUnlocked(levelIndex);
            if (PlayerPrefs.GetInt("LevelsCompleted", 0) > levelIndex)
                _panelAnimator.SetBool("LevelCompleted", true);
        }
        else
        {
            panel.gameObject.SetActive(true);
            outlet.gameObject.SetActive(true);
            _panelAnimator.SetBool("LevelCompleted", true);
        }
        
    }
    
    private void LoadLevel()
    {
        SceneManager.LoadScene(levelIndex + 1);
    }
}
