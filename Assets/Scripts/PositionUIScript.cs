using UnityEngine;
using UnityEngine.UI;

public class PositionUIScript : MonoBehaviour
{
    [Header("Segment Limit Offset")]
    [SerializeField] private float heightOffset;
    
    [Header("Dependencies")]
    [SerializeField] private Transform player;
    [SerializeField] private SegmentGen segmentGen;
    
    private Slider _slider; 
    
    private void Start()
    {
        _slider = GetComponentInChildren<Slider>();

        if (segmentGen.UseSegmentLimit)
        {
            _slider.maxValue = (segmentGen.SegmentLimit * segmentGen.segmentHeight) - heightOffset;
        }
        else
        {
            _slider.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        ShowPosition();
    }

    private void ShowPosition()
    {
        if (!segmentGen.UseSegmentLimit) return;
        _slider.value = player.position.y;
    }
}
