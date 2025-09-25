using System;
using System.Collections.Generic;
using UnityEngine;

public class SegmentDetectorScript : MonoBehaviour
{
    public List<GameObject> platforms;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            platforms.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground") && platforms.Count > 0)
        {
            platforms.Remove(other.gameObject);
        }   
    }
}
