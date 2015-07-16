using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using QPP.Wpf.UI.Controls.Gantt.Core;
using QPP.Wpf.UI.Controls.Gantt.TimespanHeader;
using System.Collections.Generic;

namespace QPP.Wpf.UI.Controls.Gantt
{
    public class GanttDependencyItem : Grid
	{
		public static DependencyProperty LineWidthProperty = 
            DependencyProperty.Register("LineWidth", typeof(double), typeof(GanttDependencyItem),
            new PropertyMetadata(1d));

        public static DependencyProperty LineBrushProperty =
            DependencyProperty.Register("LineBrush", typeof(Brush), typeof(GanttDependencyItem),
            new PropertyMetadata(null));

		public double LineWidth
		{
			get { return (double)GetValue(LineWidthProperty); }
			set { SetValue(LineWidthProperty, value); }
		}

        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

		private GanttDependency _Dependency;
        public GanttDependency Dependency
        {
            get { return _Dependency; }
            set
            {
                _Dependency = value;
                if (value != null)                    
                    value.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(value_PropertyChanged);
            }
        }

        void value_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }

		public GanttDependenciesPresenter ParentPresenter { get; set; }
        
        static GanttDependencyItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GanttDependencyItem), new FrameworkPropertyMetadata(typeof(GanttDependencyItem)));
        }

        public GanttDependencyItem()
		{
			this.Loaded += new RoutedEventHandler(GanttDependencyItem_Loaded);
		}
		
        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();

        //    UpdateDependencyLines();
        //}

        private void GanttDependencyItem_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDependencyLines();
        }

	    internal void UpdateDependencyLines()
	    {
            if (Dependency == null || ParentPresenter == null)
	            return;

	        switch (Dependency.Type)
	        {
	            default:
	            case DependencyType.FS:
	                DrawChildBeginsAtParentEndLines();
	                break;
	        }
	    }

        private void DrawChildBeginsAtParentEndLines()
        {
            Children.Clear();
            var timeUnit = ParentPresenter.ParentPanel.ParentGanttChart.TimespanHeader.RowsPresenter.Children.Cast<TimespanHeaderRow>().Last().TimeUnit;
            //var endDate = Dependency.Predecessor.EndDate.AddDays(1);
            var endDate = Dependency.Predecessor.EndDate;
            bool isEquivolent = TimeUnitScalar.IsEquivolent(Dependency.Successor.StartDate,
                endDate, timeUnit);

            bool inverted = isEquivolent || (Dependency.Successor.StartDate <= endDate);

            int parentIndex = ParentPresenter.ParentPanel.Nodes.IndexOf(Dependency.Predecessor);
            int childIndex = ParentPresenter.ParentPanel.Nodes.IndexOf(Dependency.Successor);

            bool parentAboveChild = parentIndex < childIndex;

            var offset = TimeUnitScalar.ConvertToPixels(ParentPresenter.ParentPanel.CurrentTime, timeUnit);

            double startX = TimeUnitScalar.GetPosition(ParentPresenter.ParentPanel.CurrentTime, endDate) - 1;
            double startY = (parentIndex * ParentPresenter.ParentPanel.RowHeight)
                + (ParentPresenter.ParentPanel.RowHeight / 2d)
                - (ParentPresenter.ParentPanel.RowHeight * ParentPresenter.ParentPanel.TopNodeIndex);

            //double endX = TimeUnitScalar.GetPosition(ParentPresenter.ParentPanel.CurrentTime, Dependency.Successor.StartDate) + offset / 2;
            double endX = TimeUnitScalar.GetPosition(ParentPresenter.ParentPanel.CurrentTime, Dependency.Successor.StartDate) + 5;
            double endY = 0;
            if (childIndex > parentIndex)
                endY = childIndex * ParentPresenter.ParentPanel.RowHeight + 1
                    - ParentPresenter.ParentPanel.RowHeight * ParentPresenter.ParentPanel.TopNodeIndex;
            else
                endY = (childIndex + 1) * ParentPresenter.ParentPanel.RowHeight - 2
                    - ParentPresenter.ParentPanel.RowHeight * ParentPresenter.ParentPanel.TopNodeIndex;
            
            double pip = TimeUnitScalar.ConvertToPixels(ParentPresenter.ParentPanel.CurrentTime,
                ParentPresenter.ParentPanel.ParentGanttChart.TimespanHeader.LowerUnit) / 2d;

            Polyline line = new Polyline();
            line.Points.Add(new Point(startX, startY));
            line.Points.Add(new Point(endX, startY));
            line.Points.Add(new Point(endX, endY));
            line.StrokeEndLineCap = PenLineCap.Triangle;
            line.SnapsToDevicePixels = true;
            line.Stroke = LineBrush;
            line.StrokeThickness = LineWidth;

            Polyline lineHandler = new Polyline();
            lineHandler.Points.Add(new Point(startX, startY));
            lineHandler.Points.Add(new Point(endX, startY));
            lineHandler.Points.Add(new Point(endX, endY));
            lineHandler.StrokeEndLineCap = PenLineCap.Triangle;
            lineHandler.SnapsToDevicePixels = true;
            lineHandler.Stroke = new SolidColorBrush(Colors.Transparent);
            lineHandler.Cursor = System.Windows.Input.Cursors.Hand;
            lineHandler.StrokeThickness = LineWidth + 6;
            lineHandler.ContextMenu = ItemContextMenu;
            lineHandler.ContextMenuOpening += new ContextMenuEventHandler(lineHandler_ContextMenuOpening);

            Polygon triangle = new Polygon();
            triangle.Fill = LineBrush;
            triangle.Points.Add(new Point(endX, endY));
            if (endY < startY)
            {
                triangle.Points.Add(new Point(endX + 4, endY + 3));
                triangle.Points.Add(new Point(endX - 4, endY + 3));
            }
            else
            {
                triangle.Points.Add(new Point(endX + 4, endY - 3));
                triangle.Points.Add(new Point(endX - 4, endY - 3));
            }
            Children.Add(triangle);
            Children.Add(line);
            Children.Add(lineHandler);
        }

        void lineHandler_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ItemContextMenu.Tag = Dependency;
        }

        ContextMenu contextMenu;
        ContextMenu ItemContextMenu
        {
            get
            {
                if (contextMenu == null)
                {
                    contextMenu = new ContextMenu();
                    var item = new MenuItem();
                    item.IsEnabled = !ParentPresenter.ParentPanel.IsReadOnly;
                    item.Header = QPP.Wpf.UI.Properties.Resources.Delete;
                    item.Click += new RoutedEventHandler(item_Click);
                    contextMenu.Items.Add(item);
                }
                return contextMenu;
            }
        }

        void item_Click(object sender, RoutedEventArgs e)
        {
            Dependency.Predecessor.Successors.Remove(Dependency);
            ParentPresenter.ParentPanel.Dependencies.Remove(Dependency);
        }

		//private void UpdateDependencyLines()
		//{
		//    if (Dependency == null || TopLeft == null)
		//        return;

		//    bool ParentAboveChild = true;

		//    if(ParentAboveChild)
		//    {
		//        switch (Dependency.Type)
		//        {
		//            default:
		//            case DependencyType.ChildBeginsAtParentEnd:

		//                if (Dependency.ParentNode.EndDate > Dependency.ChildNode.StartDate)
		//                {
		//                    TopLeft.Left = TopLeft.Right = true;
		//                    TopCenter.Left = TopCenter.Down = true;
		//                    MiddleCenter.Up = MiddleCenter.Down = true;
		//                    BottomCenter.Up = BottomCenter.Right = true;
		//                    BottomRight.Left = BottomRight.Right = true;

		//                    TopRight.Visibility =
		//                        MiddleLeft.Visibility =
		//                        MiddleRight.Visibility =
		//                        BottomLeft.Visibility = Visibility.Collapsed;

		//                    TopLeft.Visibility =
		//                        TopCenter.Visibility =
		//                        MiddleCenter.Visibility =
		//                        BottomCenter.Visibility =
		//                        BottomRight.Visibility = Visibility.Visible;
		//                }
		//                break;
		//        }

		//    }
		//}
		
	}
}
