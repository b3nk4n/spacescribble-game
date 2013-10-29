using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using System.IO;
using SpaceScribble.Inputs;
using Microsoft.Phone.Applications.Common;

namespace SpaceScribble
{
    class PlayerManager : ILevel
    {
        #region Members

        public Sprite playerSprite;
        private Rectangle playerAreaLimit;

        private Vector2 startLocation;

        private long playerScore = 0;

        // Init values
        private float initPlayerSpeed;
        private float initShotSpeed;
        private int initSpecialShots;
        private float initShotPower;
        private float initMaxHitPoints;
        private float initMaxShieldPoints;
        private float initMinSpecialShotTimer;
        private float initMinSpecialShotReloadTimer;
        private float initInaccuracy;

        private const int PLAYER_STARTING_LIVES = 0;
        private int livesRemaining = PLAYER_STARTING_LIVES;
        public int SpecialShotsRemaining;
        private const int MaxSpecialShots = 99;

        public const int CARLI_ROCKET_EXPLOSION_RADIUS = 150;
        
        public const float ROCKET_POWER_AT_CENTER = 300.0f;

        private float hitPoints = 100.0f;

        private Vector2 gunOffset = new Vector2(16, 5);
        private float shotTimer = 0.0f;
        private float specialShotTimer = 0.0f;
        private float minShotTimer = 0.18f;

        private float specialShotReloadTimer = 0.0f;
        
        private const int PlayerRadius = 18;
        public ShotManager PlayerShotManager;

        Vector3 currentAccValue = Vector3.Zero;

        Rectangle leftSideScreen;
        Rectangle rightSideScreen;
        Rectangle upperScreen;

        // Shield
        private float shieldPoints = 0.0f;

        SettingsManager settings = SettingsManager.GetInstance();

        GameInput gameInput;
        private const string ActionLeft = "Left";
        private const string ActionRight = "Right";
        private const string ActionUpper = "UpperLeft";
        private static readonly string[] ActionUpgradeTouch = {"Up0",
                                                               "Up1",
                                                               "Up2",
                                                               "Up3",
                                                               "Up4",
                                                               "Up5",
                                                               "Up6",
                                                               "Up7",
                                                               "Up8",
                                                               "Up9"};
        private const string ActionMove = "MoveWithFinger";
        private const string ActionUpperMove = "UpperMove";
        private readonly Rectangle UpperMoveRectSecondTouch = new Rectangle(0, 0, 480, 750);

        private Vector2 targetPlayerPosition;
        private readonly Vector2 PLAYER_POSITION_OFFSET = new Vector2(-25, -100);

        // Player ship type/texture
        public enum PlayerType { GreenHornet, Medium, Hard, Tank, Speeder, Easy };
        private PlayerType shipType = PlayerType.Easy;
        Texture2D tex;

        private long credits;

        // Upgrade levels
        /* upgrade is not possible if the boss is active */
        private bool canUpgrade;

        private int upgrades;

        private static int laserPowerUpgrades;
        private static int laserFrequencyUpgrades;
        private static int laserModeUpgrades;
        private static int laserSpeedUpgrades;
        private static int laserAccuracyUpgrades;
        private static int agilityUpgrades;
        private static int specialReloadUpgrades;

        // Upgrade factors/steps
        private const float SHOTSPEED_PER_UPGRADE = 25.0f;
        private const float FREQUENCY_PER_UPGRADE = 0.0075f;
        private const float SHOTPOWER_PER_UPGRADE = 5.0f;
        private const float AGILITY_PER_UPGRADE = 15.0f;

        Random rand = new Random();

        public const int UPGRADES_COUNT = 10;
        public const int MAX_UPGRADE_LEVEL = 9; // Note: 0 = Level 1 = default

        // Crash constants
        public const float CRASH_TO_ENEMY = 20.0f;

        private long shotCounter;

        private const int PADDING_BOTTOM = 30;

        public const float SONIC_SHOT_POWER = 5.0f;

        private float autofireStartTimer;
        private const float autofireStartMin = 2.0f;

        // Scales opacity and speed at startup (0 -> 1)
        private float startUpScale;

        /**
         * Defines whether sensor or touch input is active. 
         * This setting is not stored in isolated storage!!!
         */
        public static bool IsSensorInput = false;

        #endregion

        #region Constructors

        public PlayerManager(Texture2D texture, Rectangle initialFrame,
                             int frameCount, Rectangle screenBounds,
                             Vector2 startLocation, GameInput input)
        {
            tex = texture;

            this.playerSprite = new Sprite(new Vector2(500, 500),
                                           texture,
                                           initialFrame,
                                           Vector2.Zero);

            SelectPlayerType(PlayerType.Easy);
            
            this.PlayerShotManager = new ShotManager(texture,
                                                     new Rectangle(650, 160, 20, 20),
                                                     4,
                                                     2,
                                                     initShotSpeed,
                                                     screenBounds);

            this.playerAreaLimit = new Rectangle(0,
                                                 screenBounds.Height / 9,
                                                 screenBounds.Width,
                                                 8 * screenBounds.Height / 9);

            playerSprite.CollisionRadius = PlayerRadius;

            AccelerometerHelper.Instance.ReadingChanged += new EventHandler<AccelerometerHelperReadingEventArgs>(OnAccelerometerHelperReadingChanged);
            AccelerometerHelper.Instance.Active = true;

            leftSideScreen = new Rectangle(0,
                                           screenBounds.Height / 2,
                                           screenBounds.Width / 2,
                                           screenBounds.Height / 2 - 75);

            rightSideScreen = new Rectangle(screenBounds.Width / 2,
                                           screenBounds.Height / 2,
                                           screenBounds.Width / 2,
                                           screenBounds.Height / 2 - 75);

            upperScreen = new Rectangle(0, 0,
                                            screenBounds.Width,
                                            screenBounds.Height / 2);

            this.startLocation = startLocation;

            gameInput = input;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            gameInput.AddTouchTapInput(ActionLeft,
                                       leftSideScreen,
                                       false);
            gameInput.AddTouchTapInput(ActionRight,
                                       rightSideScreen,
                                       false);

            // update icons
            for (int i = 0; i < PlayerManager.UPGRADES_COUNT; ++i)
            {
                Rectangle dest;
   
                // large
                dest = new Rectangle((int)Hud.UpgradesLocation.X - Hud.UPGRADE_LARGE_DIMENSION / 2 + (i * Hud.UPGRADES_OFFSET_X),
                    (int)Hud.UpgradesLocation.Y - ((Hud.UPGRADE_LARGE_DIMENSION - Hud.UPGRADE_SMALL_DIMENSION)),
                    Hud.UPGRADE_LARGE_DIMENSION,
                    Hud.UPGRADE_LARGE_DIMENSION);

                /*gameInput.AddTouchTapInput(ActionUpgradeTouch[i],
                                       dest,
                                       false);*/
                gameInput.AddTouchGestureInput(ActionUpgradeTouch[i],
                    GestureType.Tap,
                    dest);
            }

            gameInput.AddTouchTapInput(ActionMove,
                new Rectangle(0, 50, 480, 725),
                false);
        }

        public void Reset()
        {
            this.PlayerShotManager.Shots.Clear();
            this.PlayerShotManager.Rockets.Clear();
            this.PlayerShotManager.Sonic.Clear();

            this.playerSprite.Location = startLocation;

            this.hitPoints = initMaxHitPoints;
            this.shieldPoints = 0.0f;

            this.specialShotReloadTimer = initMinSpecialShotReloadTimer;

            this.credits = 0;

            this.upgrades = 0;

            this.shotCounter = 0;

            this.autofireStartTimer = 0.0f;

            this.startUpScale = 0;

            targetPlayerPosition = Vector2.Zero;
            this.playerSprite.Velocity = Vector2.Zero;
        }

        public void resetUpgradeLevels()
        {
            laserPowerUpgrades = 0;
            laserFrequencyUpgrades = 0;
            laserModeUpgrades = 0;
            laserSpeedUpgrades = 0;
            laserAccuracyUpgrades = 0;
            agilityUpgrades = 0;
            specialReloadUpgrades = 0;
        }

        public void SelectPlayerType(PlayerType type)
        {
            this.shipType = type;

            switch (type)
            {
                case PlayerType.GreenHornet:
                    playerSprite = new Sprite(new Vector2(500, 500),
                                           tex,
                                           new Rectangle(0, 100, 50, 50),
                                           Vector2.Zero);

                    for (int x = 1; x < 6; x++)
                    {
                        this.playerSprite.AddFrame(new Rectangle(0 + (x * 50),
                                                                 100,
                                                                 50,
                                                                 50));
                    }
                    break;

                case PlayerType.Easy:
                    playerSprite = new Sprite(new Vector2(500, 500),
                                           tex,
                                           new Rectangle(0, 150, 50, 50),
                                           Vector2.Zero);

                    for (int x = 1; x < 6; x++)
                    {
                        this.playerSprite.AddFrame(new Rectangle(0 + (x * 50),
                                                                 150,
                                                                 50,
                                                                 50));
                    }
                    break;

                case PlayerType.Medium:
                    playerSprite = new Sprite(new Vector2(500, 500),
                                           tex,
                                           new Rectangle(0, 200, 50, 50),
                                           Vector2.Zero);

                    for (int x = 1; x < 6; x++)
                    {
                        this.playerSprite.AddFrame(new Rectangle(0 + (x * 50),
                                                                 200,
                                                                 50,
                                                                 50));
                    }
                    break;

                case PlayerType.Hard:
                    playerSprite = new Sprite(new Vector2(500, 500),
                                           tex,
                                           new Rectangle(350, 150, 50, 50),
                                           Vector2.Zero);

                    for (int x = 1; x < 6; x++)
                    {
                        this.playerSprite.AddFrame(new Rectangle(350 + (x * 50),
                                                                 150,
                                                                 50,
                                                                 50));
                    }
                    break;

                case PlayerType.Speeder:
                    playerSprite = new Sprite(new Vector2(500, 500),
                                           tex,
                                           new Rectangle(350, 200, 50, 50),
                                           Vector2.Zero);

                    for (int x = 1; x < 6; x++)
                    {
                        this.playerSprite.AddFrame(new Rectangle(350 + (x * 50),
                                                                 200,
                                                                 50,
                                                                 50));
                    }
                    break;

                case PlayerType.Tank:
                    playerSprite = new Sprite(new Vector2(500, 500),
                                           tex,
                                           new Rectangle(350, 100, 50, 50),
                                           Vector2.Zero);

                    for (int x = 1; x < 6; x++)
                    {
                        this.playerSprite.AddFrame(new Rectangle(350 + (x * 50),
                                                                 100,
                                                                 50,
                                                                 50));
                    }
                    break;
                default:
                    break;
            }

            playerSprite.Location = startLocation;
            playerSprite.CollisionRadius = PlayerRadius;
            playerSprite.Rotation = MathHelper.PiOver2 * 3;

            initPlayer(type);
        }

        private void initPlayer(PlayerType type)
        {
            switch (type)
            {
                case PlayerType.Easy:
                    initPlayerSpeed = 225.0f;
                    initShotSpeed = 325.0f;
                    initShotPower = 35.0f;
                    initMaxHitPoints = 250.0f;
                    initMaxShieldPoints = 200.0f;
                    initSpecialShots = 10;
                    initMinSpecialShotTimer = 0.5f;
                    initMinSpecialShotReloadTimer = 3.5f;
                    initInaccuracy = 10.0f;
                    break;

                case PlayerType.GreenHornet:
                    initPlayerSpeed = 200.0f;
                    initShotSpeed = 275.0f;
                    initShotPower = 42.5f;
                    initMaxHitPoints = 300.0f;
                    initMaxShieldPoints = 200.0f;
                    initSpecialShots = 15;
                    initMinSpecialShotTimer = 0.25f;
                    initMinSpecialShotReloadTimer = 2.5f;
                    initInaccuracy = 8.0f;
                    break;

                case PlayerType.Medium:
                    initPlayerSpeed = 175.0f;
                    initShotSpeed = 275.0f;
                    initShotPower = 37.5f;
                    initMaxHitPoints = 275.0f;
                    initMaxShieldPoints = 200.0f;
                    initSpecialShots = 5;
                    initMinSpecialShotTimer = 0.75f;
                    initMinSpecialShotReloadTimer = 7.0f;
                    initInaccuracy = 9.0f;
                    break;

                case PlayerType.Hard:
                    initPlayerSpeed = 150.0f;
                    initShotSpeed = 300.0f;
                    initShotPower = 40.0f;
                    initMaxHitPoints = 325.0f;
                    initMaxShieldPoints = 200.0f;
                    initSpecialShots = 5;
                    initMinSpecialShotTimer = 1.0f;
                    initMinSpecialShotReloadTimer = 8.0f;
                    initInaccuracy = 7.0f;
                    break;
                
                case PlayerType.Tank:
                    initPlayerSpeed = 150.0f;
                    initShotSpeed = 300.0f;
                    initShotPower = 45.0f;
                    initMaxHitPoints = 350.0f;
                    initMaxShieldPoints = 200.0f;
                    initSpecialShots = 5;
                    initMinSpecialShotTimer = 1.5f;
                    initMinSpecialShotReloadTimer = 9.0f;
                    initInaccuracy = 9.0f;
                    break;
                
                case PlayerType.Speeder:
                    initPlayerSpeed = 250.0f;
                    initShotSpeed = 350.0f;
                    initShotPower = 37.5f;
                    initMaxHitPoints = 275.0f;
                    initMaxShieldPoints = 200.0f;
                    initSpecialShots = 15;
                    initMinSpecialShotTimer = 0.35f;
                    initMinSpecialShotReloadTimer = 3.0f;
                    initInaccuracy = 10.0f;
                    break;
            }
        }

        public void ResetSpecialWeapons()
        {
            this.SpecialShotsRemaining = initSpecialShots;
        }

        public void ResetRemainingLives()
        {
            this.livesRemaining = PLAYER_STARTING_LIVES;
        }

        private void fireShot()
        {
            if (shotTimer <= 0.0f)
            {
                float factor = initInaccuracy - (laserAccuracyUpgrades / (float)MAX_UPGRADE_LEVEL) * initInaccuracy;

                float rnd1 = (float)rand.NextDouble() * 2 * factor - factor;
                float rnd2 = (float)rand.NextDouble() * 2 * factor - factor;
                float rnd3 = (float)rand.NextDouble() * 2 * factor - factor;

                ++shotCounter;

                switch (laserModeUpgrades)
                {
                    case 0:
                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(90.0f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(90.0f + rnd1))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);
                        break;

                    case 1:
                        if (shotCounter % 2 == 0)
                        {
                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(90.0f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(90.0f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        true);
                        }
                        else
                        {
                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(85f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(85f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        true);

                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(95f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(95f + rnd2))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);
                        }
                        break;

                    case 2:
                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset + new Vector2(13, 0),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(89.0f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(89.0f + rnd1))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);
                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, 0),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(91.0f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(91.0f + rnd2))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    false);
                        break;

                    case 3:
                        if (shotCounter % 2 == 0)
                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(90.0f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(90.0f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);

                        if (shotCounter % 2 == 0)
                        {
                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-3, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(85f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(85f + rnd2))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        true);

                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(75f + rnd3)), -(float)Math.Sin(MathHelper.ToRadians(75f + rnd3))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);
                        }
                        else
                        {
                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(3, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(95f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(95f + rnd2))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        true);

                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(105f + rnd3)), -(float)Math.Sin(MathHelper.ToRadians(105f + rnd3))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);
                        }
                        break;

                    case 4:
                        if (shotCounter % 2 == 0)
                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(90.0f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(90.0f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-13, 0),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(85f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(85f + rnd2))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, 0),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(95f + rnd3)), -(float)Math.Sin(MathHelper.ToRadians(95f + rnd3))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    false);
                        break;

                    case 5:
                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(90.0f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(90.0f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        true);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(85f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(85f + rnd2))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(95f + rnd3)), -(float)Math.Sin(MathHelper.ToRadians(95f + rnd3))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);
                        break;

                    case 6:
                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(90.0f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(90.0f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        true);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-13, -8),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(89f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(89f + rnd2))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, -8),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(91f + rnd3)), -(float)Math.Sin(MathHelper.ToRadians(91f + rnd3))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);
                        break;

                    case 7:
                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-3, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(89.5f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(89.5f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        true);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(3, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(90.5f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(90.5f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);
                        if (shotCounter % 2 == 0)
                        {
                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-13, -8),
                                                            new Vector2((float)Math.Cos(MathHelper.ToRadians(88f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(88f + rnd2))),
                                                            true,
                                                            new Color(0, 255, 33),
                                                            false);

                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, -8),
                                                            new Vector2((float)Math.Cos(MathHelper.ToRadians(92f + rnd3)), -(float)Math.Sin(MathHelper.ToRadians(92f + rnd3))),
                                                            true,
                                                            new Color(0, 255, 33),
                                                            false);
                        }
                        break;

                    case 8:
                        if (shotCounter % 4 == 0 || shotCounter % 3 == 0)
                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-3, 0),
                                                            new Vector2((float)Math.Cos(MathHelper.ToRadians(89.5f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(89.5f + rnd1))),
                                                            true,
                                                            new Color(0, 255, 33),
                                                            false);
                        if (shotCounter % 4 == 1 || shotCounter % 3 == 1)
                            this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(3, 0),
                                                            new Vector2((float)Math.Cos(MathHelper.ToRadians(90.5f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(90.5f + rnd1))),
                                                            true,
                                                            new Color(0, 255, 33),
                                                            false);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-13, -8),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(88f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(88f + rnd2))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        true);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, -8),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(92f + rnd3)), -(float)Math.Sin(MathHelper.ToRadians(92f + rnd3))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);

                        break;

                    default:
                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-3, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(89.5f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(89.5f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        true);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(3, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(90.5f + rnd1)), -(float)Math.Sin(MathHelper.ToRadians(90.5f + rnd1))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-13, -8),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(88f + rnd2)), -(float)Math.Sin(MathHelper.ToRadians(88f + rnd2))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);

                        this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, -8),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(92f + rnd3)), -(float)Math.Sin(MathHelper.ToRadians(92f + rnd3))),
                                                        true,
                                                        new Color(0, 255, 33),
                                                        false);

                        break;
                }

                shotTimer = minShotTimer - laserFrequencyUpgrades * FREQUENCY_PER_UPGRADE;
            }
        }

        private void fireMediumSpecial()
        {
            if (specialShotTimer <= 0.0f &&
                SpecialShotsRemaining > 0)
            {
                SpecialShotsRemaining--;

                // first wave:
                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2(0, -1),
                                                true,
                                                new Color(75, 75, 255),
                                                true);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(83.0f)), -(float)Math.Sin(MathHelper.ToRadians(83.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                true);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(97.0f)), -(float)Math.Sin(MathHelper.ToRadians(97.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                false);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(76.0f)), -(float)Math.Sin(MathHelper.ToRadians(76.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                false);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(104.0f)), -(float)Math.Sin(MathHelper.ToRadians(104.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                false);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(69.0f)), -(float)Math.Sin(MathHelper.ToRadians(69.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                false);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(111.0f)), -(float)Math.Sin(MathHelper.ToRadians(111.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                false);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(62.0f)), -(float)Math.Sin(MathHelper.ToRadians(62.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                false);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(118.0f)), -(float)Math.Sin(MathHelper.ToRadians(118.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                false);

                specialShotTimer = initMinSpecialShotTimer;
            }
        }

        private void fireHardSpecial()
        {
            if (specialShotTimer <= 0.0f &&
                SpecialShotsRemaining > 0)
            {
                SpecialShotsRemaining--;

                this.PlayerShotManager.FireRocket(this.playerSprite.Location + gunOffset,
                                                        new Vector2(0, -1),
                                                        true,
                                                        Color.White,
                                                        true);

                specialShotTimer = initMinSpecialShotTimer;
            }
        }
        private void fireGreenHornetSpecial()
        {
            if (specialShotTimer <= 0.0f &&
                SpecialShotsRemaining > 0)
            {
                SpecialShotsRemaining--;

                this.PlayerShotManager.FireSonic(this.playerSprite.Location + gunOffset - new Vector2(-10, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(88.0f)), -(float)Math.Sin(MathHelper.ToRadians(88.0f))),
                                                        Color.Black * 0.5f,
                                                        true);

                this.PlayerShotManager.FireSonic(this.playerSprite.Location + gunOffset - new Vector2(10, 0),
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(92.0f)), -(float)Math.Sin(MathHelper.ToRadians(92.0f))),
                                                Color.Black * 0.5f,
                                                true);

                specialShotTimer = initMinSpecialShotTimer;
            }
        }

        private void fireEasySpecial()
        {
            if (specialShotTimer <= 0.0f &&
                SpecialShotsRemaining > 0)
            {
                SpecialShotsRemaining--;

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(84.0f)), -(float)Math.Sin(MathHelper.ToRadians(84.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                true);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(96.0f)), -(float)Math.Sin(MathHelper.ToRadians(96.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                true);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(78.0f)), -(float)Math.Sin(MathHelper.ToRadians(78.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                false);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2((float)Math.Cos(MathHelper.ToRadians(102.0f)), -(float)Math.Sin(MathHelper.ToRadians(102.0f))),
                                                true,
                                                new Color(75, 75, 255),
                                                false);

                specialShotTimer = initMinSpecialShotTimer;
            }
        }

        private void fireSpeederSpecial()
        {
            if (specialShotTimer <= 0.0f &&
                SpecialShotsRemaining > 0)
            {
                SpecialShotsRemaining--;

                this.PlayerShotManager.FireSonic(this.playerSprite.Location + gunOffset,
                                                        new Vector2(0, -1),
                                                        Color.White,
                                                        true);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset + new Vector2(13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(87.0f)), -(float)Math.Sin(MathHelper.ToRadians(89.0f))),
                                                        true,
                                                        Color.White,
                                                        true);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset + new Vector2(-13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(93.0f)), -(float)Math.Sin(MathHelper.ToRadians(91.0f))),
                                                        true,
                                                        Color.White,
                                                        false);

                specialShotTimer = initMinSpecialShotTimer;
            }
        }

        private void fireTankSpecial()
        {
            if (specialShotTimer <= 0.0f &&
                SpecialShotsRemaining > 0)
            {
                SpecialShotsRemaining--;

                this.PlayerShotManager.FireRocket(this.playerSprite.Location + gunOffset,
                                                        new Vector2(0, -1),
                                                        true,
                                                        Color.White,
                                                        true);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset + new Vector2(10, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(88.0f)), -(float)Math.Sin(MathHelper.ToRadians(88.0f))),
                                                        true,
                                                        Color.White,
                                                        true);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset + new Vector2(-10, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(92.0f)), -(float)Math.Sin(MathHelper.ToRadians(92.0f))),
                                                        true,
                                                        Color.White,
                                                        false);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset + new Vector2(13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(85.0f)), -(float)Math.Sin(MathHelper.ToRadians(85.0f))),
                                                        true,
                                                        Color.White,
                                                        false);

                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset + new Vector2(-13, 0),
                                                        new Vector2((float)Math.Cos(MathHelper.ToRadians(95.0f)), -(float)Math.Sin(MathHelper.ToRadians(95.0f))),
                                                        true,
                                                        Color.White,
                                                        false);

                specialShotTimer = initMinSpecialShotTimer;
            }
        }

        private void upgradePlayer()
        {
            if (upgrades > 0 && canUpgrade)
            {
                switch (upgrades)
                {
                    case 1:
                        if (agilityUpgrades < MAX_UPGRADE_LEVEL)
                        {
                            ++agilityUpgrades;
                            ZoomTextManager.ShowInfo("Agility!");
                            SoundManager.PlayUpgradeActivatedSound();
                            upgrades = 0;
                        }
                        
                        break;
                        
                    case 2:
                        if (laserAccuracyUpgrades < MAX_UPGRADE_LEVEL)
                        {
                            ++laserAccuracyUpgrades;
                            ZoomTextManager.ShowInfo("Laser accuracy!");
                            SoundManager.PlayUpgradeActivatedSound();
                            upgrades = 0;
                        }
                        break;
                        
                    case 3:
                        if (laserSpeedUpgrades < MAX_UPGRADE_LEVEL)
                        {
                            ++laserSpeedUpgrades;
                            ZoomTextManager.ShowInfo("Laser speed!");
                            SoundManager.PlayUpgradeActivatedSound();
                            upgrades = 0;
                        }
                        break;

                    case 4:
                        if (shipType == PlayerType.Hard ||
                            shipType == PlayerType.Tank)
                        {
                            SpecialShotsRemaining += 10;
                        }
                        else if (shipType == PlayerType.Easy ||
                                 shipType == PlayerType.Medium)
                        {
                            SpecialShotsRemaining += 20;
                        }
                        else
                        {
                            SpecialShotsRemaining += 30;
                        }
                        ZoomTextManager.ShowInfo("Ammo!");
                        SoundManager.PlayUpgradeActivatedSound();
                        upgrades = 0;
                        break;

                    case 5:
                        if (laserPowerUpgrades < MAX_UPGRADE_LEVEL)
                        {
                            ++laserPowerUpgrades;
                            ZoomTextManager.ShowInfo("Laser power!");
                            SoundManager.PlayUpgradeActivatedSound();
                            upgrades = 0;
                        }
                        break;

                    case 6:
                        if (laserFrequencyUpgrades < MAX_UPGRADE_LEVEL)
                        {
                            ++laserFrequencyUpgrades;
                            ZoomTextManager.ShowInfo("Laser frequency!");
                            SoundManager.PlayUpgradeActivatedSound();
                            upgrades = 0;
                        }
                        break;

                    case 7:
                        IncreaseShield(100.0f);
                        ZoomTextManager.ShowInfo("Shields!");
                        SoundManager.PlayShieldSound();
                        upgrades = 0;
                        break;

                    case 8:
                        if (specialReloadUpgrades < MAX_UPGRADE_LEVEL)
                        {
                            ++specialReloadUpgrades;
                            ZoomTextManager.ShowInfo("Reload speed!");
                            SoundManager.PlayUpgradeActivatedSound();
                            upgrades = 0;
                        }
                        break;

                    case 9:
                        if (laserModeUpgrades < MAX_UPGRADE_LEVEL)
                        {
                            ++laserModeUpgrades;
                            ZoomTextManager.ShowInfo("Laser upgrade!");
                            SoundManager.PlayUpgradeActivatedSound();
                            upgrades = 0;
                        }
                        break;

                    default:
                        IncreaseHitPoints(175.0f);
                        ZoomTextManager.ShowInfo("Repait kit!");
                        SoundManager.PlayRepairSound();
                        upgrades = 0;
                        break;

                }
            }
        }

        private void HandleTouchInput(TouchCollection touches)
        {
            if (IsSensorInput)
                handleSensorControl(touches);
            else
                handleTouchOnlyInput(touches);

            // Upgrade via direct touch
            handleUpgradeTouched();
        }

        private int lastSpecialId = -1;

        private void handleTouchOnlyInput(TouchCollection touches)
        {
            bool fireLaser = false;
            bool fireSpecial = false;

            if (autofireStartTimer > autofireStartMin)
                fireLaser = true;

            TouchLocation specialTouch;
            if (touches.FindById(lastSpecialId, out specialTouch))
            {
                fireSpecial = true;
            }
            else
            {
                if (touches.Count == 2)
                {
                    if (UpperMoveRectSecondTouch.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)))
                    {
                        // store touch id
                        lastSpecialId = touches[1].Id;

                        fireSpecial = true;
                    }
                }
            }

            if (fireLaser || fireSpecial)
                PlayerShotManager.ShotSpeed = initShotSpeed + laserSpeedUpgrades * SHOTSPEED_PER_UPGRADE;

            if (fireLaser)
            {
                fireShot();
            }

            if (fireSpecial)
            {
                fireSpecialShot();
            }

            // set target position in input detected
            if (gameInput.IsPressed(ActionMove))
            {
                bool move = false;
                Vector2 touch = Vector2.Zero;
                // store touch id
                if (touches.Count == 1)
                {
                    if (touches[0].Id != lastSpecialId)
                    {
                        move = true;
                        touch = touches[0].Position;
                    }
                }
                else if (touches.Count == 2)
                {
                    if (touches[0].Id != lastSpecialId)
                    {
                        move = true;
                        touch = touches[0].Position;
                    }
                    else
                    {
                        move = true;
                        touch = touches[1].Position;
                    }
                }

                if (move)
                {
                    targetPlayerPosition = touch + PLAYER_POSITION_OFFSET;
                }
            }

            // move player if target is specified
            if (targetPlayerPosition != Vector2.Zero)
            {
                Vector2 playerDirection = targetPlayerPosition - playerSprite.Location;
                // if target reached
                if (playerDirection.Length() < 2)
                {
                    targetPlayerPosition = Vector2.Zero;
                    playerSprite.Velocity = Vector2.Zero;
                }
                else
                {
                    float factor = 2.0f;
                    float distance = playerDirection.Length();

                    if (distance < 50)
                        factor = Math.Max(0.5f, distance / 25);

                    playerDirection.Normalize();


                    playerDirection *= factor;
                    playerSprite.Velocity = playerDirection;
                }
            }
        }

        private void handleSensorControl(TouchCollection touches)
        {
            bool fireLaser = false;
            bool fireSpecial = false;
            bool upgrade = false;

            if (settings.GetAutofireValue() == false)
            {
                if (touches.Count == 1)
                {
                    // Resove touches
                    if (gameInput.IsPressed(ActionLeft))
                    {
                        if (settings.ControlPosition == SettingsManager.ControlPositionValues.Left)
                            fireLaser = true;
                        else
                            fireSpecial = true;
                    }

                    if (gameInput.IsPressed(ActionRight))
                    {
                        if (settings.ControlPosition == SettingsManager.ControlPositionValues.Left)
                            fireSpecial = true;
                        else
                            fireLaser = true;
                    }

                    if (gameInput.IsPressed(ActionUpper))
                    {
                        upgrade = true;
                    }
                }
                else if (touches.Count == 2)
                {
                    if ((leftSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                        rightSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                        (leftSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                        rightSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                    {
                        fireLaser = true;
                        fireSpecial = true;
                    }
                }
            }
            else
            {
                if (autofireStartTimer > autofireStartMin)
                    fireLaser = true;

                if (gameInput.IsPressed(ActionLeft) || gameInput.IsPressed(ActionRight))
                {
                    fireSpecial = true;
                }

                if (gameInput.IsPressed(ActionUpper))
                {
                    upgrade = true;
                }
            }

            // Resolve actions
            if (upgrade)
            {
                upgradePlayer();
            }

            if (fireLaser || fireSpecial)
                PlayerShotManager.ShotSpeed = initShotSpeed + laserSpeedUpgrades * SHOTSPEED_PER_UPGRADE;

            if (fireLaser)
            {
                fireShot();
            }

            if (fireSpecial)
            {
                fireSpecialShot();
            }

            // Accelerometer controls
            Vector3 current = currentAccValue;
            current.Y = current.Y + (float)Math.Sin(settings.GetNeutralPosition());

            current.Y = MathHelper.Clamp(current.Y, -0.4f, 0.4f);
            current.X = MathHelper.Clamp(current.X, -0.4f, 0.4f);

            playerSprite.Velocity = new Vector2(current.X * 6,
                                                -current.Y * 5);

            if (playerSprite.Velocity.Length() < 0.15f)
            {
                playerSprite.Velocity = Vector2.Zero;
            }
        }

        private void handleUpgradeTouched()
        {
            int index = upgrades - 1;

            if (index >= 0 && index < ActionUpgradeTouch.Length)
            {
                if (gameInput.IsPressed(ActionUpgradeTouch[index]))
                {
                    upgradePlayer();
                }
            }
        }

        private void fireSpecialShot()
        {
            if (shipType == PlayerType.GreenHornet)
                fireGreenHornetSpecial();
            else if (shipType == PlayerType.Easy)
                fireEasySpecial();
            else if (shipType == PlayerType.Medium)
                fireMediumSpecial();
            else if (shipType == PlayerType.Hard)
                fireHardSpecial();
            else if (shipType == PlayerType.Speeder)
                fireSpeederSpecial();
            else if (shipType == PlayerType.Tank)
                fireTankSpecial();
        }

        private void HandleKeyboardInput(KeyboardState state)
        {
            #if DEBUG

            bool fireLaser = false;
            bool fireSpecial = false;
            bool upgrade = false;

            // Resolve keys
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1))
            {
                fireLaser = true;
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D2))
            {
                fireSpecial = true;
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D3))
            {
                upgrade = true;
            }

            // Resolve actions
            if (upgrade)
            {
                upgradePlayer();
            }

            if (fireLaser || fireSpecial)
                PlayerShotManager.ShotSpeed = initShotSpeed + laserSpeedUpgrades * SHOTSPEED_PER_UPGRADE;

            if (fireLaser)
            {
                fireShot();
            }

            if (fireSpecial)
            {
                fireSpecialShot();
            }

            Vector2 velo = Vector2.Zero;

            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                velo += new Vector2(-1.0f, 0.0f);
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                velo += new Vector2(1.0f, 0.0f);
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                velo += new Vector2(0.0f, -1.0f);
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                velo += new Vector2(0.0f, 1.0f);
            }

            if (velo != Vector2.Zero)
                velo.Normalize();

            playerSprite.Velocity = velo;

            #endif
        }

        private void adaptMovementLimits()
        {
            Vector2 location = playerSprite.Location;

            if (location.X < playerAreaLimit.X)
            {
                location.X = playerAreaLimit.X;
                playerSprite.Velocity = Vector2.Zero;
            }

            if (location.X > (playerAreaLimit.Right - playerSprite.Source.Width))
            {
                location.X = (playerAreaLimit.Right - playerSprite.Source.Width);
                playerSprite.Velocity = Vector2.Zero;
            }

            if (location.Y < playerAreaLimit.Y)
            {
                location.Y = playerAreaLimit.Y;
                playerSprite.Velocity = Vector2.Zero;
            }

            if (location.Y > (playerAreaLimit.Bottom - playerSprite.Source.Height - PADDING_BOTTOM))
            {
                location.Y = (playerAreaLimit.Bottom - playerSprite.Source.Height - PADDING_BOTTOM);
                playerSprite.Velocity = Vector2.Zero;
            }

            playerSprite.Location = location;
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            PlayerShotManager.Update(gameTime);

            if (!IsDestroyed)
            {
                shotTimer -= elapsed;
                specialShotTimer -= elapsed;
                specialShotReloadTimer -= elapsed;

                autofireStartTimer += elapsed;

                startUpScale += elapsed;

                if (startUpScale > 1)
                    startUpScale = 1;

                HandleTouchInput(TouchPanel.GetState());
                HandleKeyboardInput(Keyboard.GetState());

                if (playerSprite.Velocity.Length() != 0.0f)
                {
                    playerSprite.Velocity.Normalize();
                }

                float startFactor = (float)Math.Pow(startUpScale, 2.0);

                playerSprite.Velocity *= ((initPlayerSpeed + AGILITY_PER_UPGRADE * agilityUpgrades) * startFactor);

                playerSprite.Rotation = MathHelper.PiOver2 * 3;
                playerSprite.Update(gameTime);
                adaptMovementLimits();

                this.playerSprite.TintColor = Color.White * startFactor;

                // Special shot reload
                if (specialShotReloadTimer <= 0.0f && SpecialShotsRemaining < MaxSpecialShots)
                {
                    SpecialShotsRemaining += 1;
                    specialShotReloadTimer = SpecialShotReloadTimeMax;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);

            if (!IsDestroyed)
            {
                playerSprite.Draw(spriteBatch);
            }
        }

        public void SetLevel(int lvl)
        {
            // nothing happens for player when a level up occures
        }

        public void IncreaseUpgrades()
        {
            if (upgrades < UPGRADES_COUNT)
            {
                ++upgrades;
            }
        }

        public void IncreaseShield()
        {
            this.shieldPoints = initMaxShieldPoints;
        }

        public void IncreaseShield(float value)
        {
            this.shieldPoints += value;

            if (shieldPoints > initMaxShieldPoints)
                this.shieldPoints = initMaxShieldPoints;
        }

        public void IncreasePlayerScore(long score)
        {
            this.playerScore += score;
        }

        public void SetHitPoints(float hp)
        {
            this.hitPoints = MathHelper.Clamp(hp, 0.0f, initMaxHitPoints);
        }

        public void IncreaseHitPoints(float hp)
        {
            if (hp < 0)
                throw new ArgumentException("Negative values are not allowed.");

            this.hitPoints += hp;
            this.hitPoints = MathHelper.Clamp(hitPoints, 0.0f, initMaxHitPoints);
        }

        public void DecreaseHitPoints(float hp, bool affectsShield)
        {
            if (hp < 0)
                throw new ArgumentException("Positive values are not allowed.");
            if (affectsShield)
            {
                float diff = Math.Max(0, hp - this.shieldPoints);

                this.shieldPoints -= hp;
                this.shieldPoints = Math.Max(0, shieldPoints);

                this.hitPoints -= diff;
                this.hitPoints = MathHelper.Clamp(hitPoints, 0.0f, initMaxHitPoints);
            }
            else
            {
                this.hitPoints -= hp;
                this.hitPoints = MathHelper.Clamp(hitPoints, 0.0f, initMaxHitPoints);

                // Clear shield points if player is dead
                if (hitPoints == 0.0f)
                    shieldPoints = 0.0f;
            }
        }

        public void Kill()
        {
            this.hitPoints = 0;
        }

        public void DecreaseLives()
        {
            --livesRemaining;
        }

        public void ResetPlayerScore()
        {
            this.playerScore = 0;
        }

        public long GetPlayerResultScore()
        {
            return 999;
        }

        public void IncreaseCredits(long value)
        {
            credits += value;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            //Player sprite
            playerSprite.Activated(reader);

            this.playerScore = Int64.Parse(reader.ReadLine());

            this.livesRemaining = Int32.Parse(reader.ReadLine());
            this.SpecialShotsRemaining = Int32.Parse(reader.ReadLine());

            this.hitPoints = Single.Parse(reader.ReadLine());

            this.shotTimer = Single.Parse(reader.ReadLine());
            this.specialShotTimer = Single.Parse(reader.ReadLine());

            PlayerShotManager.Activated(reader);

            this.shieldPoints = Single.Parse(reader.ReadLine());

            this.credits = Int32.Parse(reader.ReadLine());

            this.upgrades = Int32.Parse(reader.ReadLine());

            this.canUpgrade = Boolean.Parse(reader.ReadLine());

            laserPowerUpgrades = Int32.Parse(reader.ReadLine());
            laserFrequencyUpgrades = Int32.Parse(reader.ReadLine());
            laserModeUpgrades = Int32.Parse(reader.ReadLine());
            laserSpeedUpgrades = Int32.Parse(reader.ReadLine());
            laserAccuracyUpgrades = Int32.Parse(reader.ReadLine());
            agilityUpgrades = Int32.Parse(reader.ReadLine());
            specialReloadUpgrades = Int32.Parse(reader.ReadLine());

            this.shotCounter = Int64.Parse(reader.ReadLine());

            this.autofireStartTimer = Single.Parse(reader.ReadLine());

            this.startUpScale = Single.Parse(reader.ReadLine());

            IsSensorInput = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Player sprite
            playerSprite.Deactivated(writer);

            writer.WriteLine(this.playerScore);

            writer.WriteLine(this.livesRemaining);
            writer.WriteLine(this.SpecialShotsRemaining);

            writer.WriteLine(this.hitPoints);

            writer.WriteLine(this.shotTimer);
            writer.WriteLine(this.specialShotTimer);

            PlayerShotManager.Deactivated(writer);

            writer.WriteLine(this.shieldPoints);

            writer.WriteLine(credits);

            writer.WriteLine(upgrades);

            writer.WriteLine(canUpgrade);

            writer.WriteLine(laserPowerUpgrades);
            writer.WriteLine(laserFrequencyUpgrades);
            writer.WriteLine(laserModeUpgrades);
            writer.WriteLine(laserSpeedUpgrades);
            writer.WriteLine(laserAccuracyUpgrades);
            writer.WriteLine(agilityUpgrades);
            writer.WriteLine(specialReloadUpgrades);

            writer.WriteLine(shotCounter);

            writer.WriteLine(autofireStartTimer);

            writer.WriteLine(startUpScale);

            writer.WriteLine(IsSensorInput);
        }

        public static int GetUpdateLevelOfIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return agilityUpgrades;
                case 1:
                    return laserAccuracyUpgrades;
                case 2:
                    return laserSpeedUpgrades;
                case 4:
                    return laserPowerUpgrades;
                case 5:
                    return laserFrequencyUpgrades;
                case 7:
                    return specialReloadUpgrades;
                case 8:
                    return laserModeUpgrades;
                default:
                    return 0;
            }
        }

        #endregion

        #region Properties

        public int LivesRemaining
        {
            get
            {
                return this.livesRemaining;
            }
        }

        public float HitPoints
        {
            get
            {
                return this.hitPoints;
            }
        }

        public bool IsDestroyed
        {
            get
            {
                return this.hitPoints <= 0.0f;
            }
        }

        public float ShotPower
        {
            get
            {
                return this.initShotPower + SHOTPOWER_PER_UPGRADE * laserPowerUpgrades;
            }
        }

        public long PlayerScore
        {
            get
            {
                return this.playerScore;
            }
        }

        public float ShieldPoints
        {
            get
            {
                return this.shieldPoints;
            }
        }

        public bool IsShieldActive
        {
            get
            {
                return this.shieldPoints > 0.0f;
            }
        }

        public PlayerType ShipType
        {
            get
            {
                return shipType;
            }
        }

        public long Credits
        {
            get
            {
                return credits;
            }
        }

        public int Upgrades
        {
            get
            {
                return this.upgrades;
            }
        }

        public float InitPlayerSpeed
        {
            get
            {
                return this.initPlayerSpeed;
            }
        }

        public float InitShotSpeed
        {
            get 
            { 
                return this.initShotSpeed; 
            }
        }

        public int InitSpecialShots
        {
            get
            {
                return this.initSpecialShots;
            }
        }

        public float InitShotPower
        {
            get
            {
                return this.initShotPower;
            }
        }

        public float InitMaxHitPoints
        {
            get
            {
                return this.initMaxHitPoints;
            }
        }

        public float InitMaxShieldPoints
        {
            get
            {
                return this.initMaxShieldPoints;
            }
        }

        public float InitMinSpecialShotTimer
        {
            get
            {
                return this.initMinSpecialShotTimer;
            }
        }

        public float InitMinSpecialShotReloadTimer
        {
            get
            {
                return this.initMinSpecialShotReloadTimer;
            }
        }

        public float InitInaccuracy
        {
            get
            {
                return this.initInaccuracy;
            }
        }

        public float SpecialShotReloadTime
        {
            get
            {
                return this.specialShotReloadTimer;
            }
        }

        public float SpecialShotReloadTimeMax
        {
            get
            {
                return initMinSpecialShotReloadTimer - 0.66f * (specialReloadUpgrades / (float)MAX_UPGRADE_LEVEL) * initMinSpecialShotReloadTimer;
            }
        }

        public bool CanUpgrade
        {
            set
            {
                this.canUpgrade = value;
            }
        }

        #endregion

        #region Events

        private void OnAccelerometerHelperReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            currentAccValue = new Vector3((float)e.OptimalyFilteredAcceleration.X,
                                          (float)e.OptimalyFilteredAcceleration.Y,
                                          (float)e.OptimalyFilteredAcceleration.Z);
        }

        #endregion
    }
}
