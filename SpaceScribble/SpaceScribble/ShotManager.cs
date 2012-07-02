using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpaceScribble
{
    class ShotManager
    {
        #region Members

        public List<Sprite> Shots = new List<Sprite>();
        
        private Rectangle screenBounds;

        // Laser
        private static Texture2D Texture;
        private static Rectangle InitialFrame;
        private static int FrameCount;
        private float defaultShotSpeed;
        private static int CollisionRadius;

        // Rocket
        public List<Sprite> Rockets = new List<Sprite>();
        private static Rectangle InitialRocketFrame;
        private static int RocketFrameCount = 5;
        private float rocketSpeed = 100.0f;
        private float softRocketSpeed = 50.0f;
        private static int RocketCollisionRadius = 7;

        private const float ROCKET_SMOKE_DELAY = 0.025f;
        private float rocketSmokeTimer = ROCKET_SMOKE_DELAY;

        // Sonic
        public List<Sprite> Sonic = new List<Sprite>();
        private static Rectangle InitialSonicFrame;
        private static int SonicFrameCount = 5;
        private float sonicSpeed = 400.0f;
        private static int SonicCollisionRadius = 8;

        #endregion

        #region Constructors

        public ShotManager(Texture2D texture, Rectangle initialFrame, int frameCount,
                           int collisionRadius, float speed, Rectangle screenBounds)
        {
            ShotManager.Texture = texture;
            ShotManager.InitialFrame = initialFrame;
            ShotManager.FrameCount = frameCount;
            ShotManager.CollisionRadius = collisionRadius;
            this.defaultShotSpeed = speed;
            this.screenBounds = screenBounds;

            ShotManager.InitialRocketFrame = new Rectangle(650, 180, 24, 24);
            ShotManager.InitialSonicFrame = new Rectangle(650, 210, 24, 24);
        }

        #endregion

        #region Methods

        public void FireShot(Vector2 location, Vector2 direction, bool playerFired, Color tintColor, bool sound)
        {
            FireShot(location, direction, defaultShotSpeed, playerFired, tintColor, sound);
        }

        public void FireShot(Vector2 location, Vector2 direction, float speed, bool playerFired, Color tintColor, bool sound)
        {
            Sprite newShot = new Sprite(location,
                                        ShotManager.Texture,
                                        ShotManager.InitialFrame,
                                        direction);

            newShot.TintColor = tintColor;

            newShot.Velocity *= speed;
            newShot.RotateTo(direction);

            for (int x = 0; x < ShotManager.FrameCount; x++)
            {
                newShot.AddFrame(new Rectangle(ShotManager.InitialFrame.X + (x * ShotManager.InitialFrame.Width),
                                               ShotManager.InitialFrame.Y,
                                               ShotManager.InitialFrame.Width,
                                               ShotManager.InitialFrame.Height));
            }
            newShot.CollisionRadius = ShotManager.CollisionRadius;
            Shots.Add(newShot);

            if (sound)
            {
                if (playerFired)
                {
                    SoundManager.PlayPlayerShot();
                }
                else
                {
                    SoundManager.PlayEnemyShot();
                }
            }
        }

        public void FireRocket(Vector2 location, Vector2 direction, bool playerFired, Color tintColor, bool sound)
        {
            Sprite newShot = new Sprite(location,
                                        ShotManager.Texture,
                                        ShotManager.InitialRocketFrame,
                                        direction);

            newShot.TintColor = tintColor;

            if (newShot.Velocity != Vector2.Zero)
                newShot.Velocity.Normalize();

            if (playerFired)
                newShot.Velocity *= rocketSpeed;
            else
                newShot.Velocity *= softRocketSpeed;

            newShot.RotateTo(direction);

            for (int x = 0; x < ShotManager.RocketFrameCount; x++)
            {
                newShot.AddFrame(new Rectangle(ShotManager.InitialRocketFrame.X + (x * ShotManager.InitialRocketFrame.Width),
                                               ShotManager.InitialRocketFrame.Y,
                                               ShotManager.InitialRocketFrame.Width,
                                               ShotManager.InitialRocketFrame.Height));
            }
            newShot.CollisionRadius = ShotManager.RocketCollisionRadius;
            Rockets.Add(newShot);

            if (sound)
            {
                if (playerFired)
                    SoundManager.PlayRocketSound();
                else
                    SoundManager.PlayEnemyRocketSound();
            }
        }

        public void FireSonic(Vector2 location, Vector2 direction, Color tintColor, bool sound)
        {
            Sprite newShot = new Sprite(location,
                                        ShotManager.Texture,
                                        ShotManager.InitialSonicFrame,
                                        direction);

            newShot.TintColor = tintColor;

            if (newShot.Velocity != Vector2.Zero)
                newShot.Velocity.Normalize();

            newShot.Velocity *= sonicSpeed;

            newShot.RotateTo(direction);

            for (int x = 0; x < ShotManager.SonicFrameCount; x++)
            {
                newShot.AddFrame(new Rectangle(ShotManager.InitialSonicFrame.X + (x * ShotManager.InitialSonicFrame.Width),
                                               ShotManager.InitialSonicFrame.Y,
                                               ShotManager.InitialSonicFrame.Width,
                                               ShotManager.InitialSonicFrame.Height));
            }
            newShot.CollisionRadius = ShotManager.SonicCollisionRadius;
            Sonic.Add(newShot);

            if (sound)
            {
                SoundManager.PlaySonicSound();
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int x = Shots.Count - 1; x >= 0; --x)
            {
                Shots[x].Update(gameTime);
                if (!screenBounds.Intersects(Shots[x].Destination))
                {
                    Shots.RemoveAt(x);
                }
            }

            for (int x = Rockets.Count - 1; x >= 0; --x)
            {
                Rockets[x].Update(gameTime);
                Rockets[x].Velocity *= 1.0f + (float)gameTime.ElapsedGameTime.TotalSeconds * 1.4f;
                
                float speed = Rockets[x].Velocity.Length();

                if (speed > 450)
                {
                    Rockets[x].Velocity /= speed;
                    Rockets[x].Velocity *= 450;
                }
                

                if (!screenBounds.Intersects(Rockets[x].Destination))
                {
                    Rockets.RemoveAt(x);
                }
            }

            rocketSmokeTimer -= elapsed;

            if (rocketSmokeTimer <= 0.0f)
            {
                foreach (var rocket in Rockets)
                {
                    Vector2 offset = -rocket.Velocity;
                    offset.Normalize();
                    offset *= 20;

                    EffectManager.AddRocketSmoke(rocket.Center + offset,
                                                 rocket.Velocity);
                }

                rocketSmokeTimer = ROCKET_SMOKE_DELAY;
            }

            for (int x = Sonic.Count - 1; x >= 0; --x)
            {
                Sonic[x].Update(gameTime);
                if (!screenBounds.Intersects(Sonic[x].Destination))
                {
                    Sonic.RemoveAt(x);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var shot in this.Shots)
	        {
                shot.Draw(spriteBatch);
	        }

            foreach (var rocket in this.Rockets)
            {
                rocket.Draw(spriteBatch);
            }

            foreach (var sonic in this.Sonic)
            {
                sonic.Draw(spriteBatch);
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Shots
            int shotsCount = Int32.Parse(reader.ReadLine());

            Shots.Clear();

            for (int i = 0; i < shotsCount; ++i)
            {
                Vector2 location = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));
                Vector2 direction = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

                if (direction != Vector2.Zero)
                    direction.Normalize();

                FireShot(location,
                         direction,
                         true,
                         new Color(Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine())),
                         false);
            }

            // Rockets
            int rocketsCount = Int32.Parse(reader.ReadLine());

            Rockets.Clear();

            for (int i = 0; i < rocketsCount; ++i)
            {
                bool playerFired = false;
                Vector2 location = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));
                Vector2 direction = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

                if (direction != Vector2.Zero)
                    direction.Normalize();

                if (direction.X == 0.0f)
                    playerFired = true;

                FireRocket(location,
                           direction,
                           playerFired,
                           new Color(Int32.Parse(reader.ReadLine()),
                                     Int32.Parse(reader.ReadLine()),
                                     Int32.Parse(reader.ReadLine()),
                                     Int32.Parse(reader.ReadLine())),
                           false);
            }

            this.rocketSmokeTimer = Single.Parse(reader.ReadLine());

            // Sonic
            int sonicCount = Int32.Parse(reader.ReadLine());

            Sonic.Clear();

            for (int i = 0; i < sonicCount; ++i)
            {
                Vector2 location = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));
                Vector2 direction = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

                if (direction != Vector2.Zero)
                    direction.Normalize();

                FireSonic(location,
                         direction,
                         new Color(Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine())),
                         false);
            }
        }

        public void Deactivated(StreamWriter writer)
        {
            // Shots
            writer.WriteLine(Shots.Count);

            for (int i = 0; i < Shots.Count; ++i)
            {
                writer.WriteLine(Shots[i].Location.X);
                writer.WriteLine(Shots[i].Location.Y);
                writer.WriteLine(Shots[i].Velocity.X);
                writer.WriteLine(Shots[i].Velocity.Y);
                writer.WriteLine((int)Shots[i].TintColor.R);
                writer.WriteLine((int)Shots[i].TintColor.G);
                writer.WriteLine((int)Shots[i].TintColor.B);
                writer.WriteLine((int)Shots[i].TintColor.A);
            }

            // Rockets
            writer.WriteLine(Rockets.Count);

            for (int i = 0; i < Rockets.Count; ++i)
            {
                writer.WriteLine(Rockets[i].Location.X);
                writer.WriteLine(Rockets[i].Location.Y);
                writer.WriteLine(Rockets[i].Velocity.X);
                writer.WriteLine(Rockets[i].Velocity.Y);
                writer.WriteLine((int)Rockets[i].TintColor.R);
                writer.WriteLine((int)Rockets[i].TintColor.G);
                writer.WriteLine((int)Rockets[i].TintColor.B);
                writer.WriteLine((int)Rockets[i].TintColor.A);
            }

            writer.WriteLine(rocketSmokeTimer);

            // Sonic
            writer.WriteLine(Sonic.Count);

            for (int i = 0; i < Sonic.Count; ++i)
            {
                writer.WriteLine(Sonic[i].Location.X);
                writer.WriteLine(Sonic[i].Location.Y);
                writer.WriteLine(Sonic[i].Velocity.X);
                writer.WriteLine(Sonic[i].Velocity.Y);
                writer.WriteLine((int)Sonic[i].TintColor.R);
                writer.WriteLine((int)Sonic[i].TintColor.G);
                writer.WriteLine((int)Sonic[i].TintColor.B);
                writer.WriteLine((int)Sonic[i].TintColor.A);
            }
        }

        #endregion

        #region Properties

        public float ShotSpeed
        {
            get
            {
                return defaultShotSpeed;
            }
            set
            {
                this.defaultShotSpeed = value;
            }
        }

        #endregion
    }
}
