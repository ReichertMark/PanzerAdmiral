

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PanzerAdmiral.Helpers
{
    public static class Extensions
    {
        public static Vector2 CalculateSize(this IEnumerable<Vector2> source)
        {
            return new Vector2(
                source.Max(v => v.X) - source.Min(v => v.X),
                source.Max(v => v.Y) - source.Min(v => v.Y)
            );
        }

        // a random number generator that the whole sample can share.
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        //  a handy little function that gives a random float between two
        // values. This will be used in several places in the sample, in particular in
        // ParticleSystem.InitializeParticle.
        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }


    }
}
