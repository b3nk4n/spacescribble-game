using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpaceScribble
{
    class ZoomTextManager
    {
        #region Members

        private Queue<ZoomText> zoomTexts = new Queue<ZoomText>(8);
        private Vector2 location;
        private SpriteFont font;
        private SpriteFont bigFont;

        private static Queue<ZoomText> infoTexts = new Queue<ZoomText>(8);

        private static Queue<ZoomText> buyTexts = new Queue<ZoomText>(4);

        private static List<ZoomText> creditTexts = new List<ZoomText>(16);

        #endregion

        #region Constructors

        public ZoomTextManager(Vector2 location, SpriteFont font, SpriteFont bigFont)
        {
            this.location = location;
            this.font = font;
            this.bigFont = bigFont;
        }

        #endregion

        #region Methods

        public void ShowText(string text)
        {
            zoomTexts.Enqueue(new ZoomText(text,
                                           Color.Black,
                                           95,
                                           0.0225f,
                                           location));
        }

        public static void ShowInfo(string text)
        {
            infoTexts.Enqueue(new ZoomText(text,
                                       Color.Black,
                                       85,
                                       0.02f,
                                       new Vector2(240, 150)));
        }

        public static void ShowBuyPrice(string text)
        {
            buyTexts.Enqueue(new ZoomText(text,
                                       Color.Black,
                                       60,
                                       0.05f,
                                       new Vector2(300, 260)));
        }

        public static void ShowCredit(string text, Vector2 position)
        {
            creditTexts.Add(new ZoomText(text,
                                       Color.Black,
                                       50,
                                       0.04f,
                                       position));
        }

        public void Update()
        {
            if (zoomTexts.Count > 0)
            {
                zoomTexts.First().Update();

                if (zoomTexts.First().IsCompleted)
                {
                    zoomTexts.Dequeue();
                }
            }

            if (infoTexts.Count > 0)
            {
                infoTexts.First().Update();

                if (infoTexts.First().IsCompleted)
                {
                    infoTexts.Dequeue();
                }
            }

            if (buyTexts.Count > 0)
            {
                buyTexts.First().Update();

                if (buyTexts.First().IsCompleted)
                {
                    buyTexts.Dequeue();
                }
            }

            for (int i = 0; i < creditTexts.Count; i++)
			{
			    creditTexts[i].Update();

                if (creditTexts[i].IsCompleted)
                {
                    creditTexts.RemoveAt(i);
                }
			}
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var zoom in zoomTexts)
            {
                spriteBatch.DrawString(bigFont,
                                       zoom.text,
                                       zoom.Location,
                                       zoom.drawColor * (float)(1.0f - Math.Pow(zoom.Progress, 4.0f)),
                                       0.0f,
                                       new Vector2(bigFont.MeasureString(zoom.text).X / 2,
                                                   bigFont.MeasureString(zoom.text).Y / 2),
                                       zoom.Scale,
                                       SpriteEffects.None,
                                       0.0f);
            }

            foreach (var info in infoTexts)
            {
                spriteBatch.DrawString(bigFont,
                                       info.text,
                                       info.Location,
                                       info.drawColor * (float)(1.0f - Math.Pow(info.Progress, 4.0f)),
                                       0.0f,
                                       new Vector2(bigFont.MeasureString(info.text).X / 2,
                                                   bigFont.MeasureString(info.text).Y / 2),
                                       info.Scale,
                                       SpriteEffects.None,
                                       0.0f);
            }

            foreach (var buyText in buyTexts)
            {
                spriteBatch.DrawString(font,
                                       buyText.text,
                                       new Vector2(buyText.Location.X, buyText.Location.Y - buyText.Progress * 40),
                                       buyText.drawColor * (float)(1.0f - Math.Pow(buyText.Progress, 4.0f)),
                                       0.0f,
                                       new Vector2(font.MeasureString(buyText.text).X / 2,
                                                   font.MeasureString(buyText.text).Y / 2),
                                       buyText.Scale,
                                       SpriteEffects.None,
                                       0.0f);
            }

            foreach (var creaditText in creditTexts)
            {
                spriteBatch.DrawString(font,
                                       creaditText.text,
                                       new Vector2(creaditText.Location.X, creaditText.Location.Y - creaditText.Progress * 40),
                                       creaditText.drawColor * (float)(1.0f - Math.Pow(creaditText.Progress, 4.0f)),
                                       0.0f,
                                       new Vector2(font.MeasureString(creaditText.text).X / 2,
                                                   font.MeasureString(creaditText.text).Y / 2),
                                       creaditText.Scale,
                                       SpriteEffects.None,
                                       0.0f);
            }
        }

        public void Reset()
        {
            this.zoomTexts.Clear();
            infoTexts.Clear();
            buyTexts.Clear();
            creditTexts.Clear();
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Texts
            int textsCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < textsCount; ++i)
            {
                ZoomText text = new ZoomText();
                text.Activated(reader);
                zoomTexts.Enqueue(text);
            }

            // Infos
            int infosCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < infosCount; ++i)
            {
                ZoomText info = new ZoomText();
                info.Activated(reader);
                infoTexts.Enqueue(info);
            }

            // Buy
            int buyCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < buyCount; ++i)
            {
                ZoomText buy = new ZoomText();
                buy.Activated(reader);
                infoTexts.Enqueue(buy);
            }

            // Credit
            int creditCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < creditCount; ++i)
            {
                ZoomText credit = new ZoomText();
                credit.Activated(reader);
                infoTexts.Enqueue(credit);
            }
        }

        public void Deactivated(StreamWriter writer)
        {
            // Texts
            int textsCount = zoomTexts.Count;
            writer.WriteLine(textsCount);

            for (int i = 0; i < textsCount; ++i)
            {
                ZoomText text = zoomTexts.Dequeue();
                text.Deactivated(writer);
            }

            // Infos
            int infosCount = infoTexts.Count;
            writer.WriteLine(infosCount);

            for (int i = 0; i < infosCount; ++i)
            {
                ZoomText info = infoTexts.Dequeue();
                info.Deactivated(writer);
            }

            // Buy
            int buyCount = buyTexts.Count;
            writer.WriteLine(buyCount);

            for (int i = 0; i < buyCount; ++i)
            {
                ZoomText buy = buyTexts.Dequeue();
                buy.Deactivated(writer);
            }

            // Credit
            int creditCount = creditTexts.Count;
            writer.WriteLine(creditCount);

            for (int i = 0; i < creditCount; ++i)
            {
                ZoomText credit = creditTexts[i];
                credit.Deactivated(writer);
            }
        }

        #endregion
    }
}
