using System.Collections.Generic;
using UnityEngine;

public class SegmentScript : MonoBehaviour
{
    [SerializeField] private float maxPlatformRange = 5f; 
    [SerializeField] private float minPlatformRange = 0f; 
    [SerializeField] private List<GameObject> platforms;

    private System.Random rng;

    public void InitializeSegment(System.Random rng)
    {
        this.rng = rng;
        
        for (int i = 0; i < platforms.Count; i++)
        {
            platforms[i].transform.position = GetRandomPosition(platforms[i].transform.position);
        }
    }
    
    private Vector3 GetRandomPosition(Vector3 position)
    {
        return new Vector3(UnityEngine.Random.Range(minPlatformRange, maxPlatformRange), position.y, position.z); 
    }
}
