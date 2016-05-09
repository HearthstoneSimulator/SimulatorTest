using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearthstone
{
    abstract class CardFunctions 
    {
        public List<string> TargetTags;
        public Card AssociatedCard;

        abstract public void Perform(GameEngineFunctions engine, GameRepresentation Game);
    }
    class DrawCard : CardFunctions
    {
        public int ammountOfCards;
        public DrawCard(int ammountOfCards, List<string> targets, Card AssociatedCard)
        {
            this.TargetTags = targets;
            this.ammountOfCards = ammountOfCards;
            this.AssociatedCard = AssociatedCard;
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
    class CountEntities : CardFunctions
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
    class GiveBuff : CardFunctions 
    {

        public override void Perform(GameEngineFunctions engine, GameRepresentation Game)
        {
            throw new NotImplementedException();
        }
    }
    class Heal : CardFunctions
    {
        public int ammount;
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game)
        {
            //TODO
        }
    }
    class DealDamage : CardFunctions
    {

        public override void Perform(GameEngineFunctions engine, GameRepresentation Game)
        {
            throw new NotImplementedException();
        }
    }

}
