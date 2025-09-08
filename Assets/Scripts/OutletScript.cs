using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OutletScript : MonoBehaviour
{
    private Animator _animator;
    public int outletIndex;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.SetBool("AnimPlayed", PlayerPrefs.GetInt($"AnimationPlayed_{outletIndex}", 0) == 1);
    }

    public void AnimationPlayed()
    {
        string key = $"AnimationPlayed_{outletIndex}";
        PlayerPrefs.SetInt(key, 1);
    }
}
