namespace Adventure.Game.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct SkillData
    {
        public string name
        {
            get
            {
                return skill.name;
            }
        }

        public float exp;
        public Skill skill;  
    }
}