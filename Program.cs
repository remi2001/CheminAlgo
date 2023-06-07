using CheminAlgo;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using static System.Formats.Asn1.AsnWriter;

namespace Algo
{
    internal class Program : Dijkstra
    {
        public static void Main(string[] args)
        {
            Map map = new Map();

            Point PointDeDepart = new Point(2, 14);

            List<Point> PointsImportants = new List<Point>();

            PointsImportants = RecuperationPoints();

            Trajet[,] TabTrajet = new Trajet[PointsImportants.Count,PointsImportants.Count];

            Trajet TrajetFinal;
            
            //Calcul des trajets entre tous les points importants
            for(int lignes=0;lignes<PointsImportants.Count;lignes++)
            {
                for(int colonnes=0;colonnes<PointsImportants.Count;colonnes++)
                {
                    if (PointsImportants[lignes] != PointsImportants[colonnes])
                    {
                        Trajet? CalculTrajet = new Trajet();
                        DijkstraAlgo(PointsImportants[lignes], PointsImportants[colonnes], map, CalculTrajet);
                        TabTrajet[lignes, colonnes] = CalculTrajet;
                        CalculTrajet = null;
                    }
                    else
                    {
                        TabTrajet[lignes, colonnes] = null;
                    }
                }
            }

            //Cacul des trajets entre le point de départ et tous les autres points
            Trajet[] TabTrajetEntreDepartPoint = new Trajet[PointsImportants.Count];
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                Trajet CalculTrajet = new Trajet();
                DijkstraAlgo(PointDeDepart, PointsImportants[lignes], map,CalculTrajet);
                TabTrajetEntreDepartPoint[lignes] = CalculTrajet;
                CalculTrajet = null;
            }

            TrajetFinal = ChoixDuTrajet(PointsImportants,TabTrajetEntreDepartPoint,TabTrajet, PointDeDepart);

            map.AfficheMapEtTrajet(TrajetFinal);
        }

        #region Recupérations des points
        /// <summary>
        /// Methodes permettant de récupérer les Points a parcourir
        /// </summary>
        /// <returns> Liste de points a parcourir</returns>
        public static List<Point> RecuperationPoints()
        {
            int[,] TabPoint = new int[14, 4];
            int ligne = 0;

            List<Point> PointsImportants = new List<Point>();

            StreamReader reader = new StreamReader(File.OpenRead(@"..\..\..\Point.csv"));

            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                string[]? values = line.Split(';');

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
        #endregion

        #region Choix du trajet emprunter
        /// <summary>
        /// Methodes permettant de choisir le Trajet que l'on va emprunter
        /// </summary>
        /// <param name="PointsImportants">Liste des points importants a parcourir</param>
        /// <param name="TabTrajetEntreDepartPoint">Tableau regroupant les trajet entre le point de départ et tous les autres points</param>
        /// <param name="TabTrajet">Trajet entre chaque points important possible</param>
        /// <param name="PointDeDepart">Point de Depart du chemin</param>
        /// <returns>Trajet emprunter complet</returns>
        public static Trajet ChoixDuTrajet(List<Point> PointsImportants, Trajet[] TabTrajetEntreDepartPoint, Trajet[,] TabTrajet, Point PointDeDepart)
        {
            int nombrePointImportantAAteindre = 0;
            int nombrePointImportantsAtteint = 0;

            bool Continuer = true;
            Trajet? TrajetChoisiAAjouter = null;
            Trajet? TrajetEmprunter = null;
            Trajet? TrajetARetourner = null;
            int? ScorePotentiel = null;
            int? Score = 0;

            NombreDePointsAAtteindre(PointsImportants, TabTrajetEntreDepartPoint, ref nombrePointImportantAAteindre);

            ChoixEntrePointDepartEtUnPointImportant(PointsImportants, TabTrajetEntreDepartPoint, ref Score, ref ScorePotentiel, ref TrajetChoisiAAjouter, ref TrajetEmprunter, ref TrajetARetourner, ref nombrePointImportantsAtteint);


            //Supression des trajet dont le point d'arrivé et égale au point d'arrivé du trajet que l'on a choisi
            //Ceci permet de ne pas repasser deux fois par le meme point
            SuppressionDesTrajetsContenantLesPointsParcourues(PointsImportants, TrajetEmprunter, ref TabTrajet);

            while (Continuer == true)
            {                
                TrajetChoisiAAjouter = null;
                ScorePotentiel = null;


                //Choix du trajet en fonction du trajet qui rapporte le plus de score entre tous les points
                //Stockage du trajet que l'on va emprunter + affichage + vérification si le point atteint est un point important
                ChoixDuTrajetEntre2PointsEnFonctionDuScore(PointsImportants, ref TrajetEmprunter, TabTrajet, ref Score, ref ScorePotentiel, ref TrajetChoisiAAjouter,
                    ref TrajetARetourner, ref nombrePointImportantsAtteint);

                //Si on a atteint tous les points importants l'on peut s'arreter la
                //Sinon il faut parcourir les autres points importants
                if (nombrePointImportantsAtteint == nombrePointImportantAAteindre)
                    Continuer = false;

                //Supression des trajet dont le point d'arrivé et égale au point d'arrivé  du trajet que l'on a choisi
                //Ceci permet de ne pas repasser deux fois par le meme point
                SuppressionDesTrajetsContenantLesPointsParcourues(PointsImportants, TrajetEmprunter, ref TabTrajet);
            }

            return TrajetARetourner;
        }

        /// <summary>
        /// Compte le nombre de points importants présent a ateindre
        /// </summary>
        /// <param name="PointsImportants">Liste des points importants a parcourir</param>
        /// <param name="TabTrajetEntreDepartPoint">Tableau regroupant les trajet entre le point de départ et tous les autres points</param>
        /// <param name="nombrePointImportantAAteindre">Nombre de point que l'on doit absolument atteindre</param>
        public static void NombreDePointsAAtteindre(List<Point> PointsImportants, Trajet[] TabTrajetEntreDepartPoint, ref int nombrePointImportantAAteindre )
        {
            for (int lignes = 0; lignes<PointsImportants.Count; lignes++)
            {
                if (TabTrajetEntreDepartPoint[lignes].PointArr.GetUtile == 1)
                {
                    nombrePointImportantAAteindre++;
                }
            }
        }

        /// <summary>
        /// Méthode permettant de choisir un trajet entre le point de depart et le point important le plus proche en fonction du score
        /// </summary>
        /// <param name="PointsImportants">Liste des points importants</param>
        /// <param name="TabTrajetEntreDepartPoint">Tableau contenant tous les trajets possibles entre le points de depart et les Points Importants</param>
        /// <param name="Score">Score actuel du trajet complet</param>
        /// <param name="ScorePotentiel">Score potentiel du trajet selon le trajet en cours d'analyse</param>
        /// <param name="TrajetChoisiAAjouter">Trajet Que l'on va ajouter au trajet parcourue</param>
        /// <param name="TrajetEmprunter">Trajet que l'on va emprunter permettant des vérifications plus tard</param>
        /// <param name="TrajetARetourner">Trajet complet que l'on retournera par la suite</param>
        /// <param name="nombrePointImportantsAtteint">Nombre de points atteint apres le choix du trajet que l'on va ajouter</param>
        public static void ChoixEntrePointDepartEtUnPointImportant(List<Point> PointsImportants, Trajet[] TabTrajetEntreDepartPoint,
            ref int? Score, ref int? ScorePotentiel, ref Trajet? TrajetChoisiAAjouter, ref Trajet? TrajetEmprunter, ref Trajet? TrajetARetourner, ref int nombrePointImportantsAtteint)
        {
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                if (Score + (TabTrajetEntreDepartPoint[lignes].PointArr.GetValeur - TabTrajetEntreDepartPoint[lignes].CoutTrajet) > Score + ScorePotentiel || ScorePotentiel == null)
                {
                    TrajetChoisiAAjouter = TabTrajetEntreDepartPoint[lignes];
                    ScorePotentiel = TabTrajetEntreDepartPoint[lignes].PointArr.GetValeur - TabTrajetEntreDepartPoint[lignes].CoutTrajet;
                }
            }

            if (TrajetChoisiAAjouter != null)
            {
                TrajetChoisiAAjouter.AfficheTrajet();
                TrajetEmprunter = TrajetChoisiAAjouter;
                TrajetARetourner = TrajetEmprunter;

                if (TrajetChoisiAAjouter.PointArr.GetUtile == 1)
                {
                    nombrePointImportantsAtteint++;
                }

                Score += ScorePotentiel;
                Console.WriteLine("Score : " + Score);
            }
        }

        /// <summary>
        /// Suppresion des trajets contenant les points parcourues afin de ne pas repasser deux foix sur le meme points
        /// </summary>
        /// <param name="PointsImportants">Liste des points importants</param>
        /// <param name="TrajetEmprunter">Trajet Emprunter lors du choix du trajet</param>
        /// <param name="TabTrajet">Tableau de tous les trajets entre tous les points importants</param>
        public static void SuppressionDesTrajetsContenantLesPointsParcourues(List<Point> PointsImportants,Trajet? TrajetEmprunter, ref Trajet[,] TabTrajet)
        {
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                for (int colonnes = 0; colonnes < PointsImportants.Count; colonnes++)
                {
                    if (TabTrajet[lignes, colonnes] != null && TrajetEmprunter != null
                        && TabTrajet[lignes, colonnes].PointArr.GetSetX == TrajetEmprunter.PointArr.GetSetX
                        && TabTrajet[lignes, colonnes].PointArr.GetSetY == TrajetEmprunter.PointArr.GetSetY)
                    {
                        TabTrajet[lignes, colonnes] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Méthode permettant de choisir un trajet entre 2 points importants en fonction du score
        /// </summary>
        /// <param name="PointsImportants">Liste des points importants</param>
        /// <param name="TrajetEmprunter">Trajet que l'on va emprunter permettant des vérifications plus tard</param>
        /// <param name="TabTrajet">Tableau contenant tous les trajets possibles entre les points importants</param>
        /// <param name="Score">Score actuel du trajet complet</param>
        /// <param name="ScorePotentiel">Score potentiel du trajet selon le trajet en cours d'analyse</param>
        /// <param name="TrajetChoisiAAjouter">Trajet Que l'on va ajouter au trajet parcourue</param>
        /// <param name="TrajetARetourner">Trajet complet que l'on retournera par la suite</param>
        /// <param name="nombrePointImportantsAtteint">Nombre de points atteint apres le choix du trajet que l'on va ajouter</param>
        public static void ChoixDuTrajetEntre2PointsEnFonctionDuScore(List<Point> PointsImportants, ref Trajet? TrajetEmprunter, Trajet[,] TabTrajet, ref int? Score, ref int? ScorePotentiel,
            ref Trajet? TrajetChoisiAAjouter, ref Trajet? TrajetARetourner, ref int nombrePointImportantsAtteint)
        {
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                for (int colonnes = 0; colonnes < PointsImportants.Count; colonnes++)
                {
                    if (TrajetEmprunter != null && TabTrajet[lignes, colonnes] != null
                        && TabTrajet[lignes, colonnes].PointDep.GetSetX == TrajetEmprunter.PointArr.GetSetX
                        && TabTrajet[lignes, colonnes].PointDep.GetSetY == TrajetEmprunter.PointArr.GetSetY)
                    {

                        if (Score + (TabTrajet[lignes, colonnes].PointArr.GetValeur - TabTrajet[lignes, colonnes].CoutTrajet) > Score + ScorePotentiel || ScorePotentiel == null)
                        {
                            TrajetChoisiAAjouter = TabTrajet[lignes, colonnes];
                            ScorePotentiel = TabTrajet[lignes, colonnes].PointArr.GetValeur - TabTrajet[lignes, colonnes].CoutTrajet;
                        }
                    }
                }
            }

            if (TrajetChoisiAAjouter != null && TrajetChoisiAAjouter != TrajetEmprunter)
            {

                TrajetChoisiAAjouter.AfficheTrajet();
                TrajetEmprunter = TrajetChoisiAAjouter;

                if (TrajetARetourner != null)
                {
                    TrajetARetourner.PointArr = TrajetEmprunter.PointArr;
                    foreach (Point p in TrajetEmprunter.ListePointsParcourue)
                    {
                        TrajetARetourner.ListePointsParcourue.Add(p);
                    }
                    //Ajoute le point d'arrivé car n'est pas ajouté automatiquement
                    TrajetARetourner.ListePointsParcourue.Add(TrajetARetourner.PointArr);
                }

                if (TrajetChoisiAAjouter.PointArr.GetUtile == 1)
                {
                    nombrePointImportantsAtteint++;
                }

                Score += ScorePotentiel;
                Console.WriteLine("Score : " + Score);
            }
        }
        #endregion
    }
}