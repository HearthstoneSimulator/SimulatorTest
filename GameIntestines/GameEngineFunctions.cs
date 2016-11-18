using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Hearthstone
{
    public class StatBuffWrapper
    {
        public int AttackValue { get; set; }
        public int HPValue { get; set; }
        public string AttackModifierType { get; }
        public string HPModifierType { get; }
        public string Duration { get; }
        public string Description { get; }
        public StatBuffWrapper(int AVal, int HPVal, string AMType, string HPMType, string Duration, string Description = null)
        {
            this.AttackValue = AVal;
            this.HPValue = HPVal;
            this.AttackModifierType = AMType;
            this.HPModifierType = HPMType;
            this.Duration = Duration;
            if (Description == null)
            {
                this.Description = "+" + AttackValue + "/+" + HPValue;
            }
            else
            {
                this.Description = Description;
            }
        }
        

    }
   public enum TargetSelector
    {
        Player,All

    }

 
    public class GameEngineFunctions
    {
        public void TransformMinions(List<string> TargetTags, string Selector, Card Origin, int TargetCount, string TransformedInto, GameRepresentation Game)
        {
            switch (Selector)
            {
                case "PLAYER":
                    if (Game.Fields[0].Remove(Game.TargetForSomething))
                    {
                        MonsterRemovedFromPlay(Game.TargetForSomething, Game);
                        Game.FastSpellDamage[0] -= Game.TargetForSomething.currentSpelldmg;
                        SummonMonster(TransformedInto, 0, Game);
                    }
                    else
                    {
                        if (Game.Fields[1].Remove(Game.TargetForSomething))
                        {
                            MonsterRemovedFromPlay(Game.TargetForSomething, Game);
                            Game.FastSpellDamage[1] -= Game.TargetForSomething.currentSpelldmg;
                            SummonMonster(TransformedInto, 1, Game);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public void GiveStatBuffToCards(List<string> TargetTags, StatBuffWrapper Buff, GameRepresentation Game, Card Origin, string Selector, int number_of_targets)
        {
            List<Card> targets = new List<Card>();
            switch (Selector)
            {
                case "PLAYER":
                    GiveStatBuffToCard(Game.TargetForSomething, Buff, Game, Origin);
                    break;
                case "RANDOM":
                    targets = Get_Targets(TargetTags, Origin, Game);
                    for (int i = 0; i < number_of_targets; i++)
                    {
                        Random rng = new Random();
                        int rngint = rng.Next(0, targets.Count);
                        GiveStatBuffToCard(targets[rngint], Buff, Game, Origin);
                    }
                    break;
                case "ALL":
                    targets = Get_Targets(TargetTags, Origin, Game);
                    foreach (Card target in targets)
                    {
                        GiveStatBuffToCard(target, Buff, Game, Origin);
                    }
                    break;
                default:
                    break;
            }
        }
        public void GiveStatBuffToCard(Card Target, StatBuffWrapper Buff, GameRepresentation Game, Card Origin)
        {
            Target.ListOfStatBuffs.Add(Buff);
            if (Buff.AttackValue != 0)
            {
                if (Buff.AttackModifierType == "ADDITION")
                {
                    Target.currentattack += Buff.AttackValue;
                }
                if (Buff.AttackModifierType == "SUBSTRACTION")
                {
                    Target.currentattack -= Buff.AttackValue;
                }
            }
            if (Buff.HPValue != 0)
            {
                if (Buff.HPModifierType == "ADDITION")
                {
                    Target.currenthitpoints += Buff.HPValue;
                    Target.basehitpoints += Buff.HPValue;
                }
                if (Buff.HPModifierType == "SUBSTRACTION")
                {
                    Target.currenthitpoints -= Buff.HPValue;
                    Target.basehitpoints -= Buff.HPValue;
                    if (Target.currenthitpoints <=0)
                    {
                        KillMonster(Target, Game, Origin); //currently
                    }
                }

            }
        }
        public void CountAndReplace(List<string> targetTags, List<string> WhatToReplace, Card Origin, string TargetFunctionName, int TargetSkillPosition, int TargetFunctionPosition, GameRepresentation Game)
        {
            if (Origin.Skills[TargetSkillPosition].Functions[TargetFunctionPosition].name == TargetFunctionName)
            {

                CardFunctions functionToModify = null;
                switch (TargetFunctionName)
                {
                    case "Give_Buff":
                        functionToModify = (GiveBuff)Origin.Skills[TargetSkillPosition].Functions[TargetFunctionPosition];
                        break;
                    default:
                        //nothing to do here in current card base
                        break;
                }
            int NumberCounted = Get_Targets(targetTags, Origin, Game).Count;
            
            switch (TargetFunctionName)
            {
                case "Give_Buff":
                    if (WhatToReplace.Contains("Attack"))
                    {
                            (functionToModify as GiveBuff).BuffRepresentation.AttackValue = NumberCounted;
                    }
                        if (WhatToReplace.Contains("HP"))
                        {
                            (functionToModify as GiveBuff).BuffRepresentation.HPValue = NumberCounted;
                        }
                    break;
                default:
                    break;
            }

            }
        }
        public void FreezeTargets(List<string> targetTags, string Selector, Card Origin, GameRepresentation Game, int targetcount)
        {
            switch (Selector)
            {
                case "PLAYER":
                    FreezeTarget(Game.TargetForSomething, Game, Origin);
                    break; 
                case "ALL":
                    List<Card> frozenTargets = Get_Targets(targetTags, Origin, Game);
                    foreach (Card target in frozenTargets)
                    {
                        FreezeTarget(target, Game, Origin);
                    }
                    break;
                default:
                    break;
            }
        }
        public void FreezeTarget(Card Target,GameRepresentation Game, Card Origin)
        {
            DebugText(Origin.name + " freezed " + Target.name);
            GiveTagToCard(Target, "Frozen", Game);
            Target.frozen = 1;
        }
        public void GiveTagToCards(string Tag, List<string> TargetTags, Card Origin, int TargetCount, string Selector, GameRepresentation Game)
        {
            switch (Selector)
            {
                case "PLAYER":
                    GiveTagToCard(Game.TargetForSomething, Tag, Game);
                    break;
                default:
                    break;
            }
        }
        public void GiveTagToCard(Card Target, string TagToAdd, GameRepresentation Game)
        {
            if (!Target.tags.Contains(TagToAdd))
            {
                Target.tags.Add(TagToAdd);
            }
        }
        public void RemoveBuffFromCard(Card Target, StatBuffWrapper BuffToRemove, GameRepresentation Game)
        {
            if (Target.ListOfStatBuffs.Remove(BuffToRemove))
            {
                //I removed the buff from the list, now we need to update the card stats
                if (BuffToRemove.AttackValue != 0)
                {
                    if (BuffToRemove.AttackModifierType == "ADDITION")
                    {
                        Target.currentattack -= BuffToRemove.AttackValue;
                    }
                    if (BuffToRemove.AttackModifierType == "SUBSTRACTION")
                    {
                        Target.currentattack += BuffToRemove.AttackValue;
                    }
                }
                if (BuffToRemove.HPValue != 0)
                {
                    if (BuffToRemove.HPModifierType == "ADDITION")
                    {
                        Target.basehitpoints -= BuffToRemove.HPValue;
                        if (Target.currenthitpoints > Target.basehitpoints)
                        {
                            Target.currenthitpoints = Target.basehitpoints;
                        }
                    }
                    if (BuffToRemove.HPModifierType == "SUBSTRACTION")
                    {
                        Target.currenthitpoints += BuffToRemove.HPValue;
                        Target.basehitpoints += BuffToRemove.HPValue;
                        
                    }

                }
            }
        }
        public void ActivateAura(Card Origin, Abbility AuraActivated,GameRepresentation Game)
        {
            List<Card> originalValidTargets = Get_Targets(AuraActivated.TargetTags, Origin, Game);
            AuraActivated.Perform(null);
            foreach (Card target in originalValidTargets)
            {
                target.Auras.Add(AuraActivated);
            }
            /*
            foreach (XElement efekt in AuraActivated.Effects)
            {
                switch (efekt.Value)
                {
                    case "Give_Buff":
                        //this is what should happen anyway
                        if (efekt.Attribute("Attack") != null)
                        {
                            if (efekt.Attribute("AttackModificationType").Value == "ADDITION")
                            {
                                foreach (Card affectedMonster in Get_Targets(AuraActivated,Game))
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
            }*/
        }
        public void PerformAbbility(Card Origin, Abbility UsedAbbility, Card PossibleTarget,GameRepresentation Game)
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
                            Deal_Damage(Origin, Convert.ToInt32(efekt.Attribute("value").Value), solotarget, efekt.Attribute("selector").Value, efekt.Attribute("targets").Value,Game);
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
        public void Deal_Damage(Card Origin, int ammount, List<string> TargetTags, string selector, int number_of_targets, GameRepresentation Game)
        {
            List<Card> targets = new List<Card>();
            if (Origin.tags.Contains("Spell"))
            {
                ammount += Game.FastSpellDamage[Game.CurrentPlayer];
            }

            switch (selector)
            {
                case "PLAYER":
                    targets.Add(Game.TargetForSomething);
                    DealDamage(targets, ammount, Game, Origin);
                    //targets[0].currenthitpoints -= ammount;
                    break;
                case "RANDOM":
                    targets = Get_Targets(TargetTags, Origin, Game);
                    for (int i = 0; i < number_of_targets; i++)
                    {
                        Random rng = new Random();
                        int rngint = rng.Next(0, targets.Count);
                        DealDamage(targets[rngint], ammount, Game, Origin);
                       // targets[rngint].currenthitpoints -= ammount;
                    }
                    break;
                case "ALL":
                    targets = Get_Targets(TargetTags, Origin, Game);
                    DealDamage(targets, ammount, Game, Origin);
                    /*foreach (Card target in targets)
                    {
                        target.currenthitpoints -= ammount;
                    }*/
                    break;
                case "RANDOM_SPLIT":
                        targets = Get_Targets(TargetTags, Origin, Game);
                    int originalTargetCount = targets.Count;
                        for (int i = 0; i < ammount; i++)
                        {
                        if (originalTargetCount <= 0)
                        {
                            break;  
                        }
                            Random rng = new Random();
                            int rngint = rng.Next(0, originalTargetCount);
                        DealDamage(targets[rngint], 1, Game, Origin);
                        if (targets[rngint].currenthitpoints <=0)
                        {
                            targets.RemoveAt(rngint);
                            originalTargetCount--;
                        }
                           /* targets[rngint].currenthitpoints -= 1;
                        if (targets[rngint].currenthitpoints <=0)
                        {
                            KillMonster(targets[rngint], Game, Origin);
                            targets.RemoveAt(rngint);
                            originalTargetCount--;
                        }*/
                        }
                    break;
                default:
                    break;
            }
            /*
            foreach (Card tar in targets)
            {
                if (tar.currenthitpoints <= 0)
                {
                    KillMonster(tar, Game, Origin);
                }
            }*/
        }
        public void Deal_Damage(Card Origin, int ammount, List<Card> targets, string selector, string number_of_targets, GameRepresentation Game)
        {
            switch (selector)
            {
                case "PLAYER":
                    targets[0].currenthitpoints -= ammount;
                    break;
                case "RANDOM":
                    for (int i = 0; i < Convert.ToInt32(number_of_targets); i++)
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
                    KillMonster(tar, Game, Origin);
                }
            }
        }
        public void Heal(int ammount, Card Target, GameRepresentation Game)
        {
            if (Target.currenthitpoints < Target.basehitpoints )
            {
                if (Target.currenthitpoints + ammount < Target.basehitpoints)
                {
                    Target.currenthitpoints += ammount;

                }
                else
                {
                    Target.currenthitpoints = Target.basehitpoints;
                }
            }
            
        }
        public void AuraTigger(GameRepresentation Game)
        {
            foreach (Abbility aura in Game.ActiveAuras)
            {
                ActivateAura(aura.Owner, aura,Game);
            }
        }
        public void MonsterGetsPlayed(Card SelectedCard, GameRepresentation Game)
        {
            foreach (Abbility ski in SelectedCard.Skills)
            {
                if (ski.Kind == "Battlecry")
                {
                    //PerformAbbility(SelectedCard, ski, Game.TargetForSomething,Game);
                    ski.Perform(Game.TargetForSomething);
                }
            }
        }
        public void MonsterComesIntoPlay(Card SelectedCard, GameRepresentation Game)
        {
            foreach (Abbility ski in SelectedCard.Skills)
            {
                if (ski.Kind == "Aura")
                {
                    Game.ActiveAuras.Add(ski);
                }
            }
            AuraTigger(Game);
            Game.FastSpellDamage[Game.CurrentPlayer] += SelectedCard.currentSpelldmg;

        }
        public void PlaySpellFromHand(Card SelectedCard, GameRepresentation Game)
        {
            if (SelectedCard.needsTargetSelected)
            {
                foreach (Abbility eff in SelectedCard.Skills)
                {
                    if (eff.Kind != "Triggered")
                    {
                        eff.Perform(Game.TargetForSomething);
                    }
                }

            }
            else
            {
                foreach (Abbility eff in SelectedCard.Skills)
                {
                    eff.Perform(Game.TargetForSomething);//check this
                }
            }
        }
        public void PlayMonsterFromHand(Card SelectedCard,GameRepresentation Game)
        {
            //now I need to find out which card I selected - it is some card from active player -> can find if it is from hand or from table
            if (Game.Hands[Game.CurrentPlayer].Contains(SelectedCard))
            {
                //the card is in players hand
                if (SelectedCard.manacost <= Game.Manapool[Game.CurrentPlayer].availible && SelectedCard.tags.Contains("Minion"))
                {
                    //I have mana to play the card

                    Game.Hands[Game.CurrentPlayer].Remove(SelectedCard);
                    Game.Manapool[Game.CurrentPlayer].availible -= SelectedCard.manacost;
                    Game.Fields[Game.CurrentPlayer].Add(SelectedCard);
                    MonsterComesIntoPlay(SelectedCard,Game);
                    MonsterGetsPlayed(SelectedCard,Game);

                    /*
                    if (ManaChanged != null)
                        ManaChanged(this, new EventArgs());*/
                }
                else
                {
                    if (SelectedCard.manacost <= Game.Manapool[Game.CurrentPlayer].availible && SelectedCard.tags.Contains("Spell"))
                    {
                        //I am trying to play a spell
                        Game.Hands[Game.CurrentPlayer].Remove(SelectedCard);
                        Game.Manapool[Game.CurrentPlayer].availible -= SelectedCard.manacost;
                        PlaySpellFromHand(SelectedCard, Game);
                    }
                }

            }
            else
            {
                //the card is on the players battlefield
                //I am attacking the target (that is already valid and selected)
                if (Game.TargetForSomething != null && SelectedCard != null)
                {
                    AttackWithMonster(SelectedCard, Game.TargetForSomething,Game);

                }

            }
            GetSelectableCards(Game);
        }
        public void SelectCardFromHand(Card SelectedCard, GameRepresentation Game)
        {
            Game.ValidTargetsP.Clear();
            //now I need to find out which card I selected - it is some card f rom active player -> can find if it is from hand or from table
            if (Game.Hands[Game.CurrentPlayer].Contains(SelectedCard))
            {
                //the card is in players hand
                if (SelectedCard.manacost <= Game.Manapool[Game.CurrentPlayer].availible)
                {
                    if (SelectedCard.tags.Contains("Minion"))
                    {
                        //The card is a minion
                    
                        //check if I have mana for the card -> yes
                        if (SelectedCard.Skills.Count > 0)
                        {
                            //the card has some skills
                            foreach (Abbility skill in SelectedCard.Skills)
                            {
                                if (skill.Kind == "Battlecry" && !skill.TargetTags.Contains("NonTargetable") )
                                {
                                    //the skill is a battlecry and I need to select a target for it
                                    List<Card> possibleTargets = Get_Targets(skill,Game);
                                    if (possibleTargets != null)
                                    {
                                        foreach (Card target in possibleTargets)
                                        {
                                            Game.ValidTargetsP.Add(target);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (SelectedCard.tags.Contains("Spell"))
                        {
                            //the card is a spell
                            if (SelectedCard.needsTargetSelected)
                            {
                                //the spell is targetable
                                //convention: first skill is the one determining the target
                                List<Card> possibleTargets = Get_Targets(SelectedCard.Skills[0], Game);
                                if (possibleTargets != null)
                                {
                                    if (possibleTargets.Count != 0)
                                    {
                                        //I can process the targets to the player
                                        foreach (Card target in possibleTargets)
                                        {
                                            Game.ValidTargetsP.Add(target);
                                        }
                                    }
                                    else
                                    {
                                        //there was a mistake
                                        Game.SelectableCards.Remove(SelectedCard);
                                        
                                    }
                                }
                            }
                            else
                            {
                                //the spell is not targetable -> I will just select it
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
                    foreach (Card target in GetValidTargets(SelectedCard, Game))
                    {
                        Game.ValidTargetsP.Add(target);
                    }

                }
                else
                {
                    //it might be a player
                    if (Game.Players[Game.CurrentPlayer] == SelectedCard)
                    {
                        Game.ValidTargetsP.Clear();
                        foreach (Card target in GetValidTargets(SelectedCard, Game))
                        {
                            Game.ValidTargetsP.Add(target);
                        }
                    }
                }
                //GetValidTargets(SelectedCard) ;
            }
            /*
            if (ManaChanged != null)
                ManaChanged(this, new EventArgs());*/
        }
        public void GetSelectableCards(GameRepresentation Game)
        {
            Game.SelectableCards.Clear();

            foreach (Card card in Game.Hands[Game.CurrentPlayer])
            {
                if (card.manacost <= Game.Manapool[Game.CurrentPlayer].availible)
                {
                    if (card.tags.Contains("Minion"))
                    {
                        Game.SelectableCards.Add(card);
                    }
                    else
                    {
                        if (card.tags.Contains("Spell"))
                        {
                            List<Card> possibleTargets = Get_Targets(card.Skills[0], Game);
                            if (possibleTargets != null)
                            {
                                if (possibleTargets.Count != 0 || card.needsTargetSelected == false)
                                {
                                    Game.SelectableCards.Add(card);
                                }
                                else
                                {
                                    
                                }
                            }
                        }
                    }
                }
            }
            foreach (Card card in Game.Fields[Game.CurrentPlayer])
            {
                if (CanAttack(card,Game))
                {
                    Game.SelectableCards.Add(card);
                }
            }
            if (Game.Players[Game.CurrentPlayer].currentattack > 0)
            {
                //player can also attack
                if (CanAttack(Game.Players[Game.CurrentPlayer],Game))
                {
                    Game.SelectableCards.Add(Game.Players[Game.CurrentPlayer]);
                }
            }
        }
        public void SelectSecondaryTarget(Card SelectedCard, GameRepresentation Game)
        {
            if (Game.ValidTargetsP.Contains(SelectedCard))
            {
                Game.TargetForSomething = SelectedCard;
            }
        }
        public void DebugText(string outString)
        {
            if (true)
            {
                Console.WriteLine(outString);
            }
        }
        public void DrawCard(int TargetPlayer, GameRepresentation Game)
        {
            //handle card drawing listeners
            if (Game.Decks[TargetPlayer].Count >= 1)
            {
                if (Game.Hands[TargetPlayer].Count < 10)
                {
                    DebugText("Player " + TargetPlayer + " drew " + Game.Decks[TargetPlayer][0].name);
                    Game.Hands[TargetPlayer].Add(Game.Decks[TargetPlayer][0]);
                }
                else
                {
                    DebugText("Player " + TargetPlayer + " couldn't draw " + Game.Decks[TargetPlayer][0].name + " because of full hand");
                }
                    Game.Decks[TargetPlayer].RemoveAt(0);
          /*      if (ManaChanged != null)
                    ManaChanged(this, new EventArgs());*/
            }
            else
            {
                Game.Players[Game.CurrentPlayer].fatigue++;
                DebugText("player " + Game.Players[TargetPlayer] + " Cannot draw cards and suffered "+Game.Players[Game.CurrentPlayer].fatigue+" fatigue damage");
                DealDamage(Game.Players[Game.CurrentPlayer], Game.Players[Game.CurrentPlayer].fatigue, Game, Game.EvulEngine);
                
            }
        }
        public void DiscardCards(int CardAmmount, List<string> targetTags, GameRepresentation Game)
        {
            if (targetTags.Contains("You"))
            {
                for (int i = 0; i < CardAmmount; i++)
                {
                    DiscardCard(Game.CurrentPlayer, Game);
                }
            }
        }

        public void DiscardCard(int TargetPlayer, GameRepresentation Game)
        {
            if (Game.Hands[TargetPlayer].Count > 0)
            {
                Random rng = new Random();
                int removelocation = rng.Next(Game.Hands[TargetPlayer].Count);
                DebugText(Game.Hands[TargetPlayer][removelocation].name + " was discarded");
                Game.Hands[TargetPlayer].RemoveAt(removelocation); 

            }
        }
        private void IncreaseMana(int TargetPlayer, GameRepresentation Game)
        {
            Game.Manapool[TargetPlayer].Turn();
            /*
            if (ManaChanged != null)
                ManaChanged(this, new EventArgs());*/
        }
        public void UseHeroPower(GameRepresentation Game)
        {
            //todo
        }
        public void ResetHeroPowerUsages(GameRepresentation Game)
        {
            Game.HeroPowerUsages[Game.CurrentPlayer] = 0;
        }
        public void ResetMonsterAttacks(GameRepresentation Game)
        {

            foreach (Card monster in Game.Fields[Game.CurrentPlayer])
            {
                monster.AttackedXTimes = 0;
            }
            foreach (Card monster in Game.Fields[Game.CurrentPlayer])
            {
                if (monster.tags.Contains("Frozen"))
                {
                    if (monster.frozen == 0)
                    {
                        monster.tags.Remove("Frozen");
                    }
                    else
                    {
                        monster.frozen--;
                    }
                }
                
            }
            /*
            for (int i = 0; i < 2; i++)
            {

                foreach (Card Monster in Game.Fields[i])
                {
                    Monster.AttackedXTimes = 0;
                }
            }*/
        }
        public void IncreaseMonsterTurns(GameRepresentation Game)
        {
            foreach (Card Monster in Game.Fields[Game.CurrentPlayer])
            {
                Monster.turnsingame++;
            }
        }
        public void InitialiseTurn(GameRepresentation Game)
        {
            int oldplayer = Game.CurrentPlayer;
            //increase turn counter
            Game.TurnsTotal++;
            IncreaseMonsterTurns(Game);
            ResetMonsterAttacks(Game);
            ResetHeroPowerUsages(Game);
            //handle start turn listeners
            //TODO

            //increase mana
            IncreaseMana(Game.CurrentPlayer, Game);
            //draw card
            DrawCard(Game.CurrentPlayer, Game);
            GetSelectableCards(Game);
            if (Game.isThisPlayerAi[Game.CurrentPlayer])
            {
                DebugText("AI player "+Game.CurrentPlayer + " turn "+Game.TurnsTotal +" started");
                bool shouldEndTurn = false;
                while (!shouldEndTurn)
                {
                    Game.Inteligences[Game.CurrentPlayer].getAction(Game).Perform(this, Game);
                    if (Game.CurrentPlayer != oldplayer)
                    {
                        shouldEndTurn = true;
                    }
                }
            }
            #region stuff
            /*
             *  
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
                                  Console.WriteLine("Monster " + akce.FirstCardSelection + " attakced " + akce.SecondCardSelection);

                              }
                              break;
                          default:
                              break;
                      }
                  }

              }
            */
            #endregion
        }
        public void test(GameRepresentation Game)
        {
            Card a = Game.Hands[0][1];
            Card b = Game.Hands[0][1];
            if (object.ReferenceEquals(a,b))
            {
                DebugText("YAY");
            }
            else
            {
                DebugText("NAY");
            }
        }
        public void EndTurn(GameRepresentation Game)
        {
            //Game.CurrentPlayer = Game.CurrentPlayer + 1 - Game.CurrentPlayer*2;
            for (int i = 0; i < 2; i++)
            {

            foreach (Card minionInPlay in Game.Fields[i])
            {
                    for (int j = 0; j < minionInPlay.ListOfStatBuffs.Count; j++)
                    {
                        if (minionInPlay.ListOfStatBuffs[j].Duration == "EOT")
                        {
                            RemoveBuffFromCard(minionInPlay, minionInPlay.ListOfStatBuffs[j], Game);
                            j--;
                        }
                    }
                    
            }
                for (int j = 0; j < Game.Players[i].ListOfStatBuffs.Count; j++)
                {
                    if (Game.Players[i].ListOfStatBuffs[j].Duration == "EOT")
                    {
                        RemoveBuffFromCard(Game.Players[i], Game.Players[i].ListOfStatBuffs[j], Game);
                        j--;
                    }

                }
                
            }
            Game.CurrentPlayer = GetOtherPlayer(Game.CurrentPlayer);
            InitialiseTurn(Game);
        }
        public int GetOtherPlayer(int currentPlayer)
        {
            return currentPlayer + 1 - currentPlayer * 2;
        }
        public bool CheckTagForValidity(Tags tag, Card Target, Card Origin, GameRepresentation Game)
        {
            switch (tag)
            {
                case Tags.Self:
                    if (Target == Origin)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case Tags.Other:
                    if (Target == Origin)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case Tags.Any:
                    return true;
                case Tags.Own:
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
                case Tags.Enemy:
                    if (Game.Fields[GetOtherPlayer(Game.CurrentPlayer)].Contains(Target))
                    {
                        return true;
                    }
                    else
                    {
                        //this should actually never even happen
                        if (Game.Hands[GetOtherPlayer(Game.CurrentPlayer)].Contains(Target))
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
                    if (Target.tags2.Contains(tag))
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
        public bool CheckTagForValidity(string tag, Card Target, Card Origin, GameRepresentation Game)
        {
            switch (tag)
            {
                case "UniqueInstance":
                    if (Target.Auras.Contains(Origin.Skills[0]))
                    {
                        return false;
                    }
                    return true;
                case "NonTargetable":
                    return true;
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
                    if (object.ReferenceEquals(Target,Origin))
                        //Target == Origin)
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
                    if (Game.Fields[Game.CurrentPlayer].Contains(Target) || Game.Players[Game.CurrentPlayer] == Target )
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
                    if (Game.Fields[GetOtherPlayer(Game.CurrentPlayer)].Contains(Target) || Game.Players[GetOtherPlayer(Game.CurrentPlayer)] == Target)
                    {
                        return true;
                    }
                    else
                    {
                        //this should actually never even happen
                        if (Game.Hands[GetOtherPlayer(Game.CurrentPlayer)].Contains(Target))
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
        public bool CheckTagsForValidity(List<string> Tags, Card Target, Card Origin, GameRepresentation Game)
        {
            bool passed = true;
            foreach (string tag in Tags)
            {
                if (!CheckTagForValidity(tag, Target, Origin, Game ))
                {
                    return false;
                }
            }

            return passed;
        }
        public Card GetSelectedTarget(GameRepresentation Game)
        {
            return Game.TargetForSomething;
        }
        
        public List<Card> Get_Targets(List<string> TargetTags,Card OriginCard, GameRepresentation Game)
        {
            List<Card> targets = new List<Card>();
            for (int i = 0; i < 2; i++)
            {
                foreach (Card possibleTarget in Game.Fields[i])
                {
                    if (CheckTagsForValidity(TargetTags, possibleTarget, OriginCard, Game))
                    {
                        targets.Add(possibleTarget);
                    }
                }
                if (CheckTagsForValidity(TargetTags, Game.Players[i], OriginCard, Game))
                {
                    targets.Add(Game.Players[i]);
                }
            }

            return targets;
        }
        public List<Card> Get_Targets(Abbility abb, GameRepresentation Game)
        {
            List<Card> targets = new List<Card>();
            for (int i = 0; i < 2; i++)
            {
                foreach (Card possibleTarget in Game.Fields[i])
                {
                    if (CheckTagsForValidity(abb.TargetTags, possibleTarget, abb.Owner, Game))
                    {
                        targets.Add(possibleTarget);
                    }
                }
                if (CheckTagsForValidity(abb.TargetTags, Game.Players[i], abb.Owner, Game))
                {
                    targets.Add(Game.Players[i]);
                }
            }

            return targets;
        }
        public void DestroyMonsters(List<string> targetTags, string selector, int targetCount, Card Origin, GameRepresentation Game)
        {
            switch (selector)
            {
                case "PLAYER":
                    if (CheckTagsForValidity(targetTags,Game.TargetForSomething,Origin,Game))
                    {
                        KillMonster(Game.TargetForSomething, Game, Origin);
                    }
                    break;
                case "ALL":
                    //not needed so far
                    break;
                default:
                    break;
            }
        }
        public void DeactivateAura(Card Origin, Abbility AuraDeactivated, GameRepresentation Game)
        {
            Game.ActiveAuras.Remove(AuraDeactivated);
            foreach (Card minion in Game.Fields[0])
            {
                if (minion.Auras.Contains(AuraDeactivated))
                {

                    if (AuraDeactivated.Functions[0] is GiveBuff)
                    {
                        for (int i = 0; i < minion.ListOfStatBuffs.Count; i++)
                        {
                            
                            if (minion.ListOfStatBuffs[i] == (AuraDeactivated.Functions[0] as GiveBuff).BuffRepresentation)
                            {
                                RemoveBuffFromCard(minion, minion.ListOfStatBuffs[i], Game);
                                i--;
                            }
                                                        
                        }
                    }
                }
            }
            /*
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

                                foreach (Card affectedMonster in Get_Targets(AuraDeactivated, Game))
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
            }*/
        }
        public void KillMonster(Card SelectedMonster,GameRepresentation Game, Card Origin)
        {
            foreach (Abbility abb in Origin.Skills)
            {
                if (abb.Kind == "Triggered")
                {
                    if (abb.Trigger == "KillTarget")
                    {
                        abb.Perform(null);
                    }
                }
            }
            //TODO TRIGGER FOR MONSTER DEATH
            RemoveCardFromPlay(SelectedMonster,Game);
            MonsterRemovedFromPlay(SelectedMonster,Game);
        }
        public void MonsterRemovedFromPlay(Card SelectedMonster,GameRepresentation Game)
        {
            foreach (Abbility abb in SelectedMonster.Skills)
            {
                switch (abb.Kind)
                {
                    case "Deathrattle":
                        //TODO
                        break;
                    case "Aura":
                        DeactivateAura(SelectedMonster, abb,Game);
                        break;
                    default:
                        break;
                }
            }
            if (SelectedMonster.currentSpelldmg >= 0)
            {
            }
        }
        public void RemoveCardFromPlay(Card SelectedCard,GameRepresentation Game)
        {
            if (SelectedCard.currentSpelldmg >= 0)
            {
                if (Game.Fields[0].Contains(SelectedCard))
                {
                    Game.FastSpellDamage[0] -= SelectedCard.currentSpelldmg;
                }
                if (Game.Fields[1].Contains(SelectedCard))
                {
                    Game.FastSpellDamage[1] -= SelectedCard.currentSpelldmg;
                }
            }
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
                            if (Game.Players[0] == SelectedCard)
                            {
                                Game.Players.Remove(SelectedCard);
                                DebugText("player 0 (you) was defeated!");
                                System.Environment.Exit(0);
                            }
                            else
                            {
                                if (Game.Players[1] == SelectedCard)
                                {
                                    Game.Players.Remove(SelectedCard);
                                    DebugText("player 1 (your opponent) was defeated!");
                                    System.Environment.Exit(0);
                                }
                                else
                                {
                                    DebugText("Card couldnt be removed");
                                    throw new Exception("Card not found Exception");
                               }
                            }
                        }
                    }
                }
            }
        }
        public void DealDamage(Card Target, int ammount, GameRepresentation Game, Card Origin)
        {
            string orig = "Engine";
            if (Origin != null)
            {
                orig = Origin.name;

            }
            string tar = Target.name;
            if (orig == null)
            {
                orig = "Engine";
            }
            if (tar == null)
            {
                tar = "unknown error";
            }
            DebugText(orig + " dealt " + ammount + " dmg to " + tar);
            Target.currenthitpoints -= ammount;
            foreach (Abbility abb in Target.Skills)
            {
                if (abb.Kind == "Triggered")
                {
                    if (abb.Trigger == "Self_Takes_Damage")
                    {
                        abb.Perform(Target);
                    }
                }
            }
            foreach (Abbility abb in Origin.Skills)
            {
                if (abb.Kind == "Triggered")
                {
                    if (abb.Trigger == "Self_Deals_Damage")
                    {
                        abb.Perform(Target);
                    }
                }
            }
            if (Target.currenthitpoints <= 0)
            {
                KillMonster(Target, Game, Origin);
            }
            
        }
        public void DealDamage(List<Card> Target, int ammount,GameRepresentation Game, Card Origin)
        {
            foreach (Card target in Target)
            {
                DealDamage(target, ammount, Game, Origin);
                /*
                target.currenthitpoints -= ammount;
                if (target.currenthitpoints <= 0)
                {
                    KillMonster(target,Game, Origin);
                }*/
            }
        }
        public void Clash(Card Attacker, Card Target,GameRepresentation Game)
        {
            //TODO: DEALDAMAGE(Target,Ammount), DEALDAMAGE(Attacker,Ammount), 
            int attackerdmg = Attacker.currentattack;
            int targetdmg = Target.currentattack;
            List<Card> attacker = new List<Card>();
            attacker.Add(Attacker);
            List<Card> target = new List<Card>();
            target.Add(Target);
            DealDamage(target, attackerdmg,Game,Attacker);
            DealDamage(attacker, targetdmg,Game, Target);
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
        public void AttackWithMonster(Card SelectedMonster, Card SelectedTarget,GameRepresentation Game)
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
            if (tmplist.Capacity < 2 || tmplist[0] == null || tmplist[1] == null)
            {
                //something happened that canceled the attack
                return;
            }
            //TODO: IF_MONSTER STILL_ATTACKS -> CLASH(MONSTER,TARGET)
            Clash(tmplist[0], tmplist[1],Game);
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
        public bool CanAttack(Card MonsterCard, GameRepresentation Game)
        {
            if (MonsterCard.currentattack <= 0)
            {
                return false;
            }
            if (MonsterCard.tags.Contains("Frozen"))
            {
                return false;
            }
            if (MonsterCard.turnsingame == 0 && !MonsterCard.tags.Contains("Charge") && MonsterCard != Game.Players[Game.CurrentPlayer])
            {
                return false;
            }
            if (MonsterCard.AttackedXTimes >= 1 && !MonsterCard.tags.Contains("Windfury"))
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

        public void SummonMonster(string MonsterName,int WhichPlayerSummons, GameRepresentation Game)
        {
            //TODO: Trigger "MonsterSummoned"
            foreach (Card mbysummon in Game.AllCards)
            {
                if (mbysummon.ToString() == MonsterName)
                {
                    Game.Fields[WhichPlayerSummons].Add(mbysummon.Clone());
                    break;
                }
            }
        }
    }
}
