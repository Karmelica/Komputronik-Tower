using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ColorPicker
{
    private static Dictionary<int, Color> colors = new();

    static ColorPicker()
    {
        colors = new Dictionary<int, Color>
        {
            { 1, new Color(0f, 0f, 1f, 0f) },             // Blue (R=0, G=0, B=1, A=1)
            { 2, new Color(0f, 1f, 0f, 0f) },             // Green
            { 3, new Color(1f, 0f, 0f, 0f) },             // Red
            { 4, new Color(1f, 0.08f, 0.58f, 0f) },       // DeepPink (approx: #FF1493)
            { 5, new Color(1f, 0.65f, 0f, 0f) },          // Orange (approx: #FFA500)
        };
    }
    
    public static Color GetColor()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        return colors[sceneIndex];
    }
}
