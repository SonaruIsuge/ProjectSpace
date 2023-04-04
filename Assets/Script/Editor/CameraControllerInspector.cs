
using System;
using SonaruUtilities;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CameraController))]
public class CameraControllerInspector : Editor
{
    private CameraController cameraController;


    private void OnEnable()
    {
        cameraController = (CameraController)target;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        EditorTool.DrawUILine();
        
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Last Camera"))
        {
            cameraController.EditorSwitchCamera(-1);
        }

        if (GUILayout.Button("Next Camera"))
        {
            cameraController.EditorSwitchCamera(1);
        }
        
        GUILayout.EndHorizontal();
        
    }
}
