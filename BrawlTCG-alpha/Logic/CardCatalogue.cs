using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BrawlTCG_alpha.Logic.Cards;
using Images = BrawlTCG_alpha.Properties.Resources;
using ac = BrawlTCG_alpha.Logic.Cards.AttackCatalogue;
using ec = BrawlTCG_alpha.Logic.Cards.EffectCatalogue;
using Microsoft.VisualBasic;


namespace BrawlTCG_alpha.Logic
{
    public class CardCatalogue
    {
        public static Dictionary<int, Card> CardDictionary = new Dictionary<int, Card>()
        {
            // Essence
            { 0, new EssenceCard(id: 0, "Essence", 0, Elements.Magic, Images.Essence, startTurnEffect: ec.Essence) },

            // Stages
            { 100, new StageCard(id:100, "Mustafar", 2, Elements.Fire, Images.Mustafar, startTurnEffect: ec.Mustafar) },
            { 101, new StageCard(id:101, "Fangwild", 3, Elements.Nature, Images.Fangwild, startTurnEffect: ec.Fangwild) },
            { 102, new StageCard(id:102, "Matrix", 1, Elements.Cosmic, Images.Matrix) },
            { 103, new StageCard(id:103, "The Workshop", 2, Elements.Fire, Images.TheWorkshop, startTurnEffect: ec.Workshop_StartTurn, whenDiscardedEffect: ec.Workshop_Discarded) },
            { 104, new StageCard(id:104, "Evil Hideout", 3, Elements.Fire, Images.Evil_Hideout, whenPlayedEffect: ec.EvilHideout_WhenPlayed, whileInPlayEffect: ec.EvilHideout_WhileInPlay, whenDiscardedEffect: ec.EvilHideout_WhenDiscarded) },
            { 105, new StageCard(id:105, "Space Time", 1, Elements.Cosmic, Images.SpaceTime, startTurnEffect: ec.SpaceTime) },
            { 106, new StageCard(id:106, "Atlantis", 3, Elements.Arctic, Images.Atlantis, startTurnEffect: ec.Atlantis_StartTurn, whenPlayedEffect: ec.Atlantis_WhenPlayed, whileInPlayEffect: ec.Atlantis_WhileInPlay, whenDiscardedEffect: ec.Atlantis_WhenDiscarded)  },
            { 107, new StageCard(id:107, "Silent Galaxy", 5, Elements.Cosmic, Images.SilentGalaxy, startTurnEffect: ec.SilentGalaxy_StartTurn, whenPlayedEffect: ec.SilentGalaxy_WhenPlayed, whileInPlayEffect: ec.SilentGalaxy_WhilePlay, whenDiscardedEffect: ec.SilentGalaxy_WhenDiscarded) },
            
            
            // Nature
            { 1000, new LegendCard(id: 1000, "Briar Rose", 3, Elements.Nature, Images.BriarRose, 6, 2, 9, 0, Weapons.Spear, Weapons.Greatsword, attack1: ac.Spear_Stab, attack2: ac.Any_BlowAKiss) },
            { 1001, new LegendCard(id: 1001, "Forest Guardian", 2, Elements.Nature, Images.ElvenhollowMagyar, 1, 4, 13, 0, Weapons.Hammer, Weapons.Greatsword, attack1: ac.Greatsword_Swing, attack2: ac.Greatsword_String) },
            { 1002, new LegendCard(id: 1002, "Deathcap", 5, Elements.Nature, Images.DeathCap, 10, 5, 5, 0, Weapons.Spear, Weapons.Orb, attack1: ac.Spear_Stab, attack2: ac.Orb_Throw, attack3: ac.DeathCap_Storm) },
            { 1003, new LegendCard(id: 1003, "Rayman", 3, Elements.Nature, Images.Rayman, 7, 1, 7, 0, Weapons.Gauntlets, Weapons.Axe, attack1: ac.Axe_Swing, attack2: ac.Gauntlets_Punch, attack3: ac.Gauntlets_PowerPunch) },
            { 1004, new LegendCard(id: 1004, "Kor", 2, Elements.Nature, Images.Kor, 0, 0, 16, 0, Weapons.Gauntlets, Weapons.Hammer) },
            { 1005, new LegendCard(id: 1005, "Yumiko", 4, Elements.Nature, Images.Yumiko, 3, 3, 15, 0, Weapons.Bow, Weapons.Hammer, attack1: ac.Hammer_Swing, attack2: ac.Hammer_Gimp, attack3:ac.Yumiko_GrabOrbs) },
            { 1006, new LegendCard(id: 1006, "Forest Spirit", 1, Elements.Nature, Images.Forest_Spirit, 3, 2, 1, 0, Weapons.Orb, Weapons.Gauntlets, attack1: ac.ForestSpirit_Explode, attack2: ac.ForestSpirit_Heal) },
            { 1007, new LegendCard(id: 1007, "Demon Bride", 3, Elements.Nature, Images.Demon_Bride, 4, 3, 9, 0, Weapons.Sword, Weapons.Spear, attack1: ac.Sword_Slash, attack2: ac.Any_BlowAKiss) },
            { 1008, new LegendCard(id: 1008, "Faerie Queen", 3, Elements.Nature, Images.RealQueen, 1, 3, 7, 0, Weapons.Spear, Weapons.Greatsword, attack1: ac.Spear_Stab, attack2: ac.Any_BlowAKiss, whileInPlayEffect: ec.BoostAllNatureLegendsStats_WhileInPlay) },
            // Fire
            { 2000, new LegendCard(id: 2000, "Iron Lady", 5, Elements.Fire, Images.IronLady, 7, 5, 15, 0, Weapons.RocketLance, Weapons.Scythe, attack1: ac.Scythe_Slash, attack2: ac.Scythe_Gimp, attack3: ac.Lance_Flamethrower, attack4: ac.Artemis_IronLady_MeltDown) },
            { 2001, new LegendCard(id: 2001, "Heatblast", 4, Elements.Fire, Images.Heatblast, 9, 5, 6, 0, Weapons.Blasters, Weapons.Spear, attack1: ac.Spear_Stab, attack2: ac.Heatblast_Burn) },
            { 2002, new LegendCard(id: 2002, "Ulgrim", 5, Elements.Fire, Images.Ulgrim, 5, 5, 16, 0, Weapons.Axe, Weapons.RocketLance, attack1: ac.Axe_Swing, attack2: ac.Lance_Flamethrower, attack3: ac.Any_CraftFireCard, whenPlayedEffect: ec.GenerateAndPlayWorkshop) },
            { 2003, new LegendCard(id: 2003, "Molten Kor", 3, Elements.Fire, Images.MoltenKor, 0, 2, 16, 0, Weapons.Gauntlets, Weapons.Hammer, whenPlayedEffect: ec.GenerateAndPlayMustafar)},
            { 2004, new LegendCard(id: 2004, "Jhala", 3, Elements.Fire, Images.Jhala, 11, 1, 1, 0, Weapons.Axe, Weapons.Sword, attack1: ac.Sword_Slash, attack2: ac.Axe_Swing)},
            { 2005, new LegendCard(id: 2005, "Hothead Jiro", 2, Elements.Fire, Images.Hothead_Jiro, 4, 2, 4, 0, Weapons.Sword, Weapons.Scythe, attack1: ac.Sword_Slash, attack2: ac.Scythe_Slash) },
            { 2006, new LegendCard(id: 2006, "Fury Shang", 3, Elements.Fire, Images.FuryShang, 6, 3, 6, 0, Weapons.Gauntlets, Weapons.Spear, attack1: ac.Spear_Stab, attack2: ac.Gauntlets_Punch, whileInPlayEffect: ec.BoostFireLegendStats_WhileInPlayEffect) },
            { 2007, new LegendCard(id: 2007, "Seven", 2, Elements.Fire, Images.BrawlLogo, 4, 2, 4, 0, Weapons.Spear, Weapons.Cannon, attack1: ac.Cannon_Blast, attack2: ac.Seven_CraftWeapons) }, // new card
            // Cosmic
            { 3000, new LegendCard(id: 3000, "Artemis", 3, Elements.Cosmic, Images.Artemis, 5, 5, 12, 0, Weapons.RocketLance, Weapons.Scythe, attack1: ac.Scythe_Slash, attack2: ac.Scythe_Gimp, attack3: ac.Lance_Flamethrower) },
            { 3001, new LegendCard(id: 3001, "Orion", 3, Elements.Cosmic, Images.Orion, 4, 6, 12, 0, Weapons.RocketLance, Weapons.Spear, attack1: ac.Spear_Stab, attack2: ac.Lance_Flamethrower) },
            { 3002, new LegendCard(id: 3002, "Wu Shang", 4, Elements.Cosmic, Images.Spyrox_WuShang, 7, 1, 13, 0, Weapons.Gauntlets, Weapons.Spear, attack1: ac.Spear_Stab, attack2: ac.Gauntlets_Punch, attack3: ac.WuShang_DownSig) },
            { 3003, new LegendCard(id: 3003, "Aurora Brynn", 5, Elements.Cosmic, Images.AuroraBrynn, 10, 2, 16, 0, Weapons.Axe, Weapons.Spear, attack1: ac.Axe_Swing, attack2: ac.Spear_Stab) },
            { 3004, new LegendCard(id: 3004, "Astro Commander", 4, Elements.Cosmic, Images.AstralCoreAda, 14, 1, 3, 0, Weapons.Blasters, Weapons.Spear, attack1: ac.Blaster_Shot) },
            { 3005, new LegendCard(id: 3005, "Breaker Shang", 2, Elements.Cosmic, Images.Breaker_Shang, 7, 1, 13, 0, Weapons.Gauntlets, Weapons.Spear, attack1: ac.Spear_Stab, attack2: ac.Gauntlets_Punch, attack3: ac.Any_CraftCosmicCard) },
            { 3006, new LegendCard(id: 3006, "Witch Scarlet", 2, Elements.Cosmic, Images.WitchScarlet, 1, 3, 6, 0, Weapons.Hammer, Weapons.RocketLance, attack1: ac.Hammer_Swing, attack2: ac.Lance_Poke, attack3: ac.Any_GenerateAndPlayMatrix) },
            // Magic
            { 4000, new LegendCard(id: 4000, "Arcadia", 3, Elements.Magic, Images.FaerieQueen, 6, 3, 8, 0, Weapons.Spear, Weapons.Greatsword, attack1: ac.Spear_Stab, attack2: ac.Greatsword_Swing, attack3: ac.Greatsword_String, attack4: ac.Arcadia_PinkRoses) },
            { 4001, new LegendCard(id: 4001, "Enchantress", 4, Elements.Magic, Images.Enchantress, 0, 5, 15, 0, Weapons.Scythe, Weapons.Orb, attack1: ac.Enchantress_EnchantHealth, attack2: ac.Enchantress_EnchantPower, attack3: ac.Enchantress_CurseHealth, attack4: ac.Enchantress_CursePower) },
            // old dark mage card don't use 4002
            { 4003, new LegendCard(id: 4003, "Plague Knight", 4, Elements.Magic, Images.PlagueKnight, 1, 2, 14, 0, Weapons.Gauntlets, Weapons.Katars, attack1: ac.PlagueKnight_GrabHealingPotion, attack2: ac.Any_CraftBattleCard) },
            { 4004, new LegendCard(id: 4004, "Fait", 3, Elements.Magic, Images.Fait, 6, 3, 6, 0, Weapons.Scythe, Weapons.Orb, attack1: ac.Scythe_Slash, attack2: ac.Orb_Swing, attack3: ac.Any_BurnForThreeCard) },
            // Shadow
            { 4002, new LegendCard(id: 4002, "Dark Mage", 5, Elements.Shadow, Images.DarkMage, 11, 1, 12, 0, Weapons.Scythe, Weapons.Orb, attack1: ac.Scythe_Slash) },
            { 5000, new LegendCard(id: 5000, "Caspian", 2, Elements.Shadow, Images.MasterThief, 4, 5, 4, 0, Weapons.Gauntlets, Weapons.Katars, attack1: ac.Any_BurnForThreeCard, attack2: ac.MasterThief_GrabBomb) },
            { 5001, new LegendCard(id: 5001, "Raymesis", 5, Elements.Shadow, Images.Raymesis, 9, 5, 16, 0, Weapons.Gauntlets, Weapons.Axe, attack1: ac.Axe_Swing) },
            { 5002, new LegendCard(id: 5002, "Mastermind", 4, Elements.Shadow, Images.Loki, 5, 6, 10, 0, Weapons.Scythe, Weapons.Orb, attack1: ac.Scythe_Slash, whenPlayedEffect: ec.GenerateAndPlayMatrix) },
            { 5003, new LegendCard(id: 5003, "Ninja Spirit", 1, Elements.Shadow, Images.Ninja_Spirit, 3, 2, 1, 0, Weapons.Sword, Weapons.Scythe, attack1: ac.NinjaSpirit_PhantomSlash) },
            { 5004, new LegendCard(id: 5004, "Hellshot Hattori", 3, Elements.Shadow, Images.Hellshot_Hattori, 8, 2, 6, 0, Weapons.Sword, Weapons.Blasters, attack1: ac.Sword_Slash, attack2: ac.Blaster_Shot, attack3: ac.Blaster_DoubleShot) },
            { 5005, new LegendCard(id: 5005, "Dullahan Jiro", 3, Elements.Shadow, Images.DullahanJiro, 5, 3, 7, 0, Weapons.Sword, Weapons.Scythe, attack1: ac.Sword_Slash, attack2: ac.Scythe_Slash, attack3: ac.Jiro_SpawnAndPlayNinjaSpirit) },
            { 5006, new LegendCard(id: 5006, "Kitsune Hattori", 6, Elements.Shadow, Images.Kitsune, 15, 2, 13, 0, Weapons.Sword, Weapons.Spear, attack1: ac.Sword_Slash, attack2: ac.Spear_Stab) },
            // Wild
            { 6000, new LegendCard(id: 6000, "The Minotaur", 5, Elements.Wild, Images.Teros, 10, 1, 15, 0, Weapons.Axe, Weapons.Hammer, attack1: ac.Axe_Swing, attack2: ac.Hammer_Swing) },
            { 6001, new LegendCard(id: 6001, "Fox Spirit", 2, Elements.Wild, Images.Fox_Spirit, 1, 4, 6, 0, Weapons.Bow, Weapons.Hammer, attack1: ac.Hammer_Swing, attack3:ac.Yumiko_GrabOrbs) },
            { 6002, new LegendCard(id: 6002, "Dander", 1, Elements.Wild, Images.Dander, 6, 0, 2, 0, Weapons.Gauntlets, Weapons.Katars, attack1: ac.Katar_Slash) },
            // Arctic
            { 7000, new LegendCard(id: 7000, "Snowman Kor", 4, Elements.Arctic, Images.SnowmanKor, 0, 2, 16, 0, Weapons.Gauntlets, Weapons.Hammer, attack1: ac.Any_Freeze) },
            { 7001, new LegendCard(id: 7001, "Atlantean Ada", 2, Elements.Arctic, Images.Atlantean_Ada, 2, 3, 5, 0, Weapons.Blasters, Weapons.Spear, attack1: ac.Blaster_Shot, attack2: ac.Spear_Stab, attack3: ac.Ada_SpawnAndPlayAtlantis) },


            // Axe
            { 13000, new WeaponCard(id: 13000, "Axe of Regrowth", 1, Elements.Nature, Images.Axe_of_Regrowth, Weapons.Axe) },
            { 13001, new WeaponCard(id: 13001, "Buzz Axe", 1, Elements.Wild, Images.BuzzAxe, Weapons.Axe) },
            { 13002, new WeaponCard(id: 13002, "Boiling Point", 1, Elements.Fire, Images.BoilingPoint, Weapons.Axe) },
            // Battle Boots
            { 14000, new WeaponCard(id: 14000, "Battle Boots", 1, Elements.Wild, Images.BrawlLogo, Weapons.BattleBoots) }, // new card
            // Blasters
            { 15000, new WeaponCard(id: 15000, "Blazing Fire", 1, Elements.Fire, Images.BlazingFire, Weapons.Blasters) },
            { 15001, new WeaponCard(id: 15001, "Revolvers", 1, Elements.Shadow, Images.Revolvers, Weapons.Blasters) },
            { 15002, new WeaponCard(id: 15002, "Aqua Blasters", 1, Elements.Arctic, Images.Aqua_Blasters, Weapons.Blasters) },
            { 15003, new WeaponCard(id: 15003, "Splish Splash", 1, Elements.Arctic, Images.Splish_Splash, Weapons.Blasters) },
            // Bow
            { 16000, new WeaponCard(id: 16000, "Sakura Strike", 1, Elements.Nature, Images.Sakura_Strike, Weapons.Bow) },
            // Canon
            { 26000, new WeaponCard(id: 26000, "Canon", 1, Elements.Fire, Images.BrawlLogo, Weapons.Cannon) }, // new card
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
            { 20001, new WeaponCard(id: 20001, "Winter Daggers", 1, Elements.Arctic, Images.Winter_Daggers, Weapons.Katars) },
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
            { 23003, new WeaponCard(id: 23003, "Candlelit Scythe", 1, Elements.Fire, Images.Candlelit_Scythe, Weapons.Scythe) },
            { 23004, new WeaponCard(id: 23004, "Amethyst Scythe", 1, Elements.Magic, Images.Constellation_Carver, Weapons.Scythe) },
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
            { 25002, new WeaponCard(id: 25002, "Frozen Edge", 1, Elements.Arctic, Images.FrozenEdge, Weapons.Sword) },
            { 25003, new WeaponCard(id: 25003, "Ancestor's Flame", 1, Elements.Fire, Images.Ancestors_Flame, Weapons.Sword) },




            { 500, new BattleCard(id: 500, "Bouncy Bomb", 3, "When Played: Deals direct damage 7", Elements.Shadow, Images.BouncyBomb, oneTimeUse: true, stackable: false, friendlyFire: false, whenPlayedEffect: ec.BattleCardDirectDamageWhenPlayed, damage: 7) },
            { 501, new BattleCard(id: 501, "Vial of Crows", 1, "When Played: Heals legend by 6", Elements.Shadow, Images.Vial_of_Crows, true, false, true, whenPlayedEffect: ec.BattleCardHealWhenPlayed, healthModifier: 6) },
            { 502, new BattleCard(id: 502, "Snowball", 2, "When Played: Deals direct damage 3", Elements.Arctic, Images.Snowball, true, false, false, whenPlayedEffect: ec.BattleCardDirectDamageWhenPlayed, damage: 3) },
            { 503, new BattleCard(id: 503, "Super Saiyan", 6, "When Played: Boosts legend stats +5/+5", Elements.Wild, Images.SuperSaiyan, false, true, true, whenPlayedEffect: ec.BoostHealthAndPower, powerModifier: 5, healthModifier: 5) },
            { 504, new BattleCard(id: 504, "Orb", 0, "When Played: Deals direct damage 1", Elements.Magic, Images.Orb, true, false, false, whenPlayedEffect: ec.BattleCardDirectDamageWhenPlayed, damage: 1) },
            { 505, new BattleCard(id: 505, "Xull's Fury", 3, "When Played: Boosts legend stats +5 Attack", Elements.Fire, Images.XullsFury, false, true, true, whenPlayedEffect: ec.BoostHealthAndPower, powerModifier: 5, healthModifier: 0) },
            { 506, new BattleCard(id: 506, "Chest", 1, "When Played: Obtain 3 random cards", Elements.Magic, Images.CardChest, true, false, true, whenPlayedEffect: ec.CardChest, targetRequired: false) },
            { 507, new BattleCard(id: 507, "<3", 2, "When Played: Boosts legend stats +5 HP", Elements.Magic, Images.Avatar_Heart, true, false, true, whenPlayedEffect: ec.BoostHealthAndPower, powerModifier: 0, healthModifier: 5) },
            { 508, new BattleCard(id: 508, "Fire Nation", 2, "When Played: Boosts all your fire legend stats +2/+2", Elements.Fire, Images.Avatar_Fire, true, false, true, whenPlayedEffect: ec.BoostHealthAndPowerAllYourLegends, powerModifier: 2, healthModifier: 2, targetElements: [Elements.Fire], multiTarget: true) },
            { 509, new BattleCard(id: 509, "Water Tribe", 2, "When Played: Boosts all your Arctic legend stats +2/+2", Elements.Arctic, Images.Avatar_Water, true, false, true, whenPlayedEffect: ec.BoostHealthAndPowerAllYourLegends, powerModifier: 2, healthModifier: 2, targetElements: [Elements.Arctic], multiTarget: true) },
            { 510, new BattleCard(id: 510, "Krabby Patty", 2, "When Played: Fully heals legend", Elements.Arctic, Images.Avatar_Krabby_Patty, true, false, true, whenPlayedEffect: ec.BattleCardHealWhenPlayed, healthModifier: 1000) },
            { 511, new BattleCard(id: 511, "Bubble", 1, "When Played: Tap a card", Elements.Arctic, Images.Bubble, oneTimeUse: true, stackable: false, friendlyFire: false, whenPlayedEffect: ec.Bubble) },
            { 512, new BattleCard(id: 512, "Cosmic Chest", 3, "When Played: Obtain three random Cosmic cards", Elements.Cosmic, Images.Cosmic_Chest, true, false, true, whenPlayedEffect: ec.CosmicChest, targetRequired: false) },
            { 513, new BattleCard(id: 513, "Wild Chest", 3, "When Played: Obtain three random Wild cards", Elements.Wild, Images.Wild_Chest, true, false, true, whenPlayedEffect: ec.WildChest, targetRequired: false) },
            { 514, new BattleCard(id: 514, "Dragon's Chest", 3, "When Played: Obtain three random Fire cards", Elements.Fire, Images.Dragon_Chest, true, false, true, whenPlayedEffect: ec.DragonChest, targetRequired: false) },
            { 515, new BattleCard(id: 515, "Sunken Chest", 3, "When Played: Obtain three random Arctic cards", Elements.Arctic, Images.Sunken_Chest, true, false, true, whenPlayedEffect: ec.SunkenChest, targetRequired: false) },
            { 516, new BattleCard(id: 516, "Forgeborne Chest", 3, "When Played: Obtain three random Shadow cards", Elements.Shadow, Images.Shadow_Chest, true, false, true, whenPlayedEffect: ec.ShadowChest, targetRequired: false) },
            { 517, new BattleCard(id: 517, "Dark Duo", 4, "When Played: Legend Obtain 2 Shadow legends", Elements.Shadow, Images.DarkDuo, true, false, true, whenPlayedEffect: ec.DarkDuo, targetRequired: false) },
            { 518, new BattleCard(id: 518, "Witch Party", 6, "When Played: Obtain Fait, Witch Scarlet, Amethyst Scythe and Galaxy Lance", Elements.Magic, Images.WitchParty, true, false, true, whenPlayedEffect: ec.WitchParty, targetRequired: false) },
            { 519, new BattleCard(id: 519, "Promotion I", 5, "When Played: Player Max health goes up by 5", Elements.Magic, Images.Avatar_CollectorsPackI, true, false, false, whenPlayedEffect: ec.PromotionI, targetRequired: false) },
            { 520, new BattleCard(id: 520, "Promotion II", 10, "When Played: Player Max health goes up by 10", Elements.Magic, Images.Avatar_CollectorsPack, true, false, false, whenPlayedEffect: ec.PromotionII, targetRequired: false) },
            { 521, new BattleCard(id: 521, "Adrenaline", 2, "When Played: Legend can attack again", Elements.Nature, Images.BrawlLogo, true, false, false, whenPlayedEffect: ec.Adrenaline) }, // new card
            { 522, new BattleCard(id: 522, "Death's Hour", 7, "When Played: Every legend's HP becomes 1", Elements.Shadow, Images.BrawlLogo, true, false, false, whenPlayedEffect: ec.DeathsHour, targetRequired: false) }, // new card
            { 523, new BattleCard(id: 523, "Cursed Kunai", 3, "When Played: Obtain a random Katar Legend and random Katars", Elements.Shadow, Images.BrawlLogo, true, false, false, whenPlayedEffect: ec.CursedKunai, targetRequired: false) }, // new card
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

        public static Card GetRandomWeaponCard()
        {
            var random = new Random();

            // Get all values from the Weapons enum, excluding 'Any'
            Array weaponValues = Enum.GetValues(typeof(Weapons))
                                      .Cast<Weapons>()
                                      .Where(weapon => weapon != Weapons.Any) // Exclude 'Any'
                                      .ToArray();

            // Select a random weapon from the filtered values
            Weapons randomWeapon = (Weapons)weaponValues.GetValue(random.Next(weaponValues.Length));

            // Filter the dictionary to only include WeaponCard objects and match the randomly selected weapon
            List<WeaponCard> weaponCards = CardDictionary.Values
                .OfType<WeaponCard>()  // Ensure we're working with WeaponCard specifically
                .Where(card => card.Weapon == randomWeapon) // Assuming Weapon is a property of WeaponCard
                .ToList();

            // Ensure there are cards for the given weapon
            if (weaponCards.Count == 0)
            {
                throw new Exception($"No cards available for weapon type {randomWeapon}.");
            }

            // Select a random card and return a clone
            return weaponCards[random.Next(weaponCards.Count)].Clone();
        }

        public static Card GetRandomWeaponCard(Weapons weapon)
        {
            var random = new Random();

            // Filter the dictionary to only include WeaponCard objects and match the randomly selected weapon
            List<WeaponCard> weaponCards = CardDictionary.Values
                .OfType<WeaponCard>()  // Ensure we're working with WeaponCard specifically
                .Where(card => card.Weapon == weapon) // Assuming Weapon is a property of WeaponCard
                .ToList();

            // Ensure there are cards for the given weapon
            if (weaponCards.Count == 0)
            {
                throw new Exception($"No cards available for weapon type {weapon}.");
            }

            // Select a random card and return a clone
            return weaponCards[random.Next(weaponCards.Count)].Clone();
        }

        public static Card GetRandomCard(Elements? element = null, Type cardType = null)
        {
            var random = new Random();

            // Filter cards by element (if specified) and optionally by card type
            var filteredCards = CardDictionary.Values
                .Where(card => (!element.HasValue || card.Element == element.Value) &&
                               (cardType == null || card.GetType() == cardType))
                .ToList();

            // Ensure there's at least one valid card
            if (filteredCards.Count == 0)
            {
                throw new InvalidOperationException($"No matching cards found for element '{element?.ToString() ?? "Any"}' and type '{cardType?.Name ?? "Any"}'.");
            }

            // Select a random card and clone it to avoid modifying the original
            return filteredCards[random.Next(filteredCards.Count)].Clone();
        }

        public static LegendCard GetRandomLegendCard(Weapons weapon)
        {
            var random = new Random();

            // Filter the LegendCards in the dictionary (assuming LegendCard is in CardDictionary)
            List<LegendCard> legendCards = CardDictionary.Values
                .OfType<LegendCard>()
                .Where(legend => legend.PrimaryWeapon == weapon || legend.SecondaryWeapon == weapon) // Ensure the legend has the specified weapon
                .ToList();

            // Ensure we have legends that match the specified weapon
            if (legendCards.Count == 0)
            {
                throw new Exception($"No legends available with the weapon {weapon}.");
            }

            // Select a random legend that matches the specified weapon
            LegendCard randomLegend = legendCards[random.Next(legendCards.Count)];

            // Return the selected legend
            return randomLegend;
        }






        public static List<Card> CloneList(List<Card> originalList)
        {
            return originalList.Select(card => card.Clone()).ToList(); // Deep clone each card in the list
        }
    }
}
