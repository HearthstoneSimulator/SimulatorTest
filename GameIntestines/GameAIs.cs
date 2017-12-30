using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameIntestines
{
    public class AISelector
    {
        public GenericAI GetAI(string name)
        {
            switch (name)
            {
                case "RandomAI":
                    return new RandomAI();

                case "FaceHunterAI":
                    return new FaceHunterAI();
                default:
                    Console.WriteLine("No AI matched the selected name. Exiting.");
                    Environment.Exit(1);
                    return null;
                    
            }
        }
    }
    public abstract class GenericAI
    {
        public abstract GenericAction getAction(GameRepresentation Game);
    }

    public class RandomAI : GenericAI
    {
        GameEngineFunctions engine = new GameEngineFunctions();

        public override GenericAction getAction(GameRepresentation Game)
        {
            Random rng = new Random();
            if (true)//rng.Next(2) == 1) eventually possible to end turn at random
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
                            return new PlayCardFromHandAction(karta, target);
                        }
                        else
                        {
                            return new PlayCardFromHandAction(karta, null);
                        }
                    }
                }



                
            } 
            return new EndTurnAction();

        }
    }
    public class FaceHunterAI : GenericAI
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
}
