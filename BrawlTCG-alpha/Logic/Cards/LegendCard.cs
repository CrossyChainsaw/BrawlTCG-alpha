using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    internal class LegendCard : Card
    {
        public int HitPoints { get; internal set; }
        public int CurrentHP { get; internal set; }
        public int Power { get; internal set; }
        public int Dexterity { get; internal set; }
        public int Defense { get; internal set; }
        public int Speed { get; internal set; }
        public Weapons PrimaryWeapon { get; internal set; }
        public Weapons SecondaryWeapon { get; internal set; }
        public List<Card> StackedCards { get; internal set; }
        // Nullable
        public Action<object>? Ability { get; private set; }
        public Action<object>? Attack1 { get; private set; }
        public Action<object>? Attack2 { get; private set; }
        public Action<object>? Attack3 { get; private set; }
        public Action<object>? Attack4 { get; private set; }

        public LegendCard(
            // Card
            string name,
            int cost,
            string description,
            Elements element,
            Image image,
            // LegendCard
            int power,
            int dexterity,
            int defense,
            int speed,
            Weapons primaryWeapon,
            Weapons secondaryWeapon,
            // Card Opt.
            Action<object>? startTurnEffect = null,
            Action<object>? endTurnEffect = null,
            Action<object>? whenPlayedEffect = null,
            // LegendCard Opt.
            Action<object>? ability = null,
            Action<object>? attack1 = null,
            Action<object>? attack2 = null,
            Action<object>? attack3 = null,
            Action<object>? attack4 = null
        ) : base(name, cost, description, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect)
        {
            // Card
            Name = name;
            Cost = cost;
            Description = description;
            Element = element;
            Image = image;
            // LegenCard
            Power = power;
            Dexterity = dexterity;
            Defense = defense;
            Speed = speed;
            HitPoints = Defense + Speed;
            CurrentHP = HitPoints;
            PrimaryWeapon = primaryWeapon;
            SecondaryWeapon = secondaryWeapon;
            // Card Optional
            StartTurnEffect = startTurnEffect;
            EndTurnEffect = endTurnEffect;
            WhenPlayedEffect = whenPlayedEffect;
            // LegendCard Optional
            Ability = ability;
            Attack1 = attack1;
            Attack2 = attack2;
            Attack3 = attack3;
            Attack4 = attack4;
            // Other
            StackedCards = new List<Card>();
        }

        public override Card Clone()
        {
            return new LegendCard(
                Name,
                Cost,
                Description,
                Element,
                Image,
                Power,
                Dexterity,
                Defense,
                Speed,
                PrimaryWeapon,
                SecondaryWeapon,
                StartTurnEffect,
                EndTurnEffect,
                WhenPlayedEffect,
                Ability,
                Attack1,
                Attack2,
                Attack3,
                Attack4
            );
        }
        public List<Weapons> GetWeapons()
        {
            return new List<Weapons> { PrimaryWeapon, SecondaryWeapon };
        }

        internal void StackCard(Card card)
        {
            // TODO: Differentiate battle/weapon cards

            if (card is WeaponCard weaponCard)
            {
                StackedCards.Add(weaponCard);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
