using UnityEngine;

[CreateAssetMenu(fileName = "PlatformSO", menuName = "Scriptable Objects/PlatformSO")]
public class PlatformSO : ScriptableObject
{
    [Header("Platform Type")] 
    public PlatformEnum platformEnum;

    [Header("Platform Settings")]
    public Sprite platformOff;
    public Sprite platformOn;
    public Sprite platformHighlight;
    public Sprite platformFunctional;

    public float functionalSpriteOffset;
    
    [Header("Material settigns")]
    [Range(0f,1f)] public float friction;
    [Range(0f,1f)] public float bounciness;
}

public enum PlatformEnum
{
    defaultPlatform,
    icyPlatform,
    fanPlatform,
}
