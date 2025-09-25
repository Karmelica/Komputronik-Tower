using UnityEngine;

public class ColorBackgroundScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float offset;

    private float _highestY;

    private void Start()
    {
        _highestY = transform.position.y;
    }
    
    private void Update()
    {
        float targetY = player.position.y + offset;

        if (targetY > _highestY)
        {
            _highestY = targetY;
        }
        
        transform.position = new Vector2(transform.position.x, _highestY);
    }
}
