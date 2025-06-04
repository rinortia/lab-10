using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Model.Core
{
    public class BreakablePlatform : PlatformBase
    {
        private bool _used = false;

        public bool IsActive { get; set; } = true;

        public BreakablePlatform(float x, float y) : base(x, y) { }

        protected override Brush PlatformBrush => Brushes.IndianRed;

        public override bool OnLand(Player player)
        {
            if (!_used && IsActive)
            {
                _used = true;
                IsActive = false;
                player.Jump();
                return false;
            }
            return false;
        }

        public override void Draw(Graphics g)
        {
            if (IsActive)
            {
                base.Draw(g);

                using (var pen = new Pen(Color.DarkRed, 2))
                {
                    g.DrawLine(pen,
                        Position.X + 5, Position.Y + 3,
                        Position.X + Size.Width - 5, Position.Y + Size.Height - 3);
                    g.DrawLine(pen,
                        Position.X + Size.Width - 5, Position.Y + 3,
                        Position.X + 5, Position.Y + Size.Height - 3);
                }
            }
        }
    }
}