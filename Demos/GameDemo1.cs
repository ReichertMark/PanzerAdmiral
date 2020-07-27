using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PanzerAdmiral.Graphics;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Helpers;
using FarseerPhysics;
using PanzerAdmiral.Physics;
using PanzerAdmiral.AI;
using PanzerAdmiral.Sounds;
using PanzerAdmiral.Particles;

namespace PanzerAdmiral.Demos
{
    internal class GameDemo1 : PhysicsGameScreen, IDemoScreen
    {
        #region Variables
        public static Vehicle vehicle;
        public static List<PhysicTexture> physicTextures = new List<PhysicTexture>();
        public static List<PhysicBreakable> physicBreakables = new List<PhysicBreakable>();

        // Parallax Background stuff
        private ParallaxBackground _background, _clouds;
        Vector2 camPos;
        Vector2 camPosLastFrame;

        public static GameTime GameTime = new GameTime();

        // Level class, loads, unloads, Updates and draws all actors (PickUp, vehicle, Enemies)
        Level level;

        HealthBar healthBar;

        // Vehicle Shots
        public static List<Shot> Shots = new List<Shot>();

        FadeUp fadeUp;

        // Bridges
        Bridge bridge1;
        ChainBridge chainBridge1;

        // Traps, Riddles and Stuff
        Trap trap;

        public static ParticleSystemManager particleManager;

        #endregion

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Panzer Admiral!";
        } // GetTitle()

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Hit 1 Button to damage the towers");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Exit to menu: Escape");
            return sb.ToString();
        } // GetDetails()

        #endregion

        #region LoadContent
        /// <summary> Loads all Content </summary>
        public override void LoadContent()
        {
            base.LoadContent();
            World.Gravity = new Vector2(0f, 10f);

            HasCursor = true;           // Enable MouseCursor for aiming
            EnableCameraControl = true; // Enable the Camera
            HasVirtualStick = false;


            fadeUp = new FadeUp();

            // Stop Music and Play the Game Music instead!
            Sound.StopMusic();
            Sound.Play(Sound.Sounds.GameMusic);

            //AnimatedObject animatedObject = new AnimatedObject(new Vector2(461, 2), 1.4f, new Animation("Enemies/Schnabelman", 5, 12, 0, 58, 3000, true));
            //AnimatedObject animatedObject2 = new AnimatedObject(new Vector2(240, 12), 0.75f, new Animation("Backgrounds/cogwheel", 4, 15, 0, 60, 3000, true));
            //AnimatedObject animatedObject3 = new AnimatedObject(new Vector2(368, 9), 7f, new Animation("Enemies/Tinman", 6, 13, 0, 71, 2000, true));
            StaticObject staticObject = new StaticObject("Enemies/Elephant", new Vector2(240, 0f), 2.8f);
            StaticObject staticObject2 = new StaticObject("Enemies/stomper_housing", new Vector2(862f, 9f), 1f);
            StaticObject staticObject3 = new StaticObject("Enemies/tentontower", new Vector2(190f, 17f), 0.2f);
            //staticObject.Rotation = MathHelper.ToRadians(90);

            healthBar = new HealthBar(GameSettings.PlayerHitpoints);
            vehicle = new Vehicle();
            vehicle.SetPosition(new Vector2(22.0f, 0.0f));


            // Create Level
            int numSegments = 1;//10;
            Vector2 levelStartPosition = new Vector2(30, 30);

            for (int i = 0; i < numSegments; i++)
            {
                PhysicTexture texture = new PhysicTexture(levelStartPosition, 0.1f, "Levels/island1_n", "Levels/island1_c");
                levelStartPosition.X += texture.Size.X; //-1; // falls lücken entstehen
                PhysicTexture texture2 = new PhysicTexture(new Vector2(levelStartPosition.X + 19.89f, 47.1f), 0.1f, "Levels/island2_n", "Levels/island2_c");
                levelStartPosition.X += texture2.Size.X; //-1; // falls lücken entstehen
                PhysicTexture texture3 = new PhysicTexture(new Vector2(levelStartPosition.X + 16f, 43.6f), 0.1f, "Levels/island3_n", "Levels/island3_c");
                levelStartPosition.X += texture3.Size.X; // falls lücken entstehen
                PhysicTexture texture4 = new PhysicTexture(new Vector2(levelStartPosition.X + 13.5f, 42f), 0.1f, "Levels/island4_n", "Levels/island4_c");
                levelStartPosition.X += texture4.Size.X; // falls lücken entstehen
                PhysicTexture texture5 = new PhysicTexture(new Vector2(levelStartPosition.X + 5.8f, 43.1f), 0.1f, "Levels/island5_n", "Levels/island5_c");
                levelStartPosition.X += texture5.Size.X; //-1; // falls lücken entstehen
                PhysicTexture texture6 = new PhysicTexture(new Vector2(levelStartPosition.X + 16.3f, 49.5f), 0.1f, "Levels/island6_n", "Levels/island6_c");
                levelStartPosition.X += texture6.Size.X; //-1; // falls lücken entstehen
                PhysicTexture texture7 = new PhysicTexture(new Vector2(levelStartPosition.X + 16.3f, 49.5f), 0.1f, "Levels/island7_n", "Levels/island7_c");
                levelStartPosition.X += texture7.Size.X; //-1; // falls lücken entstehen
                PhysicTexture texture8 = new PhysicTexture(new Vector2(levelStartPosition.X + 16.3f, 50.6f), 0.1f, "Levels/island8_n", "Levels/island8_c");
                levelStartPosition.X += texture8.Size.X; //-1; // falls lücken entstehen
                PhysicTexture texture9 = new PhysicTexture(new Vector2(levelStartPosition.X + 34.3f, 27.62f), 0.1f, "Levels/island9_n", "Levels/island9_c");
                levelStartPosition.X += texture9.Size.X; //-1; // falls lücken entstehen


                physicTextures.Add(texture);
                physicTextures.Add(texture2);
                physicTextures.Add(texture3);
                physicTextures.Add(texture4);
                physicTextures.Add(texture5);
                physicTextures.Add(texture6);
                physicTextures.Add(texture7);
                physicTextures.Add(texture8);
                physicTextures.Add(texture9);
                //levelStartPosition.Y += 10f;
            }


            // Create Towers                          Position2D           HP   diffuseTexture  collisionTexture _c
            physicBreakables.Add(new PhysicBreakable(new Vector2(136, 17), GameSettings.HeavyTowerHitPoints, "Enemies/towerbig", "Enemies/towerbig_c", true));
            physicBreakables.Add(new PhysicBreakable(new Vector2(331, 1.5f), GameSettings.HeavyTowerHitPoints, "Enemies/towerbig", "Enemies/towerbig_c", true));
            //physicBreakables.Add(new PhysicBreakable(new Vector2(264, 19), GameSettings.HeavyTowerHitPoints, "Enemies/tower", "Enemies/tower_c", true));
            //physicBreakables.Add(new PhysicBreakable(new Vector2(344, 9), GameSettings.HeavyTowerHitPoints, "Enemies/tower", "Enemies/tower_c", true));
            //physicBreakables.Add(new PhysicBreakable(new Vector2(588, 32), GameSettings.HeavyTowerHitPoints, "Enemies/tower", "Enemies/tower_c", true));
           
            // Create Level (handles Actors updating and drawing + AI)
            level = new Level();



            // Create Triggers
            BreakableTrigger breakableTrigger = new BreakableTrigger(new Vector2(378, 12));
            EnemyTrigger trigger = new EnemyTrigger(new Vector2(150, 23));
            //EnemyTrigger trigger2 = new EnemyTrigger(new Vector2(242, 17));
            //EnemyTrigger trigger3 = new EnemyTrigger(new Vector2(306, 3));
            //  BreakableTrigger breakableTrigger2 = new BreakableTrigger(new Vector2(377, 8));


            // Create Eyes
            //Animation eyeAnimation = new Animation("Enemies/Eye", 4, 15, 0, 59, 5000, true);
            Eye eye = new Eye(new Vector2(40, -8), 1.0f, new Animation("Enemies/Eye", 4, 15, 0, 59, 5000, true));
            Eye eye2 = new Eye(new Vector2(60, -4), 1.0f, new Animation("Enemies/Eye", 4, 15, 0, 59, 5000, true));
            Eye eye3 = new Eye(new Vector2(80, -1), 1.0f, new Animation("Enemies/Eye", 4, 15, 0, 59, 5000, true));
            Eye eye4 = new Eye(new Vector2(100, 3), 1.0f, new Animation("Enemies/Eye", 4, 15, 0, 59, 5000, true));

            // Create Bridges
            bridge1 = new Bridge(new Vector2(549.2f, -2f), 20f, new Vector2(0f, -10f));
            chainBridge1 = new ChainBridge(new Vector2(590.8f, -2.3f));
            trap = new Trap();

            // Create Background
            CreateParallaxBG();

            // Create Particles
            particleManager = new ParticleSystemManager();

            Camera.MinRotation = -0.05f;
            Camera.MaxRotation = 0.05f;

            Camera.TrackingBody = vehicle.Body;
            Camera.EnableTracking = true;
            // LoadContent()
        }
        #endregion

        #region CreateParallaxBG
        /// <summary> Creates Parallax Background </summary>
        private void CreateParallaxBG()
        {
            // Background layers
             List<Texture2D> lst = new List<Texture2D>();
             lst.Add(ScreenManager.Content.Load<Texture2D>("Backgrounds/dusky sky endless loop"));

            _background = new ParallaxBackground(lst, ParallaxDirection.Horizontal)
            {
                Height = ScreenManager.GraphicsDevice.Viewport.Height,
                Width = ScreenManager.GraphicsDevice.Viewport.Width,
                //Position = Vector2.Zero, //this._backgroundPosition
            };

            // Cloud Layer
            lst = new List<Texture2D>();
            lst.Add(ScreenManager.Content.Load<Texture2D>("Backgrounds/clouds_small"));
            _clouds = new ParallaxBackground(lst, ParallaxDirection.Horizontal)
            {
                Height = ScreenManager.GraphicsDevice.Viewport.Height,
                Width = ScreenManager.GraphicsDevice.Viewport.Width
                //Position = Vector2.Zero
            };

        } // CreateParallaxBG()
        #endregion

        #region UnloadContent
        /// <summary> Unloads all Content </summary>
        public override void UnloadContent()
        {
            physicTextures.Clear();
            physicBreakables.Clear();
            level.Unload();
            Shots.Clear();
            StaticObject.Objects.Clear();
            AnimatedObject.Objects.Clear();
            Trigger.Triggers.Clear();
            Eye.Eyes.Clear();

            base.UnloadContent();
        } // UnloadContent
        #endregion

        #region Update
        /// <summary>
        /// Updates all objects in the scene
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        /// <returns></returns>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Update GameTime
            GameTime = gameTime;


            // Move parallax layers and don't do shit if there's no cam movement! 
            camPosLastFrame = camPos;
            camPos = Camera.Position;
            float difference = camPos.X - camPosLastFrame.X; // Suckx for negative X-CamPosition (Screenspace)!
            if (difference <= 0) 
                difference *= -1;//= 0; 
            _background.Move(ConvertUnits.ToDisplayUnits(new Vector2(difference / 200, 0))); // _background.Move(ConvertUnits.ToDisplayUnits(Vector2.UnitX / 30));
            _clouds.Move(ConvertUnits.ToDisplayUnits(new Vector2(difference / 100, 0))); // _clouds.Move(ConvertUnits.ToDisplayUnits(Vector2.UnitX / 20));

            // Update Towers
            for (int i = 0; i < physicBreakables.Count; i++)
            {
                physicBreakables[i].Update(gameTime);
            }

            // Update Shots
            if (ScreenManager.Input.IsNewMouseButtonPress(MouseButtons.LeftButton))
            {
                // Create new Shot if Left Mouse is pressed
                Shots.Add(new Shot(false));
                Vector2 particlePos = GameDemo1.vehicle.cannon.Body.Position;
                particlePos.X += 0.3f;
                particleManager.AddCannonSmoke(particlePos);
                

                if (Shots.Count > 10)           // if we have reached more than 10 shots, remove the oldest shot
                {
                    Shots[0].shotBody.Dispose();
                    Shots.RemoveAt(0);
                }
                // And play some Cannon sound
                Sound.Play(Sound.Sounds.CannonShot);
            }

            for (int i = 0; i < Shots.Count; i++)
            {
                Shot shot = Shots[i];
                shot.Update();
                shot.shotBody.IgnoreCollisionWith(vehicle.Body);
                shot.shotBody.IgnoreCollisionWith(vehicle.WheelBackBody);
                shot.shotBody.IgnoreCollisionWith(vehicle.WheelFrontBody);
                shot.shotBody.IgnoreCollisionWith(vehicle.cannon.Body);
                if (Shots[i].shouldbedisposed == true)
                {
                    Shots[i].shotBody.Dispose();
                    Shots.RemoveAt(i);
                }
            }

            // Update Actors
            level.Update();

            // Update Triggers!
            for (int i = 0; i < Trigger.Triggers.Count; i++)
            {
                Trigger trigger = Trigger.Triggers[i];
                trigger.Update();
            }
            
            // Update Eyes
            foreach (Eye eye in Eye.Eyes)
                eye.Update();

            // Update Bridges
            bridge1.Update();

            // Update traps and Stuff
            trap.Update();

            // Update fucking particles!!!
            particleManager.Update();


            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            // Draw Background and clouds in seperate Spritebatch call without using View Matrix
            ScreenManager.SpriteBatch.Begin();

            _background.Draw(ScreenManager.SpriteBatch);
            _clouds.Draw(ScreenManager.SpriteBatch);
//             DrawDebug();
//             healthBar.Draw();
            ScreenManager.SpriteBatch.End();

            // Draw all background Objects in seperate Spritebatch call, else Background Objects will appear in front of the towers
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            foreach (StaticObject obj in StaticObject.Objects)
                obj.Draw();
            foreach (AnimatedObject obj in AnimatedObject.Objects)
                obj.Draw();
            // render eyes
            foreach (Eye eye in Eye.Eyes)
                eye.Draw();
            ScreenManager.SpriteBatch.End();



            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);


            particleManager.Draw();

            // Render breakables Towers before Level part and their cannons before themselves
            foreach (PhysicBreakable breakable in physicBreakables)
            {
                if (breakable.WithCannon == true)
                    breakable.cannon.Draw();
            }

            foreach (PhysicBreakable breakable in physicBreakables)       
                      breakable.Draw(gameTime);

            // Render Actors
            level.Draw();

            // draw shots
            foreach (Shot shot in Shots)
                shot.Draw();

            // Render Level parts
            foreach (PhysicTexture tex in physicTextures)
                tex.Draw();




            // draw fadeup Texts
            fadeUp.Draw();

            // draw Bridges
            bridge1.Draw(0.3f);
            chainBridge1.Draw(0.2f);
            trap.Draw();

            ScreenManager.SpriteBatch.End();

            // Draw All UI stuff without using View Matrix
            ScreenManager.SpriteBatch.Begin();
            DrawDebug();
            healthBar.Draw();
            ScreenManager.SpriteBatch.End();
             

            base.Draw(gameTime);
        } // Draw
        #endregion

        #region DrawDebug
        private void DrawDebug()
        {
            // Display HighScore
            ScreenManager.SpriteBatch.DrawString(Style.FontLarge, "Score: " + GameSettings.HighScore, new Vector2(20, 0), Color.DarkRed);

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "VehicleMass = " + vehicle.Body.Mass, new Vector2(30, 65), Color.Black);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "MousePosScreen = " + ScreenManager.Input.Cursor, new Vector2(30, 85), Color.Black);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "FixtureFriction = " + vehicle.wheelBackfix.Friction, new Vector2(30, 105), Color.Black);
            
            Vector2 cursorPositionInSimulationSpace = ConvertUnits.ToDisplayUnits(Camera.ConvertScreenToWorld(ScreenManager.Input.Cursor));
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "MotorTorque = " + vehicle._springBack.MaxMotorTorque, new Vector2(30, 125), Color.Black);

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "MousePosWorld = " + Camera.ConvertScreenToWorld(ScreenManager.Input.Cursor), new Vector2(30, 145), Color.Black);
//             string text = String.Format("enemy state: {0}", Level.enemy.State);
//             ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, text, new Vector2(30, 145), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);


            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "Maxspeed = " + vehicle._maxSpeed, new Vector2(30, 165), Color.Black);

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "vehicle Rotation= " + vehicle.Body.Rotation, new Vector2(30, 185), Color.Black);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "CarIsOnground= " + vehicle.IsOnground, new Vector2(30, 205), Color.Black);

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "CarVelocity= " + vehicle.Body.LinearVelocity, new Vector2(30, 225), Color.Black);

            //ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "CarsmokeBody.Position= " + vehicle.smokeBody.Position, new Vector2(30, 245), Color.Black);

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "Car exhaust Position= " + particleManager.exhaustPosition, new Vector2(30, 265), Color.Black);
//             ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "VehicleActorPos= " + vehicle.Position, new Vector2(30, 185), Color.Black);

//             ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "VehicleBodyRot= " + vehicle.Body.Rotation, new Vector2(30, 205), Color.Black);
//             ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "VehicleActorRot= " + vehicle.Rotation, new Vector2(30, 225), Color.Black);

//             ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "HealthCurrent= " + HealthBar.hitPoints, new Vector2(30, 285), Color.Black);
//             ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "HealthMax= " + HealthBar.maxHitpoints, new Vector2(30, 305), Color.Black);

           // ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "BreakablePos= " + physicBreakables[0].body.Position, new Vector2(30, 345), Color.Black);
            //ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "CannonPos= " + physicBreakables[0].cannon.Body.Position, new Vector2(30, 365), Color.Black);
            // display Hitpoints
            //int yOffset = 85;
            //int i = 0;
//             foreach (PhysicBreakable breakable in physicBreakables)
//             {
//                 ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "Tower " + i + " Hitpoints = " + breakable.hitpoints, 
//                                                      new Vector2(30, yOffset += 20), Color.Black);
//                 i++;   
//             }


            //ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "cannonScreen = " + cannon.cannonScreen, new Vector2(30, 105), Color.Black);
            //ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "_cursorDistance = " + cannon._cursorDistance, new Vector2(30, 125), Color.Black);
            //ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "_angleRadians = " + cannon._angleRadians, new Vector2(30, 145), Color.Black);
            //ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "rotationPoint = " + cannon.rotationPoint, new Vector2(30, 165), Color.Black);
            //ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, "angle = " + cannon.angle, new Vector2(30, 185), Color.Black);
        } // DrawDebug()
        #endregion
    } // class GameDemo1
} // namespace PanzerAdmiral.Demos
