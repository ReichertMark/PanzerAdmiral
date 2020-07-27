using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PanzerAdmiral.AI
{
    class BehaviorNav : Behavior
    {
        /// <summary>current node index</summary>
        int nodeIndex = 0;
        /// <summary>the current path being navigated</summary>
        List<Node> path;
        /// <summary>Goal reached event</summary>
        public event EventHandler GoalReached = delegate(object sender, EventArgs e) { };
        /// <summary>Node reached event</summary>
        public event EventHandler NodeReached = delegate(object sender, EventArgs e) { };

        public BehaviorNav(float weight)
            : base(weight)
        {
        }

        public void BeginNavigation(List<Node> path)
        {
            nodeIndex = 0;
            this.path = path;
        } // BeginNavigation(path)


        /// <summary>
        /// Updates the actor's motion to reach the goal path and fires the Events correctly
        /// Don't call base here
        /// </summary>
        /// <param name="actor"></param>
        public override void Update(Actor actor)
        {
            // f we have a path and we are not at the end of the path
            if (path != null && nodeIndex < path.Count)
            {
                Node nextNode = path[nodeIndex];
                if (Vector2.Distance(actor.Position, nextNode.Position) < nextNode.Radius)
                {
                    nodeIndex++;
                    if (nodeIndex == path.Count) // have we reached our goal? then fire the GoalReached event 
                        GoalReached(this, EventArgs.Empty);
                    else  // we have reached a Node, fire the NodeReached event
                        NodeReached(this, EventArgs.Empty);
                    return;
                } // if

                Vector2 targetDirection = nextNode.Position - actor.Position;
                if (targetDirection != Vector2.Zero)
                    targetDirection.Normalize();

                // use that TargetDirection to influence the actor
                actor.Direction += targetDirection * this.Weight;

            } // if

        } // Update(actor)
    } // class BehaviorNav
} // namespace PanzerAdmiral.AI
