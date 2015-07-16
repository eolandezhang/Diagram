using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace QPP.Wpf.UI.Controls
{
    public class DataGridSelectColumn : DataGridColumn
    {
        List<CheckBox> ckbs = new List<CheckBox>();
        CheckBox ckbHeader;
        bool suppressChanged = false;

        public static readonly DependencyProperty SelectedItemsProperty;
        public static readonly DependencyProperty ShowHeaderProperty;
        public static readonly DependencyProperty SelectionModeProperty;


        static DataGridSelectColumn()
        {
            var thisType = typeof(DataGridSelectColumn);
            SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), thisType, new UIPropertyMetadata(OnSelectedItemsChanged));
            ShowHeaderProperty = DependencyProperty.RegisterAttached("ShowHeader", typeof(bool), thisType, new PropertyMetadata(OnShowHeaderChanged));
            SelectionModeProperty = DependencyProperty.RegisterAttached("SelectionMode", typeof(SelectionMode), thisType, new PropertyMetadata(SelectionMode.Extended));
        }

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        static void OnShowHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DataGridSelectColumn;
            if (c != null)
                c.ckbHeader.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DataGridSelectColumn;
            if (c != null)
            {
                var n = e.NewValue as INotifyCollectionChanged;
                if (n != null)
                    n.CollectionChanged += (s, a) =>
                    {
                        if (a.Action == NotifyCollectionChangedAction.Add)
                        {
                            foreach (object i in a.NewItems)
                                foreach (var ckb in c.ckbs)
                                {
                                    if (ckb.Tag.Equals(i))
                                        ckb.IsChecked = true;
                                }
                        }
                        else if (a.Action == NotifyCollectionChangedAction.Remove)
                        {
                            foreach (object i in a.OldItems)
                                foreach (var ckb in c.ckbs)
                                {
                                    if (ckb.Tag.Equals(i))
                                        ckb.IsChecked = false;
                                }
                        }
                        else if (a.Action == NotifyCollectionChangedAction.Reset)
                        {
                            //foreach (object i in c.SelectedItems.OfType<Object>().ToList())
                            //    foreach (var ckb in c.ckbs.ToList())
                            //        ckb.IsChecked = ckb.Tag == i;
                            foreach (var ckb in c.ckbs.ToList())
                                if (c.SelectedItems.OfType<object>().Any(p => p.Equals(ckb.Tag)))
                                    ckb.IsChecked = true;
                                else
                                    ckb.IsChecked = false;
                        }
                    };
            }
        }

        public DataGridSelectColumn()
        {
            CanUserResize = false;
            CanUserReorder = false;
            Header = CreateHeaderCheckBox();
            DataGridHelper.SetCanHide(this, false);
        }

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public virtual IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        protected override System.Windows.FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return CreateCheckBox(cell, dataItem);
        }

        protected override System.Windows.FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return CreateCheckBox(cell, dataItem);
        }

        CheckBox CreateHeaderCheckBox()
        {
            ckbHeader = new CheckBox();
            ckbHeader.Checked += ckbHeader_Checked;
            ckbHeader.Unchecked += ckbHeader_Checked;
            ckbHeader.Visibility = Visibility.Collapsed;
            return ckbHeader;
        }


        void ckbHeader_Checked(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (suppressChanged) return;

            if (ckb.IsChecked == true)
            {
                SelectedItems.Clear();
                foreach (var d in DataGridOwner.ItemsSource)
                    SelectedItems.Add(d);
            }
            else
                SelectedItems.Clear();
        }

        CheckBox CreateCheckBox(DataGridCell cell, object dataItem)
        {
            var ckb = new CheckBox();
            ckb.HorizontalAlignment = HorizontalAlignment.Center;
            ckb.Margin = new Thickness(5, 1, 0, 1);
            if (SelectedItems != null && IsSelected(dataItem))
                ckb.IsChecked = true;
            ckb.Checked += ckb_Checked;
            ckb.Unchecked += ckb_Checked;
            ckb.Tag = dataItem;
            cell.Tag = ckb;
            cell.DataContextChanged += new DependencyPropertyChangedEventHandler(cell_DataContextChanged);
            ckbs.Add(ckb);
            return ckb;
        }

        void cell_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var cell = sender as DataGridCell;
            var ckb = cell.Tag as CheckBox;
            ckb.Tag = cell.DataContext;
            ckb.IsChecked = SelectedItems != null && IsSelected(cell.DataContext);
        }

        void ckb_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SelectedItems == null) return;
            var ckb = sender as CheckBox;
            var dataItem = ckb.Tag;
            if (dataItem == null || dataItem.ToSafeString() == "{NewItemPlaceholder}") return;
            if (ckb.IsChecked.HasValue && ckb.IsChecked.Value)
            {
                if (SelectionMode == SelectionMode.Single)
                    ckbs.ForEach(p => { if (p.Tag != ckb.Tag) p.IsChecked = false; });
                if (!IsSelected(dataItem))
                    SelectedItems.Add(dataItem);
                if (suppressChanged || !ShowHeader) return;
            }
            else
            {
                if (IsSelected(dataItem))
                    SelectedItems.Remove(dataItem);
                if (suppressChanged || !ShowHeader) return;
            }
        }

        bool IsSelected(object item)
        {
            return SelectedItems.OfType<object>().Any(p => object.Equals(p, item));
        }
    }
}
