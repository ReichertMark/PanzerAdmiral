using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.ScreenSystem;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using PanzerAdmiral.Helpers;
using FarseerPhysics.Dynamics.Contacts;
using PanzerAdmiral.Sounds;
using PanzerAdmiral.Graphics;

namespace PanzerAdmiral.Demos
{
    /// <summary>
    /// Collectable PickUp, Heals 20, hardcoded
    /// </summary>
    public class PickUp
    {
        #region Variables
        Body body;
        float density = 1.0f;
        Texture2D texture;
        public Vector2 Position;
        public float Scale = 1.0f;
        Vector2 origin;


        /// <summary> Static list of PickUps in our Level</summary>
        public static List<PickUp> PickUps = new List<PickUp>();

        public bool pickedUp = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Create Pickup
        /// </summary>
        /// <param name="setTexture">set texture Filename</param>
        /// <param name="setPosition">set Position in Sim Space</param>
        /// <param name="setScale">set scale</param>
        public PickUp(string setTexture, Vector2 setPosition, float setScale)
        {
            Scale = setScale;

            texture = ScreenManager.Content.Load<Texture2D>(setTexture);
            body = BodyFactory.CreateRectangle(PhysicsGameScreen.World, ConvertUnits.ToSimUnits(texture.Width) * Scale,
                                              ConvertUnits.ToSimUnits(texture.Height) * Scale, density);

            //body.IgnoreGravity = true;
            body.BodyType = BodyType.Dynamic;
            body.OnCollision += new OnCollisionEventHandler(Body_OnCollision);
            body.Position = setPosition;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);

            body.UserData = "pickup";

            PickUps.Add(this);
        } // PickUp()
        #endregion

        #region Body_OnCollision
        /// <summary>
        /// Body_OnCollision, Checks collision between Player and the Pickup.
        /// Heals the Tank 20 Hitpoints
        /// </summary>
        /// <param name="fixtureA">Pickup fixture</param>
        /// <param name="fixtureB">player fixture</param>
        /// <param name="contact">contact</param>
        /// <returns>false, don't collide on pickup</returns>
        public bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // if we have some valid userdata
            if (fixtureB.Body.UserData != null )
            {
                // if this fixture is the player
                //if (fixtureB.Body.UserData.ToString() == "player" && fixtureA.Body.UserData.ToString() == "pickup")
                if ((fixtureB.Body.UserData.ToString() == "player" || fixtureB.Body.UserData.ToString() == "wheel") && fixtureA.Body.UserData.ToString() == "pickup")
                {
                    // and if we are not already picked up by the player
                    if (pickedUp == false)
                    {
                        // pickup this
                        pickedUp = true;
                        fixtureA.UserData = "disable";
                        Sound.Play(Sound.Sounds.PickUp);
                        FadeUp.AddTextMessage("+" + GameSettings.PickupHeal, FadeUp.TextTypes.Heal, fixtureB.Body.Position);
                        HealthBar.SetHeal(GameSettings.PickupHeal);
                        fixtureA.Body.Dispose();
                        PickUps.Remove(this);

                        // don't collide at first time
                        return false;
                    } // if (pickedUp == false)
                } // if ("player")
            } // if (UserData != null )
            //else collide with the environment
            return true;
        } // Body_OnCollision
        #endregion

        #region Draw
        /// <summary>
        /// Draw PickUp
        /// </summary>
        public void Draw()
        {
            ScreenManager.SpriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(body.Position), null, Color.Red, 
                                           body.Rotation, origin, Scale, SpriteEffects.None, 0f);
        } // Draw()
        #endregion
    } // class PickUp
} // namespace PanzerAdmiral.Demos
