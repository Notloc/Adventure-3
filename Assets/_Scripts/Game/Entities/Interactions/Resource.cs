namespace Adventure.Game.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Adventure.Game.Skills;
    using Adventure.Game.Items;

    public class Resource : Interaction
    {
        [Header("Resource Graphics")]
        [SerializeField] GameObject resourceGraphics; 

        [Header("Skill Info")]
        [SerializeField] Skill requiredSkill;
        [SerializeField] int requiredSkillLevel;

        [Header("Resource Variables")]
        [SerializeField] Item item;
        [SerializeField] AnimationCurve amountRange;
        [SerializeField] AnimationCurve respawnTimeRange;

        [Header("Gathering Variables")]
        [SerializeField] AnimationCurve levelToGatheringTime;
        [SerializeField] AnimationCurve levelToSuccessChance;
        [SerializeField] float chanceRate = 5;


        int currentAmount = 0;
        bool respawning = false;

        protected override void Start()
        {
            base.Start();
            currentAmount = Mathf.RoundToInt(amountRange.Evaluate(UnityEngine.Random.Range(0f,1f)));
        }

        private void StartRespawn()
        {
            if(respawning == false)
            {
                resourceGraphics.SetActive(false);
                respawning = true;
                StartCoroutine(Respawn());
            }
        }

        IEnumerator Respawn()
        {
            //Wait a random amount of time
            float waitTime = respawnTimeRange.Evaluate(UnityEngine.Random.Range(0f, 1f));
            yield return new WaitForSeconds(waitTime);

            //Respawn
            currentAmount = Mathf.RoundToInt(amountRange.Evaluate(UnityEngine.Random.Range(0f, 1f)));
            respawning = false;
            resourceGraphics.SetActive(true);
        }

        //Returns true if the object can interact and assigns a callback for the objet to use to the out variable
        public override bool Interact(object user, out Func<object, InteractionData, bool> interactionFunction)
        {
            interactionFunction = null;

            iSkilled userSkilled = user as iSkilled;
            if (userSkilled == null || userSkilled.GetSkillLevel(requiredSkill) < requiredSkillLevel)
                return false;

            interactionFunction = GatherResource;
            return true;
        }

        private bool GatherResource(object user, InteractionData interactionData)
        {
            //Stop if the resource is empty
            if (currentAmount <= 0)
            {
                StartRespawn();
                return false;
            }
                

            //Stop if the user does not have the proper interfaces
            iSkilled userSkilled = user as iSkilled;
            iContainer userContainer = user as iContainer;
            if (userSkilled == null || userContainer == null)
                return false;


            int skillLevel = userSkilled.GetSkillLevel(requiredSkill);
            float minimumGatheringTime = levelToGatheringTime.Evaluate(skillLevel);

            interactionData.timer += Time.fixedDeltaTime;
            if (interactionData.timer >= minimumGatheringTime)
            {
                if (RollForSuccess(skillLevel))
                {
                    currentAmount--;
                    Debug.Log("Item Get!");
                    interactionData.Reset();

                    if(currentAmount > 0)
                        return true;

                    StartRespawn();
                    return false;
                }
                else
                {
                    Debug.Log("Chop");
                    interactionData.timer -= (1.0f / chanceRate);
                    return true;
                }
            }
            else
            {
                Debug.Log("Chop0");
                return true;
            }
        }
        private bool RollForSuccess(int skillLevel)
        {
            float chance = levelToSuccessChance.Evaluate(skillLevel);

            if(UnityEngine.Random.Range(0f, 1f) <= chance)
            {
                return true;
            }
            return false;
        }
    }
}