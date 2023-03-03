using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Algo
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Map map = new Map();

            Point PointDeDepart = new Point(11, 9);

            //Recupérer la map grace au getter

            List<Point> PointsImportants = new List<Point>();

            PointsImportants = RecuperationPointsImportants();

            //De base 14 points
            Console.WriteLine(PointsImportants.Count);
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
                if (TabPoint[ligne, 2] != 30 && TabPoint[ligne, 3] != 0)
                    PointsImportants.Add(new Point(TabPoint[ligne,0], TabPoint[ligne,1], TabPoint[ligne, 2], TabPoint[ligne, 3]));
                ligne++;
            }

            return PointsImportants;
        }
    }
}