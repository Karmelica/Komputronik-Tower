using System;
using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Collider2D platformCollider;
    private Rigidbody2D _rigibody2D;
    private bool _isGravityEnabled;
    
    private void Start()
    {
        if (!TryGetComponent<Collider2D>(out platformCollider))
            Debug.LogError("No Collider2D component found on the character.", this);
        if (!TryGetComponent<Rigidbody2D>(out _rigibody2D))
            Debug.LogError("No Rigidbody2D component found on the character.", this);
    }

    private void Update()
    {
        if (Camera.main.transform.position.y - 1f <= transform.position.y)
        {
            platformCollider.enabled = false;
        }
        else
        {
            platformCollider.enabled = true;
            StartCoroutine(EnableGravity());
        }
        
    }
    
    private IEnumerator EnableGravity()
    {
        if (_isGravityEnabled) yield break; // Prevent multiple calls
        _isGravityEnabled = true;
        yield return new WaitForSeconds(50f);
        _rigibody2D.gravityScale = 1f;
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }
}
