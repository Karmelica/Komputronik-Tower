using System;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    [Header("Generation Settings")]
    public bool infiniteGeneration = false;
    
    [Header("Segment Settings")]
    public int segmentLimit = 2;
    public float segmentHeight = 47.5f;

    private void Start()
    {
        HighScoreManager.Instance.infiniteGeneration = infiniteGeneration;
    }

    private void Update()
    {
        if (!infiniteGeneration)
        {
            HighScoreManager.Instance.AddScore(1 * Time.deltaTime);
        }
    }
}
