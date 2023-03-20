using Algo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheminAlgo
{
    internal class Trajet
    {
        public Point PointDep, PointArr;
        public List<Point> ListPointPracourure;
        public int CoutTrajet;

        public Trajet()
        {
            PointDep = null;
            PointArr = null;
            ListPointPracourure = null;
            CoutTrajet = 0;
        }

        public void AfficheTrajet()
        {
            Console.WriteLine("Point Départ : (" + this.PointDep.GetSetX + " , " + this.PointDep.GetSetY + " ) ");
            Console.WriteLine("Point Arrivée : (" + this.PointArr.GetSetX + " , " + this.PointArr.GetSetY + " ) ");
            Console.WriteLine("Cout : " + this.CoutTrajet);
            foreach (Point p in this.ListPointPracourure)
            {
                Console.Write("( " + p.GetSetX + " , " + p.GetSetY + " ) - ");
            }
        }
    }
}
