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

        public Point(int CoodX, int CoodY)
        {
            this.x = CoodX;
            this.y = CoodY;
            this.utile = 0;
        }

        public Point(int x, int y, int valeur, int utile)
        {
            this.x = x;
            this.y = y;
            this.valeur = valeur;
            this.utile = utile;
        }

        public int SetDistance { get; set; }
        public int GetSetX { get { return x; } set { this.x = value; } }
        public int GetSetY { get { return y; } set { this.y = value; } }
        public Point? GetSetParent { get; set; }

        public int GetUtile { get { return utile; } }

        public int GetValeur { get { return valeur; } }


    }
}
