using FarseerPhysics.Dynamics;
using PanzerAdmiral.Graphics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics.Joints;
using PanzerAdmiral.Helpers;
using System;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using System.Collections.Generic;
using FarseerPhysics.Factories;
using PanzerAdmiral.AI;
using PanzerAdmiral.Demos;
using PanzerAdmiral.Sounds;
using FarseerPhysics.Dynamics.Contacts;
using PanzerAdmiral.Particles;

namespace PanzerAdmiral.Physics
{
    /// <summary>
    /// Vehicle class, has 2 wheels and 1 chassis body
    /// UserData="player"
    /// </summary>
    public class Vehicle : Actor
    {
        #region Variables
        private Body _vehicleBody;
        public Body Body
        {
            get { return _vehicleBody; }
        } // Body

        // vehicle Texture and Damage states
        private Texture2D _vehicleTexture;
        private Texture2D _destruct1;
        private Texture2D _destruct2;
        private Texture2D _destruct3;

        private Vector2 _origin;
        private float _scale;   // car scale
        private float _wscale;  // wheel scale

        private Body _wheelBack;
        public Body WheelBackBody
        {
            get { return _wheelBack; }
        }

        private Body _wheelFront;
        public Body WheelFrontBody
        {
            get { return _wheelFront; }
        }

        public LineJoint _springBack;
        private LineJoint _springFront;
        private Sprite _wheel;
        public Fixture wheelBackfix;
        public Fixture wheelFrontfix;

        public bool IsOnground = false;


        // Car physics values
        private float _zeta;


        private float _hzBack;
        private float _hzFront;
        public float _maxSpeed;

        private float rotationSpeed = 0.05f;

        // Cannon and its joint
        public Cannon cannon;
        private RevoluteJoint _anchor;



        #endregion

        #region Constructor
        /// <summary> Create Vehicle </summary>
        public Vehicle()
            : base("Samples/car", new Color(64, 255, 64))
        {
            _hzFront = 3.0f;
            _hzBack = 3.0f;
            _zeta = 0.7f; //0.85f;
            _maxSpeed = 30.0f;

            // default car scale
            // Apply Scaling in ACtor class! This is very important for radius checks!
            Scale = _scale = 0.3f; 
            SetScale();

            SetupPhysics();

            // Create canon
            cannon = new Cannon(false);
            //cannon.Scale = 0.3f;

            // Ignore collision and create Joints for cannon
            _vehicleBody.IgnoreCollisionWith(cannon.Body);
            _wheelBack.IgnoreCollisionWith(cannon.Body);
            _wheelFront.IgnoreCollisionWith(cannon.Body);

            _anchor = new RevoluteJoint(cannon.Body, _vehicleBody, _wheelFront.GetLocalVector(cannon.Body.Position), new Vector2(2f, 0f));
            _anchor.MotorSpeed = 1.0f;
            _anchor.MaxMotorTorque = 1000f;
            _anchor.MotorEnabled = true;
            _anchor.UpperLimit = 0.5f;
            _anchor.LowerLimit = 0f;
            _anchor.LimitEnabled = false;
            PhysicsGameScreen.World.AddJoint(_anchor);
            _vehicleBody.UserData = "player";

        } // Vehicle()
        #endregion

        #region SetupPhysics
        /// <summary> SetupPhysics </summary>
        private void SetupPhysics()
        {
            // define car countour and damage state textures
            _vehicleTexture = ScreenManager.Content.Load<Texture2D>("Samples/car");
            _destruct1 = ScreenManager.Content.Load<Texture2D>("Samples/car_destruct1");
            _destruct2 = ScreenManager.Content.Load<Texture2D>("Samples/car_destruct2");
            _destruct3 = ScreenManager.Content.Load<Texture2D>("Samples/car_destruct3");


            uint[] data = new uint[_vehicleTexture.Width * _vehicleTexture.Height];
            _vehicleTexture.GetData(data);

            Vertices textureVertices = PolygonTools.CreatePolygon(data, _vehicleTexture.Width, false);
            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);
            _origin = -centroid;

            textureVertices = SimplifyTools.ReduceByDistance(textureVertices, 4);
            List<Vertices> list = BayazitDecomposer.ConvexPartition(textureVertices);
            //  _scale = 0.2f;
            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1)) * _scale;

            foreach (Vertices vertixes in list)
            {
                vertixes.Scale(ref vertScale);
            }

            _vehicleBody = BodyFactory.CreateCompoundPolygon(PhysicsGameScreen.World, list, 1f, BodyType.Dynamic);
            _vehicleBody.BodyType = BodyType.Dynamic;
            _vehicleBody.Position = new Vector2(0.0f, -2.0f);
            _vehicleBody.Mass = 5f;

            _wheelBack = new Body(PhysicsGameScreen.World);
            _wheelBack.BodyType = BodyType.Dynamic;
            _wheelBack.Position = new Vector2(-2.0f, -0.8f);//new Vector2(-1.709f, -0.78f);
            wheelBackfix = _wheelBack.CreateFixture(new CircleShape(1.1f, 1f));
            wheelBackfix.Friction = 120.0f;
            //_wheelBack.Mass = 2f;
            wheelBackfix.Body.Mass = 4f;
            //wheelBackfix.Restitution = 0f;
            _wheelBack.UserData = "wheel";
            _wheelBack.OnCollision += new OnCollisionEventHandler(Body_OnCollision);

            _wheelFront = new Body(PhysicsGameScreen.World);
            _wheelFront.BodyType = BodyType.Dynamic;
            _wheelFront.Position = new Vector2(2.0f, -0.8f);
            wheelFrontfix = _wheelFront.CreateFixture(new CircleShape(1.1f, 1f));
            wheelFrontfix.Friction = 120.0f;
            //_wheelFront.Mass = 2f;
            wheelFrontfix.Body.Mass = 4f;
            //wheelFrontfix.Restitution = 0f;
            wheelFrontfix.Body.UserData = "wheel";
            wheelFrontfix.Body.OnCollision += new OnCollisionEventHandler(Body_OnCollision);
            

            Vector2 axis = new Vector2(0.0f, 1.0f);//0.8f);
            _springBack = new LineJoint(_vehicleBody, _wheelBack, _wheelBack.Position, axis);
            _springBack.MotorSpeed = 0.0f;
            _springBack.MaxMotorTorque = 90;// 80.0f;//100;//20.0f;
            _springBack.MotorEnabled = true;
            _springBack.Frequency = _hzBack;
            _springBack.DampingRatio = _zeta;
            PhysicsGameScreen.World.AddJoint(_springBack);

            _springFront = new LineJoint(_vehicleBody, _wheelFront, _wheelFront.Position, axis);
            _springFront.MotorSpeed = 0.0f;
            _springFront.MaxMotorTorque = 90;// 80.0f;//100;//10.0f;
            _springFront.MotorEnabled = true;
            _springFront.Frequency = _hzFront;
            _springFront.DampingRatio = _zeta;
            PhysicsGameScreen.World.AddJoint(_springFront);


            _wscale = 0.05f;
            //   _vehicleBody = new Sprite(ScreenManager.Content.Load<Texture2D>("Samples/car"),
            //    AssetCreator.CalculateOrigin(_vehicleBody) / _scale);
            _wheel = new Sprite(ScreenManager.Content.Load<Texture2D>("Samples/tankwheel2"));
        } // SetupPhysics()
        #endregion

        #region OnCollision
        /// <summary>
        /// Wheel and PhysicTexture OnCollision, checks if we are on ground for jumping
        /// </summary>
        /// <param name="fixtureA">wheel</param>
        /// <param name="fixtureB">physictexture</param>
        /// <param name="contact"></param>
        /// <returns>true</returns>
        public bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // if we have some valid userdata
            if (fixtureB.Body.UserData != null)
            {
                // if this fixture is the wheel and physictexture
                if (fixtureB.Body.UserData.ToString() == "physictexture" && fixtureA.Body.UserData.ToString() == "wheel")
                {
                    IsOnground = true;
                }
            }
            return true;
        }
        #endregion

        #region Update()
        /// <summary> Update </summary>
        public override void Update()
        {
            HandleInput();

            // Sync AI, Use display units
            //base.Update();    // no need, just update Position and Rotation
            Position = ConvertUnits.ToDisplayUnits(_vehicleBody.Position);
            Rotation = Body.Rotation;

            if (_vehicleBody.UserData.ToString() == "getroffen")
            {
                HealthBar.SetDamage(GameSettings.TowerDamage);
                _vehicleBody.UserData = "player";
            }
            cannon.Update();

        } // Update()
        #endregion

        #region HandleInput
        /// <summary> 
        /// HandleInput 
        /// TODO: Needs Brake / Hover
        /// </summary>
        public void HandleInput()
        {
            // Play sound if new Key is Press and Stop the Sound when a new Key is released!
            if (ScreenManager.Input.IsNewKeyPress(Keys.D) || ScreenManager.Input.IsNewKeyPress(Keys.A))
            {
                // Play motor sound and also make more Smoke! Change the TimeTillTankPuff to get an immediate timerchange!
                Sound.Play(Sound.Sounds.Motor);
                GameDemo1.particleManager.TimeBetweenTankSmokePuffs = 0.3f;
                GameDemo1.particleManager.timeTillTankPuff = 0f;
            }
            else if (ScreenManager.Input.IsNewKeyRelease(Keys.D) || ScreenManager.Input.IsNewKeyRelease(Keys.A))
            {
                // Stop motor sound and reset Particle Timer to normal puff timing
                Sound.StopSound(Sound.Sounds.Motor);
                GameDemo1.particleManager.TimeBetweenTankSmokePuffs = ParticleSystemManager.DefaultTimeBetweentankSmokePuffs;
            }

            // Handle acceleration
            if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.D))
            {
                _springBack.MotorEnabled = true;
                _springFront.MotorEnabled = true;

                _springBack.MotorSpeed = _maxSpeed;
                _springFront.MotorSpeed = _maxSpeed;
            }
            else if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.A))
            {
                _springBack.MotorEnabled = true;
                _springFront.MotorEnabled = true;

                _springBack.MotorSpeed = -_maxSpeed;
                _springFront.MotorSpeed = -_maxSpeed;
            }
            else if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.S))
            {
                _springBack.MotorEnabled = true;
                _springFront.MotorEnabled = true;

                if (_springBack.MotorSpeed > 0)
                    _vehicleBody.LinearVelocity = _vehicleBody.GetLinearVelocityFromWorldPoint(_vehicleBody.WorldCenter) - new Vector2(12, 0);
                if (_springBack.MotorSpeed < 0)
                    _vehicleBody.LinearVelocity = _vehicleBody.GetLinearVelocityFromWorldPoint(_vehicleBody.WorldCenter) + new Vector2(12, 0);

                _springBack.MotorSpeed = 0f;
                _springFront.MotorSpeed = 0f;
            }
            else if (ScreenManager.Input.KeyboardState.IsKeyUp(Keys.D) && ScreenManager.Input.KeyboardState.IsKeyUp(Keys.A))
            {
                _springBack.MotorEnabled = false;
                _springFront.MotorEnabled = false;
            }

            // Handle Rotation from Input
            if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Q))
                _vehicleBody.Rotation -= rotationSpeed;
            if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.E))
                _vehicleBody.Rotation += rotationSpeed;

            // jump with space
            if (ScreenManager.Input.IsNewKeyPress(Keys.Space) && IsOnground == true)
            {
                IsOnground = false;
                _vehicleBody.LinearVelocity = _vehicleBody.GetLinearVelocityFromWorldPoint(_vehicleBody.WorldCenter) + new Vector2(0, -20);
            }

            // Reset Position with Enter
            if (ScreenManager.Input.IsNewKeyPress(Keys.Enter))
                SetPosition(new Vector2(0, -6));

//             else if (ScreenManager.Input.IsNewKeyPress(Keys.R))
//             {
//                 _hzFront = Math.Max(0.0f, _hzFront - 1.0f);
//                 _springBack.Frequency = _hzFront;
//                 _springFront.Frequency = _hzFront;
//             }
//             else if (ScreenManager.Input.IsNewKeyPress(Keys.T))
//             {
//                 _hzFront += 1.0f;
//                 _springBack.Frequency = _hzFront;
//                 _springFront.Frequency = _hzFront;
//             }

            // Veränderbare Werte Fahrzeug-Tweaking
            if (ScreenManager.Input.IsNewKeyPress(Keys.D9))
            {
                _springBack.MaxMotorTorque -= 10f;
            }
            else if (ScreenManager.Input.IsNewKeyPress(Keys.D0))
            {
                _springBack.MaxMotorTorque += 10f;
            }
            else if (ScreenManager.Input.IsNewKeyPress(Keys.D8))
            {
                wheelBackfix.Friction += 10.0f; ;
            }
            else if (ScreenManager.Input.IsNewKeyPress(Keys.D7))
            {
                wheelBackfix.Friction -= 10.0f;
            }
            else if (ScreenManager.Input.IsNewKeyPress(Keys.D6))
            {
                _maxSpeed += 10f;
            }
            else if (ScreenManager.Input.IsNewKeyPress(Keys.D5))
            {
                _maxSpeed -= 10f;
            }
            else if (ScreenManager.Input.IsNewKeyPress(Keys.D4))
            {
                _vehicleBody.Mass += 10f;
            }
            else if (ScreenManager.Input.IsNewKeyPress(Keys.D3))
            {
                _vehicleBody.Mass -= 10f;
            }
        } // UpdateInput(input, gameTime)
        #endregion

        #region SetPosition
        /// <summary>
        /// Reset dynamics and also transform wheels near car (prevent wheels from colliding with env)
        /// </summary>
        /// <param name="setPosition">set Position</param>
        public void SetPosition(Vector2 setPosition)
        {
            Body.ResetDynamics();
            Body.SetTransform(setPosition, 0f);
            _wheelBack.SetTransform(setPosition, 0f);
            _wheelFront.SetTransform(setPosition, 0f);
            cannon.Body.SetTransform(setPosition, 0f);

        } // SetPosition(setPosition)
        #endregion

        #region Draw
        /// <summary> draw 2 wheels and car chassis </summary>
        public override void Draw()
        {
            if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Y))
            {
                base.Draw();
                DrawCircle(Radius);
            }
            // draw cannnon first
            cannon.Draw();

            // get damage texture states from hitpoint percentage
            Texture2D tex = _vehicleTexture;
            if (HealthBar.currentHealthInPercent >= 0.75)
                tex = _vehicleTexture;
            if (HealthBar.currentHealthInPercent >= 0.5 && HealthBar.currentHealthInPercent < 0.75)
                tex = _destruct1;
            if (HealthBar.currentHealthInPercent >= 0.25 && HealthBar.currentHealthInPercent < 0.5)
                tex = _destruct2;
            if (HealthBar.currentHealthInPercent < 0.25f)
                tex = _destruct3;

            // Draw car and wheels
            ScreenManager.SpriteBatch.Draw(tex, ConvertUnits.ToDisplayUnits(_vehicleBody.Position), null,
                                           Color.White, _vehicleBody.Rotation, _origin, _scale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(_wheel.Texture, ConvertUnits.ToDisplayUnits(_wheelBack.Position), null,
                                          Color.White, _wheelBack.Rotation, _wheel.Origin, _wscale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(_wheel.Texture, ConvertUnits.ToDisplayUnits(_wheelFront.Position), null,
                                           Color.White, _wheelFront.Rotation, _wheel.Origin, _wscale, SpriteEffects.None, 0f);
        } // Draw()
        #endregion

        #region DrawCircle
        /// <summary> Draws the SghtRadius for Debugging </summary>
        private void DrawCircle(float radius)
        {
            // use transparent white color with 80 alpha
            Color color = new Color(255, 255, 255, 80);
            Texture2D texture = Style.NodeTexture;
            int textureRadius = texture.Width / 2;

            float scale = radius / textureRadius;
            Vector2 origin = new Vector2(textureRadius, textureRadius);
            ScreenManager.SpriteBatch.Draw(texture, this.Position, null, color, 0f, origin, scale, SpriteEffects.None, 1f);
        } // DrawCircle(radius)
        #endregion
    } // class Vehicle
} // namespace PanzerAdmiral.Physics
