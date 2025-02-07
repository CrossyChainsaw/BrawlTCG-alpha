using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;
using Microsoft.VisualBasic.Devices;
using System.Configuration;
using System.Numerics;
using System.Security.Policy;

namespace BrawlTCG_alpha
{
    public partial class FRM_PlayingField : Form
    {
        // Any sort of relation to logic
        Player player1 = new Player("John", CardCatalogue.Deck1);
        Player player2 = new Player("Jane", CardCatalogue.Deck2);

        // Visuals
        const int BASE_OFFSET_LEFT = 20;
        const int CARD_WIDTH = 150;
        const int CARD_HEIGHT = 200;
        List<ZoneControl> Zones = new List<ZoneControl>();
        Color _gameBackgroundColor = Color.MidnightBlue;
        Game _game;

        // Methods & Events
        public FRM_PlayingField()
        {
            InitializeComponent();
            _game = new Game(player1, player2);

            InitializePlayingFieldZones(); // build the playing field
            BackColor = _gameBackgroundColor; // background color

            // Single
            _game.UI_ChangeCardZone += ChangeCardZone;
            _game.UI_EnableCards += EnableCards;
            _game.UI_ShowCards += ShowCards;
            _game.UI_InitializeCardsInHand += InitializeCardsInHand;
            _game.UI_UpdateEssenceCardsInEssenceField += InitializeCardsInEssenceField;
            _game.UI_UpdateCardsInDeckPile += UpdateCardsInDeckPile;
            _game.UI_UpdatePlayerInformation += UpdatePlayerInformation;
            // Multi
            _game.UI_Multi_DisableCardsOnEssenceZones += DisableCardsOnEssenceZones;
            _game.UI_Multi_InitializeDeckPiles += InitializeDeckPiles;
        }
        private void FRM_PlayingField_Load(object sender, EventArgs e)
        {
            _game.Prepare();
            _game.Start();
        }

        // Visual UI Functions
        /// <summary>Removes the Deck Pile if you have 0 cards in your deck pile</summary>
        void UpdateCardsInDeckPile(Player player)
        {
            // If you have no cards in your deck pile, make the deck pile dissapear
            ZoneControl zone = GetMyZone(ZoneTypes.Deck, player);
            if (player.Deck.Count == 0)
            {
                foreach (CardControl cardControl in zone.CardsControls.ToList())
                {
                    if (cardControl.Owner == player)
                    {
                        // Remove both from the screen and from memory
                        RemoveCardControl(cardControl, zone);
                    }
                }
            }
        }
        void RemoveCardControl(CardControl cardControl, ZoneControl zone)
        {
            Controls.Remove(cardControl);
            zone.CardsControls.Remove(cardControl);
        }
        void AddCardControl(CardControl cardControl, ZoneControl zone)
        {
            Controls.Add(cardControl);
            zone.CardsControls.Add(cardControl);
            cardControl.BringToFront();
            zone.SendToBack();
        }
        void ShowCards(Player player, bool show)
        {
            ZoneControl playerHand = GetMyZone(ZoneTypes.Hand, player);
            foreach (CardControl card in playerHand.CardsControls)
            {
                if (show)
                {
                    if (!card.IsOpen)
                    {
                        card.FlipCard();
                    }
                }
                else
                {
                    if (card.IsOpen)
                    {
                        card.FlipCard();
                    }
                }
            }
        }
        void EnableCards(Player player, bool enable)
        {
            ZoneControl zone = GetMyZone(ZoneTypes.Hand, player);
            foreach (CardControl card in zone.CardsControls)
            {
                if (enable)
                {
                    card.Enabled = true;
                }
                else
                {
                    card.Enabled = false;
                }
            }
        }
        void DisableCardsOnEssenceZones()
        {
            List<ZoneControl> zones = GetZones(ZoneTypes.EssenceField);
            foreach (ZoneControl zone in zones)
            {
                foreach (CardControl card in zone.CardsControls)
                {
                    card.Enabled = false;
                }
            }
        }
        void UpdatePlayerInformation(Player player)
        {
            ZoneControl zone = GetMyZone(ZoneTypes.PlayerInfo, player);
            zone.Label.Text = $"Health: {player.Health}\nEssence: {player.Essence}";
        }
        // Build & Rearrange the Playing Field
        void InitializePlayingFieldZones()
        {
            // Fields
            int cardHeight = CARD_HEIGHT + 20;
            int cardWidth = CARD_WIDTH + 20;
            int leftOffset = BASE_OFFSET_LEFT;
            int topOffset = 20;
            int playingCardsWidth = 1500;
            int fieldWidth = 1200;
            int essenseWidth = CARD_WIDTH * 3;
            int bottomOffset = 1080 - (cardHeight + 100);

            // Player Zones
            CreateZone("Your Deck", leftOffset, bottomOffset, cardWidth, cardHeight, ZoneTypes.Deck, _game.BottomPlayer);
            CreateZone("Your Cards", leftOffset + cardWidth + 20, bottomOffset, playingCardsWidth, cardHeight, ZoneTypes.Hand, _game.BottomPlayer);
            CreateZone("Discard Pile", leftOffset + playingCardsWidth + 20 + cardWidth + 20, bottomOffset, cardWidth, cardHeight, ZoneTypes.DiscardPile, _game.BottomPlayer);
            CreateZone($"Health: {_game.BottomPlayer.Health}\nEssence: {_game.BottomPlayer.Essence}", leftOffset, bottomOffset - 20 - (cardHeight / 2 - 25), cardWidth, cardHeight / 2 - 25, ZoneTypes.PlayerInfo, _game.BottomPlayer);
            CreateZone("Your Field", leftOffset + cardWidth + 20, bottomOffset - cardHeight - 20, fieldWidth, cardHeight, ZoneTypes.PlayingField, _game.BottomPlayer);
            CreateZone("Your Essence", leftOffset + cardWidth + 20 + fieldWidth + 20, bottomOffset - cardHeight - 20, essenseWidth, cardHeight, ZoneTypes.EssenceField, _game.BottomPlayer);

            // Enemy Zones
            CreateZone("Enemy Deck", leftOffset, topOffset, cardWidth, cardHeight, ZoneTypes.Deck, _game.TopPlayer);
            CreateZone("Enemy Cards", leftOffset + cardWidth + 20, topOffset, playingCardsWidth, cardHeight, ZoneTypes.Hand, _game.TopPlayer);
            CreateZone("Enemy Discard Pile", leftOffset + playingCardsWidth + 20 + cardWidth + 20, topOffset, cardWidth, cardHeight, ZoneTypes.DiscardPile, _game.TopPlayer);
            CreateZone($"Health: {_game.TopPlayer.Health}\nEssence: {_game.TopPlayer.Essence}", leftOffset, 20 + cardHeight + 20, cardWidth, cardHeight / 2 - 25, ZoneTypes.PlayerInfo, _game.TopPlayer);
            CreateZone("Enemy Field", leftOffset + cardWidth + 20, topOffset + cardHeight + 20, fieldWidth, cardHeight, ZoneTypes.PlayingField, _game.TopPlayer);
            CreateZone("Enemy Essence", leftOffset + cardWidth + 20 + fieldWidth + 20, topOffset + cardHeight + 20, essenseWidth, cardHeight, ZoneTypes.EssenceField, _game.TopPlayer);

            // Neutral Zones
            CreateZone("Stage", leftOffset, 400, cardWidth, cardHeight, ZoneTypes.Stage, null);


            // Function to create zones
            void CreateZone(string name, int x, int y, int width, int height, ZoneTypes zoneType, Player owner)
            {
                ZoneControl zone = new ZoneControl(name, width, height, zoneType, owner)
                {
                    Location = new Point(x, y)
                };

                Zones.Add(zone);
                this.Controls.Add(zone);
            }

        }
        void InitializeDeckPiles()
        {
            List<ZoneControl> zones = GetZones(ZoneTypes.Deck);

            foreach (ZoneControl zone in zones)
            {
                Player player = (zone.Owner == _game.BottomPlayer) ? _game.BottomPlayer : _game.TopPlayer;

                if (player.Deck.Count > 0)
                {
                    foreach (Card card in player.Deck)
                    {
                        CardControl cardControl = new CardControl(card, owner: player)
                        {
                            Location = new Point(zone.Location.X + 10, zone.Location.Y + 10),
                        };
                        cardControl.CardReleased += async () => await TryToSnapCard(cardControl, card, player);

                        AddCardControl(cardControl, zone);
                    }
                }
            }
        }
        void InitializeCardsInHand(Player player)
        {
            if (player.Hand.Count > 0)
            {
                // first delete the current hand
                ZoneControl zone = GetMyZone(ZoneTypes.Hand, player);
                foreach (CardControl cardControl in zone.CardsControls.ToList())
                {
                    if (cardControl.Owner == player)
                    {
                        RemoveCardControl(cardControl, zone);
                    }
                }

                ZoneControl handZone = GetMyZone(ZoneTypes.Hand, player);

                // Default spacing
                int spacing = 20; // Initial spacing between cards

                // Calculate the total width needed for the cards and spacing
                int totalCardWidth = CARD_WIDTH * player.Hand.Count;
                int totalSpacingWidth = spacing * (player.Hand.Count - 1);

                // Check if the total width exceeds the handZone width, and adjust spacing accordingly
                int totalWidth = totalCardWidth + totalSpacingWidth;
                if (totalWidth > handZone.Width) // If the cards + spacing exceed the available width
                {
                    // Reduce spacing to fit within the handZone width
                    spacing = (handZone.Width - totalCardWidth - 20) / (player.Hand.Count - 1);
                }

                // Calculate the starting X position to center the cards
                int startX = handZone.Location.X + (handZone.Width - (CARD_WIDTH * player.Hand.Count) - (spacing * (player.Hand.Count - 1))) / 2;

                for (int i = 0; i < player.Hand.Count; i++)
                {
                    Card card = player.Hand[i];

                    // Create the card control and position it based on the calculated X and fixed Y
                    CardControl cardControl = new CardControl(card, isOpen: false, owner: player)
                    {
                        Location = new Point(startX + i * (CARD_WIDTH + spacing), handZone.Location.Y + 10),
                    };
                    cardControl.CardReleased += async () => await TryToSnapCard(cardControl, card, player);

                    // Add the card to the controls and bring it to the front
                    AddCardControl(cardControl, handZone);
                }
            }
        }
        void InitializeCardsInEssenceField(Player player)
        {
            if (player.EssenceField.Count > 0)
            {
                ZoneControl essenceZone = GetMyZone(ZoneTypes.EssenceField, player);

                // Default spacing
                int spacing = 20; // Initial spacing between cards

                // Calculate the total width needed for the cards and spacing
                int totalCardWidth = CARD_WIDTH * player.EssenceField.Count;
                int totalSpacingWidth = spacing * (player.EssenceField.Count - 1);

                // Check if the total width exceeds the handZone width, and adjust spacing accordingly
                int totalWidth = totalCardWidth + totalSpacingWidth;
                if (totalWidth > essenceZone.Width) // If the cards + spacing exceed the available width
                {
                    // Reduce spacing to fit within the handZone width
                    spacing = (essenceZone.Width - totalCardWidth - 20) / (player.EssenceField.Count - 1);
                }

                // Calculate the starting X position to center the cards
                int startX = essenceZone.Location.X + (essenceZone.Width - (CARD_WIDTH * player.EssenceField.Count) - (spacing * (player.EssenceField.Count - 1))) / 2;

                for (int i = 0; i < player.EssenceField.Count; i++)
                {
                    Card card = player.EssenceField[i];

                    // Create the card control and position it based on the calculated X and fixed Y
                    CardControl cardControl = new CardControl(card, isOpen: true)
                    {
                        Location = new Point(startX + i * (CARD_WIDTH + spacing), essenceZone.Location.Y + 10),
                    };
                    cardControl.CardReleased += async () => await TryToSnapCard(cardControl, card, player);

                    // Add the card to the controls and bring it to the front
                    AddCardControl(cardControl, essenceZone);
                }
            }
        }
        void ArrangeCards(Player player, ZoneTypes zoneType, List<Card> cardList, int spacing = 20)
        {
            ZoneControl zone = GetMyZone(zoneType, player);
            if (cardList.Count != zone.CardsControls.Count)
            {
                throw new Exception("Both lists should be equal");
            }
            if (cardList.Count > 0)
            {
                // Default spacing is passed as an argument, can be adjusted
                // Calculate the total width needed for the cards and spacing
                int totalCardWidth = CARD_WIDTH * cardList.Count;
                int totalSpacingWidth = spacing * (cardList.Count - 1);

                // Check if the total width exceeds the handZone width, and adjust spacing accordingly
                int totalWidth = totalCardWidth + totalSpacingWidth;
                if (totalWidth > zone.Width) // If the cards + spacing exceed the available width
                {
                    // Reduce spacing to fit within the handZone width
                    spacing = (zone.Width - totalCardWidth - 20) / (cardList.Count - 1);
                }

                // Calculate the starting X position to center the cards
                int startX = zone.Location.X + (zone.Width - (CARD_WIDTH * cardList.Count) - (spacing * (cardList.Count - 1))) / 2;

                for (int i = 0; i < cardList.Count; i++)
                {
                    zone.CardsControls[i].Location = new Point(startX + i * (CARD_WIDTH + spacing), zone.Location.Y + 10);
                }
            }
        }


        // Frontend Logic
        List<ZoneControl> GetZones(ZoneTypes targetZoneType)
        {
            List<ZoneControl> zones = new List<ZoneControl>();
            foreach (ZoneControl zone in Zones)
            {
                if (zone.ZoneType == targetZoneType)
                {
                    zones.Add(zone);
                }
            }
            return zones;
        }
        ZoneControl GetMyZone(ZoneTypes targetZoneType, Player player)
        {
            foreach (ZoneControl zone in Zones)
            {
                if (zone.ZoneType == targetZoneType && player == zone.Owner)
                {
                    return zone;
                }
            }
            return null;
        }
        CardControl? GetCardControl(Player player, ZoneTypes zoneType, Card card)
        {
            ZoneControl zone = GetMyZone(zoneType, player);
            foreach (CardControl cardControl in zone.CardsControls)
            {
                if (cardControl.Card == card)
                {
                    return cardControl;
                }
            }
            return null;
        }
        void ChangeCardZone(Player player, Card card, ZoneTypes oldZoneType, ZoneTypes targetZoneType)
        {
            CardControl? cardControlOld = GetCardControl(player, oldZoneType, card);
            RemoveCardControl(cardControlOld, GetMyZone(oldZoneType, player));

            CardControl cardControl = new CardControl(card, isOpen: cardControlOld.IsOpen, owner: player)
            {
                Location = new Point(0, 0) // rearrange it
            };
            cardControl.CardReleased += async () => await TryToSnapCard(cardControl, card, player);

            AddCardControl(cardControl, GetMyZone(targetZoneType, player));
            if (targetZoneType == ZoneTypes.Hand)
            {
                ArrangeCards(player, ZoneTypes.Hand, player.Hand);
            }
            else
            {
                throw new Exception();
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space)
            {
                _game.SwitchTurn();
                _game.StartTurn();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        // Play Cards
        internal async Task<bool> TryToSnapCard(CardControl cardControl, Card card, Player player)
        {
            if (card is EssenceCard)
            {
                bool result = TryPlayEssenceCard(player, card, cardControl);
                return result;
            }
            else if (card is StageCard)
            {
                bool result = TryPlayStageCard(player, card, cardControl);
                return result;
            }
            else if (card is LegendCard)
            {
                bool result = TryPlayLegendCard(player, card, cardControl);
                return result;
            }
            return false;
        }
        bool TryPlayStageCard(Player player, Card card, CardControl cardControlOld)
        {
            ZoneControl stageZone = GetZones(ZoneTypes.Stage)[0];
            if (IsMouseInZone(stageZone))
            {
                if (player.Essence >= card.Cost)
                {
                    PlayCardInZone(player, card, cardControlOld, stageZone);
                    return true;
                }
                else
                {
                    MessageBox.Show("Not enough Essence");
                }
            }
            return false;
        }
        bool TryPlayEssenceCard(Player player, Card card, CardControl cardControlOld)
        {
            // Check if the mouse is in the Essence Field
            ZoneControl essenceZone = GetMyZone(ZoneTypes.EssenceField, player);
            if (IsMouseInZone(essenceZone))
            {
                // Only 1 essence card per turn
                if (!player.PlayedEssenceCardThisTurn())
                {
                    PlayCardInZone(player, card, cardControlOld, essenceZone, (p) =>
                    {
                        p.PlayedEssenceCardThisTurn(true);
                        ArrangeCards(p, ZoneTypes.EssenceField, p.EssenceField);
                    });
                    return true;
                }
                else
                {
                    MessageBox.Show("You already played Essence this turn");
                }
            }
            return false;
        }
        bool TryPlayLegendCard(Player player, Card card, CardControl cardControlOld)
        {
            ZoneControl playZone = GetMyZone(ZoneTypes.PlayingField, player);
            if (IsMouseInZone(playZone))
            {
                if (player.Essence >= card.Cost)
                {
                    // are there already 5?
                    if (playZone.CardsControls.Count < 5)
                    {
                        PlayCardInZone(player, card, cardControlOld, playZone, (p) =>
                        {
                            ArrangeCards(p, ZoneTypes.PlayingField, p.PlayingField, spacing: 60);
                        });
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Already 5 Legends");
                    }
                }
                else
                {
                    MessageBox.Show("Not enough Essence");
                }
            }
            return false;
        }
        bool IsMouseInZone(ZoneControl zone)
        {
            Point mousePos = this.PointToClient(Cursor.Position);
            Rectangle zoneBounds = zone.Parent.RectangleToScreen(zone.Bounds);
            zoneBounds = this.RectangleToClient(zoneBounds);
            return zoneBounds.Contains(mousePos);
        }
        /// <summary>Handles playing cards visually and logically</summary>
        void PlayCardInZone(Player player, Card card, CardControl cardControlOld, ZoneControl targetZone, Action<Player> postPlayAction = null)
        {
            player.PlayCard(card);
            RemoveCardControl(cardControlOld, GetMyZone(ZoneTypes.Hand, player));

            CardControl cardControl = new CardControl(card, isOpen: true, owner: player)
            {
                Location = new Point(targetZone.Location.X + 10, targetZone.Location.Y + 10)
            };
            cardControl.CardReleased += async () => await TryToSnapCard(cardControl, card, player);

            AddCardControl(cardControl, targetZone);
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);

            postPlayAction?.Invoke(player);  // Additional actions specific to the card type

            cardControl.Enabled = false; // Disable after playing
            UpdatePlayerInformation(player);
        }
    }
}
