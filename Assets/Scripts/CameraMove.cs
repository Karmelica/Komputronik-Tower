using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject target; // The target object to follow
    
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
    }
}
