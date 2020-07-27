using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;
using Microsoft.Xna.Framework.Input;
using System;

namespace PanzerAdmiral.AI
{
    #region Behavior Class
    /// <summary> Behavior class </summary>
    public class Behavior
    {
        /// <summary> Weight describes how much the behavior will infect the actor's movement </summary>
        protected float Weight;

        /// <summary>
        /// Create Behavior
        /// </summary>
        /// <param name="setWeight"></param>
        /// <returns></returns>
        public Behavior(float setWeight)
        {
            Weight = setWeight;
        } // Behavior(setWeight)


        /// <summary>
        /// Update Actor Behavior
        /// </summary>
        /// <param name="actor">The actor that gets infected by the Behavior</param>
        public virtual void Update(Actor actor) { }
    } // class Behavior
    #endregion

    #region BehaviorConstant class
    /// <summary> Constant Behavior influence class, influences direction taking the weight into account </summary>
    class BehaviorConstant : Behavior
    {
        Vector2 direction;

        public BehaviorConstant(float weight, Vector2 direction)
            : base(weight)
        {
            this.direction = direction;
        }

        /// <summary>
        /// Affect the actor's direction
        /// </summary>
        /// <param name="actor">The actor that gets infected by the Behavior</param>
        public override void Update(Actor actor)
        {
            actor.Direction += direction * this.Weight;

        } //  Update(actor)
    } // class BehaviorConstant
    #endregion

    #region BehaviorInput class
    /// <summary> Updates the Direction via Input </summary>
    class BehaviorInput : Behavior
    {
        public BehaviorInput(float weight)
            : base(weight)
        {
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="actor">set actor</param>
        public override void Update(Actor actor)
        {
            if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Right))
                actor.Direction += new Vector2(1, 0) * Weight;
             if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Left))
                 actor.Direction += new Vector2(-1, 0) * Weight;
             if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Up))
                 actor.Direction += new Vector2(0, -1) * Weight;
             if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Down))
                 actor.Direction += new Vector2(0, 1) * Weight;

        } // Update(actor)
    } // class BehaviorInput
    #endregion

    #region BehaviorWander class
    class BehaviorWander : Behavior
    {
        static Random random = new Random();
        int changeInterval; // direction change interval
        int tick;
        Vector2 direction;

        /// <summary>
        /// Create BehaviorWander
        /// </summary>
        /// <param name="weight">set weight</param>
        /// <param name="changeInterval">Determines when the wandering will change the direction (60 fps)</param>
        /// <returns></returns>
        public BehaviorWander(float weight, int changeInterval)
            : base(weight)
        {
            this.changeInterval = changeInterval;
        }


        /// <summary>
        /// Updates the wander behavior
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public override void Update(Actor actor)
        {
            // get random direction
            if (tick == 0)
                this.direction = Actor.GetRandomDirection();

            tick++;
            tick %= changeInterval;

            actor.Direction += direction * Weight;

        } //  Update(Actor actor)

    } // class BehaviorWander
    #endregion

    #region BehaviorSeek class
    class BehaviorSeek : Behavior
    {
        /// <summary> Target Actor the affected actor shall seek </summary>
        Actor target;

        /// <summary>
        /// Create Seek Behavior
        /// </summary>
        /// <param name="weight">set weight</param>
        /// <param name="target">set Target Actor</param>
        /// <returns></returns>
        public BehaviorSeek(float weight, Actor target)
            : base(weight)
        {
            this.target = target;
        }

        /// <summary>
        /// Update Seek Actor
        /// </summary>
        /// <param name="actor">Actor to influence</param>
        public override void Update(Actor actor)
        {
            // Position that will point from the actor to the target
            Vector2 targetDirection = target.Position - actor.Position;

            // normalize and modify direction
            if(targetDirection != Vector2.Zero)
                targetDirection.Normalize();

            actor.Direction += targetDirection * this.Weight;
        } // Update(Actor actor)

    } // class BehaviorSeek
    #endregion

    #region BehaviorAvoid class
    class BehaviorAvoid : Behavior
    {
        /// <summary> Target Actor </summary>
        Actor target;
        float radius;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight">set weight</param>
        /// <param name="target">Target Actor</param>
        /// <param name="radius">set obstacle radius</param>
        public BehaviorAvoid(float weight, Actor target, float radius)
            : base(weight)
        {
            this.target = target;
            this.radius = radius;
        } // (weight, target, radius)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor">The actor that gets infected by the Behavior</param>
        public override void Update(Actor actor)
        {
            // also contains distance between objects
            Vector2 targetDirection = actor.Position - target.Position;
            if (targetDirection.Length() < radius)
            {
                // normalize and modify direction
                if (targetDirection != Vector2.Zero)
                    targetDirection.Normalize();

                actor.Direction += targetDirection * this.Weight;
            }

        } // Update( actor)

    } // class BehaviorAvoid
    #endregion

} // namespace PanzerAdmiral.AI
