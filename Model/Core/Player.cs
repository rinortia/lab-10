using System;
using System.Drawing;
using System.IO;

namespace Model.Core
{
    public class Player
    {
        public PointF Position { get; set; }
        public bool IsOnGround { get; set; }
        public float VelocityY { get; set; }
        public float VelocityX { get; set; } // Новая переменная для горизонтальной скорости

        public const float Gravity = 0.5f;
        private const float MaxFallSpeed = 12f;
        private const float MoveSpeed = 5f;
        private const float AirResistance = 0.8f; // Сопротивление воздуха в прыжке
        private const float GroundFriction = 0.9f; // Трение о землю
        private const float Acceleration = 0.8f; // Ускорение при движении

        public const float InitialJumpForce = 14f;
        public const int Width = 70;
        public const int Height = 70;

        private static Image playerImage;

        static Player()
        {
            LoadImage();
        }

        public Player(float x, float y)
        {
            Position = new PointF(x, y);
            VelocityY = 0;
            VelocityX = 0;
        }

        private static void LoadImage()
        {
            try
            {
                string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                string path = Path.Combine(resourcesPath, "kitten.png");

                if (!File.Exists(path))
                {
                    Console.WriteLine($"[Player] Изображение не найдено: {path}");
                    return;
                }

                playerImage = Image.FromFile(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Player] Ошибка загрузки изображения: {ex.Message}");
            }
        }

        public void Update()
        {
            // Вертикальное движение с улучшенной физикой
            if (!IsOnGround)
            {
                VelocityY += Gravity;

                // Дополнительное ускорение при падении
                if (VelocityY > 0)
                    VelocityY *= 1.05f;
            }
            else
            {
                // Трение о землю
                VelocityX *= GroundFriction;
            }

            // Ограничиваем скорость падения
            if (VelocityY > MaxFallSpeed)
                VelocityY = MaxFallSpeed;

            // Обновляем позицию
            Position = new PointF(Position.X + VelocityX, Position.Y + VelocityY);
        }

        public void ApplyJumpForce(float force)
        {
            VelocityY = -force;
            IsOnGround = false;

            // Сопротивление воздуха при прыжке
            VelocityX *= AirResistance;
        }

        public void Jump()
        {
            if (IsOnGround)
                ApplyJumpForce(InitialJumpForce);
        }

        public void MoveLeft()
        {
            if (IsOnGround)
                VelocityX = -MoveSpeed;
            else
                VelocityX = -MoveSpeed * 0.7f; // Меньший контроль в воздухе

            // Плавное ускорение
            VelocityX = Math.Max(VelocityX - Acceleration, -MoveSpeed * 1.5f);
        }

        public void MoveRight()
        {
            if (IsOnGround)
                VelocityX = MoveSpeed;
            else
                VelocityX = MoveSpeed * 0.7f; // Меньший контроль в воздухе

            // Плавное ускорение
            VelocityX = Math.Min(VelocityX + Acceleration, MoveSpeed * 1.5f);
        }

        public void Draw(Graphics g)
        {
            if (playerImage != null)
                g.DrawImage(playerImage, Position.X, Position.Y, Width, Height);
            else
                g.FillEllipse(Brushes.Green, Position.X, Position.Y, Width, Height);
        }

        public RectangleF GetBounds()
        {
            return new RectangleF(Position.X, Position.Y, Width, Height);
        }

        public bool CheckPlatformCollision(HighJumpPlatform platform)
        {
            RectangleF playerBounds = GetBounds();
            RectangleF platformBounds = new RectangleF(platform.Position, platform.Size);

            bool vertical = playerBounds.Bottom >= platformBounds.Top &&
                          playerBounds.Bottom <= platformBounds.Top + 10;

            bool horizontal = playerBounds.Right > platformBounds.Left &&
                            playerBounds.Left < platformBounds.Right;

            if (vertical && horizontal)
            {
                IsOnGround = true;
                Position = new PointF(Position.X, platformBounds.Top - Height);
                return true;
            }

            return false;
        }
    }
}
