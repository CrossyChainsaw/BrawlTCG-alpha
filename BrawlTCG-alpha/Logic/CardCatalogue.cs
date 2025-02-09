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
        public static StageCard Fangwild = new StageCard("Fangwild", 3, "Start Turn: Magic and Nature Legends will be healed by 2", Elements.Nature, Properties.Resources.Fangwild, startTurnEffect: StageEffectCatalogue.FangwildEffect);
        public static StageCard Mustafar = new StageCard("Mustafar", 2, "Start Turn: Every non-Fire Legend will lose 1hp", Elements.Fire, Properties.Resources.Mustafar, startTurnEffect: StageEffectCatalogue.MustafarEffect);

        // Legends
        public static LegendCard FaerieQueen = new LegendCard("Faerie Queen", 2, "Legend", Elements.Magic, Properties.Resources.FaerieQueen, 7, 7, 4, 4, Weapons.Spear, Weapons.Greatsword, attack1: AttackCatalogue.Spear_Stab.Clone(), attack2: AttackCatalogue.Greatsword_Swing.Clone(), attack3: AttackCatalogue.Greatsword_String.Clone(), attack4: AttackCatalogue.Arcadia_PinkRoses.Clone()); // base stance
        public static LegendCard BriarRose = new LegendCard("Briar Rose", 2, "Legend", Elements.Nature, Properties.Resources.BriarRose, 6, 7, 4, 5, Weapons.Spear, Weapons.Greatsword, attack1: AttackCatalogue.Spear_Stab.Clone(), attack2: AttackCatalogue.Any_Seduce.Clone()); // speed stance
        public static LegendCard Artemis = new LegendCard("Artemis", 2, "Legend", Elements.Cosmic, Properties.Resources.Artemis, 5, 5, 4, 8, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash.Clone(), attack2: AttackCatalogue.Scythe_Gimp.Clone(), attack3: AttackCatalogue.Lance_Flamethrower.Clone());
        public static LegendCard IronLady = new LegendCard("Iron Lady", 3, "Legend", Elements.Fire, Properties.Resources.IronLady, 5 + 3, 5 + 3, 4 + 3, 8, Weapons.RocketLance, Weapons.Scythe, attack1: AttackCatalogue.Scythe_Slash.Clone(), attack2: AttackCatalogue.Scythe_Gimp.Clone(), attack3: AttackCatalogue.Lance_Flamethrower.Clone(), attack4: AttackCatalogue.Artemis_IronLady_MeltDown.Clone()); // Base Stance + Imaginary stats
        public static LegendCard Orion = new LegendCard("Orion", 2, "Legend", Elements.Cosmic, Properties.Resources.Orion, 4, 6, 6, 6, Weapons.RocketLance, Weapons.Spear, attack1: AttackCatalogue.Spear_Stab.Clone(), attack3: AttackCatalogue.Lance_Flamethrower.Clone());

        // Weapons
        public static WeaponCard MagmaSpear = new WeaponCard("Magma Spear", 1, "Spear", Elements.Fire, Properties.Resources.MagmaSpear, Weapons.Spear);
        public static WeaponCard BlazingFire = new WeaponCard("Blazing Fire", 1, "Blasters", Elements.Fire, Properties.Resources.BlazingFire, Weapons.Blasters);
        public static WeaponCard GalaxyLance = new WeaponCard("Galaxy Lance", 1, "Lance", Elements.Cosmic, Properties.Resources.GalaxyLance, Weapons.RocketLance);
        public static WeaponCard StarryScythe = new WeaponCard("Starry Scythe", 1, "Scythe", Elements.Cosmic, Properties.Resources.StarryScythe, Weapons.Scythe);
        public static WeaponCard SearingBlade = new WeaponCard("Searing Blade", 1, "Scythe", Elements.Fire, Properties.Resources.SearingBlade, Weapons.Scythe);
        public static WeaponCard LawOfTheLand = new WeaponCard("Law of the Land", 1, "Greatsword", Elements.Nature, Properties.Resources.LawOfTheLand, Weapons.Greatsword);

        // Decks
        public static List<Card> FaerieQueenDeck = new List<Card>() {
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),

            Fangwild.Clone(),
            Fangwild.Clone(),
            Fangwild.Clone(),
            Fangwild.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            BriarRose.Clone(),
            BriarRose.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            LawOfTheLand.Clone(),
            LawOfTheLand.Clone(),
            LawOfTheLand.Clone(),
            LawOfTheLand.Clone(),
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
