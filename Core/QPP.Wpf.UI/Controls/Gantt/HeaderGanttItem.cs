using System.Windows;

namespace QPP.Wpf.UI.Controls.Gantt
{
	/// <summary>
	/// This class is the control that is displayed for nodes that have children.
	/// </summary>
	public class HeaderGanttItem : GanttItem
	{
		#region Constructors and overrides

        static HeaderGanttItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderGanttItem), new FrameworkPropertyMetadata(typeof(HeaderGanttItem)));
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (ParentRow.ParentPanel.RowPresenter.Predecessor == this)
                Node.Expanded = !Node.Expanded;
        }

		#endregion
	}
}
