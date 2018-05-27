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

        void Interact(Interaction interaction);

        void Move(Vector2Int coordinate);
        void Move(Path path);
    }
}