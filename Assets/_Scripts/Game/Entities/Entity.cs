namespace Adventure.Game.Entities
{
    using Adventure.Engine.Navigation;

    using System.Collections.Generic;
    using UnityEngine;   

    [RequireComponent(typeof(Rigidbody))]
    public class Entity : MonoBehaviour
    {
        [SerializeField] GameObject graphics;
        [SerializeField] protected List<Interaction> interactions;
        [SerializeField] protected LocationData locationData;

        public List<Interaction> Interactions
        {
            get
            {
                return interactions;
            }
        }
        public Interaction DefaultInteraction
        {
            get
            {
                if (interactions.Count == 0)
                    return null;
                return interactions[0];
            }
        }
        public LocationData LocationData
        {
            get
            {
                return locationData;
            }
        }

        public void Enable3D()
        {
            graphics.SetActive(true);
        }
        public void Disable3D()
        {
            graphics.SetActive(false);
        }

        private void Reset()
        {
            AutoAcquireLocationData();
        }
        private void AutoAcquireLocationData()
        {
            locationData.navgrid = NavGrid.GetClosestNavGrid(this.transform.position);
            if(locationData.navgrid == null)
                return;

            locationData.coordinates = locationData.navgrid.WorldPointToNode(this.transform.position);
        }
    }
}