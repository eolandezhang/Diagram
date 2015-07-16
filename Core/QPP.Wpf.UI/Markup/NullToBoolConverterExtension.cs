using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;

namespace QPP.Wpf.UI.Markup
{
    public class NullToBoolConverterExtension : MarkupExtension
    {
        public bool NullValue
        {
            get;
            set;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new NullToBoolConverter(NullValue);
        }
    }

    public class NullToBoolConverter : IValueConverter
    {
        public NullToBoolConverter(bool nullValue)
        {
            NullValue = nullValue;
        }

        public bool NullValue
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return NullValue;
            return !NullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
