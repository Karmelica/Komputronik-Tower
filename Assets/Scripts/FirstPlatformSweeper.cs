using UnityEngine;

public class FirstPlatformSweeper : MonoBehaviour
{
    [SerializeField] private Character player;
    [SerializeField] private Transform startPoint;
    [SerializeField] private float graceTime;

    public float speed;
    
    private bool _started = false;
    private float _graceTimer;
    private Rigidbody2D _body;
    private Collider2D _collider;

    private void Awake()
    {
        _started = false;
        _collider = GetComponent<Collider2D>();
        _body = GetComponent<Rigidbody2D>();
        
        _collider.enabled = false;
    }
    
    private void Update()
    {
        if (player.transform.position.y > startPoint.position.y && !_started)
        {
            _started = true;
            _graceTimer = graceTime;
        }

        if (!_started) return;
        
        if (_graceTimer > 0)
        {
            _graceTimer -= Time.deltaTime;
            return;
        }
        
        _collider.enabled = true;
        _body.linearVelocity = new Vector2(0, speed);

        if (transform.position.y > startPoint.position.y + 10f)
        {
            gameObject.SetActive(false);
        }
    }
}
