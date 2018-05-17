using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Tool that handles creation and display of SubGrids
public static class SubGridTool
{
/* SUB GRID DISPLAY CODE
*      Code that handles displaying the selected SubGrid
*/
    //Draws the given SubGrid and its contents
    public static SubGrid DrawSubGrid(SubGrid selectedSubGrid, Color subGridColor)
    {
        List<SubGrid> subGrids = selectedSubGrid.GetChildSubGrids();

        //If the current SubGrid has child SubGrids
        if (subGrids.Count > 0)
        {
            DrawSubGridHandle(selectedSubGrid, selectedSubGrid, subGridColor);

            foreach (SubGrid subGrid in subGrids)
                if (DrawSubGridHandle(subGrid, selectedSubGrid, subGridColor))
                    return subGrid;
        }
        else
        {
            DrawNodeHandles(selectedSubGrid);
        }

        return selectedSubGrid;
    }

    private static bool DrawSubGridHandle(SubGrid subGrid, SubGrid activeSubGrid, Color subGridColor)
    {

        Vector2i subGridOriginPosition = subGrid.GetOriginCoordinate();
        Vector3 subGridWorldPosition = subGrid.GetNavGrid().GetWorldPosition() + new Vector3(subGridOriginPosition.x, 0, subGridOriginPosition.y);

        Vector3[] vertexs =
        {
            subGridWorldPosition,    //Bottom-Left
            subGridWorldPosition + new Vector3(subGrid.GetWidth(),0,0),  //Bottom-Right

            subGridWorldPosition + new Vector3(subGrid.GetWidth(),0,subGrid.GetHeight()),    //Top-Right
            subGridWorldPosition + new Vector3(0,0,subGrid.GetHeight())  //Top-Left

        };

        Handles.DrawSolidRectangleWithOutline(vertexs, subGridColor, Color.black);

        if(subGrid.Equals(activeSubGrid))
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

    private static void DrawNodeHandles(SubGrid subGrid)
    {
        float NODE_SIZE = 0.45f;

        NavGrid navGrid = subGrid.GetNavGrid();

        Vector2i subGridOriginPosition = subGrid.GetOriginCoordinate();
        Vector3 subGridWorldPosition = navGrid.GetWorldPosition() + new Vector3(subGridOriginPosition.x, 0, subGridOriginPosition.y);

        int width = subGrid.GetWidth();
        int height = subGrid.GetHeight();

        NavGridTool navGridTool = EditorWindow.GetWindow(typeof(NavGridTool)) as NavGridTool;
        if(!navGridTool)
            return;

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

                Handles.color = navGridTool.ChooseNodeColor(nodeCoordinates);

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
        navGridTool.HandleNodeClick(subGrid.GetNavGrid(), nodeCoordinates);
    }




/* SUB GRID CREATION CODE
*      Code that creates SubGrids from a NavGrid for display purposes
*/

    //Prepares the NavGrid for display by dividing it into smaller SubGrids
    //This is needed as drawing too many Handles at once slows down the editor
    public static SubGrid PrepareNavGridForDisplay(NavGrid navGrid, int maxHandlesPerFrame)
    {
        SubGrid primarySubGrid = new SubGrid(navGrid, null, new Vector2i(), navGrid.GetWidth(), navGrid.GetHeight());

        GenerateSubGrids(primarySubGrid, navGrid, maxHandlesPerFrame);

        return primarySubGrid;
    }

    //Recursively divides the provided subgrid into smaller subgrids until acceptable sizes are reached
    private static void GenerateSubGrids(SubGrid subGrid, NavGrid navGrid, int maxHandlesPerFrame)
    {
        //Determine how many sections we will be dividing into
        int numberOfSections = CalculateTargetNumberOfSections(subGrid, maxHandlesPerFrame);
        if (numberOfSections == 1)
            return;

        //Calculate how we will be dividing the grid, avoiding creating side lengths that are too small
        Vector2 gridDivisors = CalculateGridDivisions(subGrid, numberOfSections, maxHandlesPerFrame);

        //Divide the grid into SubGrids
        List<SubGrid> newSubGrids = DivideSubGrid(subGrid, navGrid, gridDivisors);

        //Divide each newly generated SubGrid
        foreach (SubGrid newSubGrid in newSubGrids)
            GenerateSubGrids(newSubGrid, navGrid, maxHandlesPerFrame);
    }

    //Calculate how many sections the grid should be divided into
    private static int CalculateTargetNumberOfSections(SubGrid subGrid, int maxHandlesPerFrame)
    {
        float targetAmount = (subGrid.GetWidth() * subGrid.GetHeight()) / (float)maxHandlesPerFrame;

        if (targetAmount <= 1f)
            return 1;

        if (targetAmount <= 2f)
            return 2;

        if (targetAmount <= 4)
            return 4;

        if (targetAmount <= 9f)
            return 9;

        if (targetAmount <= 16f)
            return 16;

        if (targetAmount <= 25f)
            return 25;

        return 36;
    }

    //Calculates how many times each side of the grid will be divided
    private static Vector2 CalculateGridDivisions(SubGrid subGrid, int targetNumberOfSections, int maxHandlesPerFrame)
    {
        int width = subGrid.GetWidth();
        int height = subGrid.GetHeight();

        //If we only need 2 sections, divide the longest side
        if (targetNumberOfSections == 2)
        {
            if (width > height)
            {
                return new Vector2(2, 1);
            }
            else
            {
                return new Vector2(1, 2);
            }
        }
        else
        {
            Vector2 gridDivisions = new Vector2();
            int divisor = Mathf.CeilToInt(Mathf.Sqrt(targetNumberOfSections));

            //Calculate the shortest side first
            if (width < height)
            {
                gridDivisions.x = CalculateSideDivisions(width, divisor, maxHandlesPerFrame);

                //Calculate how many divisions are needed for the other side
                if (gridDivisions.x != divisor)
                {
                    divisor = Mathf.CeilToInt((targetNumberOfSections / (float)gridDivisions.x));
                }
                gridDivisions.y = Mathf.CeilToInt(targetNumberOfSections / gridDivisions.x);
            }
            else
            {
                gridDivisions.y = CalculateSideDivisions(height, divisor, maxHandlesPerFrame);

                //Calculate how many divisions are needed for the other side
                if (gridDivisions.y != divisor)
                {
                    divisor = Mathf.CeilToInt((targetNumberOfSections / (float)gridDivisions.y));
                }
                gridDivisions.x = Mathf.CeilToInt(targetNumberOfSections / gridDivisions.y);
            }

            return gridDivisions;
        }
    }

    //Calculates how many times a side should be divided, ensuring the resulting slices are at least MIN_SECTION_SIZE
    private static int CalculateSideDivisions(int sideLength, int divisor, float maxHandlesPerFrame)
    {
        int divisions = 1;

        int MIN_SECTION_SIZE = Mathf.FloorToInt(Mathf.Sqrt(maxHandlesPerFrame));

        for (int i = divisor; i > 1; i--)
        {
            if ((sideLength / (float)i) >= MIN_SECTION_SIZE)
            {
                divisions = i;
                break;
            }
        }

        return divisions;
    }

    //Divides the provided subgrid using the provided devisions
    private static List<SubGrid> DivideSubGrid(SubGrid currentSubGrid, NavGrid navGrid, Vector2 divisions)
    {
        int width = currentSubGrid.GetWidth();
        int height = currentSubGrid.GetHeight();

        List<SubGrid> newSubGrids = new List<SubGrid>();

        float widthDivisions = divisions.x;
        float heightDivisions = divisions.y;

        Vector2i originCoordinate = currentSubGrid.GetOriginCoordinate();
        Vector2i currentCoordinate = originCoordinate;

        int xSize = Mathf.CeilToInt(width / widthDivisions);
        int ySize = Mathf.CeilToInt(height / heightDivisions);

        for (int y = 0; y < heightDivisions; y++)
        {
            //Correct ySize on the final row
            if (y == heightDivisions - 1)
                ySize = height - Mathf.RoundToInt(currentCoordinate.y - originCoordinate.y);

            currentCoordinate.x = originCoordinate.x;
            for (int x = 0; x < widthDivisions; x++)
            {
                SubGrid newSubGrid;

                //Correct xSize on the final column
                if (x == widthDivisions - 1)
                    newSubGrid = new SubGrid(navGrid, currentSubGrid, currentCoordinate, width - Mathf.RoundToInt(currentCoordinate.x - originCoordinate.x), ySize);
                else
                    newSubGrid = new SubGrid(navGrid, currentSubGrid, currentCoordinate, xSize, ySize);

                currentSubGrid.AddChildSubGrid(newSubGrid);
                newSubGrids.Add(newSubGrid);

                currentCoordinate.x += xSize;
            }

            currentCoordinate.y += ySize;
        }

        return newSubGrids;
    }
}
