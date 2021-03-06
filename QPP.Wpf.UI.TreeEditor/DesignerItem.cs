﻿using QPP.Wpf.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace QPP.Wpf.UI.TreeEditor
{
    //These attributes identify the types of the named parts that are used for templating
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable, IGroupable//, ICloneable
    {
        #region Fields

        public bool Suppress = false;
        #region 位置
        public double OriginalLeft;/*记录拖拽前的位置*/
        public double OriginalTop;
        #endregion
        public DesignerItem ShadowOrignal;/*当此节点为shadow时，记录shadow的原节点*/
        public DiagramControl DiagramControl;
        #endregion

        #region Property

        public DesignerItem ParentDesignerItem { get; set; }
        private List<DesignerItem> _childDesignerItems;
        public List<DesignerItem> ChildrenDesignerItems
        {
            get
            {
                if (_childDesignerItems == null)
                {
                    _childDesignerItems = new List<DesignerItem>();
                }
                return _childDesignerItems;
            }
        }
        //当样式变化，需要更新DataContext的ItemsStyle字段
        public ItemStyle ItemStyle { get; set; }
        ImageUrl _SelectedImage;
        public ImageUrl SelectedImage
        {
            get { return _SelectedImage; }
            set
            {
                if (_SelectedImage == value) return;
                _SelectedImage = value;
            }
        }
        #region ItemId Property
        public static readonly DependencyProperty ItemIdProperty = DependencyProperty.Register(
        "ItemId", typeof(string), typeof(DesignerItem), new PropertyMetadata(default(string)));

        public string ItemId
        {
            get { return (string)GetValue(ItemIdProperty); }
            set { SetValue(ItemIdProperty, value); }
        }
        #endregion
        #region ItemParentId Property
        public static readonly DependencyProperty ItemParentIdProperty = DependencyProperty.Register(
            "ItemParentId", typeof(string), typeof(DesignerItem), new PropertyMetadata(default(string)));
        public string ItemParentId
        {
            get { return (string)GetValue(ItemParentIdProperty); }
            set { SetValue(ItemParentIdProperty, value); }
        }
        #endregion
        #region Text Property
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(DesignerItem), new PropertyMetadata(""));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion
        #region Left , Top Property

        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
            "Left", typeof(double), typeof(DesignerItem), new PropertyMetadata(0d));

        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
            "Top", typeof(double), typeof(DesignerItem), new PropertyMetadata(0d));

        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }
        #endregion

        #region IsSelected Property 被选中的
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(DesignerItem),
                                       new FrameworkPropertyMetadata(false));
        #endregion
        #region IsExpanded Property 是否展开

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded", typeof(bool), typeof(DesignerItem),
            new PropertyMetadata(true, (d, e) =>
            {
                var designerItem = d as DesignerItem;
                if (designerItem != null)
                {
                    if (designerItem.Suppress) return;
                    var canvas = designerItem.Parent as DesignerCanvas;
                    if (canvas != null)
                    {
                        var diagramControl = canvas.TemplatedParent as DiagramControl;
                        if (diagramControl != null)
                            diagramControl.Manager.HideOrExpandChildItems(designerItem);
                    }
                }
            }));


        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        #endregion
        #region IsExpanderVisible Property 折叠按钮是否显示

        public static readonly DependencyProperty IsExpanderVisibleProperty = DependencyProperty.Register(
            "IsExpanderVisible", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

        public bool IsExpanderVisible
        {
            get { return (bool)GetValue(IsExpanderVisibleProperty); }
            set { SetValue(IsExpanderVisibleProperty, value); }
        }

        #endregion
        #region CanCollapsed Property 是否可以折叠

        public static readonly DependencyProperty CanCollapsedProperty = DependencyProperty.Register(
            "CanCollapsed", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(true));

        public bool CanCollapsed
        {
            get { return (bool)GetValue(CanCollapsedProperty); }
            set { SetValue(CanCollapsedProperty, value); }
        }

        #endregion
        #region IsNewParent Property 是否是新父节点

        public static readonly DependencyProperty IsNewParentProperty = DependencyProperty.Register(
            "IsNewParent", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

        public bool IsNewParent
        {
            get { return (bool)GetValue(IsNewParentProperty); }
            set { SetValue(IsNewParentProperty, value); }
        }

        #endregion
        #region ItemContextMenu Property 右键菜单
        public static readonly DependencyProperty ItemContextMenuProperty =
            DependencyProperty.RegisterAttached("ItemContextMenu", typeof(ContextMenu), typeof(DesignerItem));
        public static ContextMenu GetItemContextMenu(UIElement element)
        {
            return (ContextMenu)element.GetValue(ItemContextMenuProperty);
        }
        public static void SetItemContextMenu(UIElement element, ContextMenu value)
        {
            element.SetValue(ItemContextMenuProperty, value);
        }
        #endregion
        #region IsShadow Property 标识是否是拖拽阴影

        public static readonly DependencyProperty IsShadowProperty = DependencyProperty.Register(
            "IsShadow", typeof(bool), typeof(DesignerItem), new PropertyMetadata(false));

        public bool IsShadow
        {
            get { return (bool)GetValue(IsShadowProperty); }
            set { SetValue(IsShadowProperty, value); }
        }

        #endregion
        #region IsDragItemChild 拖拽元素的子元素，灰色边框样式

        public static readonly DependencyProperty IsDragItemChildProperty = DependencyProperty.Register(
            "IsDragItemChild", typeof(bool), typeof(DesignerItem), new PropertyMetadata(default(bool)));

        public bool IsDragItemChild
        {
            get { return (bool)GetValue(IsDragItemChildProperty); }
            set { SetValue(IsDragItemChildProperty, value); }
        }
        #endregion
        #region 树状图，不使用的属性
        #region ParentID Property 分组时用的，并不是表示父节点ID

        public string ParentID
        {
            get { return (string)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(string), typeof(DesignerItem));
        #endregion
        #region IsGroup Property 分组
        public bool IsGroup
        {
            get { return (bool)GetValue(IsGroupProperty); }
            set { SetValue(IsGroupProperty, value); }
        }
        public static readonly DependencyProperty IsGroupProperty =
            DependencyProperty.Register("IsGroup", typeof(bool), typeof(DesignerItem));
        #endregion
        #region DragThumbTemplate Property 拖拽模板

        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }

        #endregion
        #region ConnectorDecoratorTemplate Property 连接点模板

        // can be used to replace the default template for the ConnectorDecorator
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetConnectorDecoratorTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(ConnectorDecoratorTemplateProperty);
        }

        public static void SetConnectorDecoratorTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(ConnectorDecoratorTemplateProperty, value);
        }

        #endregion
        #region IsDragConnectionOver Property

        // while drag connection procedure is ongoing and the mouse moves over 
        // this item this value is true; if true the ConnectorDecorator is triggered
        // to be visible, see template
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion
        #endregion

        #endregion

        #region Constructors
        public DesignerItem(string id, DiagramControl diagramControl)
        {
            ItemId = id;
            DiagramControl = diagramControl;
            Focusable = false;
            MouseDoubleClick += (sender, e) => { diagramControl.Manager.Edit(this); };
            Loaded += DesignerItem_Loaded;
        }
        void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (Template != null)
            {
                ContentPresenter contentPresenter =
                    Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter != null)
                {
                    UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                    if (contentVisual != null)
                    {
                        DragThumb thumb = this.Template.FindName("PART_DragThumb", this) as DragThumb;
                        if (thumb != null)
                        {
                            ControlTemplate template = GetDragThumbTemplate(contentVisual);
                            if (template != null) thumb.Template = template;
                        }
                    }
                    //InitImage();
                }

                var c = Template.FindName("TextContent", this) as ContentControl;
                if (c != null)
                {
                    c.MouseDoubleClick += (a, b) =>
                    {
                        DiagramControl.Manager.Edit(this);
                        b.Handled = true;
                    };
                }

            }
        }

        public DesignerItem(DiagramControl diagramControl)
     : this(Guid.NewGuid().ToString(), diagramControl)
        { }
        public DesignerItem(DiagramItem diagramItem, DiagramControl diagramControl)
        {
            Top = 0d;
            Left = 0d;
            DiagramControl = diagramControl;
            DataContext = diagramItem;
            SetBinding(TextProperty, new Binding("Text"));
            SetBinding(ItemIdProperty, new Binding("Id"));
            SetBinding(ItemParentIdProperty, new Binding("PId"));
            ContextMenu = GetItemContextMenu(diagramControl);
            Focusable = false;
            Loaded += DesignerItem_Loaded;

        }
        public DesignerItem(object itemData, DiagramControl diagramControl)
        {
            DiagramControl = diagramControl;
            DataContext = itemData;
            SetBinding(TextProperty, new Binding(diagramControl.TextField));
            SetBinding(ItemIdProperty, new Binding(diagramControl.IdField));
            SetBinding(ItemParentIdProperty, new Binding(diagramControl.ParentIdField));
            SetBinding(LeftProperty, new Binding(diagramControl.LeftField));
            SetBinding(TopProperty, new Binding(diagramControl.TopField));
            ContextMenu = GetItemContextMenu(diagramControl);
            Focusable = false;
            Loaded += DesignerItem_Loaded;

        }
        static DesignerItem()
        {
            // set the key to reference the style for this control
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }
        #endregion

        #region override

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            if (designer == null) return;
            if ((e.OriginalSource as Image) == null)
            {
                DiagramControl.Focus();
                if (e.ClickCount == 1)
                {
                    CreateShadow(designer, e);
                }
            }

        }
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {

            //base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            // update selection
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    if (IsSelected)
                    {
                        designer.SelectionService.RemoveFromSelection(this);
                        if (DiagramControl.SelectedItems != null && DiagramControl.SelectedItems.Contains(DataContext))
                        {
                            DiagramControl.SelectedItems.Remove(DataContext);
                            DiagramControl.SelectedDesignerItems.Remove(this);
                        }
                    }
                    else
                    {
                        designer.SelectionService.AddToSelection(this);
                        if (DiagramControl.SelectedItems != null && !DiagramControl.SelectedItems.Contains(DataContext))
                        {
                            DiagramControl.SelectedItems.Add(DataContext);
                            DiagramControl.CanExpandAndCollapseSelectedItem = true;
                            DiagramControl.SelectedDesignerItems.Add(this);
                        }
                    }
                }
                else if (!IsSelected)
                {
                    designer.SelectionService.SelectItem(this);
                    if (DiagramControl.SelectedItems != null)
                    {
                        DiagramControl.SelectedItems.Clear();
                        DiagramControl.SelectedItems.Add(DataContext);
                        DiagramControl.CanExpandAndCollapseSelectedItem = true;
                        DiagramControl.SelectedDesignerItems.Clear();
                        DiagramControl.SelectedDesignerItems.Add(this);
                    }
                }
                //Focus();
                DiagramControl.Focus();
            }
        }
        void CreateShadow(DesignerCanvas designer, MouseButtonEventArgs e)
        {
            designer.IsMouseDown = false;
            designer.IsChangingParent = false;

            var shadow = DiagramControl.Manager.CreateItemShadow(this);
            designer.Shadow.ShadowItem = shadow;
            designer.Children.Add(designer.Shadow.ShadowItem);
            designer.Shadow.DesignerItem = this;

            designer.Root = DiagramControl.Manager.GetRoot(designer.Shadow.DesignerItem);


            designer.Shadow.SelectedItemsAllSubItems = DiagramControl.Manager.GetSelectedItemsAndAllSubItems();

            #region 位置
            var canvasPosition = e.GetPosition(designer);
            var itemPosition = e.GetPosition(this);
            designer.Shadow.MousePoint = canvasPosition;
            var top = canvasPosition.Y - itemPosition.Y;
            var left = canvasPosition.X - itemPosition.X;
            designer.Shadow.X = itemPosition.X;//点击处与左边框的距离
            designer.Shadow.Y = itemPosition.Y;
            Panel.SetZIndex(designer.Shadow.ShadowItem, 10000);
            Canvas.SetTop(designer.Shadow.ShadowItem, top);
            Canvas.SetLeft(designer.Shadow.ShadowItem, left);

            shadow.ApplyTemplate();
            shadow.SetTemplate();
            //shadow.UpdateLayout();

            designer.Shadow.ShadowItem.Visibility = Visibility.Collapsed;
            designer.IsMouseDown = true;

            #endregion
        }

        #endregion


        public void SetTemplate()
        {
            if (Template != null)
            {
                ContentPresenter contentPresenter =
                    Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter != null)
                {
                    if (contentPresenter.ContentTemplate == null)
                    {
                        contentPresenter.ContentTemplate = DiagramControl.GetDesignerItemTemplate(this.DiagramControl);
                        UpdateLayout();
                    }
                }
            }
        }
        public virtual int Level
        {
            get
            {
                int result = 1;
                var parent = ParentDesignerItem;
                while (parent != null)
                {
                    result++;
                    parent = parent.ParentDesignerItem;
                }

                return result;
            }
        }
        public virtual bool HasChild
        {
            get
            {
                return ChildrenDesignerItems != null && ChildrenDesignerItems.Count > 0;
            }
        }
        public void UpdateExpander()
        {
            if (ParentDesignerItem != null)
                ParentDesignerItem.IsExpanderVisible = ParentDesignerItem.HasChild && ParentDesignerItem.CanCollapsed;
        }

        #region Command
        public ICommand DeleteImage
        {
            get { return new RelayCommand<ImageUrl>(DeleteImageAction); }
        }
        private void DeleteImageAction(ImageUrl param)
        {
            SelectedImage = param;
            ItemStyle.ImageUrl.Remove(SelectedImage);
        }

        #endregion
    }
}
