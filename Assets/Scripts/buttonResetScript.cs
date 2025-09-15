using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class buttonResetScript : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private Button button;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void OnDisable()
    {
        button.animator.SetTrigger(button.animationTriggers.normalTrigger);
        _animator.keepAnimatorStateOnDisable = true;
    }
}
