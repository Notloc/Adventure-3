namespace Adventure.Engine.Navigation
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Path
    {
        Stack<Vector2Int> directions = new Stack<Vector2Int>();

        public void Add(Vector2Int coordinate)
        {
            directions.Push(coordinate);
        }

        public bool HasDirections()
        {
            return directions.Count > 0;
        }

        public Vector2Int Next()
        {
            return directions.Pop();
        }
    }
}