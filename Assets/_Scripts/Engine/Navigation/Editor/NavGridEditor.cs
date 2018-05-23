namespace Adventure.Engine.Navigation.Internal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(NavGrid))]
    public class NavGridEditor : Editor
    {
        private NavGrid navGrid;

        SerializedProperty nodesProp, widthProp, heightProp, offsetProp;

        //Resize Variables
        int newWidth;
        int newHeight;

        int selectedAnchorPoint;

        private void OnEnable()
        {
            navGrid = target as NavGrid;

            //Get properties
            nodesProp = serializedObject.FindProperty("nodes");
            widthProp = serializedObject.FindProperty("width");
            heightProp = serializedObject.FindProperty("height");
            offsetProp = serializedObject.FindProperty("positionOffset");

            newWidth = widthProp.intValue;
            newHeight = heightProp.intValue;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Width: " + widthProp.intValue);
            EditorGUILayout.LabelField("Height: " + heightProp.intValue);
            EditorGUILayout.PropertyField(offsetProp);

            DrawResizeControls();

            serializedObject.ApplyModifiedProperties();
            //base.OnInspectorGUI();
        }

        private void DrawResizeControls()
        {
            GUILayout.BeginVertical("box");
            {
                if (GUILayout.Button("Resize Grid"))
                {
                    ResizeGrid();
                }

                newWidth = EditorGUILayout.IntField("New Width", newWidth);
                newHeight = EditorGUILayout.IntField("New Height", newHeight);

                //Anchor selection
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("Anchor Point");

                    string[] labels = { "North-East", "North-West", "South-East", "South-West" };
                    selectedAnchorPoint = GUILayout.SelectionGrid(selectedAnchorPoint, labels, 4, "toggle");
                }
                GUILayout.EndVertical();

            }
            GUILayout.EndVertical();
        }

        private void ResizeGrid()
        {
            AnchorPoint anchorPoint = AnchorPoint.NORTH_EAST;
            switch (selectedAnchorPoint)
            {
                case 0:
                    anchorPoint = AnchorPoint.NORTH_EAST;
                    break;

                case 1:
                    anchorPoint = AnchorPoint.NORTH_WEST;
                    break;

                case 2:
                    anchorPoint = AnchorPoint.SOUTH_EAST;
                    break;

                case 3:
                    anchorPoint = AnchorPoint.SOUTH_WEST;
                    break;

            }

            navGrid.ResizeGrid(newWidth, newHeight, anchorPoint);
        }
    }
}