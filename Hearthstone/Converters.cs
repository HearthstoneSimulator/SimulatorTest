using GameIntestines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hearthstone
{
    class CardToDetailedStringConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Card)
            {
                return (value as Card).getAllText();
            }
            if (value == null)
            {
                return "";
            }
            throw new Exception("Bad_Imput_Format");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class ElementToSelectabilityConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Card && parameter is ListBox)
            {

                return (parameter as ListBox).Items.Contains(value as Card);
            }
            //throw new Exception("Bad_Imput_Format");
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
