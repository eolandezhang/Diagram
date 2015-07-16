using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using QPP.Wpf.UI.Controls.XGantt;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;

namespace QPP.Wpf.UI.Controls
{
    public class XGanttChartHelper : DependencyObject
    {
        public static readonly DependencyProperty DataGridProperty;

        static XGanttChartHelper()
        {
            var thisType = typeof(XGanttChartHelper);
            DataGridProperty = DependencyProperty.RegisterAttached("DataGrid", typeof(DataGrid), thisType, new UIPropertyMetadata(null, OnDataGridScrollViewerChanged));


        }

        public static void SetDataGrid(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(DataGridProperty, value);
        }

        public static DataGrid GetDataGrid(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(DataGridProperty);
        }

        static void OnDataGridScrollViewerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ganttchart = d as XGanttChart;
            if (ganttchart != null)
            {
                ganttchart.Loaded += ganttchart_Loaded;
            }
        }

        static void ganttchart_Loaded(object sender, RoutedEventArgs e)
        {
            var ganttchart = sender as XGanttChart;

            var dataGrid = GetDataGrid(ganttchart);
            if (dataGrid != null)
            {
                //var scrollViewer = LogicalTreeHelper.FindLogicalNode(dataGrid, "DG_ScrollViewer") as ScrollViewer;
                ScrollViewer scrollViewer = null;
                FindScrollViewer(dataGrid, ref scrollViewer);

                if (scrollViewer != null)
                {
                    //ScrollBar verticalScrollbar = null;
                    //FindVerticalScrollbar(ganttchart, ref verticalScrollbar);
                    ScrollBar verticalScrollbar = GetVerticalScrollbar(ganttchart);
                    if (verticalScrollbar != null)
                    {

                        verticalScrollbar.Scroll += new ScrollEventHandler(VerticalScrollbar_Scroll);

                        Binding heightBinding = new Binding("ScrollableHeight");
                        Binding visibilityBinding = new Binding("ScrollableHeight");
                        Binding viewportBinding = new Binding("ViewportHeight");
                        if (scrollViewer != null)
                        {
                            heightBinding.Source = scrollViewer;//滚动条高度
                            visibilityBinding.Source = scrollViewer;//可见性
                            viewportBinding.Source = scrollViewer;//滚动条滑块size
                        }

                        BindingOperations.ClearBinding(verticalScrollbar, ScrollBar.MaximumProperty);

                        verticalScrollbar.SetBinding(ScrollBar.MaximumProperty, heightBinding);

                        visibilityBinding.Converter = new QPP.Wpf.UI.Converters.BoolToVisibilityConverter();

                        BindingOperations.ClearBinding(verticalScrollbar, ScrollBar.VisibilityProperty);
                        verticalScrollbar.SetBinding(ScrollBar.VisibilityProperty, visibilityBinding);

                        BindingOperations.ClearBinding(verticalScrollbar, ScrollBar.ViewportSizeProperty);
                        verticalScrollbar.SetBinding(ScrollBar.ViewportSizeProperty, viewportBinding);
                    }
                }
            }
        }

        #region 获取相应DataGrid的ScrollViewer
        static ScrollViewer GetScrollViewer(DependencyObject element)
        {
            ScrollViewer scrollViewer = null;
            FindScrollViewer(element, ref scrollViewer);
            return scrollViewer;
        }

        static void FindScrollViewer(DependencyObject element, ref ScrollViewer scrollViewer)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {

                Visual child = (Visual)VisualTreeHelper.GetChild(element, i);
                if (child is ScrollViewer)
                {
                    scrollViewer = child as ScrollViewer;
                    return;
                }
                else
                    FindScrollViewer(child, ref scrollViewer);
            }
        }

        #endregion

        #region 获取甘特图的垂直滚动条

        static ScrollBar GetVerticalScrollbar(DependencyObject element)
        {
            ScrollBar verticalScrollbar = null;
            FindVerticalScrollbar(element, ref verticalScrollbar);
            return verticalScrollbar;
        }

        static void FindVerticalScrollbar(DependencyObject element, ref ScrollBar scrollBar)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {

                Visual child = (Visual)VisualTreeHelper.GetChild(element, i);
                if (child is ScrollBar)
                {
                    scrollBar = child as ScrollBar;
                    return;
                }
                else
                    FindVerticalScrollbar(child, ref scrollBar);
            }
        }
        #endregion


        static void VerticalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.NewValue <= 0) return;
            XGanttChart ganttChart = GetGanttChart(sender as DependencyObject);

            var dataGrid = GetDataGrid(ganttChart);
            if (dataGrid != null)
            {
                var scrollViewer = GetScrollViewer(dataGrid);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset((int)e.NewValue);//使滚动条同步滑动
                }
            }

        }

        static XGanttChart GetGanttChart(DependencyObject element)
        {
            if (element == null) return null;
            var parent = VisualTreeHelper.GetParent(element);
            if (parent == null)
                return null;
            else
            {
                if (parent is XGanttChart)
                    return parent as XGanttChart;
                else
                    return GetGanttChart(parent);
            }

        }

    }
}
