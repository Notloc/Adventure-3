namespace Adventure.Game.Entities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(Entity))]
    public class EntityCI : Editor
    {
        Entity entity;

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
            GUILayout.Label("Information", EditorStyles.boldLabel);
            entity.name = EditorGUILayout.TextField("Name", entity.name);
        }

    }
}