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
    public enum Tags
    {
        //Target related asdr s
        Self,Other,Any,Own,Enemy
        
    }
    public class AIAction
    {
        public int ActionIndex;
        public Card FirstCardSelection;
        public Card SecondCardSelection;
        public AIAction(int i)
        {
            this.ActionIndex = i;
        }
        public AIAction(int i, Card FirstCard)
        {
            this.ActionIndex = i;
            this.FirstCardSelection = FirstCard;
        }
        public AIAction(int i, Card FirstCard, Card SecondCard)
        {
            this.ActionIndex = i;
            this.FirstCardSelection = FirstCard;
            this.SecondCardSelection = SecondCard;
        }
    }
    public abstract class GenericAI
    {
        public abstract GenericAction getAction(GameRepresentation Game);
    }
    public class PlayerNotAi : GenericAI
    {

        public override GenericAction getAction(GameRepresentation Game)
        {
            
            throw new NotImplementedException();
        }
    }
    public class MyCustomAI : GenericAI
    {
        GameEngineFunctions engine = new GameEngineFunctions();

        public override GenericAction getAction(GameRepresentation Game)
        {

            List<Card> kartykzahrani = new List<Card>();
            foreach (Card kartazruky in Game.Hands[Game.CurrentPlayer])
            {
                if (kartazruky.manacost <= Game.Manapool[Game.CurrentPlayer].availible && kartazruky.Skills.Count == 0)
                {
                    //kartykzahrani.Add(kartazruky);
                   // return new AIAction(1, kartazruky);
                    return new PlayCardFromHandAction(kartazruky,null);
                }
            }
            foreach (Card kartanastole in Game.Fields[Game.CurrentPlayer])
            {
                if (engine.CanAttack(kartanastole, Game))
                {
                    List<Card> targets = engine.GetValidTargets(kartanastole, Game);
                    if (targets.Contains(Game.Players[0]))
                    {
                       // return new AIAction(2, kartanastole, Game.Players[0]);
                        return new AttackWithMonsterAction(kartanastole, Game.Players[engine.GetOtherPlayer(Game.CurrentPlayer)]);
                    }
                    else
                    {
                        if (targets.Count != 0)
                        {
                           // return new AIAction(2, kartanastole, targets[0]);
                            return new AttackWithMonsterAction(kartanastole, targets[0]);
                        }

                    }
                }
            }
           // return new AIAction(0);
            return new EndTurnAction();
            //return new AttackWithMonsterAction(null, null);
            //throw new NotImplementedException();
        }
    }
    public class AI
    {
        public List<Card> GetValidTargets(Card SelectedCard, GameRepresentation Game)
        {
            List<Card> ValidTargets = new List<Card>();
            foreach (Card EnemyMonster in Game.Fields[Convert.ToInt32(!Convert.ToBoolean(Game.CurrentPlayer))])
            {
                if (EnemyMonster.tags.Contains("Taunt") && !EnemyMonster.tags.Contains("STEALTH") && !EnemyMonster.tags.Contains("IMMUNE"))
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
        public bool CanAttack(Card MonsterCard,GameRepresentation Game)
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
            if (GetValidTargets(MonsterCard,Game).Capacity == 0)
            {
                return false;
            }

            return true;
        }
        public AIAction getAction(GameRepresentation game)
        {
            List<Card> kartykzahrani = new List<Card>();
            foreach (Card kartazruky in game.Hands[game.CurrentPlayer])
            {
                if (kartazruky.manacost<=game.Manapool[game.CurrentPlayer].availible &&kartazruky.Skills.Count == 0)
                {
                    //kartykzahrani.Add(kartazruky);
                    return new AIAction(1, kartazruky);
                }
            }
            foreach (Card kartanastole in game.Fields[game.CurrentPlayer])
            {
                if (CanAttack(kartanastole,game))
                {
                    List<Card> targets = GetValidTargets(kartanastole, game);
                    if (targets.Contains(game.Players[0]))
                    {
                        return new AIAction(2, kartanastole, game.Players[0]);
                    }
                    else
                    {
                        if (targets.Count != 0)
                        {
                        return new AIAction(2, kartanastole, targets[0]);

                        }
                        
                    }
                }
            }
            return new AIAction(0);
        }
    }
   

    public class Card
    {

        public string name; //jméno karty a zároveň identifikátor
        public int baseattack;
        public int basehitpoints;
        public int currentattack;
        public int currenthitpoints;
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
        public enum Target { SINGLE, NONE };
        public Target target;
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
                +"\n CurrentAttack: "+currentattack+"\n BaseHP: "+basehitpoints+
                "\n currhp: "+currenthitpoints;
            return outstr;

        }
        public override string ToString()
        {
            return name;
        }
        public Card Clone()
        {
            Card cc = new Card();
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
            cc.Skills = this.Skills;
           // cc.tags = this.tags;
            cc.turnsingame = this.turnsingame;
            foreach (string  tag in this.tags)
            {
                cc.tags.Add(tag);
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
        public string Kind;
        public Card Owner;
        public List<string> TargetTags;
        public List<Tags> TargetTags2;
        public List<XElement> Effects;
        public string description;
        public Abbility(String Kind )
        {
            this.Kind = Kind;
            this.TargetTags = new List<string>();
            this.TargetTags2 = new List<Tags>();
            this.Effects = new List<XElement>();
        }
        public void Perform(Card PossibleTarget)
        {
            foreach (XElement effect in Effects)
            {
                switch (effect.Value)
                {
                    case "Deal_Damage":
                        
                        break;
                    default:
                        break;
                }
            }
        }
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
    }

    class GameEngine 
    {
        XDocument Xdoc;
        GameRepresentation Game;
        public event EventHandler ManaChanged;
        public int mana = 0;
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
        }/*
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
        }
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
            Game  = new GameRepresentation();
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

            //Creating decks and setting up the game itself
            for (int i = 0; i < 2; i++)
            {
                Game.Manapool.Add( new Mana());
                Game.Hands.Add(new ObservableCollection<Card>());
                Game.Decks.Add(new ObservableCollection<Card>());
                Game.Fields.Add(new ObservableCollection<Card>());
                Game.Players.Add(new Card());
                
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
            Random rng = new Random(1);
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
            //Determine starting player (and give the opponent the coin)
            Game.ActivePlayer = true; // I am the active player
            Game.isThisPlayerAi.Add(false);
            Game.isThisPlayerAi.Add(true);
            Game.AIs.Add(null);
            Game.AIs.Add( new AI());
            Game.Inteligences.Add(null);
            Game.Inteligences.Add(new MyCustomAI());
            //Start the game
            IncreaseMana(Game.CurrentPlayer);
            DrawCard(Game.CurrentPlayer);

            Game.Hand1.Add(Game.AllCards[1].Clone());
            Game.Hand1.Add(Game.AllCards[1].Clone());
            GetSelectableCards();
        }
        private void StartGameCycle()
        {
            while (true)
            {
                Game.Inteligences[Game.CurrentPlayer].getAction(Game).Perform(Game);
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
            Game.CurrentPlayer = GetOtherPlayer(Game.CurrentPlayer);
            InitialiseTurn();
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
                    Game.Inteligences[Game.CurrentPlayer].getAction(Game).Perform(Game);
                    if (Game.CurrentPlayer != oldplayer)
                    {
                        shouldEndTurn = true;
                    }
                }
            }
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
                    PerformAbbility(SelectedCard, ski, Game.TargetForSomething);

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
            GetSelectableCards();
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
        public void PlayGame()
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
        }
        public void SelectSecondaryTarget(Card SelectedCard)
        {
            if (Game.ValidTargetsP.Contains(SelectedCard))
            {
                Game.TargetForSomething = SelectedCard;
            }
        }
        public void SelectCardFromHand(Card SelectedCard)
        {
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

            if (ManaChanged != null)
                ManaChanged(this, new EventArgs());
        }

        
    }
}
