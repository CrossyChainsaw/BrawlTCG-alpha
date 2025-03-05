using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    public class EffectCatalogue
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
        public static void GenerateAndPlayMustafar(object target, Card card, Game game)
        {
            // first give the player essence before playing it!
            StageCard generatedCard = (StageCard)CardCatalogue.GetCardById(100); // #100: Mustafar
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
            // target must be the form here if possible

            if (game.ActivePlayer == game.Me)
            {
                // generate random card
                Card generatedCard = CardCatalogue.GetRandomCard();
                // send random card id
                game.SendMessageToPeer($"RANDOM_CARD_ID:{generatedCard.ID}");
                // add card to deck
                game.AddCardToHandZone(game.ActivePlayer, generatedCard);
            }
            else
            {
                // get the random card the peer got
                if (game.RandomCardID == -1)
                {
                    while (game.RandomCardID == -1)
                    {
                        // wait till its not -1
                    }
                }
                Card generatedCard = CardCatalogue.GetCardById(game.RandomCardID);
                // add to his hand
                game.AddCardToHandZone(game.ActivePlayer, generatedCard);
                // reset the property
                game.RandomCardID = -1;
            }
        }

        public static void DrawCard(object target, Card card, Game game)
        {
            game.DrawCardFromDeck(game.ActivePlayer);
            game.ShowCards();
        }

        public static void DrawTwoCards(object target, Card card, Game game)
        {
            game.DrawCardFromDeck(game.ActivePlayer);
            game.DrawCardFromDeck(game.ActivePlayer);
            game.ShowCards();
        }

        public static void DrawThreeCards(object target, Card card, Game game)
        {
            game.DrawCardFromDeck(game.ActivePlayer);
            game.DrawCardFromDeck(game.ActivePlayer);
            game.DrawCardFromDeck(game.ActivePlayer);
            game.ShowCards();
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