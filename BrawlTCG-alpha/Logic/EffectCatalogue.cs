using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic.Cards
{
    public class EffectCatalogue
    {
        // Start Turn Effect
        static int mustafarDamage = 2;
        public static Effect Mustafar = new Effect(
            description: $"All non-Fire Legends lose {mustafarDamage} health.",
            effectAction: (target, card, game, playedCard) => _StartTurnDamage(target, mustafarDamage, Elements.Fire)
        );

        static int fangwildHeal = 2;
        public static Effect Fangwild = new Effect(
            description: $"All Magic and Nature Legends will gain {fangwildHeal} health.",
            effectAction: (target, card, game, playedCard) => _StartTurnHeal(target, fangwildHeal, Elements.Nature, Elements.Magic)
        );

        public static Effect Workshop_StartTurn = new Effect(
            description: $"Instead of drawing a card you obtain a random card",
            effectAction: (target, card, game, playedCard) => {
                GenerateRandomCards(game, 1);
                game.DrawCardStartTurn = false;
            }
        );

        public static Effect Workshop_Discarded = new Effect(
            description: $"N/A",
            effectAction: (target, card, game, playedCard) => {
                game.DrawCardStartTurn = true;
            }
        );

        static int spaceTimeExtraDrawnCards = 2;
        public static Effect SpaceTime = new Effect(
            description: $"Draw {spaceTimeExtraDrawnCards} extra cards",
            effectAction: (target, card, game, playedCard) => DrawCards(game, spaceTimeExtraDrawnCards, startTurn: true)
        );

        public static Effect Essence = new Effect(
            description: $"Gain 1 Essence",
            effectAction: (target, card, game, playedCard) => _GivePlayerEssence(target)
        );

        public static Effect PromotionI = new Effect(
            description: $"Boost player max HP by 5",
            effectAction: (target, card, game, playedCard) => { game.ActivePlayer.BoostMaxHealth(5); });

        public static Effect PromotionII = new Effect(
            description: $"Boost player max HP by 10",
            effectAction: (target, card, game, playedCard) => { game.ActivePlayer.BoostMaxHealth(10); });

        public static Effect BoostFireLegendStats_WhileInPlayEffect = new Effect(
            description: "When you play a Fire legend, that Fire legend gets +2/+2",
            effectAction: (target, card, game, playedCard) => _BoostStats_WhenPlayed(card, playedCard, Elements.Fire));

        public static Effect BoostAllNatureLegendsStats_WhileInPlay = new Effect(
            description: "When you play a Nature card, all nature legends get +1/+1",
            effectAction: (target, card, game, playedCard) => _BoostAllLegendsStats_WhenPlayed(card, game, playedCard, Elements.Nature),
            multipleTriggersInTurn: true);

        static void _BoostStats_WhenPlayed(Card effectOwner, Card playedCard, Elements element)
        {
            if (playedCard is LegendCard legend && playedCard != effectOwner && effectOwner.Owner == playedCard.Owner)
            {
                if (legend.Element == element)
                {
                    legend.ModifyStat(Stats.Power, 2);
                    legend.ModifyStat(Stats.Health, 2);
                }
            }
        }

        static void _BoostAllLegendsStats_WhenPlayed(Card effectOwner, Game game, Card playedCard, Elements element)
        {
            List<LegendCard> legends = game.GetAllMyLegendsOnThePlayingField(game.ActivePlayer);
            foreach (LegendCard legend in legends)
            {
                if (playedCard.Element == element && legend.Element == element && playedCard != effectOwner && effectOwner.Owner == playedCard.Owner)
                {
                    legend.ModifyStat(Stats.Power, 1);
                    legend.ModifyStat(Stats.Health, 1);
                }
            }
        }

        // Evil Hideout
        public static Effect EvilHideout_WhenPlayed = new Effect(
            description: "N/A",
            effectAction: (target, card, game, playedCard) => _ModifyStatsOfAllLegendsWhenPlayed(game, new List<Elements> { Elements.Fire, Elements.Wild, Elements.Shadow }, Stats.Power, 3)
        );
        public static Effect EvilHideout_WhileInPlay = new Effect(
            description: "Shadow, Wild and Fire Legends get +3 Power",
            effectAction: (target, card, game, playedCard) => _EvilHideout_WhilePlay(target)
        );
        public static Effect EvilHideout_WhenDiscarded = new Effect(
            description: "N/A",
            effectAction: (target, card, game, playedCard) => _EvilHideout_WhenDiscard(game)
        );

        // Silent Galaxy
        public static Effect SilentGalaxy_WhenPlayed = new Effect(
            description: $"N/A",
            effectAction: (target, card, game, playedCard) => _SilentGalaxy_WhenPlayed(game)
        );
        public static Effect SilentGalaxy_WhilePlay = new Effect(
            description: $"Only Cosmic Legends can Attack",
            effectAction: (target, card, game, playedCard) => _SilentGalaxy_WhilePlay(target)
        );
        public static Effect SilentGalaxy_WhenDiscarded = new Effect(
            description: $"N/A",
            effectAction: (target, card, game, playedCard) => _SilentGalaxy_WhenDiscard(game)
        );
        public static Effect SilentGalaxy_StartTurn = new Effect(
            description: $"N/A",
            effectAction: (target, card, game, playedCard) => _SilentGalaxy_StartTurn(game)
        );

        // Atlantis
        public static Effect Atlantis_WhenPlayed = new Effect(
            description: "N/A",
            effectAction: (target, card, game, playedCard) =>
                _ModifyStatsOfAllLegendsWhenPlayed(game,
                    Enum.GetValues(typeof(Elements)).Cast<Elements>().Where(e => e != Elements.Arctic).ToList(), // all except arctic
                    Stats.Power, -2)
        );
        public static Effect Atlantis_WhileInPlay = new Effect(
            description: "Shadow, Wild and Fire Legends get +3 Power",
            effectAction: (target, card, game, playedCard) => _Atlantis_WhilePlay(target)
        );
        public static Effect Atlantis_WhenDiscarded = new Effect(
            description: "N/A",
            effectAction: (target, card, game, playedCard) => _Atlantis_WhenDiscard(game)
        );
        static int atlantisDamage = 1;
        static Elements immuneType = Elements.Arctic;
        public static Effect Atlantis_StartTurn = new Effect(
            description: $"All non-{immuneType} Legends lose {atlantisDamage} health.",
            effectAction: (target, card, game, playedCard) => _StartTurnDamage(target, atlantisDamage, immuneType)
        );

        // When Played Effect
        static int mustafarID = 100;
        public static Effect GenerateAndPlayMustafar = new Effect(
            description: $"Change stage to Mustafar",
            effectAction: (target, card, game, playedCard) => GenerateAndPlayStage(game, mustafarID)
        );

        static int matrixID = 102;
        public static Effect GenerateAndPlayMatrix = new Effect(
            description: $"Change stage to Matrix",
            effectAction: (target, card, game, playedCard) => GenerateAndPlayStage(game, matrixID)
        );

        static int workshopID = 103;
        public static Effect GenerateAndPlayWorkshop = new Effect(
            description: $"Change stage to Workshop",
            effectAction: (target, card, game, playedCard) => {
                GenerateAndPlayStage(game, workshopID);
            }
        );

        static int atlantisID = 106;
        public static Effect GenerateAndPlayAtlantis = new Effect(
            description: $"Change stage to Workshop",
            effectAction: (target, card, game, playedCard) => GenerateAndPlayStage(game, atlantisID)
        );

        public static Effect BattleCardDirectDamageWhenPlayed = new Effect(
            description: "Deals direct damage to opposing Legend",
            effectAction: (target, card, game, playedCard) => _DirectDamage(target, card)
        );

        public static Effect BattleCardHealWhenPlayed = new Effect(
            description: "Heals your Legend",
            effectAction: (target, card, game, playedCard) => _Heal(target, card)
        );

        public static Effect BoostHealthAndPower = new Effect(
            description: "Modifies Health and Power",
            effectAction: (target, card, game, playedCard) => _ModifyHealthAndPower(target, card)
        );

        public static Effect BoostHealthAndPowerAllYourLegends = new Effect(
            description: "Modifies Health and Power",
            effectAction: (target, card, game, playedCard) => _ModifyHealthAndPowerAllYourLegends(card, game)
        );

        static int nCards = 3;
        public static Effect CardChest = new Effect(
            description: $"Obtain {nCards} random cards",
            effectAction: (target, card, game, playedCard) => GenerateRandomCards(game, nCards)
        );

        public static Effect Bubble = new Effect(
            description: "Tap legend Card",
            effectAction: (target, card, game, playedCard) => TapLegendCard(target)
        );

        static int nDragonChest = 3;
        public static Effect DragonChest = new Effect(
            description: $"Obtain {nCards} random Fire cards",
            effectAction: (target, card, game, playedCard) => GenerateRandomCards(game, nDragonChest, Elements.Fire)
        );

        static int nWildChest = 3;
        public static Effect WildChest = new Effect(
            description: $"Obtain {nCards} random Wild cards",
            effectAction: (target, card, game, playedCard) => GenerateRandomCards(game, nWildChest, Elements.Wild)
        );

        static int nSunkenChest = 3;
        public static Effect SunkenChest = new Effect(
            description: $"Obtain {nCards} random Arctic cards",
            effectAction: (target, card, game, playedCard) => GenerateRandomCards(game, nSunkenChest, Elements.Arctic)
        );

        static int nShadowChest = 3;
        public static Effect ShadowChest = new Effect(
            description: $"Obtain {nCards} random Shadow cards",
            effectAction: (target, card, game, playedCard) => GenerateRandomCards(game, nShadowChest, Elements.Shadow)
        );

        static int nCosmicChest = 3;
        public static Effect CosmicChest = new Effect(
            description: $"Obtain {nCards} random Cosmic cards",
            effectAction: (target, card, game, playedCard) => GenerateRandomCards(game, nCosmicChest, Elements.Cosmic)
        );

        public static Effect DarkDuo = new Effect(
            description: $"Obtain 2 random Shadow Legends",
            effectAction: (target, card, game, playedCard) => GenerateRandomCards(game, 2, Elements.Shadow, typeof(LegendCard))
        );

        public static Effect WitchParty = new Effect(
            description: $"Obtain Fait, Witch Scarlet, Amethyst Scythe and Galaxy Lance",
            effectAction: (target, card, game, playedCard) =>
            {
                int faitID = 4004;
                GenerateCard(game, faitID);
                int amethystScytheID = 23004;
                GenerateCard(game, amethystScytheID);
                int witchScarletID = 3006;
                GenerateCard(game, witchScarletID);
                int galaxyLanceID = 21000;
                GenerateCard(game, galaxyLanceID);
            }
        );

        public static Effect Adrenaline = new Effect(
            description: $"Allow legend to attack again",
            effectAction: (target, card, game, playedCard) =>
            {
                if (target is LegendCard legend)
                {
                    legend.AttackedThisTurn = false;
                }
            });

        public static Effect DeathsHour = new Effect(
            description: $"Every legend's HP becomes 1",
            effectAction: (target, card, game, playedCard) =>
            {
                List<LegendCard> legends = game.GetAllLegendsOnPlayingField();
                foreach (LegendCard legend in legends)
                {
                    legend.CurrentHP = 1;
                }
            });

        public static Effect CursedKunai = new Effect(
            description: $"Obtain random Katars Legend and random Katars",
            effectAction: (target, card, game, playedCard) =>
            {
                GenerateRandomLegendWithSpecificWeapon(game, 1, Weapons.Katars);
                GenerateRandomWeaponCards(game, 1, Weapons.Katars);
            });



        // Generic Methods
        public static void GenerateRandomCards(Game game, int nCards, Elements? element = null, Type cardType = null)
        {
            if (game.ActivePlayer == game.Me)
            {
                List<int> generatedCardIDs = new List<int>();

                for (int i = 0; i < nCards; i++)
                {
                    // Generate a random elemental card (optionally filtered by element and type)
                    Card generatedCard = CardCatalogue.GetRandomCard(element, cardType);

                    // Store the generated card ID
                    generatedCardIDs.Add(generatedCard.ID);

                    // Add the generated card to the player's hand
                    game.AddCardToHandZone(game.ActivePlayer, generatedCard);
                }

                // Form a single message containing all card IDs
                string message = "RANDOM_CARD_ID:" + string.Join(":", generatedCardIDs);

                // Send the message to the peer
                game.SendMessageToPeer(message);

                // Show the generated cards
                game.ShowCards();
            }
            else
            {
                // Wait until we receive all expected card IDs (Avoid infinite loop!)
                while (game.RandomCardIDs.Count < nCards)
                {
                    Thread.Sleep(10);
                }

                // Retrieve all generated cards based on the received random card IDs
                List<Card> generatedCards = game.RandomCardIDs
                    .Select(id => CardCatalogue.GetCardById(id))
                    .Where(card => card != null)
                    .ToList();

                // Add each generated card to the player's hand
                foreach (Card generatedCard in generatedCards)
                {
                    game.AddCardToHandZone(game.ActivePlayer, generatedCard);
                }

                // Clear the list instead of setting it to null
                game.RandomCardIDs.Clear();
            }
        }
        public static void GenerateRandomLegendWithSpecificWeapon(Game game, int nCards, Weapons weapon, Elements? element = null)
        {
            if (game.ActivePlayer == game.Me)
            {
                List<int> generatedCardIDs = new List<int>();

                for (int i = 0; i < nCards; i++)
                {
                    // Generate a random elemental card (optionally filtered by element and type)
                    Card generatedCard = CardCatalogue.GetRandomLegendCard(weapon);

                    // Store the generated card ID
                    generatedCardIDs.Add(generatedCard.ID);

                    // Add the generated card to the player's hand
                    game.AddCardToHandZone(game.ActivePlayer, generatedCard);
                }

                // Form a single message containing all card IDs
                string message = "RANDOM_CARD_ID:" + string.Join(":", generatedCardIDs);

                // Send the message to the peer
                game.SendMessageToPeer(message);

                // Show the generated cards
                game.ShowCards();
            }
            else
            {
                // Wait until we receive all expected card IDs (Avoid infinite loop!)
                while (game.RandomCardIDs.Count < nCards)
                {
                    Thread.Sleep(10);
                }

                // Retrieve all generated cards based on the received random card IDs
                List<Card> generatedCards = game.RandomCardIDs
                    .Select(id => CardCatalogue.GetCardById(id))
                    .Where(card => card != null)
                    .ToList();

                // Add each generated card to the player's hand
                foreach (Card generatedCard in generatedCards)
                {
                    game.AddCardToHandZone(game.ActivePlayer, generatedCard);
                }

                // Clear the list instead of setting it to null
                game.RandomCardIDs.Clear();
            }
        }

        public static void GenerateRandomWeaponCards(Game game, int nCards, Elements? element = null)
        {
            if (game.ActivePlayer == game.Me)
            {
                List<int> generatedCardIDs = new List<int>();

                for (int i = 0; i < nCards; i++)
                {
                    // Generate a random elemental card (optionally filtered by element and type)
                    Card generatedCard = CardCatalogue.GetRandomWeaponCard();

                    // Store the generated card ID
                    generatedCardIDs.Add(generatedCard.ID);

                    // Add the generated card to the player's hand
                    game.AddCardToHandZone(game.ActivePlayer, generatedCard);
                }

                // Form a single message containing all card IDs
                string message = "RANDOM_CARD_ID:" + string.Join(":", generatedCardIDs);

                // Send the message to the peer
                game.SendMessageToPeer(message);

                // Show the generated cards
                game.ShowCards();
            }
            else
            {
                // Wait until we receive all expected card IDs (Avoid infinite loop!)
                while (game.RandomCardIDs.Count < nCards)
                {
                    Thread.Sleep(10);
                }

                // Retrieve all generated cards based on the received random card IDs
                List<Card> generatedCards = game.RandomCardIDs
                    .Select(id => CardCatalogue.GetCardById(id))
                    .Where(card => card != null)
                    .ToList();

                // Add each generated card to the player's hand
                foreach (Card generatedCard in generatedCards)
                {
                    game.AddCardToHandZone(game.ActivePlayer, generatedCard);
                }

                // Clear the list instead of setting it to null
                game.RandomCardIDs.Clear();
            }
        }
        public static void GenerateRandomWeaponCards(Game game, int nCards, Weapons weapon, Elements? element = null)
        {
            if (game.ActivePlayer == game.Me)
            {
                List<int> generatedCardIDs = new List<int>();

                for (int i = 0; i < nCards; i++)
                {
                    // Generate a random wep card
                    Card generatedCard = CardCatalogue.GetRandomWeaponCard(weapon);

                    // Store the generated card ID
                    generatedCardIDs.Add(generatedCard.ID);

                    // Add the generated card to the player's hand
                    game.AddCardToHandZone(game.ActivePlayer, generatedCard);
                }

                // Form a single message containing all card IDs
                string message = "RANDOM_CARD_ID:" + string.Join(":", generatedCardIDs);

                // Send the message to the peer
                game.SendMessageToPeer(message);

                // Show the generated cards
                game.ShowCards();
            }
            else
            {
                // Wait until we receive all expected card IDs (Avoid infinite loop!)
                while (game.RandomCardIDs.Count < nCards)
                {
                    Thread.Sleep(10);
                }

                // Retrieve all generated cards based on the received random card IDs
                List<Card> generatedCards = game.RandomCardIDs
                    .Select(id => CardCatalogue.GetCardById(id))
                    .Where(card => card != null)
                    .ToList();

                // Add each generated card to the player's hand
                foreach (Card generatedCard in generatedCards)
                {
                    game.AddCardToHandZone(game.ActivePlayer, generatedCard);
                }

                // Clear the list instead of setting it to null
                game.RandomCardIDs.Clear();
            }
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
        public static void GenerateAndPlayStage(Game game, int cardID)
        {
            // first give the player essence before playing it!
            StageCard generatedCard = (StageCard)CardCatalogue.GetCardById(cardID);
            StageCard card2 = generatedCard;
            game.ActivePlayer.GainEssence(card2.Cost);
            game.AddCardToHandZone(game.ActivePlayer, card2);
            game.PlayStageCard(card2);
        }
        public static void GenerateCard(Game game, int cardID)
        {
            Card card = CardCatalogue.GetCardById(cardID);
            // add to hand
            game.AddCardToHandZone(game.ActivePlayer, card);
            // flip to show
            game.ShowCards();
        }
        public static void DrawCards(Game game, int n, bool startTurn = false)
        {
            if (startTurn && game.ActivePlayer.Hand.Count > 15)
            {

            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    game.DrawCardFromDeck(game.ActivePlayer);
                }
                game.ShowCards();
            }
        }
        public static void TapLegendCard(object target)
        {
            if (target is LegendCard legend)
            {
                legend.TapOut();
            }
        }
        static void _StartTurnHeal(object target, int heal, params Elements[] targetElements)
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
        static void _StartTurnDamage(object target, int damage, params Elements[] immuneElements)
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
        static void _GivePlayerEssence(object target)
        {
            if (target is Player player)
            {
                player.GainEssence(1);
            }
        }
        static void _ModifyStatsOfAllLegendsWhenPlayed(Game game, List<Elements> targetElements, Stats stat, int modifier)
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
        static void _DirectDamage(object target, Card card)
        {
            if (card is BattleCard battleCard)
            {
                if (target is LegendCard legend)
                {
                    legend.LoseHealth(battleCard.Damage);
                }
            }
        }
        static void _Heal(object target, Card card)
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
        static void _ModifyHealthAndPower(object target, Card card)
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
        static void _ModifyHealthAndPowerAllYourLegends(Card card, Game game)
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
        // Atlantis
        static void _Atlantis_WhilePlay(object target)
        {
            if (target is LegendCard legend)
            {
                List<Elements> atlantisTargetElements = new List<Elements> { Elements.Arctic };

                if (!atlantisTargetElements.Contains(legend.Element))
                {
                    legend.ModifyStat(Stats.Power, -2);
                }
            }
        }
        static void _Atlantis_WhenDiscard(Game game)
        {
            // target is everyone

            // my cards
            foreach (Card c in game.Me.PlayingField)
            {
                if (c is LegendCard legend)
                {
                    List<Elements> atlantisTargetElements = new List<Elements> { Elements.Arctic };

                    if (!atlantisTargetElements.Contains(legend.Element))
                    {
                        legend.ModifyStat(Stats.Power, 2);
                    }
                }
            }
            // enemy cards
            foreach (Card c in game.Opponent.PlayingField)
            {
                if (c is LegendCard legend)
                {
                    List<Elements> atlantisTargetElements = new List<Elements> { Elements.Arctic };

                    if (!atlantisTargetElements.Contains(legend.Element))
                    {
                        legend.ModifyStat(Stats.Power, 2);
                    }
                }
            }
        }
        // Evil Hideout
        static void _EvilHideout_WhilePlay(object target)
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
        static void _EvilHideout_WhenDiscard(Game game)
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
        // Silent Galaxy
        static void _SilentGalaxy_StartTurn(Game game)
        {
            List<LegendCard> legends = game.GetAllLegendsOnPlayingField();
            foreach (LegendCard legend in legends)
            {
                if (legend.Element != Elements.Cosmic)
                {
                    legend.CanAttack = false;
                }
            }
        }
        static void _SilentGalaxy_WhilePlay(object target)
        {
            if (target is LegendCard legend)
            {
                if (legend.Element != Elements.Cosmic)
                {
                    legend.CanAttack = false;
                }
            }
        }
        static void _SilentGalaxy_WhenDiscard(Game game)
        {
            List<LegendCard> legends = game.GetAllLegendsOnPlayingField();
            foreach (LegendCard legend in legends)
            {
                legend.CanAttack = true;
            }
        }
        static void _SilentGalaxy_WhenPlayed(Game game)
        {
            List<LegendCard> legends = game.GetAllLegendsOnPlayingField();
            foreach (LegendCard legend in legends)
            {
                if (legend.Element != Elements.Cosmic)
                {
                    legend.CanAttack = false;
                }
            }
        }
    }
}