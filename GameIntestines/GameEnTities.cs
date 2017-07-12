using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameIntestines
{
    public enum Tags
    {
        //Target related asdr s
        Self, Other, Any, Own, Enemy

    }

    public enum Triggers
    {
        DamageTaken, MonsterPlayed
    }

    public class Card
    {
        public bool TemplateInstance = false;
        public string name; //jméno karty a zároveň identifikátor
        public int baseattack;
        public int basehitpoints;
        public int currentattack;
        public int currenthitpoints;
        public int defaultHP;
        public int defaultAttack;
        public int defaultSpelldmg;
        public int currentSpelldmg;
        public int frozen;
        public bool needsTargetSelected;

        public int armor;
        public CardType typkarty;
        public bool divineshield;
        public bool invulnerable;
        public int manacost;
        public int turnsingame = 0;
        public bool charge = false;
        public int AttackedXTimes = 0;
        public List<string> tags = new List<string>();
        public List<Tags> tags2 = new List<Tags>();
        public List<Abbility> Skills = new List<Abbility>();
        public int fatigue = 0;
        public List<Abbility> Auras = new List<Abbility>();
        public List<StatBuffWrapper> ListOfStatBuffs = new List<StatBuffWrapper>();
        public int owner;
        public int controller;
        public enum Target { SINGLE, NONE };
        public Target target;
        public int tmpCount = 0;
        public Card(int i)
        {
            this.name = "dummy " + i;
        }
        public Card()
        {
            this.name = "";
        }
        public string getAllText()
        {
            string outstr = "";
            outstr = outstr + "name: " + name + "\n" + "baseattack: " + baseattack
                + "\n CurrentAttack: " + currentattack + "\n BaseHP: " + basehitpoints +
                "\n currhp: " + currenthitpoints;
            return outstr;

        }
        public override string ToString()
        {
            return name;
        }
        public Card Clone()
        {
            Card cc = new Card();
            cc.defaultAttack = this.defaultAttack;
            cc.needsTargetSelected = this.needsTargetSelected;
            cc.defaultSpelldmg = this.defaultSpelldmg;
            cc.currentSpelldmg = this.currentSpelldmg;
            cc.frozen = this.frozen;
            cc.defaultHP = this.defaultHP;
            cc.armor = this.armor;
            cc.AttackedXTimes = this.AttackedXTimes;
            // cc.Auras = this.Auras;
            cc.baseattack = this.baseattack;
            cc.basehitpoints = this.basehitpoints;
            cc.charge = this.charge;
            cc.currentattack = this.currentattack;
            cc.currenthitpoints = this.currenthitpoints;
            cc.divineshield = this.divineshield;
            cc.invulnerable = this.invulnerable;
            cc.manacost = this.manacost;
            cc.name = this.name;
            //cc.Skills = this.Skills;
            foreach (Abbility abb in this.Skills)
            {
                cc.Skills.Add(abb.Clone());
            }
            // cc.tags = this.tags;
            cc.turnsingame = this.turnsingame;
            foreach (string tag in this.tags)
            {
                cc.tags.Add(tag);
            }
            foreach (Abbility abb in cc.Skills)
            {
                abb.Owner = cc;
            }
            return cc;
        }

    }
    public class CardType
    {
        public enum typ { SPELL, WEAPON, CHARACTER };
        public enum subtyp { NONE, BEAST, PLAYER, MURLOC };

    }

    public class Abbility
    {
        //Kind determines keyword specification of the Abbility (Skill) of the Minion / Spell
        //There are currently following types: Battlecry, Triggered, Aura
        public string Kind;
        //Which Card owns this specific abbility
        public Card Owner;
        //Flavor text that should be displayed on the Card - allows better merging of more effect together
        public string description;
        //List of triggers that triger the Abbility
        public List<Triggers> AbbilityTriggers;
        public string Trigger;
        public GameEngineFunctions engine;
        public GameRepresentation Game;
        public Abbility Clone()
        {
            Abbility outone = new Abbility(this.Kind, this.engine, this.Game);
            foreach (CardFunctions fnc in this.Functions)
            {
                outone.Functions.Add(fnc.Clone());
            }
            outone.TargetTags = new List<string>(this.TargetTags);
            outone.Trigger = this.Trigger;
            return outone;
        }
        //Constructor
        public Abbility(String Kind, GameEngineFunctions Engine, GameRepresentation Game)
        {
            this.Kind = Kind;
            this.TargetTags = new List<string>();
            this.Functions = new List<CardFunctions>();
            this.engine = Engine;
            this.Game = Game;
            this.AbbilityTriggers = new List<Triggers>();

            this.TargetTags2 = new List<Tags>();
            this.Effects = new List<XElement>();
        }
        //This should perform the Abbility of the card and all its effects
        public void Perform(Card PossibleTarget)
        {
            foreach (CardFunctions Function in Functions)
            {
                Function.Perform(engine, Game, this.Owner);
            }

            /* foreach (XElement effect in Effects)
             {
                 switch (effect.Value)
                 {
                     case "Deal_Damage":

                         break;
                     case "Heal":

                         break;
                     default:
                         break;
                 }
             }*/
        }

        public List<CardFunctions> Functions;
        public List<string> TargetTags;
        public List<Tags> TargetTags2;
        public List<XElement> Effects;

        public List<Card> GetTargets()
        {
            return null;
        }
    }
    public class Mana
    {
        public int availible;
        int total;
        public Mana()
        {
            this.availible = 0;
            this.total = 0;
        }

        public int max()
        {
            return this.total;
        }
        public void setMax(int i)
        {
            this.total = i;
        }
        public override string ToString()
        {
            return availible + " / " + total;
            //return base.ToString();
        }
        public void Turn()
        {
            if (this.total < 10)
            {
                this.total++;
            }
            this.availible = this.total;

        }
    }
    public class Decklist
    {
        public string name;
        public ObservableCollection<Card> cards;
        public Decklist(string name)
        {
            this.name = name;
            this.cards = new ObservableCollection<Card>();
        }
        public ObservableCollection<Card> clone(bool shuffled = false, Random rng = null)
        {
            ObservableCollection<Card> deck = new ObservableCollection<Card>();
            for (int i = 0; i < cards.Count; i++)
            {
                deck.Add(cards[i].Clone());
            }
            if (shuffled)
            {
                ObservableCollection<Card> shuffledDeck = new ObservableCollection<Card>();
                int cardCount = deck.Count;
                for (int i = 0; i < cardCount; i++)
                {
                    int location = rng.Next(deck.Count);
                    shuffledDeck.Add(deck[location].Clone());
                    deck.RemoveAt(location);
                }
                return shuffledDeck;
            }
            return deck;
        }
    }
    public class GameRepresentation
    {
        public List<Card> AllCards = new List<Card>();
        public ObservableCollection<Card> Deck1 = new ObservableCollection<Card>();
        public ObservableCollection<Card> Deck2 = new ObservableCollection<Card>();
        public ObservableCollection<Card> Hand1 = new ObservableCollection<Card>();
        public ObservableCollection<Card> Hand2 = new ObservableCollection<Card>();
        public ObservableCollection<Card> Field1 = new ObservableCollection<Card>();
        public ObservableCollection<Card> Field2 = new ObservableCollection<Card>();
        public ObservableCollection<Card> ValidTargetsP = new ObservableCollection<Card>();
        public ObservableCollection<Card> SelectableCards = new ObservableCollection<Card>();
        public Mana ManaP1 = new Mana();
        public Mana ManaP2 = new Mana();
        public bool ActivePlayer = false; // 0 / 1 ... me vs opponent 
        public int CurrentPlayer = 0; // 0 me; 1 opponent
        public int TurnsTotal = 0;
        public List<Mana> Manapool = new List<Mana>();
        public ObservableCollection<ObservableCollection<Card>> Hands = new ObservableCollection<ObservableCollection<Card>>();
        public ObservableCollection<ObservableCollection<Card>> Fields = new ObservableCollection<ObservableCollection<Card>>();
        public ObservableCollection<ObservableCollection<Card>> Decks = new ObservableCollection<ObservableCollection<Card>>();
        public List<Card> Players = new List<Card>();
        public Card TargetForSomething;
        public List<Abbility> ActiveAuras = new List<Abbility>();
        public List<bool> isThisPlayerAi = new List<bool>();
        public List<AI> AIs = new List<AI>();
        public List<GenericAI> Inteligences = new List<GenericAI>();
        public bool EndTheGame;
        public int TimeOfTheTurn = 0;
        public int MaxTimeForTurn = 1;
        public List<int> FastSpellDamage = new List<int>();
        public Card EvulEngine;
        public List<int> HeroPowerUsages = new List<int>();
        public int winner = 0; //0,1 players as designed, 3 = draw
        public List<Decklist> decklists = new List<Decklist>();
        public List<Card> heroPowers = new List<Card>();

        public void reset()
        {
            AllCards = new List<Card>();
            Deck1 = new ObservableCollection<Card>();
            Deck2 = new ObservableCollection<Card>();
            Hand1 = new ObservableCollection<Card>();
            Hand2 = new ObservableCollection<Card>();
            Field1 = new ObservableCollection<Card>();
            Field2 = new ObservableCollection<Card>();
            ValidTargetsP = new ObservableCollection<Card>();
            SelectableCards = new ObservableCollection<Card>();
            ManaP1 = new Mana();
            ManaP2 = new Mana();
            ActivePlayer = false; // 0 / 1 ... me vs opponent 
            CurrentPlayer = 0; // 0 me; 1 opponent
            TurnsTotal = 0;
            Manapool = new List<Mana>();
            Hands = new ObservableCollection<ObservableCollection<Card>>();
            Fields = new ObservableCollection<ObservableCollection<Card>>();
            Decks = new ObservableCollection<ObservableCollection<Card>>();
            Players = new List<Card>();
            TargetForSomething = null;
            ActiveAuras = new List<Abbility>();
            isThisPlayerAi = new List<bool>();
            AIs = new List<AI>();
            Inteligences = new List<GenericAI>();
            EndTheGame = false;
            TimeOfTheTurn = 0;
            MaxTimeForTurn = 1;
            FastSpellDamage = new List<int>();
            // public Card EvulEngine;
            HeroPowerUsages = new List<int>();
            winner = 0; //0,1 players as designed, 3 = draw
            decklists = new List<Decklist>();
            heroPowers = new List<Card>();
        }
        public GameRepresentation Clone()
        {
            GameRepresentation Klon = new GameRepresentation();

            return Klon;
        }
    }
    class GameEnTities
    {
    }
}
