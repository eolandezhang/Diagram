using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace QPP.Wpf.UI.Controls
{
    [DefaultProperty("Columns")]
    [ContentProperty("Columns")]
    public class DataGridMultiComboBoxColumn : DataGridComboBoxColumn
    {
        //Columns of DataGrid
        private ObservableCollection<DataGridTextColumn> columns;
        //Grid Combobox  cell edit
        private QPP.Wpf.UI.Controls.MultiComboBox.MultiComboBox comboBox;

        public DataGridMultiComboBoxColumn()
        {
            comboBox = new MultiComboBox.MultiComboBox();
        }

        //The property is default and Content property for GridComboBox
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<DataGridTextColumn> Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new ObservableCollection<DataGridTextColumn>();
                }
                return this.columns;
            }
        }
        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {               

            if (comboBox.Columns.Count == 0)
            {
                //Add columns to DataGrid columns
                for (int i = 0; i < columns.Count; i++)
                    comboBox.Columns.Add(columns[i]);
            }

            return comboBox;
        }

        private void ReadColumn(MultiComboBox.MultiComboBox comboBox)
        {
            if (comboBox.Columns.Count == 0)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    comboBox.Columns.Add(columns[i]);
                }
            }
        }

        private void ApplyColumnProperties(QPP.Wpf.UI.Controls.MultiComboBox.MultiComboBox comboBox)
        {
            ApplyBinding(this.SelectedItemBinding, comboBox, Selector.SelectedItemProperty);
            ApplyBinding(this.SelectedValueBinding, comboBox, Selector.SelectedValueProperty);
        }

        private static void ApplyBinding(BindingBase binding, DependencyObject target, DependencyProperty property)
        {
            if (binding != null)
            {
                BindingOperations.SetBinding(target, property, binding);
            }
            else
            {
                BindingOperations.ClearBinding(target, property);
            }
        }

        //获得选择的值
        private object GetMultiComboBoxSelectionValue(QPP.Wpf.UI.Controls.MultiComboBox.MultiComboBox comboBox)
        {
            if (this.SelectedItemBinding != null)
            {
                return comboBox.SelectedItem;
            }
            if (this.SelectedValueBinding != null)
            {
                return comboBox.SelectedValue;
            }
            return comboBox.SelectedValue;
        }

        //获得鼠标和键盘事件
        private static bool IsComboBoxOpeningInputEvent(RoutedEventArgs e)
        {
            KeyEventArgs args = e as KeyEventArgs;
            if ((args == null) || (((byte)(args.KeyStates & KeyStates.Down)) != 1))
            {
                return false;
            }
            bool flag = (args.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
            Key systemKey = args.Key;
            if (systemKey == Key.System)
            {
                systemKey = args.SystemKey;
            }
            if ((systemKey == Key.F4) && !flag)
            {
                return true;
            }
            if ((systemKey != Key.Up) && (systemKey != Key.Down))
            {
                return false;
            }
            return flag;
        }

        //資料行針對處於編輯模式之儲存格所顯示的項目
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            comboBox.Focus();
            object selectionValue = this.GetMultiComboBoxSelectionValue(comboBox);
            if (IsComboBoxOpeningInputEvent(editingEventArgs))
            {
                comboBox.IsDropDownOpen = true;
            }
            this.ApplyColumnProperties(comboBox);  
            comboBox.ItemsSource = ItemsSource;
            comboBox.SelectedValuePath = SelectedValuePath;
            comboBox.DisplayMemberPath = DisplayMemberPath;
            return selectionValue;
        }
    }
}
