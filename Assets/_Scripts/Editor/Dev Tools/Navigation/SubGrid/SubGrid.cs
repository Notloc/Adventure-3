namespace Adventure.DevTools.Navigation
{
    using Adventure.Engine.Navigation;
    using Adventure.Engine.Navigation.Internal;

    using System.Collections.Generic;
    using UnityEngine;

    public class SubGrid
    {
        NavGrid navGrid;

        Vector2Int originCoordinate;

        int width;
        public int Width
        {
            get
            {
                return width;
            }
        }
        int height;
        public int Height
        {
            get
            {
                return height;
            }
        }

        SubGrid parent;
        List<SubGrid> subGrids;

        public SubGrid(NavGrid navGrid, SubGrid parent, Vector2Int originCoordinate, int width, int height)
        {
            this.navGrid = navGrid;
            this.parent = parent;

            this.originCoordinate = originCoordinate;

            this.width = width;
            this.height = height;

            subGrids = new List<SubGrid>();
        }

        public NavGrid GetNavGrid()
        {
            return navGrid;
        }

        public SubGrid GetParentSubGrid()
        {
            return parent;
        }

        public void AddChildSubGrid(SubGrid newGrid)
        {
            subGrids.Add(newGrid);
        }

        public List<SubGrid> GetChildSubGrids()
        {
            return subGrids;
        }

        public Vector2Int GetOriginCoordinate()
        {
            return originCoordinate;
        }



    }
}