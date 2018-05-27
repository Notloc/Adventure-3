namespace Adventure.Game.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Adventure.Game.Entities;

    public abstract class Skill : ScriptableObject
    {
        [SerializeField] Dictionary<Entity, Skill[]> users;
    }
}