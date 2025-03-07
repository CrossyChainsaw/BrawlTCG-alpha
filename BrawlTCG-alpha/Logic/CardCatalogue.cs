﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BrawlTCG_alpha.Logic.Cards;
using Images = BrawlTCG_alpha.Properties.Resources;
using AC = BrawlTCG_alpha.Logic.Cards.AttackCatalogue;


namespace BrawlTCG_alpha.Logic
{
    public class CardCatalogue
    {
        public static Dictionary<int, Card> CardDictionary = new Dictionary<int, Card>()
        {
            // Essence
            { 0, new EssenceCard(id: 0, "Essence", 0, Elements.Magic, Images.Essence) },

            // Stages
            { 100, new StageCard(id:100, "Mustafar", 2, "Start Turn: Every non-Fire Legend will lose 1 HP", Elements.Fire, Images.Mustafar, startTurnEffect: EffectCatalogue.Mustafar) },
            { 101, new StageCard(id:101, "Fangwild", 3, "Start Turn: Magic and Nature Legends will be healed by 2", Elements.Nature, Images.Fangwild, startTurnEffect: EffectCatalogue.Fangwild) },
            { 102, new StageCard(id:102, "Matrix", 1, "Nothing", Elements.Cosmic, Images.Matrix) },
            { 103, new StageCard(id:103, "The Workshop", 2, "Start Turn: You obtain a random card", Elements.Fire, Images.TheWorkshop, startTurnEffect: EffectCatalogue.Workshop) },
            { 104, new StageCard(id:104, "Evil Hideout", 3, "While in play: Fire, Wild and Shadow legends will do +3 damage", Elements.Fire, Images.Evil_Hideout, whenPlayedEffect: EffectCatalogue.EvilHideoutWhenPlayed, whileInPlayEffect: EffectCatalogue.EvilHideoutWhilePlay, whenDiscardedEffect: EffectCatalogue.EvilHideoutWhenDiscard) }, // all fire, wild, shadow legends get+3 attack
            { 105, new StageCard(id:105, "Space Time", 2, "Start Turn: Draw 2 extra cards", Elements.Cosmic, Images.SpaceTime, startTurnEffect: EffectCatalogue.SpaceTime) },
            // only cosmic cards can attack cost: 5

            // Nature
            { 1000, new LegendCard(id: 1000, "Briar Rose", 3, Elements.Nature, Images.BriarRose, 6, 5, 9, 0, Weapons.Spear, Weapons.Greatsword, attack1: AC.Spear_Stab, attack2: AC.Any_BlowAKiss) },
            { 1001, new LegendCard(id: 1001, "Forest Guardian", 2, Elements.Nature, Images.ElvenhollowMagyar, 1, 4, 13, 0, Weapons.Hammer, Weapons.Greatsword, attack1: AC.Greatsword_Swing, attack2: AC.Greatsword_String) },
            { 1002, new LegendCard(id: 1002, "Deathcap", 5, Elements.Nature, Images.DeathCap, 10, 5, 5, 0, Weapons.Spear, Weapons.Orb, attack1: AC.Spear_Stab, attack2: AC.Orb_Throw, attack3: AC.DeathCap_Storm) },
            { 1003, new LegendCard(id: 1003, "Rayman", 3, Elements.Nature, Images.Rayman, 7, 1, 7, 0, Weapons.Gauntlets, Weapons.Axe, attack1: AC.Axe_Swing, attack2: AC.Gauntlets_Punch, attack3: AC.Gauntlets_PowerPunch) },
            { 1004, new LegendCard(id: 1004, "Kor", 2, Elements.Nature, Images.Kor, 0, 0, 16, 0, Weapons.Gauntlets, Weapons.Hammer) },
            { 1005, new LegendCard(id: 1005, "Yumiko", 4, Elements.Nature, Images.Yumiko, 3, 3, 15, 0, Weapons.Bow, Weapons.Hammer, attack1: AC.Hammer_Swing, attack2: AC.Hammer_Gimp, attack3:AC.Yumiko_GrabOrbs) },
            { 1006, new LegendCard(id: 1006, "Forest Spirit", 1, Elements.Nature, Images.Forest_Spirit, 3, 2, 1, 0, Weapons.Orb, Weapons.Gauntlets, attack1: AC.ForestSpirit_Explode, attack2: AC.ForestSpirit_Heal) },
            { 1007, new LegendCard(id: 1007, "Demon Bride", 3, Elements.Nature, Images.Demon_Bride, 4, 3, 9, 0, Weapons.Sword, Weapons.Spear, attack1: AC.Sword_Slash, attack2: AC.Any_BlowAKiss) },


            // Fire
            { 2000, new LegendCard(id: 2000, "Iron Lady", 5, Elements.Fire, Images.IronLady, 7, 5, 15, 0, Weapons.RocketLance, Weapons.Scythe, attack1: AC.Scythe_Slash, attack2: AC.Scythe_Gimp, attack3: AC.Lance_Flamethrower, attack4: AC.Artemis_IronLady_MeltDown) },
            { 2001, new LegendCard(id: 2001, "Heatblast", 4, Elements.Fire, Images.Heatblast, 9, 5, 6, 0, Weapons.Blasters, Weapons.Spear, attack1: AC.Spear_Stab, attack2: AC.Heatblast_Burn) },
            { 2002, new LegendCard(id: 2002, "Ulgrim", 5, Elements.Fire, Images.Ulgrim, 5, 5, 16, 0, Weapons.Axe, Weapons.RocketLance, attack1: AC.Axe_Swing, attack2: AC.Lance_Flamethrower, attack3: AC.Any_CraftFireCard, whenPlayedEffect: EffectCatalogue.GenerateAndPlayWorkshop) }, // craft a fire card
            { 2003, new LegendCard(id: 2003, "Molten Kor", 3, Elements.Fire, Images.MoltenKor, 0, 2, 16, 0, Weapons.Gauntlets, Weapons.Hammer, whenPlayedEffect: EffectCatalogue.GenerateAndPlayMustafar)},
            { 2004, new LegendCard(id: 2004, "Jhala", 3, Elements.Fire, Images.Jhala, 13, 1, 1, 0, Weapons.Axe, Weapons.Sword, attack1: AC.Sword_Slash, attack2: AC.Axe_Swing)},
            { 2005, new LegendCard(id: 2005, "Hothead Jiro", 2, Elements.Fire, Images.Hothead_Jiro, 4, 2, 4, 0, Weapons.Sword, Weapons.Scythe, attack1: AC.Sword_Slash, attack2: AC.Scythe_Slash) },


            // Cosmic
            { 3000, new LegendCard(id: 3000, "Artemis", 3, Elements.Cosmic, Images.Artemis, 5, 5, 12, 0, Weapons.RocketLance, Weapons.Scythe, attack1: AC.Scythe_Slash, attack2: AC.Scythe_Gimp, attack3: AC.Lance_Flamethrower) },
            { 3001, new LegendCard(id: 3001, "Orion", 3, Elements.Cosmic, Images.Orion, 4, 6, 12, 0, Weapons.RocketLance, Weapons.Spear, attack1: AC.Spear_Stab, attack2: AC.Lance_Flamethrower) },
            { 3002, new LegendCard(id: 3002, "Wu Shang", 4, Elements.Cosmic, Images.Spyrox_WuShang, 7, 1, 13, 0, Weapons.Gauntlets, Weapons.Spear, attack1: AC.Spear_Stab, attack2: AC.Gauntlets_Punch, attack3: AC.WuShang_DownSig) },
            { 3003, new LegendCard(id: 3003, "Aurora Brynn", 5, Elements.Cosmic, Images.AuroraBrynn, 10, 2, 16, 0, Weapons.Axe, Weapons.Spear, attack1: AC.Axe_Swing, attack2: AC.Spear_Stab) },
            { 3004, new LegendCard(id: 3004, "Astro Commander", 4, Elements.Cosmic, Images.AstralCoreAda, 14, 1, 4, 0, Weapons.Blasters, Weapons.Spear, attack1: AC.Blaster_Shot) },


            // Magic
            { 4000, new LegendCard(id: 4000, "Faerie Queen", 3, Elements.Magic, Images.FaerieQueen, 7, 3, 8, 0, Weapons.Spear, Weapons.Greatsword, attack1: AC.Spear_Stab, attack2: AC.Greatsword_Swing, attack3: AC.Greatsword_String, attack4: AC.Arcadia_PinkRoses) }, // base stance
            { 4001, new LegendCard(id: 4001, "Enchantress", 4, Elements.Magic, Images.Enchantress, 0, 5, 15, 0, Weapons.Scythe, Weapons.Orb, attack1: AC.Enchantress_EnchantHealth, attack2: AC.Enchantress_EnchantPower, attack3: AC.Enchantress_CurseHealth, attack4: AC.Enchantress_CursePower) }, // bugged
            { 4002, new LegendCard(id: 4002, "Dark Mage", 5, Elements.Shadow, Images.DarkMage, 11, 1, 12, 0, Weapons.Scythe, Weapons.Orb, attack1: AC.Scythe_Slash) },
            { 4003, new LegendCard(id: 4003, "Plague Knight", 4, Elements.Magic, Images.PlagueKnight, 1, 5, 14, 0, Weapons.Gauntlets, Weapons.Katars, attack1: AC.PlagueKnight_GrabHealingPotion) }, // healer

            // Shadow
            { 5000, new LegendCard(id: 5000, "Caspian", 2, Elements.Shadow, Images.MasterThief, 4, 5, 4, 0, Weapons.Gauntlets, Weapons.Katars, attack1: AC.Any_BurnForThreeCard, attack2: AC.MasterThief_GrabBomb) },
            { 5001, new LegendCard(id: 5001, "Raymesis", 5, Elements.Shadow, Images.Raymesis, 9, 5, 16, 0, Weapons.Gauntlets, Weapons.Axe, attack1: AC.Axe_Swing) },
            { 5002, new LegendCard(id: 5002, "Mastermind", 4, Elements.Shadow, Images.Loki, 5, 6, 10, 0, Weapons.Scythe, Weapons.Orb, attack1: AC.Scythe_Slash, whenPlayedEffect: EffectCatalogue.GenerateAndPlayMatrix) },
            { 5003, new LegendCard(id: 5003, "Ninja Spirit", 1, Elements.Shadow, Images.Ninja_Spirit, 3, 2, 1, 0, Weapons.Sword, Weapons.Scythe, attack1: AC.NinjaSpirit_PhantomSlash) },
            { 5004, new LegendCard(id: 5004, "Hellshot Hattori", 3, Elements.Shadow, Images.Hellshot_Hattori, 8, 2, 6, 0, Weapons.Sword, Weapons.Blasters, attack1: AC.Sword_Slash, attack2: AC.Blaster_Shot, attack3: AC.Blaster_DoubleShot) },
            { 5005, new LegendCard(id: 5005, "Dullahan Jiro", 3, Elements.Shadow, Images.DullahanJiro, 5, 3, 7, 0, Weapons.Sword, Weapons.Scythe, attack1: AC.Sword_Slash, attack2: AC.Scythe_Slash, attack3: AC.Jiro_SpawnAndPlayNinjaSpirit) },
            { 5006, new LegendCard(id: 5006, "Kitsune Hattori", 5, Elements.Shadow, Images.Kitsune, 15, 3, 10, 0, Weapons.Sword, Weapons.Spear, attack1: AC.Sword_Slash, attack2: AC.Spear_Stab) }, // hide in cloak

            // Wild
            { 6000, new LegendCard(id: 6000, "The Minotaur", 5, Elements.Wild, Images.Teros, 10, 1, 15, 0, Weapons.Axe, Weapons.Hammer, attack1: AC.Axe_Swing, attack2: AC.Hammer_Swing) },
            { 6001, new LegendCard(id: 6001, "Fox Spirit", 2, Elements.Wild, Images.Fox_Spirit, 1, 4, 6, 0, Weapons.Bow, Weapons.Hammer, attack1: AC.Hammer_Swing, attack3:AC.Yumiko_GrabOrbs) },
            { 6002, new LegendCard(id: 6002, "Dander", 1, Elements.Wild, Images.Dander, 6, 0, 2, 0, Weapons.Gauntlets, Weapons.Katars, attack1: AC.Katar_Slash) },


            // Arctic
            { 7000, new LegendCard(id: 7000, "Snowman Kor", 4, Elements.Arctic, Images.SnowmanKor, 0, 2, 20, 0, Weapons.Gauntlets, Weapons.Hammer, attack1: AC.Any_Freeze) },


            // Axe
            { 13000, new WeaponCard(id: 13000, "Axe of Regrowth", 1, Elements.Nature, Images.Axe_of_Regrowth, Weapons.Axe) },
            { 13001, new WeaponCard(id: 13001, "Buzz Axe", 1, Elements.Wild, Images.BuzzAxe, Weapons.Axe) },
            { 13002, new WeaponCard(id: 13002, "Boiling Point", 1, Elements.Fire, Images.BoilingPoint, Weapons.Axe) },
            // Battle Boots (no cards yet, keeping the comment)
            // Blasters
            { 15000, new WeaponCard(id: 15000, "Blazing Fire", 1, Elements.Fire, Images.BlazingFire, Weapons.Blasters) },
            { 15001, new WeaponCard(id: 15001, "Revolvers", 1, Elements.Shadow, Images.Revolvers, Weapons.Blasters) },
            // Bow (no cards yet, keeping the comment)
            { 16000, new WeaponCard(id: 16000, "Sakura Strike", 1, Elements.Nature, Images.Sakura_Strike, Weapons.Bow) },
            // Gauntlets
            { 17000, new WeaponCard(id: 17000, "Sleight of Hand", 1, Elements.Shadow, Images.Sleight_of_Hand, Weapons.Gauntlets) },
            { 17001, new WeaponCard(id: 17001, "Rippers", 1, Elements.Wild, Images.MordexGaunts, Weapons.Gauntlets) },
            // Greatsword
            { 18000, new WeaponCard(id: 18000, "Law of the Land", 1, Elements.Nature, Images.LawOfTheLand, Weapons.Greatsword) },
            // Hammer
            { 19000, new WeaponCard(id: 19000, "Primrose Mallet", 1, Elements.Nature, Images.Primrose_Mallet, Weapons.Hammer) },
            { 19001, new WeaponCard(id: 19001, "Chikara", 1, Elements.Wild, Images.Chikara, Weapons.Hammer) },
            { 19002, new WeaponCard(id: 19002, "Ice Crusher", 1, Elements.Arctic, Images.IceCrusher, Weapons.Hammer) },

            // Katars
            { 20000, new WeaponCard(id: 20000, "Nightmare Claws", 1, Elements.Wild, Images.Nightmare_Claws, Weapons.Katars) },
            // Lance
            { 21000, new WeaponCard(id: 21000, "Galaxy Lance", 1, Elements.Cosmic, Images.GalaxyLance, Weapons.RocketLance) },
            // Orb
            { 22000, new WeaponCard(id: 22000, "Remnant of Fate", 1, Elements.Magic, Images.RemnantOfFate, Weapons.Orb) },
            { 22001, new WeaponCard(id: 22001, "Sacred Relic", 1, Elements.Nature, Images.SacredRelic, Weapons.Orb) },
            { 22002, new WeaponCard(id: 22002, "Scrying Glass", 1, Elements.Cosmic, Images.ScryingGlass, Weapons.Orb) },
            // Scythe
            { 23000, new WeaponCard(id: 23000, "Searing Blade", 1, Elements.Fire, Images.SearingBlade, Weapons.Scythe) },
            { 23001, new WeaponCard(id: 23001, "Shooting Star", 1, Elements.Cosmic, Images.ShootingStar, Weapons.Scythe) },
            { 23002, new WeaponCard(id: 23002, "Starry Scythe", 1, Elements.Cosmic, Images.StarryScythe, Weapons.Scythe) },
            // Spear
            { 24000, new WeaponCard(id: 24000, "Magma Spear", 1, Elements.Fire, Images.MagmaSpear, Weapons.Spear) },
            { 24001, new WeaponCard(id: 24001, "Piercing Regret", 1, Elements.Nature, Images.PiercingRegret, Weapons.Spear) },
            { 24002, new WeaponCard(id: 24002, "Sweet Magi", 1, Elements.Cosmic, Images.SweetMagi, Weapons.Spear) },
            { 24003, new WeaponCard(id: 24003, "Shattering Star", 1, Elements.Cosmic, Images.ShatteringStar, Weapons.Spear) },
            { 24004, new WeaponCard(id: 24004, "Glacier's Grace", 1, Elements.Arctic, Images.GlaciersGrace, Weapons.Spear) },
            { 24005, new WeaponCard(id: 24005, "Hand Saw", 1, Elements.Wild, Images.HandSaw, Weapons.Spear) },
            { 24006, new WeaponCard(id: 24006, "Holy Covenant", 1, Elements.Magic, Images.HolyCovenant, Weapons.Spear) },
            // Sword
            { 25000, new WeaponCard(id: 25000, "Shadow Edge", 1, Elements.Shadow, Images.ShadowEdge, Weapons.Sword) },
            { 25001, new WeaponCard(id: 25001, "Mahou Shoujo", 1, Elements.Cosmic, Images.MahouShoujo, Weapons.Sword) },




            { 500, new BattleCard(id: 500, "Bouncy Bomb", 2, "When Played: Deals direct damage 7", Elements.Shadow, Images.BouncyBomb, oneTimeUse: true, stackable: false, friendlyFire: false, whenPlayedEffect: EffectCatalogue.BattleCardDirectDamageWhenPlayed, damage: 7) },
            { 501, new BattleCard(id: 501, "Vial of Crows", 1, "When Played: Heals legend by 6", Elements.Shadow, Images.Vial_of_Crows, true, false, true, whenPlayedEffect: EffectCatalogue.BattleCardHealWhenPlayed, healthModifier: 6) },
            { 502, new BattleCard(id: 502, "Snowball", 1, "When Played: Deals direct damage 3", Elements.Arctic, Images.Snowball, true, false, false, whenPlayedEffect: EffectCatalogue.BattleCardDirectDamageWhenPlayed, damage: 3) },
            { 503, new BattleCard(id: 503, "Super Saiyan", 5, "When Played: Boosts legend stats +5/+5", Elements.Wild, Images.SuperSaiyan, false, true, true, whenPlayedEffect: EffectCatalogue.BoostHealthAndPower, powerModifier: 5, healthModifier: 5) },
            { 504, new BattleCard(id: 504, "Orb", 0, "When Played: Deals direct damage 1", Elements.Magic, Images.Orb, true, false, false, whenPlayedEffect: EffectCatalogue.BattleCardDirectDamageWhenPlayed, damage: 1) },
            { 505, new BattleCard(id: 505, "Xull's Fury", 3, "When Played: Boosts legend stats +5 Attack", Elements.Fire, Images.XullsFury, false, true, true, whenPlayedEffect: EffectCatalogue.BoostHealthAndPower, powerModifier: 5, healthModifier: 0) },
            { 506, new BattleCard(id: 506, "Chest", 1, "When Played: Legend opens the chest, obtain 3 random cards", Elements.Magic, Images.CardChest, true, false, true, whenPlayedEffect: EffectCatalogue.CardChest) },
            { 507, new BattleCard(id: 507, "<3", 2, "When Played: Boosts legend stats +5 HP", Elements.Magic, Images.Avatar_Heart, true, false, true, whenPlayedEffect: EffectCatalogue.BoostHealthAndPower, powerModifier: 0, healthModifier: 5) },
            { 508, new BattleCard(id: 508, "Fire Nation", 2, "When Played: Boosts all your fire legend stats +2/+2", Elements.Fire, Images.Avatar_Fire, true, false, true, whenPlayedEffect: EffectCatalogue.BoostHealthAndPowerAllYourLegends, powerModifier: 2, healthModifier: 2, targetElements: [Elements.Fire], multiTarget: true) },
            { 509, new BattleCard(id: 509, "Water Tribe", 2, "When Played: Boosts all your Arctic legend stats +2/+2", Elements.Arctic, Images.Avatar_Water, true, false, true, whenPlayedEffect: EffectCatalogue.BoostHealthAndPowerAllYourLegends, powerModifier: 2, healthModifier: 2, targetElements: [Elements.Arctic], multiTarget: true) },
            { 510, new BattleCard(id: 510, "Krabby Patty", 2, "When Played: Fully heals legend", Elements.Arctic, Images.Avatar_Krabby_Patty, true, false, true, whenPlayedEffect: EffectCatalogue.BattleCardHealWhenPlayed, healthModifier: 1000) },
            { 511, new BattleCard(id: 511, "Bubble", 1, "When Played: Tap a card", Elements.Arctic, Images.Bubble, oneTimeUse: true, stackable: false, friendlyFire: false, whenPlayedEffect: EffectCatalogue.Bubble) },
            { 512, new BattleCard(id: 512, "Cosmic Chest", 3, "When Played: Legend opens the chest, obtain three random Cosmic cards", Elements.Cosmic, Images.Cosmic_Chest, true, false, true, whenPlayedEffect: EffectCatalogue.CosmicChest) },
            { 513, new BattleCard(id: 513, "Wild Chest", 3, "When Played: Legend opens the chest, obtain three random Wild cards", Elements.Wild, Images.Wild_Chest, true, false, true, whenPlayedEffect: EffectCatalogue.WildChest) },
            { 514, new BattleCard(id: 514, "Dragon's Chest", 3, "When Played: Legend opens the chest, obtain three random Fire cards", Elements.Fire, Images.Dragon_Chest, true, false, true, whenPlayedEffect: EffectCatalogue.DragonChest) },
            { 515, new BattleCard(id: 515, "Sunken Chest", 3, "When Played: Legend opens the chest, obtain three random Arctic cards", Elements.Arctic, Images.Sunken_Chest, true, false, true, whenPlayedEffect: EffectCatalogue.SunkenChest) },
            { 516, new BattleCard(id: 516, "Forgeborne Chest", 3, "When Played: Legend opens the chest, obtain three random Shadow cards", Elements.Shadow, Images.Shadow_Chest, true, false, true, whenPlayedEffect: EffectCatalogue.ShadowChest) },
            
            // adrenaline image = heartbeat. legend can attack 2 times this turn
            // more elemental chests
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

        public static Card GetCardById(int cardId)
        {
            // Find the card by ID
            var card = CardDictionary.Values.FirstOrDefault(c => c.ID == cardId);

            if (card != null)
            {
                return card.Clone(); // Clone the card to avoid direct modification
            }
            else
            {
                throw new Exception($"Card with ID {cardId} not found.");
            }
        }

        public static Card GetRandomCard()
        {
            var random = new Random();

            // Ensure the dictionary is not empty
            if (CardDictionary.Count == 0)
            {
                throw new Exception("No cards available.");
            }

            // Get a random key from the dictionary
            var randomKey = CardDictionary.Keys.ElementAt(random.Next(CardDictionary.Count));

            return CardDictionary[randomKey].Clone(); // Clone to avoid modifying the original
        }
        public static Card GetRandomElementalCard(Elements element)
        {
            var random = new Random();

            // Filter fire cards from the dictionary
            var elementalCards = CardDictionary.Values.Where(card => card.Element == element).ToList();

            // Select a random fire card
            return elementalCards[random.Next(elementalCards.Count)].Clone(); // Clone to avoid modifying the original
        }




        public static List<Card> CloneList(List<Card> originalList)
        {
            return originalList.Select(card => card.Clone()).ToList(); // Deep clone each card in the list
        }
    }
}
