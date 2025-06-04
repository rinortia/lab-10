using Model.Core;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model.Core
{
    public partial class GameWorld
    {
        private Player player;
        private readonly Random rnd = new Random();
        private List<IPlatform> platforms;
        private int platformsCreated = 0;
        private float nextPlatformY = -40;
        private float lastPlatformX = 150;
        private readonly float minPlatformSpacing = 40f;
        private readonly float maxPlatformSpacing = 100f;
        private readonly int minVisiblePlatforms = 13;
        private readonly float maxJumpHeight = 230f;
        private readonly float ScrollTriggerY = 250f; 
        private int score;
        private bool isGameOver = false;

        public event Action<int> ScoreUpdated;
        public event Action GameOver;

        public Player Player => player;
        public IReadOnlyList<IPlatform> Platforms => platforms;
        public int Score => score;
        public bool IsGameOver => isGameOver;

        public GameWorld(Player player)
        {
            this.player = player;
            platforms = new List<IPlatform>();
        }

        public void StartNewGame(Size worldSize)
        {
            platforms.Clear();
            platformsCreated = 0;
            nextPlatformY = -40;
            lastPlatformX = 150;
            score = 0;
            isGameOver = false;

            float startPlatformY = worldSize.Height - 20;
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
                float x = rnd.Next(50, worldSize.Width - 100);
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
        }

        public void LoadGameState(string saveFormat, Size worldSize, string saveFolder)
        {
            try
            {
                var state = SaveManager.Load(saveFormat, saveFolder);
                if (state == null || state.Platforms == null || state.Platforms.Count == 0)
                {
                    StartNewGame(worldSize);
                    return;
                }

                player = new Player(state.PlayerPosition.X, state.PlayerPosition.Y)
                {
                    VelocityY = state.IsPlayerOnGround ? 0f : Math.Max(-10f, Math.Min(10f, state.PlayerVelocityY)),
                    VelocityX = 0f,
                    IsOnGround = state.IsPlayerOnGround
                };

                platforms = state.Platforms
                    .Where(p => p != null && p.Y >= -50 && p.Y <= worldSize.Height + 50)
                    .Select(p => CreatePlatform(p))
                    .ToList();

                if (platforms.Count == 0)
                {
                    StartNewGame(worldSize);
                    return;
                }

                score = state.Score;
                platformsCreated = platforms.Count;
                nextPlatformY = platforms.Min(p => p.Position.Y) - minPlatformSpacing;
                lastPlatformX = platforms.Last().Position.X;

                var nearestPlatform = platforms.OrderBy(p => Math.Abs(p.Position.Y - player.Position.Y)).First();
                player.Position = new PointF(
                    Math.Max(0, Math.Min(worldSize.Width - Player.Width, nearestPlatform.Position.X)),
                    nearestPlatform.Position.Y - Player.Height
                );
                player.IsOnGround = true;
                player.VelocityY = 0f;

                isGameOver = false;

                Console.WriteLine($"Loaded: Player Y={player.Position.Y}, VelocityY={player.VelocityY}, Platforms={platforms.Count}");
            }
            catch (Exception ex)
            {
                StartNewGame(worldSize);
                Console.WriteLine($"Ошибка загрузки: {ex.Message}");
            }
        }

        public void SaveGameState(string saveFormat, string saveFolder)
        {
            try
            {
                var gameState = new GameState
                {
                    PlayerPosition = player.Position,
                    PlayerVelocityY = player.VelocityY,
                    IsPlayerOnGround = player.IsOnGround,
                    Score = score,
                    Platforms = platforms.Select(p => new PlatformData
                    {
                        X = p.Position.X,
                        Y = p.Position.Y,
                        Size = p.Size,
                        Type = GetPlatformType(p),
                        IsActive = !(p is BreakablePlatform breakable) || breakable.IsActive
                    }).ToList()
                };

                SaveManager.Save(gameState, saveFormat, saveFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
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
                    return new BreakablePlatform(data.X, data.Y) { IsActive = data.IsActive };
                case PlatformType.HighJump:
                    return new HighJumpPlatform(data.X, data.Y);
                default:
                    return new NormalPlatform(data.X, data.Y);
            }
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
    }
}