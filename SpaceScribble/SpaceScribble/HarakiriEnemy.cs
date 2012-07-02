using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpaceScribble
{
    class HarakiriEnemy : ILevel
    {
        #region Members

        private Sprite enemySprite;
        private Vector2 gunOffset = new Vector2(25, 25);

        private float speed;

        private const int EnemyRadiusEasy = 14;
        private const int EnemyRadiusMedium = 20; // TODO: adjust radius
        private const int EnemyRadiusHard = 18;

        private const float InitialHitPoints = 200.0f;

        private readonly Rectangle EasySource = new Rectangle(0, 150,
                                                              50, 50);
        private readonly Rectangle MediumSource = new Rectangle(0, 200,
                                                                50, 50);
        private readonly Rectangle HardSource = new Rectangle(350, 150,
                                                              50, 50);
        private const int EasyFrameCount = 6;
        private const int MediumFrameCount = 6;
        private const int HardFrameCount = 6;

        private Vector2 previousCenter = Vector2.Zero;

        private float hitPoints;
        public float MaxHitPoints;

        EnemyType type;

        private readonly int initialHitScore;
        private int hitScore;
        private readonly int initialKillScore;
        private int killScore;

        private readonly float initialShotChance;
        private float shotChance;

        private Vector2 currentTarget;

        public const float ShotSpeed = 175.0f;

        public const float CampingOuterRadius = 275.0f;
        public const float CampingInnerRadius = 200.0f;
        private bool isCamper = false;

        Random rand = new Random();

        #endregion

        #region Constructors

        private HarakiriEnemy(Texture2D texture, Vector2 location,
                              float speed, int hitScore, int killScore, float shotChance,
                              int collisionRadius, EnemyType type)
        {
            Rectangle initialFrame;
            int frameCount = 0;

            switch (type)
            {
                case EnemyType.Easy:
                    initialFrame = EasySource;
                    frameCount = EasyFrameCount;
                    break;
                case EnemyType.Medium:
                    initialFrame = MediumSource;
                    frameCount = MediumFrameCount;
                    break;
                default:
                    initialFrame = HardSource;
                    frameCount = HardFrameCount;
                    break;
            }

            enemySprite = new Sprite(location,
                                     texture,
                                     initialFrame,
                                     Vector2.Zero);

            for (int x = 1; x < frameCount; x++)
            {
                EnemySprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
                previousCenter = location;
                EnemySprite.CollisionRadius = collisionRadius;
            }

            this.speed = speed;

            this.initialHitScore = hitScore;
            this.hitScore = hitScore;
            this.initialKillScore = killScore;
            this.killScore = killScore;

            this.initialShotChance = shotChance;
            this.shotChance = shotChance;

            this.type = type;

            if (type != EnemyType.Easy)
            {
                isCamper = true;
            }
        }

        #endregion

        #region Methods

        public static HarakiriEnemy CreateEasyEnemy(Texture2D texture, Vector2 location)
        {
            HarakiriEnemy enemy = new HarakiriEnemy(texture,
                                location,
                                90.0f,
                                100,
                                750,
                                0.2f,
                                EnemyRadiusEasy,
                                EnemyType.Easy);

            enemy.hitPoints = InitialHitPoints;
            enemy.MaxHitPoints = InitialHitPoints;

            return enemy;
        }

        public static HarakiriEnemy CreateMediumEnemy(Texture2D texture, Vector2 location)
        {
            HarakiriEnemy enemy = new HarakiriEnemy(texture,
                                location,
                                80.0f,
                                100,
                                750,
                                0.75f,
                                EnemyRadiusMedium,
                                EnemyType.Medium);

            enemy.hitPoints = InitialHitPoints;
            enemy.MaxHitPoints = InitialHitPoints;

            return enemy;
        }

        public static HarakiriEnemy CreateHardEnemy(Texture2D texture, Vector2 location)
        {
            HarakiriEnemy enemy = new HarakiriEnemy(texture,
                                location,
                                60.0f,
                                100,
                                1000,
                                0.5f,
                                EnemyRadiusHard,
                                EnemyType.Hard);

            enemy.hitPoints = InitialHitPoints;
            enemy.MaxHitPoints = InitialHitPoints;

            return enemy;
        }

        public bool TargetReached()
        {
            // No camping if the enemy is out of screen
            if (enemySprite.Location.X < -25 || enemySprite.Location.X > 505 || enemySprite.Location.Y < -25)
                return false;

            float campingOffset = isCamper ? CampingOuterRadius : 0;
            
            if (Vector2.Distance(EnemySprite.Center, currentTarget) <
                (float)EnemySprite.Source.Width / 2 + campingOffset)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsActive()
        {
            if (IsDestroyed)
            {
                return false;
            }

            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (IsActive())
            {
                Vector2 heading;

                // If there is no player then go straight forward
                if (currentTarget == Vector2.Zero)
                    heading = enemySprite.Velocity;
                else
                    heading = currentTarget - enemySprite.Center;

                if (heading != Vector2.Zero)
                {
                    heading.Normalize();
                }

                heading *= speed;
                enemySprite.Velocity = heading;
                
                if (TargetReached())
                {
                    enemySprite.Velocity *= (((currentTarget - enemySprite.Center).Length() - CampingInnerRadius) / (CampingOuterRadius - CampingInnerRadius));

                    if ((currentTarget - enemySprite.Center).Length() < CampingInnerRadius)
                    {
                        enemySprite.Velocity = Vector2.Zero;
                    }
                }

                if (!previousCenter.Equals(enemySprite.Center))
                    previousCenter = enemySprite.Center;

                enemySprite.Update(gameTime);

                if (currentTarget != Vector2.Zero)
                    enemySprite.Rotation = (float)Math.Atan2(currentTarget.Y - enemySprite.Center.Y,
                                                             currentTarget.X - enemySprite.Center.X);
                else
                    enemySprite.Rotation = (float)Math.Atan2(enemySprite.Center.Y - previousCenter.Y,
                                                             enemySprite.Center.X - previousCenter.X);

                this.enemySprite.TintColor = Color.White;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
            {
                EnemySprite.Draw(spriteBatch);
            }
        }

        public void SetLevel(int lvl)
        {
            this.hitScore = initialHitScore + (lvl - 1) * (initialHitScore / 10);
            this.killScore = initialKillScore + (lvl - 1) * (initialKillScore / 10);

            this.shotChance = initialShotChance + (float)Math.Sqrt(lvl - 1) * 0.02f * initialShotChance * (lvl - 1) * 0.002f;

            this.MaxHitPoints += (lvl - 1) * 20;
            this.HitPoints += (lvl - 1) * 20;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Enemy sprite
            enemySprite.Activated(reader);

            this.currentTarget = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));

            this.speed = Single.Parse(reader.ReadLine());

            this.previousCenter = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

            this.hitPoints = Single.Parse(reader.ReadLine());
            this.MaxHitPoints = Single.Parse(reader.ReadLine());

            this.type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

            this.hitScore = Int32.Parse(reader.ReadLine());

            this.killScore = Int32.Parse(reader.ReadLine());

            this.shotChance = Single.Parse(reader.ReadLine());

            this.isCamper = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Enemy sprite
            enemySprite.Deactivated(writer);

            writer.WriteLine(currentTarget.X);
            writer.WriteLine(currentTarget.Y);

            writer.WriteLine(speed);

            writer.WriteLine(previousCenter.X);
            writer.WriteLine(previousCenter.Y);

            writer.WriteLine(hitPoints);
            writer.WriteLine(MaxHitPoints);

            writer.WriteLine(type);

            writer.WriteLine(hitScore);

            writer.WriteLine(killScore);

            writer.WriteLine(shotChance);

            writer.WriteLine(isCamper);
        }

        #endregion

        #region Properties

        public Sprite EnemySprite
        {
            get
            {
                return this.enemySprite;
            }
        }

        public EnemyType Type
        {
            get
            {
                return this.type;
            }
        }

        public Vector2 GunOffset
        {
            get
            {
                return this.gunOffset;
            }
        }

        public int HitScore
        {
            get
            {
                return this.hitScore;
            }
        }

        public int InitialHitScore
        {
            get
            {
                return this.initialHitScore;
            }
        }

        public int KillScore
        {
            get
            {
                return this.killScore;
            }
        }

        public int InitialKillScore
        {
            get
            {
                return this.initialKillScore;
            }
        }

        public float ShotChance
        {
            get
            {
                return this.shotChance;
            }
        }

        public float HitPoints
        {
            get
            {
                return this.hitPoints;
            }
            set
            {
                this.hitPoints = MathHelper.Clamp(value, 0.0f, MaxHitPoints);
            }
        }

        public bool IsDestroyed
        {
            get
            {
                return this.hitPoints <= 0.0f;
            }
        }

        public Vector2 CurrentTarget
        {
            set
            {
                this.currentTarget = value;
            }
        }

        #endregion
    }
}
