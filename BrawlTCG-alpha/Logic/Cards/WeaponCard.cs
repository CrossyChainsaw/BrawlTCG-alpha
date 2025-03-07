﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BrawlTCG_alpha.Logic.Cards
{
    public enum Weapons
    {
        Any,
        Blasters,
        RocketLance,
        Spear,
        Katars,
        Axe,
        Bow,
        Gauntlets,
        Scythe,
        Sword,
        Cannon,
        Orb,
        Greatsword,
        BattleBoots,
        Hammer,
    }

    public class WeaponCard : Card
    {
        // Properties
        public Weapons Weapon { get; internal set; }

        // Methods
        public WeaponCard(int id, string name, int cost, Elements element, Image image, Weapons weapon) : base(id, name, cost, element, image)
        {
            Description = GetWeaponCardDescription(weapon);
            Weapon = weapon;
        }
        string GetWeaponCardDescription(Weapons weapon)
        {
            return $"Play this on {GetArticle(weapon.ToString())} {weapon.ToString()} Legend to unlock attacks";
        }
        public string GetArticle(string word)
        {
            // Convert the first letter of the word to lowercase
            word = word.Trim().ToLower();

            // Define a list of vowels
            string vowels = "aeiou";

            // Check if the first letter of the word is a vowel sound
            if (vowels.Contains(word[0]))
            {
                return "an";
            }

            // Otherwise, use "a"
            return "a";
        }
        public override Card Clone()
        {
            return new WeaponCard(ID, Name, Cost, Element, Image, Weapon);
        }
    }
}
