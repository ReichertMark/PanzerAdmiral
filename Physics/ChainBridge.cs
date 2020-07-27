using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using PanzerAdmiral.ScreenSystem;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.Helpers;
using PanzerAdmiral.Demos;

namespace PanzerAdmiral.Physics
{
    public class ChainBridge
    {
        private static Texture2D ChainBridgeTexture = ScreenManager.Content.Load<Texture2D>("Samples/piece");
        public static List<Body> segment = new List<Body>();
        Vector2 origin = new Vector2(ChainBridgeTexture.Width / 2, ChainBridgeTexture.Height / 2);

        public ChainBridge(Vector2 setPosition)
        {
            Body ground = new Body(PhysicsGameScreen.World);
            PolygonShape shape = new PolygonShape(1);
            shape.SetAsBox(0.9f, 0.125f);

            Body prevBody = ground;
            for (int i = 0; i < 11; ++i)
            {
                Body body = new Body(PhysicsGameScreen.World);
                body.BodyType = BodyType.Dynamic;
                body.Position = new Vector2(setPosition.X + 1.8f * i, setPosition.Y);
                body.IgnoreCollisionWith(GameDemo1.physicTextures[8].body);
                Fixture fix = body.CreateFixture(shape);
                fix.Friction = 0.6f;

                Vector2 anchor = new Vector2(-1, 0);
                JointFactory.CreateRevoluteJoint(PhysicsGameScreen.World, prevBody, body, anchor);

                body.Mass = 1f;
                prevBody = body;
                segment.Add(prevBody);
            }

            Vector2 anchor2 = new Vector2(1.0f, 0);
            JointFactory.CreateRevoluteJoint(PhysicsGameScreen.World, ground, prevBody, anchor2);
        }

        public void Draw(float scale)
        {
            foreach (Body body in segment)
            {
                ScreenManager.SpriteBatch.Draw(ChainBridgeTexture, ConvertUnits.ToDisplayUnits(body.Position), null,
                                               Color.White, body.Rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
