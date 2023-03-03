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

        public int[,] GetMap { get;}
    }
}
