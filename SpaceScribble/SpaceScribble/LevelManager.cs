using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace SpaceScribble
{
    class LevelManager
    {
        #region Members

        private List<ILevel> components = new List<ILevel>();

        public const int StartLevel = 1;

        private float levelTimer = 0.0f;
        public const float TimeForLevel = 40.0f;
        public const int TimeForShowerMin = 15;
        public const int TimeForShowerMax = 20;
        public const float TimeForBoss = 3.0f;

        public enum LevelStates
        {
            Enemies1, AsteroidShower, Enemies2, Boss
        }

        private LevelStates levelState;

        private static int currentLevel;
        private int lastLevel;

        private bool hasChanged = false;

        Random rand = new Random();

        #endregion

        #region Constructors

        public LevelManager()
        {
            Reset();
        }

        #endregion

        #region Methods

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            levelTimer -= elapsed;

            if (levelTimer <= 0.0f)
            {
                ////SetLevelAll(currentLevel + 1);
                this.hasChanged = true;
                //GoToNextState();
                
            }
            else
            {
                this.hasChanged = false;
            }
        }

        public void GoToNextState()
        {
            switch (levelState)
            {
                case LevelStates.Enemies1:
                    levelState = LevelStates.AsteroidShower;
                    break;
                case LevelStates.AsteroidShower:
                    levelState = LevelStates.Enemies2;
                    break;
                case LevelStates.Enemies2:
                    levelState = LevelStates.Boss;
                    break;
                case LevelStates.Boss:
                    SetLevelAll(currentLevel + 1);
                    levelState = LevelStates.Enemies1;
                    break;
            }

            resetTimer();
        }

        private void resetTimer()
        {
            switch (levelState)
            {
                case LevelStates.Enemies1:
                    levelTimer = TimeForLevel;
                    break;
                case LevelStates.AsteroidShower:
                    levelTimer = rand.Next(TimeForShowerMin, TimeForShowerMax);
                    break;
                case LevelStates.Enemies2:
                    levelTimer = TimeForLevel;
                    break;
                case LevelStates.Boss:
                    levelTimer = TimeForBoss;
                    break;
            }
        }

        public void GoToLastEnemyState()
        {
            if (levelState == LevelStates.AsteroidShower)
                levelState = LevelStates.Enemies1;
            else if (levelState == LevelStates.Boss)
                levelState = LevelStates.Enemies2;

            resetTimer();
        }

        public void Register(ILevel comp)
        {
            if (!components.Contains(comp))
            {
                components.Add(comp);
            }
        }

        private void SetLevelAll(int lvl)
        {
            this.lastLevel = currentLevel;
            currentLevel = lvl;

            foreach (var comp in components)
            {
                comp.SetLevel(lvl);
            }
        }

        public void SetLevel(int level)
        {
            currentLevel = level;

            SetLevelAll(currentLevel);
        }


        public void Reset()
        {
            //ResetLevelTimer();
            
            levelState = LevelStates.Enemies1;

            levelTimer = TimeForLevel;

            currentLevel = 1;
            this.lastLevel = 1;

            SetLevelAll(LevelManager.StartLevel);

            
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.levelTimer = Single.Parse(reader.ReadLine());
            currentLevel = Int32.Parse(reader.ReadLine());
            this.lastLevel = Int32.Parse(reader.ReadLine());
            this.hasChanged = Boolean.Parse(reader.ReadLine());
            this.levelState = (LevelStates)Enum.Parse(levelState.GetType(), reader.ReadLine(), false); 
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(levelTimer);
            writer.WriteLine(currentLevel);
            writer.WriteLine(lastLevel);
            writer.WriteLine(hasChanged);
            writer.WriteLine(levelState);
        }

        #endregion

        #region Properties

        public int CurrentLevel
        {
            get
            {
                return currentLevel;
            }
        }

        /// <summary>
        /// Implemented to let the powerup-manager have easy access to the current level.
        /// Very ugly code! Shame on you...
        /// </summary>
        public static int CurrentLevelStatic
        {
            get
            {
                return currentLevel;
            }
        }

        public bool HasChanged
        {
            get
            {
                return hasChanged;
            }
        }

        public LevelStates LevelState
        {
            get
            {
                return this.levelState;
            }
        }

        #endregion
    }
}
