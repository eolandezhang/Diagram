using QPP.Dialog;
using QPP.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QPP.Wpf.Dialogs
{
    public class XDialog : IDialog
    {
        public void Show(DialogMessage message)
        {
            Window owner = null;
            var sender = message.Sender as DependencyObject;
            if (sender != null)
                owner = sender as Window ?? Window.GetWindow(sender);
            //else if (DockManager.Default != null)
            //    owner = DockManager.Default.Window;
            MessageBoxResult result = MessageBoxResult.None;
            if (owner != null)
            {
                result = MessageBox.Show(owner,
                   message.Content,
                   message.Caption,
                   (MessageBoxButton)message.Button,
                   (MessageBoxImage)message.Icon,
                   (MessageBoxResult)message.DefaultResult);
            }
            else
            {
                result = MessageBox.Show(
                   message.Content,
                   message.Caption,
                   (MessageBoxButton)message.Button,
                   (MessageBoxImage)message.Icon,
                   (MessageBoxResult)message.DefaultResult);
            }

            if (message.Callback != null)
                message.Callback((DialogResult)result);
        }
    }
}
