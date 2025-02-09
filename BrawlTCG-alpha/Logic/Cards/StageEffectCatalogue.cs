using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    internal class StageEffectCatalogue
    {
        public static void MustafarEffect(object target)
        {
            if (target is List<LegendCard> legends)
            {
                foreach (LegendCard legend in legends)
                {
                    if (legend.Element != Elements.Fire)
                    {
                        legend.LoseHealth(1);
                    }
                }
            }
        }

        public static void FangwildEffect(object target)
        {
            if (target is List<LegendCard> legends)
            {
                foreach (LegendCard legend in legends)
                {
                    if (legend.Element == Elements.Magic || legend.Element == Elements.Nature)
                    {
                        legend.GainHealth(2);
                    }
                }
            }
        }
    }
}
