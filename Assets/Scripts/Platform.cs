using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlatformEffector2D))]
public class Platform : MonoBehaviour
{
    public PlatformSO platformSo;
    
    private Collider2D _platformCollider;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private float _initialPosition;
    
    private Coroutine _gravityCoroutine;
    private PlatformEffector2D effector2D;

    private void Awake()
    {
        if (!TryGetComponent<SpriteRenderer>(out _spriteRenderer))
            Debug.Log("No sprite renderer", this);
        if (!TryGetComponent<Collider2D>(out _platformCollider))
            Debug.LogError("No Collider2D component found on the character.", this);
        if (!TryGetComponent<Rigidbody2D>(out _rigidbody2D))
            Debug.LogError("No Rigidbody2D component found on the character.", this);
        
        // set initial position
        _initialPosition = transform.localPosition.y;
    }
    private void Start()
    {
        // new material instance
        PhysicsMaterial2D material = new PhysicsMaterial2D
        {
            friction = platformSo.friction,
            bounciness = platformSo.bounciness
        };
        
        _platformCollider.sharedMaterial = material;
    }

    private void OnEnable()
    {
        _spriteRenderer.color = platformSo.debugColor; 
        
        DisableGravity();
        //StartCoroutine(EnableGravity(10f));
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        
        ContactPoint2D contact = collision.GetContact(0);
        if (contact.normal.y < -0.5f) // Platform's top is being hit
        {
            StartCoroutine(EnableGravity(platformSo.durationToFall));
        }
    }
    
    private void DisableGravity()
    {
        if(_gravityCoroutine != null) {
            StopCoroutine(_gravityCoroutine);
            _gravityCoroutine = null;
        }
        _platformCollider.enabled = true;
        
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.gravityScale = 0f;
        
        transform.localPosition = new Vector3(transform.localPosition.x, _initialPosition, transform.localPosition.z);
    }
    
    private IEnumerator EnableGravity(float timeToFall)
    {
        yield return new WaitForSeconds(timeToFall);
        _platformCollider.enabled = false;
        
        _rigidbody2D.gravityScale = 1f;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }
}
