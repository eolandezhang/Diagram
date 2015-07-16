using QPP.Wpf.UI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace QPP.Wpf.UI.Behaviours
{
    public class WindowBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty CloseActionProperty;
        public static readonly DependencyProperty ActivateActionProperty;

        static WindowBehavior()
        {
            CloseActionProperty = DependencyProperty.Register("CloseAction", typeof(Action), typeof(WindowBehavior), new FrameworkPropertyMetadata(null));
            ActivateActionProperty = DependencyProperty.Register("ActivateAction", typeof(Action), typeof(WindowBehavior), new FrameworkPropertyMetadata(null));
        }

        public Action CloseAction
        {
            get { return (Action)GetValue(CloseActionProperty); }
            set { SetValue(CloseActionProperty, value); }
        }

        public Action ActivateAction
        {
            get { return (Action)GetValue(ActivateActionProperty); }
            set { SetValue(ActivateActionProperty, value); }
        }

        protected override void OnAttached()
        {
            CloseAction = () =>
            {
                AssociatedObject.Close();
            };
            ActivateAction = () =>
            {
                AssociatedObject.Activate();
            };
            base.OnAttached();
        }
    }
}
