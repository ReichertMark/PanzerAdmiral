using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Helpers;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Input;

namespace PanzerAdmiral.Physics
{
    public class Trap
    {
        public Body weightBody;
        public Vector2 origin;
        public Chain chain;
        private Texture2D weightTexture = ScreenManager.Content.Load<Texture2D>("Enemies/tentonweight");
        private float scale = 0.6f;
        private float density = 1.0f;

        public Trap()
        {
            weightBody = BodyFactory.CreateRectangle(PhysicsGameScreen.World, ConvertUnits.ToSimUnits(weightTexture.Width) * scale,
                                              ConvertUnits.ToSimUnits(weightTexture.Height) * scale, density);
            weightBody.BodyType = BodyType.Dynamic;
            weightBody.Mass = 100f;
            origin = new Vector2(weightTexture.Width / 2, weightTexture.Height / 2);
            weightBody.Position = new Vector2(204f, 9f);

            chain = new Chain(new Vector2(190f, 7f), weightBody.LocalCenter, new Vector2(190f, 7f), true, new Vector2(weightBody.LocalCenter.X, weightBody.LocalCenter.Y - 1.8f), 12, weightBody, false);
        }

        #region Update

        public void Update()
        {
            if (ScreenManager.Input.IsNewKeyPress(Keys.L))
                weightBody.ApplyLinearImpulse(new Vector2(-100f, 50f));
           // chain.Update();
                
        }

        #endregion
        #region Draw
        /// <summary>
        /// Draw Cannon
        /// </summary>
        public void Draw()
        {
            chain.Draw();
            ScreenManager.SpriteBatch.Draw(weightTexture, ConvertUnits.ToDisplayUnits(weightBody.Position),
                                        null, Color.White, weightBody.Rotation, origin, scale, SpriteEffects.None,
                                        0f);
        } // Draw()
        #endregion
    }
}
