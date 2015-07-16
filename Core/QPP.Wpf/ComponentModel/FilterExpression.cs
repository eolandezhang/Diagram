using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace QPP.Wpf.ComponentModel
{
    [ContentProperty("Expression")]
    public class FilterExpression : FrameworkElement
    {
        public static readonly DependencyProperty ExpressionProperty =
            DependencyProperty.Register("Expression", typeof(ObservableCollection<FieldExpression>), typeof(FilterExpression),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty FilterCriteriaProperty =
            DependencyProperty.Register("FilterCriteria", typeof(FilterCriteria), typeof(FilterExpression),
            new FrameworkPropertyMetadata());

        static FilterExpression()
        {
            VisibilityProperty.OverrideMetadata(typeof(FilterExpression), new PropertyMetadata(Visibility.Collapsed));
        }

        public FilterExpression()
        {
            Expression = new ObservableCollection<FieldExpression>();
            Loaded += FilterExpression_Loaded;
        }

        void FilterExpression_Loaded(object sender, RoutedEventArgs e)
        {
            if (FilterCriteria != null)
                FilterCriteria.Expressions.AddRange(Expression);
        }

        public ObservableCollection<FieldExpression> Expression
        {
            get { return (ObservableCollection<FieldExpression>)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }
        public FilterCriteria FilterCriteria
        {
            get { return (FilterCriteria)GetValue(FilterCriteriaProperty); }
            set { SetValue(FilterCriteriaProperty, value); }
        }
    }
}
