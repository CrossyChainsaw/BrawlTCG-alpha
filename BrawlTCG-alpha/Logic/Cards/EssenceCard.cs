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
        public EssenceCard(string name, int cost, string description, Elements element, Image image) : base(name, cost, description, element, image)
        {
            Name = name;
            Cost = cost;
            Description = description;
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
            return new EssenceCard(name: Name, cost: Cost, description: Description, element: Element, image: Image);
        }
    }

}
