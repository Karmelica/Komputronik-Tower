using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboScript : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float positionOffset;
    [SerializeField] private float platformRaycastLength;
    
    [Header("Combo Settings")]
    [SerializeField] private float comboTimer = 3f;
    [SerializeField] private int platformComboCount;
    [SerializeField] private int currentComboCount;
    
    [Header("Dependencies")]
    [SerializeField] private Image comboImage;
    [SerializeField] private TMP_Text comboText;

    [SerializeField] private int totalPlatformPassed;
    
    private float _currentComboTime;
    private RaycastHit2D _platformRaycast;
    private Rigidbody2D _body;
    private CharacterMovement _characterMovement;
    private Collider2D _lastPlatform;
    
    private bool _wasHittingPlatform;
    private bool _timerStarted;
    
    private void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
       _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleRaycastCombo();
        
        if (_body.linearVelocity.y == 0)
        {
            PlatformChecker();
        }
        
        ComboTimer();
        
        comboImage.fillAmount = _currentComboTime / comboTimer;
        comboText.text = platformComboCount.ToString();
    }

    private void HandleRaycastCombo()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(positionOffset, transform.position.y), Vector2.right, platformRaycastLength, LayerMask.GetMask("Ground"));
        Debug.DrawRay(new Vector2(positionOffset, transform.position.y), Vector2.right * platformRaycastLength, Color.red);
        
        bool isHittingPlatform = hit;
        
        if (_body.linearVelocity.y > 0)
        {
            if (isHittingPlatform && !_wasHittingPlatform)
            {
                currentComboCount++;
                totalPlatformPassed++;
            }
            _wasHittingPlatform = isHittingPlatform;
        }
        else if (_body.linearVelocity.y < 0)
        {
            if (isHittingPlatform && !_wasHittingPlatform)
            {
                currentComboCount = Mathf.Max(0, currentComboCount - 1);
                totalPlatformPassed = Mathf.Max(0, totalPlatformPassed - 1);
            }
            _wasHittingPlatform = isHittingPlatform;
        }
    }

    private void PlatformChecker()
    {
        Collider2D currentPlatform = _characterMovement.LastHit;

        if (currentPlatform == null) return;

        if (_lastPlatform == null || currentPlatform != _lastPlatform)
        {
            if (currentComboCount > 1)
            {
                _timerStarted = true;
                platformComboCount += currentComboCount;
                _currentComboTime = comboTimer;
            }
            else
            {
                ResetCombo();
            }
            
            _lastPlatform = currentPlatform;
            currentComboCount = 0;
        }
    }

    private void ComboTimer()
    {
        if (!_timerStarted) return;
        
        _currentComboTime -= Time.deltaTime;
        if (_currentComboTime <= 0f)
        {
            ResetCombo();
        }
    }

    private void ResetCombo()
    {
        CalculateBonus();
        platformComboCount = 0;
        currentComboCount = 0;
        _currentComboTime = 0;
        
        _timerStarted = false;
    }

    private int CalculateBonus()
    {
        int bonus = totalPlatformPassed * platformComboCount;
        
        Debug.Log($"total platform passed: {totalPlatformPassed}, combo: {platformComboCount}, total: {bonus}");
        return bonus;
    }
}
