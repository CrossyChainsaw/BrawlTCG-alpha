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
        public EssenceCard(int id, string name, int cost, Elements element, Image image) : base(id, name, cost, element, image)
        {
            Description = "Start Turn: Gives the player 1 Essence";
            StartTurnEffect = (target, _, __) => GivePlayerEssence(target, _, __);
        }
        void GivePlayerEssence(object target, Card _, Game __)
        {
            if (target is Player player)
            {
                player.GainEssence(ESSENCE_GAIN);
            }
        }
        public override Card Clone()
        {
            return new EssenceCard(id: ID, name: Name, cost: Cost, element: Element, image: Image);
        }
    }

}
