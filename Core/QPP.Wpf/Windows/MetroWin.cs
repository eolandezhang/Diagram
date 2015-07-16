using QPP.Layout;
using QPP.Modularity;
using QPP.Wpf.UI.Controls.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Windows
{
    public class MetroWin : MetroWindow, IWindow
    {
        public IPresenter Presenter
        {
            get { return DataContext as IPresenter; }
            set { DataContext = value; }
        }

        public string ContentKey { get; set; }

        IWindow IWindow.Owner
        {
            get { return Owner as IWindow; }
            set { Owner = value as System.Windows.Window; }
        }
    }
}