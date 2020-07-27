using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;
using Microsoft.Xna.Framework.Input;

namespace PanzerAdmiral.Helpers
{
    /// <summary>
    /// Displays the FPS
    /// </summary>
    public class FPSCounter : DrawableGameComponent
    {
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private NumberFormatInfo _format;
        private int _frameCounter;
        private int _frameRate;
        private Vector2 _position;
        private ScreenManager _screenManager;
        private bool _showFPS = true;

        public FPSCounter(ScreenManager screenManager)
            : base(screenManager.Game)
        {
            _screenManager = screenManager;
            _format = new NumberFormatInfo();
            _format.NumberDecimalSeparator = ".";

            _position = new Vector2(30, 25);
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime <= TimeSpan.FromSeconds(1)) return;

            _elapsedTime -= TimeSpan.FromSeconds(1);
            _frameRate = _frameCounter;
            _frameCounter = 0;


            if (ScreenManager.Input.IsNewKeyPress(Keys.F9))
                _showFPS = !_showFPS;

        } // Update(gameTime)

        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            string fps = string.Format(_format, "{0} fps", _frameRate);

            if (_showFPS)
            {
                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.DrawString(_screenManager.Fonts.FrameRateCounterFont, fps, _position + Vector2.One, Color.Black);
                ScreenManager.SpriteBatch.DrawString(_screenManager.Fonts.FrameRateCounterFont, fps, _position, Color.White);
                ScreenManager.SpriteBatch.End();
            }

        } // Draw(gameTime)
    } // class FPSCounter
} // namespace PanzerAdmiral.Helpers
