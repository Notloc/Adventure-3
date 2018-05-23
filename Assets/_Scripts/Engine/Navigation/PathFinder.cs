namespace Adventure.Engine.Navigation
{
    using System;
    using System.Collections;
    using System.Threading;
    using System.Diagnostics;

    public static class PathFinder
    {
        public static Thread BeginPathFinding(NavGrid navgrid, Vector2i targetNode, Vector2i currentNode, Path path)
        {
            Thread daThread = new Thread( () => PathFind(navgrid, targetNode, currentNode, path) );
            daThread.Start();
            return daThread;
        }


        private static void PathFind(NavGrid navgrid, Vector2i targetNode, Vector2i currentNode, Path path)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            float timePassed = 0;
            while(timePassed < 3f)
            {
                timePassed = (timer.ElapsedMilliseconds) / 1000.0f;
            }

            UnityEngine.Debug.Log("Threads done!");
        }
    }
}