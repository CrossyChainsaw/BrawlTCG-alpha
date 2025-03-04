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

namespace BrawlTCG_alpha.Logic
{
    internal class Game
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
        public event Action UI_Multi_InitializeDeckPiles;
        public event Action UI_UpdateCardControlInPlayingFieldInformation;
        public event Action<Player, Card> UI_MoveCardZoneFromDeckToHand;
        public event Action<Player, Card> UI_AddCardToHandZone;
        public event Action<Player, StageCard> UI_PlayStageCard;
        public event Action<Player> UI_InitializeCardsInHand;
        public event Action<Player> UI_UpdateEssenceCardsInEssenceField;
        public event Action<Player> UI_UpdateCardsInDeckPile;
        public event Action<Player, bool> UI_ShowCards;
        public event Action<Player> UI_UpdatePlayerInformation;
        public event Action<Player> UI_UntapPlayerCards;
        // Set in Ctor
        public event Action<Player, ZoneTypes, bool> UI_EnableCardsInZone;
        public event Action<string> UI_PopUpNotification;
        // Networking
        public event Action<string> NETWORK_SendMessage;
        // Fields
        StageCardManager _stageCardManager;
        PlayerManager _playerManager;
        AttackManager _attackManager;



        public Game(Player player1, Player player2, Action<string> ui_PopUpNotification, Action<Player, ZoneTypes, bool> ui_EnableCardsInZone)
        {
            // UI Stuff
            UI_PopUpNotification += ui_PopUpNotification;
            UI_EnableCardsInZone += ui_EnableCardsInZone;

            // Managers
            _stageCardManager = new StageCardManager(this);
            _playerManager = new PlayerManager(player1, player2, UI_PopUpNotification);
            _attackManager = new AttackManager(UI_EnableCardsInZone);
        }
        public void Prepare()
        {
            // Setup all the zones visually
            UI_InitializeZones.Invoke();

            // Define Players
            //RandomizeStartingPlayer(); // host always starts

            // Initialize Decks Visually
            UI_Multi_InitializeDeckPiles.Invoke();
            UI_EnableCardsInZone(ActivePlayer, ZoneTypes.Deck, false);
            UI_EnableCardsInZone(InactivePlayer, ZoneTypes.Deck, false);

            // Draw Starting Hands and Display Visually
            DrawStartingHand(ActivePlayer, STARTING_HAND_CARDS);
            DrawStartingHand(InactivePlayer, STARTING_HAND_CARDS);

            UI_InitializeCardsInHand.Invoke(ActivePlayer);
            UI_InitializeCardsInHand.Invoke(InactivePlayer);

            // Obtain first Essence card and display visually - and disable them
            for (int i = 0; i < STARTING_ESSENCE; i++)
            {
                _playerManager.ActivePlayer.EssenceField.Add(CardCatalogue.GetCardById(cardId: 0)); // 0 is essence card
                _playerManager.InactivePlayer.EssenceField.Add(CardCatalogue.GetCardById(cardId: 0));
            }
            // update cards in essence fields
            UI_UpdateEssenceCardsInEssenceField.Invoke(ActivePlayer);
            UI_UpdateEssenceCardsInEssenceField.Invoke(InactivePlayer);
            // Disable cards in essence zones
            UI_EnableCardsInZone.Invoke(ActivePlayer, ZoneTypes.EssenceField, false);
            UI_EnableCardsInZone.Invoke(InactivePlayer, ZoneTypes.EssenceField, false);

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
            _playerManager.ActivePlayer.GetEssence();

            ShowCards(); // SHOWS THE CARDS BY FLIPPING THEM
            UI_UpdatePlayerInformation.Invoke(ActivePlayer);
            return Task.CompletedTask;
        }
        public void ShowCards()
        {
            UI_ShowCards(_playerManager.Me, true);
            //UI_ShowCards(Opponent, true); // show opponent cards for debugging
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
            _playerManager.ActivePlayer.PlayedEssenceCardThisTurn(false);
            // Hide Enemy Hand and Disable it
            UI_EnableCardsInZone(InactivePlayer, ZoneTypes.Hand, false);
            // Show Player Hand and Enable it
            UI_EnableCardsInZone(ActivePlayer, ZoneTypes.Hand, true);
            // Enable all the cards on the field
            UI_EnableCardsInZone(ActivePlayer, ZoneTypes.PlayingField, true);
            UI_EnableCardsInZone(InactivePlayer, ZoneTypes.PlayingField, true);
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
            Card? card = _playerManager.ActivePlayer.DrawCardFromDeck();
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
        public void SendMessageToPeer(string msg)
        {
            NETWORK_SendMessage.Invoke(msg);
        }
        public void EnableCardsInZone(Player p, ZoneTypes z, bool enabled)
        {
            UI_EnableCardsInZone.Invoke(p, z, enabled);
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