using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* A structure thats contains a piece of a navGrid
* As well as possibly more SubGrids
*/
public struct SubGrid
{
    NavGrid navGrid;

    Vector2 localStartingPoint;
    int width, height;

    List<SubGrid> subGrids;

    public SubGrid(NavGrid navGrid, Vector2 startingPoint, int width, int height)
    {
        this.navGrid = navGrid;

        localStartingPoint = new Vector2(Mathf.RoundToInt(startingPoint.x), Mathf.RoundToInt(startingPoint.y));

        this.width = width;
        this.height = height;

        subGrids = new List<SubGrid>();
    }

    public void AddSubGrid(SubGrid newGrid)
    {
        subGrids.Add(newGrid);
    }

    public List<SubGrid> GetSubGrids()
    {
        return subGrids;
    }


    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public Vector2 GetPosition()
    {
        return localStartingPoint;
    }

    public NavGrid GetNavGrid()
    {
        return navGrid;
    }

}
