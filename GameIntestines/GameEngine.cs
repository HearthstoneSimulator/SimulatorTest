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


        public void EndTurn()
        {
            //Game.CurrentPlayer = Game.CurrentPlayer + 1 - Game.CurrentPlayer*2;

            Engine.EndTurn(Game);
            /*Game.CurrentPlayer = GetOtherPlayer(Game.CurrentPlayer);
            InitialiseTurn();*/
        }
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
        

        public void StartOneGame()
        {
            while (!Game.EndTheGame)
            {
                //InitialiseTurn();

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

        public void DebugText(string outString)
        {
            if (true)
            {
                Console.WriteLine(outString);
            }
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

    }
}
