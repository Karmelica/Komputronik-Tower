using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour, InputSystemActions.IPlayerActions
{
    private Rigidbody2D _rigidbody2D;
    private InputSystemActions _input;
    private InputSystemActions.PlayerActions _playerInput;
    
    private float _moveInput;
    private bool _isGrounded = true;
    public float jumpForce = 5f;

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody2D>(out _rigidbody2D))
            Debug.LogError("No Rigidbody2D component found on the character.", this);
        _input = new InputSystemActions();
        _input.Player.SetCallbacks(this);
        _playerInput = _input.Player;
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            _isGrounded = true;
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            _isGrounded = false;
    }

    private void FixedUpdate()
    {
        _rigidbody2D.AddForce(new Vector2(_moveInput * 20f, 0), ForceMode2D.Force);
        _rigidbody2D.linearVelocity = new Vector2(Mathf.Clamp(_rigidbody2D.linearVelocity.x, -20f, 20f), Mathf.Clamp(_rigidbody2D.linearVelocity.y, -1000f, 50f));
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
            _rigidbody2D.AddForce(Vector2.up * jumpForce * (_rigidbody2D.linearVelocity.x > 0 ? _rigidbody2D.linearVelocity.x * 0.2f : 1f), ForceMode2D.Impulse);
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
