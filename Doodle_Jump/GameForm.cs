using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Model.Core;
using Model.Data;
using Newtonsoft.Json;

namespace Doodle_Jump
{
    public partial class GameForm : Form
    {
        private Player player;
        private const int ScrollTriggerY = 250;
        private int score;
        private readonly Random rnd = new Random();
        private List<IPlatform> platforms;
        private int platformsCreated = 0;
        private float nextPlatformY = -40;
        private float lastPlatformX = 150;
        private readonly float minPlatformSpacing = 40f;
        private readonly float maxPlatformSpacing = 100f;
        private readonly int minVisiblePlatforms = 13;
        private readonly float maxJumpHeight = 230f;
        private readonly bool loadSavedGame;
        private bool isGameOver = false;
        private readonly string saveFormat;

        public GameForm(bool loadSavedGame, string saveFormat = "JSON")
        {
            InitializeComponent();
            this.loadSavedGame = loadSavedGame;
            this.saveFormat = saveFormat;

            game_timer = new Timer();
            game_timer.Interval = 16;
            game_timer.Tick += GameTimer_Tick;

            ClientSize = new Size(400, 600);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            player = new Player(150, ClientSize.Height - 20 - Player.Height)
            {
                VelocityY = -10f,
                IsOnGround = false
            };
            platforms = new List<IPlatform>();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;

            if (loadSavedGame)
                LoadGame();
            else
                StartNewGame();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (isGameOver) return;

            player.IsOnGround = false;
            player.Update();

            if (player.Position.X < -Player.Width)
                player.Position = new PointF(ClientSize.Width, player.Position.Y);
            else if (player.Position.X > ClientSize.Width)
                player.Position = new PointF(-Player.Width, player.Position.Y);

            foreach (var p in platforms.ToList())
            {
                if (IsPlayerLandingOn(p))
                {
                    bool stillExists = p.OnLand(player);
                    if (!stillExists) platforms.Remove(p);
                }
            }

            if (player.Position.Y < ScrollTriggerY)
            {
                float dy = ScrollTriggerY - player.Position.Y;
                ScrollWorld(dy);
                score += (int)dy;
            }

            if (player.Position.Y > ClientSize.Height)
                GameOver();

            Invalidate();
        }

        private void SaveGame()
        {
            var state = new GameState
            {
                PlayerPosition = player.Position,
                Score = score,
                Platforms = platforms.Select(p => new PlatformData
                {
                    X = p.Position.X,
                    Y = p.Position.Y,
                    Type = GetPlatformType(p)
                }).ToList()
            };

            SaveManager.Save(state, saveFormat);
        }

        private void LoadGame()
        {
            var state = SaveManager.Load(saveFormat);
            if (state == null)
            {
                StartNewGame();
                return;
            }

            player.Position = state.PlayerPosition;
            score = state.Score;
            platforms = state.Platforms.Select(CreatePlatform).ToList();
            platformsCreated = platforms.Count;

            nextPlatformY = platforms.Min(p => p.Position.Y) - minPlatformSpacing;

            player.VelocityY = -(float)Math.Sqrt(2 * Player.Gravity * maxJumpHeight);
            player.IsOnGround = false;

            game_timer.Start();
        }

        private PlatformType GetPlatformType(IPlatform platform)
        {
            if (platform is BreakablePlatform) return PlatformType.Breakable;
            if (platform is HighJumpPlatform) return PlatformType.HighJump;
            return PlatformType.Normal;
        }

        private IPlatform CreatePlatform(PlatformData data)
        {
            switch (data.Type)
            {
                case PlatformType.Breakable:
                    return new BreakablePlatform(data.X, data.Y);
                case PlatformType.HighJump:
                    return new HighJumpPlatform(data.X, data.Y);
                default:
                    return new NormalPlatform(data.X, data.Y);
            }
        }

        private void StartNewGame()
        {
            platforms.Clear();
            platformsCreated = 0;
            nextPlatformY = -40;
            lastPlatformX = 150;
            score = 0;

            float startPlatformY = ClientSize.Height - 20;
            platforms.Add(new NormalPlatform(150, startPlatformY));
            platformsCreated++;

            player = new Player(150, startPlatformY - Player.Height)
            {
                VelocityY = -10f,
                IsOnGround = false
            };

            float currentY = startPlatformY;

            for (int i = 0; i < minVisiblePlatforms; i++)
            {
                float x = rnd.Next(50, ClientSize.Width - 100);
                float spacing;

                do
                {
                    spacing = (float)(rnd.NextDouble() * (maxPlatformSpacing - minPlatformSpacing) + minPlatformSpacing);
                }
                while (spacing > maxJumpHeight);

                currentY -= spacing;
                platforms.Add(new NormalPlatform(x, currentY));
                platformsCreated++;
                lastPlatformX = x;
                nextPlatformY = currentY - spacing;
            }

            game_timer.Start();
        }

        private void ScrollWorld(float dy)
        {
            player.Position = new PointF(player.Position.X, ScrollTriggerY);

            foreach (var p in platforms)
                p.Position = new PointF(p.Position.X, p.Position.Y + dy);

            platforms.RemoveAll(p => p.Position.Y > ClientSize.Height + 50);

            int visibleCount = platforms.Count(p => p.Position.Y >= 0 && p.Position.Y <= ClientSize.Height);
            while (visibleCount < minVisiblePlatforms)
            {
                AddRandomPlatform();
                visibleCount++;
            }
        }

        private void AddRandomPlatform()
        {
            float x = rnd.Next(50, ClientSize.Width - 50);
            float spacing;

            do
            {
                spacing = (float)(rnd.NextDouble() * (maxPlatformSpacing - minPlatformSpacing) + minPlatformSpacing);
            }
            while (spacing > maxJumpHeight);

            float y = nextPlatformY;
            IPlatform newPlatform = CreateNewPlatform(x, y);

            platforms.Add(newPlatform);
            platformsCreated++;
            nextPlatformY -= spacing;
        }

        private IPlatform CreateNewPlatform(float x, float y)
        {
            if (platformsCreated < 6)
                return new NormalPlatform(x, y);

            double chance = rnd.NextDouble();

            if (chance < 0.2)
                return new BreakablePlatform(x, y);
            if (chance < 0.35)
                return new HighJumpPlatform(x, y);

            return new NormalPlatform(x, y);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.A)
                player.MoveLeft();
            else if (keyData == Keys.Right || keyData == Keys.D)
                player.MoveRight();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            foreach (var p in platforms)
                p.Draw(g);

            player.Draw(g);
            g.DrawString($"Score: {score}", Font, Brushes.Black, 10, 10);
        }

        private bool IsPlayerLandingOn(IPlatform p)
        {
            RectangleF pr = player.GetBounds();
            RectangleF pl = new RectangleF(p.Position, p.Size);

            bool verticalHit = player.VelocityY > 0 &&
                               pr.Bottom >= pl.Top &&
                               pr.Bottom <= pl.Top + player.VelocityY + 5;

            bool horizontalOverlap = pr.Right > pl.Left && pr.Left < pl.Right;

            if (verticalHit && horizontalOverlap)
            {
                player.IsOnGround = true;
                player.VelocityY = 0;
                player.Position = new PointF(player.Position.X, pl.Top - Player.Height);

                if (p is HighJumpPlatform highJumpPlatform)
                    highJumpPlatform.IsActive = true;

                return true;
            }
            return false;
        }

        private void GameOver()
        {
            if (isGameOver) return;

            isGameOver = true;
            game_timer.Stop();

            SaveGame();

            if (this.IsHandleCreated)
            {
                MessageBox.Show($"Игра окончена!\nСчёт: {score}", "Doodle Jump");
            }

            this.Close();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveGame();

            player = null;
            platforms.Clear();
            game_timer.Dispose();

            if (!isGameOver)
            {
                var menu = new MainMenuForm();
                menu.Show();
            }
        }
    }
}
