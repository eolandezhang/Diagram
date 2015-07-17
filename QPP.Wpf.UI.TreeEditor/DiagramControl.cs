using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

/*
 * 树状图控件
 */
namespace QPP.Wpf.UI.TreeEditor
{
    public class DiagramControl : ContentControl
    {
        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var designer = (DesignerCanvas)GetTemplateChild("Designer");
            if (designer != null)
            {
                DesignerCanvas = designer;
            }
            var diagramHeader = (GroupBox)GetTemplateChild("DiagramHeader");
            if (diagramHeader != null) diagramHeader.Header = DiagramHeader;
        }


        #endregion

        #region Fields & Properties

        public ObservableCollection<DesignerItem> DesignerItems { get; set; }
        /*节点元素*/
        public DesignerCanvas DesignerCanvas { get; set; }
        public bool Suppress /*阻止通知*/ { get; set; }
        public bool IsOnEditing;/*双击出现编辑框，标识编辑状态，此时回车按键按下之后，会阻止新增相邻节点命令*/
        public DiagramManager DiagramManager { get; set; }
        #endregion

        #region Dependency Property 用于数据绑定

        #region IItemSource Property 数据源

        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(
            "ItemSource", typeof(ObservableCollection<TreeItemNode>), typeof(DiagramControl),
            new FrameworkPropertyMetadata(new ObservableCollection<TreeItemNode>(), (d, e) =>
            {
                var dc = d as DiagramControl;
                if (dc == null) return;
                dc.DesignerItems = dc.GenerateDesignerItemList();
            }));

        public ObservableCollection<TreeItemNode> ItemSource
        {
            get { return (ObservableCollection<TreeItemNode>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        #endregion
        #region SelectedItems 选中项,用于向界面返回选中项
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(ObservableCollection<TreeItemNode>), typeof(DiagramControl),
            new FrameworkPropertyMetadata(new ObservableCollection<TreeItemNode>()));

        public ObservableCollection<TreeItemNode> SelectedItems
        {
            get { return (ObservableCollection<TreeItemNode>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        #endregion
        #region ZoomBoxControlProperty 缩放控件，以后需要修改

        public static readonly DependencyProperty ZoomBoxControlProperty = DependencyProperty.Register(
            "ZoomBoxControl", typeof(ZoomBoxControl), typeof(DiagramControl),
            new PropertyMetadata(default(ZoomBoxControl),
                (d, e) =>
                {
                    var diagramControl = d as DiagramControl;
                    if (diagramControl != null)
                    {
                        var scrollViewer =
                            diagramControl.Template.FindName("DesignerScrollViewer", diagramControl) as ScrollViewer;
                        diagramControl.ZoomBoxControl.ScrollViewer = scrollViewer;
                    }
                    if (diagramControl != null) diagramControl.ZoomBoxControl.OnApplyTemplate();
                }));

        public ZoomBoxControl ZoomBoxControl
        {
            get { return (ZoomBoxControl)GetValue(ZoomBoxControlProperty); }
            set { SetValue(ZoomBoxControlProperty, value); }
        }

        #endregion
        #region DiagramHeaderProperty 画板区域标题

        public static readonly DependencyProperty DiagramHeaderProperty = DependencyProperty.Register(
            "DiagramHeader", typeof(string), typeof(DiagramControl), new PropertyMetadata(default(string)));

        public string DiagramHeader
        {
            get { return (string)GetValue(DiagramHeaderProperty); }
            set { SetValue(DiagramHeaderProperty, value); }
        }

        #endregion
        #region DesignerItemTemplate 节点模板

        public static readonly DependencyProperty DesignerItemTemplateProperty =
            DependencyProperty.RegisterAttached("DesignerItemTemplate", typeof(DataTemplate), typeof(DiagramControl));

        public static DataTemplate GetDesignerItemTemplate(UIElement element)
        {
            return (DataTemplate)element.GetValue(DesignerItemTemplateProperty);
        }

        public static void SetDesignerItemTemplate(UIElement element, DataTemplate value)
        {
            element.SetValue(DesignerItemTemplateProperty, value);
        }
        #endregion

        #endregion

        #region Constructors

        public DiagramControl()
        {
            DiagramManager = new DiagramManager(this);
            DesignerItems = new ObservableCollection<DesignerItem>();
            DesignerItems.CollectionChanged += (s, e) =>
            {
                if (Suppress) return;
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var items = e.NewItems.Cast<DesignerItem>().ToList();
                    if (!items.Any()) return;
                    foreach (var designerItem in items)
                    {
                        designerItem.ContextMenu = DesignerItem.GetItemContextMenu(this);
                    }
                }
            };
            Loaded += (d, e) => { Bind(); };/*界面上，如果控件未设定ItemSource属性，在后台代码中设定，则需要调用Bind()方法*/
        }

        #endregion

        #region 用数据源创建节点
        internal ObservableCollection<DesignerItem> GenerateDesignerItemList()
        {
            ObservableCollection<DesignerItem> list = new ObservableCollection<DesignerItem>();

            foreach (var t in ItemSource)
            {
                var newitem = new DesignerItem(t.Id, t.Parent == null ? "" : t.Parent.Id, t.Tag, this);
                list.Add(newitem);
                var childs = GenerateDesignerItemList(t);
                foreach (var c in childs)
                {
                    list.Add(c);
                }
            }
            return list;
        }
        internal ObservableCollection<DesignerItem> GenerateDesignerItemList(TreeItemNode node)
        {
            ObservableCollection<DesignerItem> list = new ObservableCollection<DesignerItem>();
            foreach (var t in node.Children)
            {
                list.Add(new DesignerItem(t.Id, t.Parent.Id, t.Tag, this));
                var childs = GenerateDesignerItemList(t);
                foreach (var c in childs)
                {
                    list.Add(c);
                }
            }
            return list;
        }
        #endregion

        #region 绑定
        public void Bind()
        {
            if (IsLoaded && DesignerItems.Any()) DiagramManager.Draw();
            DesignerItems.ToList().ForEach(designerItem =>
           {
               designerItem.ContextMenu = DesignerItem.GetItemContextMenu(this);
           });
        }
        #endregion
    }
}