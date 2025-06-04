using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public abstract class PlatformBase : IPlatform
    {
        public PointF Position { get; set; }
        public SizeF Size { get; protected set; } = new SizeF(60, 15);

        protected PlatformBase(float x, float y)
        {
            Position = new PointF(x, y);
        }

        protected abstract Brush PlatformBrush { get; }

        public virtual bool OnLand(Player player)
        {
            player.Jump();
            return true;                
        }

        public virtual void Draw(Graphics g)
        {
            g.FillRectangle(PlatformBrush, Position.X, Position.Y, Size.Width, Size.Height);
        }
    }
}
