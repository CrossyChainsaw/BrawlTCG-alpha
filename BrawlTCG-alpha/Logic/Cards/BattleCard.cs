using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BrawlTCG_alpha.Logic.Cards
{
    internal class BattleCard : Card
    {
        public bool OneTimeUse { get; set; }
        public int Damage { get; set; }
        public BattleCard(string name, int cost, string description, Elements element, Image image, bool oneTimeUse, Action<object>? startTurnEffect = null, Action<object>? endTurnEffect = null, Action<object, Card>? whenPlayedEffect = null, int damage = 0) : base(name, cost, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect)
        {
            // req
            Name = name;
            Cost = cost;
            Description = description;
            Element = element;
            Image = image;
            // opt
            StartTurnEffect = startTurnEffect;
            EndTurnEffect = endTurnEffect;
            WhenPlayedEffect = whenPlayedEffect;
            // other opt
            OneTimeUse = oneTimeUse;
            Damage = damage;
        }

        public override Card Clone()
        {
            return new BattleCard(Name, Cost, Description, Element, Image, OneTimeUse, StartTurnEffect, EndTurnEffect, WhenPlayedEffect, Damage);
        }
    }
}
