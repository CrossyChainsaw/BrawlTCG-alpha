using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrawlTCG_alpha.Logic.Cards;

namespace BrawlTCG_alpha.Logic
{
    internal class CardCatalogue
    {
        // Essence
        public static EssenceCard Essence = new EssenceCard("Essence", 0, "Start Turn: Gives the player 1 Essence", Elements.Magic, Properties.Resources.Essence);
        
        // Stages
        public static StageCard Mustafar = new StageCard("Mustafar", 2, "Start Turn: Every non-Fire Legend will lose 1hp", Elements.Fire, Properties.Resources.Mustafar, startTurnEffect: StageEffectCatalogue.MustafarEffect);
        public static StageCard Fangwild = new StageCard("Fangwild", 3, "Start Turn: Magic and Nature Legends will be healed by 2", Elements.Nature, Properties.Resources.Fangwild, startTurnEffect: StageEffectCatalogue.FangwildEffect);

        // Legends - every 5 is a cost ish
        public static LegendCard IronLady = new LegendCard("Iron Lady", 5, "Legend", Elements.Fire, Properties.Resources.IronLady, 7, 5, 7, 8, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash, attack2: AttackCatalogue.Scythe_Gimp, attack3: AttackCatalogue.Lance_Flamethrower, attack4: AttackCatalogue.Artemis_IronLady_MeltDown); // Base Stance + Imaginary stats
        public static LegendCard Artemis = new LegendCard("Artemis", 3, "Legend", Elements.Cosmic, Properties.Resources.Artemis, 5, 5, 4, 8, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash, attack2: AttackCatalogue.Scythe_Gimp, attack3: AttackCatalogue.Lance_Flamethrower);
        public static LegendCard Orion = new LegendCard("Orion", 3, "Legend", Elements.Cosmic, Properties.Resources.Orion, 4, 6, 6, 6, Weapons.RocketLance, Weapons.Spear, attack1: AttackCatalogue.Spear_Stab, attack3: AttackCatalogue.Lance_Flamethrower);
        public static LegendCard BriarRose = new LegendCard("Briar Rose", 3, "Legend", Elements.Nature, Properties.Resources.BriarRose, 6, 7, 4, 5, Weapons.Spear, Weapons.Greatsword, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Any_Seduce); // speed stance
        public static LegendCard ForestGuardian = new LegendCard("Forest Guardian", 2, "Legend", Elements.Nature, Properties.Resources.ElvenhollowMagyar, 1, 4, 11, 4, Weapons.Hammer, Weapons.Greatsword, attack1: AttackCatalogue.Greatsword_Swing, attack2:AttackCatalogue.Greatsword_String);
        public static LegendCard DeathCap = new LegendCard("Deathcap", 5, "Legend", Elements.Nature, Properties.Resources.DeathCap, 10, 10, 5, 0, Weapons.Spear, Weapons.Orb, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Orb_Throw, attack3: AttackCatalogue.DeathCap_Storm);
        public static LegendCard FaerieQueen = new LegendCard("Faerie Queen", 3, "Legend", Elements.Magic, Properties.Resources.FaerieQueen, 7, 7, 4, 4, Weapons.Spear, Weapons.Greatsword, attack1: AttackCatalogue.Spear_Stab, attack2: AttackCatalogue.Greatsword_Swing, attack3: AttackCatalogue.Greatsword_String, attack4: AttackCatalogue.Arcadia_PinkRoses); // base stance
        public static LegendCard Enchantress = new LegendCard("Enchantress", 4, "Legend", Elements.Magic, Properties.Resources.Enchantress, 0, 4, 11, 7, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Enchantress_EnchantHealth, attack2: AttackCatalogue.Enchantress_EnchantPower, attack3: AttackCatalogue.Enchantress_CurseHealth, attack4: AttackCatalogue.Enchantress_CursePower);
        public static LegendCard DarkMage = new LegendCard("Dark Mage", 5, "Legend", Elements.Magic, Properties.Resources.DarkMage, 11, 4, 5, 7, Weapons.Scythe, Weapons.Orb, attack1: AttackCatalogue.Scythe_Slash);
        // heatblast

        // Weapons
        public static WeaponCard MagmaSpear = new WeaponCard("Magma Spear", 1, "Spear", Elements.Fire, Properties.Resources.MagmaSpear, Weapons.Spear);
        public static WeaponCard BlazingFire = new WeaponCard("Blazing Fire", 1, "Blasters", Elements.Fire, Properties.Resources.BlazingFire, Weapons.Blasters);
        public static WeaponCard SearingBlade = new WeaponCard("Searing Blade", 1, "Scythe", Elements.Fire, Properties.Resources.SearingBlade, Weapons.Scythe);
        public static WeaponCard GalaxyLance = new WeaponCard("Galaxy Lance", 1, "Lance", Elements.Cosmic, Properties.Resources.GalaxyLance, Weapons.RocketLance);
        public static WeaponCard StarryScythe = new WeaponCard("Starry Scythe", 1, "Scythe", Elements.Cosmic, Properties.Resources.StarryScythe, Weapons.Scythe);
        public static WeaponCard ShootingStar = new WeaponCard("Shooting Star", 1, "Scythe", Elements.Cosmic, Properties.Resources.ShootingStar, Weapons.Scythe);
        public static WeaponCard ScryingGlass = new WeaponCard("Scrying Glass", 1, "Orb", Elements.Cosmic, Properties.Resources.ScryingGlass, Weapons.Orb);
        public static WeaponCard LawOfTheLand = new WeaponCard("Law of the Land", 1, "Greatsword", Elements.Nature, Properties.Resources.LawOfTheLand, Weapons.Greatsword);
        public static WeaponCard PiercingRegret = new WeaponCard("Piercing Regret", 1, "Spear", Elements.Nature, Properties.Resources.PiercingRegret, Weapons.Spear);
        public static WeaponCard RemnantOfFate = new WeaponCard("Remnant of Fate", 1, "Orb", Elements.Magic, Properties.Resources.RemnantOfFate, Weapons.Orb);
        public static WeaponCard SacredRelic = new WeaponCard("Sacred Relic", 1, "Orb", Elements.Nature, Properties.Resources.SacredRelic, Weapons.Orb);



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

            // Weapons
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

            // Weapons
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
        public static List<Card> ThreeCards = new List<Card>() {
            Essence.Clone(),
            Essence.Clone(),
            MagmaSpear.Clone(),
        };
        public static List<Card> CloneList(List<Card> originalList)
        {
            return originalList.Select(card => card.Clone()).ToList(); // Deep clone each card in the list
        }
    }
}
