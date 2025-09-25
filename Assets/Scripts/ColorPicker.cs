using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ColorPicker
{
    private static Dictionary<int, Color> outerColors = new();
    private static Dictionary<int, Color> innerColors = new();

    static ColorPicker()
    {
        outerColors = new Dictionary<int, Color>
        {
            { 1, new Color32(  0,  90, 255, 0) },          // Blue (R=0, G=0, B=1, A=1)
            { 2, new Color32(  0, 255,   7, 0) },          // Green
            { 3, new Color32(255,  68,   0, 0) },          // Red
            { 4, new Color32(255,   0, 219, 0) },          // DeepPink (approx: #FF1493)
            { 5, new Color32(255, 203,   0, 0) },          // Orange (approx: #FFA500)
            { 6, new Color32(255,  68,   0, 0) },          // Red for arcade level 
        };
        
        innerColors = new Dictionary<int, Color>
        {
            { 1, new Color32(220, 233, 255, 0) },          // Blue (R=0, G=0, B=1, A=1)
            { 2, new Color32(220, 255, 221, 0) },          // Green
            { 3, new Color32(255, 234, 220, 0) },          // Red
            { 4, new Color32(255, 220, 250, 0) },          // DeepPink (approx: #FF1493)
            { 5, new Color32(255, 250, 220, 0) },          // Orange (approx: #FFA500)
            { 6, new Color32(255, 234, 220, 0) },          // Red for arcade level   
        };
    }
    
    public static Color GetOuterColor()
    {
        return outerColors[SceneIndex()];
    }

    public static Color GetInnerColor()
    {
        return innerColors[SceneIndex()];
    }

    private static int SceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}
