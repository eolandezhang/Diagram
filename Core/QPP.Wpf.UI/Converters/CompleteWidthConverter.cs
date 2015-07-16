using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace QPP.Wpf.UI.Converters
{
    public class PercentUnitConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var d = Math.Min(100, System.Convert.ToDouble(value));
            return new GridLength(d / (100.1-d), GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PercentUnitConverterExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PercentUnitConverter();
        }
    }
}
