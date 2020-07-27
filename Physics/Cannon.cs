using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.ScreenSystem;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using PanzerAdmiral.Helpers;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Factories;
using PanzerAdmiral.Demos;

namespace PanzerAdmiral.Physics
{
    public class Cannon
    {
        #region Variables
        private Body _cannonBody;
        public Body Body
        {
            get { return _cannonBody; }
        }
        public Vector2 _origin;
        private static Texture2D _cannonTexture = ScreenManager.Content.Load<Texture2D>("Samples/cannon");
        public Vector2 _cursorDistance;
        public float Scale = 0.1f;

        public Vector2 rotationPoint = Vector2.Zero;

        public Vector2 cannonScreen;

        float density = 1.0f;


        Vector2 ZeroDegreeVectorByDanielLeusch = new Vector2(0, -1);
        public float Angle = 0.0f;

        public bool isEnemyCannon;
        #endregion

        #region Constructor
        /// <summary>
        /// Create Cannon
        /// </summary>
        /// <param name="isEnemyCannon"></param>
        /// <returns></returns>
        public Cannon(bool isEnemyCannon)
        {
            this.isEnemyCannon = isEnemyCannon;
            _cannonBody = BodyFactory.CreateRectangle(PhysicsGameScreen.World, ConvertUnits.ToSimUnits(_cannonTexture.Width) * Scale,
                                               ConvertUnits.ToSimUnits(_cannonTexture.Height) * Scale, density);
            
            _cannonBody.BodyType = BodyType.Dynamic;
            _cannonBody.Mass = 0f;
            _origin = new Vector2(_cannonTexture.Width / 2, _cannonTexture.Height / 2);
        }
        #endregion

        #region Update
        public void Update()
        {
            if (!isEnemyCannon)
            {
                rotationPoint = _cannonBody.WorldCenter;

                // mouse position in World Space! (Unproject)
                Vector2 cursorPosScreen = PhysicsGameScreen.Camera.ConvertScreenToWorld(ScreenManager.Input.Cursor);
                cannonScreen = rotationPoint;

                _cursorDistance = cursorPosScreen - cannonScreen;
                _cursorDistance.Normalize();

                Angle = (float)Math.Acos(Vector2.Dot(_cursorDistance, ZeroDegreeVectorByDanielLeusch) /
                                         _cursorDistance.Length() * ZeroDegreeVectorByDanielLeusch.Length());//);

                Angle = MathHelper.ToDegrees(Angle);

                if (_cursorDistance.X > 0)
                    _cannonBody.Rotation = MathHelper.ToRadians(Angle);
                else
                    _cannonBody.Rotation = MathHelper.ToRadians(360 - Angle);
            } // if (player cannon)
            else
            {
                rotationPoint = _cannonBody.WorldCenter;
                Vector2 ZeroDegreeVector = new Vector2(0, -1);
                // mouse position in World Space! (Unproject)
                Vector2 vehiclePosition = GameDemo1.vehicle.Body.Position;
                cannonScreen = rotationPoint;

                _cursorDistance = vehiclePosition - cannonScreen;
                _cursorDistance.Normalize();

                Angle = (float)Math.Acos(//MathHelper.ToRadians(
                                  Vector2.Dot(_cursorDistance, ZeroDegreeVector) /
                                  _cursorDistance.Length() * ZeroDegreeVector.Length());//);



                Angle = MathHelper.ToDegrees(Angle);

                if (_cursorDistance.X > 0)
                    _cannonBody.Rotation = MathHelper.ToRadians(Angle);
                else
                    _cannonBody.Rotation = MathHelper.ToRadians(360 - Angle);
            } // if (isEnemyCannon)

        } // Update()
        #endregion

        #region Draw
        /// <summary>
        /// Draw Cannon
        /// </summary>
        public void Draw()
        {
            ScreenManager.SpriteBatch.Draw(_cannonTexture, ConvertUnits.ToDisplayUnits(_cannonBody.Position),
                                        null, Color.White, _cannonBody.Rotation, _origin, Scale, SpriteEffects.None,
                                        0f);
        } // Draw()
        #endregion
    } //  class Cannon
} // namespace PanzerAdmiral.Physics
