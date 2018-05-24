namespace Adventure.DevTools.Navigation
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    using Adventure.Engine.Navigation;

    public class NavGridTool2DGUI
    {
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
                        dNodeControls(tool);
                        DrawModeControls(tool);
                        break;

                    //Settings
                    case 1:
                        DrawSettingsMenu(tool);
                        DrawModeControls(tool);
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

        private void dNodeControls(NavGridTool tool)
        {
            Vector2Int selectedNode = tool.SelectedNode;

            GUILayout.BeginVertical("box");
            {
                GUILayout.Label("Selected Node", EditorStyles.boldLabel);

                if (selectedNode.Equals(NavGrid.NO_NODE))
                {
                    GUILayout.Label("No Node Selected");
                }
                else
                {
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

            }
            GUILayout.EndVertical();
        }

        private void DrawModeControls(NavGridTool tool)
        {
            GUILayout.BeginHorizontal("box");
            {
                // Render Toggle
                if(NavGridTool3DGUI.currentMode == NavGridTool3DGUI.Mode.RENDER || NavGridTool3DGUI.currentMode == NavGridTool3DGUI.Mode.EDIT)
                {
                    if (GUILayout.Button("Disable Render"))
                    {
                        tool.GUI3D.SetMode(NavGridTool3DGUI.Mode.DISABLED);
                        tool.UnselectNode();
                    }
                }
                else
                {
                    if (GUILayout.Button("Enable Render"))
                        tool.GUI3D.SetMode(NavGridTool3DGUI.Mode.RENDER);
                }
                
                // View | Edit Mode
                if(NavGridTool3DGUI.currentMode == NavGridTool3DGUI.Mode.DISABLED)
                {
                    GUILayout.Button("...");
                }
                else
                {
                    if(NavGridTool3DGUI.currentMode == NavGridTool3DGUI.Mode.RENDER)
                    {
                        if (GUILayout.Button("Edit Mode ->"))
                        {
                            tool.GUI3D.SetMode(NavGridTool3DGUI.Mode.EDIT);
                            tool.RegenerateSubGrids();
                        }
                    }
                    else if(NavGridTool3DGUI.currentMode == NavGridTool3DGUI.Mode.EDIT)
                    {
                        if (GUILayout.Button("Render Mode ->"))
                        {
                            tool.GUI3D.SetMode(NavGridTool3DGUI.Mode.RENDER);
                            tool.UnselectNode();
                        }
                    }
                }


            }
            GUILayout.EndHorizontal();
        }


        private void DrawSettingsMenu(NavGridTool tool)
        {
            GUILayout.Label("Settings", EditorStyles.boldLabel);

            dNodeLimit(tool);
        }
        private void dNodeLimit(NavGridTool tool)
        {
            tool.NODE_RENDER_LIMIT = Mathf.Clamp(EditorGUILayout.IntField("Node Render Limit", tool.NODE_RENDER_LIMIT), 25, 10000);

            if (GUILayout.Button("Apply Now"))
            {
                tool.RegenerateSubGrids();
            }
        }

    }
}