using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Visuals.BrawlTCG_alpha.Visuals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrawlTCG_alpha.Visuals
{
    public partial class FRM_Menu : Form
    {
        public FRM_Menu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form frm = new FRM_DeckBuilder();
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // load decks
            List<Card> player1Deck = LoadDeckFromFile("deckPlayer1.txt");
            List<Card> player2Deck = LoadDeckFromFile("deckPlayer2.txt");


            // Check if both players have saved their decks
            if (player1Deck.Count == 0 || player2Deck.Count == 0)
            {
                MessageBox.Show("Both players must have a deck before starting the game!");
                return;
            }

            // Show form to get player names
            FormPlayerNames playerNamesForm = new FormPlayerNames();
            if (playerNamesForm.ShowDialog() == DialogResult.OK)
            {
                string player1Name = playerNamesForm.Player1Name;
                string player2Name = playerNamesForm.Player2Name;

                // Show message with player names
                MessageBox.Show($"Game starting with Player 1: {player1Name} and Player 2: {player2Name}");

                // Create players and start the game
                Player p1 = new Player(player1Name, player1Deck);
                Player p2 = new Player(player2Name, player2Deck);

                new FRM_Game(p1, p2).Show();
                this.Hide(); // Optionally hide the deck builder form
            }
        }
        private List<Card> LoadDeckFromFile(string filePath)
        {
            List<Card> deck = new List<Card>();

            if (File.Exists(filePath))
            {
                // Read each line in the file
                var cardNames = File.ReadAllLines(filePath);

                foreach (var cardName in cardNames)
                {
                    // Try to get the card from the CardCatalogue using the card name
                    if (CardCatalogue.CardDictionary.TryGetValue(cardName, out Card card))
                    {
                        deck.Add(card.Clone()); // Add a clone of the card to the deck
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
