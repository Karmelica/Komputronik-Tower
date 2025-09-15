using UnityEngine;
using UnityEngine.Events;

public class buttonResetScript : MonoBehaviour
{
    public UnityEvent OnDisableButton;
    public UnityEvent OnEnableButton;
    
    private Animator _animator;
    
    private void OnDisable()
    {
        OnDisableButton?.Invoke();
    }

    private void OnEnable()
    {
        OnEnableButton?.Invoke();
    }
}
