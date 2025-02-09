using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    internal class AttackCatalogue
    {
        public static void DefaultAttack(LegendCard attacker, object target, Attack attack)
        {
            int damage = attacker.Power + attack.AttackModifier;
            if (damage < 0)
            {
                damage = 0;
            }

            if (target is LegendCard legendCard)
                legendCard.LoseHealth(damage);
            else if (target is Player player)
                player.LoseHealth(damage);
            else
                throw new Exception("Invalid target.");

            attacker.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
            attacker.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);
        }

        public static void OneHitKO(LegendCard attacker, object target, Attack attack)
        {
            if (target is LegendCard legendCard)
                legendCard.LoseHealth(legendCard.CurrentHP);
            else if (target is Player player)
                MessageBox.Show("You cannot attack the player with this attack");
            else
                throw new Exception("Invalid target.");

            attacker.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
        }

        public static void TapEnemyCard(LegendCard attacker, object target, Attack attack)
        {
            if (target is LegendCard legendCard)
                legendCard.TapOut();
            else if (target is Player player)
                MessageBox.Show("You cannot attack the player with this attack");
            else
                throw new Exception("Invalid target.");

            attacker.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
        }

        // Default Weapon Attacks
        public static Attack Spear_Stab = new Attack("Spear Stab", 0, Weapons.Spear, 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Greatsword_Swing = new Attack("Great Swing", 0, Weapons.Greatsword, 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Greatsword_String = new Attack("Stab, Slice, Swing", 5, Weapons.Greatsword, 3, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Scythe_Slash = new Attack("Scythe Slash", 1, Weapons.Scythe, 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Scythe_Gimp = new Attack("Scythe Gimp", int.MaxValue, Weapons.Scythe, 3, execute: (attacker, target, attack) =>
        {
            OneHitKO(attacker, target, attack);
        }, weaponOneBurnAmount: 2);

        public static Attack Lance_Flamethrower = new Attack("Flamethrower", 3, Weapons.RocketLance, 2, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });
        public static Attack Any_Seduce = new Attack("Blow a Kiss", -1000, Weapons.Any, 2, execute: (attacker, target, attack) =>
        {
            TapEnemyCard(attacker, target, attack);
        });

        // Signature Attacks
        public static Attack Arcadia_PinkRoses = new Attack("Pink Roses", 4, Weapons.Spear, 1, weaponTwo: Weapons.Greatsword, weaponTwoAmount: 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });
        public static Attack Artemis_IronLady_MeltDown = new Attack("Meltdown", 4, Weapons.Scythe, 1, weaponOneBurnAmount: 1, weaponTwo: Weapons.RocketLance, weaponTwoAmount: 1, weaponTwoBurnAmount: 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack);
        });
    }
}
