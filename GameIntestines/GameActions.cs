using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GameIntestines
{
    public abstract class GenericAction //: GameEngineFunctions
    {
        public abstract void Perform(GameEngineFunctions Engine, GameRepresentation Game);


    }
    public class EndTurnAction : GenericAction
    {

        public override void Perform(GameEngineFunctions Engine, GameRepresentation Game)
        {
            Engine.DebugText("Performed EndTurnAction");
            Engine.EndTurn(Game);
            //throw new NotImplementedException();
        }
    }
    public class SelectCardAction : GenericAction
    {
        private Card SelectedCard;
        //needed for player interaction (in theory)
        public SelectCardAction(Card SelectedCard)
        {
            this.SelectedCard = SelectedCard;
        }
        public override void Perform(GameEngineFunctions Engine, GameRepresentation Game)
        {
            Engine.DebugText("selected " + SelectedCard.name + " from hand");
            Engine.SelectCardFromHand(SelectedCard, Game);
           // throw new NotImplementedException();
        }
    }



    public class PlayCardFromHandAction : GenericAction
    {
        private Card SelectedCard;
        private Card PossibleTarget;

        public PlayCardFromHandAction(Card SelectedCard, Card PossibleTarget)
        {
            this.SelectedCard = SelectedCard;
            this.PossibleTarget = PossibleTarget;
        }
        public override void Perform(GameEngineFunctions Engine, GameRepresentation Game)
        {
            Engine.DebugText("PlayCardAction");
            Engine.PlayMonsterFromHand(SelectedCard, Game);
            //throw new NotImplementedException();
        }
        public Card getTarget()
        {
            return PossibleTarget;
        }
    }
    public class UseHeroPower : GenericAction
    {
        private Card PossibleTarget;
        public UseHeroPower(Card PossibleTarget = null)
        {
            this.PossibleTarget = PossibleTarget;
        }
        public override void Perform(GameEngineFunctions Engine, GameRepresentation Game)
        {
            Engine.DebugText("Used Hero Power");
        }
    }

    public class SelectTargetAction : GenericAction
    {
        private Card SelectedCard;
        //probably needed for player interaction
        public SelectTargetAction(Card SelectedCard)
        {
            this.SelectedCard = SelectedCard;
        }
        public override void Perform(GameEngineFunctions Engine, GameRepresentation Game)
        {
            Engine.DebugText("selectTarget");
            Engine.SelectSecondaryTarget(SelectedCard, Game);
           // throw new NotImplementedException();
        }
    }
    public class AttackWithMonsterAction : GenericAction
    {
        private Card AttackingMonster;
        private Card SelectedTarget;
        public AttackWithMonsterAction(Card AttackingMonster, Card SelectedTarget)
        {
            this.AttackingMonster = AttackingMonster;
            this.SelectedTarget = SelectedTarget;
        }
        public override void Perform(GameEngineFunctions Engine, GameRepresentation Game)
        {
            Engine.DebugText("attack action with "+AttackingMonster.name+" on "+SelectedTarget);
            if (Engine.CanAttack(AttackingMonster, Game))
            {
                Engine.AttackWithMonster(AttackingMonster, SelectedTarget, Game);
            }
            //throw new NotImplementedException();
        }


    }
    public class DealDamageAction : GenericAction
    {
        private Card Owner;
        private int AmmountOfTargets;
        private int DmgPerTarget;
        private TargetSelector Selector;
        private List<XElement> TargetTags;
        public DealDamageAction(Card Owner, int AmmountOfTargets, int DmgPerTarget, TargetSelector TargetSelect, List<XElement> TargetTags)
        {
            this.Owner = Owner;
            this.AmmountOfTargets = AmmountOfTargets;
            this.DmgPerTarget = DmgPerTarget;
            this.Selector = TargetSelect;
            this.TargetTags = TargetTags;

        }
        public override void Perform(GameEngineFunctions Engine, GameRepresentation Game)
        {
            Engine.DebugText("attempted to perform dealdamageaction which is currently not implemented cause it is cheating!");
            //throw new NotImplementedException();
        }
    }
    class GameActions
    {
    }
}
