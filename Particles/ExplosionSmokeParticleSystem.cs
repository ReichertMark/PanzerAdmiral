using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PanzerAdmiral.Particles
{
    /// <summary>
    /// ExplosionSmokeParticleSystem is a specialization of ParticleSystem which
    /// creates a circular pattern of smoke. It should be combined with
    /// ExplosionParticleSystem for best effect.
    /// </summary>
    public class ExplosionSmokeParticleSystem : ParticleSystem
    {
        public ExplosionSmokeParticleSystem(int howManyEffects)
            : base(howManyEffects)
        {
        }

        #region InitializeConstants
        /// <summary>
        /// Set up the constants that will give this particle system its behavior and properties.
        /// </summary>
        protected override void InitializeConstants()
        {
            textureFilename = "Particles/smoke";

            // less initial speed than the explosion itself
            minInitialSpeed = 20;
            maxInitialSpeed = 80; //200;

            // acceleration is negative, so particles will accelerate away from the
            // initial velocity.  this will make them slow down, as if from wind
            // resistance. we want the smoke to linger a bit and feel wispy, though,
            // so we don't stop them completely like we do ExplosionParticleSystem
            // particles.
            minAcceleration = -10;
            maxAcceleration = -50;

            // explosion smoke lasts for longer than the explosion itself, but not
            // as long as the plumes do.
            minLifetime = 1.0f;
            maxLifetime = 2.5f;

            minScale = 0.3f;//1.0f;
            maxScale = 0.9f; //2.0f;

            // we need to reduce the number of particles on Windows Phone in order to keep a good framerate
            minNumParticles = 10;
            maxNumParticles = 20;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            blendState = BlendState.AlphaBlend;

            //DrawOrder = AlphaBlendDrawOrder;
        } // InitializeConstants()
        #endregion
    } // class ExplosionSmokeParticleSystem
} // namespace PanzerAdmiral.Particles
