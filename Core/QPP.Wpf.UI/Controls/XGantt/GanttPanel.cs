
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QPP.Wpf.UI.Controls.XGantt.Core;
using QPP.Wpf.UI.Controls.XGantt.TimespanHeader;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace QPP.Wpf.UI.Controls.XGantt
{


	/// <summary>
	/// This control will display the gantt nodes' date ranges in a graphical layout.
	/// </summary>
	[TemplatePart(Name = "RowPresenter", Type = typeof(GanttRowsPresenter)),
	TemplatePart(Name = "ColumnPresenter", Type = typeof(GanttPanelColumnsPresenter)),
	TemplatePart(Name = "DependenciesPresenter", Type = typeof(GanttDependenciesPresenter)),
    TemplatePart(Name = "MainElement", Type = typeof(FrameworkElement)),
    TemplatePart(Name = "BackgroundCanvas", Type = typeof(Canvas))]
	public class GanttPanel : ContentControl
	{
		#region Dependency Properties
        public static DependencyProperty IsReadOnlyProperty;
        public static DependencyProperty RowHeightProperty;
        public static DependencyProperty GapBackgroundBrushProperty;
        public static DependencyProperty GapBorderBrushProperty;

		public Brush GapBorderBrush
		{
			get { return (Brush)GetValue(GapBorderBrushProperty); }
			set { SetValue(GapBorderBrushProperty, value); }
		}
		public Brush GapBackgroundBrush
		{
			get { return (Brush)GetValue(GapBackgroundBrushProperty); }
			set { SetValue(GapBackgroundBrushProperty, value); }
        }
		/// <summary>
		/// Sets the Height of each of the GanttRows
		/// </summary>
		public double RowHeight { get { return (double)GetValue(RowHeightProperty); } set { SetValue(RowHeightProperty, value); } }
		/// <summary>
		/// Determines whether the items can be moved.
		/// </summary>
		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }

		#endregion

		#region Events
		public event EventHandler<GanttItemEventArgs> ItemChanging;
		protected internal void RaiseItemChanging(GanttItemEventArgs e)
		{
			if (ItemChanging != null)
				ItemChanging(this, e);
		}


		public event EventHandler<GanttItemEventArgs> ItemChanged;
		protected internal void RaiseItemChanged(GanttItemEventArgs e)
		{
			if (ItemChanged != null)
				ItemChanged(this, e);

			UpdateDependencies(e.Item);
		}

		#endregion

		#region Properties
		/// <summary>
		/// This is the first DateTime represented in the panel.
		/// </summary>
		private DateTime _CurrentTime;

	    public DateTime CurrentTime
	    {
	        get { return _CurrentTime; }
	        set
	        {
	            _CurrentTime = value;
	            InvalidateItemPositions();
	        }
	    }

	    internal bool RowsValid { get; set; }

		private int _TopNodeIndex = 0;
		internal int TopNodeIndex
		{
			get { return _TopNodeIndex; }
			set
			{
				if (_TopNodeIndex != value)
				{
					_TopNodeIndex = value;
					ResetRows();
					UpdateDependencies();
				}
			}
		}
		internal int RowCount
		{
			get
			{
				if (RowPresenter == null)
					return 0;

				return this.RowPresenter.Children.Count;
			}
		}

		private ObservableCollection<IGanttNode> _Nodes;
        public ObservableCollection<IGanttNode> Nodes
        {
            get { return _Nodes; }
            set
            {
                _Nodes = value;
                _Nodes.CollectionChanged += Nodes_CollectionChanged;
                ResetRows();
                UpdateDependencies();
            }
        }

		private ObservableCollection<GanttDependency> _Dependencies;
		public ObservableCollection<GanttDependency> Dependencies
		{
            get { return _Dependencies; }
			set
			{
				if (_Dependencies != value)
				{
					_Dependencies = value;
					UpdateDependencies();
				}
			}
		}

		internal XGanttChart ParentGanttChart { get; set; }
		#endregion

		#region Public Handlers

		internal double ConvertDateToPosition(DateTime date)
		{
			return TimeUnitScalar.GetPosition(CurrentTime, date);
		}
        //internal TimeSpan ConvertDistanceToTimeSpan(double distance)
        //{
        //    return TimeUnitScalar.GetTimespan(CurrentTime, distance);
        //}
		#endregion

		#region Template Parts
		protected internal GanttRowsPresenter RowPresenter { get; private set; }
		//protected internal GanttPanelColumnsPresenter ColumnPresenter { get; private set; }
		protected internal GanttDependenciesPresenter DependencyPresenter { get; private set; }
        protected internal FrameworkElement MainElement { get; private set; }
        protected internal Canvas BackgroundCanvas { get; private set; }

        private bool _IsShowActualMark;
        public bool IsShowActualMark 
        {
            get { return _IsShowActualMark; }
            set 
            {
                _IsShowActualMark = value;
                if (RowPresenter != null)
                    RowPresenter.Children.OfType<GanttRow>().ToList().ForEach(r =>
                        r.IsShowActualItem = _IsShowActualMark);
            } 
        }
		#endregion

		#region Constructors and overrides

        static GanttPanel()
        {
            var thisType = typeof(GanttPanel);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));

            IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), thisType, new PropertyMetadata(false));
            RowHeightProperty = DependencyProperty.Register("RowHeight", typeof(double), thisType, new PropertyMetadata(20d));
            GapBackgroundBrushProperty = DependencyProperty.Register("GapBackgroundBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
            GapBorderBrushProperty = DependencyProperty.Register("GapBorderBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        }

        public GanttPanel()
		{
			UseLayoutRounding = false;
		}

        protected override Size MeasureOverride(Size availableSize)
        {
            GenerateRows(availableSize.Height);
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            RectangleGeometry r = new RectangleGeometry();

			r.Rect = new Rect(0, 0,
				Math.Max(0, finalSize.Width - BorderThickness.Left - BorderThickness.Right),
				Math.Max(0, finalSize.Height - BorderThickness.Top - BorderThickness.Bottom));
			
            MainElement.Clip = r;

            InvalidateItemPositions();

            return base.ArrangeOverride(finalSize);
        }

	    public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			RowPresenter = (GanttRowsPresenter)GetTemplateChild("RowPresenter");
            RowPresenter.ParentPanel = this;

            //ColumnPresenter = (GanttPanelColumnsPresenter)GetTemplateChild("ColumnPresenter");
            //ColumnPresenter.ParentPanel = this;

            BackgroundCanvas = (Canvas)GetTemplateChild("BackgroundCanvas");

			DependencyPresenter = (GanttDependenciesPresenter)GetTemplateChild("DependenciesPresenter");
			DependencyPresenter.ParentPanel = this;

			MainElement = (FrameworkElement)GetTemplateChild("MainElement");
		}

		#endregion

		#region Public functions

		public void InvalidateItemPositions()
		{
            if (RowPresenter != null)
            {
                RowPresenter.Children.OfType<GanttRow>().ToList().ForEach(r =>
                {
                    r.ItemsPresenter.InvalidateMeasure();
                });
            }

            //if (ColumnPresenter != null)
            //    ColumnPresenter.Invalidate();
			if (DependencyPresenter != null)
                DependencyPresenter.Invalidate();            
            
            UpdateBackgroud();
		}

        internal void UpdateBackgroud()
        {
            if (BackgroundCanvas == null) return;
            BackgroundCanvas.Children.Clear();
            TimeUnits unit = (ParentGanttChart.TimespanHeader.RowsPresenter
                .Children[ParentGanttChart.TimespanHeader.RowsPresenter.Children.Count - 1] as TimespanHeaderRow).TimeUnit;
            double totalWidth = 0d;
            DateTime date = CurrentTime;

            while (totalWidth < ActualWidth)
            {
                double unitWidth = TimeUnitScalar.ConvertToPixels(date, unit);

                if (!ParentGanttChart.Calendar.IsWorking(date))
                {
                    Rectangle r = new Rectangle();
                    r.Fill = new SolidColorBrush(Colors.Gray);                    
                    r.Opacity = .3;
                    r.Width = unitWidth;
                    r.Height = ActualHeight;
                    r.SetValue(Canvas.LeftProperty, totalWidth);
                    BackgroundCanvas.Children.Add(r);
                }
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    Rectangle r = new Rectangle();
                    r.Fill = new SolidColorBrush(Colors.Gray);                    
                    r.Opacity = .7;
                    r.Height = ActualHeight;
                    r.Width = 1;
                    r.SetValue(Canvas.LeftProperty, totalWidth);
                    BackgroundCanvas.Children.Add(r);
                }

                date = date.AddType(unit, 1);

                totalWidth += unitWidth;
            }
        }

		public void ValidateRowCount()
		{
			if (RowCount != (int)Math.Round(this.ActualWidth / this.RowHeight))
			{
				RowsValid = false;
				InvalidateArrange();
			}
		}
        
        public void CheckGanttRowsIsSelecting(IEnumerable<IGanttNode> selectednodes)
        {
            if (RowPresenter != null)
            {
                RowPresenter.Children.OfType<GanttRow>().ToList().ForEach(r =>
                {
                    if (selectednodes.Contains(r.Node))
                        r.IsSelecting = true;
                    else
                        r.IsSelecting = false;
                });
            }
        }
		#endregion

		#region Private functions

        private double m_OldHeight;

	    internal void ReGenerateRows()
	    {
	        RowsValid = false;
            GenerateRows(m_OldHeight);
	    }
        
        private void GenerateRows(double finalHeight, bool ignoreSameSize = true)
        {
            if (!RowsValid && (m_OldHeight != finalHeight || !ignoreSameSize))
            {
                m_OldHeight = finalHeight;

                if (RowPresenter == null)
                    return;

                RowsValid = true;
                RowPresenter.Children.Clear();
                
                for (double i = 0; i < finalHeight; i += RowHeight)
                {
                    GanttRow row = new GanttRow
                    {
                        ParentPanel = this,
                        BorderBrush = this.ParentGanttChart.GanttRowBorderBrush,
                        GapBackgroundBrush = this.GapBackgroundBrush,
                        GapBorderBrush = this.GapBorderBrush
                    };
                    row.IsShowActualItem = IsShowActualMark;
                    this.RowPresenter.Children.Add(row);
                }
                ResetRows();
            }
        }

		private void ResetRows()
		{
			if (RowPresenter == null)
				return;

			for (int i = 0; i < RowPresenter.Children.Count; i++)
			{
				GanttRow row = RowPresenter.Children[i] as GanttRow;
				row.BorderBrush = ParentGanttChart.GanttRowBorderBrush;
				row.Background = Background;
			    row._sameSizeCnt = 1;

				if (i + TopNodeIndex < Nodes.Count)
				{
					row.Node = Nodes[i + TopNodeIndex];
				}
				else
					row.Node = null;
            }
		}
		private GanttRow CreateRow(IGanttNode node)
		{
			GanttRow row = new GanttRow();
			row.BorderBrush = BorderBrush;
			row.Background = Background;
			row.ParentPanel = this;
			row.Node = node;
			return row;
		}

		internal void UpdateDependencies()
		{
			if (DependencyPresenter != null)
				DependencyPresenter.Invalidate();
		}

		private void UpdateDependencies(GanttItem ganttItem)
		{
			var deps = Dependencies.Where(d => d.Successor == ganttItem.Node || d.Predecessor == ganttItem.Node);

            var items = DependencyPresenter.Children.OfType<GanttDependencyItem>()
                .Where(d => d.Dependency.Successor == ganttItem.Node
                    || d.Dependency.Predecessor == ganttItem.Node);

			foreach (var d in items)
				d.UpdateDependencyLines();
		}
		#endregion

		#region Event Handler functions
		private void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					ResetRows();
					break;
				case NotifyCollectionChangedAction.Remove:
					ResetRows();
					break;
				case NotifyCollectionChangedAction.Reset:
				default:
					ResetRows();
					break;
			}

			UpdateDependencies();
		}
		#endregion
	}
}
