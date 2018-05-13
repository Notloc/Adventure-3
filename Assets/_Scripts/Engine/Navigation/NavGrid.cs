using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class NavGrid : MonoBehaviour
{
    [SerializeField] Node[] nodes;
    [SerializeField] int width;
    [SerializeField] int height;

    [SerializeField] Vector3 positionOffset;

    //Creates the NavGrid's nodes
    public void GenerateNodes()
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        nodes = new Node[width * height];

    }

//ACCESSORS
    //Gets the navgrids origin position in 3D space
    public Vector3 GetWorldPosition()
    {
        return this.transform.position + positionOffset;
    }

    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }


 //NODE METHODS
    //Checks if a nodes coordinates are valid for the NavGrid
    private bool ValidNode(Vector2i nodeCoordinates)
    {
        if(nodeCoordinates.x < 0 || nodeCoordinates.x >= width)
        {
            return false;
        }

        if (nodeCoordinates.y < 0 || nodeCoordinates.y >= height)
        {
            return false;
        }

        return true;
    }
    
    //Checks if a node is pathable
    public bool IsPathable(Vector2i nodeCoordinates)
    {
        if (!ValidNode(nodeCoordinates))
            return false;

        return nodes[(nodeCoordinates.y * width) + nodeCoordinates.x].IsPathable();
    }

    //Toggles a square of nodes, corners of the square are defined by vector2s
    public void ToggleNodes(Vector2i startingNode, Vector2i endingNode)
    {
        if (!ValidNode(startingNode) || !ValidNode(endingNode))
            return;

        int startingNodeIndex = (startingNode.y * width) + startingNode.x;
        bool previousState = nodes[startingNodeIndex].IsPathable();

        if (startingNode.Equals(endingNode))
        {
            Debug.Log(startingNode);
            nodes[startingNodeIndex].SetPathable(!previousState);
        }
    }
}
