using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System.Runtime.Serialization;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Helpers;

namespace PanzerAdmiral.Graphics
{
    /// <summary>
    /// Animation class
    /// </summary>
    public class Animation 
    {
        #region Vars
        private int _rows;
        private int _columns;

        private Vector2 _size;
        private Vector2 _origin;
        private Vector2 _offset;

        private double _time;
        private long _timePerFrame;
        private float _animationSpeed;

        private int _stopFrame;
        private int _startFrame;
        private int _currentFrame;

        private bool _isLoop;
        /// <summary> isAlive has to be true (Play Button) </summary>
        private bool _isAlive;

        private float _angle;
        Texture2D texture;

        public Vector2 CurrentPosition;

        public float Scale = 1.0f;
        #endregion

        #region Properties
        public int StartFrame
        {
            get { return _startFrame; }
            set { _startFrame = value; }
        }

        public int StopFrame
        {
            get { return _stopFrame; }
            set { _stopFrame = value; }
        }

        public int FrameCount
        {
            get { return _stopFrame - _startFrame; }
        }

        public int Rows
        {
            get { return _rows; }
            set
            {
                _rows = value;
                _setSize();
            }
        }

        public int Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                _setSize();
            }
        }

        public bool IsAlive
        {
            get { return this._isAlive; }
            set { _isAlive = value; }
        }

        public int CurrentFrame
        {
            get { return this._currentFrame; }
            set
            {
                _currentFrame = value;

                if (value >= _stopFrame) _currentFrame = _stopFrame;
                if (value <= _startFrame) _currentFrame = _startFrame;
            }
        }

        public float AnimationSpeed
        {
            get { return _animationSpeed; }
            set
            {
                _animationSpeed = value;
                if (_animationSpeed <= 0) _animationSpeed = 1;
                _setSpeed();
            }
        }

        public bool Loop
        {
            get { return _isLoop; }
            set { _isLoop = value; }
        }

        public bool Ready
        {
            get
            {
                return texture != null &&
                        _rows > 0 &&
                        _columns > 0 &&
                        _stopFrame > 0 &&
                        _size.X > 0 &&
                        _size.Y > 0;
            }
        }

        public float Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        public Vector2 Size
        {
            get { return _size; }
        } // Size

        public Vector2 Origin
        {
            get { return _origin; }
        } // Origin

        public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; }
        } // Offset
        #endregion

        #region Constructor
        /// <summary>
        /// Create Animation
        /// </summary>
        /// <param name="setTexture">Animation Texture</param>
        /// <param name="rows">anzahl der Reihen</param>
        /// <param name="columns">anzahl der Spalten</param>
        /// <param name="startframe">StarFrame of animation (0-25 -> 5 Reihen, 5 Spalten)</param>
        /// <param name="stopframe">EndFrame of animation</param>
        /// <param name="animationSpeed">Zeitraum, in der die Gesamte animation abgespielt wird (für alle frames)</param>
        /// <param name="loop">loop animation? </param>
        /// <returns></returns>
        public Animation(string setTexture, int rows, int columns, int startframe, int stopframe, float animationSpeed, bool loop)
        {
            texture = ScreenManager.Content.Load<Texture2D>(setTexture);

            _rows = rows;
            _columns = columns;
            _startFrame = startframe;
            _stopFrame = stopframe;
            _animationSpeed = animationSpeed;
            _currentFrame = startframe;
            _time = 0;
            _isLoop = loop;


            _setSize();
            _setSpeed();
        }

        private void _setSize()
        {
            if (texture != null && _columns != 0 && _rows != 0)
            {
                _size = new Vector2(texture.Width / _columns, texture.Height / _rows);
                _origin = _size / 2;
            }
        } // _setSize()

        private void _setSpeed()
        {
            _timePerFrame = (long)_animationSpeed / (_stopFrame - _startFrame);
        } // _setSpeed()
        #endregion

        #region Draw
        public void Draw()
        {
            Point size = new Point((int)_size.X, (int)_size.Y);

            ScreenManager.SpriteBatch.Draw(
                texture,
                ConvertUnits.ToDisplayUnits(CurrentPosition),
                new Rectangle(
                    _currentFrame % _columns * size.X,
                    _currentFrame / _columns * size.Y,
                    size.X,
                    size.Y
                ),
                Color.White,
                _angle,
                _origin - ConvertUnits.ToDisplayUnits(_offset),
                Scale,
                SpriteEffects.None,
                0f//this.LayerDepth
            );
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            if (!_isAlive) return;

            _time += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_time > _animationSpeed && !_isLoop)
                this.Stop();

            _time %= _animationSpeed;
            _currentFrame = (int)(_time / _timePerFrame);

            if (!_isAlive) _time = 0;
        }
        #endregion

        #region Stop
        public void Stop()
        {
            _isAlive = false;
        }
        #endregion
    } // class Animation 
} // namespace PanzerAdmiral.Graphics


