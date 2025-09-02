using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField] private float smoothTime = 0.2f;
    
    public GameObject target; // The target object to follow

    private float _velocityY = 0f;
    
    private void LateUpdate()
    {
        if (target == null) return;

        float targetY = transform.position.y;
        float distanceY = target.transform.position.y - transform.position.y;

        if (distanceY > offset)
        {
            targetY = target.transform.position.y - offset;
        }
        
        else if (distanceY < -offset)
        {
            targetY = target.transform.position.y + offset;
        }
        
        //transform.position = camPos;
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref _velocityY, smoothTime);
        
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
