using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpaceScribble
{
    /// <summary>
    /// Manages the enemies.
    /// </summary>
    class EnemyManager : ILevel
    {
        #region Members

        private Texture2D texture;

        public List<Enemy> Enemies = new List<Enemy>(64);

        public ShotManager EnemyShotManager;
        private PlayerManager playerManager;

        public const int SOFT_ROCKET_EXPLOSION_RADIUS = 100;
        public const float ROCKET_POWER_AT_CENTER = 40.0f;

        public const int DAMAGE_LASER_MIN = 7;
        public const int DAMAGE_LASER_MAX = 10;

        public int MinShipsPerWave = 5;
        public int MaxShipsPerWave = 8;
        private float nextWaveTimer = 0.0f;
        public const float InitialNextWaveMinTimer = 5.0f;
        private float nextWaveMinTimer = 5.0f;
        private float shipSpawnTimer = 0.0f;
        private float shipSpawnWaitTimer = 0.5f;

        private List<List<Vector2>> pathWayPoints = new List<List<Vector2>>(32);

        private Dictionary<int, WaveInfo> waveSpawns = new Dictionary<int, WaveInfo>(16);

        public bool IsActive = false;

        private Random rand = new Random();

        private int currentLevel;

        private readonly Rectangle screen = new Rectangle(0, 0, 480, 800);

        // Harakiri enemies
        public List<HarakiriEnemy> Harakiries = new List<HarakiriEnemy>();

        private const float INIT_HARAKIRI_SPAWNTIME_MIN = 15.0f;
        private const float INIT_HARAKIRI_SPAWNTIME_MAX = 30.0f;

        private float harakiriSpawnTimer = INIT_HARAKIRI_SPAWNTIME_MAX;

        private float harakiriCampShotTimer = 0.0f;
        private const float HarakiriCampShotTimerMin = 2.0f;

        #endregion

        #region Constructors

        public EnemyManager(Texture2D texture, PlayerManager playerManager,
                            Rectangle screenBounds)
        {
            this.texture = texture;
            this.playerManager = playerManager;

            EnemyShotManager = new ShotManager(texture,
                                               new Rectangle(650, 160, 20, 20),
                                               4,
                                               2,
                                               225.0f,
                                               screenBounds);

            setUpWayPoints();

            this.currentLevel = 1;
        }

        #endregion

        #region Methods

        private void setUpWayPoints()
        {
            // Horizontal
            List<Vector2> path0 = new List<Vector2>();
            path0.Add(new Vector2(-50, 150));
            path0.Add(new Vector2(530, 150));
            pathWayPoints.Add(path0);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path1 = new List<Vector2>();
            path1.Add(new Vector2(530, 200));
            path1.Add(new Vector2(-50, 200));
            pathWayPoints.Add(path1);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path2 = new List<Vector2>();
            path2.Add(new Vector2(-50, 250));
            path2.Add(new Vector2(530, 250));
            pathWayPoints.Add(path2);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path3 = new List<Vector2>();
            path3.Add(new Vector2(530, 300));
            path3.Add(new Vector2(-50, 300));
            pathWayPoints.Add(path3);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path4 = new List<Vector2>();
            path4.Add(new Vector2(-50, 350));
            path4.Add(new Vector2(530, 350));
            pathWayPoints.Add(path4);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path5 = new List<Vector2>();
            path5.Add(new Vector2(530, 400));
            path5.Add(new Vector2(-50, 400));
            pathWayPoints.Add(path5);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path6 = new List<Vector2>();
            path6.Add(new Vector2(-50, 450));
            path6.Add(new Vector2(530, 450));
            pathWayPoints.Add(path6);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path7 = new List<Vector2>();
            path7.Add(new Vector2(530, 500));
            path7.Add(new Vector2(-50, 500));
            pathWayPoints.Add(path7);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path8 = new List<Vector2>();
            path8.Add(new Vector2(-50, 550));
            path8.Add(new Vector2(530, 550));
            pathWayPoints.Add(path8);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path9 = new List<Vector2>();
            path9.Add(new Vector2(530, 600));
            path9.Add(new Vector2(-50, 600));
            pathWayPoints.Add(path9);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            // Vertical
            List<Vector2> path10 = new List<Vector2>();
            path10.Add(new Vector2(50, -50));
            path10.Add(new Vector2(50, 850));
            pathWayPoints.Add(path10);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path11 = new List<Vector2>();
            path11.Add(new Vector2(100, -50));
            path11.Add(new Vector2(100, 850));
            pathWayPoints.Add(path11);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path12 = new List<Vector2>();
            path12.Add(new Vector2(150, -50));
            path12.Add(new Vector2(150, 850));
            pathWayPoints.Add(path12);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path13 = new List<Vector2>();
            path13.Add(new Vector2(200, -50));
            path13.Add(new Vector2(200, 850));
            pathWayPoints.Add(path13);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path14 = new List<Vector2>();
            path14.Add(new Vector2(250, -50));
            path14.Add(new Vector2(250, 850));
            pathWayPoints.Add(path14);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path15 = new List<Vector2>();
            path15.Add(new Vector2(300, -50));
            path15.Add(new Vector2(300, 850));
            pathWayPoints.Add(path15);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path16 = new List<Vector2>();
            path16.Add(new Vector2(350, -50));
            path16.Add(new Vector2(350, 850));
            pathWayPoints.Add(path16);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path17 = new List<Vector2>();
            path17.Add(new Vector2(400, -50));
            path17.Add(new Vector2(400, 850));
            pathWayPoints.Add(path17);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path18 = new List<Vector2>();
            path18.Add(new Vector2(450, -50));
            path18.Add(new Vector2(450, 850));
            pathWayPoints.Add(path18);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            // Others
            List<Vector2> path19 = new List<Vector2>();
            path19.Add(new Vector2(-50, 100));
            path19.Add(new Vector2(25, 100));
            path19.Add(new Vector2(50, 125));
            path19.Add(new Vector2(75, 175));
            path19.Add(new Vector2(100, 250));
            path19.Add(new Vector2(100, 350));
            path19.Add(new Vector2(150, 400));
            path19.Add(new Vector2(200, 450));
            path19.Add(new Vector2(280, 450));
            path19.Add(new Vector2(330, 400));
            path19.Add(new Vector2(380, 350));
            path19.Add(new Vector2(380, 250));
            path19.Add(new Vector2(405, 175));
            path19.Add(new Vector2(430, 125));
            path19.Add(new Vector2(455, 100));
            path19.Add(new Vector2(530, 100));
            pathWayPoints.Add(path19);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path20 = new List<Vector2>();
            path20.Add(new Vector2(530, 200));
            path20.Add(new Vector2(455, 200));
            path20.Add(new Vector2(430, 225));
            path20.Add(new Vector2(405, 275));
            path20.Add(new Vector2(380, 350));
            path20.Add(new Vector2(380, 450));
            path20.Add(new Vector2(330, 500));
            path20.Add(new Vector2(280, 550));
            path20.Add(new Vector2(200, 550));
            path20.Add(new Vector2(150, 500));
            path20.Add(new Vector2(100, 450));
            path20.Add(new Vector2(100, 350));
            path20.Add(new Vector2(75, 275));
            path20.Add(new Vector2(50, 225));
            path20.Add(new Vector2(25, 200));
            path20.Add(new Vector2(-50, 200));
            pathWayPoints.Add(path20);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path21 = new List<Vector2>();
            path21.Add(new Vector2(-50, 500));
            path21.Add(new Vector2(100, 250));
            path21.Add(new Vector2(150, 175));
            path21.Add(new Vector2(200, 150));
            path21.Add(new Vector2(280, 150));
            path21.Add(new Vector2(330, 175));
            path21.Add(new Vector2(380, 250));
            path21.Add(new Vector2(530, 500));
            pathWayPoints.Add(path21);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path22 = new List<Vector2>();
            path22.Add(new Vector2(530, 550));
            path22.Add(new Vector2(380, 300));
            path22.Add(new Vector2(330, 250));
            path22.Add(new Vector2(280, 225));
            path22.Add(new Vector2(200, 225));
            path22.Add(new Vector2(150, 250));
            path22.Add(new Vector2(100, 300));
            path22.Add(new Vector2(-50, 550));
            pathWayPoints.Add(path22);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            // Half loop to bottom
            List<Vector2> path23 = new List<Vector2>();
            path23.Add(new Vector2(-50, -50));
            path23.Add(new Vector2(400, 400));
            path23.Add(new Vector2(430, 450));
            path23.Add(new Vector2(430, 475));
            path23.Add(new Vector2(400, 500));
            path23.Add(new Vector2(380, 525));
            path23.Add(new Vector2(260, 550));
            path23.Add(new Vector2(220, 550));
            path23.Add(new Vector2(180, 500));
            path23.Add(new Vector2(100, 400));
            path23.Add(new Vector2(50, 250));
            path23.Add(new Vector2(-50, 100));
            pathWayPoints.Add(path23);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path24 = new List<Vector2>();
            path24.Add(new Vector2(-50, -50));
            path24.Add(new Vector2(50, 400));
            path24.Add(new Vector2(100, 450));
            path24.Add(new Vector2(180, 475));
            path24.Add(new Vector2(220, 500));
            path24.Add(new Vector2(260, 525));
            path24.Add(new Vector2(380, 550));
            path24.Add(new Vector2(400, 550));
            path24.Add(new Vector2(430, 500));
            path24.Add(new Vector2(430, 400));
            path24.Add(new Vector2(400, 250));
            path24.Add(new Vector2(-50, 100));
            pathWayPoints.Add(path24);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            // Half loop to top

            List<Vector2> path25 = new List<Vector2>();
            path25.Add(new Vector2(-50, 600));
            path25.Add(new Vector2(50, 300));
            path25.Add(new Vector2(100, 250));
            path25.Add(new Vector2(180, 200));
            path25.Add(new Vector2(220, 175));
            path25.Add(new Vector2(260, 175));
            path25.Add(new Vector2(380, 200));
            path25.Add(new Vector2(400, 250));
            path25.Add(new Vector2(430, 350));
            path25.Add(new Vector2(430, 450));
            path25.Add(new Vector2(400, 500));
            path25.Add(new Vector2(100, 850));
            pathWayPoints.Add(path25);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path26 = new List<Vector2>();
            path26.Add(new Vector2(-50, 600));
            path26.Add(new Vector2(400, 450));
            path26.Add(new Vector2(430, 400));
            path26.Add(new Vector2(430, 350));
            path26.Add(new Vector2(400, 250));
            path26.Add(new Vector2(380, 200));
            path26.Add(new Vector2(260, 175));
            path26.Add(new Vector2(220, 175));
            path26.Add(new Vector2(180, 200));
            path26.Add(new Vector2(100, 250));
            path26.Add(new Vector2(50, 300));
            path26.Add(new Vector2(-50, 600));
            pathWayPoints.Add(path26);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            // Horizontal and back
            List<Vector2> path27 = new List<Vector2>();
            path27.Add(new Vector2(-50, 100));
            path27.Add(new Vector2(350, 100));
            path27.Add(new Vector2(400, 125));
            path27.Add(new Vector2(430, 150));
            path27.Add(new Vector2(430, 175));
            path27.Add(new Vector2(400, 200));
            path27.Add(new Vector2(350, 225));
            path27.Add(new Vector2(-50, 225));
            pathWayPoints.Add(path27);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path28 = new List<Vector2>();
            path28.Add(new Vector2(530, 275));
            path28.Add(new Vector2(130, 275));
            path28.Add(new Vector2(80, 250));
            path28.Add(new Vector2(50, 225));
            path28.Add(new Vector2(50, 200));
            path28.Add(new Vector2(80, 175));
            path28.Add(new Vector2(130, 150));
            path28.Add(new Vector2(530, 150));
            pathWayPoints.Add(path28);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path29 = new List<Vector2>();
            path29.Add(new Vector2(-50, 300));
            path29.Add(new Vector2(350, 300));
            path29.Add(new Vector2(400, 325));
            path29.Add(new Vector2(430, 350));
            path29.Add(new Vector2(430, 375));
            path29.Add(new Vector2(400, 400));
            path29.Add(new Vector2(350, 425));
            path29.Add(new Vector2(-50, 425));
            pathWayPoints.Add(path29);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path30 = new List<Vector2>();
            path30.Add(new Vector2(530, 475));
            path30.Add(new Vector2(130, 475));
            path30.Add(new Vector2(80, 450));
            path30.Add(new Vector2(50, 425));
            path30.Add(new Vector2(50, 400));
            path30.Add(new Vector2(80, 375));
            path30.Add(new Vector2(130, 350));
            path30.Add(new Vector2(530, 350));
            pathWayPoints.Add(path30);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            // Diagonal
            List<Vector2> path31 = new List<Vector2>();
            path31.Add(new Vector2(-50, 50)); 
            path31.Add(new Vector2(530, 600));
            pathWayPoints.Add(path31);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path32 = new List<Vector2>();
            path32.Add(new Vector2(530, 50));
            path32.Add(new Vector2(-50, 600));
            pathWayPoints.Add(path32);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path33 = new List<Vector2>();
            path33.Add(new Vector2(-50, 500));
            path33.Add(new Vector2(530, -50));
            pathWayPoints.Add(path33);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path34 = new List<Vector2>();
            path34.Add(new Vector2(530, -50));
            path34.Add(new Vector2(-50, 500));
            pathWayPoints.Add(path34);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            // L-Form
            List<Vector2> path35 = new List<Vector2>();
            path35.Add(new Vector2(260, -50));
            path35.Add(new Vector2(260, 400));
            path35.Add(new Vector2(280, 450));
            path35.Add(new Vector2(300, 475));
            path35.Add(new Vector2(330, 500));
            path35.Add(new Vector2(530, 500));
            pathWayPoints.Add(path35);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path36 = new List<Vector2>();
            path36.Add(new Vector2(220, -50));
            path36.Add(new Vector2(220, 400));
            path36.Add(new Vector2(200, 450));
            path36.Add(new Vector2(180, 475));
            path36.Add(new Vector2(150, 500));
            path36.Add(new Vector2(-50, 500));
            pathWayPoints.Add(path36);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path37 = new List<Vector2>();
            path37.Add(new Vector2(220, -50));
            path37.Add(new Vector2(220, 350));
            path37.Add(new Vector2(200, 400));
            path37.Add(new Vector2(180, 425));
            path37.Add(new Vector2(150, 450));
            path37.Add(new Vector2(100, 450));
            path37.Add(new Vector2(80, 425));
            path37.Add(new Vector2(50, 375));
            path37.Add(new Vector2(25, 300));
            path37.Add(new Vector2(50, 250));
            path37.Add(new Vector2(100, 200));
            path37.Add(new Vector2(530, -50));
            pathWayPoints.Add(path37);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path38 = new List<Vector2>();
            path38.Add(new Vector2(260, -50));
            path38.Add(new Vector2(260, 350));
            path38.Add(new Vector2(260, 400));
            path38.Add(new Vector2(300, 425));
            path38.Add(new Vector2(330, 450));
            path38.Add(new Vector2(380, 450));
            path38.Add(new Vector2(400, 425));
            path38.Add(new Vector2(430, 375));
            path38.Add(new Vector2(455, 300));
            path38.Add(new Vector2(430, 250));
            path38.Add(new Vector2(380, 200));
            path38.Add(new Vector2(-50, -50));
            pathWayPoints.Add(path38);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            // S-Form
            List<Vector2> path39 = new List<Vector2>();
            path39.Add(new Vector2(-50, 100));
            path39.Add(new Vector2(350, 100));
            path39.Add(new Vector2(400, 125));
            path39.Add(new Vector2(430, 150));
            path39.Add(new Vector2(430, 175));
            path39.Add(new Vector2(400, 200));
            path39.Add(new Vector2(350, 225));
            path39.Add(new Vector2(130, 225));
            path39.Add(new Vector2(80, 250));
            path39.Add(new Vector2(50, 275));
            path39.Add(new Vector2(50, 300));
            path39.Add(new Vector2(80, 325));
            path39.Add(new Vector2(130, 350));
            path39.Add(new Vector2(530, 350));
            pathWayPoints.Add(path39);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path40 = new List<Vector2>();
            path40.Add(new Vector2(530, 150));
            path40.Add(new Vector2(130, 150));
            path40.Add(new Vector2(80, 175));
            path40.Add(new Vector2(50, 200));
            path40.Add(new Vector2(50, 225));
            path40.Add(new Vector2(80, 250));
            path40.Add(new Vector2(130, 275));
            path40.Add(new Vector2(350, 275));
            path40.Add(new Vector2(400, 300));
            path40.Add(new Vector2(430, 325));
            path40.Add(new Vector2(430, 350));
            path40.Add(new Vector2(400, 375));
            path40.Add(new Vector2(350, 400));
            path40.Add(new Vector2(-50, 400));
            pathWayPoints.Add(path40);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            // Kurve
            List<Vector2> path41 = new List<Vector2>();
            path41.Add(new Vector2(-50, 75));
            path41.Add(new Vector2(240, 75));
            path41.Add(new Vector2(280, 85));
            path41.Add(new Vector2(320, 100));
            path41.Add(new Vector2(360, 125));
            path41.Add(new Vector2(400, 175));
            path41.Add(new Vector2(460, 850));
            pathWayPoints.Add(path41);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path42 = new List<Vector2>();
            path42.Add(new Vector2(530, 75));
            path42.Add(new Vector2(240, 75));
            path42.Add(new Vector2(200, 85));
            path42.Add(new Vector2(160, 100));
            path42.Add(new Vector2(120, 125));
            path42.Add(new Vector2(80, 175));
            path42.Add(new Vector2(20, 850));
            pathWayPoints.Add(path42);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path43 = new List<Vector2>();
            path43.Add(new Vector2(-50, 150));
            path43.Add(new Vector2(240, 150));
            path43.Add(new Vector2(280, 160));
            path43.Add(new Vector2(320, 175));
            path43.Add(new Vector2(360, 200));
            path43.Add(new Vector2(400, 250));
            path43.Add(new Vector2(530, 850));
            pathWayPoints.Add(path43);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path44 = new List<Vector2>();
            path44.Add(new Vector2(530, 150));
            path44.Add(new Vector2(240, 150));
            path44.Add(new Vector2(200, 160));
            path44.Add(new Vector2(160, 175));
            path44.Add(new Vector2(120, 200));
            path44.Add(new Vector2(80, 250));
            path44.Add(new Vector2(-50, 850));
            pathWayPoints.Add(path44);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path45 = new List<Vector2>();
            path45.Add(new Vector2(240, -50));
            path45.Add(new Vector2(240, 550));
            path45.Add(new Vector2(280, 600));
            path45.Add(new Vector2(320, 625));
            path45.Add(new Vector2(340, 625));
            path45.Add(new Vector2(380, 600));
            path45.Add(new Vector2(420, 550));
            path45.Add(new Vector2(420, 500));
            path45.Add(new Vector2(380, 450));
            path45.Add(new Vector2(340, 425));
            path45.Add(new Vector2(140, 425));
            path45.Add(new Vector2(100, 450));
            path45.Add(new Vector2(60, 500));
            path45.Add(new Vector2(60, 550));
            path45.Add(new Vector2(100, 600));
            path45.Add(new Vector2(140, 625));
            path45.Add(new Vector2(160, 625));
            path45.Add(new Vector2(200, 600));
            path45.Add(new Vector2(240, 550));
            path45.Add(new Vector2(240, -50));
            pathWayPoints.Add(path45);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path46 = new List<Vector2>();
            path46.Add(new Vector2(240, -50));
            path46.Add(new Vector2(240, 550));
            path46.Add(new Vector2(200, 600));
            path46.Add(new Vector2(160, 625));
            path46.Add(new Vector2(140, 625));
            path46.Add(new Vector2(100, 600));
            path46.Add(new Vector2(60, 550));
            path46.Add(new Vector2(60, 500));
            path46.Add(new Vector2(100, 450));
            path46.Add(new Vector2(140, 425));
            path46.Add(new Vector2(140, 425));
            path46.Add(new Vector2(380, 450));
            path46.Add(new Vector2(420, 500));
            path46.Add(new Vector2(420, 550));
            path46.Add(new Vector2(380, 600));
            path46.Add(new Vector2(340, 625));
            path46.Add(new Vector2(320, 625));
            path46.Add(new Vector2(280, 600));
            path46.Add(new Vector2(240, 550));
            path46.Add(new Vector2(240, -50));
            pathWayPoints.Add(path46);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path47 = new List<Vector2>();
            path47.Add(new Vector2(240, -50));
            path47.Add(new Vector2(240, 350));
            path47.Add(new Vector2(280, 400));
            path47.Add(new Vector2(320, 425));
            path47.Add(new Vector2(340, 425));
            path47.Add(new Vector2(380, 400));
            path47.Add(new Vector2(420, 350));
            path47.Add(new Vector2(420, 300));
            path47.Add(new Vector2(380, 250));
            path47.Add(new Vector2(340, 225));
            path47.Add(new Vector2(140, 225));
            path47.Add(new Vector2(100, 250));
            path47.Add(new Vector2(60, 300));
            path47.Add(new Vector2(60, 350));
            path47.Add(new Vector2(100, 400));
            path47.Add(new Vector2(140, 425));
            path47.Add(new Vector2(160, 425));
            path47.Add(new Vector2(200, 400));
            path47.Add(new Vector2(240, 350));
            path47.Add(new Vector2(240, -50));
            pathWayPoints.Add(path47);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path48 = new List<Vector2>();
            path48.Add(new Vector2(240, -50));
            path48.Add(new Vector2(240, 350));
            path48.Add(new Vector2(200, 400));
            path48.Add(new Vector2(160, 425));
            path48.Add(new Vector2(140, 425));
            path48.Add(new Vector2(100, 400));
            path48.Add(new Vector2(60, 350));
            path48.Add(new Vector2(60, 300));
            path48.Add(new Vector2(100, 250));
            path48.Add(new Vector2(140, 225));
            path48.Add(new Vector2(140, 225));
            path48.Add(new Vector2(380, 250));
            path48.Add(new Vector2(420, 300));
            path48.Add(new Vector2(420, 350));
            path48.Add(new Vector2(380, 400));
            path48.Add(new Vector2(340, 425));
            path48.Add(new Vector2(320, 425));
            path48.Add(new Vector2(280, 400));
            path48.Add(new Vector2(240, 350));
            path48.Add(new Vector2(240, -50));
            pathWayPoints.Add(path48);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));
        }

        public void SpawnEnemy(int path, EnemyType type)
        {
            Enemy newEnemy;

            switch (type)
            {
                case EnemyType.Medium:
                    newEnemy = Enemy.CreateMediumEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;

                case EnemyType.Hard:
                    newEnemy = Enemy.CreateHardEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;

                case EnemyType.Speeder:
                    newEnemy = Enemy.CreateSpeederEnemy(texture,
                                                        pathWayPoints[path][0]);
                    break;

                case EnemyType.Tank:
                    newEnemy = Enemy.CreateTankEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;

                default:
                    newEnemy = Enemy.CreateEasyEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;
            }

            newEnemy.SetLevel(currentLevel);

            for (int x = 0; x < pathWayPoints[path].Count; x++)
            {
                newEnemy.AddWayPoint(pathWayPoints[path][x]);
            }

            Enemies.Add(newEnemy);
        }

        public void SpawnWave(int waveType)
        {
            int spawns = rand.Next(MinShipsPerWave, MaxShipsPerWave + 1);

            EnemyType type;
            int rnd = rand.Next(0, 5);

            switch (rnd)
            {
                case 0:
                    type = EnemyType.Medium;
                    break;

                case 1:
                    type = EnemyType.Hard;
                    break;

                case 2:
                    type = EnemyType.Speeder;
                    break;

                case 3:
                    type = EnemyType.Tank;
                    break;

                default:
                    type = EnemyType.Easy;
                    break;
            }

            waveSpawns[waveType] = new WaveInfo(spawns, type);
        }

        private void updateWaveSpawns(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            shipSpawnTimer += elapsed;

            if (shipSpawnTimer >= shipSpawnWaitTimer)
            {
                for (int x = waveSpawns.Count - 1; x >= 0; --x)
                {
                    if (waveSpawns[x].SpawnsCount > 0)
                    {
                        waveSpawns[x].DecrementSpawns();

                        SpawnEnemy(x, waveSpawns[x].Type);
                    }
                }

                shipSpawnTimer = 0.0f;
            }

            nextWaveTimer += elapsed;

            if (nextWaveTimer > nextWaveMinTimer)
            {
                int rnd1 = rand.Next(0, pathWayPoints.Count);
                int rnd2 = rand.Next(0, pathWayPoints.Count);

                int rndSecondSpawn = rand.Next(0, 50 / (currentLevel));
                // Lvl conforms to probability!

                SpawnWave(rnd1);

                if (rndSecondSpawn == 0 && rnd1 != rnd2)
                    SpawnWave(rnd2);

                nextWaveTimer = 0.0f;
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            EnemyShotManager.Update(gameTime);

            for (int x = Enemies.Count - 1; x >= 0; --x)
            {
                Enemy enemy = Enemies[x];

                enemy.Update(gameTime);

                if (!enemy.IsActive())
                {
                    Enemies.RemoveAt(x);
                }
                else
                {
                    float rndShot = (float)rand.Next(0, ((int)(2000))) / 10;

                    if (rndShot <= enemy.ShotChance &&
                        !playerManager.IsDestroyed &&
                         screen.Contains((int)enemy.EnemySprite.Center.X,
                                         (int)enemy.EnemySprite.Center.Y))
                    {
                        Vector2 fireLocation = enemy.EnemySprite.Location;
                        fireLocation += enemy.GunOffset;

                        Vector2 shotDirection = ((playerManager.playerSprite.Center + playerManager.playerSprite.Velocity / (2.0f + (float)rand.NextDouble() * 8.0f)) - fireLocation);

                        shotDirection.Normalize();

                        if (enemy.Type == EnemyType.Tank &&
                            (float)rand.Next(0, 7) == 0) // 14.2%
                        {
                            EnemyShotManager.FireRocket(fireLocation,
                                                             shotDirection,
                                                             false,
                                                             Color.White,
                                                             true);
                        }
                        else
                        {
                            EnemyShotManager.FireShot(fireLocation,
                                                      shotDirection,
                                                      false,
                                                      new Color(1.0f, 0.1f, 0.1f),
                                                      true);
                        }
                    }
                }
            }

            if (this.IsActive)
            {
                updateWaveSpawns(gameTime);
            }

            updateHarakiries(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyShotManager.Draw(spriteBatch);

            foreach (var enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }

            drawHarakiries(spriteBatch);
        }

        public void Reset()
        {
            this.Enemies.Clear();
            this.EnemyShotManager.Shots.Clear();

            this.nextWaveTimer = 0.0f;
            this.shipSpawnTimer = 0.0f;

            this.nextWaveMinTimer = InitialNextWaveMinTimer;

            for (int i = 0; i < waveSpawns.Count; i++)
            {
                waveSpawns[i] = new WaveInfo(0, EnemyType.Easy);
            }

            this.IsActive = true;

            this.Harakiries.Clear();
            resetHarakiriSpawnTimer();
        }

        public void SetLevel(int lvl)
        {
            this.currentLevel = lvl;

            float tmp = (int)(InitialNextWaveMinTimer - (float)Math.Sqrt(lvl - 1) * 0.075f + 0.02 * (lvl - 1)); // 5 - WURZEL(A2-1) / 2 * 0,15 - 0,02 * (A2 - 1)

            this.nextWaveMinTimer = Math.Max(tmp, 1.0f);
        }

        #region Harakiri

        private void resetHarakiriSpawnTimer()
        {
            this.harakiriSpawnTimer = INIT_HARAKIRI_SPAWNTIME_MIN
                + (float)rand.NextDouble() * (INIT_HARAKIRI_SPAWNTIME_MAX - INIT_HARAKIRI_SPAWNTIME_MIN);
        }

        private Vector2 randomLocation()
        {
            Vector2 location = Vector2.Zero;
            
            switch (rand.Next(0, 3))
            {
                case 0:
                    location.X = -50;
                    location.Y = rand.Next(0, screen.Height / 3);
                    break;

                case 1:
                    location.X = screen.Width;
                    location.Y = rand.Next(0, screen.Height / 3);
                    break;

                case 2:
                    location.X = rand.Next(0, screen.Width);
                    location.Y = -50;
                    break;
            }

            return location;
        }

        private void spawnHarakiri()
        {
            if (IsActive)
            {
                HarakiriEnemy enemy;
                
                int rnd = rand.Next(5);

                switch (rnd)
                {
                    case 0:
                    case 1:
                        enemy = HarakiriEnemy.CreateMediumEnemy(texture, randomLocation());
                        break;

                    case 2:
                        enemy = HarakiriEnemy.CreateHardEnemy(texture, randomLocation());
                        break;

                    default:
                        enemy = HarakiriEnemy.CreateEasyEnemy(texture, randomLocation());
                        break;
                }
                
                Harakiries.Add(enemy);
            }
        }

        private void updateHarakiries(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            harakiriSpawnTimer -= elapsed;

            if (harakiriSpawnTimer <= 0.0f)
            {
                spawnHarakiri();
                resetHarakiriSpawnTimer();
            }

            harakiriCampShotTimer += elapsed;

            for (int i = Harakiries.Count - 1; i >= 0; --i)
            {
                HarakiriEnemy harakiri = Harakiries[i];

                if (!playerManager.IsDestroyed)
                    harakiri.CurrentTarget = playerManager.playerSprite.Center;
                else
                    harakiri.CurrentTarget = Vector2.Zero; // to indicate the enemy that there is no player!

                harakiri.Update(gameTime);

                if (!harakiri.IsActive())
                {
                    Harakiries.RemoveAt(i);
                }
                else
                {
                    bool fire = false;
                    
                    if (harakiri.EnemySprite.Velocity.Length() < 7.5f)
                    {
                        if (harakiriCampShotTimer >= HarakiriCampShotTimerMin)
                        {
                            harakiriCampShotTimer = 0.0f;
                            fire = true;
                        }
                    }
                    else if (harakiri.EnemySprite.Velocity.Length() > 15.0f)
                    {
                        if (harakiriCampShotTimer >= HarakiriCampShotTimerMin)
                        {
                            float rndShot = (float)rand.Next(0, ((int)(2000))) / 10;

                            if (rndShot <= Harakiries[i].ShotChance &&
                                !playerManager.IsDestroyed &&
                                 screen.Contains((int)harakiri.EnemySprite.Center.X,
                                                 (int)harakiri.EnemySprite.Center.Y))
                            {
                                harakiriCampShotTimer = 0.0f;
                                fire = true;
                            } 
                        }
                    }

                    if (fire)
                    {
                        Vector2 fireLocation = harakiri.EnemySprite.Center;

                        Vector2 shotDirection = ((playerManager.playerSprite.Center + playerManager.playerSprite.Velocity / (1.75f + (float)rand.NextDouble() * 3.25f)) - fireLocation);

                        shotDirection.Normalize();

                        Matrix m = new Matrix();
                        m.M11 = (float)Math.Cos(Math.PI / 2);
                        m.M12 = (float)-Math.Sin(Math.PI / 2);
                        m.M21 = (float)Math.Sin(Math.PI / 2);
                        m.M22 = (float)Math.Cos(Math.PI / 2);

                        Vector2 fireLocationOffset8 = Vector2.Transform(shotDirection, m) * 8;

                        switch (harakiri.Type)
                        {
                            case EnemyType.Medium:
                                EnemyShotManager.FireShot(fireLocation + fireLocationOffset8,
                                                      shotDirection,
                                                      HarakiriEnemy.ShotSpeed,
                                                      false,
                                                      new Color(1.0f, 0.1f, 0.1f),
                                                      true);
                                EnemyShotManager.FireShot(fireLocation - fireLocationOffset8,
                                                      shotDirection,
                                                      HarakiriEnemy.ShotSpeed,
                                                      false,
                                                      new Color(1.0f, 0.1f, 0.1f),
                                                      true);
                                break;
                            case EnemyType.Hard:
                                EnemyShotManager.FireRocket(fireLocation,
                                                            shotDirection,
                                                            false,
                                                            Color.White,
                                                            true);
                                break;
                            default:
                                // nothing
                                break;
                        }
                    }
                }
            }
        }

        public void drawHarakiries(SpriteBatch spriteBatch)
        {   
            foreach (var enemy in Harakiries)
            {
                enemy.Draw(spriteBatch);
            }
        }

        #endregion

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Enemies
            int enemiesCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < enemiesCount; ++i)
            {
                EnemyType type = EnemyType.Easy;
                Enemy e;

                type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

                switch (type)
                {
                    case EnemyType.Easy:
                        e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Medium:
                        e = Enemy.CreateMediumEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Hard:
                        e = Enemy.CreateHardEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Speeder:
                        e = Enemy.CreateSpeederEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Tank:
                        e = Enemy.CreateTankEnemy(texture, Vector2.Zero);
                        break;
                    default:
                        e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                }
                e.Activated(reader);

                Enemies.Add(e);
            }

            EnemyShotManager.Activated(reader);

            this.MinShipsPerWave = Int32.Parse(reader.ReadLine());
            this.MaxShipsPerWave = Int32.Parse(reader.ReadLine());

            this.nextWaveTimer = Single.Parse(reader.ReadLine());
            this.nextWaveMinTimer = Single.Parse(reader.ReadLine());
            this.shipSpawnTimer = Single.Parse(reader.ReadLine());
            this.shipSpawnWaitTimer = Single.Parse(reader.ReadLine());

            // Wave spawns
            int waveSpawnsCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < waveSpawnsCount; ++i)
            {
                int idx = Int32.Parse(reader.ReadLine());
                WaveInfo waveInfo = new WaveInfo();
                waveInfo.Activated(reader);
                waveSpawns[idx] = waveInfo;
            }

            this.IsActive = Boolean.Parse(reader.ReadLine());

            this.currentLevel = Int32.Parse(reader.ReadLine());

            // Harakiries
            int harakiriesCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < harakiriesCount; ++i)
            {
                EnemyType type = EnemyType.Easy;
                HarakiriEnemy e;

                type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

                switch (type)
                {
                    case EnemyType.Medium:
                        e = HarakiriEnemy.CreateMediumEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Hard:
                        e = HarakiriEnemy.CreateHardEnemy(texture, Vector2.Zero);
                        break;
                    default:
                        e = HarakiriEnemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                }
                e.Activated(reader);

                Harakiries.Add(e);
            }

            this.harakiriSpawnTimer = Single.Parse(reader.ReadLine());

            this.harakiriCampShotTimer = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            //Enemies
            writer.WriteLine(Enemies.Count);

            for (int i = 0; i < Enemies.Count; ++i)
            {
                writer.WriteLine(Enemies[i].Type);
                Enemies[i].Deactivated(writer);
            }

            EnemyShotManager.Deactivated(writer);

            writer.WriteLine(MinShipsPerWave);
            writer.WriteLine(MaxShipsPerWave);

            writer.WriteLine(nextWaveTimer);
            writer.WriteLine(nextWaveMinTimer);
            writer.WriteLine(shipSpawnTimer);
            writer.WriteLine(shipSpawnWaitTimer);

            // Wave spawns
            writer.WriteLine(waveSpawns.Count);

            foreach (var waveSpawn in waveSpawns)
            {
                writer.WriteLine(waveSpawn.Key);
                waveSpawn.Value.Deactivated(writer);
            }

            writer.WriteLine(IsActive);

            writer.WriteLine(currentLevel);

            //Enemies
            writer.WriteLine(Harakiries.Count);

            for (int i = 0; i < Harakiries.Count; ++i)
            {
                writer.WriteLine(Harakiries[i].Type);
                Harakiries[i].Deactivated(writer);
            }

            writer.WriteLine(harakiriSpawnTimer);

            writer.WriteLine(harakiriCampShotTimer);
        }

        #endregion
    }
}
