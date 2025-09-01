using System;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBc : MonoBehaviour
{
    public Vector3 OriginalPosition => _originalPos;
    public static event Action<ParallaxBc> OnBackgroundDeactivation;
    public Transform target;
    
    [SerializeField] private Vector2 parallaxMultiplier = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 offset;
    [SerializeField] private float despawnOffset;

    private Vector3 _originalPos;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        Vector2 targetMovement = target.position;

        Vector3 newPos = _originalPos + new Vector3(
            targetMovement.x * parallaxMultiplier.x,
            targetMovement.y * parallaxMultiplier.y,
            0
        );

        transform.position = new Vector2(transform.position.x, newPos.y + offset.y);
        
        if (Camera.main.transform.position.y > transform.position.y + despawnOffset)
        {
            OnBackgroundDeactivation?.Invoke(this);
            PoolingManager.Instance.Return("Background", this);
        }
    }

    public void InitializeBc(Sprite[] sprites, Random rng)
    {
        _originalPos = transform.position;
        _renderer.sprite = sprites[rng.Next(sprites.Length)];
        
        gameObject.SetActive(true);
    }
}
