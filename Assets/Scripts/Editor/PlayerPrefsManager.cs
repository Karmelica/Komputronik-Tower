using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class PlayerPrefsManager : EditorWindow
{
    private int levelsCompleted;
    
    private List<int> animationPlayed = new List<int> {0, 0, 0, 0, 0, 0};
    private string statusMessage = "";
    private int infoScreenShown;

    [MenuItem("Tools/PlayerPrefs Manager")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsManager>("PlayerPrefs Manager");
    }
    
    private void OnEnable()
    {
        RefreshValues();
    }
    
    private void OnGUI()
    {
        GUILayout.Label("PlayerPrefs Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Sekcja LevelsCompleted
        GUILayout.Label("Levels Completed Management", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        // Wyświetl aktualną wartość
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.IntField("Info Screen Shown:", infoScreenShown);
        EditorGUI.EndDisabledGroup();
        
        if (GUILayout.Button("Set to 0", GUILayout.Height(30)))
        {
            infoScreenShown = 0;
            PlayerPrefs.SetInt("EmailConfirmed", infoScreenShown);
            PlayerPrefs.Save();
            statusMessage = "InfoScreenShown set to 0.";
            Repaint();
        }
        
        if (GUILayout.Button("Set to 1", GUILayout.Height(30)))
        {
            infoScreenShown = 1;
            PlayerPrefs.SetInt("EmailConfirmed", infoScreenShown);
            PlayerPrefs.Save();
            statusMessage = "InfoScreenShown set to 1.";
            Repaint();
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        // Sekcja LevelsCompleted
        GUILayout.Label("Levels Completed Management", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Wyświetl aktualną wartość
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.IntField("Current LevelsCompleted:", levelsCompleted);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Set to 0", GUILayout.Height(30)))
        {
            SetLevelsCompleted(0);
        }
        
        if (GUILayout.Button("Set to 1", GUILayout.Height(30)))
        {
            SetLevelsCompleted(1);
        }
        if (GUILayout.Button("Set to 2", GUILayout.Height(30)))
        {
            SetLevelsCompleted(2);
        }
        if (GUILayout.Button("Set to 3", GUILayout.Height(30)))
        {
            SetLevelsCompleted(3);
        }
        if (GUILayout.Button("Set to 4", GUILayout.Height(30)))
        {
            SetLevelsCompleted(4);
        }
        if (GUILayout.Button("Set to 5", GUILayout.Height(30)))
        {
            SetLevelsCompleted(5);
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Sekcja LevelsCompleted
        GUILayout.Label("Animations Played Managment", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        for (int i = 0; i < animationPlayed.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField($"AnimationPlayed_{i}:", animationPlayed[i]);
            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("Set to 0 (false)", GUILayout.Height(30)))
            {
                SetAnimationPlayed(i, 0);
            }
        
            if (GUILayout.Button("Set to 1 (true)", GUILayout.Height(30)))
            {
                SetAnimationPlayed(i, 1);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        
        
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Sekcja LevelsCompleted
        GUILayout.Label("Arcade Score Managment", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset Arcade Score", GUILayout.Height(30)))
        {
            DeleteArcadeScore();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Refresh Values", GUILayout.Height(30)))
        {
            RefreshValues();
        }
        
        EditorGUILayout.Space();
        GUI.backgroundColor = Color.white;
        
        // Status message
        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
        }
    }
    
    private void RefreshValues()
    {
        levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted", 0);
        infoScreenShown = PlayerPrefs.GetInt("EmailConfirmed", 0);
        for (var i = 0; i < animationPlayed.Count; i++)
        {
            animationPlayed[i] = PlayerPrefs.GetInt($"AnimationPlayed_{i}", 0);
        }
        statusMessage = "Values refreshed.";
        Repaint();
    }

    private void DeleteArcadeScore()
    {
        PlayerPrefs.SetInt("ArcadeScore", 0);
        PlayerPrefs.Save();
        statusMessage = "Arcade score deleted.";
        Repaint();
    }

    private void SetAnimationPlayed(int i, int value)
    {
        PlayerPrefs.SetInt($"AnimationPlayed_{i}", value);
        animationPlayed[i] = value;
        PlayerPrefs.Save();
        statusMessage = $"AnimationPlayed_{i} set to {value}.";
        Repaint();
    }

    private void SetLevelsCompleted(int value)
    {
        PlayerPrefs.SetInt("LevelsCompleted", value);
        PlayerPrefs.Save();
        levelsCompleted = value;
        statusMessage = $"LevelsCompleted set to {value}.";
        Repaint();
    }
    
    private void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        levelsCompleted = 0;
        statusMessage = "All PlayerPrefs deleted!";
        Repaint();
    }
}
