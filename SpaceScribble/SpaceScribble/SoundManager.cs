using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace SpaceScribble
{
    public static class SoundManager
    {
        #region Members

        private static SettingsManager settings;

        private static List<SoundEffect> explosions = new List<SoundEffect>();
        private static int explosionsCount = 4;

        private static SoundEffect playerShot;
        private static SoundEffect enemyShot;

        private static SoundEffect coinSound;
        private static SoundEffect repairSound;
        private static SoundEffect upgradeSound;
        private static SoundEffect extraSonicSound;
        private static SoundEffect rocketSound;
        private static SoundEffect upgradeActivatedSound;
        private static SoundEffect enemyRocketSound;
        private static SoundEffect shieldSound;

        private static SoundEffect bossEasySound;
        private static SoundEffect bossMediumSound;
        private static SoundEffect bossHardSound;
        private static SoundEffect bossSpeederSound;
        private static SoundEffect bossTankSound;

        private static SoundEffect startGameSound;

        private static List<SoundEffect> hitSounds = new List<SoundEffect>();
        private static int hitSoundsCount = 6;

        private static List<SoundEffect> asteroidHitSounds = new List<SoundEffect>();
        private static int asteroidHitSoundsCount = 5;

        private static List<SoundEffect> asteroidSounds = new List<SoundEffect>();
        private static int asteroidSoundsCount = 6;

        private static Random rand = new Random();

        private static Song backgroundSound;
        //private static SoundEffectInstance backgroundSound;

        private static SoundEffect selectSound;

        // performance improvements
        public const float MinTimeBetweenHitSound = 0.2f;
        private static float hitTimer = 0.0f;
        private static float asteroidHitTimer = 0.0f;

        public const float MinTimeBetweenExplosionSound = 0.1f;
        private static float explosionTimer = 0.0f;

        public const float MinTimeBetweenAsteroidSound = 0.2f;
        private static float asteroidTimer = 0.0f;

        #endregion

        #region Methods

        public static void Initialize(ContentManager content)
        {
            try
            {
                settings = SettingsManager.GetInstance();

                playerShot = content.Load<SoundEffect>(@"Sounds\Shot2");
                enemyShot = content.Load<SoundEffect>(@"Sounds\Shot1");

                repairSound = content.Load<SoundEffect>(@"Sounds\Repair");
                coinSound = content.Load<SoundEffect>(@"Sounds\Coin");
                upgradeSound = content.Load<SoundEffect>(@"Sounds\Upgrade");
                extraSonicSound = content.Load<SoundEffect>(@"Sounds\Sonic");
                rocketSound = content.Load<SoundEffect>(@"Sounds\Rocket");
                upgradeActivatedSound = content.Load<SoundEffect>(@"Sounds\UpgradeActivated");
                enemyRocketSound = content.Load<SoundEffect>(@"Sounds\EnemyRocket");
                shieldSound = content.Load<SoundEffect>(@"Sounds\Shield");

                bossEasySound = content.Load<SoundEffect>(@"Sounds\boss_easy");
                bossMediumSound = content.Load<SoundEffect>(@"Sounds\boss_medium");
                bossHardSound = content.Load<SoundEffect>(@"Sounds\boss_hard");
                bossSpeederSound = content.Load<SoundEffect>(@"Sounds\boss_speeder");
                bossTankSound = content.Load<SoundEffect>(@"Sounds\boss_tank");

                startGameSound = content.Load<SoundEffect>(@"Sounds\StartGame");

                for (int x = 1; x <= explosionsCount; x++)
                {
                    explosions.Add(content.Load<SoundEffect>(@"Sounds\Explosion"
                                                             + x.ToString()));
                }

                for (int x = 1; x <= hitSoundsCount; x++)
                {
                    hitSounds.Add(content.Load<SoundEffect>(@"Sounds\Hit"
                                                             + x.ToString()));
                }

                for (int x = 1; x <= asteroidHitSoundsCount; x++)
                {
                    asteroidHitSounds.Add(content.Load<SoundEffect>(@"Sounds\AsteroidHit"
                                                             + x.ToString()));
                }

                for (int x = 1; x <= asteroidSoundsCount; x++)
                {
                    asteroidSounds.Add(content.Load<SoundEffect>(@"Sounds\asteroid"
                                                                 + x.ToString()));
                }

                backgroundSound = content.Load<Song>(@"Sounds\GameSound");

                selectSound = content.Load<SoundEffect>(@"Sounds\Select");
            }
            catch
            {
                Debug.WriteLine("SoundManager: Content not found.");
            }
        }

        public static void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            SoundManager.hitTimer += elapsed;
            SoundManager.asteroidHitTimer += elapsed;
            SoundManager.explosionTimer += elapsed;
            SoundManager.asteroidTimer += elapsed;
        }

        public static void PlayExplosion()
        {
            if (SoundManager.explosionTimer > SoundManager.MinTimeBetweenExplosionSound)
            {
                try
                {
                    SoundEffectInstance s = explosions[rand.Next(0, explosionsCount)].CreateInstance();
                    s.Volume = settings.GetSfxValue();
                    s.Play();
                }
                catch
                {
                    Debug.WriteLine("SoundManager: Play explosion failed.");
                }

                SoundManager.explosionTimer = 0.0f;
            }
        }

        public static void PlayPlayerShot()
        {
            try
            {
                SoundEffectInstance s = playerShot.CreateInstance();
                s.Volume = 0.75f * settings.GetSfxValue() * 0.6f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play player shot failed.");
            }
        }

        public static void PlayEnemyShot()
        {
            try
            {
                SoundEffectInstance s = enemyShot.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.8f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayCoinSound()
        {
            try
            {
                SoundEffectInstance s = coinSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.7f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayRepairSound()
        {
            try
            {
                SoundEffectInstance s = repairSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play repait sound failed.");
            }
        }

        public static void PlayUpgradeSound()
        {
            try
            {
                SoundEffectInstance s = upgradeSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.7f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlaySonicSound()
        {
            try
            {
                SoundEffectInstance s = extraSonicSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayHitSound()
        {
            if (SoundManager.hitTimer > SoundManager.MinTimeBetweenHitSound)
            {
                try
                {
                    SoundEffectInstance s = hitSounds[rand.Next(0, hitSoundsCount)].CreateInstance();
                    s.Volume = settings.GetSfxValue();
                    s.Play();
                }
                catch
                {
                    Debug.WriteLine("SoundManager: Play explosion failed.");
                }

                SoundManager.hitTimer = 0.0f;
            }
        }

        public static void PlayAsteroidHitSound()
        {
            if (SoundManager.asteroidHitTimer > SoundManager.MinTimeBetweenHitSound)
            {
                try
                {
                    SoundEffectInstance s = asteroidHitSounds[rand.Next(0, asteroidHitSoundsCount)].CreateInstance();
                    s.Volume = settings.GetSfxValue();
                    s.Play();
                }
                catch
                {
                    Debug.WriteLine("SoundManager: Play explosion failed.");
                }

                SoundManager.asteroidHitTimer = 0.0f;
            }
        }

        public static void PlayRocketSound()
        {
            try
            {
                SoundEffectInstance s = rocketSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayUpgradeActivatedSound()
        {
            try
            {
                SoundEffectInstance s = upgradeActivatedSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayEnemyRocketSound()
        {
            try
            {
                SoundEffectInstance s = enemyRocketSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayShieldSound()
        {
            try
            {
                SoundEffectInstance s = shieldSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayBossEasySound()
        {
            try
            {
                SoundEffectInstance s = bossEasySound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayBossMediumSound()
        {
            try
            {
                SoundEffectInstance s = bossMediumSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayBossHardSound()
        {
            try
            {
                SoundEffectInstance s = bossHardSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayBossSpeederSound()
        {
            try
            {
                SoundEffectInstance s = bossSpeederSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayBossTankSound()
        {
            try
            {
                SoundEffectInstance s = bossTankSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayAsteroidSound(float factor)
        {
            if (SoundManager.asteroidTimer > SoundManager.MinTimeBetweenAsteroidSound)
            {
                try
                {
                    if (factor > 0.2f)
                    {
                        SoundEffectInstance s = asteroidSounds[rand.Next(0, asteroidSoundsCount)].CreateInstance();
                        s.Volume = settings.GetSfxValue() * 0.5f * factor;
                        s.Play();
                    }
                }
                catch
                {
                    Debug.WriteLine("SoundManager: Play asteroid sound failed.");
                }

                asteroidTimer = 0.0f;
            }
        }

        public static void PlaySelectSound()
        {
            try
            {
                SoundEffectInstance s = selectSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.4f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play select sound failed.");
            }
        }

        public static void PlayStartGameSound()
        {
            try
            {
                SoundEffectInstance s = startGameSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.8f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play select sound failed.");
            }
        }

        public static void PlayBackgroundSound()
        {
            try
            {
                if (MediaPlayer.GameHasControl)
                {
                    MediaPlayer.Play(backgroundSound);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = settings.GetMusicValue();
                }
            }
            catch (UnauthorizedAccessException)
            {
                // play no music...
            }
            catch (InvalidOperationException)
            {
                // play no music (because of Zune on PC)
            }
        }

        public static void RefreshMusicVolume()
        {
            float val = settings.GetMusicValue();
            MediaPlayer.Volume = val;
        }

        #endregion
    }
}
