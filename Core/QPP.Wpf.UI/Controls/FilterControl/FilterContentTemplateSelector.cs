using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace QPP.Wpf.UI.Controls.FilterControl
{
    public class FilterContentTemplateSelector: DataTemplateSelector
    {
        public FrameworkElement Element { get; set; }

        public List<FilterDataTemplate> DataTemplateCollection { get; set; }

        public FilterContentTemplateSelector()
        {
            DataTemplateCollection = new List<FilterDataTemplate>();
        }         

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(DataTemplateCollection!=null)
            {
                var node = item as FilterNode;
                if (node.FilterDataTemplate != null)
                {
                    return node.FilterDataTemplate;
                }
                else if (node.Type == TypeCode.String)
                {
                    if (node.Action == ActionType.Between || node.Action == ActionType.NotBetween)
                        return DataTemplateCollection.FirstOrDefault(p => (p.TypeName == "StringRangePanel"));
                    else
                        return DataTemplateCollection.FirstOrDefault(p => p.TypeName == "TextBox");
                }
                else if (node.Type == TypeCode.DateTime
                    || node.Type == TypeCode.Int16
                    || node.Type == TypeCode.Int32
                    || node.Type == TypeCode.Int64
                    || node.Type == TypeCode.Single
                    || node.Type == TypeCode.Double
                    || node.Type == TypeCode.Decimal)
                {
                    if (node.Action == ActionType.Between || node.Action == ActionType.NotBetween)
                        return DataTemplateCollection.FirstOrDefault(p => (p.TypeName == "NumericUpDownRangePanel"));
                    else
                        return DataTemplateCollection.FirstOrDefault(p => p.TypeName == "NumericUpDown");
                }
                else if (node.Type == TypeCode.DateTime)
                {
                    if (node.Action == ActionType.Between || node.Action == ActionType.NotBetween)
                        return DataTemplateCollection.FirstOrDefault(p => (p.TypeName == "DatePickerRangePanel"));
                    else
                        return DataTemplateCollection.FirstOrDefault(p => p.TypeName == "DatePicker");
                }
            }
            return null;
        }
    }
}
