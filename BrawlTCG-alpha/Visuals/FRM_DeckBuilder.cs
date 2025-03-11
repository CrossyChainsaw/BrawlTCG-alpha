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
        Element,
        All,
        Fire,
        Magic,
        Nature,
        Cosmic,
        Wild,
        Shadow,
        Arctic
    }

    public partial class FRM_DeckBuilder : Form
    {
        private ListView listAvailableCards, listDeck;
        private Button btnAddCard, btnRemoveCard, btnSaveDeck, btnSwitchPlayer, btnStartGame;
        private ComboBox cmbFilterType, cmbFilterElement, cmbFilterWeapon;
        private PictureBox picCardPreview;
        private List<Card> player1Deck = new List<Card>();
        private List<Card> player2Deck = new List<Card>();
        private List<Card> availableCards;
        private const int MAX_COPIES = 4;
        private bool isPlayer1Turn = true; // Track whose turn it is to build the deck
        private bool isPlayer1DeckSaved = false;
        private bool isPlayer2DeckSaved = false;
        private ListBox listLegendAttacks, listCardEffects;
        private Card _selectedCard;

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
            listDeck.SelectedIndexChanged += ListDeck_SelectedIndexChanged;
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

            btnSwitchPlayer = new Button { Text = "Switch to Player 2", Location = new Point(840, 580), Size = new Size(100, 60) };
            btnSwitchPlayer.Click += BtnSwitchPlayer_Click;
            Controls.Add(btnSwitchPlayer);

            // Create the ComboBox for Card Type with a placeholder
            cmbFilterType = new ComboBox { Location = new Point(20, 20), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilterType.Items.Add("Card"); // Placeholder
            cmbFilterType.Items.AddRange(new string[] { "All", "Legend", "Stage", "Weapon", "Essence", "Battle" });
            cmbFilterType.SelectedIndex = 0; // Set default selection to "Card"
            cmbFilterType.DropDown += (s, e) => RemovePlaceholder(cmbFilterType, "Card");
            cmbFilterType.DropDownClosed += (s, e) => RestorePlaceholder(cmbFilterType, "Card");
            cmbFilterType.SelectedIndexChanged += (s, e) => PopulateAvailableCards();
            Controls.Add(cmbFilterType);

            // Create the ComboBox for Element Type with a placeholder
            cmbFilterElement = new ComboBox { Location = new Point(180, 20), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilterElement.Items.Add("Element"); // Placeholder
            cmbFilterElement.Items.AddRange(Enum.GetNames(typeof(CardElementFilter)));
            cmbFilterElement.SelectedIndex = 0; // Set default selection to "Element"
            cmbFilterElement.DropDown += (s, e) => RemovePlaceholder(cmbFilterElement, "Element");
            cmbFilterElement.DropDownClosed += (s, e) => RestorePlaceholder(cmbFilterElement, "Element");
            cmbFilterElement.SelectedIndexChanged += (s, e) => PopulateAvailableCards();
            Controls.Add(cmbFilterElement);

            // Create the ComboBox for Weapon Type with a placeholder
            cmbFilterWeapon = new ComboBox { Location = new Point(340, 20), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilterWeapon.Items.Add("All");
            cmbFilterWeapon.Items.AddRange(Enum.GetNames(typeof(Weapons)));
            cmbFilterWeapon.SelectedIndex = 0;
            cmbFilterWeapon.SelectedIndexChanged += (s, e) => PopulateAvailableCards();
            Controls.Add(cmbFilterWeapon);

            void RemovePlaceholder(ComboBox comboBox, string placeholder)
            {
                if (comboBox.SelectedIndex == 0 && comboBox.Items[0].ToString() == placeholder)
                {
                    comboBox.Items.RemoveAt(0); // Remove placeholder
                    comboBox.SelectedIndex = -1; // Reset selection
                }
            }
            void RestorePlaceholder(ComboBox comboBox, string placeholder)
            {
                if (comboBox.SelectedIndex == -1)
                {
                    comboBox.Items.Insert(0, placeholder); // Restore placeholder
                    comboBox.SelectedIndex = 0; // Select placeholder
                }
            }



            picCardPreview = new PictureBox { Location = new Point(640, 460), Size = new Size(180, 240), BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(picCardPreview);

            listLegendAttacks = new ListBox { Location = new Point(20, 460), Size = new Size(600, 100), Visible = false };
            Controls.Add(listLegendAttacks);

            listCardEffects = new ListBox { Location = new Point(20, 570), Size = new Size(600, 100), Visible = false };
            Controls.Add(listCardEffects);

            Size = new Size(1200, 800);
        }

        private void ListAvailableCards_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listAvailableCards.SelectedItems.Count > 0)
            {
                _selectedCard = (Card)listAvailableCards.SelectedItems[0].Tag;
                picCardPreview.SizeMode = PictureBoxSizeMode.Zoom;
                picCardPreview.Image = _selectedCard.Image ?? null;

                // Check if the selected card is a Legend
                if (_selectedCard is LegendCard legend)
                {
                    listLegendAttacks.Items.Clear();
                    listLegendAttacks.Visible = true;

                    foreach (var attack in legend.GetAttacks())
                    {
                        string weaponDescription = attack.WeaponTwo != null
                            ? $"{attack.WeaponOneAmount}x {attack.WeaponOne} {GetBurnWeaponEmojis(attack.WeaponOneBurnAmount)} + {attack.WeaponTwoAmount}x {attack.WeaponTwo} {GetBurnWeaponEmojis(attack.WeaponTwoBurnAmount)}"
                            : $"{attack.WeaponOneAmount}x {attack.WeaponOne} {GetBurnWeaponEmojis(attack.WeaponOneBurnAmount)}";
                        weaponDescription += attack.MultiHit ? " - Hits All" : "";

                        int dmg = AttackCatalogue.CalculateDamage(legend, attack);
                        listLegendAttacks.Items.Add($"{weaponDescription} - {dmg} Damage - {attack.Name}");
                    }
                }
                else
                {
                    listLegendAttacks.Visible = false;
                }

                // Display card effects
                listCardEffects.Items.Clear();
                listCardEffects.Visible = false;

                if (_selectedCard.StartTurnEffect != null) listCardEffects.Items.Add($"Start Turn Effect: {_selectedCard.StartTurnEffect.Description}");
                if (_selectedCard.EndTurnEffect != null) listCardEffects.Items.Add($"End Turn Effect: [WORK IN PROGRESS]");
                if (_selectedCard.WhenPlayedEffect != null) listCardEffects.Items.Add($"When Played Effect: {_selectedCard.WhenPlayedEffect.Description}");
                if (_selectedCard.WhenDiscardedEffect != null) listCardEffects.Items.Add($"When Discarded Effect: {_selectedCard.WhenDiscardedEffect.Description}");

                // Check for WhileInPlayEffect if it's a StageCard
                if (_selectedCard is StageCard stage && stage.WhileInPlayEffect != null)
                {
                    listCardEffects.Items.Add($"While In Play Effect: {stage.WhileInPlayEffect.Description}");
                }

                if (listCardEffects.Items.Count > 0)
                {
                    listCardEffects.Visible = true;
                }
            }
            else
            {
                listLegendAttacks.Visible = false;
                listCardEffects.Visible = false;
            }
        }

        private void ListDeck_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listDeck.SelectedItems.Count > 0)
            {
                string selectedCardName = listDeck.SelectedItems[0].Text;
                var currentDeck = isPlayer1Turn ? player1Deck : player2Deck;

                // Find the first instance of the card in the deck
                _selectedCard = currentDeck.FirstOrDefault(c => c.Name == selectedCardName);

                if (_selectedCard != null)
                {
                    picCardPreview.SizeMode = PictureBoxSizeMode.Zoom;
                    picCardPreview.Image = _selectedCard.Image ?? null;

                    // Display attacks if the card is a Legend
                    if (_selectedCard is LegendCard legend)
                    {
                        listLegendAttacks.Items.Clear();
                        listLegendAttacks.Visible = true;

                        foreach (var attack in legend.GetAttacks())
                        {
                            string weaponDescription = attack.WeaponTwo != null
                                ? $"{attack.WeaponOneAmount}x {attack.WeaponOne} {GetBurnWeaponEmojis(attack.WeaponOneBurnAmount)} + {attack.WeaponTwoAmount}x {attack.WeaponTwo} {GetBurnWeaponEmojis(attack.WeaponTwoBurnAmount)}"
                                : $"{attack.WeaponOneAmount}x {attack.WeaponOne} {GetBurnWeaponEmojis(attack.WeaponOneBurnAmount)}";
                            weaponDescription += attack.MultiHit ? " - Hits All" : "";

                            int dmg = AttackCatalogue.CalculateDamage(legend, attack);
                            listLegendAttacks.Items.Add($"{weaponDescription} - {dmg} Damage - {attack.Name}");
                        }
                    }
                    else
                    {
                        listLegendAttacks.Visible = false;
                    }

                    // Display card effects
                    listCardEffects.Items.Clear();
                    listCardEffects.Visible = false;

                    if (_selectedCard.StartTurnEffect != null) listCardEffects.Items.Add($"Start Turn Effect: {_selectedCard.StartTurnEffect.Description}");
                    if (_selectedCard.EndTurnEffect != null) listCardEffects.Items.Add($"End Turn Effect: [WORK IN PROGRESS]");
                    if (_selectedCard.WhenPlayedEffect != null) listCardEffects.Items.Add($"When Played Effect: {_selectedCard.WhenPlayedEffect.Description}");
                    if (_selectedCard.WhenDiscardedEffect != null) listCardEffects.Items.Add($"When Discarded Effect: {_selectedCard.WhenDiscardedEffect.Description}");

                    if (_selectedCard is StageCard stage && stage.WhileInPlayEffect != null)
                    {
                        listCardEffects.Items.Add($"While In Play Effect: {stage.WhileInPlayEffect.Description}");
                    }

                    if (listCardEffects.Items.Count > 0)
                    {
                        listCardEffects.Visible = true;
                    }
                }
            }
            else
            {
                listLegendAttacks.Visible = false;
                listCardEffects.Visible = false;
            }
        }

        string GetBurnWeaponEmojis(int nBurn)
        {
            string emojis = "";
            for (int i = 0; i < nBurn; i++)
            {
                emojis += "🔥";
            }
            return emojis;
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
            CardElementFilter filterElement = cmbFilterElement.SelectedItem.ToString() == "All" || cmbFilterElement.SelectedItem.ToString() == "Element" ?
                CardElementFilter.All : (CardElementFilter)Enum.Parse(typeof(CardElementFilter), cmbFilterElement.SelectedItem.ToString());

            string filterWeapon = cmbFilterWeapon.SelectedItem.ToString(); // Get selected weapon

            foreach (var card in availableCards)
            {
                bool matchesType = filterType == "All" || card.GetType().Name.Contains(filterType);
                bool matchesElement = filterElement == CardElementFilter.All || card.Element.ToString() == filterElement.ToString();
                bool matchesWeapon = filterWeapon == "All" || CardHasWeapon(card, filterWeapon);

                if (matchesType && matchesElement && matchesWeapon)
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

        private bool CardHasWeapon(Card card, string weaponName)
        {
            if (card is LegendCard legend)
            {
                return legend.PrimaryWeapon.ToString() == weaponName ||
                       (legend.SecondaryWeapon != null && legend.SecondaryWeapon.ToString() == weaponName);
            }
            if (card is WeaponCard weaponCard)
            {
                return weaponCard.Weapon.ToString() == weaponName;
            }
            return false;
        }

        private void BtnAddCard_Click(object sender, EventArgs e)
        {
            var selectedCard = _selectedCard;
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

        //private void ListAvailableCards_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (listAvailableCards.SelectedItems.Count > 0)
        //    {
        //        var card = (Card)listAvailableCards.SelectedItems[0].Tag;
        //        picCardPreview.SizeMode = PictureBoxSizeMode.Zoom;
        //        picCardPreview.Image = card.Image ?? null;
        //    }
        //}

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
                // Assuming currentDeck is your list of cards and card.ID is an integer
                List<int> cardIDs = currentDeck.Select(card => card.ID).ToList();

                // Convert the integers to strings
                List<string> cardIDStrings = cardIDs.Select(id => id.ToString()).ToList();

                // Write the card IDs to the file
                File.WriteAllLines(deckFileName, cardIDStrings);


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

        List<Card> LoadDeckFromFile(string filePath)
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
