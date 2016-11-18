using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GameIntestines
{

    class CardLoader
    {
        GameRepresentation Game;
        GameEngineFunctions Engine;
        XDocument Xdoc;
        public CardLoader(GameEngineFunctions Engine, GameRepresentation Game)
        {
            this.Game = Game;
            this.Engine = Engine;
        }
        public void LoadCards(string XdocName)
        {
            Xdoc = XDocument.Load("XMLCardBase.xml");
            var rawCards = from cr in Xdoc.Descendants("Card") select cr;
            //card loading
            foreach (XElement item in rawCards)
            {
                Card newcard = new Card();
                string s = item.Element("Name").Value;
                newcard.name = s;
                foreach (var tag in item.Element("Tags").Descendants())
                {
                    newcard.tags.Add(tag.Name.ToString());
                    if (tag.Name.ToString() == "Spell_Damage")
                    {
                        newcard.defaultSpelldmg = Convert.ToInt32(tag.Value);
                        newcard.currentSpelldmg = newcard.defaultSpelldmg;
                    }
                }
                if (newcard.tags.Contains("Minion"))
                {
                    newcard.manacost = Convert.ToInt32(item.Element("Manacost").Value);
                    newcard.baseattack = Convert.ToInt32(item.Element("Attack").Value);
                    newcard.basehitpoints = Convert.ToInt32(item.Element("HP").Value);
                    newcard.currentattack = newcard.baseattack;
                    newcard.currenthitpoints = newcard.basehitpoints;
                    //TODO skills

                }
                else
                {
                    if (newcard.tags.Contains("Spell"))
                    {
                        //the card is a spell
                        newcard.manacost = Convert.ToInt32(item.Element("Manacost").Value);
                        if (item.Element("TargetNeeded").Value == "YES")
                        {
                            newcard.needsTargetSelected = true;
                        }
                        else
                        {
                            newcard.needsTargetSelected = false;
                        }
                    }
                    //TODO spells
                }
                //Loading skills
                if (item.Element("Skills") != null)
                {
                    //Card has some skills -> load them one by one
                    foreach (XElement skill in item.Element("Skills").Elements("Skill"))
                    {
                        Abbility abb = ParseAbbility(skill, newcard);
                        abb.Owner = newcard;
                        newcard.Skills.Add(abb);
                    }
                }
                //Add the card to the Card Database
                newcard.TemplateInstance = true;
                Game.AllCards.Add(newcard);
            }
        }
        public Abbility ParseAbbility(XElement Abbilityxml,Card AssociatedCard)
        {
            Abbility ab = new Abbility(Abbilityxml.Element("Type").Value,Engine,Game);
            if (ab.Kind == "Triggered")
            {
                ab.Trigger = Abbilityxml.Element("Trigger").Value;
            }
            foreach (XElement tag in Abbilityxml.Element("Target").Element("Tags").Descendants())
            {
                ab.TargetTags.Add(tag.Name.ToString());
            }
            foreach (XElement effect in Abbilityxml.Element("Effect").Elements("Function"))
            {
                ab.Effects.Add(effect);
                ab.Functions.Add(ParseFunction(effect,Engine,Game,ab,AssociatedCard));
            }
            return ab;
        }
        public CardFunctions ParseFunction(XElement effect, GameEngineFunctions Engine, GameRepresentation Game, Abbility AssociatedAbbility,Card Asscard)
        {
            switch (effect.Value)
            {
                case "Heal":
                    return new Heal(Asscard, AssociatedAbbility.TargetTags,Convert.ToInt32(effect.Attribute("value").Value),effect.Attribute("selector").Value, Convert.ToInt32(effect.Attribute("targets").Value));
                case "Draw_Card":
                    return new DrawCard(Convert.ToInt32(effect.Attribute("value").Value),  AssociatedAbbility.TargetTags,effect.Attribute("selector").Value,Asscard);
                //todo
                case "Deal_Damage":
                    return new DealDamage(Convert.ToInt32(effect.Attribute("value").Value), Convert.ToInt32(effect.Attribute("targets").Value), effect.Attribute("selector").Value, Asscard, AssociatedAbbility.TargetTags) ;
                case "Give_Buff":
                    int AttackValue = Convert.ToInt32(effect.Attribute("Attack").Value);
                    int HPValue = Convert.ToInt32(effect.Attribute("Defense").Value);
                    string AttackModType = effect.Attribute("AttackModificationType").Value;
                    string HPModType = effect.Attribute("DefenseModificationType").Value;
                    string Duration = effect.Attribute("duration").Value;
                    string Selector = effect.Attribute("selector").Value;
                    int TargetCount = Convert.ToInt32(effect.Attribute("targets").Value); 
                    return new GiveBuff(AssociatedAbbility.TargetTags, Asscard, AttackValue, AttackModType, HPValue, HPModType , Selector, Duration,TargetCount) ;
                case "Summon":
                    return new Summon(effect.Attribute("summonedCreatureName").Value, Asscard, AssociatedAbbility.TargetTags);
                case "Discard":
                    return new Discard(Asscard, AssociatedAbbility.TargetTags, Convert.ToInt32(effect.Attribute("value").Value), effect.Attribute("selector").Value);
                case "Destroy":
                    return new Destroy(Asscard, AssociatedAbbility.TargetTags, Convert.ToInt32(effect.Attribute("targets").Value), effect.Attribute("selector").Value);
                case "CountAndReplace":
                    return new CountAndReplace(AssociatedAbbility.TargetTags, Asscard, new List<string>( effect.Attribute("WhatToReplace").Value.Split(new[] {"," },StringSplitOptions.RemoveEmptyEntries)), effect.Attribute("FName").Value, Convert.ToInt32(effect.Attribute("SPos").Value), Convert.ToInt32(effect.Attribute("FPos").Value));
                case "Freeze":
                    return new Freeze(Asscard, effect.Attribute("selector").Value, Convert.ToInt32(effect.Attribute("targets").Value), AssociatedAbbility.TargetTags);
                case "Transform":
                    return new Transform(Asscard, effect.Attribute("selector").Value, Convert.ToInt32(effect.Attribute("targets").Value), effect.Attribute("summonedCreatureName").Value, AssociatedAbbility.TargetTags);
                case "Give_Tag":
                    return new GiveTag(AssociatedAbbility.TargetTags, effect.Attribute("selector").Value, Convert.ToInt32(effect.Attribute("targets").Value), Asscard, effect.Attribute("addedTag").Value);
                default:
                    return null;
                

            }
        }
    }
}
