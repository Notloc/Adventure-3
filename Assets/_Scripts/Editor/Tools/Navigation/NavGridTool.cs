using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// Tool that allows easy editing of NavGrids via 3D GUI in the sceneview
[InitializeOnLoad]
public class NavGridTool : Editor
{
    static Vector2i NO_NODE = new Vector2i(-1, -1);

    static Color PATHABLE_NODE_COLOR = new Color(0f, 0.2f, 1f, 0.35f);
    static Color UNPATHABLE_NODE_COLOR = new Color(1f, 0.2f, 0f, 0.35f);
    static Color SELECTED_NODE_COLOR = new Color(0f, 1f, 0f, 0.35f);

    static NavGrid currentNavGrid;
    static SubGrid selectedSubGrid;

    static int selectedTool = 0;

    static Vector2i selectedNode = NO_NODE;

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

                int newTool = GUILayout.SelectionGrid(
                    selectedTool,
                    toolLabels,
                    4,
                    EditorStyles.toolbarButton,
                    GUILayout.Width(300));

                if(newTool != selectedTool)
                {
                    selectedTool = newTool;
                    selectedNode = NO_NODE;
                }

            }
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }

    public static void HandleNodeClick(NavGrid navGrid, Vector2i nodeCoordinates)
    {
        //SELECT
        if (selectedTool == 0)
        {
            selectedNode = nodeCoordinates;
        }

        //SINGLE
        if (selectedTool == 1)
        {
            navGrid.ToggleNodes(nodeCoordinates, nodeCoordinates);
        }

        //SQUARE
        if(selectedTool == 2)
        {
            if (selectedNode.Equals(NO_NODE))
            {
                selectedNode = nodeCoordinates;
            }
            else
            {
                //Toggle entire squares of walkable area
                navGrid.ToggleNodes(selectedNode, nodeCoordinates);
                selectedNode = NO_NODE;
            }
        }

        if(selectedTool == 3)
        {
            //Toggle sides of squares for walkable area
        }
    }

    //Returns what color the given node should be
    public static Color ChooseNodeColor(Vector2i nodeCoordinate)
    {
        if (nodeCoordinate.Equals(selectedNode))
        {
            return SELECTED_NODE_COLOR;
        }

        //Node color is based on if the node is pathable
        if (currentNavGrid.IsPathable(nodeCoordinate))
        {
            return PATHABLE_NODE_COLOR;
        }
        else
        {
            return UNPATHABLE_NODE_COLOR;
        }
    }

}