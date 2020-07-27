using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PanzerAdmiral.Particles
{
    /// <summary>
    /// ExplosionParticleSystem is a specialization of ParticleSystem which creates a
    /// fiery explosion. It should be combined with ExplosionSmokeParticleSystem for
    /// best effect.
    /// </summary>
    public class ExplosionParticleSystem : ParticleSystem
    {
        public ExplosionParticleSystem(int howManyEffects)
            : base(howManyEffects)
        {
        }

        #region InitializeConstants
        /// <summary>
        /// Set up the constants that will give this particle system its behavior and
        /// properties.
        /// </summary>
        protected override void InitializeConstants()
        {
            textureFilename = "Particles/explosion";

            // high initial speed with lots of variance.  make the values closer
            // together to have more consistently circular explosions.
            minInitialSpeed = 40;
            maxInitialSpeed = 250;//500;

            // doesn't matter what these values are set to, acceleration is tweaked in
            // the override of InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            // explosions should be relatively short lived
            minLifetime = .5f;
            maxLifetime = 1.0f;

            minScale = 0.2f;//.3f;
            maxScale = 0.4f; //1.0f;

            // we need to reduce the number of particles on Windows Phone in order to keep a good framerate
            minNumParticles = 20;
            maxNumParticles = 30;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            // additive blending is very good at creating fiery effects.
            blendState = BlendState.Additive;

            //DrawOrder = AdditiveDrawOrder;
        } // InitializeConstants()
        #endregion

        #region InitializeParticle
        /// <summary>
        /// The base works fine except for acceleration. Explosions move outwards,
        /// then slow down and stop because of air resistance. Let's change
        /// acceleration so that when the particle is at max lifetime, the velocity
        /// will be zero.
        /// </summary>
        /// <param name="p">set Particle</param>
        /// <param name="where">set position</param>
        protected override void InitializeParticle(Particle p, Vector2 where)
        {
            base.InitializeParticle(p, where);

            // We'll use the equation vt = v0 + (a0 * t). (If you're not familar with
            // this, it's one of the basic kinematics equations for constant
            // acceleration, and basically says:
            // velocity at time t = initial velocity + acceleration * t)
            // We'll solve the equation for a0, using t = p.Lifetime and vt = 0.
            p.Acceleration = -p.Velocity / p.Lifetime;
        }
        #endregion
    } // class ExplosionParticleSystem
} // namespace PanzerAdmiral.Particles
