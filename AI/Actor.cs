using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Helpers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace PanzerAdmiral.AI
{
    /// <summary>
    /// Actor Class, handles basic AI 
    /// TODO: Remove Body and implement in Enemy class that inherits from Actor
    /// </summary>
    public partial class Actor
    {
        #region Variables
        /// <summary> static list of all our Actors </summary>
        public static List<Actor> Actors = new List<Actor>();
        /// <summary> Random class for random number generation </summary>
        private static Random random = new Random();

        public Color Color;
        /// <summary> Actor texture </summary>
        public Texture2D Texture;
        /// <summary> actual position in screenspace</summary>
        public Vector2 Position;
        /// <summary> Farseer Simulation Units Position </summary>
        public Vector2 PositionSim;
        /// <summary> </summary>
        public Vector2 Direction;
        /// <summary> Actor speed </summary>
        public float Speed;
        /// <summary> The sprite's origin </summary>
        public Vector2 Origin;
        /// <summary> current rotation </summary>
        public float Rotation;
        /// <summary> BehaviorList </summary>
        public List<Behavior> BehaviorList = new List<Behavior>();
        /// <summary> Actor radius for selection and drawing </summary>
        public float Radius;
        /// <summary> Texture Scale </summary>
        public float Scale = 1f;
        #endregion

        #region Constructor
        /// <summary>
        /// Create Actor
        /// </summary>
        /// <param name="setTexture"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public Actor(string setTexture, Color setColor)
        {
            // add the current actor to the static list
            Actors.Add(this);
            Color = setColor;
            Texture = ScreenManager.Content.Load<Texture2D>(setTexture);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            this.Radius = this.Origin.X;
        } // Actor(setTexture, setColor)
        #endregion

        #region SetScale
        /// <summary> Supports different scaling values by recalculationg the Radius </summary>
        public void SetScale()
        {
            this.Radius = this.Origin.X * Scale;
        } // SetScale()
        #endregion

        #region Delete
        /// <summary> Remove actor from the List </summary>
        public virtual void Delete()
        {
            Actors.Remove(this);
        } // Delete()
        #endregion

        #region GetRandomPosition
        /// <summary>
        /// Get random Position using Random object
        /// </summary>
        /// <param name="rangeX">set range X</param>
        /// <param name="rangeY">set range y</param>
        /// <returns>Vector2 Position</returns>
        public static Vector2 GetRandomPosition(int rangeX, int rangeY)
        {
            return new Vector2(random.Next(rangeX), random.Next(rangeY));
        } // GetRandomPosition(int rangeX, int rangeY)
        #endregion

        #region GetRandomDirection
        /// <summary>
        /// Get Random Direction
        /// </summary>
        /// <returns>Vector2 Direction</returns>
        public static Vector2 GetRandomDirection()
        {
            double rotation = random.NextDouble() * MathHelper.TwoPi; // 2PI = full circle
            return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        } // GetRandomDirection()
        #endregion
        
        #region Update
        /// <summary> Update the list of Behaviors and the Actor's position </summary>
        public virtual void Update()
        {
            // Update the list of Behaviors
            foreach (var behavior in BehaviorList)
                behavior.Update(this);

            
            // Normalize Direction Vector if it's not zero
            if (this.Direction != Vector2.Zero)
                this.Direction.Normalize();

            // Update Position and sync physics
            Position += Direction * Speed;
            PositionSim = ConvertUnits.ToSimUnits(Position);


            // calculate rotation, do this in Update!
            Rotation = (float)Math.Atan2(Direction.Y, Direction.X) + MathHelper.PiOver2;
//            body.Position = PositionSim;
        } // Update()
        #endregion


        #region Draw
        /// <summary> Draws the Actor with rotation</summary>
        public virtual void Draw()
        {

            Color color = this.Color;
            if (this.IsSelected)
                color = Style.SelectionColor;

//            body.Rotation = Rotation;
            // Draw Sprite
            ScreenManager.SpriteBatch.Draw(Texture, Position, null, color, Rotation, Origin, Scale, SpriteEffects.None, Style.ActorLayer);

            


        } // Draw()
        #endregion

    } // class Actor
} // namespace PanzerAdmiral.AI
