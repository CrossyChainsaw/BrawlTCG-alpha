﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    public class Game
    {
        // Properties
        public Player ActivePlayer => _playerManager.ActivePlayer;
        public Player InactivePlayer => _playerManager.InactivePlayer;
        public Player Me => _playerManager.Me;
        public Player Opponent => _playerManager.Opponent;
        public int RandomCardID { get; set; } = -1;

        // Fields
        const int STARTING_ESSENCE = 1; // 1
        const int STARTING_HAND_CARDS = 7; // 7


        // VISUALS
        public event Action UI_InitializeZones;
        public event Action UI_UpdateCardControlInPlayingFieldInformation;
        public event Action<Player, Card> UI_MoveCardZoneFromDeckToHand;
        public event Action<Player, Card> UI_AddCardToHandZone;
        public event Action<Player, StageCard> UI_PlayStageCard;
        public event Action<Player> UI_UpdateEssenceCardsInEssenceField;
        public event Action<Player> UI_UpdateCardsInDeckPile;
        public event Action<Player> UI_UpdatePlayerInformation;
        public event Action<Player> UI_UntapPlayerCards;

        // Fields
        StageCardManager _stageCardManager;
        PlayerManager _playerManager;
        AttackManager _attackManager;
        NetworkManager _networkManager;
        UIManager _uiManager;



        public Game(Player player1, Player player2, NetworkManager networkManager, UIManager uiManager)
        {
            // Managers
            _uiManager = uiManager;
            _stageCardManager = new StageCardManager(this);
            _playerManager = new PlayerManager(player1, player2, _uiManager);
            _attackManager = new AttackManager(_uiManager);
            _networkManager = networkManager;
        }
        public void Prepare()
        {
            // Setup all the zones visually
            _uiManager.InitializeZones();

            // Define Players
            //RandomizeStartingPlayer(); // host always starts

            // Initialize Decks Visually
            _uiManager.InitializeDeckPile(ActivePlayer);
            _uiManager.InitializeDeckPile(InactivePlayer);

            _uiManager.EnableCardsInZone(ActivePlayer, ZoneTypes.Deck, false);
            _uiManager.EnableCardsInZone(InactivePlayer, ZoneTypes.Deck, false);

            // Draw Starting Hands and Display Visually
            DrawStartingHand(ActivePlayer, STARTING_HAND_CARDS);
            DrawStartingHand(InactivePlayer, STARTING_HAND_CARDS);
            _uiManager.InitializeCardsInHand(ActivePlayer);
            _uiManager.InitializeCardsInHand(InactivePlayer);

            // Obtain first Essence card and display visually - and disable them
            for (int i = 0; i < STARTING_ESSENCE; i++)
            {
                ActivePlayer.EssenceField.Add(CardCatalogue.GetCardById(cardId: 0)); // 0 is essence card
                InactivePlayer.EssenceField.Add(CardCatalogue.GetCardById(cardId: 0));
            }
            // update cards in essence fields
            UI_UpdateEssenceCardsInEssenceField.Invoke(ActivePlayer);
            UI_UpdateEssenceCardsInEssenceField.Invoke(InactivePlayer);
            // Disable cards in essence zones
            _uiManager.EnableCardsInZone(ActivePlayer, ZoneTypes.EssenceField, false);
            _uiManager.EnableCardsInZone(InactivePlayer, ZoneTypes.EssenceField, false);

            // only show your own cards
            ShowCards();
        }
        void DrawStartingHand(Player player, int nCards)
        {
            for (int i = 0; i < nCards; i++)
            {
                Card card = player.DrawCardFromDeck();
                UI_MoveCardZoneFromDeckToHand(player, card);
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
            DrawCardFromDeck(ActivePlayer); // draw card
            _stageCardManager.StartTurnEffect(ActivePlayer);
            BurnDamage();
            UI_UpdateCardControlInPlayingFieldInformation.Invoke();
            ActivePlayer.GetEssence();

            ShowCards(); // SHOWS THE CARDS BY FLIPPING THEM
            UI_UpdatePlayerInformation.Invoke(ActivePlayer);
            return Task.CompletedTask;
        }
        public void SwitchTurn()
        {
            // END TURN
            // ...
            // untap all cards of the active player
            UI_UntapPlayerCards.Invoke(ActivePlayer);
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
            _uiManager.EnableCardsInZone(InactivePlayer, ZoneTypes.Hand, false);
            // Show Player Hand and Enable it
            _uiManager.EnableCardsInZone(ActivePlayer, ZoneTypes.Hand, true);
            // Enable all the cards on the field
            _uiManager.EnableCardsInZone(ActivePlayer, ZoneTypes.PlayingField, true);
            _uiManager.EnableCardsInZone(InactivePlayer, ZoneTypes.PlayingField, true);
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
                UI_MoveCardZoneFromDeckToHand.Invoke(player, card);
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

            // add card to discard pile visually
        }
        public void AddCardToHandZone(Player player, Card card)
        {
            // logically
            player.AddCardToHand(card);
            // visually
            UI_AddCardToHandZone.Invoke(player, card);
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
            _uiManager.EnableCardsInZone(player, zoneType, enabled);
        }
        public void ShowCards()
        {
            _uiManager.ShowCards(Me, true);
            //_uiManager.ShowCards(Opponent, true); // show opponent cards for debugging
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
        public void PlayStageCard(StageCard card)
        {
            UI_PlayStageCard(ActivePlayer, card);
        }
        public StageCard? GetActiveStageCard()
        {
            return _stageCardManager.ActiveStageCard;
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.