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
        static int mustafarDamage = 1;
        public static Effect Mustafar = new Effect(
            description: $"All non-Fire Legends lose {mustafarDamage} health.",
            effectAction: (target, card, game) => StartTurnDamage(target, mustafarDamage, Elements.Fire)
        );

        static int fangwildHeal = 2;
        public static Effect Fangwild = new Effect(
            description: $"All Magic and Nature Legends will gain {fangwildHeal} health.",
            effectAction: (target, card, game) => StartTurnHeal(target, fangwildHeal, Elements.Nature, Elements.Magic)
        );

        public static Effect Workshop = new Effect(
            description: $"Obtain a random card",
            effectAction: (target, card, game) => GenerateRandomCard(game)
        );

        static int spaceTimeExtraDrawnCards = 2;
        public static Effect SpaceTime = new Effect(
            description: $"Draw {spaceTimeExtraDrawnCards} extra cards",
            effectAction: (target, card, game) => DrawCards(game, spaceTimeExtraDrawnCards)
        );

        public static Effect Essence = new Effect(
            description: $"Gain 1 Essence",
            effectAction: (target, card, game) => GivePlayerEssence(target)
        );

        // When Played Effect
        static int mustafarID = 100;
        public static Effect GenerateAndPlayMustafar = new Effect(
            description: $"Change stage to Mustafar",
            effectAction: (target, card, game) => GenerateAndPlayStage(card, game, mustafarID)
        );

        static int matrixID = 102;
        public static Effect GenerateAndPlayMatrix = new Effect(
            description: $"Change stage to Matrix",
            effectAction: (target, card, game) => GenerateAndPlayStage(card, game, matrixID)
        );

        static int workshopID = 103;
        public static Effect GenerateAndPlayWorkshop = new Effect(
            description: $"Change stage to Workshop",
            effectAction: (target, card, game) => GenerateAndPlayStage(card, game, workshopID)
        );

        public static Effect EvilHideoutWhenPlayed = new Effect(
            description: "While in play: Fire, Wild, and Shadow legends get +3 Power",
            effectAction: (target, card, game) => ModifyStatsOfAllLegendsWhenPlayed(game, new List<Elements> { Elements.Fire, Elements.Wild, Elements.Shadow }, Stats.Power, 3)
        );

        public static Effect BattleCardDirectDamageWhenPlayed = new Effect(
            description: "Deals direct damage to opposing Legend",
            effectAction: (target, card, game) => DirectDamage(target, card)
        );

        public static Effect BattleCardHealWhenPlayed = new Effect(
            description: "Heals your Legend",
            effectAction: (target, card, game) => Heal(target, card)
        );

        public static Effect BoostHealthAndPower = new Effect(
            description: "Modifies Health and Power",
            effectAction: (target, card, game) => ModifyHealthAndPower(target, card)
        );

        public static Effect BoostHealthAndPowerAllYourLegends = new Effect(
            description: "Modifies Health and Power",
            effectAction: (target, card, game) => ModifyHealthAndPowerAllYourLegends(card, game)
        );

        static int nCards = 3;
        public static Effect CardChest = new Effect(
            description: $"Draw {nCards} cards",
            effectAction: (target, card, game) => DrawCards(game, nCards)
        );


        // Generic Methods
        static void StartTurnHeal(object target, int heal, params Elements[] targetElements)
        {
            if (target is List<LegendCard> legends)
            {
                foreach (LegendCard legend in legends)
                {
                    if (targetElements.Contains(legend.Element))
                    {
                        legend.GainHealth(heal);
                    }
                }
            }
        }
        static void StartTurnDamage(object target, int damage, params Elements[] immuneElements)
        {
            if (target is List<LegendCard> legends)
            {
                foreach (LegendCard legend in legends)
                {
                    if (!immuneElements.Contains(legend.Element))
                    {
                        legend.LoseHealth(damage);
                    }
                }
            }
        }
        static void GenerateRandomCard(Game game)
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
        static void GenerateRandomCards(Game game, int nCards)
        {
            for (int i = 0; i < nCards; i++)
            {
                GenerateRandomCard(game);
            }
        } // more than 1 random card doesnt work now. because it sends a message this is the random card and then sends it again and overwrties it. find a way to send all random cards together
        static void GivePlayerEssence(object target)
        {
            if (target is Player player)
            {
                player.GainEssence(1);
            }
        }
        static void DrawCards(Game game, int n)
        {
            for (int i = 0; i < n; i++)
            {
                game.DrawCardFromDeck(game.ActivePlayer);
            }
            game.ShowCards();
        }
        static void GenerateAndPlayStage(Card card, Game game, int cardID)
        {
            // first give the player essence before playing it!
            StageCard generatedCard = (StageCard)CardCatalogue.GetCardById(cardID);
            StageCard card2 = generatedCard;
            game.ActivePlayer.GainEssence(card2.Cost);
            game.AddCardToHandZone(game.ActivePlayer, card2);
            game.PlayStageCard(card2);
        }
        internal static void GenerateAndPlayLegend(Game game, int cardID)
        {
            // first give the player essence before playing it!
            LegendCard generatedCard = (LegendCard)CardCatalogue.GetCardById(cardID);
            LegendCard legend = generatedCard;
            game.ActivePlayer.GainEssence(legend.Cost);
            game.AddCardToHandZone(game.ActivePlayer, legend);
            game.UiManager.PlayLegendCard(game.ActivePlayer, legend);
        }
        static void ModifyStatsOfAllLegendsWhenPlayed(Game game, List<Elements> targetElements, Stats stat, int modifier)
        {
            // Apply to both your cards and opponent's cards
            ApplyEffectToLegends(game.Me.PlayingField, targetElements, stat, modifier);
            ApplyEffectToLegends(game.Opponent.PlayingField, targetElements, stat, modifier);

            static void ApplyEffectToLegends(List<Card> playingField, List<Elements> targetElements, Stats stat, int modifier)
            {
                foreach (Card c in playingField)
                {
                    if (c is LegendCard legend)
                    {
                        if (targetElements.Contains(legend.Element))
                        {
                            legend.ModifyStat(stat, modifier);
                        }
                    }
                }
            }
        }
        static void DirectDamage(object target, Card card)
        {
            if (card is BattleCard battleCard)
            {
                if (target is LegendCard legend)
                {
                    legend.LoseHealth(battleCard.Damage);
                }
            }
        }
        static void Heal(object target, Card card)
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
        static void ModifyHealthAndPower(object target, Card card)
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
        static void ModifyHealthAndPowerAllYourLegends(Card card, Game game)
        {
            if (card is BattleCard battleCard)
            {
                List<LegendCard> myLegends = game.ActivePlayer.GetAllLegendInPlayingField();
                foreach (LegendCard legend in myLegends)
                {
                    if (battleCard.TargetElements.Contains(legend.Element))
                    {
                        legend.ModifyStat(Stats.Health, battleCard.HealthModifier);
                        legend.ModifyStat(Stats.Power, battleCard.PowerModifier);
                    }
                }
            }
        }

        // To do

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
        public static void GenerateRandomFireCard(object target, Card card, Game game)
        {
            if (game.ActivePlayer == game.Me)
            {
                // generate random card
                Card generatedCard = CardCatalogue.GetRandomFireCard();
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
    }
}