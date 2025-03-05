using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;

namespace BrawlTCG_alpha.Logic
{
    public class Player
    {
        // Fields
        const int STARTING_HEALTH = 30;
        const int MAX_CARDS_IN_HAND = 15;

        // Properties
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int Essence { get; private set; }
        public List<Card> Deck { get; private set; }
        public List<Card> Hand { get; private set; }
        public List<Card> PlayingField { get; private set; }
        public List<Card> DiscardPile { get; private set; }
        public List<Card> EssenceField { get; private set; }
        public bool IsHost { get; private set; }
        public bool IsMe { get; private set; }

        bool _playedEssenceCardThisTurn = false;


        // Methods
        public Player(string name, List<Card> deck, bool isHost, bool isMe)
        {
            Name = name;
            Health = STARTING_HEALTH;
            Essence = 0;
            Deck = deck;
            Hand = new List<Card>();
            PlayingField = new List<Card>();
            DiscardPile = new List<Card>();
            EssenceField = new List<Card>();
            IsHost = isHost;
            IsMe = isMe;
        }
        public void ShuffleDeck()
        {
            Random rng = new Random();
            int n = Deck.Count;

            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (Deck[i], Deck[j]) = (Deck[j], Deck[i]);
            }
        }
        public Card? DrawCardFromDeck()
        {
            if (Hand.Count < MAX_CARDS_IN_HAND)
            {
                if (Deck.Count > 0)
                {
                    int lastCardIndex = Deck.Count - 1;
                    Card card = Deck[lastCardIndex];
                    Deck.RemoveAt(lastCardIndex);
                    Hand.Add(card);
                    return card;
                }
            }
            return null;
        }

        /// <summary>
        /// For adding a specific card to your hand
        /// </summary>
        public void AddCardToHand(Card card)
        {
            Hand.Add(card);
        }
        public void RemoveEssence(Card card)
        {
            Essence = Essence - card.Cost;
        }
        /// <summary>Remove Card from Hand, Add Card to Essence Field or PlayingField if it's that card</summary>
        public void PlayCard(Card card)
        {
            if (card is EssenceCard essenceCard)
            {
                EssenceField.Add(essenceCard);
                _playedEssenceCardThisTurn = true;
            }
            else if (card is LegendCard legendCard)
            {
                PlayingField.Add(legendCard);
                legendCard.OnPlayingField = true;
            }
            else if (card is BattleCard battleCard)
            {
                if (battleCard.OneTimeUse)
                {
                    DiscardPile.Add(battleCard);
                }
                else if (battleCard.Stackable)
                {
                    // don't add to discard pile
                }
                else
                {
                    throw new Exception();
                }
            }
            Hand.Remove(card);
            RemoveEssence(card);
        }
        public void GainEssence(int gain)
        {
            Essence += gain;
        }
        public void LoseHealth(int damage)
        {
            Health -= damage;
        }
        public bool PlayedEssenceCardThisTurn()
        {
            return _playedEssenceCardThisTurn;
        }
        public void PlayedEssenceCardThisTurn(bool boolean)
        {
            _playedEssenceCardThisTurn = boolean;
        }
        public void GetEssence()
        {
            this.Essence = EssenceField.Count;
        }
        public List<Card> GetAllCardsInPlayingField()
        {
            List<Card> cards = new List<Card>();
            foreach (Card card in PlayingField)
            {
                cards.Add(card);
            }
            return cards;
        }
    }
}
