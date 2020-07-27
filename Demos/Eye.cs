using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Helpers;
using PanzerAdmiral.Graphics;

namespace PanzerAdmiral.Demos
{
    public class Eye
    {
        #region Variables
        public static List<Eye> Eyes = new List<Eye>();
        //Texture2D eyeTexture;
        public float Scale = 0.5f;
        Vector2 position;
        Vector2 ZeroDegreeVectorByDanielLeusch = new Vector2(0, -1);
        public float Rotation;

        Animation animation;
        #endregion

        #region Constructor
        /// <summary>
        /// Create Eye, always looks at player
        /// </summary>
        /// <param name="setPosition">set Position</param>
        /// <param name="setAnimation">set Animation</param>
        /// <returns></returns>
        public Eye(Vector2 setPosition, float setScale, Animation setAnimation)
        {
            position = setPosition;
            // Create animation
            animation = setAnimation;
            animation.IsAlive = true;
            animation.Scale = setScale;

            animation.CurrentPosition = setPosition;
            Eyes.Add(this);
        } // Eye(setTexture, setPosition)
        #endregion

        #region Update
        /// <summary> Update just computes the Angle between two Vectors </summary>
        public void Update()
        {
            Vector2 distance = GameDemo1.vehicle.Body.Position - position;
            distance.Normalize();

            float Angle = (float)Math.Acos(Vector2.Dot(distance, ZeroDegreeVectorByDanielLeusch) /
                                          distance.Length() * ZeroDegreeVectorByDanielLeusch.Length());//);

            Angle = MathHelper.ToDegrees(Angle);

            if (distance.X > 0)
                Rotation = MathHelper.ToRadians(Angle);
            else
                Rotation = MathHelper.ToRadians(360 - Angle);

            animation.Angle = Rotation -MathHelper.ToRadians(90);
            animation.Update(GameDemo1.GameTime);

        } // Update()
        #endregion

        #region Draw
        /// <summary> Draw Eye </summary>
        public void Draw()
        {
            animation.Draw();
        } // Draw()
        #endregion
    } // class Eye
} // namespace PanzerAdmiral.Demos