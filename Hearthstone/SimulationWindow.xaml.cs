using GameIntestines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Hearthstone
{

    /// <summary>
    /// Interaction logic for SimulationWindow.xaml
    /// </summary>
    public partial class SimulationWindow : Window, INotifyPropertyChanged
    {
        List<Card> AllCards = new List<Card>();
        List<Card> Deck1 = new List<Card>();
        List<Card> Deck2 = new List<Card>();
        List<Card> Hand1 = new List<Card>();
        List<Card> Hand2 = new List<Card>();
        Mana ManaP1 = new Mana();
        Mana ManaP2 = new Mana();
        // GameEngine ge;
        public class Mana
        {
            int availible;
            int total;
            public Mana()
            {
                this.availible = 0;
                this.total = 0;
            }
            public override string ToString()
            {
                return availible + " / " + total;
                //return base.ToString();
            }
            public void Turn()
            {
                if (this.total < 10)
                {
                    this.total++;
                }
                this.availible = this.total;

            }
        }
        public bool isPlayer0Turn
        {
            get { return isP0Turn; }
            set { isP0Turn = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(isPlayer0Turn))); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(isPlayer1Turn))); }
        }
        public bool isPlayer1Turn
        {
            get { return !isPlayer0Turn; }
        }
        bool isTargetingPhase = false;
        bool isP0Turn;

        //TODO:
        /*
         * switch turns so I write only one code
         * 
        */
        public class CardType
        {
            public enum typ { SPELL, WEAPON, CHARACTER };
            public enum subtyp { NONE, BEAST, PLAYER, MURLOC };

        }

        /*
        public class Card
        {
            
            public string name; //jméno karty a zároveň identifikátor
            public int baseattack;
            public int basehitpoints;
            public int currentattack;
            public int currenthitpoints;
            public int armor;
            public CardType typkarty;
            public bool divineshield;
            public bool invulnerable;
            public enum Target { SINGLE, NONE };
            public Target target;
            public Card(int i)
            {
                this.name = "dummy " + i;
            }
            public string getAllText()
            {
                string outstr = "";
                outstr = outstr + "name: " + name + "\n" + "attack: " + baseattack;
                return outstr;

            }

        }

        */
        public void checkforcardstodisplay()
        {
            bool curpl = isPlayer0Turn;
            while (curpl == isPlayer0Turn)
            {

                this.Dispatcher.Invoke((Action)(() =>
                    {

                        if (P1Hand.SelectedItem != null)
                        {
                            MyCard.Content = Hand1[P1Hand.SelectedIndex].getAllText();
                            if (Hand1[0].target == Card.Target.NONE)
                            {

                            }
                        }
                    }));
                /*
                Card selected = (Card)P1Hand.SelectedItem;
                if (selected != null)
                {
                    MyCard.Content = selected.name;
                }*/
            }
        }
        public void update(bool player)
        {

            if (player)
            {
                if (Deck1.Count != 0)
                {
                    this.P1Hand.Items.Add(Deck1[0].name);
                    Hand1.Add(Deck1[0]);
                    Deck1.RemoveAt(0);
                }
                else
                {
                    //fatigue
                }
                //ManaP1.Turn();
                //P1Mana.Content = ManaP1;
                VM.dostuff();
            }
            else
            {
                if (Deck2.Count != 0)
                {
                    this.P0Hand.Items.Add(Deck2[0].name);
                    Hand2.Add(Deck2[0]);
                    Deck2.RemoveAt(0);
                }
                else
                {
                    //fatigue
                }
                //ManaP2.Turn();
                // P2Mana.Content = ManaP2;
                VM.dostuff();
            }

        }

        ViewModel VM;

        public event PropertyChangedEventHandler PropertyChanged;

        public SimulationWindow()
        {
            isPlayer0Turn = true;
            InitializeComponent();
            VM = ViewModel.Instance;
            DataContext = VM;
            /*
            
            Random rng = new Random(1);
            for (int i = 0; i < 30; i++)
            {
                //make two decks
                AllCards.Add(new Card(i));
            }
            for (int i = 0; i < 15; i++)
            {
                int ind = rng.Next(29-2*i);
                Deck1.Add(AllCards[ind]);
                AllCards.RemoveAt(ind);
                ind = rng.Next(29 - 2*i - 1);
                Deck2.Add(AllCards[ind]);
                AllCards.RemoveAt(ind);
            }
            for (int i = 0; i < 3; i++)
            {
                this.P1Hand.Items.Add(Deck1[0].name);
                Hand1.Add(Deck1[0]);
                Deck1.RemoveAt(0);
                this.P0Hand.Items.Add(Deck2[0].name);
                Hand2.Add(Deck2[0]);
                Deck2.RemoveAt(0);
            }
            
            */

        }


        //QuitButton
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        //EndTurnButton
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            VM.IncreaseManaProperTest();
            if (!VM.IsPlayer1AI)
            {
                isPlayer0Turn = !isPlayer0Turn;

            }

            SelectableCards.SelectedIndex = -1;
            P0Field.SelectedItem = null;
            P1Field.SelectedItem = null;
            P0Hand.SelectedItem = null;
            P1Hand.SelectedItem = null;
            Confirm.IsEnabled = false;

            ResetSelectableFields();

            //update(playerturn);
            //playerturn = !playerturn;
            //Task gui = new Task(this.checkforcardstodisplay);
            //  gui.Start();
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            VM.dostuff();
        }

        private void P0Hand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Card c = ((sender as ListBox).SelectedItem as Card);
            VM.SelectCardFromHand(((sender as ListBox).SelectedItem as Card));
            //VM.SelectCardFromHand((Card)(P0Hand.SelectedItem as Card));
        }

        private void PHand_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (sender is ListBox)
            //  (sender as ListBox).SelectedIndex = -1;


        }

        /*    private void P1Hand_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
               // VM.SelectCardFromHand(P1Hand.SelectedIndex);
            }*/

        private void ResetSelectableFields()
        {
            var source = P0Hand.ItemsSource;
            P0Hand.ItemsSource = null;
            P0Hand.ItemsSource = source;

            source = P1Hand.ItemsSource;
            P1Hand.ItemsSource = null;
            P1Hand.ItemsSource = source;

            source = P0Field.ItemsSource;
            P0Field.ItemsSource = null;
            P0Field.ItemsSource = source;

            source = P1Field.ItemsSource;
            P1Field.ItemsSource = null;
            P1Field.ItemsSource = source;

            source = Player0Avatar.ItemsSource;
            Player0Avatar.ItemsSource = null;
            Player0Avatar.ItemsSource = source;

            source = Player1Avatar.ItemsSource;
            Player1Avatar.ItemsSource = null;
            Player1Avatar.ItemsSource = source;
        }

        //Confirm Button
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (isPlayer0Turn)
            {
                if (SelectableCards.SelectedIndex != -1)// && ValidTargets.Items.Count == 0)
                {
                    VM.PlaySelectedMonster(SelectableCards.SelectedItem as Card);
                }
                if (P0Hand.SelectedIndex != -1)
                {
                    //  P0Field.Items.Add(P0Hand.SelectedItem);
                    //  P0Hand.Items.RemoveAt(P0Hand.SelectedIndex);

                    VM.PlaySelectedMonster((P0Hand.SelectedItem as Card));
                }

            }
            else
            {
                if (SelectableCards.SelectedIndex != -1)//&& ValidTargets.Items.Count == 0)
                {
                    VM.PlaySelectedMonster(SelectableCards.SelectedItem as Card);
                }
                if (P1Hand.SelectedIndex != -1)
                {
                    //  P0Field.Items.Add(P0Hand.SelectedItem);
                    //  P0Hand.Items.RemoveAt(P0Hand.SelectedIndex);

                    VM.PlaySelectedMonster((P1Hand.SelectedItem as Card));

                }
            }
            SelectableCards.SelectedIndex = -1;
            //ValidTargets.SelectedIndex = -1;

            ResetSelectableFields();
            Confirm.IsEnabled = false;
        }

        private void SelectableCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            P0Field.SelectedItem = null;
            P1Field.SelectedItem = null;
            P0Hand.SelectedItem = null;
            P1Hand.SelectedItem = null;*/
            if ((sender as ListBox).SelectedItem == null)
            {
                return;
            }

            if (isTargetingPhase)
            {
                return;
            }

            VM.SelectCardFromHand(((sender as ListBox).SelectedItem as Card));

            if (ValidTargets.HasItems)
            {
                Confirm.IsEnabled = false;
            }
            else
            {
                Confirm.IsEnabled = true;
            }
            if (isPlayer0Turn)
            {
                if (P0Hand.Items.Contains((sender as ListBox).SelectedItem as Card))
                {
                    P0Hand.SelectedItem = (sender as ListBox).SelectedItem as Card;
                }
                else
                {
                    P0Hand.SelectedIndex = -1;
                }

                if (P0Field.Items.Contains((sender as ListBox).SelectedItem as Card))
                {
                    P0Field.SelectedItem = (sender as ListBox).SelectedItem as Card;
                }
                else
                {
                    P0Field.SelectedIndex = -1;
                }
                if (Player0Avatar.Items.Contains((sender as ListBox).SelectedItem as Card))
                {
                    Player0Avatar.SelectedItem = (sender as ListBox).SelectedItem as Card;
                }
                else
                {
                    Player0Avatar.SelectedIndex = -1;
                }
            }
            else
            {
                if (P1Hand.Items.Contains((sender as ListBox).SelectedItem as Card))
                {
                    P1Hand.SelectedItem = (sender as ListBox).SelectedItem as Card;
                }
                else
                {
                    P1Hand.SelectedIndex = -1;
                }

                if (P1Field.Items.Contains((sender as ListBox).SelectedItem as Card))
                {
                    P1Field.SelectedItem = (sender as ListBox).SelectedItem as Card;
                }
                else
                {
                    P1Field.SelectedIndex = -1;
                }

                if (Player1Avatar.Items.Contains((sender as ListBox).SelectedItem as Card))
                {
                    Player1Avatar.SelectedItem = (sender as ListBox).SelectedItem as Card;
                }
                else
                {
                    Player1Avatar.SelectedIndex = -1;
                }
            }
            SelectableCards.SelectedItem = (sender as ListBox).SelectedItem as Card;

        }

        private void ValidTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isTargetingPhase = true;

            if (Player0Avatar.Items.Contains((sender as ListBox).SelectedItem as Card))
            {
                Player0Avatar.SelectedItem = (sender as ListBox).SelectedItem as Card;
            }
            else
            {
                Player0Avatar.SelectedIndex = -1;
            }

            if (Player1Avatar.Items.Contains((sender as ListBox).SelectedItem as Card))
            {
                Player1Avatar.SelectedItem = (sender as ListBox).SelectedItem as Card;
            }
            else
            {
                Player1Avatar.SelectedIndex = -1;
            }

            if (P0Field.Items.Contains((sender as ListBox).SelectedItem as Card))
            {
                P0Field.SelectedItem = (sender as ListBox).SelectedItem as Card;
            }
            else
            {
                P0Field.SelectedIndex = -1;
            }

            if (P1Field.Items.Contains((sender as ListBox).SelectedItem as Card))
            {
                P1Field.SelectedItem = (sender as ListBox).SelectedItem as Card;
            }
            else
            {
                P1Field.SelectedIndex = -1;
            }

            VM.SelectSecondaryTarget(ValidTargets.SelectedItem as Card);
            Confirm.IsEnabled = true;
            isTargetingPhase = false;
        }


    }
}
