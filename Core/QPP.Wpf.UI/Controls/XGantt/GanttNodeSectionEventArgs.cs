using System;

namespace QPP.Wpf.UI.Controls.XGantt
{
    public class GanttNodeSectionEventArgs : EventArgs
    {
        public GanttNodeSection Section;

        public GanttNodeSectionEventArgs(GanttNodeSection section)
        {
            Section = section;
        }
    }
}
