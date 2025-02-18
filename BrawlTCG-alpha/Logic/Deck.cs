using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    internal class DeckLoader
    {
        public static List<Card> LoadDeckFromFile(string filePath)
        {
            List<Card> deck = new List<Card>();
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                // Based on your text file structure, we can check and clone the correct card
                //if (line == "Essence") deck.Add(Essence.Clone());
                //else if (line == "Fangwild") deck.Add(Fangwild.Clone());
                //else if (line == "FaerieQueen") deck.Add(FaerieQueen.Clone());
                //else if (line == "BriarRose") deck.Add(BriarRose.Clone());
                //else if (line == "ForestGuardian") deck.Add(ForestGuardian.Clone());
                //else if (line == "Heatblast") deck.Add(Heatblast.Clone());
                //else if (line == "BlazingFire") deck.Add(BlazingFire.Clone());
                //else if (line == "MagmaSpear") deck.Add(MagmaSpear.Clone());
                // Add more conditions for other cards as needed
            }

            return deck;
        }
    }
}
