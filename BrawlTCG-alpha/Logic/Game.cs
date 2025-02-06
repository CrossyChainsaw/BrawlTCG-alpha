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
        public event Action<Player> InitializeCardsInHand;
        public event Action<Player> UI_UpdateEssenceCardsInEssenceField;
        public event Action<Player> UI_UpdateCardsInDeckPile;
        public event Action<Player> UI_FlipPlayerCards;
        public event Action<Player> UI_EnableEnemyCards;
        public event Action UI_Multi_DisableCardsOnEssenceZones;
        public event Action<Player> UI_UpdatePlayerInformation;
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
            // Initialize Decks Visually
            UI_Multi_InitializeDeckPiles.Invoke();

            // Draw Starting Hands and Display Visually
            TopPlayer.DrawStartingHandFromDeck();
            BottomPlayer.DrawStartingHandFromDeck();
            InitializeCardsInHand.Invoke(BottomPlayer);
            InitializeCardsInHand.Invoke(TopPlayer);

            // Obtain first Essence card and display visually - and disable them
            TopPlayer.EssenceField.Add(CardCatalogue.Essence.Clone());
            BottomPlayer.EssenceField.Add(CardCatalogue.Essence.Clone());
            UI_UpdateEssenceCardsInEssenceField.Invoke(BottomPlayer);
            UI_UpdateEssenceCardsInEssenceField.Invoke(TopPlayer);
            UI_Multi_DisableCardsOnEssenceZones.Invoke();
        }
        public async Task Start()
        {
            // GAME STARTS
            InactivePlayer = BottomPlayer;
            ActivePlayer = TopPlayer;
            _bottomPlayerTurn = false;
            SwitchTurn();
            StartTurn();
        }
        public void StartTurn()
        {
            // Before you start

            UI_FlipPlayerCards(InactivePlayer);
            ActivePlayer.DrawCardFromDeck();
            UI_DrawCardFromDeck(ActivePlayer);
            ActivePlayer.GetEssence();
            UI_UpdatePlayerInformation(ActivePlayer);

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
        }

        // Logic
        void PlayCard(Card card)
        {

        }

        // Visual Method Collection
        void UI_DrawCardFromDeck(Player player)
        {
            UI_UpdateCardsInDeckPile(player);
            InitializeCardsInHand(player);
        }
    }
}
