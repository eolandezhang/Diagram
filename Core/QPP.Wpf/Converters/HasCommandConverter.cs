using QPP.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace QPP.Wpf.Converters
{
    public class HasCommandConverter : IValueConverter
    {
        public string PropertyName { get; set; }

        public HasCommandConverter() { }

        public HasCommandConverter(string propertyName)
        {
            PropertyName = propertyName;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var p = value.GetType().GetProperty(PropertyName);
            if (p != null && p.PropertyType == typeof(ICommand))
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
