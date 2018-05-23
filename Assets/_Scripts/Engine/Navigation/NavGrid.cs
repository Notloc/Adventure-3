namespace Adventure.Engine.Navigation
{
    using Navigation.Internal;
    using UnityEngine;

    using System.Collections;
    using System.Threading;

    public class NavGrid : MonoBehaviour
    {
        [SerializeField] Node[] nodes;

        [SerializeField] int width;
        public int Width
        {
            get
            {
                return width;
            }
        }

        [SerializeField] int height;
        public int Height
        {
            get
            {
                return height;
            }
        }

        [SerializeField] Vector3 positionOffset;

        //Creates the NavGrid's nodes
        public void ResizeGrid(int newWidth, int newHeight, AnchorPoint anchor)
        {
            //Return if the inputs are invalid
            if (newWidth < 0 || newHeight < 0)
            {
                return;
            }

            //If there is no existing grid, just make a new empty one and return
            if (width == 0 || height == 0)
            {
                width = newWidth;
                height = newHeight;
                nodes = new Node[width * height];
                return;
            }



            //Create the new empty grid
            Node[] newNodes = new Node[newWidth * newHeight];

            int xStart=0, xEnd=0;
            int yStart=0, yEnd=0;

            int xOffset = 0, yOffset = 0;
            int xMovement = 0, yMovement = 0;

            //Calculate where to insert the data into the new grid based on the selected anchor
            switch(anchor)
            {
                case AnchorPoint.NORTH_EAST:
                    //X
                    xStart = 0;
                    xEnd = Mathf.Min(newWidth, width);

                    //Y
                    if(newHeight >= height)
                    {
                        yStart = newHeight - height;
                        yEnd = newHeight;
                        yMovement = -yStart;
                    }
                    else
                    {
                        yStart = 0;
                        yEnd = newHeight;
                        yOffset = height - newHeight;
                        yMovement = yOffset;
                    }
                    break;

                case AnchorPoint.NORTH_WEST:
                    //X
                    if (newWidth >= width)
                    {
                        xStart = newWidth - width;
                        xEnd = newWidth;
                        xMovement = -xStart;
                    }
                    else
                    {
                        xStart = 0;
                        xEnd = newWidth;
                        xOffset = width - newWidth;
                        xMovement = xOffset;
                    }

                    //Y
                    if (newHeight >= height)
                    {
                        yStart = newHeight - height;
                        yEnd = newHeight;
                        yMovement = -yStart;
                    }
                    else
                    {
                        yStart = 0;
                        yEnd = newHeight;
                        yOffset = height - newHeight;
                        yMovement = yOffset;
                    }
                    break;

                case AnchorPoint.SOUTH_EAST:
                    xStart = 0;
                    xEnd = Mathf.Min(newWidth, width);

                    yStart = 0;
                    yEnd = Mathf.Min(newHeight, height);
                    break;

                case AnchorPoint.SOUTH_WEST:
                    //X
                    if (newWidth >= width)
                    {
                        xStart = newWidth - width;
                        xEnd = newWidth;
                        xMovement = -xStart;
                    }
                    else
                    {
                        xStart = 0;
                        xEnd = newWidth;
                        xOffset = width - newWidth;
                        xMovement = xOffset;
                    }

                    yStart = 0;
                    yEnd = Mathf.Min(newHeight, height);
                    break;

                default:
                    return;
            }

            //Insert old data into the new grid
            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = xStart; x < xEnd; x++)
                {
                    int newIndex = (newHeight * y) + x;
                    int oldIndex = (height * (y - yStart + yOffset)) + (x - xStart + xOffset);

                    newNodes[newIndex] = nodes[oldIndex];
                }
            }

            //Update variables
            nodes = newNodes;
            width = newWidth;
            height = newHeight;

            positionOffset += new Vector3(xMovement, 0, yMovement);
        }

        //Gets the navgrids origin position in 3D space
        public Vector3 GetOriginWorldPosition()
        {
            return this.transform.position + positionOffset;
        }

        //Checks if a nodes coordinates are valid for the NavGrid
        private bool ValidNode(Vector2i nodeCoordinates)
        {
            if (nodeCoordinates.x < 0 || nodeCoordinates.x >= width)
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

                for (int y = bottomLeft.y; y <= topRight.y; y++)
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
}