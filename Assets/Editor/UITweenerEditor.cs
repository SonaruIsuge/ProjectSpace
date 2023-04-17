
using System;
using SonaruUtilities;
using UnityEditor;
using UnityEngine;

public class UITweenerEditor : EditorWindow
{
    private GameObject nowSelect;

    public GameObject NowSelect
    {
        get => nowSelect;
        set
        {
            if (nowSelect == value) return;
            nowSelect = value;
            OnNowSelectChanged?.Invoke(nowSelect);
        }
    }
    
    private UITweenBase[] allUITween;
    private Vector2 scrollPosition = Vector2.zero;

    private bool demoTween;

    private event Action<GameObject> OnNowSelectChanged;
    
    
    [MenuItem("Custom Editors/UI Tween Collection Editor")]
    private static void ShowWindow() {
        var window = GetWindow<UITweenerEditor>();
        window.titleContent = new GUIContent("All Tween");
        window.Show();
    }


    private void OnEnable()
    {
        OnNowSelectChanged += CheckAllUITween;
    }


    private void OnDisable()
    {
        OnNowSelectChanged += CheckAllUITween;
    }


    private void OnGUI()
    {
        GUILayout.Label("Selection GameObject");
        NowSelect = Selection.activeGameObject;

        if (!NowSelect || allUITween.Length == 0)
        {
            Repaint();
            return;
        }

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Select", nowSelect, typeof(GameObject), true);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Space();
        EditorTool.DrawUILine();
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (var i = 0; i < allUITween.Length; i++)
        {
            var uiTween = allUITween[i];
            var serializedObj = new SerializedObject(uiTween);
            var property = serializedObj.GetIterator();
            
            EditorGUILayout.LabelField("Object name:",  $"{uiTween.gameObject.name}", EditorStyles.boldLabel);
            
            // First property: script self
            property.NextVisible(true);
            EditorGUILayout.ObjectField(property.displayName, property.objectReferenceValue, property.objectReferenceValue.GetType(), true);
            
            while (property.NextVisible(false))
            {
                if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
                {
                    EditorGUILayout.ObjectField(property.displayName, property.objectReferenceValue, property.objectReferenceValue.GetType(), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(property, false);
                }
            }
            EditorGUILayout.Space();
            EditorTool.DrawUILine();
            
            serializedObj.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Tween Demo"))
        {
            
        }
        
        EditorGUILayout.EndScrollView();
    }


    private void CheckAllUITween(GameObject selectObj)
    {
        if(selectObj) allUITween = nowSelect.GetComponentsInChildren<UITweenBase>(true);
    }
}
