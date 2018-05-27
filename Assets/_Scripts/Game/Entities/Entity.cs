namespace Adventure.Game.Entities
{
    using System.Collections.Generic;
    using UnityEngine;
    using Adventure.Engine.Navigation;

    [RequireComponent(typeof(Rigidbody))]
    public class Entity : MonoBehaviour
    {
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

    [System.Serializable]
    public struct LocationData
    {
        public NavGrid navgrid;
        public Vector2Int coordinates;
    }
}