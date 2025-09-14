using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OutletScript : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    private Animator _animator;
    public int outletIndex;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
    
    public void PlaySound()
    {
        if (audioSource) audioSource.Play();
    }
}
