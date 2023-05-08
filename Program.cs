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

            PointsImportants = RecuperationPointsImportants();

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

            //Cacul des trajets entre point de départ et autre points
            Trajet[] TabTrajetEntreDepartPoint = new Trajet[PointsImportants.Count];
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                Trajet CalculTrajet = new Trajet();
                DijkstraAlgo(PointDeDepart, PointsImportants[lignes], map,CalculTrajet);
                TabTrajetEntreDepartPoint[lignes] = CalculTrajet;
                CalculTrajet = null;
            }

            TrajetFinal = ChoixDuTrajet(PointsImportants,TabTrajetEntreDepartPoint,TabTrajet, PointDeDepart);

            TrajetFinal.AfficheTrajet();
            map.AfficheMapEtTrajet(TrajetFinal);
        }

        public static List<Point> RecuperationPointsImportants()
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
            bool Continuer = true;
            int? ScorePotentiel = null;
            Trajet? TrajetChoisi = null;
            Trajet? TrajetEmprunter = null;
            Trajet? TrajetARetourner = null;
            int? Score = 0;
            int nombrePointImportantsAtteint = 0;
            int nombrePointImportantAAteindre = 0;
            int? Cout = null;

            //Compte le nombre de points importants présent a ateindre
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                if (TabTrajetEntreDepartPoint[lignes].PointArr.GetSetUtile == 1)
                {
                    nombrePointImportantAAteindre++;
                }
            }
            

            //Choix du trajet entre le point de départ et le premier point rapportant le plus de score
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                if (Score + (TabTrajetEntreDepartPoint[lignes].PointArr.GetSetValeur - TabTrajetEntreDepartPoint[lignes].CoutTrajet) > Score + ScorePotentiel || Score==0)
                {
                    TrajetChoisi = TabTrajetEntreDepartPoint[lignes];
                    ScorePotentiel = TabTrajetEntreDepartPoint[lignes].PointArr.GetSetValeur - TabTrajetEntreDepartPoint[lignes].CoutTrajet;
                }
            }

            //Stockage du trajet que l'on va emprunter + affichage + vérification si le point atteint est un point important
            if (TrajetChoisi != null)
            {
                TrajetChoisi.AfficheTrajet();
                TrajetEmprunter = TrajetChoisi;
                TrajetARetourner = TrajetEmprunter;

                if (TrajetChoisi.PointArr.GetSetUtile == 1)
                {
                    nombrePointImportantsAtteint++;
                }

                Score = Score + ScorePotentiel;
                Console.WriteLine("Score : " + Score);
            }

            //Supression des trajet dont le point d'arrivé et égale au point d'arrivé du trajet que l'on a choisi
            //Ceci permet de ne pas repasser deux fois par le meme point
            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                for (int colonnes = 0; colonnes < PointsImportants.Count; colonnes++)
                {
                    if (TabTrajet[lignes, colonnes] != null && TrajetEmprunter != null
                        && ((TabTrajet[lignes, colonnes].PointArr.GetSetX == TrajetEmprunter.PointArr.GetSetX
                        && TabTrajet[lignes, colonnes].PointArr.GetSetY == TrajetEmprunter.PointArr.GetSetY) ||
                        (TabTrajet[lignes, colonnes].PointDep.GetSetX == TrajetEmprunter.PointArr.GetSetX
                        && TabTrajet[lignes, colonnes].PointDep.GetSetY == TrajetEmprunter.PointArr.GetSetY)))
                    {
                        TabTrajet[lignes, colonnes] = null;
                    }
                }
            }


            while (Continuer == true)
            {
                TrajetChoisi = null;
                ScorePotentiel = null;
                Cout = null;

                int lignesTrajetChoisi = 0;

                //Choix du trajet en fonction du trajet qui rapporte le plus de score
                for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
                {
                    for (int colonnes = 0; colonnes < PointsImportants.Count; colonnes++)
                    {
                        if (TrajetEmprunter != null && TabTrajet[lignes, colonnes] != null
                            && TabTrajet[lignes, colonnes].PointDep.GetSetX == TrajetEmprunter.PointArr.GetSetX
                            && TabTrajet[lignes, colonnes].PointDep.GetSetY == TrajetEmprunter.PointArr.GetSetY
                            && TabTrajet[lignes, colonnes].PointArr.GetSetX != TrajetEmprunter.PointDep.GetSetX
                            && TabTrajet[lignes, colonnes].PointArr.GetSetY != TrajetEmprunter.PointDep.GetSetY
                            && TabTrajet[lignes, colonnes].PointDep.GetSetX != PointDeDepart.GetSetX
                            && TabTrajet[lignes, colonnes].PointDep.GetSetY != PointDeDepart.GetSetY)
                        {

                            if (Score + (TabTrajet[lignes, colonnes].PointArr.GetSetValeur - TabTrajet[lignes, colonnes].CoutTrajet) > Score + ScorePotentiel || ScorePotentiel == null)
                            {
                                TrajetChoisi = TabTrajet[lignes, colonnes];
                                ScorePotentiel = TabTrajet[lignes, colonnes].PointArr.GetSetValeur - TabTrajet[lignes, colonnes].CoutTrajet;
                                lignesTrajetChoisi = lignes;
                            }
                        }
                    }
                }

                //Stockage du trajet que l'on va emprunter + affichage + vérification si le point atteint est un point important
                if (TrajetChoisi != null && TrajetChoisi != TrajetEmprunter)
                {

                    TrajetChoisi.AfficheTrajet();
                    TrajetEmprunter = TrajetChoisi;

                    if (TrajetARetourner != null)
                    {
                        TrajetARetourner.PointArr = TrajetEmprunter.PointArr;
                        foreach (Point p in TrajetEmprunter.ListPointPracourure)
                            TrajetARetourner.ListPointPracourure.Add(p);
                    }

                    if (TrajetChoisi.PointArr.GetSetUtile == 1)
                    {
                        nombrePointImportantsAtteint++;
                    }

                    Score = Score + ScorePotentiel;
                    Console.WriteLine("Score : " + Score);
                }
                else
                {
                    //Choix du trajet en foction du point qui coute le moins
                    for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
                    {
                        for (int colonnes = 0; colonnes < PointsImportants.Count; colonnes++)
                        {
                            if (TabTrajet[lignes, colonnes] != null && TrajetEmprunter != null
                                && TabTrajet[lignes, colonnes].PointDep.GetSetX == TrajetEmprunter.PointArr.GetSetX
                                && TabTrajet[lignes, colonnes].PointDep.GetSetY == TrajetEmprunter.PointArr.GetSetY
                                && TabTrajet[lignes, colonnes].PointArr.GetSetX != TrajetEmprunter.PointDep.GetSetX
                                && TabTrajet[lignes, colonnes].PointArr.GetSetY != TrajetEmprunter.PointDep.GetSetY
                                && TabTrajet[lignes, colonnes].PointDep.GetSetX != PointDeDepart.GetSetX
                                && TabTrajet[lignes, colonnes].PointDep.GetSetY != PointDeDepart.GetSetY)
                            {

                                if (TabTrajet[lignes, colonnes].CoutTrajet < Cout || Cout == null)
                                {
                                    TrajetChoisi = TabTrajet[lignes, colonnes];
                                    Cout = TabTrajet[lignes, colonnes].CoutTrajet;
                                }
                                lignesTrajetChoisi = lignes;
                            }
                        }
                    }
                    //Stockage du trajet que l'on va emprunter + affichage + vérification si le point atteint est un point important
                    if (TrajetChoisi != null && TrajetChoisi != TrajetEmprunter)
                    {

                        TrajetChoisi.AfficheTrajet();
                        TrajetEmprunter = TrajetChoisi;

                        if (TrajetARetourner != null)
                        {
                            TrajetARetourner.PointArr = TrajetEmprunter.PointArr;
                            foreach (Point p in TrajetEmprunter.ListPointPracourure)
                                TrajetARetourner.ListPointPracourure.Add(p);
                        }

                        if (TrajetChoisi.PointArr.GetSetUtile == 1)
                        {
                            nombrePointImportantsAtteint++;
                        }

                        Score = Score + (TrajetChoisi.PointArr.GetSetValeur - Cout);
                        Console.WriteLine("Score : " + Score);
                    }
                }

                //Si on a atteint tous les points importants l'on peut s'arreter la
                //Sinon il faut parcou les autres points importants
                if (nombrePointImportantsAtteint == nombrePointImportantAAteindre)
                    Continuer = false;

                //Supression des trajet dont le point d'arrivé et égale au point d'arrivén du trajet que l'on a choisi
                //Ceci permet de ne pas repasser deux fois par le meme point
                for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
                {
                    for (int colonnes = 0; colonnes < PointsImportants.Count; colonnes++)
                    {
                        if (TabTrajet[lignes, colonnes] != null && TrajetEmprunter != null
                        && ((TabTrajet[lignes, colonnes].PointArr.GetSetX == TrajetEmprunter.PointArr.GetSetX
                        && TabTrajet[lignes, colonnes].PointArr.GetSetY == TrajetEmprunter.PointArr.GetSetY) ||
                        (TabTrajet[lignes, colonnes].PointDep.GetSetX == TrajetEmprunter.PointArr.GetSetX
                        && TabTrajet[lignes, colonnes].PointDep.GetSetY == TrajetEmprunter.PointArr.GetSetY)))
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