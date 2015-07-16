using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace QPP.Wpf.UI.Controls.FilterControl
{
    public class FilterColumn : DependencyObject
    {
        public static readonly DependencyProperty CaptionProperty;
        static FilterColumn()
        {
            CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(FilterColumn));
        }

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public virtual string FieldName { get; set; }

        private TypeCode columnType = TypeCode.String;

        public virtual TypeCode ColumnType
        {
            get { return columnType; }
            set { columnType = value; }
        }

        public FilterDataTemplate FilterDataTemplate { get; set; }
    }
}
