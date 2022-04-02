using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace SpaceScribble.Extensions
{
    public static class SpriteBatchExtensions
    {
        private static string[] digits = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private static string[] charBuffer = new string[19];
        private static float[] xposBuffer = new float[19];
        private static readonly string minValue32 = Int32.MinValue.ToString(CultureInfo.InvariantCulture);
        private static readonly string minValue64 = Int64.MinValue.ToString(CultureInfo.InvariantCulture);

        private static readonly Vector2 Border1 = new Vector2(1, 1);
        private static readonly Vector2 Border2 = new Vector2(-1, 1);
        private static readonly Vector2 Border3 = new Vector2(1, -1);
        private static readonly Vector2 Border4 = new Vector2(-1, -1);

        /// <summary>  
        /// Extension method for SpriteBatch that draws an integer without allocating  
        /// any memory. This function avoids garbage collections that are normally caused  
        /// by calling Int32.ToString or String.Format.  
        /// </summary>  
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>  
        /// <param name="spriteFont">The SpriteFont to draw the integer value with.</param>  
        /// <param name="value">The integer value to draw.</param>  
        /// <param name="position">The screen position specifying where to draw the value.</param>  
        /// <param name="color">The color of the text drawn.</param>  
        /// <returns>The next position on the line to draw text. This value uses position.Y and position.X plus the equivalent of calling spriteFont.MeasureString on value.ToString(CultureInfo.InvariantCulture).</returns>  
        public static Vector2 DrawInt32(this SpriteBatch spriteBatch, SpriteFont spriteFont, int value, Vector2 position, Color color)
        {
            if (spriteBatch == null) throw new ArgumentNullException("spriteBatch");
            if (spriteFont == null) throw new ArgumentNullException("spriteFont");

            Vector2 nextPosition = position;

            if (value == Int32.MinValue)
            {
                nextPosition.X = nextPosition.X + spriteFont.MeasureString(minValue32).X;
                spriteBatch.DrawString(spriteFont, minValue32, position, color);
                position = nextPosition;
            }
            else
            {
                if (value < 0)
                {
                    nextPosition.X = nextPosition.X + spriteFont.MeasureString("-").X;
                    spriteBatch.DrawString(spriteFont, "-", position, color);
                    value = -value;
                    position = nextPosition;
                }

                int index = 0;

                do
                {
                    int modulus = value % 10;
                    value = value / 10;

                    charBuffer[index] = digits[modulus];
                    xposBuffer[index] = spriteFont.MeasureString(digits[modulus]).X;
                    index += 1;
                }
                while (value > 0);

                for (int i = index - 1; i >= 0; --i)
                {
                    nextPosition.X = nextPosition.X + xposBuffer[i];
                    spriteBatch.DrawString(spriteFont, charBuffer[i], position, color);
                    position = nextPosition;
                }
            }
            return position;
        }

        /// <summary>  
        /// Extension method for SpriteBatch that draws an integer without allocating  
        /// any memory. This function avoids garbage collections that are normally caused  
        /// by calling Int64.ToString or String.Format.  
        /// </summary>  
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>  
        /// <param name="spriteFont">The SpriteFont to draw the integer value with.</param>  
        /// <param name="value">The long value to draw.</param>  
        /// <param name="position">The screen position specifying where to draw the value.</param>  
        /// <param name="color">The color of the text drawn.</param>  
        /// <returns>The next position on the line to draw text. This value uses position.Y and position.X plus the equivalent of calling spriteFont.MeasureString on value.ToString(CultureInfo.InvariantCulture).</returns>  
        public static Vector2 DrawInt64(this SpriteBatch spriteBatch, SpriteFont spriteFont, long value, Vector2 position, Color color)
        {
            if (spriteBatch == null) throw new ArgumentNullException("spriteBatch");
            if (spriteFont == null) throw new ArgumentNullException("spriteFont");

            Vector2 nextPosition = position;

            if (value == Int64.MinValue)
            {
                nextPosition.X = nextPosition.X + spriteFont.MeasureString(minValue32).X;
                spriteBatch.DrawString(spriteFont, minValue64, position, color);
                position = nextPosition;
            }
            else
            {
                if (value < 0)
                {
                    nextPosition.X = nextPosition.X + spriteFont.MeasureString("-").X;
                    spriteBatch.DrawString(spriteFont, "-", position, color);
                    value = -value;
                    position = nextPosition;
                }

                int index = 0;

                do
                {
                    long modulus = value % 10;
                    value = value / 10;

                    charBuffer[index] = digits[modulus];
                    xposBuffer[index] = spriteFont.MeasureString(digits[modulus]).X;
                    index += 1;
                }
                while (value > 0);

                for (int i = index - 1; i >= 0; --i)
                {
                    nextPosition.X = nextPosition.X + xposBuffer[i];
                    spriteBatch.DrawString(spriteFont, charBuffer[i], position, color);
                    position = nextPosition;
                }
            }
            return position;
        }

        /// <summary>  
        /// Extension method for SpriteBatch that draws an integer without allocating  
        /// any memory. This function avoids garbage collections that are normally caused  
        /// by calling Int64.ToString or String.Format.  
        /// </summary>  
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>  
        /// <param name="spriteFont">The SpriteFont to draw the integer value with.</param>  
        /// <param name="value">The long value to draw.</param>  
        /// <param name="position">The screen position specifying where to draw the value.</param>  
        /// <param name="color">The color of the text drawn.</param>
        /// <param name="numberLength">The length of the number.</param>  
        /// <returns>The next position on the line to draw text. This value uses position.Y and position.X plus the equivalent of calling spriteFont.MeasureString on value.ToString(CultureInfo.InvariantCulture).</returns>  
        public static Vector2 DrawInt64WithZeros(this SpriteBatch spriteBatch, SpriteFont spriteFont, long value, Vector2 position, Color color, int numberLength)
        {
            if (spriteBatch == null) throw new ArgumentNullException("spriteBatch");
            if (spriteFont == null) throw new ArgumentNullException("spriteFont");

            Vector2 nextPosition = position;

            if (value == Int64.MinValue)
            {
                nextPosition.X = nextPosition.X + spriteFont.MeasureString(minValue32).X;
                spriteBatch.DrawString(spriteFont, minValue64, position, color);
                position = nextPosition;
            }
            else
            {
                if (value < 0)
                {
                    nextPosition.X = nextPosition.X + spriteFont.MeasureString("-").X;
                    spriteBatch.DrawString(spriteFont, "-", position, color);
                    value = -value;
                    position = nextPosition;
                }

                int index = 0;

                do
                {
                    long modulus = value % 10;
                    value = value / 10;

                    charBuffer[index] = digits[modulus];
                    xposBuffer[index] = spriteFont.MeasureString(digits[modulus]).X;
                    index += 1;
                }
                while (value > 0);

                float zero_xpos = spriteFont.MeasureString(digits[0]).X;

                for (int i = numberLength - index - 1; i >= 0; --i)
                {
                    nextPosition.X = nextPosition.X + zero_xpos;
                    spriteBatch.DrawString(spriteFont, digits[0], position, color);
                    position = nextPosition;
                }

                for (int i = index - 1; i >= 0; --i)
                {
                    nextPosition.X = nextPosition.X + xposBuffer[i];
                    spriteBatch.DrawString(spriteFont, charBuffer[i], position, color);
                    position = nextPosition;
                }
            }
            return position;
        }

        public static void DrawStringBordered(this SpriteBatch spriteBatch, SpriteFont font, string text,
            Vector2 location, Color color, Color borderColor)
        {
            spriteBatch.DrawString(font,
                                  text,
                                  location + Border1,
                                  borderColor);
            spriteBatch.DrawString(font,
                                  text,
                                  location + Border2,
                                  borderColor);
            spriteBatch.DrawString(font,
                                  text,
                                  location + Border3,
                                  borderColor);
            spriteBatch.DrawString(font,
                                  text,
                                  location + Border4,
                                  borderColor);

            spriteBatch.DrawString(font,
                                  text,
                                  location,
                                  color);
        }

        public static void DrawStringBordered(this SpriteBatch spriteBatch, SpriteFont font, string text,
            Vector2 location, Color color, Color borderColor, float rotation, Vector2 origin, float scale)
        {
            spriteBatch.DrawString(font,
                                  text,
                                  location + Border1,
                                  borderColor,
                                  rotation,
                                  origin,
                                  scale,
                                  SpriteEffects.None,
                                  0.0f);
            spriteBatch.DrawString(font,
                                  text,
                                  location + Border2,
                                  borderColor,
                                  rotation,
                                  origin,
                                  scale,
                                  SpriteEffects.None,
                                  0.0f);
            spriteBatch.DrawString(font,
                                  text,
                                  location + Border3,
                                  borderColor,
                                  rotation,
                                  origin,
                                  scale,
                                  SpriteEffects.None,
                                  0.0f);
            spriteBatch.DrawString(font,
                                  text,
                                  location + Border4,
                                  borderColor,
                                  rotation,
                                  origin,
                                  scale,
                                  SpriteEffects.None,
                                  0.0f);

            spriteBatch.DrawString(font,
                                  text,
                                  location,
                                  color,
                                  rotation,
                                  origin,
                                  scale,
                                  SpriteEffects.None,
                                  0.0f);
        }

        public static Vector2 DrawInt64WithZerosBordered(this SpriteBatch spriteBatch, SpriteFont font, long number,
            Vector2 location, Color color, Color borderColor, int zeros)
        {
            spriteBatch.DrawInt64WithZeros(font,
                                  number,
                                  location + Border1,
                                  borderColor,
                                  zeros);
            spriteBatch.DrawInt64WithZeros(font,
                                  number,
                                  location + Border2,
                                  borderColor,
                                  zeros);
            spriteBatch.DrawInt64WithZeros(font,
                                  number,
                                  location + Border3,
                                  borderColor,
                                  zeros);
            spriteBatch.DrawInt64WithZeros(font,
                                  number,
                                  location + Border4,
                                  borderColor,
                                  zeros);

            return spriteBatch.DrawInt64WithZeros(font,
                                  number,
                                  location,
                                  color,
                                  zeros);
        }

        public static Vector2 DrawInt64Bordered(this SpriteBatch spriteBatch, SpriteFont font, long number,
            Vector2 location, Color color, Color borderColor)
        {
            spriteBatch.DrawInt64(font,
                                  number,
                                  location + Border1,
                                  borderColor);
            spriteBatch.DrawInt64(font,
                                  number,
                                  location + Border2,
                                  borderColor);
            spriteBatch.DrawInt64(font,
                                  number,
                                  location + Border3,
                                  borderColor);
            spriteBatch.DrawInt64(font,
                                  number,
                                  location + Border4,
                                  borderColor);

            return spriteBatch.DrawInt64(font,
                                  number,
                                  location,
                                  color);
        }

        public static void DrawBordered(this SpriteBatch spriteBatch, Texture2D texture, Vector2 location,
            Rectangle source, Color color, Color borderColor)
        {
            spriteBatch.Draw(texture,
                    location + Border1,
                    source,
                    borderColor);
            spriteBatch.Draw(texture,
                    location + Border2,
                    source,
                    borderColor);
            spriteBatch.Draw(texture,
                    location + Border3,
                    source,
                    borderColor);
            spriteBatch.Draw(texture,
                    location + Border4,
                    source,
                    borderColor);

            spriteBatch.Draw(texture,
                    location,
                    source,
                    color);
        }

        public static void DrawBordered(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destination,
            Rectangle source, Color color, Color borderColor)
        {
            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)(destination.X + Border1.X),
                        (int)(destination.Y + Border1.Y),
                        destination.Width,
                        destination.Height),
                    source,
                    borderColor);
            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)(destination.X + Border2.X),
                        (int)(destination.Y + Border2.Y),
                        destination.Width,
                        destination.Height),
                    source,
                    borderColor);
            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)(destination.X + Border3.X),
                        (int)(destination.Y + Border3.Y),
                        destination.Width,
                        destination.Height),
                    source,
                    borderColor);
            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)(destination.X + Border4.X),
                        (int)(destination.Y + Border4.Y),
                        destination.Width,
                        destination.Height),
                    source,
                    borderColor);

            spriteBatch.Draw(texture,
                    destination,
                    source,
                    color);
        }

        public static void DrawBordered(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destination,
            Rectangle source, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth, Color borderColor)
        {
            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)(destination.X + Border1.X),
                        (int)(destination.Y + Border1.Y),
                        destination.Width,
                        destination.Height),
                    source,
                    borderColor,
                    rotation,
                    origin,
                    effects,
                    layerDepth);
            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)(destination.X + Border2.X),
                        (int)(destination.Y + Border2.Y),
                        destination.Width,
                        destination.Height),
                    source,
                    borderColor,
                    rotation,
                    origin,
                    effects,
                    layerDepth);
            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)(destination.X + Border3.X),
                        (int)(destination.Y + Border3.Y),
                        destination.Width,
                        destination.Height),
                    source,
                    borderColor,
                    rotation,
                    origin,
                    effects,
                    layerDepth);
            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)(destination.X + Border4.X),
                        (int)(destination.Y + Border4.Y),
                        destination.Width,
                        destination.Height),
                    source,
                    borderColor,
                    rotation,
                    origin,
                    effects,
                    layerDepth);

            spriteBatch.Draw(texture,
                    destination,
                    source,
                    color,
                    rotation,
                    origin,
                    effects,
                    layerDepth);
        }
    }
}
