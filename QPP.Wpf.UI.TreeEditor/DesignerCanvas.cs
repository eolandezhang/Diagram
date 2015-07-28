using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace QPP.Wpf.UI.TreeEditor
{
    public class Shadow
    {
        public double X { get; set; }
        public double Y { get; set; }
        public DesignerItem ShadowItem { get; set; }
        public DesignerItem DesignerItem { get; set; }
        public List<DesignerItem> SelectedItemsAllSubItems { get; set; }
    }
    public partial class DesignerCanvas : Canvas
    {
        #region Fields & Properties

        private Point? rubberbandSelectionStartPoint = null;
        private DiagramControl _diagramControl;
        public Shadow Shadow { get; set; }
        private DiagramControl DiagramControl
        {
            get
            {
                if (_diagramControl == null)
                {
                    _diagramControl = TemplatedParent as DiagramControl;
                }
                return _diagramControl;
            }
        }
        private SelectionService selectionService;
        internal SelectionService SelectionService
        {
            get
            {
                if (selectionService == null)
                    selectionService = new SelectionService(this, DiagramControl);

                return selectionService;
            }
        }
        private DesignerItem NewParent;
        private bool isGray = false;
        public bool IsMouseDown = false;
        #endregion



        #region Override

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                // in case that this click is the start of a 
                // drag operation we cache the start point
                this.rubberbandSelectionStartPoint = new Point?(e.GetPosition(this));

                // if you click directly on the canvas all 
                // selected items are 'de-selected'
                SelectionService.ClearSelection();
                DiagramControl.SelectedItems.Clear();
                DiagramControl.CanExpandAndCollapseSelectedItem = false;
                Focus();
                e.Handled = true;
                isGray = false;
                IsMouseDown = true;
            }
            if (e.ClickCount == 2)
            {
                DiagramControl.CanvasDoubleClickCommand.Execute(e.GetPosition(this));
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                this.rubberbandSelectionStartPoint = null;

            // ... but if mouse button is pressed and start
            // point value is set we do have one
            if (this.rubberbandSelectionStartPoint.HasValue)
            {
                // create rubberband adorner
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(_diagramControl, this, rubberbandSelectionStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }
            }
            e.Handled = true;
            if (IsMouseDown)
            {
                Move(e); AutoScroll(e);
            }
        }
        void Move(MouseEventArgs e)//移动影子
        {
            if (Shadow == null) return;
            if (Shadow.ShadowItem == null) return;
            if (Math.Abs(Shadow.X - GetLeft(Shadow.DesignerItem)) < 2 && Math.Abs(Shadow.Y - GetTop(Shadow.DesignerItem)) < 2) return;
            Shadow.ShadowItem.Visibility = Visibility.Visible;
            var canvasPosition = e.GetPosition(this);
            var y = canvasPosition.Y - Shadow.Y;
            var x = canvasPosition.X - Shadow.X;
            SetTop(Shadow.ShadowItem, y);
            SetLeft(Shadow.ShadowItem, x);
            var manager = _diagramControl.DiagramManager;
            NewParent = manager.ChangeParent(new Point(x, y), Shadow.DesignerItem, Shadow.SelectedItemsAllSubItems);
            if (!isGray)
            {
                Shadow.SelectedItemsAllSubItems.ForEach(c => { c.IsDragItemChild = true; });
                isGray = true;
            }
            DiagramControl.DiagramManager.CreateHelperConnection(NewParent, Shadow.ShadowItem);
            DiagramControl.DiagramManager.MoveUpAndDown(NewParent, Shadow.ShadowItem);

        }
        IScrollInfo _scrollInfo;
        void AutoScroll(MouseEventArgs e)//自动滚动
        {
            var scrollViewer = DiagramControl.Template.FindName("DesignerScrollViewer", DiagramControl) as ScrollViewer;
            if (scrollViewer == null) return;
            var parent = e.OriginalSource as DependencyObject;
            while (parent != null)
            {
                _scrollInfo = parent as IScrollInfo;
                if (_scrollInfo != null && _scrollInfo.ScrollOwner == scrollViewer)
                {
                    break;
                }
                _scrollInfo = null;
                parent = VisualTreeHelper.GetParent(parent);
            }

            UIElement scrollable = _scrollInfo as UIElement;
            if (scrollable != null)
            {
                var mousePos = e.GetPosition(scrollable);

                #region Vertical

                var v = 40;
                if (mousePos.Y < v)
                {
                    var delta = (mousePos.Y - v) / 1; //translate back to original unit
                    _scrollInfo.SetVerticalOffset(_scrollInfo.VerticalOffset + delta);
                    //_diagramControl.AddToMessage("滚动", "上移");
                }
                else if (mousePos.Y > (scrollable.RenderSize.Height - v))
                {
                    var delta = (mousePos.Y + v - scrollable.RenderSize.Height) / 1; //translate back to original unit
                    _scrollInfo.SetVerticalOffset(_scrollInfo.VerticalOffset + delta);
                    //_diagramControl.AddToMessage("滚动", "下移");
                }
                #endregion

                #region Horizontal
                var h = 300;
                if (mousePos.X < h)
                {
                    var delta = (mousePos.X - h) / 1; //translate back to original unit
                    _scrollInfo.SetHorizontalOffset(_scrollInfo.HorizontalOffset + delta);
                }
                else if (mousePos.X > (scrollable.RenderSize.Width - h))
                {
                    var delta = (mousePos.X + h - scrollable.RenderSize.Width) / 1; //translate back to original unit
                    _scrollInfo.SetHorizontalOffset(_scrollInfo.HorizontalOffset + delta);
                }
                #endregion
            }

        }
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            var canvasPosition = e.GetPosition(this);
            Finish(canvasPosition);
            //if (Shadow != null && Shadow.ShadowItem != null)
            //{
            //    var canvasPosition = e.GetPosition(this);
            //    var y = canvasPosition.Y - Shadow.Y;
            //    var x = canvasPosition.X - Shadow.X;
            //    //var orgParent = _diagramControl.DesignerItems.FirstOrDefault(p => p.ItemId == Shadow.DesignerItem.ItemParentId);

            //    var ox = Math.Abs(x - GetLeft(Shadow.DesignerItem));
            //    var oy = Math.Abs(y - GetTop(Shadow.DesignerItem));

            //    if (ox > 2 || oy > 2)
            //    {
            //        _diagramControl.DiagramManager.AfterChangeParent(Shadow.DesignerItem, NewParent, new Point(x, y), Shadow.SelectedItemsAllSubItems);
            //    }

            //    Shadow.SelectedItemsAllSubItems.ForEach(c => { c.IsDragItemChild = false; });
            //    Children.Remove(Shadow.ShadowItem);
            //}
            //Shadow = null;
            //NewParent = null;
            //isGray = false;
            //IsMouseDown = false;
            ////_diagramControl.AddToMessage("移除影子", "");
        }

        void Finish(Point canvasPosition)
        {
            if (Shadow != null && Shadow.ShadowItem != null)
            {
                var y = canvasPosition.Y - Shadow.Y;
                var x = canvasPosition.X - Shadow.X;
                //var orgParent = _diagramControl.DesignerItems.FirstOrDefault(p => p.ItemId == Shadow.DesignerItem.ItemParentId);

                var ox = Math.Abs(x - GetLeft(Shadow.DesignerItem));
                var oy = Math.Abs(y - GetTop(Shadow.DesignerItem));

                if (ox > 2 || oy > 2)
                {
                    _diagramControl.DiagramManager.AfterChangeParent(Shadow.DesignerItem, NewParent, new Point(x, y), Shadow.SelectedItemsAllSubItems);
                }

                Shadow.SelectedItemsAllSubItems.ForEach(c => { c.IsDragItemChild = false; });
                Children.Remove(Shadow.ShadowItem);
            }
            Shadow = null;
            NewParent = null;
            isGray = false;
            IsMouseDown = false;
        }
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            DragObject dragObject = e.Data.GetData(typeof(DragObject)) as DragObject;
            if (dragObject != null && !String.IsNullOrEmpty(dragObject.Xaml))
            {
                DesignerItem newItem = null;
                Object content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

                if (content != null)
                {
                    DiagramControl diagramControl = TemplatedParent as DiagramControl;
                    newItem = new DesignerItem(diagramControl);
                    newItem.Content = content;

                    Point position = e.GetPosition(this);

                    if (dragObject.DesiredSize.HasValue)
                    {
                        Size desiredSize = dragObject.DesiredSize.Value;
                        newItem.Width = desiredSize.Width;
                        newItem.Height = desiredSize.Height;

                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
                    }
                    else
                    {
                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y));
                    }

                    Canvas.SetZIndex(newItem, this.Children.Count);
                    this.Children.Add(newItem);
                    SetConnectorDecoratorTemplate(newItem);

                    //update selection
                    this.SelectionService.SelectItem(newItem);
                    newItem.Focus();

                }

                e.Handled = true;
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            foreach (UIElement element in this.InternalChildren)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                //measure desired size for each child
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            // add margin 
            size.Width += 500;
            size.Height += 500;

            //foreach (DesignerItem item in SelectionService.CurrentSelection.OfType<DesignerItem>())
            //{
            //    DragThumb dragThumb = item.Template.FindName("PART_DragThumb", item) as DragThumb;

            //    if (dragThumb != null)
            //    {
            //        var textBlock = dragThumb.Template.FindName("Text", dragThumb);
            //    }
            //}

            return size;
        }
        private void SetConnectorDecoratorTemplate(DesignerItem item)
        {
            if (item.ApplyTemplate() && item.Content is UIElement)
            {
                ControlTemplate template = DesignerItem.GetConnectorDecoratorTemplate(item.Content as UIElement);
                Control decorator = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                if (decorator != null && template != null)
                    decorator.Template = template;
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            var canvasPosition = e.GetPosition(this);
            Finish(canvasPosition);
        }

        #endregion

    }
}
