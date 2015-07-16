using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using QPP.Wpf.UI.Controls.XGantt.Core;
using System;
using System.Windows.Input;

namespace QPP.Wpf.UI.Controls.XGantt
{
	/// <summary>
	/// Arranges and Measures it's child GanttRows for it's parent Panel.
	/// </summary>
    public class GanttRowsPresenter : Panel
	{
		#region Properties
		internal GanttPanel ParentPanel { get; set; }
		#endregion

		#region Constructors and Overrides
		protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.WriteLine("GanttRowsPresenter.ArrangeOverride(" + finalSize.ToString() + ")");
            double position = 0d;

            Children.OfType<GanttRow>().ToList<GanttRow>().ForEach(g =>
                {
                    if (g.Visibility == Visibility.Visible)
                    {
                        g.Arrange(new Rect(0d, position, finalSize.Width, ParentPanel.RowHeight));
                        position += ParentPanel.RowHeight;
                    }
                    else
                        g.Arrange(new Rect(0d, 0d, 0d, 0d));
                }
            );

            return base.ArrangeOverride(finalSize);
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            Debug.WriteLine("GanttRowsPresenter.MeasureOverride()");
            Children.OfType<GanttRow>().ToList().ForEach(g =>
            {
                g.Measure(new Size(availableSize.Width, ParentPanel.RowHeight));
            });

            return base.MeasureOverride(availableSize);
		}

        public GanttRowsPresenter()
        {
        }

        GanttItem predecessor;
        internal GanttItem Predecessor
        {
            get { return predecessor; }
            set
            {
                predecessor = value;
                if (predecessor != null)
                {
                    var tpoint = predecessor.TranslatePoint(
                        new Point(predecessor.NodeWidth / 2, predecessor.ActualHeight / 2),
                        ParentPanel.DependencyPresenter);
                    ParentPanel.DependencyPresenter.DependencyLinker.X1 = tpoint.X;
                    ParentPanel.DependencyPresenter.DependencyLinker.Y1 = tpoint.Y;
                }
            }
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (ParentPanel.IsReadOnly) return;
            foreach (var row in Children.OfType<GanttRow>())
                foreach (GanttItem item in row.ItemsPresenter.Children.OfType<GanttItem>())
                    item.DragState = DragState.None;
            Predecessor = null;
            ParentPanel.DependencyPresenter.DependencyLinker.Visibility = System.Windows.Visibility.Collapsed;
            ParentPanel.DependencyPresenter.IsHitTestVisible = true;
            ReleaseMouseCapture();
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (ParentPanel.IsReadOnly) return;
            ParentPanel.DependencyPresenter.IsHitTestVisible = false;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (ParentPanel.IsReadOnly) return;
            Predecessor = null;
            ParentPanel.DependencyPresenter.DependencyLinker.Visibility = System.Windows.Visibility.Collapsed;
            ParentPanel.DependencyPresenter.IsHitTestVisible = true;
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (ParentPanel.IsReadOnly) return;
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                e.MouseDevice.Capture(this, System.Windows.Input.CaptureMode.SubTree);
                var point = e.GetPosition(this);
                if (point.X < 10 && Predecessor != null)
                {
                    ParentPanel.ParentGanttChart.CurrentTime = ParentPanel.CurrentTime.AddDays(-1);
                    if (Predecessor != null && Predecessor.ParentRow.ItemsPresenter.ItemShadow.StartDate != DateTime.MinValue)
                    {
                        Predecessor.ParentRow.ItemsPresenter.ItemShadow.StartDate
                            = Predecessor.ParentRow.ItemsPresenter.ItemShadow.StartDate.AddDays(-1);
                        Predecessor.ParentRow.ItemsPresenter.ItemShadow.EndDate
                            = Predecessor.ParentRow.ItemsPresenter.ItemShadow.EndDate.AddDays(-1);
                    }
                }
                if (point.X > ActualWidth - 10 && Predecessor != null)
                {
                    ParentPanel.ParentGanttChart.CurrentTime = ParentPanel.CurrentTime.AddDays(1);
                    if (Predecessor != null && Predecessor.ParentRow.ItemsPresenter.ItemShadow.EndDate != DateTime.MaxValue)
                    {
                        Predecessor.ParentRow.ItemsPresenter.ItemShadow.StartDate
                            = Predecessor.ParentRow.ItemsPresenter.ItemShadow.StartDate.AddDays(1);
                        Predecessor.ParentRow.ItemsPresenter.ItemShadow.EndDate
                            = Predecessor.ParentRow.ItemsPresenter.ItemShadow.EndDate.AddDays(1);
                    }
                }
                if ((point.X < 10 || point.X > ActualWidth - 10) && Predecessor != null)
                {
                    var tpoint = Predecessor.TranslatePoint(
                        new Point(Predecessor.ActualWidth / 2, Predecessor.ActualHeight / 2),
                        ParentPanel);
                    ParentPanel.DependencyPresenter.DependencyLinker.X1 = tpoint.X;
                    ParentPanel.DependencyPresenter.DependencyLinker.Y1 = tpoint.Y;
                }
            }
            if (Predecessor != null && Predecessor.ParentRow.IsLinking)
            {
                var tpoint = Predecessor.TranslatePoint(
                    new Point(Predecessor.ActualWidth / 2, Predecessor.ActualHeight / 2),
                    ParentPanel);
                ParentPanel.DependencyPresenter.DependencyLinker.X1 = tpoint.X;
                ParentPanel.DependencyPresenter.DependencyLinker.Y1 = tpoint.Y;
                var point = e.GetPosition(ParentPanel.DependencyPresenter);
                ParentPanel.DependencyPresenter.DependencyLinker.Visibility = System.Windows.Visibility.Visible;
                ParentPanel.DependencyPresenter.DependencyLinker.X2 = point.X;
                ParentPanel.DependencyPresenter.DependencyLinker.Y2 = point.Y;
            }
        }

		#endregion
	}
}
