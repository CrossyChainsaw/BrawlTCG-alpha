#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
        public Player BottomPlayer { get; private set; }
        public Player TopPlayer { get; private set; }
        public Player ActivePlayer { get; private set; }
        public Player InactivePlayer { get; private set; }
        public event Action UI_Multi_InitializeDeckPiles;
        public event Action<Player> UI_InitializeCardsInHand;
        public event Action<Player> UI_UpdateEssenceCardsInEssenceField;
        public event Action<Player> UI_UpdateCardsInDeckPile;
        public event Action<Player, bool> UI_ShowCards;
        public event Action<Player, ZoneTypes, bool> UI_EnableCards;
        public event Action UI_Multi_DisableCardsOnEssenceZones;
        public event Action<Player> UI_UpdatePlayerInformation;
        public event Action<Player, Card, ZoneTypes, ZoneTypes> UI_ChangeCardZone;
        public event Action<string> UI_PopUpNotification;
        bool _bottomPlayerTurn = false;

        public Game(Player playerOne, Player playerTwo)
        {
            BottomPlayer = playerOne;
            TopPlayer = playerTwo;
        }
        public void Prepare()
        {
            // Define Players
            RandomizeStartingPlayer();

            // Initialize Decks Visually
            UI_Multi_InitializeDeckPiles.Invoke();
            UI_EnableCards(ActivePlayer, ZoneTypes.Deck, false);
            UI_EnableCards(InactivePlayer, ZoneTypes.Deck, false);

            // Draw Starting Hands and Display Visually
            ActivePlayer.DrawStartingHandFromDeck();
            InactivePlayer.DrawStartingHandFromDeck();
            UI_InitializeCardsInHand.Invoke(ActivePlayer);
            UI_InitializeCardsInHand.Invoke(InactivePlayer);

            // Obtain first Essence card and display visually - and disable them
            ActivePlayer.EssenceField.Add(CardCatalogue.Essence.Clone());
            InactivePlayer.EssenceField.Add(CardCatalogue.Essence.Clone());
            UI_UpdateEssenceCardsInEssenceField.Invoke(ActivePlayer);
            UI_UpdateEssenceCardsInEssenceField.Invoke(InactivePlayer);
            UI_Multi_DisableCardsOnEssenceZones.Invoke();
        }
        public void Start()
        {
            //SwitchTurn();
            StartTurn();
        }
        public Task StartTurn()
        {
            // Before you start
            UI_EnableCards.Invoke(InactivePlayer, ZoneTypes.Hand, false); // disable enemy cards
            DrawCardFromDeck(ActivePlayer); // draw card
            UI_ShowCards(ActivePlayer, true); // enable your cards
            ActivePlayer.GetEssence();
            UI_UpdatePlayerInformation.Invoke(ActivePlayer);
            return Task.CompletedTask;
        }

        public void SwitchTurn()
        {
            // Switch the Turn
            _bottomPlayerTurn = !_bottomPlayerTurn;
            ActivePlayer.PlayedEssenceCardThisTurn(false);
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
            UI_ShowCards(InactivePlayer, false);
            UI_EnableCards(InactivePlayer, ZoneTypes.Hand, false);
            UI_ShowCards(ActivePlayer, true);
            UI_EnableCards(ActivePlayer, ZoneTypes.Hand, true);
        }
        void DrawCardFromDeck(Player player)
        {
            // Logic
            Card? card = ActivePlayer.DrawCardFromDeck();
            if (card != null)
            {
                UI_ChangeCardZone.Invoke(player, card, ZoneTypes.Deck, ZoneTypes.Hand);
                UI_UpdateCardsInDeckPile.Invoke(player);
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

    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.