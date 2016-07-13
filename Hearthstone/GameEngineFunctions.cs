using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Hearthstone
{
   public enum TargetSelector
    {
        Player,All

    }

 
    public class GameEngineFunctions
    {
        public void ActivateAura(Card Origin, Abbility AuraActivated,GameRepresentation Game)
        {
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
            }
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
                    KillMonster(tar, Game);
                }
            }
        }
        public void Heal(int ammount, Card Target, GameRepresentation Game)
        {
            //todo
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
                    PerformAbbility(SelectedCard, ski, Game.TargetForSomething,Game);

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

                    MonsterGetsPlayed(SelectedCard,Game);

                    Game.Fields[Game.CurrentPlayer].Add(SelectedCard);
                    Game.Hands[Game.CurrentPlayer].Remove(SelectedCard);
                    MonsterComesIntoPlay(SelectedCard,Game);
                    Game.Manapool[Game.CurrentPlayer].availible -= SelectedCard.manacost;/*
                    if (ManaChanged != null)
                        ManaChanged(this, new EventArgs());*/
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
                    Game.SelectableCards.Add(card);
                }
            }
            foreach (Card card in Game.Fields[Game.CurrentPlayer])
            {
                if (CanAttack(card,Game))
                {
                    Game.SelectableCards.Add(card);
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
        public void DrawCard(int TargetPlayer, GameRepresentation Game)
        {
            //handle card drawing listeners
            if (Game.Decks[TargetPlayer].Count >= 1)
            {

                Game.Hands[TargetPlayer].Add(Game.Decks[TargetPlayer][0]);
                Game.Decks[TargetPlayer].RemoveAt(0);
          /*      if (ManaChanged != null)
                    ManaChanged(this, new EventArgs());*/
            }
            else
            {
                DealDamage(Game.Players[Game.CurrentPlayer], Game.Players[Game.CurrentPlayer].fatigue++, Game);
                
                DebugText("player " + Game.Players[TargetPlayer] + " Cannot draw cards and suffered "+Game.Players[Game.CurrentPlayer].fatigue+" fatigue damage");
            }
        }
        private void IncreaseMana(int TargetPlayer, GameRepresentation Game)
        {
            Game.Manapool[TargetPlayer].Turn();
            /*
            if (ManaChanged != null)
                ManaChanged(this, new EventArgs());*/
        }
        public void ResetMonsterAttacks(GameRepresentation Game)
        {
            for (int i = 0; i < 2; i++)
            {

                foreach (Card Monster in Game.Fields[i])
                {
                    Monster.AttackedXTimes = 0;
                }
            }
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
            //increase turn counter
            Game.TurnsTotal++;
            IncreaseMonsterTurns(Game);
            ResetMonsterAttacks(Game);
            //handle start turn listeners
            //TODO

            //increase mana
            IncreaseMana(Game.CurrentPlayer, Game);
            //draw card
            DrawCard(Game.CurrentPlayer, Game);
            GetSelectableCards(Game);
          /*  if (Game.isThisPlayerAi[Game.CurrentPlayer])
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

            }*/
        }
        public void EndTurn(GameRepresentation Game)
        {
            //Game.CurrentPlayer = Game.CurrentPlayer + 1 - Game.CurrentPlayer*2;
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
                if (!CheckTagForValidity(tag, Target, Origin, Game))
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
        public void DeactivateAura(Card Origin, Abbility AuraDeactivated, GameRepresentation Game)
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
            }
        }
        public void KillMonster(Card SelectedMonster,GameRepresentation Game)
        {
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
        }
        public void RemoveCardFromPlay(Card SelectedCard,GameRepresentation Game)
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
        public void DealDamage(Card Target, int ammount, GameRepresentation Game)
        {
            Target.currenthitpoints -= ammount;
            if (Target.currenthitpoints <= 0)
            {
                KillMonster(Target, Game);
            }
            
        }
        public void DealDamage(List<Card> Target, int ammount,GameRepresentation Game)
        {
            foreach (Card target in Target)
            {
                target.currenthitpoints -= ammount;
                if (target.currenthitpoints <= 0)
                {
                    KillMonster(target,Game);
                }
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
            DealDamage(target, attackerdmg,Game);
            DealDamage(attacker, targetdmg,Game);
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
