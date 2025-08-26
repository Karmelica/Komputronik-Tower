using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{    
    [Header("Generation Seed")]
    public int seed;

    [SerializeField] private Sprite[] bcSprites;
    
    [Header("Dependencies")] 
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject previousBackground; // Reference to the last segment, if needed
    [SerializeField] private GameObject backgroundPrefab;
    
    [Header("Background Settings")]
    public float backgroundHeight = 24f; // Height offset for new segments
    
    private System.Random _rng;
    private readonly HashSet<ParallaxBc> _backgrounds = new();
    private bool _canGenerate;
    
    private void Awake()
    {
        _rng = new System.Random(seed);
    }
    
    private void Start()
    {
        if (backgroundPrefab == null)
        {
            Debug.LogError("Segment prefab is not assigned in the SegmentGen script.", this);
        }

        if (previousBackground == null)
        {
            var background = PoolingManager.Instance.Get<ParallaxBc>("Background");
            background.transform.position = new Vector3(0,0,0);
            background.transform.rotation = Quaternion.identity;
            background.target = playerTransform;
            background.InitializeBc(bcSprites, _rng);
            
            previousBackground = background.gameObject;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Background"))
        {
            ParallaxBc bcScript = other.GetComponent<ParallaxBc>();
            if (bcScript == null || _backgrounds.Contains(bcScript)) return;
            
            _backgrounds.Add(bcScript);
            
            Vector3 newPosition = bcScript.OriginalPosition + new Vector3(0f, backgroundHeight, 0f);
            
            ParallaxBc backgroundScript = PoolingManager.Instance.Get<ParallaxBc>("Background");
            backgroundScript.transform.position = newPosition;
            backgroundScript.transform.rotation = Quaternion.identity;
            backgroundScript.target = playerTransform;
            backgroundScript.InitializeBc(bcSprites, _rng);
            
            previousBackground = backgroundScript.gameObject;
        }
    }
    private void RemoveSegment(ParallaxBc segment)
    {
        if (_backgrounds.Contains(segment))
        {
            _backgrounds.Remove(segment);
        }
    }
    
    private void OnEnable()
    {
        ParallaxBc.OnBackgroundDeactivation += RemoveSegment;
    }

    private void OnDisable()
    {
        ParallaxBc.OnBackgroundDeactivation -= RemoveSegment;
    }
}
