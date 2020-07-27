using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PanzerAdmiral.ScreenSystem;

namespace PanzerAdmiral.AI
{
    public static class Style
    {
        #region Variables
        /// <summary>light Grey Node Color</summary>
        public static Color NodeColor = new Color(200, 200, 200);
        /// <summary> Selection Color for editor</summary>
        public static Color SelectionColor = new Color(144, 238, 144);
        /// <summary></summary>
        public static Color OriginalColor = new Color(80, 80, 80);
        /// <summary>Color of the Lines between Nodes</summary>
        public static Color LineColor = new Color(80, 80, 80);
        /// <summary>Color of the Lines that are close to the target</summary>
        public static Color ClosedColor = new Color(255, 200, 160);
        /// <summary>Path Color</summary>
        public static Color PathColor = new Color(180, 180, 255);
        /// <summary>BrightText Color</summary>
        public static Color BrightText = new Color(255, 255, 255);
        /// <summary>DarkText Color</summary>
        public static Color DarkText = new Color(150, 150, 150);

        /// <summary>Layers for depths</summary>
        public static float LineLayer = 0f;
        public static float ParentLayer = 0.25f;
        public static float ActorLayer = 0.5f;
        public static float MarkerLayer = 0.75f;
        public static float TextLayer = 1f;

        public static Texture2D FillTexture;
        public static Texture2D ArrowTexture;
        public static Texture2D NodeTexture;
        public static Texture2D HealthTexture;
        //public static Texture2D BackgroundTexture;


        public static Texture2D MarkerTexture;
        public static Vector2 MarkerOrigin;

        public static Texture2D TailTexture;
        public static Vector2 TailOrigin;

        public static SpriteFont FontLarge;
        public static SpriteFont FontSmall;
        #endregion

        public static void LoadContent()
        {
            FontLarge = ScreenManager.Content.Load<SpriteFont>("Fonts/font_large");
            FontSmall = ScreenManager.Content.Load<SpriteFont>("Fonts/font_small");

            FillTexture = new Texture2D(ScreenManager.Device, 1, 1);
            FillTexture.SetData<Color>(new Color[] { Color.White });

            ArrowTexture = ScreenManager.Content.Load<Texture2D>("AI/arrow");
            NodeTexture = ScreenManager.Content.Load<Texture2D>("AI/circle16");
            HealthTexture = ScreenManager.Content.Load<Texture2D>("AI/health");
            //BackgroundTexture = new Texture("EditorLevelColor.bmp");

            MarkerTexture = ScreenManager.Content.Load<Texture2D>("AI/ring24");
            MarkerOrigin = new Vector2(MarkerTexture.Width / 2, MarkerTexture.Height / 2);

            TailTexture = ScreenManager.Content.Load<Texture2D>("AI/line22x3");
            TailOrigin = new Vector2(0, TailTexture.Height / 2);
        } // LoadContent

    } // class Style
} // namespace PanzerAdmiral.AI
