using System;
using System.Drawing;
using System.IO;

namespace Model.Core
{
    public class Player
    {
        private const float Gravity = 0.3f;
        private const float MaxFallSpeed = 15f;
        private const float MoveSpeed = 3f;
        private const float AirResistance = 0.5f;
        private const float GroundFriction = 0.6f;
        private const float FallAcceleration = 0.05f;
        public const float InitialJumpForce = 10f;
        public const int Width = 70;
        public const int Height = 70;

        private static Image _playerImage;

        public PointF Position { get; set; }
        public bool IsOnGround { get; set; }
        public float VelocityY { get; set; }
        public float VelocityX { get; set; }

        static Player()
        {
            LoadPlayerImage();
        }

        public Player(float x, float y)
        {
            Position = new PointF(x, y);
            VelocityY = 0;
            VelocityX = 0;
        }

        private static void LoadPlayerImage()
        {
            try
            {
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "kitten.png");
                if (File.Exists(imagePath))
                {
                    _playerImage = Image.FromFile(imagePath);
                }
            }
            catch
            {
                // Игнорируем ошибки загрузки изображения
            }
        }

        public void Update()
        {
            ApplyGravity();
            ApplyMovement();
            LimitFallSpeed();
        }

        private void ApplyGravity()
        {
            if (!IsOnGround)
            {
                VelocityY += Gravity;
                if (VelocityY > 0)
                {
                    VelocityY += FallAcceleration;
                }
            }
            else
            {
                VelocityX *= GroundFriction;
            }
        }

        private void ApplyMovement()
        {
            Position = new PointF(Position.X + VelocityX, Position.Y + VelocityY);
        }

        private void LimitFallSpeed()
        {
            VelocityY = Math.Min(VelocityY, MaxFallSpeed);
        }

        public void Jump()
        {
            if (IsOnGround)
            {
                ApplyJumpForce(InitialJumpForce);
            }
        }

        public void ApplyJumpForce(float force)
        {
            VelocityY = -force;
            IsOnGround = false;
            VelocityX *= AirResistance;
        }

        public void MoveLeft()
        {
            VelocityX = IsOnGround ? -MoveSpeed : -MoveSpeed * 0.7f;
        }

        public void MoveRight()
        {
            VelocityX = IsOnGround ? MoveSpeed : MoveSpeed * 0.7f;
        }

        public void Draw(Graphics g)
        {
            if (_playerImage != null)
            {
                g.DrawImage(_playerImage, Position.X, Position.Y, Width, Height);
            }
            else
            {
                g.FillEllipse(Brushes.Green, Position.X, Position.Y, Width, Height);
            }
        }

        public RectangleF GetBounds()
        {
            return new RectangleF(Position.X, Position.Y, Width, Height);
        }

        public bool CheckPlatformCollision(PlatformBase platform)
        {
            var playerBounds = GetBounds();
            var platformBounds = new RectangleF(platform.Position, platform.Size);

            bool isVerticalCollision = playerBounds.Bottom >= platformBounds.Top &&
                                     playerBounds.Bottom <= platformBounds.Top + 10;

            bool isHorizontalCollision = playerBounds.Right > platformBounds.Left &&
                                        playerBounds.Left < platformBounds.Right;

            if (isVerticalCollision && isHorizontalCollision)
            {
                HandlePlatformCollision(platformBounds.Top);
                return true;
            }

            return false;
        }

        private void HandlePlatformCollision(float platformTop)
        {
            IsOnGround = true;
            Position = new PointF(Position.X, platformTop - Height);
        }
    }
}