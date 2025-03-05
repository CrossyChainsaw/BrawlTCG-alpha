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
        public UIManager(FRM_Game mainForm)
        {
            _mainForm = mainForm;
            _zones = new List<ZoneControl>();
        }


        // Initialize
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


        //Update
        public void UpdateCardControlsInPlayingFieldInformation()
        {
            UI_UpdateCardControlsInPlayingFieldInformation.Invoke();
        }

        public void UpdateEssenceCardsInEssenceField(Player p)
        {
            UI_UpdateEssenceCardsInEssenceField(p);
        }

        public void UpdatePlayerInformation(Player p)
        {
            UI_UpdatePlayerInformation(p);
        }


        // Cards
        public void AddCardToHandZone(Player p, Card card)
        {
            UI_AddCardToHandZone.Invoke(p, card);
        }

        public void EnableCardsInZone(Player p, ZoneTypes zoneType, bool enable)
        {
            UI_EnableCardsInZone.Invoke(p, zoneType, enable);
        }

        public void MoveCardFromDeckZoneToHandZone(Player p, Card card)
        {
            UI_MoveCardFromDeckZoneToHandZone.Invoke(p, card);
        }

        public void PlayStageCard(Player p, StageCard stage)
        {
            UI_PlayStageCard.Invoke(p, stage);
        }

        public void ShowCards(Player p, bool enable)
        {
            UI_ShowCards.Invoke(p, enable);
        }

        public void UntapPlayerCards(Player p)
        {
            UI_UntapPlayerCards.Invoke(p);
        }


        // Other
        public void MessageBox(string s)
        {
            UI_PopUpNotification.Invoke(s);
        }
    }
}
