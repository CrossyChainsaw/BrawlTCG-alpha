using BrawlTCG_alpha.Logic.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    internal class AttackCatalogue
    {
        public static void DefaultAttack(LegendCard attacker, object target, Attack attack)
        {
            int damage = attacker.Power + attack.AttackModifier;

            if (target is LegendCard legendCard)
                legendCard.LoseHealth(damage);
            else if (target is Player player)
                player.LoseHealth(damage);
            else
                throw new Exception("Invalid target.");

            attacker.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
        }

        // Default Weapon Attacks
        public static Attack Spear_Stab = new Attack("Spear Stab", 0, Weapons.Spear, 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        }, weaponOneBurnAmount: 1);

        public static Attack Greatsword_Swing = new Attack("Great Swing", 0, Weapons.Greatsword, 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Greatsword_String = new Attack("Stab, Slice, Swing", 5, Weapons.Greatsword, 3, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Scythe_Slash = new Attack("Scythe Slash", 2, Weapons.Scythe, 2, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Scythe_Gimp = new Attack("Scythe Gimp", int.MaxValue, Weapons.Scythe, 3, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        }, weaponOneBurnAmount: 2);

        public static Attack Lance_Flamethrower = new Attack("Flamethrower", 2, Weapons.RocketLance, 2, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        // Signature Attacks
        public static Attack Arcadia_PinkRoses = new Attack("Pink Roses", 4, Weapons.Spear, 1, weaponTwo: Weapons.Greatsword, weaponTwoAmount: 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });

    }
}
