﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entity))]
public class EntityCI : EasyEditor
{
    Entity entity;

    SerializedProperty health;
    SerializedProperty stamina;
    SerializedProperty mana;

    SerializedProperty skills;

    private void OnEnable()
    {
        entity = (Entity)target;

        health = serializedObject.FindProperty("health");
        stamina = serializedObject.FindProperty("stamina");
        mana = serializedObject.FindProperty("mana");

        skills = serializedObject.FindProperty("skills");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawInfo();

        EditorGUILayout.Space();


        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI();
    }

    private void DrawInfo()
    {
        Header("Information");
        entity.name = EditorGUILayout.TextField("Name", entity.name);
    }
    
}
