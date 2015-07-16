using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;

namespace QPP.Wpf.UI.Controls
{
    public class DataGridDateColumn : DataGridTextColumn
    {
        public static readonly DependencyProperty DateTimeFormatProperty
            = DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(DataGridDateColumn),
            new PropertyMetadata("yyyy-MM-dd HH:mm:ss"));

		public DataGridDateColumn()
		{

		}

        public string DateTimeFormat
        {
            get { return (string)GetValue(DateTimeFormatProperty); }
            set { SetValue(DateTimeFormatProperty, value); }
        }

		protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
		{
            DatePicker picker = new DatePicker();
            picker.MinHeight = 16;
            picker.Padding = new Thickness(0);
            picker.BorderThickness = new Thickness(0);
			picker.SetBinding(DatePicker.SelectedDateProperty, Binding);
			picker.IsDropDownOpen = true;
			return picker;
		}

        //protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        //{
        //    TextBlock block = new TextBlock
        //    {
        //        Margin = new Thickness(4.0),
        //        VerticalAlignment = VerticalAlignment.Center
        //    };
        //    block.SetBinding(TextBlock.TextProperty, this.Binding);
        //    return block;
        //}

		protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
		{
			DatePicker picker = editingElement as DatePicker;
			picker.IsDropDownOpen = true;

			return picker.SelectedDate;
		}
    }
}
