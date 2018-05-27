namespace Adventure.Game.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using Adventure.Engine.Navigation;

    [RequireComponent(typeof(Entity))]
    public abstract class Interaction : MonoBehaviour
    {
        [SerializeField] [Range(0, 12)] int activationRange = 1;

        PathFinder pathFinder = new PathFinder();

        public abstract void Interact();
        public virtual bool InRange(LocationData userLocationData)
        {
            Vector2Int interactionNode = this.GetComponent<Entity>().LocationData.coordinates;

            if (NavGrid.Distance(interactionNode, userLocationData.coordinates) > activationRange)
                return false;

            return true;

        }
        public virtual void GeneratePath(LocationData userLocationData, Action<Path> CallBack)
        {
            Path path = new Path();

            pathFinder.BeginPathFinding(userLocationData, this.GetComponent<Entity>().LocationData.coordinates, CallBack);

            CallBack(path);
        }

    }
}