using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    public class Effect
    {
        // Properties
        public string Description { get; }
        public Action<object, Card, Game, Card>? EffectAction { get; }
        public bool MultipleTriggersInTurn { get; }

        // Methods
        public Effect(string description, Action<object, Card, Game, Card>? effectAction, bool multipleTriggersInTurn = false)
        {
            Description = description;
            EffectAction = effectAction;
            MultipleTriggersInTurn = multipleTriggersInTurn;
        }

        public void Invoke(object target, Card card, Game game, Card playedCard)
        {
            EffectAction?.Invoke(target, card, game, playedCard);
        }
    }
}
