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
        /// <summary>
        /// Trouve le chemin le plus court possible entre deux points
        /// </summary>
        /// <param name="Depart"> Le points de départ du chemin à trouver</param>
        /// <param name="Arriver">Le points d'arriver du chemin à trouver</param>
        /// <param name="map"> La carte pour pouvoir se déplacer et récupérer des informations utiles comme le cout utilisé si on passe par ce point</param>
        /// <param name="Trajet"> Le trajet qui est vide au début, mais c'est pour pouvoir lui enregistré le trajet</param>
        public static void DijkstraAlgo(Point Depart, Point Arriver, Map map,Trajet Trajet)
        {
            List<Point> FileAttente = new List<Point>();
            List<Point> ListVoisin = new List<Point>();

            Point PointActuel;
            int DistanceTotal;
            bool Arrive = false;

            //Tableau permettant de voir l'évolution du programme
            char[,] TabAvancé = new char[20, 20];

            Trajet.PointDep = Depart;
            Trajet.PointArr = Arriver;

            //Vu que le programme commence au départ, la distance du départ vaut 0. Il n'a pas bougé
            Depart.SetDistance = 0;

            //Ajout le point de départ à la file d'attente pour que le programme commence à parcourir la carte
            FileAttente.Add(Depart);

            //Tant que le programme n'a pas trouvé l'arriver et que la file d'attente n'est pas vide
            while (FileAttente.Count() != 0 && Arrive == false)
            {
                //Sélectionne le point le plus avantagueux à analyser
                PointActuel = SelectionPoints(FileAttente);

                FileAttente.Remove(PointActuel);

                //Si le point actuellement en cours de traitement vaut le point d'arriver
                if (PointActuel.GetSetX == Arriver.GetSetX && PointActuel.GetSetY == Arriver.GetSetY)
                {
                    ReconstitutionChemin(PointActuel,Trajet);

                    DistanceTotal = PointActuel.SetDistance + map.ValeurPoint(PointActuel.GetSetX, PointActuel.GetSetY);
                    Trajet.CoutTrajet = DistanceTotal;

                    Arrive = true;
                }
                else
                {
                    //Liste tous les voisins possible du point actuel.
                    ListVoisin = ParcoursVoisinCase(PointActuel, map);

                    foreach (Point Voisin in ListVoisin)
                    {
                        //Au début, la distance des voisins vaut 99999999 pour dire qu'on a pas analyser sa distance par rapport au départ
                        //Après, il possible que le programme parcours deux fois un point mais avec un distance plus faible, car il a trouvé
                        //un meilleur chemin plutôt. Alors, on doit mettre à sa distance
                        if (Voisin.SetDistance > PointActuel.SetDistance + map.ValeurPoint(Voisin.GetSetX, Voisin.GetSetY))
                        {
                            Voisin.SetDistance = PointActuel.SetDistance + map.ValeurPoint(Voisin.GetSetX, Voisin.GetSetY);
                            Voisin.GetSetParent = PointActuel;

                            //Si le voisin n'est pas déjà dans la file d'attente //Pour éviter d'avoir les voisins trouvé, toujours pas 
                            //traité mais qui attendent. Sa fait une sorte de boucle infini
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

        /// <summary>
        /// Sélection le point avec la distance la plus faible
        /// </summary>
        /// <param name="FileAttente"> Liste de tous les voisins d'un point</param>
        /// <returns> Un point avec la plus faible distance</returns>
        private static Point SelectionPoints(List<Point> FileAttente)
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

        /// <summary>
        /// Trouve tous les voisins possible d'un point
        /// </summary>
        /// <param name="PointActuel"> Le point à qui on doit trouver ses voisins</param>
        /// <param name="map"> La carte, pour pouvoir sélectionner ses potentiels voisins</param>
        /// <returns> Liste de tous les voisins du point passé en paramètre</returns>
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

        /// <summary>
        /// Vérifie si le point est accessible
        /// </summary>
        /// <param name="map">La carte pour récupérer la valeur contenu à des coordonnées précise</param>
        /// <param name="PositionX"> La coordonnée X du point qui faut vérifier l'accessibilité</param>
        /// <param name="PositionY">La coordonnée Y du point qui faut vérifier l'accessibilité</param>
        /// <returns>Retourne vrai si le point est accessible, sinon faux</returns>
        private static bool VerificationConditionChemin(Map map, int PositionX, int PositionY)
        {
            bool SiPointAccessible = false;

            if (map.ValeurPoint(PositionX, PositionY) != -1)
            {
                SiPointAccessible = true;
            }

            return SiPointAccessible;
        }

        /// <summary>
        /// Affiche le tableau comportant tous les points accessibles
        /// </summary>
        /// <param name="TabAvancé">Le tableau de caractère de l'évolution</param>
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

        /// <summary>
        /// Reconstitue le chemin le plus court trouvé
        /// </summary>
        /// <param name="Point"> Le point d'arriver</param>
        /// <param name="Trajet"> Le chemin le plus court trouvé avec le cout, la liste de point parcourue et point de départ+arriver</param>
        /// <returns>Retourne le chemin en chaîne de caractère sous cette forme (X,Y) (X,Y)</returns>
        private static string ReconstitutionChemin(Point Point,Trajet Trajet)
        {
            string Chemin = "";

            while (Point.GetSetParent != null)
            {
                Chemin = "(" + Point.GetSetX + ";" + Point.GetSetY + ")" + Chemin;
                Point = Point.GetSetParent;
                Trajet.ListePointsParcourue.Add(Point);
            }
            Chemin = "(" + Point.GetSetX + ";" + Point.GetSetY + ")" + Chemin;
            Trajet.ListePointsParcourue.Add(Point);

            return Chemin;
        }
    }
}
