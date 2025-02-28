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
        public static Dictionary<int, Card> CardDictionary = new Dictionary<int, Card>()
        {
            // Essence
            { 0, new EssenceCard(id: 0, "Essence", 0, Elements.Magic, Images.Essence) },

            // Stages
            { 100, new StageCard(id:100, "Mustafar", 2, "Start Turn: Every non-Fire Legend will lose 1 HP", Elements.Fire, Images.Mustafar, startTurnEffect: EffectCatalogue.MustafarEffect) },
            { 101, new StageCard(id:101, "Fangwild", 3, "Start Turn: Magic and Nature Legends will be healed by 2", Elements.Nature, Images.Fangwild, startTurnEffect: EffectCatalogue.FangwildEffect) },
            { 102, new StageCard(id:102, "Matrix", 1, "Nothing", Elements.Cosmic, Images.Matrix) },

            // Nature
            //CardCatalogue.BriarRose.Clone(),
            //CardCatalogue.ForestGuardian.Clone(),
            //CardCatalogue.Deathcap.Clone(),
            // Nature
            { 1000, new LegendCard(id: 1000, "Briar Rose", 3, Elements.Nature, Images.BriarRose, 6, 7, 9, 0, Weapons.Spear, Weapons.Greatsword, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Any_Seduce) }, // speed stance 
            { 1001, new LegendCard(id: 1001, "Forest Guardian", 2, Elements.Nature, Images.ElvenhollowMagyar, 1, 4, 15, 0, Weapons.Hammer, Weapons.Greatsword, attack1: AttackCatalogue.Greatsword_Swing, attack2: AttackCatalogue.Greatsword_String) },
            { 1002, new LegendCard(id: 1002, "Deathcap", 5, Elements.Nature, Images.DeathCap, 10, 10, 5, 0, Weapons.Spear, Weapons.Orb, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Orb_Throw, attack3: AttackCatalogue.DeathCap_Storm) },
            { 1003, new LegendCard(id: 1003, "Rayman", 5, Elements.Nature, Images.Rayman, 7, 1, 7, 0, Weapons.Gauntlets, Weapons.Axe, attack1: AttackCatalogue.Axe_Swing, attack2: AttackCatalogue.Gauntlets_Punch, attack3: AttackCatalogue.Gauntlets_PowerPunch) },

            // Fire
            { 2000, new LegendCard(id: 2000, "Iron Lady", 5, Elements.Fire, Images.IronLady, 7, 5, 15, 0, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash, attack2: AttackCatalogue.Scythe_Gimp, attack3: AttackCatalogue.Lance_Flamethrower, attack4: AttackCatalogue.Artemis_IronLady_MeltDown) },
            { 2001, new LegendCard(id: 2001, "Heatblast", 4, Elements.Fire, Images.Heatblast, 9, 5, 6, 0, Weapons.Blasters, Weapons.Spear, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Heatblast_Burn) },

            // Cosmic
            { 3000, new LegendCard(id: 3000, "Artemis", 3, Elements.Cosmic, Images.Artemis, 5, 5, 12, 0, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash, attack2: AttackCatalogue.Scythe_Gimp, attack3: AttackCatalogue.Lance_Flamethrower) },
            { 3001, new LegendCard(id: 3001, "Orion", 3, Elements.Cosmic, Images.Orion, 4, 6, 12, 0, Weapons.RocketLance, Weapons.Spear, attack1: AttackCatalogue.Spear_Stab, attack3: AttackCatalogue.Lance_Flamethrower) },

            // Magic
            { 4000, new LegendCard(id: 4000, "Faerie Queen", 3, Elements.Magic, Images.FaerieQueen, 7, 3, 8, 0, Weapons.Spear, Weapons.Greatsword, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Greatsword_Swing, attack3: AttackCatalogue.Greatsword_String, attack4: AttackCatalogue.Arcadia_PinkRoses) }, // base stance
            { 4001, new LegendCard(id: 4001, "Enchantress", 4, Elements.Magic, Images.Enchantress, 0, 5, 18, 0, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Enchantress_EnchantHealth, attack2: AttackCatalogue.Enchantress_EnchantPower, attack3: AttackCatalogue.Enchantress_CurseHealth, attack4: AttackCatalogue.Enchantress_CursePower) },
            { 4002, new LegendCard(id: 4002, "Dark Mage", 5, Elements.Magic, Images.DarkMage, 11, 1, 12, 0, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Scythe_Slash) },
            { 4003, new LegendCard(id: 4003, "Plague Knight", 4, Elements.Magic, Images.PlagueKnight, 1, 5, 14, 0, Weapons.Gauntlets, Weapons.Katars, attack1: AttackCatalogue.PlagueKnight_GrabHealingPotion) }, // healer

            // Shadow
            { 5000, new LegendCard(id: 5000, "Master Thief", 2, Elements.Shadow, Images.MasterThief, 4, 5, 4, 0, Weapons.Gauntlets, Weapons.Katars, attack1: AttackCatalogue.Any_BurnForCard, attack2: AttackCatalogue.MasterThief_GrabBomb) },
            { 5001, new LegendCard(id: 5001, "Raymesis", 5, Elements.Shadow, Images.Raymesis, 9, 5, 16, 0, Weapons.Gauntlets, Weapons.Axe, attack1: AttackCatalogue.Axe_Swing) },
            { 5002, new LegendCard(id: 5002, "Mastermind", 4, Elements.Shadow, Images.Loki, 5, 6, 10, 0, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Scythe_Slash, whenPlayedEffect: EffectCatalogue.GenerateMatrix) },

            // Wild
            { 6000, new LegendCard(id: 6000, "The Minotaur", 5, Elements.Wild, Images.Teros, 10, 1, 15, 0, Weapons.Axe, Weapons.Hammer, attack1: AttackCatalogue.Axe_Swing, attack2: AttackCatalogue.Hammer_Swing) },

            // Axe
            { 13000, new WeaponCard(id: 13000, "Axe of Regrowth", 1, Elements.Nature, Images.Axe_of_Regrowth, Weapons.Axe) },
            // Battle Boots (no cards yet, keeping the comment)
            // Blasters
            { 15000, new WeaponCard(id: 15000, "Blazing Fire", 1, Elements.Fire, Images.BlazingFire, Weapons.Blasters) },
            // Bow (no cards yet, keeping the comment)
            // Gauntlets
            { 17000, new WeaponCard(id: 17000, "Sleight of Hand", 1, Elements.Shadow, Images.Sleight_of_Hand, Weapons.Gauntlets) },
            // Greatsword
            { 18000, new WeaponCard(id: 18000, "Law of the Land", 1, Elements.Nature, Images.LawOfTheLand, Weapons.Greatsword) },
            // Hammer
            { 19000, new WeaponCard(id: 19000, "Primrose Mallet", 1, Elements.Nature, Images.Primrose_Mallet, Weapons.Hammer) },
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
            // Sword (no cards yet, keeping the comment)

        
            { 500, new BattleCard(id: 500, "Bouncy Bomb", 2, "When Played: Deals direct damage", Elements.Shadow, Images.BouncyBomb, oneTimeUse: true, stackable: false, friendlyFire: false, whenPlayedEffect: EffectCatalogue.DirectDamage, damage: 8) },
            { 501, new BattleCard(id: 501, "Vial of Crows", 1, "When Played: Boosts legend stats", Elements.Shadow, Images.Vial_of_Crows, true, false, true, whenPlayedEffect: EffectCatalogue.Heal, healthModifier: 5) },
            { 502, new BattleCard(id: 502, "Snowball", 1, "When Played: Deals direct damage", Elements.Arctic, Images.Snowball, true, false, false, whenPlayedEffect: EffectCatalogue.DirectDamage, damage: 4) },
            { 503, new BattleCard(id: 503, "Super Saiyan", 5, "When Played: Boosts legend stats", Elements.Wild, Images.SuperSaiyan, false, true, true, whenPlayedEffect: EffectCatalogue.BoostStats, powerModifier: 5, healthModifier: 5) },

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


        public static List<Card> CloneList(List<Card> originalList)
        {
            return originalList.Select(card => card.Clone()).ToList(); // Deep clone each card in the list
        }
    }
}
