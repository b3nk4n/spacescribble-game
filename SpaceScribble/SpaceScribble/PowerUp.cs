using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpaceScribble
{
    class PowerUp
    {
        #region Members

        private Sprite powerUpSprite;

        private const float SPEED = 125.0f;
        private const int RADIUS = 13;

        public enum PowerUpType { LowBonusScore, MediumBonusScore, HighBonusScore, Upgrade};

        private PowerUpType type;

        private bool isActive = true;

        private const int ScreenHeight = 800;
        private const int ScreenWidth = 480;

        private const int FramesCount = 16;

        #endregion

        #region Constructor

        public PowerUp(Texture2D texture, Vector2 location, Rectangle initialFrame,
                       PowerUpType type)
        {
            powerUpSprite = new Sprite(location, texture, initialFrame, new Vector2(0, 1) * SPEED);

            for (int x = 1; x < FramesCount; x++)
            {
                this.powerUpSprite.AddFrame(new Rectangle(initialFrame.X + (x * 30),
                                                         initialFrame.Y,
                                                         initialFrame.Width,
                                                         initialFrame.Height));
            }

            powerUpSprite.CollisionRadius = RADIUS;

            this.type = type;
        }

        #endregion

        public void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                powerUpSprite.Update(gameTime);

                if (!IsInScreen)
                {
                    IsActive = false;
                }

                checkBoundsX();
            }
        }

        private void checkBoundsX()
        {
            if (powerUpSprite.Location.X < -powerUpSprite.Source.Width / 2)
            {
                powerUpSprite.Location = new Vector2(-powerUpSprite.Source.Width / 2, powerUpSprite.Location.Y);
            }
            else if (powerUpSprite.Location.X > ScreenWidth - powerUpSprite.Source.Width / 2)
            {
                powerUpSprite.Location = new Vector2(ScreenWidth - powerUpSprite.Source.Width / 2, powerUpSprite.Location.Y);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                powerUpSprite.Draw(spriteBatch);
            }
        }

        #region Methods

        public bool isCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            return this.powerUpSprite.IsCircleColliding(otherCenter, otherRadius);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            powerUpSprite.Activated(reader);

            this.type = (PowerUpType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

            this.isActive = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Powerup sprite
            powerUpSprite.Deactivated(writer);

            writer.WriteLine(type);

            writer.WriteLine(isActive);
        }

        #endregion

        #region Properties

        public PowerUpType Type
        {
            get
            {
                return type;
            }
            set
            {
                this.type = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                this.isActive = value;
            }
        }

        public bool IsInScreen
        {
            get
            {
                return powerUpSprite.Location.Y < ScreenHeight;
            }
        }

        public Vector2 Center
        {
            get
            {
                return powerUpSprite.Center;
            }
        }

        #endregion
    }
}
