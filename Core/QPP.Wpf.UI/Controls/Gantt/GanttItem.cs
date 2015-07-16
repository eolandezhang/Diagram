using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace QPP.Wpf.UI.Controls.Gantt
{
    /// <summary>
    /// This control represents a node's date range in the GanttPanel
    /// </summary>
    public class GanttItem : Control
    {
        #region Constants
        public const double HANDLE_MARGIN = 5.0;
        #endregion

        #region Dependency Properties

        public static DependencyProperty XMarginProperty;
        
        
        public static DependencyProperty ItemContentProperty;
        public static DependencyProperty ToolTipContentTemplateProperty;
        public static DependencyProperty IsDragDropEnabledProperty;
        public static DependencyProperty PercentCompleteWidthProperty;
        public static DependencyProperty GapBackgroundBrushProperty;
        public static DependencyProperty GapBorderBrushProperty;

        public static DependencyProperty IsMilestoneProperty;

        public static DependencyProperty ItemLeftTemplateProperty;
        public static DependencyProperty ItemRightTemplateProperty;
        public static DependencyProperty ItemTopTemplateProperty;
        public static DependencyProperty ItemBottomTemplateProperty;

        public Thickness XMargin
        {
            get { return (Thickness)GetValue(XMarginProperty); }
            set { SetValue(XMarginProperty, value); }
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
        public DataTemplate ItemTopTemplate
        {
            get { return (DataTemplate)GetValue(ItemTopTemplateProperty); }
            set { SetValue(ItemTopTemplateProperty, value); }
        }
        public DataTemplate ItemBottomTemplate
        {
            get { return (DataTemplate)GetValue(ItemBottomTemplateProperty); }
            set { SetValue(ItemBottomTemplateProperty, value); }
        }
        public bool IsMilestone
        {
            get { return (bool)GetValue(IsMilestoneProperty); }
            set { SetValue(IsMilestoneProperty, value); }
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
        public object ItemContent
        {
            get { return GetValue(ItemContentProperty); }
            set { SetValue(ItemContentProperty, value); }
        }
        public DataTemplate ToolTipContentTemplate
        {
            get { return (DataTemplate)GetValue(ToolTipContentTemplateProperty); }
            set { SetValue(ToolTipContentTemplateProperty, value); }
        }
        public double PercentCompleteWidth
        {
            get { return (double)GetValue(PercentCompleteWidthProperty); }
            internal set { SetValue(PercentCompleteWidthProperty, value); }
        }
        public bool IsDragDropEnabled
        {
            get { return !ParentRow.ParentPanel.IsReadOnly && (bool)GetValue(IsDragDropEnabledProperty); }
            set { SetValue(IsDragDropEnabledProperty, value); }
        }
        #endregion
        //protected internal bool IsActualMark { get; set; }
        protected internal FrameworkElement NodeElement { get; set; }
        private IGanttNode _Node;
        internal IGanttNode Node
        {
            get { return _Node; }
            set
            {
                if (_Node != null)
                    _Node.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Node_PropertyChanged);
                _Node = value;
                _Node.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Node_PropertyChanged);
            }
        }

        internal GanttRow ParentRow { get; set; }
        public DragState DragState { get; set; }
        protected internal double NodeWidth { get; set; }
        private Grid _PercentCompleteElement;

        static GanttItem()
        {
            var thisType = typeof(GanttItem);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));

            ItemContentProperty = DependencyProperty.Register("ItemContent", typeof(object), thisType, new PropertyMetadata(null));
            ToolTipContentTemplateProperty = DependencyProperty.Register("ToolTipContentTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));
            IsDragDropEnabledProperty = DependencyProperty.Register("IsDragDropEnabled", typeof(bool), thisType, new PropertyMetadata(true));
            PercentCompleteWidthProperty = DependencyProperty.Register("PercentCompleteWidth", typeof(double), thisType, new PropertyMetadata(0d));
            GapBackgroundBrushProperty = DependencyProperty.Register("GapBackgroundBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
            GapBorderBrushProperty = DependencyProperty.Register("GapBorderBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
            IsMilestoneProperty = DependencyProperty.Register("IsMilestone", typeof(bool), thisType, new PropertyMetadata(null));

            ItemLeftTemplateProperty = DependencyProperty.Register("ItemLeftTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));
            ItemRightTemplateProperty = DependencyProperty.Register("ItemRightTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));
            ItemTopTemplateProperty = DependencyProperty.Register("ItemTopTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));
            ItemBottomTemplateProperty = DependencyProperty.Register("ItemBottomTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));

            
            
            XMarginProperty = DependencyProperty.Register("XMargin", typeof(Thickness), thisType, new PropertyMetadata(new Thickness(0,3,0,3)));
        }

        public override void OnApplyTemplate()
        {
            if (_Node.IsMilestone == true && IsMilestone == false)
            {
                IsMilestone = true;
            }
            //if (_Node.Resources.Length > 0)
            //{
            //    string temp = _Node.Resources;
            //    _Node.Resources = "";
            //    _Node.Resources = temp;
            //}
            base.OnApplyTemplate();
            _PercentCompleteElement = (Grid)GetTemplateChild("PercentCompleteElement");

            NodeElement = (FrameworkElement)GetTemplateChild("NodeElement");

            if (DragState == DragState.None)
                VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //double nodeStartPosition = ParentRow.ParentPanel.ConvertDateToPosition(Node.StartDate);
            //double nodeStartPosition = ParentRow.ParentPanel.ConvertDateToPosition(Node.StartDate.Date);
            //double nodeEndPosition = ParentRow.ParentPanel.ConvertDateToPosition(Node.EndDate.AddDays(1));
            //double nodeEndPosition = ParentRow.ParentPanel.ConvertDateToPosition(Node.EndDate
            //    .Date.AddHours(23).AddMinutes(59).AddSeconds(59));
            double nodeStartPosition = ParentRow.ParentPanel.ConvertDateToPosition(Node.StartDate);
            double nodeEndPosition = ParentRow.ParentPanel.ConvertDateToPosition(Node.EndDate);


            NodeWidth = Math.Max(5, (nodeEndPosition - nodeStartPosition));
            if (NodeWidth > 0)
            //if (NodeWidth > 0 && !IsActualMark)
                PercentCompleteWidth = (Node.PercentComplete / 100) * NodeWidth;
            else
                PercentCompleteWidth = 0;
            return base.MeasureOverride(availableSize);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (DragState == DragState.None)
                VisualStateManager.GoToState(this, "MouseOver", true);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (DragState == DragState.None)
                VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //if (IsActualMark) return;
            base.OnMouseLeftButtonDown(e);
            if (ParentRow.ParentPanel.IsReadOnly) return;
            if (IsDragDropEnabled)
            {
                Point p = e.GetPosition(this);
                if (p.X > NodeWidth - HANDLE_MARGIN && p.X < NodeWidth + HANDLE_MARGIN)
                {
                    DragState = DragState.ResizeRight;
                }
                else if (p.X < NodeWidth)
                {
                    DragState = DragState.Whole;
                }
                VisualStateManager.GoToState(this, "Normal", true);

                ParentRow.ItemsPresenter.ItemShadow.StartDate = Node.StartDate;
                ParentRow.ItemsPresenter.ItemShadow.EndDate = Node.EndDate;
                ParentRow.ItemsPresenter.ItemShadow.Visibility = System.Windows.Visibility.Visible;
            }
            ParentRow.ParentPanel.RowPresenter.Predecessor = this;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            //if (IsActualMark) return;
            base.OnMouseLeftButtonUp(e);
            if (ParentRow.ParentPanel.IsReadOnly) return;
            var item = ParentRow.ParentPanel.RowPresenter.Predecessor;
            if (item != null && item != this)
            {
                if (!item.Node.Successors.Any(p => p.Successor == Node && p.Predecessor == item.Node))
                    item.Node.Successors.Add(new GanttDependency()
                    {
                        Predecessor = item.Node,
                        Type = DependencyType.FS,//TODO: Other type support
                        Successor = Node
                    });
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DragState == DragState.None && IsDragDropEnabled)
            {
                //根据位置改变鼠标光标形状
                Point p = e.GetPosition(this);
                if (p.X > NodeWidth - HANDLE_MARGIN && p.X < NodeWidth + HANDLE_MARGIN)
                    this.Cursor = Cursors.SizeWE;
                else if (p.X < NodeWidth)
                    this.Cursor = Cursors.SizeAll;
                else
                    this.Cursor = Cursors.Arrow;
            }
        }

        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ParentRow.Invalidate();
            if (e.PropertyName == "StartDate" || e.PropertyName == "EndDate")
                ParentRow.ParentPanel.RaiseItemChanged(new GanttItemEventArgs(this));

            if (e.PropertyName == "PercentComplete")
            {
                if (_PercentCompleteElement == null)
                    return;

                PercentCompleteWidth = (Node.PercentComplete / 100) * NodeWidth;

                _PercentCompleteElement.Width = PercentCompleteWidth;
                if (_PercentCompleteElement.Width == 0) 
                    _PercentCompleteElement.Visibility = System.Windows.Visibility.Collapsed;
                else
                    _PercentCompleteElement.Visibility = System.Windows.Visibility.Visible;
            }
            if (e.PropertyName == "IsMilestone")
            {
                /// <summary>
                /// 如果是历程碑的话，将长方形结点的透明度设置为0，菱形的透明度为1
                /// 如果是历程碑的话，将长方形结点的透明度设置为1，菱形的透明度为0
                ///</summary>
                
                IsMilestone = (sender as IGanttNode).IsMilestone;
            }
        }
    }
}
