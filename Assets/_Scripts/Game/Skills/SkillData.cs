namespace Adventure.Game.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct SkillData
    {
        public string Name
        {
            get
            {
                return skill.name;
            }
        }

        [SerializeField] Skill skill;
        [SerializeField] float exp;

        public Skill Skill
        {
            get
            {
                return skill;
            }
        }
        public float Experience
        {
            get
            {
                return exp;
            }
        }
        
        public void GainExperience(float amount)
        {
            if (amount <= 0)
                return;

            exp += amount;
        }

    }
}