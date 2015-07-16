using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using QPP.Wpf.UI.Converters;
using System.Windows.Media;
using System.Windows.Input;
using QPP.Wpf.UI.Models;

namespace QPP.Wpf.UI.Controls
{
    public class DataGridExpanderColumn : DataGridTextColumn
    {
        static DataGridExpanderColumn()
        {
            CanUserSortProperty.OverrideMetadata(typeof(DataGridExpanderColumn), new FrameworkPropertyMetadata(null, (s, e) => { return false; }));
        }
        IValueConverter _IndentConverter = new LevelToWidthConverter();
        public IValueConverter IndentConverter
        {
            get { return _IndentConverter; }
            set { _IndentConverter = value; }
        }
        string _LevelPath = "Level";
        public string LevelPath
        {
            get { return _LevelPath; }
            set { _LevelPath = value; }
        }
        string _IsExpandedPath = "Expanded";
        public string IsExpandedPath
        {
            get { return _IsExpandedPath; }
            set { _IsExpandedPath = value; }
        }
        string _HasChildPath = "HasChild";
        public string HasChildPath
        {
            get { return _HasChildPath; }
            set { _HasChildPath = value; }
        }
        IValueConverter _ExpanderVisibilityConverter = new ChildToVisibilityConverter();
        public IValueConverter ExpanderVisibilityConverter
        {
            get { return _ExpanderVisibilityConverter; }
            set { _ExpanderVisibilityConverter = value; }
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            StackPanel panel = GeneratePanel(dataItem);
            panel.Children.Add(base.GenerateElement(cell, dataItem));
            return panel;
        }

        Border GenerateIndent()
        {
            var bind = new Binding(LevelPath);
            bind.Mode = BindingMode.OneWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind.Converter = IndentConverter;

            var b = new Border();
            b.BorderThickness = new Thickness(0);
            b.Background = new SolidColorBrush(Colors.Transparent);
            b.SetBinding(Border.WidthProperty, bind);
            return b;
        }

        ToggleExpander GenerateExpander()
        {
            var expander = new ToggleExpander();
            expander.SetBinding(ToggleExpander.IsExpandedProperty, new Binding(IsExpandedPath) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            expander.SetBinding(ToggleExpander.VisibilityProperty, new Binding(HasChildPath) { Converter = ExpanderVisibilityConverter });
            expander.VerticalAlignment = VerticalAlignment.Center;
            return expander;
        }

        StackPanel GeneratePanel(object dataItem)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.Children.Add(GenerateIndent());
            panel.Children.Add(GenerateExpander());
            return panel;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            //return base.GenerateEditingElement(cell, dataItem);
            var bind = new Binding(LevelPath);
            bind.Converter = IndentConverter;
            var be = new BindingEvaluator(bind);
            be.DataContext = dataItem;
            var left = (double)be.Value + 16;
            var element = base.GenerateEditingElement(cell, dataItem);
            element.Margin = new Thickness(element.Margin.Left + left, element.Margin.Top, element.Margin.Right, element.Margin.Bottom);
            return element;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            TextBox box = editingElement as TextBox;
            if (box == null)
                return string.Empty;
            string text = box.Text;
            int length = text.Length;
            KeyEventArgs args = editingEventArgs as KeyEventArgs;
            if ((args != null) && (args.Key == Key.F2))
            {
                box.Select(length, length);
                return text;
            }
            box.Select(0, length);

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }
    }
}

