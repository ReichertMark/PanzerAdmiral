

using System.Collections.Generic;
using PanzerAdmiral.Graphics;
using Microsoft.Xna.Framework;

namespace PanzerAdmiral.Demos
{
    /// <summary> Used to initialize and Draw animated Objects in the Background </summary>
    public class AnimatedObject
    {

        public static List<AnimatedObject> Objects = new List<AnimatedObject>();
        Animation animation;

        public AnimatedObject(Vector2 setPosition, float setScale, Animation setAnimation)
        {
            // Create animation
            animation = setAnimation; 
            animation.IsAlive = true;
            animation.Scale = setScale;

            animation.CurrentPosition = setPosition;
            animation.Angle -= MathHelper.PiOver2;  // rotate

            Objects.Add(this);
        } // BackgroundObject(setPosition)

        /// <summary> Update and Draws the Animation </summary>
        public void Draw()
        {
            animation.Update(GameDemo1.GameTime);
            animation.Draw();
        } // Draw()

    } // class BackgroundObject
} // namespace PanzerAdmiral.Demos
