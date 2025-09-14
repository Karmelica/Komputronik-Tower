using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    #region Variables

    public Collider2D CurrentHit;
    public bool Grounded => _isGrounded;
    public static bool CanMove = true;

    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float rDistance;
    [SerializeField] private float secondDistance;
    [SerializeField] private float secondOffset;
    
    [Header("Max move and jump settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float jumpForce;
    
    [Header("Acceleration settings")]
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    
    [Header("Gravity settings")]
    [SerializeField] private float upwardMultiplier;
    [SerializeField] private float fallMultiplier;

    [Header("Wall Bounce settings")]
    [SerializeField] private AnimationCurve bounceCurve;
    [SerializeField] private float bounceX = 1f;
    [SerializeField] private float bounceY = 1f;
    
    [Header("Jump Buffer Settings")]
    [SerializeField] private float jumpBufferTime;

    private float _jumpBufferCounter;
    
    private SoundPlayer _soundPlayer;
    private InputSystemActions _inputActions;
    private Rigidbody2D _body;

    private Vector2 _lastInput;
    private Vector2 _preCollisionVelocity;
    private Vector2 _moveInput;
    private Collider2D _lastGroundCollider = null;
    private bool _isGrounded;
    private bool _wasGrounded;
    
    private float _gameStartTimer;
    private bool _gameOver;
    
    private Animator _animator;

    public static bool levelEnded;
    public static bool startCounting;
    
    [SerializeField] private SegmentDetectorScript segmentDetector;
    [SerializeField] private GenerationManager generationManager;
    
    
    [SerializeField] private ParticleSystem particleSystem;

    #endregion

    #region Unity Functions
    private void Awake()
    {
        _inputActions = new InputSystemActions();
        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _soundPlayer = GetComponent<SoundPlayer>();
    }

    private void Start()
    {
        startCounting = false;
        CanMove = true;
        levelEnded = false;
        _gameStartTimer = 2f;
        _animator.SetFloat("LastInputX", 1f);
    }
    
    private void Update()
    {
        if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
        
        if (_gameStartTimer > -1f)
        {
            _gameStartTimer -= Time.deltaTime;
        }

        if(segmentDetector != null){
            // Sprawdzanie segmentów tylko po upływie opóźnienia startowego
            if (_gameStartTimer <= 0 && segmentDetector.platforms.Count <= 0)
            {
                if (!_gameOver)
                {
                    _gameOver = true;
                    //Debug.Log("You Died!");
                    HighScoreManager.Instance.GameOver(true);
                }
            }
        }
        
        // sprawdzamy czy postac jest na ziemi tylko jesli opada lub velocity jest na 0
        if (_body.linearVelocity.y <= 0)
        {
            Vector2 secondPos = new Vector2(raycastOrigin.position.x, raycastOrigin.position.y + secondOffset);
            RaycastHit2D secondHit = Physics2D.Raycast(secondPos, Vector2.down, secondDistance, LayerMask.GetMask("Ground"));
            Debug.DrawRay(secondPos, Vector2.down * secondDistance, Color.blue);
            
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.position, Vector2.down, rDistance, LayerMask.GetMask("Ground"));
            Debug.DrawRay(raycastOrigin.position, Vector2.down * rDistance, Color.red);

            if (hit.collider != null && secondHit.collider == null)
            {
                _isGrounded =  true;
                CurrentHit = hit.collider;
            }
            else
            {
                _isGrounded = false;
            }
            
            //Debug.Log(_isGrounded);
        }
        
        if (_isGrounded && !_wasGrounded)
        {
            if (_lastGroundCollider == null)
            {
                _lastGroundCollider = CurrentHit;
            }

            if (_lastGroundCollider != null && CurrentHit.transform.position.y > _lastGroundCollider.transform.position.y && generationManager.infiniteGeneration)
            {
                _lastGroundCollider = CurrentHit;
                HighScoreManager.Instance.AddScore(10);
            }
        }
        
        if (_isGrounded && _jumpBufferCounter > 0f)
        {
            PerformJump();
            _jumpBufferCounter = 0f; // consume buffered input
        }

        if (Mathf.Abs(_body.linearVelocity.y) <= 3f && particleSystem != null)
        {
            particleSystem.Stop();
        }
        
        _wasGrounded = _isGrounded;
        
        if (CanMove) _animator.SetFloat("Velocity", _moveInput.x);
        _animator.SetFloat("YVelocity", _body.linearVelocity.y);
        _animator.SetBool("Grounded", _isGrounded);

        if (_isGrounded)
        {
            _animator.SetBool("Combo", false);
        }
    }
    
    private void FixedUpdate()
    {
        if (!CanMove) return;
        _preCollisionVelocity = _body.linearVelocity;
        
        HandleMovement();
        HandleGravity();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Wall")) return;
        
        ContactPoint2D contact = collision.contacts[0];
        GiveVelocityBounce(contact.normal, bounceX, bounceY);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LevelEnd"))
        {
            int level = SceneManager.GetActiveScene().buildIndex;
            if(PlayerPrefs.GetInt("LevelsCompleted") < level) PlayerPrefs.SetInt("LevelsCompleted", level);
            levelEnded = true;
            Invoke(nameof(EndLevel), 2f);
        }
    }

    private void EndLevel()
    {
        //HighScoreManager.Instance.LevelEnd();
        HighScoreManager.Instance.GameOver(false);
    }
    
    private void OnEnable()
    {
        _inputActions.Enable();
        
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        
        _inputActions.Player.Jump.performed += OnJump;
        
        _inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;
        
        _inputActions.Player.Jump.performed -= OnJump;
        
        _inputActions.Player.Interact.performed -= OnInteract;
    }
    
    #endregion
    
    #region Functions
    
    private void HandleMovement()
    {
        float currentVelocityX = _body.linearVelocity.x;
        
        float currentDeacceleration = deceleration;

        if (CurrentHit != null && CurrentHit.TryGetComponent(out Platform platform))
        {
            if (platform.platformSo.platformEnum == PlatformEnum.icyPlatform)
            {
                currentDeacceleration = platform.platformSo.platformModifierValue;
            }
        }
        
        if (_moveInput != Vector2.zero)
        {
            // normalizing speed to 0 and 1
            float speedPercent = Mathf.Abs(_body.linearVelocity.x) / moveSpeed;
            speedPercent = Mathf.Clamp01(speedPercent);
            
            // apply normalized speed to animation curve
            float accelFactor = accelerationCurve.Evaluate(speedPercent);
            
            bool reversing = Mathf.Sign(_moveInput.x) != Mathf.Sign(currentVelocityX) && Mathf.Abs(currentVelocityX) > 0.1f;

            // faster direction change 
            if (reversing)
            {
                _body.AddForce(new Vector2(_moveInput.x * acceleration * 2f, 0), ForceMode2D.Force);
            }
            else
            {
                // normal acceleration
                _body.AddForce(new Vector2(_moveInput.x * acceleration * accelFactor, 0), ForceMode2D.Force);
            }
        }
        else
        {
            float newVelocity = Mathf.MoveTowards(_body.linearVelocity.x, 0, currentDeacceleration * Time.fixedDeltaTime);
            _body.linearVelocity = new Vector2(newVelocity, _body.linearVelocity.y);
        }
        
        // max speed clamp
        _body.linearVelocity = 
            new Vector2(
                Mathf.Clamp(_body.linearVelocity.x, -moveSpeed, moveSpeed), 
                Mathf.Clamp(_body.linearVelocity.y, -jumpSpeed, jumpSpeed)
            );
    }

    private void HandleGravity()
    {
        // additional gravity when jumping and falling to reach top speed quicker
        if (_body.linearVelocity.y < 0)
        {
            _body.linearVelocity += Vector2.up * (Physics2D.gravity.y * fallMultiplier * Time.fixedDeltaTime);
        }

        if (_body.linearVelocity.y > 0)
        {
            _body.linearVelocity += Vector2.up * (Physics2D.gravity.y * upwardMultiplier * Time.fixedDeltaTime);
        }
    }
    
    private bool CheckVelocity(Rigidbody2D body, float velocity)
    {
        return Mathf.Abs(body.linearVelocity.x) >= velocity;
    }
    
    private void GiveVelocityBounce(Vector2 contactNormal, float horizontalMultiplier, float verticalMultiplier)
    {
        Vector2 velocity = _preCollisionVelocity;
        Vector2 reflectedVelocity = Vector2.Reflect(velocity, contactNormal);

        float yFactor = bounceCurve.Evaluate(Mathf.Abs(velocity.y) / jumpSpeed);
        float dynamicHorizontal = horizontalMultiplier * yFactor;
        
        float verticalBounce = velocity.y > 0f ? velocity.y * verticalMultiplier : velocity.y;

        _body.linearVelocity = new Vector2(
            reflectedVelocity.x * dynamicHorizontal,
            verticalBounce);
    }
    
    #endregion
    
    #region InputActions
    
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();

        if (_moveInput != Vector2.zero)
        {
            _lastInput = _moveInput;
        }
        
        _animator.SetFloat("LastInputX", _lastInput.x);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        /*if (_isGrounded && CanMove)
        {
            
        }*/

        if (context.performed && CanMove)
        {
            _jumpBufferCounter = jumpBufferTime;
        }
    }

    private void PerformJump()
    {
        if(startCounting == false){
            startCounting = true;
        }
            
        float velocityBoost = CheckVelocity(_body, 8f) ? Mathf.Abs(_body.linearVelocity.x) * 0.33f : 2.75f;
        //Debug.Log(velocityBoost);

        float currentJumpForce = jumpForce;

        if (CurrentHit != null && CurrentHit.TryGetComponent(out Platform platform))
        {
            if (platform.platformSo.platformEnum == PlatformEnum.fanPlatform)
            {
                currentJumpForce += platform.platformSo.platformModifierValue; // fan boost
            }
        }

        _body.AddForce(Vector2.up * currentJumpForce * velocityBoost, ForceMode2D.Impulse);

        if (_soundPlayer != null)
        {
            if (velocityBoost > 3f)
            {
                if (particleSystem != null)
                {
                    particleSystem.Play();
                }
                _soundPlayer.PlayRandom("Combo");
                _animator.SetBool("Combo", true);
            }
            else
            {
                _soundPlayer.PlayRandom("Jump");
            }
        }
            
        //Debug.Log(_body.linearVelocity);
        _isGrounded = false;
    }
    
    private void OnInteract(InputAction.CallbackContext context)
    {
        /*if (canEndLevel)
        {
            SceneManager.LoadScene(0);
        }*/
    }
    
    #endregion
}
