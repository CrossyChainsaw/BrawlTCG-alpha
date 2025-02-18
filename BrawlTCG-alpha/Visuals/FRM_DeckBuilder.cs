using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Logic.Cards;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BrawlTCG_alpha.Visuals
{
    public enum CardElementFilter
    {
        All,
        Fire,
        Magic,
        Nature,
        Cosmic,
        Water,
        Wild,
        Shadow
    }

    public partial class FRM_DeckBuilder : Form
    {
        private ListView listAvailableCards, listDeck;
        private Button btnAddCard, btnRemoveCard, btnSaveDeck, btnSwitchPlayer, btnStartGame;
        private ComboBox cmbFilterType, cmbFilterElement;
        private PictureBox picCardPreview;
        private List<Card> player1Deck = new List<Card>();
        private List<Card> player2Deck = new List<Card>();
        private List<Card> availableCards;
        private const int MAX_COPIES = 4;
        private bool isPlayer1Turn = true; // Track whose turn it is to build the deck
        private bool isPlayer1DeckSaved = false;
        private bool isPlayer2DeckSaved = false;

        public FRM_DeckBuilder()
        {
            Text = "Deck Builder";
            Size = new Size(800, 600);
            InitializeComponents();
            LoadCards();
            LoadDecks();    
        }



        private void InitializeComponents()
        {
            // Initialize components (no change)
            listAvailableCards = new ListView { Location = new Point(20, 50), Size = new Size(800, 400), View = View.Details, FullRowSelect = true };
            listAvailableCards.Columns.Add("Card Name", 150);
            listAvailableCards.Columns.Add("Cost", 70);
            listAvailableCards.Columns.Add("Element", 80);
            listAvailableCards.Columns.Add("Description", 480);
            listAvailableCards.SelectedIndexChanged += ListAvailableCards_SelectedIndexChanged;
            Controls.Add(listAvailableCards);

            listDeck = new ListView { Location = new Point(840, 50), Size = new Size(300, 400), View = View.Details, FullRowSelect = true };
            listDeck.Columns.Add("Card Name", 200);
            listDeck.Columns.Add("Copies", 80);
            Controls.Add(listDeck);

            btnAddCard = new Button { Text = "➕ Add", Location = new Point(840, 460), Size = new Size(100, 30) };
            btnAddCard.Click += BtnAddCard_Click;
            Controls.Add(btnAddCard);

            btnRemoveCard = new Button { Text = "➖ Remove", Location = new Point(840, 500), Size = new Size(100, 30) };
            btnRemoveCard.Click += BtnRemoveCard_Click;
            Controls.Add(btnRemoveCard);

            btnSaveDeck = new Button { Text = "💾 Save Deck", Location = new Point(840, 540), Size = new Size(100, 30) };
            btnSaveDeck.Click += BtnSaveDeck_Click;
            Controls.Add(btnSaveDeck);

            btnStartGame = new Button { Text = "🎮 Start Game", Location = new Point(840, 580), Size = new Size(100, 30) };
            btnStartGame.Click += BtnStartGame_Click;
            Controls.Add(btnStartGame);


            btnSwitchPlayer = new Button { Text = "Switch to Player 2", Location = new Point(20, 460), Size = new Size(150, 30) };
            btnSwitchPlayer.Click += BtnSwitchPlayer_Click;
            Controls.Add(btnSwitchPlayer);

            cmbFilterType = new ComboBox { Location = new Point(20, 20), Width = 150 };
            cmbFilterType.Items.AddRange(new string[] { "All", "Legend", "Stage", "Weapon", "Essence", "Battle" });
            cmbFilterType.SelectedIndex = 0;
            cmbFilterType.SelectedIndexChanged += (s, e) => PopulateAvailableCards();
            Controls.Add(cmbFilterType);

            cmbFilterElement = new ComboBox { Location = new Point(180, 20), Width = 140 };
            cmbFilterElement.Items.Add("All");
            cmbFilterElement.Items.AddRange(Enum.GetNames(typeof(Elements)));
            cmbFilterElement.SelectedIndex = 0;
            cmbFilterElement.SelectedIndexChanged += (s, e) => PopulateAvailableCards();
            Controls.Add(cmbFilterElement);

            picCardPreview = new PictureBox { Location = new Point(640, 460), Size = new Size(180, 240), BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(picCardPreview);
        }

        private void LoadCards()
        {
            availableCards = new List<Card>();

            // Loop through the dictionary and clone each card
            foreach (var cardEntry in CardCatalogue.CardDictionary)
            {
                availableCards.Add(cardEntry.Value.Clone());
            }

            PopulateAvailableCards();
        }

        private void PopulateAvailableCards()
        {
            listAvailableCards.Items.Clear();

            string filterType = cmbFilterType.SelectedItem.ToString();
            CardElementFilter filterElement = cmbFilterElement.SelectedItem.ToString() == "All" ? CardElementFilter.All : (CardElementFilter)Enum.Parse(typeof(CardElementFilter), cmbFilterElement.SelectedItem.ToString());

            foreach (var card in availableCards)
            {
                bool matchesType = filterType == "All" || card.GetType().Name.Contains(filterType);
                bool matchesElement = filterElement == CardElementFilter.All || card.Element.ToString() == filterElement.ToString();

                if (matchesType && matchesElement)
                {
                    ListViewItem item = new ListViewItem(new string[]
                    {
                        card.Name,
                        card.Cost.ToString(),
                        card.Element.ToString(),
                        card.Description,
                    });
                    item.Tag = card;
                    listAvailableCards.Items.Add(item);
                }
            }
        }

        private void BtnAddCard_Click(object sender, EventArgs e)
        {
            if (listAvailableCards.SelectedItems.Count == 0) return;

            var selectedCard = (Card)listAvailableCards.SelectedItems[0].Tag;
            string cardName = selectedCard.Name;

            var currentDeck = isPlayer1Turn ? player1Deck : player2Deck;

            // Check if the card already exists in the current deck
            var existingCard = currentDeck.FirstOrDefault(c => c.Name == cardName);

            if (existingCard != null)
            {
                // If the card already exists, check the number of copies
                int cardCount = currentDeck.Count(c => c.Name == cardName);
                if (!(selectedCard is EssenceCard) && cardCount >= MAX_COPIES)
                {
                    MessageBox.Show("You can only add up to 4 copies of this card!");
                    return;
                }

                // Add the card again (duplicate in the list)
                currentDeck.Add(selectedCard.Clone());
            }
            else
            {
                // If the card doesn't exist in the deck, add it
                currentDeck.Add(selectedCard.Clone());
            }

            PopulateDeck();
        }

        private void BtnRemoveCard_Click(object sender, EventArgs e)
        {
            if (listDeck.SelectedItems.Count == 0) return;

            string cardName = listDeck.SelectedItems[0].Text;
            int selectedIndex = listDeck.SelectedItems[0].Index;

            var currentDeck = isPlayer1Turn ? player1Deck : player2Deck;

            // Find the card to remove from the deck
            var cardToRemove = currentDeck.FirstOrDefault(c => c.Name == cardName);

            if (cardToRemove != null)
            {
                currentDeck.Remove(cardToRemove);
            }

            PopulateDeck();

            if (listDeck.Items.Count > 0)
            {
                int newIndex = Math.Min(selectedIndex, listDeck.Items.Count - 1);
                listDeck.Items[newIndex].Selected = true;
                listDeck.Select();
            }
        }

        private void PopulateDeck()
        {
            listDeck.Items.Clear();

            var currentDeck = isPlayer1Turn ? player1Deck : player2Deck;

            // Group cards by name and display the count
            var groupedDeck = currentDeck.GroupBy(card => card.Name)
                                         .Select(group => new { CardName = group.Key, Count = group.Count() });

            foreach (var entry in groupedDeck)
            {
                listDeck.Items.Add(new ListViewItem(new string[] { entry.CardName, entry.Count.ToString() }));
            }

            // Toggle Save Deck button on deck population
            ToggleSaveDeckButton();
        }

        private void ListAvailableCards_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listAvailableCards.SelectedItems.Count > 0)
            {
                var card = (Card)listAvailableCards.SelectedItems[0].Tag;
                picCardPreview.SizeMode = PictureBoxSizeMode.Zoom;
                picCardPreview.Image = card.Image ?? null;
            }
        }

        private void BtnSaveDeck_Click(object sender, EventArgs e)
        {
            var currentDeck = isPlayer1Turn ? player1Deck : player2Deck;
            List<Card> finalDeck = new List<Card>();

            foreach (Card card in currentDeck)
            {
                finalDeck.Add(card.Clone());
            }

            // Ensure deck contains at least 10 Essence cards
            int essenceCardCount = currentDeck.Count(c => c is EssenceCard);
            if (essenceCardCount < 10)
            {
                MessageBox.Show("Your deck must contain at least 10 Essence cards!");
                return;
            }

            // Ensure deck contains at least 20 cards in total
            if (currentDeck.Count < 20)
            {
                MessageBox.Show("Your deck must contain at least 20 cards!");
                return;
            }

            // Save the deck to a text file
            string deckFileName = isPlayer1Turn ? "deckPlayer1.txt" : "deckPlayer2.txt";

            try
            {
                // Write the card names to the file
                var cardNames = currentDeck.Select(card => card.Name).ToList();
                File.WriteAllLines(deckFileName, cardNames);

                // Display message and update the corresponding player status
                if (isPlayer1Turn)
                {
                    isPlayer1DeckSaved = true;
                }
                else
                {
                    isPlayer2DeckSaved = true;
                }

                MessageBox.Show($"{(isPlayer1Turn ? "Player 1" : "Player 2")} deck saved successfully!");

                // Disable/Enable the Start Game button based on the deck save status
                UpdateStartGameButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving deck: {ex.Message}");
            }
        }

        private void UpdateStartGameButton()
        {
            // Check if both players have saved their decks
            btnStartGame.Enabled = isPlayer1DeckSaved && isPlayer2DeckSaved;
        }

        private void BtnSwitchPlayer_Click(object sender, EventArgs e)
        {
            isPlayer1Turn = !isPlayer1Turn;
            btnSwitchPlayer.Text = isPlayer1Turn ? "Switch to Player 2" : "Switch to Player 1";
            listDeck.Items.Clear(); // Clear the current deck view when switching players
            PopulateDeck();

            // Disable the Save Deck button if the current deck doesn't meet the requirements
            ToggleSaveDeckButton();
        }

        private void ToggleSaveDeckButton()
        {
            var currentDeck = isPlayer1Turn ? player1Deck : player2Deck;
            // Disable the Save Deck button if the current deck doesn't meet the minimum requirements
            btnSaveDeck.Enabled = currentDeck.Count >= 20 && currentDeck.Count(c => c is EssenceCard) >= 10;
        }

        private void BtnStartGame_Click(object sender, EventArgs e)
        {
            // Check if both players have saved their decks
            if (player1Deck.Count == 0 || player2Deck.Count == 0)
            {
                MessageBox.Show("Both players must have a deck before starting the game!");
                return;
            }

            // Transition to the game (here, we can just show a message for now)
            MessageBox.Show("Game starting with Player 1 and Player 2 decks!");

            Player p1 = new Player("Player 1", player1Deck);
            Player p2 = new Player("Player 2", player2Deck);

            new FRM_Game(p1, p2).Show();
            this.Hide(); // Optionally hide the deck builder form
        }

        private void FRM_SetupGame_Load(object sender, EventArgs e)
        {
            // Initially disable the Start Game button
            btnStartGame.Enabled = false;
        }

        private void LoadDecks()
        {
            // Load Player 1's deck from the file
            player1Deck = LoadDeckFromFile("deckPlayer1.txt");
            // Load Player 2's deck from the file
            player2Deck = LoadDeckFromFile("deckPlayer2.txt");

            // Populate the deck list UI based on the current player's deck
            PopulateDeck();
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
