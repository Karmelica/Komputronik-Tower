using System;
using System.Collections.Generic;
using UnityEngine;

public class SegmentDetectorScript : MonoBehaviour
{
    public List<GameObject> segments;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Segment"))
        {
            segments.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Segment") && segments.Count > 0)
        {
            segments.Remove(other.gameObject);
        }   
    }
}
