namespace Adventure.Engine.Navigation
{
    using Navigation.Internal;
    using UnityEngine;

    using System.Collections;
    using System.Threading;

    public class NavGrid : MonoBehaviour
    {
        public enum Direction { NORTH, EAST, SOUTH, WEST };
        public readonly static Vector2Int NO_NODE = new Vector2Int(-1, -1);

        [SerializeField] Node[] nodes;
        public Node[] Nodes
        {
            get
            {
                return nodes;
            }
        }

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



        public bool HasWall(Direction direction, Vector3 position)
        {
            return HasWall(direction, WorldPointToNode(position));
        }
        public bool HasWall(Direction direction, Vector2Int nodeCoordinate)
        {
            if(direction == Direction.NORTH)
                return Node(nodeCoordinate).HasWallNorth();

            else if (direction == Direction.EAST)
                return Node(nodeCoordinate).HasWallEast();

            else if (direction == Direction.SOUTH)
                return Node(nodeCoordinate).HasWallSouth();

            else if (direction == Direction.WEST)
                return Node(nodeCoordinate).HasWallWest();

            return false;
        }




        public Node Node(Vector2Int coordinate)
        {
            if (!ValidNode(coordinate))
                return new Node();

            return Nodes[(coordinate.y * width) + coordinate.x];
        }


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

                case AnchorPoint.NORTH_WEST:
                    //X
                    xStart = 0;
                    xEnd = Mathf.Min(newWidth, width);

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

                case AnchorPoint.SOUTH_WEST:

                    xStart = 0;
                    xEnd = Mathf.Min(newWidth, width);

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
                    int newIndex = (newWidth * y) + x;
                    int oldIndex = (width * (y - yStart + yOffset)) + (x - xStart + xOffset);
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
        private bool ValidNode(Vector2Int nodeCoordinates)
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
        public bool IsPathable(Vector2Int nodeCoordinates)
        {
            if (!ValidNode(nodeCoordinates))
                return false;

            int nodeIndex = (nodeCoordinates.y * width) + nodeCoordinates.x;
            return nodes[nodeIndex].IsPathable();
        }

        //Toggles the pathablity a square of nodes, the corners being defined by Vector2is
        public void TogglePathablity(Vector2Int startingNode, Vector2Int endingNode)
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
                Vector2Int bottomLeft = new Vector2Int(Mathf.Min(startingNode.x, endingNode.x), Mathf.Min(startingNode.y, endingNode.y));
                Vector2Int topRight = new Vector2Int(Mathf.Max(startingNode.x, endingNode.x), Mathf.Max(startingNode.y, endingNode.y));

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

        public Vector2Int WorldPointToNode(Vector3 worldPoint)
        {
            int x = Mathf.RoundToInt(worldPoint.x - positionOffset.x);
            int y = Mathf.RoundToInt(worldPoint.z - positionOffset.z);

            if(x >= 0 && x < width    &&    y >= 0 && y < height)
            {
                return new Vector2Int(x,y);
            }
            return new Vector2Int(-1, -1);
        }

        public Vector3 NodeToWorldPoint(Vector2Int coordinate)
        {
            return positionOffset + new Vector3(coordinate.x, 0, coordinate.y);
        }

        public static NavGrid ClosestNavGrid(Vector3 position)
        {
            NavGrid[] navgrids = GameObject.FindObjectsOfType<NavGrid>();

            float lowestDistance = float.MaxValue;
            NavGrid closestGrid = null;
            foreach(NavGrid navgrid in navgrids)
            {
                float distance = Vector3.Distance(navgrid.transform.position, position);
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    closestGrid = navgrid;
                }
            }
            return closestGrid;
        }
    }
}