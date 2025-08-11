using System;
using UnityEngine;

public class OriginalPlatformScript : MonoBehaviour
{
    [SerializeField] private float deactivationTime = 5f;

    private float timer;

    private void Start()
    {
        timer = deactivationTime;
    }
    
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
