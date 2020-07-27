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
    public class Bridge
    {
        private Body bridgeBody;
        private Vector2 origin;
        private static Texture2D bridgeTexture = ScreenManager.Content.Load<Texture2D>("Samples/tinman");
        private Body anchor;
        private RevoluteJoint anchorfix;
        private Body gear;
        private Body gearfix;
        private RevoluteJoint gearJoint;
        private static Texture2D gearTexture = ScreenManager.Content.Load<Texture2D>("Samples/gear");
        private Vector2 gearOrigin;

        public Bridge(Vector2 Position, float BridgeLength, Vector2 jointVector)
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

            gear = BodyFactory.CreateGear(PhysicsGameScreen.World, 3f, 12, 0f, 1f, 1f);
            gear.BodyType = BodyType.Dynamic;
            gear.Position = new Vector2(bridgeBody.Position.X, bridgeBody.Position.Y + 10f);
            gearOrigin = new Vector2(gearTexture.Width / 2, gearTexture.Height / 2);

            gearfix = BodyFactory.CreateCircle(PhysicsGameScreen.World, 1f, 1f);
            gearfix.BodyType = BodyType.Static;
            gearfix.Position = new Vector2(bridgeBody.Position.X - 10f, bridgeBody.Position.Y - 15f);
            gearfix.IgnoreCollisionWith(gear);

            gearJoint = new RevoluteJoint(gear, gearfix, new Vector2(0f, 0f), new Vector2(0f, 0f));
            gearJoint.MaxMotorTorque = 100f;
            gearJoint.MotorEnabled = true;
            gearJoint.MotorSpeed = 0f;
            PhysicsGameScreen.World.AddJoint(gearJoint);

        }

        public void Update()
        {
            if (gear.Revolutions >= 2 || gear.Revolutions <= -2)
                anchorfix.MotorSpeed = -50000f;

        }

        public void Draw(float scale)
        {
            ScreenManager.SpriteBatch.Draw(bridgeTexture, ConvertUnits.ToDisplayUnits(bridgeBody.Position), null,
                                           Color.White, bridgeBody.Rotation, origin, scale, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(gearTexture, ConvertUnits.ToDisplayUnits(gear.Position), null,
                                           Color.White, gear.Rotation, gearOrigin, 0.6f, SpriteEffects.None, 0f);
        }
    }
}
