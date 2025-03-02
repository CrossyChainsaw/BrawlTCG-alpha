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
    Arctic,
    Wild,
    Shadow,
}

namespace BrawlTCG_alpha.Logic
{
    internal abstract class Card
    {
        public int ID {  get; internal set; }
        public string Name { get; internal set; }
        public int Cost { get; internal set; }
        public string Description { get; internal set; }
        public Elements Element { get; internal set; }
        public bool IsOpen { get; internal set; }
        public bool IsDiscarded { get; set; } = false;
        public Action<object, Card, Game>? StartTurnEffect { get; internal set; }
        public Action<object>? EndTurnEffect { get; internal set; }
        public Action<object, Card, Game>? WhenPlayedEffect { get; internal set; }
        public Image Image { get; internal set; }
        public Color CardColor { get; internal set; }
        public Color TextColor { get; internal set; }

        static Color MagicColor = Color.DarkViolet;
        static Color NatureColor = Color.Teal; // DarkTurquoise
        static Color FireColor = Color.DarkRed;
        static Color CosmicColor = Color.DarkBlue;
        static Color ShadowColor = Color.FromArgb(30, 30, 30);
        static Color WildColor = Color.Sienna;
        static Color ArcticColor = Color.LightCyan;


        public Card(int id, string name, int cost, Elements element, Image image, Action<object, Card, Game>? startTurnEffect = null, Action<object>? endTurnEffect = null, Action<object, Card, Game>? whenPlayedEffect = null)
        {
            ID = id;
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

        public void OnStartTurn(object target, Card card, Game game) => StartTurnEffect?.Invoke(target, card, game);
        public void OnEndTurn(object target) => EndTurnEffect?.Invoke(target);
        public void OnPlayedEffect(object target, Card card, Game game) => WhenPlayedEffect?.Invoke(target, this, game);
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
            else if (element == Elements.Shadow)
            {
                return ShadowColor;
            }
            else if (element == Elements.Wild)
            {
                return WildColor;
            }
            else if (element == Elements.Wild)
            {
                return WildColor;
            }
            else if (element == Elements.Arctic)
            {
                return ArcticColor;
            }
            throw new Exception("This element has no card color");
        }
        public static Color SetTextColor(Elements element)
        {
            if (element == Elements.Magic || element == Elements.Fire || element == Elements.Nature || element == Elements.Wild || element == Elements.Arctic)
            {
                return Color.Black;
            }
            else if (element == Elements.Cosmic || element == Elements.Shadow)
            {
                return Color.White;
            }
            throw new Exception("This element has no text color");
        }
    }
}
