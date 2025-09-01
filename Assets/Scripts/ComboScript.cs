using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ComboScript : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float xPositionOffset;
    [SerializeField] private float yPositionOffset;
    [SerializeField] private float platformRaycastLength;
    
    [Header("Combo Settings")]
    [SerializeField] private float comboTimer = 3f;
    [SerializeField] private int streakComboCount;
    [SerializeField] private int currentComboCount;
    [SerializeField] private int currentStreak;
    
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
        
        if (_characterMovement.Grounded)
        {
            PlatformChecker();
        }
        
        ComboTimer();
        
        comboImage.fillAmount = _currentComboTime / comboTimer;
        comboText.text = currentStreak > 1 ? streakComboCount.ToString() : null;
    }

    private void HandleRaycastCombo()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(xPositionOffset, transform.position.y + yPositionOffset),
            Vector2.right, platformRaycastLength, 
            LayerMask.GetMask("Ground"));
        
        Debug.DrawRay(
            new Vector2(xPositionOffset, transform.position.y + yPositionOffset),
            Vector2.right * platformRaycastLength, 
            Color.red);
        
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
        Collider2D currentPlatform = _characterMovement.CurrentHit;

        if (currentPlatform == null) return;

        if (_lastPlatform == null || currentPlatform != _lastPlatform)
        {
            if (_lastPlatform == null)
            {
                _lastPlatform = currentPlatform;
            }
            
            if (currentComboCount > 1 && currentPlatform.transform.position.y > _lastPlatform.transform.position.y)
            {
                _timerStarted = true;
                streakComboCount += currentComboCount;
                currentStreak++;
                _currentComboTime = comboTimer;
            }
            else
            {
                currentComboCount = 0;
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
        HighScoreManager.Instance.AddScore(CalculateBonus());
        streakComboCount = 0;
        currentComboCount = 0;
        currentStreak = 0;
        _currentComboTime = 0;
        
        _timerStarted = false;
    }

    private int CalculateBonus()
    {
        if (currentStreak > 1)
        {
            int bonus = totalPlatformPassed * 10 + (streakComboCount * currentStreak);
            Debug.Log($"total platform passed: {totalPlatformPassed}, combo steak: {currentStreak}, combo: {streakComboCount}, total: {bonus}");
            return bonus;
        }
        
        return 0;
    }
}
