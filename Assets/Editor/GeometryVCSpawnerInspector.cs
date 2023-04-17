
using System;
using SonaruUtilities;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GeometryVCSpawner))]
public class GeometryVCSpawnerInspector : Editor
{

    private GeometryVCSpawner spawner;
    
    private void OnEnable()
    {
        spawner = (GeometryVCSpawner)target;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        EditorTool.DrawUILine();
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Spawn VC"))
        {
            spawner.SpawnVC();
        }

        if (GUILayout.Button("Clear VC"))
        {
            spawner.ClearVC();
        }
        
        GUILayout.EndHorizontal();
    }
}
