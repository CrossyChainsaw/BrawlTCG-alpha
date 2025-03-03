using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    // ideas: start turn both players get a x sword
    internal class StageCard : Card
    {
        public Action<object, Card, Game>? WhileInPlayEffect { get; internal set; }

        public StageCard(int id, string name, int cost, string description, Elements element, Image image, Action<object, Card, Game>? startTurnEffect = null, Action<object>? endTurnEffect = null, Action<object, Card, Game>? whenPlayedEffect = null, Action<object, Card, Game>? whenDiscardedEffect = null, Action<object, Card, Game>? whileInPlayEffect = null) : base(id, name, cost, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect, whenDiscardedEffect)
        {
            Description = description;
            WhileInPlayEffect = whileInPlayEffect;
        }

        public override Card Clone()
        {
            return new StageCard(ID, Name, Cost, Description, Element, Image, StartTurnEffect, EndTurnEffect, WhenPlayedEffect, WhenDiscardedEffect, WhileInPlayEffect);
        }
    }
}
