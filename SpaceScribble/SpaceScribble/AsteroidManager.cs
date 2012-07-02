using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpaceScribble
{
    class AsteroidManager : ILevel
    {
        #region Members

        private int screenWidth = 480;
        private int screenHeight = 800;
        private int screenPadding = 10;

        private Rectangle initialFrame;
        private int asteroidFrames;
        private Texture2D texture;

        private List<DestroyableAsteroid> asteroids = new List<DestroyableAsteroid>();
        private int minSpeed = 100;
        private int maxSpeed = 200;

        private Random rand = new Random();

        private readonly int initialCount;
        private int count;
        private const int MaxAsteroidsCount = 15;

        private bool isActive = true;

        public const int CRASH_POWER_MIN = 15;
        public const int CRASH_POWER_MAX = 20;

        // asteroidShower
        private List<DestroyableAsteroid> showerAsteroids = new List<DestroyableAsteroid>();

        private const int MIN_SHOWER_SPEED = 250;
        private const int MAX_SHOWER_SPEED = 400;

        public const float ABSOLUTE_MAX_SPEED = 450;

        private const int SHOWER_ASTEROIDS_COUNT = 12;

        private bool isShowerActive;

        private Vector2 offScreen = new Vector2(-500, -500);

        #endregion

        #region Constructors

        public AsteroidManager(int asteroidCount, Texture2D texture, Rectangle initialFrame,
                               int asteroidFrames, int screenWidth, int screenHeight)
        {
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.asteroidFrames = asteroidFrames;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.initialCount = asteroidCount;
            this.count = asteroidCount;

            for (int x = 0; x < this.count; x++)
            {
                AddAsteroid();
            }

            for (int x = 0; x < SHOWER_ASTEROIDS_COUNT; x++)
            {
                AddShowerAsteroid();
            }
        }

        #endregion

        #region Methods

        public void AddAsteroid()
        {
            DestroyableAsteroid newAsteroid = new DestroyableAsteroid(new Vector2(-500, -500),
                                            texture,
                                            initialFrame);

            for (int x = 0; x < asteroidFrames; x++)
            {
                newAsteroid.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
            }
            
            newAsteroid.Rotation = MathHelper.ToRadians((float)rand.Next(0, 360));
            newAsteroid.CollisionRadius = 15;
            newAsteroid.Reset();
            asteroids.Add(newAsteroid);
        }

        private void AddAsteroidAfterResume(float locationX, float locationY, float rotation,
                                            float velocityX, float velocityY)
        {
            DestroyableAsteroid newAsteroid = new DestroyableAsteroid(new Vector2(locationX, locationY),
                                            texture,
                                            initialFrame);

            for (int x = 0; x < asteroidFrames; x++)
            {
                newAsteroid.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
            }

            newAsteroid.Rotation = rotation;
            newAsteroid.Velocity = new Vector2(velocityX, velocityY);
            newAsteroid.CollisionRadius = 15;
            asteroids.Add(newAsteroid);
        }

        public void AddShowerAsteroid()
        {
            DestroyableAsteroid newAsteroid = new DestroyableAsteroid(new Vector2(-500, -500),
                                            texture,
                                            initialFrame);

            for (int x = 0; x < asteroidFrames; x++)
            {
                newAsteroid.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
            }

            newAsteroid.Rotation = MathHelper.ToRadians((float)rand.Next(0, 360));
            newAsteroid.CollisionRadius = 15;
            showerAsteroids.Add(newAsteroid);
        }

        private void AddShowerAsteroidAfterResume(float locationX, float locationY, float rotation,
                                                  float velocityX, float velocityY)
        {
            DestroyableAsteroid newAsteroid = new DestroyableAsteroid(new Vector2(locationX, locationY),
                                            texture,
                                            initialFrame);

            for (int x = 0; x < asteroidFrames; x++)
            {
                newAsteroid.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
            }

            newAsteroid.Rotation = rotation;
            newAsteroid.Velocity = new Vector2(velocityX, velocityY);
            newAsteroid.CollisionRadius = 15;
            showerAsteroids.Add(newAsteroid);
        }

        public void Clear()
        {
            asteroids.Clear();
            showerAsteroids.Clear();
        }

        private Vector2 randomLocation()
        {
            Vector2 location = Vector2.Zero;
            bool locationOK = true;
            int tryCount = 0;

            do
            {
                locationOK = true;
                switch (rand.Next(0, 3))
                {
                    case 0:
                        location.X = -initialFrame.Width;
                        location.Y = rand.Next(0, screenHeight / 2);
                        break;

                    case 1:
                        location.X = screenWidth;
                        location.Y = rand.Next(0, screenHeight / 2);
                        break;

                    case 2:
                        location.X = rand.Next(0, screenWidth);
                        location.Y = -initialFrame.Height;
                        break;
                }

                foreach (var asteroid in asteroids)
                {
                    if (asteroid.isBoxColliding(new Rectangle((int)location.X,
                                                              (int)location.Y,
                                                              initialFrame.Width,
                                                              initialFrame.Height)))
                    {
                        locationOK = false;
                    }
                }

                ++tryCount;

                if (tryCount > 5 && locationOK == false)
                {
                    location = new Vector2(-500, -500);
                    locationOK = true;
                }
            } while (locationOK == false);

            return location;
        }

        private Vector2 randomShowerLocation()
        {
            Vector2 location = Vector2.Zero;
            bool locationOK = true;
            int tryCount = 0;

            do
            {
                locationOK = true;

                location.X = rand.Next(0, screenWidth);
                location.Y = rand.Next(-800, -50);

                foreach (var asteroid in asteroids)
                {
                    if (asteroid.isBoxColliding(new Rectangle((int)location.X,
                                                              (int)location.Y,
                                                              initialFrame.Width,
                                                              initialFrame.Height)))
                    {
                        locationOK = false;
                    }
                }

                ++tryCount;

                if (tryCount > 20 && locationOK == false)
                {
                    location = new Vector2(-500, -500);
                    locationOK = true;
                }
            } while (locationOK == false);

            return location;
        }

        private Vector2 randomVelocity()
        {
            Vector2 velocity = new Vector2(rand.Next(0, 101) - 50,
                                           rand.Next(0, 101) - 50);
            velocity.Normalize();
            velocity *= rand.Next(minSpeed, maxSpeed);
            return velocity;
        }

        private Vector2 randomShowerVelocity()
        {
            Vector2 velocity = new Vector2(rand.Next(0, 51) - 25,
                                           150);
            velocity.Normalize();
            velocity *= rand.Next(MIN_SHOWER_SPEED, MAX_SHOWER_SPEED);
            return velocity;
        }

        private bool isOnScreen(Sprite asteroid)
        {
            if (asteroid.Destination.Intersects(new Rectangle(-screenPadding,
                                                              -screenPadding,
                                                              screenWidth + screenPadding,
                                                              screenHeight + screenPadding)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isShowerOnScreen(Sprite asteroid)
        {
            if (asteroid.Destination.Intersects(new Rectangle(-screenPadding,
                                                              -screenPadding - 800,
                                                              screenWidth + screenPadding,
                                                              screenHeight + screenPadding + 800)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (this.count > asteroids.Count && isActive)
            {
                this.AddAsteroid();
            }
            else if (this.count < asteroids.Count || !isActive)
            {
                for (int i = 0; i < asteroids.Count; i++)
                {
                    if (!isOnScreen(asteroids[i]))
                    {
                        // Remove just one Asteroid per loop
                        asteroids.RemoveAt(i);
                        break;
                    }
                }
            }

            foreach (var asteroid in asteroids)
            {
                asteroid.Update(gameTime);
                if (!isOnScreen(asteroid) && isActive)
                {
                    asteroid.Location = randomLocation();
                    asteroid.Velocity = randomVelocity();
                    asteroid.Reset();
                }
            }

            // bounce normal vs. normal
            for (int x = 0; x < asteroids.Count; x++)
            {
                for (int y = x + 1; y < asteroids.Count; y++)
                {
                    if (Asteroids[x].IsCircleColliding(asteroids[y].Center,
                                                       asteroids[y].CollisionRadius))
                    {
                        BounceAsteroids(asteroids[x], asteroids[y]);
                    }
                }
            }

            updateShower(gameTime);

            // bounce normal vs. shower
            for (int x = 0; x < asteroids.Count; x++)
            {
                for (int y = 0; y < showerAsteroids.Count; y++)
                {
                    if (asteroids[x].IsCircleColliding(showerAsteroids[y].Center,
                                                       showerAsteroids[y].CollisionRadius))
                    {
                        BounceAsteroids(asteroids[x], showerAsteroids[y]);
                    }
                }
            }
        }

        private void updateShower(GameTime gameTime)
        {
            foreach (var asteroid in showerAsteroids)
            {
                asteroid.Update(gameTime);

                if (!isShowerOnScreen(asteroid) && isShowerActive && isActive)
                {
                    asteroid.Location = randomShowerLocation();
                    asteroid.Velocity = randomShowerVelocity();
                    asteroid.Reset();
                }
            }

            // bounce shower vs. shower
            for (int x = 0; x < showerAsteroids.Count; x++)
            {
                for (int y = x + 1; y < showerAsteroids.Count; y++)
                {
                    if (showerAsteroids[x].IsCircleColliding(showerAsteroids[y].Center,
                                                       showerAsteroids[y].CollisionRadius))
                    {
                        BounceAsteroids(showerAsteroids[x], showerAsteroids[y]);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var asteroid in asteroids)
            {
                asteroid.Draw(spriteBatch);
            }

            foreach (var asteroid in showerAsteroids)
            {
                asteroid.Draw(spriteBatch);
            }
        }

        private void BounceAsteroids(DestroyableAsteroid asteroid1, DestroyableAsteroid asteroid2)
        {
            if (!isShowerOnScreen(asteroid1) ||
                !isShowerOnScreen(asteroid2))
                return;

            Vector2 cOfMass = (asteroid1.Velocity + asteroid2.Velocity) / 2;

            Vector2 normal1 = asteroid2.Center - asteroid1.Center;
            normal1.Normalize();
            Vector2 normal2 = asteroid1.Center - asteroid2.Center;
            normal2.Normalize();

            asteroid1.Velocity -= cOfMass;
            asteroid1.Velocity = Vector2.Reflect(asteroid1.Velocity, normal1);
            asteroid1.Velocity += cOfMass;

            asteroid2.Velocity -= cOfMass;
            asteroid2.Velocity = Vector2.Reflect(asteroid2.Velocity, normal2);
            asteroid2.Velocity += cOfMass;

            if (asteroid1.RemainingSustainingHits < asteroid2.RemainingSustainingHits)
                asteroid1.DecreaseRemainingSustainingHits();
            else
                asteroid1.DecreaseRemainingSustainingHits();

            if (asteroid1.IsDestroyed)
            {
                EffectManager.AddAsteroidExplosion(asteroid1.Center,
                                       asteroid1.Velocity / 10,
                                       false);
                asteroid1.Location = offScreen;
            }

            if (asteroid2.IsDestroyed)
            {
                EffectManager.AddAsteroidExplosion(asteroid2.Center,
                                       asteroid2.Velocity / 10,
                                       false);
                asteroid2.Location = offScreen;
            }
        }

        public void Reset()
        {
            foreach (var asteroid in asteroids)
            {
                asteroid.Location = new Vector2(-500, -500);
            }

            foreach (var asteroid in showerAsteroids)
            {
                asteroid.Location = new Vector2(-500, -500);
            }

            isShowerActive = false;
        }

        public void SetLevel(int lvl)
        {
            int newCount = (int)(initialCount + Math.Sqrt(lvl - 1) / 2 + (lvl - 1) * 0.05f);

            this.count = Math.Min(newCount, MaxAsteroidsCount);
        }

        public void AcivateShower()
        {
            isShowerActive = true;

            foreach (var asteroid in showerAsteroids)
            {
                asteroid.Location = randomShowerLocation();
                asteroid.Velocity = randomShowerVelocity();
            }
        }

        public void DeactivateShower()
        {
            isShowerActive = false;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.count = Int32.Parse(reader.ReadLine());

            asteroids.Clear();

            for (int i = 0; i < this.count; ++i)
            {
                AddAsteroidAfterResume(Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()));
                asteroids[i].DestroyableActivated(reader);
            }

            this.isActive = Boolean.Parse(reader.ReadLine());

            // shower asteroids
            int showerCount = Int32.Parse(reader.ReadLine());

            showerAsteroids.Clear();

            for (int i = 0; i < showerCount; ++i)
            {
                AddShowerAsteroidAfterResume(Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()));
                showerAsteroids[i].DestroyableActivated(reader);
            }

            this.isShowerActive = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            int realAsteroidsCount = Math.Min(this.count, asteroids.Count);
            
            writer.WriteLine(realAsteroidsCount);
            
            for (int i = 0; i < realAsteroidsCount; ++i)
            {
                writer.WriteLine(asteroids[i].Location.X);
                writer.WriteLine(asteroids[i].Location.Y);
                writer.WriteLine(asteroids[i].Rotation);
                writer.WriteLine(asteroids[i].Velocity.X);
                writer.WriteLine(asteroids[i].Velocity.Y);
                asteroids[i].DestroyableDeactivated(writer);
            }

            writer.WriteLine(this.isActive);

            // shower asteroids
            int realShowerAsteroidsCount = showerAsteroids.Count;

            writer.WriteLine(realShowerAsteroidsCount);

            for (int i = 0; i < realShowerAsteroidsCount; ++i)
            {
                writer.WriteLine(showerAsteroids[i].Location.X);
                writer.WriteLine(showerAsteroids[i].Location.Y);
                writer.WriteLine(showerAsteroids[i].Rotation);
                writer.WriteLine(showerAsteroids[i].Velocity.X);
                writer.WriteLine(showerAsteroids[i].Velocity.Y);
                showerAsteroids[i].DestroyableDeactivated(writer);
            }

            writer.WriteLine(this.isShowerActive);
        }

        #endregion

        #region Properties

        public List<DestroyableAsteroid> Asteroids
        {
            get
            {
                return this.asteroids;
            }
        }

        public bool IsActive
        {
            set
            {
                this.isActive = value;
            }
            get
            {
                return this.isActive;
            }
        }

        public List<DestroyableAsteroid> ShowerAsteroids
        {
            get
            {
                return this.showerAsteroids;
            }
        }

        #endregion
    }
}
