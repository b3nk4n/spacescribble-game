using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SpaceScribble
{
    class CollisionManager
    {
        #region Members

        private AsteroidManager asteroidManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private BossManager bossManager;
        private PowerUpManager powerUpManager;
        private Vector2 offScreen = new Vector2(-500, -500);
        private Vector2 shotToAsteroidImpact = new Vector2(0, -20);

        private const string INFO_BONUSCREDIT_LOW = "1000 $";
        private const string INFO_BONUSCREDIT_MEDIUM = "2000 $";
        private const string INFO_BONUSCREDIT_HIGH = "3000 $";

        private const long LOW_CREDITS = 1000;
        private const long MEDIUM_CREDITS = 2000;
        private const long HIGH_CREDITS = 3000;

        private const float VELOCITY_TO_PLAY_ASTEROID_SOUND = 150.0f;

        Random rand = new Random();

        #endregion

        #region Constructors

        public CollisionManager(AsteroidManager asteroidManager, PlayerManager playerManager,
                                EnemyManager enemyManager, BossManager bossManager,
                                PowerUpManager powerUpManager)
        {
            this.asteroidManager = asteroidManager;
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
            this.bossManager = bossManager;
            this.powerUpManager = powerUpManager;
        }

        #endregion

        #region Methods

        private void checkShotToEnemyCollisions()
        {
            List<Enemy> enemies = enemyManager.Enemies;

            for (int i = 0; i < enemies.Count; ++i)
            {
                Enemy enemy = enemies[i];

                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                List<Sprite> shots = playerManager.PlayerShotManager.Shots;

                for (int j = 0; j < shots.Count; ++j)
                {
                    Sprite shot = shots[j];

                    if (shot.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.HitPoints -= playerManager.ShotPower;

                        location = shot.Location;
                        velocity = shot.Velocity;

                        shot.Location = offScreen;
                        
                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }

                if (location != Vector2.Zero)
                {
                    if (enemy.IsDestroyed)
                    {
                        playerManager.IncreasePlayerScore(enemy.KillScore);

                        EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                        enemy.EnemySprite.Velocity / 10);

                        powerUpManager.ProbablySpawnPowerUp(enemy.EnemySprite.Center);
                    }
                    else
                    {
                        playerManager.IncreasePlayerScore(enemy.HitScore);

                        EffectManager.AddLargeSparksEffect(location,
                                                      velocity,
                                                      -velocity,
                                                      Color.DarkGray);
                    }
                }
            }
        }

        private void checkShotToHarakiriCollisions()
        {
            List<HarakiriEnemy> enemies = enemyManager.Harakiries;

            for (int i = 0; i < enemies.Count; ++i)
            {
                HarakiriEnemy enemy = enemies[i];

                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                List<Sprite> shots = playerManager.PlayerShotManager.Shots;

                for (int j = 0; j < shots.Count; ++j)
                {
                    Sprite shot = shots[j];

                    if (shot.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.HitPoints -= playerManager.ShotPower;

                        location = shot.Location;
                        velocity = shot.Velocity;

                        shot.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }

                if (location != Vector2.Zero)
                {
                    if (enemy.IsDestroyed)
                    {
                        playerManager.IncreasePlayerScore(enemy.KillScore);

                        EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                        enemy.EnemySprite.Velocity / 10);

                        powerUpManager.ProbablySpawnPowerUpWithHighChance(enemy.EnemySprite.Center);
                    }
                    else
                    {
                        playerManager.IncreasePlayerScore(enemy.HitScore);

                        EffectManager.AddLargeSparksEffect(location,
                                                      velocity,
                                                      -velocity,
                                                      Color.DarkGray);
                    }
                }
            }
        }

        private void checkShotToBossCollisions()
        {
            List<Boss> bosses = bossManager.Bosses;

            for (int i = 0; i < bosses.Count; ++i)
            {
                Boss boss = bosses[i];

                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                List<Sprite> shots = playerManager.PlayerShotManager.Shots;

                for (int j = 0; j < shots.Count; ++j)
                {
                    Sprite shot = shots[j];

                    if (shot.IsCircleColliding(boss.BossSprite.Center,
                                               boss.BossSprite.CollisionRadius) &&
                        !boss.IsDestroyed)
                    {
                        boss.HitPoints -= playerManager.ShotPower;

                        location = shot.Location;
                        velocity = shot.Velocity;

                        shot.Location = offScreen;
                        
                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }    
                }

                if (location != Vector2.Zero)
                {
                    if (boss.IsDestroyed)
                    {
                        playerManager.IncreasePlayerScore(boss.KillScore);

                        EffectManager.AddBossExplosion(boss.BossSprite.Center,
                                                        boss.BossSprite.Velocity / 10);

                        powerUpManager.SpawnUpgradePowerUp(boss.BossSprite.Center);
                    }
                    else
                    {
                        playerManager.IncreasePlayerScore(boss.HitScore);

                        EffectManager.AddLargeSparksEffect(location,
                                                           velocity,
                                                           -velocity,
                                                           Color.DarkGray);
                    }
                }
            }
        }

        private void checkRocketToEnemyCollisions()
        {
            List<Sprite> rockets = playerManager.PlayerShotManager.Rockets;

            for (int i = 0; i < rockets.Count; ++i)
            {
                Sprite rocket = rockets[i];

                List<Enemy> enemies = enemyManager.Enemies;

                for (int j = 0; j < enemies.Count; ++j)
                {
                    Enemy enemy = enemies[j];

                    if (rocket.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.HitPoints = 0;

                        playerManager.IncreasePlayerScore(enemy.KillScore);

                        EffectManager.AddRocketExplosion(enemy.EnemySprite.Center,
                                                         enemy.EnemySprite.Velocity / 10);

                        powerUpManager.ProbablySpawnPowerUp(enemy.EnemySprite.Center);

                        List<Enemy> otherEnemies = enemyManager.Enemies;

                        for (int k = 0; k < otherEnemies.Count; ++k)
                        {
                            Enemy otherEnemy = otherEnemies[k];

                            if (enemy != otherEnemy)
                            {
                                float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                                if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                    !otherEnemies[k].IsDestroyed)
                                {
                                    float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                    float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                    otherEnemy.HitPoints -= damage;

                                    if (otherEnemy.IsDestroyed)
                                    {
                                        playerManager.IncreasePlayerScore(otherEnemy.KillScore);

                                        EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                        otherEnemy.EnemySprite.Velocity / 10);

                                        powerUpManager.ProbablySpawnPowerUp(otherEnemy.EnemySprite.Center);
                                    }
                                    else
                                    {
                                        playerManager.IncreasePlayerScore(otherEnemy.HitScore);
                                    }
                                }
                            }
                        }

                        List<HarakiriEnemy> otherHarakiri = enemyManager.Harakiries;

                        for (int k = 0; k < otherHarakiri.Count; ++k)
                        {
                            HarakiriEnemy otherEnemy = otherHarakiri[k];

                            float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                            if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                !otherEnemies[k].IsDestroyed)
                            {
                                float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                otherEnemy.HitPoints -= damage;

                                if (otherEnemy.IsDestroyed)
                                {
                                    playerManager.IncreasePlayerScore(otherEnemy.KillScore);

                                    EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                    otherEnemy.EnemySprite.Velocity / 10);

                                    powerUpManager.ProbablySpawnPowerUpWithHighChance(otherEnemy.EnemySprite.Center);
                                }
                                else
                                {
                                    playerManager.IncreasePlayerScore(otherEnemy.HitScore);
                                }
                            }
                        }

                        rocket.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkRocketToHarakiriCollisions()
        {
            List<Sprite> rockets = playerManager.PlayerShotManager.Rockets;

            for (int i = 0; i < rockets.Count; ++i)
            {
                Sprite rocket = rockets[i];

                List<HarakiriEnemy> enemies = enemyManager.Harakiries;

                for (int j = 0; j < enemies.Count; ++j)
                {
                    HarakiriEnemy enemy = enemies[j];

                    if (rocket.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.HitPoints = 0;

                        playerManager.IncreasePlayerScore(enemy.KillScore);

                        EffectManager.AddRocketExplosion(enemy.EnemySprite.Center,
                                                         enemy.EnemySprite.Velocity / 10);

                        powerUpManager.ProbablySpawnPowerUpWithHighChance(enemy.EnemySprite.Center);

                        List<Enemy> otherEnemies = enemyManager.Enemies;

                        for (int k = 0; k < otherEnemies.Count; ++k)
                        {
                            Enemy otherEnemy = otherEnemies[k];

                            float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                            if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                !otherEnemy.IsDestroyed)
                            {
                                float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount /  PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                otherEnemy.HitPoints -= damage;

                                if (otherEnemy.IsDestroyed)
                                {
                                    playerManager.IncreasePlayerScore(otherEnemy.KillScore);

                                    EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                    otherEnemy.EnemySprite.Velocity / 10);

                                    powerUpManager.ProbablySpawnPowerUpWithHighChance(otherEnemy.EnemySprite.Center);
                                }
                                else
                                {
                                    playerManager.IncreasePlayerScore(otherEnemy.HitScore);
                                }
                            }  
                        }

                        rocket.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkRocketToBossCollisions()
        {
            List<Sprite> rockets = playerManager.PlayerShotManager.Rockets;

            for (int i = 0; i < rockets.Count; ++i)
            {
                Sprite rocket = rockets[i];

                List<Boss> bosses = bossManager.Bosses;

                for (int j = 0; j < bosses.Count; ++j)
                {
                    Boss boss = bosses[j];

                    if (rocket.IsCircleColliding(boss.BossSprite.Center,
                                               boss.BossSprite.CollisionRadius) &&
                        !boss.IsDestroyed)
                    {
                        boss.HitPoints -= PlayerManager.ROCKET_POWER_AT_CENTER;

                        if (boss.IsDestroyed)
                        {
                            playerManager.IncreasePlayerScore(boss.KillScore);

                            powerUpManager.SpawnUpgradePowerUp(boss.BossSprite.Center);

                            EffectManager.AddBossExplosion(boss.BossSprite.Center,
                                                           boss.BossSprite.Velocity / 10);
                        }
                        else
                        {
                            playerManager.IncreasePlayerScore(boss.HitScore * 6);

                            EffectManager.AddRocketExplosion(rocket.Center,
                                                             boss.BossSprite.Velocity / 10);
                        }

                        rocket.Location = offScreen;
                    }
                }
            }
        }

        private void checkRocketToAsteroidCollisions(List<DestroyableAsteroid> asteroids)
        {
            List<Sprite> rockets = playerManager.PlayerShotManager.Rockets;

            for (int i = 0; i < rockets.Count; ++i)
            {
                Sprite rocket = rockets[i];

                for (int j = 0; j < asteroids.Count; ++j)
                {
                    DestroyableAsteroid asteroid = asteroids[j];

                    if (rocket.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddRocketExplosion(asteroid.Center,
                                                         asteroid.Velocity / 10);

                        List<Enemy> otherEnemies = enemyManager.Enemies;

                        for (int k = 0; k < otherEnemies.Count; ++k)
                        {
                            Enemy otherEnemy = otherEnemies[k];

                            float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                            if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                !otherEnemy.IsDestroyed)
                            {
                                float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                otherEnemy.HitPoints -= damage;

                                if (otherEnemy.IsDestroyed)
                                {
                                    playerManager.IncreasePlayerScore(otherEnemy.KillScore);

                                    EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                    otherEnemy.EnemySprite.Velocity / 10);

                                    powerUpManager.ProbablySpawnPowerUp(otherEnemy.EnemySprite.Center);
                                }
                                else
                                {
                                    playerManager.IncreasePlayerScore(otherEnemy.HitScore);
                                }
                            }
                        }

                        List<HarakiriEnemy> otherHarakiries = enemyManager.Harakiries;

                        for (int k = 0; k < otherHarakiries.Count; ++k)
                        {
                            HarakiriEnemy otherHarakiri = otherHarakiries[k];

                            float distance = Math.Abs((rocket.Center - otherHarakiri.EnemySprite.Center).Length());

                            if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                !otherHarakiri.IsDestroyed)
                            {
                                float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                otherHarakiri.HitPoints -= damage;

                                if (otherHarakiri.IsDestroyed)
                                {
                                    playerManager.IncreasePlayerScore(otherHarakiri.KillScore);

                                    EffectManager.AddLargeExplosion(otherHarakiri.EnemySprite.Center,
                                                                    otherHarakiri.EnemySprite.Velocity / 10);

                                    powerUpManager.ProbablySpawnPowerUp(otherHarakiri.EnemySprite.Center);
                                }
                                else
                                {
                                    playerManager.IncreasePlayerScore(otherHarakiri.HitScore);
                                }
                            }
                        }

                        List<Boss> otherBosses = bossManager.Bosses;

                        for (int k = 0; k < otherBosses.Count; ++k)
                        {
                            Boss otherBoss = otherBosses[k];

                            float distance = Math.Abs((rocket.Center - otherBoss.BossSprite.Center).Length());

                            if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                !otherBoss.IsDestroyed)
                            {
                                float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                otherBoss.HitPoints -= damage;

                                if (otherBoss.IsDestroyed)
                                {
                                    playerManager.IncreasePlayerScore(otherBoss.KillScore);

                                    EffectManager.AddLargeExplosion(otherBoss.BossSprite.Center,
                                                                    otherBoss.BossSprite.Velocity / 10);

                                    powerUpManager.ProbablySpawnPowerUp(otherBoss.BossSprite.Center);
                                }
                                else
                                {
                                    playerManager.IncreasePlayerScore(otherBoss.HitScore);
                                }
                            }

                        }

                        rocket.Location = offScreen;
                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkSonicToEnemyCollisions()
        {
            List<Enemy> enemies = enemyManager.Enemies;

            for (int i = 0; i < enemies.Count; ++i)
            {
                Enemy enemy = enemies[i];

                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                List<Sprite> sonics = playerManager.PlayerShotManager.Sonic;

                for (int j = 0; j < sonics.Count; ++j)
                {
                    Sprite sonic = sonics[j];

                    if (sonic.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.HitPoints -= PlayerManager.SONIC_SHOT_POWER;

                        location = sonic.Location;
                        velocity = sonic.Velocity;
                    }
                }

                if (location != Vector2.Zero)
                {
                    if (enemy.IsDestroyed)
                    {
                        playerManager.IncreasePlayerScore(enemy.KillScore);

                        EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                        enemy.EnemySprite.Velocity / 10);

                        powerUpManager.ProbablySpawnPowerUp(enemy.EnemySprite.Center);
                    }
                    else
                    {
                        playerManager.IncreasePlayerScore(enemy.HitScore / 10);
                    }
                }
            }
        }

        private void checkSonicToHarakiriCollisions()
        {
            List<HarakiriEnemy> enemies = enemyManager.Harakiries;

            for (int i = 0; i < enemies.Count; ++i)
            {
                HarakiriEnemy enemy = enemies[i];

                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                List<Sprite> sonics = playerManager.PlayerShotManager.Sonic;

                for (int j = 0; j < sonics.Count; ++j)
                {
                    Sprite sonic = sonics[j];

                    if (sonic.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.HitPoints -= PlayerManager.SONIC_SHOT_POWER;

                        location = sonic.Location;
                        velocity = sonic.Velocity;
                    }
                }

                if (location != Vector2.Zero)
                {
                    if (enemy.IsDestroyed)
                    {
                        playerManager.IncreasePlayerScore(enemy.KillScore);

                        EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                        enemy.EnemySprite.Velocity / 10);

                        powerUpManager.ProbablySpawnPowerUpWithHighChance(enemy.EnemySprite.Center);
                    }
                    else
                    {
                        playerManager.IncreasePlayerScore(enemy.HitScore / 10);
                    }
                }
            }
        }

        private void checkSonicToBossCollisions()
        {
            List<Boss> bosses = bossManager.Bosses;

            for (int i = 0; i < bosses.Count; ++i)
            {
                Boss boss = bosses[i];

                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                List<Sprite> sonics = playerManager.PlayerShotManager.Sonic;

                for (int j = 0; j < sonics.Count; ++j)
                {
                    Sprite sonic = sonics[j];

                    if (sonic.IsCircleColliding(boss.BossSprite.Center,
                                               boss.BossSprite.CollisionRadius) &&
                        !boss.IsDestroyed)
                    {
                        boss.HitPoints -= PlayerManager.SONIC_SHOT_POWER;

                        location = sonic.Location;
                        velocity = sonic.Velocity;
                    }
                }

                if (location != Vector2.Zero)
                {
                    if (boss.IsDestroyed)
                    {
                        playerManager.IncreasePlayerScore(boss.KillScore);

                        EffectManager.AddBossExplosion(boss.BossSprite.Center,
                                                        boss.BossSprite.Velocity / 10);

                        powerUpManager.SpawnUpgradePowerUp(boss.BossSprite.Center);
                    }
                    else
                    {
                        playerManager.IncreasePlayerScore(boss.HitScore / 10);
                    }
                }
            }
        }

        private void checkEnemyRocketToPlayerCollisions()
        {
            List<Sprite> rockets = enemyManager.EnemyShotManager.Rockets;

            for (int i = 0; i < rockets.Count; ++i)
            {
                Sprite rocket = rockets[i];

                if (rocket.IsCircleColliding(playerManager.playerSprite.Center,
                                             playerManager.playerSprite.CollisionRadius) &&
                    !playerManager.IsDestroyed)
                {
                    playerManager.DecreaseHitPoints(rand.Next(EnemyManager.DAMAGE_LASER_MIN * 2,
                                                              EnemyManager.DAMAGE_LASER_MAX * 3 + 1),
                                                    true);

                    EffectManager.AddRocketExplosion(playerManager.playerSprite.Center,
                                                     playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.3f);

                    List<Enemy> otherEnemies = enemyManager.Enemies;

                    for (int j = 0; j < otherEnemies.Count; ++j)
                    {
                        Enemy otherEnemy = otherEnemies[j];

                        float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                        if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                            !otherEnemy.IsDestroyed)
                        {
                            float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                            float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                            otherEnemy.HitPoints -= damage;

                            if (otherEnemy.IsDestroyed)
                            {
                                EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                otherEnemy.EnemySprite.Velocity / 10);
                            }
                        }
                    }

                    rocket.Location = offScreen;
                }
            }
        }

        private void checkBossRocketToPlayerCollisions()
        {
            List<Sprite> rockets = bossManager.BossShotManager.Rockets;

            for (int i = 0; i < rockets.Count; ++i)
            {
                Sprite rocket = rockets[i];

                if (rocket.IsCircleColliding(playerManager.playerSprite.Center,
                                             playerManager.playerSprite.CollisionRadius) &&
                    !playerManager.IsDestroyed)
                {
                    playerManager.DecreaseHitPoints(rand.Next(BossManager.DAMAGE_LASER_MIN * 2,
                                                              BossManager.DAMAGE_LASER_MAX * 3 + 1),
                                                     true);

                    EffectManager.AddRocketExplosion(playerManager.playerSprite.Center,
                                                     playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.3f);

                    rocket.Location = offScreen;
                }
            }
        }

        private void checkEnemyRocketToAsteroidCollisions(List<DestroyableAsteroid> asteroids)
        {
            List<Sprite> rockets = enemyManager.EnemyShotManager.Rockets;

            for (int i = 0; i < rockets.Count; ++i)
            {
                Sprite rocket = rockets[i];

                for (int j = 0; j < asteroids.Count; ++j)
                {
                    DestroyableAsteroid asteroid = asteroids[j];

                    if (rocket.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddRocketExplosion(asteroid.Center,
                                                         asteroid.Velocity / 10);


                        List<Enemy> otherEnemies = enemyManager.Enemies;

                        for (int k = 0; k < otherEnemies.Count; ++k)
                        {
                            Enemy otherEnemy = otherEnemies[k];

                            float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                            if (distance < EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS &&
                                !otherEnemy.IsDestroyed)
                            {
                                float distAmount = Math.Max(0, EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = EnemyManager.ROCKET_POWER_AT_CENTER * (distAmount / EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS);

                                otherEnemy.HitPoints -= damage;

                                if (otherEnemy.IsDestroyed)
                                {
                                    EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                    otherEnemy.EnemySprite.Velocity / 10);
                                }
                            }
                        }

                        float distance2 = Math.Abs((rocket.Center - playerManager.playerSprite.Center).Length());

                        if (distance2 < EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS &&
                            !playerManager.IsDestroyed)
                        {
                            float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance2);

                            float damage = EnemyManager.ROCKET_POWER_AT_CENTER * (distAmount / EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS);

                            playerManager.DecreaseHitPoints(damage,
                                                            true);

                            VibrationManager.Vibrate(0.2f);

                            if (playerManager.IsDestroyed)
                            {
                                EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                                playerManager.playerSprite.Velocity / 10);
                            }
                        }

                        rocket.Location = offScreen;
                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkBossRocketToAsteroidCollisions(List<DestroyableAsteroid> asteroids)
        {
            List<Sprite> rockets = bossManager.BossShotManager.Rockets;

            for (int i = 0; i < rockets.Count; ++i)
            {
                Sprite rocket = rockets[i];

                for (int j = 0; j < asteroids.Count; ++j)
                {
                    DestroyableAsteroid asteroid = asteroids[j];

                    if (rocket.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddRocketExplosion(asteroid.Center,
                                                         asteroid.Velocity / 10);

                        float distance2 = Math.Abs((rocket.Center - playerManager.playerSprite.Center).Length());

                        if (distance2 < BossManager.SOFT_ROCKET_EXPLOSION_RADIUS &&
                            !playerManager.IsDestroyed)
                        {
                            float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance2);

                            float damage = BossManager.ROCKET_POWER_AT_CENTER * (distAmount / BossManager.SOFT_ROCKET_EXPLOSION_RADIUS);

                            playerManager.DecreaseHitPoints(damage,
                                                            true);

                            VibrationManager.Vibrate(0.2f);

                            if (playerManager.IsDestroyed)
                            {
                                EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                                playerManager.playerSprite.Velocity / 10);
                            }
                        }

                        rocket.Location = offScreen;
                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkPlayerShotToAsteroidCollisions(List<DestroyableAsteroid> asteroids)
        {
            List<Sprite> shots = playerManager.PlayerShotManager.Shots;

            for (int i = 0; i < shots.Count; ++i)
            {
                Sprite shot = shots[i];

                for (int j = 0; j < asteroids.Count; ++j)
                {
                    DestroyableAsteroid asteroid = asteroids[j];

                    if (!asteroid.IsDestroyed && shot.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddSparksEffect(shot.Location,
                                                      shot.Velocity,
                                                      asteroid.Velocity,
                                                      Color.Gray,
                                                      true);
                        shot.Location = offScreen;
                        Vector2 direction = shot.Velocity;
                        direction.Normalize();
                        direction *= 20;
                        asteroid.Velocity += direction;

                        asteroid.DecreaseRemainingSustainingHits();

                        if (asteroid.IsDestroyed)
                        {
                            EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10,
                                                   true);
                            asteroid.Location = offScreen;
                        }

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkEnemyShotToAsteroidCollisions(List<DestroyableAsteroid> asteroids)
        {
            List<Sprite> shots = enemyManager.EnemyShotManager.Shots;

            for (int i = 0; i < shots.Count; ++i)
            {
                Sprite shot = shots[i];

                for (int j = 0; j < asteroids.Count; ++j)
                {
                    DestroyableAsteroid asteroid = asteroids[j];

                    if (!asteroid.IsDestroyed && shot.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddSparksEffect(shot.Location,
                                                      shot.Velocity,
                                                      asteroid.Velocity,
                                                      Color.Gray,
                                                      true);
                        shot.Location = offScreen;
                        Vector2 direction = shot.Velocity;
                        direction.Normalize();
                        direction *= 20;
                        asteroid.Velocity += direction;

                        asteroid.DecreaseRemainingSustainingHits();

                        if (asteroid.IsDestroyed)
                        {
                            EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10,
                                                   true);

                            asteroid.Location = offScreen;
                        }

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkBossShotToAsteroidCollisions(List<DestroyableAsteroid> asteroids)
        {
            List<Sprite> shots = bossManager.BossShotManager.Shots;

            for (int i = 0; i < shots.Count; ++i)
            {
                Sprite shot = shots[i];

                for (int j = 0; j < asteroids.Count; ++j)
                {
                    DestroyableAsteroid asteroid = asteroids[j];

                    if (shot.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddSparksEffect(shot.Location,
                                                      shot.Velocity,
                                                      asteroid.Velocity,
                                                      Color.Gray,
                                                      true);
                        shot.Location = offScreen;
                        Vector2 direction = shot.Velocity;
                        direction.Normalize();
                        direction *= 20;
                        asteroid.Velocity += direction;

                        asteroid.DecreaseRemainingSustainingHits();

                        if (asteroid.IsDestroyed)
                        {
                            EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10,
                                                   true);
                            asteroid.Location = offScreen;
                        }

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkEnemyShotToPlayerCollisions()
        {
            List<Sprite> shots = enemyManager.EnemyShotManager.Shots;

            for (int i = 0; i < shots.Count; ++i)
            {
                Sprite shot = shots[i];

                if (!playerManager.IsDestroyed &&
                    shot.IsCircleColliding(playerManager.playerSprite.Center,
                                           playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.DecreaseHitPoints(rand.Next(EnemyManager.DAMAGE_LASER_MIN,
                                                              EnemyManager.DAMAGE_LASER_MAX + 1),
                                                    true);

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.5f);
                    }
                    else
                    {
                        EffectManager.AddExplosion(playerManager.playerSprite.Center,
                                                   playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.2f);
                    }
                    
                    shot.Location = offScreen;
                }
            }
        }

        private void checkBossShotToPlayerCollisions()
        {
            List<Sprite> shots = bossManager.BossShotManager.Shots;

            for (int i = 0; i < shots.Count; ++i)
            {
                Sprite shot = shots[i];

                if (!playerManager.IsDestroyed &&
                    shot.IsCircleColliding(playerManager.playerSprite.Center,
                                           playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.DecreaseHitPoints(rand.Next(BossManager.DAMAGE_LASER_MIN,
                                                              BossManager.DAMAGE_LASER_MAX + 1),
                                                    true);

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.5f);
                    }
                    else
                    {
                        EffectManager.AddExplosion(playerManager.playerSprite.Center,
                                                   playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.2f);
                    }

                    shot.Location = offScreen;
                }
            }
        }

        private void checkEnemyToPlayerCollisions()
        {
            List<Enemy> enemies = enemyManager.Enemies;

            for (int i = 0; i < enemies.Count; ++i)
            {
                Enemy enemy = enemies[i];

                if (!playerManager.IsDestroyed &&
                    enemy.EnemySprite.IsCircleColliding(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.CollisionRadius))
                {
                    enemy.HitPoints -= Math.Max(enemy.HitPoints, 99.0f);

                    EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                    enemy.EnemySprite.Velocity / 10);

                    playerManager.DecreaseHitPoints(PlayerManager.CRASH_TO_ENEMY,
                                                    false);
                    EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                    playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.5f);
                }
            }
        }

        private void checkHarakiriToPlayerCollisions()
        {
            List<HarakiriEnemy> enemies = enemyManager.Harakiries;

            for (int i = 0; i < enemies.Count; ++i)
            {
                HarakiriEnemy enemy = enemies[i];

                if (!playerManager.IsDestroyed &&
                    enemy.EnemySprite.IsCircleColliding(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.CollisionRadius))
                {
                    enemy.HitPoints -= Math.Max(enemy.HitPoints, 99.0f);

                    EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                    enemy.EnemySprite.Velocity / 10);

                    playerManager.DecreaseHitPoints(PlayerManager.CRASH_TO_ENEMY,
                                                    false);
                    EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                    playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.5f);
                }
            }
        }

        private void checkBossToPlayerCollisions()
        {
            List<Boss> bosses = bossManager.Bosses;

            for (int i = 0; i < bosses.Count; ++i)
            {
                Boss boss = bosses[i];

                if (!playerManager.IsDestroyed &&
                    boss.BossSprite.IsCircleColliding(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.Kill();
                    EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                    playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.5f);
                }
            }
        }

        private void checkAsteroidToPlayerCollisions(List<DestroyableAsteroid> asteroids)
        {
            for (int i = 0; i < asteroids.Count; ++i)
            {
                DestroyableAsteroid asteroid = asteroids[i];

                if (!playerManager.IsDestroyed &&
                    asteroid.IsCircleColliding(playerManager.playerSprite.Center,
                                               playerManager.playerSprite.CollisionRadius))
                {
                    EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10,
                                                   true);

                    playerManager.DecreaseHitPoints(rand.Next(AsteroidManager.CRASH_POWER_MIN,
                                                              AsteroidManager.CRASH_POWER_MAX + 1),
                                                    false);

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.5f);
                    }
                    else
                    {
                        EffectManager.AddExplosion(playerManager.playerSprite.Center,
                                                   playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.2f);
                    }

                    
                    
                    asteroid.Location = offScreen;
                }
                
                // check for asteroid sound:
                if (!asteroid.AsteroidSoundPlayed)
                    {
                        if (asteroid.isBoxColliding(new Rectangle(0, 0, 480, 800))
                            && asteroid.Velocity.Length() > VELOCITY_TO_PLAY_ASTEROID_SOUND)
                        {
                            asteroid.PlayAsteroidSound(MathHelper.Clamp(asteroid.Velocity.Length() / AsteroidManager.ABSOLUTE_MAX_SPEED, 0.0f, 1.0f));
                        }
                    }
            }
        }

        private void checkAsteroidToEnemiesCollisions(List<DestroyableAsteroid> asteroids)
        {
            for (int i = 0; i < asteroids.Count; ++i)
            {
                DestroyableAsteroid asteroid = asteroids[i];

                List<Enemy> enemies = enemyManager.Enemies;

                for (int j = 0; j < enemies.Count; ++j)
                {
                    Enemy enemy = enemies[j];

                    if (asteroid.IsCircleColliding(enemy.EnemySprite.Center,
                                                   enemy.EnemySprite.CollisionRadius))
                    {
                        EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10,
                                                   true);

                        enemy.HitPoints -= rand.Next(AsteroidManager.CRASH_POWER_MIN,
                                                     AsteroidManager.CRASH_POWER_MAX + 1);

                        EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                        enemy.EnemySprite.Velocity / 10);
                        
                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkAsteroidToHarakiriesCollisions(List<DestroyableAsteroid> asteroids)
        {
            for (int i = 0; i < asteroids.Count; ++i)
            {
                DestroyableAsteroid asteroid = asteroids[i];

                List<HarakiriEnemy> enemies = enemyManager.Harakiries;

                for (int j = 0; j < enemies.Count; ++j)
                {
                    HarakiriEnemy enemy = enemies[j];

                    if (asteroid.IsCircleColliding(enemy.EnemySprite.Center,
                                                   enemy.EnemySprite.CollisionRadius))
                    {
                        EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10,
                                                   true);

                        enemy.HitPoints -= rand.Next(AsteroidManager.CRASH_POWER_MIN,
                                                     AsteroidManager.CRASH_POWER_MAX + 1);

                        EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                        enemy.EnemySprite.Velocity / 10);

                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkAsteroidToBossesCollisions(List<DestroyableAsteroid> asteroids)
        {
            for (int i = 0; i < asteroids.Count; ++i)
            {
                DestroyableAsteroid asteroid = asteroids[i];

                List<Boss> bosses = bossManager.Bosses;

                for (int j = 0; j < bosses.Count; ++j)
                {
                    Boss boss = bosses[j];

                    if (asteroid.IsCircleColliding(boss.BossSprite.Center,
                                                   boss.BossSprite.CollisionRadius))
                    {
                        EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10,
                                                   true);

                        boss.HitPoints -= (rand.Next(AsteroidManager.CRASH_POWER_MIN,
                                                     AsteroidManager.CRASH_POWER_MAX + 1) / 5.0f);
                        if (boss.IsDestroyed)
                        {
                            EffectManager.AddBossExplosion(boss.BossSprite.Center,
                                                           boss.BossSprite.Velocity / 10);
                        }

                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkPowerUpToPlayerCollision()
        {
            List<PowerUp> powerUps = powerUpManager.PowerUps;

            for (int i = 0; i < powerUps.Count; ++i)
            {
                PowerUp powerUp = powerUps[i];

                if (powerUp.isCircleColliding(playerManager.playerSprite.Center, playerManager.playerSprite.CollisionRadius))
                {
                    // Activate power-up
                    switch (powerUp.Type)
                    {
                        case PowerUp.PowerUpType.LowBonusScore:
                            playerManager.IncreaseCredits(LOW_CREDITS);
                            SoundManager.PlayCoinSound();
                            ZoomTextManager.ShowCredit(INFO_BONUSCREDIT_LOW, powerUp.Center);
                            break;

                        case PowerUp.PowerUpType.MediumBonusScore:
                            playerManager.IncreaseCredits(MEDIUM_CREDITS);
                            SoundManager.PlayCoinSound();
                            ZoomTextManager.ShowCredit(INFO_BONUSCREDIT_MEDIUM, powerUp.Center);
                            break;

                        case PowerUp.PowerUpType.HighBonusScore:
                            playerManager.IncreaseCredits(HIGH_CREDITS);
                            SoundManager.PlayCoinSound();
                            ZoomTextManager.ShowCredit(INFO_BONUSCREDIT_HIGH, powerUp.Center);
                            break;

                        case PowerUp.PowerUpType.Upgrade:
                            playerManager.IncreaseUpgrades();
                            SoundManager.PlayUpgradeSound();
                            break;
                    }

                    powerUp.IsActive = false;
                }
            }
        }

        public void Update()
        {
            checkPlayerShotToAsteroidCollisions(asteroidManager.Asteroids);
            checkPlayerShotToAsteroidCollisions(asteroidManager.ShowerAsteroids);
            checkEnemyShotToAsteroidCollisions(asteroidManager.Asteroids);
            checkEnemyShotToAsteroidCollisions(asteroidManager.ShowerAsteroids);
            checkBossShotToAsteroidCollisions(asteroidManager.Asteroids);
            checkBossShotToAsteroidCollisions(asteroidManager.ShowerAsteroids);
            checkShotToEnemyCollisions();
            checkShotToHarakiriCollisions();
            checkShotToBossCollisions();
            checkRocketToEnemyCollisions();
            checkRocketToHarakiriCollisions();
            checkRocketToBossCollisions();
            checkRocketToAsteroidCollisions(asteroidManager.Asteroids);
            checkRocketToAsteroidCollisions(asteroidManager.ShowerAsteroids);
            checkAsteroidToEnemiesCollisions(asteroidManager.Asteroids);
            checkAsteroidToEnemiesCollisions(asteroidManager.ShowerAsteroids);
            checkAsteroidToHarakiriesCollisions(asteroidManager.Asteroids);
            checkAsteroidToHarakiriesCollisions(asteroidManager.ShowerAsteroids);
            checkAsteroidToBossesCollisions(asteroidManager.Asteroids);
            checkAsteroidToBossesCollisions(asteroidManager.ShowerAsteroids);
            checkEnemyRocketToAsteroidCollisions(asteroidManager.Asteroids);
            checkEnemyRocketToAsteroidCollisions(asteroidManager.ShowerAsteroids);
            checkBossRocketToAsteroidCollisions(asteroidManager.Asteroids);
            checkBossRocketToAsteroidCollisions(asteroidManager.ShowerAsteroids);
            checkEnemyRocketToPlayerCollisions();
            checkBossRocketToPlayerCollisions();
            checkSonicToEnemyCollisions();
            checkSonicToHarakiriCollisions();
            checkSonicToBossCollisions();

            if (!playerManager.IsDestroyed)
            {
                checkEnemyShotToPlayerCollisions();
                checkBossShotToPlayerCollisions();
                checkAsteroidToPlayerCollisions(asteroidManager.Asteroids);
                checkAsteroidToPlayerCollisions(asteroidManager.ShowerAsteroids);
                checkEnemyToPlayerCollisions();
                checkHarakiriToPlayerCollisions();
                checkBossToPlayerCollisions();
                checkPowerUpToPlayerCollision();
            }
        }

        #endregion
    }
}
