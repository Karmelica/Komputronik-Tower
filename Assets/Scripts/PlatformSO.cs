using UnityEngine;

[CreateAssetMenu(fileName = "PlatformSO", menuName = "Scriptable Objects/PlatformSO")]
public class PlatformSO : ScriptableObject
{
    [Header("Platform Settings")]
    // docelowo miejsce na teksture
    public Sprite sprite;
    
    // narazie skoro nie mamy jeszcze tekstur to
    // platformy maja swoje kolory zeby moc je od siebie rozrozniac
    public Color debugColor;
    
    [Header("Material settigns")]
    [Range(0f,1f)] public float friction;
    [Range(0f,1f)] public float bounciness;
}
