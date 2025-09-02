using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlatformEffector2D))]
public class Platform : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    public bool isDynamic = true;
    
    public SpriteRenderer platformOff;
    [SerializeField] private SpriteRenderer platformOn;
    [SerializeField] private SpriteRenderer platformOnHighlight;
    [SerializeField] private SpriteRenderer platformFunctional;
    
    [SerializeField] private GameObject visual;
    
    public PlatformSO platformSo;
    
    private BoxCollider2D _platformCollider;
    private Rigidbody2D _rigidbody2D;
    
    private float _initialPosition;
    
    private Coroutine _gravityCoroutine;
    private PlatformEffector2D _effector2D;
    private Coroutine _coroutine;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        
        if (animator != null)
        {
            animator.enabled = false;
        }

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
        
        // collider bounds adjustment for milestone platforms
        _platformCollider.size = platformOff.size;
    }

    public void ChangePlatform()
    {
        gameObject.SetActive(true);
        _platformCollider.size = platformOff.size;
        
        //sprite assigner
        if (isDynamic)
        {
            platformOff.sprite = platformSo.platformOff;
            platformOn.sprite = platformSo.platformOn;
            platformOnHighlight.sprite = platformSo.platformHighlight;
            platformFunctional.sprite = platformSo.platformFunctional;
            
            Vector2 size = platformOff.size;
            
            platformOn.size = size;
            platformOnHighlight.size = size;
            platformFunctional.size = size;
            
            if (platformFunctional.sprite != null)
            {
                platformFunctional.transform.position = new Vector2(
                    platformFunctional.transform.position.x, 
                    platformFunctional.transform.position.y + platformSo.functionalSpriteOffset);
            }
        }
    }

    private void OnEnable()
    {
        animator.enabled = false;
        
        // color adjustments
        Color onColor = platformOn.color;
        Color highlightColor = platformOnHighlight.color;
        
        onColor.a = 0;
        highlightColor.a = 0;
        
        platformOn.color = onColor;
        platformOnHighlight.color = highlightColor;
        
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
