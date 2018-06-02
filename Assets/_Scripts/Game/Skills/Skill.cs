namespace Adventure.Game.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Adventure.Game.Entities;

    public abstract class Skill : ScriptableObject
    {
        [SerializeField] AnimationCurve levelCurve;

        public int GetLevel(float experience)
        {
            return Mathf.Clamp(Mathf.FloorToInt(levelCurve.Evaluate(experience)), 1, 99);
        }

    }
}