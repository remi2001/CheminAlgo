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
            int? Cout = null;
            Trajet? TrajetChoisi = null;
            Trajet? TrajetEmprunter = null;

            Map map = new Map();

            Point PointDeDepart = new Point(2, 14);

            //map.AfficheMap();

            List<Point> PointsImportants = new List<Point>();

            PointsImportants = RecuperationPointsImportants();

            Trajet[,] TabTrajet = new Trajet[PointsImportants.Count,PointsImportants.Count];
            
            //Calcul des trajets entre tous les points importants
            for(int lignes=0;lignes<PointsImportants.Count;lignes++)
            {
                for(int colonnes=0;colonnes<PointsImportants.Count;colonnes++)
                {
                    if (PointsImportants[lignes] != PointsImportants[colonnes])
                    {
                        Trajet? CalculTrajet = new Trajet();
                        DijkstraAlgo(PointsImportants[lignes], PointsImportants[colonnes], map, ref CalculTrajet);
                        TabTrajet[lignes, colonnes] = CalculTrajet;
                        CalculTrajet = null;
                        //TabTrajet[lignes, colonnes].AfficheTrajet();
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
                DijkstraAlgo(PointDeDepart, PointsImportants[lignes], map, ref CalculTrajet);
                TabTrajetEntreDepartPoint[lignes] = CalculTrajet;
                CalculTrajet = null;
                //TabTrajetEntreDepartPoint[lignes].AfficheTrajet();
            }

            for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
            {
                if (TabTrajetEntreDepartPoint[lignes].CoutTrajet<Cout || Cout==null)
                {
                    TrajetChoisi = TabTrajetEntreDepartPoint[lignes];
                    Cout = TabTrajetEntreDepartPoint[lignes].CoutTrajet;
                }
            }
            if (TrajetChoisi != null)
            {
                TrajetChoisi.AfficheTrajet();
                TrajetEmprunter = TrajetChoisi;
            }

            bool Continuer = true;
            int nbRepet = 0;

            while (Continuer==true)
            {
                Cout = null;

                //int lignesTrajetChoisi=0;
                for (int lignes = 0; lignes < PointsImportants.Count; lignes++)
                {
                    for (int colonnes = 0; colonnes < PointsImportants.Count; colonnes++)
                    {
                        if (TabTrajet[lignes, colonnes] != null && TabTrajet[lignes, colonnes].PointDep.GetSetX == TrajetEmprunter.PointArr.GetSetX && TabTrajet[lignes, colonnes].PointDep.GetSetY == TrajetEmprunter.PointArr.GetSetY
                            && TabTrajet[lignes, colonnes].PointArr.GetSetX != TrajetEmprunter.PointDep.GetSetX && TabTrajet[lignes, colonnes].PointArr.GetSetY != TrajetEmprunter.PointDep.GetSetY)
                        {
                            if (TabTrajet[lignes, colonnes].CoutTrajet < Cout || Cout==null)
                            {
                                TrajetChoisi = TabTrajet[lignes, colonnes];
                                Cout = TabTrajet[lignes, colonnes].CoutTrajet;
                            }
                            //lignesTrajetChoisi = lignes;
                        }
                    }
                }
                if (TrajetChoisi != null)
                {
                    TrajetChoisi.AfficheTrajet();
                    TrajetEmprunter = TrajetChoisi;
                }

                /*for (int colonnes = 0; colonnes < PointsImportants.Count; colonnes++)
                {
                    TabTrajet[lignesTrajetChoisi, colonnes] = null;
                }*/

                    nbRepet++;
                if (nbRepet == 12)
                    Continuer = false;
            }
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
    }
}