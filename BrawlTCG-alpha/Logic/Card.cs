using BrawlTCG_alpha.Logic.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;

public enum Elements
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
    public abstract class Card
    {
        // Fields
        static Color MagicColor = Color.DarkViolet;
        static Color NatureColor = Color.Teal; // DarkTurquoise
        static Color FireColor = Color.DarkRed;
        static Color CosmicColor = Color.DarkBlue;
        static Color ShadowColor = Color.FromArgb(30, 30, 30);
        static Color WildColor = Color.Sienna;
        static Color ArcticColor = Color.LightCyan;
        
        // Properties
        public int ID {  get; internal set; }
        public string Name { get; internal set; }
        public int Cost { get; internal set; }
        public string Description { get; internal set; }
        public Elements Element { get; internal set; }
        public bool IsOpen { get; internal set; }
        public bool IsDiscarded { get; set; } = false;
        public Effect? StartTurnEffect { get; internal set; }
        public Action<object>? EndTurnEffect { get; internal set; }
        public Effect? WhenPlayedEffect { get; internal set; }
        public Effect? WhenDiscardedEffect { get; internal set; }
        public Effect? WhileInPlayEffect { get; internal set; }
        public Image Image { get; internal set; }
        public Color CardColor { get; internal set; }
        public Color TextColor { get; internal set; }


        // Methods
        public Card(int id, string name, int cost, Elements element, Image image, Effect? startTurnEffect = null, Action<object>? endTurnEffect = null, Effect? whenPlayedEffect = null, Effect? whenDiscardedEffect = null, Effect? whileInPlayEffect = null)
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
            WhenDiscardedEffect = whenDiscardedEffect;
            WhileInPlayEffect = whileInPlayEffect;
        }
        
        public void OnStartTurn(object target, Card card, Game game) => StartTurnEffect?.Invoke(target, card, game, card);
        public void OnEndTurn(object target) => EndTurnEffect?.Invoke(target);
        public void OnPlayedEffect(object target, Card card, Game game) => WhenPlayedEffect?.Invoke(target, this, game, this);
        public void Discard()
        {
            IsDiscarded = true;
            IsOpen = true;
        }

        public static string GenerateEffectDescription(Card c)
        {
            string description = "";

            // Effects
            if (c.StartTurnEffect != null)
                description += $"Start Turn: {c.StartTurnEffect.Description}\n";
            if (c.EndTurnEffect != null)
                description += $"End Turn: [WORK IN PROGRESS]\n";
            if (c.WhenPlayedEffect != null)
                description += $"When Played: {c.WhenPlayedEffect.Description}\n";
            if (c.WhenDiscardedEffect != null)
                description += $"When Discarded: {c.WhenDiscardedEffect.Description} \n";
            if (c.WhileInPlayEffect != null)
                description += $"While In Play: {c.WhileInPlayEffect.Description}\n";

            return description;
        }
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

        public abstract Card Clone();
    }
}
