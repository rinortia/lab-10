using Model.Core;
using System.Collections.Generic;
using System.Drawing;

namespace Model.Data
{
    public class GameState
    {
        public PointF PlayerPosition { get; set; }
        public float PlayerVelocityY { get; set; }
        public bool IsPlayerOnGround { get; set; }
        public List<PlatformData> Platforms { get; set; } = new List<PlatformData>();
        public int Score { get; set; }
        public int GameTime { get; set; }
        public int Version { get; } = 1;
    }

    public class PlatformData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public SizeF Size { get; set; }
        public PlatformType Type { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public enum PlatformType
    {
        Normal,
        Breakable,
        HighJump,
        Moving
    }
}