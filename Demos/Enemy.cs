using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.AI;
using PanzerAdmiral.ScreenSystem;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using PanzerAdmiral.Helpers;
using FarseerPhysics.Factories;
using PanzerAdmiral.Graphics;


namespace PanzerAdmiral.Demos
{
    #region EnemyState
    public enum EnemyState
    {
        Wander,     // Simply wander to random pathnodes
        SeekPlayer, // If player close enough (SightRadius), begin chasing him
        Dead,       // When we are in meelee range, explode !
    } // EnemyState
    #endregion

    /// <summary> Enemy class, contains the state of the enemy and a sightradius. If the  </summary>
    public class Enemy : Actor
    {
        #region Variables
        BehaviorNav enemyNav;

        Random random = new Random();

        // default State is Wander!
        public EnemyState State = EnemyState.Wander;
        public float SightRadius = 200; //256;//128;

        public Body body;                         // Farseer Body
        //public Vector2 _size = new Vector2(1);  // texture dimensions
        float density = 1.0f;

        Animation _animationCogwheel;

        public bool exploded = false;

        public int HitPoints;
        #endregion

        #region Constructor
        /// <summary> Create Red Arrow Enemy with arrow Texture </summary>
        public Enemy(Vector2 setPosition, int setHitPoints)
            : base("AI/arrow", new Color(255, 0 , 0))
        {

            // Create animation
            _animationCogwheel = new Animation("Enemies/DerGeraet", 7, 5, 0, 32, 1000, true);
            //_animationCogwheel = new Animation("Enemies/cogwheel", 4, 12, 0, 48, 2000, true);
            _animationCogwheel.IsAlive = true;
            _animationCogwheel.Scale = 0.3f;


            // Assign hitpoints and create rectangle from Texture
            HitPoints = setHitPoints;
            body = BodyFactory.CreateRectangle(PhysicsGameScreen.World, ConvertUnits.ToSimUnits(Texture.Width), ConvertUnits.ToSimUnits(Texture.Height), density);
            body.UserData = "enemy";


            // Create Behavior
            enemyNav = new BehaviorNav(0.2f);
            enemyNav.GoalReached += new EventHandler(enemyNav_GoalReached);
            enemyNav.NodeReached += new EventHandler(enemyNav_NodeReached);
            BehaviorList.Add(enemyNav);

            // set Position and start navigation
            Position = setPosition;
            NavigateToActor(Node.GetClosestNode(setPosition));
             
        } // Enemy(Vector2 setPosition, int setHitPoints)
        #endregion

        #region enemyNav_GoalReached
        /// <summary>
        /// Event fired if an Enemy Reaches a Goal Node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        void enemyNav_GoalReached(object sender, EventArgs e)
        {
            // if we wander, navigate to random node
            if (State == EnemyState.Wander)
            {
                NavigateToActor(Node.GetRandomNode(random));
            }
            else if (State == EnemyState.SeekPlayer) // if we Seek the Player
            {
                NavigateToActor(GameDemo1.vehicle);
            }
        } // enemyNav_GoalReached
        #endregion

        #region enemyNav_NodeReached
        /// <summary>
        /// Event fired if an Enemy Reaches a Node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        void enemyNav_NodeReached(object sender, EventArgs e)
        {
            // Always navigate to Player actor when a node is reached when in SeekPlayer mode
            if (State == EnemyState.SeekPlayer)
            {
                NavigateToActor(GameDemo1.vehicle);
            }
        } // enemyNav_NodeReached
        #endregion

        #region NavigateToActor
        /// <summary>
        /// Navigates an Enemy Actor to an Actor
        /// </summary>
        /// <param name="actor">The Actor to Navigate to</param>
        void NavigateToActor(Actor actor)
        {
            Node start = Node.GetClosestNode(Position);
            Node goal = Node.GetClosestNode(actor.Position);

            if (start != null && goal != null)  // Make sure both nodes are valid
            {
                List<Node> path = AStar.FindPath(start, goal);
                if (path != null)
                {
                    if (path.Count > 1)
                    {
                        path[0].InPath = false;
                        path.RemoveAt(0);
                    }

                    enemyNav.BeginNavigation(path);
                }


            } // if

        } // NavigateToActor(actor)
        #endregion

        #region Update
        /// <summary> Update Enemy behavior and shots by player </summary>
        public override void Update()
        {
            // Handle damage taken from the player
            if (body.UserData.ToString() == "damage")
            {
                // substract hitpoints
                HitPoints -= GameSettings.PlayerDamage;
                body.UserData = "enemy";                // reset userdata to enemy and receive no more damage from this shot!

                // case when enemy is dead, TODO: Play killed animation and dispose then.
                if (HitPoints <= 0)
                {
                    // receive Score
                    GameSettings.HighScore += (uint)GameSettings.EnemyScore;
                    FadeUp.AddTextMessage("+ " + GameSettings.EnemyScore, FadeUp.TextTypes.Score, body.Position);

                    body.UserData = "disable";
                    body.Dispose();
                    Delete();
                } // if (HitPoints <= 0)

            } //  if ("damage")


            // Meelee range!  Distance based Collision Check between player's radius and enemy's radius
            if (Vector2.Distance(GameDemo1.vehicle.Position, Position) < GameDemo1.vehicle.Radius + Radius)
            {
                // Only explode once
                if (exploded == false)
                {
                    FadeUp.AddTextMessage("- " + GameSettings.EnemyDamage, FadeUp.TextTypes.Damage, GameDemo1.vehicle.Body.Position);
                    HealthBar.SetDamage(GameSettings.EnemyDamage);
                    exploded = true;
                }

                // if we are exploded and did the Damage, Delete the enemy!
                if (exploded)
                {
                    body.Dispose();
                    Delete();
                }

                ChangeState(EnemyState.Dead);
                //health.Active = true;
            }
            // Distance based Collision Check between player's radius and enemy's sightradius
            else if (Vector2.Distance(GameDemo1.vehicle.Position, Position) < GameDemo1.vehicle.Radius + SightRadius)
            {
                // if we are not Dead!
                if(State != EnemyState.Dead)
                    ChangeState(EnemyState.SeekPlayer); // Change state to seek player
            }


            base.Update();

        } // Update()
        #endregion

        #region ChangeState
        /// <summary>
        /// Changes the State of this Enemy
        /// </summary>
        /// <param name="state">set Enemy State</param>
        public void ChangeState(EnemyState state)
        {
            this.State = state;
        } // ChangeState(state)
        #endregion

        #region DrawCircle
        /// <summary> Draws the SghtRadius for Debugging </summary>
        private void DrawCircle(float radius)
        {
            // use transparent white color with 80 alpha
            Color color = new Color(255, 255, 255, 80);
            Texture2D texture = Style.NodeTexture;
            int textureRadius = texture.Width / 2;
            
            float scale = radius / textureRadius;
            Vector2 origin = new Vector2(textureRadius, textureRadius);
            ScreenManager.SpriteBatch.Draw(texture, this.Position, null, color, 0f,  origin, scale, SpriteEffects.None, 1f);
        } // DrawCircle(radius)
        #endregion

        #region Draw
        /// <summary> Override Draw , Draws SightRadius if Y Key is pressed</summary>
        public override void Draw()
        {
            // debug Sightradius
            if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Y))
            {
                DrawCircle(SightRadius);
                base.Draw();
            }

            // sync physics
            body.Position = PositionSim;
            body.Rotation = Rotation;
            _animationCogwheel.CurrentPosition = PositionSim;
            _animationCogwheel.Angle = Rotation;
            _animationCogwheel.Update(GameDemo1.GameTime);
            _animationCogwheel.Draw();

   
        } // Draw()
        #endregion
    } // class Enemy
} // namespace PanzerAdmiral.Demos
