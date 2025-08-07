using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Platform : MonoBehaviour
{
    private Collider2D _platformCollider;
    private Rigidbody2D _rigidbody2D;
    private bool _isGravityEnabled;
    private float _posY;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _posY = transform.localPosition.y;
    }
    private Transform _cameraTransform;
    
    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        if (!TryGetComponent<Collider2D>(out _platformCollider))
            Debug.LogError("No Collider2D component found on the character.", this);
        if (!TryGetComponent<Rigidbody2D>(out _rigidbody2D))
            Debug.LogError("No Rigidbody2D component found on the character.", this);
    }

    private void OnEnable()
    {
        DisableGravity();
    }

    private void Update()
    {
        if (_cameraTransform.position.y - 1f < transform.position.y)
        {
            _platformCollider.enabled = false;
        }
        else if (_cameraTransform.transform.position.y - 1.1f >= transform.position.y)
        {
            _platformCollider.enabled = true;
            StartCoroutine(EnableGravity());
        }
    }

    private void DisableGravity()
    {
        StopAllCoroutines();
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.gravityScale = 0f;
        transform.localPosition = new Vector3(transform.localPosition.x, _posY, transform.localPosition.z);
        _platformCollider.enabled = false;
        _isGravityEnabled = false;
    }
    
    private IEnumerator EnableGravity()
    {
        if (_isGravityEnabled) yield break; // Prevent multiple calls
        _isGravityEnabled = true;
        yield return new WaitForSeconds(10f);
        _platformCollider.enabled = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        // _rigidbody2D.gravityScale = 1f;
        // yield return new WaitForSeconds(3f);
    }
}
