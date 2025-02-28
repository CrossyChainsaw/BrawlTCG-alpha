using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    internal class Deck
    {
        public static List<Card> LoadDeckFromFile(string filePath)
        {
            List<Card> deck = new List<Card>();

            if (File.Exists(filePath))
            {
                // Read each line in the file
                var cardIds = File.ReadAllLines(filePath);

                foreach (var cardIdStr in cardIds)
                {
                    if (int.TryParse(cardIdStr, out int cardId)) // Parse the ID from the file line
                    {
                        try
                        {
                            // Try to get the card by ID from the CardCatalogue
                            Card card = CardCatalogue.GetCardById(cardId);
                            deck.Add(card); // Add the cloned card to the deck
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error loading card with ID {cardId}: {ex.Message}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Invalid card ID found in deck file: {cardIdStr}");
                    }
                }
            }
            else
            {
                MessageBox.Show($"Deck file not found: {filePath}");
            }

            return deck;
        }

    }
}
