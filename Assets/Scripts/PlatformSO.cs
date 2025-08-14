using UnityEngine;

[CreateAssetMenu(fileName = "PlatformSO", menuName = "Scriptable Objects/PlatformSO")]
public class PlatformSO : ScriptableObject
{
    [Header("Platform Settings")]
    public Sprite sprite;
    public Color debugColor;
    public float durationToFall;
    
    [Header("Material settigns")]
    [Range(0f,1f)] public float friction;
    [Range(0f,1f)] public float bounciness;
}
