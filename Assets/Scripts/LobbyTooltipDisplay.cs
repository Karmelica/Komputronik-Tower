using System;
using TMPro;
using UnityEngine;

public class LobbyTooltipDisplay : MonoBehaviour
{
    [SerializeField] private Animator coolTooltip;
    private bool _isDestroyed;
    
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 
    }

    private void OnDestroy()
    {
        _isDestroyed = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if(coolTooltip && !_isDestroyed)
            coolTooltip.SetBool("Show", true);
        _audioSource?.Play();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if(coolTooltip && !_isDestroyed)
            coolTooltip.SetBool("Show", false);
    }
}
