using System;
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
        public static void DirectDamage(object target, Card card, Game game)
        {
            if (card is BattleCard battleCard)
            {
                if (target is LegendCard legend)
                {
                    legend.LoseHealth(battleCard.Damage);
                }
            }
        }
        public static void BoostStats(object target, Card card, Game game)
        {
            if (card is BattleCard battleCard)
            {
                if (target is LegendCard legend)
                {
                    legend.ModifyStat(Stats.Health, battleCard.HealthModifier);
                    legend.ModifyStat(Stats.Power, battleCard.PowerModifier);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        public static void Heal(object target, Card card, Game game)
        {
            if (card is BattleCard battleCard)
            {
                if (target is LegendCard legend)
                {
                    legend.GainHealth(battleCard.HealthModifier);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        public static void GenerateMatrix(object target, Card card, Game game)
        {
            StageCard generatedCard = (StageCard)CardCatalogue.GetCardByName("Matrix");
            StageCard card2 = generatedCard;
            game.AddCardToHandZone(game.ActivePlayer, card2);
            game.PlayStageCard(card2);

            //// add to hand
            //game.AddCardToHandZone(game.ActivePlayer, card2);
            //// flip to show
            //game.ShowCards();
        }

        public static void GenerateCard(LegendCard attacker, object target, Attack attack, Player activePlayer, Game game, Card generatedCard)
        {
            Card card = generatedCard;
            // add to hand
            game.AddCardToHandZone(activePlayer, card);
            // flip to show
            game.ShowCards();
        }
    }
}