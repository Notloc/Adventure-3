namespace Adventure.Game.Entities
{
    using UnityEngine;
    using Adventure.Engine.Navigation;

    using System.Collections;
    using System.Threading;

    public class Creature : Entity, iDamagable
    {
        public int Health
        {
            get
            {
                return stats.health;
            }
        }
        public int MovementSpeed
        {
            get
            {
                return stats.movementSpeed;
            }
        }

        [SerializeField] Stats stats = new Stats();

        Path path;
        PathFinder pathfinder = new PathFinder();
        bool pathReady = false;


        void Update()
        {
            Movement();
        }

        public void Movement()
        {
            if(pathReady)
            {
                StopAllCoroutines();
                pathReady = false;
                StartCoroutine(FollowPath(path));
            }
        }

        IEnumerator FollowPath(Path path)
        {
            float timePassed = 0;
            float timePerMove = (1.0f / MovementSpeed);

            while (path.HasDirections())
            {
                Vector2Int nextLocation = path.Next();

                Vector3 currentPosition = transform.position;
                Vector3 targetPosition = locationData.navgrid.NodeToWorldPoint(nextLocation);

                while (timePassed < timePerMove)
                {
                    yield return null;
                    timePassed += Time.deltaTime;
                    this.transform.position = Vector3.Lerp(currentPosition, targetPosition, (timePassed/timePerMove));
                }
                locationData.coordinates = nextLocation;
                timePassed -= timePerMove;
            }
        }

        public void Damage(int amount)
        {
            stats.health -= amount;
        }

        public void MoveTo(NavGrid navgrid, Vector2Int targetPoint)
        {
            //Quit if a different NavGrid was clicked
            if(navgrid != locationData.navgrid)
                return;

            //Begin Pathfinding
            pathfinder.BeginPathFinding(navgrid, targetPoint, locationData.coordinates, ReceivePath);
        }

        public void ReceivePath(Path newPath)
        {
            this.path = newPath;
            pathReady = true;
        }

    }

    [System.Serializable]
    public struct Stats
    {
        public int health;
        public int movementSpeed;
    }
}
