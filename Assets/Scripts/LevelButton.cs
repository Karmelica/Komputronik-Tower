using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    private Button _button;
    [SerializeField] private int levelIndex;
    [SerializeField] private GlobalTimeManager globalTimeManager;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    
    private void Start()
    {
        _button.interactable = false;
        
        if (globalTimeManager == null)
        {
            Debug.LogError("GlobalTimeManager is not assigned in LevelButton.");
            return;
        }
        
        if (globalTimeManager.IsTimeDataLoaded)
        {
            UpdateButtonState();
        }
        else
        {
            GlobalTimeManager.OnTimeDataLoaded += UpdateButtonState;
        }
    }
    
    private void OnDestroy()
    {
        GlobalTimeManager.OnTimeDataLoaded -= UpdateButtonState;
    }
    
    private void UpdateButtonState()
    {
        if (globalTimeManager != null && globalTimeManager.IsTimeDataLoaded)
        {
            _button.interactable = globalTimeManager.IsLevelUnlocked(levelIndex);
        }
    }
}
