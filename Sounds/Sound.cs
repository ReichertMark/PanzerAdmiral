using Microsoft.Xna.Framework.Audio;
using PanzerAdmiral.ScreenSystem;
using Microsoft.Xna.Framework.Input;
using PanzerAdmiral.Helpers;
using System.Threading;

namespace PanzerAdmiral.Sounds
{	
    /// <summary>
    /// XACT Sound Class
    /// </summary>
    class Sound
    {
        #region Enums
        /// <summary>
        /// Sounds we use in this game. This are all the sounds and even the music, only the motor sounds are handled seperately below.
        /// </summary>
        public enum Sounds
        {
            // Menu Sounds

            // Game Sounds
            CannonShot,     // Haubitze 3a roepke
            Motor,          // Panzer 3 ( Loop ) roepke. Has extra Motor Category to limit instances
            Explosion,      // Explo_3_long roepke
            PickUp,
            // Additional Game Sounds (gear sounds are extra)

            // Music
            MenuMusic,      // XNA Racing game music
            GameMusic,      // Charlie and the choclate factory - Main Theme
        } // enum Sounds
        #endregion

        #region Variables
        /// <summary> Sound stuff for XAct </summary>
        static AudioEngine audioEngine;
        /// <summary> Wave bank </summary>
        static WaveBank waveBank;
        /// <summary> Sound bank </summary>
        static SoundBank soundBank;
        #endregion

        #region Constructor
        /// <summary>
        /// Private constructor to prevent instantiation.
        /// </summary>
        private Sound()
        {
        } // Sound()

        /// <summary>
        /// Create sound.
        /// </summary>
        static Sound()
        {
            audioEngine = new AudioEngine("Content/Sounds/PanzerAdmiral.xgs");
            waveBank = new WaveBank(audioEngine, "Content/Sounds/Wave Bank.xwb");

            // Dummy wavebank call to get rid of the warning that waveBank is
            // never used (well it is used, but only inside of XNA).
            if (waveBank != null)
                soundBank = new SoundBank(audioEngine, "Content/Sounds/Sound Bank.xsb");

        } // Sound()
        #endregion

        #region Update
        /// <summary>
        /// Update, just calls audioEngine.Update!
        /// </summary>
        public static void Update()
        {
            if (audioEngine != null)
                audioEngine.Update();

            if (ScreenManager.Input.IsNewKeyPress(Keys.M))
                Play(Sounds.MenuMusic);

            if (ScreenManager.Input.IsNewKeyPress(Keys.N))
                StopMusic(); //Play(Sounds.MenuMusic);
//             if (ScreenManager.Input.IsNewMouseButtonPress(MouseButtons.LeftButton))
//                 Sound.Play(Sounds.CannonShot);

        } // Update()
        #endregion

        #region Play
        /// <summary>
        /// Plays sound cue (string)
        /// </summary>
        /// <param name="soundName">Sound name</param>
        public static void Play(string soundName)
        {
            if (soundBank == null)
                return;
                
            soundBank.PlayCue(soundName);
  
        } // Play(soundName)

        /// <summary>
        /// Play sound cue (Enum)
        /// </summary>
        /// <param name="sound">Sound</param>
        public static void Play(Sounds sound)
        {
            Play(sound.ToString());
        } // Play(sound)
        #endregion

        #region StopMusic
        /// <summary>
        /// Stop music
        /// </summary>
        public static void StopMusic()
        {
            if (soundBank == null)
                return;

            // Use a little trick, start new music, but use the cue. This will
            // replace the old music, then stop the music and everything is stopped!
            Cue musicCue = soundBank.GetCue("MenuMusic");
            musicCue.Play();
            // Wait for a short while to let Xact kick in ^^
            Thread.Sleep(10);
            musicCue.Stop(AudioStopOptions.Immediate);
        } // StopMusic()
        #endregion

        #region StopSound
        /// <summary>
        /// Stops a specific sound, it must have limited instances in XACT in order to work correctly.
        /// </summary>
        public static void StopSound(Sounds setSound)
        {
            if (soundBank == null)
                return;

            // Use a little trick, start new music, but use the cue. This will
            // replace the old music, then stop the music and everything is stopped!
            Cue musicCue = soundBank.GetCue(setSound.ToString());
            musicCue.Play();
            // Wait for a short while to let Xact kick in ^^
            Thread.Sleep(10);
            musicCue.Stop(AudioStopOptions.Immediate);
        } // StopMusic()
        #endregion
    } // class Sound
} // namespace PanzerAdmiral.Sounds
