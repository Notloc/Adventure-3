using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavGrid))]
public class NavGridCI : Editor
{
    private NavGrid navGrid;

    //NavGrid variables
    SerializedProperty nodesProp, widthProp, heightProp, positionProp;

    private void OnEnable()
    {
        navGrid = target as NavGrid;

        nodesProp = serializedObject.FindProperty("nodes");
        widthProp = serializedObject.FindProperty("width");
        heightProp = serializedObject.FindProperty("height");

        positionProp = serializedObject.FindProperty("navGridPosition");

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

        EditorGUILayout.PropertyField(positionProp);

        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        Handles.Label(navGrid.transform.position, "Nav Grid");
    }

}
