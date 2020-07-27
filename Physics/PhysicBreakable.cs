using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using PanzerAdmiral.Helpers;
using FarseerPhysics.Common;
using PanzerAdmiral.ScreenSystem;
using FarseerPhysics.Common.Decomposition;
using Microsoft.Xna.Framework.Input;
using PanzerAdmiral.Demos;
using FarseerPhysics.Dynamics.Joints;
using PanzerAdmiral.Sounds;
using PanzerAdmiral.Graphics;

namespace PanzerAdmiral.Physics
{
    #region PhysicBreakable class
    /// <summary> Do segment Counter for disabling collision with objects? </summary>
    public class PhysicBreakable
    {
        #region Variables
        public bool _broken = false;
        public BreakableBody _breakableBody;
        public Body body;


        private bool _reselect = false;

        private BasicEffect _effectXna;

        private Vector2 _size;
        private Vector2 _origin;

        private Texture2D _textureBodyXna;

        private Texture2D _diffuseTexture;

        private Body[] _parts = new Body[0];
        private Dictionary<Guid, FixtureInfo> _infos = new Dictionary<Guid, FixtureInfo>();

        public int hitpoints;
        bool disableLevelCollision = false;

        // Create Cannons
        public bool WithCannon;
        public Cannon cannon;
        private RevoluteJoint _fix;
        private Shot enemyShot;
        private float ShotTimer = 3.0f; //Shussintervall
        private float TimeSinceShot; //Vergangene Zeit seit letztem Schuss
        public float AggroRange = 700; // Sichtradius
        public bool MayFire = false;   // Sichtradius Tower überschneidet sich mit Sichtradius Player
        public float DeleteAfter = 3.0f; //Zeit nach der zerbrochene Tower gelöscht werden
        private float TimeBroken; //Zeit die in Tower schon zerbrochen ist, nach Reset is der Wert komisch, aber es geht^^


        /// <summary> Animation used in BreakableTrigger, just moves up and down </summary>
        public bool Animate = false;
        /// <summary> Wanna have explosion? </summary>
        public bool Explode = true;

        #endregion

        #region Constructor
        /// <summary>
        /// Create PhysicBreakable
        /// </summary>
        /// <param name="position">set position</param>
        /// <param name="setHitPoints">set HitPoints, object breaks if this value is lesser equal zero</param>
        /// <param name="setDiffuseTexture">set diffuseTexture</param>
        /// <param name="setCollisionTexture">set collisionTexture</param>
        public PhysicBreakable(Vector2 position, int setHitPoints, string setDiffuseTexture, string setCollisionTexture, bool WithCannon)
        {
            this.WithCannon = WithCannon;
            // set Hitpoints
            hitpoints = setHitPoints;

            // load collision and diffuse Texture
            _textureBodyXna = ScreenManager.Content.Load<Texture2D>(setCollisionTexture);
            _diffuseTexture = ScreenManager.Content.Load<Texture2D>(setDiffuseTexture);

            // initialize Effect
            _effectXna = new BasicEffect(ScreenManager.Device);
            _effectXna.Texture = _diffuseTexture;
            _effectXna.TextureEnabled = true;

            // Create the body an set the position
            CreateBody();
            body.Position = position;
            body.UserData = "enemyTower";

            if (WithCannon == true)
            {

                cannon = new Cannon(true);
                body.IgnoreCollisionWith(cannon.Body);

                cannon.Body.IgnoreCollisionWith(GameDemo1.vehicle.Body);
                cannon.Body.IgnoreCollisionWith(GameDemo1.vehicle.WheelBackBody);
                cannon.Body.IgnoreCollisionWith(GameDemo1.vehicle.WheelFrontBody);
                cannon.Body.IgnoreCollisionWith(GameDemo1.vehicle.cannon.Body);

                // TODO Cannon fix
                //_fix = new RevoluteJoint(cannon.Body, body, body.GetLocalVector(cannon.Body.Position), cannon.Body.GetLocalVector(body.Position));
                _fix = new RevoluteJoint(cannon.Body, body, body.GetLocalVector(cannon.Body.Position), new Vector2(0f, -1f));
                _fix.MotorSpeed = 1.0f;
                _fix.MaxMotorTorque = 1000f;
                _fix.MotorEnabled = true;
                PhysicsGameScreen.World.AddJoint(_fix);

                enemyShot = new Shot(true);
                enemyShot.shotBody.UserData = "enemyShot";
                enemyShot.shotBody.Position = body.Position;
                //enemyShot.shotBody.IgnoreGravity = true;
                enemyShot.shotBody.IgnoreCollisionWith(cannon.Body);
                body.IgnoreCollisionWith(enemyShot.shotBody);
            }
        } // PhysicBreakable(position, diffuseTexture, collisionTexture)
        #endregion

        #region Delete
        public void Delete()
        {
            for (int i = 0; i < _parts.Length; i++)
                _parts[i].Dispose();

            GameDemo1.physicBreakables.Remove(this);
        }
        #endregion

        #region CreateBody
        private void CreateBody()
        {
            uint[] data = new uint[_textureBodyXna.Width * _textureBodyXna.Height];
            _textureBodyXna.GetData(data);

            Vertices textureVertices = PolygonTools.CreatePolygon(data, _textureBodyXna.Width, true);

            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);


            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1));
            List<Vertices> triangulated = BayazitDecomposer.ConvexPartition(textureVertices);

            triangulated.ForEach(v => v.Scale(ref vertScale));

            _size = _calcSize(triangulated.SelectMany(v => v));
            _origin = -centroid * vertScale;

            body = CreateBodyByList(triangulated);

            _parts = new Body[0];
            _infos.Values.ToList().ForEach(info => info.Effect.Dispose());
            _infos.Clear();

            foreach (Fixture fixture in body.FixtureList)
            {
                Guid id = Guid.NewGuid();

                fixture.UserData = id;
                _infos[id] = new FixtureInfo(_effectXna.Clone(), fixture, _origin,
                                             new Vector2(_diffuseTexture.Width, _diffuseTexture.Height)); //this.TextureSize
            }

        } // CreateBody()
        #endregion

        #region CalcSize
        private Vector2 _calcSize(IEnumerable<Vector2> vert)
        {
            return new Vector2(vert.Max(v => v.X) - vert.Min(v => v.X),
                               vert.Max(v => v.Y) - vert.Min(v => v.Y));
        } // _calcSize(vertices)
        #endregion

        #region CreateBodyByList
        /// <summary>
        /// Creates a breakable Body from a list of Vertices
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Body CreateBodyByList(List<Vertices> list)
        {
            // if we already have a breakable body, clear it up!
            if (_breakableBody != null)
            {
                PhysicsGameScreen.World.BreakableBodyList.Remove(_breakableBody);
                _breakableBody.MainBody.Dispose();
                _breakableBody = null;
            }

            // this object is not broken initially
            _broken = false;
            _breakableBody = new BreakableBody(list, PhysicsGameScreen.World, 10);
            _breakableBody.Strength = float.MaxValue;                   // don't break when falling down, we use hitpoints!
            _breakableBody.MainBody.Mass = 10000f;
            PhysicsGameScreen.World.AddBreakableBody(_breakableBody);



            return _breakableBody.MainBody;
        } // CreateBodyByList(list)
        #endregion

        #region Update
        /// <summary>
        /// Updates the PhysicBreakable
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public void Update(GameTime gameTime)
        {
            // used in BreakableTrigger
            if (Animate)
            {

                // Bounce control constants
                float bounce;
                const float BounceHeight = 0.18f;//0.18f;
                const float BounceRate = 3.0f;
                //const float BounceSync = -0.75f;

                // Bounce along a sine curve over time.
                // Include the X coordinate so that neighboring gems bounce in a nice wave pattern ?            
                double t = gameTime.TotalGameTime.TotalSeconds * BounceRate; //+ body.Position.X * BounceSync;
                bounce = (float)Math.Sin(t) * BounceHeight;// *ConvertUnits.ToSimUnits(_textureBodyXna.Height);// *32; //texture.Height; 

                Vector2 position = body.Position;
                position.Y -= bounce;
                body.Position = position;
            }

            if (body.UserData.ToString() == "damage")
            {
                hitpoints -= GameSettings.PlayerDamage;
                body.UserData = "enemyTower";
            }

            // Is Shooting for the Tower allowed?
            for (int t = 0; t < GameDemo1.physicBreakables.Count; t++)
            {
                if (Vector2.Distance(GameDemo1.vehicle.Position, ConvertUnits.ToDisplayUnits(GameDemo1.physicBreakables[t].body.Position))
                        < GameDemo1.vehicle.Radius + GameDemo1.physicBreakables[t].AggroRange)

                    GameDemo1.physicBreakables[t].MayFire = true;
                else
                    GameDemo1.physicBreakables[t].MayFire = false;
            }

            // Break the Body if we have no more hitpoints and Start the Timer
            if (hitpoints <= 0 && _breakableBody != null)
            {
                _breakableBody.Break();
                disableLevelCollision = true;
                TimeBroken += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (TimeBroken >= DeleteAfter)
            {
                Delete();

                if (WithCannon == true)
                {
                    cannon.Body.Dispose();

                    if (enemyShot != null)
                        enemyShot.shotBody.Dispose();
                }
               TimeBroken -= DeleteAfter;
            }

            if (_breakableBody != null && _broken != _breakableBody.Broken)
            {
                _reselect = _broken = true;
            }

            if (_reselect || _parts.Length == 0)
            {
                _parts = PhysicsGameScreen.World.BodyList.Where(
                    b => b.FixtureList.Any(
                        f => f.UserData is Guid && _infos.ContainsKey((Guid)f.UserData))
                        ).ToArray();

                // if we are broken, disable collision with level or car
                if(disableLevelCollision)
                    foreach (Body b in _parts)
                    {
                        b.IgnoreCollisionWith(GameDemo1.vehicle.Body);
                        b.IgnoreCollisionWith(GameDemo1.vehicle.WheelBackBody);
                        b.IgnoreCollisionWith(GameDemo1.vehicle.WheelFrontBody);
                        b.IgnoreCollisionWith(GameDemo1.vehicle.cannon.Body);
                    }

                // case when we are Destroyed
                if (_reselect == true)
                {
                    // Only explode if this are no background object. else vehicle gets raped too hard :(
                    if (Explode)
                    {
                        Vector2 explosionDirection = GameDemo1.vehicle.cannon.Body.Position - _breakableBody.MainBody.Position;
                        explosionDirection.Normalize();
                        new Explosion(_breakableBody.MainBody.Position + explosionDirection, 200);
                    }

                    // still play explosion sound, else it's too booring!
                    Sound.Play(Sound.Sounds.Explosion);
                    GameDemo1.particleManager.AddExplosion(body.Position);

                    // receive Score
                    GameSettings.HighScore += (uint)GameSettings.TowerScore;
                    FadeUp.AddTextMessage("+ " + GameSettings.TowerScore, FadeUp.TextTypes.Score, body.Position);

                    // Don't animate any more, causes errors!
                    Animate = false;
                    body.IgnoreGravity = false; // Gravity enabled = better performance + look&feel!
                    if (WithCannon) // drop a Pickup if this breakable has a cannon
                        new PickUp("AI/health", body.Position, 2.0f);
                }
                _reselect = false;
            }

            // Update cannon and shots if we are not broken
            if (!_broken && WithCannon == true)
            {
                if (enemyShot != null && enemyShot.shotBody.UserData.ToString() == "disable")
                {
                    enemyShot.shotBody.Dispose();
                    enemyShot = null;
                }

                cannon.Update();
                TimeSinceShot += (float)gameTime.ElapsedGameTime.TotalSeconds; //Zeit seit letztem Update dazuaddieren

                if (TimeSinceShot >= ShotTimer) //Wenn die vergangene Zeit größer ist als der Timer, do...
                {
                    TimeSinceShot = 0;

                    if (MayFire == true)
                    {
                        Sound.Play(Sound.Sounds.CannonShot);
                        GameDemo1.particleManager.AddEnemyCannonParticles(cannon.Body);
                        UpdateShot();
                    }
                }
            }

        } // Update(gameTime)
        #endregion

        #region UpdateShot
        /// <summary>
        /// Updates the cannon and its shot direction
        /// </summary>
        private void UpdateShot()
        {
            Vector2 vehiclePosition = GameDemo1.vehicle.Body.Position;

            // TODO simplify! get right Position (shot? cannon?)
            Vector2 shotScreen =  cannon.Body.Position;
            Vector2 shotDirection = vehiclePosition - shotScreen;
            shotDirection.Normalize();

                // Dipose old Body first
                //enemyShot.shotBody.Dispose();

                enemyShot = new Shot(true);
                enemyShot.shotBody.UserData = "enemyShot";
                enemyShot.shotBody.Position = body.Position;
                //enemyShot.shotBody.IgnoreGravity = true;
                enemyShot.shotBody.IgnoreCollisionWith(cannon.Body);
                body.IgnoreCollisionWith(enemyShot.shotBody);
                enemyShot.shotBody.ApplyLinearImpulse(shotDirection * 500f);
            
        } // UpdateCannonsAndShot()
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {
            // Draw shot if we have a cannon and the shot is not disposed!
            if (WithCannon == true && enemyShot != null)
                enemyShot.Draw();

            foreach (Body b in _parts)
            {
                Transform xf;
                b.GetTransform(out xf);

                if (b.FixtureList == null) continue;

                foreach (Fixture f in b.FixtureList)
                {
                    if (f.UserData == null) continue;

                    Guid id = (Guid)f.UserData;
                    if (_infos.ContainsKey(id))
                    {
                        var info = _infos[id];
                        BasicEffect effect = ((BasicEffect)info.Effect);
                        effect.World = Matrix.CreateRotationZ(xf.Angle) * Matrix.CreateTranslation(xf.Position.X, xf.Position.Y, 0);
                        effect.View = PhysicsGameScreen.Camera.SimView;
                        effect.Projection = PhysicsGameScreen.Camera.SimProjection;

                        //GI.PolygonBatch.Draw(info.VertexBuffer, info.IndexBuffer, info.Effect);
                        info.Draw(xf);
                    }
                } // foreach (Fixture)
            } // foreach (Body)

            // Draw Circle
            if (ScreenManager.Input.KeyboardState.IsKeyDown(Keys.Y))
                DrawCircle(AggroRange);

        } // Draw(GameTime gameTime)
        #endregion

        #region DrawCircle
        public void DrawCircle(float radius)
        {

            // use transparent white color with 80 alpha
            Color color = new Color(255, 255, 255, 80);
            Texture2D texture = PanzerAdmiral.AI.Style.NodeTexture;
            int textureRadius = texture.Width / 2;

            float scale = radius / textureRadius;
            Vector2 origin = new Vector2(textureRadius, textureRadius);
            ScreenManager.SpriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(this.body.Position), null, color, 0f, origin, scale, SpriteEffects.None, 1f);


        } // DrawCircle(radius)
        #endregion
    } // class PhysicBreakable
    #endregion

    #region FixtureInfo class
    class FixtureInfo
    {
        #region Variables
        public Effect Effect;

        private int[] _indices;
        private VertexPositionColorTexture[] _vertices;

        public IndexBuffer IndexBuffer;
        public VertexBuffer VertexBuffer;
        #endregion

        #region Constructor
        public FixtureInfo(Effect effect, Fixture f, Vector2 topLeft, Vector2 textureSize)
        {
            Effect = effect;
            PolygonShape shape = (PolygonShape)f.Shape;
            int vertexCount = shape.Vertices.Count;

            List<int> indices = new List<int>();
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();

            // Generate Vertices
            foreach (Vector2 vertex in shape.Vertices)
            {
                vertices.Add(new VertexPositionColorTexture(new Vector3(vertex, 0), Color.Blue,
                                                            ConvertUnits.ToDisplayUnits(vertex + topLeft) / textureSize));
            }

            // Generate Inidces
            for (int i = 1; i < vertexCount - 1; i++)
            {
                indices.AddRange(new int[] { 0, i, i + 1 });
            }

            _indices = indices.ToArray();
            _vertices = vertices.ToArray();

            IndexBuffer = new IndexBuffer(ScreenManager.Device, typeof(int), _indices.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData(_indices);

            VertexBuffer = new VertexBuffer(ScreenManager.Device, VertexPositionColorTexture.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(_vertices);
        }
        #endregion

        #region Draw
        public void Draw(Transform xf)
        {
//             this.Effect.Parameters["World"].SetValue(Matrix.CreateRotationZ(xf.Angle) * 
//                                                      Matrix.CreateTranslation(xf.Position.X, xf.Position.Y, 0));

            ScreenManager.Device.Indices = this.IndexBuffer;
            ScreenManager.Device.SetVertexBuffer(this.VertexBuffer);

            foreach (var pass in this.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                ScreenManager.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0, _indices.Length / 3);
            }
        } // Draw(Transform)
        #endregion
    } // class FixtureInfo
    #endregion
} // namespace PanzerAdmiral.Physics
