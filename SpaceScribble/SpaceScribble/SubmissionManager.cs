using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using SpaceScribble.Inputs;
using SpaceScribble.Extensions;

namespace SpaceScribble
{
    class SubmissionManager
    {
        #region Members

        LeaderboardManager leaderboardManager;

        private readonly Rectangle submitSource = new Rectangle(0, 1120, 
                                                                240, 80);
        private readonly Rectangle submitDestination = new Rectangle(5, 710,
                                                                     230, 77);

        private readonly Rectangle cancelSource = new Rectangle(0, 800,
                                                                240, 80);
        private readonly Rectangle cancelDestination = new Rectangle(245, 710,
                                                                     230, 77);

        private readonly Rectangle retrySource = new Rectangle(0, 1040,
                                                                  240, 80);
        private readonly Rectangle retryDestination = new Rectangle(5, 710,
                                                                    230, 77);

        private static SubmissionManager submissionManager;

        public const int MaxScores = 10;

        public static Texture2D Texture;
        public static SpriteFont Font;
        private readonly Rectangle TitleSource = new Rectangle(0, 300,
                                                               480, 100);
        private readonly Vector2 TitlePosition = new Vector2(0.0f, 100.0f);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private string name = string.Empty;
        private long score;
        private int level;
        private long earnedCredits;

        private bool cancelClicked = false;
        private bool retryClicked = false;


        private readonly string[] TEXT_SUBMIT = {"You have now the ability to share your",
                                                 "score with players from all over the world!"};
        private const string TEXT_NAME = "Name:";
        private const string TEXT_SCORE = "Score:";
        private const string TEXT_LEVEL = "Level:";
        private const string TEXT_CREDITS = "Credits:";

        private enum SubmitState { Submit, Submitted };
        private SubmitState submitState = SubmitState.Submit;

        public static GameInput GameInput;
        private const string SubmitAction = "Submit";
        private const string CancelAction = "Cancel";
        private const string RetryAction = "Retry";

        private const int TextPositionX = 140;
        private const int ValuePositionX = 260;

        #endregion

        #region Constructors

        private SubmissionManager()
        {
            leaderboardManager = LeaderboardManager.GetInstance();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(SubmitAction,
                                           GestureType.Tap,
                                           submitDestination);
            GameInput.AddTouchGestureInput(CancelAction,
                                           GestureType.Tap,
                                           cancelDestination);
            GameInput.AddTouchGestureInput(RetryAction,
                                           GestureType.Tap,
                                           retryDestination);
        }

        public static SubmissionManager GetInstance()
        {
            if (submissionManager == null)
            {
                submissionManager = new SubmissionManager();
            }

            return submissionManager;
        }

        private void handleTouchInputs()
        {
            if (submitState == SubmitState.Submit)
            {
                // Submit
                if (GameInput.IsPressed(SubmitAction))
                {
                    SoundManager.PlaySelectSound();
                    leaderboardManager.Submit(LeaderboardManager.SUBMIT,
                                              name,
                                              score,
                                              level);
                    submitState = SubmitState.Submitted;
                }
            }
            else
            {
                // Retry
                if (GameInput.IsPressed(RetryAction))
                {
                    if (submitState == SubmitState.Submitted)
                    {
                        retryClicked = true;
                    }
                }
            }
            
            if (GameInput.IsPressed(CancelAction))
            {
                leaderboardManager.StatusText = LeaderboardManager.TEXT_NONE;
                cancelClicked = true;
            }
        }

        public void SetUp(string name, long score, int level, long earnedCredits)
        {
            this.name = name;
            this.score = score;
            this.level = level;
            this.earnedCredits = earnedCredits;
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                                 cancelDestination,
                                 cancelSource,
                                 Color.White * opacity);

            if (submitState == SubmitState.Submit)
            {
                spriteBatch.Draw(Texture,
                                 submitDestination,
                                 submitSource,
                                 Color.White * opacity);
            }
            else if (submitState == SubmitState.Submitted)
            {
                spriteBatch.Draw(Texture,
                                 retryDestination,
                                 retrySource,
                                 Color.White * opacity);

                spriteBatch.DrawString(Font,
                                   leaderboardManager.StatusText,
                                   new Vector2(240 - Font.MeasureString(leaderboardManager.StatusText).X / 2,
                                               635),
                                   Color.Black * opacity);

            }

            for (int i = 0; i < TEXT_SUBMIT.Length; i++)
            {
                spriteBatch.DrawString(Font,
                                   TEXT_SUBMIT[i],
                                   new Vector2(240 - Font.MeasureString(TEXT_SUBMIT[i]).X / 2,
                                               250 + (i * 35)),
                                   Color.Black * opacity);
            }

            // Title:
            spriteBatch.DrawString(Font,
                                   TEXT_NAME,
                                   new Vector2(TextPositionX,
                                               400),
                                   Color.Black * opacity);

            spriteBatch.DrawString(Font,
                                   TEXT_SCORE,
                                   new Vector2(TextPositionX,
                                               450),
                                   Color.Black * opacity);

            spriteBatch.DrawString(Font,
                                   TEXT_LEVEL,
                                   new Vector2(TextPositionX,
                                               500),
                                   Color.Black * opacity);

            spriteBatch.DrawString(Font,
                                   TEXT_CREDITS,
                                   new Vector2(TextPositionX,
                                               550),
                                   Color.Black * opacity);

            // Content:
            spriteBatch.DrawString(Font,
                                   name,
                                   new Vector2(ValuePositionX,
                                               400),
                                   Color.Black * opacity);

            spriteBatch.DrawInt64(Font,
                                  score,
                                  new Vector2(ValuePositionX,
                                              450),
                                  Color.Black * opacity);

            spriteBatch.DrawInt64(Font,
                                  level,
                                  new Vector2(ValuePositionX,
                                              500),
                                  Color.Black * opacity);

            Vector2 pos = spriteBatch.DrawInt64(Font,
                                  earnedCredits,
                                  new Vector2(ValuePositionX,
                                              550),
                                  Color.Black * opacity);

            spriteBatch.DrawString(Font,
                                   " $",
                                   pos,
                                   Color.Black);

            spriteBatch.Draw(Texture,
                             TitlePosition,
                             TitleSource,
                             Color.White * opacity);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
            this.name = reader.ReadLine();
            this.score = Int64.Parse(reader.ReadLine());
            this.level = Int32.Parse(reader.ReadLine());
            this.earnedCredits = Int64.Parse(reader.ReadLine());
            this.submitState = (SubmitState)Enum.Parse(submitState.GetType(), reader.ReadLine(), false);
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
            writer.WriteLine(name);
            writer.WriteLine(score);
            writer.WriteLine(level);
            writer.WriteLine(earnedCredits);
            writer.WriteLine(submitState);
        }

        #endregion

        #region Properties

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;

                if (isActive == false)
                {
                    this.opacity = OpacityMin;
                    this.retryClicked = false;
                    this.cancelClicked = false;
                    this.submitState = SubmitState.Submit;
                }
            }
        }

        public bool CancelClicked
        {
            get
            {
                return this.cancelClicked;
            }
        }

        public bool RetryClicked
        {
            get
            {
                return this.retryClicked;
            }
        }

        #endregion
    }
}
