using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QPP.Wpf.UI.Controls
{
    [TemplatePart(Name = "CollapseAnimation", Type = typeof(DoubleAnimation)),
    TemplatePart(Name = "ExpandAnimation", Type = typeof(DoubleAnimation))]
    public class ToggleExpander : Control
    {
        protected DoubleAnimation CollapseAnimation { get; private set; }
        protected DoubleAnimation ExpandAnimation { get; private set; }
        protected RotateTransform ExpanderRotate { get; private set; }

        public event EventHandler IsExpandedChanged;

        public static DependencyProperty IsExpandedProperty;

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        protected void OnIsExpandedChanged()
        {
            SetVisualState(false);
            if (IsExpandedChanged != null)
                IsExpandedChanged(this, EventArgs.Empty);
        }

        static ToggleExpander()
        {
            var thisType = typeof(ToggleExpander);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
            IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), thisType,
                new PropertyMetadata(false, (s, e) => { ((ToggleExpander)s).OnIsExpandedChanged(); }));
        }

        public ToggleExpander()
        {
            DataContextChanged += ToggleExpander_DataContextChanged;
        }

        void ToggleExpander_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //GetBindingExpression(ToggleExpander.VisibilityProperty).UpdateTarget();
            GetBindingExpression(ToggleExpander.IsExpandedProperty).UpdateTarget();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            CollapseAnimation = (DoubleAnimation)GetTemplateChild("CollapseAnimation");
            ExpandAnimation = (DoubleAnimation)GetTemplateChild("ExpandAnimation");
            ExpanderRotate = (RotateTransform)(GetTemplateChild("Triangle") as Polygon).RenderTransform;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (ExpanderRotate != null)
                ExpanderRotate.Angle = IsExpanded ? 90 : 0;
            base.OnRender(drawingContext);
        }

        private void SetVisualState(bool useTransitions)
        {
            if (IsExpanded)
                VisualStateManager.GoToState(this, "Expanded", useTransitions);
            else
                VisualStateManager.GoToState(this, "Collapsed", useTransitions);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            IsExpanded = !IsExpanded;
            Focus();
            e.Handled = true;
            base.OnMouseDown(e);
        }
    }
}
