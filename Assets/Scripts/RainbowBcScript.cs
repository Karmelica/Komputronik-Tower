using UnityEngine;

public class RainbowBcScript : MonoBehaviour
{
    public float speed;
    public float distance;
    
    private Vector3 _startPos;
    
    private void Start()
    {
        _startPos = transform.localPosition;
    }
    
    private void Update()
    {
        float newX = Mathf.Repeat(Time.time * speed, distance);
        transform.localPosition = _startPos + Vector3.right * newX;
    }
}
