using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameIntestines
{
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
        GameEngineFunctions engine = new GameEngineFunctions();
        public override GenericAction getAction(GameRepresentation Game)
        {
            int me = Game.CurrentPlayer;
            int opponent = engine.GetOtherPlayer(Game.CurrentPlayer);
            if (Game.HeroPowerUsages[me] == 0)
            {
                List<Card> weaklings = new List<Card>();
                foreach (Card monster in Game.Fields[opponent])
                {
                    if (monster.currenthitpoints == 1)
                    {
                        weaklings.Add(monster);
                    }
                }
                if (weaklings.Count >= 1)
                {
                    return new UseHeroPower(weaklings[0]);
                }
            }
            int lethal = Game.Players[opponent].currenthitpoints;
            List<Card> targetsToDestroy = new List<Card>();
            foreach (Card monster in Game.Fields[opponent])
            {
                if (monster.tags.Contains("Taunt"))
                {

                }
            }

            int myBoardDmg = 0;
            foreach (Card monster in Game.Fields[Game.CurrentPlayer])
            {
                if (engine.CanAttack(monster, Game))
                {
                    myBoardDmg += monster.currentattack;
                }
            }


            return new EndTurnAction();
            //throw new NotImplementedException();
        }
    }
    public class RandomAI : GenericAI
    {
        GameEngineFunctions engine = new GameEngineFunctions();

        public override GenericAction getAction(GameRepresentation Game)
        {
            Random rng = new Random();
            if (true)//rng.Next(2) == 1)
            {
                List<GenericAction> akce = new List<GenericAction>();
                ObservableCollection<Card> tmpCardList = Game.SelectableCards;
                int choice = rng.Next(tmpCardList.Count);
                if (tmpCardList.Count == 0)
                {
                    return new EndTurnAction();
                }
                else
                {
                    Card karta = tmpCardList[choice];
                    if (engine.CanAttack(karta, Game))
                    {
                        List<Card> trgets = engine.GetValidTargets(karta, Game);
                        return new AttackWithMonsterAction(karta, trgets[rng.Next(trgets.Count)]);
                    }
                    else
                    {
                        //it is not a monster taht can attack - it is a card we can play from our hand
                        if (karta.needsTargetSelected)
                        {
                            List<Card> tars = engine.Get_Targets(karta.Skills[0], Game);
                            //some error prevention
                            if (tars.Count == 0)
                            {
                                return new EndTurnAction();
                            }
                            Card target = tars[rng.Next(tars.Count)];
                            //Game.TargetForSomething = target;
                            return new PlayCardFromHandAction(karta, target);
                        }
                        else
                        {
                            return new PlayCardFromHandAction(karta, null);
                        }
                    }
                }



                return new EndTurnAction();
            }
            //engine = null; 


        }
    }
    public class MyCustomAI : GenericAI
    {
        GameEngineFunctions engine = new GameEngineFunctions();

        /// <summary>
        /// gets action
        /// </summary>
        /// <param name="Game"></param>
        /// <returns></returns>
        public override GenericAction getAction(GameRepresentation Game)
        {

            List<Card> kartykzahrani = new List<Card>();
            foreach (Card kartazruky in Game.Hands[Game.CurrentPlayer])
            {
                if (kartazruky.manacost <= Game.Manapool[Game.CurrentPlayer].availible && kartazruky.Skills.Count == 0)
                {
                    //kartykzahrani.Add(kartazruky);
                    // return new AIAction(1, kartazruky);
                    return new PlayCardFromHandAction(kartazruky, null);
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
        public bool CanAttack(Card MonsterCard, GameRepresentation Game)
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
            if (GetValidTargets(MonsterCard, Game).Capacity == 0)
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
                if (kartazruky.manacost <= game.Manapool[game.CurrentPlayer].availible && kartazruky.Skills.Count == 0)
                {
                    //kartykzahrani.Add(kartazruky);
                    return new AIAction(1, kartazruky);
                }
            }
            foreach (Card kartanastole in game.Fields[game.CurrentPlayer])
            {
                if (CanAttack(kartanastole, game))
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
}
