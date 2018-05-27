namespace Adventure.Game.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Adventure.Game.Skills;

    public class Resource : Interaction
    {
        [SerializeField] Item item;
        [SerializeField] Skill requiredSkill;

        public override void Interact()
        {
            throw new NotImplementedException();
        }
    }
}