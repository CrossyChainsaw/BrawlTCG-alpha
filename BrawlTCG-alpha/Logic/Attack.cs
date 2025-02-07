using BrawlTCG_alpha.Logic.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    internal class Attack
    {
        // Req
        public string Name { get; private set; }
        public Weapons WeaponOne { get; private set; }
        public int WeaponOneAmount { get; private set; }
        public Action<LegendCard, object, Attack> Effect { get; private set; }
        public int AttackModifier { get; private set; }
        // Opt.
        public Weapons? WeaponTwo { get; private set; }
        public int? WeaponTwoAmount { get; private set; }

        public Attack(string name, int attackModifier, Weapons weaponOne, int weaponOneAmount, Action<LegendCard, object, Attack> execute, Weapons? weaponTwo = null, int? weaponTwoAmount = null)
        {
            // Req
            Name = name;
            AttackModifier = attackModifier;
            WeaponOne = weaponOne;
            WeaponOneAmount = weaponOneAmount;
            Effect = execute;
            // Opt.
            WeaponTwo = weaponTwo;
            WeaponTwoAmount = weaponTwoAmount;
        }
        public void Execute(LegendCard attacker, object target)
        {
            if (target is LegendCard legendCard)
            {
                Effect(attacker, legendCard, this);
            }
            else
            {
                throw new Exception();
            }
        }
        public Attack Clone()
        {
            return new Attack(Name, AttackModifier, WeaponOne, WeaponOneAmount, Effect, WeaponTwo, WeaponTwoAmount);
        }

    }
}
