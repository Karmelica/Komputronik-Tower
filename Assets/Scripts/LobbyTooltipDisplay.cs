using System;
using TMPro;
using UnityEngine;

public class LobbyTooltipDisplay : MonoBehaviour
{
    [SerializeField] private Animator coolTooltip;
    
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        coolTooltip.SetBool("Show", true);
        _audioSource.Play();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        coolTooltip.SetBool("Show", false);
    }
}
