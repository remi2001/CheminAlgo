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

            Dijkstra(PointDeDepart, PointsImportants.First(), map);
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
                    PointsImportants.Add(new Point(TabPoint[ligne,0], TabPoint[ligne,1], TabPoint[ligne, 2], TabPoint[ligne, 3]));
                ligne++;
            }

            return PointsImportants;
        }


        public static void Dijkstra(Point Depart, Point Arriver, Map map)
        {
            List<Point> FileAttente = new List<Point>();
            List<Point> ListCasesTraite = new List<Point>();
            List<Point> ListVoisin = new List<Point>();
            Point PointActuel;
            int Distance = 0;

            char[,] TabAvancé = new char[20, 20];

            Depart.SetDistance = Distance;
            FileAttente.Add(Depart);

            while (FileAttente.Count() != 0)
            {
                Console.Clear();
                for(int ligne = 0; ligne < 20; ligne++)
                {
                    for(int colonne = 0; colonne < 20; colonne++)
                    {
                        Console.Write("|"+TabAvancé[ligne,colonne]);
                    }
                    Console.WriteLine();
                }

                Thread.Sleep(10);

                PointActuel = SelectionPoids(FileAttente);

                //Console.WriteLine("Le point en cours de traitement : " + PointActuel.GetX + ";" + PointActuel.GetY);

                FileAttente.Remove(PointActuel);
                ListCasesTraite.Add(PointActuel);

                if(PointActuel.GetX == 11 && PointActuel.GetY == 9)
                {
                    Console.WriteLine("Arrivé !");
                    Console.WriteLine(PointActuel.GetX + ";" + PointActuel.GetY);
                    TabAvancé[PointActuel.GetX, PointActuel.GetY] = 'O';

                    Console.Clear();
                    for (int ligne = 0; ligne < 20; ligne++)
                    {
                        for (int colonne = 0; colonne < 20; colonne++)
                        {
                            Console.Write("|" + TabAvancé[ligne, colonne]);
                        }
                        Console.WriteLine();
                    }

                    break;
                }
                else
                {
                    ListVoisin = ParcoursVoisinCase(PointActuel, map);

                    foreach (Point Voisin in ListVoisin)
                    {
                        //Console.WriteLine("Un point aux coordonnée : " + Voisin.GetX + ";" + Voisin.GetY + " été trouvé !");
                        //Console.WriteLine("La distance du point de départ est de : " + Voisin.SetDistance);
                        if(TabAvancé[Voisin.GetX, Voisin.GetY] != 'X')
                        {
                            if (Voisin.SetDistance > PointActuel.SetDistance + map.ValeurPoint(Voisin.GetX, Voisin.GetY))
                            {
                                Voisin.SetDistance = PointActuel.SetDistance + map.ValeurPoint(Voisin.GetX, Voisin.GetY);
                                Voisin.SetParent = PointActuel;
                                FileAttente.Add(Voisin);
                                TabAvancé[Voisin.GetX, Voisin.GetY] = 'X';
                            }
                        }
                    }
                }
            }
        }

        public static Point SelectionPoids(List<Point> FileAttente)
        {
            Point LePointSelectionner = FileAttente.First();

            foreach(Point p in FileAttente)
            {
                if(p.SetDistance < LePointSelectionner.SetDistance)
                {
                    LePointSelectionner = p;
                }
            }
            return LePointSelectionner;
        }

        public static List<Point> ParcoursVoisinCase(Point PointActuel, Map map)
        {
            List<Point> ListVoisin = new List<Point>();
            int PositionX = PointActuel.GetX;
            int PositionY = PointActuel.GetY;

            if (PositionY - 1 > 0)
            {
                //Vérification du voisin SUD
                if(VerificationConditionChemin(map, PositionX, PositionY - 1))
                {
                    Point UnVoisin = new Point(0, 0);
                    UnVoisin.GetX = PositionX;
                    UnVoisin.GetY = PositionY - 1;
                    UnVoisin.SetDistance = 99999;
                    ListVoisin.Add(UnVoisin);
                }
            }

            //if (PositionY + 1 <= map.GetMap.GetLength(1))
            if (PositionY + 1 < 20)
            {
                //Vérification du voisin NORD
                if(VerificationConditionChemin(map, PositionX, PositionY + 1))
                {
                    Point UnVoisin = new Point(0, 0);
                    UnVoisin.GetX = PositionX;
                    UnVoisin.GetY = PositionY + 1;
                    UnVoisin.SetDistance = 99999;
                    ListVoisin.Add(UnVoisin);
                }
            }

            //if (PositionX + 1 <= map.GetMap.GetLength(0))
            if (PositionX + 1 < 20)
            {
                //Vérification du voisin EST
                if(VerificationConditionChemin(map, PositionX + 1, PositionY))
                {
                    Point UnVoisin = new Point(0,0);
                    UnVoisin.GetX = PositionX + 1;
                    UnVoisin.GetY = PositionY;
                    UnVoisin.SetDistance = 99999;
                    ListVoisin.Add(UnVoisin);
                }
            }

            if (PositionX - 1 > 0)
            {
                //Vérification du voisin OUEST
                if(VerificationConditionChemin(map, PositionX - 1, PositionY))
                {
                    Point UnVoisin = new Point(0, 0);
                    UnVoisin.GetX = PositionX - 1;
                    UnVoisin.GetY = PositionY;
                    UnVoisin.SetDistance = 99999;
                    ListVoisin.Add(UnVoisin);
                }
            }

            return ListVoisin;
        }

        public static bool VerificationConditionChemin(Map map, int PositionX, int PositionY)
        {
            bool SiPointAccessible = false;

            if (map.ValeurPoint(PositionX, PositionY) != -1)
            {
                SiPointAccessible = true;
            }
            
            return SiPointAccessible;
        }
    }
}