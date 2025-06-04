using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public interface IPlatform
    {
        PointF Position { get; set; }
        SizeF Size { get; }

       
        bool OnLand(Player player);


        void Draw(Graphics g);
    }
}
