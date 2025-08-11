using System;
using System.Collections.Generic;
using UnityEngine;

public class SegmentGen : MonoBehaviour
{
    private readonly HashSet<SegmentScript> _segments = new();
    
    public int seed;
    
    [SerializeField] private GameObject previousSegment; // Reference to the last segment, if needed
    [SerializeField] private GameObject segmentPrefab;
    
    public float segmentHeight = 24f; // Height offset for new segments
    
    /*[Header("Gizmo Settings")]
    [SerializeField] private Vector2 gizmoSize = new (5f, 10f);
    [SerializeField] private Color gizmoColor = Color.green;*/

    private System.Random rng;
    
    private void Awake()
    {
        rng = new System.Random(seed);
    }
    
    private void Start()
    {
        if (segmentPrefab == null)
        {
            Debug.LogError("Segment prefab is not assigned in the SegmentGen script.", this);
        }

        if (previousSegment == null)
        {
            var segment = PoolingManager.Instance.Get<SegmentScript>("Segment");
            segment.transform.position = new Vector3(0,0,0);
            segment.transform.rotation = Quaternion.identity;
            segment.InitializeSegment(rng);
            
            previousSegment = segment.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Segment"))
        {
            SegmentScript segScript = other.GetComponent<SegmentScript>();
            if (segScript == null || _segments.Contains(segScript)) return;
            
            _segments.Add(other.GetComponent<SegmentScript>());
            
            GameObject segment = other.gameObject;
            
            Vector3 newPosition = segment.transform.position + new Vector3(0f, segmentHeight, 0f);
            
            SegmentScript segmentScript = PoolingManager.Instance.Get<SegmentScript>("Segment");
            segmentScript.transform.position = newPosition;
            segmentScript.transform.rotation = Quaternion.identity;
            
            segmentScript.InitializeSegment(rng);
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
