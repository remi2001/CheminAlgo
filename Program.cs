using CheminAlgo;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

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

            //Compte le nombre de points importants présent a ateindre
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                if (TabTrajetEntreDepartPoint[lignes].PointArr.GetUtile == 1)
                {
                    nombrePointImportantAAteindre++;
                }
            }
            

            //Choix du trajet entre le point de départ et le premier point rapportant le plus de score
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                if (Score + (TabTrajetEntreDepartPoint[lignes].PointArr.GetValeur - TabTrajetEntreDepartPoint[lignes].CoutTrajet) > Score + ScorePotentiel || ScorePotentiel==null)
                {
                    TrajetChoisiAAjouter = TabTrajetEntreDepartPoint[lignes];
                    ScorePotentiel = TabTrajetEntreDepartPoint[lignes].PointArr.GetValeur - TabTrajetEntreDepartPoint[lignes].CoutTrajet;
                }
            }

            //Stockage du trajet que l'on va emprunter + affichage + vérification si le point atteint est un point important
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

            //Supression des trajet dont le point d'arrivé et égale au point d'arrivé du trajet que l'on a choisi
            //Ceci permet de ne pas repasser deux fois par le meme point
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

            while (Continuer == true)
            {                
                TrajetChoisiAAjouter = null;
                ScorePotentiel = null;

                
                //Choix du trajet en fonction du trajet qui rapporte le plus de score entre tous les points
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

                //Stockage du trajet que l'on va emprunter + affichage + vérification si le point atteint est un point important
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

                //Si on a atteint tous les points importants l'on peut s'arreter la
                //Sinon il faut parcourir les autres points importants
                if (nombrePointImportantsAtteint == nombrePointImportantAAteindre)
                    Continuer = false;

                //Supression des trajet dont le point d'arrivé et égale au point d'arrivé  du trajet que l'on a choisi
                //Ceci permet de ne pas repasser deux fois par le meme point
                for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
                {
                    for (int colonnes = 0; colonnes < PointsImportants.Count; colonnes++)
                    {
                        //Point de départ pas vérifier car égale au point de départ du tous début
                        if (TabTrajet[lignes, colonnes] != null && TrajetEmprunter != null
                        && TabTrajet[lignes, colonnes].PointArr.GetSetX == TrajetEmprunter.PointArr.GetSetX
                        && TabTrajet[lignes, colonnes].PointArr.GetSetY == TrajetEmprunter.PointArr.GetSetY)
                        {
                            TabTrajet[lignes, colonnes] = null;
                        }
                    }
                }
            }

            return TrajetARetourner;
        }
    }
}