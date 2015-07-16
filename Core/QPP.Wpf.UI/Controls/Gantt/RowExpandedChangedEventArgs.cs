using System;
using System.Windows.Controls;

namespace QPP.Wpf.UI.Controls.Gantt
{
    public class RowExpandedChangedEventArgs : EventArgs
    {
        public IGanttNode Node { get; set; }
    }
}
