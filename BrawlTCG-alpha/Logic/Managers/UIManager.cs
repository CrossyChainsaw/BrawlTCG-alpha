using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BrawlTCG_alpha.Logic
{
    public class UIManager
    {
        // Fields
        const int BASE_OFFSET_LEFT = 20;
        const int CARD_WIDTH = 150;
        const int CARD_HEIGHT = 200;
        List<ZoneControl> _zones = new List<ZoneControl>();
        NetworkManager _networkManager;
        FRM_Game _mainForm;
        private Game _game => _mainForm.game;

        // UI - Initialize
        public event Action UI_InitializeZones;
        public event Action<Player> UI_InitializeCardsInHand;
        public event Action<Player> UI_InitializeDeckPile;
        // UI - Update
        public event Action UI_UpdateCardControlsInPlayingFieldInformation;
        public event Action<Player> UI_UpdateEssenceCardsInEssenceField;
        public event Action<Player> UI_UpdatePlayerInformation;
        // UI - Cards
        public event Action<Player, Card> UI_AddCardToHandZone;
        public event Action<Player, ZoneTypes, bool> UI_EnableCardsInZone;
        public event Action<Player, Card> UI_MoveCardFromDeckZoneToHandZone;
        public event Action<Player, StageCard> UI_PlayStageCard;
        public event Action<Player, bool> UI_ShowCards;
        public event Action<Player> UI_UntapPlayerCards;
        // UI - Other
        public event Action<string> UI_PopUpNotification;


        // Methods
        public UIManager(FRM_Game mainForm, Action<string> ui_PopUp)
        {
            UI_PopUpNotification += ui_PopUp;

            _mainForm = mainForm;
            _networkManager = _mainForm.Network;
            _zones = new List<ZoneControl>();
        }



        // Initialize
        public void InitializeZones()
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
            CreateZone("Your Deck", leftOffset, bottomOffset, cardWidth, cardHeight, ZoneTypes.Deck, _game.Me);
            CreateZone("Your Cards", leftOffset + cardWidth + 20, bottomOffset, playingCardsWidth, cardHeight, ZoneTypes.Hand, _game.Me);
            CreateZone("Discard Pile", leftOffset + playingCardsWidth + 20 + cardWidth + 20, bottomOffset, cardWidth, cardHeight, ZoneTypes.DiscardPile, _game.Me);
            CreateZone($"{_game.Me.Name}\nHealth: {_game.Me.Health}\nEssence: {_game.Me.Essence}", leftOffset, bottomOffset - 20 - (cardHeight / 2 - 25), cardWidth, cardHeight / 2 - 25, ZoneTypes.PlayerInfo, _game.Me);
            CreateZone("Your Field", leftOffset + cardWidth + 20, bottomOffset - cardHeight - 20, fieldWidth, cardHeight, ZoneTypes.PlayingField, _game.Me);
            CreateZone("Your Essence", leftOffset + cardWidth + 20 + fieldWidth + 20, bottomOffset - cardHeight - 20, essenseWidth, cardHeight, ZoneTypes.EssenceField, _game.Me);

            // Enemy Zones
            CreateZone("Enemy Deck", leftOffset, topOffset, cardWidth, cardHeight, ZoneTypes.Deck, _game.Opponent);
            CreateZone("Enemy Cards", leftOffset + cardWidth + 20, topOffset, playingCardsWidth, cardHeight, ZoneTypes.Hand, _game.Opponent);
            CreateZone("Enemy Discard Pile", leftOffset + playingCardsWidth + 20 + cardWidth + 20, topOffset, cardWidth, cardHeight, ZoneTypes.DiscardPile, _game.Opponent);
            CreateZone($"{_game.Opponent.Name}\nHealth: {_game.Opponent.Health}\nEssence: {_game.Opponent.Essence}", leftOffset, 20 + cardHeight + 20, cardWidth, cardHeight / 2 - 25, ZoneTypes.PlayerInfo, _game.Opponent);
            CreateZone("Enemy Field", leftOffset + cardWidth + 20, topOffset + cardHeight + 20, fieldWidth, cardHeight, ZoneTypes.PlayingField, _game.Opponent);
            CreateZone("Enemy Essence", leftOffset + cardWidth + 20 + fieldWidth + 20, topOffset + cardHeight + 20, essenseWidth, cardHeight, ZoneTypes.EssenceField, _game.Opponent);

            // Neutral Zones
            CreateZone("Stage", leftOffset, 400, cardWidth, cardHeight, ZoneTypes.Stage, null);


            // Function to create zones
            void CreateZone(string name, int x, int y, int width, int height, ZoneTypes zoneType, Player? owner)
            {
                ZoneControl zone = new ZoneControl(name, width, height, zoneType, owner)
                {
                    Location = new Point(x, y)
                };

                // indicate turn
                if (zone.ZoneType == ZoneTypes.PlayerInfo)
                {
                    if (zone.Owner == _game.ActivePlayer)
                    {
                        zone.BackColor = Color.Green;
                    }
                }

                _zones.Add(zone);
                _mainForm.Controls.Add(zone);
            }
        }
        public void InitializeCardsInHand(Player player)
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
                    CardControl cardControl = new CardControl(_game, card, ArrangeCardsInPlayingField, isOpen: false, owner: player, players: _game.GetPlayers())
                    {
                        Location = new Point(startX + i * (CARD_WIDTH + spacing), handZone.Location.Y + 10),
                    };
                    cardControl.CardReleased += async () => await TryToSnapCard(cardControl, card, player);
                    cardControl.UI_AddCardToDiscardPile += MoveCardControlFromPlayingFieldToDiscardPile;

                    // Add the card to the controls and bring it to the front
                    AddCardControl(cardControl, handZone);
                }
            }
        }
        public void InitializeDeckPile(Player player)
        {
            ZoneControl zone = GetMyZone(ZoneTypes.Deck, player);

            if (player.Deck.Count > 0)
            {
                for (int i = 0; i < player.Deck.Count; i++)
                {
                    Card card = player.Deck[i];
                    CardControl cardControl = CreateCardControl(player, zone, card, false, extraPaddingY: i * 1);

                    AddCardControl(cardControl, zone);
                }
            }

        }


        // CardControl
        public CardControl? GetCardControl(Player player, ZoneTypes zoneType, Card card)
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
        /// <summary>Removes from UI and Zone</summary>
        void RemoveCardControl(CardControl cardControl, ZoneControl zone)
        {
            // Remove visually
            _mainForm.Controls.Remove(cardControl);
            zone.CardsControls.Remove(cardControl);
        }
        /// <summary>Adds to UI and Zone</summary>
        void AddCardControl(CardControl cardControl, ZoneControl zone)
        {
            _mainForm.Controls.Add(cardControl);
            zone.CardsControls.Add(cardControl);
            cardControl.BringToFront();
            zone.SendToBack();
        }
        /// <summary>Create a CardControl in a Zone</summary>
        CardControl CreateCardControl(Player player, ZoneControl zone, Card card, bool isOpen, int extraPaddingY = 0)
        {
            int padding = 10;
            CardControl cardControl = new CardControl(_game, card, ArrangeCardsInPlayingField, isOpen: isOpen, owner: player, players: _game.GetPlayers())
            {
                Location = new Point(zone.Location.X + padding, zone.Location.Y + padding + extraPaddingY),
            };
            cardControl.CardReleased += async () => await TryToSnapCard(cardControl, card, player);
            cardControl.UI_AddCardToDiscardPile += MoveCardControlFromPlayingFieldToDiscardPile;
            cardControl.UI_UpdatePlayerInformation += UpdatePlayerInformation;
            return cardControl;
        }
        /// <summary>Create a WeaponCardControl on top of a Legend</summary>
        CardControl CreateCardControl(Player player, CardControl legendCardControl, Card weapon, LegendCard legend)
        {
            CardControl cardControl = new CardControl(_game, weapon, ArrangeCardsInPlayingField, true, player, players: _game.GetPlayers()) // create the weapon card same loc as legendcard
            {
                Location = new Point(legendCardControl.Location.X, legendCardControl.Location.Y - (20 * legend.StackedCards.Count)) // make sure we stack weapon cards on weapon cards visually
            };
            cardControl.CardReleased += async () => await TryToSnapCard(cardControl, weapon, player);
            cardControl.UI_AddCardToDiscardPile += MoveCardControlFromPlayingFieldToDiscardPile;
            cardControl.UI_UpdatePlayerInformation += UpdatePlayerInformation;
            cardControl.SetCanDrag(false);
            return cardControl;
        }



        // Zones
        List<ZoneControl> GetZones(ZoneTypes targetZoneType)
        {
            List<ZoneControl> zones = new List<ZoneControl>();
            foreach (ZoneControl zone in _zones)
            {
                if (zone.ZoneType == targetZoneType)
                {
                    zones.Add(zone);
                }
            }
            return zones;
        }
        public ZoneControl? GetMyZone(ZoneTypes targetZoneType, Player player)
        {
            foreach (ZoneControl zone in _zones)
            {
                if (zone.ZoneType == targetZoneType && player == zone.Owner)
                {
                    return zone;
                }
            }
            return null;
        }
        ZoneControl GetStageZone()
        {
            return GetZones(ZoneTypes.Stage)[0];
        }



        //Update
        public void UpdateCardControlsInPlayingFieldInformation()
        {
            //List<CardControl> cardControls = new List<CardControl>();
            List<ZoneControl> zoneControls = GetZones(ZoneTypes.PlayingField);
            foreach (ZoneControl zoneControl in zoneControls)
            {
                foreach (CardControl cardControl in zoneControl.CardsControls.ToList())
                {
                    if (cardControl.Card is LegendCard legendCard && legendCard.IsTapped)
                    {
                        cardControl.Enabled = false;
                    }
                    cardControl.Invalidate();
                    cardControl.Update();
                    cardControl.CheckIfDead();
                }
            }
        }
        public void UpdateEssenceCardsInEssenceField(Player player)
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
                    CardControl cardControl = new CardControl(_game, card, ArrangeCardsInPlayingField, isOpen: true, players: this._game.GetPlayers())
                    {
                        Location = new Point(startX + i * (CARD_WIDTH + spacing), essenceZone.Location.Y + 10),
                    };
                    cardControl.CardReleased += async () => await TryToSnapCard(cardControl, card, player);
                    cardControl.UI_AddCardToDiscardPile += MoveCardControlFromPlayingFieldToDiscardPile;

                    // Add the card to the controls and bring it to the front
                    AddCardControl(cardControl, essenceZone);
                }
            }
        }
        internal void UpdatePlayerInformation(Player player)
        {
            ZoneControl zone = GetMyZone(ZoneTypes.PlayerInfo, player);
            zone.Label.Text = $"{player.Name}\nHealth: {player.Health}\nEssence: {player.Essence}";
        }



        // Build / Arrange
        internal void ArrangeCardsInPlayingField(Player player)
        {
            List<Card> cardList = player.PlayingField;
            int spacing = 20 * 3;
            ZoneControl zone = GetMyZone(ZoneTypes.PlayingField, player);
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
                    // Arrange Cards
                    zone.CardsControls[i].Location = new Point(startX + i * (CARD_WIDTH + spacing), zone.Location.Y + 10);
                    zone.CardsControls[i].SetCanDrag(false);

                    // Arrange Stacked Cards
                    if (zone.CardsControls[i].Card is LegendCard legendCard)
                    {
                        foreach (CardControl cardControl in zone.CardsControls[i].CardsControls)
                        {
                            cardControl.Location = new Point(startX + i * (CARD_WIDTH + spacing), cardControl.Location.Y);
                        }
                    }
                }
            }
        }
        /// <summary>Intended for Hand & Essence</summary>
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
                    // Arrange Cards
                    zone.CardsControls[i].Location = new Point(startX + i * (CARD_WIDTH + spacing), zone.Location.Y + 10);

                    // Arrange Stacked Cards
                    if (zone.CardsControls[i].Card is LegendCard legendCard)
                    {
                        foreach (CardControl cardControl in zone.CardsControls[i].CardsControls)
                        {
                            cardControl.Location = new Point(startX + i * (CARD_WIDTH + spacing), cardControl.Location.Y);
                        }
                    }
                }
            }
        }



        // Mouse
        bool IsMouseInZone(ZoneControl zone)
        {
            Point mousePos = _mainForm.PointToClient(Cursor.Position);
            Rectangle zoneBounds = zone.Parent.RectangleToScreen(zone.Bounds);
            zoneBounds = _mainForm.RectangleToClient(zoneBounds);
            return zoneBounds.Contains(mousePos);
        }
        bool IsMouseOnCardControl(CardControl cardControl)
        {
            Point mousePos = _mainForm.PointToClient(Cursor.Position);
            Rectangle zoneBounds = cardControl.Parent.RectangleToScreen(cardControl.Bounds);
            zoneBounds = _mainForm.RectangleToClient(zoneBounds);
            return zoneBounds.Contains(mousePos);
        }



        // Cards
        public void AddCardToHandZone(Player player, Card card)
        {
            ZoneControl handZone = GetMyZone(ZoneTypes.Hand, player);
            CardControl cardControl = CreateCardControl(player, handZone, card, false);
            AddCardControl(cardControl, handZone);
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);
        }
        public void EnableCardsInZone(Player player, ZoneTypes zoneType, bool enable = true)
        {
            ZoneControl zone = GetMyZone(zoneType, player);


            foreach (CardControl CC in zone.CardsControls)
            {
                if (enable && CC.Card.IsOpen)
                {
                    EnableCC(CC, true);
                }
                else
                {
                    EnableCC(CC, false);
                }
            }

            // Local Methods
            void EnableCC(CardControl CC, bool enable)
            {
                if (!CC.IsHandleCreated)
                {
                    CC.Enabled = enable;
                    return; // Exit early if the control is not ready
                }
                CC.Invoke(new Action(() => CC.Enabled = enable));
            }
        }
        public void MoveCardFromDeckZoneToHandZone(Player player, Card card)
        {
            // set variables
            ZoneTypes oldZoneType = ZoneTypes.Deck;

            // Find CardControl
            CardControl? cardControlOld = GetCardControl(player, oldZoneType, card);
            if (cardControlOld != null)
            {
                // Remove from old zone
                RemoveCardControl(cardControlOld, GetMyZone(oldZoneType, player));

                // Reinitailize in new zone
                AddCardToHandZone(player, card);
            }
        }
        /// <summary>Delete from UI and Zone, Add to UI and Zone</summary>
        void MoveCardControlFromPlayingFieldToDiscardPile(Player player, CardControl cardControl)
        {
            // Remove from UI and Zone
            ZoneControl playingFieldZone = GetMyZone(ZoneTypes.PlayingField, player);
            RemoveCardControl(cardControl, playingFieldZone);

            // Add to UI and Zone
            AddCardToDiscardPile(player, cardControl);
            cardControl.Enabled = false;
        }
        public void ShowCards(Player player, bool show = true)
        {
            ZoneControl playerHand = GetMyZone(ZoneTypes.Hand, player);
            foreach (CardControl cardControl in playerHand.CardsControls)
            {
                if (show)
                {
                    if (!cardControl.Card.IsOpen)
                    {
                        cardControl.FlipCard();
                    }
                }
                else
                {
                    if (cardControl.Card.IsOpen)
                    {
                        cardControl.FlipCard();
                    }
                }
            }
        }
        public void UntapPlayerCards(Player player)
        {
            ZoneControl zone = GetMyZone(ZoneTypes.PlayingField, player);
            foreach (CardControl cardControl in zone.CardsControls)
            {
                if (!cardControl.Card.IsOpen)
                {
                    cardControl.Card.IsOpen = true;
                    cardControl.Invalidate();
                    cardControl.Update();
                }
            }
        }        /// <summary>deletes old card control and creates a new one</summary>
        void AddCardToDiscardPile(Player player, CardControl cardControl)
        {
            cardControl.Card.Discard();
            // Add to UI and Zone
            ZoneControl discardPileZone = GetMyZone(ZoneTypes.DiscardPile, player);
            CardControl cardControlNew = CreateCardControl(player, discardPileZone, cardControl.Card, true);
            cardControlNew.Location = new Point(discardPileZone.Location.X + 10, discardPileZone.Location.Y + 10 + (player.DiscardPile.Count * 1));
            AddCardControl(cardControlNew, discardPileZone);
            
        }



        // Play Card
        internal async Task<bool> TryToSnapCard(CardControl cardControl, Card card, Player player)
        {
            if (card is EssenceCard)
            {
                bool result = TryPlayEssenceCard(player, card, cardControl);
                return result;
            }
            else if (card is StageCard stageCard)
            {
                bool result = TryPlayStageCard(player, stageCard, cardControl);
                return result;
            }
            else if (card is LegendCard legendCard)
            {
                bool result = TryPlayLegendCard(player, legendCard);
                return result;
            }
            else if (card is WeaponCard weaponCard)
            {
                bool result = TryPlayWeaponCard(player, weaponCard, cardControl);
                return result;
            }
            else if (card is BattleCard battleCard)
            {
                bool result = TryPlayBattleCard(player, battleCard, cardControl);
                return result;
            }
            return false;

        }
        bool TryPlayWeaponCard(Player player, WeaponCard weapon, CardControl cardControlOld)
        {
            ZoneControl playingFieldZone = GetMyZone(ZoneTypes.PlayingField, player);
            if (IsMouseInZone(playingFieldZone))
            {
                if (player.Essence >= weapon.Cost)
                {
                    // Try to get the card control you are hovering over
                    CardControl targetCardControl = null;
                    foreach (CardControl cardControl in playingFieldZone.CardsControls)
                    {
                        if (IsMouseOnCardControl(cardControl))
                        {
                            targetCardControl = cardControl;
                            break;
                        }
                    }

                    // if none, return a failed drag
                    if (targetCardControl == null)
                    {
                        return false;
                    }

                    // does the legend even have this weapon?
                    if (targetCardControl.Card is LegendCard legendCard)
                    {
                        if (legendCard.HasWeapon(weapon))
                        {
                            // get card index before removing it
                            int handIndex = player.Hand.IndexOf(weapon);
                            int fieldIndexCC = playingFieldZone.CardsControls.IndexOf(targetCardControl);

                            // Play card
                            PlayWeaponCard(player, legendCard, weapon, cardControlOld);

                            // communicate to opponent
                            string msg = $"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_LEGEND:{fieldIndexCC}";
                            _networkManager.SendMessageToPeer(msg);

                            return true;
                        }
                        else
                        {
                            MessageBox($"You cannot play a {weapon.Weapon} on this legend");
                        }
                    }
                }
                else
                {
                    MessageBox("Not enough Essence");
                }
            }
            return false;
        }
        bool TryPlayLegendCard(Player player, LegendCard legendCard)
        {
            ZoneControl playZone = GetMyZone(ZoneTypes.PlayingField, player);
            if (IsMouseInZone(playZone))
            {
                if (player.Essence >= legendCard.Cost)
                {
                    // are there already 5?
                    if (playZone.CardsControls.Count < 5)
                    {
                        // get card index before removing it
                        int handIndex = player.Hand.IndexOf(legendCard);

                        // Play the Card
                        PlayLegendCard(player, legendCard);

                        // communicate to opponent
                        string msg = $"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_ZONE:{ZoneTypes.PlayingField}";
                        _networkManager.SendMessageToPeer(msg);

                        return true;
                    }
                    else
                    {
                        MessageBox("Already 5 Legends");
                    }
                }
                else
                {
                    MessageBox("Not enough Essence");
                }
            }
            return false;
        }
        bool TryPlayStageCard(Player player, Card card, CardControl cardControl)
        {
            // Find stage zone
            ZoneControl stageZone = GetStageZone();

            if (IsMouseInZone(stageZone))
            {
                if (player.Essence >= card.Cost)
                {
                    // get card index before removing it
                    int handIndex = player.Hand.IndexOf(card);

                    // play the stage card
                    PlayStageCard(player, (StageCard)card);

                    // communicate to opponent
                    string msg = $"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_ZONE:{ZoneTypes.Stage}";
                    _networkManager.SendMessageToPeer(msg);

                    return true;
                }
                else
                {
                    MessageBox("Not enough Essence");
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
                if (!player.PlayedEssenceCardThisTurn())
                {
                    // get card index before removing it
                    int handIndex = player.Hand.IndexOf(card);

                    // play the essence card
                    PlayEssenceCard(player, card, cardControlOld, essenceZone);

                    // Communicate to opponent
                    string msg = $"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_ZONE:{ZoneTypes.EssenceField}";
                    _networkManager.SendMessageToPeer(msg);

                    return true;
                }
                else
                {
                    MessageBox("You already played Essence this turn");
                }
            }
            return false;
        }
        bool TryPlayBattleCard(Player player, BattleCard battleCard, CardControl cardControlOld)
        {
            // get opponent
            Player enemy = _game.GetOtherPlayer(player);
            ZoneControl enemyPlayingFieldZone = GetMyZone(ZoneTypes.PlayingField, enemy);
            ZoneControl myPlayingFieldZone = GetMyZone(ZoneTypes.PlayingField, player);


            if (IsMouseInZone(enemyPlayingFieldZone) || IsMouseInZone(myPlayingFieldZone))
            {
                if (player.Essence >= battleCard.Cost)
                {
                    CardControl targetCardControl = null;
                    int fieldIndexCC = -1;
                    bool friendlyFire = false;
                    if (battleCard.FriendlyFire)
                    {
                        // Try to get your own CardControl you are hovering over
                        foreach (CardControl cardControl in myPlayingFieldZone.CardsControls)
                        {
                            if (IsMouseOnCardControl(cardControl))
                            {
                                targetCardControl = cardControl;
                                fieldIndexCC = myPlayingFieldZone.CardsControls.IndexOf(targetCardControl);
                                friendlyFire = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Try to get your CardControl you are hovering over
                        foreach (CardControl cardControl in enemyPlayingFieldZone.CardsControls)
                        {
                            if (IsMouseOnCardControl(cardControl))
                            {
                                targetCardControl = cardControl;
                                fieldIndexCC = enemyPlayingFieldZone.CardsControls.IndexOf(targetCardControl);
                                friendlyFire = false;
                                break;
                            }
                        }
                    }
                    // Didn't find any CardControl
                    if (targetCardControl == null || fieldIndexCC == -1)
                    {
                        return false;
                    }


                    // Apply the effect
                    if (targetCardControl.Card is LegendCard)
                    {

                        // get card index before removing it
                        int handIndex = player.Hand.IndexOf(cardControlOld.Card);

                        // play battle card
                        PlayBattleCard(player, battleCard, cardControlOld, targetCardControl);

                        // Communicate to opponent
                        string msg = $"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_LEGEND:{fieldIndexCC}:FRIENDLY_FIRE:{friendlyFire}";
                        _networkManager.SendMessageToPeer(msg);

                        return true;
                    }
                    else
                    {
                        MessageBox($"You cannot play this BattleCard on this Card");
                    }
                }
                else
                {
                    MessageBox("Not enough Essence");
                }
            }
            return false;
        }
        /// <summary>Reorders the Z-Layers of a legend's stacked cards</summary>
        static void ReorderZLayer(CardControl legendCardControl)
        {
            for (int i = legendCardControl.CardsControls.Count - 1; i >= 0; i--)
            {
                CardControl cardControl = legendCardControl.CardsControls[i]; // battle or wep card
                cardControl.BringToFront();
            }
            // put the legend in front of the weapon
            legendCardControl.BringToFront();
        }
        public void PlayEssenceCard(Player player, Card card, CardControl cardControlOld, ZoneControl essenceZone)
        {
            // Play Card
            CardControl essenceCardControl = PlayCardInZone(player, card, cardControlOld, essenceZone);
            // Disable new Essence Card
            essenceCardControl.Enabled = false;
            // Arrange Cards
            ArrangeCards(player, ZoneTypes.EssenceField, player.EssenceField);
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);
        }
        public void PlayStageCard(Player player, StageCard stageCard)
        {
            // Find stage zone
            ZoneControl stageZone = GetStageZone();

            // discard old stage card
            if (_game.GetActiveStageCard() != null)
            {
                MoveOldStageCardToDiscardPile(stageZone);
            }

            // Play the card in the zone on screen
            CardControl stageCardControl = PlayCardInStageZone(player, stageCard);
            // set the stage card in game memory
            _game.SetStageCardFromForm(player, stageCard);
            // Disable Drag
            stageCardControl.SetCanDrag(false);

            // when played effect - (first get all cards on screen then do)
            if (stageCard.WhenPlayedEffect != null)
            {
                List<Card> l1 = _game.Me.GetAllCardsInPlayingField();
                List<Card> l2 = _game.Opponent.GetAllCardsInPlayingField();
                List<Card> allCards = l1.Concat(l2).ToList();
                stageCard.WhenPlayedEffect.Invoke(allCards, stageCard, _game);
            }

            // update all cards
            UpdateCardControlsInPlayingFieldInformation();

            // Local Functions
            void MoveOldStageCardToDiscardPile(ZoneControl stageZone)
            {
                // Find CardControl
                CardControl stageCardCardControl = stageZone.CardsControls[0];
                // Remove from UI
                RemoveCardControl(stageCardCardControl, stageZone);

                // Add card to discard pile
                StageCard oldStageCard = (StageCard)stageCardCardControl.Card; // = _game.ActiveStageCard;
                Player oldOwner = stageCardCardControl.Owner; //                  = _game.ActiveStageCardOwner;
                _game.AddCardToDiscardPile(oldOwner, oldStageCard);
                if (_game.GetActiveStageCard().WhenDiscardedEffect != null)
                {
                    List<Card> l1 = _game.Me.GetAllCardsInPlayingField();
                    List<Card> l2 = _game.Opponent.GetAllCardsInPlayingField();
                    List<Card> allCards = l1.Concat(l2).ToList();
                    _game.GetActiveStageCard().WhenDiscardedEffect.Invoke(null, stageCard, _game);
                }

                // add to discard pile visually
                ZoneControl discardPileZone = GetMyZone(ZoneTypes.DiscardPile, oldOwner);
                stageCardCardControl.Location = new Point(discardPileZone.Location.X + 10, discardPileZone.Location.Y + 10 + oldOwner.DiscardPile.Count * 1);
                AddCardControl(stageCardCardControl, discardPileZone);
            }
        }
        public void PlayLegendCard(Player player, LegendCard legendCard)
        {
            CardControl cardControl = GetCardControl(player, ZoneTypes.Hand, legendCard);
            ZoneControl playZone = GetMyZone(ZoneTypes.PlayingField, player);
            Game game = _mainForm.game;
            // Give legend Card the ability to burn cards (this should happen in legen initiliazation not here right?)
            legendCard.UI_BurnWeaponCard += BurnWeaponCard;
            // Play Card
            CardControl legendCC = PlayCardInZone(player, legendCard, cardControl, playZone);
            // when played effect
            legendCard.OnPlayedEffect(null, null, game);
            // the active stage effect
            if (game.GetActiveStageCard() != null)
            {
                if (game.GetActiveStageCard().WhileInPlayEffect != null)
                {
                    StageCard stage = game.GetActiveStageCard();
                    stage.WhileInPlayEffect(legendCard, stage, game);
                    legendCC.Invalidate();
                }
            }
            // Arrange Cards
            ArrangeCardsInPlayingField(player);
            // Arrange Cards in hand
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);
        }
        public void PlayWeaponCard(Player player, LegendCard legendCard, WeaponCard weapon, CardControl cardControlOld)
        {
            // Stack Card
            legendCard.StackCard(weapon);
            player.PlayCard(weapon); // play in memory
            UpdatePlayerInformation(player); // update essence

            // play in UI
            ZoneControl handZone = GetMyZone(ZoneTypes.Hand, player);
            RemoveCardControl(cardControlOld, handZone); // remove from hand

            // Find the legend card
            CardControl legendCardControl = GetCardControl(player, ZoneTypes.PlayingField, legendCard); // first find the legend control
            CardControl cardControlNew = CreateCardControl(player, legendCardControl, weapon, legendCard); // Create a weapon card behind the legend

            // Add to UI
            _mainForm.Controls.Add(cardControlNew);
            legendCardControl.CardsControls.Add(cardControlNew);

            // Reorder Z-Layer Stacked Cards
            ReorderZLayer(legendCardControl);

            // Rearrange cards in hand
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);
        }
        public void PlayBattleCard(Player player, BattleCard battleCard, CardControl cardControlOld, CardControl targetCardControl)
        {
            // Card Effect
            LegendCard targetLegend = (LegendCard)targetCardControl.Card;
            battleCard.OnPlayedEffect(targetLegend, battleCard, _game);
            // Update CardControl Info
            if (battleCard.MultiTarget) // update all my legends
            {
                ZoneControl playZone = GetMyZone(ZoneTypes.PlayingField, player);
                foreach (CardControl CC in playZone.CardsControls)
                {
                    if (CC.Card is LegendCard)
                    {
                        CC.Invalidate();
                        CC.Update();
                    }
                }
            }
            else // only update target legend
            {
                targetCardControl.Invalidate();
                targetCardControl.CheckIfDead();
            }

            // remove card from hand (and in DP for some cards)
            player.PlayCard(battleCard);

            // remove card from hand visually
            ZoneControl handZone = GetMyZone(ZoneTypes.Hand, player);
            RemoveCardControl(cardControlOld, handZone);

            // Add to UI and Zone visually
            if (battleCard.OneTimeUse)
            {
                AddCardToDiscardPile(player, cardControlOld);
            }
            else
            {
                // stack the card
                targetLegend.StackCard(battleCard);
                // create the new CardControl
                CardControl battleCardControl = CreateCardControl(player, targetCardControl, battleCard, targetLegend);

                // Add to UI
                _mainForm.Controls.Add(battleCardControl);
                targetCardControl.CardsControls.Add(battleCardControl);

                // Reorder Z-Layer Stacked Cards
                ReorderZLayer(targetCardControl);
            }
            cardControlOld.Enabled = false;

            // Arrange
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);

            // Update player info
            UpdatePlayerInformation(player);
        }
        /// <summary>Handles playing cards visually and logically</summary>
        CardControl PlayCardInZone(Player player, Card card, CardControl cardControlOld, ZoneControl targetZone)
        {
            // Remove Card from Hand, Add to x
            player.PlayCard(card);

            // Remove Visually
            ZoneControl handZone = GetMyZone(ZoneTypes.Hand, player);
            RemoveCardControl(cardControlOld, handZone);

            // Add Visually
            CardControl cardControl = CreateCardControl(player, targetZone, card, true);
            AddCardControl(cardControl, targetZone);

            // Update info for essence
            UpdatePlayerInformation(player);

            return cardControl;
        }
        CardControl PlayCardInStageZone(Player player, StageCard card)
        {
            // Find stage zone
            ZoneControl stageZone = GetStageZone();
            // Find stage CardControl
            CardControl stageCardControl = GetCardControl(player, ZoneTypes.Hand, card);

            // Play card
            player.PlayCard(card);

            // Remove from UI
            ZoneControl handZone = GetMyZone(ZoneTypes.Hand, player);
            RemoveCardControl(stageCardControl, handZone);

            // Create the new control in the correct zone and position
            CardControl cardControl = CreateCardControl(player, stageZone, card, true);

            // Add it in the UI
            AddCardControl(cardControl, stageZone);
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);

            // Update player essence
            UpdatePlayerInformation(player);

            // Return the new Control
            return cardControl;
        }
        void BurnWeaponCard(LegendCard legendCard, WeaponCard wepCard)
        {
            Player player = _game.ActivePlayer;
            ZoneControl zone = GetMyZone(ZoneTypes.PlayingField, player);
            foreach (CardControl cardControl in zone.CardsControls)
            {
                // is the legend card matching
                if (cardControl.Card == legendCard)
                {
                    // remove the wep card
                    foreach (CardControl wepCardControl in cardControl.CardsControls.ToList())
                    {
                        if (wepCardControl.Card == wepCard)
                        {
                            // Remove Logically
                            cardControl.CardsControls.Remove(wepCardControl);
                            // Remove Visually
                            _mainForm.Controls.Remove(wepCardControl);
                            // Add to discard pile logically
                            player.DiscardPile.Add(wepCard);
                            // Add to discard pile visually 
                            AddCardToDiscardPile(player, wepCardControl);
                        }
                    }
                }
            }
        }



        // Other
        public void MessageBox(string s)
        {
            UI_PopUpNotification.Invoke(s);
        }
    }
}
