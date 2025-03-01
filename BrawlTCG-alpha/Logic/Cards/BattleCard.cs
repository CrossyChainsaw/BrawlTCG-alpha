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
        public bool OneTimeUse { get; private set; }
        public bool Stackable { get; private set; }
        public bool FriendlyFire { get; private set; }
        public int Damage { get; private set; }
        public int PowerModifier { get; private set; }
        public int HealthModifier { get; private set; }
        public BattleCard(int id, string name, int cost, string description, Elements element, Image image, bool oneTimeUse, bool stackable, bool friendlyFire, Action<object, Card, Game>? startTurnEffect = null, Action<object>? endTurnEffect = null, Action<object, Card, Game>? whenPlayedEffect = null, int damage = 0, int powerModifier = 0, int healthModifier = 0) : base(id, name, cost, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect)
        {
            // req
            Name = name;
            Cost = cost;
            Description = description;
            Element = element;
            Image = image;
            OneTimeUse = oneTimeUse;
            Stackable = stackable;
            FriendlyFire = friendlyFire;
            // opt
            StartTurnEffect = startTurnEffect;
            EndTurnEffect = endTurnEffect;
            WhenPlayedEffect = whenPlayedEffect;
            // other opt
            Damage = damage;
            PowerModifier = powerModifier;
            HealthModifier = healthModifier;
        }

        public override Card Clone()
        {
            return new BattleCard(ID, Name, Cost, Description, Element, Image, OneTimeUse, Stackable, FriendlyFire, StartTurnEffect, EndTurnEffect, WhenPlayedEffect, Damage, PowerModifier, HealthModifier);
        }
    }
}
