using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PanzerAdmiral.Helpers;
using PanzerAdmiral.Demos;

namespace PanzerAdmiral.Particles
{
    class TankFireSmoke : ParticleSystem
    {
                // wind force
        float windForce = 20;
        // wind in X direction, depends on linear velocity of Vehicle
        float xWind;

        #region Constructor
        /// <summary>
        /// Create TankSmokePlume
        /// </summary>
        /// <param name="game">must set a game for components</param>
        /// <param name="howManyEffects">how many effects should the PS have?</param>
        /// <returns></returns>
        public TankFireSmoke(int howManyEffects)
            : base(howManyEffects)
        {
        }
        #endregion

        #region InitializeConstants
        /// <summary>
        /// Set up the constants that will give this particle system its behavior and properties.
        /// </summary>
        protected override void InitializeConstants()
        {
            textureFilename = "Particles/smoke";

            minInitialSpeed = 40;
            maxInitialSpeed = 100;

            // we don't want the particles to accelerate at all, aside from what we
            // do in our overriden InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            // long lifetime, this can be changed to create thinner or thicker smoke.
            // tweak minNumParticles and maxNumParticles to complement the effect.
            minLifetime = 0.2f;
            maxLifetime = 0.3f;

            minScale = 0.05f;
            maxScale = 0.1f;

            // we need to reduce the number of particles on Windows Phone in order to keep
            // a good framerate
            minNumParticles = 6;
            maxNumParticles = 15;//15;


            // rotate slowly, we want a fairly relaxed effect
            minRotationSpeed = -MathHelper.PiOver4 / 2.0f;
            maxRotationSpeed = MathHelper.PiOver4 / 2.0f;

            blendState = BlendState.AlphaBlend;

            ParticleColor = new Color(120, 120, 120, 255);  // new Color(169, 169, 169, 255);  //Color.DarkGray;

            //DrawOrder = AlphaBlendDrawOrder;
        }
        #endregion

        #region PickRandomDirection
        /// <summary>
        /// PickRandomDirection is overriden so that we can make the particles always 
        /// move have an initial velocity pointing up.
        /// </summary>
        /// <returns>a random direction which points basically up.</returns>
        protected override Vector2 PickRandomDirection()
        {
            // Point the particles somewhere between 80 and 100 degrees.
            // tweak this to make the smoke have more or less spread.
            //float radians = Extensions.RandomBetween(MathHelper.ToRadians(80),  MathHelper.ToRadians(100));
            //float radians = Extensions.RandomBetween(MathHelper.ToRadians(90), MathHelper.ToRadians(180));
            float radians = Extensions.RandomBetween(MathHelper.ToRadians(85), MathHelper.ToRadians(95));//(100));

            // from the unit circle, cosine is the x coordinate and sine is the
            // y coordinate. We're negating y because on the screen increasing y moves down the monitor.
            Vector2 direction = Vector2.Zero;
            direction.X = (float)Math.Cos(radians);
            direction.Y = -(float)Math.Sin(radians);
            return direction;
        } // PickRandomDirection()
        #endregion

        #region InitializeParticle
        /// <summary>
        /// InitializeParticle is overridden to add the appearance of wind.
        /// </summary>
        /// <param name="p">the particle to set up</param>
        /// <param name="where">where the particle should be placed</param>
        protected override void InitializeParticle(Particle p, Vector2 where)
        {
            base.InitializeParticle(p, where);

            // the base is mostly good, but we want to simulate a little bit of wind heading to the left.
            xWind = GameDemo1.vehicle.Body.LinearVelocity.X * -1;


            p.Acceleration.X += xWind * windForce; //Extensions.RandomBetween(xWind * windForce, xWind * windForce);

        } // InitializeParticle(Particle, Position)
        #endregion

    } // class TankFireSmoke 
}  // namespace PanzerAdmiral.Particles
