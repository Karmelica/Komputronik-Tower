using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    #region Variables
    
    public static bool CanMove = true;
    
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
    
    private InputSystemActions _inputActions;
    private Rigidbody2D _body;

    private Vector2 _preCollisionVelocity;
    private Vector2 _moveInput;
    private bool _isGrounded;
    
    private float _gameStartTimer;
    private bool _gameOver;
    
    private Animator _animator;
    
    [SerializeField] private SegmentDetectorScript segmentDetector;

    #endregion

    #region Unity Functions
    private void Awake()
    {
        _inputActions = new InputSystemActions();
        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CanMove = true;
        _gameStartTimer = 2f;
    }
    
    private void Update()
    {
        if (_gameStartTimer > -1f)
        {
            _gameStartTimer -= Time.deltaTime;
        }
        
        // Sprawdzanie segmentów tylko po upływie opóźnienia startowego
        if (_gameStartTimer <= 0 && segmentDetector.platforms.Count <= 0)
        {
            if (!_gameOver)
            {
                _gameOver = true;
                Debug.Log("You Died!");
                HighScoreManager.Instance.GameOver();
            }
        }
        
        // sprawdzamy czy postac jest na ziemi tylko jesli opada lub velocity jest na 0
        if (_body.linearVelocity.y <= 0)
        {
            _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, LayerMask.GetMask("Ground"));
        }
        
        _animator.SetFloat("Velocity", _moveInput.x);
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
    
    private void OnEnable()
    {
        _inputActions.Enable();
        
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        
        _inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;
        
        _inputActions.Player.Jump.performed -= OnJump;
    }
    
    #endregion
    
    #region Functions
    
    private void HandleMovement()
    {
        float currentVelocityX = _body.linearVelocity.x;
        
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
            
            // apply movement force
            //_body.AddForce(new Vector2(_moveInput.x * acceleration * accelFactor, 0), ForceMode2D.Force);
        }
        else
        {
            // deacceleration
            /*if (Mathf.Abs(_body.linearVelocity.x) > 0.01f)
            {
                float decelStep = deceleration * Time.fixedDeltaTime * Mathf.Sign(_body.linearVelocity.x);
                float newVelocity = _body.linearVelocity.x - decelStep;

                if (Mathf.Sign(newVelocity) != Mathf.Sign(_body.linearVelocity.x))
                {
                    newVelocity = 0f;
                }
                
                _body.linearVelocity = new Vector2(newVelocity, _body.linearVelocity.y);
            }*/
            
            float newVelocity = Mathf.MoveTowards(_body.linearVelocity.x, 0, deceleration * Time.fixedDeltaTime);
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
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded && CanMove)
        {
            float velocityBoost = CheckVelocity(_body, 8f) ? Mathf.Abs(_body.linearVelocity.x) * 0.33f : 2.75f;
            //Debug.Log(velocityBoost);

            _body.AddForce(Vector2.up * jumpForce * velocityBoost, ForceMode2D.Impulse);

            //Debug.Log(_body.linearVelocity);
            _isGrounded = false;
        }
    }
    #endregion
}
