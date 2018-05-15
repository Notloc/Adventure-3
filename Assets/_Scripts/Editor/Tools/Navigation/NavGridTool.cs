using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// Tool that allows easy editing of NavGrids via 3D GUI in the sceneview and an editor window
public class NavGridTool : EditorWindow
{
// CONSTANTS
    readonly static Vector2i NO_NODE = new Vector2i(-1, -1);
    readonly static Color PATHABLE_NODE_COLOR = new Color(0f, 0.2f, 1f, 0.35f);
    readonly static Color UNPATHABLE_NODE_COLOR = new Color(1f, 0.2f, 0f, 0.35f);
    readonly static Color SELECTED_NODE_COLOR = new Color(0f, 1f, 0f, 0.35f);
// END CONSTANTS


// VARIABLES
    static NavGrid currentNavGrid;
    static SubGrid selectedSubGrid;
    static Vector2i selectedNode = NO_NODE;
    int selectedTool = 0;
    
// END VARIABLES

// LIFE CYCLE
    //Creates and shows the window, also registers for the OnSceneGUI delegate
    [MenuItem("Window/NavGrid Tool")]
    public static void ShowWindow()
    {
        ResetVariables();

        EditorWindow.GetWindow(typeof(NavGridTool));
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    //Ensures all static globals are reset properly
    private static void ResetVariables()
    {
        currentNavGrid = null;
        selectedSubGrid = new SubGrid();
        selectedNode = NO_NODE;
    }

    //Unregisters from OnSceneGUI delegate
    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        Debug.Log("Goodbye");
    }
// END LIFE CYCLE


// SCENE GUI
    static void OnSceneGUI(SceneView sceneView)
    {
        //Only draw in the scene when a NavGrid is selected
        if(IsNavGridSelected() == false)
            return;

        selectedSubGrid = SubGridTool.DrawSubGrid(selectedSubGrid);
    }

    private static bool IsNavGridSelected()
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
// END SCENE GUI


// WINDOW GUI
    private void OnGUI()
    {
        DrawToolsMenu();
        GUILayout.Space(30);
        DrawNodeMenu();
    }

    private void DrawToolsMenu()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("Tools", EditorStyles.boldLabel);

            string[] toolLabels = new string[] { "Select", "Single", "Square", "Wall Mode" };

            int newTool = GUILayout.SelectionGrid(
                selectedTool,
                toolLabels,
                4,
                EditorStyles.toolbarButton,
                GUILayout.Width(300));

            if (newTool != selectedTool)
            {
                selectedTool = newTool;
                selectedNode = NO_NODE;
            }

        }
        GUILayout.EndVertical();

        
    }

    private void DrawNodeMenu()
    {
        GUILayout.BeginVertical("box");
        {
            if (selectedNode.Equals(NO_NODE))
            {
                GUILayout.Label("No Node Selected");
            }
            else
            {
                GUILayout.Label("Node: (" + selectedNode.x + ", " + selectedNode.y + ")");

            }
        }
        GUILayout.EndVertical();


    }
// END WINDOW GUI


// OTHER
    public void HandleNodeClick(NavGrid navGrid, Vector2i nodeCoordinates)
    {
        //SELECT
        if (selectedTool == 0)
        {
            selectedNode = nodeCoordinates;
        }

        //SINGLE
        if (selectedTool == 1)
        {
            navGrid.TogglePathablity(nodeCoordinates, nodeCoordinates);
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
                navGrid.TogglePathablity(selectedNode, nodeCoordinates);
                selectedNode = NO_NODE;
            }
        }

        //Wall Mode
        if(selectedTool == 3)
        {
            //Soon TM
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
// END OTHER
}