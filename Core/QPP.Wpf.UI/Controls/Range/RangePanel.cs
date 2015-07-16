using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using QPP.Wpf.UI.Models;
using QPP.ComponentModel;

namespace QPP.Wpf.UI.Controls.Range
{
    public class RangePanel : Control
    {
        public static readonly DependencyProperty FromProperty;

        public static readonly DependencyProperty ToProperty;

        public static readonly DependencyProperty ValueProperty;

        static RangePanel()
        {
            var thisType = typeof(RangePanel);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
            FromProperty = DependencyProperty.Register("From", typeof(object), thisType);
            ToProperty = DependencyProperty.Register("To", typeof(object), thisType);
            ValueProperty = DependencyProperty.Register("Value", typeof(Scope), thisType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, ValueCoerceCallback));
        }

        private static object ValueCoerceCallback(DependencyObject d, object baseValue)
        {
            var dp = d as RangePanel;
            Scope bv = baseValue as Scope;
            if (bv == null)
            {
                var s = new Scope();
                dp.Value = s;
                return s;
            }
            return baseValue;
        }

        public object From
        {
            get { return (object)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public object To
        {
            get { return (object)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public Scope Value
        {
            get { return (Scope)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
