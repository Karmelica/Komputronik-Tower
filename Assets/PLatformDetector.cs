using UnityEngine;

public class PLatformDetector : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField] private Transform player;
    [SerializeField] private Transform gracePoint;

    //private Collider2D collider;
    
    private void Awake()
    {
        //collider = GetComponent<Collider2D>();
        //collider.enabled = false;
        transform.position = new Vector3(transform.position.x, player.position.y - offset, transform.position.z);
    }
    
    private void Update()
    {
        transform.position = new Vector3(transform.position.x, player.position.y - offset, transform.position.z);

        /*if (transform.position.y >= gracePoint.position.y)
        {
            collider.enabled = true;
        }*/
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name);
    }
}
