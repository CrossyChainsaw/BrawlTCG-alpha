using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    internal class Attack
    {
        // Req
        public string Name { get; private set; }
        public Weapons WeaponOne { get; private set; }
        public int WeaponOneAmount { get; private set; }
        public Action<LegendCard, object, Attack, Player> Effect { get; private set; }
        public int AttackModifier { get; private set; }
        // Opt.
        public Weapons? WeaponTwo { get; private set; }
        public int? WeaponTwoAmount { get; private set; }
        public int WeaponOneBurnAmount { get; private set; }
        public int WeaponTwoBurnAmount { get; private set; }
        public bool FriendlyFire { get; private set; }
        public bool MultiHit { get; private set; } // hits everyone
        public bool InstaEffect { get; private set; }
        //public string Description { get; private set; }


        public Attack(string name, int attackModifier, Weapons weaponOne, int weaponOneAmount, Action<LegendCard, object, Attack, Player> execute, int weaponOneBurnAmount = 0, Weapons? weaponTwo = null, int? weaponTwoAmount = null, int weaponTwoBurnAmount = 0, bool friendlyFire = false, bool multiHit = false, bool instaEffect = false)
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
            WeaponOneBurnAmount = weaponOneBurnAmount;
            WeaponTwoBurnAmount = weaponTwoBurnAmount;
            // Other Opt.
            FriendlyFire = friendlyFire;
            MultiHit = multiHit;
            InstaEffect = instaEffect;
        }
        public void Execute(LegendCard attacker, object target, Player activePlayer)
        {
            if (target is LegendCard legendCard)
            {
                Effect(attacker, legendCard, this, activePlayer);
            }
            else
            {
                throw new Exception();
            }
        }
        public Attack Clone()
        {
            return new Attack(Name, AttackModifier, WeaponOne, WeaponOneAmount, Effect, WeaponOneBurnAmount, WeaponTwo, WeaponTwoAmount, WeaponTwoBurnAmount, FriendlyFire);
        }

    }
}
