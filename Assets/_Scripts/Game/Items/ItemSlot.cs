namespace Adventure.Game.Items
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class ItemSlot
    {
        public string name
        {
            get
            {
                return item.name;
            }
        }

        [SerializeField] Item item;
        [SerializeField] int count;

    }
}