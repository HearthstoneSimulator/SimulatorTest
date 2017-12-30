using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;

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
        /// <summary>
        /// This will load a state of game into given GameRepresentation instance from a given XML file.
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="Game">Instance of this GameRepresentation will be modified to match the gamestate of the XML file. This instance has to have card database loaded else it will throw an error.</param>
        /// <param name="XdocName">Name of the XML file which contains desired gamestate. By default set to "Gamestate.xml."</param>
        public void LoadGameRepresenation(GameEngine Engine, GameRepresentation Game, string XdocName)
        {

        }
        public void LoadDecks()
        {
            if (Game.decklists.Count == 2)
            {
                for (int j = 0; j < 2; j++)
                {

                    if (Game.decklists[j].name != "random")
                    {

                        StreamReader Twilight = new StreamReader(Game.decklists[j].name);
                        string cardName = Twilight.ReadLine();
                        if (cardName == "Shaman" || cardName == "Mage" || cardName == "Warlock")
                        {
                            //we can load hero
                            Card heroCard = new Card();
                            if (cardName == "Shaman")
                            {
                                cardName = "Totemic Call";
                            }
                            if (cardName == "Mage")
                            {
                                cardName = "Fireblast";
                            }
                            if (cardName == "Warlock")
                            {
                                cardName = "Life Tap";
                            }
                            for (int i = 0; i < Game.AllCards.Count; i++)
                            {
                                if (Game.AllCards[i].name == cardName)
                                {
                                    Game.heroPowers.Add(Game.AllCards[i].Clone());                                    
                                    break;//maybe add log that card wasnt sucessfully found if such thing happens?
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("First line of decklist was not valid hero class. Automatically selected Mage as hero class instead.");
                            for (int i = 0; i < Game.AllCards.Count; i++)
                            {
                                if (Game.AllCards[i].name == "Fireblast")
                                {
                                    Game.heroPowers.Add(Game.AllCards[i].Clone());
                                    break;//maybe add log that card wasnt sucessfully found if such thing happens?
                                }
                            }
                        }
                        while (!Twilight.EndOfStream)
                        {
                            cardName = Twilight.ReadLine();
                            bool successAdd = false;
                            for (int i = 0; i < Game.AllCards.Count; i++)
                            {
                                if (Game.AllCards[i].name == cardName)
                                {
                                    Game.decklists[j].cards.Add(Game.AllCards[i].Clone());
                                    successAdd = true;
                                    break;
                                }
                            }
                            if (!successAdd)
                            {
                                Console.WriteLine("Card {0} could not be added to the deck because it didn't match any card name in the card database.", cardName);
                            }
                        }
                    }
                    else
                    {
                        Random rng = new Random(j);
                        
                        for (int i = 0; i < 30; i++)
                        {
                            Game.decklists[j].cards.Add(Game.AllCards[rng.Next(Game.AllCards.Count)].Clone());
                        }
                    }
                }

            }
        }

        public GameLoadState LoadGameState(string XdocName)
        {
            GameLoadState loadedState = new GameLoadState();
            Xdoc = XDocument.Load(XdocName);
            var state = from cr in Xdoc.Descendants("GameState") select cr;
            foreach (XElement item in state)
            {
                XElement tmp = item.Element("Player0");

                loadedState.P0HP = Convert.ToInt32(tmp.Element("HPCurrent").Value);
                loadedState.P0MAX = Convert.ToInt32(tmp.Element("HPMax").Value);
                loadedState.ManaCrystals[0].total = Convert.ToInt32(tmp.Element("ManaTotal").Value);
                loadedState.ManaCrystals[0].availible = Convert.ToInt32(tmp.Element("ManaCurrent").Value);
                loadedState.Fatigues[0] = Convert.ToInt32(tmp.Element("Fatigue").Value);

                foreach (var removedCard in tmp.Element("Deck").Descendants("CardName"))
                {
                    loadedState.P0CardsToRemoveFromDeck.Add(removedCard.Value);
                }
                foreach (var handCard in tmp.Element("Hand").Descendants("CardName"))
                {
                    loadedState.P0Hand.Add(handCard.Value);
                }
                foreach (var fieldCard in tmp.Element("Field").Descendants("Card"))
                {
                    Card dummyCard = new Card();
                    dummyCard.name = fieldCard.Element("CardName").Value;
                    dummyCard.baseattack = Convert.ToInt32(fieldCard.Element("MaxAT").Value);
                    dummyCard.currentattack = Convert.ToInt32(fieldCard.Element("CurrAT").Value);
                    dummyCard.basehitpoints = Convert.ToInt32(fieldCard.Element("MaxHP").Value);
                    dummyCard.currenthitpoints = Convert.ToInt32(fieldCard.Element("CurrHP").Value);

                    loadedState.P0Board.Add(dummyCard);
                }

                tmp = item.Element("Player1");
                loadedState.P1HP = Convert.ToInt32(tmp.Element("HPCurrent").Value);
                loadedState.P1MAX = Convert.ToInt32(tmp.Element("HPMax").Value);
                loadedState.ManaCrystals[1].total = Convert.ToInt32(tmp.Element("ManaTotal").Value);
                loadedState.ManaCrystals[1].availible = Convert.ToInt32(tmp.Element("ManaCurrent").Value);

                foreach (var removedCard in tmp.Element("Deck").Descendants("CardName"))
                {
                    loadedState.P1CardsToRemoveFromDeck.Add(removedCard.Value);
                }
                foreach (var handCard in tmp.Element("Hand").Descendants("CardName"))
                {
                    loadedState.P1Hand.Add(handCard.Value);
                }
                foreach (var fieldCard in tmp.Element("Field").Descendants("Card"))
                {
                    Card dummyCard = new Card();
                    dummyCard.name = fieldCard.Element("CardName").Value;
                    dummyCard.baseattack = Convert.ToInt32(fieldCard.Element("MaxAT").Value);
                    dummyCard.currentattack = Convert.ToInt32(fieldCard.Element("CurrAT").Value);
                    dummyCard.basehitpoints = Convert.ToInt32(fieldCard.Element("MaxHP").Value);
                    dummyCard.currenthitpoints = Convert.ToInt32(fieldCard.Element("CurrHP").Value);

                    loadedState.P1Board.Add(dummyCard);
                }

                tmp = item.Element("Actions");
                foreach (var playAction in tmp.Descendants("Play"))
                {
                    PlayCardActionForLoadingGamestate pcfl = new PlayCardActionForLoadingGamestate();
                    pcfl.CardName = playAction.Element("CardName").Value;
                    pcfl.PossibleTarget = Convert.ToInt32(playAction.Element("TargetPosition").Value);
                    pcfl.Owner = Convert.ToInt32(playAction.Element("Owner").Value);
                    if (pcfl.Owner == 0)
                    {
                        loadedState.P0Actions.Add(pcfl);
                    }
                    else
                    {
                        if (pcfl.Owner == 1)
                        {
                            loadedState.P1Actions.Add(pcfl);

                        }
                        else
                        {
                            Console.WriteLine("PlayAction had invalid owner number and was not added");
                        }
                    }
                }
            }
            return loadedState;
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
                    else
                    {
                        if (newcard.tags.Contains("HeroPower"))
                        {
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
                case "Totemic Call":
                    return new TotemicCall(Asscard, AssociatedAbbility.TargetTags);
                default:
                    throw new Exception("Tried to load unknow card function");
                    //return null;
                

            }
        }
    }
}
