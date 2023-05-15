using CheminAlgo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo
{
    internal class Map
    {
        private int[,] TabMap = new int[20, 20];
        public Map()
        {
            int ligne = 0;

            StreamReader reader = new StreamReader(File.OpenRead(@"..\..\..\Map.csv"));

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(';');

                for (int colonne = 0; colonne < values.Length; colonne++)
                {
                    TabMap[ligne, colonne] = Convert.ToInt32(values[colonne]);
                }
                ligne++;
            }
        }

        public void AfficheMap()
        {
            for(int ligne=0; ligne < 20; ligne++)
            {
                Console.Write("|");
                for (int colonne = 0; colonne < 20; colonne++)
                {
                    if(TabMap[ligne, colonne]==(-1))
                        Console.Write(TabMap[ligne, colonne] + "|");
                    else
                        Console.Write(TabMap[ligne, colonne] + " |");
                }
                Console.WriteLine();
            }
        }


        //A revoir
        public void AfficheMapEtTrajet(Trajet Trajet)
        {
            bool ecrit = false;
            List<Point> PointDejaEcrit = new List<Point>();
            
            for (int ligne = 0; ligne < 20; ligne++)
            {
                Console.Write("|");
                for (int colonne = 0; colonne < 20; colonne++)
                {
                    foreach (Point p in Trajet.ListePointsParcourue)
                    {
                        if (p.GetSetX/*-1*/ == ligne && p.GetSetY/*-1*/ == colonne)
                        {
                            bool DejatEcrit = false;
                            foreach(Point p2 in PointDejaEcrit)
                            {
                                if (p.GetSetX == p2.GetSetX && p.GetSetY == p2.GetSetY)
                                    DejatEcrit = true;
                            }

                            if (DejatEcrit == false)
                            {
                                if (TabMap[ligne, colonne] == (-1))
                                {
                                    if (p.GetUtile == 1)
                                    {
                                        Console.BackgroundColor = ConsoleColor.Red;
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.Write(TabMap[ligne, colonne]);
                                    }
                                    else
                                    {
                                        Console.BackgroundColor = ConsoleColor.Blue;
                                        Console.ForegroundColor = ConsoleColor.Black;
                                        Console.Write(TabMap[ligne, colonne]);
                                    }
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write("|");
                                }
                                else
                                {
                                    if (p.GetUtile == 1)
                                    {
                                        Console.BackgroundColor = ConsoleColor.Red;
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.Write(TabMap[ligne, colonne]);
                                    }
                                    else
                                    {
                                        Console.BackgroundColor = ConsoleColor.Blue;
                                        Console.ForegroundColor = ConsoleColor.Black;
                                        Console.Write(TabMap[ligne, colonne]);
                                    }
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write(" |");
                                }
                                ecrit = true;
                                PointDejaEcrit.Add(p);
                            }
                        }
                    }
                    if (ecrit == false)
                    {
                        if (TabMap[ligne, colonne] == (-1))
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write(TabMap[ligne, colonne] + "|");
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write(TabMap[ligne, colonne] + " |");
                        }
                    }
                    ecrit = false;
                }
                Console.WriteLine();
            }
        }

        public int ValeurPoint(int x, int y)
        {
            return TabMap[x, y];
        }

        public int[,] GetMap { get; set; }
    }}
