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
        public StageCard(string name, int cost, string description, Elements element, Image image, Action<object>? startTurnEffect = null, Action<object>? endTurnEffect = null, Action<object>? whenPlayedEffect = null) : base(name, cost, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect)
        {
            Name = name;
            Cost = cost;
            Description = description;
            Element = element;
            Image = image;
            StartTurnEffect = startTurnEffect;
            EndTurnEffect = endTurnEffect;
            WhenPlayedEffect = whenPlayedEffect;
        }

        public override Card Clone()
        {
            return new StageCard(Name, Cost, Description, Element, Image, StartTurnEffect, EndTurnEffect, WhenPlayedEffect);
        }
    }
}
