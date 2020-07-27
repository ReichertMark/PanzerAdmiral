
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.ScreenSystem;
using System;
using Microsoft.Xna.Framework;

namespace PanzerAdmiral.Graphics
{
    class Layer
    {
        public Texture2D[] Textures { get; private set; }
        public float ScrollRate { get; private set; }

        public Layer(string basePath, float scrollRate)
        {
            // Assumes each layer only has 3 segments.
            Textures = new Texture2D[3];
            for (int i = 0; i < 3; ++i)
                Textures[i] = ScreenManager.Content.Load<Texture2D>(basePath + "_" + i);

            ScrollRate = scrollRate;
        }

        public void Draw(SpriteBatch spriteBatch, float cameraPosition)
        {
            // Assume each segment is the same width.
            int segmentWidth = Textures[0].Width;


            // Calculate which segments to draw and how much to offset them.
            float x = Math.Abs(cameraPosition) * ScrollRate;//float x = cameraPosition * ScrollRate;
            int leftSegment = (int)Math.Floor(x / segmentWidth);
            int rightSegment = leftSegment + 1;
            x = (x / segmentWidth - leftSegment) * -segmentWidth;

            spriteBatch.Draw(Textures[leftSegment % Textures.Length], new Vector2(x, 0.0f), Color.White);
            spriteBatch.Draw(Textures[rightSegment % Textures.Length], new Vector2(x + segmentWidth, 0.0f), Color.White);
        }
    }
}
