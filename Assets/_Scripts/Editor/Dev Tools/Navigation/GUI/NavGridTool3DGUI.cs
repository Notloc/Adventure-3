namespace Adventure.DevTools.Navigation
{
    using Adventure.Engine.Navigation;

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class NavGridTool3DGUI
    {
        readonly static Color SUBGRID_COLOR = new Color(0f, 0.2f, 1.0f, 0.8f);

        //Draws the given SubGrid and its contents
        public static void DrawSubGrid(NavGridTool tool)
        {
            SubGrid selectedSubGrid = tool.SelectedSubGrid;

            List<SubGrid> subGrids = selectedSubGrid.GetChildSubGrids();

            //If the current SubGrid has child SubGrids
            if (subGrids.Count > 0)
            {
                DrawSubGridHandle(selectedSubGrid, selectedSubGrid, SUBGRID_COLOR);

                for (int i = 0; i < subGrids.Count; i++)
                {
                    if (DrawSubGridHandle(subGrids[i], selectedSubGrid, SUBGRID_COLOR))
                    {
                        tool.SelectChildSubGrid(i);
                        return;
                    }
                }
                        
            }
            else
            {
                DrawNodeHandles(tool, selectedSubGrid);
            }
        }

        private static bool DrawSubGridHandle(SubGrid subGrid, SubGrid activeSubGrid, Color subGridColor)
        {

            Vector2i subGridOriginPosition = subGrid.GetOriginCoordinate();
            Vector3 subGridWorldPosition = subGrid.GetNavGrid().GetOriginWorldPosition() + new Vector3(subGridOriginPosition.x, 0, subGridOriginPosition.y);

            Vector3[] vertexs =
            {
            subGridWorldPosition,    //Bottom-Left
            subGridWorldPosition + new Vector3(subGrid.Width,0,0),  //Bottom-Right

            subGridWorldPosition + new Vector3(subGrid.Width,0,subGrid.Height),    //Top-Right
            subGridWorldPosition + new Vector3(0,0,subGrid.Height)  //Top-Left

        };

            Handles.DrawSolidRectangleWithOutline(vertexs, subGridColor, Color.black);

            if (subGrid.Equals(activeSubGrid))
            {
                return false;
            }

            float centerX = vertexs[0].x + ((vertexs[1].x - vertexs[0].x) / 2f);
            float centerZ = vertexs[0].z + ((vertexs[3].z - vertexs[0].z) / 2f);

            Vector3 centerPosition = new Vector3(centerX, vertexs[0].y, centerZ);

            float buttonSize = 6f;
            if (Handles.Button(centerPosition, Quaternion.LookRotation(Vector3.up), buttonSize, buttonSize, Handles.RectangleHandleCap))
            {
                return true;
            }
            return false;
        }

        private static void DrawNodeHandles(NavGridTool tool, SubGrid subGrid)
        {
            float NODE_SIZE = 0.45f;

            NavGrid navGrid = subGrid.GetNavGrid();

            Vector2i subGridOriginPosition = subGrid.GetOriginCoordinate();
            Vector3 subGridWorldPosition = navGrid.GetOriginWorldPosition() + new Vector3(subGridOriginPosition.x, 0, subGridOriginPosition.y);

            int width = subGrid.Width;
            int height = subGrid.Height;

            //Draw each node
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2i nodeCoordinates =
                                        new Vector2i(
                                            subGridOriginPosition.x + x,
                                            subGridOriginPosition.y + y
                                        );

                    Handles.color = tool.ChooseNodeColor(nodeCoordinates);

                    //Draw nodes as buttons and watch for input
                    if (Handles.Button(subGridWorldPosition + new Vector3(x, 0, y), Quaternion.LookRotation(Vector3.up), NODE_SIZE, NODE_SIZE, Handles.CubeHandleCap))
                        HandleNodeClick(subGrid, nodeCoordinates);
                }
            }

        }

        private static void HandleNodeClick(SubGrid subGrid, Vector2i nodeCoordinates)
        {
            //Get the active instance of NavGridTool and send it the NodeClick
            NavGridTool navGridTool = (EditorWindow.GetWindow(typeof(NavGridTool)) as NavGridTool);
            navGridTool.HandleNodeClick(nodeCoordinates);
        }

    }
}