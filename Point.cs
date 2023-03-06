using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo
{
    internal class Point
    {
        private int x, y, valeur, utile;
        private int Distance;
        private Point Parent;

        private List<Point> PointsUtiles;
        public Point(int CoodX, int CoodY)
        {
            x = CoodX;
            y = CoodY;
        }

        public Point(int x, int y, int valeur, int utile)
        {
            this.x = x;
            this.y = y;
            this.valeur = valeur;
            this.utile = utile;
        }

        public int SetDistance { get; set; }
        public int GetX { get; set; }
        public int GetY { get; set; }
        public Point SetParent { get; set; }
    }
}
