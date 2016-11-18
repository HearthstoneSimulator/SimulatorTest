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
        public string name;
        abstract public void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard);
        abstract public CardFunctions Clone();
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
            this.name = "Draw_Card";
        }

        public override CardFunctions Clone()
        {
            return this;
        }

        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            if (TargetTags.Contains("You"))
            {
                for (int i = 0; i < ammountOfCards; i++)
                {
                    engine.DrawCard(SauceCard.owner, Game);
                }
            }
            if (TargetTags.Contains("Opponent"))
            {
                for (int i = 0; i < ammountOfCards; i++)
                {
                    engine.DrawCard(engine.GetOtherPlayer(SauceCard.owner), Game);
                }
            }
        }
    }
    public class CountAndReplace : CardFunctions
    {
        List<string> whatToReplace;
        string targetFncName;
        int targetSkillpos;
        int targetFncPos;
        public CountAndReplace(List<string> TargetTags, Card AssociatedCard, List<string> WhatToReplace, string TargetFunctionName, int TargetSkillPosition, int TargetFunctionPosition)
        {
            this.TargetTags = TargetTags;
            this.AssociatedCard = AssociatedCard;
            this.targetFncName = TargetFunctionName;
            this.targetFncPos = TargetFunctionPosition;
            this.targetSkillpos = TargetSkillPosition;
            this.whatToReplace = WhatToReplace;
            this.name = "CountAndReplace";
        }

        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            engine.CountAndReplace(TargetTags, whatToReplace, SauceCard, targetFncName, targetSkillpos, targetFncPos, Game);
        }

        public override CardFunctions Clone()
        {
            return this;
        }
    }
    public class GiveBuff : CardFunctions 
    {
        //int AttackModifierValue;
        //int HPModifierValue;
        //string AttackModifierType;
        //string HPModifierType;
        //string Duration;
        string Selector;
        int targetCount;
        public StatBuffWrapper BuffRepresentation;
        public GiveBuff(List<string> TargetTags,Card AssociatedCard, int attackValue, string attackModificationType, int HPValue, string HPModificationType, string Selector, string duration, int targets)
        {
            this.TargetTags = TargetTags;
            this.AssociatedCard = AssociatedCard;
            this.BuffRepresentation = new StatBuffWrapper(attackValue, HPValue, attackModificationType, HPModificationType, duration); 
            //this.AttackModifierValue = attackValue;
            //this.HPModifierValue = HPValue;
            //this.AttackModifierType = attackModificationType;
            //this.HPModifierType = HPModificationType;
            this.Selector = Selector;
            //this.Duration = duration;
            this.targetCount = targets;
            this.name = "Give_Buff";
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            engine.GiveStatBuffToCards(TargetTags, BuffRepresentation, Game, SauceCard, Selector, targetCount);
            /*
            if (Selector == "PLAYER")
            {
               // engine.GiveStatBuffToCard(Game.TargetForSomething, BuffRepresentation, Game);
            }*/
        }

        public override CardFunctions Clone()
        {
            return new GiveBuff(this.TargetTags, this.AssociatedCard, this.BuffRepresentation.AttackValue, this.BuffRepresentation.AttackModifierType, this.BuffRepresentation.HPValue, this.BuffRepresentation.HPModifierType, this.Selector, this.BuffRepresentation.Duration, this.targetCount);
        }
    }
    public class Heal : CardFunctions
    {
        public int amount;
        public int targetCount;
        public string Selector;
       
        public Heal (Card AssociatedCard, List<string> TargetTags ,int Amount, string Selector, int Targets )
        {
            this.AssociatedCard = AssociatedCard;
            this.amount = Amount;
            this.Selector = Selector;
            this.targetCount = Targets;
            this.TargetTags = TargetTags;
            this.name = "Heal";
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            switch (Selector)
            {
                case "PLAYER":
                    engine.Heal(amount, Game.TargetForSomething, Game);
                    break;
                case "ALL":
                    foreach (Card validTarget in engine.Get_Targets(TargetTags,SauceCard,Game))
                    {
                        engine.Heal(amount, validTarget, Game);
                    }
                    break;
                default:
                    break;
            }
            //TODO
        }

        public override CardFunctions Clone()
        {
            return this;
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
            this.name = "Deal_Damage";
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            engine.Deal_Damage(SauceCard, Value, TargetTags, Selector, TargetCount, Game);/*
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
                case "RANDOM":
                    engine.Deal_Damage(AssociatedCard, Value, TargetTags, Selector, TargetCount, Game);
                    break;
                default:
                    //includes RANDOM, which is not used in current card set
                    //should not happen
                    break;
            }*/
        }
        public override CardFunctions Clone()
        {
            return this;
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
            this.name = "Summon";
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
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
        public override CardFunctions Clone()
        {
            return this;
        }
    }
    public class Discard : CardFunctions
    {
        int DiscardAmmount;
        string Selector;
        public Discard(Card AssociatedCard, List<string> TargetTags, int AmmountOfCards, string Selector)
        {
            this.AssociatedCard = AssociatedCard;
            this.TargetTags = TargetTags;
            this.DiscardAmmount = AmmountOfCards;
            this.Selector = Selector;
            this.name = "Discard";
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            engine.DiscardCards(DiscardAmmount, TargetTags, Game);
        }
        public override CardFunctions Clone()
        {
            return this;
        }
    }
    public class Destroy : CardFunctions
    {
        string selector;
        int ammountOfTargets;
        public Destroy(Card AssociatedCard, List<string> TargetTags, int AmmountOfTargets, string Selector )
        {
            this.ammountOfTargets = AmmountOfTargets;
            this.selector = Selector;
            this.AssociatedCard = AssociatedCard;
            this.TargetTags = TargetTags;
            this.name = "Destroy";
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            engine.DestroyMonsters(TargetTags, selector, ammountOfTargets, SauceCard, Game);
        }
        public override CardFunctions Clone()
        {
            return this;
        }
    }
    public class Freeze : CardFunctions
    {
        string Selector;
        int targets;
        public Freeze(Card associatedCard, string Selector,int targetCount,List<string> targetTags )
        {
            this.AssociatedCard = associatedCard;
            this.Selector = Selector;
            this.targets = targetCount;
            this.TargetTags = targetTags;
            this.name = "Freeze";
        }

        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            engine.FreezeTargets(TargetTags, Selector, SauceCard, Game, targets);
        }
        public override CardFunctions Clone()
        {
            return this;
        }
    }
    public class Transform : CardFunctions
    {
        string Selector;
        int targets;
        string transformedInto;
        public Transform(Card AssociatedCard, string Selector, int TargetCount, string TransformedName, List<string> TargetTags)
        {
            this.AssociatedCard = AssociatedCard;
            this.Selector = Selector;
            this.targets = TargetCount;
            this.TargetTags = TargetTags;
            this.transformedInto = TransformedName;
        }

        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            engine.TransformMinions(TargetTags, Selector, SauceCard, targets, transformedInto, Game);
        }
        public override CardFunctions Clone()
        {
            return this;
        }
    }
    public class GiveTag : CardFunctions
    {
        string TagToGive;
        string Selector;
        int targets;
        public GiveTag(List<string> TargetTags,string Selector, int TargetCount, Card AssociatedCard, string TagToGive)
        {
            this.TagToGive = TagToGive;
            this.Selector = Selector;
            this.AssociatedCard = AssociatedCard;
            this.TargetTags = TargetTags;
            this.targets = TargetCount;
        }
        public override void Perform(GameEngineFunctions engine, GameRepresentation Game, Card SauceCard)
        {
            engine.GiveTagToCards(TagToGive, TargetTags, SauceCard, targets, Selector, Game);
        }
        public override CardFunctions Clone()
        {
            return this;
        }
    }

}
