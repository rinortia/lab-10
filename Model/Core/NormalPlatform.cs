using System;
using System.Drawing;

namespace Model.Core
{
    public class NormalPlatform : PlatformBase
    {
        public NormalPlatform(float x, float y)
            : base(new PointF(x, y), new SizeF(60, 15)) { }

        protected override Brush PlatformBrush => Brushes.SkyBlue;
    }
}