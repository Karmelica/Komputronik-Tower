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
    
    [Header("Wall Boost Timer Settings")]
    [SerializeField] private float wallBoostTimeWindow = 0.2f;
    
    [Header("Minimum Bounce Speed")]
    [SerializeField, Min(0f)] float bounceSpeedTreshhold = 3f;
    
    
    [HideInInspector] public Rigidbody2D rb2D;
    private InputSystemActions _input;
    private InputSystemActions.PlayerActions _playerInput;
    
    private float _moveInput;
    private bool _isGrounded = true;
    public static bool CanMove = true;

    private float wallBoostTimer = 0f;
    private Vector2 lastWallNormal;
    private bool canWallBoost = false;
    private Vector2 preCollistionVelocity;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Wall")) return;
        
        ContactPoint2D contact = other.contacts[0];
        lastWallNormal = contact.normal;
        wallBoostTimer = wallBoostTimeWindow;
        
        canWallBoost = rb2D.linearVelocity.y >= 0f && Mathf.Abs(preCollistionVelocity.x) >= bounceSpeedTreshhold;
        
        GiveVelocityBounce(contact.normal, bounceX, bounceY);
    }

    private void GiveVelocityBounce(Vector2 contactNormal, float horizontalMultiplier, float verticalMultiplier)
    {
        Vector2 velocity = rb2D.linearVelocity;
        Vector2 reflectedVelocity = Vector2.Reflect(contactNormal, velocity);
        
        float verticalBounce = velocity.y > 0f ? velocity.y * verticalMultiplier : velocity.y;
        
        rb2D.linearVelocity = new Vector2(
            reflectedVelocity.x * horizontalMultiplier, 
            verticalBounce);
    }
    
    // Invoked when "Move" action is either started, performed or canceled.
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>().x;

        // if(Mathf.Abs(rb2D.linearVelocity.x) > 0f) Debug.Log(Mathf.Abs(rb2D.linearVelocity.x));
        if (canWallBoost && CheckVelocity(rb2D, 3f) && Mathf.Approximately(Mathf.Sign(_moveInput), Mathf.Sign(lastWallNormal.x)))
        {
            GiveVelocityBounce(-lastWallNormal, bounceBoostX, bounceBoostY);
        }

        canWallBoost = false;
        wallBoostTimer = 0f;
    }

    void InputSystemActions.IPlayerActions.OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded && CanMove)
        {
            float velocityBoost = CheckVelocity(rb2D, 6f) ? Mathf.Abs(rb2D.linearVelocity.x) * 0.33f : 2f;
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
        
        if (wallBoostTimer > 0)
        {
            wallBoostTimer -= Time.deltaTime;
            if (wallBoostTimer <= 0)
            {
                canWallBoost = false;
            }
        }

        Debug.Log(canWallBoost);
    }

    private void FixedUpdate()
    {
        preCollistionVelocity = rb2D.linearVelocity;

        if (!CanMove) return;
        rb2D.AddForce(new Vector2(_moveInput * 20f, 0), ForceMode2D.Force);
        rb2D.linearVelocity = new Vector2(Mathf.Clamp(rb2D.linearVelocity.x, -moveSpeed, moveSpeed),
            Mathf.Clamp(rb2D.linearVelocity.y, Single.MinValue, moveSpeed));
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
