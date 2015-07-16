using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System;

namespace QPP.Wpf.UI.Controls.Gantt
{
	/// <summary>
	/// This class will arrange and measure it's child items for it's parent row.
	/// </summary>
    public class GanttItemsPresenter : Panel
	{
		#region Properties
		internal GanttRow ParentRow { get; set; }
        public GanttItemShadow ItemShadow { get; private set; }
		#endregion

		#region Constructors and overrides
		
        public GanttItemsPresenter()
        {
            this.UseLayoutRounding = false;
            ItemShadow = new GanttItemShadow();
            Children.Add(ItemShadow);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x1, x2;
            //x1 = ParentRow.ParentPanel.ConvertDateToPosition(ItemShadow.StartDate);
            //x2 = ParentRow.ParentPanel.ConvertDateToPosition(ItemShadow.EndDate.Date.AddDays(1));
            //x1 = ParentRow.ParentPanel.ConvertDateToPosition(ItemShadow.StartDate.Date);
            //x2 = ParentRow.ParentPanel.ConvertDateToPosition(ItemShadow.EndDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

            x1 = ParentRow.ParentPanel.ConvertDateToPosition(ItemShadow.StartDate);
            x2 = ParentRow.ParentPanel.ConvertDateToPosition(ItemShadow.EndDate);

            double width = Math.Max(5, x2 - x1);

            if (width > 0)
                ItemShadow.Arrange(new Rect(x1, 0, width, ParentRow.ActualHeight));
            else
                ItemShadow.Arrange(new Rect(0, 0, 0, ParentRow.ActualHeight));
            var items = Children.OfType<GanttItem>().ToList();
            foreach (var gi in items)
            {
                //var marginLeft = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.StartDate) -
                //    ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.StartDate.Date);
                //var marginRight = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.EndDate.Date.AddDays(1)) -
                //   ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.EndDate);
                //if (marginLeft < 0 || marginRight < 0)
                //{ }
                //gi.XMargin = new Thickness(marginLeft, 3, marginRight, 3);

                //x1 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.StartDate.Date);
                //x2 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.EndDate.Date.AddDays(1));
                x1 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.StartDate);
                x2 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.EndDate);
                width = Math.Max(5, x2 - x1);

                if (width > 0)
                    gi.Arrange(new Rect(x1, 0, width, ParentRow.ActualHeight));
                else
                    gi.Arrange(new Rect(0, 0, 0, ParentRow.ActualHeight));
            }
            var aitems = Children.OfType<GanttActualItem>().ToList();
            foreach (var gi in aitems)
            {
                if (gi.Node.ActualStartDate != null && gi.Node.ActualEndDate != null)
                {
                    
                    //x1 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.ActualStartDate.Value.Date);
                    //x2 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.ActualEndDate.Value.Date.AddDays(1));
                    x1 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.ActualStartDate.Value);
                    x2 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.ActualEndDate.Value);
                    width = Math.Max(5, x2 - x1);

                    if (width > 0)
                        gi.Arrange(new Rect(x1, 0, width, ParentRow.ActualHeight));
                    else
                        gi.Arrange(new Rect(0, 0, 0, ParentRow.ActualHeight));
                }
                
            }
            return base.ArrangeOverride(finalSize);
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            double x1, x2;
            x1 = ParentRow.ParentPanel.ConvertDateToPosition(ItemShadow.StartDate);
            x2 = ParentRow.ParentPanel.ConvertDateToPosition(ItemShadow.EndDate.AddDays(1));
            double width = Math.Max(5, x2 - x1);
            if (width < 0)
                width = 0;
            ItemShadow.Measure(new Size(width, ParentRow.ActualHeight));
            var items = Children.OfType<GanttItem>().ToList();
            foreach (var gi in items)
            {
                x1 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.StartDate);
                x2 = ParentRow.ParentPanel.ConvertDateToPosition(gi.Node.EndDate.AddDays(1));
                width = Math.Max(5, x2 - x1);
                if (width < 0)
                    width = 0;
                gi.Measure(new Size(width, ParentRow.ActualHeight));
            }
            return base.MeasureOverride(availableSize);
        }
		#endregion

    }
}
