using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;
using FarseerPhysics.Dynamics;
using PanzerAdmiral.Helpers;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

namespace PanzerAdmiral.Demos
{
    public class Trigger
    {
        public static List<Trigger> Triggers = new List<Trigger>();

        public Vector2 Position = Vector2.Zero;
        public bool Activated = false;

        private static int Width = 20;
        private static int Height = 100;

//         public static Body body = BodyFactory.CreateRectangle(PhysicsGameScreen.World, ConvertUnits.ToSimUnits(Width),
//                                               ConvertUnits.ToSimUnits(Height), 1.0f);
        public Body body;

        #region Constructor
        /// <summary>
        /// Create Trigger at position
        /// </summary>
        /// <param name="setPosition">set Trigger position in sim space</param>
        public Trigger(Vector2 setPosition)
        {
            Position = setPosition;

            body = BodyFactory.CreateRectangle(PhysicsGameScreen.World, ConvertUnits.ToSimUnits(Width),
                                              ConvertUnits.ToSimUnits(Height), 1.0f);
            body.BodyType = BodyType.Static;
            //body.IgnoreGravity = true;
            body.OnCollision += new OnCollisionEventHandler(Body_OnCollision);
            body.Position = Position;
            body.UserData = "trigger";

            Triggers.Add(this);

        } // Trigger(setPosition)
        #endregion


        #region Body_OnCollision
        /// <summary>
        /// Body_OnCollision, Checks collision between Player and the Trigger
        /// </summary>
        /// <param name="fixtureA">Trigger fixture</param>
        /// <param name="fixtureB">player fixture</param>
        /// <param name="contact">contact</param>
        /// <returns>false, don't collide on pickup</returns>
        public bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // if we have some valid userdata
            if (fixtureB.Body.UserData != null)
            {
                // if this fixture is the player
                if (fixtureB.Body.UserData.ToString() == "player" && fixtureA.Body.UserData.ToString() == "trigger")
                {
                    // and if we have not already activated the Trigger
                    if (Activated == false)
                    {
                        // activate this Trigger
                        Activated = true;
                        //fixtureA.UserData = "disable";
                        fixtureA.Body.Dispose();

                        // and do the event logic stuff
                        DoEventLogic();

                    } // if (pickedUp == false)
                } // if ("player")

            } // if (UserData != null )

            return false;

        } // Body_OnCollision
        #endregion

        #region DoEventLogic
       /// <summary> Do Trigger Event Logic </summary>
        public virtual void DoEventLogic()
        {
            // just fadeup some debug stuff
            PanzerAdmiral.Graphics.FadeUp.AddTextMessage(this.GetType().Name, PanzerAdmiral.Graphics.FadeUp.TextTypes.Heal, GameDemo1.vehicle.Body.Position);
        } // DoEventLogic()
        #endregion

        /// <summary> Update Trigger </summary>
        public virtual void Update()
        {

        } // Update()

    } // class Trigger
} // namespace PanzerAdmiral.Demos
