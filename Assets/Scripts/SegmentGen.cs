using System;
using UnityEngine;

public class SegmentGen : MonoBehaviour
{
    [SerializeField] private GameObject lastSegment; // Reference to the last segment, if needed
    [SerializeField] private GameObject segmentPrefab;
    public float segmentHeight = 10f; // Height offset for new segments
    
    [Header("Gizmo Settings")]
    [SerializeField] private Vector2 gizmoSize = new (5f, 10f);
    [SerializeField] private Color gizmoColor = Color.green;

    
    private void Start()
    {
        if (segmentPrefab == null)
        {
            Debug.LogError("Segment prefab is not assigned in the SegmentGen script.", this);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Segment"))
        {
            // Instantiate a new segment at the position of the exiting segment
            Vector3 newPosition = lastSegment.transform.position + new Vector3(0, segmentHeight, 0); // Adjust the Y offset as needed
            lastSegment.GetComponent<Collider2D>().enabled = false;
            lastSegment = Instantiate(segmentPrefab, newPosition, Quaternion.identity);
            
            // Optionally destroy the old segment
            //Destroy(other.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        // Show where new segments will be spawned
        if (segmentPrefab != null && lastSegment != null)
        {
            Gizmos.color = gizmoColor;
            Vector3 newSegmentPosition = lastSegment.transform.position + new Vector3(0, segmentHeight, 0);
            Vector3 gizmoDrawSize = new Vector3(gizmoSize.x, gizmoSize.y, 1f);
            Gizmos.DrawWireCube(newSegmentPosition, gizmoDrawSize);
        }
    }
}
