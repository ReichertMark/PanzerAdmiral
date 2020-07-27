using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace PanzerAdmiral.AI
{
    /// <summary>Holds the methods Necessary for the algorithm, multiple step implementation</summary>
    static class AStarStep
    {
        /// <summary>Node we are starting from</summary>
        static Node start;
        /// <summary>Target Node</summary>
        static Node goal;
        /// <summary>All the neighbour nodes are kept in the so called openList</summary>
        static List<Node> openList = new List<Node>();
        /// <summary>The Path to walk on</summary>
        static List<Node> path;
        /// <summary>Tells whether or not we have begun searching</summary>
        public static bool InProgress = false;
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

        #region Begin
        /// <summary>
        /// Resets all nodes
        /// Sets start and goal node
        /// creates the openList and adds the start node
        /// </summary>
        /// <param name="start">Starting Node</param>
        /// <param name="goal">Target Node</param>
        public static void Begin(Node start, Node goal)
        {
            foreach (var node in Actor.Actors.OfType<Node>())
                node.Reset();

            AStarStep.start = start;
            AStarStep.goal = goal;

            openList = new List<Node>();
            openList.Add(start);

            InProgress = true;
        } // Begin(start, goal) 
        #endregion

        #region Continue
        /// <summary>
        /// Continues with the search algorithm
        /// </summary>
        public static void Continue()
        {
            if (!InProgress)
                return;

            // once we have reached the Goal node, the openList will be empty
            if (openList.Count == 0)
                return;

            // Sorte the List by FScore
            openList.Sort(FScoreComparison);

            // Get the current Node, remove it from the openList and set it to closed
            Node current = openList[0];
            openList.Remove(current);
            current.Closed = true;

            // case when we have found the goal, end of Progress
            if (current == goal)
            {
                InProgress = false;
                path = BuildPath(goal);     // calculate the path to the goal
                return;
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
                else if (neighborGScore < neighbor.GScore) //  Nodes that are already n the openList
                 {
                     // Update Scores and recalculate FScore
                     neighbor.Parent = current;
                     neighbor.GScore = neighborGScore;
                     neighbor.FScore = neighbor.GScore + neighbor.HScore;
 
                 } // else if
            } // foreach (neighbor)
        } // Continue()
        #endregion

        #region BuildPath
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

            // Put the path in order and return it
            path.Reverse();
            return path;
        } // BuildPath(goal)
        #endregion
    } // class AStarStep
} // namespace PanzerAdmiral.AI
