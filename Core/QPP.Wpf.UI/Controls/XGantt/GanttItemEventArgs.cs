using System;

namespace QPP.Wpf.UI.Controls.XGantt
{
	public class GanttItemEventArgs : EventArgs
	{
		public GanttItem Item { get; set; }
		public GanttItemEventArgs(GanttItem item)
		{
			Item = item;
		}
	}
}
