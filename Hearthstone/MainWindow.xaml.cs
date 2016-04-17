using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hearthstone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List<Stav> StavyHry;
        Stav StavAutomatu;
        
        public MainWindow()
        {
            InitializeComponent();
            StavAutomatu = new ZakladniStav();
            StavAutomatu.Inicializuj();
            mujlabel.Content = StavAutomatu.ToString();
            foreach (Stav moznestavy in StavAutomatu.CiloveStavy)
            {
                mujlist.Items.Add(moznestavy);
            }
        }
        public abstract class Udalost
        {

        }
        public class HracHrajeKartuUdalost : Udalost
        {
            //<- HracMaVybranouKartuStav
            //-> HracZahralKartuStav (pokud zadna z udalosti nezabrani zahrani karty)
            //-> HracJeNaTahuStav (pokud se eventy zmeni a hrac kartu nezahraje - napr counterspell)
            //triggerne vsechny efekty reagujici na tuto udalost (posileni monster, lizani karet, apod.  - efekty mohou invoknout dalsi udalosti
        }
        public class HracZahralKartuUdalost : Udalost
        {
            //<- HracZahralKartuStav (okamzite vytvori tuto udalost pote co se provede efekt karty (volani udalosti))
            //-> HracJeNaTahuStav (po resolvu)
        }
        public class ObnovaZivotuUdalost : Udalost
        {
            //udalost co rika ze se obnovuji zivoty
        }
        public class DamageDoneUdalost : Udalost
        {
            //udalost co rika ze doslo k udeleni zraneni
        }
        public class MonstrumUtociUdalost : Udalost
        {
            //udalost pustena v okamziku kdy hrac vybere cil utoku
        }
        public class KonecKolaUdalost : Udalost
        {

        }
        public abstract class Stav
        {
            public List<Stav> CiloveStavy;
            public Stav()
            {
                CiloveStavy = new List<Stav>();
            }
            public abstract void Inicializuj();
        }
        public class ZakladniStav : Stav
        {

            public override void Inicializuj()
            {
                //base.CiloveStavy = new List<Stav>();
                CiloveStavy.Add(new HracMaVybranouKartuNaRuce());
                CiloveStavy.Add(new HracMaVybraneMonstrumStav());
                CiloveStavy.Add(new KonecKola());
            }

        }
        public class HracMaVybraneMonstrumStav : Stav
        {
            public override void Inicializuj()
            {
 	        
                //base.CiloveStavy = new List<Stav>();
                CiloveStavy.Add(new HracVybralCilProMonstrum());
                CiloveStavy.Add(new ZakladniStav());
            }
        }

        public class HracMaVybranouKartuNaRuce : Stav
        {
            public override void Inicializuj()
            {
                CiloveStavy.Add(new HracHrajeKartuZRuky());
                CiloveStavy.Add(new ZakladniStav());
            } 
        }
        public class HracVybiraKartu : Stav
        {
            public override void Inicializuj()
            {
                CiloveStavy.Add(new ZakladniStav());
            } 
        }

        public class HracHrajeKartuZRuky : Stav
        {
            public override void Inicializuj()
            {
                CiloveStavy.Add(new HracZahralKartuZRuky());
                CiloveStavy.Add(new ZakladniStav());
            } 
        }

        public class HracZahralKartuZRuky : Stav
        {
            //leads to resolve card effects (for spells) -> hracjenatahu
            public override void Inicializuj()
            {
                CiloveStavy.Add(new ZakladniStav());
            } 
        }
        public class HracVybralCilProMonstrum : Stav
        {
            public override void Inicializuj()
            {
                CiloveStavy.Add(new MonstrumXUtociDoCileY());
            } 
        }
        public class MonstrumXUtociDoCileY : Stav
        {
            //bude resolvovano jestli se to povedlo ci co vsechno to triggernulo pres eventy

            public override void Inicializuj()
            {
                CiloveStavy.Add(new MonstrumXZautociloDoCileY());
            } 
        }
        public class MonstrumXZautociloDoCileY : Stav
        {
            //monstrum uspesne provedlo utok, zbyva resolvnout damage a pripadne dalsi efekty
            public override void Inicializuj()
            {
                CiloveStavy.Add(new ProbehlDamageVypocetSouboje());
            } 
        }
        public class ProbehlDamageVypocetSouboje : Stav
        {
            //vse se resolvnulo -> monstra umrou a prejde se do "hracjenatahu"
            public override void Inicializuj()
            {
                CiloveStavy.Add(new ZakladniStav());
            } 
        }
        public class KonecKola : Stav
        {
            public override void Inicializuj()
            {
                CiloveStavy.Add(new ZakladniStav());
            }
        }
        public void ZmenStav(Stav NovyStav, Stav SoucasnyStav)
        {
            StavAutomatu = SoucasnyStav;
            SoucasnyStav.Inicializuj();
            mujlabel.Content = SoucasnyStav.ToString();
            mujlist.Items.Clear();
            foreach (Stav moznestavy in StavAutomatu.CiloveStavy)
            {
                mujlist.Items.Add(moznestavy);
            }

        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //zmeni stav na vybrany
            if (mujlist.SelectedItem != null)
            {                
                ZmenStav(StavAutomatu, (Stav)mujlist.SelectedItem);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SimulationWindow sw = new SimulationWindow();
            sw.Show();
            this.Close();


        }
        
    }
}
