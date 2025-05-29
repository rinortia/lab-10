using Model.Core;
using System.Collections.Generic;
using System.Drawing;

namespace Model.Data
{
    public class GameState
    {
        public PointF PlayerPosition { get; set; }
        public List<PlatformData> Platforms { get; set; }
        public int Score { get; set; }
    }

    public class PlatformData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public PlatformType Type { get; set; } // Normal, Breakable, HighJump
    }

    public enum PlatformType { Normal, Breakable, HighJump }
}