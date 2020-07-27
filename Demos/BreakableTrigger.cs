using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PanzerAdmiral.Physics;
using FarseerPhysics.Dynamics;

namespace PanzerAdmiral.Demos
{
    /// <summary> Creates 3 breakables and moves them </summary>
    class BreakableTrigger : Trigger
    {
        PhysicBreakable breakable1;
        PhysicBreakable breakable2;
        PhysicBreakable breakable3;


        private float moveDownTimer = 5f;    // move down after 5 seconds
        private float deleteTimer = 10f;     // remove after 10 seconds

        private float TimeSinceEvent;        // Point of time when event is fired

        private bool eventTriggered = false; // is the Event already triggered?

        #region Constructor
        /// <summary>
        /// Create breakables and Trigger
        /// </summary>
        /// <param name="setPosition">Breakable Trigger Position</param>
        /// <returns></returns>
        public BreakableTrigger(Vector2 setPosition)
            : base(setPosition)
        {
            Vector2 spawnPosition = setPosition;
            spawnPosition.X += 25;

            breakable1 = new PhysicBreakable(spawnPosition, GameSettings.ShieldHitpoints, "Enemies/plakat1", "Enemies/plakat1_c", false);
            IgnoreVehicleAndLevelCollision(breakable1.body);
            breakable1.body.IgnoreGravity = true;
            breakable1.DeleteAfter = 1.0f;
            breakable1.Explode = false;     // don't explode!
            GameDemo1.physicBreakables.Add(breakable1);

            spawnPosition.X += 10;
            breakable2 = new PhysicBreakable(spawnPosition, GameSettings.ShieldHitpoints, "Enemies/plakat1", "Enemies/plakat1_c", false);
            IgnoreVehicleAndLevelCollision(breakable2.body);
            breakable2.body.IgnoreGravity = true;
            breakable2.DeleteAfter = 1.0f;
            breakable2.Explode = false;     // don't explode!
            GameDemo1.physicBreakables.Add(breakable2);

            spawnPosition.X += 10;
            breakable3 = new PhysicBreakable(spawnPosition, GameSettings.ShieldHitpoints, "Enemies/plakat1", "Enemies/plakat1_c", false);
            IgnoreVehicleAndLevelCollision(breakable3.body);
            breakable3.body.IgnoreGravity = true;
            breakable3.DeleteAfter = 1.0f;
            breakable3.Explode = false;     // don't explode!
            GameDemo1.physicBreakables.Add(breakable3);

        } // BreakableTrigger(setPosition)
        #endregion

        #region DoEventLogic
        /// <summary> DoEventLogic </summary>
        public override void DoEventLogic()
        {
            eventTriggered = true;

            // animate and ignore gravity if we are not broken!
            if (breakable1._broken == false)
                breakable1.Animate = true;
            if (breakable2._broken == false)
                breakable2.Animate = true;
            if (breakable3._broken == false)
                breakable3.Animate = true;

            // Draw debug
            //base.DoEventLogic();
        }
        #endregion

        #region Update
        /// <summary> Updates the move and remove timer </summary>
        public override void Update()
        {
          
            if (eventTriggered == true)
            {
                // move
                TimeSinceEvent += (float)GameDemo1.GameTime.ElapsedGameTime.TotalSeconds; //Zeit seit letztem Update dazuaddieren
                if (TimeSinceEvent >= moveDownTimer) //Wenn die vergangene Zeit größer ist als der Timer, do...
                {
                    if (breakable1._broken == false)
                    {
                        Vector2 pos = breakable1.body.Position;
                        breakable1.body.Position = new Vector2(pos.X, pos.Y + 0.05f);
                        breakable1.body.Rotation = MathHelper.ToRadians(0);//0;
                        //breakable1._breakableBody.MainBody.AngularVelocity = 0;// MathHelper.ToRadians(0);
                    }

                    if (breakable2._broken == false)
                    {
                        Vector2 pos = breakable2.body.Position;
                        breakable2.body.Position = new Vector2(pos.X, pos.Y + 0.05f);
                        breakable2.body.Rotation = MathHelper.ToRadians(0); //0;
                        breakable2.body.AngularVelocity = 0;
                    }

                    if (breakable3._broken == false)
                    {
                        Vector2 pos = breakable3.body.Position;
                        breakable3.body.Position = new Vector2(pos.X, pos.Y + 0.05f);
                        breakable3.body.Rotation = MathHelper.ToRadians(0);//0;
                        breakable3.body.AngularVelocity = 0;
                    }
                } // if (TimeSinceEvent >= moveDownTimer)

                // delete
                if (TimeSinceEvent >= deleteTimer)
                {
                    breakable1.Delete();
                    breakable2.Delete();
                    breakable3.Delete();
                    Triggers.Remove(this);
                } //  if (TimeSinceEvent >= deleteTimer)


            } //  if (eventTriggered == true)

        } // Update()
        #endregion

        #region IgnoreVehicleAndLevelCollision
        /// <summary>
        /// Ignore Vehicle Collision and Level Collision
        /// </summary>
        /// <param name="body">The body to ignore the collision with</param>
        private void IgnoreVehicleAndLevelCollision(Body body)
        {
            // Ignore Vehicle Collision
            body.IgnoreCollisionWith(GameDemo1.vehicle.Body);
            body.IgnoreCollisionWith(GameDemo1.vehicle.WheelBackBody);
            body.IgnoreCollisionWith(GameDemo1.vehicle.WheelFrontBody);
            body.IgnoreCollisionWith(GameDemo1.vehicle.cannon.Body);

            // Ignore Level Collision
            foreach (PhysicTexture tex in GameDemo1.physicTextures)
                body.IgnoreCollisionWith(tex.body);
        } // IgnoreVehicleAndLevelCollision(body)
        #endregion

    } // class BreakableTrigger
} // namespace PanzerAdmiral.Demos
