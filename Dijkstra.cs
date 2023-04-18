using Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheminAlgo
{
    internal abstract class Dijkstra
    {

        public static void DijkstraAlgo(Point Depart, Point Arriver, Map map,Trajet Trajet)
        {
            List<Point> FileAttente = new List<Point>();
            List<Point> ListVoisin = new List<Point>();

            Point PointActuel;
            int DistanceTotal;
            bool Arrive = false;

            char[,] TabAvancé = new char[20, 20];

            Trajet.PointDep = Depart;
            Trajet.PointArr = Arriver;
            Depart.SetDistance = 0;
            FileAttente.Add(Depart);

            while (FileAttente.Count() != 0 && Arrive == false)
            {
                PointActuel = SelectionPoids(FileAttente);

                FileAttente.Remove(PointActuel);

                if (PointActuel.GetSetX == Arriver.GetSetX && PointActuel.GetSetY == Arriver.GetSetY)
                {
                    ReconstitutionChemin(PointActuel,Trajet);

                    DistanceTotal = PointActuel.SetDistance + map.ValeurPoint(PointActuel.GetSetX, PointActuel.GetSetY);
                    Trajet.CoutTrajet = DistanceTotal;

                    Arrive = true;
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
                            }
                        }
                    }
                    ListVoisin.Clear();
                }
            }
        }

        private static Point SelectionPoids(List<Point> FileAttente)
        {
            Point LePointSelectionner = FileAttente.First();

            foreach (Point point in FileAttente)
            {
                if (point.SetDistance < LePointSelectionner.SetDistance)
                {
                    LePointSelectionner = point;
                }
            }
            return LePointSelectionner;
        }

        private static List<Point> ParcoursVoisinCase(Point PointActuel, Map map)
        {
            List<Point> ListVoisin = new List<Point>();
            int PositionX = PointActuel.GetSetX;
            int PositionY = PointActuel.GetSetY;

            if (PositionY - 1 >= 0)
            {
                //Vérification du voisin SUD
                if (VerificationConditionChemin(map, PositionX, PositionY - 1))
                {
                    Point UnVoisin = new Point(PositionX, PositionY - 1);
                    UnVoisin.SetDistance = 99999;
                    ListVoisin.Add(UnVoisin);
                }
            }

            if (PositionY + 1 < 20)
            {
                //Vérification du voisin NORD
                if (VerificationConditionChemin(map, PositionX, PositionY + 1))
                {
                    Point UnVoisin = new Point(PositionX, PositionY + 1);
                    UnVoisin.SetDistance = 99999;
                    ListVoisin.Add(UnVoisin);
                }
            }

            if (PositionX + 1 < 20)
            {
                //Vérification du voisin EST
                if (VerificationConditionChemin(map, PositionX + 1, PositionY))
                {
                    Point UnVoisin = new Point(PositionX + 1, PositionY);
                    UnVoisin.SetDistance = 99999;
                    ListVoisin.Add(UnVoisin);
                }
            }

            if (PositionX - 1 >= 0)
            {
                //Vérification du voisin OUEST
                if (VerificationConditionChemin(map, PositionX - 1, PositionY))
                {
                    Point UnVoisin = new Point(PositionX - 1, PositionY);
                    UnVoisin.SetDistance = 99999;
                    ListVoisin.Add(UnVoisin);
                }
            }

            return ListVoisin;
        }

        private static bool VerificationConditionChemin(Map map, int PositionX, int PositionY)
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
            for (int ligne = 0; ligne < 20; ligne++)
            {
                for (int colonne = 0; colonne < 20; colonne++)
                {
                    Console.Write("|" + TabAvancé[ligne, colonne]);
                }
                Console.WriteLine();
            }

            Thread.Sleep(1);
        }

        private static string ReconstitutionChemin(Point Point,Trajet Trajet)
        {
            string Chemin = "";

            while (Point.SetParent != null)
            {
                Chemin = "(" + Point.GetSetX + ";" + Point.GetSetY + ")" + Chemin;
                Point = Point.SetParent;
                Trajet.ListPointPracourure.Add(Point);
            }
            Chemin = "(" + Point.GetSetX + ";" + Point.GetSetY + ")" + Chemin;
            
            return Chemin;
        }
    }
}
