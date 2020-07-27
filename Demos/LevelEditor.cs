using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Helpers;
using Microsoft.Xna.Framework.Input;
using PanzerAdmiral.AI;
using Microsoft.Xna.Framework;

namespace PanzerAdmiral.Demos
{
    enum EditMode
    {
        Select,
        Move,
    } // enum EditMode

    /// <summary> Level Editor class, used to Edit the Node Path Network </summary>
    class LevelEditor
    {
        /// <summary> Editor Mode, presss Q,W to switch between select and move </summary>
        EditMode editMode;
        /// <summary>The bot-Actor walking on the path</summary>
        Actor bot;
        /// <summary>Behavior Navigation for the bot</summary>
        BehaviorNav nav;

        /// <summary> used to get a random node </summary>
        Random random = new Random();

        #region Constructor
        public LevelEditor()
        {
            //InputHelper.AddKey(Keys.Q);
            //InputHelper.AddKey(Keys.W);

            // just add all keys
            Keys[] allKeys = (Keys[])Enum.GetValues(typeof(Keys));
            foreach (var key in allKeys)
                InputHelper.AddKey(key);

            InputHelper.KeyRelease += new KeyHandler(KeyboardInput_KeyRelease);
            InputHelper.MouseMove += new MouseMoveHandler(MouseInput_MouseMove);
            InputHelper.MouseDown += new MouseClickHandler(MouseInput_MouseDown);
            InputHelper.MouseUp += new MouseClickHandler(MouseInput_MouseUp);

            // tst actor navigation
            // Create behavior and the actor
            nav = new BehaviorNav(0.2f);
            nav.GoalReached += new EventHandler(nav_GoalReached);

            bot = new Actor("AI/arrow", Color.White);
            bot.Position = new Vector2(0, -380);
            bot.BehaviorList.Add(nav);

        } // LevelEditor()
        #endregion

        #region nav_GoalReached
        void nav_GoalReached(object sender, EventArgs e)
        {
            //bot.Speed = 0; // stops the bot when we reach our goal
            Node start = Node.GetClosestNode(bot.Position);
            Node goal = Node.GetRandomNode(random);
            NavigateToNode(start, goal);
        } // nav_GoalReached(sender, e)
        #endregion

        #region KeyboardInput_KeyRelease
        /// <summary>
        /// Handle Key Release
        /// Q = Select Mode
        /// W = Move Mode
        /// Delete = Delete Nodes
        /// C = Connect two selected nodes
        /// X = Break connection between nodes
        /// Strg + s Saves Node Network
        /// Strg + o Loads Node network
        /// </summary>
        /// <param name="keys">Keyboard keys</param>
        /// <param name="keyState">Keyboard State</param>
        /// <returns></returns>
        void KeyboardInput_KeyRelease(Keys key, KeyboardState keyState)
        {
            if (key == Keys.Q)
            {
                this.editMode = EditMode.Select;
            }
            else if (key == Keys.W)
            {
                this.editMode = EditMode.Move;
            }
            else if (key == Keys.Delete)
            {
                // Delete everything that is selected
                foreach (var actor in Actor.Selection)
                    actor.Delete();

                // make sure everything is cleared
                Actor.Selection.Clear();

            } // if (Delete)
            else if (key == Keys.C && Actor.Selection.Count >= 2)
            {
                for (int i = 1; i < Actor.Selection.Count; i++)
                {
                    Node nodeA = Actor.Selection[i - 1] as Node;
                    Node nodeB = Actor.Selection[i] as Node;

                    if (nodeA != null && nodeB != null)
                        nodeA.BidiConnect(nodeB);
                }
            } // if(C)
            else if (key == Keys.X && Actor.Selection.Count >= 2)
            {
                for (int i = 1; i < Actor.Selection.Count; i++)
                {
                    Node nodeA = Actor.Selection[i - 1] as Node;
                    Node nodeB = Actor.Selection[i] as Node;

                    if (nodeA != null && nodeB != null)
                        nodeA.BidiDisconnect(nodeB);
                }
            } // if(X)
            else if (key == Keys.P && Actor.Selection.Count >= 2) // AStarStep Begin
            {
                Node start = Actor.Selection[0] as Node;
                Node goal = Actor.LastSelected as Node;

                if (start != null && goal != null)
                {
                    AStarStep.Begin(start, goal);
                    AStarStep.Continue();

                } // if (start && goal)
            }
            else if (key == Keys.Space)         // AstarStep Continue
            {
                if (AStarStep.InProgress)
                    AStarStep.Continue();
            } // if (Space)

            else if (key == Keys.Enter)  // AStar Actor navigation
            {
                Node start = Node.GetClosestNode(bot.Position); // get closest node to our AI Actor
                Node goal = Actor.LastSelected as Node;
                NavigateToNode(start, goal);
            } // if (enter)
            else if (key == Keys.S && ScreenManager.Input.IsControlDown)
            {
                //NodeIO.SaveNodes("nodes.txt");
                NodeIO.ShowSaveDialog();
            } // if (ctrl + s)
            else if (key == Keys.O && ScreenManager.Input.IsControlDown)
            {
               // NodeIO.LoadNodes("nodes.txt");
                NodeIO.ShowLoadDialog();
            } // if (ctrl + o)

        }
        #endregion

        #region MouseInput_MouseMove
        /// <summary>
        /// Left MouseButton (Move Mode) = Move Actors
        /// Z = Calculate MouseOver path
        /// </summary>
        /// <param name="position"></param>
        /// <param name="movement"></param>
        /// <returns></returns>
        void MouseInput_MouseMove(Vector2 position, Vector2 movement)
        {
            // find path between selected and mouseover node
            if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Z) && Actor.Selection.Count > 0)
            {
                Node start = Actor.Selection[0] as Node;
                Node goal = Node.GetClosestNode(position);

                if (start != null && goal != null)
                    AStar.FindPath(start, goal);

            } // if Z

            // if we are in edit mode and user pressed the left button
            if (this.editMode == EditMode.Move && ScreenManager.Input.IsLeftButtonDown)
            {
                foreach (var actor in Actor.Selection)
                {
                    actor.Position += movement;
                }
            }

        } // MouseInput_MouseMove position, movement)
        #endregion

        #region MouseInput_MouseDown
        /// <summary>
        /// Control Key allows multiple actor selection if we double clicke on an actor, he gets deselected
        /// </summary>
        /// <param name="position">mousePos</param>
        void MouseInput_MouseDown(Vector2 position)
        {
            // Creation with Shift 
            if (ScreenManager.Input.IsShiftDown)
            {
                Node node = new Node();
                node.Position = position;

                // if there are any selected nodes make Two Connections
                Node lastNode = Actor.LastSelected as Node;
                if (lastNode != null)
                {
                    node.BidiConnect(lastNode);
                }

                Actor.Selection.Clear();
                node.Select();

                // avoid the selection code if shift is pressed
                return;
            } // if (IsShiftDown)
            

            // Handle Actor Selection! 
            Actor actor = GetActorAt(position);
            if (actor != null)
            {
                // deselect if this actor is not selected and control is not don
                if (!actor.IsSelected && !ScreenManager.Input.IsControlDown)
                    Actor.Selection.Clear();
                // 
                if (ScreenManager.Input.IsControlDown)
                    actor.ToggleSelect();
                else
                    actor.Select();
            }
            else if(!ScreenManager.Input.IsControlDown)
            {
                Actor.Selection.Clear();
            }

        } // MouseInput_MouseDown(position)
        #endregion

        #region MouseInput_MouseUp
        void MouseInput_MouseUp(Vector2 position)
        {
            if (Actor.Selection.Count > 0 && !ScreenManager.Input.IsControlDown)
            {
                Actor.Selection.Clear();

                Actor actor = GetActorAt(position);
                if (actor != null)
                    actor.Select();

            }

        } // MouseInput_MouseUp( position)
        #endregion

        #region GetActorAt
        /// <summary>
        /// returns an actor at the given mouse position
        /// </summary>
        /// <param name="position">position to check</param>
        /// <returns>Actor, null if there's no actor at the current Position</returns>
        Actor GetActorAt(Vector2 position)
        {
            for (int i = Actor.Actors.Count - 1; i >= 0; i--)
            {
                Actor actor = Actor.Actors[i];
                if (Vector2.Distance(actor.Position, position) < actor.Radius)
                {
                    return actor;
                }
            }

            return null;

        } // GetActorAt(position)
        #endregion

        #region NavigateToNode
        /// <summary>
        /// Navigates the bot to the Node
        /// </summary>
        /// <param name="start">start node</param>
        /// <param name="goal">goal node</param>
        /// <returns></returns>
        void NavigateToNode(Node start, Node goal)
        {
            if (start != null && goal != null)
            {
                List<Node> path = AStar.FindPath(start, goal);

                if (path != null)
                {
                    nav.BeginNavigation(path);
                    bot.Speed = 1f;
                } // if

            } // if 
        } // NavigateToNode(start, goal)
        #endregion

        #region Draw
        /// <summary> Draw Editor Mode String </summary>
        public void Draw()
        {
            string text = String.Format("mode: {0}", this.editMode);
            ScreenManager.SpriteBatch.End();
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(Style.FontLarge, text, new Vector2(10, 200), Color.Red);
            //ScreenManager.SpriteBatch.DrawString(Style.FontLarge, text, new Vector2(200, 30), Color.White);
            ScreenManager.SpriteBatch.End();
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, PhysicsGameScreen.Camera.View);
        } // Draw
        #endregion
    } // class LevelEditor
} // namespace PanzerAdmiral.Demos
