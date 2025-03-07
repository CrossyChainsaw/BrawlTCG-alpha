﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BrawlTCG_alpha.Logic.Cards
{
    public class BattleCard : Card
    {
        public bool OneTimeUse { get; private set; }
        public bool Stackable { get; private set; }
        public bool FriendlyFire { get; private set; }
        public int Damage { get; private set; }
        public int PowerModifier { get; private set; }
        public int HealthModifier { get; private set; }
        public Elements[] TargetElements { get; private set; }
        public bool MultiTarget { get; private set; }
        public BattleCard(int id, string name, int cost, string description, Elements element, Image image, bool oneTimeUse, bool stackable, bool friendlyFire, 
            Effect? startTurnEffect = null, Action<object>? endTurnEffect = null, 
            Effect? whenPlayedEffect = null, int damage = 0, int powerModifier = 0, int healthModifier = 0, Elements[] targetElements = null, bool multiTarget = false) : base(id, name, cost, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect)
        {
            Description = description;
            OneTimeUse = oneTimeUse;
            Stackable = stackable;
            FriendlyFire = friendlyFire;
            // other opt
            Damage = damage;
            PowerModifier = powerModifier;
            HealthModifier = healthModifier;
            TargetElements = targetElements;
            MultiTarget = multiTarget;
        }

        public override Card Clone()
        {
            return new BattleCard(ID, Name, Cost, Description, Element, Image, OneTimeUse, Stackable, FriendlyFire, StartTurnEffect, EndTurnEffect, WhenPlayedEffect, Damage, PowerModifier, HealthModifier, TargetElements, MultiTarget);
        }
    }
}
