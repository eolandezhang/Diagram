using System;
using System.Windows.Data;
using QPP.Wpf.UI.Models;

namespace QPP.Wpf.UI.Converters
{
    /// <summary>
    /// This class converts a node's Level property into an indention width for the gantt chart.
    /// </summary>
    public class LevelToWidthConverter : IValueConverter
    {
        public const double LevelIndention = 15d;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double width = 0d;

            int Level = (int)value;
            Level--;
            width += Level * LevelIndention;

            //var node = value as IHierarchicalData;
            //if (node != null)
            //{
            //    int Level = node.Level;
            //    Level--;
            //    width += Level * LevelIndention;
            //}

            return width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
