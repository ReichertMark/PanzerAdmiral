using PanzerAdmiral.AI;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Helpers;
using PanzerAdmiral.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PanzerAdmiral.Physics;
using System.Collections.Generic;
using PanzerAdmiral.Sounds;
using PanzerAdmiral.Particles;

namespace PanzerAdmiral.Demos
{
    /// <summary> 
    /// Bombing Enemy, just wanders in the left direction and throws some bombs if the player is in Sightradius
    /// Deletes Shots that collided with the ground or the player (see Shot.cs OnCollision)
    /// </summary>
    public class BombingEnemy : Actor
    {
        public int HitPoints;
        public float SightRadius = 600; //500; //256;//128;
        public Body body;               // Farseer Body
        float density = 1.0f;
        
        List<Shot> Shots = new List<Shot>();
        private float ShotTimer = 400f;//350f; //Shussintervall
        private float TimeSinceShot; //Vergangene Zeit seit letztem Schuss
        private int loopCount = 0;

        #region Constructor
        public BombingEnemy(Vector2 setPosition, int setHitPoints)
            : base("Enemies/airship", Color.White)
        {
            // Assign hitpoints and create rectangle from Texture
            HitPoints = setHitPoints;
            body = BodyFactory.CreateRectangle(PhysicsGameScreen.World, ConvertUnits.ToSimUnits(Texture.Width), ConvertUnits.ToSimUnits(Texture.Height), density);
            body.UserData = "enemy";

            // set Position and start navigation
            Position = setPosition;
            BehaviorList.Add(new BehaviorConstant(0.2f, new Vector2(-1, 0)));
        } // BombingEnemy(setPosition, setHitPoints)
        #endregion

        #region Update
        /// <summary> Update shots by player and player hitpoints</summary>
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
                    GameSettings.HighScore += (uint)GameSettings.BombingEnemyScore;
                    FadeUp.AddTextMessage("+ " + GameSettings.BombingEnemyScore, FadeUp.TextTypes.Score, body.Position); //GameDemo1.vehicle.Body.Position);
                    Sound.Play(Sound.Sounds.Explosion);
                    GameDemo1.particleManager.AddExplosion(body.Position);


                    body.UserData = "disable";
                    Delete();

                    // also spawn a pickup
                    new PickUp("AI/health", body.Position, 2.0f);

                    //body.Dispose();
                } // if (HitPoints <= 0)

            } //  if ("damage")

            // throw bombs if the player's position and the enemy's position is lesser than the player's radius and the enemy's sightradius
            if (Vector2.Distance(GameDemo1.vehicle.Position, Position) < GameDemo1.vehicle.Radius + SightRadius)
            {

                TimeSinceShot += (float)GameDemo1.GameTime.ElapsedGameTime.TotalMilliseconds;//TotalSeconds; //Zeit seit letztem Update dazuaddieren
                if (TimeSinceShot >= ShotTimer) //Wenn die vergangene Zeit größer ist als der Timer, do...
                {
                    Shot shot = new Shot(true);
                    shot.isBomb = true;
                    shot.shotBody.Position = PositionSim + new Vector2(0, 3);   // add a little offset
                    Shots.Add(shot);

                    TimeSinceShot -= ShotTimer; // Resette den Timer.
                } // if (TimeSinceShot >= ShotTimer)
            } // if (Vector2.Distance)
   
            // loop all shots
            for (int i = 0; i < Shots.Count; i++)
            {
                // Draw particles every fourth frame (performance)
                if (loopCount % 4 == 0)
                {
                    loopCount = 0;
                    GameDemo1.particleManager.AddEnemyBombParticles(Shots[i].shotBody);
                }

                // remove disabled user Shots
                if (Shots[i].shotBody.UserData.ToString() == "disable")
                {
                    GameDemo1.particleManager.AddExplosion(Shots[i].shotBody.Position);
                    Sound.Play(Sound.Sounds.Explosion);

                    Shots[i].shotBody.Dispose();
                    Shots.Remove(Shots[i]);
                }   // for if

            } // for (i < Shots.Count)


            // count the update loops
            loopCount++;

            // Update actor
            base.Update();

        } // Update()
        #endregion

        #region Delete
        /// <summary> Releases all shots </summary>
        public override void Delete()
        {
            for (int i = 0; i < Shots.Count; i++)
            {
                Shots[i].shotBody.Dispose();
            }
            Shots.Clear();
            body.Dispose();
            base.Delete();
        } // Delete()
        #endregion

        #region Draw
        /// <summary> Override Draw , Draws SightRadius if Y Key is pressed</summary>
        public override void Draw()
        {
            // debug Sightradius
            if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Y))
            {
                DrawCircle(SightRadius);
            }

            // Draw all shots
            foreach (Shot shot in Shots)
                shot.Draw();

            // TODO: Draw Animation
            base.Draw();


            // sync physics
            body.Position = PositionSim;
            body.Rotation = Rotation;

        } // Draw()
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
            ScreenManager.SpriteBatch.Draw(texture, this.Position, null, color, 0f, origin, scale, SpriteEffects.None, 1f);
        } // DrawCircle(radius)
        #endregion

    } // class BombingEnemy
} // namespace PanzerAdmiral.Demos
