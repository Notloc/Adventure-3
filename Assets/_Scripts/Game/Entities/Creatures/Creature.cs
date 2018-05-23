namespace Adventure.Game.Entities
{
    using System.Collections;
    using UnityEngine;
    using Adventure.Engine.Navigation;
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

        void Start()
        {
            MoveTo(null, new Vector2i(0,0));
        }

        public void Damage(int amount)
        {
            stats.health -= amount;
        }


        public void MoveTo(NavGrid navgrid, Vector2i targetPoint)
        {
            StartCoroutine(Pathfind(navgrid, targetPoint));
        }

        IEnumerator Pathfind(NavGrid navgrid, Vector2i targetNode)
        {
            yield return null;

            Thread myThread = PathFinder.BeginPathFinding(navgrid, targetNode, targetNode, path);
            while(myThread.IsAlive)
            {
                Debug.Log("Waiting");
                yield return null;
            }
            Debug.Log("Done!");
        }

    }

    [System.Serializable]
    public struct Stats
    {
        public int health;
        public int movementSpeed;
    }
}
