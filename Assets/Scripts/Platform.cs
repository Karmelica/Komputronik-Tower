using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlatformEffector2D))]
public class Platform : MonoBehaviour
{
    public PlatformSO platformSo;
    
    [SerializeField] private GameObject visual;
    
    private Collider2D _platformCollider;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private float _initialPosition;
    
    private Coroutine _gravityCoroutine;
    private PlatformEffector2D effector2D;
    private Coroutine coroutine;
    
    private void Awake()
    {
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        visual = spriteRenderer.gameObject;
        _spriteRenderer = spriteRenderer;
        
        /*if (!TryGetComponent<SpriteRenderer>(out _spriteRenderer))
            Debug.Log("No sprite renderer", this);*/
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
        
        DisableFall();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("PDetector")) return;
        
        if (coroutine != null) return;
        coroutine = StartCoroutine(EnableFall());
        StartShake();
    }
    
    private IEnumerator EnableFall()
    {
        yield return new WaitForSeconds(2);
        _rigidbody2D.linearVelocity = new Vector2(0, -2f);
        
        // how long the platform will drop until disabling 
        yield return new WaitForSeconds(5f);
        
        gameObject.SetActive(false);
    }

    private void DisableFall()
    {
        coroutine = null;
        _rigidbody2D.linearVelocity = Vector2.zero;
        
        Vector3 initialPosition = new Vector3(transform.localPosition.x, _initialPosition, transform.localPosition.z);
        
        transform.localPosition = initialPosition;
        visual.transform.localPosition = Vector3.zero;
    }

    private void StartShake()
    {
        visual.transform.DOShakePosition(1.5f, 0.01f, 10);
    }
}
