using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    public class EssenceCard : Card
    {
        public EssenceCard(int id, string name, int cost, Elements element, Image image, Effect startTurnEffect) : base(id, name, cost, element, image)
        {
            StartTurnEffect = startTurnEffect;
            Description = GenerateEffectDescription();
        }
        public override Card Clone()
        {
            return new EssenceCard(
                id: ID,
                name: Name,
                cost: Cost,
                element: Element,
                image: Image,
                startTurnEffect: StartTurnEffect
            );
        }
    }

}
