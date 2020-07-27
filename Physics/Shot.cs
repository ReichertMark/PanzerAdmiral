using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;
using FarseerPhysics.Factories;
using PanzerAdmiral.Helpers;
using PanzerAdmiral.Demos;
using PanzerAdmiral.Graphics;

namespace PanzerAdmiral.Physics
{
    /// <summary>
    /// Shot Class
    /// UserData PlayerShot = "s"
    /// UserData EnemyShot = enemyShot
    /// </summary>
    class Shot
    {
        #region Variables
        public Body shotBody;
        private Vector2 _origin;
        private static Texture2D _shotTexture = ScreenManager.Content.Load<Texture2D>("Samples/shot");
        private static Texture2D _bombTexture = ScreenManager.Content.Load<Texture2D>("Samples/bomb");
        private Vector2 _shotDirection;
        private Vector2 shotScreen;
        private float Scale = 0.2f;
        float density = 1.0f;
        float mass = 20;
        float shotForce = 600.0f;
        public bool fired = false;
        public bool isEnemyShot;
        public bool isBomb = false;
        private float shotLifetime = 1.5f;
        public  float shotTimer;
        public bool shouldbedisposed = false;
        #endregion

        #region Constructor
        public Shot(bool isEnemyShot)
        {
            this.isEnemyShot = isEnemyShot;
            shotBody = BodyFactory.CreateRectangle(PhysicsGameScreen.World, ConvertUnits.ToSimUnits(_shotTexture.Width) * Scale,
                                   ConvertUnits.ToSimUnits(_shotTexture.Height) * Scale, density);

            _origin = new Vector2(_shotTexture.Width / 2, _shotTexture.Height / 2);

            shotBody.BodyType = BodyType.Dynamic;
            shotBody.Mass = mass;

            if (!isEnemyShot)
            {

                shotBody.UserData = "s";
                shotBody.Position = GameDemo1.vehicle.cannon.Body.Position;//new Vector2(GameDemo1.vehicle.Body.Position.X + 2f,  GameDemo1.vehicle.Body.Position.Y);
            }
            else
            {
                shotBody.UserData = "enemyShot";
                shotBody.Mass = 1f;
            }

           
            shotBody.OnCollision += new OnCollisionEventHandler(shot_OnCollision);
        }
        #endregion

        #region shot_OnCollision
        bool shot_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureB.Body.UserData != null)
            {
                // if EnemyTower gets hit by player shot
                if (fixtureB.Body.UserData.ToString() == "enemyTower" && 
                    fixtureA.Body.UserData.ToString() == "s")
                {
                    //FadeUp.AddTextMessage("-" + GameSettings.PlayerDamage, FadeUp.TextTypes.Damage, fixtureB.Body.Position);
                    fixtureA.Body.UserData = "disable";     // Disable user Shot!
                    fixtureB.Body.UserData = "damage";      // but save the damage the tower will receive
                    
                }

                // if flying enemy gets hit by player shot
                if (fixtureB.Body.UserData.ToString() == "enemy" &&
                    fixtureA.Body.UserData.ToString() == "s")
                {
                    //FadeUp.AddTextMessage("-" + GameSettings.PlayerDamage, FadeUp.TextTypes.Damage, fixtureB.Body.Position);
                    fixtureA.Body.UserData = "disable";     // Disable user Shot!
                    fixtureB.Body.UserData = "damage";
                }

                // Disable Player Shots if they collide with the levelparts
                 if (fixtureB.Body.UserData.ToString() == "physictexture" &&
                    fixtureA.Body.UserData.ToString() == "s")
                {
                    fixtureA.Body.UserData = "disable";     // disable damage for this shot
                }

                // if the player gets shot by EnemyTowers
                if (fixtureB.Body.UserData.ToString() == "player" &&
                    fixtureA.Body.UserData.ToString() == "enemyShot")
                {
                    FadeUp.AddTextMessage("-" + GameSettings.TowerDamage, FadeUp.TextTypes.Damage, fixtureB.Body.Position);
                    fixtureB.Body.UserData = "getroffen";   // Enable damage
                    fixtureA.Body.UserData = "disable";     // disable damage for this shot
                }

                // Disable enemy Shots if they collide with the levelparts
                if (fixtureB.Body.UserData.ToString() == "physictexture" &&
                    fixtureA.Body.UserData.ToString() == "enemyShot")
                {
                    fixtureA.Body.UserData = "disable";     // disable damage for this shot
                }

                // Disable player Shots if they collide with the Triggers
                else if (fixtureB.Body.UserData.ToString() == "trigger" &&
                         fixtureA.Body.UserData.ToString() == "s")
                {
                    return false;
                }

                else if (fixtureB.Body.UserData.ToString() == "trigger" &&
                         fixtureA.Body.UserData.ToString() == "disable")
                {
                    return false;
                }
            }
            return true;

        }
        #endregion

        #region Update
        public void Update()
        {
            // Player shot
            if (!isEnemyShot)
            {
                Vector2 cursorPosScreen = PhysicsGameScreen.Camera.ConvertScreenToWorld(ScreenManager.Input.Cursor);
                shotScreen = shotBody.Position;// center;

                _shotDirection = cursorPosScreen - shotScreen;
                _shotDirection.Normalize();

                if (ScreenManager.Input.IsNewMouseButtonPress(MouseButtons.LeftButton) &&
                    shotBody.UserData.ToString() == "s" &&
                    fired == false)
                {
                    shotBody.ApplyLinearImpulse(_shotDirection * shotForce);
                    if (GameDemo1.vehicle.Body.LinearVelocity.X >= 0f)
                        shotBody.LinearVelocity = shotBody.LinearVelocity + GameDemo1.vehicle.Body.LinearVelocity;

                    fired = true;
                }

                if (shotBody.UserData.ToString() == "disable")
                    shotTimer += (float)GameDemo1.GameTime.ElapsedGameTime.TotalSeconds;

                if (shotBody.UserData.ToString() == "disable" && shotTimer >= shotLifetime)
                    shouldbedisposed = true;

            }
            else
            {
                // Enemy Shot is handled in PhysicBreakable?
            }
    
        } // Update
        #endregion

        #region Draw
        /// <summary> Draw Shot </summary>
        public void Draw()
        {
            Texture2D texture;
            if (isBomb)
            {
                texture = _bombTexture;
                Scale = 0.4f;
            }
            else
                texture = _shotTexture;

            ScreenManager.SpriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(shotBody.Position), null,
                                           Color.White, shotBody.Rotation, _origin, Scale, SpriteEffects.None, 0f);
        } // Draw()
        #endregion
    } // class Shot
} // namespace PanzerAdmiral.Physics
