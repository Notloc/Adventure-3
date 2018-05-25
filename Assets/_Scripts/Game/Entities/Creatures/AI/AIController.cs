namespace Adventure.Game.Entities.AI
{
    using UnityEngine;
    using Adventure.Engine.Navigation;

    public abstract class AIController : MonoBehaviour
    {
        protected iControllable character;
        protected PathFinder pathFinder;

        private void Start()
        {
            character = this.GetComponent<iControllable>();
            pathFinder = new PathFinder();
        }
    }
}