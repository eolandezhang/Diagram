using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows.Data;
using QPP.Wpf.UI.Properties;
using System.Windows.Input;

namespace QPP.Wpf.UI.Controls
{
    public class DataGridHelper : DependencyObject
    {
        private static bool inWidthChange = false;

        public static readonly DependencyProperty ShowRowNumProperty;
        public static readonly DependencyProperty RowContextMenuProperty;
        public static readonly DependencyProperty ColumnHeaderContextMenuProperty;
        public static readonly DependencyProperty StateIdProperty;
        public static readonly DependencyProperty IsStatefulProperty;
        public static readonly DependencyProperty DataGridSettingsProperty;
        public static readonly DependencyProperty CanHideProperty;
        public static readonly DependencyProperty AttachedBindingProperty;
        public static readonly DependencyProperty RowDoubleClickCommandProperty;

        static DataGridHelper()
        {
            var thisType = typeof(DataGridHelper);
            ShowRowNumProperty = DependencyProperty.RegisterAttached("ShowRowNum", typeof(bool), thisType, new UIPropertyMetadata(false, OnShowRowNumChanged));
            RowContextMenuProperty = DependencyProperty.RegisterAttached("RowContextMenu", typeof(ContextMenu), thisType, new UIPropertyMetadata(null));
            ColumnHeaderContextMenuProperty = DependencyProperty.RegisterAttached("ColumnHeaderContextMenu", typeof(ContextMenu), thisType, new UIPropertyMetadata(null));
            StateIdProperty = DependencyProperty.RegisterAttached("StateId", typeof(string), thisType, new UIPropertyMetadata(null));
            IsStatefulProperty = DependencyProperty.RegisterAttached("IsStateful", typeof(bool), thisType, new UIPropertyMetadata(false, OnIsStatefulChanged));
            DataGridSettingsProperty = DependencyProperty.Register("DataGridSettings", typeof(IDataGridSettings), thisType, new PropertyMetadata(new DataGridApplicationSettings()));
            CanHideProperty = DependencyProperty.RegisterAttached("CanHide", typeof(bool), thisType, new UIPropertyMetadata(true, null));
            AttachedBindingProperty = DependencyProperty.RegisterAttached("AttachedBinding", typeof(string), thisType);


            RowDoubleClickCommandProperty = DependencyProperty.RegisterAttached("RowDoubleClickCommand", typeof(ICommand), thisType);

        }

        #region Get/Set Property

        public static void SetRowDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(RowDoubleClickCommandProperty, value);
        }

        public static ICommand GetRowDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(RowDoubleClickCommandProperty);
        }

        public static void SetDataGridSettings(DependencyObject obj, IDataGridSettings value)
        {
            obj.SetValue(DataGridSettingsProperty, value);
        }

        public static IDataGridSettings GetDataGridSettings(DependencyObject obj)
        {
            return (IDataGridSettings)obj.GetValue(DataGridSettingsProperty);
        }

        public static void SetIsStateful(DependencyObject obj, bool value)
        {
            obj.SetValue(IsStatefulProperty, value);
        }

        public static bool GetIsStateful(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsStatefulProperty);
        }

        public static void SetStateId(DependencyObject obj, string value)
        {
            obj.SetValue(StateIdProperty, value);
        }

        public static string GetStateId(DependencyObject obj)
        {
            return (string)obj.GetValue(StateIdProperty);
        }

        public static void SetColumnHeaderContextMenu(DependencyObject obj, ContextMenu value)
        {
            obj.SetValue(ColumnHeaderContextMenuProperty, value);
        }

        public static ContextMenu GetColumnHeaderContextMenu(DependencyObject obj)
        {
            return (ContextMenu)obj.GetValue(ColumnHeaderContextMenuProperty);
        }

        public static void SetRowContextMenu(DependencyObject obj, ContextMenu value)
        {
            obj.SetValue(RowContextMenuProperty, value);
        }

        public static ContextMenu GetRowContextMenu(DependencyObject obj)
        {
            return (ContextMenu)obj.GetValue(RowContextMenuProperty);
        }

        public static void SetShowRowNum(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowRowNumProperty, value);
        }

        public static bool GetShowRowNum(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowRowNumProperty);
        }

        public static void SetCanHide(DependencyObject obj, bool value)
        {
            obj.SetValue(CanHideProperty, value);
        }

        public static bool GetCanHide(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanHideProperty);
        }

        public static string GetAttachedBinding(DependencyObject d)
        {
            return (string)d.GetValue(AttachedBindingProperty);
        }

        public static void SetAttachedBinding(DependencyObject d, string value)
        {
            d.SetValue(AttachedBindingProperty, value);
        }

        #endregion

        static void OnIsStatefulChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                if ((bool)e.NewValue)
                {
                    dataGrid.Loaded += dataGrid_Loaded;
                    dataGrid.ColumnReordered += dataGrid_ColumnReordered;
                    ContextMenu columnHeaderContextMenu = new ContextMenu();
                    columnHeaderContextMenu.Tag = dataGrid;
                    columnHeaderContextMenu.Loaded += columnHeaderContextMenu_Loaded;
                    SetColumnHeaderContextMenu(dataGrid, columnHeaderContextMenu);
                }
                else
                {
                    dataGrid.Loaded -= dataGrid_Loaded;
                    dataGrid.ColumnReordered -= dataGrid_ColumnReordered;
                    dataGrid.ClearValue(ColumnHeaderContextMenuProperty);
                }
            }
        }

        static void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            if (GetIsStateful(dataGrid))
            {
                string id = GetId(dataGrid);
                if (id.IsNotEmpty())
                {
                    var settings = GetDataGridSettings(dataGrid);
                    if (settings.DataGridSettings != null && settings.DataGridSettings.ColumnSettings.ContainsKey(id))
                    {
                        var columns = settings.DataGridSettings.ColumnSettings[id];
                        for (var index = 0; index < columns.Length; index++)
                        {
                            if (index < dataGrid.Columns.Count)
                                columns[index].Apply(dataGrid.Columns[index], dataGrid.Columns.Count);
                        }
                    }

                    EventHandler widthPropertyChangedHandler = (s, x) => inWidthChange = true;

                    var widthPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn));
                    foreach (var column in dataGrid.Columns)
                    {
                        widthPropertyDescriptor.AddValueChanged(column, widthPropertyChangedHandler);
                        dataGrid.PreviewMouseLeftButtonUp += dataGrid_PreviewMouseLeftButtonUp;
                    }
                }
            }

            dataGrid.Loaded -= dataGrid_Loaded;
        }

        static void dataGrid_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (inWidthChange && sender is DataGrid)
            {
                inWidthChange = false;
                UpdateColumnInfo(sender as DataGrid);
            }
        }

        private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }

        static void OnShowRowNumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid)
            {
                var dataGrid = d as DataGrid;
                dataGrid.LoadingRow += dg_LoadingRow;
                dataGrid.MouseDoubleClick += new MouseButtonEventHandler(dataGrid_MouseDoubleClick);
                if (GetShowRowNum(dataGrid))
                {
                    ItemsChangedEventHandler itemsChangedHandler = null;
                    itemsChangedHandler = (object sender, ItemsChangedEventArgs ea) =>
                    {
                        if (!GetShowRowNum(dataGrid))
                            dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
                        else
                        {
                            GetVisualChildCollection<DataGridRow>(dataGrid).
                                ForEach(r => r.Header = r.GetIndex() + 1);
                        }
                    };
                    dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
                }
            }
        }

        static void dataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            UpdateColumnInfo(dataGrid);
        }

        static void UpdateColumnInfo(DataGrid dataGrid)
        {
            if (GetIsStateful(dataGrid))
            {
                string id = GetId(dataGrid);
                if (id.IsNotEmpty())
                {
                    var columnInfo = new List<DataGridColumnInfo>(dataGrid.Columns.Select((x) => new DataGridColumnInfo(x))).ToArray();
                    var settings = GetDataGridSettings(dataGrid);
                    if (settings.DataGridSettings == null)
                        settings.DataGridSettings = new DataGridSettings();
                    settings.DataGridSettings.ColumnSettings[id] = columnInfo;
                    settings.Save();
                }
            }
        }
        public static void DeleteColumnInfo(DataGrid dataGrid)
        {
            if (GetIsStateful(dataGrid))
            {
                string id = DataGridHelper.GetStateId(dataGrid);
                if (id.IsNotEmpty())
                {
                    var settings = DataGridHelper.GetDataGridSettings(dataGrid);
                    if (settings.DataGridSettings == null)
                        settings.DataGridSettings = new DataGridSettings();
                    settings.DataGridSettings.ColumnSettings.Remove(id);
                    settings.Save();
                }
            }
        }

        static string GetId(DataGrid dataGrid)
        {
            string id = GetStateId(dataGrid);
            if (id.IsNullOrEmpty())
                id += BaseUriHelper.GetBaseUri(dataGrid).AbsoluteUri + "_DataGrid_" + dataGrid.Name;
            return id;
        }

        static MenuItem GetMenu(DataGridColumn column, DataGrid dataGrid)
        {
            if (!GetCanHide(column))
                return null;
            object header = null;
            if (column.Header is FrameworkElement)
            {
                string xaml = System.Windows.Markup.XamlWriter.Save(column.Header);
                var el = System.Windows.Markup.XamlReader.Parse(xaml) as FrameworkElement;
                el.Visibility = Visibility.Visible;
                header = el;
            }
            else
                header = column.Header;
            if (header == null)
                return null;

            var item = new MenuItem
            {
                Header = header,
                IsCheckable = true,
                IsChecked = column.Visibility == System.Windows.Visibility.Visible
            };

            item.StaysOpenOnClick = true;
            item.Tag = column;
            item.Checked += (s, t) =>
            {
                var col = (s as MenuItem).Tag as DataGridColumn;
                col.Visibility = System.Windows.Visibility.Visible;
                UpdateColumnInfo(dataGrid);
            };
            item.Unchecked += (s, t) =>
            {
                var col = (s as MenuItem).Tag as DataGridColumn;
                col.Visibility = System.Windows.Visibility.Collapsed;
                var element = col.Header as FrameworkElement;
                if (element != null)//从逻辑树中移除，bug for XP
                    (element.Parent as DataGridColumnHeader).Content = null;
                UpdateColumnInfo(dataGrid);
            };
            return item;
        }

        static void columnHeaderContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            var dataGrid = menu.Tag as DataGrid;

            menu.Items.Clear();
            foreach (var column in dataGrid.Columns)
            {
                var item = GetMenu(column, dataGrid);
                if (item != null)
                    menu.Items.Add(item);
            }

            menu.Items.Add(new Separator());

            var reset = new MenuItem();
            reset.Header = Resources.ResetDataGridColumn;
            reset.Click += (s, t) => OnResetColumns(dataGrid);
            menu.Items.Add(reset);
        }

        static void OnResetColumns(DataGrid dataGrid)
        {
            var result = MessageBox.Show(Resources.ResetDataGridColumnConfirmation,
                Resources.Confirmation, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                if (GetIsStateful(dataGrid))
                {
                    string id = GetId(dataGrid);
                    if (id.IsNotEmpty())
                    {
                        var settings = GetDataGridSettings(dataGrid);
                        if (settings.DataGridSettings == null)
                            settings.DataGridSettings = new DataGridSettings();
                        settings.DataGridSettings.ColumnSettings.Remove(id);
                        settings.Save();
                    }
                }
                int index = 0;
                foreach (var c in dataGrid.Columns)
                {
                    c.Visibility = Visibility.Visible;
                    c.DisplayIndex = index++;
                }
            }
        }

        static void dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg != null)
            {
                if ((bool)dg.GetValue(ShowRowNumProperty))
                    e.Row.Header = e.Row.GetIndex() + 1;
                var menu = GetRowContextMenu(dg);
                if (menu != null)
                {
                    e.Row.ContextMenu = menu;
                    e.Row.ContextMenuOpening += (x, y) =>
                    {
                        var row = x as DataGridRow;
                        foreach (var i in row.ContextMenu.Items)
                        {
                            var item = i as MenuItem;
                            if (item != null)
                                item.CommandParameter = row.Item;
                        }
                    };
                }
            }
        }

        static void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var dg = sender as DataGrid;
                if (dg != null)
                {
                    DependencyObject source = e.OriginalSource as DependencyObject;

                    var row = Util.TryFindParent<DataGridRow>(source);

                    if (row != null)
                    {
                        var cmd = (ICommand)dg.GetValue(RowDoubleClickCommandProperty);
                        if (cmd != null && cmd.CanExecute(row.DataContext))
                            cmd.Execute(row.DataContext);
                    }
                }
            }
        }
    }
}
