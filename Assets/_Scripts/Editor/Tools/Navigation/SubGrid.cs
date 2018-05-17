using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubGrid
{
    NavGrid navGrid;

    Vector2i originCoordinate;
    int width, height;

    SubGrid parent;
    List<SubGrid> subGrids;

    public SubGrid(NavGrid navGrid, SubGrid parent, Vector2i originCoordinate, int width, int height)
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

    

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public Vector2i GetOriginCoordinate()
    {
        return originCoordinate;
    }

    

}
