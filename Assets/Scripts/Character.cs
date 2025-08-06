using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class Character : MonoBehaviour, InputSystemActions.IPlayerActions
{
    [HideInInspector] public Rigidbody2D rb2D;
    private InputSystemActions _input;
    private InputSystemActions.PlayerActions _playerInput;
    
    private float _moveInput;
    private bool _isGrounded = true;
    public float jumpForce = 5f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Wall"))
        {
            if(rb2D.linearVelocity.y > 0f)
                rb2D.AddForce(new Vector2(0, rb2D.linearVelocity.y * 0.1f), ForceMode2D.Impulse);
        }
    }

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody2D>(out rb2D))
            Debug.LogError("No Rigidbody2D component found on the character.", this);
        _input = new InputSystemActions();
        _input.Player.SetCallbacks(this);
        _playerInput = _input.Player;
    }
    
    private void Update()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, LayerMask.GetMask("Ground"));
    }

    private void FixedUpdate()
    {
        rb2D.AddForce(new Vector2(_moveInput * 20f, 0), ForceMode2D.Force);
        rb2D.linearVelocity = new Vector2(Mathf.Clamp(rb2D.linearVelocity.x, -20f, 20f), Mathf.Clamp(rb2D.linearVelocity.y, Single.MinValue, 20f));
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
    
    // Invoked when "Move" action is either started, performed or canceled.
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>().x;
        //Debug.Log($"OnMove: {context.ReadValue<Vector2>().x}");
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        return;
    }

    void InputSystemActions.IPlayerActions.OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            rb2D.AddForce(Vector2.up * jumpForce * (rb2D.linearVelocity.x > 3 ? rb2D.linearVelocity.x * 0.3f : 1f), ForceMode2D.Impulse);
            _isGrounded = false;
        }
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

}
