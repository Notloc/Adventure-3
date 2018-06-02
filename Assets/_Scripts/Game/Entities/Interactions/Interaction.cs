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
        [Header("Range")]
        [SerializeField] [Range(-1, 12)] int activationRange = 1;
        [SerializeField] bool squareRange = false;

        PathFinder pathFinder = new PathFinder();
        protected Entity entity;

        protected virtual void Start()
        {
            entity = this.GetComponent<Entity>();
        }

        public abstract bool Interact(object user, out Func<object, InteractionData, bool> func);

        public virtual bool InRange(Vector2Int coordinates)
        {
            if (activationRange == -1)
                return true;

            Vector2Int interactionNode = this.GetComponent<Entity>().LocationData.coordinates;
            if(squareRange)
            {
                //Simply check if the user is within the square
                int xDifference = Mathf.Abs(interactionNode.x - coordinates.x);
                int yDifference = Mathf.Abs(interactionNode.y - coordinates.y);

                if (xDifference <= activationRange && yDifference <= activationRange)
                    return true;

                return false;
            }
            else
            {
                //Use the navgrid to measure the movement distance
                if (NavGrid.Distance(interactionNode, coordinates) <= activationRange)
                    return true;

                return false;
            }
        }
        public virtual void GeneratePath(LocationData userLocationData, Action<Path> CallBack)
        {
            if (activationRange == -1)
                return;

            HashSet<Vector2Int> validCoordinates = new HashSet<Vector2Int>();
            Vector2Int interactionCoordinates = this.GetComponent<Entity>().LocationData.coordinates;

            //Generate set of valid nodes
            for (int y = -activationRange; y <= activationRange; y++)
            {
                for(int x = -activationRange; x <= activationRange; x++)
                {
                    Vector2Int newCoordinates = interactionCoordinates + new Vector2Int(x, y);
                    if (InRange(newCoordinates))
                    {
                        validCoordinates.Add(newCoordinates);
                    }
                }
            }

            pathFinder.BeginPathFinding(userLocationData, interactionCoordinates, validCoordinates, CallBack);
        }

    }
}