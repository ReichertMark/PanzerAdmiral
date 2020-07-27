using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.ScreenSystem;


namespace PanzerAdmiral.AI
{
    /// <summary> Helper for drawing 2D lines </summary>
    static class Line
    {
        /// <summary>
        /// Draws Lines between two actors (A and B)
        /// </summary>
        /// <param name="texture">Line Texture</param>
        /// <param name="color">Line Color</param>
        /// <param name="actorA">ActorA</param>
        /// <param name="actorB">ActorB</param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static void DrawLine(Texture2D texture, Color color, Actor actorA, Actor actorB, float layer)
        {
            // get actor positions
            Vector2 pointA = actorA.Position;
            Vector2 pointB = actorB.Position;

            // compute direction and normalize it
            Vector2 direction = pointA - pointB;

            if(direction != Vector2.Zero)
                direction.Normalize();

            pointA -= direction * actorA.Radius;
            pointB += direction * actorB.Radius;

            // Get Line Length and rotation value
            float length = (pointA - pointB).Length(); 
            float rotation = (float) Math.Atan2(direction.Y, direction.X);

            // get rect an its origin
            Rectangle rect = new Rectangle((int)(pointA.X + pointB.X) / 2,  // X
                                           (int)(pointA.Y + pointB.Y) / 2,  // Y
                                           (int)length,                     // width
                                           2);
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            // Draw the line sprite
            ScreenManager.SpriteBatch.Draw(texture, rect, null, color, rotation, origin, SpriteEffects.None, layer);


        } // DrawLine(texture, color, actorA, actorB, float layer)

    } // class Line
} // namespace PanzerAdmiral.AI.Editor
