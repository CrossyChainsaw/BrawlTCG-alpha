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
        public static void DefaultAttack(LegendCard attacker, object target, int attackModifier = 0)
        {
            int damage = attacker.Power + attackModifier;

            if (target is LegendCard legendCard)
                legendCard.TakeDamage(damage);
            else if (target is Player player)
                player.TakeDamage(damage);
            else
                throw new Exception("Invalid target.");
        }


        public static Attack SpearStab = new Attack("Spear Stab", 0, Weapons.Spear, weaponOneAmount: 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack.AttackModifier);
        });

        public static Attack GreatSwordSwing = new Attack("Great Sword Swing", 0, Weapons.Greatsword, weaponOneAmount: 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack.AttackModifier);
        });


        public static Attack GreatSwordString = new Attack("Stab, Slice, Swing", 5, Weapons.Greatsword, 3, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack.AttackModifier);
        });

        public static Attack SpearGreatSwordPinkRoses = new Attack("Pink Roses", 4, Weapons.Spear, 1, weaponTwo: Weapons.Greatsword, weaponTwoAmount: 1, execute: (attacker, target, attack) =>
        {
            DefaultAttack(attacker, target, attack.AttackModifier);
        });

    }
}
