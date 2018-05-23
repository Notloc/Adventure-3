namespace Adventure.DevTools.Navigation
{
    using Adventure.Engine.Navigation;
    using Adventure.Engine.Navigation.Internal;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    // Tool that allows easy editing of NavGrids via 3D GUI in the sceneview and an editor window
    public class NavGridTool : EditorWindow
    {
        //Constants
        readonly static Color PATHABLE_NODE_COLOR = new Color(0f, 0.2f, 1f, 0.35f);
        readonly static Color UNPATHABLE_NODE_COLOR = new Color(1f, 0.2f, 0f, 0.35f);
        readonly static Color SELECTED_NODE_COLOR = new Color(0f, 1f, 0f, 0.35f);
        readonly static Vector2i NO_NODE = new Vector2i(-1, -1);


        NavGrid currentNavGrid;

        SubGrid _selectedSubGrid;
        public SubGrid SelectedSubGrid
        {
            get
            {
                return _selectedSubGrid;
            }
        }

        Vector2i _selectedNode;
        public Vector2i SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                _selectedNode = value;
            }
        }

        int _selectedTool;
        public int SelectedTool
        {
            get
            {
                return _selectedTool;
            }
            set
            {
                _selectedTool = value;
            }
        }

        NavGridTool2DGUI gui2D;
        NavGridTool3DGUI gui3D;

        int _NODE_RENDER_LIMIT = 400;
        public int NODE_RENDER_LIMIT
        {
            get
            {
                return _NODE_RENDER_LIMIT;
            }
            set
            {
                _NODE_RENDER_LIMIT = value;
            }
        }


        //Creates the Window
        [MenuItem("Window/NavGrid Tool")]
        public static void CreateWindow()
        {
            EditorWindow.GetWindow(typeof(NavGridTool), false, "NavGrid Tools");
        }

        //Initializes Tool
        private void Awake()
        {
            gui2D = new NavGridTool2DGUI();
            gui3D = new NavGridTool3DGUI();

            if (EditorPrefs.HasKey("MAX_HANDLES"))
                NODE_RENDER_LIMIT = EditorPrefs.GetInt("MAX_HANDLES");

            OnSelectionChange();   //Manually call to handle a NavGrid being preselected
        }

        //Fires when the active scene object changes
        private void OnSelectionChange()
        {
            currentNavGrid = FindSelectedNavGrid();

            if (currentNavGrid)
            {
                SceneView.onSceneGUIDelegate -= OnSceneGUI;
                SceneView.onSceneGUIDelegate += OnSceneGUI;

                _selectedSubGrid = SubGridGenerator.CreateSubGrids(currentNavGrid, NODE_RENDER_LIMIT);
            }
            else
            {
                SceneView.onSceneGUIDelegate -= OnSceneGUI;
            }
        }

        //Retrieves the selected NavGrid if possible
        private NavGrid FindSelectedNavGrid()
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
        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            EditorPrefs.SetInt("MAX_HANDLES", NODE_RENDER_LIMIT);
        }

        //3D GUI
        private void OnSceneGUI(SceneView sceneView)
        {
            gui3D.DrawSubGrid(this);
        }

        //2D GUI
        private void OnGUI()
        {
            gui2D.Render2DGUI(this);
        }



        //Input Handling
        public void HandleNodeClick(Vector2i nodeCoordinates)
        {
            //SELECT
            if (SelectedTool == 0)
            {
                SelectedNode = nodeCoordinates;
            }

            //SINGLE
            if (SelectedTool == 1)
            {
                currentNavGrid.TogglePathablity(nodeCoordinates, nodeCoordinates);
            }

            //SQUARE
            if (SelectedTool == 2)
            {
                if (SelectedNode.Equals(NO_NODE))
                {
                    SelectedNode = nodeCoordinates;
                }
                else
                {
                    //Toggle entire squares of walkable area
                    currentNavGrid.TogglePathablity(SelectedNode, nodeCoordinates);
                    SelectedNode = NO_NODE;
                }
            }
        }

        public void UnselectNode()
        {
            SelectedNode = NO_NODE;
        }

        public void SelectParentSubGrid()
        {
            SubGrid parentSubGrid = _selectedSubGrid.GetParentSubGrid();
            if (parentSubGrid != null)
            {
                _selectedSubGrid = parentSubGrid;
            }
            SceneView.RepaintAll();
        }

        public void SelectChildSubGrid(int index)
        {
            if(index >= 0 && index < _selectedSubGrid.GetChildSubGrids().Count)
            {
                _selectedSubGrid = _selectedSubGrid.GetChildSubGrids()[index];
            }
            SceneView.RepaintAll();
        }

        public void CreateSubGridAtSelection()
        {
            if (SelectedNode.Equals(NO_NODE))
                return;

            Vector2i point = _selectedNode;
            point.x = Mathf.Clamp(point.x - (_selectedSubGrid.Width/2), 0, currentNavGrid.Width - 1);
            point.y = Mathf.Clamp(point.y - (_selectedSubGrid.Height/ 2), 0, currentNavGrid.Height - 1);

            _selectedSubGrid = SubGridGenerator.CreateSubGridAtPoint(currentNavGrid, _selectedSubGrid, point, _NODE_RENDER_LIMIT);
            SceneView.RepaintAll();
        }

        public void RegenerateSubGrids()
        {
            if (currentNavGrid)
            {
                _selectedSubGrid = SubGridGenerator.CreateSubGrids(currentNavGrid, NODE_RENDER_LIMIT);
                SceneView.RepaintAll();
            }
        }

        //Returns what color the given node should be
        public Color ChooseNodeColor(Vector2i nodeCoordinate)
        {
            if (nodeCoordinate.Equals(SelectedNode))
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
}