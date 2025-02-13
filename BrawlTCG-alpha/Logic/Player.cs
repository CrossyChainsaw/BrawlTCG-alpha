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
    internal class Player
    {
        // Fields
        const int STARTING_HEALTH = 20;
        const int STARTING_HAND_CARDS = 20; // 7?
        
        // Properties
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int Essence { get; private set; }
        public List<Card> Deck { get; private set; }
        public List<Card> Hand { get; private set; }
        public List<Card> PlayingField { get; private set; }
        public List<Card> DiscardPile { get; private set; }
        public List<Card> EssenceField { get; private set; }
        bool _playedEssenceCardThisTurn = false;

        // Methods
        public Player(string name, List<Card> deck)
        {
            Name = name;
            Health = STARTING_HEALTH;
            Essence = 0;
            Deck = deck;
            Hand = new List<Card>();
            PlayingField = new List<Card>();
            DiscardPile = new List<Card>();
            EssenceField = new List<Card>();
            ShuffleDeck();
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
        public void DrawStartingHandFromDeck()
        {
            for (int i = 0; i < STARTING_HAND_CARDS; i++)
            {
                DrawCardFromDeck();
            }
        }
        public Card? DrawCardFromDeck()
        {
            if (Deck.Count > 0)
            {
                Card card = Deck[0];
                Deck.RemoveAt(0);
                Hand.Add(card);
                return card;
            }
            return null;
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
            }
            else if (card is LegendCard legendCard)
            {
                PlayingField.Add(legendCard);
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
    }
}
