using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace QPP.Wpf.Controls
{
    public class SeparatorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (object.Equals(item, "-"))
            {
                return SeparatorTemplate;
            }
            return ItemTemplate;
        }
    }
}
