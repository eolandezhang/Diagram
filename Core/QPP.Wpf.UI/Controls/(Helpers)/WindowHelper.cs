using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QPP.Wpf.UI.Controls
{
    public class WindowHelper : DependencyObject
    {
        public static readonly DependencyProperty MainMenuToolTipProperty;

        static WindowHelper()
        {
            MainMenuToolTipProperty = DependencyProperty.RegisterAttached("MainMenuToolTip", typeof(string), typeof(WindowHelper), new FrameworkPropertyMetadata(null));
        }

        public static void SetMainMenuToolTip(DependencyObject obj, string value)
        {
            obj.SetValue(MainMenuToolTipProperty, value);
        }

        public static string GetMainMenuToolTip(DependencyObject obj)
        {
            return (string)obj.GetValue(MainMenuToolTipProperty);
        }
    }
}
