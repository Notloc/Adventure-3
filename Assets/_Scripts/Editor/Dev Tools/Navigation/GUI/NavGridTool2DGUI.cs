namespace Adventure.DevTools.Navigation
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class NavGridTool2DGUI
    {
        readonly static Vector2i NO_NODE = new Vector2i(-1, -1);

        int selectedTab = 0;

        public void Render2DGUI(NavGridTool tool)
        {
            //Title and Tabs
            GUILayout.BeginVertical("box");
            {
                DrawTitle();
                DrawTabSelection();
            }
            GUILayout.EndVertical();
            GUILayout.Space(20);

            //Render Selected Tab
            GUILayout.BeginVertical("box");
            {
                switch (selectedTab)
                {
                    //Tools
                    case 0:
                        DrawToolsMenu(tool);
                        GUILayout.Space(15);
                        DrawNodeMenu(tool);
                        break;

                    //Settings
                    case 1:
                        DrawSettingsMenu(tool);
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
            string[] tabNames = new string[] { "Tools", "Settings" };

            selectedTab = GUILayout.SelectionGrid(
                                        selectedTab,
                                        tabNames,
                                        2);
        }


        //NODE TOOLS
        private void DrawToolsMenu(NavGridTool tool)
        {
            int selectedTool = tool.SelectedTool;

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
                    tool.SelectedTool = newTool;
                    tool.UnselectNode();
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

        }

        private void DrawNodeMenu(NavGridTool tool)
        {
            Vector2i selectedNode = tool.SelectedNode;

            GUILayout.BeginVertical("box");
            {
                GUILayout.Label("Selected Node", EditorStyles.boldLabel);

                if (selectedNode.Equals(NO_NODE))
                {
                    GUILayout.Label("No Node Selected");
                }
                else
                {
                    DrawNodeControls(tool);
                }

            }
            GUILayout.EndVertical();
        }

        private void DrawNodeControls(NavGridTool tool)
        {
            Vector2i selectedNode = tool.SelectedNode;

            GUILayout.Label("Node: (" + selectedNode.x + ", " + selectedNode.y + ")");

            if (GUILayout.Button("Parent SubGrid"))
            {
                tool.SelectParentSubGrid();
            }

            if (GUILayout.Button("Open Grid Here"))
            {
                tool.CreateSubGridAtSelection();
            }
        }
        //END NODE TOOLS


        //SETTINGS
        private void DrawSettingsMenu(NavGridTool tool)
        {
            GUILayout.Label("Settings", EditorStyles.boldLabel);

            DrawNodeLimit(tool);
        }

        private void DrawNodeLimit(NavGridTool tool)
        {
            tool.NODE_RENDER_LIMIT = Mathf.Clamp(EditorGUILayout.IntField("Node Render Limit", tool.NODE_RENDER_LIMIT), 25, 10000);

            if (GUILayout.Button("Apply Now"))
            {
                tool.RegenerateSubGrids();
            }
        }
        //END SETTINGS

    }
}