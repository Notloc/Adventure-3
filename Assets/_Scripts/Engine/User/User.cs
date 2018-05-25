namespace Adventure.Engine.User
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using Adventure.Game.Entities;

    [RequireComponent(typeof(MouseHandler))]
    public class User : MonoBehaviour
    {
        [SerializeField] Creature character;

        MouseHandler mouseHandler;

        private void Start()
        {
            mouseHandler = this.GetComponent<MouseHandler>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Click"))
            {
                mouseHandler.HandleClick(character);
            }
        }

    }
}