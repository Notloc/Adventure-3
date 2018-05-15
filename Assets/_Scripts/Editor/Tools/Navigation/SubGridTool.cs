using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Tool that handles creation and display of SubGrids
public static class SubGridTool
{
    static int MAX_HANDLES_PER_FRAME = 400;
    static int MIN_SECTION_SIZE = 15;

    static Color SUBGRID_COLOR = new Color(0f, 0.3f, 1.0f, 0.3f);

/* SUB GRID DISPLAY CODE
*      Code that handles displaying the selected SubGrid
*/

    //Draws the given SubGrid and its contents
    public static SubGrid DrawSubGrid(SubGrid selectedSubGrid)
    {
        List<SubGrid> subGrids = selectedSubGrid.GetSubGrids();

        //If the current SubGrid has child SubGrids
        if (subGrids.Count > 0)
        {
            DrawSubGridHandle(selectedSubGrid, selectedSubGrid);

            foreach (SubGrid subGrid in subGrids)
                if (DrawSubGridHandle(subGrid, selectedSubGrid))
                    return subGrid;
        }
        else
        {
            DrawNodeHandles(selectedSubGrid);
        }

        return selectedSubGrid;
    }

    private static bool DrawSubGridHandle(SubGrid subGrid, SubGrid activeSubGrid)
    {

        Vector2 subGridLocalPosition = subGrid.GetPosition();
        Vector3 subGridWorldPosition = subGrid.GetNavGrid().GetWorldPosition() + new Vector3(subGridLocalPosition.x, 0, subGridLocalPosition.y);

        Vector3[] vertexs =
        {
            subGridWorldPosition,    //Bottom-Left
            subGridWorldPosition + new Vector3(subGrid.GetWidth(),0,0),  //Bottom-Right

            subGridWorldPosition + new Vector3(subGrid.GetWidth(),0,subGrid.GetHeight()),    //Top-Right
            subGridWorldPosition + new Vector3(0,0,subGrid.GetHeight())  //Top-Left

        };

        Handles.DrawSolidRectangleWithOutline(vertexs, SUBGRID_COLOR, Color.black);

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
        float buttonSize = 0.45f;

        NavGrid navGrid = subGrid.GetNavGrid();

        Vector2 subGridNodePosition = subGrid.GetPosition();
        Vector3 subGridWorldPosition = navGrid.GetWorldPosition() + new Vector3(subGridNodePosition.x, 0, subGridNodePosition.y);

        int width = subGrid.GetWidth();
        int height = subGrid.GetHeight();


        //Draw each node
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                
                Vector2i nodeCoordinate = new Vector2i(
                                            Mathf.RoundToInt(subGrid.GetPosition().x + x),
                                            Mathf.RoundToInt(subGrid.GetPosition().y + y)
                                        );

                Handles.color = NavGridTool.ChooseNodeColor(nodeCoordinate);
                
                if (Handles.Button(subGridWorldPosition + new Vector3(x, 0, y), Quaternion.LookRotation(Vector3.up), buttonSize, buttonSize, Handles.SphereHandleCap))
                {
                    HandleNodeClick(subGrid, x, y);
                }
            }
        }

    }

    private static void HandleNodeClick(SubGrid subGrid, int xPos, int yPos)
    {
        Vector2i nodeCoordinates = new Vector2i(Mathf.RoundToInt(subGrid.GetPosition().x + xPos),
                                                    Mathf.RoundToInt(subGrid.GetPosition().y + yPos));

        //This is a bit disgusting
        (EditorWindow.GetWindow(typeof(NavGridTool)) as NavGridTool).HandleNodeClick(subGrid.GetNavGrid(), nodeCoordinates);
    }




/* SUB GRID CREATION CODE
*      Code that creates SubGrids from a NavGrid for display purposes
*/

    //Prepares the NavGrid for display by dividing it into smaller SubGrids
    //This is needed as drawing too many Handles at once slows down the editor
    public static SubGrid PrepareNavGridForDisplay(NavGrid navGrid)
    {
        SubGrid primarySubGrid = new SubGrid(navGrid, new Vector2(), navGrid.GetWidth(), navGrid.GetHeight());

        GenerateSubGrids(primarySubGrid, navGrid);

        return primarySubGrid;
    }

    //Recursively divides the provided subgrid into smaller subgrids until acceptable sizes are reached
    private static void GenerateSubGrids(SubGrid subGrid, NavGrid navGrid)
    {
        //Determine how many sections we will be dividing into
        int numberOfSections = CalculateTargetNumberOfSections(subGrid);
        if (numberOfSections == 1)
            return;

        //Calculate how we will be dividing the grid, avoiding creating side lengths that are too small
        Vector2 gridDivisors = CalculateGridDivisions(subGrid, numberOfSections);

        //Divide the grid into SubGrids
        List<SubGrid> newSubGrids = DivideSubGrid(subGrid, navGrid, gridDivisors);

        //Divide each newly generated SubGrid
        foreach (SubGrid newSubGrid in newSubGrids)
            GenerateSubGrids(newSubGrid, navGrid);
    }

    //Calculate how many sections the grid should be divided into
    private static int CalculateTargetNumberOfSections(SubGrid subGrid)
    {
        float targetAmount = (subGrid.GetWidth() * subGrid.GetHeight()) / (float)MAX_HANDLES_PER_FRAME;

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
    private static Vector2 CalculateGridDivisions(SubGrid subGrid, int targetNumberOfSections)
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
                gridDivisions.x = CalculateSideDivisions(width, divisor);

                //Calculate how many divisions are needed for the other side
                if (gridDivisions.x != divisor)
                {
                    divisor = Mathf.CeilToInt((targetNumberOfSections / (float)gridDivisions.x));
                }
                gridDivisions.y = Mathf.CeilToInt(targetNumberOfSections / gridDivisions.x);
            }
            else
            {
                gridDivisions.y = CalculateSideDivisions(height, divisor);

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
    private static int CalculateSideDivisions(int sideLength, int divisor)
    {
        int divisions = 1;

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
    private static List<SubGrid> DivideSubGrid(SubGrid subGrid, NavGrid navGrid, Vector2 divisions)
    {
        int width = subGrid.GetWidth();
        int height = subGrid.GetHeight();

        List<SubGrid> newSubGrids = new List<SubGrid>();

        float widthDivisions = divisions.x;
        float heightDivisions = divisions.y;

        Vector2 startingPosition = subGrid.GetPosition();
        Vector2 currentPosition = startingPosition;

        int xSize = Mathf.CeilToInt(width / widthDivisions);
        int ySize = Mathf.CeilToInt(height / heightDivisions);

        for (int y = 0; y < heightDivisions; y++)
        {
            //Correct ySize on the final row
            if (y == heightDivisions - 1)
                ySize = height - Mathf.RoundToInt(currentPosition.y - startingPosition.y);

            currentPosition.x = startingPosition.x;
            for (int x = 0; x < widthDivisions; x++)
            {
                SubGrid newSubGrid;

                //Correct xSize on the final column
                if (x == widthDivisions - 1)
                    newSubGrid = new SubGrid(navGrid, currentPosition, width - Mathf.RoundToInt(currentPosition.x - startingPosition.x), ySize);
                else
                    newSubGrid = new SubGrid(navGrid, currentPosition, xSize, ySize);

                subGrid.AddSubGrid(newSubGrid);
                newSubGrids.Add(newSubGrid);

                currentPosition.x += xSize;
            }

            currentPosition.y += ySize;
        }

        return newSubGrids;
    }
}
