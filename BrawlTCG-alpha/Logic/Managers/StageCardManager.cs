using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BrawlTCG_alpha.Logic
{
    public class StageCardManager
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
                ActiveStageCard.StartTurnEffect.Invoke(legends, ActiveStageCard, _game, ActiveStageCard); // i want to reference the game instance i am in if possbile
            }
        }

        public void WhenPlayedEffect()
        {
            if (ActiveStageCard.WhenPlayedEffect != null)
            {
                // get all cards
                List<Card> l1 = _game.Me.GetAllCardsInPlayingField();
                List<Card> l2 = _game.Opponent.GetAllCardsInPlayingField();
                List<Card> allCards = l1.Concat(l2).ToList();
                // effect
                ActiveStageCard.WhenPlayedEffect.Invoke(allCards, ActiveStageCard, _game, ActiveStageCard);
            }
        }

        public void WhenDiscardedEffect()
        {
            if (ActiveStageCard.WhenDiscardedEffect != null)
            {
                // get all cards
                List<Card> l1 = _game.Me.GetAllCardsInPlayingField();
                List<Card> l2 = _game.Opponent.GetAllCardsInPlayingField();
                List<Card> allCards = l1.Concat(l2).ToList();
                // effect
                ActiveStageCard.WhenDiscardedEffect.Invoke(allCards, ActiveStageCard, _game, ActiveStageCard);
            }
        }

        public StageCard WhileInPlayEffect(LegendCard legend)
        {
            if (ActiveStageCard != null)
            {
                if (ActiveStageCard.WhileInPlayEffect != null)
                {
                    ActiveStageCard.WhileInPlayEffect.Invoke(legend, ActiveStageCard, _game, ActiveStageCard);
                }
            }
            return ActiveStageCard;
        }
    }
}
