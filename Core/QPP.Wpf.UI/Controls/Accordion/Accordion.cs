using QPP.Wpf.UI.Controls.Expander;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace QPP.Wpf.UI.Controls.Accordion
{
    [ContentProperty("Children")]
    [TemplatePart(Name = "PART_Container", Type = typeof(Grid))]
    public class Accordion : Control
    {
        StackPanel panel = new StackPanel();
        Grid container;
        bool suppress;
        bool expanded;

        static Accordion()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Accordion), new FrameworkPropertyMetadata(typeof(Accordion)));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            panel.Height = finalSize.Height;
            if (!expanded)
            {
                expanded = true;
                var item = panel.Children.OfType<ExpanderItem>().FirstOrDefault();
                if (item != null)
                {
                    suppress = true;
                    item.IsExpanded = true;
                    suppress = false;
                }
            }
            UpdateSize(finalSize.Height);
            return base.ArrangeOverride(finalSize);
        }

        void UpdateSize(double panelHeight)
        {
            var height = 0d;
            foreach (var item in Children)
            {
                if (!item.IsExpanded)
                {
                    var ctl = LogicalTreeHelper.GetChildren(item).OfType<FrameworkElement>().FirstOrDefault();
                    if (ctl != null)
                    {
                        if (item.DesiredSize.Height > ctl.DesiredSize.Height)
                            height += item.DesiredSize.Height - ctl.DesiredSize.Height;
                        else
                            height += item.DesiredSize.Height;
                        ctl.Height = 0;
                    }
                    //height += item.Margin.Top + item.Margin.Bottom;
                }
            }
            var expandedItem = Children.FirstOrDefault(p => p.IsExpanded && p.Visibility != System.Windows.Visibility.Collapsed);
            if (expandedItem != null)
            {
                var ctl = LogicalTreeHelper.GetChildren(expandedItem).OfType<FrameworkElement>().FirstOrDefault();
                if (ctl != null)
                {
                    if (expandedItem.DesiredSize.Height > ctl.DesiredSize.Height)
                        height += expandedItem.DesiredSize.Height - ctl.DesiredSize.Height;
                    else
                        height += expandedItem.DesiredSize.Height;
                    ctl.Height = panelHeight - height;// - expandedItem.Margin.Top - expandedItem.Margin.Bottom;
                }
            }
            panel.InvalidateArrange();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<ExpanderItem> Children { get; private set; }

        public Accordion()
        {
            Children = new ObservableCollection<ExpanderItem>();
            Children.CollectionChanged += new NotifyCollectionChangedEventHandler(Children_CollectionChanged);
        }

        public override void OnApplyTemplate()
        {
            container = GetTemplateChild("PART_Container") as Grid;
            if (container.Children.Count == 0)
                container.Children.Add(panel);
            base.OnApplyTemplate();
        }

        void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (ExpanderItem item in e.NewItems)
                {
                    item.IsExpanded = false;
                    item.Expanded += item_Expanded;
                    item.Collapsed += item_Collapsed;
                    var ctl = LogicalTreeHelper.GetChildren(item).OfType<FrameworkElement>().FirstOrDefault();
                    if (ctl != null)
                        ctl.Height = 0;
                    panel.Children.Add(item);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (ExpanderItem item in e.OldItems)
                {
                    item.Expanded -= item_Expanded;
                    item.Collapsed -= item_Collapsed;
                    panel.Children.Remove(item);
                }
            }
            InvalidateArrange();
        }

        void item_Collapsed(object sender, RoutedEventArgs e)
        {
            if (suppress) return;
            suppress = true;
            (sender as ExpanderItem).IsExpanded = true;
            suppress = false;
        }

        void item_Expanded(object sender, RoutedEventArgs e)
        {
            if (suppress) return;
            suppress = true;
            foreach (var item in Children)
            {
                if (item != sender)
                    item.IsExpanded = false;
            }
            UpdateSize(ActualHeight);
            suppress = false;
        }
    }
}
