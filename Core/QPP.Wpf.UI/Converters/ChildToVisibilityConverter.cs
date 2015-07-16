using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using QPP.Wpf.UI.Models;
using System.Windows;

namespace QPP.Wpf.UI.Converters
{
    public class ChildToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true.Equals(value) ? Visibility.Visible : Visibility.Hidden;
            //var node = value as IHierarchicalData;
            //if (node != null)
            //    return node.HasChild ? Visibility.Visible : Visibility.Hidden;
            //return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
