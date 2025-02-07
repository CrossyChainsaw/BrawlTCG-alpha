using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BrawlTCG_alpha.Logic.Cards
{
    enum Weapons
    {
        Blasters,
        RocketLance,
        Spear,
        Katars,
        Axe,
        Bow,
        Gauntlets,
        Scythe,
        Cannon,
        Orb,
        Greatsword,
        BattleBoots
    }

    internal class WeaponCard : Card
    {
        public Weapons Weapon { get; internal set; }

        public WeaponCard(string name, int cost, string description, Elements element, Image image, Weapons weapon, Action<object>? startTurnEffect = null, Action<object>? endTurnEffect = null, Action<object>? whenPlayedEffect = null) : base(name, cost, description, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect)
        {
            // Card
            Name = name;
            Cost = cost;
            Description = description;
            Element = element;
            Image = image;
            // Weapon Card
            Weapon = weapon;
            // Card Opt.
            StartTurnEffect = startTurnEffect;
            EndTurnEffect = endTurnEffect;
            WhenPlayedEffect = whenPlayedEffect;
            
        }

        public override Card Clone()
        {
            return new WeaponCard(Name, Cost, Description, Element, Image, Weapon, StartTurnEffect, EndTurnEffect, WhenPlayedEffect);
        }
    }
}
