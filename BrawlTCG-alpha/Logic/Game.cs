#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace BrawlTCG_alpha.Logic
{
    public class Game
    {
        // Properties
        public Player ActivePlayer => _playerManager.ActivePlayer;
        public Player InactivePlayer => _playerManager.InactivePlayer;
        public Player Me => _playerManager.Me;
        public Player Opponent => _playerManager.Opponent;
        public List<int> RandomCardIDs { get; private set; } = new List<int>();
        public UIManager UiManager { get; private set; }

        // Fields
        const int STARTING_ESSENCE = 1; // 1
        const int STARTING_HAND_CARDS = 7; // 7
        const int MAX_CARDS_IN_HAND = 15;
        StageCardManager _stageCardManager;
        PlayerManager _playerManager;
        AttackManager _attackManager;
        NetworkManager _networkManager;



        public Game(Player player1, Player player2, NetworkManager networkManager, UIManager uiManager)
        {
            // Managers
            UiManager = uiManager;
            _stageCardManager = new StageCardManager(this);
            _playerManager = new PlayerManager(player1, player2, UiManager);
            _attackManager = new AttackManager(UiManager);
            _networkManager = networkManager;
        }
        public void Prepare()
        {
            // Setup all the zones visually
            UiManager.InitializeZones();

            // Define Players
            //RandomizeStartingPlayer(); // host always starts

            // Initialize Decks Visually
            UiManager.InitializeDeckPile(ActivePlayer);
            UiManager.InitializeDeckPile(InactivePlayer);

            UiManager.EnableCardsInZone(ActivePlayer, ZoneTypes.Deck, false);
            UiManager.EnableCardsInZone(InactivePlayer, ZoneTypes.Deck, false);

            // Draw Starting Hands and Display Visually
            DrawStartingHand(ActivePlayer, STARTING_HAND_CARDS);
            DrawStartingHand(InactivePlayer, STARTING_HAND_CARDS);
            UiManager.InitializeCardsInHand(ActivePlayer);
            UiManager.InitializeCardsInHand(InactivePlayer);

            // Obtain first Essence card and display visually - and disable them
            for (int i = 0; i < STARTING_ESSENCE; i++)
            {
                ActivePlayer.EssenceField.Add(CardCatalogue.GetCardById(cardId: 0)); // 0 is essence card
                InactivePlayer.EssenceField.Add(CardCatalogue.GetCardById(cardId: 0));
            }

            // update cards in essence fields
            UiManager.UpdateEssenceCardsInEssenceField(ActivePlayer);
            UiManager.UpdateEssenceCardsInEssenceField(InactivePlayer);
            // Disable cards in essence zones
            UiManager.EnableCardsInZone(ActivePlayer, ZoneTypes.EssenceField, false);
            UiManager.EnableCardsInZone(InactivePlayer, ZoneTypes.EssenceField, false);

            // only show your own cards
            ShowCards();
        }
        void DrawStartingHand(Player player, int nCards)
        {
            for (int i = 0; i < nCards; i++)
            {
                Card card = player.DrawCardFromDeck();
                UiManager.MoveCardFromDeckZoneToHandZone(player, card);
            }

        }
        public void Start()
        {
            //SwitchTurn();
            StartTurn();
        }
        public Task StartTurn()
        {
            // Start Turn
            if (ActivePlayer.Hand.Count < MAX_CARDS_IN_HAND)
            {
                DrawCardFromDeck(ActivePlayer); // draw card
            }
            _stageCardManager.StartTurnEffect(ActivePlayer);
            BurnDamage();
            UiManager.UpdateCardControlsInPlayingFieldInformation();
            ActivePlayer.GetEssence();

            ShowCards(); // SHOWS THE CARDS BY FLIPPING THEM
            UiManager.UpdatePlayerInformation(ActivePlayer);
            return Task.CompletedTask;
        }
        public void SwitchTurn()
        {
            // END TURN
            // ...
            // untap all cards of the active player
            UiManager.UntapPlayerCards(ActivePlayer);

            // Legends can attack again
            foreach (LegendCard legend in GetAllMyLegendsOnThePlayingField(ActivePlayer))
            {
                legend.AttackedThisTurn = false;
            }

            // Switch the Turn
            _playerManager.SwitchActivePlayer();

            // Reset Variables
            ActivePlayer.PlayedEssenceCardThisTurn(false);
            // Hide Enemy Hand and Disable it
            UiManager.EnableCardsInZone(InactivePlayer, ZoneTypes.Hand, false);
            // Show Player Hand and Enable it
            UiManager.EnableCardsInZone(ActivePlayer, ZoneTypes.Hand, true);
            // Enable all the cards on the field
            UiManager.EnableCardsInZone(ActivePlayer, ZoneTypes.PlayingField, true);
            UiManager.EnableCardsInZone(InactivePlayer, ZoneTypes.PlayingField, true);
        }
        void BurnDamage()
        {
            List<LegendCard> legends = GetAllMyLegendsOnThePlayingField(ActivePlayer);
            foreach (LegendCard legend in legends)
            {
                legend.TakeBurnDamage();
            }
        }
        public void DrawCardFromDeck(Player player)
        {
            // Logic
            Card? card = ActivePlayer.DrawCardFromDeck();
            if (card != null)
            {
                UiManager.MoveCardFromDeckZoneToHandZone(player, card);
            }
        }
        public List<LegendCard> GetAllLegendsOnPlayingField()
        {
            List<Player> players = GetPlayers();
            List<LegendCard> legends = new List<LegendCard>();
            foreach (Player player in players)
            {
                foreach (LegendCard legend in player.PlayingField)
                {
                    legends.Add(legend);
                }
            }
            return legends;
        }
        public void AddCardToDiscardPile(Player player, Card card)
        {
            player.DiscardPile.Add(card);
            card.Discard();
            // add card to discard pile visually
        }
        public void AddCardToHandZone(Player player, Card card)
        {
            // logically
            player.AddCardToHand(card);
            // visually
            UiManager.AddCardToHandZone(player, card);
        }
        public List<LegendCard> GetAllMyLegendsOnThePlayingField(Player player)
        {
            List<LegendCard> legends = new List<LegendCard>();
            foreach (LegendCard legend in player.PlayingField)
            {
                legends.Add(legend);
            }
            return legends;
        }

        // UI-Manager
        public void EnableCardsInZone(Player player, ZoneTypes zoneType, bool enabled)
        {
            UiManager.EnableCardsInZone(player, zoneType, enabled);
        }
        public void PlayStageCard(StageCard card)
        {
            UiManager.PlayStageCard(ActivePlayer, card);
        }
        public void ShowCards()
        {
            UiManager.ShowCards(Me, true);
            //UiManager.ShowCards(Opponent, true); // show opp cards nice for debugging
        }

        // NetworkManager
        public void SendMessageToPeer(string msg)
        {
            _networkManager.SendMessageToPeer(msg);
        }

        // AttackManager
        public Attack GetSelectedAttack()
        {
            return _attackManager.SelectedAttack;
        }
        public bool GetSomeoneIsAttacking()
        {
            return _attackManager.SomeoneIsAttacking;
        }
        public void StartAttack(Attack attack)
        {
            _attackManager.StartAttack(attack, _playerManager);
        }
        public void StopAttack()
        {
            _attackManager.StopAttack(_playerManager);
        }

        // PlayerManager
        public Player GetOtherPlayer(Player p)
        {
            return _playerManager.GetOtherPlayer(p);
        }
        public List<Player> GetPlayers()
        {
            return _playerManager.GetPlayers();
        }
        void RandomizeStartingPlayer()
        {
            _playerManager.RandomizeStartingPlayer();
        }


        // StageCardManager
        public void SetStageCardFromForm(Player owner, StageCard stage)
        {
            _stageCardManager.SetStageCard(owner, stage);
        }
        public StageCard? GetActiveStageCard()
        {
            return _stageCardManager.ActiveStageCard;
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.