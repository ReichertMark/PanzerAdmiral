using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;

namespace PanzerAdmiral.Physics
{
    public class Explosion
    {
        /// <summary>
        /// Creates an explosion at an arbitary point in Simulation space
        /// </summary>
        /// <param name="setPosition">set explosion Position, must have an offset when using Body.Position, else Divide by zero error</param>
        /// <param name="setStrength">set the Strength of the Explosion</param>
        /// <returns></returns>
        public Explosion(Vector2 setPosition, float setStrength)
        {

            // Create Axis Aligned Bounding Box, use 1 as X, else a Vehicle near exploding towers will fly away!
            Vector2 min = setPosition - new Vector2(1, 10);
            Vector2 max = setPosition + new Vector2(1, 10);
            FarseerPhysics.Collision.AABB aabb = new FarseerPhysics.Collision.AABB(ref min, ref max);

            PhysicsGameScreen.World.QueryAABB(fixture =>
            {
                Vector2 fv = fixture.Body.Position - setPosition;
                fv.Normalize();
                fv *= setStrength; //200;//40;
                fixture.Body.ApplyLinearImpulse(ref fv);
                return true;
            }, ref aabb);
        }
    } // class Explosion
} // namespace PanzerAdmiral.Physics
