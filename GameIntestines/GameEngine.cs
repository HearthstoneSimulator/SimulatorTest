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
        public bool WriteDebugTexts = true;
        public event EventHandler ManaChanged;
        public GenericAction PlayersAction;
        public int mana = 0;
        public string deck0name = "random";
        public string deck1name = "random";
        public string AI0name;
        public string AI1name;
        GameLoadState LoadedGamestate;
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
        
        public ReadOnlyObservableCollection<Card> OpponentAvatarTest
        {
            get
            {
                ObservableCollection<Card> opptest = new ObservableCollection<Card>();
                opptest.Add(Game.Players[1]);
                
                return  new ReadOnlyObservableCollection<Card>(opptest); //Game.Hand1;
            }
        }
        public ReadOnlyObservableCollection<Card> Player0Avatar
        {
            get
            {
                ObservableCollection<Card> P0test = new ObservableCollection<Card>();
                P0test.Add(Game.Players[0]);

                return new ReadOnlyObservableCollection<Card>(P0test); //Game.Hand1;
            }
        }
        public Card TargetForSomethingTest
        {
            get
            {
                if (Game != null)
                {
                    return Game.TargetForSomething;
                }
                return null;
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
        public bool IsPlayer1AI
        {
            get
            {
                return Game.isThisPlayerAi[1];
            }
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
            try
            {
                LoadedGamestate = Loader.LoadGameState("GameState.xml");
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured in loading gamstate from file");
                //throw;
            }
            Engine.DebugText("**********************GAME INITIATION STARTED**********************");
        }
        /// <summary>
        /// Sets game for simulation by reseting certain parts of GameRepresentation to the initial state (or any desired state for that matter)
        /// </summary>
        public void setStuff()
        {
            AISelector selector = new AISelector();
            List<Card> AllCardsBackup = Game.AllCards;
            List<Decklist> decksBackup = Game.decklists;
            Card evulenginebackup = Game.EvulEngine;
            Game.reset();
            Game.AllCards = AllCardsBackup;
            Game.decklists = decksBackup;
            Game.EvulEngine = evulenginebackup;
            
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
            Game.ActivePlayer = true; // I am the active player
            Game.isThisPlayerAi.Add(true);
            Game.isThisPlayerAi.Add(true);
            Game.Inteligences.Add(selector.GetAI(AI0name));
            Game.Inteligences.Add(selector.GetAI(AI1name));
            
            Game.winner = -1;
        }
        public void simulateOneGame()
        {
            //Engine.TurnManagement(Game);

            Engine.InitialiseTurn(Game);
            while (!Game.EndTheGame)
            {
                GenericAction a = Game.Inteligences[Game.CurrentPlayer].getAction(Game);

                if (a is PlayCardFromHandAction)
                {
                    PlayCardFromHandAction p = a as PlayCardFromHandAction;
                    Game.TargetForSomething = p.getTarget();
                }
                //Game.TargetForSomething = 
                a.Perform(Engine, Game);
            }

        }
        public int getWinner()
        {
            return Game.winner;
        }
        public int getTurns()
        {
            return Game.TurnsTotal;
        }
        
        /// <summary>
        /// LoadGame is used for loading the game from predefined game state. User can use 
        /// </summary>
        /// <param name="GameState"></param>
        public void LoadGame(string GameState = "default")
        {
            try
            {
                bool useGamestate = false;
                deck0name = "basic_mage.txt";
                deck1name = "basic_mage.txt";
                prepareGame();
                if (LoadedGamestate != null)
                {
                    useGamestate = true;
                }
                AISelector selector = new AISelector();
                List<Card> AllCardsBackup = Game.AllCards;
                List<Decklist> decksBackup = Game.decklists;
                Card evulenginebackup = Game.EvulEngine;
                Game.reset();
                Game.AllCards = AllCardsBackup;
                Game.decklists = decksBackup;
                Game.EvulEngine = evulenginebackup;

                if (useGamestate)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Game.Manapool.Add(LoadedGamestate.ManaCrystals[i]);
                        Game.Hands.Add(new ObservableCollection<Card>());
                        Game.Decks.Add(new ObservableCollection<Card>());
                        Game.Fields.Add(new ObservableCollection<Card>());
                        Game.Players.Add(new Card());

                        Game.FastSpellDamage.Add(0);
                        Game.FastSpellDamage.Add(0);
                        Game.Players[i].basehitpoints = LoadedGamestate.PlayerMAX[i];
                        Game.Players[i].currenthitpoints = LoadedGamestate.PlayerHitpoints[i];
                        Game.Players[i].tags.Add("Player");
                    }
                }
                else
                {

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
                }
                Game.Field1 = Game.Fields[0];
                Game.Field2 = Game.Fields[1];
                Game.Players[0].name = "PLAYER0";
                Game.Players[1].name = "PLAYER1";
                Game.Hand1 = Game.Hands[0];
                Game.Hand2 = Game.Hands[1];
                //            Random rng = new Random(1);
                Random rng = new Random();

                //Make decks
                Game.Decks[0] = Game.decklists[0].clone(true, rng);
                Game.Decks[1] = Game.decklists[1].clone(true, rng);
                Game.Deck1 = Game.Decks[0];
                Game.Deck2 = Game.Decks[1];

                //Deal cards to players
                if (useGamestate)
                {
                    Card cardToAdd;
                    for (int i = 0; i < LoadedGamestate.P0Hand.Count; i++)
                    {
                        for (int j = 0; j < Game.Deck1.Count; j++)
                        {
                            if (LoadedGamestate.P0Hand[i] == Game.Deck1[j].name)
                            {
                                cardToAdd = Game.Deck1[j];
                                Game.Deck1.RemoveAt(j);
                                Game.Hand1.Add(cardToAdd);
                                break;//card was added
                            }
                            if (j == Game.Deck1.Count-1)
                            {
                                Console.WriteLine("Card {0} was not included in the player 0 deck", LoadedGamestate.P0Hand[i]);
                            }
                        }
                    }


                    for (int i = 0; i < LoadedGamestate.P1Hand.Count; i++)
                    {
                        for (int j = 0; j < Game.Deck2.Count; j++)
                        {
                            if (LoadedGamestate.P1Hand[i] == Game.Deck2[j].name)
                            {
                                cardToAdd = Game.Deck2[j];
                                Game.Deck2.RemoveAt(j);
                                Game.Hand2.Add(cardToAdd);
                                break;
                            }
                            if (j == Game.Deck2.Count - 1)
                            {
                                Console.WriteLine("Card {0} was not included in the player 1 deck", LoadedGamestate.P0Hand[i]);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        //this.P1Hand.Items.Add(Game.Deck1[0].name);
                        Game.Hand1.Add(Game.Deck1[0]);
                        Game.Deck1.RemoveAt(0);
                        //this.P0Hand.Items.Add(Game.Deck2[0].name);
                        Game.Hand2.Add(Game.Deck2[0]);
                        Game.Deck2.RemoveAt(0);
                    }
                }

                if (useGamestate)
                {
                    //put monsters on board
                    for (int i = 0; i < LoadedGamestate.P0Board.Count; i++)
                    {
                        Card loadedMonster = LoadedGamestate.P0Board[i];
                        Card monsterToAdd = Engine.GetCopyOfCardFromDatabase(loadedMonster.name, Game);
                        monsterToAdd.currentattack = loadedMonster.currentattack;
                        monsterToAdd.currenthitpoints = loadedMonster.currenthitpoints;
                        monsterToAdd.baseattack = loadedMonster.baseattack;
                        monsterToAdd.basehitpoints = loadedMonster.basehitpoints;
                        Game.Field1.Add(monsterToAdd);
                    }

                    for (int i = 0; i < LoadedGamestate.P1Board.Count; i++)
                    {
                        Card loadedMonster = LoadedGamestate.P1Board[i];
                        Card monsterToAdd = Engine.GetCopyOfCardFromDatabase(loadedMonster.name, Game);
                        monsterToAdd.currentattack = loadedMonster.currentattack;
                        monsterToAdd.currenthitpoints = loadedMonster.currenthitpoints;
                        monsterToAdd.baseattack = loadedMonster.baseattack;
                        monsterToAdd.basehitpoints = loadedMonster.basehitpoints;
                        Game.Field2.Add(monsterToAdd);
                    }

                    //Perform played cards 
                    try
                    {
                        for (int i = 0; i < LoadedGamestate.P0Actions.Count; i++)
                        {
                            PlayCardActionForLoadingGamestate p = LoadedGamestate.P0Actions[i];
                            Card cardToPlayFromOutsideOfTheGameToAffectTheGameState = Engine.GetCopyOfCardFromDatabase(p.CardName, Game);
                            if (p.PossibleTarget == 0)
                            {
                                Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                            }
                            else
                            {
                                if (p.PossibleTarget == -1)
                                {
                                    Game.TargetForSomething = Game.Players[1];
                                    Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                                }
                                else
                                {
                                    if (p.PossibleTarget == 1)
                                    {
                                        Game.TargetForSomething = Game.Players[0];
                                        Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                                    }
                                    else
                                    {
                                        if (p.PossibleTarget < 0)
                                        {
                                            int pos = p.PossibleTarget * -1;
                                            pos = pos - 2;
                                            Game.TargetForSomething = Game.Field2[pos];
                                            Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                                        }
                                        else
                                        {
                                            int pos = p.PossibleTarget - 2;
                                            Game.TargetForSomething = Game.Field1[pos];
                                            Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                                        }
                                    }
                                }
                            }
                        }
                        Game.CurrentPlayer = Engine.GetOtherPlayer(Game.CurrentPlayer);
                        for (int i = 0; i < LoadedGamestate.P1Actions.Count; i++)
                        {
                            PlayCardActionForLoadingGamestate p = LoadedGamestate.P1Actions[i];
                            Card cardToPlayFromOutsideOfTheGameToAffectTheGameState = Engine.GetCopyOfCardFromDatabase(p.CardName, Game);
                            if (p.PossibleTarget == 0)
                            {
                                Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                                
                            }
                            else
                            {
                                if (p.PossibleTarget == -1)
                                {
                                    Game.TargetForSomething = Game.Players[1];
                                    Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                                }
                                else
                                {
                                    if (p.PossibleTarget == 1)
                                    {
                                        Game.TargetForSomething = Game.Players[0];
                                        Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                                    }
                                    else
                                    {
                                        if (p.PossibleTarget < 0)
                                        {
                                            int pos = p.PossibleTarget * -1;
                                            pos = pos - 2;
                                            Game.TargetForSomething = Game.Field2[pos];
                                            Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                                        }
                                        else
                                        {
                                            int pos = p.PossibleTarget - 2;
                                            Game.TargetForSomething = Game.Field1[pos];
                                            Engine.PlaySpellFromHand(cardToPlayFromOutsideOfTheGameToAffectTheGameState, Game);
                                        }
                                    }
                                }
                            }
                        }

                        Game.CurrentPlayer = Engine.GetOtherPlayer(Game.CurrentPlayer);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("One of the loaded actions has invalid parametres and was not performed." );
                        throw;
                    }
                }
                //initalise hero powers
                for (int i = 0; i < 2; i++)
                {
                    Game.HeroPowerUsages.Add(0);
                }
                Game.ActivePlayer = true; // I am the active player
                Game.isThisPlayerAi.Add(false);
                Game.isThisPlayerAi.Add(true);
                Game.Inteligences.Add(new RandomAI());
                Game.Inteligences.Add(new RandomAI());

                Game.winner = -1;
                if (useGamestate)
                {
                    Engine.GetSelectableCards(Game);
                }
                else
                {
                Engine.InitialiseTurn(Game);

                }
            }
            catch (Exception)
            {
                Console.WriteLine("Game initialisation failed.");
                Environment.Exit(1);
                //throw;
            }
        }
        
        
        /// <summary>
        /// Initialise game is used for initialising the game for the Hearthstone application in graphical mode. It is equivalent in a way of calling prepareGame and Stuff but is used this way as using
        /// the graphical mode is expected to be used for different purposes with different game setup.
        /// </summary>
        public void InitialiseGame()
        {
            try
            {


            Game = new GameRepresentation();
            Game.ConsoleMode = false;
            Engine = new GameEngineFunctions();
            Engine.writeDebugTexts = WriteDebugTexts;
            Game.EvulEngine = new Card();
            Game.EvulEngine.name = "Engine";
            //select deck name manually
            Game.decklists.Add(new Decklist("basic_mage.txt"));
            Game.decklists.Add(new Decklist("basic_mage.txt"));
            CardLoader Loader = new CardLoader(Engine, Game);
            
            //preparing two decklists
            Loader.LoadCards("XMLCardBase.xml");
            Loader.LoadDecks();
            Engine.DebugText("**********************GAME INITIATION STARTED**********************");
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
                Game.Players[0].name = "PLAYER0";// "ME";
                Game.Players[1].name = "PLAYER1";// "OPPONENT";
            Game.Hand1 = Game.Hands[0];
            Game.Hand2 = Game.Hands[1];
            Game.Deck1 = Game.Decks[0];
            Game.Deck2 = Game.Decks[1];
            Random rng = new Random();

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
            Game.isThisPlayerAi.Add(false);// true);// true); //false
                Game.isThisPlayerAi.Add(true);// true);
            //select kind of AI manually
            Game.Inteligences.Add(new RandomAI()); 
            Game.Inteligences.Add(new RandomAI());
            //Start the game
            
            //example of adding certain cards to hand for testing

            Game.Hand1.Add(Engine.GetCopyOfCardFromDatabase("Polymorph", Game));
            Game.Hand1.Add(Game.AllCards[76].Clone());


            Game.winner = -1;
                //Engine.TurnManagement(Game);
                //replaced by
                Engine.InitialiseTurn(Game);

            }
            catch (Exception)
            {
                Console.WriteLine("There was an error during initialisation.");
                Environment.Exit(1);
            }
        }

        public void ProcessInputFromGUI(GenericAction Action)
        {
            try
            {

                Action.Perform(Engine, Game);

                if (ManaChanged != null)
                    ManaChanged(this, new EventArgs());
                if (Game.EndTheGame)
                {
                    Environment.Exit(0);
                }
                while (Game.isThisPlayerAi[Game.CurrentPlayer])
                {
#if VERBOSE
                DebugText("AI player "+Game.CurrentPlayer + " turn "+Game.TurnsTotal +" started");
#endif
                    
                    GenericAction a = Game.Inteligences[Game.CurrentPlayer].getAction(Game);

                    if (a is PlayCardFromHandAction)
                    {
                        PlayCardFromHandAction p = a as PlayCardFromHandAction;
                        Game.TargetForSomething = p.getTarget();
                    }
                    //Game.TargetForSomething = 
                    a.Perform(Engine, Game);
                    if (Game.EndTheGame)
                    {
                        Environment.Exit(0);
                    }

                }

            

                /*

                                if (Action is SelectCardAction)
                                {
                                    Action.Perform(Engine, Game);
                                }
                                if (Action is EndTurnAction)
                                {
                                    Action.Perform(Engine, Game);
                                }*/
            }
            catch (Exception)
            {
                Console.WriteLine("Your last action caused an error.");
                //throw;
            }
        }
        public void EndTurn()
        {
            try
            {
                Engine.EndTurn(Game);
            }
            catch (Exception)
            {
                Console.WriteLine("Your last action caused an error.");
                //throw;
            }

        }
        public void SelectSecondaryTarget(Card SelectedCard)
        {
            try
            {
            Engine.SelectSecondaryTarget(SelectedCard, Game);

            }
            catch (Exception)
            {
                Console.WriteLine("Your last action caused an error.");
                //throw;
            }

        }
        public void SelectCardFromHand(Card SelectedCard)
        {
            try
            {
            Engine.SelectCardFromHand(SelectedCard, Game);
            if (ManaChanged != null)
                ManaChanged(this, new EventArgs());

            }
            catch (Exception)
            {
                Console.WriteLine("Your last action caused an error.");
                //throw;
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
            try
            {
            Engine.PlayMonsterFromHand(SelectedCard, Game);

            }
            catch (Exception)
            {
                Console.WriteLine("Your last action caused an error.");
                //throw;
            }
        }

    }
}
