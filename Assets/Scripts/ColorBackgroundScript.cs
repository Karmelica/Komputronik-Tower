using UnityEngine;

public class ColorBackgroundScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float offset;
    
    private void Update()
    {
        transform.position = new Vector2(transform.position.x, player.position.y + offset);
    }
}
