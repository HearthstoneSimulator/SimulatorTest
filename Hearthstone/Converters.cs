using GameIntestines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                return "xaxxaxaxa";
            }
            throw new Exception("Bad_Imput_Format");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
