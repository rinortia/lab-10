using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public class HighJumpPlatform : IPlatform
    {
        public PointF Position { get; set; }
        public SizeF Size { get; } = new SizeF(60, 10);
        public Color Color { get; } = Color.Green;
        public bool IsActive { get; set; } = false;

        public HighJumpPlatform(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color), Position.X, Position.Y, Size.Width, Size.Height);
        }

        public bool OnLand(Player player)
        {
            if (IsActive)
            {
                player.ApplyJumpForce(Player.InitialJumpForce * 2.5f);
                return true;
            }
            return true;
        }

    }
}
