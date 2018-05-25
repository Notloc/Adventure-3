namespace Adventure.Game.Entities.AI
{
    using UnityEngine;
    using Adventure.Engine.Navigation;

    public class CowController : AIController
    {
        [Header("Wander Settings")]
        [SerializeField] Vector3 homeLocation;
        [SerializeField] int wanderRadius;

        private void Reset()
        {
            this.homeLocation = this.transform.position;
        }

        void Update()
        {
            Wander();
        }

        float timeBeforeNextWander = 0f;
        void Wander()
        {
            //Manage time between wandering
            timeBeforeNextWander -= Time.deltaTime;
            if(timeBeforeNextWander > 0f)
                return;
            timeBeforeNextWander = Random.Range(3f, 12f);


            //Choose destination
            Vector3 randomOffset = new Vector3(Random.Range(-wanderRadius, wanderRadius), 0f, Random.Range(-wanderRadius, wanderRadius));
            Vector2Int destination = character.LocationData.navgrid.WorldPointToNode(homeLocation + randomOffset);

            pathFinder.BeginPathFinding(character.LocationData, destination, HandlePathReceive);

        }

        void HandlePathReceive(Path path)
        {
            character.ReceivePath(path);
        }

    }
}