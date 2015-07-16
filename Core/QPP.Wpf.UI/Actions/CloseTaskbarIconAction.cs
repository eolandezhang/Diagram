using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows;
using QPP.Wpf.UI.Controls.ToolbarIcon;

namespace QPP.Wpf.UI.Actions
{
    public class CloseTaskbarIconAction : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty TaskbarIconProperty 
            = DependencyProperty.Register("TaskbarIcon", typeof(TaskbarIcon), typeof(CloseTaskbarIconAction));

        public TaskbarIcon TaskbarIcon
        {
            get { return (TaskbarIcon)GetValue(TaskbarIconProperty); }
            set { SetValue(TaskbarIconProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            if (TaskbarIcon != null)
                TaskbarIcon.Dispose();
        }
    }
}
