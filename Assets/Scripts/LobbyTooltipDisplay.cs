using System;
using TMPro;
using UnityEngine;

public class LobbyTooltipDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coolTooltip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        coolTooltip.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        coolTooltip.enabled = false;
    }
}
