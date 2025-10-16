using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    public class Attack
    {
        // Req
        public string Name { get; private set; }
        public Weapons WeaponOne { get; private set; }
        public int WeaponOneAmount { get; private set; }
        public Action<LegendCard, object, Attack, Player, Game> Effect { get; private set; }
        public int AttackModifier { get; private set; }
        // Opt.
        public Weapons? WeaponTwo { get; private set; }
        public int? WeaponTwoAmount { get; private set; }
        public int WeaponOneBurnAmount { get; private set; }
        public int WeaponTwoBurnAmount { get; private set; }
        public bool FriendlyFire { get; private set; }
        public bool MultiHit { get; private set; } // hits everyone
        public bool InstaEffect { get; private set; } // change this property to targetRequired default value true?
        public int Recoil {  get; private set; }


        public Attack(string name, int attackModifier, Weapons weaponOne, int weaponOneAmount, Action<LegendCard, object, Attack, Player, Game> execute, int weaponOneBurnAmount = 0, Weapons? weaponTwo = null, int? weaponTwoAmount = null, int weaponTwoBurnAmount = 0, bool friendlyFire = false, bool multiHit = false, bool instaEffect = false, int recoilDamage=0)
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
            Recoil = recoilDamage;
        }

        public static int CheckElementalDamageBoost(LegendCard attackingLegend, Attack attack)
        {
            int elementalDamageBoost = 0;
            foreach (Card card in attackingLegend.StackedCards)
            {
                if (card is WeaponCard weaponCard)
                {
                    int requiredMatches = attack.WeaponOneAmount;
                    int foundMatches = 0;
                    if (weaponCard.Weapon == attack.WeaponOne)
                    {
                        if (weaponCard.Element == attackingLegend.Element)
                        {
                            foundMatches++;
                            if (foundMatches == requiredMatches)
                            {
                                elementalDamageBoost += requiredMatches;
                                break;
                            }
                        }
                    }
                }
            }
            if (attack.WeaponTwo != null)
            {
                foreach (Card card in attackingLegend.StackedCards)
                {
                    if (card is WeaponCard weaponCard)
                    {
                        int requiredMatches = (int)attack.WeaponTwoAmount;
                        int foundMatches2 = 0;
                        if (weaponCard.Weapon == attack.WeaponTwo)
                        {
                            if (weaponCard.Element == attackingLegend.Element)
                            {
                                foundMatches2++;
                                if (foundMatches2 == requiredMatches)
                                {
                                    elementalDamageBoost += requiredMatches;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return elementalDamageBoost;
        }

        public void Invoke(LegendCard legend, object target, Attack attack, Player player, Game game)
        {
            Effect?.Invoke(legend, target, attack, player, game); // maybe check if the attack succeeded  
        }
    }
}
