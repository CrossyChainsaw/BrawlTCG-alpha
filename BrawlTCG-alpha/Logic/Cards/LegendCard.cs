using BrawlTCG_alpha.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    enum Stats
    {
        Health,
        Power,
    }
    internal class LegendCard : Card
    {
        // Constants
        const int BURN_DAMAGE = 1;
        // Properties
        public int BaseHealth { get; internal set; }
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
        public Attack Attack1 { get; private set; }
        public Attack Attack2 { get; private set; }
        public Attack Attack3 { get; private set; }
        public Attack Attack4 { get; private set; }
        public bool IsTapped { get; private set; }
        public bool IsBurned { get; internal set; } = false;
        public bool AttackedThisTurn { get; set; } = false;
        public bool OnPlayingField { get; set; } = false;

        public event Action<LegendCard, WeaponCard> UI_BurnWeaponCard;
        public event Action<LegendCard> UI_RearrangeMyStackedCards;

        public LegendCard(
            // Card
            int id,
            string name,
            int cost,
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
            Action<object, Card, Game>? startTurnEffect = null,
            Action<object>? endTurnEffect = null,
            Action<object, Card, Game>? whenPlayedEffect = null,
            // LegendCard Opt.
            Action<object>? ability = null,
            Attack attack1 = null,
            Attack attack2 = null,
            Attack attack3 = null,
            Attack attack4 = null
        ) : base(id, name, cost, element, image, startTurnEffect, endTurnEffect, whenPlayedEffect)
        {
            // LegenCard
            Power = power;
            Dexterity = dexterity;
            Defense = defense;
            Speed = speed;
            BaseHealth = Defense + Speed;
            CurrentHP = BaseHealth;
            PrimaryWeapon = primaryWeapon;
            SecondaryWeapon = secondaryWeapon;
            Description = $"{BaseHealth,4}hp {power,4}att {primaryWeapon,20} {secondaryWeapon,20}";
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
                ID,
                Name,
                Cost,
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
        List<Weapons> GetWeapons()
        {
            return new List<Weapons> { PrimaryWeapon, SecondaryWeapon };
        }
        internal void StackCard(Card card)
        {
            // TODO: Differentiate battle/weapon cards

            if (card is WeaponCard || card is BattleCard)
            {
                StackedCards.Add(card);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public void LoseHealth(int damage)
        {
            CurrentHP -= damage;
        }
        public void GainHealth(int healing)
        {
            CurrentHP += healing;
            if (CurrentHP > BaseHealth)
            {
                CurrentHP = BaseHealth;
            }
        }
        public List<Attack> GetAttacks()
        {
            // this technique is called LINQ
            return new List<Attack> { Attack1, Attack2, Attack3, Attack4 }
                .Where(attack => attack != null)
                .ToList();
        }
        public void BurnWeapon(Weapons? attackWeapon, int? burnAmount)
        {
            if (attackWeapon != null)
            {
                // burn cards
                int cardsBurned = 0;
                for (int i = 0; i < StackedCards.Count && cardsBurned < burnAmount; i++)
                {
                    if (StackedCards[i] is WeaponCard wepCard)
                    {
                        if (wepCard.Weapon == attackWeapon || attackWeapon == Weapons.Any)
                        {
                            StackedCards.RemoveAt(i);
                            cardsBurned++;
                            UI_BurnWeaponCard.Invoke(this, wepCard);
                            i--; // Adjust index because we've removed an item
                        }
                    }
                }
                // rearrange legend stacked cards
                if (cardsBurned > 0)
                {

                }
            }
        }
        public void TapOut()
        {
            IsOpen = false;
            MessageBox.Show($"{this.Name} is now tapped");
        }
        public void SetBurn(bool burn)
        {
            if (Element != Elements.Fire)
            {
                IsBurned = burn;
            }
        } // also add a fire emoji bottom left that would be nice
        /// <summary>Take burn damage if -> IsBurned == true</summary>
        public void TakeBurnDamage()
        {
            if (IsBurned)
            {
                LoseHealth(BURN_DAMAGE);
            }
        }
        public void ModifyStat(Stats stat, int modifyAmount)
        {
            if (stat == Stats.Health)
            {
                BaseHealth += modifyAmount;
                CurrentHP += modifyAmount;
            }
            else if (stat == Stats.Power)
            {
                Power += modifyAmount;
            }
        }
        public bool HasWeapon(WeaponCard weaponCard)
        {
            return GetWeapons().Contains(weaponCard.Weapon);
        }
    }
}
