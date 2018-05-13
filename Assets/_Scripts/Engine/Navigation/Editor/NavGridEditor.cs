using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavGrid))]
public class NavGridEditor : Editor
{
    private NavGrid navGrid;

    SerializedProperty nodesProp, widthProp, heightProp, offsetProp;

    private void OnEnable()
    {
        navGrid = target as NavGrid;

        //Get properties
        nodesProp = serializedObject.FindProperty("nodes");
        widthProp = serializedObject.FindProperty("width");
        heightProp = serializedObject.FindProperty("height");
        offsetProp = serializedObject.FindProperty("positionOffset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("Generate Nodes"))
        {
            navGrid.GenerateNodes();
        }

        EditorGUILayout.PropertyField(widthProp);
        EditorGUILayout.PropertyField(heightProp);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(offsetProp);

        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI();
    }
}

