﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    internal class EffectCatalogue
    {
        // Start Turn Effect
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

        // When Played Effect
        public static void BouncyBomb(object target)
        {
            int bouncyBombDamage = 8;
            if (target is LegendCard legend)
            {
                legend.LoseHealth(bouncyBombDamage);
            }
        }
    }
}
