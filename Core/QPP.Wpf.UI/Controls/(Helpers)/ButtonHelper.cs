using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QPP.Wpf.UI.Controls
{
    public class ButtonHelper : DependencyObject
    {
        public static readonly DependencyProperty ImageUrlProperty;
        public static readonly DependencyProperty LabelProperty;

        static ButtonHelper()
        {
            ImageUrlProperty = DependencyProperty.RegisterAttached("ImageUrl", typeof(string), typeof(ButtonHelper), new PropertyMetadata(null));
            LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(string), typeof(ButtonHelper), new PropertyMetadata(null));
        }

        public static void SetImageUrl(DependencyObject obj, string value)
        {
            obj.SetValue(ImageUrlProperty, value);
        }

        public static string GetImageUrl(DependencyObject obj)
        {
            return (string)obj.GetValue(ImageUrlProperty);
        }

        public static void SetLabel(DependencyObject obj, string value)
        {
            obj.SetValue(LabelProperty, value);
        }

        public static string GetLabel(DependencyObject obj)
        {
            return (string)obj.GetValue(LabelProperty);
        }
    }
}
