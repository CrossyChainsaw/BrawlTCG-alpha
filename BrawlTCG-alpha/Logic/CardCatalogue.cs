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
        public static StageCard Fangwild = new StageCard("Fangwild", 2, "Start Turn: Magic and Nature Legends will be healed by 2", Elements.Nature, Properties.Resources.Fangwild2);
        // Legends
        public static LegendCard FaerieQueen = new LegendCard("Faerie Queen", 2, "Legend", Elements.Magic, Properties.Resources.FaerieQueen, 7, 7, 4, 4, Weapons.Spear, Weapons.Greatsword); // base stance
        public static LegendCard BriarRose = new LegendCard("Briar Rose", 2, "Legend", Elements.Nature, Properties.Resources.BriarRose, 6, 7, 4, 5, Weapons.Spear, Weapons.Greatsword); // speed stance
        public static LegendCard Artemis = new LegendCard("Artemis", 2, "Legend", Elements.Cosmic, Properties.Resources.Artemis, 5, 5, 4, 8, Weapons.RocketLance, Weapons.Scythe);
        public static LegendCard Orion = new LegendCard("Orion", 2, "Legend", Elements.Cosmic, Properties.Resources.Orion, 4, 6, 6, 6, Weapons.RocketLance, Weapons.Spear);
        // Weapons
        public static WeaponCard MagmaSpear = new WeaponCard("Magma Spear", 1, "Spear", Elements.Fire, Properties.Resources.MagmaSpear, Weapons.Spear);
        public static WeaponCard BlazingFire = new WeaponCard("Blazing Fire", 1, "Blasters", Elements.Fire, Properties.Resources.BlazingFire, Weapons.Blasters);
        public static WeaponCard GalaxyLance = new WeaponCard("Galaxy Lance", 1, "Lance", Elements.Cosmic, Properties.Resources.GalaxyLance, Weapons.RocketLance);
        public static WeaponCard StarryScythe = new WeaponCard("Starry Scythe", 1, "Scythe", Elements.Cosmic, Properties.Resources.StarryScythe, Weapons.Scythe);

        // Decks
        public static List<Card> Deck1 = new List<Card>() {
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
            Fangwild.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            BriarRose.Clone(),
            BriarRose.Clone(),
            BriarRose.Clone(),
            BriarRose.Clone(),
            BriarRose.Clone(),
        };
        public static List<Card> TestDeck = new List<Card>() {
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Essence.Clone(),
            Fangwild.Clone(),
            FaerieQueen.Clone(),
            FaerieQueen.Clone(),
            BriarRose.Clone(),
            BriarRose.Clone(),
            MagmaSpear.Clone(),
            MagmaSpear.Clone(),
            BlazingFire.Clone(),
            BlazingFire.Clone(),
            StarryScythe.Clone(),
            StarryScythe.Clone(),
            GalaxyLance.Clone(),
            GalaxyLance.Clone(),
            Artemis.Clone(),
            Artemis.Clone(),
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
