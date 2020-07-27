using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.Graphics;

namespace PanzerAdmiral.Helpers
{
    #region MouseButtons
    /// <summary> an enum of all available mouse buttons. </summary>
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton,
        ExtraButton1,
        ExtraButton2
    } // MouseButtons
    #endregion

    public delegate void MouseClickHandler(Vector2 position);
    public delegate void MouseMoveHandler(Vector2 position, Vector2 movement);
    public delegate void KeyHandler(Keys key, KeyboardState keyState);

    public class InputHelper
    {
        private readonly List<GestureSample> _gestures = new List<GestureSample>();
        private GamePadState _currentGamePadState;
        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;
        private GamePadState _currentVirtualState;

        private GamePadState _lastGamePadState;
        private KeyboardState _lastKeyboardState;
        private MouseState _lastMouseState;
        private GamePadState _lastVirtualState;
        private bool _handleVirtualStick;

        private Vector2 _cursor;
        private bool _cursorIsValid;
        private bool _cursorIsVisible;
        private bool _cursorMoved;
        private Sprite _cursorSprite;

        private ScreenManager _manager;
        private Viewport _viewport;

        // Mouse extensions and Event Handlers
        public static Vector2 movement;
        public static bool dragging;
        public static event MouseClickHandler MouseDown = delegate(Vector2 position) { };
        public static event MouseClickHandler MouseUp = delegate(Vector2 position) { };
        public static event MouseClickHandler StartDrag = delegate(Vector2 position) { };
        public static event MouseClickHandler EndDrag = delegate(Vector2 position) { };
        public static event MouseMoveHandler MouseMove = delegate(Vector2 position, Vector2 movement) { };

        // Keyboard extensions
        public static event KeyHandler KeyPress = delegate(Keys key, KeyboardState keyState) { };
        public static event KeyHandler KeyRelease = delegate(Keys key, KeyboardState keyState) { };
        public static List<Keys> boundKeys = new List<Keys>();

        #region Properties

        public bool IsLeftButtonDown
        {
            get { return _lastMouseState.LeftButton == ButtonState.Pressed; }
        }

        public  bool IsControlDown
        {
            get { return _lastKeyboardState.IsKeyDown(Keys.LeftControl) ||   _lastKeyboardState.IsKeyDown(Keys.RightControl); } 
        }

        public bool IsShiftDown
        {
            get { return _lastKeyboardState.IsKeyDown(Keys.LeftShift) || _lastKeyboardState.IsKeyDown(Keys.RightShift); }
        }

        public bool IsAltDown
        {
            get { return _lastKeyboardState.IsKeyDown(Keys.LeftAlt) || _lastKeyboardState.IsKeyDown(Keys.RightAlt); }
        }

        public GamePadState GamePadState
        {
            get { return _currentGamePadState; }
        }

        public KeyboardState KeyboardState
        {
            get { return _currentKeyboardState; }
        }

        public MouseState MouseState
        {
            get { return _currentMouseState; }
        }

        public GamePadState VirtualState
        {
            get { return _currentVirtualState; }
        }

        public GamePadState PreviousGamePadState
        {
            get { return _lastGamePadState; }
        }

        public KeyboardState PreviousKeyboardState
        {
            get { return _lastKeyboardState; }
        }

        public MouseState PreviousMouseState
        {
            get { return _lastMouseState; }
        }

        public GamePadState PreviousVirtualState
        {
            get { return _lastVirtualState; }
        }

        public bool ShowCursor
        {
            get { return _cursorIsVisible && _cursorIsValid; }
            set { _cursorIsVisible = value; }
        }

        public bool EnableVirtualStick
        {
            get { return _handleVirtualStick; }
            set { _handleVirtualStick = value; }
        }

        public Vector2 Cursor
        {
            get { return _cursor; }
        }

        public bool IsCursorMoved
        {
            get { return _cursorMoved; }
        }

        public bool IsCursorValid
        {
            get { return _cursorIsValid; }
        }

        #endregion


        /// <summary>
        ///   Constructs a new input state.
        /// </summary>
        public InputHelper(ScreenManager manager)
        {
            _currentKeyboardState = new KeyboardState();
            _currentGamePadState = new GamePadState();
            _currentMouseState = new MouseState();
            _currentVirtualState = new GamePadState();

            _lastKeyboardState = new KeyboardState();
            _lastGamePadState = new GamePadState();
            _lastMouseState = new MouseState();
            _lastVirtualState = new GamePadState();

            _manager = manager;

            _cursorIsVisible = false;
            _cursorMoved = false;

            _cursorIsValid = true;
            _cursor = Vector2.Zero;

            _handleVirtualStick = false;
        }

        public static void AddKey(Keys key)
        {
            boundKeys.Add(key);
        }


        public void LoadContent()
        {
            _cursorSprite = new Sprite(ScreenManager.Content.Load<Texture2D>("Common/cursor"));
            _viewport = _manager.GraphicsDevice.Viewport;
        }

        /// <summary>
        ///   Reads the latest state of the keyboard and gamepad and mouse/touchpad.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            _lastKeyboardState = _currentKeyboardState;
            _lastGamePadState = _currentGamePadState;
            _lastMouseState = _currentMouseState;
            if (_handleVirtualStick)
            {
                _lastVirtualState = _currentVirtualState;
            }

            _currentKeyboardState = Keyboard.GetState();
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
            _currentMouseState = Mouse.GetState();

            if (_handleVirtualStick)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    _currentVirtualState = GamePad.GetState(PlayerIndex.One);
                }
                else
                {
                    _currentVirtualState = HandleVirtualStickWin();
                }

            }

            _gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                _gestures.Add(TouchPanel.ReadGesture());
            }

            // Update cursor
            Vector2 oldCursor = _cursor;
            if (_currentGamePadState.IsConnected && _currentGamePadState.ThumbSticks.Left != Vector2.Zero)
            {
                Vector2 temp = _currentGamePadState.ThumbSticks.Left;
                _cursor += temp * new Vector2(300f, -300f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Mouse.SetPosition((int)_cursor.X, (int)_cursor.Y);
            }
            else
            {
                _cursor.X = _currentMouseState.X;
                _cursor.Y = _currentMouseState.Y;
            }
            _cursor.X = MathHelper.Clamp(_cursor.X, 0f, _viewport.Width);
            _cursor.Y = MathHelper.Clamp(_cursor.Y, 0f, _viewport.Height);

            if (_cursorIsValid && oldCursor != _cursor)
            {
                _cursorMoved = true;
            }
            else
            {
                _cursorMoved = false;
            }

            if (_viewport.Bounds.Contains(_currentMouseState.X, _currentMouseState.Y))
            {
                _cursorIsValid = true;
            }
            else
            {
                _cursorIsValid = false;
            }



            // Mouse Fire events
            Vector2 currentPosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);
            if(PhysicsGameScreen.Camera != null)
                currentPosition = ConvertUnits.ToDisplayUnits(PhysicsGameScreen.Camera.ConvertScreenToWorld(new Vector2(_currentMouseState.X, _currentMouseState.Y)));

            Vector2 lastPosition = new Vector2(_lastMouseState.X, _lastMouseState.Y);
            if (PhysicsGameScreen.Camera != null)
                lastPosition = ConvertUnits.ToDisplayUnits(PhysicsGameScreen.Camera.ConvertScreenToWorld(new Vector2(_lastMouseState.X, _lastMouseState.Y)));

            if (IsNewMouseButtonPress(MouseButtons.LeftButton))
            {
                MouseDown(currentPosition);
            }
            if (IsNewMouseButtonRelease(MouseButtons.LeftButton))
            {
                if (dragging)
                {
                    dragging = false;
                    EndDrag(currentPosition);
                } // if (dragging)
                else
                {
                    MouseUp(currentPosition);
                } // else
            } //  if (LeftButton release)


            //don't fire off mous event if there is no movement
            movement = currentPosition - lastPosition;
            if (movement.Length() > 0f)
            {
                // if left button down an we are not dragging, set it to true and start drag
                if (_currentMouseState.LeftButton == ButtonState.Pressed && !dragging)
                {
                    dragging = true;
                    StartDrag(currentPosition);
                }
                // Move the mouse
                MouseMove(currentPosition, movement);
            } // if (movement > 0)


            // Keyboard fire events
            for (int i = 0; i < boundKeys.Count; i++)
            {
                if (_currentKeyboardState.IsKeyDown(boundKeys[i]) && _lastKeyboardState.IsKeyUp(boundKeys[i]))
                    KeyPress(boundKeys[i], _currentKeyboardState);

                if (_currentKeyboardState.IsKeyUp(boundKeys[i]) && _lastKeyboardState.IsKeyDown(boundKeys[i]))
                    KeyRelease(boundKeys[i], _currentKeyboardState);
            }



        }

        public void Draw()
        {
            if (_cursorIsVisible && _cursorIsValid)
            {
                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(_cursorSprite.Texture, _cursor, null, Color.White, 0f, _cursorSprite.Origin, 1f, SpriteEffects.None, 0f);
                ScreenManager.SpriteBatch.End();
            }
        }

        private GamePadState HandleVirtualStickWin()
        {
            Vector2 _leftStick = Vector2.Zero;
            List<Buttons> _buttons = new List<Buttons>();

            if (_currentKeyboardState.IsKeyDown(Keys.A))
            {
                _leftStick.X -= 1f;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.S))
            {
                _leftStick.Y -= 1f;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.D))
            {
                _leftStick.X += 1f;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.W))
            {
                _leftStick.Y += 1f;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Space))
            {
                _buttons.Add(Buttons.A);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                _buttons.Add(Buttons.B);
            }
            if (_leftStick != Vector2.Zero)
            {
                _leftStick.Normalize();
            }

            return new GamePadState(_leftStick, Vector2.Zero, 0f, 0f, _buttons.ToArray());
        }

        private GamePadState HandleVirtualStickWP7()
        {
            List<Buttons> _buttons = new List<Buttons>();
            Vector2 _stick = Vector2.Zero;
            return new GamePadState(_stick, Vector2.Zero, 0f, 0f, _buttons.ToArray());
        }

        /// <summary>
        ///   Helper for checking if a key was newly pressed during this update.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            return (_currentKeyboardState.IsKeyDown(key) &&
                    _lastKeyboardState.IsKeyUp(key));
        }

        public bool IsNewKeyRelease(Keys key)
        {
            return (_lastKeyboardState.IsKeyDown(key) &&
                    _currentKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        ///   Helper for checking if a button was newly pressed during this update.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            return (_currentGamePadState.IsButtonDown(button) &&
                    _lastGamePadState.IsButtonUp(button));
        }

        public bool IsNewButtonRelease(Buttons button)
        {
            return (_lastGamePadState.IsButtonDown(button) &&
                    _currentGamePadState.IsButtonUp(button));
        }

        /// <summary>
        ///   Helper for checking if a mouse button was newly pressed during this update.
        /// </summary>
        public bool IsNewMouseButtonPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (_currentMouseState.LeftButton == ButtonState.Pressed &&
                            _lastMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (_currentMouseState.RightButton == ButtonState.Pressed &&
                            _lastMouseState.RightButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (_currentMouseState.MiddleButton == ButtonState.Pressed &&
                            _lastMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (_currentMouseState.XButton1 == ButtonState.Pressed &&
                            _lastMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (_currentMouseState.XButton2 == ButtonState.Pressed &&
                            _lastMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }


        /// <summary>
        /// Checks if the requested mouse button is released.
        /// </summary>
        /// <param name="button">The button.</param>
        public bool IsNewMouseButtonRelease(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (_lastMouseState.LeftButton == ButtonState.Pressed &&
                            _currentMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (_lastMouseState.RightButton == ButtonState.Pressed &&
                            _currentMouseState.RightButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (_lastMouseState.MiddleButton == ButtonState.Pressed &&
                            _currentMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (_lastMouseState.XButton1 == ButtonState.Pressed &&
                            _currentMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (_lastMouseState.XButton2 == ButtonState.Pressed &&
                            _currentMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }

        /// <summary>
        ///   Checks for a "menu select" input action.
        /// </summary>
        public bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) ||
                   IsNewKeyPress(Keys.Enter) ||
                   IsNewButtonPress(Buttons.A) ||
                   IsNewButtonPress(Buttons.Start) ||
                   IsNewMouseButtonPress(MouseButtons.LeftButton);
        }

        public bool IsMenuPressed()
        {
            return _currentKeyboardState.IsKeyDown(Keys.Space) ||
                   _currentKeyboardState.IsKeyDown(Keys.Enter) ||
                   _currentGamePadState.IsButtonDown(Buttons.A) ||
                   _currentGamePadState.IsButtonDown(Buttons.Start) ||
                   _currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsMenuReleased()
        {
            return IsNewKeyRelease(Keys.Space) ||
                   IsNewKeyRelease(Keys.Enter) ||
                   IsNewButtonRelease(Buttons.A) ||
                   IsNewButtonRelease(Buttons.Start) ||
                   IsNewMouseButtonRelease(MouseButtons.LeftButton);
        }

        /// <summary>
        ///   Checks for a "menu cancel" input action.
        /// </summary>
        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape) ||
                   IsNewButtonPress(Buttons.Back);
        }
    }
} // namespace PanzerAdmiral.Helpers
