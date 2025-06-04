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
        private void HandleScreenWrapping(Size worldSize)
        {
            if (player.Position.X < -Player.Width)
            {
                player.Position = new PointF(worldSize.Width, player.Position.Y);
            }
            else if (player.Position.X > worldSize.Width)
            {
                player.Position = new PointF(-Player.Width, player.Position.Y);
            }
        }

        private void AddRandomPlatform(int worldWidth)
        {
            const int margin = 20;
            const int platformWidth = 60; 
            float x = rnd.Next(margin, worldWidth - margin - platformWidth);

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

        public void Update(Size worldSize)
        {
            if (isGameOver) return;

            player.IsOnGround = false;
            player.Update();

            HandleScreenWrapping(worldSize);

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
                ScrollWorld(dy, worldSize);
                score += (int)dy;
                ScoreUpdated?.Invoke(score);
            }

            if (player.Position.Y > worldSize.Height)
            {
                EndGame();
            }

            int visibleCount = platforms.Count(p => p.Position.Y >= 0 && p.Position.Y <= worldSize.Height);
            while (visibleCount < minVisiblePlatforms)
            {
                AddRandomPlatform(worldSize.Width);
                visibleCount++;
            }
        }

        private void ScrollWorld(float dy, Size worldSize)
        {
            player.Position = new PointF(player.Position.X, ScrollTriggerY);

            foreach (var p in platforms)
            {
                p.Position = new PointF(p.Position.X, p.Position.Y + dy);
            }

            platforms.RemoveAll(p => p.Position.Y > worldSize.Height + 50);
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

        private void EndGame()
        {
            isGameOver = true;
            GameOver?.Invoke();
        }
    }
}