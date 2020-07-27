using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PanzerAdmiral.Particles
{
    class EnemyCannonSmoke : ParticleSystem
    {

        public EnemyCannonSmoke(int howManyEffects)
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
            textureFilename = "Particles/smoke";

            minInitialSpeed = 150;//50;
            maxInitialSpeed = 300;//200;

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
        } // InitializeConstants()
        #endregion

    } // class EnemyCannonSmoke
} // namespace PanzerAdmiral.Particles
