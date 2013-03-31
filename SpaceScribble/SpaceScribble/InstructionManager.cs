using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.IsolatedStorage;
using SpaceScribble.Extensions;

namespace SpaceScribble
{
    class InstructionManager
    {
        #region Members

        private float progressTimer = 0.0f;

        public enum InstructionStates { 
            Welcome,                       
            Movement,              
            PrimaryShot, 
            SpecialShot,
            ChangeControls,
            Autofire,
            UpgradeCollect,
            UpgradeActivate,
            UpgradeSortiment,
            Upgrade1,
            Upgrade2,
            Upgrade3,
            Upgrade4,
            Upgrade5,
            Upgrade6,
            Upgrade7,
            Upgrade8,
            Upgrade9,
            Upgrade10,
            UpgradeKey,
            HitPoints, 
            Shield,
            Reload, 
            BewareAsteroid, 
            KillEnemies, 
            Credits, 
            KillBoss,
            BossBonus,
            GoodLuck, 
            Finished};

        private InstructionStates state = InstructionStates.Welcome;

        private const float WelcomeLimit = 2.0f;
        private const float MovementLimit = 5.0f;
        private const float PrimaryShotLimit = 9.0f;
        private const float SpecielShotLimit = 13.0f;
        private const float ChangeControlsLimit = 17.0f;
        private const float AutofireLimit = 21.0f;
        private const float UpgradeCollectLimit = 27.0f;
        private const float UpgradeActivateLimit = 33.0f;
        private const float UpgradeSortimentLimit = 36.0f;
        private const float Upgrade1Limit = 38.0f;
        private const float Upgrade2Limit = 40.0f;
        private const float Upgrade3Limit = 42.0f;
        private const float Upgrade4Limit = 44.0f;
        private const float Upgrade5Limit = 46.0f;
        private const float Upgrade6Limit = 48.0f;
        private const float Upgrade7Limit = 50.0f;
        private const float Upgrade8Limit = 52.0f;
        private const float Upgrade9Limit = 54.0f;
        private const float Upgrade10Limit = 56.0f;
        private const float UpgradeKeyLimit = 59.0f;
        private const float HitPointsLimit = 62.0f;
        private const float ShieldLimit = 65.0f;
        private const float ReloadLimit = 68.0f;
        private const float BewareAsteroidsLimit = 71.0f;
        private const float KillEnemiesLimit = 74.0f;
        private const float CreditsLimit = 77.0f;
        private const float KillBossLimit = 80.0f;
        private const float BossBonusLimit = 83.0f;
        private const float GoodLuckLimit = 86.0f;
        private const float FinishedLimit = 89.0f;

        private SpriteFont font;

        private Texture2D texture;

        private Rectangle screenBounds;

        private Rectangle areaSmallSource = new Rectangle(380, 550, 220, 380);
        private Rectangle areaBigSource = new Rectangle(600, 550, 460, 380);
        private Rectangle arrowRightSource = new Rectangle(650, 140, 40, 20);

        private Rectangle leftShotDestination = new Rectangle(10, 410, 220, 350);
        private Rectangle rightShotDestination = new Rectangle(250, 410, 220, 350);
        private Rectangle topDestination = new Rectangle(10, 20, 460, 350);
        private Rectangle bottomDestination = new Rectangle(10, 410, 460, 350);
        private Rectangle hitPointsDestination = new Rectangle(262, 5, 40, 20);
        private Rectangle shieldDestination = new Rectangle(262, 25, 40, 20);
        private Rectangle reloadDestination = new Rectangle(233, 45, 40, 20);

        private Rectangle upgradeArrowDestination = new Rectangle(52, 720, 40, 20);

        private Color areaTint = Color.Black * 0.5f;
        private Color arrowTint = Color.Black * 0.8f;

        private AsteroidManager asteroidManager;

        private EnemyManager enemyManager;

        private PlayerManager playerManager;

        private PowerUpManager powerUpManager;

        private BossManager bossManager;

        private readonly string WelcomeText = "Welcome to SpaceScribble!";
        private readonly string MovementText = "Move the spaceship by tilting your phone";
        private readonly string PrimaryShotText = "Press here to fire your laser gun";
        private readonly string SpecialShotText = "And here to fire your special weapon!";
        private readonly string SpecialShotAutofireText = "Press here to fire your special weapon!";
        private readonly string ChangeControlsText = "You can swap controls...";
        private readonly string AutofireDisableText = "Or disable autofire in settings menu!";
        private readonly string AutofireEnableText = "Or enable autofire in settings menu!";
        private readonly string UpgradeCollectText = "Collect upgrade items!";
        private readonly string[] UpgradeActivateText = {"DOUBLE TAP here", 
                                                         "to activate your desired upgrade",
                                                         "or",
                                                         "TAP the highlighted upgrade below!"};
        private readonly string UpgradeSortimentText = "There a several power ups...";
        private readonly string Upgrade1Text = "Agility";
        private readonly string Upgrade2Text = "Laser accuracy";
        private readonly string Upgrade3Text = "Laser speed";
        private readonly string Upgrade4Text = "Extra ammo";
        private readonly string Upgrade5Text = "Laser power";
        private readonly string Upgrade6Text = "Laser frequency";
        private readonly string Upgrade7Text = "Shields";
        private readonly string Upgrade8Text = "Reload speed";
        private readonly string Upgrade9Text = "Laser upgrade";
        private readonly string Upgrade10Text = "Repair kit";
        private readonly string[] UpgradeKeyText = {"Choosing the right upgrades", "plays a key role in SpaceScribble!"};
        private readonly string HitPointsText = "The HUD display your current hit points...";
        private readonly string ShieldText = "Your shield level...";
        private readonly string ReloadText = "And your remaining special weapon ammo";
        private readonly string BewareAsteroidsText = "Beware flying asteroids!";
        private readonly string KillEnemiesText = "Kill enemies to score!";
        private readonly string CreditsText = "Collect the credits to buy new spaceships!";
        private readonly string KillBossText = "Defeat the BOSS to reach the next level";
        private readonly string[] BossBonusText = {"Kill the BOSS at the first try", "to gain bonus score!"};
        private readonly string GoodLuckText = "Good luck commander!";
        private readonly string ReturnWithBackButtonText = "Press BACK to return...";
        private readonly string ContinueWithBackButtonText = "Press BACK to start the game...";

        private bool hasDoneInstructions = false;

        private const string OLD_INSTRUCTION_FILE = "instructions2.txt";
        private const string INSTRUCTION_FILE = "instructions3.txt";

        SettingsManager settings = SettingsManager.GetInstance();

        private bool powerUpsDropped;

        private bool isInvalidated = false;

        private bool isAutostarted;

        #endregion

        #region Constructors

        public InstructionManager(Texture2D texture, SpriteFont font, Rectangle screenBounds,
                                  AsteroidManager asteroidManager, PlayerManager playerManager,
                                  EnemyManager enemyManager, BossManager bossManager, PowerUpManager powerUpManager)
        {
            this.texture = texture;
            this.font = font;
            this.screenBounds = screenBounds;

            this.asteroidManager = asteroidManager;
            this.asteroidManager.Reset();

            this.enemyManager = enemyManager;
            this.enemyManager.Reset();

            this.bossManager = bossManager;
            this.bossManager.Reset();

            this.playerManager = playerManager;
            this.playerManager.Reset();

            this.powerUpManager = powerUpManager;
            this.powerUpManager.Reset();

            loadHasDoneInstructions();
        }

        #endregion

        #region Methods

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            progressTimer += elapsed;

            if (playerManager.IsDestroyed)
            {
                this.state = InstructionStates.Finished;

                powerUpManager.Update(gameTime);
                enemyManager.Update(gameTime);
                bossManager.Update(gameTime);
                asteroidManager.IsActive = true;
            }
            else if (progressTimer < WelcomeLimit)
            {
                this.state = InstructionStates.Welcome;
                asteroidManager.IsActive = false;
            }
            else if (progressTimer < MovementLimit)
            {
                this.state = InstructionStates.Movement;
            }
            else if (progressTimer < PrimaryShotLimit)
            {
                if (settings.GetAutofireValue())
                {
                    this.state = InstructionStates.SpecialShot;
                    progressTimer += 4.0f;
                }
                else
                {
                    this.state = InstructionStates.PrimaryShot;
                }
            }
            else if (progressTimer < SpecielShotLimit)
            {
                this.state = InstructionStates.SpecialShot;
            }
            else if (progressTimer < ChangeControlsLimit)
            {
                this.state = InstructionStates.ChangeControls;
            }
            else if (progressTimer < AutofireLimit)
            {
                this.state = InstructionStates.Autofire;
            }
            else if (progressTimer < UpgradeCollectLimit)
            {
                this.state = InstructionStates.UpgradeCollect;

                if (!powerUpsDropped)
                {
                    powerUpsDropped = true;
                    dropPowerUps();
                }

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < UpgradeActivateLimit)
            {
                this.state = InstructionStates.UpgradeActivate;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < UpgradeSortimentLimit)
            {
                this.state = InstructionStates.UpgradeSortiment;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade1Limit)
            {
                this.state = InstructionStates.Upgrade1;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade2Limit)
            {
                this.state = InstructionStates.Upgrade2;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade3Limit)
            {
                this.state = InstructionStates.Upgrade3;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade4Limit)
            {
                this.state = InstructionStates.Upgrade4;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade5Limit)
            {
                this.state = InstructionStates.Upgrade5;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade6Limit)
            {
                this.state = InstructionStates.Upgrade6;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade7Limit)
            {
                this.state = InstructionStates.Upgrade7;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade8Limit)
            {
                this.state = InstructionStates.Upgrade8;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade9Limit)
            {
                this.state = InstructionStates.Upgrade9;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < Upgrade10Limit)
            {
                this.state = InstructionStates.Upgrade10;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < UpgradeKeyLimit)
            {
                this.state = InstructionStates.UpgradeKey;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < HitPointsLimit)
            {
                this.state = InstructionStates.HitPoints;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < ShieldLimit)
            {
                this.state = InstructionStates.Shield;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < ReloadLimit)
            {
                this.state = InstructionStates.Reload;

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < BewareAsteroidsLimit)
            {
                this.state = InstructionStates.BewareAsteroid;

                asteroidManager.IsActive = true;

                enemyManager.Update(gameTime);

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < KillEnemiesLimit)
            {
                this.state = InstructionStates.KillEnemies;
                
                enemyManager.Update(gameTime);

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < CreditsLimit)
            {
                this.state = InstructionStates.Credits;

                enemyManager.Update(gameTime);

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < KillBossLimit)
            {
                if (this.state != InstructionStates.KillBoss)
                {
                    bossManager.SpawnRandomBoss();
                }

                this.state = InstructionStates.KillBoss;

                enemyManager.Update(gameTime);

                bossManager.Update(gameTime);

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < BossBonusLimit)
            {
                this.state = InstructionStates.BossBonus;

                enemyManager.Update(gameTime);

                bossManager.Update(gameTime);

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < GoodLuckLimit)
            {
                this.state = InstructionStates.GoodLuck;

                enemyManager.Update(gameTime);
                bossManager.Update(gameTime);
                powerUpManager.Update(gameTime);
            }
            else
            {
                this.state = InstructionStates.Finished;

                enemyManager.Update(gameTime);
                bossManager.Update(gameTime);
                powerUpManager.Update(gameTime);
            }

            playerManager.Update(gameTime);
            asteroidManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch(this.state)
            {
                case InstructionStates.Welcome:
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, WelcomeText);
                    break;

                case InstructionStates.Movement:
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, MovementText);
                    break;

                case InstructionStates.PrimaryShot:
                    Rectangle destPrim;

                    if (settings.ControlPosition == SettingsManager.ControlPositionValues.Left)
                        destPrim = leftShotDestination;
                    else
                        destPrim = rightShotDestination;

                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                        destPrim,
                                        areaSmallSource,
                                        areaTint * 0.6f,
                                        0.0f,
                                        Vector2.Zero,
                                        SpriteEffects.None,
                                        0.0f);
                    drawCenteredText(spriteBatch, PrimaryShotText);
                break;

                case InstructionStates.SpecialShot:
                    if (settings.GetAutofireValue())
                    {
                        playerManager.Draw(spriteBatch);

                        spriteBatch.Draw(texture,
                                         bottomDestination,
                                         areaBigSource,
                                         areaTint * 0.6f,
                                         0.0f,
                                         Vector2.Zero,
                                         SpriteEffects.FlipHorizontally,
                                         0.0f);
                        drawCenteredText(spriteBatch, SpecialShotAutofireText);
                    }
                    else
                    {
                        Rectangle destSpecial;

                        if (settings.ControlPosition == SettingsManager.ControlPositionValues.Right)
                            destSpecial = leftShotDestination;
                        else
                            destSpecial = rightShotDestination;

                        playerManager.Draw(spriteBatch);

                        spriteBatch.Draw(texture,
                                         destSpecial,
                                         areaSmallSource,
                                         areaTint * 0.6f,
                                         0.0f,
                                         Vector2.Zero,
                                         SpriteEffects.FlipHorizontally,
                                         0.0f);
                        drawCenteredText(spriteBatch, SpecialShotText);
                    }
                    break;

                case InstructionStates.ChangeControls:
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, ChangeControlsText);
                    break;

                case InstructionStates.Autofire:
                    playerManager.Draw(spriteBatch);
                    if (settings.GetAutofireValue())
                        drawCenteredText(spriteBatch, AutofireDisableText);
                   else
                        drawCenteredText(spriteBatch, AutofireEnableText);
                    break;

                case InstructionStates.UpgradeCollect:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, UpgradeCollectText);
                    break;

                case InstructionStates.UpgradeActivate:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     topDestination,
                                     areaBigSource,
                                     areaTint * 0.6f,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.FlipHorizontally,
                                     0.0f);

                    drawCenteredText(spriteBatch, UpgradeActivateText);
                    break;

                case InstructionStates.UpgradeSortiment:
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, UpgradeSortimentText);
                    break;

                case InstructionStates.Upgrade1:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade1Text);
                    break;

                case InstructionStates.Upgrade2:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X + 1 * Hud.UPGRADES_OFFSET_X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade2Text);
                    break;

                case InstructionStates.Upgrade3:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X + 2 * Hud.UPGRADES_OFFSET_X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade3Text);
                    break;

                case InstructionStates.Upgrade4:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X + 3 * Hud.UPGRADES_OFFSET_X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade4Text);
                    break;

                case InstructionStates.Upgrade5:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X + 4 * Hud.UPGRADES_OFFSET_X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade5Text);
                    break;

                case InstructionStates.Upgrade6:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X + 5 * Hud.UPGRADES_OFFSET_X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade6Text);
                    break;

                case InstructionStates.Upgrade7:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X + 6 * Hud.UPGRADES_OFFSET_X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade7Text);
                    break;

                case InstructionStates.Upgrade8:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X + 7 * Hud.UPGRADES_OFFSET_X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade8Text);
                    break;

                case InstructionStates.Upgrade9:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X + 8 * Hud.UPGRADES_OFFSET_X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade9Text);
                    break;

                case InstructionStates.Upgrade10:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     new Rectangle(upgradeArrowDestination.X + 9 * Hud.UPGRADES_OFFSET_X, upgradeArrowDestination.Y,
                                                   upgradeArrowDestination.Width, upgradeArrowDestination.Height),
                                     arrowRightSource,
                                     arrowTint,
                                     MathHelper.PiOver2,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f,
                                     Color.White);
                    drawCenteredText(spriteBatch, Upgrade10Text);
                    break;

                case InstructionStates.UpgradeKey:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, UpgradeKeyText);
                    break;

                case InstructionStates.HitPoints:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     hitPointsDestination,
                                     arrowRightSource,
                                     arrowTint,
                                     Color.White);
                    drawCenteredText(spriteBatch, HitPointsText);
                    break;

                case InstructionStates.Shield:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     shieldDestination,
                                     arrowRightSource,
                                     arrowTint,
                                     Color.White);
                    drawCenteredText(spriteBatch, ShieldText);
                    break;

                case InstructionStates.Reload:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(texture,
                                     reloadDestination,
                                     arrowRightSource,
                                     arrowTint,
                                     Color.White);
                    drawCenteredText(spriteBatch, ReloadText);
                    break;

                case InstructionStates.BewareAsteroid:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, BewareAsteroidsText);
                    break;

                case InstructionStates.KillEnemies:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, KillEnemiesText);
                    break;

                case InstructionStates.Credits:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, CreditsText);
                    break;

                case InstructionStates.KillBoss:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    bossManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, KillBossText);
                    break;

                case InstructionStates.BossBonus:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    bossManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, BossBonusText);
                    break;

                case InstructionStates.GoodLuck:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    bossManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawCenteredText(spriteBatch, GoodLuckText);
                    break;

                case InstructionStates.Finished:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    bossManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    if (isAutostarted)
                        drawCenteredText(spriteBatch, ContinueWithBackButtonText);
                    else
                        drawCenteredText(spriteBatch, ReturnWithBackButtonText);
                    break;
            }
        }

        private void drawCenteredText(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.DrawStringBordered(font,
                                   text,
                                   new Vector2(screenBounds.Width / 2 - font.MeasureString(text).X / 2,
                                               screenBounds.Height / 2 - font.MeasureString(text).Y / 2),
                                   Color.Black,
                                   Color.White);
        }

        private void drawCenteredText(SpriteBatch spriteBatch, string[] texts)
        {
            for (var i = 0; i < texts.Length; ++i)
            {
                spriteBatch.DrawStringBordered(font,
                                   texts[i],
                                   new Vector2(screenBounds.Width / 2 - font.MeasureString(texts[i]).X / 2,
                                               (screenBounds.Height / 2 - font.MeasureString(texts[i]).Y / 2) - 16 + (i * 32)),
                                   Color.Black,
                                   Color.White);
            }
        }

        private void dropPowerUps()
        {
            powerUpManager.SpawnUpgradePowerUp(new Vector2(150, -50));
            powerUpManager.SpawnUpgradePowerUp(new Vector2(210, -100));
            powerUpManager.SpawnUpgradePowerUp(new Vector2(270, -150));
            powerUpManager.SpawnUpgradePowerUp(new Vector2(330, -200));
        }

        public void Reset()
        {
            this.progressTimer = 0.0f;
            this.state = InstructionStates.Welcome;
            this.powerUpsDropped = false;
            this.isAutostarted = false;
        }

        public void InstructionsDone()
        {
            if (!hasDoneInstructions)
            {
                hasDoneInstructions = true;
                isInvalidated = true;
            }
        }

        public void SaveHasDoneInstructions()
        {
            if (!isInvalidated)
                return;

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(INSTRUCTION_FILE, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(hasDoneInstructions);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        private void loadHasDoneInstructions()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(INSTRUCTION_FILE);

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(INSTRUCTION_FILE, FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    isInvalidated = false;

                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            hasDoneInstructions = Boolean.Parse(sr.ReadLine());
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(hasDoneInstructions);

                            // ... ? 
                        }
                    }
                }

                // Delete the old file
                if (isf.FileExists(OLD_INSTRUCTION_FILE))
                    isf.DeleteFile(OLD_INSTRUCTION_FILE);
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.progressTimer = Single.Parse(reader.ReadLine());
            hasDoneInstructions = Boolean.Parse(reader.ReadLine());
            this.powerUpsDropped = Boolean.Parse(reader.ReadLine());
            this.isInvalidated = Boolean.Parse(reader.ReadLine());
            this.isAutostarted = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(progressTimer);
            writer.WriteLine(hasDoneInstructions);
            writer.WriteLine(powerUpsDropped);
            writer.WriteLine(isInvalidated);
            writer.WriteLine(isAutostarted);
        }

        #endregion

        #region Properties

        public bool HasDoneInstructions
        {
            get
            {
                return hasDoneInstructions;
            }
        }

        public bool IsAutostarted
        {
            set
            {
                this.isAutostarted = value;
            }
            get
            {
                return this.isAutostarted;
            }
        }

        public bool EnougthInstructionsDone
        {
            get
            {
                return (progressTimer > 5.0f);
            }
        }

        #endregion
    }
}
