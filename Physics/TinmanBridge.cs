using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.ScreenSystem;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using PanzerAdmiral.Helpers;
using Microsoft.Xna.Framework.Input;

namespace PanzerAdmiral.Physics
{
    public class TinmanBridge
    {
        private Body bridgeBody;
        private Vector2 origin;
        private static Texture2D bridgeTexture = ScreenManager.Content.Load<Texture2D>("Samples/tinman");
        private Body anchor;
        private RevoluteJoint anchorfix;
        private Chain chain;
        private Body weight;
        private static Texture2D weightTexture = ScreenManager.Content.Load<Texture2D>("Samples/gear");
        private Vector2 weightOrigin;
        private float bridgeScale;
        private float weightScale;
        private float density = 1.0f;

        public TinmanBridge(Vector2 Position, float BridgeLength, Vector2 jointVector)
        {
            bridgeBody = BodyFactory.CreateRectangle(PhysicsGameScreen.World, 1f, BridgeLength, 1f);
            bridgeBody.BodyType = BodyType.Dynamic;
            bridgeBody.Mass = 2000f;
            bridgeBody.Position = new Vector2(Position.X + 2f, Position.Y);
            origin = new Vector2(bridgeTexture.Width / 2, bridgeTexture.Height / 2);

            anchor = BodyFactory.CreateCircle(PhysicsGameScreen.World, 0.5f, 1f);
            anchor.BodyType = BodyType.Static;
            anchor.Position = Position;

            anchorfix = new RevoluteJoint(bridgeBody, anchor, jointVector, new Vector2(0f, 0f));
            anchorfix.MotorSpeed = 50000f;
            anchorfix.MaxMotorTorque = 300000f;
            anchorfix.MotorEnabled = true;
            anchorfix.LowerLimit = 1.6f;
            anchorfix.UpperLimit = 3.0f;
            anchorfix.LimitEnabled = true;
            PhysicsGameScreen.World.AddJoint(anchorfix);

            weight = BodyFactory.CreateRectangle(PhysicsGameScreen.World, ConvertUnits.ToSimUnits(weightTexture.Width) * weightScale,
                                              ConvertUnits.ToSimUnits(weightTexture.Height) * weightScale, density);
            weight.BodyType = BodyType.Dynamic;
            weight.Mass = 1f;
            origin = new Vector2(weightTexture.Width / 2, weightTexture.Height / 2);
            weight.Position = new Vector2(-100f, -60f);

            chain = new Chain(new Vector2(-100f, -80f), weight.LocalCenter, new Vector2(-100f, -80f), true, weight.LocalCenter, 10, weight, true);

        }

        public void Update()
        {
            

        }

        public void Draw(float scale)
        {
            ScreenManager.SpriteBatch.Draw(bridgeTexture, ConvertUnits.ToDisplayUnits(bridgeBody.Position), null,
                                           Color.White, bridgeBody.Rotation, origin, scale, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(weightTexture, ConvertUnits.ToDisplayUnits(weight.Position), null,
                                           Color.White, weight.Rotation, weightOrigin, 0.6f, SpriteEffects.None, 0f);
        }
    }
}
