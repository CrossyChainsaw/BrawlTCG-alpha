using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrawlTCG_alpha.Logic.Cards;
using Images = BrawlTCG_alpha.Properties.Resources;

namespace BrawlTCG_alpha.Logic
{
    internal class CardCatalogue
    {
        public static Dictionary<string, Card> CardDictionary = new Dictionary<string, Card>()
        {
            // Essence
            { "Essence", new EssenceCard("Essence", 0, Elements.Magic, Images.Essence) },

            // Stages
            { "Mustafar", new StageCard("Mustafar", 2, "Start Turn: Every non-Fire Legend will lose 1 HP", Elements.Fire, Images.Mustafar, startTurnEffect: EffectCatalogue.MustafarEffect) },
            { "Fangwild", new StageCard("Fangwild", 3, "Start Turn: Magic and Nature Legends will be healed by 2", Elements.Nature, Images.Fangwild, startTurnEffect: EffectCatalogue.FangwildEffect) },
            { "Matrix", new StageCard("Matrix", 1, "Nothing", Elements.Cosmic, Images.Matrix) },

            // Fire
            { "IronLady", new LegendCard("Iron Lady", 5, Elements.Fire, Images.IronLady, 7, 5, 15, 0, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash, attack2: AttackCatalogue.Scythe_Gimp, attack3: AttackCatalogue.Lance_Flamethrower, attack4: AttackCatalogue.Artemis_IronLady_MeltDown) },
            { "Heatblast", new LegendCard("Heatblast", 4, Elements.Fire, Images.Heatblast, 9, 5, 6, 0, Weapons.Blasters, Weapons.Spear, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Heatblast_Burn) },
            // Cosmic
            { "Artemis", new LegendCard("Artemis", 3, Elements.Cosmic, Images.Artemis, 5, 5, 12, 0, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash, attack2: AttackCatalogue.Scythe_Gimp, attack3: AttackCatalogue.Lance_Flamethrower) },
            { "Orion", new LegendCard("Orion", 3, Elements.Cosmic, Images.Orion, 4, 6, 12, 0, Weapons.RocketLance, Weapons.Spear, attack1: AttackCatalogue.Spear_Stab, attack3: AttackCatalogue.Lance_Flamethrower) },
            // Magic
            { "FaerieQueen", new LegendCard("Faerie Queen", 3, Elements.Magic, Images.FaerieQueen, 7, 3, 8, 0, Weapons.Spear, Weapons.Greatsword, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Greatsword_Swing, attack3: AttackCatalogue.Greatsword_String, attack4: AttackCatalogue.Arcadia_PinkRoses) }, // base stance
            { "Enchantress", new LegendCard("Enchantress", 4, Elements.Magic, Images.Enchantress, 0, 5, 18, 0, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Enchantress_EnchantHealth, attack2: AttackCatalogue.Enchantress_EnchantPower, attack3: AttackCatalogue.Enchantress_CurseHealth, attack4: AttackCatalogue.Enchantress_CursePower) },
            { "DarkMage", new LegendCard("Dark Mage", 5, Elements.Magic, Images.DarkMage, 11, 1, 12, 0, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Scythe_Slash) },
            { "PlagueKnight", new LegendCard("Plague Knight", 4, Elements.Magic, Images.PlagueKnight, 1, 5, 14, 0, Weapons.Gauntlets, Weapons.Katars, attack1: AttackCatalogue.PlagueKnight_GrabHealingPotion) }, // healer
            // Shadow
            { "MasterThief", new LegendCard("Master Thief", 2, Elements.Shadow, Images.MasterThief, 4, 5, 4, 0, Weapons.Gauntlets, Weapons.Katars, attack1: AttackCatalogue.Any_BurnForCard, attack2: AttackCatalogue.MasterThief_GrabBomb) },
            { "Raymesis", new LegendCard("Raymesis", 5, Elements.Shadow, Images.Raymesis, 9, 5, 16, 0, Weapons.Gauntlets, Weapons.Axe, attack1: AttackCatalogue.Axe_Swing) },
            { "Mastermind", new LegendCard("Mastermind", 4, Elements.Shadow, Images.Loki, 5, 6, 10, 0, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Scythe_Slash) },
            // Wild
            { "TheMinotaur", new LegendCard("The Minotaur", 5, Elements.Wild, Images.Teros, 10, 1, 15, 0, Weapons.Axe, Weapons.Hammer, attack1: AttackCatalogue.Axe_Swing, attack2: AttackCatalogue.Hammer_Swing) },

            //Axe
            { "AxeOfRegrowth", new WeaponCard("Axe of Regrowth", 1, Elements.Nature, Images.Axe_of_Regrowth, Weapons.Axe) },
            // Battle Boots
            // Blasters
            { "BlazingFire", new WeaponCard("Blazing Fire", 1, Elements.Fire, Images.BlazingFire, Weapons.Blasters) },
            // Bow
            // Gauntlets
            { "SleightOfHand", new WeaponCard("Sleight of Hand", 1, Elements.Shadow, Images.Sleight_of_Hand, Weapons.Gauntlets) },
            // Greatsword
            { "LawOfTheLand", new WeaponCard("Law of the Land", 1, Elements.Nature, Images.LawOfTheLand, Weapons.Greatsword) },
            // Hammer
            { "PrimroseMallet", new WeaponCard("Primrose Mallet", 1, Elements.Nature, Images.Primrose_Mallet, Weapons.Hammer) },
            // Katars
            { "NightmareClaws", new WeaponCard("Nightmare Claws", 1, Elements.Wild, Images.Nightmare_Claws, Weapons.Katars) },
            // Lance
            { "GalaxyLance", new WeaponCard("Galaxy Lance", 1, Elements.Cosmic, Images.GalaxyLance, Weapons.RocketLance) },
            // Orb
            { "RemnantOfFate", new WeaponCard("Remnant of Fate", 1, Elements.Magic, Images.RemnantOfFate, Weapons.Orb) },
            { "SacredRelic", new WeaponCard("Sacred Relic", 1, Elements.Nature, Images.SacredRelic, Weapons.Orb) },
            { "ScryingGlass", new WeaponCard("Scrying Glass", 1, Elements.Cosmic, Images.ScryingGlass, Weapons.Orb) },
            // Scythe
            { "SearingBlade", new WeaponCard("Searing Blade", 1, Elements.Fire, Images.SearingBlade, Weapons.Scythe) },
            { "ShootingStar", new WeaponCard("Shooting Star", 1, Elements.Cosmic, Images.ShootingStar, Weapons.Scythe) },
            { "StarryScythe", new WeaponCard("Starry Scythe", 1, Elements.Cosmic, Images.StarryScythe, Weapons.Scythe) },
            // Spear
            { "MagmaSpear", new WeaponCard("Magma Spear", 1, Elements.Fire, Images.MagmaSpear, Weapons.Spear) },
            { "PiercingRegret", new WeaponCard("Piercing Regret", 1, Elements.Nature, Images.PiercingRegret, Weapons.Spear) },
            // Sword
        
            { "BouncyBomb", new BattleCard("Bouncy Bomb", 2, "When Played: Deals direct damage", Elements.Shadow, Images.BouncyBomb, oneTimeUse: true, stackable: false, friendlyFire: false, whenPlayedEffect: EffectCatalogue.DirectDamage, damage: 8) },
            { "VialOfCrows", new BattleCard("Vial of Crows", 1, "When Played: Boosts legend stats", Elements.Shadow, Images.Vial_of_Crows, true, false, true, whenPlayedEffect: EffectCatalogue.Heal, healthModifier: 5) },
            { "Snowball", new BattleCard("Snowball", 1, "When Played: Deals direct damage", Elements.Arctic, Images.Snowball, true, false, false, whenPlayedEffect: EffectCatalogue.DirectDamage, damage: 4) },
            { "SuperSaiyan", new BattleCard("Super Saiyan", 4, "When Played: Boosts legend stats", Elements.Wild, Images.SuperSaiyan, false, true, true, whenPlayedEffect: EffectCatalogue.BoostStats, powerModifier: 6, healthModifier: 6) },

        };




        // Fire
        //priya
        //ulgrim
        //barazza
        //scarlet
        //jhala
        //seven
        //hellboy
        //zuko
        //devil jin
        //lin fei
        //petra

        // Cosmic
        //val
        //wu shang
        //megaman
        //ada
        //sentinel
        //vraxx
        //zariel
        //thea
        //redraptor
        //thor
        //vector

        // Nature
        //glowbox
        //gnash
        //kor
        //diana
        //ember
        //yumiko
        //reno

        // Magic

        // Shadow
        //cross
        //jiro
        //dusk
        //volkov
        //loki
        //nix
        //jaeyun
        //diana
        //lucien

        // Wild
        //beardvar
        //gnash
        //asuri
        //asuri
        //ragnir
        //mordex
        //yumiko dog
        //ember squirle
        //lionheart
        //onyx
        //munin
        //imugi
        //tezca
        //vivi
        //hattori
        //xull
        //mako
        //tezca
        //reno

        // Ice // arctic? + water?
        // yeetee teros
        // christmas skins



        // Decks


        public static List<Card> Deck1 = null;
        public static List<Card> Deck2 = null;


        public static Card GetCardByName(string cardName)
        {
            if (CardDictionary.ContainsKey(cardName))
            {
                return CardDictionary[cardName].Clone(); // Clone the card to avoid direct modification
            }
            else
            {
                throw new Exception($"Card with name {cardName} not found.");
            }
        }

        public static List<Card> CloneList(List<Card> originalList)
        {
            return originalList.Select(card => card.Clone()).ToList(); // Deep clone each card in the list
        }
    }
}
