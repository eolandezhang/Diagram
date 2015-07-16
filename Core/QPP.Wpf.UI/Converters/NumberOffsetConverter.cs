using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QPP.Wpf.UI.Converters
{
    // this converter is only used by DatePicker to convert the font size to width and height of the icon button
    public class NumberOffsetConverter : IValueConverter
    {
        private static NumberOffsetConverter _instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static NumberOffsetConverter()
        {
        }

        private NumberOffsetConverter()
        {
        }

        public static NumberOffsetConverter Instance
        {
            get { return _instance ?? (_instance = new NumberOffsetConverter()); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && parameter is double) {
                var offset = (double)parameter;
                var orgValue = (double)value;
                return Math.Round(orgValue + offset);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}