using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entity))]
public class EntityCI : EasyEditor
{
    Entity entity;

    SerializedProperty noah;

    SerializedProperty skills;

    private void OnEnable()
    {
        entity = (Entity)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawInfo();
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

    private void DrawInfo()
    {
        Header("Information");
        entity.name = EditorGUILayout.TextField("Name", entity.name);
    }
    
}
