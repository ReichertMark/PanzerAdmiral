using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PanzerAdmiral.Demos;
using PanzerAdmiral.ScreenSystem;
using PanzerAdmiral.AI;
using PanzerAdmiral.Helpers;

namespace PanzerAdmiral.Graphics
{
    class FadeUp
    {
        /// <summary>
        /// Text types
        /// </summary>
        public enum TextTypes
        {
            Heal,
            Score,
            GotWeapon,
            GotKey,
            Damage,
            Died,
        } // enum TextTypes

        /// <summary>
        /// Colors for each text type
        /// </summary>
        static readonly Color[] TextTypeColors = new Color[]
			{
				Color.Green, //new Color(0, 200, 200),//Color.White,
				Color.DarkRed, //darkRed, //new Color(67, 21, 9), //Color.Yellow,
				Color.Orange,
				Color.LightBlue,
				Color.Red,
				Color.Red,
			};

        class FadeupText
        {
            public string text;
            public Color color;
            public float showTimeMs;
            public const float MaxShowTimeMs = 2000; //750;//3000;
            public Vector2 FontPos;

            public FadeupText(string setText, Color setColor, Vector2 setFontPos)
            {
                FontPos = setFontPos;
                text = setText;
                color = setColor;
                showTimeMs = MaxShowTimeMs;
            } // TimeFadeupText(setText, setShowTimeMs, setFontPos)
        } // TimeFadeupText
        static List<FadeupText> fadeupTexts = new List<FadeupText>();

        /// <summary>
        /// Add text message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="type">Type</param>
        /// /// <param name="position">Text Position</param>
        public static void AddTextMessage(string message, TextTypes type, Vector2 position)
        {
            // Check number of texts that were added in the last 400 ms!
            int numOfRecentTexts = 0;
            for (int num = 0; num < fadeupTexts.Count; num++)
            {
                FadeupText fadeupText = fadeupTexts[num];
                if (fadeupText.showTimeMs > FadeupText.MaxShowTimeMs - 400)
                    numOfRecentTexts++;
            } // for (num)

            fadeupTexts.Add(new FadeupText(message, TextTypeColors[(int)type], ConvertUnits.ToDisplayUnits(position)));
            // Add offset to this text to be displayed below the already existing
            // texts! This fixes the overlapping texts!
            fadeupTexts[fadeupTexts.Count - 1].showTimeMs += numOfRecentTexts * 400;
        } // AddTextMessage(message, type)





        public void Draw()
        {
            for (int num = 0; num < fadeupTexts.Count; num++)
            {
                FadeupText fadeupText = fadeupTexts[num];
                fadeupText.showTimeMs -= (float)GameDemo1.GameTime.ElapsedGameTime.Milliseconds; //TotalMilliseconds.ElapsedTimeThisFrameInMs;
                if (fadeupText.showTimeMs < 0)
                {
                    fadeupTexts.Remove(fadeupText);
                    num--;
                } // if
                else
                {
                    // Fade out
                    float alpha = 1.0f;
                    if (fadeupText.showTimeMs < 1500) //400)
                        alpha = fadeupText.showTimeMs / 1500.0f;// 400; 
                    // Move up
                    float moveUpAmount = (FadeupText.MaxShowTimeMs - fadeupText.showTimeMs) /
                                          FadeupText.MaxShowTimeMs;


                    ScreenManager.SpriteBatch.DrawString(Style.FontLarge, fadeupText.text,
                                                        new Vector2(fadeupText.FontPos.X, fadeupText.FontPos.Y + moveUpAmount * 3), 
                                                        new Color(fadeupText.color.R, fadeupText.color.G, fadeupText.color.B, alpha));
                } // else
            }

        }



    } // class FadeUp
} // namespace PanzerAdmiral.Graphics
