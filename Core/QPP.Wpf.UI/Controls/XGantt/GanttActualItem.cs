using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QPP.Wpf.UI.Controls.XGantt
{
    public class GanttActualItem : Control
    {
        #region 依赖属性
        public static DependencyProperty IsFinishedProperty;
        public static DependencyProperty ItemContentProperty;
        public static DependencyProperty ToolTipContentTemplateProperty;
        #endregion

        #region 属性、字段
        public bool IsFinished
        {
            get { return (bool)GetValue(IsFinishedProperty); }
            set { SetValue(IsFinishedProperty, value); }
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
        #endregion

        #region event handlers
        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "PercentComplete" && _Node.PercentComplete == 100)
            //    IsFinished = true;
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            VisualStateManager.GoToState(this, "MouseOver", true);
        }
        #endregion
        
        static GanttActualItem()
        {
            var thisType = typeof(GanttActualItem);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
            IsFinishedProperty = DependencyProperty.Register("IsFinished", typeof(bool), thisType, new PropertyMetadata(false));
            ItemContentProperty = DependencyProperty.Register("ItemContent", typeof(object), thisType, new PropertyMetadata(null));
            ToolTipContentTemplateProperty = DependencyProperty.Register("ToolTipContentTemplate", typeof(DataTemplate), thisType, new PropertyMetadata(null));
        }
    }
}
