using QPP.Command;
using QPP.Wpf.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using QPP.ComponentModel;

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
        public ObservableCollection<DiagramItem> Items { get; set; }
        public ObservableCollection<DesignerItem> DesignerItems { get; set; }
        public DesignerCanvas DesignerCanvas { get; set; }
        public bool IsOnEditing;/*双击出现编辑框，标识编辑状态，此时回车按键按下之后，会阻止新增相邻节点命令*/
        public DiagramManager DiagramManager { get; set; }

        #endregion

        #region Dependency Property 用于数据绑定
        #region IdField Property
        public static readonly DependencyProperty IdFieldProperty = DependencyProperty.Register(
            "IdField", typeof(string), typeof(DiagramControl), new PropertyMetadata(default(string)));
        public string IdField
        {
            get { return (string)GetValue(IdFieldProperty); }
            set { SetValue(IdFieldProperty, value); }
        }
        #endregion
        #region ParentIdField Property
        public static readonly DependencyProperty ParentIdFieldProperty = DependencyProperty.Register(
            "ParentIdField", typeof(string), typeof(DiagramControl), new PropertyMetadata(default(string)));
        public string ParentIdField
        {
            get { return (string)GetValue(ParentIdFieldProperty); }
            set { SetValue(ParentIdFieldProperty, value); }
        }
        #endregion
        #region TextField Property
        public static readonly DependencyProperty TextFieldProperty = DependencyProperty.Register(
            "TextField", typeof(string), typeof(DiagramControl), new PropertyMetadata(default(string)));
        public string TextField
        {
            get { return (string)GetValue(TextFieldProperty); }
            set { SetValue(TextFieldProperty, value); }
        }
        #endregion
        #region DeletedItems 存储被删除的数据

        public static readonly DependencyProperty DeletedItemsProperty = DependencyProperty.Register(
            "DeletedItems", typeof(IList), typeof(DiagramControl), new PropertyMetadata(null));

        public IList DeletedItems
        {
            get { return (IList)GetValue(DeletedItemsProperty); }
            set { SetValue(DeletedItemsProperty, value); }
        }
        #endregion
        #region ItemSource Property 数据源
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(IList), typeof(DiagramControl),
            new FrameworkPropertyMetadata(null, (d, e) =>
            {
                var dc = d as DiagramControl;
                if (dc == null) return;
                if (dc.ItemsSource == null) return;
                if (dc.Check())
                {
                    dc.DesignerItems = dc.GenerateDesignerItemList();
                }
                if (e.NewValue is INotifyCollectionChanged)
                {
                    CollectionChanged(dc, (INotifyCollectionChanged)e.NewValue);
                }
            }));
        static void CollectionChanged(DiagramControl dc, INotifyCollectionChanged items)
        {
            items.CollectionChanged += (sender, arg) =>
            {
                switch (arg.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddAction(dc, arg.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        DeleteAction(dc, arg.OldItems);
                        break;

                }

            };
        }

        public void EnsureDeletedItems()
        {
            if (DeletedItems == null)
            {
                DeletedItems = new ObservableCollection<object>();
            }
        }
        private static void DeleteAction(DiagramControl dc, IList oldItems)
        {
            if (oldItems == null || oldItems.Count == 0) return;
            dc.EnsureDeletedItems();
            foreach (var oldItem in oldItems)
            {
                var model = oldItem as DataModel;
                if (model == null) continue;

                //删除所有子节点
                var children = dc.GetChildren(oldItem);
                children.ForEach(x =>
                {
                    var a = x as DataModel;
                    if (a != null)
                    {
                        a.MarkDeleted();
                        dc.DeletedItems.Add(a);
                        dc.DiagramManager.DeleteItem(dc.GetId(a));
                    }
                });

                //删除本身
                model.MarkDeleted();
                dc.DeletedItems.Add(oldItem);
                dc.DiagramManager.DeleteItem(dc.GetId(oldItem));

                //更新展开按钮显示状态
                var pid = dc.GetPId(oldItem);
                if (pid.IsNotEmpty())
                {
                    var parentDesignerItem = dc.DiagramManager.GetDesignerItemById(pid);
                    dc.DiagramManager.GetDirectSubItemsAndUpdateExpander(parentDesignerItem);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                    {
                        //var m = dc.DiagramManager.GetTime(dc.DiagramManager.Arrange);
                        var m = dc.DiagramManager.GetTime(() => { dc.DiagramManager.DeleteArrange(parentDesignerItem); });
                        dc.AddToMessage("删除后重新布局", m);
                        dc.DiagramManager.SetSelectItem(parentDesignerItem);
                        dc.DiagramManager.Scroll(parentDesignerItem);
                    }));
                }

            }
        }
        string GetPId(object item)
        {
            var oType = item.GetType();
            var parentIdField = ParentIdField;
            var pid = oType.GetProperty(parentIdField);
            return pid.GetValue(item, null).ToString();
        }
        string GetId(object item)
        {
            var oType = item.GetType();
            var idField = IdField;
            var id = oType.GetProperty(idField);
            return id.GetValue(item, null).ToString();
        }

        List<object> GetChildren(object item)
        {
            List<object> children = new List<object>();
            foreach (var c in ItemsSource)
            {
                string pid = GetPId(c);
                string id = GetId(item);
                if (id == pid)
                {
                    children.Add(c);
                    children.AddRange(GetChildren(c));
                }
            }
            return children;
        }


        static void AddAction(DiagramControl dc, IList newItems)
        {
            if (newItems == null || newItems.Count == 0) return;
            foreach (var newItem in newItems)
            {
                var model = newItem as DataModel;
                if (model == null) continue;
                model.MarkCreated();

                var item = new DesignerItem(newItem, dc);
                dc.DesignerItems.Add(item);
                var parentid = dc.DiagramManager.GetPId(item);
                if (parentid.IsNotEmpty())
                {
                    var msg = dc.DiagramManager.GetTime(() =>
                    {
                        var parent = dc.DesignerItems.FirstOrDefault(a => a.ItemId == parentid);
                        dc.DiagramManager.DrawChild(parent, item);
                        dc.DiagramManager.SetSelectItem(item);
                    });
                    dc.AddToMessage("增加节点", msg);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                    {
                        //var m = dc.DiagramManager.GetTime(dc.DiagramManager.Arrange);
                        var m = dc.DiagramManager.GetTime(() => { dc.DiagramManager.AddNewArrange(item); }); 
                        dc.DiagramManager.Scroll(item);
                        dc.AddToMessage("新增后重新布局", m);
                    }));

                }
            }
        }

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        bool Check/*检查是否设定了id,pid,text的列名*/()
        {
            var textField = TextField.IsNotEmpty();
            var idField = IdField.IsNotEmpty();
            var parentIdField = ParentIdField.IsNotEmpty();
            if (textField && idField && parentIdField) { return true; }
            return false;
        }
        #endregion
        #region SelectedItems 选中项,用于向界面返回选中项
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(IList), typeof(DiagramControl),
            new FrameworkPropertyMetadata(null));

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        #endregion
        #region Message Property

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(DiagramControl), new PropertyMetadata(""));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
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
            Items = new ObservableCollection<DiagramItem>();
            DiagramManager = new DiagramManager(this);
            DesignerItems = new ObservableCollection<DesignerItem>();

            /*界面上，如果控件未设定ItemSource属性，在后台代码中设定，则需要调用Bind()方法*/
            Loaded += (d, e) => { Bind(); };
        }

        #endregion

        #region 用数据源创建节点

        internal ObservableCollection<DesignerItem> GenerateDesignerItemList()
        {
            ObservableCollection<DesignerItem> list = new ObservableCollection<DesignerItem>();
            foreach (var t in ItemsSource)
            {
                list.Add(new DesignerItem(t, this));
            }
            return list;
        }
        #endregion

        #region 绑定
        public void Bind()
        {
            if (IsLoaded)
            {
                if (Items.Any())
                {
                    if (ItemsSource == null)
                    {
                        SetItemsParent();
                        DesignerItems.Clear();
                        GenerateFromItems();
                    }
                    else
                    {
                        throw new Exception("在使用 ItemsSource 之前，项集Items合必须为空");
                    }
                }
                if (ItemsSource != null)
                {
                    DesignerItems = GenerateDesignerItemList();
                }

                if (DesignerItems.Any())
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                    {
                        AddToMessage("载入数据源", DiagramManager.GetTime(DiagramManager.Draw));
                    }));
                }
            }
        }
        #region SetItemsParent
        public void SetItemsParent()
        {
            foreach (var diagramItem in Items)
            {
#if DEBUG
                //diagramItem.Text += "_" + diagramItem.Id;
#endif
                SetItemsParent(diagramItem);
            }
        }
        void SetItemsParent(DiagramItem diagramItem)
        {
            foreach (var item in diagramItem.Items)
            {
#if DEBUG
                //item.Text += "_" + item.Id;
#endif
                item.PId = diagramItem.Id;
                SetItemsParent(item);
            }
        }
        #endregion
        #region GenerateFromItems
        void GenerateFromItems()
        {
            if (ItemsSource != null) return;
            if (DesignerItems.Any()) return;
            var list = new List<DesignerItem>();
            foreach (var x in Items)
            {
                list.Add(new DesignerItem(x, this));
                list.AddRange(GenerateFromItems(x));
            }
            foreach (var designerItem in list)
            {
                DesignerItems.Add(designerItem);
            }
        }

        List<DesignerItem> GenerateFromItems(DiagramItem diagramItem)
        {
            var list = new List<DesignerItem>();
            foreach (var x in diagramItem.Items)
            {
                list.Add(new DesignerItem(x, this));
                list.AddRange(GenerateFromItems(x));

            }
            return list;
        }
        #endregion
        #endregion

        #region Command

        public ICommand RefreshCommand
        {
            get { return new RelayCommand(Bind); }
        }
        #endregion

        void AddToMessage(string title, string msg)
        {

            if (Message.Length > 5000) Message = "";
            Message = "[" + DateTime.Now + "]" + title + ":" + msg + "\r\n" + Message;
        }
    }
}