

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using PanzerAdmiral.Helpers;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common.Decomposition;
using PanzerAdmiral.ScreenSystem;
using System.Linq;

namespace PanzerAdmiral.Physics
{
    public class PhysicTexture
    {
        #region Variables
        private Vector2 size;
        private Vector2 origin;

        private Texture2D collisionTexture;
        private Texture2D diffuseTexture;

        public Body body;

        private float density;
        private Vector2 position;

        public Vector2 Size
        {
            get { return size; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Create PhysicTexture
        /// </summary>
        /// <param name="setPos">set Position</param>
        /// <param name="setDensity">set Density</param>
        /// <param name="setDiffuseTexture">diffuse texture, used for drawing</param>
        /// <param name="setCollisionTexture">collision texture, used to creat the physics body</param>
        public PhysicTexture(Vector2 setPos, float setDensity, string setDiffuseTexture, string setCollisionTexture)
        {
            // assign values
            density = setDensity;
            position = setPos;

            // load collision and diffuse Texture
            collisionTexture = ScreenManager.Content.Load<Texture2D>(setCollisionTexture);
            diffuseTexture = ScreenManager.Content.Load<Texture2D>(setDiffuseTexture);

            // Create physics Body from Collision Texture
            CreateBody();
        } // PhysicTexture(setPos, setDensity, setDiffuseTexture, setCollisionTexture)
        #endregion

        #region CreateBody
        /// <summary> Creates the physics Body </summary>
        private void CreateBody()
        {
            uint[] data = new uint[collisionTexture.Width * collisionTexture.Height];
            collisionTexture.GetData(data);

            Vertices textureVertices = PolygonTools.CreatePolygon(data, collisionTexture.Width, true);

            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);

            origin = -centroid;

            textureVertices = SimplifyTools.ReduceByDistance(textureVertices, 4f);

            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1));
            List<Vertices> list = BayazitDecomposer.ConvexPartition(textureVertices);

            list.ForEach(v => v.Scale(ref vertScale));

            size = list.SelectMany(v => v).CalculateSize();

            body = BodyFactory.CreateCompoundPolygon(PhysicsGameScreen.World, list, density, BodyType.Static);//BodyType.Dynamic);
            body.Position = position;
            body.UserData = "physictexture";
            body.Friction = 10f;
        } // CreateBody(setPos, setDensity)

        #endregion

        #region Draw
        /// <summary> Draw diffuse texture </summary>
        public void Draw()
        {
            ScreenManager.SpriteBatch.Draw(diffuseTexture, ConvertUnits.ToDisplayUnits(body.Position), null, Color.White,
                                           body.Rotation, origin, 1f, SpriteEffects.None, 0f );
        } // Draw()
        #endregion

        #region DrawCollision
        /// <summary> Draw collision texture </summary>
        public void DrawCollision()
        {
            ScreenManager.SpriteBatch.Draw(collisionTexture, ConvertUnits.ToDisplayUnits(body.Position), null, Color.White,
                                           body.Rotation, origin, 1f, SpriteEffects.None, 0f);
        } // DrawCollision()
        #endregion
    } // class PhysicTexture
} // namespace PanzerAdmiral.Physics
