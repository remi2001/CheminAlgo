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
        public List<Point> ListPointPracourure;
        private int CoutTrajet;

        public Trajet(List<Point> listPointPracourure, int coutTrajet)
        {
            ListPointPracourure = listPointPracourure;
            CoutTrajet = coutTrajet;
        }
    }
}
