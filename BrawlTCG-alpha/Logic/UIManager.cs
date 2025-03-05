using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    public class UIManager
    {
        // Fields
        private FRM_Game _mainForm;
        private List<ZoneControl> _zones;
        public event Action<string> UI_PopUpNotification;
        public event Action UI_InitializeZones;
        public event Action<Player> UI_InitializeDeckPile;



        public event Action<Player> UI_InitializeCardsInHand;
        // To do
        public event Action UI_UpdateCardControlInPlayingFieldInformation;
        public event Action<Player, Card> UI_MoveCardZoneFromDeckToHand;
        public event Action<Player, Card> UI_AddCardToHandZone;
        public event Action<Player, StageCard> UI_PlayStageCard;
        public event Action<Player> UI_UpdateEssenceCardsInEssenceField;
        public event Action<Player> UI_UpdateCardsInDeckPile;
        public event Action<Player, bool> UI_ShowCards;
        public event Action<Player> UI_UpdatePlayerInformation;
        public event Action<Player> UI_UntapPlayerCards;
        // Set in Ctor
        public event Action<Player, ZoneTypes, bool> UI_EnableCardsInZone;






        // Methods
        public UIManager(FRM_Game mainForm)
        {
            _mainForm = mainForm;
            _zones = new List<ZoneControl>();
        }

        public void InitializeZones()
        {
            UI_InitializeZones.Invoke();
        }

        public void InitializeCardsInHand(Player p)
        {
            UI_InitializeCardsInHand.Invoke(p);
        }

        public void InitializeDeckPile(Player p)
        {
            UI_InitializeDeckPile.Invoke(p);
        }

        public void MessageBox(string s)
        {
            UI_PopUpNotification.Invoke(s);
        }
    }
}
