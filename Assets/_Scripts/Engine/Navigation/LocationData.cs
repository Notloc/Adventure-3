namespace Adventure.Engine.Navigation
{
    using UnityEngine;

    [System.Serializable]
    public struct LocationData
    {
        public NavGrid navgrid;
        public Vector2Int coordinates;
    }
}