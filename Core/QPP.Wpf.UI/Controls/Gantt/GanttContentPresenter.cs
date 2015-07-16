using System.Windows.Controls;
using System.Windows.Input;

namespace QPP.Wpf.UI.Controls.Gantt
{
    public class GanttContentPresenter : ContentPresenter
    {
        public GanttContentPresenter()
		{
			this.MouseLeftButtonDown += new MouseButtonEventHandler(GanttContentPresenter_MouseLeftButtonDown);
			this.MouseLeftButtonUp += new MouseButtonEventHandler(GanttContentPresenter_MouseLeftButtonUp);

			
		}
		
	

		void GanttContentPresenter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = false;
		}

	

		void GanttContentPresenter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = false;
		}
		
    }
}
