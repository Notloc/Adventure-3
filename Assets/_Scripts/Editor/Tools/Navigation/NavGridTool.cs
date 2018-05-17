using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// Tool that allows easy editing of NavGrids via 3D GUI in the sceneview and an editor window
public class NavGridTool : EditorWindow
{
// CONSTANTS
    //SubGrids
    readonly static Color SUBGRID_COLOR = new Color(0f, 0.2f, 1.0f);

    //Nodes
    readonly static Vector2i NO_NODE = new Vector2i(-1, -1);
    readonly static Color PATHABLE_NODE_COLOR = new Color(0f, 0.2f, 1f, 0.35f);
    readonly static Color UNPATHABLE_NODE_COLOR = new Color(1f, 0.2f, 0f, 0.35f);
    readonly static Color SELECTED_NODE_COLOR = new Color(0f, 1f, 0f, 0.35f);
// END CONSTANTS

// VARIABLES
    NavGrid currentNavGrid;
    SubGrid selectedSubGrid;
    Vector2i selectedNode = NO_NODE;
    int selectedTool = 0;
    int selectedTab = 0;
// END VARIABLES

// SETTINGS
    int MAX_HANDLES_PER_FRAME = 400;

// LIFE CYCLE
    //Creates and shows the window
    [MenuItem("Window/NavGrid Tool")]
    public static void CreateWindow()
    {
        NavGridTool tool = EditorWindow.GetWindow(typeof(NavGridTool), false, "NavGrid Tools") as NavGridTool;

        if (EditorPrefs.HasKey("MAX_HANDLES"))
            tool.MAX_HANDLES_PER_FRAME = EditorPrefs.GetInt("MAX_HANDLES");

        tool.OnSelectionChange();   //Manually call to handle a NavGrid being preselected
    }

    private void Awake()
    {
        if (EditorPrefs.HasKey("MAX_HANDLES"))
            MAX_HANDLES_PER_FRAME = EditorPrefs.GetInt("MAX_HANDLES");
    }

    //Fires when the active scene object changes
    private void OnSelectionChange()
    {
        currentNavGrid = GetSelectedNavGrid();

        if (currentNavGrid)
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;

            selectedSubGrid = SubGridTool.PrepareNavGridForDisplay(currentNavGrid, MAX_HANDLES_PER_FRAME);
        }
        else
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }
    }

    private NavGrid GetSelectedNavGrid()
    {
        GameObject selection = Selection.activeGameObject;
        if (selection)
        {
            NavGrid navGrid = selection.GetComponent<NavGrid>();
            if (navGrid)
            {
                return navGrid;
            }
        }

        return null;
    }

    //Unregisters from OnSceneGUI delegate
    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        EditorPrefs.SetInt("MAX_HANDLES", MAX_HANDLES_PER_FRAME);
    }
// END LIFE CYCLE


// SCENE GUI
    //Renders the selected SubGrid
    void OnSceneGUI(SceneView sceneView)
    {
        selectedSubGrid = SubGridTool.DrawSubGrid(selectedSubGrid, SUBGRID_COLOR);
    }
    // END SCENE GUI


    // WINDOW GUI
    private void OnGUI()
    {
        GUILayout.BeginVertical("box");
        {
            DrawTitle();
            DrawTabSelection();
        }
        GUILayout.EndVertical();

        GUILayout.Space(20);

        GUILayout.BeginVertical("box");
        {
            switch (selectedTab)
            {
                case 0:
                    DrawToolsMenu();
                    GUILayout.Space(15);
                    DrawNodeMenu();
                    break;

                case 1:
                    DrawSettingsMenu();
                    break;
            }
        }
        GUILayout.EndVertical();
    }

    private void DrawTitle()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("NAVGRID TOOLS", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
    }

    private void DrawTabSelection()
    {
            string[] tabNames = new string[] {"Tools", "Settings" };

            selectedTab = GUILayout.SelectionGrid(
                                        selectedTab,
                                        tabNames,
                                        2);
    }

    private void DrawToolsMenu()
    {

        GUILayout.Label("Tools", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal("box");
        {
            GUILayout.FlexibleSpace();

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

            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
 
    }

    private void DrawNodeMenu()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("Selected Node", EditorStyles.boldLabel);

            if (selectedNode.Equals(NO_NODE))
            {
                GUILayout.Label("No Node Selected");
            }
            else
            {
                DrawNodeControls();
            }

        }
        GUILayout.EndVertical();
    }

    private void DrawNodeControls()
    {
        GUILayout.Label("Node: (" + selectedNode.x + ", " + selectedNode.y + ")");

        if (GUILayout.Button("Parent SubGrid"))
        {
            SubGrid parentSubGrid = selectedSubGrid.GetParentSubGrid();
            if (parentSubGrid != null)
            {
                selectedSubGrid = parentSubGrid;
            }
        }

        if (GUILayout.Button("Open Grid Here"))
        {
            //hmm
        }
    }

    private void DrawSettingsMenu()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);

        MAX_HANDLES_PER_FRAME = EditorGUILayout.IntField("Node Limit", MAX_HANDLES_PER_FRAME);


        if (GUILayout.Button("Apply Now"))
        {
            if (currentNavGrid)
            {
                selectedSubGrid = SubGridTool.PrepareNavGridForDisplay(currentNavGrid, MAX_HANDLES_PER_FRAME);
            }
            EditorPrefs.SetInt("MAX_HANDLES", MAX_HANDLES_PER_FRAME);
        }

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
    public Color ChooseNodeColor(Vector2i nodeCoordinate)
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