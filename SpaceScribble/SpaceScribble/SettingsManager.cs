using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO.IsolatedStorage;
using System.IO;
using SpaceScribble.Inputs;

namespace SpaceScribble
{
    class SettingsManager
    {
        #region Members

        private const string OLD_SETTINGS_FILE = "settings.txt";
        private const string SETTINGS_FILE = "settings2.txt";

        private static SettingsManager settingsManager;

        private static Texture2D texture;
        private static SpriteFont font;
        private readonly Rectangle SettingsTitleSource = new Rectangle(0,1360, //640,
                                                                       240, 80);
        private readonly Vector2 TitlePosition = new Vector2(120.0f, 100.0f);

        public enum SoundValues {Off, VeryLow, Low, Med, High, VeryHigh};
        public enum ToggleValues { On, Off };
        public enum NeutralPositionValues { Angle0, Angle10, Angle20, Angle30, Angle40, Angle50, Angle60, Unsupported };
        public enum ControlPositionValues { Left, Right };

        private const string MUSIC_TITLE = "Music: ";
        private SoundValues musicValue = SoundValues.Med;
        private readonly int musicPositionY = 260;
        private readonly Rectangle musicDestination = new Rectangle(90, 245,
                                                                    300, 50);

        private const string SFX_TITLE = "SFX: ";
        private SoundValues sfxValue = SoundValues.High;
        private readonly int sfxPositionY = 340;
        private readonly Rectangle sfxDestination = new Rectangle(90, 325,
                                                                  300, 50);

        private const string VIBRATION_TITLE = "Vibration: ";
        private ToggleValues vibrationValue = ToggleValues.On;
        private readonly int vibrationPositionY = 420;
        private readonly Rectangle vibrationDestination = new Rectangle(90, 405,
                                                                        300, 50);

        private NeutralPositionValues neutralPositionValue = NeutralPositionValues.Angle20;

        private const string SENSOR_SETTINGS_TITLE = "Sensor control settings: ";
        private readonly int sensorTitlePositionY = 525;

        private const string AUTOFIRE_TITLE = "Autofire: ";
        private ToggleValues autofireValue = ToggleValues.On;
        private readonly int autoforePositionY = 600;
        private readonly Rectangle autofireDestination = new Rectangle(90, 585,
                                                                       300, 50);

        private const string CONTROL_POSITION_TITLE = "Control Position: ";
        private ControlPositionValues controlPositionValue = ControlPositionValues.Right;
        private readonly int controlPositionY = 680;
        private readonly Rectangle controlPositionDestination = new Rectangle(90, 665,
                                                                              300, 50);

        private static Rectangle screenBounds;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        public static GameInput GameInput;
        private const string MusicAction = "MusicSetting";
        private const string SfxAction = "SFXSetting";
        private const string VibrationAction = "VibrationSetting";
        private const string NeutralPositionAction = "NeutralPosSetting";
        private const string ControlPositionAction = "ControlPosSetting";
        private const string AutofireAction = "AutofireSetting";

        private const string ON = "ON";
        private const string OFF = "OFF";
        private const string VERY_LOW = "VERY LOW";
        private const string LOW = "LOW";
        private const string MEDIUM = "MEDIUM";
        private const string HIGH = "HIGH";
        private const string VERY_HIGH = "VERY HIGH";
        private const string LEFT = "LEFT";
        private const string RIGHT = "RIGHT";

        private const int TextPositonX = 80;
        private const int ValuePositionX = 400;

        private bool isInvalidated = false;

        #endregion

        #region Constructors

        private SettingsManager()
        {
            this.Load();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(MusicAction,
                                           GestureType.Tap,
                                           musicDestination);
            GameInput.AddTouchGestureInput(SfxAction,
                                           GestureType.Tap,
                                           sfxDestination);
            GameInput.AddTouchGestureInput(VibrationAction,
                                           GestureType.Tap,
                                           vibrationDestination);
            GameInput.AddTouchGestureInput(ControlPositionAction,
                                           GestureType.Tap,
                                           controlPositionDestination);
            GameInput.AddTouchGestureInput(AutofireAction,
                                           GestureType.Tap,
                                           autofireDestination);
        }

        public void Initialize(Texture2D tex, SpriteFont f, Rectangle screen)
        {
            texture = tex;
            font = f;
            screenBounds = screen;
        }

        public static SettingsManager GetInstance()
        {
            if (settingsManager == null)
            {
                settingsManager = new SettingsManager();
            }

            return settingsManager;
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             TitlePosition,
                             SettingsTitleSource,
                             Color.White * opacity);

            drawMusic(spriteBatch);
            drawSfx(spriteBatch);
            drawVibration(spriteBatch);

            // sensor settings title:
            spriteBatch.DrawString(font,
                                   SENSOR_SETTINGS_TITLE,
                                   new Vector2((240 - font.MeasureString(SENSOR_SETTINGS_TITLE).X / 2),
                                               sensorTitlePositionY),
                                   Color.Black * opacity);

            drawControlPosition(spriteBatch);
            drawAutofire(spriteBatch);
        }

        private void handleTouchInputs()
        {
            // Vibration
            if (GameInput.IsPressed(VibrationAction))
            {
                toggleVibration();
                SoundManager.PlayPaperSound();
            }
            // Music
            else if (GameInput.IsPressed(MusicAction))
            {
                toggleMusic();
                SoundManager.PlayPaperSound();
            }
            // Sfx
            else if (GameInput.IsPressed(SfxAction))
            {
                toggleSfx();
                SoundManager.PlayPaperSound();
            }
            // ControlPosition
            else if (GameInput.IsPressed(ControlPositionAction))
            {
                toggleControlPosition();
                SoundManager.PlayPaperSound();
            }
            // Autofire
            else if (GameInput.IsPressed(AutofireAction))
            {
                toggleAutofire();
                SoundManager.PlayPaperSound();
            }
        }

        private void toggleMusic()
        {
            switch (musicValue)
            {
                case SoundValues.Off:
                    musicValue = SoundValues.VeryLow;
                    break;
                case SoundValues.VeryLow:
                    musicValue = SoundValues.Low;
                    break;
                case SoundValues.Low:
                    musicValue = SoundValues.Med;
                    break;
                case SoundValues.Med:
                    musicValue = SoundValues.High;
                    break;
                case SoundValues.High:
                    musicValue = SoundValues.VeryHigh;
                    break;
                case SoundValues.VeryHigh:
                    musicValue = SoundValues.Off;
                    break;
            }
            isInvalidated = true;
            SoundManager.RefreshMusicVolume();
        }

        private void toggleSfx()
        {
            switch (sfxValue)
            {
                case SoundValues.Off:
                    sfxValue = SoundValues.VeryLow;
                    break;
                case SoundValues.VeryLow:
                    sfxValue = SoundValues.Low;
                    break;
                case SoundValues.Low:
                    sfxValue = SoundValues.Med;
                    break;
                case SoundValues.Med:
                    sfxValue = SoundValues.High;
                    break;
                case SoundValues.High:
                    sfxValue = SoundValues.VeryHigh;
                    break;
                case SoundValues.VeryHigh:
                    sfxValue = SoundValues.Off;
                    break;
            }
            isInvalidated = true;
            if (sfxValue != SoundValues.Off)
                SoundManager.PlayPlayerShot();
        }

        private void toggleVibration()
        {
            switch (vibrationValue)
            {
                case ToggleValues.Off:
                    vibrationValue = ToggleValues.On;
                    break;
                case ToggleValues.On:
                    vibrationValue = ToggleValues.Off;
                    break;
            }
            isInvalidated = true;
            if (vibrationValue == ToggleValues.On)
                VibrationManager.Vibrate(0.2f);
        }

        public void SetNeutralPosition(NeutralPositionValues value)
        {
            this.neutralPositionValue = value;
        }

        private void toggleControlPosition()
        {
            if (autofireValue == ToggleValues.On)
                return;

            switch (controlPositionValue)
            {
                case ControlPositionValues.Left:
                    controlPositionValue = ControlPositionValues.Right;
                    break;
                case ControlPositionValues.Right:
                    controlPositionValue = ControlPositionValues.Left;
                    break;
                default:
                    break;
            }
            isInvalidated = true;
        }

        private void toggleAutofire()
        {
            switch (autofireValue)
            {
                case ToggleValues.Off:
                    autofireValue = ToggleValues.On;
                    break;
                case ToggleValues.On:
                    autofireValue = ToggleValues.Off;
                    break;
            }
            isInvalidated = true;
        }

        private void drawMusic(SpriteBatch spriteBatch)
        {
            string text;

            switch (musicValue)
            {
                case SoundValues.VeryLow:
                    text = VERY_LOW;
                    break;
                case SoundValues.Low:
                    text = LOW;
                    break;
                case SoundValues.Med:
                    text = MEDIUM;
                    break;
                case SoundValues.High:
                    text = HIGH;
                    break;
                case SoundValues.VeryHigh:
                    text = VERY_HIGH;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   MUSIC_TITLE,
                                   new Vector2(TextPositonX,
                                               musicPositionY),
                                   Color.Black * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
                                               musicPositionY),
                                   Color.Black * opacity);
        }

        private void drawSfx(SpriteBatch spriteBatch)
        {
            string text;

            switch (sfxValue)
            {
                case SoundValues.VeryLow:
                    text = VERY_LOW;
                    break;
                case SoundValues.Low:
                    text = LOW;
                    break;
                case SoundValues.Med:
                    text = MEDIUM;
                    break;
                case SoundValues.High:
                    text = HIGH;
                    break;
                case SoundValues.VeryHigh:
                    text = VERY_HIGH;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   SFX_TITLE,
                                   new Vector2(TextPositonX,
                                               sfxPositionY),
                                   Color.Black * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
                                               sfxPositionY),
                                   Color.Black * opacity);
        }

        private void drawVibration(SpriteBatch spriteBatch)
        {
            string text;

            switch (vibrationValue)
            {
                case ToggleValues.On:
                    text = ON;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   VIBRATION_TITLE,
                                   new Vector2(TextPositonX,
                                               vibrationPositionY),
                                   Color.Black * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
                                               vibrationPositionY),
                                   Color.Black * opacity);
        }

        private void drawControlPosition(SpriteBatch spriteBatch)
        {
            string text;
            Color c;

            switch (controlPositionValue)
            {
                case ControlPositionValues.Left:
                    text = LEFT;
                    break;
                default:
                    text = RIGHT;
                    break;
            }

            if (autofireValue == ToggleValues.Off)
                c = Color.Black * opacity;
            else
                c = Color.Black * opacity * 0.45f;


            spriteBatch.DrawString(font,
                                       CONTROL_POSITION_TITLE,
                                       new Vector2(TextPositonX,
                                                   controlPositionY),
                                       c);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
                                               controlPositionY),
                                   c);
        }

        private void drawAutofire(SpriteBatch spriteBatch)
        {
            string text;

            switch (autofireValue)
            {
                case ToggleValues.On:
                    text = ON;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   AUTOFIRE_TITLE,
                                   new Vector2(TextPositonX,
                                               autoforePositionY),
                                   Color.Black * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
                                               autoforePositionY),
                                   Color.Black * opacity);
        }

        #endregion

        #region Load/Save

        public void Save()
        {
            if (!isInvalidated)
                return;

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(SETTINGS_FILE, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(this.musicValue);
                        sw.WriteLine(this.sfxValue);
                        sw.WriteLine(this.vibrationValue);
                        sw.WriteLine(this.neutralPositionValue);
                        sw.WriteLine(this.controlPositionValue);
                        sw.WriteLine(this.autofireValue);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        public void Load()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(SETTINGS_FILE);

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(SETTINGS_FILE, FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            isInvalidated = false;
                            this.musicValue = (SoundValues)Enum.Parse(musicValue.GetType(), sr.ReadLine(), true);
                            this.sfxValue = (SoundValues)Enum.Parse(sfxValue.GetType(), sr.ReadLine(), true);
                            this.vibrationValue = (ToggleValues)Enum.Parse(vibrationValue.GetType(), sr.ReadLine(), true);
                            this.neutralPositionValue = (NeutralPositionValues)Enum.Parse(neutralPositionValue.GetType(), sr.ReadLine(), true);
                            this.controlPositionValue = (ControlPositionValues)Enum.Parse(controlPositionValue.GetType(), sr.ReadLine(), true);
                            this.autofireValue = (ToggleValues)Enum.Parse(autofireValue.GetType(), sr.ReadLine(), true);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(this.musicValue);
                            sw.WriteLine(this.sfxValue);
                            sw.WriteLine(this.vibrationValue);
                            sw.WriteLine(this.neutralPositionValue);
                            sw.WriteLine(this.controlPositionValue);
                            sw.WriteLine(this.autofireValue);

                            // ... ? 
                        }
                    }
                }

                // Delete the old file
                if (isf.FileExists(OLD_SETTINGS_FILE))
                    isf.DeleteFile(OLD_SETTINGS_FILE);
            }
        }

        public float GetMusicValue()
        {
            switch (settingsManager.musicValue)
            {
                case SoundValues.Off:
                    return 0.0f;

                case SoundValues.VeryLow:
                    return 0.1f;

                case SoundValues.Low:
                    return 0.2f;

                case SoundValues.Med:
                    return 0.3f;

                case SoundValues.High:
                    return 0.4f;

                case SoundValues.VeryHigh:
                    return 0.5f;

                default:
                    return 0.3f;
            }
        }

        public float GetSfxValue()
        {
            switch (settingsManager.sfxValue)
            {
                case SoundValues.Off:
                    return 0.0f;

                case SoundValues.VeryLow:
                    return 0.2f;

                case SoundValues.Low:
                    return 0.4f;

                case SoundValues.Med:
                    return 0.6f;

                case SoundValues.High:
                    return 0.8f;

                case SoundValues.VeryHigh:
                    return 1.0f;

                default:
                    return 0.6f;
            }
        }

        public bool GetVabrationValue()
        {
            switch (settingsManager.vibrationValue)
            {
                case ToggleValues.On:
                    return true;

                case ToggleValues.Off:
                    return false;

                default:
                    return true;
            }
        }

        public float GetNeutralPosition()
        {
            return GetNeutralPositionValue(settingsManager.neutralPositionValue);
        }

        private float GetNeutralPositionValue(NeutralPositionValues value)
        {
            switch (value)
            {
                case NeutralPositionValues.Angle0:
                    return 0.0f;

                case NeutralPositionValues.Angle10:
                    return (float)Math.PI * 10.0f / 180.0f;

                case NeutralPositionValues.Angle20:
                    return (float)Math.PI * 20.0f / 180.0f;

                case NeutralPositionValues.Angle30:
                    return (float)Math.PI * 30.0f / 180.0f;

                case NeutralPositionValues.Angle40:
                    return (float)Math.PI * 40.0f / 180.0f;

                case NeutralPositionValues.Angle50:
                    return (float)Math.PI * 50.0f / 180.0f;

                case NeutralPositionValues.Angle60:
                    return (float)Math.PI * 60.0f / 180.0f;

                default:
                    return 0.0f;
            }
        }

        public float GetNeutralPositionRadianValue(float angle)
        {
            return (float)Math.PI * angle / 180.0f;
        }

        public int GetNeutralPositionIndex()
        {
            switch (settingsManager.neutralPositionValue)
            {
                case NeutralPositionValues.Angle0:
                    return 0;

                case NeutralPositionValues.Angle10:
                    return 1;

                case NeutralPositionValues.Angle20:
                    return 2;

                case NeutralPositionValues.Angle30:
                    return 3;

                case NeutralPositionValues.Angle40:
                    return 4;

                case NeutralPositionValues.Angle50:
                    return 5;

                case NeutralPositionValues.Angle60:
                    return 6;

                default:
                    return -1;
            }
        }

        public bool GetAutofireValue()
        {
            switch (settingsManager.autofireValue)
            {
                case ToggleValues.On:
                    return true;

                case ToggleValues.Off:
                    return false;

                default:
                    return true;
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            opacity = Single.Parse(reader.ReadLine());
            isInvalidated = Boolean.Parse(reader.ReadLine());
            neutralPositionValue = (NeutralPositionValues)Enum.Parse(neutralPositionValue.GetType(), reader.ReadLine(), false);

        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(opacity);
            writer.WriteLine(isInvalidated);
            writer.WriteLine(neutralPositionValue);
        }

        #endregion

        #region Properties

        public ControlPositionValues ControlPosition
        {
            get
            {
                return controlPositionValue;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;

                if (isActive == false)
                {
                    this.opacity = OpacityMin;
                    Save();
                }
            }
        }

        #endregion
    }
}
