using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    public class EssenceCard : Card
    {
        public EssenceCard(int id, string name, int cost, Elements element, Image image) : base(id, name, cost, element, image)
        {
            Description = "Start Turn: Gives the player 1 Essence";
            StartTurnEffect = EffectCatalogue.Essence;
        }
        public override Card Clone()
        {
            return new EssenceCard(id: ID, name: Name, cost: Cost, element: Element, image: Image);
        }
    }

}
