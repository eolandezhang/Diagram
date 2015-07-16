using System;
using System.Windows.Controls;

namespace QPP.Wpf.UI.Controls.XGantt
{
    public class RowExpandedChangedEventArgs : EventArgs
    {
        public IGanttNode Node { get; set; }
    }
}
