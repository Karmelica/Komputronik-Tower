using TMPro;
using UnityEngine;
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
    [SerializeField] private GenerationManager generationManager;

    [SerializeField] private int totalPlatformPassed;
    
    private float _currentComboTime;
    private RaycastHit2D _platformRaycast;
    private Rigidbody2D _body;
    private CharacterMovement _characterMovement;
    private Collider2D _lastPlatform;
    private SoundPlayer _soundPlayer;
    
    private bool _wasHittingPlatform;
    private bool _timerStarted;
    private bool _firstCombo;

    [SerializeField] private Transform startingPoint;
    
    private void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
       _body = GetComponent<Rigidbody2D>();
       _soundPlayer = GetComponent<SoundPlayer>();

       if (!generationManager.infiniteGeneration)
       {
           comboImage.enabled = false;
           comboText.enabled = false;
       }
       
       _firstCombo = true;
    }

    private void Update()
    {
        HandleRaycastCombo();
        
        if (!generationManager.infiniteGeneration) return;
        
        if (_characterMovement.Grounded)
        {
            PlatformChecker();
        }
        
        ComboTimer();
        
        comboImage.fillAmount = _currentComboTime / comboTimer;
        comboText.text = currentStreak > 0 ? $"{streakComboCount} x{currentStreak}" : null;
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

        // platform animator activator
        if (hit.collider != null && hit.collider.TryGetComponent(out Platform platform))
        {
            platform.animator.enabled = true;
        }
        
        bool isHittingPlatform = hit;
        
        if (_body.linearVelocity.y > 0 && _body.transform.position.y >= startingPoint.position.y)
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
                if (_firstCombo)
                {
                    currentComboCount = 0;
                    ResetCombo();
                    _firstCombo = false;
                }
                
                _timerStarted = true;
                streakComboCount += currentComboCount;
                currentStreak++;
                _currentComboTime = comboTimer;
                //_soundPlayer.PlayRandom("Combo");
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
        if (_firstCombo)
        {
            return 0;
        }
        
        int bonus = 0;
        
        if (streakComboCount <= 1)
        {
            if (_lastPlatform != null && _characterMovement.CurrentHit != null)
            {
                float lastY = _lastPlatform.transform.position.y;
                float currentY = _characterMovement.CurrentHit.transform.position.y;
                
                if (currentY > lastY)
                {
                    bonus = streakComboCount * currentStreak;
                }
            }
        }
        else
        {
            bonus = totalPlatformPassed * 10 + (streakComboCount * currentStreak);
        }
        
        //int bonus = totalPlatformPassed * 10 + (streakComboCount * currentStreak); 
        Debug.Log($"total platform passed: {totalPlatformPassed}, combo steak: {currentStreak}, combo: {streakComboCount}, total: {bonus}"); 
        return bonus;
    }
}
