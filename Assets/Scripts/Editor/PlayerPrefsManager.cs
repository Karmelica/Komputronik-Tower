using UnityEngine;
using UnityEditor;

public class PlayerPrefsManager : EditorWindow
{
    private int levelsCompleted;
    private string statusMessage = "";
    
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
        
        // Wyświetl aktualną wartość
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.IntField("Current LevelsCompleted:", levelsCompleted);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Space();
        
        // Przyciski akcji
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Reset to 0", GUILayout.Height(30)))
        {
            ResetLevelsCompleted();
        }
        
        if (GUILayout.Button("Refresh Values", GUILayout.Height(30)))
        {
            RefreshValues();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        
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
        
        // Sekcja do ustawiania konkretnej wartości
        /*GUILayout.Label("Set Custom Value", EditorStyles.boldLabel);
        int newValue = EditorGUILayout.IntField("New Value:", levelsCompleted);
        
        if (GUILayout.Button("Set Value", GUILayout.Height(25)) && newValue != levelsCompleted)
        {
            SetLevelsCompleted(newValue);
        }
        
        EditorGUILayout.Space()*/;
        
        // Przycisk do usunięcia wszystkich PlayerPrefs (ostrożnie!)
        EditorGUILayout.Space();
        GUILayout.Label("Danger Zone", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("DELETE ALL PLAYERPREFS", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Confirm Deletion", 
                "Are you sure you want to delete ALL PlayerPrefs? This action cannot be undone!", 
                "Yes, Delete All", "Cancel"))
            {
                DeleteAllPlayerPrefs();
            }
        }
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
        statusMessage = "Values refreshed.";
        Repaint();
    }
    
    private void ResetLevelsCompleted()
    {
        PlayerPrefs.SetInt("LevelsCompleted", 0);
        PlayerPrefs.Save();
        levelsCompleted = 0;
        statusMessage = "LevelsCompleted reset to 0.";
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
