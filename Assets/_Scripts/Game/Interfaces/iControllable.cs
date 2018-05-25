namespace Adventure.Game
{
    using UnityEngine;
    using Adventure.Engine.Navigation;
    using Adventure.Game.Entities;

    public interface iControllable
    {
        LocationData LocationData
        {
            get;
        }

        void MoveTo(Vector2Int coordinate, NavGrid navgrid);
        void ReceivePath(Path path);
    }
}