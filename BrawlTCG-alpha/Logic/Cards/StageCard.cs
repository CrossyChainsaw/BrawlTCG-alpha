﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    // ideas: start turn both players get a x sword
    public class StageCard : Card
    {
        public Action<object, Card, Game>? WhileInPlayEffect { get; internal set; }

        public StageCard(int id, string name, int cost, string description, Elements element, Image image, Effect? startTurnEffect = null, Action<object>? endTurnEffect = null,Effect? whenPlayedEffect = null, Action<object, Card, Game>? whenDiscardedEffect = null, Action<object, Card, Game>? whileInPlayEffect = null) : base(id, name, cost, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect, whenDiscardedEffect)
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
