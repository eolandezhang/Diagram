using QPP.ComponentModel;
using QPP.Wpf.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Markup;

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
        public IDiagramManager DiagramManager { get; set; }
        public List<DesignerItem> DeletedDesignerItems = new List<DesignerItem>();
        #endregion

        #region Dependency Property
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
        #region Left Field,Top Field Property
        public static readonly DependencyProperty LeftFieldProperty = DependencyProperty.Register(
            "LeftField", typeof(string), typeof(DiagramControl), new PropertyMetadata("Left"));
        public string LeftField
        {
            get { return (string)GetValue(LeftFieldProperty); }
            set { SetValue(LeftFieldProperty, value); }
        }
        public static readonly DependencyProperty TopFieldProperty = DependencyProperty.Register(
            "TopField", typeof(string), typeof(DiagramControl), new PropertyMetadata("Top"));
        public string TopField
        {
            get { return (string)GetValue(TopFieldProperty); }
            set { SetValue(TopFieldProperty, value); }
        }
        #endregion
        //#region ImageUri Field Property

        //public static readonly DependencyProperty ImageUriProperty = DependencyProperty.Register(
        //    "ImageUri", typeof(string), typeof(DiagramControl), new PropertyMetadata("ImageUri"));

        //public string ImageUri
        //{
        //    get { return (string)GetValue(ImageUriProperty); }
        //    set { SetValue(ImageUriProperty, value); }
        //}
        //#endregion
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
                    ItemsSourceCollectionChanged(dc, (INotifyCollectionChanged)e.NewValue);
                }
            }));
        static void ItemsSourceCollectionChanged(DiagramControl dc, INotifyCollectionChanged items)
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
                    case NotifyCollectionChangedAction.Reset:
                        dc.AddToMessage("Reset", "");
                        dc.DesignerCanvas.Children.Clear();
                        dc.DesignerItems.Clear();
                        dc.DeletedDesignerItems.Clear();
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

            #region 先取得所有选中节点的子节点，再判断被删除的节点是否在内，是则不删除

            var items = dc.DiagramManager.GetSelectedItemsAllSubItems();
            List<DesignerItem> allSubItems = items.Where(designerItem => !items.Contains(designerItem)).ToList();

            #endregion

            foreach (var oldItem in oldItems)
            {
                var id = dc.GetId(oldItem);
                if (allSubItems.Any(x => x.ItemId == id))
                {
                    continue;
                }
                var deleteItem = dc.DiagramManager.GetDesignerItemById(dc.GetId(oldItem));
                if (deleteItem == null) continue;
                dc.DiagramManager.DeleteArrange(deleteItem);
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
                //更新折叠按钮显示状态，选中节点，滚动到节点
                var pid = dc.GetPId(oldItem);
                if (pid.IsNotEmpty())
                {
                    var parentDesignerItem = dc.DiagramManager.GetDesignerItemById(pid);
                    var subitems = dc.DiagramManager.GetDirectSubItemsAndUpdateExpander(parentDesignerItem);//更新展开按钮显示状态
                    #region 选中上方相邻节点
                    var topItems = subitems.Where(x => Canvas.GetTop(x) < Canvas.GetTop(deleteItem));
                    var item = topItems.Any() ? topItems.Aggregate((a, b) => Canvas.GetTop(a) > Canvas.GetTop(b) ? a : b) : parentDesignerItem;
                    dc.DiagramManager.SetSelectItem(item);
                    #endregion
                    dc.DiagramManager.Scroll(item);
                }
            }
            //dc.DiagramManager.Arrange();
            dc.DiagramManager.SavePosition();
        }
        string GetPId(object item)
        {
            var oType = item.GetType();
            var parentIdField = ParentIdField;
            var pid = oType.GetProperty(parentIdField);
            var pidValue = pid.GetValue(item, null);
            return pidValue == null ? "" : pidValue.ToString();
        }
        string GetId(object item)
        {
            var oType = item.GetType();
            var idField = IdField;
            var id = oType.GetProperty(idField);
            return id.GetValue(item, null).ToString();
        }
        string GetText(object item)
        {
            var oType = item.GetType();
            var textField = TextField;
            var id = oType.GetProperty(textField);
            return id.GetValue(item, null).ToString();
        }
        double GetLeft(object item)
        {
            var oType = item.GetType();
            var leftField = LeftField;
            var left = oType.GetProperty(leftField);
            return (double)left.GetValue(item, null);
        }
        double GetTop(object item)
        {
            var oType = item.GetType();
            var topField = TopField;
            var top = oType.GetProperty(topField);
            return (double)top.GetValue(item, null);
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
                item.SetTemplate();
                item.Top = double.MaxValue;
                var left = dc.GetLeft(newItem);
                var top = dc.GetTop(newItem);

                dc.DesignerItems.Add(item);
                var parentid = dc.GetPId(newItem);// dc.DiagramManager.GetPId(item);
                if (parentid.IsNotEmpty())
                {
                    //dc.AddToMessage("增加节点", dc.DiagramManager.GetTime(() =>
                    //{
                    var parent = dc.DesignerItems.FirstOrDefault(a => a.ItemId == parentid);
                    dc.DiagramManager.DrawChild(parent, item);
                    dc.DiagramManager.SetSelectItem(item);
                    //}));
                }
                else
                {
                    //dc.AddToMessage("增加节点", dc.DiagramManager.GetTime(() =>
                    //   {
                    dc.DiagramManager.DrawRoot(item, top, left);
                    dc.DiagramManager.SetSelectItem(item);

                    //  }));
                }
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                {
                    //dc.DiagramManager.ExpandAll();
                    dc.DiagramManager.AddNewArrange(item);
                    dc.DiagramManager.Scroll(item);
                }));
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
            new FrameworkPropertyMetadata(null, (d, e) =>
            {
                //var dc = d as DiagramControl;
                //if (dc == null) return;
                //SelectedItemsCollectionChanged(dc, (INotifyCollectionChanged)e.NewValue);
            }));

        private static void SelectedItemsCollectionChanged(DiagramControl dc, INotifyCollectionChanged items)
        {
            items.CollectionChanged += (sender, arg) =>
            {
                switch (arg.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            var newItems = arg.NewItems;
                            foreach (var newItem in newItems)
                            {
                                var id = dc.GetId(newItem);
                                var text = dc.GetText(newItem);
                                dc.AddToMessage("选中", "id:" + id + "," + "text:" + text);
                            }
                        }
                        break;
                }
            };
        }



        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
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
        #region Message Property

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(DiagramControl), new PropertyMetadata(""));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        #endregion

        #region Canvas Double Click Command Property

        public static readonly DependencyProperty CanvasDoubleClickCommandProperty = DependencyProperty.Register(
            "CanvasDoubleClickCommand", typeof(ICommand), typeof(DiagramControl), new PropertyMetadata(null));

        public ICommand CanvasDoubleClickCommand
        {
            get { return (ICommand)GetValue(CanvasDoubleClickCommandProperty); }
            set { SetValue(CanvasDoubleClickCommandProperty, value); }
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
        #region CanExpandAndCollapseSelected ItemProperty 展开折叠选中项菜单是否可用
        public static readonly DependencyProperty CanExpandAndCollapseSelectedItemProperty = DependencyProperty.Register(
            "CanExpandAndCollapseSelectedItem", typeof(bool), typeof(DiagramControl), new PropertyMetadata(default(bool)));

        public bool CanExpandAndCollapseSelectedItem
        {
            get { return (bool)GetValue(CanExpandAndCollapseSelectedItemProperty); }
            set { SetValue(CanExpandAndCollapseSelectedItemProperty, value); }
        }
        #endregion
        #region SingleRoot Property

        public static readonly DependencyProperty SingleRootProperty = DependencyProperty.Register(
            "SingleRoot", typeof(bool), typeof(DiagramControl), new PropertyMetadata(true));

        public bool SingleRoot
        {
            get { return (bool)GetValue(SingleRootProperty); }
            set { SetValue(SingleRootProperty, value); }
        }
        #endregion

        #endregion

        #region Constructors
        DispatcherTimer timer = new DispatcherTimer();
        public DiagramControl()
        {
            DiagramManager = new VerticalTreeManager(this);
            Items = new ObservableCollection<DiagramItem>();
            DesignerItems = new ObservableCollection<DesignerItem>();
            /*界面上，如果控件未设定ItemSource属性，在后台代码中设定，则需要调用Bind()方法*/
            Loaded += (d, e) => { Bind(); };
            PreviewKeyDown += DiagramControl_PreviewKeyDown;

            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);   //设置刷新的间隔时间
            //timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ShowMemory.Execute(null);
        }

        #endregion

        #region 按键
        void DiagramControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //重点：为了不让父元素响应键盘，要将e.Handled设定为true;
            //否则上下左右键，会让其它元素获得焦点
            if (!IsOnEditing)
            {
                if (Keyboard.Modifiers == ModifierKeys.None && ((int)e.Key >= 34 && (int)e.Key <= 69 || (int)e.Key >= 74 && (int)e.Key <= 83))
                {
                    DiagramManager.Edit(); e.Handled = true;
                }
                //if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
                //{
                   
                    
                //}
                switch (e.Key)
                {
                    case Key.Up: { DiagramManager.SelectUpDown(true); e.Handled = true; } break;
                    case Key.Down: { DiagramManager.SelectUpDown(false); e.Handled = true; } break;
                    case Key.Left: { DiagramManager.SelectRightLeft(false); e.Handled = true; } break;
                    case Key.Right: { DiagramManager.SelectRightLeft(true); e.Handled = true; } break;
                    case Key.F2: { DiagramManager.Edit(); e.Handled = true; } break;
                    case Key.Divide: { DiagramManager.CollapseAll(); e.Handled = true; } break;
                    case Key.Multiply: { DiagramManager.ExpandAll(); e.Handled = true; } break;
                    case Key.Add: { DiagramManager.ExpandSelectedItem(); e.Handled = true; } break;
                    case Key.Subtract: { DiagramManager.CollapseSelectedItem(); e.Handled = true; } break;
                }
            }

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
                        DiagramManager.Draw();

                    }));
                }
                else
                {
                    DesignerCanvas.Children.Clear();
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
        public ICommand EditSelectedItemCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        DiagramManager.Edit();
                    });
            }
        }
        public ICommand RefreshCommand
        {
            get { return new RelayCommand(Bind); }
        }
        public ICommand CollapseAllCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        DiagramManager.CollapseAll();
                    });
            }
        }
        public ICommand ExpandAllCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DiagramManager.ExpandAll();
                });
            }
        }
        public ICommand ExpandSelectedItemCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DiagramManager.ExpandSelectedItem();
                }, CanExpandAndCollapseSelectedItemCommand);
            }
        }
        public bool CanExpandAndCollapseSelectedItemCommand()
        {
            if (DesignerCanvas == null) return false;
            if (DesignerCanvas.SelectionService == null) return false;
            var s = DesignerCanvas.SelectionService.CurrentSelection;
            if (s != null && s.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ICommand CollapseSelectedItemCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DiagramManager.CollapseSelectedItem();
                }, CanExpandAndCollapseSelectedItemCommand);
            }
        }

        public ICommand ClearMessage
        {
            get { return new RelayCommand(() => { Message = ""; }); }
        }
        public ICommand ShowMemory
        {
            get
            {
                return new RelayCommand(() =>
          {
              Process proc = Process.GetCurrentProcess();

              long usedMemory = proc.PrivateMemorySize64 / 1024;//单位Byte 比特，任务管理器的单位是
              AddToMessage("内存", usedMemory.ToString("N"));
          });
            }
        }
        #endregion
        Regex regexNewline = new Regex(Environment.NewLine);
        #region 消息
        public void AddToMessage(string title, string msg)
        {
            MatchCollection mc = regexNewline.Matches(Message);
            if (mc.Count > 20)
            {
                var start = Message.LastIndexOf(Environment.NewLine, StringComparison.CurrentCultureIgnoreCase);
                if (start != -1)
                {
                    Message = Message.Remove(start);
                }
            }
            Message = "[" + DateTime.Now + "]" + title + ":" + msg + Environment.NewLine + Message;
        }
        #endregion
    }
}