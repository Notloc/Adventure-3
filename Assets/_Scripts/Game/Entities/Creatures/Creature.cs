namespace Adventure.Game.Entities
{
    using UnityEngine;
    using Adventure.Engine.Navigation;
    using Adventure.Game.Items;
    using Adventure.Game.Skills;

    using System.Collections;
    using System;

    public class Creature : Entity, iDamagable, iControllable, iSkilled, iContainer
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

        [SerializeField] ItemContainer inventory;

        [SerializeField] protected SkillData[] skills;

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

        InteractionData _interactionData = new InteractionData();
        public InteractionData InteractionData
        {
            get
            {
                return _interactionData;
            }
        }

        public ItemContainer ItemContainer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Interaction currentInteraction;

        Path userPath;
        Path autoPath;
        PathFinder pathfinder = new PathFinder();
        bool userPathReady = false;
        bool autoPathReady = false;

        void FixedUpdate()
        {
            ExecuteCurrentState(state);
        }
        private void ExecuteCurrentState(State state)
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

        Func<object, InteractionData, bool> interactionMethod;
        private void TryInteraction(Interaction interaction)
        {
            if (interaction == null)
            {
                FinishInteracting();
                return;
            }
                

            if(interactionState == InteractingSubState.INITIAL)
            {
                //Make sure we are in range of the interaction, path to it if we are not
                
                if (interaction.InRange(this.locationData.coordinates))
                {
                    if (interaction.Interact(this, out interactionMethod))
                    {
                        interactionState = InteractingSubState.INTERACTING;
                        return;
                    }
                }
                else
                {
                    interactionState = InteractingSubState.PATHING;
                    interaction.GeneratePath(this.locationData, AutoMove);
                    return;
                }
            }

            if(interactionState == InteractingSubState.PATHING)
            {
                //Await the autopath and start it when ready

                if (autoPathReady)
                {
                    StopAllCoroutines();
                    autoPathReady = false;
                    StartCoroutine(FollowPath(autoPath, FinishAutoMove));
                    return;
                }
            }

            if(interactionState == InteractingSubState.INTERACTING)
            {
                //Interact with the interaction until we have a reason to stop

                if(interactionMethod == null)
                {
                    FinishInteracting();
                    return;
                }

                if(!interactionMethod(this, InteractionData))
                {
                    FinishInteracting();
                    return;
                }
            }

        }
        private void FinishInteracting()
        {
            this.currentInteraction = null;
            this.interactionMethod = null;
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
            this.state = State.IDLE;
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
        private void FinishAutoMove()
        {
            interactionState = InteractingSubState.INITIAL;
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
            CallBack();
        }

        public void Interact(Interaction interaction)
        {
            ClearAutoPath();
            this.currentInteraction = interaction;
            this.InteractionData.Reset();
            this.interactionState = InteractingSubState.INITIAL;
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

        public int GetSkillLevel(Skill skill)
        {
            foreach (SkillData skillData in skills)
            {
                if(skillData.skill == skill)
                {
                    return skill.GetLevel(skillData.exp);
                }
            }

            return 0;
        }
    }

    [System.Serializable]
    public struct Attributes
    {
        public int health;
        public int movementSpeed;
    }
}
