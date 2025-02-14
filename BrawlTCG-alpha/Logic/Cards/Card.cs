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
    Cosmic,
    Water,
    Wild,
}

namespace BrawlTCG_alpha.Logic
{
    internal abstract class Card
    {
        public string Name { get; internal set; }
        public int Cost { get; internal set; }
        public string Description { get; internal set; }
        public Elements Element { get; internal set; }
        public bool IsOpen { get; internal set; }
        public Action<object>? StartTurnEffect { get; internal set; }
        public Action<object>? EndTurnEffect { get; internal set; }
        public Action<object>? WhenPlayedEffect { get; internal set; }
        public Image Image { get; internal set; }
        public Color CardColor { get; internal set; }
        public Color TextColor { get; internal set; }

        static Color MagicColor = Color.DarkViolet;
        static Color NatureColor = Color.Teal; // DarkTurquoise
        static Color FireColor = Color.DarkRed;
        static Color CosmicColor = Color.DarkBlue;


        public Card(string name, int cost, Elements element, Image image, Action<object>? startTurnEffect = null, Action<object>? endTurnEffect = null, Action<object>? whenPlayedEffect = null)
        {
            Name = name;
            Cost = cost;
            Element = element;
            Image = image;
            CardColor = SetCardColor(element);
            TextColor = SetTextColor(element);
            // Effects
            StartTurnEffect = startTurnEffect;
            EndTurnEffect = endTurnEffect;
            WhenPlayedEffect = whenPlayedEffect;
        }

        public void OnStartTurn(object target) => StartTurnEffect?.Invoke(target);
        public void OnEndTurn(object target) => EndTurnEffect?.Invoke(target);
        public void OnPlayed(object target) => WhenPlayedEffect?.Invoke(target);
        public abstract Card Clone();
        public static Color SetCardColor(Elements element)
        {
            if (element == Elements.Magic)
            {
                return MagicColor;
            }
            else if (element == Elements.Nature)
            {
                return NatureColor;
            }
            else if (element == Elements.Fire)
            {
                return FireColor;
            }
            else if (element == Elements.Cosmic)
            {
                return CosmicColor;
            }
            throw new Exception("This element has no color");
        }
        public static Color SetTextColor(Elements element)
        {
            if (element == Elements.Magic || element == Elements.Fire || element == Elements.Nature)
            {
                return Color.Black;
            }
            else if (element == Elements.Cosmic)
            {
                return Color.White;
            }
            throw new Exception("This element has no color");
        }
    }
}
