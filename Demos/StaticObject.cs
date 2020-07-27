
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Helpers;

namespace PanzerAdmiral.Demos
{
    /// <summary> Used to initialize and Draw static Objects in the Background </summary>
    public class StaticObject
    {
        public static List<StaticObject> Objects = new List<StaticObject>();

        public Vector2 Position;
        public Vector2 Origin;
        public float Rotation;
        public float Scale = 1f;
        public Texture2D texture; 

        public StaticObject(string setTexture, Vector2 setPosition, float setScale)
        {
            texture = ScreenManager.Content.Load<Texture2D>(setTexture);
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Scale = setScale;
            Position = setPosition;

            Objects.Add(this);
        } // StaticObject(setTexture, setPosition, setScale)

        public void Draw()
        {
            ScreenManager.SpriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(Position), null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        } // Draw()

    } // class StaticObject
} // namespace PanzerAdmiral.Demos
