using BrawlTCG_alpha.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

enum Elements
{
    Fire,
    Magic,
    Nature,
    Electric
}

namespace BrawlTCG_alpha.Logic
{
    internal abstract class Card
    {
        public string Name { get; internal set; }
        public int Cost { get; internal set; }
        public string Description { get; internal set; }
        public Elements Element { get; internal set; }
        public Action<object> StartTurnEffect { get; internal set; }
        public Action<object> EndTurnEffect { get; internal set; }
        public Action<object> WhenPlayedEffect { get; internal set; }
        public Image Image { get; internal set; }
        public Color CardColor { get; internal set; }

        public Card(string name, int cost, string description, Elements element, Image image, Action<object> startTurnEffect = null, Action<object> endTurnEffect = null, Action<object> whenPlayedEffect = null)
        {
            Name = name;
            Cost = cost;
            Description = description;
            Element = element;
            Image = image;
            // Effects
            StartTurnEffect = startTurnEffect;
            EndTurnEffect = endTurnEffect;
            WhenPlayedEffect = whenPlayedEffect;
        }

        public void OnStartTurn(object target) => StartTurnEffect?.Invoke(target);
        public void OnEndTurn(object target) => EndTurnEffect?.Invoke(target);
        public void OnPlayed(object target) => WhenPlayedEffect?.Invoke(target);
        public abstract Card Clone();
    }
}
