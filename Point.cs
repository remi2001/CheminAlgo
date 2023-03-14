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
        //private int Distance;
        //private Point? Parent;

        public Point()
        {
            this.x = 0;
            this.y = 0;
        }

        public Point(int CoodX, int CoodY)
        {
            this.x = CoodX;
            this.y = CoodY;
        }

        public Point(int x, int y, int valeur, int utile)
        {
            this.x = x;
            this.y = y;
            this.valeur = valeur;
            this.utile = utile;
        }

        public void affichePoint()
        {
            Console.WriteLine("x = "+this.x);
            Console.WriteLine("y = "+this.y);
        }

        public int SetDistance { get; set; }
        public int GetSetX { get { return x; } set { this.x = value; } }
        public int GetSetY { get { return y; } set { this.y = value; } }
        public Point? SetParent { get; set; }


    }
}
