namespace Adventure.Engine.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Diagnostics;

    using UnityEngine;
    using Adventure.Game.Entities;
    using Navigation.Internal;

    public class PathFinder
    {
        Thread pathfindingThread;
        uint activeThreadID = 0;

        public void BeginPathFinding(LocationData locationData, Vector2Int targetNode, Action<Path> CALLBACK)
        {
            //Update thread ID
            activeThreadID++;
            if(activeThreadID == uint.MaxValue)
                activeThreadID = 0;

            //Create and start the pathfinding thread
            pathfindingThread = new Thread(() => PathFindingThread(activeThreadID, locationData, targetNode, CALLBACK));
            pathfindingThread.Start();
        }

        //The method used by the pathfinding thread, calls back with the finished path when ready if no other threads are started first
        private void PathFindingThread(uint activeThreadID, LocationData locationData,  Vector2Int targetNode, Action<Path> CALLBACK)
        {
            NavGrid navgrid = locationData.navgrid;

            bool foundPath = false;

            // A* PATHFINDING IMPLEMENTATION

            List<PathingNode> openNodes = new List<PathingNode>();
            HashSet<PathingNode> closedNodes = new HashSet<PathingNode>();
            PathingNode selectedNode = null;

            openNodes.Add(new PathingNode(locationData.coordinates, null, 0, CalculateHCost(locationData.coordinates, targetNode)));

            while(openNodes.Count > 0)
            {
                selectedNode = SelectLowestCostNode(openNodes);
                openNodes.Remove(selectedNode);
                closedNodes.Add(selectedNode);

                if(selectedNode.coordinate == targetNode)
                {
                    foundPath = true;
                    break;
                }
                    
                //Check each surrounding node
                for(int i = 0; i < 4; i++)
                {
                    //Setup direction
                    NavGrid.Direction direction = NavGrid.Direction.NORTH;
                    Vector2Int offset = new Vector2Int();
                    switch(i)
                    {
                        case 0:
                            direction = NavGrid.Direction.NORTH;
                            offset = new Vector2Int(0, 1);
                            break;

                        case 1:
                            direction = NavGrid.Direction.EAST;
                            offset = new Vector2Int(1, 0);
                            break;

                        case 2:
                            direction = NavGrid.Direction.SOUTH;
                            offset = new Vector2Int(0, -1);
                            break;

                        case 3:
                            direction = NavGrid.Direction.WEST;
                            offset = new Vector2Int(-1, 0);
                            break;
                    }

                    //Is there is no wall
                    if (!navgrid.HasWall(direction, selectedNode.coordinate))
                    {
                        Vector2Int newCoordinate = selectedNode.coordinate + offset;
                        PathingNode newPathNode = new PathingNode(newCoordinate, selectedNode);

                        //Make sure it isn't in the closed list and Check for pathability
                        if (!closedNodes.Contains(newPathNode) && navgrid.IsPathable(newCoordinate))
                        {
                            //Compute score
                            newPathNode.gCost = selectedNode.gCost;
                            newPathNode.hCost = CalculateHCost(newCoordinate, targetNode);

                            //Add to open list if its not already in it
                            if(!openNodes.Contains(newPathNode))
                            {
                                openNodes.Add(newPathNode);
                            }
                            //If it is, modify it if the new version has a lower gcost
                            else
                            {
                                int index = openNodes.IndexOf(newPathNode);
                                if (newPathNode.gCost < openNodes[index].gCost)
                                {
                                    openNodes.Remove(newPathNode);
                                    openNodes.Add(newPathNode);
                                }
                            }
                            
                        }
                    }
                }

            }

            if (!foundPath)
                return;
            
            //Construct and send the path
            Path path = ConstructPath(selectedNode);
            if (this.activeThreadID == activeThreadID)
                CALLBACK(path);
        }

        private Path ConstructPath(PathingNode node)
        {
            //UnityEngine.Debug.Log(node);
            Path path = new Path();

            while (node.parentNode != null)
            {
                path.Add(node.coordinate);
                node = node.parentNode;
            }
            path.Add(node.coordinate);

            return path;
        }

        private int CalculateHCost(Vector2Int point1, Vector2Int point2)
        {
            return Mathf.Abs(point1.x - point2.x) + Mathf.Abs(point1.y - point2.y);
        }

        private PathingNode SelectLowestCostNode(List<PathingNode> openNodes)
        {
            PathingNode lowestCostNode = openNodes[0];
            for(int i = 1; i < openNodes.Count; i++)
            {
                if(openNodes[i].fCost < lowestCostNode.fCost || openNodes[i].fCost == lowestCostNode.fCost && openNodes[i].hCost < lowestCostNode.hCost)
                {
                    lowestCostNode = openNodes[i];
                }
            }
            return lowestCostNode;
        }

        private class PathingNode
        {
            public PathingNode(Vector2Int coordinate, PathingNode parentNode)
            {
                this.parentNode = parentNode;
                this.coordinate = coordinate;
            }

            public PathingNode(Vector2Int nodeCoordinate, PathingNode parentNode, int gCost, int hCost)
            {
                this.parentNode = parentNode;
                coordinate = nodeCoordinate;
                this.hCost = hCost;
                this.gCost = gCost;
            }

            public PathingNode parentNode;
            public Vector2Int coordinate;
            public int hCost;
            public int gCost;
            public int fCost
            {
                get
                {
                    return gCost + hCost;
                }
            }

            public override bool Equals(object obj)
            {
                return ((PathingNode)obj).coordinate == this.coordinate;
            }

            public override int GetHashCode()
            {
                return this.coordinate.GetHashCode();
            }

            public override string ToString()
            {
                if (parentNode != null)
                {
                    return coordinate.ToString() + " => " + parentNode.ToString();
                }
                else
                {
                    return coordinate.ToString();
                }
            }
        }
    }
}