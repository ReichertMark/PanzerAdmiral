using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PanzerAdmiral.AI;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Helpers;

namespace PanzerAdmiral.Demos
{
    /// <summary> 
    /// Level class.
    /// manages all actors and Pickups in the scene 
    /// </summary>
    public class Level
    {
        LevelEditor levelEditor;

       
        public Level()
        {
            // Load fonts and debug stuff
            Style.LoadContent();

            levelEditor = new LevelEditor();

            //LoadLevel1();
            //LoadLevel2();
            //LoadLevel3();


            Load();

        } // Level()

        #region Load
        public void Load()
        {
            // Load map Nodes
            NodeIO.LoadNodes("map.txt");

        }
        #endregion

        #region Unload
        /// <summary> Clears Lists of actors and Pickups </summary>
        public void Unload()
        {
            Actor.Actors.Clear();
            PickUp.PickUps.Clear();
        }
        #endregion


        #region LoadLevel3
        /// <summary> Tests Actor Selection </summary>
        private void LoadLevel3()
        {
            for (int i = 0; i < 3; i++)
            {
                Actor actor = new Actor("AI/arrow", Color.White);
                actor.Position = new Vector2(i * 100 + 300, -300);
            }

            //Actor.Actors[0].Select();
            //Actor.Actors[2].Select();

        }
        #endregion

        #region LoadLevel2
        /// <summary> Line Drawing Test </summary>
        private void LoadLevel2()
        {
            Actor mouseActor;
            Actor actorA;
            mouseActor = new Actor("AI/circle16", Style.NodeColor);
            actorA = new Actor("AI/circle16", Style.NodeColor);
            actorA.Position = new Vector2(400, -300);
        }
        #endregion

        #region LoadLevel1
        private void LoadLevel1()
        {

            List<Actor> obstacles = new List<Actor>();
            Color obstacleColor = new Color(255, 200, 96);

            for (int i = 0; i < 4; i++)
            {
                obstacles.Add(new Actor("Common/arrow", obstacleColor));
            }

            obstacles[0].Position = new Vector2(250, -200);
            obstacles[1].Position = new Vector2(530, -200);
            obstacles[2].Position = new Vector2(250, -400);
            obstacles[3].Position = new Vector2(530, -400);

            Actor leader = new Actor("Common/arrow", new Color(64, 255, 64));
            leader.Speed = 1.0f;
            leader.Direction = Actor.GetRandomDirection();
            leader.Position = new Vector2(0, -100); // Actor.GetRandomPosition(1280, 720);
            //leader.BehaviorList.Add(new BehaviorConstant(0.1f, new Vector2(1, 0))); // Direction pointing right
            leader.BehaviorList.Add(new BehaviorInput(0.5f));
            leader.BehaviorList.Add(new BehaviorWander(0.05f, 60)); // if we have 60 fps, this changes the wander direction every second

            Behavior seek = new BehaviorSeek(0.05f, leader);

            for (int i = 0; i < 10; i++)
            {
                Actor drone = new Actor("Common/arrow", Color.White);
                drone.Speed = 0.9f;
                drone.Direction = Actor.GetRandomDirection();
                drone.Position = new Vector2(0, -200);
                drone.BehaviorList.Add(seek);
                drone.BehaviorList.Add(new BehaviorWander(0.03f, 15));      // flocking result, better: avoid behavior

                // drone must avoid each of the multiple obstacles
                foreach (var obstacle in obstacles)
                {
                    drone.BehaviorList.Add(new BehaviorAvoid(0.2f, obstacle, 60));
                }
            }

            // leader shall also avoid the obstacles 
            foreach (var obstacle in obstacles)
            {
                leader.BehaviorList.Add(new BehaviorAvoid(0.2f, obstacle, 60));
            }
        }
        #endregion

        #region Update
        /// <summary> Updates all Actors in the Level</summary>
        public void Update()
        {
            for (int i = 0; i < Actor.Actors.Count; i++)
            {
                Actor.Actors[i].Update();
            }

        } // Update()
        #endregion

        #region Draw
        /// <summary> Draws all Actors in the Level</summary>
        public void Draw()
        {
            // Draw actors
            foreach (Actor actor in Actor.Actors)
            {
                actor.Draw();
            }

            //levelEditor.Draw();

            //Line.DrawLine(Style.FillTexture, Style.LineColor, actorA, mouseActor, 0);

            // Draw all Pickups
            foreach (PickUp pickUp in PickUp.PickUps)
            {
                pickUp.Draw();
            }

        }
        #endregion
    } // class Level
} // namespace PanzerAdmiral.Demos
