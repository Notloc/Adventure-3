namespace Adventure.Game.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface iSkilled
    {
        int GetSkillLevel(Skill skill);
        void GainExperience(Skill skill, float amount);
    }
}