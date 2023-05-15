﻿using Algo;
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
        public List<Point> ListePointsParcourue;
        public int CoutTrajet;

        public Trajet()
        {
            PointDep = null;
            PointArr = null;
            ListePointsParcourue = new List<Point>();
            CoutTrajet = 0;
        }

        public void AfficheTrajet()
        {
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine("Point Départ : (" + this.PointDep.GetSetX + " , " + this.PointDep.GetSetY + " ) ");
            Console.WriteLine("Point Arrivée : (" + this.PointArr.GetSetX + " , " + this.PointArr.GetSetY + " ) ");
            Console.WriteLine("Cout : " + this.CoutTrajet);
            
            foreach (Point p in this.ListePointsParcourue)
            {
                Console.Write("( " + p.GetSetX + " , " + p.GetSetY + " ) - ");
            }
            Console.WriteLine("");
        }
    }
}
