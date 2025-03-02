#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;
using Microsoft.VisualBasic.Devices;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Policy;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace BrawlTCG_alpha
{
    public partial class FRM_Game : Form
    {
        // Visuals
        const int BASE_OFFSET_LEFT = 20;
        const int CARD_WIDTH = 150;
        const int CARD_HEIGHT = 200;
        List<ZoneControl> Zones = new List<ZoneControl>();
        Color _gameBackgroundColor = Color.MidnightBlue;
        Game _game;

        // Networking
        private TcpClient _client;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private bool _isHost, _myTurn;

        // Methods

        // For Host
        internal FRM_Game(TcpListener host, TcpClient client, Player hostPlayer, Player peerPlayer)
        {
            // setup
            InitializeComponent();
            _isHost = true;
            _myTurn = true;
            this.Text = "BrawlTCG Host";

            // Setup Client
            _client = client;
            _stream = client.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream) { AutoFlush = true };

            // SETUP GAME
            _game = new Game(hostPlayer, peerPlayer, (message) => MessageBox.Show(message), EnableCardsInZone);
            SetupBoard(_game);
            _game.Prepare();
            _game.Start();

            // Start listening for incoming messages
            Task.Run(() => ListenForMessages());
        }

        // For Peer
        internal FRM_Game(TcpClient client, Player hostPlayer, Player peerPlayer)
        {
            // setup
            InitializeComponent();
            _isHost = true;
            _myTurn = true;
            this.Text = "BrawlTCG Peer";

            // Setup Client
            _client = client;
            _stream = client.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream) { AutoFlush = true };

            // SETUP GAME
            _game = new Game(peerPlayer, hostPlayer, (message) => MessageBox.Show(message), EnableCardsInZone);
            SetupBoard(_game);
            _game.Prepare();
            _game.Start();

            // Start listening for incoming messages
            Task.Run(() => ListenForMessages());
        }

        void SetupBoard(Game game)
        {
            BackColor = _gameBackgroundColor; // background color

            // Give all the functions to the game class so that the game class controls the entire game.
            // Single
            game.UI_InitializeZones += InitializeZones;
            game.UI_MoveCardZoneFromDeckToHand += MoveCardFromDeckZoneToHandZone;
            game.UI_EnableCardsInZone += EnableCardsInZone;
            game.UI_ShowCards += ShowCards;
            game.UI_InitializeCardsInHand += InitializeCardsInHand;
            game.UI_UpdateEssenceCardsInEssenceField += InitializeCardsInEssenceField;
            game.UI_UpdateCardControlInPlayingFieldInformation += UpdateCardControlsInPlayingFieldInformation;
            game.UI_UpdatePlayerInformation += UpdatePlayerInfo;
            game.UI_UntapPlayerCards += UntapPlayerCards;
            game.UI_AddCardToHandZone += AddCardToHandZone;
            game.UI_PlayStageCard += PlayStageCard;
            // Multi
            game.UI_Multi_InitializeDeckPiles += InitializeDeckPiles;
            // Non-Player
            game.UI_PopUpNotification += (message) => MessageBox.Show(message);
        }


        // Networking Methods

        public void SendMessageToServer(string message)
        {
            try
            {
                if (_game.GetActivePlayer() == _game.GetMe())
                {
                    if (_writer != null)
                    {
                        _writer.WriteLine(message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}");
            }
        }

        // Methods
        async Task ListenForMessages()
        {
            while (true)
            {
                string message = await _reader.ReadLineAsync();  // Use async/await here for non-blocking
                if (message != null)
                {
                    string[] parts = message.Split(':');

                    if (parts[0] == "SWITCH_TURN")
                    {
                        // Use Invoke to safely update the UI from the background thread
                        this.Invoke((Action)(() =>
                        {
                            SwitchTurn();
                        }));
                    }
                    else if (parts[0] == "PLAY_CARD") // PLAY_CARD:HAND_INDEX:<hand-index>:TARGET_ZONE:<target-zone>
                    {
                        if (parts[4] == ZoneTypes.EssenceField.ToString())
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            Card card = _game.GetOpponent().Hand[handIndex];
                            CardControl oldCC = GetCardControl(_game.GetOpponent(), ZoneTypes.Hand, card);
                            ZoneControl zone = GetMyZone(ZoneTypes.EssenceField, _game.GetOpponent());
                            this.Invoke((Action)(() =>
                            {
                                PlayEssenceCard(_game.GetOpponent(), card, oldCC, zone);
                            }));
                        }
                        else if (parts[4] == ZoneTypes.Stage.ToString())
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            Card card = _game.GetOpponent().Hand[handIndex];
                            this.Invoke((Action)(() =>
                            {
                                PlayStageCard(_game.GetOpponent(), (StageCard)card);
                            }));
                        }
                        else if (parts[4] == ZoneTypes.PlayingField.ToString())
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            LegendCard legendCard = (LegendCard)_game.GetOpponent().Hand[handIndex];
                            CardControl oldCC = GetCardControl(_game.GetOpponent(), ZoneTypes.Hand, legendCard);
                            ZoneControl zone = GetMyZone(ZoneTypes.PlayingField, _game.GetOpponent());
                            this.Invoke((Action)(() =>
                            {
                                PlayLegendCard(_game.GetOpponent(), legendCard, oldCC, zone);
                            }));
                        }
                        else if (parts[3] == "TARGET_LEGEND")
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            Card card = _game.GetOpponent().Hand[handIndex];

                            // play wep card
                            if (card is WeaponCard weaponCard)
                            {
                                int indexCC = Convert.ToInt32(parts[4]);
                                ZoneControl playZone = GetMyZone(ZoneTypes.PlayingField, _game.GetOpponent());
                                CardControl legendCC = playZone.CardsControls[indexCC];
                                CardControl oldCC = GetCardControl(_game.GetOpponent(), ZoneTypes.Hand, weaponCard);
                                this.Invoke((Action)(() =>
                                {
                                    PlayWeaponCard(_game.GetOpponent(), (LegendCard)legendCC.Card, weaponCard, oldCC);
                                }));
                            }
                            // play battle card
                            else if (card is BattleCard battleCard)
                            {
                                int indexCC = Convert.ToInt32(parts[4]); // card control index
                                bool friendlyFire = bool.Parse(parts[6]);
                                Player targetPlayer;
                                ZoneControl targetZone;
                                if (friendlyFire)
                                {
                                    targetPlayer = _game.GetOpponent();
                                }
                                else
                                {
                                    targetPlayer = _game.GetMe();
                                }
                                targetZone = GetMyZone(ZoneTypes.PlayingField, targetPlayer);
                                CardControl oldCC = GetCardControl(_game.GetOpponent(), ZoneTypes.Hand, card);
                                CardControl targetCC = targetZone.CardsControls[indexCC];

                                this.Invoke((Action)(() =>
                                {
                                    PlayBattleCard(_game.GetOpponent(), battleCard, oldCC, targetCC);
                                }));
                            }
                        }
                    }
                    else if (parts[0] == "ATTACK_PLAYER")
                    {
                        // Find legend
                        int fieldIndex = Convert.ToInt32(parts[2]);
                        Card card = _game.GetOpponent().PlayingField[fieldIndex];
                        CardControl legendCC = GetCardControl(_game.GetOpponent(), ZoneTypes.PlayingField, card);
                        LegendCard legend = (LegendCard)legendCC.Card;

                        // Find attack
                        string correctAttackName = parts[4];
                        Attack chosenAttack = null;
                        foreach (Attack attack in legend.GetAttacks())
                        {
                            if (attack.Name == correctAttackName)
                            {
                                chosenAttack = attack;
                            }
                        }

                        // perform the attack
                        this.Invoke((Action)(() =>
                        {
                            legendCC.AttackThePlayer(legendCC, _game.GetMe(), chosenAttack);
                        }));
                    }
                    else if (parts[0] == "STATUS_ATTACK")
                    {
                        // Find legend
                        int fieldIndex = Convert.ToInt32(parts[2]);
                        Card card = _game.GetOpponent().PlayingField[fieldIndex];
                        CardControl legendCC = GetCardControl(_game.GetOpponent(), ZoneTypes.PlayingField, card);
                        LegendCard legend = (LegendCard)legendCC.Card;

                        // Find attack
                        string correctAttackName = parts[4];
                        Attack chosenAttack = null;
                        foreach (Attack attack in legend.GetAttacks())
                        {
                            if (attack.Name == correctAttackName)
                            {
                                chosenAttack = attack;
                            }
                        }

                        // perform the attack
                        this.Invoke((Action)(() =>
                        {
                            chosenAttack.Effect.Invoke(legend, null, chosenAttack, _game.GetActivePlayer(), _game);
                        }));
                    }
                    else if (parts[0] == "ATTACK_LEGEND") // NETWORK_SendMessage($"ATTACK_LEGEND:LEGEND_INDEX:{fieldIndex}:ATTACK:{attack.Name}");
                    {
                        // Find attacker
                        int fieldIndex = Convert.ToInt32(parts[2]);
                        Card card = _game.GetOpponent().PlayingField[fieldIndex];
                        CardControl legendCC = GetCardControl(_game.GetOpponent(), ZoneTypes.PlayingField, card);
                        LegendCard legend = (LegendCard)legendCC.Card;

                        // Find attack
                        string correctAttackName = parts[4];
                        Attack chosenAttack = null;
                        foreach (Attack attack in legend.GetAttacks())
                        {
                            if (attack.Name == correctAttackName)
                            {
                                chosenAttack = attack;
                            }
                        }

                        // Find Target
                        CardControl targetCC;
                        if (chosenAttack.FriendlyFire)
                        {
                            int targetFieldIndex = Convert.ToInt32(parts[6]);
                            Card targetCard = _game.GetOpponent().PlayingField[targetFieldIndex]; // crash
                            targetCC = GetCardControl(_game.GetOpponent(), ZoneTypes.PlayingField, targetCard);
                        }
                        else
                        {
                            int targetFieldIndex = Convert.ToInt32(parts[6]);
                            Card targetCard = _game.GetMe().PlayingField[targetFieldIndex]; // crash
                            targetCC = GetCardControl(_game.GetMe(), ZoneTypes.PlayingField, targetCard);
                        }


                        // ATTACK
                        _game.StartAttack(chosenAttack);
                        this.Invoke((Action)(() =>
                        {
                            legendCC.AttackLegendCard(legendCC, targetCC);
                        }));
                    }
                }
            }
        }



        // Inside Game.cs
        void ShowCards(Player player, bool show = true)
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
        void EnableCardsInZone(Player player, ZoneTypes zoneType, bool enable = true)
        {
            ZoneControl zone = GetMyZone(zoneType, player);


            foreach (CardControl CC in zone.CardsControls)
            {
                if (enable && CC.Card.IsOpen)
                {
                    // if already attacked, don't enable
                    if (CC.Card is LegendCard legend && legend.AttackedThisTurn)
                    {
                        EnableCC(CC, false);
                        continue;
                    }
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

        void UpdateCardControlsInPlayingFieldInformation()
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
        void UntapPlayerCards(Player player)
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
        }
        void InitializeZones()
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
            CreateZone("Your Deck", leftOffset, bottomOffset, cardWidth, cardHeight, ZoneTypes.Deck, _game.GetMe());
            CreateZone("Your Cards", leftOffset + cardWidth + 20, bottomOffset, playingCardsWidth, cardHeight, ZoneTypes.Hand, _game.GetMe());
            CreateZone("Discard Pile", leftOffset + playingCardsWidth + 20 + cardWidth + 20, bottomOffset, cardWidth, cardHeight, ZoneTypes.DiscardPile, _game.GetMe());
            CreateZone($"{_game.GetMe().Name}\nHealth: {_game.GetMe().Health}\nEssence: {_game.GetMe().Essence}", leftOffset, bottomOffset - 20 - (cardHeight / 2 - 25), cardWidth, cardHeight / 2 - 25, ZoneTypes.PlayerInfo, _game.GetMe());
            CreateZone("Your Field", leftOffset + cardWidth + 20, bottomOffset - cardHeight - 20, fieldWidth, cardHeight, ZoneTypes.PlayingField, _game.GetMe());
            CreateZone("Your Essence", leftOffset + cardWidth + 20 + fieldWidth + 20, bottomOffset - cardHeight - 20, essenseWidth, cardHeight, ZoneTypes.EssenceField, _game.GetMe());

            // Enemy Zones
            CreateZone("Enemy Deck", leftOffset, topOffset, cardWidth, cardHeight, ZoneTypes.Deck, _game.GetOpponent());
            CreateZone("Enemy Cards", leftOffset + cardWidth + 20, topOffset, playingCardsWidth, cardHeight, ZoneTypes.Hand, _game.GetOpponent());
            CreateZone("Enemy Discard Pile", leftOffset + playingCardsWidth + 20 + cardWidth + 20, topOffset, cardWidth, cardHeight, ZoneTypes.DiscardPile, _game.GetOpponent());
            CreateZone($"{_game.GetOpponent().Name}\nHealth: {_game.GetOpponent().Health}\nEssence: {_game.GetOpponent().Essence}", leftOffset, 20 + cardHeight + 20, cardWidth, cardHeight / 2 - 25, ZoneTypes.PlayerInfo, _game.GetOpponent());
            CreateZone("Enemy Field", leftOffset + cardWidth + 20, topOffset + cardHeight + 20, fieldWidth, cardHeight, ZoneTypes.PlayingField, _game.GetOpponent());
            CreateZone("Enemy Essence", leftOffset + cardWidth + 20 + fieldWidth + 20, topOffset + cardHeight + 20, essenseWidth, cardHeight, ZoneTypes.EssenceField, _game.GetOpponent());

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
                    if (zone.Owner == _game.GetActivePlayer())
                    {
                        zone.BackColor = Color.Green;
                    }
                }

                Zones.Add(zone);
                this.Controls.Add(zone);
            }

        }
        void InitializeDeckPiles()
        {
            List<ZoneControl> zones = GetZones(ZoneTypes.Deck);

            foreach (ZoneControl zone in zones)
            {
                Player player = (zone.Owner == _game.GetMe()) ? _game.GetMe() : _game.GetOpponent();

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
                    CardControl cardControl = new CardControl(_game, card, ArrangeCardsInPlayingField, isOpen: true, players: _game.GetPlayers())
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
        // needs a rework, only used to change a card from deck to hand
        void MoveCardFromDeckZoneToHandZone(Player player, Card card)
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
        internal void AddCardToHandZone(Player player, Card card)
        {
            ZoneControl handZone = GetMyZone(ZoneTypes.Hand, player);
            CardControl cardControl = CreateCardControl(player, handZone, card, false);
            AddCardControl(cardControl, handZone);
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);
        }

        // LegendCard.cs
        void BurnWeaponCard(LegendCard legendCard, WeaponCard wepCard)
        {
            Player player = _game.GetActivePlayer();
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
                            Controls.Remove(wepCardControl);
                            // Add to discard pile logically
                            player.DiscardPile.Add(wepCard);
                            // Add to discard pile visually 
                            AddCardToDiscardPile(player, wepCardControl);
                        }
                    }
                }
            }
        }
        // Multiple
        internal void UpdatePlayerInfo(Player player)
        {
            ZoneControl zone = GetMyZone(ZoneTypes.PlayerInfo, player);
            zone.Label.Text = $"{player.Name}\nHealth: {player.Health}\nEssence: {player.Essence}";
        }


        // Visual UI Functions
        /// <summary>Removes from UI and Zone</summary>
        internal void RemoveCardControl(CardControl cardControl, ZoneControl zone)
        {
            // Remove visually
            Controls.Remove(cardControl);
            zone.CardsControls.Remove(cardControl);
        }
        /// <summary>Adds to UI and Zone</summary>
        void AddCardControl(CardControl cardControl, ZoneControl zone)
        {
            Controls.Add(cardControl);
            zone.CardsControls.Add(cardControl);
            cardControl.BringToFront();
            zone.SendToBack();
        }




        // Build & Rearrange the Playing Field
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
        void RearrangeMyStackedCards()
        {

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
        internal ZoneControl? GetMyZone(ZoneTypes targetZoneType, Player player)
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
        internal CardControl? GetCardControl(Player player, ZoneTypes zoneType, Card card)
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
        internal ZoneControl GetStageZone()
        {
            return GetZones(ZoneTypes.Stage)[0];
        }


        // Play Cards
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
                bool result = TryPlayLegendCard(player, legendCard, cardControl);
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
                            SendMessageToServer($"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_LEGEND:{fieldIndexCC}");

                            return true;
                        }
                        else
                        {
                            MessageBox.Show($"You cannot play a {weapon.Weapon} on this legend");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not enough Essence");
                }
            }
            return false;
        }
        bool TryPlayLegendCard(Player player, LegendCard legendCard, CardControl cardControlOld)
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
                        PlayLegendCard(player, legendCard, cardControlOld, playZone);

                        // communicate to opponent
                        SendMessageToServer($"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_ZONE:{ZoneTypes.PlayingField}");

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
                    SendMessageToServer($"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_ZONE:{ZoneTypes.Stage}");

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
                if (!player.PlayedEssenceCardThisTurn())
                {
                    // get card index before removing it
                    int handIndex = player.Hand.IndexOf(card);

                    // play the essence card
                    PlayEssenceCard(player, card, cardControlOld, essenceZone);

                    // Communicate to opponent
                    SendMessageToServer($"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_ZONE:{ZoneTypes.EssenceField}");

                    return true;
                }
                else
                {
                    MessageBox.Show("You already played Essence this turn");
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
                        SendMessageToServer($"PLAY_CARD:HAND_INDEX:{handIndex}:TARGET_LEGEND:{fieldIndexCC}:FRIENDLY_FIRE:{friendlyFire}");

                        return true;
                    }
                    else
                    {
                        MessageBox.Show($"You cannot play this BattleCard on this Card");
                    }
                }
                else
                {
                    MessageBox.Show("Not enough Essence");
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


        bool IsMouseInZone(ZoneControl zone)
        {
            Point mousePos = this.PointToClient(Cursor.Position);
            Rectangle zoneBounds = zone.Parent.RectangleToScreen(zone.Bounds);
            zoneBounds = this.RectangleToClient(zoneBounds);
            return zoneBounds.Contains(mousePos);
        }
        bool IsMouseOnCardControl(CardControl cardControl)
        {
            Point mousePos = this.PointToClient(Cursor.Position);
            Rectangle zoneBounds = cardControl.Parent.RectangleToScreen(cardControl.Bounds);
            zoneBounds = this.RectangleToClient(zoneBounds);
            return zoneBounds.Contains(mousePos);
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

        /// <summary>deletes old card control and creates a new one</summary>
        void AddCardToDiscardPile(Player player, CardControl cardControlOld)
        {
            // Add to UI and Zone
            ZoneControl discardPileZone = GetMyZone(ZoneTypes.DiscardPile, player);
            CardControl cardControlNew = CreateCardControl(player, discardPileZone, cardControlOld.Card, true);
            cardControlNew.Location = new Point(discardPileZone.Location.X + 10, discardPileZone.Location.Y + 10 + (player.DiscardPile.Count * 3));
            AddCardControl(cardControlNew, discardPileZone);
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
            UpdatePlayerInfo(player);

            return cardControl;
        } // COMMUNICATE PLAY CARD
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
            UpdatePlayerInfo(player);

            // Return the new Control
            return cardControl;
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
            cardControl.UI_UpdatePlayerInformation += UpdatePlayerInfo;
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
            cardControl.UI_UpdatePlayerInformation += UpdatePlayerInfo;
            cardControl.SetCanDrag(false);
            return cardControl;
        }

        // COMMUNICATION-READY METHODS
        void SwitchTurn()
        {
            _game.SwitchTurn();
            _game.StartTurn();

            // Update UI (turn indication)
            ZoneControl zone = GetMyZone(ZoneTypes.PlayerInfo, _game.GetActivePlayer());
            zone.BackColor = Color.Green;

            ZoneControl zone2 = GetMyZone(ZoneTypes.PlayerInfo, _game.GetInactivePlayer());
            zone2.BackColor = SystemColors.ControlDarkDark;
        }
        void PlayEssenceCard(Player player, Card card, CardControl cardControlOld, ZoneControl essenceZone)
        {
            // Play Card
            CardControl essenceCardControl = PlayCardInZone(player, card, cardControlOld, essenceZone);
            // Disable new Essence Card
            essenceCardControl.Enabled = false;
            // Arrange Cards
            ArrangeCards(player, ZoneTypes.EssenceField, player.EssenceField);
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);
        }
        void PlayStageCard(Player player, StageCard stageCard)
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
                List<Card> l1 = _game.GetMe().GetAllCardsInPlayingField();
                List<Card> l2 = _game.GetOpponent().GetAllCardsInPlayingField();
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
                    List<Card> l1 = _game.GetMe().GetAllCardsInPlayingField();
                    List<Card> l2 = _game.GetOpponent().GetAllCardsInPlayingField();
                    List<Card> allCards = l1.Concat(l2).ToList();
                    _game.GetActiveStageCard().WhenDiscardedEffect.Invoke(null, stageCard, _game);
                }

                // add to discard pile visually
                ZoneControl discardPileZone = GetMyZone(ZoneTypes.DiscardPile, oldOwner);
                stageCardCardControl.Location = new Point(discardPileZone.Location.X + 10, discardPileZone.Location.Y + 10 + oldOwner.DiscardPile.Count * 3);
                AddCardControl(stageCardCardControl, discardPileZone);
            }
        }
        void PlayLegendCard(Player player, LegendCard legendCard, CardControl cardControlOld, ZoneControl playZone)
        {
            // Give legend Card the ability to burn cards (this should happen in legen initiliazation not here right?)
            legendCard.UI_BurnWeaponCard += BurnWeaponCard;
            // Play Card
            CardControl legendCardControl = PlayCardInZone(player, legendCard, cardControlOld, playZone);
            // when played effect
            legendCard.OnPlayedEffect(null, null, _game);
            // the active stage effect
            if (_game.GetActiveStageCard() != null)
            {
                if (_game.GetActiveStageCard().WhileInPlayEffect != null)
                {
                    StageCard stage = _game.GetActiveStageCard();
                    stage.WhileInPlayEffect(legendCard, stage, _game);
                    legendCardControl.Invalidate();
                }
            }
            // Arrange Cards
            ArrangeCardsInPlayingField(player);
            // Arrange Cards in hand
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);
        }
        void PlayWeaponCard(Player player, LegendCard legendCard, WeaponCard weapon, CardControl cardControlOld)
        {
            // Stack Card
            legendCard.StackCard(weapon);
            player.PlayCard(weapon); // play in memory
            UpdatePlayerInfo(player); // update essence

            // play in UI
            ZoneControl handZone = GetMyZone(ZoneTypes.Hand, player);
            RemoveCardControl(cardControlOld, handZone); // remove from hand

            // Find the legend card
            CardControl legendCardControl = GetCardControl(player, ZoneTypes.PlayingField, legendCard); // first find the legend control
            CardControl cardControlNew = CreateCardControl(player, legendCardControl, weapon, legendCard); // Create a weapon card behind the legend

            // Add to UI
            Controls.Add(cardControlNew);
            legendCardControl.CardsControls.Add(cardControlNew);

            // Reorder Z-Layer Stacked Cards
            ReorderZLayer(legendCardControl);

            // Rearrange cards in hand
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);
        }
        void PlayBattleCard(Player player, BattleCard battleCard, CardControl cardControlOld, CardControl targetCardControl)
        {
            // Card Effect
            LegendCard targetLegend = (LegendCard)targetCardControl.Card;
            battleCard.OnPlayedEffect(targetLegend, battleCard, _game);
            targetCardControl.Invalidate();
            targetCardControl.CheckIfDead();

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
                Controls.Add(battleCardControl);
                targetCardControl.CardsControls.Add(battleCardControl);

                // Reorder Z-Layer Stacked Cards
                ReorderZLayer(targetCardControl);
            }
            cardControlOld.Enabled = false;

            // Arrange
            ArrangeCards(player, ZoneTypes.Hand, player.Hand);

            // Update player info
            UpdatePlayerInfo(player);
        } // THIS ONE DOESNT WORK!!!!!!!!!!!!!!!

        // Events
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space && _game.GetMe() == _game.GetActivePlayer())
            {
                // Send END_TURN to the server
                SendMessageToServer("SWITCH_TURN");
                SwitchTurn();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        } // COMMUNICATION !
    }

}



#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.