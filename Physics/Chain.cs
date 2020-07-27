using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using PanzerAdmiral.ScreenSystem;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using PanzerAdmiral.Demos;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.Helpers;

namespace PanzerAdmiral.Physics
{
    public class Chain
    {
        private Texture2D chainTexture;
        private float scale = 0.1f;
        private Vector2 origin;
        public Body chainLink;
        public List<Body> chainLinks = new List<Body>();

        public Chain(Vector2 pathBeginning, Vector2 pathEnd, Vector2 revolutePosition1, bool useSecondRevolute, Vector2 revolutePosition2, int numberOfChainlinks, Body revoluteBody, bool breakable)
        {
            chainTexture = ScreenManager.Content.Load<Texture2D>("Enemies/kette");
            origin = new Vector2(chainTexture.Width / 2, chainTexture.Height / 2);

            //Chain start / end
            Path path = new Path();
            path.Add(pathBeginning);
            path.Add(pathEnd);

            //A single chainlink
            PolygonShape shape = new PolygonShape(PolygonTools.CreateRectangle(0.125f, 0.6f), 20);

            //Use PathFactory to create all the chainlinks based on the chainlink created before.
            chainLinks = PathManager.EvenlyDistributeShapesAlongPath(PhysicsGameScreen.World, path, shape, BodyType.Dynamic, numberOfChainlinks);

            foreach (Body chainLink in chainLinks)
            {
                foreach (Fixture f in chainLink.FixtureList)
                {
                    f.Friction = 0.1f;
                }
            }

            //Fix the first chainlink to a Body
            FixedRevoluteJoint fixedJoint = new FixedRevoluteJoint(chainLinks[0], Vector2.Zero, revolutePosition1);
            PhysicsGameScreen.World.AddJoint(fixedJoint);


            RevoluteJoint fixedJoint2 = new RevoluteJoint(chainLinks[numberOfChainlinks -= 2], revoluteBody,   new Vector2(chainLinks[numberOfChainlinks -= 2].LocalCenter.X -0.125f,
                                                          chainLinks[numberOfChainlinks -= 2].LocalCenter.Y + 0.6f), revolutePosition2);
            PhysicsGameScreen.World.AddJoint(fixedJoint2);

            //Attach all the chainlinks together with a revolute joint
            List<RevoluteJoint> joints = PathManager.AttachBodiesWithRevoluteJoint(PhysicsGameScreen.World, chainLinks,
                                                                                   new Vector2(0, -0.6f),
                                                                                   new Vector2(0, 0.6f),
                                                                                   false, false);

      

            if (breakable == true)
            {
                //The chain is breakable
                for (int i = 0; i < joints.Count; i++)
                {
                    RevoluteJoint r = joints[i];

                    r.Breakpoint = 10000f;
                    //if (r.Broke lower bridge
                }
            }
        }

        public void Update()
        {
            foreach (Body chainLink in chainLinks)
            {
                chainLink.SetTransform(chainLink.Position, 0);
            }
        }

        public void Draw()
        {
            foreach (Body chainLink in chainLinks)
            {
                ScreenManager.SpriteBatch.Draw(chainTexture, ConvertUnits.ToDisplayUnits(chainLink.Position), null,
                                             Color.White, chainLink.Rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}

