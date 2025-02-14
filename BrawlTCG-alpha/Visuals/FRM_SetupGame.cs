using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Logic.Cards;
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

    public partial class FRM_SetupGame : Form
    {
        private ListView listAvailableCards, listDeck;
        private Button btnAddCard, btnRemoveCard, btnSaveDeck;
        private ComboBox cmbFilterType, cmbFilterElement;
        private PictureBox picCardPreview;
        private Dictionary<string, int> deck = new Dictionary<string, int>();
        private List<Card> availableCards;
        private const int MAX_COPIES = 4;

        public FRM_SetupGame()
        {
            Text = "Deck Builder";
            Size = new Size(800, 600);
            InitializeComponents();
            LoadCards();
        }

        private void InitializeComponents()
        {
            // 📜 Available Cards List (increased width and size)
            listAvailableCards = new ListView { Location = new Point(20, 50), Size = new Size(500, 400), View = View.Details, FullRowSelect = true };
            listAvailableCards.Columns.Add("Card Name", 150);
            listAvailableCards.Columns.Add("Cost", 70);
            listAvailableCards.Columns.Add("Description", 180);
            listAvailableCards.Columns.Add("Element", 80);
            listAvailableCards.SelectedIndexChanged += ListAvailableCards_SelectedIndexChanged;
            Controls.Add(listAvailableCards);

            // 🎴 Selected Deck List (shifted to the right of available cards)
            listDeck = new ListView { Location = new Point(540, 50), Size = new Size(300, 400), View = View.Details, FullRowSelect = true };
            listDeck.Columns.Add("Card Name", 200);
            listDeck.Columns.Add("Copies", 80);
            Controls.Add(listDeck);

            // ➕ Add Button (aligned with listDeck)
            btnAddCard = new Button { Text = "➕ Add", Location = new Point(540, 460), Size = new Size(100, 30) };
            btnAddCard.Click += BtnAddCard_Click;
            Controls.Add(btnAddCard);

            // ➖ Remove Button (below Add button)
            btnRemoveCard = new Button { Text = "➖ Remove", Location = new Point(540, 500), Size = new Size(100, 30) };
            btnRemoveCard.Click += BtnRemoveCard_Click;
            Controls.Add(btnRemoveCard);

            // 💾 Save Deck Button (below Remove button)
            btnSaveDeck = new Button { Text = "💾 Save Deck", Location = new Point(540, 540), Size = new Size(100, 30) };
            btnSaveDeck.Click += BtnSaveDeck_Click;
            Controls.Add(btnSaveDeck);

            // 🔍 Filter by Type (top left corner)
            cmbFilterType = new ComboBox { Location = new Point(20, 20), Width = 150 };
            cmbFilterType.Items.AddRange(new string[] { "All", "Legend", "Stage", "Move", "Essence" });
            cmbFilterType.SelectedIndex = 0;
            cmbFilterType.SelectedIndexChanged += (s, e) => PopulateAvailableCards();
            Controls.Add(cmbFilterType);

            // 🔥 Filter by Element (next to Filter by Type)
            cmbFilterElement = new ComboBox { Location = new Point(180, 20), Width = 140 };
            cmbFilterElement.Items.Add("All"); // Add "All" as the first option
            cmbFilterElement.Items.AddRange(Enum.GetNames(typeof(Elements))); // Add enum names
            cmbFilterElement.SelectedIndex = 0; // Set "All" as the default selected item
            cmbFilterElement.SelectedIndexChanged += (s, e) => PopulateAvailableCards();
            Controls.Add(cmbFilterElement);


            // 🖼️ Card Preview (right of the filters)
            picCardPreview = new PictureBox { Location = new Point(340, 460), Size = new Size(180, 240), BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(picCardPreview);
        }

        private void LoadCards()
        {
            availableCards = new List<Card>
    {
        // Essence
        CardCatalogue.Essence.Clone(),

        // Stages
        CardCatalogue.Mustafar.Clone(),
        CardCatalogue.Fangwild.Clone(),

        // Fire Legend Cards
        CardCatalogue.IronLady.Clone(),
        CardCatalogue.Heatblast.Clone(),

        // Cosmic Legend Cards
        CardCatalogue.Artemis.Clone(),
        CardCatalogue.Orion.Clone(),

        // Nature Legend Cards
        CardCatalogue.BriarRose.Clone(),
        CardCatalogue.ForestGuardian.Clone(),
        CardCatalogue.DeathCap.Clone(),

        // Magic Legend Cards
        CardCatalogue.FaerieQueen.Clone(),
        CardCatalogue.Enchantress.Clone(),
        CardCatalogue.DarkMage.Clone(),

        // Shadow Legend Cards
        CardCatalogue.MasterThief.Clone(),

        // Wild Legend Cards
        // You can add the missing cards for Wild types here like Beardvar, Asuri, etc.

        // Weapon Cards
        CardCatalogue.BlazingFire.Clone(),
        CardCatalogue.SleightOfHand.Clone(),
        CardCatalogue.LawOfTheLand.Clone(),
        CardCatalogue.GalaxyLance.Clone(),
        CardCatalogue.RemnantOfFate.Clone(),
        CardCatalogue.SacredRelic.Clone(),
        CardCatalogue.ScryingGlass.Clone(),
        CardCatalogue.SearingBlade.Clone(),
        CardCatalogue.ShootingStar.Clone(),
        CardCatalogue.StarryScythe.Clone(),
        CardCatalogue.MagmaSpear.Clone(),
        CardCatalogue.PiercingRegret.Clone()
    };

            PopulateAvailableCards();
        }


        private void PopulateAvailableCards()
        {
            listAvailableCards.Items.Clear();

            // Get the selected type filter (e.g., "All", "Legend", etc.)
            string filterType = cmbFilterType.SelectedItem.ToString();

            // Get the selected element filter using the new enum
            CardElementFilter filterElement;
            if (cmbFilterElement.SelectedItem.ToString() == "All")
            {
                filterElement = CardElementFilter.All; // 'All' in the new filter enum
            }
            else
            {
                filterElement = (CardElementFilter)Enum.Parse(typeof(CardElementFilter), cmbFilterElement.SelectedItem.ToString());
            }

            // Loop through the available cards
            foreach (var card in availableCards)
            {
                // Check if card matches the selected type and element filter
                bool matchesType = filterType == "All" || card.GetType().Name.Contains(filterType);

                // If the element filter is 'All', then any card element is allowed
                bool matchesElement = filterElement == CardElementFilter.All || card.Element.ToString() == filterElement.ToString();

                // Only add the card if it matches the filters
                if (matchesType && matchesElement)
                {
                    ListViewItem item = new ListViewItem(new string[]
                    {
                card.Name,
                card.Cost.ToString(),
                card.Description,
                card.Element.ToString()
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

            if (deck.ContainsKey(cardName))
            {
                if (!(selectedCard is EssenceCard) && deck[cardName] >= MAX_COPIES)
                {
                    MessageBox.Show("You can only add up to 4 copies of this card!");
                    return;
                }
                deck[cardName]++;
            }
            else
            {
                deck[cardName] = 1;
            }

            PopulateDeck();
        }

        private void BtnRemoveCard_Click(object sender, EventArgs e)
        {
            if (listDeck.SelectedItems.Count == 0) return;

            string cardName = listDeck.SelectedItems[0].Text;
            int selectedIndex = listDeck.SelectedItems[0].Index; // Save the index before removing

            if (deck.ContainsKey(cardName))
            {
                deck[cardName]--;

                if (deck[cardName] == 0)
                    deck.Remove(cardName);
            }

            PopulateDeck();

            // Automatically reselect the same index (if valid)
            if (listDeck.Items.Count > 0)
            {
                int newIndex = Math.Min(selectedIndex, listDeck.Items.Count - 1);
                listDeck.Items[newIndex].Selected = true;
                listDeck.Select(); // Keep focus on the list
            }
        }

        private void PopulateDeck()
        {
            listDeck.Items.Clear();

            foreach (var entry in deck)
            {
                listDeck.Items.Add(new ListViewItem(new string[] { entry.Key, entry.Value.ToString() }));
            }
        }

        private void ListAvailableCards_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listAvailableCards.SelectedItems.Count > 0)
            {
                var card = (Card)listAvailableCards.SelectedItems[0].Tag;
                if (card.Image != null)
                {
                    picCardPreview.SizeMode = PictureBoxSizeMode.Zoom; // Ensures scaling without distortion
                    picCardPreview.Image = card.Image;
                }
                else
                {
                    picCardPreview.Image = null; // Clear if no image available
                }
            }
        }

        private void BtnSaveDeck_Click(object sender, EventArgs e)
        {
            List<Card> finalDeck = new List<Card>();

            foreach (var entry in deck)
            {
                Card card = availableCards.FirstOrDefault(c => c.Name == entry.Key);
                if (card != null)
                {
                    for (int i = 0; i < entry.Value; i++)
                        finalDeck.Add(card.Clone());
                }
            }

            if (!finalDeck.Any(c => c is LegendCard))
            {
                MessageBox.Show("Your deck must contain at least one Legend card!");
                return;
            }

            MessageBox.Show("Deck saved successfully!");
        }
    }
}
