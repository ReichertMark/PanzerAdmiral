using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;
using Microsoft.Xna.Framework.Input;

namespace PanzerAdmiral.Demos
{
    public class HealthBar
    {

        public static float maxHitpoints = 200;
        // current Hitpoints
        public static float hitPoints = 200;

        public static float currentHealthInPercent = hitPoints / maxHitpoints; 

        Texture2D texture;

        public HealthBar(int setHitPoints)
        {
            hitPoints = setHitPoints;
            maxHitpoints = setHitPoints;

            // Load texture
            texture = ScreenManager.Content.Load<Texture2D>("Common/healthbar");
        }

        public static void SetDamage(float setDamage)
        {
            if (hitPoints > 0)
                hitPoints -= setDamage;
            if (hitPoints - setDamage <= 0)
                hitPoints = 0;
        }

        public static void SetHeal(float setHeal)
        {
            if (hitPoints + setHeal <= maxHitpoints)
                hitPoints += setHeal;
            if (hitPoints + setHeal > maxHitpoints)
                hitPoints = maxHitpoints;
        }


        public void Draw()
        {
            // tst!
            if(ScreenManager.Input.IsNewKeyPress(Keys.D2))
                SetDamage(10.6f);
            if (ScreenManager.Input.IsNewKeyPress(Keys.D3))
                SetHeal(10.2f);

            //calculate the percentage of health left  
            currentHealthInPercent = hitPoints / maxHitpoints;  

            // 44
            int height = texture.Height / 2 ; //20;//44 //texture.Height;
            //Draw the negative space for the health bar
             ScreenManager.SpriteBatch.Draw(texture, new Rectangle(ScreenManager.Device.PresentationParameters.BackBufferWidth / 2 - texture.Width / 2,
                                                                   30, texture.Width, height), new Rectangle(0, 45, texture.Width, height), Color.Gray);

            //Draw the current health level based on the current Health
            ScreenManager.SpriteBatch.Draw(texture, new Rectangle(ScreenManager.Device.PresentationParameters.BackBufferWidth / 2 - texture.Width / 2,
                                                                  30, (int)(texture.Width * currentHealthInPercent), height),
                                                                  new Rectangle(0, height + 1, texture.Width, height), Color.Red);

            //Draw the box around the health bar
             ScreenManager.SpriteBatch.Draw(texture, new Rectangle(ScreenManager.Device.PresentationParameters.BackBufferWidth / 2 - texture.Width / 2,
                                                                   30, texture.Width, height), new Rectangle(0, 0, texture.Width, height), Color.White);
        }

    } // class HealthBar
} // namespace PanzerAdmiral.Demos
