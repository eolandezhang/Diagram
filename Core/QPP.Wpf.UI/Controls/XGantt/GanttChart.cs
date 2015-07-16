using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using QPP.Wpf.UI.Controls.XGantt.Core;
using System.Collections.Specialized;
using System.Windows.Data;
using System.ComponentModel;

namespace QPP.Wpf.UI.Controls.XGantt
{
    /// <summary>
    /// The Gantt Chart will display a datagrid with a graphically represented schedule cross-referenced by
    /// a timeline.
    /// </summary>
    [TemplatePart(Name = "TimespanElement", Type = typeof(TimespanHeader.TimespanHeader)),
    TemplatePart(Name = "VerticalScrollbar", Type = typeof(ScrollBar)),
    TemplatePart(Name = "HorizontalScrollbar", Type = typeof(ScrollBar)),
    TemplatePart(Name = "PanelElement", Type = typeof(GanttPanel))]
    public class XGanttChart : Chart
    {
        #region Dependency Properties
        public static readonly DependencyProperty TopBarTimeUnitProperty;
        public static readonly DependencyProperty BottomBarTimeUnitProperty;
        public static readonly DependencyProperty DependenciesProperty;
        public static readonly DependencyProperty SelectedNodeProperty;
        public static readonly DependencyProperty GanttRowBorderBrushProperty;
        public static readonly DependencyProperty HorizontalScrollbarVisibilityProperty;
        public static readonly DependencyProperty ToolTipContentTemplateProperty;
        public static readonly DependencyProperty IsGridReadOnlyProperty;
        public static readonly DependencyProperty IsChartReadOnlyProperty;
        public static readonly DependencyProperty CurrentTimeProperty;
        public static readonly DependencyProperty SupportLinkVisibilityProperty;
        public static readonly DependencyProperty TimespanBackgroundProperty;
        public static readonly DependencyProperty TimespanBorderBrushProperty;
        public static readonly DependencyProperty TimespanBorderThicknessProperty;
        public static readonly DependencyProperty NodesProperty;
        public static readonly DependencyProperty GapBackgroundBrushProperty;
        public static readonly DependencyProperty GapBorderBrushProperty;
        public static readonly DependencyProperty GridWidthProperty;
        public static readonly DependencyProperty RowHeightProperty;
        public static readonly DependencyProperty CalendarProperty;

        public static readonly DependencyProperty ItemLeftTemplateProperty;
        public static readonly DependencyProperty ItemRightTemplateProperty;
        public static readonly DependencyProperty ItemContentTemplateProperty;
        //public static readonly DependencyProperty ItemBottomTemplateProperty;

        public static readonly DependencyProperty SelectedNodesProperty;
        public static readonly DependencyProperty IsShowActualMarkProperty;

        public ObservableCollection<IGanttNode> SelectedNodes
        {
            get { return (ObservableCollection<IGanttNode>)GetValue(SelectedNodesProperty); }
            set { SetValue(SelectedNodesProperty, value); }
        }

        public bool IsShowActualMark
        {
            get { return (bool)GetValue(IsShowActualMarkProperty); }
            set {SetValue(IsShowActualMarkProperty, value);}
        }

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public GridLength GridWidth
        {
            get { return (GridLength)GetValue(GridWidthProperty); }
            set { SetValue(GridWidthProperty, value); }
        }

        [Localizability(LocalizationCategory.NeverLocalize), Bindable(true), Category("Appearance")]
        public IGanttNode SelectedNode
        {
            get { return (IGanttNode)GetValue(SelectedNodeProperty); }
            set { SetValue(SelectedNodeProperty, value); }
        }

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

        public Brush GanttRowBorderBrush
        {
            get { return (Brush)GetValue(GanttRowBorderBrushProperty); }
            set { SetValue(GanttRowBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets and sets the visibility of the Horizontal Scrollbar.
        /// </summary>
        public Visibility HorizontalScrollbarVisibility
        {
            get { return (Visibility)GetValue(HorizontalScrollbarVisibilityProperty); }
            set
            {
                SetValue(HorizontalScrollbarVisibilityProperty, value);

                if (HorizontalScrollbar != null)
                    HorizontalScrollbar.Visibility = value;
            }
        }

        /// <summary>
        /// Gets and sets the Content Template that is shown in the tooltip 
        /// for the displayed GanttItems in the GanttPanel.
        /// </summary>
        public DataTemplate ToolTipContentTemplate
        {
            get { return (DataTemplate)GetValue(ToolTipContentTemplateProperty); }
            set { SetValue(ToolTipContentTemplateProperty, value); }
        }
        /// <summary>
        /// Determines whether the items in the Gantt can be moved.
        /// </summary>
        public bool IsChartReadOnly
        {
            get { return (bool)GetValue(IsChartReadOnlyProperty); }
            set
            {
                SetValue(IsChartReadOnlyProperty, value);
                if (Panel != null) Panel.IsReadOnly = value;
            }
        }

        /// <summary>
        /// This is the current or starting time for the gantt chart.
        /// </summary>
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        /// <summary>
        /// This is the brush to apply to the background of the TimespanHeader.
        /// </summary>
        public Brush TimespanBackground
        {
            get { return (Brush)GetValue(TimespanBackgroundProperty); }
            set { SetValue(TimespanBackgroundProperty, value); }
        }

        /// <summary>
        /// This is the brush to apply to the border of the TimespanHeader.
        /// </summary>
        public Brush TimespanBorderBrush
        {
            get { return (Brush)GetValue(TimespanBorderBrushProperty); }
            set { SetValue(TimespanBorderBrushProperty, value); }
        }

        /// <summary>
        /// This is the Thickness to apply to the border of the TimespanHeader.
        /// </summary>
        public Thickness TimespanBorderThickness
        {
            get { return (Thickness)GetValue(TimespanBorderThicknessProperty); }
            set { SetValue(TimespanBorderThicknessProperty, value); }
        }

        /// <summary>
        /// The visibility of the support link at the bottom of the control.
        /// </summary>
        public Visibility SupportLinkVisibility
        {
            get { return (Visibility)GetValue(SupportLinkVisibilityProperty); }
            set { SetValue(SupportLinkVisibilityProperty, value); }
        }
        public DataTemplate ItemLeftTemplate
        {
            get { return (DataTemplate)GetValue(ItemLeftTemplateProperty); }
            set { SetValue(ItemLeftTemplateProperty, value); }
        }
        public DataTemplate ItemRightTemplate
        {
            get { return (DataTemplate)GetValue(ItemRightTemplateProperty); }
            set { SetValue(ItemRightTemplateProperty, value); }
        }
        public DataTemplate ItemContentTemplate
        {
            get { return (DataTemplate)GetValue(ItemContentTemplateProperty); }
            set { SetValue(ItemContentTemplateProperty, value); }
        }
        //public DataTemplate ItemBottomTemplate
        //{
        //    get { return (DataTemplate)GetValue(ItemBottomTemplateProperty); }
        //    set { SetValue(ItemBottomTemplateProperty, value); }
        //}

        /// <summary>
        /// A list of the top level nodes to display on the gantt chart.
        /// </summary>
        [System.ComponentModel.Bindable(true)]
        public GanttNodeCollection Nodes
        {
            get { return (GanttNodeCollection)GetValue(NodesProperty); }
            set { SetValue(NodesProperty, value); }
        }

        static object OnCoerecNodesValue(DependencyObject d, object baseValue)
        {
            return baseValue ?? new GanttNodeCollection();
        }

        static void OnCurrentTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var g = ((XGanttChart)d);
            if (g.TimespanHeader != null)
            {
                //g._CurrentTimeChangedLocally = true;
                g.TimespanHeader.CurrentTime = (DateTime)e.NewValue;
            }
        }

        static void OnCalendarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var g = ((XGanttChart)d);
            if (g.TimespanHeader != null)
            {
                g.TimespanHeader.Calendar = (GanttCalendar)e.NewValue;
                g.TimespanHeader.RowsPresenter.InvalidateCells();
            }
        }

        static void OnNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = e.NewValue as GanttNodeCollection;
            ((XGanttChart)d).BindNodes();
        }

        #endregion

        #region Private Variables

        private bool _CurrentTimeChangedLocally = false;
        private DateTime _StartScrollingTime = DateTime.MinValue;
        private static GridLength _Width = new GridLength(400d);
        #endregion

        #region Properties

        public GanttCalendar Calendar
        {
            get { return (GanttCalendar)GetValue(CalendarProperty); }
            set { SetValue(CalendarProperty, value); }
        }

        private ObservableCollection<DataGridColumn> _Columns;
        public ObservableCollection<DataGridColumn> Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }

        public ObservableCollection<GanttDependency> Dependencies
        {
            get
            {
                return (ObservableCollection<GanttDependency>)GetValue(DependenciesProperty);
            }
            set
            {
                SetValue(DependenciesProperty, value);
                if (Panel != null)
                    Panel.Dependencies = value;
            }
        }

        public TimeUnits TopBarTimeUnit
        {
            get { return (TimeUnits)GetValue(TopBarTimeUnitProperty); }
            set { SetValue(TopBarTimeUnitProperty, value); }
        }

        public TimeUnits BottomBarTimeUnit
        {
            get { return (TimeUnits)GetValue(BottomBarTimeUnitProperty); }
            set { SetValue(BottomBarTimeUnitProperty, value); }
        }

        #endregion

        #region Template Parts

        protected ScrollBar VerticalScrollbar { get; private set; }
        protected ScrollBar HorizontalScrollbar { get; private set; }
        internal TimespanHeader.TimespanHeader TimespanHeader { get; private set; }
        protected GanttPanel Panel { get; private set; }

        #endregion

        #region Constructors and Overrides

        //private DispatcherTimer timer;

        static XGanttChart()
        {
            var thisType = typeof(XGanttChart);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));

            TopBarTimeUnitProperty = DependencyProperty.Register("TopBarTimeUnit", typeof(TimeUnits), thisType, new PropertyMetadata(TimeUnits.Months));
            BottomBarTimeUnitProperty = DependencyProperty.Register("BottomBarTimeUnit", typeof(TimeUnits), thisType, new PropertyMetadata(TimeUnits.Days));

            DependenciesProperty = DependencyProperty.Register("Dependencies", typeof(ObservableCollection<GanttDependency>), thisType, new FrameworkPropertyMetadata(null));
            SelectedNodeProperty = DependencyProperty.Register("SelectedNode", typeof(IGanttNode), thisType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            GanttRowBorderBrushProperty = DependencyProperty.Register("GanttRowBorderBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
            HorizontalScrollbarVisibilityProperty = DependencyProperty.Register("HorizontalScrollbarVisibility", typeof(Visibility), thisType, new PropertyMetadata(Visibility.Visible));
            ToolTipContentTemplateProperty = DependencyProperty.Register("ToolTipContentTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));
            IsGridReadOnlyProperty = DependencyProperty.Register("IsGridReadOnly", typeof(bool), thisType, new PropertyMetadata(false));
            IsChartReadOnlyProperty = DependencyProperty.Register("IsChartReadOnly", typeof(bool), thisType, new PropertyMetadata(false));
            CurrentTimeProperty = DependencyProperty.Register("CurrentTime", typeof(DateTime), thisType, new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCurrentTimeChanged));
            SupportLinkVisibilityProperty = DependencyProperty.Register("SupportLinkVisibility", typeof(Visibility), thisType, new PropertyMetadata(Visibility.Visible));
            TimespanBackgroundProperty = DependencyProperty.Register("TimespanBackground", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.White)));
            TimespanBorderBrushProperty = DependencyProperty.Register("TimespanBorderBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Black)));
            TimespanBorderThicknessProperty = DependencyProperty.Register("TimespanBorderThickness", typeof(Thickness), thisType, new PropertyMetadata(new Thickness(1d)));
            NodesProperty = DependencyProperty.Register("Nodes", typeof(GanttNodeCollection), thisType, new FrameworkPropertyMetadata(new GanttNodeCollection(), OnNodesChanged, OnCoerecNodesValue));
            GapBackgroundBrushProperty = DependencyProperty.Register("GapBackgroundBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
            GapBorderBrushProperty = DependencyProperty.Register("GapBorderBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
            GridWidthProperty = DependencyProperty.Register("GridWidth", typeof(GridLength), thisType, new PropertyMetadata(new GridLength(400d)));
            RowHeightProperty = DependencyProperty.Register("RowHeight", typeof(double), thisType, new PropertyMetadata(23d));
            CalendarProperty = DependencyProperty.Register("Calendar", typeof(GanttCalendar), thisType, new PropertyMetadata(new GanttCalendar(), OnCalendarChanged));
            
            ItemLeftTemplateProperty = DependencyProperty.Register("ItemLeftTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));
            ItemRightTemplateProperty = DependencyProperty.Register("ItemRightTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));
            ItemContentTemplateProperty = DependencyProperty.Register("ItemContentTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));
            //ItemBottomTemplateProperty = DependencyProperty.Register("ItemBottomTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));

            SelectedNodesProperty = DependencyProperty.Register("SelectedNodes", typeof(IEnumerable<IGanttNode>), thisType, new FrameworkPropertyMetadata(null));

            IsShowActualMarkProperty = DependencyProperty.Register("IsShowActualMark", typeof(bool), thisType, new PropertyMetadata(false, OnIsShowActualMarkChanged));
       
        }

        
        static void UpdateGanttActualItemVisibility(XGanttChart g, bool isShowActualMark)
        {
            if (g == null) return;
            var panel = (GanttPanel)g.GetTemplateChild("PanelElement");
            if (panel != null)
            panel.IsShowActualMark = isShowActualMark;
        }
        private static void OnIsShowActualMarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateGanttActualItemVisibility((XGanttChart)d, (bool)e.NewValue);
        }

        public XGanttChart()
        {
            _Columns = new ObservableCollection<DataGridColumn>();
            Dependencies = new ObservableCollection<GanttDependency>();
            this.SizeChanged += new SizeChangedEventHandler(GanttChart_SizeChanged);
            SelectedNodes = new ObservableCollection<IGanttNode>();
            
        }

        GanttNodeCollection nodes = new GanttNodeCollection();
        
       // override 
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //Calendar = new Gantt.GanttCalendar().SetWorking(DayOfWeek.Saturday, true).SetWorking(new DateTime(2014,1,1), false);

            TimespanHeader = (TimespanHeader.TimespanHeader)GetTemplateChild("TimespanElement");
            TimespanHeader.CurrentTimeChanged += Timespan_CurrentTimeChanged;
            TimespanHeader.ZoomFactorChanged += Timespan_ZoomFactorChanged;
            TimespanHeader.Loaded += new RoutedEventHandler(TimespanHeader_Loaded);
            TimespanHeader.Calendar = Calendar;

            Panel = (GanttPanel)GetTemplateChild("PanelElement");
            Panel.Nodes = Nodes;
            Panel.ParentGanttChart = this;
            Panel.Dependencies = this.Dependencies;
            Panel.CurrentTime = TimespanHeader.CurrentTime;
            Panel.IsShowActualMark = IsShowActualMark;
            Binding selectdValueBinding = new Binding("SelectedNode");
            selectdValueBinding.Source = this;

            #region Scrollbar
            VerticalScrollbar = (ScrollBar)GetTemplateChild("VerticalScrollbar");
            VerticalScrollbar.SmallChange = 1;
            VerticalScrollbar.LargeChange = 5;
            VerticalScrollbar.Scroll += new ScrollEventHandler(VerticalScrollbar_Scroll);

            Binding heightBinding = new Binding("ScrollableHeight");
            VerticalScrollbar.SetBinding(ScrollBar.MaximumProperty, heightBinding);
            Binding visibilityBinding = new Binding("ScrollableHeight");
            visibilityBinding.Converter = new QPP.Wpf.UI.Converters.BoolToVisibilityConverter();
            VerticalScrollbar.SetBinding(ScrollBar.VisibilityProperty, visibilityBinding);
            Binding viewportBinding = new Binding("ViewportHeight");
            VerticalScrollbar.SetBinding(ScrollBar.ViewportSizeProperty, viewportBinding);

            HorizontalScrollbar = (ScrollBar)GetTemplateChild("HorizontalScrollbar");
            HorizontalScrollbar.Scroll += new ScrollEventHandler(HorizontalScrollbar_Scroll);
            HorizontalScrollbar.MouseEnter += new MouseEventHandler(HorizontalScrollbar_MouseEnter);
            HorizontalScrollbar.Minimum = 0;
            HorizontalScrollbar.Maximum = 100;
            HorizontalScrollbar.SmallChange = 1;
            HorizontalScrollbar.LargeChange = 5;
            HorizontalScrollbar.Value = 50;
            HorizontalScrollbar.ViewportSize = 80;
            #endregion
        }

        void VerticalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            var topIndex = e.NewValue;
            if (topIndex > 0)
                Panel.TopNodeIndex = (int)topIndex;
            else
                Panel.TopNodeIndex = 0;

        }

        void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange == 0) return;
            var topIndex = e.VerticalOffset;
            if (topIndex > 0)
                Panel.TopNodeIndex = (int)topIndex;
            else
                Panel.TopNodeIndex = 0;
            VerticalScrollbar.Value = e.VerticalOffset;
        }

        #endregion

        #region Private functions
        private void BindNodes()
        {
            if (Util.IsDesignMode) return;
            Dependencies.Clear();
            foreach (IGanttNode node in Nodes)
            {
                foreach (IGanttItem item in node.Items)
                    foreach (var d in item.Successors)
                        Dependencies.Add(d);
            }
            if (Panel != null)
                Panel.Nodes = Nodes;
            Nodes.CollectionChanged += new NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);
        }

        void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace
                || e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (IGanttNode node in e.NewItems)
                {
                    foreach (var n in node.Items)
                    {
                        n.Successors.CollectionChanged += Successors_CollectionChanged;
                        foreach (var d in n.Successors)
                            if (!Dependencies.Contains(d))//可能在创建NewItems时，就已添加了Dependencies了，所以要判断是否已存在。（创建动作在添加之前）
                                Dependencies.Add(d);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace
                || e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (IGanttNode node in e.OldItems)
                {
                    foreach (var n in node.Items)
                    {
                        n.Successors.CollectionChanged -= Successors_CollectionChanged;
                        foreach (var d in n.Successors)
                            Dependencies.Remove(d);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Dependencies.Clear();
                foreach (IGanttNode node in Nodes)
                {
                    foreach (var n in node.Items)
                        foreach (var d in n.Successors)
                            Dependencies.Add(d);
                }
            }
        }

        void Successors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace
                   || e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (GanttDependency d in e.NewItems)
                    Dependencies.Add(d);
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace
                || e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (GanttDependency d in e.OldItems)
                    Dependencies.Remove(d);
            }
        }
        #endregion

        #region Control event handlers
        private void GanttChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Panel != null)
            {
                Panel.ValidateRowCount();
            }
        }
        private void HorizontalScrollbar_MouseEnter(object sender, MouseEventArgs e)
        {
            _StartScrollingTime = TimespanHeader.CurrentTime.AddDays(50 - HorizontalScrollbar.Value);
        }
        private void HorizontalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            CurrentTime = _StartScrollingTime.AddDays(e.NewValue - 50);
            //CurrentTime = TimespanHeader.CurrentTime;
            if (e.NewValue == HorizontalScrollbar.Minimum)
            {
                _StartScrollingTime = TimespanHeader.CurrentTime.AddDays(49);
                HorizontalScrollbar.Value = HorizontalScrollbar.Minimum + 1;
            }
            else if (e.NewValue == HorizontalScrollbar.Maximum)
            {
                _StartScrollingTime = TimespanHeader.CurrentTime.AddDays(-49);
                HorizontalScrollbar.Value = HorizontalScrollbar.Maximum - 1;
            }
        }
        private void Timespan_ZoomFactorChanged(object sender, EventArgs e)
        {
            HorizontalScrollbar.SmallChange = 1d / Zoom.Value;
            HorizontalScrollbar.LargeChange = 5d / Zoom.Value;

            Panel.InvalidateItemPositions();
        }
        private void Timespan_CurrentTimeChanged(object sender, EventArgs e)
        {
            if (!_CurrentTimeChangedLocally)
            {
                Panel.CurrentTime = TimespanHeader.CurrentTime;
            }
            else
                _CurrentTimeChangedLocally = false;

        }
        private void TimespanHeader_Loaded(object sender, RoutedEventArgs e)
        {
            //TimespanHeader.RowsPresenter.InvalidateCells();
        }
        #endregion
    }
}
