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
    

    public class GameEngine 
    {
        XDocument Xdoc;
        GameRepresentation Game;
        public GameEngineFunctions Engine;
        Stack<GenericAction> StackOfThingsToDo;
        public bool WriteDebugTexts = true;
        public event EventHandler ManaChanged;
        public GenericAction PlayersAction;
        public int mana = 0;
        public string deck0name = "random";
        public string deck1name = "random";
        public string AI0name;
        public string AI1name;
        
       // int iter = 1;

        public ReadOnlyObservableCollection<Card> SelectableCardsTest
        {
            get
            {
                return new ReadOnlyObservableCollection<Card>(Game.SelectableCards);
            }

        }
        public ReadOnlyObservableCollection<Card> ValidTargetsTest
        {
            get
            {
                return new ReadOnlyObservableCollection<Card>(Game.ValidTargetsP);
            }

        }
        public ReadOnlyObservableCollection<Card> P0FieldTest
        {
            get
            {
                return new ReadOnlyObservableCollection<Card>(Game.Field1);
            }
        }
        public ReadOnlyObservableCollection<Card> P1FieldTest
        {
            get
            {
                return new ReadOnlyObservableCollection<Card>(Game.Field2);
            }
        }
        public ReadOnlyObservableCollection<Card> P0HandTest
        {
            get
            {
                return new ReadOnlyObservableCollection<Card>(Game.Hand1);  //Game.Hand1;
            }
        }
        public ReadOnlyObservableCollection<Card> P1HandTest
        {
            get
            {
                return new ReadOnlyObservableCollection<Card>(Game.Hand2);  //Game.Hand1;
            }
        }
        
        public string P1ManaTest
        {
            get
            {
                if (Game == null || Game.ManaP1 == null)
                {
                    return "0/0";
                }
                else
                {
                  //  return Game.ManaP1.ToString(); 
                    return Game.Manapool[0].ToString();
                }
            }
            
        }
        public string P2ManaTest
        {
            get
            {
                if (Game == null || Game.ManaP1 == null)
                {
                    return "0/0";
                }
                else
                {
                    //  return Game.ManaP1.ToString(); 
                    return Game.Manapool[1].ToString();
                }
            }
        }
        public string P1Hitpoints
        {
            get
            {
                if (Game == null || Game.Players[0] == null)
                {
                    return "Error";
                }
                else
                {
                    return Game.Players[0].currenthitpoints.ToString() + " / " + Game.Players[0].basehitpoints.ToString();
                }
            }
        }
        public string P2Hitpoints
        {
            get
            {
                if (Game == null || Game.Players[1] == null)
                {
                    return "Error";
                }
                else
                {
                    return Game.Players[1].currenthitpoints.ToString() + " / " + Game.Players[1].basehitpoints.ToString();
                }
            }
        }
        /*
       public int P1MaxMana
       {
           get { return Game.ManaP1.max(); }
           internal set
           {
               Game.ManaP1.setMax(value);
               if (ManaChanged != null)
                   ManaChanged(this, new EventArgs());
           }
       }

       public int ToDisplay
       {
           get { return mana; }

           internal set
           {
               mana = value;

               if (ManaChanged != null)
                   ManaChanged(this, new EventArgs());
           }
       }
       public void increasemana()
       {
          // mana = iter.ToString();
           mana++;
           ToDisplay = mana;
           //iter++;
       }
       */
        bool end = false;
        public bool CheckTagForValidity(string tag, Card Target, Card Origin)
        {
            switch (tag)
            {
                case "Self":
                    if (Target == Origin)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "Other":
                    if (Target == Origin)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case "Any":
                    return true;
                case "Own":
                    if (Game.Fields[Game.CurrentPlayer].Contains(Target))
                    {
                        return true;
                    }
                    else
                    {
                        //this should actually never even happen
                        if (Game.Hands[Game.CurrentPlayer].Contains(Target))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case "Enemy":
                    if (Game.Fields[GetOtherPlayer( Game.CurrentPlayer)].Contains(Target))
                    {
                        return true;
                    }
                    else
                    {
                        //this should actually never even happen
                        if (Game.Hands[GetOtherPlayer( Game.CurrentPlayer)].Contains(Target))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                   // break;
                default:
                    if (Target.tags.Contains(tag))
                    {
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                    //break;
            }
        }
        public bool CheckTagsForValidity(List<string> Tags, Card Target, Card Origin)
        {
            bool passed = true;
            foreach (string tag in Tags)
            {
                if (!CheckTagForValidity(tag,Target,Origin))
                {
                    return false;
                }
            }
            

            return passed;
        }
        public void DeactivateAura(Card Origin, Abbility AuraDeactivated)
        {
            Game.ActiveAuras.Remove(AuraDeactivated);
            foreach (XElement efekt in AuraDeactivated.Effects)
            {
                switch (efekt.Value)
                {
                    case "Give_Buff":
                        //this is what should happen anyway
                        if (efekt.Attribute("Attack") != null)
                        {
                            if (efekt.Attribute("AttackModificationType").Value == "ADDITION")
                            {
                                //in theory each monster...

                                foreach (Card affectedMonster in Get_Targets(AuraDeactivated))
                                {
                                    if (affectedMonster.Auras.Contains(AuraDeactivated))
                                    {
                                        affectedMonster.Auras.Remove(AuraDeactivated);
                                        affectedMonster.currentattack -= Convert.ToInt32(efekt.Attribute("Attack").Value);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void ActivateAura(Card Origin, Abbility AuraActivated)
        {
            foreach (XElement  efekt  in AuraActivated.Effects)
            {
                switch (efekt.Value)
                {
                    case "Give_Buff":
                        //this is what should happen anyway
                        if (efekt.Attribute("Attack") != null)
                        {
                            if (efekt.Attribute("AttackModificationType").Value == "ADDITION")
                            {
                                foreach (Card affectedMonster in Get_Targets(AuraActivated))
                                {
                                    if (!affectedMonster.Auras.Contains(AuraActivated))
                                    {
                                        affectedMonster.Auras.Add(AuraActivated);
                                        affectedMonster.currentattack += Convert.ToInt32(efekt.Attribute("Attack").Value);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void PerformAbbility(Card Origin, Abbility UsedAbbility, Card PossibleTarget)
        {
            foreach (XElement efekt in UsedAbbility.Effects)
	        {
                
            switch (efekt.Value)
            {
                case "Deal_Damage":
                    if (PossibleTarget != null)
                    {
                        List<Card> solotarget = new List<Card>();
                        solotarget.Add(PossibleTarget);
                        Deal_Damage(Origin, Convert.ToInt32( efekt.Attribute("value").Value), solotarget, efekt.Attribute("selector").Value, efekt.Attribute("targets").Value);
                    }
                    //Deal_Damage();
                    break;
                case "Give_Buff":
                    if (PossibleTarget != null)
                    {
                        //it has one target
                    }
                    else
                    {

                    }
                    break;
                case "Draw_Card":
                    if (efekt.Attribute("selector").Value == "AUTO")
                    {
                        for (int i = 0; i < Convert.ToInt32(efekt.Attribute("value").Value); i++)
                        {
                            //that many cards to draw
                            if (UsedAbbility.TargetTags.Contains("You"))
                            {
                                DrawCard(Game.CurrentPlayer);
                            }
                            if (UsedAbbility.TargetTags.Contains("Opponent"))
                            {
                                DrawCard(GetOtherPlayer(Game.CurrentPlayer));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
	        }
        }
        public void Deal_Damage(Card Origin, int ammount, List<Card> targets, string selector, string number_of_targets)
        {
            switch (selector)
            {
                case "PLAYER":
                    targets[0].currenthitpoints -= ammount;
                    break;
                case "RANDOM":
                    for (int i = 0; i < Convert.ToInt32( number_of_targets); i++)
                    {
                        targets[i].currenthitpoints -= ammount;
                    }
                    break;
                case "ALL":
                    foreach (Card target in targets)
                    {
                        target.currenthitpoints -= ammount;
                    }
                    break;
                default:
                    break;
            }
            foreach (Card tar in targets)
            {
                if (tar.currenthitpoints <= 0)
                {
                    KillMonster(tar);
                }
            }
        }
        /*
        public Abbility ParseAbbility(XElement Abbilityxml)
        {
             Abbility ab = new Abbility(Abbilityxml.Element("Type").Value);
             foreach (XElement tag in Abbilityxml.Element("Target").Element("Tags").Descendants())
             {
                 ab.TargetTags.Add(tag.Name.ToString());
             }
             foreach (XElement effect in Abbilityxml.Element("Effect").Elements("Function"))
             {
                 ab.Effects.Add(effect);
             }
            return ab;
        }*/
        public List<Card> Get_Targets(Abbility abb)
        {
            List<Card> targets = new List<Card>();
            for (int i = 0; i < 2; i++)
            {
                foreach (Card possibleTarget in Game.Fields[i])
                {
                    if (CheckTagsForValidity(abb.TargetTags, possibleTarget,abb.Owner)) 
                    {
                        targets.Add(possibleTarget);
                    } 
                }
                if (CheckTagsForValidity(abb.TargetTags, Game.Players[i],abb.Owner))
	{
                    targets.Add(Game.Players[i]);
	}
            }
     
            return targets;
        }
        public void InitialiseGame()
        {
            
            Game = new GameRepresentation();
            Engine = new GameEngineFunctions();
            Engine.writeDebugTexts = WriteDebugTexts;
            Game.EvulEngine = new Card();
            Game.EvulEngine.name = "Engine";
            CardLoader Loader = new CardLoader(Engine, Game);
            Engine.DebugText("**********************GAME INITIATION STARTED**********************");
            Loader.LoadCards("XMLCardBase.xml");
            #region OldLoad
            /*
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
                 if (item.Element("Skills") != null  )
                 {
                     //Card has some skills -> load them one by one
                     foreach (XElement skill in item.Element("Skills").Elements("Skill"))
                     {
                         Abbility abb = ParseAbbility(skill);
                         abb.Owner = newcard;
                         newcard.Skills.Add(abb);
                     }
                 }
                //Add the card to the Card Database
                Game.AllCards.Add(newcard);
            }
            */
            #endregion
            //Creating decks and setting up the game itself
            for (int i = 0; i < 2; i++)
            {
                Game.Manapool.Add(new Mana());
                Game.Hands.Add(new ObservableCollection<Card>());
                Game.Decks.Add(new ObservableCollection<Card>());
                Game.Fields.Add(new ObservableCollection<Card>());
                Game.Players.Add(new Card());

                Game.FastSpellDamage.Add(0);
                Game.FastSpellDamage.Add(0);
                Game.Players[i].basehitpoints = 30;
                Game.Players[i].currenthitpoints = 30;
                Game.Players[i].tags.Add("Player");
            }
            Game.Field1 = Game.Fields[0];
            Game.Field2 = Game.Fields[1];
            Game.Players[0].name = "ME";
            Game.Players[1].name = "OPPONENT";
            Game.Hand1 = Game.Hands[0];
            Game.Hand2 = Game.Hands[1];
            Game.Deck1 = Game.Decks[0];
            Game.Deck2 = Game.Decks[1];
            //            Random rng = new Random(1);
            Random rng = new Random();

            //Make decks
            for (int i = 0; i < 30; i++)
            {
                //make two decks
                //   Game.AllCards.Add(new Card(i));
            }
            for (int i = 0; i < 15; i++)
            {
                int ind = rng.Next(29 - 2 * i);
                Game.Deck1.Add(Game.AllCards[ind].Clone());
                //     Game.AllCards.RemoveAt(ind);
                ind = rng.Next(29 - 2 * i - 1);
                Game.Deck2.Add(Game.AllCards[ind].Clone());
                //      Game.AllCards.RemoveAt(ind);
            }
            //Deal cards to players
            for (int i = 0; i < 3; i++)
            {
                //this.P1Hand.Items.Add(Game.Deck1[0].name);
                Game.Hand1.Add(Game.Deck1[0]);
                Game.Deck1.RemoveAt(0);
                //this.P0Hand.Items.Add(Game.Deck2[0].name);
                Game.Hand2.Add(Game.Deck2[0]);
                Game.Deck2.RemoveAt(0);
            }
            //initalise hero powers
            for (int i = 0; i < 2; i++)
            {
                Game.HeroPowerUsages.Add(0);
            }
            //Determine starting player (and give the opponent the coin)
            Game.ActivePlayer = true; // I am the active player
            Game.isThisPlayerAi.Add(false);// true);// true); //false
            Game.isThisPlayerAi.Add(true);
            Game.AIs.Add(null); //null
            Game.AIs.Add(new AI());
            Game.Inteligences.Add(new MyCustomAI()); //null
            Game.Inteligences.Add(new MyCustomAI());
            //Start the game

            //IncreaseMana(Game.CurrentPlayer);
            //  DrawCard(Game.CurrentPlayer);

            Game.Fields[1].Add(Game.AllCards[27].Clone());
            Game.Hand1.Add(Engine.GetCopyOfCardFromDatabase("Flametongue Totem",Game));
            Game.Hand1.Add(Engine.GetCopyOfCardFromDatabase("Polymorph", Game));
            Game.Hand1.Add(Game.AllCards[76].Clone());
            Game.Hand1.Add(Game.AllCards[76].Clone());
            Game.Hand1.Add(Game.AllCards[0].Clone());
            Game.Hand1.Add(Game.AllCards[4].Clone());
            Game.Hand1.Add(Game.AllCards[76].Clone());
            Game.Hand1.Add(Game.AllCards[32].Clone());
            //  Engine.GetSelectableCards(Game);
            //    Engine.test(Game);
            //Engine.InitialiseTurn(Game);
            Game.winner = -1;
            Engine.TurnManagement(Game);
        }

        public void InitialiseGame(bool bb)
        {
            prepareGame();
            #region OldLoad
            /*
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
                 if (item.Element("Skills") != null  )
                 {
                     //Card has some skills -> load them one by one
                     foreach (XElement skill in item.Element("Skills").Elements("Skill"))
                     {
                         Abbility abb = ParseAbbility(skill);
                         abb.Owner = newcard;
                         newcard.Skills.Add(abb);
                     }
                 }
                //Add the card to the Card Database
                Game.AllCards.Add(newcard);
            }
            */
            #endregion
            //Creating decks and setting up the game itself
            for (int i = 0; i < 2; i++)
            {
                Game.Manapool.Add(new Mana());
                Game.Hands.Add(new ObservableCollection<Card>());
                Game.Decks.Add(new ObservableCollection<Card>());
                Game.Fields.Add(new ObservableCollection<Card>());
                Game.Players.Add(new Card());

                Game.FastSpellDamage.Add(0);
                Game.FastSpellDamage.Add(0);
                Game.Players[i].basehitpoints = 30;
                Game.Players[i].currenthitpoints = 30;
                Game.Players[i].tags.Add("Player");
            }
            Game.Field1 = Game.Fields[0];
            Game.Field2 = Game.Fields[1];
            Game.Players[0].name = "ME";
            Game.Players[1].name = "OPPONENT";
            Game.Hand1 = Game.Hands[0];
            Game.Hand2 = Game.Hands[1];
            Game.Deck1 = Game.Decks[0];
            Game.Deck2 = Game.Decks[1];
            //            Random rng = new Random(1);
            Random rng = new Random();

            //Make decks
            for (int i = 0; i < 30; i++)
            {
                //make two decks
                //   Game.AllCards.Add(new Card(i));
            }
            for (int i = 0; i < 15; i++)
            {
                int ind = rng.Next(29 - 2 * i);
                Game.Deck1.Add(Game.AllCards[ind].Clone());
                //     Game.AllCards.RemoveAt(ind);
                ind = rng.Next(29 - 2 * i - 1);
                Game.Deck2.Add(Game.AllCards[ind].Clone());
                //      Game.AllCards.RemoveAt(ind);
            }
            //Deal cards to players
            for (int i = 0; i < 3; i++)
            {
                //this.P1Hand.Items.Add(Game.Deck1[0].name);
                Game.Hand1.Add(Game.Deck1[0]);
                Game.Deck1.RemoveAt(0);
                //this.P0Hand.Items.Add(Game.Deck2[0].name);
                Game.Hand2.Add(Game.Deck2[0]);
                Game.Deck2.RemoveAt(0);
            }
            //initalise hero powers
            for (int i = 0; i < 2; i++)
            {
                Game.HeroPowerUsages.Add(0);
            }
            //Determine starting player (and give the opponent the coin)
            Game.ActivePlayer = true; // I am the active player
            Game.isThisPlayerAi.Add(false);// true);// true); //false
            Game.isThisPlayerAi.Add(true);
            Game.AIs.Add(null); //null
            Game.AIs.Add(new AI());
            Game.Inteligences.Add(new MyCustomAI()); //null
            Game.Inteligences.Add(new MyCustomAI());
            //Start the game

            //IncreaseMana(Game.CurrentPlayer);
            //  DrawCard(Game.CurrentPlayer);

            Game.Fields[1].Add(Game.AllCards[27].Clone());
            Game.Hand1.Add(Game.AllCards[76].Clone());
            Game.Hand1.Add(Game.AllCards[76].Clone());
            Game.Hand1.Add(Game.AllCards[0].Clone());
            Game.Hand1.Add(Game.AllCards[4].Clone());
            Game.Hand1.Add(Game.AllCards[76].Clone());
            Game.Hand1.Add(Game.AllCards[32].Clone());
            //  Engine.GetSelectableCards(Game);
            //    Engine.test(Game);
            //Engine.InitialiseTurn(Game);
            Game.winner = -1;
            Engine.TurnManagement(Game);
            /*
            prepareGame();
            setStuff();
            simulateOneGame();*/
        }
        public List<Card> MakeDeck(string sourceFile)
        {
            List<Card> deckToMake = new List<Card>();

            return deckToMake;
        }

        public void slowLoadSafeTest()
        {

            Game = new GameRepresentation();
            Engine = new GameEngineFunctions();
            Engine.writeDebugTexts = WriteDebugTexts;
            Game.EvulEngine = new Card();
            Game.EvulEngine.name = "Engine";

            //preparing two decklists
            Game.decklists.Add(new Decklist(deck0name));
            Game.decklists.Add(new Decklist(deck1name));


            CardLoader Loader = new CardLoader(Engine, Game);

            Loader.LoadCards("XMLCardBase.xml");

            //jen nacte balicky ale stale nejsou pripraveny
            Loader.LoadDecks();
            
            //CardLoader Loader = new CardLoader(Engine, Game);
            Engine.DebugText("**********************GAME INITIATION STARTED**********************");
            //Loader.LoadCards("XMLCardBase.xml");
            
            //Creating decks and setting up the game itself
            for (int i = 0; i < 2; i++)
            {
                Game.Manapool.Add(new Mana());
                Game.Hands.Add(new ObservableCollection<Card>());
                Game.Decks.Add(new ObservableCollection<Card>());
                Game.Fields.Add(new ObservableCollection<Card>());
                Game.Players.Add(new Card());

                Game.FastSpellDamage.Add(0);
                Game.FastSpellDamage.Add(0);
                Game.Players[i].basehitpoints = 30;
                Game.Players[i].currenthitpoints = 30;
                Game.Players[i].tags.Add("Player");
            }
            Game.Field1 = Game.Fields[0];
            Game.Field2 = Game.Fields[1];
            Game.Players[0].name = "ME";
            Game.Players[1].name = "OPPONENT";
            Game.Hand1 = Game.Hands[0];
            Game.Hand2 = Game.Hands[1];
            Game.Deck1 = Game.Decks[0];
            Game.Deck2 = Game.Decks[1];
            //            Random rng = new Random(1);
            Random rng = new Random();

            //Make decks
            //Make decks
            Game.Decks[0] = Game.decklists[0].clone(true, rng);
            Game.Decks[1] = Game.decklists[1].clone(true, rng);
            Game.Deck1 = Game.Decks[0];
            Game.Deck2 = Game.Decks[1];

            //Deal cards to players
            for (int i = 0; i < 3; i++)
            {
                //this.P1Hand.Items.Add(Game.Deck1[0].name);
                Game.Hand1.Add(Game.Deck1[0]);
                Game.Deck1.RemoveAt(0);
                //this.P0Hand.Items.Add(Game.Deck2[0].name);
                Game.Hand2.Add(Game.Deck2[0]);
                Game.Deck2.RemoveAt(0);
            }
            //initalise hero powers
            for (int i = 0; i < 2; i++)
            {
                Game.HeroPowerUsages.Add(0);
            }
            //Determine starting player (and give the opponent the coin)
            Game.ActivePlayer = true; // I am the active player
            Game.isThisPlayerAi.Add(true);// true);// true); //false
            Game.isThisPlayerAi.Add(true);
            Game.AIs.Add(null); //null
            Game.AIs.Add(new AI());
            Game.Inteligences.Add(new RandomAI());// MyCustomAI()); //null
            Game.Inteligences.Add(new RandomAI());// MyCustomAI());
            //Start the game

            //IncreaseMana(Game.CurrentPlayer);
            //  DrawCard(Game.CurrentPlayer);

            Game.Fields[1].Add(Game.AllCards[27].Clone());
            Game.Hand1.Add(Game.AllCards[76].Clone());
            Game.Hand1.Add(Game.AllCards[76].Clone());
            Game.Hand1.Add(Game.AllCards[0].Clone());
            Game.Hand1.Add(Game.AllCards[4].Clone());
            Game.Hand1.Add(Game.AllCards[76].Clone());
            Game.Hand1.Add(Game.AllCards[32].Clone());
            //  Engine.GetSelectableCards(Game);
            //    Engine.test(Game);
            //Engine.InitialiseTurn(Game);
            Game.winner = -1;
            //Engine.TurnManagement(Game);
        }
        public void slowLoadGameTest()
        {
            Game = new GameRepresentation();
            Engine = new GameEngineFunctions();
            Engine.writeDebugTexts = WriteDebugTexts;
            Game.EvulEngine = new Card();
            Game.EvulEngine.name = "Engine";
            //preparing two decklists
            Game.decklists.Add(new Decklist(deck0name));
            Game.decklists.Add(new Decklist(deck1name));
            CardLoader Loader = new CardLoader(Engine, Game);
            Loader.LoadCards("XMLCardBase.xml");
            Loader.LoadDecks();
            Engine.DebugText("**********************GAME INITIATION STARTED**********************");
            GameRepresentation backupGame = Game;
            Game = new GameRepresentation();
            Game.AllCards = backupGame.AllCards;
            Game.decklists = backupGame.decklists;
            Game.EvulEngine = backupGame.EvulEngine;
            for (int i = 0; i < 2; i++)
            {
                Game.Manapool.Add(new Mana());
                Game.Hands.Add(new ObservableCollection<Card>());
                Game.Decks.Add(new ObservableCollection<Card>());
                Game.Fields.Add(new ObservableCollection<Card>());
                Game.Players.Add(new Card());

                Game.FastSpellDamage.Add(0);
                Game.FastSpellDamage.Add(0);
                Game.Players[i].basehitpoints = 30;
                Game.Players[i].currenthitpoints = 30;
                Game.Players[i].tags.Add("Player");
            }
            Game.Field1 = Game.Fields[0];
            Game.Field2 = Game.Fields[1];
            Game.Players[0].name = "ME";
            Game.Players[1].name = "OPPONENT";
            Game.Hand1 = Game.Hands[0];
            Game.Hand2 = Game.Hands[1];
            //            Random rng = new Random(1);
            Random rng = new Random();

            //Make decks
            Game.Decks[0] = Game.decklists[0].clone(true, rng);
            Game.Decks[1] = Game.decklists[1].clone(true, rng);
            Game.Deck1 = Game.Decks[0];
            Game.Deck2 = Game.Decks[1];

            //shuffle decks
            
            //Deal cards to players
            for (int i = 0; i < 3; i++)
            {
                //this.P1Hand.Items.Add(Game.Deck1[0].name);
                Game.Hand1.Add(Game.Deck1[0]);
                Game.Deck1.RemoveAt(0);
                //this.P0Hand.Items.Add(Game.Deck2[0].name);
                Game.Hand2.Add(Game.Deck2[0]);
                Game.Deck2.RemoveAt(0);
            }
            //initalise hero powers
            for (int i = 0; i < 2; i++)
            {
                Game.HeroPowerUsages.Add(0);
            }
            //Determine starting player (and give the opponent the coin)
            Game.ActivePlayer = true; // I am the active player
            Game.isThisPlayerAi.Add(true);// true); //false
            Game.isThisPlayerAi.Add(true);
            Game.AIs.Add(null); //null
            Game.AIs.Add(new AI());
            Game.Inteligences.Add(new RandomAI());// MyCustomAI()); //null
            Game.Inteligences.Add(new RandomAI());// MyCustomAI());
            //Start the game

            //IncreaseMana(Game.CurrentPlayer);
            //  DrawCard(Game.CurrentPlayer);

            //Game.Fields[1].Add(Game.AllCards[27].Clone());
            //Game.Hand1.Add(Game.AllCards[76].Clone());
            //Game.Hand1.Add(Game.AllCards[76].Clone());
            //Game.Hand1.Add(Game.AllCards[0].Clone());
            //Game.Hand1.Add(Game.AllCards[4].Clone());
            //Game.Hand1.Add(Game.AllCards[76].Clone());
            //Game.Hand1.Add(Game.AllCards[32].Clone());


            //  Engine.GetSelectableCards(Game);
            //    Engine.test(Game);
            //Engine.InitialiseTurn(Game);
            Game.winner = -1;
        }
        public void prepareGame()
        {
            Game = new GameRepresentation();
            Engine = new GameEngineFunctions();
            Engine.writeDebugTexts = WriteDebugTexts;
            Game.EvulEngine = new Card();
            Game.EvulEngine.name = "Engine";
            //preparing two decklists
            Game.decklists.Add(new Decklist(deck0name));
            Game.decklists.Add(new Decklist(deck1name));
            CardLoader Loader = new CardLoader(Engine, Game);
            Loader.LoadCards("XMLCardBase.xml");
            Loader.LoadDecks();
            Engine.DebugText("**********************GAME INITIATION STARTED**********************");
        }
        public void setStuff()
        {
            //GameRepresentation backupGame = Game;
            //Game = new GameRepresentation();
            List<Card> AllCardsBackup = Game.AllCards;
            List<Decklist> decksBackup = Game.decklists;
            Card evulenginebackup = Game.EvulEngine;
            Game.reset();
            Game.AllCards = AllCardsBackup;// backupGame.AllCards;
            Game.decklists = decksBackup;// backupGame.decklists;
            Game.EvulEngine = evulenginebackup;// backupGame.EvulEngine;
            //backupGame = Game;
            for (int i = 0; i < 2; i++)
            {
                Game.Manapool.Add(new Mana());
                Game.Hands.Add(new ObservableCollection<Card>());
                Game.Decks.Add(new ObservableCollection<Card>());
                Game.Fields.Add(new ObservableCollection<Card>());
                Game.Players.Add(new Card());

                Game.FastSpellDamage.Add(0);
                Game.FastSpellDamage.Add(0);
                Game.Players[i].basehitpoints = 30;
                Game.Players[i].currenthitpoints = 30;
                Game.Players[i].tags.Add("Player");
            }
            Game.Field1 = Game.Fields[0];
            Game.Field2 = Game.Fields[1];
            Game.Players[0].name = "ME";
            Game.Players[1].name = "OPPONENT";
            Game.Hand1 = Game.Hands[0];
            Game.Hand2 = Game.Hands[1];
            //            Random rng = new Random(1);
            Random rng = new Random();

            //Make decks
            Game.Decks[0] = Game.decklists[0].clone(true,rng);
            Game.Decks[1] = Game.decklists[1].clone(true,rng);
            Game.Deck1 = Game.Decks[0];
            Game.Deck2 = Game.Decks[1];

            //shuffle decks

            
            /*
            for (int i = 0; i < 30; i++)
            {
                //make two decks
                //   Game.AllCards.Add(new Card(i));
            }
            for (int i = 0; i < 15; i++)
            {
                int ind = rng.Next(29 - 2 * i);
                Game.Deck1.Add(Game.AllCards[ind].Clone());
                //     Game.AllCards.RemoveAt(ind);
                ind = rng.Next(29 - 2 * i - 1);
                Game.Deck2.Add(Game.AllCards[ind].Clone());
                //      Game.AllCards.RemoveAt(ind);
            }*/
            //Deal cards to players
            for (int i = 0; i < 3; i++)
            {
                //this.P1Hand.Items.Add(Game.Deck1[0].name);
                Game.Hand1.Add(Game.Deck1[0]);
                Game.Deck1.RemoveAt(0);
                //this.P0Hand.Items.Add(Game.Deck2[0].name);
                Game.Hand2.Add(Game.Deck2[0]);
                Game.Deck2.RemoveAt(0);
            }
            //initalise hero powers
            for (int i = 0; i < 2; i++)
            {
                Game.HeroPowerUsages.Add(0);
            }
            //Determine starting player (and give the opponent the coin)
            Game.ActivePlayer = true; // I am the active player
            Game.isThisPlayerAi.Add(true);// true); //false
            Game.isThisPlayerAi.Add(true);
            Game.AIs.Add(null); //null
            Game.AIs.Add(new AI());
            Game.Inteligences.Add(new RandomAI());// MyCustomAI()); //null
            Game.Inteligences.Add(new RandomAI());// MyCustomAI());
            //Start the game

            //IncreaseMana(Game.CurrentPlayer);
            //  DrawCard(Game.CurrentPlayer);

            //Game.Fields[1].Add(Game.AllCards[27].Clone());
            //Game.Hand1.Add(Game.AllCards[76].Clone());
            //Game.Hand1.Add(Game.AllCards[76].Clone());
            //Game.Hand1.Add(Game.AllCards[0].Clone());
            //Game.Hand1.Add(Game.AllCards[4].Clone());
            //Game.Hand1.Add(Game.AllCards[76].Clone());
            //Game.Hand1.Add(Game.AllCards[32].Clone());


            //  Engine.GetSelectableCards(Game);
            //    Engine.test(Game);
            //Engine.InitialiseTurn(Game);
            Game.winner = -1;
        }
        public void simulateOneGame()
        {
            Engine.TurnManagement(Game);
        }
        public int getWinner()
        {
            return Game.winner;
        }
        public void StartOneGame()
        {
            while (!Game.EndTheGame)
            {
                InitialiseTurn();

                if (Game.isThisPlayerAi[Game.CurrentPlayer])
                {
                    //we are dealing with an AI player
                    GenericAction ActionToPerform = Game.Inteligences[Game.CurrentPlayer].getAction(Game);
                    ActionToPerform.Perform(Engine, Game);
                    StackOfThingsToDo.Pop().Perform(Engine,Game);
                }
                else
                {
                    //the player is human
                    //TODO
                    while (PlayersAction == null && Game.TimeOfTheTurn <= Game.MaxTimeForTurn )
                    {
                        //waiting
                    }
                    if (PlayersAction != null)
                    {
                        PlayersAction.Perform(Engine, Game);
                    }
                    else
                    {
                        Engine.EndTurn(Game);
                    }
                        PlayersAction = null;
                }
            }
        }
        private void StartGameCycle()
        {
            while (true)
            {
                Game.Inteligences[Game.CurrentPlayer].getAction(Game).Perform(Engine,Game);
                /*
                if (Game.isThisPlayerAi[Game.CurrentPlayer])
                {
                    //this player is an AI
                    Game.Inteligences[Game.CurrentPlayer].getAction(Game).Perform(Game);
                }
                else
                {
                    //this player is a Player

                }*/
            }
        }

        private void DrawCard(int TargetPlayer)
        {
            //handle card drawing listeners
            if (Game.Decks[TargetPlayer].Count >= 1)
            {

            Game.Hands[TargetPlayer].Add(Game.Decks[TargetPlayer][0]);
            Game.Decks[TargetPlayer].RemoveAt(0);
            if (ManaChanged != null)
                ManaChanged(this, new EventArgs());
            }
            else
            {
                DebugText("player " + Game.Players[TargetPlayer] + " Cannot draw cards and suffered fatigue damage");
            }
        }
        private void IncreaseMana(int TargetPlayer)
        {
            Game.Manapool[TargetPlayer].Turn();

            if (ManaChanged != null)
                ManaChanged(this, new EventArgs());
        }
        public int GetOtherPlayer(int currentPlayer)
        {
            return currentPlayer + 1 - currentPlayer * 2;
        }
        public void EndTurn()
        {
            //Game.CurrentPlayer = Game.CurrentPlayer + 1 - Game.CurrentPlayer*2;

            Engine.EndTurn(Game);
            /*Game.CurrentPlayer = GetOtherPlayer(Game.CurrentPlayer);
            InitialiseTurn();*/
        }
        public void ResetMonsterAttacks()
        {
            for (int i = 0; i < 2; i++)
			{

                foreach (Card Monster in Game.Fields[i])
                {
                    Monster.AttackedXTimes = 0;    
                }
			}
        }
        public void DebugText(string outString)
        {
            if (true)
            {
                Console.WriteLine(outString);
            }
        }
        public void InitialiseTurn()
        {
            int oldplayer = Game.CurrentPlayer;
            //increase turn counter
            Game.TurnsTotal++;
            IncreaseMonsterTurns();
            ResetMonsterAttacks();
            //handle start turn listeners
            //TODO

            //increase mana
            IncreaseMana(Game.CurrentPlayer);
            //draw card
            DrawCard(Game.CurrentPlayer);
            GetSelectableCards();
            if (Game.isThisPlayerAi[Game.CurrentPlayer])
            {
                DebugText("AI player turn started");
                bool shouldEndTurn = false;
                while (!shouldEndTurn)
                {
                    Game.Inteligences[Game.CurrentPlayer].getAction(Game).Perform(Engine,Game);
                    if (Game.CurrentPlayer != oldplayer)
                    {
                        shouldEndTurn = true;
                    }
                }
            }
            #region stuff
            /*
            if (Game.isThisPlayerAi[Game.CurrentPlayer])
            {
                DebugText("AI player turn started");
                bool shouldEndTurn = false;
                while (!shouldEndTurn)
                {
                    AIAction akce = Game.AIs[Game.CurrentPlayer].getAction(Game);
                    DebugText("Asking AI for action");
                    switch (akce.ActionIndex)
                    {
                        case 0:
                            DebugText("AI wants to end the turn");
                            shouldEndTurn = true;
                            EndTurn();
                            break;
                        case 1:
                            DebugText("AI plays monster" + akce.FirstCardSelection);
                            PlayMonsterFromHand(akce.FirstCardSelection);
                            break;
                        case 2:
                            if (CanAttack(akce.FirstCardSelection))
                            {
                                AttackWithMonster(akce.FirstCardSelection, akce.SecondCardSelection);
                                Console.WriteLine("Monster "+akce.FirstCardSelection+" attakced "+akce.SecondCardSelection);
                                
                            }
                            break;
                        default:
                            break;
                    }
                }
                
            }*/
            #endregion
        }
        public void IncreaseMonsterTurns()
        {
            foreach (Card Monster in Game.Fields[Game.CurrentPlayer])
            {
                Monster.turnsingame++;
            }
        }
        public List<Card> GetValidTargets(Card SelectedCard)
        {
            List<Card> ValidTargets = new List<Card>();
            foreach (Card EnemyMonster in Game.Fields[Convert.ToInt32(!Convert.ToBoolean(Game.CurrentPlayer))])
            {
                if (EnemyMonster.tags.Contains("Taunt")&&!EnemyMonster.tags.Contains("STEALTH")&&!EnemyMonster.tags.Contains("IMMUNE"))
                {
                    //if the monster has taunt and is not stealthed or immune
                    ValidTargets.Add(EnemyMonster);
                }
            }

            if (ValidTargets.Capacity == 0)
            {
                //Add enemy hero
                if (!Game.Players[Convert.ToInt32(!Convert.ToBoolean(Game.CurrentPlayer))].tags.Contains("IMMUNE") && !Game.Players[Convert.ToInt32(!Convert.ToBoolean(Game.CurrentPlayer))].tags.Contains("STEALTH"))
                {
                    ValidTargets.Add(Game.Players[Convert.ToInt32(!Convert.ToBoolean(Game.CurrentPlayer))]);
                }
                //no enemy monster has taunt or is a valid taunt target
                foreach (Card EnemyMonster in Game.Fields[Convert.ToInt32(!Convert.ToBoolean(Game.CurrentPlayer))])
                {
                    if (!EnemyMonster.tags.Contains("STEALTH") && !EnemyMonster.tags.Contains("IMMUNE"))
                    {
                        //if the monster  is not stealthed or immune
                        ValidTargets.Add(EnemyMonster);
                    }
                }
            }

            return ValidTargets;
        }
        public bool CanAttack(Card MonsterCard)
        {
            if (MonsterCard.currentattack <= 0)
            {
                return false;
            }
            if (MonsterCard.turnsingame == 0 && !MonsterCard.tags.Contains("CHARGE"))
            {
                return false;
            }
            if (MonsterCard.AttackedXTimes >= 1 && !MonsterCard.tags.Contains("WINDFURY"))
            {
                return false;
            }
            if (MonsterCard.AttackedXTimes >= 2)
            {
                return false;
            }
            if (GetValidTargets(MonsterCard).Capacity == 0)
            {
                return false;
            }
            
            return true;
        }
        public void GetSelectableCards()
        {
            Game.SelectableCards.Clear();
            
            foreach (Card card in Game.Hands[Game.CurrentPlayer])
            {
                if (card.manacost <= Game.Manapool[Game.CurrentPlayer].availible)
                {
                    Game.SelectableCards.Add(card);
                }
            }
            foreach (Card card in Game.Fields[Game.CurrentPlayer])
            {
                if ( CanAttack(card) )
                {
                    Game.SelectableCards.Add(card);
                }
            }
        }
        public void AuraTigger()
        {
            foreach (Abbility aura in Game.ActiveAuras)
            {
                ActivateAura(aura.Owner, aura);
            }
        }
        public void MonsterGetsPlayed(Card SelectedCard)
        {
            foreach (Abbility ski in SelectedCard.Skills)
            {
                if (ski.Kind == "Battlecry")
                {
                    //PerformAbbility(SelectedCard, ski, Game.TargetForSomething);
                    ski.Perform(Game.TargetForSomething);
                }
            }
        }
        public void MonsterComesIntoPlay(Card SelectedCard)
        {
            foreach (Abbility ski in SelectedCard.Skills)
            {
                if (ski.Kind == "Aura")
                {
                    Game.ActiveAuras.Add(ski);
                }
            }
            AuraTigger();

        }
        public void PlayMonsterFromHand(Card SelectedCard)
        {
            //bool asdf = object.ReferenceEquals(SelectedCard, Game.Hands[0][7]);
            Engine.PlayMonsterFromHand(SelectedCard, Game);
            /*
            //now I need to find out which card I selected - it is some card from active player -> can find if it is from hand or from table
            if (Game.Hands[Game.CurrentPlayer].Contains(SelectedCard))
            {
                //the card is in players hand
                if (SelectedCard.manacost <= Game.Manapool[Game.CurrentPlayer].availible && SelectedCard.tags.Contains("Minion"))
                {
                    //I have mana to play the card

                    MonsterGetsPlayed(SelectedCard);

                    Game.Fields[Game.CurrentPlayer].Add(SelectedCard);
                    Game.Hands[Game.CurrentPlayer].Remove(SelectedCard);
                    MonsterComesIntoPlay(SelectedCard);
                    Game.Manapool[Game.CurrentPlayer].availible -= SelectedCard.manacost;
                    if (ManaChanged != null)
                        ManaChanged(this, new EventArgs());
                }
                
            }
            else
            {
                //the card is on the players battlefield
                //I am attacking the target (that is already valid and selected)
                if (Game.TargetForSomething != null &&SelectedCard != null)
                {
                    AttackWithMonster(SelectedCard,Game.TargetForSomething);

                }

            }
            GetSelectableCards();*/
        }
        public void DealDamage(List<Card> Target, int ammount)
        {
            foreach (Card target in Target)
            {
                target.currenthitpoints -= ammount;
                if (target.currenthitpoints <=0)
                {
                    KillMonster(target);
                }
            }
        }
        public void Clash(Card Attacker, Card Target)
        {
            //TODO: DEALDAMAGE(Target,Ammount), DEALDAMAGE(Attacker,Ammount), 
            int attackerdmg = Attacker.currentattack;
            int targetdmg = Target.currentattack;
            List<Card> attacker = new List<Card>();
            attacker.Add(Attacker);
            List<Card> target = new List<Card>();
            target.Add(Target);
            DealDamage(target, attackerdmg);
            DealDamage(attacker, targetdmg);
        }
        public Card MonsterAttacks(Card Monster)
        {
            return Monster;
            //TODO: CHECK ALL POSSIBLE TRIGGERS
        }
        public List<Card> MonsterAttacksTarget(Card Monster, Card Target)
        {
            List<Card> result = new List<Card>();
            result.Add(Monster);
            result.Add(Target);
            return result;
        }
        public void AttackWithMonster(Card SelectedMonster, Card SelectedTarget)
        {

            SelectedMonster.AttackedXTimes++;
            Card tmp;
            //TODO: MONSTER_ATTACKS EVENT
             tmp = MonsterAttacks(SelectedMonster);
             if (tmp == null)
             {
                 //monster does not attack anymore
                 return;
             }
            //TODO: IF MONSTER STILL_ATTACKS -> MONSTER_ATTACKS_TARGET EVENT
             List<Card> tmplist = MonsterAttacksTarget(tmp, SelectedTarget);
             if (tmplist.Capacity<2 || tmplist[0] == null || tmplist[1] == null)
             {
                 //something happened that canceled the attack
                 return;
             }
            //TODO: IF_MONSTER STILL_ATTACKS -> CLASH(MONSTER,TARGET)
             Clash(tmplist[0], tmplist[1]);
            //TODO: MONSTER_ATTACKED EVENT
            /*
            SelectedTarget.currenthitpoints -= SelectedMonster.currentattack;
            SelectedMonster.currenthitpoints -= SelectedTarget.currentattack;
            if (SelectedTarget.currenthitpoints<=0)
            {
                //monster dies
                KillMonster(SelectedTarget);
            }
            if (SelectedMonster.currenthitpoints <=0)
            {
                //Attacker dies
                KillMonster(SelectedMonster);
            }*/
            //TODO TRIGGERS
        }
        public void KillMonster(Card SelectedMonster)
        {
            //TODO TRIGGER FOR MONSTER DEATH
            RemoveCardFromPlay(SelectedMonster);
            MonsterRemovedFromPlay(SelectedMonster);
        }
        public void MonsterRemovedFromPlay(Card SelectedMonster)
        {
            foreach (Abbility abb in SelectedMonster.Skills)
            {
                switch (abb.Kind)
                {
                    case "Deathrattle":
                        //TODO
                        break;
                    case "Aura":
                        DeactivateAura(SelectedMonster, abb);
                        break;
                    default:
                        break;
                }
            }
        }
        public void RemoveCardFromPlay(Card SelectedCard)
        {
            //serves for removing cards from play 
            //TODO PROPERLY - SO far only BOARD
            if (!Game.Fields[0].Remove(SelectedCard))
            {
                if (!Game.Fields[1].Remove(SelectedCard))
                {
                    if (!Game.Hands[0].Remove(SelectedCard))
                    {
                        if (!Game.Hands[1].Remove(SelectedCard))
                        {
                            //Card coouldnt be found
                            throw new Exception("Card not found Exception");
                        }
                    }
                }
            }
        }
   /*     public void PlayGame()
        {
            while (!end)
            {
                //do the game cycle
                bool turactive = true;
                //inicializace noveho kola
                EndTurn();
                InitialiseTurn();
                //player turn
                while (turactive)
                {
                    break;
                    //probiha hracovo kolo

                //state dependant
                //wait for action...
                //execute action
                //eventually end turn or go back to wait for action
                }

                break;
            }
        }*/
        public void SelectSecondaryTarget(Card SelectedCard)
        {
            Engine.SelectSecondaryTarget(SelectedCard, Game);
            /*
            if (Game.ValidTargetsP.Contains(SelectedCard))
            {
                Game.TargetForSomething = SelectedCard;
            }*/
        }
        public void SelectCardFromHand(Card SelectedCard)
        {
            /*
            Game.ValidTargetsP.Clear();
            //now I need to find out which card I selected - it is some card from active player -> can find if it is from hand or from table
            if (Game.Hands[Game.CurrentPlayer].Contains(SelectedCard))
            {
                //the card is in players hand
                if (SelectedCard.manacost <= Game.Manapool[Game.CurrentPlayer].availible)
                {
                    if (SelectedCard.Skills.Count > 0)
                    {
                        foreach (Abbility skill in SelectedCard.Skills)
                        {
                            List<Card> possibleTargets = Get_Targets(skill);
                            if (possibleTargets != null)
                            {
                                foreach (Card target in possibleTargets)
                                {
                                    Game.ValidTargetsP.Add(target);
                                }
                            }
                        }
                    }
                 //   PlayMonsterFromHand(SelectedCard);
                 //   Game.Manapool[Game.CurrentPlayer].availible -= SelectedCard.manacost;
                }
            }
            else
            {
                //the card is on the players battlefield
                //I want to attack
                if (Game.Fields[Game.CurrentPlayer].Contains(SelectedCard))
                {
                    Game.ValidTargetsP.Clear();
                    foreach (Card target in GetValidTargets(SelectedCard))
                    {
                        Game.ValidTargetsP.Add(target);
                    }
                 
                }
                //GetValidTargets(SelectedCard) ;
            }
*/
            Engine.SelectCardFromHand(SelectedCard, Game);
            if (ManaChanged != null)
                ManaChanged(this, new EventArgs());
        }
        
        
    }
}
