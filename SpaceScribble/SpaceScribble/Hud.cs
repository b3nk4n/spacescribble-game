using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using SpaceScribble.Extensions;

namespace SpaceScribble
{
    class Hud
    {
        #region Members

        private static Hud hud;

        private long score;
        private int remainingLives;
        private float hitPoints;
        private float maxHitPoints;
        private float shieldPoints;
        private float maxShieldPoints;
        private int specialShots;
        private float specialShotReloadTimeMax;
        private float specialShotReloadTime;
        private int level;
        private int upgrades;

        private BossManager bossManager;
        private PlayerManager playerManager;

        private Rectangle screenBounds;
        private Texture2D texture;
        private SpriteFont font;
        private SpriteFont fontSmall;

        private Vector2 scoreLocation = new Vector2(5, 3);
        private Vector2 hitPointLocation = new Vector2(325, 10);
        private Vector2 shieldPointLocation = new Vector2(325, 30);
        private Vector2 specialShotReloadLocation = new Vector2(325, 50);

        private readonly Rectangle hitPointSymbolSoruce = new Rectangle(800, 140, 14, 14);
        private readonly Rectangle shieldPointSymbolSoruce = new Rectangle(814, 140, 14, 14);

        private Vector2 hitPointSymbolLocation = new Vector2(305, 8);
        private Vector2 shieldPointSymbolLocation = new Vector2(305, 28);
        private Vector2 specialShotReloadSymbolLocation = new Vector2(305, 48);
        
        private readonly Rectangle SpecialShotsDestination = new Rectangle(305, 50, 14, 14);
        private readonly Rectangle SpecialShotsLaserSource = new Rectangle(800, 50, 24, 24);
        private readonly Rectangle SpecialShotsRocketSource = new Rectangle(824, 50, 24, 24);
        private readonly Rectangle SpecialShotsSonicSource = new Rectangle(848, 50, 24, 24);

        private Vector2 bossHitPointLocation = new Vector2(0, 780);

        private readonly Vector2 SmallBarOverlayStart = new Vector2(600, 950);
        private readonly Vector2 BigBarOverlayStart = new Vector2(600, 930);
        private const int BarWidth = 150;
        private const int BarHeight = 10;
        private const int BossBarWidth = 480;
        private const int BossBarHeight = 20;

        private int lastLevel = -1;
        private StringBuilder currentLevelText = new StringBuilder(16);
        private const string LEVEL_PRE_TEXT = "Level: ";

        // Upgrade stairs
        public const int DISPLAYED_UPGRADES = 5;
        public static readonly Vector2 UpgradesLocation = new Vector2(42, 768);
        public const int UPGRADE_SOURCE_DIMENSION = 40;
        public const int UPGRADE_LARGE_DIMENSION = 40;
        public const int UPGRADE_SMALL_DIMENSION = 32;
        public static readonly Vector2 UpgradesSource = new Vector2(650, 100);
        public const int UPGRADES_OFFSET_X = 44;

        #endregion

        #region Constructors

        private Hud(Rectangle screen, Texture2D texture, SpriteFont font, SpriteFont fontSmall,
                    long score, int lives, float hitPoints, float maxHitPoints, float shieldPoints, float maxShieldPoints,
                    int specialShots, float specialShotReloadTimeMax, float specialShotReloadTime,
                    int level, int upgrades, BossManager bossManager, PlayerManager player)
        {
            this.screenBounds = screen;
            this.texture = texture;
            this.font = font;
            this.fontSmall = fontSmall;
            this.score = score;
            this.remainingLives = lives;
            this.hitPoints = hitPoints;
            this.maxHitPoints = maxHitPoints;
            this.shieldPoints = shieldPoints;
            this.maxShieldPoints = maxShieldPoints;
            this.specialShots = specialShots;
            this.specialShotReloadTimeMax = specialShotReloadTimeMax;
            this.specialShotReloadTime = specialShotReloadTime;
            this.level = level;
            this.upgrades = upgrades;
            this.bossManager = bossManager;
            this.playerManager = player;
        }

        #endregion

        #region Methods

        public static Hud GetInstance(Rectangle screen, Texture2D texture, SpriteFont font, SpriteFont fontSmall, long score,
                                      int lives, float hitPoints, float maxHitPoints, float shieldPoints, float maxShieldPoints,
                                      int specialShots, float specialShotReloadTimeMax, float specialShotReloadTime,
                                      int level, int upgrades, BossManager boss, PlayerManager player)
        {
            if (hud == null)
            {
                hud = new Hud(screen,
                              texture,
                              font,
                              fontSmall,
                              score,
                              lives,
                              hitPoints,
                              maxHitPoints,
                              shieldPoints,
                              maxShieldPoints,
                              specialShots,
                              specialShotReloadTimeMax,
                              specialShotReloadTime,
                              level,
                              upgrades,
                              boss,
                              player);
            }

            return hud;
        }

        public void Update(long score, int lives, float hitPoints, float maxHitPoints, float shieldPoints, float maxShieldPoints,
                           int specialShots, float specialShotReloadTimeMax, float specialShotReloadTime,
                           int level, int upgrades)
        {
            this.score = score;
            this.remainingLives = lives;
            this.hitPoints = hitPoints;
            this.maxHitPoints = maxHitPoints;
            this.shieldPoints = shieldPoints;
            this.maxShieldPoints = maxShieldPoints;
            this.specialShots = specialShots;
            this.specialShotReloadTimeMax = specialShotReloadTimeMax;
            this.specialShotReloadTime = specialShotReloadTime;
            this.level = level;
            this.upgrades = upgrades;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            drawScore(spriteBatch);

            if (remainingLives >= 0)
            {
                drawLevel(spriteBatch);
                drawHitPoints(spriteBatch);
                drawShieldPoints(spriteBatch);
                drawSpecialShots(spriteBatch);
                

                if (bossManager.Bosses.Count > 0)
                    drawBossHitPoints(spriteBatch, bossManager.Bosses[0].HitPoints, bossManager.Bosses[0].MaxHitPoints);
                else
                    drawUpgrades(spriteBatch);
            }       
        }

        private void drawScore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawInt64WithZerosBordered(font,
                                  score,
                                  scoreLocation,
                                  Color.Black * 0.8f,
                                  Color.White * 0.8f,
                                  11);
        }

        private void drawSpecialShots(SpriteBatch spriteBatch)
        {
            Rectangle src;

            if (playerManager.ShipType == PlayerManager.PlayerType.Easy ||
               playerManager.ShipType == PlayerManager.PlayerType.Medium)
            {
                src = SpecialShotsLaserSource;
            }
            else if (playerManager.ShipType == PlayerManager.PlayerType.Hard ||
               playerManager.ShipType == PlayerManager.PlayerType.Tank)
            {
                src = SpecialShotsRocketSource;
            }
            else
            {
                src = SpecialShotsSonicSource;
            }

            spriteBatch.DrawBordered(texture,
                             SpecialShotsDestination,
                             src,
                             Color.Black * 0.8f,
                             Color.White * 0.8f);

            string remainingSpecialShots = specialShots.ToString();

            spriteBatch.DrawStringBordered(fontSmall,
                                   remainingSpecialShots,
                                   new Vector2(SpecialShotsDestination.X - 6 - (font.MeasureString(remainingSpecialShots).X),
                                               SpecialShotsDestination.Y - 5),
                                   Color.Black * 0.8f,
                                   Color.White * 0.8f);

            spriteBatch.DrawBordered(texture,
                    new Rectangle(
                        (int)specialShotReloadLocation.X,
                        (int)specialShotReloadLocation.Y,
                        BarWidth,
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        BarWidth,
                        BarHeight),
                    Color.Black * 0.3f,
                    Color.White * 0.3f);

            spriteBatch.DrawBordered(texture,
                    new Rectangle(
                        (int)specialShotReloadLocation.X,
                        (int)specialShotReloadLocation.Y,
                        (int)(BarWidth * (1.0f - specialShotReloadTime / specialShotReloadTimeMax)),
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        (int)(BarWidth * (1.0f - specialShotReloadTime / specialShotReloadTimeMax)),
                        BarHeight),
                    Color.Black * 0.75f,
                    Color.White * 0.75f);
        }

        private void drawUpgrades(SpriteBatch spriteBatch)
        {
            Rectangle src = new Rectangle((int)UpgradesSource.X, (int)UpgradesSource.Y,
                                         UPGRADE_SOURCE_DIMENSION,
                                         UPGRADE_SOURCE_DIMENSION);

            for (int i = 0; i < PlayerManager.UPGRADES_COUNT; ++i)
            {
                Rectangle dest;
                Color tint;

                if (upgrades == i + 1)
                {
                    // large
                    dest = new Rectangle((int)UpgradesLocation.X - UPGRADE_LARGE_DIMENSION / 2 + (i * UPGRADES_OFFSET_X), (int)UpgradesLocation.Y - ((UPGRADE_LARGE_DIMENSION - UPGRADE_SMALL_DIMENSION)),
                                         UPGRADE_LARGE_DIMENSION, UPGRADE_LARGE_DIMENSION);
                    if (PlayerManager.GetUpdateLevelOfIndex(i) < PlayerManager.MAX_UPGRADE_LEVEL)
                        tint = Color.White * 0.8f;
                    else
                        tint = Color.White * 0.3f;
                }
                else
                {
                    // small
                    dest = new Rectangle((int)UpgradesLocation.X - UPGRADE_SMALL_DIMENSION / 2 + (i * UPGRADES_OFFSET_X), (int)UpgradesLocation.Y,
                                         UPGRADE_SMALL_DIMENSION, UPGRADE_SMALL_DIMENSION);
                    if (PlayerManager.GetUpdateLevelOfIndex(i) < PlayerManager.MAX_UPGRADE_LEVEL)
                        tint = Color.White * 0.6f;
                    else
                        tint = Color.White * 0.3f;
                }

                spriteBatch.Draw(texture,
                                 dest,
                                 src,
                                 tint);

                src.X += UPGRADE_SOURCE_DIMENSION;
            }
        }

        private void drawHitPoints(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawBordered(texture,
                             hitPointSymbolLocation,
                             hitPointSymbolSoruce,
                             Color.Black * 0.8f,
                             Color.White * 0.8f);

            spriteBatch.DrawBordered(texture,
                    new Rectangle(
                        (int)hitPointLocation.X,
                        (int)hitPointLocation.Y,
                        BarWidth,
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        BarWidth,
                        BarHeight),
                    Color.Black * 0.3f,
                    Color.White * 0.3f);

            spriteBatch.DrawBordered(texture,
                    new Rectangle(
                        (int)hitPointLocation.X,
                        (int)hitPointLocation.Y,
                        (int)(BarWidth * hitPoints / maxHitPoints),
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        (int)(BarWidth * hitPoints / maxHitPoints),
                        BarHeight),
                    Color.Black * 0.75f,
                    Color.White * 0.75f);
        }

        private void drawShieldPoints(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawBordered(texture,
                             shieldPointSymbolLocation,
                             shieldPointSymbolSoruce,
                             Color.Black * 0.8f,
                             Color.White * 0.8f);

            spriteBatch.DrawBordered(texture,
                    new Rectangle(
                        (int)shieldPointLocation.X,
                        (int)shieldPointLocation.Y,
                        BarWidth,
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        BarWidth,
                        BarHeight),
                    Color.Black * 0.3f,
                    Color.White * 0.3f);

            spriteBatch.DrawBordered(texture,
                    new Rectangle(
                        (int)shieldPointLocation.X,
                        (int)shieldPointLocation.Y,
                        (int)(BarWidth * shieldPoints / maxShieldPoints),
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        (int)(BarWidth * shieldPoints / maxShieldPoints),
                        BarHeight),
                    Color.Black * 0.75f,
                    Color.White * 0.75f);
        }

        private void drawLevel(SpriteBatch spriteBatch)
        {
            if (lastLevel != level || currentLevelText.Length == 0)
            {
                if (currentLevelText.Length != 0)
                    currentLevelText.Clear();

                lastLevel = level;

                currentLevelText.Append(LEVEL_PRE_TEXT)
                                .Append(level);
            }


            spriteBatch.DrawStringBordered(font,
                                   currentLevelText.ToString(),
                                   new Vector2(screenBounds.Width / 2 - (font.MeasureString(currentLevelText).Y / 2) - 30,
                                               5),
                                   Color.Black * 0.8f,
                                   Color.White * 0.8f);
        }

        private void drawBossHitPoints(SpriteBatch spriteBatch, float bossHitPoints, float bossMaxHitPoints)
        {
            float factor = BossBarWidth / bossMaxHitPoints;

            spriteBatch.DrawBordered(texture,
                    new Rectangle(
                        (int)bossHitPointLocation.X,
                        (int)bossHitPointLocation.Y,
                        BossBarWidth,
                        BossBarHeight),
                    new Rectangle(
                        (int)BigBarOverlayStart.X,
                        (int)BigBarOverlayStart.Y,
                        BossBarWidth,
                        BossBarHeight),
                    Color.Black * 0.3f,
                    Color.White * 0.3f);

            spriteBatch.DrawBordered(texture,
                    new Rectangle(
                        (int)bossHitPointLocation.X,
                        (int)bossHitPointLocation.Y,
                        (int)(factor * bossHitPoints),
                        BossBarHeight),
                    new Rectangle(
                        (int)BigBarOverlayStart.X,
                        (int)BigBarOverlayStart.Y,
                        (int)(factor * bossHitPoints),
                        BossBarHeight),
                    Color.Black * 0.75f,
                    Color.White * 0.75f);
        }

        #endregion
    }
}
