using BrawlTCG_alpha.Logic.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    internal class StageCardManager
    {
        // Properties
        public StageCard ActiveStageCard { get; private set; }
        public Player ActiveStageCardOwner { get; private set; }
        // Fields
        private Game _game;

        // Methods
        public StageCardManager(Game game)
        {
            _game = game;
        }

        public void SetStageCard(Player owner, StageCard stage)
        {
            ActiveStageCard = stage;
            ActiveStageCardOwner = owner;
        }

        public void StartTurnEffect(Player player)
        {
            if (ActiveStageCard != null && ActiveStageCard.StartTurnEffect != null)
            {
                var legends = player.PlayingField.OfType<LegendCard>().ToList();
                ActiveStageCard.StartTurnEffect.Invoke(legends, ActiveStageCard, _game); // i want to reference the game instance i am in if possbile
            }
        }
    }
}
