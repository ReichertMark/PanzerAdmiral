using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Demos;
using Microsoft.Xna.Framework;
using PanzerAdmiral.Helpers;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;

namespace PanzerAdmiral.Particles
{
    /// <summary> Manages all Particles and has helper functions to add Particles to specific particlesystems </summary>
    public class ParticleSystemManager
    {
        #region Variables
        // Explosion particles
        ExplosionParticleSystem explosion;
        ExplosionSmokeParticleSystem explosionSmoke;


        // TankSmokePlume particles
        TankSmokePlume smokePlume;
        // keep a timer that will tell us when it's time to add more particles to the tank smoke plume. This gets changed in the Vehicle class
        public static float DefaultTimeBetweentankSmokePuffs = 1.0f;
        public float TimeBetweenTankSmokePuffs = DefaultTimeBetweentankSmokePuffs;
        public float timeTillTankPuff = 0.0f;

        
        TankFire tankFire;
        TankFireSmoke tankFireSmoke;

        // Cannon Smoke and fire
        CannonSmoke cannonSmoke;
        CannonFire cannonFire;

        EnemyCannonFire enemyCannonFire;
        EnemyCannonSmoke enemyCannonSmoke;

        // Bomb smoke and fire
        BombFire bombFire;
        BombSmoke bombSmoke;


        public Vector2 exhaustPosition;

        #endregion

        #region Constructor
        /// <summary> Create Particle System Components </summary>
        public ParticleSystemManager()
        {
            // create the particle systems and add them to the components list.
            // we should never see more than one explosion at once
            explosion = new ExplosionParticleSystem(1);

            // but the smoke from the explosion lingers a while.
            explosionSmoke = new ExplosionSmokeParticleSystem(2);

            // we'll see lots of these effects at once; this is ok because they have a fairly small number of particles per effect.
            smokePlume = new TankSmokePlume(20);

            cannonSmoke = new CannonSmoke(15);
            cannonFire = new CannonFire(30);

            tankFire = new TankFire(5);
            tankFireSmoke = new TankFireSmoke(6);

            enemyCannonSmoke = new EnemyCannonSmoke(15);//(15);
            enemyCannonFire = new EnemyCannonFire(30);//(30);

            bombFire = new BombFire(50);
            bombSmoke = new BombSmoke(70);

        } // ParticleSystemManager()
        #endregion

        #region AddExplosion
        /// <summary>
        ///the overall explosion effect is actually comprised of two particle
        /// systems: the fiery bit, and the smoke behind it. add particles to
        /// both of those systems.
        /// </summary>
        /// <param name="where">set Position</param>
        public void AddExplosion(Vector2 where)
        {
            where = PhysicsGameScreen.Camera.ConvertWorldToScreen(where);
            explosion.AddParticles(where);
            explosionSmoke.AddParticles(where);
        } // AddExplosion(Position)
        #endregion

        #region AddCannonSmoke
        /// <summary>
        /// Adds cannnon smoke and fire for player shots
        /// </summary>
        /// <param name="where">set Position</param>
        public void AddCannonSmoke(Vector2 where)
        {
            where = PhysicsGameScreen.Camera.ConvertWorldToScreen(where);
            cannonSmoke.AddParticles(where);
            cannonFire.AddParticles(where);
        } // AddCannonSmoke(Position)
        #endregion

        #region AddEnemyCannonParticles
        /// <summary>
        /// Adds cannon Smoke and Fire particles for enemy Shots
        /// </summary>
        /// <param name="body">set body, rotation is used</param>
        public void AddEnemyCannonParticles(Body body)
        {
            // get the rotation and rotate 90 degrees
            float radians = body.Rotation - MathHelper.PiOver2;

            // create a little cone
            radians += Extensions.RandomBetween(MathHelper.ToRadians(-5), MathHelper.ToRadians(5));

            Vector2 direction = Vector2.Zero;
            // from the unit circle, cosine is the x coordinate and sine is the y coordinate. 
            direction.X = (float)Math.Cos(radians);
            direction.Y = (float)Math.Sin(radians);

            Vector2 where = PhysicsGameScreen.Camera.ConvertWorldToScreen(body.Position);
            enemyCannonFire.AddParticles(where, direction);
            enemyCannonSmoke.AddParticles(where, direction);
        } // AddEnemyCannonParticles(body)
        #endregion

        #region AddEnemyBombParticles
        /// <summary>
        /// Adds Enemy Smoke and Fire particles for enemy Bombs
        /// </summary>
        /// <param name="body">set body, rotation is used</param>
        public void AddEnemyBombParticles(Body body)
        {
            // get the rotation and rotate 90 degrees
            float radians = body.Rotation - MathHelper.PiOver2;

            // create a little cone
            radians += Extensions.RandomBetween(MathHelper.ToRadians(-5), MathHelper.ToRadians(5));

            Vector2 direction = Vector2.Zero;
            // from the unit circle, cosine is the x coordinate and sine is the y coordinate. 
            direction.X = (float)Math.Cos(radians);
            direction.Y = (float)Math.Sin(radians);

            Vector2 where = PhysicsGameScreen.Camera.ConvertWorldToScreen(body.Position);
            bombFire.AddParticles(where, direction);
            bombSmoke.AddParticles(where, direction);
        } // AddEnemyCannonParticles(body)
        #endregion

        #region Update
        /// <summary> Updates all particlesystems </summary>
        public void Update()
        {
            // Update all the Particle Systems
            explosion.Update(GameDemo1.GameTime);
            cannonSmoke.Update(GameDemo1.GameTime);
            smokePlume.Update(GameDemo1.GameTime);
            explosionSmoke.Update(GameDemo1.GameTime);
            cannonFire.Update(GameDemo1.GameTime);
            tankFire.Update(GameDemo1.GameTime);
            tankFireSmoke.Update(GameDemo1.GameTime);

            enemyCannonFire.Update(GameDemo1.GameTime);
            enemyCannonSmoke.Update(GameDemo1.GameTime);

            bombFire.Update(GameDemo1.GameTime);
            bombSmoke.Update(GameDemo1.GameTime);    

            float dt = (float)GameDemo1.GameTime.ElapsedGameTime.TotalSeconds;
            UpdateTankParticles(dt);
        } // Update()
        #endregion

        #region UpdateTankParticles
        /// <summary>
        /// this function is called when we want to demo the smoke plume effect. it
        /// updates the timeTillPuff timer, and adds more particles to the plume when necessary.
        /// </summary>
        /// <param name="dt">time since last frame</param>
        private void UpdateTankParticles(float dt)
        {
            timeTillTankPuff -= dt;
            if (timeTillTankPuff < 0)
            {
                // add more particles
//                 Vector2 where = PhysicsGameScreen.Camera.ConvertWorldToScreen(GameDemo1.vehicle.smokeBody.Position);
//                 smokePlume.AddParticles(where);
                Vector2 vehiclePos = GameDemo1.vehicle.Body.Position;

                Vector2 offset = new Vector2(-2.3f, -2.1f);
                Matrix rotation = Matrix.CreateRotationZ(GameDemo1.vehicle.Body.Rotation);
                offset = Vector2.Transform(offset, rotation);
                exhaustPosition = vehiclePos + offset;
                smokePlume.AddParticles(PhysicsGameScreen.Camera.ConvertWorldToScreen(exhaustPosition));

                // and then reset the timer.
                timeTillTankPuff = TimeBetweenTankSmokePuffs;
            }

            // Draw some smoke if we have less than 75 per cent health
            if (HealthBar.currentHealthInPercent < 0.75)
            {
                Vector2 pos = PhysicsGameScreen.Camera.ConvertWorldToScreen(GameDemo1.vehicle.Body.Position + new Vector2(0, -1));
             
                
                tankFireSmoke.AddParticles(pos);

                // Draw also some fire if we have less than 50 per cent health
                if (HealthBar.currentHealthInPercent < 0.50)
                    tankFire.AddParticles(pos);
            } // if (currentHealthInPercent < 0.75)
        } // UpdateTankParticles(float dt)
        #endregion

        #region Draw
        /// <summary> Draw all particle systems </summary>
        public void Draw()
        {
            // Begin drawing in Screenspace
            ScreenManager.SpriteBatch.End();

            // first draw the smoke stuff
            cannonSmoke.Draw(GameDemo1.GameTime);
            cannonFire.Draw(GameDemo1.GameTime);

            smokePlume.Draw(GameDemo1.GameTime);

            explosionSmoke.Draw(GameDemo1.GameTime);
            explosion.Draw(GameDemo1.GameTime);

            tankFireSmoke.Draw(GameDemo1.GameTime);
            tankFire.Draw(GameDemo1.GameTime);

            enemyCannonSmoke.Draw(GameDemo1.GameTime);
            enemyCannonFire.Draw(GameDemo1.GameTime);

            bombSmoke.Draw(GameDemo1.GameTime);
            bombFire.Draw(GameDemo1.GameTime);

            // reset old Spritebatch state
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, PhysicsGameScreen.Camera.View);

        } // Draw()
        #endregion
    } // class ParticleSystemManager
} // namespace PanzerAdmiral.Particles
