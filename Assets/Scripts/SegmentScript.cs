using System;
using System.Collections.Generic;
using UnityEngine;

public class SegmentScript : MonoBehaviour
{
    public static event Action<SegmentScript> OnSegmentDeactivation;
    
    [SerializeField] private float maxPlatformRange = 5f; 
    [SerializeField] private float minPlatformRange = -5f;
    [SerializeField] private float minPlatformLength = 6f;
    [SerializeField] private float maxPlatformLength = 10f;
    [SerializeField] private float despawnOffset = 24f;
    [SerializeField] private List<GameObject> platforms;
    
    private System.Random rng;

    public void InitializeSegment(System.Random rng)
    {
        this.rng = rng;

        foreach (var platform in platforms)
        {
            platform.transform.position = GetRandomPosition(platform.transform.position);
            platform.transform.localScale = GetRandomScale(platform.transform.localScale);
        }
    }
    
    private Vector3 GetRandomPosition(Vector3 position)
    {
        float range = (float)(rng.NextDouble() * (maxPlatformRange - minPlatformRange) + minPlatformRange);
        return new Vector3(range, position.y, position.z);
    }
    
    private Vector3 GetRandomScale(Vector3 scale)
    {
        float range = (float)(rng.NextDouble() * (maxPlatformLength - minPlatformLength) + minPlatformLength);
        return new Vector3(range, scale.y, scale.z);
    }
    
    private void Update()
    {
        if (Camera.main.transform.position.y > transform.position.y + despawnOffset)
        {
            OnSegmentDeactivation?.Invoke(this);
            PoolingManager.Instance.Return("Segment", this);
        }
    }
}
