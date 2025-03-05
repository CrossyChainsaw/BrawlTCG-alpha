using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    internal class AttackManager
    {
        // Properties
        public bool SomeoneIsAttacking { get; private set; }
        public Attack SelectedAttack { get; private set; }

        // Fields
        UIManager _uiManager;

        // Methods
        public AttackManager(UIManager uiManager)
        {
            _uiManager = uiManager;
        }
        public void SetSelectedAttack(Attack a)
        {
            SelectedAttack = a;
        }
        public void SomeoneStartedAttacking()
        {
            SomeoneIsAttacking = true;
        }
        public void StopAttack(PlayerManager playerManager)
        {
            SomeoneIsAttacking = false;
            SelectedAttack = null;

            // enable your cards again
            _uiManager.EnableCardsInZone(playerManager.ActivePlayer, ZoneTypes.PlayingField, true);
        }
        public void StartAttack(Attack attack, PlayerManager playerManager)
        {
            // ideally disable cards here
            SomeoneIsAttacking = true;
            SelectedAttack = attack;

            // enable disable correct cards
            if (attack.FriendlyFire)
            {
                _uiManager.EnableCardsInZone(playerManager.ActivePlayer, ZoneTypes.PlayingField, true);
                _uiManager.EnableCardsInZone(playerManager.InactivePlayer, ZoneTypes.PlayingField, false);
            }
            else
            {
                _uiManager.EnableCardsInZone(playerManager.ActivePlayer, ZoneTypes.PlayingField, false);
                _uiManager.EnableCardsInZone(playerManager.InactivePlayer, ZoneTypes.PlayingField, true);
            }
        }
    }
}
