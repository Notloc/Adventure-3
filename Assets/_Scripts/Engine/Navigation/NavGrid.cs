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

        int nodeIndex = (nodeCoordinates.y * width) + nodeCoordinates.x;
        return nodes[nodeIndex].IsPathable();
    }

    //Toggles the pathablity a square of nodes, the corners being defined by Vector2is
    public void TogglePathablity(Vector2i startingNode, Vector2i endingNode)
    {
        if (!ValidNode(startingNode) || !ValidNode(endingNode))
            return;

        int startingNodeIndex = (startingNode.y * width) + startingNode.x;
        bool previousState = nodes[startingNodeIndex].IsPathable();

        //Toggle Single Node
        if (startingNode.Equals(endingNode))
        {
            nodes[startingNodeIndex].SetPathable(!previousState);
        }

        //Toggle Square of Nodes
        else
        {
            Vector2i bottomLeft = new Vector2i(Mathf.Min(startingNode.x, endingNode.x), Mathf.Min(startingNode.y, endingNode.y));
            Vector2i topRight = new Vector2i(Mathf.Max(startingNode.x, endingNode.x), Mathf.Max(startingNode.y, endingNode.y));

            for(int y = bottomLeft.y; y <= topRight.y; y++)
            {
                for (int x = bottomLeft.x; x <= topRight.x; x++)
                {
                    int nodeIndex = (y * width) + x;
                    nodes[nodeIndex].SetPathable(!previousState);
                }
            }
        }
    }
}
