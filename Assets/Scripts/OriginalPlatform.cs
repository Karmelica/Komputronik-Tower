using System.Collections;
using UnityEngine;
using DG.Tweening;

public class OriginalPlatform : MonoBehaviour
{
    public PlatformSO platformSo;
    public SpriteRenderer platformOff;
    
    [SerializeField] private GameObject visual;
    
    private Rigidbody2D _rigidbody2D;
    private Coroutine _coroutine;
    private BoxCollider2D _platformCollider;
   
    
    private float _initialPosition;

    private void Awake()
    {
        _platformCollider = GetComponent<BoxCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
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
        
        _platformCollider.size = platformOff.size;
    }
    
    private void OnEnable()
    {
        DisableFall();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("PDetector")) return;
        
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(EnableFall());
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
        _coroutine = null;
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
