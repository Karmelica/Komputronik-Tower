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

    private void Awake()
    {
        _started = false;
        _body = GetComponent<Rigidbody2D>();
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
        
        _body.linearVelocity = new Vector2(0, speed);

        if (transform.position.y > startPoint.position.y)
        {
            gameObject.SetActive(false);
        }
    }
}
