using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Windows.Markup;

namespace Algo
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            Map map = new Map();

            Point PointDeDepart = new Point(2,14);

            //map.AfficheMap();

            List<Point> PointsImportants = new List<Point>();

            PointsImportants = RecuperationPointsImportants();

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
            int DistanceTotal;

            char[,] TabAvancé = new char[20, 20];

            Depart.SetDistance = Distance;
            FileAttente.Add(Depart);
            
            while (FileAttente.Count() != 0)
            {
                //Reconstituer le chemin
                AffichageTabAvolution(TabAvancé);

                PointActuel = SelectionPoids(FileAttente);

                FileAttente.Remove(PointActuel);
                ListCasesTraite.Add(PointActuel);

                if(PointActuel.GetSetX == Arriver.GetSetX && PointActuel.GetSetY == Arriver.GetSetY)
                {
                    Console.WriteLine("Arrivé !");
                    Console.WriteLine(PointActuel.GetSetX + ";" + PointActuel.GetSetY);
                    TabAvancé[PointActuel.GetSetX, PointActuel.GetSetY] = 'O';

                    AffichageTabAvolution(TabAvancé);

                    Console.WriteLine(ReconstitutionChemin(PointActuel));

                    DistanceTotal = PointActuel.SetDistance + map.ValeurPoint(PointActuel.GetSetX, PointActuel.GetSetY);
                    Console.WriteLine("Cout pour aller au point :" + DistanceTotal);
                    Thread.Sleep(10000);
                    break;
                }
                else
                {
                    ListVoisin = ParcoursVoisinCase(PointActuel, map);

                    foreach (Point Voisin in ListVoisin)
                    {
                        if (Voisin.SetDistance > PointActuel.SetDistance + map.ValeurPoint(Voisin.GetSetX, Voisin.GetSetY))
                        {
                            Voisin.SetDistance = PointActuel.SetDistance + map.ValeurPoint(Voisin.GetSetX, Voisin.GetSetY);
                            Voisin.SetParent = PointActuel;

                            //Si le voisin n'est pas déjà dans la file d'attente
                            if (!FileAttente.Any(p => p.GetSetX == Voisin.GetSetX && p.GetSetY == Voisin.GetSetY))
                            {
                                FileAttente.Add(Voisin);
                                TabAvancé[Voisin.GetSetX, Voisin.GetSetY] = 'X';
                            }
                        }
                    }
                    ListVoisin.Clear();
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
            int PositionX = PointActuel.GetSetX;
            int PositionY = PointActuel.GetSetY;

            if (PositionY - 1 >= 0)
            {
                //Vérification du voisin SUD
                if(VerificationConditionChemin(map, PositionX, PositionY - 1))
                {
                    Point UnVoisin = new Point(PositionX, PositionY - 1);
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
                    Point UnVoisin = new Point(PositionX, PositionY + 1);
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
                    Point UnVoisin = new Point(PositionX + 1,PositionY);
                    UnVoisin.SetDistance = 99999;
                    ListVoisin.Add(UnVoisin);
                }
            }

            if (PositionX - 1 >= 0)
            {
                //Vérification du voisin OUEST
                if(VerificationConditionChemin(map, PositionX - 1, PositionY))
                {
                    Point UnVoisin = new Point(PositionX - 1, PositionY);
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

        public static void AffichageTabAvolution(char[,] TabAvancé)
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

                Thread.Sleep(1);
        }

        public static string ReconstitutionChemin(Point Point)
        {
            string Chemin = "";
            while (Point.SetParent != null)
            {
                Chemin = "(" + Point.GetSetX + ";" + Point.GetSetY + ")" + Chemin;
                Point = Point.SetParent;
            }
            Chemin = "(" + Point.GetSetX + ";" + Point.GetSetY + ")" + Chemin;
            return Chemin;     
        }
    }
}