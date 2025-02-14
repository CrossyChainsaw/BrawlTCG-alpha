using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    internal class EssenceCard : Card
    {
        const int ESSENCE_GAIN = 1;
        public EssenceCard(string name, int cost, Elements element, Image image) : base(name, cost, element, image)
        {
            Name = name;
            Cost = cost;
            Description = "Start Turn: Gives the player 1 Essence";
            Element = element;
            Image = image;
            StartTurnEffect = (target) => GivePlayerEssence(target);
        }
        void GivePlayerEssence(object target)
        {
            if (target is Player player)
            {
                player.GainEssence(ESSENCE_GAIN);
            }
        }
        public override Card Clone()
        {
            return new EssenceCard(name: Name, cost: Cost, element: Element, image: Image);
        }
    }

}
