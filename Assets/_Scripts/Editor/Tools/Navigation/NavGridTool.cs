using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// Tool that allows easy editing of NavGrids via 3D GUI in the sceneview
[InitializeOnLoad]
public class NavGridTool : Editor
{
    static NavGrid currentNavGrid;
    static SubGrid selectedSubGrid;

    static int selectedTool = 0;


    //Life Cycle Code
    static NavGridTool()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }


    //Called once per Scene Frame
    static void OnSceneGUI(SceneView sceneView)
    {
        if(NavGridSelected() == false)
            return;

        //Draw
        selectedSubGrid = SubGridTool.DrawSubGrid(selectedSubGrid);
        DrawToolsMenu(sceneView.position);
    }

    private static bool NavGridSelected()
    {
        GameObject selection = Selection.activeGameObject;
        if(selection)
        {
            NavGrid navGrid = selection.GetComponent<NavGrid>();
            if (navGrid)
            {
                if(currentNavGrid != navGrid)
                {
                    currentNavGrid = navGrid;
                    selectedSubGrid = SubGridTool.PrepareNavGridForDisplay(currentNavGrid);
                }
                return true;
            }
        }

        return false;
    }


    static void DrawToolsMenu(Rect position)
    {
        Handles.BeginGUI();
        {
            GUILayout.BeginArea(new Rect(0, position.height - 35, position.width, 20), EditorStyles.toolbar);
            {
                string[] toolLabels = new string[] { "Select", "Single", "Square", "Wall Mode" };

                selectedTool = GUILayout.SelectionGrid(
                    selectedTool,
                    toolLabels,
                    4,
                    EditorStyles.toolbarButton,
                    GUILayout.Width(300));
            }
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }

    public static void HandleNodeClick(NavGrid navGrid, int x, int y)
    {
        if (selectedTool == 0)
        {
            return;
        }

        if (selectedTool == 1)
        {
            navGrid.ToggleNodes(new Vector2i(x,y), new Vector2i(x,y));
        }

        if(selectedTool == 2)
        {
            //Toggle entire squares of walkable area

        }

        if(selectedTool == 3)
        {
            //Toggle sides of squares for walkable area
        }
    }

}