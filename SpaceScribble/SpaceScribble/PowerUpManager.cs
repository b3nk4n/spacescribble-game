using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpaceScribble
{
    class PowerUpManager
    {
        #region Members

        private List<PowerUp> powerUps = new List<PowerUp>(16);
        private Texture2D texture;

        private const int SPAWN_CHANCE = 15;
        private const int SPAWN_CHANCE10 = 14;
        private const int SPAWN_CHANCE20 = 12;
        private const int SPAWN_CHANCE30 = 10;
        private const int SPAWN_CHANCE40 = 8;
        private const int HIGH_SPAWN_CHANCE = 66; // v1.0: 75%

        private Random rand = new Random();

        #endregion

        #region Constructors

        public PowerUpManager(Texture2D texture)
        {
            this.texture = texture;
        }

        #endregion

        #region Methods

        public void ProbablySpawnPowerUp(Vector2 location)
        {
            int spawnChance = rand.Next(100);

            int spawnChanceLimit;

            if (LevelManager.CurrentLevelStatic > 40)
                spawnChanceLimit = SPAWN_CHANCE40;
            else if (LevelManager.CurrentLevelStatic > 30)
                spawnChanceLimit = SPAWN_CHANCE30;
            else if (LevelManager.CurrentLevelStatic > 20)
                spawnChanceLimit = SPAWN_CHANCE20;
            else if (LevelManager.CurrentLevelStatic > 10)
                spawnChanceLimit = SPAWN_CHANCE10;
            else
                spawnChanceLimit = SPAWN_CHANCE;

            if (spawnChance >= spawnChanceLimit)
                return;

            int rnd = rand.Next(11);

            PowerUp.PowerUpType type = PowerUp.PowerUpType.LowBonusScore;
            Rectangle initialFrame = new Rectangle(0, 0, 30, 30);

            switch(rnd)
            {
                case 0:
                case 1:
                    type = PowerUp.PowerUpType.LowBonusScore;
                    initialFrame = new Rectangle(0, 0, 30, 30);
                    break;

                case 2:
                case 3:
                    type = PowerUp.PowerUpType.MediumBonusScore;
                    initialFrame = new Rectangle(0, 30, 30, 30);
                    break;

                case 4:
                    type = PowerUp.PowerUpType.HighBonusScore;
                    initialFrame = new Rectangle(0, 60, 30, 30);
                    break;

                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    type = PowerUp.PowerUpType.Upgrade;
                    initialFrame = new Rectangle(0, 90, 30, 30);
                    break;
            }

            PowerUp p = new PowerUp(texture,
                                    location - new Vector2(15, 15),
                                    initialFrame,
                                    type);

            powerUps.Add(p);
        }

        public void ProbablySpawnPowerUpWithHighChance(Vector2 location)
        {
            int spawnChance = rand.Next(100);

            if (spawnChance >= HIGH_SPAWN_CHANCE)
                return;

            int rnd = rand.Next(9);

            PowerUp.PowerUpType type = PowerUp.PowerUpType.LowBonusScore;
            Rectangle initialFrame = new Rectangle(0, 0, 30, 30);

            switch (rnd)
            {
                case 0:
                    type = PowerUp.PowerUpType.LowBonusScore;
                    initialFrame = new Rectangle(0, 0, 30, 30);
                    break;

                case 1:
                    type = PowerUp.PowerUpType.MediumBonusScore;
                    initialFrame = new Rectangle(0, 30, 30, 30);
                    break;

                case 2:
                    type = PowerUp.PowerUpType.HighBonusScore;
                    initialFrame = new Rectangle(0, 60, 30, 30);
                    break;

                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    type = PowerUp.PowerUpType.Upgrade;
                    initialFrame = new Rectangle(0, 90, 30, 30);
                    break;
            }

            PowerUp p = new PowerUp(texture,
                                    location - new Vector2(15, 15),
                                    initialFrame,
                                    type);

            powerUps.Add(p);
        }

        public void SpawnUpgradePowerUp(Vector2 location)
        {
            PowerUp.PowerUpType type = PowerUp.PowerUpType.Upgrade;
            Rectangle initialFrame = new Rectangle(0, 90, 30, 30);

            PowerUp p = new PowerUp(texture,
                                    location - new Vector2(15, 15),
                                    initialFrame,
                                    type);
            powerUps.Add(p);
        }

        public void Reset()
        {
            this.PowerUps.Clear();
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int x = powerUps.Count - 1; x >= 0; --x)
            {
                powerUps[x].Update(gameTime);

                if (!powerUps[x].IsActive)
                {
                    powerUps.RemoveAt(x);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var powerUp in powerUps)
            {
                powerUp.Draw(spriteBatch);
            }
        }

        private Rectangle getInitialFrameByType(PowerUp.PowerUpType type)
        {
            switch (type)
            {
                case PowerUp.PowerUpType.Upgrade:
                    return new Rectangle(0, 90, 30, 30);
                case PowerUp.PowerUpType.LowBonusScore:
                    return new Rectangle(0, 0, 30, 30);
                case PowerUp.PowerUpType.MediumBonusScore:
                    return new Rectangle(0, 30, 30, 30);
                case PowerUp.PowerUpType.HighBonusScore:
                    return new Rectangle(0, 60, 30, 30);
                default:
                    return new Rectangle(0, 0, 30, 30);
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Powerups
            int powerUpsCount = Int32.Parse(reader.ReadLine());
            
            powerUps.Clear();

            for (int i = 0; i < powerUpsCount; ++i)
            {
                PowerUp.PowerUpType type = PowerUp.PowerUpType.LowBonusScore;
                type = (PowerUp.PowerUpType)Enum.Parse(type.GetType(), reader.ReadLine(), false);
                PowerUp p = new PowerUp(texture,
                                    Vector2.Zero,
                                    getInitialFrameByType(type),
                                    type);
                p.Activated(reader);
                powerUps.Add(p);
            }
        }

        public void Deactivated(StreamWriter writer)
        {
            // Powerups
            writer.WriteLine(powerUps.Count);

            for (int i = 0; i < powerUps.Count; ++i)
            {
                writer.WriteLine(powerUps[i].Type);
                powerUps[i].Deactivated(writer);
            }
        }

        #endregion

        #region Properties

        public List<PowerUp> PowerUps
        {
            get
            {
                return powerUps;
            }
        }

        #endregion
    }
}
