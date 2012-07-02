using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace SpaceScribble
{
    /// <summary>
    /// An asteroid, which is destroyable
    /// </summary>
    class DestroyableAsteroid : Sprite
    {
        #region Members

        private int remainingSustainingHits;
        private int initialRemainingSustainingHits;

        public const int HITS_TO_DESTROY_ASTEROID_MIN = 10;
        public const int HITS_TO_DESTROY_ASTEROID_MAX = 15;

        private bool asteroidSoundPlayed;

        private float smoakTimer;
        private const float SmoakTimerMin = 0.075f;
        private const float SmoakTimerMax = 0.25f;

        private Random rand = new Random();

        #endregion

        #region Constructors

        public DestroyableAsteroid(Vector2 location, Texture2D texture,
                            Rectangle initialFrame)
            : base(location, texture, initialFrame, Vector2.Zero)
        {
            this.Reset();
        }

        #endregion

        #region Methods

        public void DecreaseRemainingSustainingHits()
        {
            this.remainingSustainingHits--;
        }
        
        public void Destroy()
        {
            this.remainingSustainingHits = 0;
        }
        

        public void Reset()
        {
            this.initialRemainingSustainingHits = rand.Next(HITS_TO_DESTROY_ASTEROID_MIN, HITS_TO_DESTROY_ASTEROID_MAX);
            this.remainingSustainingHits = initialRemainingSustainingHits;
            this.asteroidSoundPlayed = false;
            this.smoakTimer = SmoakTimerMin;
        }

        public void PlayAsteroidSound(float factor)
        {
            if (!asteroidSoundPlayed)
            {
                SoundManager.PlayAsteroidSound(factor);
                asteroidSoundPlayed = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDestroyed)
            {
                base.Draw(spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            smoakTimer -= elapsed;

            if (!IsDestroyed && smoakTimer <= 0.0f)
            {
                smoakTimer = MathHelper.Clamp(SmoakTimerMax - velocity.Length() * 0.0005f, SmoakTimerMin, SmoakTimerMax);

                EffectManager.AddAsteroidSmoke(this.Center, this.velocity / 25);
            }

            base.Update(gameTime);
        }

        #endregion

        #region Activated/Deactivated

        public void DestroyableActivated(StreamReader reader)
        {
            remainingSustainingHits = Int32.Parse(reader.ReadLine());
            smoakTimer = Single.Parse(reader.ReadLine());
        }

        public void DestroyableDeactivated(StreamWriter writer)
        {
            writer.WriteLine(remainingSustainingHits);
            writer.WriteLine(smoakTimer);
        }

        #endregion

        #region Properties

        public bool IsDestroyed
        {
            get
            {
                return this.remainingSustainingHits < 1;
            }
        }

        public int RemainingSustainingHits
        {
            get
            {
                return remainingSustainingHits;
            }
        }

        public bool AsteroidSoundPlayed
        {
            get
            {
                return asteroidSoundPlayed;
            }
        }

        #endregion
    }
}
