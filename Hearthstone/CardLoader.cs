using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Hearthstone
{

    class CardLoader
    {
        GameRepresentation Game;
        GameEngineFunctions Engine;
        XDocument Xdoc;
        public CardLoader( GameRepresentation Game)
        {
            this.Game = Game;
            Engine = new GameEngineFunctions();
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
                Game.AllCards.Add(newcard);
            }
        }
        public Abbility ParseAbbility(XElement Abbilityxml,Card AssociatedCard)
        {
            Abbility ab = new Abbility(Abbilityxml.Element("Type").Value,Engine,Game);
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
                    return new GiveBuff();
                case "Summon":
                    return new Summon(effect.Attribute("summonedCreatureName").Value, Asscard, AssociatedAbbility.TargetTags);
                default:
                    return null;
                

            }
        }
    }
}
