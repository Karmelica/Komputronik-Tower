using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SegmentGen : MonoBehaviour
{
    //public bool UseSegmentLimit => useSegmentLimit;
    //public int SegmentLimit => segmentLimit;
    
    [Header("Generation Seed")]
    public int seed;
    
    [Header("Generation Settings")]
    //[SerializeField] private bool useSegmentLimit;
    //[SerializeField, ShowIf("useSegmentLimit")] private int segmentLimit = 5;
    //[SerializeField, ShowIf("useSegmentLimit")] private GameObject finalSegment;
    [SerializeField] private GameObject finalSegment;
    
    [Header("Dependencies")]
    [SerializeField] private GenerationManager generationManager;
    [SerializeField] private GameObject previousSegment; // Reference to the last segment, if needed
    [SerializeField] private GameObject segmentPrefab;
    
    [Header("Segment Settings")]
    public float segmentHeight = 24f; // Height offset for new segments
    
    /*[Header("Gizmo Settings")]
    [SerializeField] private Vector2 gizmoSize = new (5f, 10f);
    [SerializeField] private Color gizmoColor = Color.green;*/

    private System.Random _rng;
    private readonly HashSet<SegmentScript> _segments = new();
    private bool _canGenerate;

    //[SerializeField, ShowIf("useSegmentLimit")]
    [SerializeField] private int segmentPassed;
    
    private void Awake()
    {
        
        _rng = new System.Random(seed);
    }
    
    private void Start()
    {
        //HighScoreManager.Instance.segmentLimited = _infiniteGeneration;
        
        if (segmentPrefab == null)
        {
            Debug.LogError("Segment prefab is not assigned in the SegmentGen script.", this);
        }

        if (previousSegment == null)
        {
            var segment = PoolingManager.Instance.Get<SegmentScript>("Segment");
            segment.transform.position = new Vector3(0,0,0);
            segment.transform.rotation = Quaternion.identity;
            segment.InitializeSegment(_rng);
            
            previousSegment = segment.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Segment"))
        {
            SegmentScript segScript = other.GetComponent<SegmentScript>();
            if (segScript == null || _segments.Contains(segScript)) return;
            
            _segments.Add(segScript);
            segmentPassed++;
            
            Vector3 newPosition = segScript.transform.position + new Vector3(0f, segmentHeight, 0f);

            if (!generationManager.infiniteGeneration && segmentPassed >= generationManager.segmentLimit)
            {
                Instantiate(finalSegment, newPosition, Quaternion.identity);
                return;
            }
            
            SegmentScript segmentScript = PoolingManager.Instance.Get<SegmentScript>("Segment");
            segmentScript.transform.position = newPosition;
            segmentScript.transform.rotation = Quaternion.identity;
            segmentScript.InitializeSegment(_rng);
            
            previousSegment = segmentScript.gameObject;
        }
    }
    private void RemoveSegment(SegmentScript segment)
    {
        if (_segments.Contains(segment))
        {
            _segments.Remove(segment);
        }
    }
    
    private void OnEnable()
    {
        SegmentScript.OnSegmentDeactivation += RemoveSegment;
    }

    private void OnDisable()
    {
        SegmentScript.OnSegmentDeactivation -= RemoveSegment;
    }
    
    /*private void OnDrawGizmos()
    {
        // Show where new segments will be spawned
        if (segmentPrefab != null && previousSegment != null)
        {
            Gizmos.color = gizmoColor;
            Vector3 newSegmentPosition = previousSegment.transform.position + new Vector3(0, segmentHeight, 0);
            Vector3 gizmoDrawSize = new Vector3(gizmoSize.x, gizmoSize.y, 1f);
            Gizmos.DrawWireCube(newSegmentPosition, gizmoDrawSize);
        }
    }*/
}
