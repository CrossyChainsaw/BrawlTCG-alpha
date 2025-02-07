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
        public event Action<Player, bool> UI_EnableCards;
        public event Action<Player> UI_EnableEnemyCards;
        public event Action UI_Multi_DisableCardsOnEssenceZones;
        public event Action<Player> UI_UpdatePlayerInformation;
        public event Action<Player, ZoneTypes, List<Card>, int> UI_RearrangeCards;
        public event Action<Player, Card, ZoneTypes, ZoneTypes> UI_ChangeCardZone;
        bool _bottomPlayerTurn = false;

        public Game(Player playerOne, Player playerTwo)
        {
            BottomPlayer = playerOne;
            TopPlayer = playerTwo;

            ActivePlayer = BottomPlayer;
            InactivePlayer = TopPlayer;
        }
        public void Prepare()
        {
            // Define Players
            InactivePlayer = BottomPlayer;
            ActivePlayer = TopPlayer;
            _bottomPlayerTurn = false;

            // Initialize Decks Visually
            UI_Multi_InitializeDeckPiles.Invoke();

            // Draw Starting Hands and Display Visually
            TopPlayer.DrawStartingHandFromDeck();
            BottomPlayer.DrawStartingHandFromDeck();
            UI_InitializeCardsInHand.Invoke(BottomPlayer);
            UI_InitializeCardsInHand.Invoke(TopPlayer);

            // Obtain first Essence card and display visually - and disable them
            TopPlayer.EssenceField.Add(CardCatalogue.Essence.Clone());
            BottomPlayer.EssenceField.Add(CardCatalogue.Essence.Clone());
            UI_UpdateEssenceCardsInEssenceField.Invoke(BottomPlayer);
            UI_UpdateEssenceCardsInEssenceField.Invoke(TopPlayer);
            UI_Multi_DisableCardsOnEssenceZones.Invoke();
        }
        public async Task Start()
        {
            SwitchTurn();
            StartTurn();
        }
        public void StartTurn()
        {
            // Before you start
            UI_EnableCards.Invoke(InactivePlayer, false); // disable enemy cards
            DrawCardFromDeck(ActivePlayer); // draw card
            UI_ShowCards(ActivePlayer, true); // enable your cards
            ActivePlayer.GetEssence();
            UI_UpdatePlayerInformation.Invoke(ActivePlayer);

            // now you can interact with cards
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
            UI_EnableCards(InactivePlayer, false);
            UI_ShowCards(ActivePlayer, true);
            UI_EnableCards(ActivePlayer, true);
        }

        // Visual Method Collection
        void DrawCardFromDeck(Player player)
        {
            // Logic
            Card? card = ActivePlayer.DrawCardFromDeck();
            if (card != null)
            {
                UI_UpdateCardsInDeckPile.Invoke(player);
                UI_ChangeCardZone.Invoke(player, card, ZoneTypes.Deck, ZoneTypes.Hand);
            }
        }
    }
}
