using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearthstone
{
    public abstract class CardFunctions 
    {
        public List<string> TargetTags;
        public Card AssociatedCard;

        abstract public void Perform(GameEngineFunctions engine, GameRepresentation Game);
    }
    public class DrawCard : CardFunctions
    {
        public int ammountOfCards;
        public string Selector;
        public DrawCard(int ammountOfCards, List<string> targets, string Selector, Card AssociatedCard)
        {
            this.TargetTags = targets;
            this.ammountOfCards = ammountOfCards;
            this.AssociatedCard = AssociatedCard;
            this.Selector = Selector;
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game)
        {
            if (TargetTags.Contains("You"))
            {
                for (int i = 0; i < ammountOfCards; i++)
                {
                    engine.DrawCard(AssociatedCard.owner, Game);
                }
            }
            if (TargetTags.Contains("Opponent"))
            {
                for (int i = 0; i < ammountOfCards; i++)
                {
                    engine.DrawCard(engine.GetOtherPlayer(AssociatedCard.owner), Game);
                }
            }
        }
    }
    public class CountEntities : CardFunctions
    {
        public CountEntities(List<string> TargetTags, Card AssociatedCard)
        {
            this.TargetTags = TargetTags;
            this.AssociatedCard = AssociatedCard;
        }

        public override void Perform(GameEngineFunctions engine, GameRepresentation Game)
        {
            AssociatedCard.tmpCount = engine.Get_Targets(TargetTags, AssociatedCard, Game).Count;
        }
    }
    public class GiveBuff : CardFunctions 
    {

        public override void Perform(GameEngineFunctions engine, GameRepresentation Game)
        {
            throw new NotImplementedException();
        }
    }
    public class Heal : CardFunctions
    {
        public int amount;
        public int targetCount;
        public string selector;
       
        public Heal (Card AssociatedCard, List<string> TargetTags ,int Amount, string Selector, int Targets )
        {
            this.AssociatedCard = AssociatedCard;
            this.amount = Amount;
            this.selector = Selector;
            this.targetCount = Targets;
            this.TargetTags = TargetTags;
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game)
        {
            if (true)
            {

            }
            //TODO
        }
    }
    public class DealDamage : CardFunctions
    {
        int Value;
        int TargetCount;
        string Selector;

        public DealDamage(int value, int targets, string selector, Card AssociatedCard, List<string> targetTags )
        {
            this.AssociatedCard = AssociatedCard;
            this.Value = value;
            this.TargetCount = targets;
            this.Selector = selector;
            this.TargetTags = targetTags;
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game)
        {
            switch (Selector)
            {
                case "PLAYER":
                    List<Card> tmpTarget = new List<Card>();
                    tmpTarget.Add(Game.TargetForSomething);
                    engine.DealDamage(Game.TargetForSomething, Value, Game);
                    break;
                case "ALL":
                    foreach (Card validTarget in engine.Get_Targets(TargetTags,AssociatedCard,Game))
                    {
                        engine.DealDamage(validTarget, Value, Game);
                    }
                    
                    break;
                case "AUTO":
                    foreach (Card validTarget in engine.Get_Targets(TargetTags, AssociatedCard, Game))
                    {
                        engine.DealDamage(validTarget, Value, Game);
                    }
                    break;

                default:
                    //includes RANDOM, which is not used in current card set
                    //should not happen
                    break;
            }
        }
    }
    public class Summon : CardFunctions
    {
        string MonsterToSummon;
        public Summon(string SummonedMonster,Card AssociatedCard, List<string> TargetTags)
        {
            MonsterToSummon = SummonedMonster;
            this.AssociatedCard = AssociatedCard;
            this.TargetTags = TargetTags;
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game)
        {
            if (TargetTags.Contains("You"))
            {
                engine.SummonMonster(MonsterToSummon, Game.CurrentPlayer, Game);

            }
            if (TargetTags.Contains("Enemy"))
            {
                engine.SummonMonster(MonsterToSummon, engine.GetOtherPlayer(Game.CurrentPlayer), Game);
            }
        }
    }

}
