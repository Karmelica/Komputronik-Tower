using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GenerationManager generationManager;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform startPoint;
    
    [Header("Settings")]
    [SerializeField] private float maxDistance;
    [SerializeField] private float graceTime;
    public float speed;
    
    private bool _moveStarted;
    private float _graceTimer;
    private bool _infiniteGeneration;

    private Collider2D _collider;
    private Rigidbody2D _body;
    
    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        
        _collider.enabled = false;
        transform.position = startPoint.position;
        _moveStarted = false;
    }

    private void Start()
    {
        _infiniteGeneration = generationManager.infiniteGeneration;
    }
    
    private void Update()
    {
        if (_infiniteGeneration)
        {
            if (player.transform.position.y >= startPoint.position.y && !_moveStarted)
            {
                _moveStarted = true;
                _graceTimer = graceTime;
            }
        
            HighScoreManager.Instance.moveStarted =  _moveStarted;
        
            if (_moveStarted)
            {
                if (_graceTimer > 0)
                {
                    _graceTimer -= Time.deltaTime;
                    return;
                }
            
                float targetY = player.transform.position.y - maxDistance;
            
                if (Mathf.Abs(transform.position.y - player.transform.position.y) < maxDistance)
                {
                    _body.linearVelocity = new Vector2(0, speed);
                }
                else
                {
                    _body.linearVelocity = Vector2.zero;
                    transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                }
                _collider.enabled = true;
            }
        }
    }
}
