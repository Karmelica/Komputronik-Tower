using UnityEngine;
using UnityEngine.UI;

public class ProgressUIScript : MonoBehaviour
{
    [Header("Segment Limit Offset")]
    [SerializeField] private float heightOffset;
    
    [Header("Dependencies")]
    [SerializeField] private Transform player;
    //[SerializeField] private SegmentGen segmentGen;
    [SerializeField] private GenerationManager generationManager;
    
    private Slider _slider; 
    
    private void Start()
    {
        _slider = GetComponentInChildren<Slider>();

        if (generationManager.infiniteGeneration == false)
        {
            _slider.maxValue = (generationManager.segmentLimit * generationManager.segmentHeight) - heightOffset;
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
        if (!generationManager.infiniteGeneration == false) return;
        _slider.value = player.position.y;
    }
}
