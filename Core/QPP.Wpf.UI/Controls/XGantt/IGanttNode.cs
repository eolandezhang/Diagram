using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using QPP.Wpf.UI.Models;
using QPP.ComponentModel;

namespace QPP.Wpf.UI.Controls.XGantt
{
    public interface IGanttItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the date that the task starts.
        /// </summary>
        DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the date that the task ends.
        /// </summary>
        DateTime EndDate { get; set; }

        ObservableCollection<GanttDependency> Successors { get; }

        bool IsMilestone { get; set; }

        IGanttNode Node { get; set; }
    }
	/// <summary>
	/// The node interface for use in the GanttChart control.
	/// </summary>
    public interface IGanttNode : IHierarchicalData<IGanttNode>
    {
        ObservableCollection<IGanttItem> Items { get; }
		/// <summary>
		/// Gets the name of the task.
		/// </summary>
        //string TaskName { get; }

        /// <summary>
        /// Gets or sets the date that the task starts.
        /// </summary>
        //DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the date that the task ends.
        /// </summary>
        //DateTime EndDate { get; set; }

		/// <summary>
		/// Gets the resources needed for the task.
		/// </summary>
        //string Resources { get; set; }

		/// <summary>
		/// Gets the percentage of work that has been completed for the task.  
		/// Format: 30.2 == 30.2%.
		/// </summary>
        //double PercentComplete { get; }

        bool IsMilestone { get; set; }

        //ObservableCollection<GanttDependency> Successors { get; }

        //DateTime? ActualStartDate { get; set; }

        //DateTime? ActualEndDate { get; set; }
    }
}
