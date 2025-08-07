using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour, InputSystemActions.IPlayerActions
{
    [Header("Movement Settings")]
    [SerializeField] public float jumpForce = 5f;
    [SerializeField, Min(0)] private float moveSpeed = 20f;
    
    [Header("Wall bounce Settings")]
    [SerializeField] float bounceX = 5f;
    [SerializeField] float bounceY = 0.5f;
    
    [Header("Wall boost Settings")]
    [SerializeField] float bounceBoostX = 8f;
    [SerializeField] float bounceBoostY = 1.5f;
    
    [Header("Timer Settings")]
    [SerializeField] private float wallBoostTimeWindow = 0.2f;
    [SerializeField] private float playerBoostTimeWindow = 0.2f;
    
    [Header("Minimum Bounce Speed")]
    [SerializeField, Min(0f)] float bounceSpeedTreshhold = 3f;
    
    [HideInInspector] public Rigidbody2D rb2D;
    private InputSystemActions _input;
    private InputSystemActions.PlayerActions _playerInput;
    
    private float _moveInput;
    private bool _isGrounded = true;
    
    private Vector2 _preCollisionVelocity;

    private float _playerBoostTimer = 0f;
    private bool _inputInRange = false;
    
    private float _wallBoostTimer = 0f;
    private bool _canWallBoost = false;
    private Vector2 _lastWallNormal;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Wall")) return;
        
        ContactPoint2D contact = other.contacts[0];
        _lastWallNormal = contact.normal;
        _wallBoostTimer = wallBoostTimeWindow;
        
        _canWallBoost = rb2D.linearVelocity.y >= 0f && Mathf.Abs(_preCollisionVelocity.x) >= bounceSpeedTreshhold;
        
        GiveVelocityBounce(contact.normal, bounceX, bounceY);
    }

    private void GiveVelocityBounce(Vector2 contactNormal, float horizontalMultiplier, float verticalMultiplier)
    {
        Vector2 velocity = _preCollisionVelocity;
        Vector2 reflectedVelocity = Vector2.Reflect(velocity, contactNormal);
        
        float verticalBounce = velocity.y > 0f ? velocity.y * verticalMultiplier : velocity.y;
        
        rb2D.linearVelocity = new Vector2(
            reflectedVelocity.x * horizontalMultiplier, 
            verticalBounce);
    }
    
    // Invoked when "Move" action is either started, performed or canceled.
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>().x;
        
        if (_inputInRange && _canWallBoost && CheckVelocity(rb2D, 3f) && Mathf.Approximately(Mathf.Sign(_moveInput), Mathf.Sign(_lastWallNormal.x)))
        {
            GiveVelocityBounce(_lastWallNormal, -bounceBoostX, bounceBoostY);
            _canWallBoost = false;
            _wallBoostTimer = 0f;
            _playerBoostTimer = 0f;
        }
    }

    void InputSystemActions.IPlayerActions.OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            float velocityBoost = CheckVelocity(rb2D, 3f) ? Mathf.Abs(rb2D.linearVelocity.x) * 0.3f : 1f;
            rb2D.AddForce(Vector2.up * jumpForce * velocityBoost, ForceMode2D.Impulse);
            _isGrounded = false;
        }
    }
    
    private bool CheckVelocity(Rigidbody2D body, float velocity)
    {
        return Mathf.Abs(body.linearVelocity.x) >= velocity;
    }
    
    private void Awake()
    {
        if (!TryGetComponent<Rigidbody2D>(out rb2D))
        {
            Debug.LogError("No Rigidbody2D component found on the character.", this);
        }
            
        _input = new InputSystemActions();
        _input.Player.SetCallbacks(this);
        _playerInput = _input.Player;
    }
    
    private void Update()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, LayerMask.GetMask("Ground"));
        
        // timer for boost
        if (_wallBoostTimer > 0)
        {
            _wallBoostTimer -= Time.deltaTime;
            if (_wallBoostTimer <= 0)
            {
                _canWallBoost = false;
            }
        }

        // buffer timer for player
        if (_moveInput != 0)
        {
            _playerBoostTimer = playerBoostTimeWindow;
        }

        if (_playerBoostTimer > 0)
        {
            _inputInRange = true;
            _playerBoostTimer -= Time.deltaTime;
            if (_playerBoostTimer <= 0)
            {
                _inputInRange = false;
            }
        }
    }

    private void FixedUpdate()
    {
        _preCollisionVelocity = rb2D.linearVelocity;
        
        rb2D.AddForce(new Vector2(_moveInput * 20f, 0), ForceMode2D.Force);
        rb2D.linearVelocity = new Vector2(Mathf.Clamp(rb2D.linearVelocity.x, -moveSpeed, moveSpeed), Mathf.Clamp(rb2D.linearVelocity.y, Single.MinValue, moveSpeed));
    }
    
    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    private void OnDestroy()
    {
        _input.Dispose();
    }
    
    #region InputActionRegion
    public void OnCrouch(InputAction.CallbackContext context)
    {
        return;
    }
    public void OnPrevious(InputAction.CallbackContext context)
    {
        return;
    }
    public void OnNext(InputAction.CallbackContext context)
    {
        return;
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        return;
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        return;
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        return;
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        return;
    }
    #endregion
}
