using CheminAlgo;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Markup;

namespace Algo
{
    internal class Program : Dijkstra
    {
        public static void Main(string[] args)
        {

            Map map = new Map();

            Point PointDeDepart = new Point(2, 14);

            //map.AfficheMap();

            List<Point> PointsImportants = new List<Point>();

            PointsImportants = RecuperationPointsImportants();

            Trajet[,] TabTrajet = new Trajet[PointsImportants.Count,PointsImportants.Count] ;

            for(int lignes=0;lignes<PointsImportants.Count;lignes++)
            {
                for(int j=0;j<PointsImportants.Count;j++)
                {
                    if (PointsImportants[1] != PointsImportants[j])
                    DijkstraAlgo(PointsImportants[lignes], PointsImportants[j], map, TabTrajet[lignes, j]);
                }
            }

            //DijkstraAlgo(PointDeDepart, PointsImportants[0], map, TabTrajet[0,0]);
        }

        public static List<Point> RecuperationPointsImportants()
        {
            int[,] TabPoint = new int[14, 4];
            int ligne = 0;

            List<Point> PointsImportants = new List<Point>();

            StreamReader reader = new StreamReader(File.OpenRead(@"..\..\..\Point.csv"));

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(';');

                for (int colonne = 0; colonne < values.Length; colonne++)
                {
                    TabPoint[ligne, colonne] = Convert.ToInt32(values[colonne]);
                }
                if (TabPoint[ligne, 2] != 30 || TabPoint[ligne, 3] != 0)
                    PointsImportants.Add(new Point(TabPoint[ligne, 0], TabPoint[ligne, 1], TabPoint[ligne, 2], TabPoint[ligne, 3]));
                ligne++;
            }

            return PointsImportants;
        }
    }
}