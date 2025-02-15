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

        // Essence
        public static EssenceCard Essence = new EssenceCard("Essence", 0, Elements.Magic, Images.Essence);

        // Stages
        public static StageCard Mustafar = new StageCard("Mustafar", 2, "Start Turn: Every non-Fire Legend will lose 1 HP", Elements.Fire, Images.Mustafar, startTurnEffect: EffectCatalogue.MustafarEffect);
        public static StageCard Fangwild = new StageCard("Fangwild", 3, "Start Turn: Magic and Nature Legends will be healed by 2", Elements.Nature, Images.Fangwild, startTurnEffect: EffectCatalogue.FangwildEffect);

        // ALL LEGEND CARDS - every 5 is a cost ish
        // Fire
        public static LegendCard IronLady = new LegendCard("Iron Lady", 5, Elements.Fire, Images.IronLady, 7, 5, 15, 0, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash, attack2: AttackCatalogue.Scythe_Gimp, attack3: AttackCatalogue.Lance_Flamethrower, attack4: AttackCatalogue.Artemis_IronLady_MeltDown); // Base Stance + Imaginary stats
        public static LegendCard Heatblast = new LegendCard("Heatblast", 4, Elements.Fire, Images.Heatblast, 9, 5, 6, 0, Weapons.Blasters, Weapons.Spear, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Heatblast_Burn);
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
        public static LegendCard Artemis = new LegendCard("Artemis", 3, Elements.Cosmic, Images.Artemis, 5, 5, 12, 0, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash, attack2: AttackCatalogue.Scythe_Gimp, attack3: AttackCatalogue.Lance_Flamethrower);
        public static LegendCard Orion = new LegendCard("Orion", 3, Elements.Cosmic, Images.Orion, 4, 6, 12, 0, Weapons.RocketLance, Weapons.Spear, attack1: AttackCatalogue.Spear_Stab, attack3: AttackCatalogue.Lance_Flamethrower);
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
        public static LegendCard BriarRose = new LegendCard("Briar Rose", 3, Elements.Nature, Images.BriarRose, 6, 7, 9, 0, Weapons.Spear, Weapons.Greatsword, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Any_Seduce); // speed stance
        public static LegendCard ForestGuardian = new LegendCard("Forest Guardian", 2, Elements.Nature, Images.ElvenhollowMagyar, 1, 4, 15, 0, Weapons.Hammer, Weapons.Greatsword, attack1: AttackCatalogue.Greatsword_Swing, attack2: AttackCatalogue.Greatsword_String);
        public static LegendCard DeathCap = new LegendCard("Deathcap", 5, Elements.Nature, Images.DeathCap, 10, 10, 5, 0, Weapons.Spear, Weapons.Orb, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Orb_Throw, attack3: AttackCatalogue.DeathCap_Storm);
        //rayman
        //glowbox
        //gnash
        //kor
        //diana
        //ember
        //yumiko
        //reno

        // Magic
        public static LegendCard FaerieQueen = new LegendCard("Faerie Queen", 3, Elements.Magic, Images.FaerieQueen, 7, 3, 8, 0, Weapons.Spear, Weapons.Greatsword, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Greatsword_Swing, attack3: AttackCatalogue.Greatsword_String, attack4: AttackCatalogue.Arcadia_PinkRoses); // base stance
        public static LegendCard Enchantress = new LegendCard("Enchantress", 4, Elements.Magic, Images.Enchantress, 0, 5, 18, 0, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Enchantress_EnchantHealth, attack2: AttackCatalogue.Enchantress_EnchantPower, attack3: AttackCatalogue.Enchantress_CurseHealth, attack4: AttackCatalogue.Enchantress_CursePower);
        public static LegendCard DarkMage = new LegendCard("Dark Mage", 5, Elements.Magic, Images.DarkMage, 11, 1, 12, 0, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Scythe_Slash);
        //nai
        //nix
        //magyar

        // Shadow
        public static LegendCard MasterThief = new LegendCard("Master Thief", 2, Elements.Shadow, Images.MasterThief, 4, 5, 4, 0, Weapons.Gauntlets, Weapons.Katars, attack1: AttackCatalogue.Any_BurnForCard, attack2: AttackCatalogue.MasterThief_GrabBomb);
        //azoth
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
        //teros
        //asuri
        //ragnir
        //mordex
        //yumiko dog
        //embed squirle
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

        // ALL WEAPON CARDS
        // Axe
        // Battle Boots
        // Blasters
        public static WeaponCard BlazingFire = new WeaponCard("Blazing Fire", 1, Elements.Fire, Images.BlazingFire, Weapons.Blasters);
        // Bow
        // Cannon
        // Gauntlets
        public static WeaponCard SleightOfHand = new WeaponCard("Sleight of Hand", 1, Elements.Shadow, Images.Sleight_of_Hand, Weapons.Gauntlets);
        // Greatsword
        public static WeaponCard LawOfTheLand = new WeaponCard("Law of the Land", 1, Elements.Nature, Images.LawOfTheLand, Weapons.Greatsword);
        // Hammer
        // Katars
        // Lance
        public static WeaponCard GalaxyLance = new WeaponCard("Galaxy Lance", 1, Elements.Cosmic, Images.GalaxyLance, Weapons.RocketLance);
        // Orb
        public static WeaponCard RemnantOfFate = new WeaponCard("Remnant of Fate", 1, Elements.Magic, Images.RemnantOfFate, Weapons.Orb);
        public static WeaponCard SacredRelic = new WeaponCard("Sacred Relic", 1, Elements.Nature, Images.SacredRelic, Weapons.Orb);
        public static WeaponCard ScryingGlass = new WeaponCard("Scrying Glass", 1, Elements.Cosmic, Images.ScryingGlass, Weapons.Orb);
        // Scythe
        public static WeaponCard SearingBlade = new WeaponCard("Searing Blade", 1, Elements.Fire, Images.SearingBlade, Weapons.Scythe);
        public static WeaponCard ShootingStar = new WeaponCard("Shooting Star", 1, Elements.Cosmic, Images.ShootingStar, Weapons.Scythe);
        public static WeaponCard StarryScythe = new WeaponCard("Starry Scythe", 1, Elements.Cosmic, Images.StarryScythe, Weapons.Scythe);
        // Spear
        public static WeaponCard MagmaSpear = new WeaponCard("Magma Spear", 1, Elements.Fire, Images.MagmaSpear, Weapons.Spear);
        public static WeaponCard PiercingRegret = new WeaponCard("Piercing Regret", 1, Elements.Nature, Images.PiercingRegret, Weapons.Spear);
        // Sword

        // Battle Cards
        public static BattleCard BouncyBomb = new BattleCard("Bouncy Bomb", 2, "When Played: Deals direct damage", Elements.Shadow, Images.BouncyBomb, true, whenPlayedEffect: EffectCatalogue.BouncyBomb);



        // Decks
        public static List<Card> GsDeck = new List<Card>() {
            // Essence
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),

            // Stages
            Fangwild.Clone(),
            Fangwild.Clone(),
            Fangwild.Clone(),
            Fangwild.Clone(),

            // Legends
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            BriarRose.Clone(),
            BriarRose.Clone(),
            BriarRose.Clone(),
            BriarRose.Clone(),
            ForestGuardian.Clone(),
            ForestGuardian.Clone(),
            ForestGuardian.Clone(),
            ForestGuardian.Clone(),
            DeathCap.Clone(),
            DeathCap.Clone(),
            DeathCap.Clone(),
            DeathCap.Clone(),
            Heatblast.Clone(),
            Heatblast.Clone(),

            // Weapons
            BlazingFire.Clone(),
            BlazingFire.Clone(),
            BlazingFire.Clone(),
            BlazingFire.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            LawOfTheLand.Clone(),
            LawOfTheLand.Clone(),
            LawOfTheLand.Clone(),
            LawOfTheLand.Clone(),
            SacredRelic.Clone(),
            SacredRelic.Clone(),
            SacredRelic.Clone(),
            SacredRelic.Clone(),
            PiercingRegret.Clone(),
            PiercingRegret.Clone(),
            PiercingRegret.Clone(),
            PiercingRegret.Clone(),
        };
        public static List<Card> CosmicDeck = new List<Card>() {
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),

            Mustafar.Clone(),
            Mustafar.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            BlazingFire.Clone(),
            BlazingFire.Clone(),
            StarryScythe.Clone(),
            StarryScythe.Clone(),
            StarryScythe.Clone(),
            SearingBlade.Clone(),
            SearingBlade.Clone(),
            GalaxyLance.Clone(),
            GalaxyLance.Clone(),
            GalaxyLance.Clone(),
            Artemis.Clone(),
            Artemis.Clone(),
            IronLady.Clone(),
            IronLady.Clone(),
            Orion.Clone(),
            Orion.Clone(),
        };
        public static List<Card> CosmicDeck2 = new List<Card>() {
            // Essence
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),

            // Stages
            Mustafar.Clone(),
            Mustafar.Clone(),
            Mustafar.Clone(),

            // Weapons
            ShootingStar.Clone(),
            ShootingStar.Clone(),
            ShootingStar.Clone(),
            ShootingStar.Clone(),
            StarryScythe.Clone(),
            StarryScythe.Clone(),
            StarryScythe.Clone(),
            StarryScythe.Clone(),
            GalaxyLance.Clone(),
            GalaxyLance.Clone(),
            GalaxyLance.Clone(),
            ScryingGlass.Clone(),
            ScryingGlass.Clone(),
            RemnantOfFate.Clone(),
            RemnantOfFate.Clone(),
            RemnantOfFate.Clone(),

            // Legends
            Artemis.Clone(),
            Artemis.Clone(),
            Artemis.Clone(),
            Orion.Clone(),
            Orion.Clone(),
            IronLady.Clone(),
            IronLady.Clone(),
            IronLady.Clone(),
            DarkMage.Clone(),
            DarkMage.Clone(),
            DarkMage.Clone(),
            DarkMage.Clone(),
            Enchantress.Clone(),
            Enchantress.Clone(),
        };
        public static List<Card> TestDeck = new List<Card>() {
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            MasterThief.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            SleightOfHand.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
            BouncyBomb.Clone(),
        };
        public static List<Card> DeathCapDeck = new List<Card>() {
            // Essence
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),

            // Stages
            Fangwild.Clone(),
            Fangwild.Clone(),
            Fangwild.Clone(),
            Fangwild.Clone(),

            // Legends
            ForestGuardian.Clone(),
            ForestGuardian.Clone(),
            ForestGuardian.Clone(),
            ForestGuardian.Clone(),
            DeathCap.Clone(),
            DeathCap.Clone(),
            DeathCap.Clone(),
            DeathCap.Clone(),
            Heatblast.Clone(),
            Heatblast.Clone(),

            // Weapons
            BlazingFire.Clone(),
            BlazingFire.Clone(),
            BlazingFire.Clone(),
            BlazingFire.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            SacredRelic.Clone(),
            SacredRelic.Clone(),
            SacredRelic.Clone(),
            SacredRelic.Clone(),
            PiercingRegret.Clone(),
            PiercingRegret.Clone(),
        };

        public static List<Card> CloneList(List<Card> originalList)
        {
            return originalList.Select(card => card.Clone()).ToList(); // Deep clone each card in the list
        }
    }
}
