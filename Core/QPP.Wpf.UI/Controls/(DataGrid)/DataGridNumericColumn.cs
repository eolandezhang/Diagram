using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using QPP.Wpf.UI.Controls.Toolkit;

namespace QPP.Wpf.UI.Controls
{
    public class DataGridNumericColumn : DataGridTextColumn
    {
        public static readonly DependencyProperty MaximumProperty
            = NumericUpDown<double?>.MaximumProperty.AddOwner(typeof(DataGridNumericColumn));

        public static readonly DependencyProperty MinimumProperty
            = NumericUpDown<double?>.MinimumProperty.AddOwner(typeof(DataGridNumericColumn));

        public static readonly DependencyProperty FormatStringProperty
            = DependencyProperty.Register("FormatString", typeof(string), typeof(DataGridNumericColumn),
            new PropertyMetadata(null));

        public string FormatString
        {
            get { return (string)GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }

        public double? Maximum
        {
            get { return (double?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double? Minimum
        {
            get { return (double?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        void ApplyColumnProperties(NumericUpDown numericUpDown)
        {
            Util.SyncColumnProperty(this, numericUpDown, NumericUpDown.MaximumProperty, MaximumProperty);
            Util.SyncColumnProperty(this, numericUpDown, NumericUpDown.MinimumProperty, MinimumProperty);
            Util.SyncColumnProperty(this, numericUpDown, NumericUpDown.FormatStringProperty, FormatStringProperty);
            Util.ApplyBinding(Binding, numericUpDown, NumericUpDown.ValueProperty);
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            NumericUpDown numericUpDown = new NumericUpDown();
            ApplyColumnProperties(numericUpDown);
            numericUpDown.VerticalAlignment = VerticalAlignment.Center;
            return numericUpDown;
        }
    }
}
