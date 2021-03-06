﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

namespace QPP.Wpf.UI.TreeEditor
{
    public class Shadow
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point MousePoint { get; set; }
        public DesignerItem ShadowItem { get; set; }
        public DesignerItem DesignerItem { get; set; }
        public List<DesignerItem> SelectedItemsAllSubItems { get; set; }
    }
    public partial class DesignerCanvas : Canvas
    {
        #region Fields & Properties

        private Point? rubberbandSelectionStartPoint = null;
        private DiagramControl _diagramControl;
        private Shadow _shadow;
        public Shadow Shadow
        {
            get
            {
                if (_shadow == null)
                {
                    _shadow = new Shadow();
                    return _shadow;
                }
                return _shadow;
            }
        }
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
        private bool IsMouseMove;
        public bool IsChangingParent;
        //public Point StartPoint = new Point(0, 0);
        public DesignerItem Root;
        #endregion



        #region Override

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
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
                isGray = false;
                IsMouseDown = true;
                e.Handled = true;
            }
            if (e.ClickCount == 2)
            {
                if (DiagramControl.CanvasDoubleClickCommand != null)
                { DiagramControl.CanvasDoubleClickCommand.Execute(e.GetPosition(this)); }
            }
            //StartPoint = e.GetPosition(this);

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
            var point = e.GetPosition(this);
            if (Shadow.MousePoint.X > 0 && Shadow.MousePoint.Y > 0)
            {
                var x = Math.Abs(Shadow.MousePoint.X - point.X);
                var y = Math.Abs(Shadow.MousePoint.Y - point.Y);
                if (IsMouseDown)
                {
                    if (Shadow.ShadowItem != null && Shadow.DesignerItem != null)
                    {

                        if (x > 2 || y > 2)
                        {
                            IsChangingParent = true;
                        }
                        else
                        {
                            IsChangingParent = false;
                        }
                    }
                }
                if (IsMouseDown && IsChangingParent && Root != null)
                {

                    Move(e, Root);
                    AutoScroll(e);



                }
            }
        }
        void Move(MouseEventArgs e, DesignerItem root)//移动影子
        {
            IsMouseMove = true;
            if (Shadow.ShadowItem == null) return;
            Shadow.ShadowItem.Visibility = Visibility.Visible;
            var canvasPosition = e.GetPosition(this);
            var y = canvasPosition.Y - Shadow.Y;
            var x = (canvasPosition.X - Shadow.X);
            SetTop(Shadow.ShadowItem, y <= 0 ? 0 : y);
            SetLeft(Shadow.ShadowItem, x <= 0 ? 0 : x);
            var manager = _diagramControl.Manager;
            NewParent = manager.ChangeParent(new Point(x, y), Shadow.DesignerItem, Shadow.SelectedItemsAllSubItems);
            if (!isGray)
            {
                Shadow.SelectedItemsAllSubItems.ForEach(c => { c.IsDragItemChild = true; });
                isGray = true;
            }
            DiagramControl.Manager.CreateHelperConnection(NewParent, Shadow.ShadowItem);

            DiagramControl.Manager.MoveUpAndDown(NewParent, Shadow.ShadowItem, root);

        }
        IScrollInfo _scrollInfo;
        public void AutoScroll(MouseEventArgs e)//自动滚动
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
                }
                else if (mousePos.Y > (scrollable.RenderSize.Height - v))
                {
                    var delta = (mousePos.Y + v - scrollable.RenderSize.Height) / 1; //translate back to original unit
                    _scrollInfo.SetVerticalOffset(_scrollInfo.VerticalOffset + delta);
                }
                #endregion

                #region Horizontal
                var h = 200;
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
        }
        void Finish(Point canvasPosition)
        {

            if (IsChangingParent & IsMouseMove && Shadow.ShadowItem != null)
            {
                var y = canvasPosition.Y - Shadow.Y;//shadow在canvas的y坐标
                var x = canvasPosition.X - Shadow.X;

                var ox = Math.Abs(x - GetLeft(Shadow.DesignerItem));//shadow与designerItem之间的偏移
                var oy = Math.Abs(y - GetTop(Shadow.DesignerItem));

                if (ox > 2 || oy > 2)
                {

                    _diagramControl.Manager.AfterChangeParent(Shadow.DesignerItem, NewParent, new Point(x <= 0 ? 0 : x, y <= 0 ? 0 : y), Shadow.SelectedItemsAllSubItems);
                    Shadow.SelectedItemsAllSubItems.ForEach(c => { c.IsDragItemChild = false; });
                }
            }
            Children.Remove(Shadow.ShadowItem);
            NewParent = null;
            isGray = false;
            IsMouseDown = false;
            IsMouseMove = false;
            IsChangingParent = false;
            Shadow.MousePoint = new Point(0, 0);
        }
        //protected override void OnDrop(DragEventArgs e)
        //{
        //    base.OnDrop(e);
        //    DragObject dragObject = e.Data.GetData(typeof(DragObject)) as DragObject;
        //    if (dragObject != null && !String.IsNullOrEmpty(dragObject.Xaml))
        //    {
        //        DesignerItem newItem = null;
        //        Object content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

        //        if (content != null)
        //        {
        //            DiagramControl diagramControl = TemplatedParent as DiagramControl;
        //            newItem = new DesignerItem(diagramControl);
        //            newItem.Content = content;

        //            Point position = e.GetPosition(this);

        //            if (dragObject.DesiredSize.HasValue)
        //            {
        //                Size desiredSize = dragObject.DesiredSize.Value;
        //                newItem.Width = desiredSize.Width;
        //                newItem.Height = desiredSize.Height;

        //                DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
        //                DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
        //            }
        //            else
        //            {
        //                DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X));
        //                DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y));
        //            }

        //            Canvas.SetZIndex(newItem, this.Children.Count);
        //            this.Children.Add(newItem);
        //            SetConnectorDecoratorTemplate(newItem);

        //            //update selection
        //            this.SelectionService.SelectItem(newItem);
        //            newItem.Focus();

        //        }

        //        e.Handled = true;
        //    }
        //}
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
            size.Width += 100;
            size.Height += 100;

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
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DiagramControl.ClickPoint = e.GetPosition(this);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            var canvasPosition = e.GetPosition(this);

            Finish(canvasPosition);
        }

        #endregion

    }
}
