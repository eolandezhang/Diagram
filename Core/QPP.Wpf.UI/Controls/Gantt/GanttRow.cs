using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using QPP.Wpf.UI.Controls.Gantt.Core;

namespace QPP.Wpf.UI.Controls.Gantt
{
	[TemplatePart(Name = "ItemsPresenterElement", Type = typeof(GanttItemsPresenter))]
	public class GanttRow : Control
	{
		#region Dependency Properties
        public static DependencyProperty GapBackgroundBrushProperty;
        public static DependencyProperty GapBorderBrushProperty;
        public static DependencyProperty IsSelectingProperty;
	
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

        public bool IsSelecting
        {
            get { return (bool)GetValue(IsSelectingProperty); }
            set { SetValue(IsSelectingProperty, value); }
        }
		#endregion

		#region Template Parts
		internal GanttItemsPresenter ItemsPresenter { get; set; }
		#endregion

		#region Private variables
		Point _DragStart = new Point(0, 0);
		bool _ProcessingMove = false;
		#endregion

		#region Properties
        private bool _IsShowActualItem;
        public bool IsShowActualItem 
        {
            get { return _IsShowActualItem ;}
            set 
            {
                _IsShowActualItem = value;
                if (ItemsPresenter!=null)
                    ItemsPresenter.Children.OfType<GanttActualItem>().ToList().ForEach(
                       p => p.Visibility = _IsShowActualItem ? Visibility.Visible : Visibility.Hidden);
            } 
        }
        internal GanttPanel ParentPanel { get; set; }
        internal bool IsLinking { get; set; }
		public bool ItemsValid { get; set; }
		private IGanttNode _Node;
        public IGanttNode Node
        {
            get { return _Node; }

            internal set
            {
                if (_Node != value)
                {
                    if (_Node != null)
                    {
                        _Node.Children.CollectionChanged -= ChildNodes_CollectionChanged;
                        _Node.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Node_PropertyChanged);
                    }
                    _Node = value;
                    if (_Node != null)
                    {
                        _Node.Children.CollectionChanged += ChildNodes_CollectionChanged;
                        _Node.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Node_PropertyChanged);
                    }
                    ItemsValid = false;
                    Invalidate();
                }
            }
        }

        void ChildNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ItemsValid = false;
            Invalidate();
        }

		public bool IsReadOnly { get { return ParentPanel.IsReadOnly; } }
        public int RowIndex { get { return ParentPanel.Nodes.IndexOf(Node); } }
		#endregion

		#region Constructors and overrides
		public GanttRow(IGanttNode node)
			: this()
		{
			this.Node = node;
		}

        static GanttRow()
        {
            var thisType = typeof(GanttRow);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
            GapBackgroundBrushProperty = DependencyProperty.Register("GapBackgroundBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
            GapBorderBrushProperty = DependencyProperty.Register("GapBorderBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
            IsSelectingProperty = DependencyProperty.Register("IsSelecting", typeof(bool), thisType, new PropertyMetadata(null));
        }

        public GanttRow()
		{
            UseLayoutRounding = false;
			ItemsValid = false;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			ItemsPresenter = (GanttItemsPresenter)GetTemplateChild("ItemsPresenterElement");
            if(ItemsPresenter!=null)
			ItemsPresenter.ParentRow = this;
		}

	    private Size _oldSize;
        private Size _oldRetSize;
	    internal int _sameSizeCnt = 0;
		protected override Size ArrangeOverride(Size finalSize)
		{
            if (_oldSize != finalSize || _sameSizeCnt != 0)
            {
                if (_oldSize != finalSize)
                    _sameSizeCnt = 1;
                else
                    _sameSizeCnt--;

		        _oldSize = finalSize;
		        if (Node != null)
		            GenerateItems();
                else
                    ItemsPresenter.Children.Clear();

                ItemsPresenter.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                _oldRetSize = base.ArrangeOverride(finalSize);

		        return _oldRetSize;
		    }

            return _oldRetSize;
		}
		protected override Size MeasureOverride(Size availableSize)
		{
			ItemsPresenter.Measure(availableSize);
			return base.MeasureOverride(availableSize);
		}
		#endregion

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (ParentPanel.IsReadOnly) return;
            _ProcessingMove = false;
            ItemsPresenter.ItemShadow.Visibility = Visibility.Collapsed;
            IsLinking = true;
            Cursor = Cursors.Arrow;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (ParentPanel.IsReadOnly) return;
            if (ParentPanel.RowPresenter.Predecessor != null && ParentPanel.RowPresenter.Predecessor.ParentRow == this)
            {
                ItemsPresenter.ItemShadow.Visibility = Visibility.Visible;
                ParentPanel.DependencyPresenter.DependencyLinker.Visibility = Visibility.Collapsed;
                CaptureMouse();
            }
            IsLinking = false;
            Cursor = Cursors.Arrow;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!IsReadOnly)
            {
                _DragStart = e.GetPosition(UIHelpers.RootUI);
                CaptureMouse();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (ParentPanel.IsReadOnly) return;
            _DragStart = new Point(0, 0);
            _ProcessingMove = false;
            Cursor = Cursors.Arrow;
            ItemsPresenter.ItemShadow.Visibility = System.Windows.Visibility.Collapsed;
            if (!IsReadOnly)
            {
                ItemsPresenter.Children.OfType<GanttItem>().ToList().ForEach(item =>
                {
                    if (item.DragState != DragState.None)
                    {
                        if (item.DragState == DragState.ResizeRight)
                        {
                            ParentPanel.RaiseItemChanging(new GanttItemEventArgs(item));
                            item.Node.EndDate = ItemsPresenter.ItemShadow.EndDate
                                //.Date.AddDays(1);
                                .Date.AddHours(item.Node.EndDate.Hour).AddMinutes(item.Node.EndDate.Minute).AddSeconds(item.Node.EndDate.Second);
                            item.InvalidateMeasure();
                            ParentPanel.RaiseItemChanged(new GanttItemEventArgs(item));
                        }
                        else if (item.DragState == DragState.Whole)
                        {
                            ParentPanel.RaiseItemChanging(new GanttItemEventArgs(item));
                            /*********从精确到天 变为 精确到时，这里可能要重构*********/
                            var d = ParentPanel.ParentGanttChart.Calendar.GetDuration(item.Node.StartDate, item.Node.EndDate);
                            item.Node.StartDate = ItemsPresenter.ItemShadow.StartDate
                                //.Date;
                                .Date.AddHours(item.Node.StartDate.Hour).AddMinutes(item.Node.StartDate.Minute).AddSeconds(item.Node.StartDate.Second);
                            item.Node.EndDate = ParentPanel.ParentGanttChart.Calendar.GetEndDate(item.Node.StartDate, d)
                                //.Date;
                                .Date.AddHours(item.Node.EndDate.Hour).AddMinutes(item.Node.EndDate.Minute).AddSeconds(item.Node.EndDate.Second);
                            /*******************/
                            item.InvalidateArrange();
                            ParentPanel.RaiseItemChanged(new GanttItemEventArgs(item));
                        }
                        item.DragState = DragState.None;
                    }
                });
            }
            ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (ParentPanel.IsReadOnly) return;
            _sameSizeCnt = 1;
            IsLinking = false;
            if (_ProcessingMove || IsReadOnly)
                return;
            else
                _ProcessingMove = true;

            Point cursorPosition = e.GetPosition(UIHelpers.RootUI);

            var items = ItemsPresenter.Children.OfType<GanttItem>();
            foreach (var item in items)
            {
                double distance = cursorPosition.X - _DragStart.X;
                if (item.DragState == DragState.ResizeRight)
                    Cursor = Cursors.SizeWE;
                else if (item.DragState == DragState.Whole)
                    Cursor = Cursors.SizeAll;
                else
                {
                    _ProcessingMove = false;
                    continue;
                }

                TimeSpan ts = TimeUnitScalar.GetTimespan(ParentPanel.CurrentTime, distance);
                if (item.DragState == DragState.ResizeRight)
                {
                    DateTime newDate = ItemsPresenter.ItemShadow.EndDate.Add(ts);
                    if (newDate >= ItemsPresenter.ItemShadow.StartDate.AddHours(-1))
                    {
                        ItemsPresenter.ItemShadow.EndDate = newDate;
                        ItemsPresenter.InvalidateArrange();
                    }
                }
                else if (item.DragState == DragState.Whole)
                {
                    DateTime newStart = ItemsPresenter.ItemShadow.StartDate.Add(ts);
                    DateTime newEnd = ItemsPresenter.ItemShadow.EndDate.Add(ts);
                    ItemsPresenter.ItemShadow.StartDate = newStart;
                    ItemsPresenter.ItemShadow.EndDate = newEnd;
                    ItemsPresenter.InvalidateMeasure();
                    ItemsPresenter.InvalidateArrange();
                }
                _DragStart = e.GetPosition(UIHelpers.RootUI);
            }
            
            var y = e.GetPosition(this).Y;
            if (y < 0 || y > ActualHeight)
                ReleaseMouseCapture();

            _ProcessingMove = false;
        }

		private void GenerateItems()
		{
			if (Node == null)
			{
				ItemsPresenter.Children.Clear();
			}
            else if (!ItemsValid)
            {
                ItemsValid = true;
                ItemsPresenter.Children.Clear();

                GanttItem item = null;

                if (Node.Children.Count == 0)
                    item = new GanttItem();
                else
                    item = new HeaderGanttItem();

                item.GapBackgroundBrush = this.GapBackgroundBrush;
                item.GapBorderBrush = this.GapBorderBrush;
                item.ItemContent = this.Node;
                item.ToolTipContentTemplate = this.ParentPanel.ParentGanttChart.ToolTipContentTemplate;
                item.ItemLeftTemplate = this.ParentPanel.ParentGanttChart.ItemLeftTemplate;
                item.ItemRightTemplate = this.ParentPanel.ParentGanttChart.ItemRightTemplate;
                item.ItemTopTemplate = this.ParentPanel.ParentGanttChart.ItemTopTemplate;
                item.ItemBottomTemplate = this.ParentPanel.ParentGanttChart.ItemBottomTemplate;
                item.ParentRow = this;
                item.Node = Node;
                ItemsPresenter.ItemShadow.StartDate = Node.StartDate;
                ItemsPresenter.ItemShadow.EndDate = Node.EndDate;
                ItemsPresenter.Children.Add(item);
                ItemsPresenter.Children.Add(ItemsPresenter.ItemShadow);

                if (Node.ActualStartDate.HasValue && Node.ActualEndDate.HasValue)
                {
                    var actualItem = new GanttActualItem();
                    actualItem.ParentRow = this;
                    actualItem.ItemContent = Node;
                    actualItem.ToolTipContentTemplate = this.ParentPanel.ParentGanttChart.ToolTipContentTemplate;
                    actualItem.Node = Node;
                    actualItem.IsFinished = Node.PercentComplete == 100;
                    actualItem.Visibility = IsShowActualItem ? Visibility.Visible : Visibility.Hidden;
                    ItemsPresenter.Children.Add(actualItem);
                }
                
            }
		}
        
		internal void Invalidate()
		{
		    _sameSizeCnt = 1;

            Debug.WriteLine("GanttRow.Invalidate()");
			if (ItemsPresenter == null)
				return;

		    ParentPanel.RowsValid = false;

            //this.InvalidateArrange();
			this.InvalidateMeasure();

			//ItemsPresenter.InvalidateArrange();
			ItemsPresenter.InvalidateMeasure();            
		}


        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActualStartDate" || e.PropertyName == "ActualEndDate")
            {
                if (Node.ActualStartDate.HasValue && Node.ActualEndDate.HasValue)
                {
                    var actualItem = new GanttActualItem();
                    actualItem.ParentRow = this;
                    actualItem.ItemContent = Node;
                    actualItem.ToolTipContentTemplate = this.ParentPanel.ParentGanttChart.ToolTipContentTemplate;
                    actualItem.Node = Node;
                    actualItem.IsFinished = Node.PercentComplete == 100;
                    actualItem.Visibility = IsShowActualItem ? Visibility.Visible : Visibility.Hidden;
                    ItemsPresenter.Children.Add(actualItem);
                }
            }
        }
	}
}
