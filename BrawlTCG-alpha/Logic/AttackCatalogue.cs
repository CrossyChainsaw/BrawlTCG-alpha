﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    public class AttackCatalogue
    {
        // target is Player or LegendCard
        public static void DefaultAttack(LegendCard attacker, object target, Attack attack, bool burn = false)
        {
            int damage = CalculateDamage(attacker, attack);

            if (target is LegendCard legendCard)
            {
                legendCard.LoseHealth(damage);
                legendCard.SetBurn(burn);
            }
            else if (target is Player player)
                player.LoseHealth(damage);
            else
                throw new Exception("Invalid target.");

            // Burn Cards
            attacker.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
            attacker.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);
        }
        public static int CalculateDamage(LegendCard attackingLegend, Attack attack)
        {
            int damage = attackingLegend.Power + attack.AttackModifier;
            if (damage < 0)
            {
                damage = 0;
            }

            // check if the element of the weapon you need for this attack is the same as the element of the legend
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
                foreach (WeaponCard weaponCard in attackingLegend.StackedCards)
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

            return damage + elementalDamageBoost;
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
            attacker.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);
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
            attacker.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);
        }
        public static void ModifyStat(LegendCard attacker, object target, Attack attack, Stats stat, int modifyAmount)
        {
            if (target is LegendCard legendCard)
                legendCard.ModifyStat(stat, modifyAmount);
            else if (target is Player player)
                MessageBox.Show("You cannot attack the player with this attack");
            else
                throw new Exception("Invalid target.");

            attacker.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
            attacker.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);
        }
        public static void DrawCards(LegendCard attacker, object target, Attack attack, Player activePlayer, Game game, int nCards)
        {
            // Draw the Cards
            for (int i = 0; i < nCards; i++)
            {
                game.DrawCardFromDeck(activePlayer);
            }
            // Show the Cards
            game.ShowCards();
            // Burn Weapons
            attacker.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
            attacker.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);
        }
        public static void GenerateCard(LegendCard attacker, object target, Attack attack, Player activePlayer, Game game, Card generatedCard)
        {
            Card card = generatedCard;
            // add to hand
            game.AddCardToHandZone(activePlayer, card);
            // flip to show
            game.ShowCards();
        }
        public static void GenerateRandomFireCard(object target, Card card, Game game)
        {
            int nCards = 1;
            EffectCatalogue.GenerateRandomElementalCards(game, nCards, Elements.Fire);
            game.ShowCards();
        }
        public static void SpawnAndPlayLegend(Game game, int cardID)
        {
            EffectCatalogue.GenerateAndPlayLegend(game, cardID);
        }

        // Default Weapon Attacks
        public static Attack Spear_Stab = new Attack("Spear Stab", 0, Weapons.Spear, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });
        public static Attack Orb_Throw = new Attack("Orb Throw", 3, Weapons.Orb, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        }, weaponOneBurnAmount: 1);
        public static Attack Greatsword_Swing = new Attack("Great Swing", 0, Weapons.Greatsword, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Greatsword_String = new Attack("Stab, Slice, Swing", 5, Weapons.Greatsword, 3, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Scythe_Slash = new Attack("Scythe Slash", 0, Weapons.Scythe, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Scythe_Gimp = new Attack("Scythe Gimp", int.MaxValue, Weapons.Scythe, 3, execute: (attacker, target, attack, activePlayer, game) =>
        {
            OneHitKO(attacker, target, attack);
        }, weaponOneBurnAmount: 2);

        public static Attack Lance_Flamethrower = new Attack("Flamethrower", 4, Weapons.RocketLance, 2, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        // Tap Out
        public static Attack Any_BlowAKiss = new Attack("Blow a Kiss", -1000, Weapons.Any, 2, execute: (attacker, target, attack, activePlayer, game) =>
        {
            TapEnemyCard(attacker, target, attack);
        });
        public static Attack Any_Freeze = new Attack("Freeze", 0, Weapons.Any, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            TapEnemyCard(attacker, target, attack);
        });

        public static Attack Any_BurnForThreeCard = new Attack("Draw Three Cards", -1000, Weapons.Any, 1, weaponOneBurnAmount: 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DrawCards(attacker, target, attack, activePlayer, game, nCards: 3);
        }, instaEffect: true);

        public static Attack Hammer_Swing = new Attack("Hammer Swing", 1, Weapons.Hammer, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });
        public static Attack Hammer_Gimp = new Attack("Hammer Gimp", int.MaxValue, Weapons.Hammer, 3, execute: (attacker, target, attack, activePlayer, game) =>
        {
            OneHitKO(attacker, target, attack);
        }, weaponOneBurnAmount: 2);

        public static Attack Axe_Swing = new Attack("Axe Swing", 1, Weapons.Axe, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Gauntlets_Punch = new Attack("Punch", 0, Weapons.Gauntlets, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Gauntlets_PowerPunch = new Attack("Power Punch", 4, Weapons.Gauntlets, 2, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Katar_Slash = new Attack("Slash", 0, Weapons.Katars, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });
        public static Attack Sword_Slash = new Attack("Slash", 0, Weapons.Sword, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Blaster_Shot = new Attack("Shot", 0, Weapons.Blasters, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });
        public static Attack Blaster_DoubleShot = new Attack("Shot", 5, Weapons.Blasters, 2, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });

        public static Attack Any_CraftFireCard = new Attack("Craft Fire Card", -1000, Weapons.Any, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            GenerateRandomFireCard(target, attacker, game);
            game.ShowCards();
        }, instaEffect: true);


        // Signature Attacks
        public static Attack Arcadia_PinkRoses = new Attack("Pink Roses", 4, Weapons.Spear, 1, weaponTwo: Weapons.Greatsword, weaponTwoAmount: 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });
        public static Attack Artemis_IronLady_MeltDown = new Attack("Meltdown", 7, Weapons.Scythe, 1, weaponOneBurnAmount: 1, weaponTwo: Weapons.RocketLance, weaponTwoAmount: 1, weaponTwoBurnAmount: 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });
        public static Attack Enchantress_CursePower = new Attack("Curse Att by 3", -1000, Weapons.Any, 1, weaponOneBurnAmount: 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            int modifyAmount = -3;
            ModifyStat(attacker, target, attack, Stats.Power, modifyAmount);
        });
        public static Attack Enchantress_CurseHealth = new Attack("Curse HP by 4", -1000, Weapons.Any, 1, weaponOneBurnAmount: 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            int modifyAmount = -4;
            ModifyStat(attacker, target, attack, Stats.Health, modifyAmount);
        });
        public static Attack Enchantress_EnchantPower = new Attack("Enchant Att by 3", -1000, Weapons.Any, 1, weaponOneBurnAmount: 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            int modifyAmount = 3;
            ModifyStat(attacker, target, attack, Stats.Power, modifyAmount);
        }, friendlyFire: true);
        public static Attack Enchantress_EnchantHealth = new Attack("Enchant HP by 4", -1000, Weapons.Any, 1, weaponOneBurnAmount: 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            int modifyAmount = 4;
            ModifyStat(attacker, target, attack, Stats.Health, modifyAmount);
        }, friendlyFire: true);
        public static Attack DeathCap_Storm = new Attack("Storm", 0, Weapons.Orb, 3, weaponOneBurnAmount: 3, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        }, multiHit: true);
        public static Attack Heatblast_Burn = new Attack("Burn", 0, Weapons.Blasters, 1, weaponOneBurnAmount: 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack, burn: true);
        });
        public static Attack MasterThief_GrabBomb = new Attack("Grab Bomb", -1000, Weapons.Gauntlets, 1, weaponOneBurnAmount: 0, execute: (attacker, target, attack, activePlayer, game) =>
        {
            Card card = CardCatalogue.GetCardById(500); // #500: Bouncy Bomb
            GenerateCard(attacker, target, attack, activePlayer, game, generatedCard: card);
        }, instaEffect: true);
        public static Attack PlagueKnight_GrabHealingPotion = new Attack("Grab Healing Potion", -1000, Weapons.Gauntlets, 1, weaponOneBurnAmount: 0, execute: (attacker, target, attack, activePlayer, game) =>
        {
            Card card = CardCatalogue.GetCardById(501); // #501: Vial of Crows 
            GenerateCard(attacker, target, attack, activePlayer, game, generatedCard: card);
        }, instaEffect: true);
        public static Attack Yumiko_GrabOrbs = new Attack("Spawn Orbs", -1000, Weapons.Any, 1, weaponOneBurnAmount: 0, execute: (attacker, target, attack, activePlayer, game) =>
        {
            Card card = CardCatalogue.GetCardById(504); // #504: Orb
            GenerateCard(attacker, target, attack, activePlayer, game, generatedCard: card);
            GenerateCard(attacker, target, attack, activePlayer, game, generatedCard: card);
            GenerateCard(attacker, target, attack, activePlayer, game, generatedCard: card);
        }, instaEffect: true);
        public static Attack WuShang_DownSig = new Attack("Gauntlet Dsig", -2, Weapons.Gauntlets, 2, weaponOneBurnAmount: 0, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        }, multiHit: true);
        public static Attack NinjaSpirit_PhantomSlash = new Attack("Phantom Slash", 0, Weapons.Sword, 0, execute: (attacker, target, attack, activePlayer, game) =>
        {
            DefaultAttack(attacker, target, attack);
        });
        public static Attack ForestSpirit_Explode = new Attack("Explode", 0, Weapons.Any, 0, execute: (attacker, target, attack, activePlayer, game) =>
        {
            int recoil = int.MaxValue;
            DefaultAttack(attacker, target, attack);
            attacker.LoseHealth(recoil);
        });
        public static Attack ForestSpirit_Heal = new Attack("Heal", 0, Weapons.Any, 0, execute: (attacker, target, attack, activePlayer, game) =>
        {
            int healAmount = attacker.Power;
            LegendCard legend = (LegendCard)target;
            legend.GainHealth(healAmount);

            int recoil = int.MaxValue;
            attacker.LoseHealth(recoil);
        }, friendlyFire: true);
        public static Attack Jiro_SpawnAndPlayNinjaSpirit = new Attack("Spawn Ninja Spirit", -1000, Weapons.Sword, 1, execute: (attacker, target, attack, activePlayer, game) =>
        {
            int ninjaSpiritID = 5003;
            SpawnAndPlayLegend(game, ninjaSpiritID);
        }, instaEffect: true);
    }
}
