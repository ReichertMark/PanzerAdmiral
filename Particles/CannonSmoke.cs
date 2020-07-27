using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PanzerAdmiral.Helpers;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.Demos;

namespace PanzerAdmiral.Particles
{
    public class CannonSmoke : ParticleSystem
    {
        public CannonSmoke(int howManyEffects)
            : base(howManyEffects)
        {
        }

        /// <summary>
        /// Set up the constants that will give this particle system its behavior and
        /// properties.
        /// </summary>
        protected override void InitializeConstants()
        {
            textureFilename = "Particles/smoke";

            minInitialSpeed = 50;
            maxInitialSpeed = 200;

            // we don't want the particles to accelerate at all, aside from what we
            // do in our overriden InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            // short lifetime, this can be changed to create thinner or thicker smoke.
            // tweak minNumParticles and maxNumParticles to complement the effect.
            minLifetime = 0.2f;//0.5f; //1.0f;
            maxLifetime = 0.5f;//1.0f; //2.5f;

            minScale = 0.05f;
            maxScale = 0.1f;

            // we need to reduce the number of particles on Windows Phone in order to keep a good framerate

            minNumParticles = 30;
            maxNumParticles = 60;

            // rotate slowly, we want a fairly relaxed effect
            minRotationSpeed = -MathHelper.PiOver4 / 2.0f;
            maxRotationSpeed = MathHelper.PiOver4 / 2.0f;

			blendState = BlendState.AlphaBlend;

            //DrawOrder = AlphaBlendDrawOrder;
        }

        /// <summary>
        /// PickRandomDirection is overriden so that we can make the particles always 
        /// move along the cannon angle.
        /// </summary>
        /// <returns>a random direction which points basically up.</returns>
        protected override Vector2 PickRandomDirection()
        {
            float radians = GameDemo1.vehicle.cannon.Body.Rotation - MathHelper.PiOver2;

            // create a little cone
            radians += Extensions.RandomBetween(MathHelper.ToRadians(-5), MathHelper.ToRadians(5));

            Vector2 direction = Vector2.Zero;
            // from the unit circle, cosine is the x coordinate and sine is the y coordinate. 
            direction.X = (float)Math.Cos(radians);
            direction.Y = (float)Math.Sin(radians);
            return direction;
        } // PickRandomDirection()

        /// <summary>
        /// InitializeParticle is overridden to add the appearance of wind.
        /// </summary>
        /// <param name="p">the particle to set up</param>
        /// <param name="where">where the particle should be placed</param>
        protected override void InitializeParticle(Particle p, Vector2 where)
        {
            base.InitializeParticle(p, where);

            p.Acceleration.Y -= Extensions.RandomBetween(10, 30);
        }
    }
} // namespace PanzerAdmiral.Particles
