namespace Adventure.Game.Entities
{
    using UnityEngine;
    using Adventure.Engine.Navigation;

    using System.Collections;
    using System.Threading;
    using System;

    public class Creature : Entity, iDamagable, iControllable
    {
        private enum State { IDLE, INTERACTING };
        private enum InteractingSubState { INITIAL, PATHING, INTERACTING };

        [SerializeField] State state = State.IDLE;
        [SerializeField] InteractingSubState interactionState = InteractingSubState.INITIAL;

        [SerializeField]
        Attributes attributes = new Attributes()
        {
            movementSpeed = 1
        };

        public int Health
        {
            get
            {
                return attributes.health;
            }
        }
        public int MovementSpeed
        {
            get
            {
                return attributes.movementSpeed;
            }
        }

        Interaction currentInteraction;

        Path userPath;
        Path autoPath;
        PathFinder pathfinder = new PathFinder();
        bool userPathReady = false;
        bool autoPathReady = false;

        void Update()
        {
            ExecuteState(state);  
        }

        private void ExecuteState(State state)
        {
            if (state == State.IDLE)
                TryMovement();
            else
            if (state == State.INTERACTING)
                TryInteraction(currentInteraction);
        }

        private void TryMovement()
        {
            if(userPathReady)
            {
                StopAllCoroutines();
                userPathReady = false;
                StartCoroutine(FollowPath(userPath, FinishMoving));
            }
        }
        private void TryAutoMovement()
        {
            if (autoPathReady)
            {
                StopAllCoroutines();
                autoPathReady = false;
                StartCoroutine(FollowPath(autoPath, FinishMoving));
            }
        }

        private void TryInteraction(Interaction interaction)
        {
            if (interaction == null)
            {
                this.state = State.IDLE;
                return;
            }
                

            if(interaction.InRange(this.locationData))
            {
                interaction.Interact();
            }

            interaction.GeneratePath(this.locationData, AutoMove);

            Debug.Log("Interacted with " + interaction.gameObject.name);



            currentInteraction = null;
            this.state = State.IDLE;
        }

        public void Move(Vector2Int targetPoint)
        {
            //Start a pathfinding thread to generate a Path
            pathfinder.BeginPathFinding(locationData, targetPoint, Move);
        }
        public void Move(Path newPath)
        {
            this.userPath = newPath;
            userPathReady = true;
        }
        private void FinishMoving()
        {
            if (this.state != State.INTERACTING)
                this.state = State.IDLE;
        }

        private void AutoMove(Path newPath)
        {
            this.autoPath = newPath;
            autoPathReady = true;
        }

        IEnumerator FollowPath(Path path, Action CallBack)
        {
            float timePassed = 0;
            float timePerMove = (1.0f / MovementSpeed);

            while (path.HasDirections())
            {
                Vector2Int nextNode = path.Next();

                Vector3 currentPosition = transform.position;
                Vector3 targetPosition = locationData.navgrid.NodeToWorldPoint(nextNode);

                if (!locationData.navgrid.IsPathable(nextNode))
                    break;

                locationData.coordinates = nextNode;

                while (timePassed < timePerMove)
                {
                    yield return null;
                    timePassed += Time.deltaTime;
                    this.transform.position = Vector3.Lerp(currentPosition, targetPosition, (timePassed / timePerMove));
                }
                timePassed -= timePerMove;
            }
        }

        public void Interact(Interaction interaction)
        {
            ClearAutoPath();
            this.currentInteraction = interaction;
            this.state = State.INTERACTING;
        }

        public void Damage(int amount)
        {
            attributes.health -= amount;
        }

        private void ClearUserPath()
        {
            this.userPath = null;
            userPathReady = false;
        }
        private void ClearAutoPath()
        {
            this.autoPath = null;
            autoPathReady = false;
        }
    }

    [System.Serializable]
    public struct Attributes
    {
        public int health;
        public int movementSpeed;
    }
}
