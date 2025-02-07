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
        public static EssenceCard Essence = new EssenceCard("Essence", 0, "Start Turn: Gives the player 1 Essence", Elements.Magic, Properties.Resources.Essence);
        public static StageCard Fangwild = new StageCard("Fangwild", 2, "Start Turn: Magic and Nature Legends will be healed by 2", Elements.Nature, Properties.Resources.Fangwild2);
        public static LegendCard FaerieQueen = new LegendCard("Faerie Queen", 2, "Legend", Elements.Magic, Properties.Resources.FaerieQueen, 7, 7, 4, 4); // base stance
        public static LegendCard BriarRose = new LegendCard("Briar Rose", 2, "Legend", Elements.Nature, Properties.Resources.BriarRose, 6, 7, 4, 5); // speed stance

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
        public static List<Card> Deck2 = new List<Card>() {
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
        };
    }
}
