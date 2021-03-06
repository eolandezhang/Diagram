﻿using QPP.Wpf.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace QPP.Wpf.UI.TreeEditor
{

    public class DiagramManager
    {
        readonly DiagramControl _diagramControl;
        public DiagramManager(DiagramControl diagramControl)
        {
            _diagramControl = diagramControl;
        }

        #region Setters
        private const double LEFT_OFFSET = 25;
        private const double MIN_ITEM_WIDTH = 200d;
        private const double FONT_SIZE = 12d;
        private static readonly SolidColorBrush SHADOW_FONT_COLOR_BRUSH = Brushes.Gray;
        private static readonly SolidColorBrush DEFAULT_FONT_COLOR_BRUSH = Brushes.Black;
        private const string PARENT_CONNECTOR = "Bottom";
        private const string CHILD_CONNECTOR = "Left";
        #endregion
        List<DesignerItem> DesignerItems
        {
            get
            {
                return _diagramControl.DesignerItems.ToList();
            }
        }
        DesignerCanvas DesignerCanvas
        {
            get
            {
                return
       _diagramControl.DesignerCanvas;
            }
        }
        #region Get
        public string GetPId(DesignerItem item)
        {
            var oType = item.DataContext.GetType();
            var parentIdField = _diagramControl.ParentIdField;
            var pid = oType.GetProperty(parentIdField);
            return pid.GetValue(item.DataContext, null).ToString();
        }
        public string GetId(DesignerItem item)
        {
            var oType = item.DataContext.GetType();
            var idField = _diagramControl.IdField;
            var id = oType.GetProperty(idField);
            return id.GetValue(item.DataContext, null).ToString();
        }
        public void SetPId(DesignerItem designerItem)
        {
            var oType = designerItem.DataContext.GetType();
            var parentIdField = _diagramControl.ParentIdField;
            var pid = oType.GetProperty(parentIdField);
            pid.SetValue(designerItem.DataContext, designerItem.ItemParentId, null);
        }


        Connector GetItemConnector/*根据名称，取得元素连接点*/(DesignerItem item, string name)
        {
            //item.UpdateLayout();
            var itemConnectorDecorator = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
            if (itemConnectorDecorator == null) return null;
            var itemConnector = itemConnectorDecorator.Template.FindName(name, itemConnectorDecorator) as Connector;
            return itemConnector;
        }
        List<Connector> GetItemConnectors/*取得所有连接点*/(DesignerItem designerItem)
        {
            var connectors = new List<Connector>();

            var leftItemConnector = GetItemConnector(designerItem, "Left");
            if (leftItemConnector != null) connectors.Add(leftItemConnector);

            var bottomItemConnector = GetItemConnector(designerItem, "Bottom");
            if (bottomItemConnector != null) connectors.Add(bottomItemConnector);

            var topItemConnector = GetItemConnector(designerItem, "Top");
            if (topItemConnector != null) connectors.Add(topItemConnector);

            var rightItemConnector = GetItemConnector(designerItem, "Right");
            if (rightItemConnector != null) connectors.Add(rightItemConnector);
            return connectors;
        }
        List<Connection> GetItemConnections/*取得所有连线*/(DesignerItem designerItem)
        {
            var connections = new List<Connection>();
            var list = GetItemConnectors(designerItem);
            if (list.Count == 0) return connections;
            foreach (var c in list.Select(connector => connector.Connections.Where(x => x.Source != null && x.Sink != null)).Where(c => c.Any()))
            {
                connections.AddRange(c);
            }
            return connections;
        }
        //public List<DesignerItem> GetDirectSubItemsAndUpdateExpander/*取得直接子节点*/(DesignerItem item)
        //{
        //    var list = item.ChildrenDesignerItems;
        //    UpdateExpander(item);
        //    return list;
        //}
        public List<DesignerItem> GetAllSubItems/*取得直接及间接的子节点*/(DesignerItem item/*某个节点*/)
        {
            var result = new List<DesignerItem>();
            var child = new List<DesignerItem>();
            var list = item.ChildrenDesignerItems.OrderBy(x => x.Top);
            foreach (var subItem in list)
            {
                child.Add(subItem);
                result.Add(subItem);
                foreach (var designerItem in child)
                {
                    result.AddRange(GetAllSubItems(designerItem));
                }
            }
            return result;
        }
        public DesignerItem GetDesignerItemById(string id)
        {
            return DesignerItems.FirstOrDefault(x => GetId(x) == id);
        }
        DesignerItem GetParent(DesignerItem designerItem)
        {
            return DesignerItems.FirstOrDefault(y => y.ItemId == designerItem.ItemParentId);
        }
        List<DesignerItem> GetRootItems()
        {
            return DesignerItems.Where(x => String.IsNullOrEmpty(x.ItemParentId)).OrderBy(x => x.Top).ToList();
        }
        DesignerItem GetSelectedItem()
        {
            var selectedItems = _diagramControl.DesignerCanvas.SelectionService.CurrentSelection;
            if (selectedItems == null || selectedItems.Count != 1) return null;
            return selectedItems.ConvertAll(x => x as DesignerItem).FirstOrDefault();
        }
        #endregion

        #region Set

        public void SetSelectItem(DesignerItem designerItem)
        {
            if (designerItem == null) return;
            _diagramControl.DesignerCanvas.SelectionService.ClearSelection();
            _diagramControl.DesignerCanvas.SelectionService.SelectItem(designerItem);
            if (_diagramControl.SelectedItems != null)
            {
                _diagramControl.SelectedItems.Clear();
                _diagramControl.SelectedDesignerItems.Clear();
                _diagramControl.SelectedItems.Add(designerItem.DataContext);
                _diagramControl.SelectedDesignerItems.Add(designerItem);
            }
            _diagramControl.Focus();
            _diagramControl.CanExpandAndCollapseSelectedItem = true;
        }


        #endregion

        #region Draw
        public void Draw()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                _diagramControl.AddToMessage("载入数据源", GetTime(() =>
                {
                    ClearCanvas();
                    if (CheckDesignerItemsIsNullOrEmpty()) return;
                    var roots = GetRootItems();
                    roots.ForEach(root =>
                    {
                        DrawRoot(root);
                        if (roots.All(x => Math.Abs(Canvas.GetTop(x)) < 1 && Math.Abs(Canvas.GetLeft(x)) < 1))
                        {
                            var newLeft = 400 * roots.IndexOf(root);
                            root.Top = double.IsNaN(Canvas.GetTop(root)) ? 0 : Canvas.GetTop(root);
                            root.OriginalTop = root.Top;
                            root.Left = newLeft;
                            root.OriginalLeft = newLeft;
                            Canvas.SetTop(root, 0);
                            Canvas.SetLeft(root, newLeft);
                        }
                        else
                        {
                            root.Top = double.IsNaN(Canvas.GetTop(root)) ? 0 : Canvas.GetTop(root);
                            root.OriginalTop = root.Top;
                            root.Left = Canvas.GetLeft(root);
                            root.OriginalLeft = root.Left;
                            Canvas.SetTop(root, 0);
                            Canvas.SetLeft(root, 0);
                        }

                        DrawDesignerItems(root);

                    });
                    SelectFirstRoot();
                }));
            }));

        }

        public List<DesignerItem> DrawDesignerItems(DesignerItem designerItem)
        {
            var list = new List<DesignerItem>();
            if (designerItem == null) return list;
            designerItem.UpdateLayout();
            designerItem.SetTemplate();
            var childs = designerItem.ChildrenDesignerItems.OrderBy(x => x.Top);
            foreach (var childItem in childs)
            {
                DrawChild(designerItem, childItem, list);
                list.AddRange(DrawDesignerItems(childItem));
            }
            return list;
        }
        public void DrawRoot(DesignerItem item)
        {
            DrawDesignerItem(item);
            item.CanCollapsed = false;
            item.IsExpanderVisible = false;

        }
        public void DrawChild(DesignerItem designerItem, DesignerItem childItem, List<DesignerItem> list)
        {
            if (designerItem == null) return;
            DrawDesignerItem(childItem);/*创建子节点*/
            childItem.UpdateLayout();
            childItem.SetTemplate();
            var top = Canvas.GetTop(designerItem) + designerItem.ActualHeight + list.Sum(x => x.ActualHeight);
            list.Add(childItem);
            var left = Canvas.GetLeft(designerItem) + GetOffset(designerItem);
            Canvas.SetTop(childItem, top);
            Canvas.SetLeft(childItem, left);
            SavePosition(childItem);
            childItem.UpdateLayout();
            var source = GetItemConnector(designerItem, PARENT_CONNECTOR);
            var sink = GetItemConnector(childItem, "Left");
            if (source == null || sink == null) return;
            #region 创建连线

            var conn = new Connection(source, sink); /*创建连线*/
            DesignerCanvas.Children.Add(conn); /*放到画布上*/
            Panel.SetZIndex(conn, -10000);

            #endregion
            childItem.CanCollapsed = true;
            childItem.UpdateExpander();
        }
        void DrawDesignerItem(DesignerItem item)
        {
            if (item.DataContext == null) return;
            GenerateDesignerItemContent(item, DEFAULT_FONT_COLOR_BRUSH);
            if (!DesignerCanvas.Children.Contains(item))
            {
                DesignerCanvas.Children.Add(item);
                var menu = DesignerItem.GetItemContextMenu(_diagramControl);
                menu.DataContext = _diagramControl.DataContext;
                item.ContextMenu = menu;
            }
            SetWidth(item);
        }
        void GenerateDesignerItemContent(DesignerItem item, SolidColorBrush fontColorBrush)
        {
            if (item == null) return;
            var textblock = new TextBlock()
            {
                IsHitTestVisible = false,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5, 2, 5, 2),
                FontFamily = new FontFamily("Arial"),
                FontSize = FONT_SIZE,
                Foreground = fontColorBrush,
                DataContext = item.DataContext
            };
            textblock.SetBinding(TextBlock.TextProperty, new Binding("Text"));
            item.Content = textblock;
        }
        bool CheckDesignerItemsIsNullOrEmpty()
        {
            return DesignerItems == null
                || !DesignerItems.Any();
        }
        void ClearCanvas() { DesignerCanvas.Children.Clear(); }
        void SelectFirstRoot()
        {
            var firstRoot = GetRootItems().FirstOrDefault();
            if (firstRoot != null) { SetSelectItem(firstRoot); }
        }
        #endregion

        #region Arrange
        public void DeleteArrange(DesignerItem delItem)
        {
            //var m = GetTime(() =>
            //{
            var root = GetRoot(delItem);
            var list = GetAllSubItems(delItem);
            list.Add(delItem);
            double h = list.Sum(designerItem => designerItem.ActualHeight);

            var allSub = list.ToList();
            if (allSub.Any())
            {
                var lastChild = allSub.Aggregate((a, b) => Canvas.GetTop(a) > Canvas.GetTop(b) ? a : b);

                var items =
                    DesignerItems.Where(
                        x => Canvas.GetTop(x) > Canvas.GetTop(lastChild) && !x.Equals(delItem) && GetRoot(x).Equals(root));
                foreach (var designerItem in items)
                {
                    Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) - h);
                }
            }
            else
            {
                var items = DesignerItems
                .Where(x => Canvas.GetTop(x) > Canvas.GetTop(delItem) && !x.Equals(delItem) && GetRoot(x).Equals(root));
                foreach (var designerItem in items)
                {
                    Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) - delItem.ActualHeight);
                }
            }


            //});
            SavePosition();
            //_diagramControl.AddToMessage("删除后重新布局", m);
        }
        void CollapseArrange(DesignerItem designerItem)
        {
            var allSubItems = GetAllExpandedSubItems(designerItem);
            double h = 0d;
            allSubItems.ForEach(x => { h += x.ActualHeight; });
            var subItems = GetAllSubItems(designerItem);
            var last = subItems.Any() ? subItems.Aggregate((a, b) =>
              a.Top > b.Top ? a : b) : designerItem;
            var list = DesignerItems.Where(x =>
            x.Top > last.Top).ToList();
            foreach (var item in list)
            {
                Canvas.SetTop(item, Canvas.GetTop(item) - h);
            }
        }
        void ExpandArrange(DesignerItem designerItem)
        {
            var allCollapsedSubItems = GetAllCollapsedSubItems(designerItem);
            if (!allCollapsedSubItems.Any()) return;
            double h = 0d;
            foreach (var allSubItem in allCollapsedSubItems)
            {
                allSubItem.UpdateLayout();
                h += allSubItem.ActualHeight;
            }
            var subItems = GetAllSubItems(designerItem);
            if (subItems.Any())
            {
                var last = subItems.Aggregate((a, b) => a.Top > b.Top ? a : b);
                var list = DesignerItems.Where(x =>
                x.Top > last.Top).ToList();
                foreach (var item in list)
                {
                    Canvas.SetTop(item, Canvas.GetTop(item) + h);
                }
            }
        }
        public void Arrange()
        {

            var m = GetTime(() =>
            {
                var items = DesignerItems.ToList();
                var roots = items.Where(x => string.IsNullOrEmpty(x.ItemParentId)).ToList();
                foreach (var root in roots)
                {
                    var top = double.IsNaN(Canvas.GetTop(root)) ? 0 : Canvas.GetTop(root);
                    var left = double.IsNaN(Canvas.GetLeft(root)) ? 0 : Canvas.GetLeft(root);
                    if (roots.All(x => Math.Abs(top) < 1 && Math.Abs(left) < 1))
                    {
                        var newLeft = 400 * roots.IndexOf(root);
                        root.Top = top;
                        root.OriginalTop = root.Top;
                        root.Left = newLeft;
                        root.OriginalLeft = newLeft;
                        Canvas.SetLeft(root, newLeft);
                    }
                    else
                    {
                        root.Top = top;
                        root.OriginalTop = root.Top;
                        root.Left = left;
                        root.OriginalLeft = root.Left;
                        Canvas.SetLeft(root, left);
                        Canvas.SetTop(root, top);
                    }
                    Arrange(root);
                }
            });
            _diagramControl.AddToMessage("全部重新布局", m);

        }
        List<DesignerItem> Arrange(DesignerItem designerItem)
        {
            List<DesignerItem> list = new List<DesignerItem>();
            if (designerItem == null) return list;
            var directSubItems = designerItem.ChildrenDesignerItems;
            designerItem.UpdateExpander();
            if (directSubItems == null || directSubItems.Count == 0) return list;
            var subItems = directSubItems
                .OrderBy(x => Canvas.GetTop(x)).ToList();
            foreach (var sub in subItems)
            {
                var top = double.IsNaN(Canvas.GetTop(designerItem)) ? 0 : Canvas.GetTop(designerItem) + designerItem.ActualHeight + list.Sum(x => x.ActualHeight);
                list.Add(sub);
                var left = Canvas.GetLeft(designerItem) + GetOffset(designerItem);
                Canvas.SetTop(sub, top);
                Canvas.SetLeft(sub, left);
                SavePosition(sub);
                list.AddRange(Arrange(sub));/*递归*/
            }
            return list;
        }
        void SavePosition(DesignerItem item)
        {
            item.Left = Canvas.GetLeft(item);
            item.Top = Canvas.GetTop(item);
            item.OriginalLeft = Canvas.GetLeft(item);
            item.OriginalTop = Canvas.GetTop(item);
        }
        public void SavePosition()
        {
            foreach (var item in DesignerItems)
            {
                SavePosition(item);
            }
        }
        public void SetWidth(DesignerItem designerItem)
        {
            double imgWidth = 0d;
            if (designerItem.ItemStyle != null)
            { imgWidth = 16d * designerItem.ItemStyle.ImageUrl.Count(); }
            designerItem.Width = GetWidth(designerItem) + imgWidth;
        }
        double GetWidth(DesignerItem designerItem)
        {
            if (designerItem.DataContext != null)
            {
                var tb = GetTextBlock(designerItem);
                if (tb != null)
                {
                    var width = GetWidth(tb.Text);
                    return width < MIN_ITEM_WIDTH ? MIN_ITEM_WIDTH : width;
                }
                return MIN_ITEM_WIDTH;
            }
            return MIN_ITEM_WIDTH;
        }

        double GetWidth(string text)
        {
            if (text.IsNotEmpty())
            {
                FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Arial"), FONT_SIZE, Brushes.Black);
                double width = formattedText.Width + 20 + 16 + 10;
                //double height = formattedText.Height;
                return width;
            }
            return MIN_ITEM_WIDTH;
        }
        double GetOffset(FrameworkElement item)
        {
            //item.UpdateLayout();
            //return item.ActualWidth.Equals(0) ? 30 : (item.ActualWidth * 0.1 + LEFT_OFFSET);
            return LEFT_OFFSET;
        }
        //public void UpdateExpander(DesignerItem parent)
        //{
        //    if (parent.HasChild)
        //    {
        //        if (parent.CanCollapsed == false)
        //        {
        //            parent.IsExpanderVisible = false;
        //        }
        //        else if (parent.HasChild)
        //        {
        //            parent.IsExpanderVisible = true;
        //        }
        //    }
        //    else
        //    {
        //        parent.IsExpanderVisible = false;
        //    }
        //}
        #endregion

        #region Style
        TextBlock GetTextBlock(DesignerItem item)
        {
            return item.Content as TextBlock;
        }
        void BringToFront(UIElement element)
        {
            List<UIElement> childrenSorted =
                (from UIElement item in DesignerCanvas.Children
                 orderby Panel.GetZIndex(item as UIElement) ascending
                 select item as UIElement).ToList();

            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (element.Equals(item))
                {
                    int idx = Panel.GetZIndex(item);
                    Panel.SetZIndex(item, childrenSorted.Count - 1 + j++);
                }
                else
                {
                    Panel.SetZIndex(item, i++);
                }
            }
        }
        #endregion

        #region Expand & Collapse
        bool IsParentCollapsed(DesignerItem designerItem, DesignerItem p)
        {
            //是否有父节点是折叠的
            var parent = GetParent(designerItem);
            if (parent != null)
            {
                //父节点均展开，则返回false
                if (parent.Equals(p)) return false;
                //有一个未展开，则返回true,表示不显示
                if (!parent.IsExpanded) { return true; }
                return IsParentCollapsed(parent, p);
            }
            return true;
        }
        bool IsParentExpandeded(DesignerItem designerItem, DesignerItem p)
        {
            //所有有父节点是展开的
            var parent = GetParent(designerItem);
            if (parent != null)
            {
                //父节点均展开，则返回false
                if (parent.Equals(p)) return false;
                //有一个未展开，则返回false,表示不显示
                if (!parent.IsExpanded) { return false; }
                return IsParentExpandeded(parent, p);
            }
            return true;
        }
        //所有展开的
        List<DesignerItem> GetAllExpandedSubItems(DesignerItem designerItem)
        {
            List<DesignerItem> list = new List<DesignerItem>();
            var allSubItems = GetAllSubItems(designerItem);
            foreach (var allSubItem in allSubItems)
            {
                var parent = GetParent(allSubItem);
                if (parent != null)
                {
                    if (!IsParentCollapsed(allSubItem, designerItem)) { list.Add(allSubItem); }
                }
            }
            return list;
        }
        //所有折叠的
        List<DesignerItem> GetAllCollapsedSubItems(DesignerItem designerItem)
        {
            List<DesignerItem> list = new List<DesignerItem>();
            var allSubItems = GetAllSubItems(designerItem);
            foreach (var allSubItem in allSubItems)
            {
                var parent = GetParent(allSubItem);
                if (parent != null)
                {
                    if (!IsParentExpandeded(allSubItem, designerItem))
                    {
                        list.Add(allSubItem);
                    }
                }
            }
            return list;
        }
        void Expand(DesignerItem item)
        {
            item.Suppress = true;
            item.IsExpanded = true;
            item.Suppress = false;
            var child = item.ChildrenDesignerItems;
            foreach (var c in child)
            {
                if (!c.IsExpanded)
                {
                    c.Suppress = true;
                    c.IsExpanded = true;
                    c.Suppress = false;
                    Show(c);
                }
                Expand(c);
            }
        }
        public void ExpandAll()
        {
            var root = DesignerItems.Where(x => x.ItemParentId.IsNullOrEmpty()).ToList();
            foreach (var r in root)
            {
                Expand(r);
            }
        }
        void Collapse(DesignerItem item)
        {
            var child = item.ChildrenDesignerItems;
            foreach (var c in child)
            {
                Collapse(c);
                if (c.CanCollapsed)
                {
                    c.Suppress = true;
                    c.IsExpanded = false;
                    c.Suppress = false;
                }
                Hide(c);
            }
            item.Suppress = true;
            item.IsExpanded = false;
            item.Suppress = false;
        }
        public void CollapseAll()
        {
            var root = DesignerItems.Where(x => x.ItemParentId.IsNullOrEmpty()).ToList();
            foreach (var r in root)
            {
                Collapse(r);
            }
        }

        void SetVisibility(IEnumerable<Connection> connections, DesignerItem designerItem, Visibility visibility)
        {
            foreach (var connection in connections)
            {
                connection.Visibility = visibility;
            }
            designerItem.Visibility = visibility;
        }
        void Hide(DesignerItem item)
        {
            var childs = GetAllExpandedSubItems(item);
            if (childs.Count == 0) return;
            CollapseArrange(item);
            childs.ForEach(designerItem =>
            {
                var connections = GetItemConnections(designerItem);
                SetVisibility(connections, designerItem, Visibility.Collapsed);
            });
        }
        void Show(DesignerItem item)
        {
            var childs = GetAllExpandedSubItems(item);
            if (childs.Count == 0) return;
            childs.ForEach(designerItem =>
            {
                var connections = GetItemConnections(designerItem).Where(x => x.Source.ParentDesignerItem.IsExpanded);
                SetVisibility(connections, designerItem, Visibility.Visible);
            });
            ExpandArrange(item);
        }
        public void HideOrExpandChildItems(DesignerItem item)
        {
            if (!item.IsExpanded) { Hide(item); }
            else if (item.IsExpanded) { Show(item); }
        }
        public void ExpandSelectedItem()
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems == null) return;
            selectedItems
                .Where(x => x.CanCollapsed)
                .ToList()
                .ForEach(selectedItem => { selectedItem.IsExpanded = true; });
        }
        public void CollapseSelectedItem()
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems == null) return;
            selectedItems
                .Where(x => x.CanCollapsed)
                .ToList()
                .ForEach(selectedItem => { selectedItem.IsExpanded = false; });
        }
        #endregion

        #region Drag

        #region ChangeParent
        public DesignerItem ChangeParent(Point position, DesignerItem designerItem, List<DesignerItem> selectedItemsAllSubItems)
        {
            var newParent = GetNewParent(position, designerItem, selectedItemsAllSubItems);
            if (newParent != null)
            {
                DesignerItems.Where(x => x.IsNewParent).ToList().ForEach(x => { x.IsNewParent = false; });
                newParent.IsNewParent = true;

            }

            return newParent;
        }
        public List<DesignerItem> GetSelectedItemsAndAllSubItems()
        {
            var selectedItemsAllSubItems = new List<DesignerItem>();
            var selectedItems = GetSelectedItems();
            var roots = selectedItems.Where(x => selectedItems.All(y => y.ItemId != x.ItemParentId));
            foreach (var r in roots)
            {
                selectedItemsAllSubItems.AddRange(GetAllSubItems(r));
            }
            //取得所有子节点，让parent不能为子节点
            selectedItemsAllSubItems.AddRange(roots);
            return selectedItemsAllSubItems;
        }
        public void CreateHelperConnection(DesignerItem newParent, DesignerItem dragItem)
        {
            RemoveHelperConnection();
            if (newParent == null) return;
            var source = GetItemConnector(newParent, PARENT_CONNECTOR);
            var sink = GetItemConnector(dragItem, CHILD_CONNECTOR);
            var connection = new Connection(source, sink);
            connection.toNewParent = true;
            source.Connections.Add(connection);
            DesignerCanvas.Children.Add(connection);
            SetConnectionColor(connection, Brushes.Red);
            BringToFront(connection);
        }
        public void MoveUpAndDown(DesignerItem parent, DesignerItem selectedItem, DesignerItem root)
        {
            if (parent == null) return;

            var itemTop = Canvas.GetTop(selectedItem);
            var designerItemsOnCanvas = DesignerCanvas.Children.OfType<DesignerItem>().Where(x => !x.IsShadow).ToList();
            var downItems = designerItemsOnCanvas.Where(x =>
                x.OriginalTop > itemTop //- selectedItem.ActualHeight
                && x.ItemId != selectedItem.ItemId
                && x.ItemParentId.IsNotEmpty()
                && GetRoot(x).Equals(root)
                ).ToList(); /*比元素大的，全部向下移*/
            foreach (var designerItem in downItems)
            {
                Canvas.SetTop(designerItem, designerItem.OriginalTop + selectedItem.ActualHeight);
            }
            var upItems = designerItemsOnCanvas.Where(x =>
                x.OriginalTop < itemTop
                && x.OriginalTop > Canvas.GetTop(parent)
                && x.ItemId != selectedItem.ItemId
                && GetRoot(x).Equals(root)
                ).ToList(); /*比父节点大的，比元素小的，恢复位置*/
            foreach (var designerItem in upItems)
            {
                Canvas.SetTop(designerItem, designerItem.OriginalTop);
                var item = designerItem.IsShadow ? designerItem.ShadowOrignal : designerItem;
                var x1 = GetAllSubItems(item);
                if (x1 == null || !x1.Any()) continue;
                var list = designerItemsOnCanvas.Where(x =>
                    x.OriginalTop <= x1.Aggregate((a, b) => a.OriginalTop > b.OriginalTop ? a : b).OriginalTop
                    && x.ItemId != selectedItem.ItemId
                    ).ToList();
                list.ForEach(x => { Canvas.SetTop(x, x.OriginalTop); });
            }
        }
        public void AfterChangeParent(DesignerItem designerItem, DesignerItem newParent, Point newPosition, List<DesignerItem> selectedItemsAllSubItems)
        {
            ChangeParent(newParent, designerItem);
            RemoveHelperConnection();
            Canvas.SetTop(designerItem, newPosition.Y);
            Canvas.SetLeft(designerItem, newPosition.X);
            DesignerItems.ForEach(x => { x.IsNewParent = false; });
            ConnectToNewParent(newParent);
            var items = selectedItemsAllSubItems.Where(x => x.ItemParentId.IsNullOrEmpty()).ToList();
            if (newParent == null)
            {
                foreach (var item in items)
                {
                    Canvas.SetLeft(item, newPosition.X);
                    Canvas.SetTop(item, newPosition.Y - (designerItem.OriginalTop - item.OriginalTop));
                }
                var topBelowZero =
                    items.Where(item => newPosition.Y - (designerItem.OriginalTop - item.OriginalTop) < 0).ToList();
                if (topBelowZero.Any())
                {
                    var minTopItem = topBelowZero.Aggregate((a, b) => Canvas.GetTop(a) < Canvas.GetTop(b) ? a : b);
                    var v = Math.Abs(Canvas.GetTop(minTopItem));
                    items.ForEach(x => { Canvas.SetTop(x, Canvas.GetTop(x) + v); });
                }
                var leftBelowZero = items.Where(item => newPosition.X < 0).ToList();
                if (leftBelowZero.Any())
                {
                    var minLeftItem = topBelowZero.Aggregate((a, b) => Canvas.GetLeft(a) < Canvas.GetLeft(b) ? a : b);
                    var v = Math.Abs(Canvas.GetLeft(minLeftItem));
                    items.ForEach(x => { Canvas.SetLeft(x, Canvas.GetTop(x) + v); });
                }
            }
            Arrange();
        }
        void ChangeParent(DesignerItem newParent, DesignerItem designerItem)
        {
            if (newParent != null)
            {
                if (designerItem.ParentDesignerItem != null)
                {
                    designerItem.ParentDesignerItem.ChildrenDesignerItems.Remove(designerItem);
                }
                designerItem.UpdateExpander();
                designerItem.ParentDesignerItem = newParent;
                newParent.ChildrenDesignerItems.Add(designerItem);
            }
        }
        void ShowId/*测试父id是否正确设置*/(DesignerItem newParent, DesignerItem item)
        {
            if (newParent != null)
            {
                //var item = DataContext as DesignerItem;

                if (item != null)
                {
                    if (_diagramControl.Items.Any())
                    {
                        var dc = item.DataContext as DiagramItem;
                        if (dc != null)
                        {
                            MessageBox.Show(item.ItemId + "[" + item.ItemParentId + "]" + "\r\n" + dc.Id + "[" + dc.PId + "]");
                        }
                    }
                    else
                    {
                        MessageBox.Show(item.ItemId + "[" + item.ItemParentId + "]" + "\r\n" + GetId(item) + "[" + GetPId(item) + "]");
                    }
                }
            }
        }
        #endregion

        #region Item finder
        public DesignerItem GetNewParent/*取得元素上方最接近的元素*/(Point position, DesignerItem itemToMove, IList<DesignerItem> selectedItemsAllSubItems)
        {
            var pre = DesignerItems.Where(x => x.Visibility.Equals(Visibility.Visible));
            var list = (from designerItem in pre
                        let parentTop = Canvas.GetTop(designerItem) + designerItem.ActualHeight - 13
                        let parentLeft = Canvas.GetLeft(designerItem) + designerItem.ActualWidth * 0.1
                        let parentRight = parentLeft + designerItem.ActualWidth
                        where position.Y >= parentTop /*top位置小于自己的top位置*/
                              && position.X >= parentLeft
                              && position.X <= parentRight
                              && !Equals(designerItem, itemToMove) /*让parent不能为自己*/
                              && !selectedItemsAllSubItems.Contains(designerItem) /*让parent不能为子节点*/
                              && designerItem.IsShadow == false
                        select designerItem).ToList();
            if (!list.Any()) return null;
            var parent = list.Aggregate((a, b) => a.Top > b.Top ? a : b);
            return parent;
        }

        public DesignerItem GetRoot(DesignerItem designerItem)
        {
            if (designerItem == null) return null;
            if (designerItem.ItemParentId.IsNullOrEmpty()) { return designerItem; }
            var parent = DesignerItems.FirstOrDefault(x => x.ItemId == designerItem.ItemParentId);
            return GetRoot(parent);
        }
        #endregion

        #region Connection operations
        void ConnectToNewParent/*根据取得的newParent,改变特定item的连线*/(DesignerItem newParent)
        {
            if (_diagramControl.SingleRoot && newParent == null) return;
            var items = GetSelectedItems();
            var itemsToChangeParent = items.Where(a => items.All(y => y.ItemId != a.ItemParentId));/*在选中的集合a中，只改变“父节点不在集合a中的”节点*/
            foreach (var designerItem in itemsToChangeParent)
            {
                if (designerItem.ParentDesignerItem != null)
                {
                    designerItem.ParentDesignerItem.ChildrenDesignerItems.Remove(designerItem);
                }
                designerItem.ParentDesignerItem = newParent;
                designerItem.UpdateExpander();
                if (newParent != null)
                { newParent.ChildrenDesignerItems.Add(designerItem); }
                designerItem.ItemParentId = newParent == null ? "" : newParent.ItemId;
                if (_diagramControl.Items.Any())//用items初始化的
                {
                    var x = designerItem.DataContext as DiagramItem;
                    if (x != null)
                    {
                        x.PId = designerItem.ItemParentId;
                    }
                }
                else //用ItemsSource初始化的
                {
                    SetPId(designerItem);
                }
                var connections = GetItemConnections(designerItem).Where(x => Equals(x.Sink.ParentDesignerItem, designerItem)).ToList();
                if (connections.Any())//有连线
                {
                    connections.ForEach(connection =>
                    {
                        if (newParent != null)/*有新父节点，则改变连线到新父节点*/ { connection.Source = GetItemConnector(newParent, PARENT_CONNECTOR); }
                        else/*没有新父节点，则移除连线*/ { RemoveConnection(connection); }
                    });
                }
                else if /*没有连线，但是有新父节点*/ (newParent != null)
                {
                    CreateConnection(newParent, designerItem);
                }
            }

        }
        void CreateConnection(DesignerItem newParent, DesignerItem designerItem)
        {
            var source = GetItemConnector(newParent, PARENT_CONNECTOR);
            var sink = GetItemConnector(designerItem, CHILD_CONNECTOR);
            var connection = new Connection(source, sink);
            source.Connections.Add(connection);
            DesignerCanvas.Children.Add(connection);
        }
        void RemoveConnection(Connection connection)
        {
            if (connection.Source != null) connection.Source.Connections.Remove(connection);
            if (connection.Sink != null) connection.Sink.Connections.Remove(connection);
            connection.Source = null;
            connection.Sink = null;
            DesignerCanvas.Children.Remove(connection);
        }
        public void RemoveHelperConnection/*移除找parent的辅助红线*/()
        {
            var connections = GetConnections().Where(x => x.toNewParent).ToList();
            if (connections.Any())//如果有连线，则将连线链接到parent
            {
                connections.ForEach(RemoveConnection);
            }
        }
        void SetConnectionColor/*设定连线颜色*/(Connection connection, Brush colorBrushes)
        {
            _diagramControl.UpdateLayout();
            var path = connection.Template.FindName("PART_ConnectionPath", connection) as Path;
            if (path != null)
            {
                path.Stroke = colorBrushes;
                //path.StrokeThickness = 2;
            }
        }
        #endregion

        #region Selected items operations
        void HideOthers/*隐藏了除了drag item以外的selected items*/(DesignerItem selectedItem)
        {
            var selectedItems = GetSelectedItems();
            selectedItem.IsExpanderVisible = false;
            foreach (var designerItem in selectedItems.Where(x => x.ItemId != selectedItem.ItemId))
            {
                designerItem.Visibility = Visibility.Hidden;
            }
        }
        void ShowOthers/*恢复显示选中元素,之前调用了HideOthers隐藏了除了drag item以外的selected items*/()
        {
            var selectedItems = DesignerItems;//GetSelectedItems();
            foreach (var selectedItem in selectedItems)
            {
                selectedItem.Visibility = Visibility.Visible;
                if (DesignerItems.Any(x => x.ItemParentId == selectedItem.ItemId))/*如果有子节点*/
                {
                    selectedItem.IsExpanderVisible = selectedItem.CanCollapsed;/*可折叠，则显示折叠/展开按钮*/
                }
            }
        }
        #endregion

        #region Canvas items operations
        List<Connection> GetConnections()
        {
            List<Connection> list = new List<Connection>();
            var itemCount = VisualTreeHelper.GetChildrenCount(DesignerCanvas);
            if (itemCount == 0) return list;
            for (int n = 0; n < itemCount; n++)
            {
                var c = VisualTreeHelper.GetChild(DesignerCanvas, n);
                var child = c as Connection;
                if (child != null) list.Add(child);
            }
            return list;
        }
        public DesignerItem CreateItemShadow/*拖动时创建的影子*/(DesignerItem item)
        {
            var shadow = new DesignerItem(_diagramControl)
            {
                IsShadow = true,
                ShadowOrignal = item,
                DataContext = item.DataContext,
                OriginalLeft = item.OriginalLeft,
                OriginalTop = item.OriginalTop,
                Left = item.Left,
                Top = item.Top,
                Width = item.Width,
                ItemStyle = item.ItemStyle
            };
            Canvas.SetLeft(shadow, item.OriginalLeft);
            Canvas.SetTop(shadow, item.OriginalTop);
            GenerateDesignerItemContent(shadow, DEFAULT_FONT_COLOR_BRUSH);
            return shadow;
        }
        #endregion

        #endregion

        #region Edit,Delete,Cut,Copy,Paste
        #region Edit
        public void Edit(DesignerItem item, string key = "")
        {
            var oldHeight = item.ActualHeight;
            var oldValue = item.Text;

            _diagramControl.IsOnEditing = true;
            var textBox = new TextBox
            {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                MinHeight = 24,
                DataContext = item.DataContext
            };
            textBox.SetBinding(TextBox.TextProperty, new Binding(_diagramControl.TextField) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

            Canvas.SetLeft(textBox, Canvas.GetLeft(item));
            Canvas.SetTop(textBox, Canvas.GetTop(item));
            DesignerCanvas.Children.Add(textBox);
            BringToFront(textBox);


            var t = textBox;
            if (key.IsNotEmpty())
            {
                //t.Focus();
                t.Text = key;
                t.SelectionStart = t.Text.Length;
                t.Focus();
            }
            else
            {
                t.SelectAll();
                t.Focus();
            }

            EditorKeyBindings(t);
            t.MinWidth = MIN_ITEM_WIDTH;
            t.LostFocus += (sender, e) =>
            {
                T_LostFocus(sender, item, oldHeight);
                item.Width = GetWidth(item);
            };
            t.TextChanged += (sender, e) => { t.Width = GetWidth(item); };
            t.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    t.Text = oldValue;
                    T_LostFocus(t, item, oldHeight);
                    //t.Width = GetWidth(item);
                }
            };
        }
        void T_LostFocus(object sender, DesignerItem item, double oldHeight)
        {
            var textBox = sender as TextBox;
            DesignerCanvas.Children.Remove(textBox);
            //Arrange();
            var list = _diagramControl.DesignerItems.Where(x => Canvas.GetTop(x) > Canvas.GetTop(item)).ToList();
            item.UpdateLayout();
            var height = item.ActualHeight;
            var h = height - oldHeight;
            list.ForEach(x => Canvas.SetTop(x, Canvas.GetTop(x) + h));
            _diagramControl.IsOnEditing = false;
            GlobalInputBindingManager.Default.Recover();
            _diagramControl.Focus();
        }
        public void Edit(string key = "")
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems == null || selectedItems.Count != 1) return;
            var selectedItem = selectedItems.FirstOrDefault();
            Edit(selectedItem, key);
        }
        void EditorKeyBindings(TextBox t)
        {
            GlobalInputBindingManager.Default.Clear();

            var kbCtrlEnter = new KeyBinding
            {
                Key = Key.Enter,
                Modifiers = ModifierKeys.Control,
                Command = new RelayCommand(() =>
                {
                    var begin = t.SelectionStart;
                    if (begin > 0)
                    {
                        t.Text = t.Text.Insert(begin, Environment.NewLine);
                        t.SelectionStart = begin + Environment.NewLine.Length;
                    }
                    else
                    {
                        t.Text += Environment.NewLine; t.SelectionStart = t.Text.Length;
                    }

                })
            };
            t.InputBindings.Add(kbCtrlEnter);

            var kbEnter = new KeyBinding
            {
                Key = Key.Enter,
                Command = new RelayCommand(() =>
                {
                    if (_diagramControl.IsOnEditing)
                    {
                        DesignerCanvas.Children.Remove(t);
                        //Arrange();
                        _diagramControl.IsOnEditing = false;
                        _diagramControl.Focus();
                    }
                })
            };
            t.InputBindings.Add(kbEnter);
        }
        #endregion
        #region Delete
        public void DeleteItem(string id)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                var item = GetDesignerItemById(id);
                if (item == null) return;
                var parent = item.ParentDesignerItem;
                if (parent != null)
                {
                    if (parent.ChildrenDesignerItems != null)
                    { parent.ChildrenDesignerItems.Remove(item); }
                }

                item.UpdateExpander();
                item.ParentDesignerItem = null;
                var connections = GetItemConnections(item).ToList();
                connections.ForEach(x => { DesignerCanvas.Children.Remove(x); });
                var connectors = GetItemConnectors(item);
                connectors.ForEach(x => { x.Connections.Clear(); });
                DesignerCanvas.Children.Remove(item);
                _diagramControl.DeletedDesignerItems.Add(item);
                _diagramControl.DesignerItems.Remove(item);
            }));

        }
        #endregion
        #endregion

        #region Select Operations
        public void SelectUpDown(bool selectUp)
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems == null || selectedItems.Count != 1)
            {
                return;
            }
            var selectedItem = selectedItems.FirstOrDefault();
            if (selectedItem == null) { return; }
            //var siblingDesignerItems = _diagramControl.DesignerItems.Where(x => x.ItemParentId == selectedItem.ItemParentId).ToList();//第一种方案使用
            DesignerItem selectedDesignerItem;
            if (selectUp)
            {
                selectedDesignerItem = UpSiblingDesignerItem(selectedItem);
            }
            else //move down
            {
                selectedDesignerItem = DownSiblingDesignerItem(selectedItem);
            }
            if (selectedDesignerItem != null) SetSelectItem(selectedDesignerItem);
            Scroll(selectedDesignerItem);
        }

        #region Up
        //第一种方案
        DesignerItem UpDesignerItem(DesignerItem selectedItem, List<DesignerItem> siblingDesignerItems)
        {
            DesignerItem selectedDesignerItem = null;
            var top = siblingDesignerItems
                .Where(x => x.Top < selectedItem.Top).ToList();
            if (top.Any())
            {
                selectedDesignerItem =
                    top.Aggregate((a, b) => a.Top > b.Top ? a : b);
            }
            else
            {
                var parent =
                    DesignerItems.Where(x => x.ItemId == selectedItem.ItemParentId).ToList();
                if (parent.Count() == 1)
                    selectedDesignerItem = parent.FirstOrDefault();
            }
            return selectedDesignerItem;
        }
        //第二种方案，直接取上方相邻节点
        DesignerItem UpSiblingDesignerItem(DesignerItem selectedItem)
        {
            var items = DesignerItems.Where(x => Canvas.GetTop(x) < Canvas.GetTop(selectedItem) && GetRoot(x).Equals(GetRoot(selectedItem)));
            if (items.Any())
            {
                var item = items.Aggregate((a, b) => Canvas.GetTop(a) > Canvas.GetTop(b) ? a : b);
                return item;
            }
            return null;
        }
        #endregion
        #region Down
        //第一种方案
        DesignerItem DownDesignerItem(DesignerItem selectedItem, List<DesignerItem> siblingDesignerItems)
        {
            DesignerItem selectedDesignerItem = null;
            var down = siblingDesignerItems
                .Where(x => x.Top > selectedItem.Top).ToList();
            if (down.Any()) //1.优先找相邻节点，处于下方的节点
            {
                selectedDesignerItem = down.Aggregate((a, b) => a.Top < b.Top ? a : b);
            }
            else //没有处于下方的相邻节点，2.有父亲节点，则找其父亲的，处于下方的相邻节点，3.如果没有父亲节点，就找子节点
            {
                var parents =
                   DesignerItems.Where(x => x.ItemId == selectedItem.ItemParentId).ToList();
                if (parents.Count() == 1 && parents.FirstOrDefault() != null) //有父节点，父节点邻居，处于下方的节点
                {
                    var parent = parents.FirstOrDefault();
                    if (parent != null)
                    {
                        var parentSibling =
                            DesignerItems.Where(
                                x => x.ItemParentId == parent.ItemParentId
                                     && x.Top > parent.Top).ToList();
                        if (parentSibling.Any())
                        {
                            selectedDesignerItem =
                                parentSibling.Aggregate((a, b) => a.Top > b.Top ? a : b);
                        }
                    }
                }
                if (selectedDesignerItem == null)//没有父节点，找子节点
                {
                    var child = DesignerItems.Where(x => x.ItemParentId == selectedItem.ItemId).ToList();
                    if (child.Any())
                    {
                        selectedDesignerItem = child.Aggregate((a, b) => a.Top < b.Top ? a : b);
                    }
                }
            }
            return selectedDesignerItem;
        }
        //第二种方案，直接取下方相邻节点
        DesignerItem DownSiblingDesignerItem(DesignerItem selectedItem)
        {
            var items = DesignerItems.Where(x => Canvas.GetTop(x) > Canvas.GetTop(selectedItem) && GetRoot(x).Equals(GetRoot(selectedItem)));
            if (items.Any())
            {
                var item = items.Aggregate((a, b) => Canvas.GetTop(a) < Canvas.GetTop(b) ? a : b);
                return item;
            }
            return null;
        }
        #endregion

        public void SelectRightLeft(bool selectRight)
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems == null || selectedItems.Count != 1) { return; }
            var selectedItem = selectedItems.FirstOrDefault();

            DesignerItem selectedDesignerItem = null;
            if (selectedItem != null)
            {
                if (selectRight)
                {
                    var child = DesignerItems.Where(x => x.ItemParentId == selectedItem.ItemId).ToList();
                    if (child.Any())
                    {
                        selectedDesignerItem = child.Aggregate((a, b) => a.Top < b.Top ? a : b);
                    }
                }
                else
                {
                    var parent = DesignerItems.Where(x => x.ItemId == selectedItem.ItemParentId).ToList();
                    if (parent.Any())
                    {
                        selectedDesignerItem = parent.Aggregate((a, b) => a.Top > b.Top ? a : b);
                    }
                }
            }
            if (selectedDesignerItem != null) SetSelectItem(selectedDesignerItem);
        }
        List<DesignerItem> GetSelectedItems()
        {
            return
                DesignerCanvas
                .SelectionService.CurrentSelection
                .ConvertAll(x => x as DesignerItem);
        }
        public void Scroll(DesignerItem designerItem)
        {
            if (designerItem == null) return;
            var sv = (ScrollViewer)_diagramControl.Template.FindName("DesignerScrollViewer", _diagramControl);
            var top = Canvas.GetTop(designerItem);
            var left = Canvas.GetLeft(designerItem);

            var x = sv.HorizontalOffset + sv.ViewportWidth - designerItem.ActualWidth;
            var y = sv.VerticalOffset + sv.ViewportHeight - designerItem.ActualHeight;

            if (left > x) { sv.ScrollToHorizontalOffset(Double.IsNaN(left) ? 100 : left); }
            if (top > y) { sv.ScrollToVerticalOffset(Double.IsNaN(top) ? 100 : top); }


        }

        #endregion

        public string GetTime(Action action)
        {
            Stopwatch t = new Stopwatch();
            t.Start();
            action.Invoke();
            t.Stop();
            TimeSpan timespan = t.Elapsed; //  获取当前实例测量得出的总时间
                                           //double hours = timespan.TotalHours; // 总小时
                                           //double minutes = timespan.TotalMinutes;  // 总分钟
                                           //double seconds = timespan.TotalSeconds;  //  总秒数
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数
                                                               //return seconds + "秒" + ",共" + _diagramControl.DesignerItems.Count + "个";
            return milliseconds + "  毫秒  " + "   ,[共" + DesignerItems.Count + "个]";
        }
    }
}