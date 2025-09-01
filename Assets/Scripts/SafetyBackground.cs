using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SafetyBackground : MonoBehaviour
{
    public Transform target;
    
    [SerializeField] private Vector2 parallaxMultiplier = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 offset;
    [SerializeField] private float despawnOffset;

    private Vector3 _originalPos;
    
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
    }
}

