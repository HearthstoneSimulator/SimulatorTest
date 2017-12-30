using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameIntestines
{
    public class PlayCardActionForLoadingGamestate
    {
        public string CardName;
        public int Owner;
        public int PossibleTarget = -2;
    }
    public class GameLoadState
    {
        public int[] PlayerHitpoints;
        public int[] PlayerMAX;
        public int[] Fatigues;
        public Mana[] ManaCrystals;
        public int P0HP
        {
            get { return PlayerHitpoints[0]; }
            set { PlayerHitpoints[0] = value; }
        }
        public int P1HP
        {
            get { return PlayerHitpoints[1]; }
            set { PlayerHitpoints[1] = value; }
        }
        public int P0MAX
        {
            get { return PlayerMAX[0]; }
            set { PlayerMAX[0] = value; }
        }
        public int P1MAX
        {
            get { return PlayerMAX[0]; }
            set { PlayerMAX[1] = value; }
        }
        public List<string> P0Hand;
        public List<string> P1Hand;
        public bool UsePlayerDecks = true;
       public List<Card> P0Board;
       public List<Card> P1Board;
       public List<PlayCardActionForLoadingGamestate> P0Actions;
       public List<PlayCardActionForLoadingGamestate> P1Actions;
       public List<string> P0CardsToRemoveFromDeck;
       public List<string> P1CardsToRemoveFromDeck;
        public GameLoadState()
        {
            PlayerHitpoints = new int[2];
            PlayerMAX = new int[2];
            Fatigues = new int[2];
            ManaCrystals = new Mana[2];
            ManaCrystals[0] = new Mana();
            ManaCrystals[1] = new Mana();
            P0Board = new List<Card>();
            P1Board = new List<Card>();
            P0Actions = new List<PlayCardActionForLoadingGamestate>();
            P1Actions = new List<PlayCardActionForLoadingGamestate>();
            P0CardsToRemoveFromDeck = new List<string>();
            P1CardsToRemoveFromDeck = new List<string>();
            P0Hand = new List<string>();
            P1Hand = new List<string>();
            

        }
    }
    public class StatBuffWrapper
    {
        public int AttackValue { get; set; }
        public int HPValue { get; set; }
        public string AttackModifierType { get; }
        public string HPModifierType { get; }
        public string Duration { get; }
        public string Description { get; }
        public StatBuffWrapper(int AVal, int HPVal, string AMType, string HPMType, string Duration, string Description = null)
        {
            this.AttackValue = AVal;
            this.HPValue = HPVal;
            this.AttackModifierType = AMType;
            this.HPModifierType = HPMType;
            this.Duration = Duration;
            if (Description == null)
            {
                this.Description = "+" + AttackValue + "/+" + HPValue;
            }
            else
            {
                this.Description = Description;
            }
        }


    }
    public enum TargetSelector
    {
        Player, All

    }


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
            foreach (StatBuffWrapper buff in ListOfStatBuffs)
            {
                cc.ListOfStatBuffs.Add(buff);
            }
            return cc;
        }
        public Card Clone(GameRepresentation Game)
        {
            Card standardClone = this.Clone();
            foreach (Abbility abb in this.Skills)
            {
                standardClone.Skills.Add(abb.Clone(Game));
            }
            foreach (Abbility abb in standardClone.Skills)
            {
                abb.Owner = standardClone;
            }
            return standardClone;
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
        public Abbility Clone(GameRepresentation Game2)
        {
            Abbility outone = new Abbility(this.Kind, this.engine, Game2);
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
        public int total;
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
        public bool ConsoleMode = true;
        public int numberofturnspassed = 0;
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
            Inteligences = new List<GenericAI>();
            EndTheGame = false;
            TimeOfTheTurn = 0;
            MaxTimeForTurn = 1;
            FastSpellDamage = new List<int>();
            // public Card EvulEngine;
            HeroPowerUsages = new List<int>();
            winner = 0; //0,1 players as designed, 3 = draw
            decklists = new List<Decklist>();
            numberofturnspassed = 0;
            ConsoleMode = true;
            //heroPowers = new List<Card>();
        }
        public GameRepresentation Clone()
        {
            GameRepresentation Klon = new GameRepresentation();
            foreach (Card karta in this.AllCards)
            {
                Klon.AllCards.Add(karta.Clone(Klon));
            }
            foreach (Card karta in this.Deck1)
            {
                Klon.Deck1.Add(karta.Clone(Klon));
            }
            foreach (Card karta in this.Deck2)
            {
                Klon.Deck2.Add(karta.Clone(Klon));
            }
            Klon.Decks.Add(Klon.Deck1);
            Klon.Decks.Add(Klon.Deck2);

            foreach (Card karta in this.Hand1)
            {
                Card clone = karta.Clone(Klon);
                Klon.Hand1.Add(clone);
                if (this.SelectableCards.Contains(karta))
                {
                    Klon.SelectableCards.Add(clone);
                }
                if (this.ValidTargetsP.Contains(karta))
                {
                    Klon.ValidTargetsP.Add(clone);
                }
                if (this.TargetForSomething == karta)
                {
                    Klon.TargetForSomething = clone;
                }
            }
            foreach (Card karta in this.Hand2)
            {
                Card clone = karta.Clone(Klon);
                Klon.Hand2.Add(clone);
                if (this.SelectableCards.Contains(karta))
                {
                    Klon.SelectableCards.Add(clone);
                }
                if (this.ValidTargetsP.Contains(karta))
                {
                    Klon.ValidTargetsP.Add(clone);
                }
                if (this.TargetForSomething == karta)
                {
                    Klon.TargetForSomething = clone;
                }
            }
            Klon.Hands.Add(Klon.Hand1);
            Klon.Hands.Add(Klon.Hand2);
            foreach (Card karta in this.Field1)
            {
                Card clone = karta.Clone(Klon);
                Klon.Field1.Add(clone);
                if (this.SelectableCards.Contains(karta))
                {
                    Klon.SelectableCards.Add(clone);
                }
                if (this.ValidTargetsP.Contains(karta))
                {
                    Klon.ValidTargetsP.Add(clone);
                }
                if (this.TargetForSomething == karta)
                {
                    Klon.TargetForSomething = clone;
                }
            }
            foreach (Card karta in this.Field2)
            {
                Card clone = karta.Clone(Klon);
                Klon.Field2.Add(clone);
                if (this.SelectableCards.Contains(karta))
                {
                    Klon.SelectableCards.Add(clone);
                }
                if (this.ValidTargetsP.Contains(karta))
                {
                    Klon.ValidTargetsP.Add(clone);
                }
                if (this.TargetForSomething == karta)
                {
                    Klon.TargetForSomething = clone;
                }
            }
            Klon.Fields.Add(Klon.Field1);
            Klon.Fields.Add(Klon.Field2);
            Klon.ManaP1.availible = this.ManaP1.availible;
            Klon.ManaP1.total = this.ManaP1.total;
            Klon.ManaP2.availible = this.ManaP2.availible;
            Klon.ManaP2.total = this.ManaP2.total;
            Klon.Manapool.Add(Klon.ManaP1);
            Klon.Manapool.Add(Klon.ManaP2);

            Klon.ActivePlayer = this.ActivePlayer;
            Klon.CurrentPlayer = this.CurrentPlayer;
            Klon.TurnsTotal = this.TurnsTotal;

            for (int i = 0; i < 2; i++)
            {
                Card player = this.Players[i];
                Card clone = player.Clone(Klon);
                if (this.SelectableCards.Contains(player))
                {
                    Klon.SelectableCards.Add(clone);
                }
                if (this.ValidTargetsP.Contains(player))
                {
                    Klon.ValidTargetsP.Add(clone);
                }
                if (this.TargetForSomething == player)
                {
                    Klon.TargetForSomething = clone;
                }
                Klon.Players.Add(clone);
            }
            //doing nasty aura cloning
            //cloning ActiveAuras
            
            foreach (Abbility aura in this.ActiveAuras)
            {
                for (int k = 0; k < 2; k++)
                {
                    if (Fields[k].Contains(aura.Owner))
                    {
                        for (int i = 0; i < this.Fields[k].Count; i++)
                        {
                            if (aura.Owner == Fields[k][i])
                            {
                                for (int j = 0; j < Fields[k][i].Skills.Count; j++)
                                {
                                    if (Fields[k][i].Skills[j] == aura)
                                    {
                                        //add aura to the klon aura list 
                                        Klon.ActiveAuras.Add(Klon.Fields[k][i].Skills[j]);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            //cloning auras to monsters
            for (int i = 0; i < 2; i++)
            {
                //we iterate over every monster on the field
                for (int k = 0; k < Fields[i].Count; k++)
                {
                    //and over all his auras
                    foreach (Abbility oldaura in Fields[i][k].Auras)
                    {
                        //we find which aura he is affected by
                        for (int j = 0; j < ActiveAuras.Count; j++)
                        {
                            if (ActiveAuras[j] == oldaura)
                            {
                                //we add aura on the same position to the monster in clone
                                Klon.Fields[i][k].Auras.Add(Klon.ActiveAuras[j]);
                                //we clone possible stat buff the aura gave to the monster so it can be properly deactivated in the future
                                if (oldaura.Functions[0] is GiveBuff)
                                {
                                    for (int l = 0; l < Fields[i][k].ListOfStatBuffs.Count; l++)
                                    {

                                        if (Fields[i][k].ListOfStatBuffs[l] == (oldaura.Functions[0] as GiveBuff).BuffRepresentation)
                                        {
                                            //we found which buff of the old card was caused possibly by the aura
                                            Klon.Fields[i][k].ListOfStatBuffs[l] = (Klon.ActiveAuras[j].Functions[0] as GiveBuff).BuffRepresentation;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            Klon.isThisPlayerAi.Add(isThisPlayerAi[0]);
            Klon.isThisPlayerAi.Add(isThisPlayerAi[1]);

            //AIs are not clonned so that the AI can actually gather some more information from the individual clones
            //can be changed depending on the players needs
            Klon.Inteligences = Inteligences;
            Klon.EndTheGame = EndTheGame;
            Klon.FastSpellDamage.Add(FastSpellDamage[0]);
            Klon.FastSpellDamage.Add(FastSpellDamage[1]);
            Klon.HeroPowerUsages.Add(HeroPowerUsages[0]);
            Klon.HeroPowerUsages.Add(HeroPowerUsages[1]);
            Klon.winner = winner;
            for (int i = 0; i < 2; i++)
            {
                Klon.decklists.Add(new Decklist(decklists[i].name));
                foreach (Card card in decklists[i].cards)
                {
                    Klon.decklists[i].cards.Add(card.Clone(Klon));
                }

            }
            Klon.heroPowers.Add(heroPowers[0].Clone(Klon));
            Klon.heroPowers.Add(heroPowers[1].Clone(Klon));
            Klon.ConsoleMode = ConsoleMode;
            Klon.numberofturnspassed = numberofturnspassed;
            return Klon;
        }
    }
    class GameEnTities
    {
    }
}
