using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    [SerializeField] private GameObject milestonePlatform;

    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<PlatformType> platformTypes;
    
    private System.Random _rng;

    public void InitializeSegment(System.Random rng)
    {
        this._rng = rng;
        
        if (platforms.Count == 0) return;
        foreach (var platform in platforms)
        {
            PlatformType chosenPlatform = InitializePlatforms(rng);
            
            platform.TryGetComponent<Platform>(out var platformObj);
                
            platformObj.platformSo = chosenPlatform.platformType;
            
            platform.transform.position = GetRandomPosition(platform.transform.position);
            platformObj.platformOff.size = GetRandomScale(platformObj.platformOff.size);
            
            platform.SetActive(true);
        }
        
        milestonePlatform.SetActive(true);
    }

    private PlatformType InitializePlatforms(System.Random rng)
    {
        int totalWeight = 0;
        foreach (var type in platformTypes)
        {
            totalWeight += type.spawningChance;
        }
        
        int roll = rng.Next(0, totalWeight);
        int cumulative = 0;

        foreach (var type in platformTypes)
        {
            cumulative += type.spawningChance;
            if (roll < cumulative)
            {
                return type;
            }
        }
        return null;
    }
    
    private Vector3 GetRandomPosition(Vector3 position)
    {
        float range = (float)(_rng.NextDouble() * (maxPlatformRange - minPlatformRange) + minPlatformRange);
        return new Vector3(range, position.y, position.z);
    }
    
    private Vector3 GetRandomScale(Vector3 scale)
    {
        float range = (float)(_rng.NextDouble() * (maxPlatformLength - minPlatformLength) + minPlatformLength);
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

[Serializable]
public class PlatformType
{
    public PlatformSO platformType;
    [Range(0, 100)] public int spawningChance;
}
