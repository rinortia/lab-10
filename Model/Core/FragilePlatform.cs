using System.Drawing;

namespace Model.Core
{
    public class FragilePlatform : PlatformBase
    {
        private bool _isBroken = false;

        public FragilePlatform(float x, float y)
            : base(new PointF(x, y), new SizeF(60, 15))
        {
        }

        protected override Brush PlatformBrush => Brushes.SandyBrown;

        public override bool OnLand(Player player)
        {
            if (_isBroken)
                return false;
            _isBroken = true;
            return false;
        }

        public override void Draw(Graphics g)
        {
            if (!_isBroken)
            {
                base.Draw(g);                
            }
        }
    }
}