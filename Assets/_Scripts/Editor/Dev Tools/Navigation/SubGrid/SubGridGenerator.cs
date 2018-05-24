namespace Adventure.DevTools.Navigation
{
    using Adventure.Engine.Navigation;
    using Adventure.Engine.Navigation.Internal;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    // Tool that handles the creation of SubGrids
    public static class SubGridGenerator
    {
        //Splits the NavGrid into SubGrids for display purposes
        //This is needed as drawing too many Handles at once slows down the editor
        public static SubGrid CreateSubGrids(NavGrid navGrid, int nodeRenderLimit)
        {
            SubGrid primarySubGrid = new SubGrid(navGrid, null, new Vector2Int(), navGrid.Width, navGrid.Height);

            GenerateSubGrids(primarySubGrid, navGrid, nodeRenderLimit);

            return primarySubGrid;
        }

        public static SubGrid CreateSubGridAtPoint(NavGrid navgrid, SubGrid currentSubgrid, Vector2Int point, int nodeRenderLimit)
        {
            
            int width = Mathf.FloorToInt(Mathf.Sqrt(nodeRenderLimit));
            int height = width;

            if(navgrid.Width < point.x + width)
            {
                width = navgrid.Width - point.x;
            }
            if(navgrid.Height < point.y + height)
            {
                height = navgrid.Height - point.y;
            }

            SubGrid newSubGrid = new SubGrid(navgrid, currentSubgrid, point, width, height);

            return newSubGrid;
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
            float targetAmount = (subGrid.Width * subGrid.Height) / (float)maxHandlesPerFrame;

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
            int width = subGrid.Width;
            int height = subGrid.Height;

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
            int width = currentSubGrid.Width;
            int height = currentSubGrid.Height;

            List<SubGrid> newSubGrids = new List<SubGrid>();

            float widthDivisions = divisions.x;
            float heightDivisions = divisions.y;

            Vector2Int originCoordinate = currentSubGrid.GetOriginCoordinate();
            Vector2Int currentCoordinate = originCoordinate;

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
}