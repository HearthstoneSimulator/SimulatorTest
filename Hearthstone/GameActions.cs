using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Hearthstone
{
    public abstract class GenericAction : GameEngineFunctions
    {
        public abstract void Perform(GameRepresentation Game);


    }
    public class EndTurnAction : GenericAction
    {

        public override void Perform(GameRepresentation Game)
        {
            base.EndTurn(Game);
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
        public override void Perform(GameRepresentation Game)
        {
            base.SelectCardFromHand(SelectedCard, Game);
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
        public override void Perform(GameRepresentation Game)
        {
            base.PlayMonsterFromHand(SelectedCard, Game);
            //throw new NotImplementedException();
        }
    }
    public class UseHeroPower : GenericAction
    {

        public override void Perform(GameRepresentation Game)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectTargetAction : GenericAction
    {
        //probably needed for player interaction
        public override void Perform(GameRepresentation Game)
        {
            throw new NotImplementedException();
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
        public override void Perform(GameRepresentation Game)
        {
            if (base.CanAttack(AttackingMonster, Game))
            {
                base.AttackWithMonster(AttackingMonster, SelectedTarget, Game);
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
        public override void Perform(GameRepresentation Game)
        {

            //throw new NotImplementedException();
        }
    }
    class GameActions
    {
    }
}
