#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using BrawlTCG_alpha.Logic.Cards;
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
        // Fields
        const int STARTING_ESSENCE = 1;
        const int STARTING_HAND_CARDS = 7; // 7

        // Properties
        public Player BottomPlayer { get; private set; }
        public Player TopPlayer { get; private set; }
        public Player ActivePlayer { get; private set; }
        public Player InactivePlayer { get; private set; }
        public bool SomeoneIsAttacking { get; private set; }
        public Attack SelectedAttack { get; private set; }
        public Player Me { get; private set; }
        public Player Opponent { get; private set; }

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
        public event Action<Player, ZoneTypes, bool> UI_EnableCardsInZone;
        public event Action<Player> UI_UpdatePlayerInformation;
        public event Action<Player> UI_UntapPlayerCards;
        public event Action<string> UI_PopUpNotification;
        // Fields
        public StageCard ActiveStageCard;
        public Player ActiveStageCardOwner;
        bool _bottomPlayerTurn = false;

        public Game(Player player1, Player player2)
        {
            // Determine opponent object
            if (player1.IsMe)
            {
                Me = player1;
                BottomPlayer = player1;
                Opponent = player2;
                TopPlayer = player2;
            }
            else
            {
                Me = player2;
                BottomPlayer = player2;
                Opponent = player1;
                TopPlayer = player1;
            }

            // Host always starts
            if (player1.IsHost)
            {
                ActivePlayer = player1;
                InactivePlayer = player2;
                _bottomPlayerTurn = true;
            }
            else
            {
                ActivePlayer = player2;
                InactivePlayer = player1;
                _bottomPlayerTurn = false;
            }

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
                ActivePlayer.EssenceField.Add(CardCatalogue.GetCardById(cardId: 0)); // 0 is essence card
                InactivePlayer.EssenceField.Add(CardCatalogue.GetCardById(cardId: 0));
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
            // Before you start
            UI_EnableCardsInZone.Invoke(InactivePlayer, ZoneTypes.Hand, false); // disable enemy cards
            DrawCardFromDeck(ActivePlayer); // draw card
            ShowCards(); // SHOWS THE CARDS BY FLIPPING THEM
            ActivePlayer.GetEssence();
            UI_UpdatePlayerInformation.Invoke(ActivePlayer);
            return Task.CompletedTask;
        }
        public void ShowCards()
        {
            UI_ShowCards(Me, true);
        }

        public void SwitchTurn()
        {
            // END TURN
            // ...
            // untap all cards of the active player
            UI_UntapPlayerCards.Invoke(ActivePlayer);

            // Switch the Turn
            _bottomPlayerTurn = !_bottomPlayerTurn;
            if (ActivePlayer == BottomPlayer)
            {
                ActivePlayer = TopPlayer;
                InactivePlayer = BottomPlayer;
            }
            else
            {
                ActivePlayer = BottomPlayer;
                InactivePlayer = TopPlayer;
            }
            // Reset Variables
            ActivePlayer.PlayedEssenceCardThisTurn(false);
            // Hide Enemy Hand and Disable it
            UI_EnableCardsInZone(InactivePlayer, ZoneTypes.Hand, false);
            // Show Player Hand and Enable it
            UI_EnableCardsInZone(ActivePlayer, ZoneTypes.Hand, true);
            // Enable all the cards on the field
            UI_EnableCardsInZone(ActivePlayer, ZoneTypes.PlayingField, true);
            UI_EnableCardsInZone(InactivePlayer, ZoneTypes.PlayingField, true);

            // START TURN
            // stage start turn effect
            List<LegendCard> legends = GetAllMyLegendsOnThePlayingField(ActivePlayer);
            if (ActiveStageCard != null)
            {
                if (ActiveStageCard.StartTurnEffect != null)
                {
                    ActiveStageCard.StartTurnEffect.Invoke(legends);
                }
            }
            // burn damage
            foreach (LegendCard legend in legends)
            {
                legend.TakeBurnDamage();
            }

            // Update legends information in playing field
            UI_UpdateCardControlInPlayingFieldInformation.Invoke();
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
        void RandomizeStartingPlayer()
        {
            // Randomize which player starts (0 for top player, 1 for bottom player)
            Random random = new Random();
            int startingPlayer = random.Next(2); // Generates either 0 or 1

            if (startingPlayer == 0)
            {
                // Top player starts
                InactivePlayer = BottomPlayer;
                ActivePlayer = TopPlayer;
                _bottomPlayerTurn = false;
                UI_PopUpNotification("Top Player Starts");
            }
            else
            {
                // Bottom player starts
                InactivePlayer = TopPlayer;
                ActivePlayer = BottomPlayer;
                _bottomPlayerTurn = true;
                UI_PopUpNotification("Bottom Player Starts");
            }
        }
        public List<Player> GetPlayers()
        {
            return [ActivePlayer, InactivePlayer];
        }
        public Player GetOtherPlayer(Player p)
        {
            if (BottomPlayer == p)
            {
                return TopPlayer;
            }
            else if (TopPlayer == p)
            {
                return BottomPlayer;
            }
            throw new Exception();
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
        public List<LegendCard> GetAllMyLegendsOnThePlayingField(Player player)
        {
            List<LegendCard> legends = new List<LegendCard>();
            foreach (LegendCard legend in player.PlayingField)
            {
                legends.Add(legend);
            }
            return legends;
        }
        public void SetStageCard(Player owner, StageCard stage)
        {
            ActiveStageCard = stage;
            ActiveStageCardOwner = owner;
        }
        public void StartAttack(Attack attack)
        {
            // ideally disable cards here
            SomeoneIsAttacking = true;
            SelectedAttack = attack;
        }
        public void StopAttack()
        {
            SomeoneIsAttacking = false;
            SelectedAttack = null;
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
        public void PlayStageCard(StageCard card)
        {
            UI_PlayStageCard(ActivePlayer, card);
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.