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
        public static void MustafarEffect(object target, Card card, Game game)
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

        public static void FangwildEffect(object target, Card card, Game game)
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
        public static void BoostHealthAndPower(object target, Card card, Game game)
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
        public static void GenerateAndPlayMatrix(object target, Card card, Game game)
        {
            // first give the player essence before playing it!
            StageCard generatedCard = (StageCard)CardCatalogue.GetCardById(102); // #102: Matrix
            StageCard card2 = generatedCard;
            game.ActivePlayer.GainEssence(card2.Cost);
            game.AddCardToHandZone(game.ActivePlayer, card2);
            game.PlayStageCard(card2);

            //// add to hand
            //game.AddCardToHandZone(game.ActivePlayer, card2);
            //// flip to show
            //game.ShowCards();
        }

        public static void GenerateAndPlayWorkshop(object target, Card card, Game game)
        {
            // first give the player essence before playing it!
            StageCard generatedCard = (StageCard)CardCatalogue.GetCardById(103); // #103: Workshop
            StageCard card2 = generatedCard;
            game.ActivePlayer.GainEssence(card2.Cost);
            game.AddCardToHandZone(game.ActivePlayer, card2);
            game.PlayStageCard(card2);
        }

        public static void GenerateRandomCard(object target, Card card, Game game)
        {
            Card generatedCard = CardCatalogue.GetRandomCard();
            game.AddCardToHandZone(game.ActivePlayer, generatedCard);
        }

        public static void GenerateCard(LegendCard attacker, object target, Attack attack, Player activePlayer, Game game, Card generatedCard)
        {
            Card card = generatedCard;
            // add to hand
            game.AddCardToHandZone(activePlayer, card);
            // flip to show
            game.ShowCards();
        }

        // While in play effect
        public static void EvilHideoutWhenPlayed(object target, Card card, Game game)
        {
            // target is everyone

            // my cards
            foreach (Card c in game.Me.PlayingField)
            {
                if (c is LegendCard legend)
                {
                    List<Elements> evilHideoutElements = new List<Elements> { Elements.Fire, Elements.Wild, Elements.Shadow };

                    if (evilHideoutElements.Contains(legend.Element))
                    {
                        legend.ModifyStat(Stats.Power, 3);
                    }
                }
            }
            // enemy cards
            foreach (Card c in game.Opponent.PlayingField)
            {
                if (c is LegendCard legend)
                {
                    List<Elements> evilHideoutElements = new List<Elements> { Elements.Fire, Elements.Wild, Elements.Shadow };

                    if (evilHideoutElements.Contains(legend.Element))
                    {
                        legend.ModifyStat(Stats.Power, 3);
                    }
                }
            }
        }
        public static void EvilHideoutWhilePlay(object target, Card card, Game game)
        {
            if (target is LegendCard legend)
            {
                List<Elements> evilHideoutElements = new List<Elements> { Elements.Fire, Elements.Wild, Elements.Shadow };

                if (evilHideoutElements.Contains(legend.Element))
                {
                    legend.ModifyStat(Stats.Power, 3);
                }
            }
        }
        public static void EvilHideoutWhenDiscard(object target, Card card, Game game)
        {
            // target is everyone

            // my cards
            foreach (Card c in game.Me.PlayingField)
            {
                if (c is LegendCard legend)
                {
                    List<Elements> evilHideoutElements = new List<Elements> { Elements.Fire, Elements.Wild, Elements.Shadow };

                    if (evilHideoutElements.Contains(legend.Element))
                    {
                        legend.ModifyStat(Stats.Power, -3);
                    }
                }
            }
            // enemy cards
            foreach (Card c in game.Opponent.PlayingField)
            {
                if (c is LegendCard legend)
                {
                    List<Elements> evilHideoutElements = new List<Elements> { Elements.Fire, Elements.Wild, Elements.Shadow };

                    if (evilHideoutElements.Contains(legend.Element))
                    {
                        legend.ModifyStat(Stats.Power, -3);
                    }
                }
            }
        }
    }
}