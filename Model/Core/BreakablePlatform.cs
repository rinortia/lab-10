using System;
using System.Drawing; // Основное пространство имен для графики
using System.Drawing.Drawing2D; // Для Brush и других графических элементов

namespace Model.Core
{
    public class BreakablePlatform : PlatformBase
    {
        private bool _used = false;

        public BreakablePlatform(float x, float y) : base(x, y) { }

        protected override Brush PlatformBrush => Brushes.IndianRed;

        public override bool OnLand(Player player)
        {
            if (!_used)
            {
                _used = true;
                player.Jump();
                return false;          // исчезает после первого прыжка
            }
            return false;              // уже разрушена
        }

        public override void Draw(Graphics g)
        {
            if (!_used)
                base.Draw(g);
            // после использования — не рисуем (или рисуем трещины)
        }
    }
}