using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace QPP.Wpf.UI.Converters
{
    public class MaximumPageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int total = values[0].ConvertTo(0);
            int pageSize = values[1].ConvertTo(1);
            return Math.Ceiling((double)total / pageSize);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
