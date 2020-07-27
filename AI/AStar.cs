using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace PanzerAdmiral.AI
{
    /// <summary>Single Call Implementation of the AStar algorithm</summary>
    static class AStar
    {
        /// <summary>Delegate to Compare the nodes by f Score</summary>
        static Comparison<Node> FScoreComparison = new Comparison<Node>(CompareNodesByFScore);

        #region CompareNodesByFScore
        static int CompareNodesByFScore(Node x, Node y)
        {
            if (x.FScore > y.FScore)
                return 1;
            if (x.FScore < y.FScore)
                return -1;

            return 0;
        } // CompareNodesByFScore(Node x, Node y)
        #endregion

        #region GetHScore
        /// <summary>
        /// Method for calculating the HScore
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>Distance between Vectors</returns>
        static float GetHScore(Vector2 vectorA, Vector2 vectorB)
        {
            return Vector2.Distance(vectorA, vectorB);
        } // GetHScore(Vector2 vectorA, Vector2 vectorB)
        #endregion

        #region FindPath
        /// <summary>
        /// Computes AStar to find the Path
        /// </summary>
        /// <param name="start">Start Node</param>
        /// <param name="goal">Goal Node</param>
        /// <returns>Returns the shortest Path or null if none was found</returns>
        public static List<Node> FindPath(Node start, Node goal)
        {
            foreach (var node in Actor.Actors.OfType<Node>())
                node.Reset();

            List<Node> path = null;

            List<Node> openList = new List<Node>();
            openList.Add(start);

            while (openList.Count > 0) // once we have reached the Goal node, the openList will be empty
            {


                // Sorte the Listby FScore
                openList.Sort(FScoreComparison);

                // Get the current Node, remove it from the openList and set it to closed
                Node current = openList[0];
                openList.Remove(current);
                current.Closed = true;

                // case when we have found the goal, end of Progress
                if (current == goal)
                {
                    path = BuildPath(goal);
                    return path;
                } // if(goal)

                // walk through all of the neighboring nodes
                foreach (var neighbor in current.Connected)
                {
                    // if neighbor closed, ignore 
                    if (neighbor.Closed)
                        continue;

                    // take the path costs into account
                    float neighborGScore = current.GScore + Vector2.Distance(current.Position, neighbor.Position);

                    // Update the state of the individual neighbor nodes
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                        neighbor.Parent = current;  // we got the neighbor from this parent now
                        neighbor.GScore = neighborGScore;
                        neighbor.HScore = GetHScore(neighbor.Position, goal.Position); // grants efficient search
                        neighbor.FScore = neighbor.GScore + neighbor.HScore;
                    }
                    else if (neighborGScore < neighbor.GScore)
                    {
                        // Nodes that are already n the openList
                        neighbor.Parent = current;
                        neighbor.GScore = neighborGScore;
                        neighbor.FScore = neighbor.GScore + neighbor.HScore;
                    } // else if
                } // foreach (neighbor in current.Connected)
            } // while (openList.Count > 0)

            return path;
        } // FindPath(Node start, Node goal)
        #endregion

        #region BuildPath
        /// <summary>
        /// Builds the path to the goal node
        /// </summary>
        /// <param name="goal"></param>
        /// <returns>A List of nodes containing the builded path</returns>
        static List<Node> BuildPath(Node goal)
        {
            List<Node> path = new List<Node>();
            Node node = goal;

            while (node != null)
            {
                // include the goal itself to the path
                path.Add(node);
                node.InPath = true;
                node = node.Parent;
            } // while

            // Put the path in order (will be removed in future)
            path.Reverse();

            return path;
        } // BuildPath(goal)
        #endregion
    } // AStar
} // namespace PanzerAdmiral.AI
