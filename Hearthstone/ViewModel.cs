using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GameIntestines;

namespace Hearthstone
{
    class ViewModel : INotifyPropertyChanged
    {

        public ReadOnlyObservableCollection<Card> SelectableCards
        {
            get { return test.SelectableCardsTest; }
        }
        public ReadOnlyObservableCollection<Card> ValidTargets
        {
            get { return test.ValidTargetsTest; }
        }
        public ReadOnlyObservableCollection<Card> P0Field
        {
            get { return test.P0FieldTest; }
        }
        public ReadOnlyObservableCollection<Card> P1Field
        {
            get { return test.P1FieldTest; }
        }
        public ReadOnlyObservableCollection<Card> P0Hand
        {
            get {return test.P0HandTest;}
        }
        public ReadOnlyObservableCollection<Card> P1Hand
        {
            get { return test.P1HandTest; }
        }
       
        public string P1ManaTest
        {
            get { return test.P1ManaTest; }
        }
        public string P2ManaTest
        {
            get { return test.P2ManaTest; }
        }
        public string P1Hitpoints
        {
            get { return test.P1Hitpoints; }
        }
        public string P2Hitpoints
        {
            get { return test.P2Hitpoints; }
        }
        public ReadOnlyObservableCollection<Card> OpponentAvatarTest
        {
            get { return test.OpponentAvatarTest; }
        }
        public ReadOnlyObservableCollection<Card> Player0Avatar
        {
            get { return test.Player0Avatar; }
        }
        public bool IsPlayer1AI
        {
            get { return test.IsPlayer1AI; }
        }
        static ViewModel VM;
        
        public static ViewModel Instance
        {
            get
            {
                if (VM == null)
                    VM = new ViewModel();

                return VM;
            }
        }

        ViewModel()
        {
            test = new GameEngine();

            test.LoadGame();
            //test.InitialiseGame();


          //  test.ManaChanged += Test_ManaChanged;
            test.ManaChanged += Test_AnyChange;
            //test.PlayGame();
           // Thread testThread = new Thread(test.StartOneGame);
           // testThread.Start();
        }

        void Test_ManaChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("P1Mana"));
        }
        void Test_AnyChange(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("P1ManaTest"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("P2ManaTest"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("P0HandTest"));
            } if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedCardTest"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("P1Hitpoints"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("P2Hitpoints"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ValidTargetsTest"));
            }
        }


        GameEngine test;
        public void dostuff()
        {
           // test.increasemana();
        }
        public void IncreaseManaProperTest()
        {
            //End Turn Button pressed
            // test.InitialiseTurn();
            //test.PlayGame();

            test.ProcessInputFromGUI(new EndTurnAction());
           // test.EndTurn();
            
            
            //test.PlayersAction = new EndTurnAction();
        }
        public void SelectCardFromHand(Card SelecctedCard)
        {
            test.ProcessInputFromGUI(new SelectCardAction(SelecctedCard));
            //test.SelectCardFromHand(SelecctedCard);
            
            
            
            //test.PlayersAction = new SelectCardAction(SelecctedCard);
        }
        public void PlaySelectedMonster(Card SelectedCard)
        {
            test.ProcessInputFromGUI(new PlayCardFromHandAction(SelectedCard,test.TargetForSomethingTest));
            //test.PlayMonsterFromHand(SelectedCard);
            
            
            //test.PlayersAction = new PlayCardFromHandAction(SelectedCard, null);
        }
        public void SelectSecondaryTarget(Card SelectedCard)
        {

            test.ProcessInputFromGUI(new SelectTargetAction(SelectedCard));
            //test.SelectSecondaryTarget(SelectedCard);
            
            
            //test.PlayersAction = new SelectTargetAction(SelectedCard);
        }
        public event PropertyChangedEventHandler PropertyChanged;


        
    }
}
