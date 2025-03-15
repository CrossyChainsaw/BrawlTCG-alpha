using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    // ideas: start turn both players get a x sword
    public class StageCard : Card
    {
        public StageCard(int id, string name, int cost, Elements element, Image image, Effect? startTurnEffect = null, Action<object>? endTurnEffect = null,Effect? whenPlayedEffect = null, Effect? whenDiscardedEffect = null, Effect? whileInPlayEffect = null) : base(id, name, cost, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect, whenDiscardedEffect, whileInPlayEffect)
        {
            Description = GenerateEffectDescription(this);
        }

        public override Card Clone()
        {
            return new StageCard(ID, Name, Cost, Element, Image, StartTurnEffect, EndTurnEffect, WhenPlayedEffect, WhenDiscardedEffect, WhileInPlayEffect);
        }
    }
}
